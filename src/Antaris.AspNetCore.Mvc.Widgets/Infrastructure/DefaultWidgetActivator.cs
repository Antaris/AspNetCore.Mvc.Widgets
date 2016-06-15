namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using Microsoft.AspNetCore.Mvc.Internal;

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
            _typeActivatorCache = Ensure.ArgumentNotNull(typeActivatorCache, nameof(typeActivatorCache));
        }

        /// <inheritdoc />
        public object Create(WidgetContext context)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            var widgetType = context.WidgetDescriptor.TypeInfo;

            var widget = _typeActivatorCache.CreateInstance<object>(
                context.ViewContext.HttpContext.RequestServices,
                context.WidgetDescriptor.TypeInfo.AsType());

            return widget;
        }

        /// <inheritdoc />
        public void Release(WidgetContext context, object widget)
        {
            Ensure.ArgumentNotNull(context, nameof(context));
            Ensure.ArgumentNotNull(widget, nameof(widget));

            var disposable = widget as IDisposable;
            disposable?.Dispose();
        }
    }
}
