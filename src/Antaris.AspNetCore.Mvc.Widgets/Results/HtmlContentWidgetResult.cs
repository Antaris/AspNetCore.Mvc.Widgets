namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Html.Abstractions;
    using Microsoft.AspNet.Mvc.Internal;
    using Microsoft.AspNet.Mvc.ViewFeatures;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Renders HTML encoded content.
    /// </summary>
    public class HtmlContentWidgetResult : IWidgetResult
    {
        /// <summary>
        /// Initialises a new instance of <see cref="HtmlContentWidgetResult"/>.
        /// </summary>
        /// <param name="encodedContent">The HTML encoded content.</param>
        public HtmlContentWidgetResult(IHtmlContent encodedContent)
        {
            if (encodedContent == null)
            {
                throw new ArgumentNullException(nameof(encodedContent));
            }

            EncodedContent = encodedContent;
        }

        /// <summary>
        /// Gets the encoded content.
        /// </summary>
        public IHtmlContent EncodedContent { get; }

        /// <inheritdoc />
        public void Execute(WidgetContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var htmlWriter = context.Writer as HtmlTextWriter;
            if (htmlWriter == null)
            {
                EncodedContent.WriteTo(context.Writer, context.HtmlEncoder);
            }
            else
            {
                htmlWriter.Write(EncodedContent);
            }
        }

        /// <inheritdoc />
        public Task ExecuteAsync(WidgetContext context)
        {
            Execute(context);

            return TaskCache.CompletedTask;
        }
    }
}
