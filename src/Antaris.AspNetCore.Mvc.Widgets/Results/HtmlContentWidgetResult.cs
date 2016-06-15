namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Internal;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Renders content directly from a widget.
    /// </summary>
    public class HtmlContentWidgetResult : IWidgetResult
    {
        /// <summary>
        /// Initialises a new instance of <see cref="HtmlContentWidgetResult"/>.
        /// </summary>
        /// <param name="encodedContent">The encoded content.</param>
        public HtmlContentWidgetResult(IHtmlContent encodedContent)
        {
            EncodedContent = Ensure.ArgumentNotNull(encodedContent, nameof(encodedContent));
        }

        /// <summary>
        /// Gets the raw content.
        /// </summary>
        public IHtmlContent EncodedContent { get; }

        /// <inheritdoc />
        public void Execute(WidgetContext context)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            context.Writer.Write(EncodedContent);
        }

        /// <inheritdoc />
        public Task ExecuteAsync(WidgetContext context)
        {
            Execute(context);

            return TaskCache.CompletedTask;
        }
    }
}
