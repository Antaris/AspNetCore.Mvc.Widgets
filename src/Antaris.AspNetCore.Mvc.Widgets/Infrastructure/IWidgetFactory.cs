namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    /// <summary>
    /// Defines the required contract for implementing a widget factory.
    /// </summary>
    public interface IWidgetFactory
    {
        /// <summary>
        /// Initialises a widget.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <returns>The widget instance.</returns>
        object CreateWidget(WidgetContext context);

        /// <summary>
        /// Releases a widget instance.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <param name="widget">The widget instance.</param>
        void ReleaseWidget(WidgetContext context, object widget);
    }
}
