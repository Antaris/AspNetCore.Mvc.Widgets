namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Reflection;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Internal;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Provides a base implementation of a widget result executor.
    /// </summary>
    public class WidgetResultExecutor
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly HtmlHelperOptions _htmlHelperOptions;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IWidgetHelper _widgetHelper;

        /// <summary>
        /// Initialises a new instance of <see cref="WidgetResultExecutor"/>
        /// </summary>
        /// <param name="mvcHelperOptions">The MVC helper options.</param>
        /// <param name="widgetHelper">The widget helper.</param>
        /// <param name="htmlEncoder">The HTML encoder.</param>
        /// <param name="modelMetadataProvider">The model metadata provider.</param>
        /// <param name="tempDataDictionaryFactory">The temp data dictionary factory.</param>
        public WidgetResultExecutor(
            IOptions<MvcViewOptions> mvcHelperOptions,
            IWidgetHelper widgetHelper,
            HtmlEncoder htmlEncoder,
            IModelMetadataProvider modelMetadataProvider,
            ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _htmlHelperOptions = Ensure.ArgumentNotNull(mvcHelperOptions, nameof(mvcHelperOptions)).Value.HtmlHelperOptions;
            _widgetHelper = Ensure.ArgumentNotNull(widgetHelper, nameof(widgetHelper));
            _htmlEncoder = Ensure.ArgumentNotNull(htmlEncoder, nameof(htmlEncoder));
            _modelMetadataProvider = Ensure.ArgumentNotNull(modelMetadataProvider, nameof(modelMetadataProvider));
            _tempDataDictionaryFactory = Ensure.ArgumentNotNull(tempDataDictionaryFactory, nameof(tempDataDictionaryFactory));
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(ActionContext context, WidgetResult widgetResult)
        {
            var response = context.HttpContext.Response;

            var viewData = widgetResult.ViewData;
            if (viewData == null)
            {
                viewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
            }

            var tempData = widgetResult.TempData;
            if (tempData == null)
            {
                tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);
            }

            string resolvedContentType;
            Encoding resolvedContentTypeEncoding;
            ResponseContentTypeHelper.ResolveContentTypeAndEncoding(widgetResult.ContentType, response.ContentType, ViewExecutor.DefaultContentType, out resolvedContentType, out resolvedContentTypeEncoding);

            response.ContentType = resolvedContentType;

            if (widgetResult.StatusCode != null)
            {
                response.StatusCode = widgetResult.StatusCode.Value;
            }

            using (var writer = new HttpResponseStreamWriter(response.Body, resolvedContentTypeEncoding))
            {
                var viewContext = new ViewContext(
                    context,
                    NullView.Instance,
                    viewData,
                    tempData,
                    writer,
                    _htmlHelperOptions);

                (_widgetHelper as IViewContextAware)?.Contextualize(viewContext);
                var result = await GetWidgetResult(_widgetHelper, widgetResult);

                result.WriteTo(writer, _htmlEncoder);
            }
        }

        /// <summary>
        /// Gets the widget result.
        /// </summary>
        /// <param name="helper">The widget helper.</param>
        /// <param name="result">The widget result.</param>
        /// <returns>The widget content.</returns>
        private Task<IHtmlContent> GetWidgetResult(IWidgetHelper helper, WidgetResult result)
        {
            if (result.WidgetType != null)
            {
                return helper.InvokeAsync(result.WidgetType.GetTypeInfo(), result.Arguments);
            }
            else
            {
                return helper.InvokeAsync(result.WidgetName, result.Arguments);
            }
        }
    }
}
