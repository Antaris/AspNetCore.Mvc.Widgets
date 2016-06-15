namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Encodings.Web;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    /// <summary>
    /// Represents a context for tracking Widget execution.
    /// </summary>
    public class WidgetContext
    {
        /// <summary>
        /// Initialises a new instance of <see cref="WidgetContext"/>.
        /// </summary>
        public WidgetContext()
        {
            WidgetDescriptor = new WidgetDescriptor();
            ViewContext = new ViewContext();
        }

        /// <summary>
        /// Initialises a new instance of <see cref="WidgetContext"/>.
        /// </summary>
        /// <param name="widgetDescriptor">The widget descriptor.</param>
        /// <param name="arguments">The set of provided invocation arguments.</param>
        /// <param name="htmlEncoder">The HTML encoder.</param>
        /// <param name="viewContext">The view context.</param>
        /// <param name="writer">The text writer.</param>
        public WidgetContext(
            WidgetDescriptor widgetDescriptor,
            IDictionary<string, object> arguments,
            HtmlEncoder htmlEncoder,
            ViewContext viewContext,
            TextWriter writer)
        {
            WidgetDescriptor = Ensure.ArgumentNotNull(widgetDescriptor, nameof(widgetDescriptor));
            Arguments = Ensure.ArgumentNotNull(arguments, nameof(arguments));
            HtmlEncoder = Ensure.ArgumentNotNull(htmlEncoder, nameof(htmlEncoder));

            ViewContext = new ViewContext(
                Ensure.ArgumentNotNull(viewContext, nameof(viewContext)),
                viewContext.View,
                new ViewDataDictionary(viewContext.ViewData),
                Ensure.ArgumentNotNull(writer, nameof(writer)));
        }

        /// <summary>
        /// Gets or sets the set of invocation-provided arguments.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// Gets or sets the HTML encoder.
        /// </summary>
        public HtmlEncoder HtmlEncoder { get; set; }

        /// <summary>
        /// Gets the model state dictionary.
        /// </summary>
        public ModelStateDictionary ModelState => ViewContext?.ModelState;

        /// <summary>
        /// Gets or sets the view context.
        /// </summary>
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// Gets the view data dictionary.
        /// </summary>
        public ViewDataDictionary ViewData => ViewContext?.ViewData;

        /// <summary>
        /// Gets or sets the widget descriptor.
        /// </summary>
        public WidgetDescriptor WidgetDescriptor { get; set; }

        /// <summary>
        /// Gets or sets the widget id.
        /// </summary>
        public string WidgetId { get; set; }

        /// <summary>
        /// Gets or sets the widget state.
        /// </summary>
        public string WidgetState { get; set; }

        /// <summary>
        /// Gets the text writer.
        /// </summary>
        public TextWriter Writer => ViewContext?.Writer;
    }
}
