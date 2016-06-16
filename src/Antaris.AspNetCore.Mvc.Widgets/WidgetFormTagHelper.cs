namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    [HtmlTargetElement("form", Attributes = "widget-*")]
    public class WidgetFormTagHelper : TagHelper
    {
        [HtmlAttributeName("widget-state")]
        public string State { get; set; }
        
        [HtmlAttributeName("widget-id")]
        public string Id { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string widgetId = Id;
            string widgetState = State;

            var widgetContext = ViewContext.ViewData[WidgetConstants.WidgetContextKey] as WidgetContext;
            if (widgetContext != null)
            {
                if (string.IsNullOrEmpty(widgetId))
                {
                    widgetId = widgetContext.WidgetId;
                }

                if (string.IsNullOrEmpty(widgetState))
                {
                    widgetState = widgetContext.WidgetState;
                }
            }

            output.PostContent.AppendHtml($"<input type='hidden' name='{WidgetConstants.PostTarget}' value='{widgetId}' />");
            output.PostContent.AppendHtml($"<input type='hidden' name='{WidgetConstants.PostState}' value='{widgetState}' />");

#if !NET451
            return Task.CompletedTask;
#else
            return Task.FromResult(0);
#endif
        }
    }
}
