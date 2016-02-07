namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.AspNet.Mvc.ViewFeatures.Internal;
    using Microsoft.Extensions.Internal;
    using Microsoft.Extensions.WebEncoders;
    using Microsoft.AspNet.Html.Abstractions;
    using Antaris.AspNetCore.Mvc.Widgets.Internal;

    /// <summary>
    /// Provides a default implementation of a widget helper.
    /// </summary>
    public class DefaultWidgetHelper : IWidgetHelper, ICanHasViewContext
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
            if (descriptorProvider == null)
            {
                throw new ArgumentNullException(nameof(descriptorProvider));
            }

            if (htmlEncoder == null)
            {
                throw new ArgumentNullException(nameof(htmlEncoder));
            }

            if (invokerFactory == null)
            {
                throw new ArgumentNullException(nameof(invokerFactory));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (viewBufferScope == null)
            {
                throw new ArgumentNullException(nameof(viewBufferScope));
            }

            _descriptorProvider = descriptorProvider;
            _htmlEncoder = htmlEncoder;
            _invokerFactory = invokerFactory;
            _selector = selector;
            _viewBufferScope = viewBufferScope;
        }

        /// <inheritdoc />
        public void Contextualize(ViewContext viewContext)
        {
            _viewContext = viewContext;
        }

        /// <inheritdoc />
        public Task<IHtmlContent> InvokeAsync(Type widgetType, object arguments = null)
        {
            if (widgetType == null)
            {
                throw new ArgumentNullException(nameof(widgetType));
            }

            var descriptor = SelectWidget(widgetType);

            if (descriptor == null)
            {
                throw new InvalidOperationException($"Cannot find widget of type '{widgetType.FullName}");
            }

            return InvokeCoreAsync(descriptor, null, arguments);
        }

        /// <inheritdoc />
        public Task<IHtmlContent> InvokeAsync(Type widgetType, string id, object arguments = null)
        {
            if (widgetType == null)
            {
                throw new ArgumentNullException(nameof(widgetType));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var descriptor = SelectWidget(widgetType);

            if (descriptor == null)
            {
                throw new InvalidOperationException($"Cannot find widget of type '{widgetType.FullName}");
            }

            return InvokeCoreAsync(descriptor, id, arguments);
        }

        /// <inheritdoc />
        public Task<IHtmlContent> InvokeAsync(string name, object arguments = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var descriptor = _selector.SelectWidget(name);

            if (descriptor == null)
            {
                throw new InvalidOperationException($"Cannot find widget '{name}");
            }

            return InvokeCoreAsync(descriptor, null, arguments);
        }

        /// <inheritdoc />
        public Task<IHtmlContent> InvokeAsync(string name, string id, object arguments = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var descriptor = _selector.SelectWidget(name);

            if (descriptor == null)
            {
                throw new InvalidOperationException($"Cannot find widget '{name}");
            }

            return InvokeCoreAsync(descriptor, id, arguments);
        }

        /// <summary>
        /// Invokes a widget asynchronously.
        /// </summary>
        /// <param name="writer">The target text writer.</param>
        /// <param name="descriptor">The widget descriptor.</param>
        /// <param name="arguments">The set of values to provide to the widget.</param>
        /// <returns>The task instance.</returns>
        private async Task<IHtmlContent> InvokeCoreAsync(WidgetDescriptor descriptor, string id, object arguments = null)
        {
            var viewBuffer = new ViewBuffer(_viewBufferScope, descriptor.FullName);
            using (var writer = new HtmlContentWrapperTextWriter(viewBuffer, _viewContext.Writer.Encoding))
            {
                var context = new WidgetContext(
                    descriptor,
                    PropertyHelper.ObjectToDictionary(arguments),
                    _htmlEncoder,
                    _viewContext,
                    writer)
                {
                    WidgetId = id
                };

                var invoker = _invokerFactory.CreateInstance(context);
                if (invoker == null)
                {
                    throw new InvalidOperationException("IWidgetComponentFactory return null.");
                }

                await invoker.InvokeAsync(context);
                return writer.ContentBuilder;
            }
        }

        /// <summary>
        /// Selects a widget based on a type.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <returns>The widget descriptor.</returns>
        private WidgetDescriptor SelectWidget(Type widgetType)
        {
            var widgetTypeInfo = widgetType.GetTypeInfo();

            var descriptors = _descriptorProvider.Widgets;

            foreach (var descriptor in descriptors.Items)
            {
                if (descriptor.TypeInfo == widgetTypeInfo)
                {
                    return descriptor;
                }
            }

            throw new InvalidOperationException("Cannot find widget type " + widgetType.FullName);
        }
    }
}
