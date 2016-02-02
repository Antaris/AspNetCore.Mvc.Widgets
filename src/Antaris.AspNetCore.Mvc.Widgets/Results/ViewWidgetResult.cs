namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.ViewEngines;
    using Microsoft.AspNet.Mvc.ViewFeatures;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.Extensions.DependencyInjection;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Renders a widget view.
    /// </summary>
    public class ViewWidgetResult : IWidgetResult
    {
        private const string ViewPath = "Widgets/{0}/{1}";
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
            var viewEngine = ViewEngine ?? ResolveViewEngine(context);
            var viewData = ViewData ?? context.ViewData;
            bool isNullOrEmptyViewName = string.IsNullOrEmpty(ViewName);

            string state = null; // TODO: Resolve from value provider?

            string qualifiedViewName;
            if (!isNullOrEmptyViewName && (ViewName[0] == '~' || ViewName[0] == '/'))
            {
                qualifiedViewName = ViewName;
            }
            else
            {
                qualifiedViewName = string.Format(ViewPath, context.WidgetDescriptor.ShortName, isNullOrEmptyViewName ? (state ?? DefaultViewName) : ViewName);
            }

            var view = FindView(context.ViewContext, viewEngine, qualifiedViewName);
            var childViewContext = new ViewContext(
                context.ViewContext,
                view,
                viewData,
                context.Writer);

            using (view as IDisposable)
            {
                await view.RenderAsync(childViewContext);
            }
        }

        /// <summary>
        /// Finds a view for the given name.
        /// </summary>
        /// <param name="context">The action context.</param>
        /// <param name="viewEngine">The view engine.</param>
        /// <param name="viewName">The view name.</param>
        /// <returns>The resolved view instance.</returns>
        private static IView FindView(ActionContext context, IViewEngine viewEngine, string viewName)
        {
            return viewEngine.FindPartialView(context, viewName).EnsureSuccessful().View;
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