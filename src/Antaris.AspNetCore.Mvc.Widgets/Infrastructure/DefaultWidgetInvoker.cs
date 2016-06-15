namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Internal;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Antaris.AspNetCore.Mvc.Widgets.Internal;

    /// <summary>
    /// Provides a default implementation of a widget invoker.
    /// </summary>
    public class DefaultWidgetInvoker : IWidgetInvoker
    {
        private readonly IWidgetFactory _widgetFactory;
        private readonly IWidgetArgumentBinder _argumentBinder;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetInvoker"/>.
        /// </summary>
        /// <param name="widgetFactory">The widget factory.</param>
        /// <param name="argumentBinder">The widget argument binder.</param>
        public DefaultWidgetInvoker(
            IWidgetFactory widgetFactory,
            IWidgetArgumentBinder argumentBinder)
        {
            _widgetFactory = Ensure.ArgumentNotNull(widgetFactory, nameof(widgetFactory));
            _argumentBinder = Ensure.ArgumentNotNull(argumentBinder, nameof(argumentBinder));
        }

        /// <inheritdoc />
        public async Task InvokeAsync(WidgetContext context)
        {
            IWidgetResult result;
            var asyncMethod = WidgetMethodSelector.FindAsyncMethod(context, context.WidgetDescriptor.TypeInfo);

            if (asyncMethod == null)
            {
                var syncMethod = WidgetMethodSelector.FindSyncMethod(context, context.WidgetDescriptor.TypeInfo);

                if (syncMethod == null)
                {
                    throw new InvalidOperationException("Cannot find an appropriate method.");
                }

                result = InvokeSyncCore(syncMethod, context);
            }
            else
            {
                result = await InvokeAsyncCore(asyncMethod, context);
            }

            await result.ExecuteAsync(context);
        }

        /// <summary>
        /// Coerces the output result of a widget to a <see cref="IWidgetResult"/> given that a method could return anything.
        /// </summary>
        /// <param name="value">The result value.</param>
        /// <returns>The coerced widget result.</returns>
        private static IWidgetResult CoerceToWidgetResult(object value)
        {
            var widgetResult = value as IWidgetResult;
            if (widgetResult != null)
            {
                return widgetResult;
            }

            var stringResult = value as string;
            if (stringResult != null)
            {
                return new ContentWidgetResult(stringResult);
            }

            var htmlContentResult = value as IHtmlContent;
            if (htmlContentResult != null)
            {
                return new HtmlContentWidgetResult(htmlContentResult);
            }

            throw new InvalidOperationException($"Widgets only support returning {typeof(string).Name}, {typeof(IHtmlContent).Name} or {typeof(IWidgetResult).Name}");
        }

        /// <summary>
        /// Invokes an asynchronous method.
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="context">The widget context.</param>
        /// <returns>The widget result.</returns>
        private async Task<IWidgetResult> InvokeAsyncCore(MethodInfo method, WidgetContext context)
        {
            var widget = _widgetFactory.CreateWidget(context);

            var arguments = await GetArgumentsAsync(context, method, context.Arguments);
            var executor = ObjectMethodExecutor.Create(method, context.WidgetDescriptor.TypeInfo);

            var result = await ControllerActionExecutor.ExecuteAsync(executor, widget, arguments);
            var widgetResult = CoerceToWidgetResult(result);
            return widgetResult;
        }

        /// <summary>
        /// Invokes a synchronous method.
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="context">The widget context.</param>
        /// <returns>The widget result.</returns>
        private IWidgetResult InvokeSyncCore(MethodInfo method, WidgetContext context)
        {
            var widget = _widgetFactory.CreateWidget(context);
            object result = null;

            var arguments = GetArgumentsAsync(context, method, context.Arguments).GetAwaiter().GetResult();

            try
            {
                result = method.Invoke(widget, arguments);
                var widgetResult = CoerceToWidgetResult(result);

                return widgetResult;
            }
            catch (TargetInvocationException ex)
            {
                var exceptionInfo = ExceptionDispatchInfo.Capture(ex.InnerException);
                exceptionInfo.Throw();

                return null; // Unreachable
            }
        }

        /// <summary>
        /// Gets the arguments for the target method. 
        /// </summary>
        /// <remarks>
        /// The argument binder will provide a mixed-result of values from the provided dictionary, and those from model binding.
        /// </remarks>
        /// <param name="context">The widget context.</param>
        /// <param name="method">The target method.</param>
        /// <param name="values">The set of provided values.</param>
        /// <returns>The bound arguments.</returns>
        private async Task<object[]> GetArgumentsAsync(WidgetContext context, MethodInfo method, IDictionary<string, object> values)
        {
            var arguments = await _argumentBinder.BindArgumentsAsync(context, method, values);
            return arguments.Values.ToArray();
        }
    }
}
