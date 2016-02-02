namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System.Threading.Tasks;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Defines the required contract for implementing a widget result.
    /// </summary>
    public interface IWidgetResult
    {
        /// <summary>
        /// Executes the result synchronously.
        /// </summary>
        /// <param name="context">The widget context.</param>
        void Execute(WidgetContext context);
        
        /// <summary>
        /// Executes the result asynchronously.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <returns>The task instance.</returns>
        Task ExecuteAsync(WidgetContext context);
    }
}
