namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Html.Abstractions;
    /// <summary>
    /// Defines the required contract for implementing a widget helper.
    /// </summary>
    public interface IWidgetHelper
    {
        /// <summary>
        /// Invokes a widget with the specified name asynchronously.
        /// </summary>
        /// <param name="name">The widget name.</param>
        /// <param name="arguments">[Optional] The set of named arguments to provide to the widget.</param>
        /// <returns>A <see cref="IHtmlContent"/> of the rendered widget content.</returns>
        Task<IHtmlContent> InvokeAsync(string name, object arguments = null);

        /// <summary>
        /// Invokes a widget with the specified name asynchronously.
        /// </summary>
        /// <param name="name">The widget name.</param>
        /// <param name="id">The widget instance id.</param>
        /// <param name="arguments">[Optional] The set of named arguments to provide to the widget.</param>
        /// <returns>A <see cref="IHtmlContent"/> of the rendered widget content.</returns>
        Task<IHtmlContent> InvokeAsync(string name, string id, object arguments = null);

        /// <summary>
        /// Invokes a widget of the specified type asynchronously.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <param name="arguments">[Optional] The set of named arguments to provide to the widget.</param>
        /// <returns>A <see cref="HtmlString"/> of the rendered widget content.</returns>
        Task<IHtmlContent> InvokeAsync(Type widgetType, object values = null);

        /// <summary>
        /// Invokes a widget of the specified type asynchronously.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <param name="id">The widget instance id.</param>
        /// <param name="arguments">[Optional] The set of named arguments to provide to the widget.</param>
        /// <returns>A <see cref="HtmlString"/> of the rendered widget content.</returns>
        Task<IHtmlContent> InvokeAsync(Type widgetType, string id, object values = null);
    }
}
