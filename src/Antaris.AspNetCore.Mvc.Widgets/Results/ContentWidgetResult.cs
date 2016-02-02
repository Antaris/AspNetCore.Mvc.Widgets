namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc.Rendering;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Renders content directly from a widget.
    /// </summary>
    public class ContentWidgetResult : IWidgetResult
    {
        /// <summary>
        /// Initialises a new instance of <see cref="ContentWidgetResult"/>.
        /// </summary>
        /// <param name="content">The raw content.</param>
        public ContentWidgetResult(string content)
        {
            Content = content ?? string.Empty;
            EncodedContent = new HtmlString(WebUtility.HtmlEncode(Content));
        }

        /// <summary>
        /// Initialises a new instance of <see cref="ContentWidgetResult"/>.
        /// </summary>
        /// <param name="encodedContent"></param>
        public ContentWidgetResult(HtmlString encodedContent)
        {
            if (encodedContent == null)
            {
                throw new ArgumentNullException(nameof(encodedContent));
            }

            EncodedContent = encodedContent;
            Content = WebUtility.HtmlDecode(encodedContent.ToString());
        }

        /// <summary>
        /// Gets the raw content.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the encoded content.
        /// </summary>
        public HtmlString EncodedContent { get; }

        /// <inheritdoc />
        public void Execute(WidgetContext context)
        {
            context.Writer.Write(EncodedContent.ToString());
        }

        /// <inheritdoc />
        public Task ExecuteAsync(WidgetContext context)
        {
            return context.Writer.WriteAsync(EncodedContent.ToString());
        }
    }
}
