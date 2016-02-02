namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using Microsoft.Extensions.Internal;

    /// <summary>
    /// Provides a default implementation of a widget activator.
    /// </summary>
    public class DefaultWidgetActivator : IWidgetActivator
    {
        private readonly Func<Type, PropertyActivator<WidgetContext>[]> _getPropertiesToActivate;
        private readonly ConcurrentDictionary<Type, PropertyActivator<WidgetContext>[]> _injectActions;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetActivator"/>
        /// </summary>
        public DefaultWidgetActivator()
        {
            _injectActions = new ConcurrentDictionary<Type, PropertyActivator<WidgetContext>[]>();
            _getPropertiesToActivate = type => PropertyActivator<WidgetContext>.GetPropertiesToActivate(type, typeof(WidgetContextAttribute), CreateActivateInfo);
        }

        /// <inheritdoc />
        public virtual void Activate(object widget, WidgetContext context)
        {
            var propertiesToActivate = _injectActions.GetOrAdd(widget.GetType(), _getPropertiesToActivate);

            for (int i = 0; i < propertiesToActivate.Length; i++)
            {
                var activateInfo = propertiesToActivate[i];
                activateInfo.Activate(widget, context);
            }
        }

        /// <summary>
        /// Creates a new property activator.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <returns>The property activator.</returns>
        private PropertyActivator<WidgetContext> CreateActivateInfo(PropertyInfo property)
        {
            return new PropertyActivator<WidgetContext>(property, ctx => ctx);
        }
    }
}
