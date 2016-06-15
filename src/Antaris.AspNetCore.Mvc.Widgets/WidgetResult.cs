namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.DependencyInjection;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Represents a widget that can be executed from a controller.
    /// </summary>
    public class WidgetResult : ActionResult
    {
        /// <summary>
        /// Gets or sets the arguments provided to the view component.
        /// </summary>
        public object Arguments { get; set; }

        /// <summary>
        /// Gets or sets the Content-Type header for the response.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITempDataDictionary"/> for this result.
        /// </summary>
        public ITempDataDictionary TempData { get; set; }

        /// <summary>
        /// Gets or sets the name of the view component to invoke. Will be ignored if <see cref="ViewComponentType"/>
        /// is set to a non-<c>null</c> value.
        /// </summary>
        public string WidgetName { get; set; }

        /// <summary>
        /// Gets or sets the type of the view component to invoke.
        /// </summary>
        public Type WidgetType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ViewDataDictionary"/> for this result.
        /// </summary>
        public ViewDataDictionary ViewData { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IViewEngine"/> used to locate views.
        /// </summary>
        /// <remarks>When <c>null</c>, an instance of <see cref="ICompositeViewEngine"/> from
        /// <c>ActionContext.HttpContext.RequestServices</c> is used.</remarks>
        public IViewEngine ViewEngine { get; set; }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<WidgetResultExecutor>();
            return executor.ExecuteAsync(context, this);
        }
    }
}
