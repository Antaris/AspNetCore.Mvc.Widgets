namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.AspNet.Mvc.ViewFeatures.Internal;
    using Microsoft.AspNet.Routing;

    /// <summary>
    /// Provides a default implementation of a widget helper.
    /// </summary>
    public class DefaultWidgetHelper : IWidgetHelper, ICanHasViewContext
    {
        private readonly IWidgetDescriptorCollectionProvider _descriptorProvider;
        private readonly IWidgetInvokerFactory _invokerFactory;
        private readonly IWidgetSelector _selector;
        private ViewContext _viewContext;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetHelper"/>.
        /// </summary>
        /// <param name="descriptorProvider">The widget descriptor collection provider.</param>
        /// <param name="invokerFactory">The invoker factory.</param>
        /// <param name="selector">The selector.</param>
        public DefaultWidgetHelper(IWidgetDescriptorCollectionProvider descriptorProvider, IWidgetInvokerFactory invokerFactory, IWidgetSelector selector)
        {
            if (descriptorProvider == null)
            {
                throw new ArgumentNullException(nameof(descriptorProvider));
            }

            if (invokerFactory == null)
            {
                throw new ArgumentNullException(nameof(invokerFactory));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            _descriptorProvider = descriptorProvider;
            _invokerFactory = invokerFactory;
            _selector = selector;
        }

        /// <inheritdoc />
        public void Contextualize(ViewContext viewContext)
        {
            _viewContext = viewContext;
        }

        /// <inheritdoc />
        public HtmlString Invoke(Type widgetType, object values = null)
        {
            var descriptor = SelectWidget(widgetType);

            using (var writer = new StringWriter())
            {
                InvokeCore(writer, descriptor, values);
                return new HtmlString(writer.ToString());
            }
        }

        /// <inheritdoc />
        public HtmlString Invoke(string name, object values = null)
        {
            var descriptor = SelectWidget(name);

            using (var writer = new StringWriter())
            {
                InvokeCore(writer, descriptor, values);
                return new HtmlString(writer.ToString());
            }
        }

        /// <inheritdoc />
        public async Task<HtmlString> InvokeAsync(Type widgetType, object values = null)
        {
            var descriptor = SelectWidget(widgetType);

            using (var writer = new StringWriter())
            {
                await InvokeCoreAsync(writer, descriptor, values);
                return new HtmlString(writer.ToString());
            }
        }

        /// <inheritdoc />
        public async Task<HtmlString> InvokeAsync(string name, object values = null)
        {
            var descriptor = SelectWidget(name);

            using (var writer = new StringWriter())
            {
                await InvokeCoreAsync(writer, descriptor, values);
                return new HtmlString(writer.ToString());
            }
        }

        /// <inheritdoc />
        public void RenderInvoke(Type widgetType, object values = null)
        {
            var descriptor = SelectWidget(widgetType);

            InvokeCore(_viewContext.Writer, descriptor, values);
        }

        /// <inheritdoc />
        public void RenderInvoke(string name, object values = null)
        {
            var descriptor = SelectWidget(name);

            InvokeCore(_viewContext.Writer, descriptor, values);
        }

        /// <inheritdoc />
        public Task RenderInvokeAsync(Type widgetType, object values = null)
        {
            var descriptor = SelectWidget(widgetType);

            return InvokeCoreAsync(_viewContext.Writer, descriptor, values);
        }

        /// <inheritdoc />
        public Task RenderInvokeAsync(string name, object values = null)
        {
            var descriptor = SelectWidget(name);

            return InvokeCoreAsync(_viewContext.Writer, descriptor, values);
        }

        /// <summary>
        /// Invokes a widget asynchronously.
        /// </summary>
        /// <param name="writer">The target text writer.</param>
        /// <param name="descriptor">The widget descriptor.</param>
        /// <param name="values">The set of values to provide to the widget.</param>
        /// <returns>The task instance.</returns>
        private Task InvokeCoreAsync(TextWriter writer, WidgetDescriptor descriptor, object values = null)
        {
            var context = new WidgetContext(descriptor, new RouteValueDictionary(values), _viewContext, writer);
            var invoker = _invokerFactory.CreateInstance(context);

            return invoker.InvokeAsync(context);
        }

        /// <summary>
        /// Invokes a widget synchronously.
        /// </summary>
        /// <param name="writer">The target text writer.</param>
        /// <param name="descriptor">The widget descriptor.</param>
        /// <param name="values">The set of values to provide to the widget.</param>
        private void InvokeCore(TextWriter writer, WidgetDescriptor descriptor, object values = null)
        {
            var context = new WidgetContext(descriptor, new RouteValueDictionary(values), _viewContext, writer);
            var invoker = _invokerFactory.CreateInstance(context);

            invoker.Invoke(context);
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
        private WidgetDescriptor SelectWidget(Type widgetType)
        {
            var descriptors = _descriptorProvider.Widgets;
            foreach (var descriptor in descriptors.Items)
            {
                if (descriptor.Type == widgetType)
                {
                    return descriptor;
                }
            }

            throw new InvalidOperationException("Cannot find widget type " + widgetType.FullName);
        }
    }
}
