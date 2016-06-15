namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.Internal;
    using System.Collections.Concurrent;

    /// <summary>
    /// Provides a default implementation of a widget invoker.
    /// </summary>
    public class DefaultWidgetFactory : IWidgetFactory
    {
        private readonly IWidgetActivator _activator;
        private readonly Func<Type, PropertyActivator<WidgetContext>[]> _getPropertiesToActivate;
        private readonly ConcurrentDictionary<Type, PropertyActivator<WidgetContext>[]> _injectActions;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetFactory"/>
        /// </summary>
        /// <param name="activator">The widget activator.</param>
        public DefaultWidgetFactory(IWidgetActivator activator)
        {
            _activator = Ensure.ArgumentNotNull(activator, nameof(activator));

            _getPropertiesToActivate = type => PropertyActivator<WidgetContext>.GetPropertiesToActivate(
                type, typeof(WidgetContextAttribute), CreateActivateInfo);

            _injectActions = new ConcurrentDictionary<Type, PropertyActivator<WidgetContext>[]>();
        }

        /// <summary>
        /// Creates a property activator for the given property.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <returns>The property activator.</returns>
        private static PropertyActivator<WidgetContext> CreateActivateInfo(PropertyInfo property)
        {
            return new PropertyActivator<WidgetContext>(property, context => context);
        }

        /// <inheritdoc />
        public object CreateWidget(WidgetContext context)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            var widget = _activator.Create(context);
            InjectProperties(context, widget);

            return widget;
        }

        /// <summary>
        /// Injects the given widget context into the target properties of the widget.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <param name="widget">The widget instance.</param>
        private void InjectProperties(WidgetContext context, object widget)
        {
            var propertiesToActivate = _injectActions.GetOrAdd(widget.GetType(), _getPropertiesToActivate);

            for (int i = 0; i < propertiesToActivate.Length; i++)
            {
                var activateInfo = propertiesToActivate[i];
                activateInfo.Activate(widget, context);
            }
        }

        public void ReleaseWidget(WidgetContext context, object widget)
        {
            _activator.Release(context, widget);
        }
    }
}
