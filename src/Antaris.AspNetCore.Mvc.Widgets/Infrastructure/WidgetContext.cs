namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.AspNet.Mvc.ViewFeatures;
    using Microsoft.AspNet.Routing;
    using Microsoft.Extensions.WebEncoders;

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
        /// <param name="arguments">The widget arguments.</param>
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
            if (widgetDescriptor == null)
            {
                throw new ArgumentNullException(nameof(widgetDescriptor));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            if (htmlEncoder == null)
            {
                throw new ArgumentNullException(nameof(htmlEncoder));
            }

            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            WidgetDescriptor = widgetDescriptor;
            Arguments = arguments;
            HtmlEncoder = htmlEncoder;

            ViewContext = new ViewContext(
                viewContext,
                viewContext.View,
                new ViewDataDictionary(viewContext.ViewData),
                writer);
        }

        /// <summary>
        /// Gets or sets the set of invocation-provided values.
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
        /// Gets the text writer.
        /// </summary>
        public TextWriter Writer => ViewContext?.Writer;
    }
}
