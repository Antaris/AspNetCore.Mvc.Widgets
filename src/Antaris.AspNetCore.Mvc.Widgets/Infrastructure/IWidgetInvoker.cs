namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the required contract for implementing a widget invoker.
    /// </summary>
    public interface IWidgetInvoker
    {
        /// <summary>
        /// Invokes a widget asynchronously.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <returns>A task instance.</returns>
        Task InvokeAsync(WidgetContext context);
    }
}
