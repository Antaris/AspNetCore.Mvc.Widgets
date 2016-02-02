namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    /// <summary>
    /// Defines the required contract for implementing a widget activator.
    /// </summary>
    public interface IWidgetActivator
    {
        /// <summary>
        /// Activates the given widget object.
        /// </summary>
        /// <param name="widget">The widget instance.</param>
        /// <param name="context">The widget context.</param>
        void Activate(object widget, WidgetContext context);
    }
}
