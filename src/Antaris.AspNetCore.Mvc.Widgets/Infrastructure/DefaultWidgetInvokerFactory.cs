namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    /// <summary>
    /// Provides a default implementation of a widget invoker factory.
    /// </summary>
    public class DefaultWidgetInvokerFactory : IWidgetInvokerFactory
    {
        private readonly IWidgetFactory _widgetFactory;
        private readonly IWidgetArgumentBinder _argumentBinder;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetInvokerFactory"/>.
        /// </summary>
        /// <param name="widgetFactory">The type activator cache.</param>
        /// <param name="widgetActivator">The widget activator.</param>
        /// <param name="argumentBinder">The widget argument binder.</param>
        public DefaultWidgetInvokerFactory(IWidgetFactory widgetFactory, IWidgetArgumentBinder argumentBinder)
        {
            _widgetFactory = Ensure.ArgumentNotNull(widgetFactory, nameof(widgetFactory));
            _argumentBinder = Ensure.ArgumentNotNull(argumentBinder, nameof(argumentBinder));
        }

        /// <inheritdoc />
        public IWidgetInvoker CreateInstance(WidgetContext context)
        {
            return new DefaultWidgetInvoker(_widgetFactory, _argumentBinder);
        }
    }
}
