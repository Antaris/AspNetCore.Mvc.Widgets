using Antaris.AspNetCore.Mvc.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public static class WidgetControllerExtensions
    {
        public static WidgetResult Widget(this Controller controller, string name, object arguments = null)
        {
            return new WidgetResult()
            {
                Arguments = arguments,
                WidgetName = name,
                TempData = controller.TempData,
                ViewData = controller.ViewData
            };
        }
    }
}
