namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Html;

    /// <summary>
    /// Defines the required contract for implementing a widget helper.
    /// </summary>
    public interface IWidgetHelper
    {
        /// <summary>
        /// Invokes a widget with the specified name asynchronously.
        /// </summary>
        /// <param name="name">The widget name.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        /// <param name="elementId">[Optional] The element id.</param>
        /// <returns>A <see cref="HtmlString"/> of the rendered widget content.</returns>
        Task<IHtmlContent> InvokeAsync(string name, object values = null, string elementId = null);

        /// <summary>
        /// Invokes a widget of the specified type asynchronously.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        /// <param name="elementId">[Optional] The element id.</param>
        /// <returns>A <see cref="HtmlString"/> of the rendered widget content.</returns>
        Task<IHtmlContent> InvokeAsync(TypeInfo widgetType, object values = null, string elementId = null);
    }
}
