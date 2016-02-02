namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    /// <summary>
    /// Defines the required contract for implementing a widget invoker factory.
    /// </summary>
    public interface IWidgetInvokerFactory
    {
        /// <summary>
        /// Creates an instance of a widget invoker for the given widget context.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <returns>The widget invoker instance.</returns>
        IWidgetInvoker CreateInstance(WidgetContext context);
    }
}
