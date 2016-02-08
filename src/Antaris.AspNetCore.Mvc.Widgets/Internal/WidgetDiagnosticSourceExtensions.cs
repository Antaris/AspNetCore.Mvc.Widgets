namespace Antaris.AspNetCore.Mvc.Widgets.Internal
{
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.AspNet.Mvc.ViewEngines;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static class MvcViewFeaturesDiagnosticSourceExtensions
    {
        public static void BeforeWidget(
            this DiagnosticSource diagnosticSource,
            WidgetContext context,
            object viewComponent)
        {
            if (diagnosticSource.IsEnabled("Antaris.AspNetCore.Mvc.Widgets.BeforeWidget"))
            {
                diagnosticSource.Write(
                "Antaris.AspNetCore.Mvc.Widgets.BeforeWidget",
                new
                {
                    actionDescriptor = context.ViewContext.ActionDescriptor,
                    viewComponentContext = context,
                    viewComponent = viewComponent
                });
            }
        }

        public static void AfterWidget(
            this DiagnosticSource diagnosticSource,
            WidgetContext context,
            IWidgetResult result,
            object viewComponent)
        {
            if (diagnosticSource.IsEnabled("Antaris.AspNetCore.Mvc.Widgets.AfterWidget"))
            {
                diagnosticSource.Write(
                "Antaris.AspNetCore.Mvc.Widgets.AfterWidget",
                new
                {
                    actionDescriptor = context.ViewContext.ActionDescriptor,
                    viewComponentContext = context,
                    viewComponentResult = result,
                    viewComponent = viewComponent
                });
            }
        }

        public static void WidgetBeforeViewExecute(
            this DiagnosticSource diagnosticSource,
            WidgetContext context,
            IView view)
        {
            if (diagnosticSource.IsEnabled("Antaris.AspNetCore.Mvc.Widgets.WidgetBeforeViewExecute"))
            {
                diagnosticSource.Write(
                    "Antaris.AspNetCore.Mvc.Widgets.WidgetBeforeViewExecute",
                    new
                    {
                        actionDescriptor = context.ViewContext.ActionDescriptor,
                        viewComponentContext = context,
                        view = view
                    });
            }
        }

        public static void WidgetAfterViewExecute(
            this DiagnosticSource diagnosticSource,
            WidgetContext context,
            IView view)
        {
            if (diagnosticSource.IsEnabled("Antaris.AspNetCore.Mvc.Widgets.WidgetAfterViewExecute"))
            {
                diagnosticSource.Write(
                    "Antaris.AspNetCore.Mvc.Widgets.WidgetAfterViewExecute",
                    new
                    {
                        actionDescriptor = context.ViewContext.ActionDescriptor,
                        viewComponentContext = context,
                        view = view
                    });
            }
        }

        public static void BeforeView(
            this DiagnosticSource diagnosticSource,
            IView view,
            ViewContext viewContext)
        {
            if (diagnosticSource.IsEnabled("Antaris.AspNetCore.Mvc.Widgets.BeforeView"))
            {
                diagnosticSource.Write(
                    "Antaris.AspNetCore.Mvc.Widgets.BeforeView",
                    new { view = view, viewContext = viewContext, });
            }
        }

        public static void AfterView(
            this DiagnosticSource diagnosticSource,
            IView view,
            ViewContext viewContext)
        {
            if (diagnosticSource.IsEnabled("Antaris.AspNetCore.Mvc.Widgets.AfterView"))
            {
                diagnosticSource.Write(
                    "Antaris.AspNetCore.Mvc.Widgets.AfterView",
                    new { view = view, viewContext = viewContext, });
            }
        }

        public static void ViewFound(
            this DiagnosticSource diagnosticSource,
            ActionContext actionContext,
            bool isMainPage,
            PartialViewResult viewResult,
            string viewName,
            IView view)
        {
            if (diagnosticSource.IsEnabled("Antaris.AspNetCore.Mvc.Widgets.ViewFound"))
            {
                diagnosticSource.Write(
                    "Antaris.AspNetCore.Mvc.Widgets.ViewFound",
                    new
                    {
                        actionContext = actionContext,
                        isMainPage = isMainPage,
                        result = viewResult,
                        viewName = viewName,
                        view = view,
                    });
            }
        }

        public static void ViewNotFound(
            this DiagnosticSource diagnosticSource,
            ActionContext actionContext,
            bool isMainPage,
            PartialViewResult viewResult,
            string viewName,
            IEnumerable<string> searchedLocations)
        {
            if (diagnosticSource.IsEnabled("Antaris.AspNetCore.Mvc.Widgets.ViewNotFound"))
            {
                diagnosticSource.Write(
                    "Antaris.AspNetCore.Mvc.Widgets.ViewNotFound",
                    new
                    {
                        actionContext = actionContext,
                        isMainPage = isMainPage,
                        result = viewResult,
                        viewName = viewName,
                        searchedLocations = searchedLocations,
                    });
            }
        }
    }
}