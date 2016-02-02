namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.IO;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.AspNet.Mvc.ViewFeatures;
    using Microsoft.AspNet.Routing;

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
            Values = new RouteValueDictionary();
            ViewContext = new ViewContext();
        }

        /// <summary>
        /// Initialises a new instance of <see cref="WidgetContext"/>.
        /// </summary>
        /// <param name="widgetDescriptor">The widget descriptor.</param>
        /// <param name="values">The set of provided invocation values.</param>
        /// <param name="viewContext">The view context.</param>
        /// <param name="writer">The text writer.</param>
        public WidgetContext(WidgetDescriptor widgetDescriptor, RouteValueDictionary values, ViewContext viewContext, TextWriter writer)
        {
            if (widgetDescriptor == null)
            {
                throw new ArgumentNullException(nameof(widgetDescriptor));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
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
            Values = values;

            ViewContext = new ViewContext(
                viewContext,
                viewContext.View,
                new ViewDataDictionary(viewContext.ViewData),
                writer);
        }

        /// <summary>
        /// Gets the model state dictionary.
        /// </summary>
        public ModelStateDictionary ModelState => ViewContext?.ModelState;

        /// <summary>
        /// Gets or sets the set of invocation-provided values.
        /// </summary>
        public RouteValueDictionary Values { get; set; }

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
