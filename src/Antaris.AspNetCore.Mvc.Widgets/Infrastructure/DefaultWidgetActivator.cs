namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using Microsoft.AspNet.Mvc.Infrastructure;
    /// <summary>
    /// Provides a default implementation of a widget activator.
    /// </summary>
    public class DefaultWidgetActivator : IWidgetActivator
    {
        private readonly ITypeActivatorCache _typeActivatorCache;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetActivator"/>
        /// </summary>
        /// <param name="typeActivatorCache">The type activator cache.</param>
        public DefaultWidgetActivator(ITypeActivatorCache typeActivatorCache)
        {
            if (typeActivatorCache == null)
            {
                throw new ArgumentNullException(nameof(typeActivatorCache));
            }

            _typeActivatorCache = typeActivatorCache;
        }

        /// <inheritdocs />
        public virtual object Create(WidgetContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var widgetType = context.WidgetDescriptor.TypeInfo;

            if (widgetType.IsValueType
                || widgetType.IsInterface
                || widgetType.IsAbstract ||
                (widgetType.IsGenericType && widgetType.IsGenericTypeDefinition))
            {
                throw new InvalidOperationException($"The type '{widgetType.FullName}' cannot be activated.");
            }

            var widget = _typeActivatorCache.CreateInstance<object>(
                context.ViewContext.HttpContext.RequestServices,
                context.WidgetDescriptor.TypeInfo.AsType());

            return widget;
        }

        /// <inheritdocs />
        public virtual void Release(WidgetContext context, object widget)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (widget == null)
            {
                throw new ArgumentNullException(nameof(widget));
            }

            var disposable = widget as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
