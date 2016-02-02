namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    /// <summary>
    /// Defines the required contract for implementing a widget selector.
    /// </summary>
    public interface IWidgetSelector
    {
        /// <summary>
        /// Selects a widget to be executed based on the given name.
        /// </summary>
        /// <param name="widgetName">The widget name.</param>
        /// <returns>The widget descriptor.</returns>
        WidgetDescriptor SelectWidget(string widgetName);
    }
}
