namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc.Rendering;

    /// <summary>
    /// Defines the required contract for implementing a widget helper.
    /// </summary>
    public interface IWidgetHelper
    {
        /// <summary>
        /// Invokes a widget with the specified name.
        /// </summary>
        /// <param name="name">The widget name.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        /// <returns>A <see cref="HtmlString"/> of the rendered widget content.</returns>
        HtmlString Invoke(string name, object values = null);

        /// <summary>
        /// Invokes a widget of the specified type.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        /// <returns>A <see cref="HtmlString"/> of the rendered widget content.</returns>
        HtmlString Invoke(Type widgetType, object values = null);

        /// <summary>
        /// Invokes a widget with the specified name asynchronously.
        /// </summary>
        /// <param name="name">The widget name.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        /// <returns>A <see cref="HtmlString"/> of the rendered widget content.</returns>
        Task<HtmlString> InvokeAsync(string name, object values = null);

        /// <summary>
        /// Invokes a widget of the specified type asynchronously.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        /// <returns>A <see cref="HtmlString"/> of the rendered widget content.</returns>
        Task<HtmlString> InvokeAsync(Type widgetType, object values = null);

        /// <summary>
        /// Invokes a widget with the specified name, rendering to the active text writer.
        /// </summary>
        /// <param name="name">The widget name.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        void RenderInvoke(string name, object values = null);

        /// <summary>
        /// Invokes a widget of the specified type, rendering to the active text writer.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        void RenderInvoke(Type widgetType, object values = null);

        /// <summary>
        /// Invokes a widget with the specified name asynchronously, rendering to the active text writer.
        /// </summary>
        /// <param name="name">The widget name.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        /// <returns>As task instance.</returns>
        Task RenderInvokeAsync(string name, object values = null);

        /// <summary>
        /// Invokes a widget of the specified type asynchronously, rendering to the active text writer.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <param name="values">[Optional] The set of values to provide to the widget.</param>
        /// <returns>As task instance.</returns>
        Task RenderInvokeAsync(Type widgetType, object values = null);
    }
}
