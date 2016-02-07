namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using Microsoft.Extensions.Internal;

    /// <summary>
    /// Provides a default implementation of a <see cref="IWidgetFactory"/>.
    /// </summary>
    public class DefaultWidgetFactory : IWidgetFactory
    {
        private readonly IWidgetActivator _activator;
        private readonly Func<Type, PropertyActivator<WidgetContext>[]> _getPropertiesToActivate;
        private readonly ConcurrentDictionary<Type, PropertyActivator<WidgetContext>[]> _injectActions;

        /// <summary>
        /// Initialises a new instance of <see cref="IWidgetActivator"/>.
        /// </summary>
        /// <param name="activator">The widget activator.</param>
        public DefaultWidgetFactory(IWidgetActivator activator)
        {
            if (activator == null)
            {
                throw new ArgumentNullException(nameof(activator));
            }

            _activator = activator;

            _getPropertiesToActivate = type => PropertyActivator<WidgetContext>.GetPropertiesToActivate(
                type,
                typeof(WidgetContextAttribute),
                CreateActivateInfo);

            _injectActions = new ConcurrentDictionary<Type, PropertyActivator<WidgetContext>[]>();
        }

        /// <inheritdoc />
        public object CreateWidget(WidgetContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var widget = _activator.Create(context);

            InjectProperties(context, widget);

            return widget;
        }

        /// <inheritdoc />
        public void ReleaseWidget(WidgetContext context, object widget)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (widget == null)
            {
                throw new ArgumentNullException(nameof(widget));
            }

            _activator.Release(context, widget);
        }

        /// <summary>
        /// Injects the target properties of the widget.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <param name="widget">The widget instance.</param>
        private void InjectProperties(WidgetContext context, object widget)
        {
            var propertiesToActivate = _injectActions.GetOrAdd(
                widget.GetType(),
                _getPropertiesToActivate);

            for (int i = 0; i < propertiesToActivate.Length; i++)
            {
                var activateInfo = propertiesToActivate[i];
                activateInfo.Activate(widget, context);
            }
        }

        /// <summary>
        /// Creates a property activator for the target property.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <returns>The property activator.</returns>
        private static PropertyActivator<WidgetContext> CreateActivateInfo(PropertyInfo property)
        {
            return new PropertyActivator<WidgetContext>(property, context => context);
        }
    }
}
