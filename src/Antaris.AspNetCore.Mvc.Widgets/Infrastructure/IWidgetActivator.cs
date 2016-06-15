namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    /// <summary>
    /// Defines the required contract for implementing a widget activator.
    /// </summary>
    public interface IWidgetActivator
    {
        /// <summary>
        /// Initialises a widget.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <returns>The widget instance.</returns>
        object Create(WidgetContext context);

        /// <summary>
        /// Releases a widget instance.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <param name="widget">The widget instance.</param>
        void Release(WidgetContext context, object widget);
    }
}
