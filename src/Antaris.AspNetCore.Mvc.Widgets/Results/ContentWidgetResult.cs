namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc.Internal;
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
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            Content = content;
        }

        /// <summary>
        /// Gets the raw content.
        /// </summary>
        public string Content { get; }

        /// <inheritdoc />
        public void Execute(WidgetContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.HtmlEncoder.Encode(context.Writer, Content);
        }

        /// <inheritdoc />
        public Task ExecuteAsync(WidgetContext context)
        {
            Execute(context);

            return TaskCache.CompletedTask;
        }
    }
}
