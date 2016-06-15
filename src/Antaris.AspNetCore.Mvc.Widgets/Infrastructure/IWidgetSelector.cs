namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Reflection;

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

        /// <summary>
        /// Selects a widget to be executed based on the widget type.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <returns>The widget descriptor.</returns>
        WidgetDescriptor SelectWidget(TypeInfo widgetType);
    }
}
