namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.DependencyInjection;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Renders a widget view.
    /// </summary>
    public class ViewWidgetResult : IWidgetResult
    {
        private const string ViewPathFormat = "Widgets/{0}/{1}";
        private const string DefaultViewName = "Default";

        /// <summary>
        /// Gets or sets the temp data dictionary.
        /// </summary>
        public ITempDataDictionary TempData { get; set; }

        /// <summary>
        /// Gets or sets the view data dictionary.
        /// </summary>
        public ViewDataDictionary ViewData { get; set; }

        /// <summary>
        /// Gets or sets the view engine.
        /// </summary>
        public IViewEngine ViewEngine { get; set; }

        /// <summary>
        /// Gets or sets the view name.
        /// </summary>
        public string ViewName { get; set; }

        /// <inheritdoc />
        public void Execute(WidgetContext context)
        {
            var task = ExecuteAsync(context);

            task.GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(WidgetContext context)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            var viewEngine = ViewEngine ?? ResolveViewEngine(context);
            var viewContext = context.ViewContext;
            var viewData = ViewData ?? context.ViewData;
            bool isNullOrEmptyViewName = string.IsNullOrEmpty(ViewName);

            ViewEngineResult result = null;
            IEnumerable<string> originalLocations = null;
            if (!isNullOrEmptyViewName)
            {
                result = viewEngine.GetView(viewContext.ExecutingFilePath, ViewName, isMainPage: false);
                originalLocations = result.SearchedLocations;
            }

            if (result == null || !result.Success)
            {
                var viewName = isNullOrEmptyViewName ? DefaultViewName : ViewName;
                var qualifiedViewName = string.Format(ViewPathFormat, context.WidgetDescriptor.ShortName, viewName);

                result = viewEngine.FindView(viewContext, qualifiedViewName, isMainPage: false);
            }

            var view = result.EnsureSuccessful(originalLocations).View;
            using (view as IDisposable)
            {
                var childViewContext = new ViewContext(
                    viewContext,
                    view,
                    ViewData ?? context.ViewData,

                    context.Writer);
                await view.RenderAsync(childViewContext);
            }
        }

        /// <summary>
        /// Resolves the view engine.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <returns>The view engine.</returns>
        private static IViewEngine ResolveViewEngine(WidgetContext context)
        {
            return context.ViewContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
        }
    }
}