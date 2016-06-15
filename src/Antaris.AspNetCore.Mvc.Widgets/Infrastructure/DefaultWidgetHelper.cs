namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Text.Encodings.Web;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
    using Microsoft.Extensions.Internal;

    /// <summary>
    /// Provides a default implementation of a widget helper.
    /// </summary>
    public class DefaultWidgetHelper : IWidgetHelper, IViewContextAware
    {
        private readonly IWidgetDescriptorCollectionProvider _descriptorProvider;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly IWidgetInvokerFactory _invokerFactory;
        private readonly IWidgetSelector _selector;
        private readonly IViewBufferScope _viewBufferScope;
        private ViewContext _viewContext;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetHelper"/>.
        /// </summary>
        /// <param name="descriptorProvider">The widget descriptor collection provider.</param>
        /// <param name="invokerFactory">The invoker factory.</param>
        /// <param name="selector">The selector.</param>
        public DefaultWidgetHelper(
            IWidgetDescriptorCollectionProvider descriptorProvider,
            HtmlEncoder htmlEncoder,
            IWidgetInvokerFactory invokerFactory,
            IWidgetSelector selector,
            IViewBufferScope viewBufferScope)
        {
            _descriptorProvider = Ensure.ArgumentNotNull(descriptorProvider, nameof(descriptorProvider));
            _htmlEncoder = Ensure.ArgumentNotNull(htmlEncoder, nameof(htmlEncoder));
            _invokerFactory = Ensure.ArgumentNotNull(invokerFactory, nameof(invokerFactory));
            _selector = Ensure.ArgumentNotNull(selector, nameof(selector));
            _viewBufferScope = Ensure.ArgumentNotNull(viewBufferScope, nameof(viewBufferScope));
        }

        /// <inheritdoc />
        public void Contextualize(ViewContext viewContext)
        {
            _viewContext = Ensure.ArgumentNotNull(viewContext, nameof(viewContext));
        }

        /// <inheritdoc />
        public Task<IHtmlContent> InvokeAsync(TypeInfo widgetType, object values = null, string widgetId = null, string widgetState = null)
        {
            var descriptor = SelectWidget(widgetType);

            return InvokeCoreAsync(descriptor, values, widgetId, widgetState);
        }

        /// <inheritdoc />
        public Task<IHtmlContent> InvokeAsync(string name, object values = null, string widgetId = null, string widgetState = null)
        {
            var descriptor = SelectWidget(name);

            return InvokeCoreAsync(descriptor, values, widgetId, widgetState);
        }

        /// <summary>
        /// Invokes a widget asynchronously.
        /// </summary>>
        /// <param name="descriptor">The widget descriptor.</param>
        /// <param name="values">The set of values to provide to the widget.</param>
        /// <param name="widgetId">[Optional] The widget id.</param>
        /// <param name="widgetState">[Optional] The widget state.</param>
        /// <returns>The task instance.</returns>
        private async Task<IHtmlContent> InvokeCoreAsync(WidgetDescriptor descriptor, object values = null, string widgetId = null, string widgetState = null)
        {
            var viewBuffer = new ViewBuffer(_viewBufferScope, descriptor.FullName, ViewBuffer.ViewComponentPageSize);
            using (var writer = new ViewBufferTextWriter(viewBuffer, _viewContext.Writer.Encoding))
            {
                var context = new WidgetContext(descriptor, PropertyHelper.ObjectToDictionary(values), _htmlEncoder, _viewContext, writer)
                {
                    WidgetId = widgetId,
                    WidgetState = widgetState
                };
                var invoker = _invokerFactory.CreateInstance(context);

                await invoker.InvokeAsync(context);
                return viewBuffer;
            }
        }

        /// <summary>
        /// Selects a widget based on a name.
        /// </summary>
        /// <param name="name">The widget name.</param>
        /// <returns>The widget descriptor.</returns>
        private WidgetDescriptor SelectWidget(string name)
        {
            var descriptor = _selector.SelectWidget(name);
            if (descriptor == null)
            {
                throw new InvalidOperationException("Cannot find widget named " + name);
            }

            return descriptor;
        }

        /// <summary>
        /// Selects a widget based on a type.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <returns>The widget descriptor.</returns>
        private WidgetDescriptor SelectWidget(TypeInfo widgetType)
        {
            var descriptor = _selector.SelectWidget(widgetType);
            if (descriptor == null)
            {
                throw new InvalidOperationException("Cannot find widget type " + widgetType.FullName);
            }

            return descriptor;
        }

        /// <summary>
        /// Sets the view context for the widget.
        /// </summary>
        /// <param name="context">The context instance.</param>
        public void SetViewContext(ViewContext context)
        {
            _viewContext = context;
        }
    }
}
