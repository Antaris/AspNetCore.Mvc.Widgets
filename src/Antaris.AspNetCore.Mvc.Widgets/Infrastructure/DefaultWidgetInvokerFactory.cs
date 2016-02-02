namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using Microsoft.AspNet.Mvc.Infrastructure;

    /// <summary>
    /// Provides a default implementation of a widget invoker factory.
    /// </summary>
    public class DefaultWidgetInvokerFactory : IWidgetInvokerFactory
    {
        private readonly ITypeActivatorCache _typeActivatorCache;
        private readonly IWidgetActivator _widgetActivator;
        private readonly IWidgetArgumentBinder _argumentBinder;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetInvokerFactory"/>.
        /// </summary>
        /// <param name="typeActivatorCache">The type activator cache.</param>
        /// <param name="widgetActivator">The widget activator.</param>
        /// <param name="argumentBinder">The widget argument binder.</param>
        public DefaultWidgetInvokerFactory(ITypeActivatorCache typeActivatorCache, IWidgetActivator widgetActivator, IWidgetArgumentBinder argumentBinder)
        {
            if (typeActivatorCache == null)
            {
                throw new ArgumentNullException(nameof(typeActivatorCache));
            }

            if (widgetActivator == null)
            {
                throw new ArgumentNullException(nameof(widgetActivator));
            }

            if (argumentBinder == null)
            {
                throw new ArgumentNullException(nameof(argumentBinder));
            }

            _typeActivatorCache = typeActivatorCache;
            _widgetActivator = widgetActivator;
            _argumentBinder = argumentBinder;
        }

        /// <inheritdoc />
        public IWidgetInvoker CreateInstance(WidgetContext context)
        {
            return new DefaultWidgetInvoker(_typeActivatorCache, _widgetActivator, _argumentBinder);
        }
    }
}
