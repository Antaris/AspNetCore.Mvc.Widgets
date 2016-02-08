namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Html.Abstractions;
    using Microsoft.AspNet.Mvc.Controllers;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Antaris.AspNetCore.Mvc.Widgets.Internal;

    /// <summary>
    /// Provides a default implementation of a widget invoker.
    /// </summary>
    public class DefaultWidgetInvoker : IWidgetInvoker
    {
        private readonly IWidgetFactory _widgetFactory;
        private readonly DiagnosticSource _diagnosticSource;
        private readonly ILogger _logger;
        private readonly IWidgetArgumentBinder _argumentBinder;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetInvoker"/>.
        /// </summary>
        /// <param name="widgetFactory">The widget factory used to create widget instances.</param>
        /// <param name="argumentBinder">The argument binder.</param>
        /// <param name="diagnosticSource">The <see cref="DiagnosticSource"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public DefaultWidgetInvoker(
            IWidgetFactory widgetFactory,
            IWidgetArgumentBinder argumentBinder,
            DiagnosticSource diagnosticSource,
            ILogger logger)
        {
            if (widgetFactory == null)
            {
                throw new ArgumentNullException(nameof(widgetFactory));
            }

            if (argumentBinder == null)
            {
                throw new ArgumentNullException(nameof(argumentBinder));
            }

            if (diagnosticSource == null)
            {
                throw new ArgumentNullException(nameof(diagnosticSource));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _widgetFactory = widgetFactory;
            _argumentBinder = argumentBinder;
            _diagnosticSource = diagnosticSource;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(WidgetContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var method = ResolveMethod(context);
            if (method == null)
            {
                throw new ArgumentNullException("Unable to determine target widget method.");
            }

            var isAsync = typeof(Task).GetTypeInfo().IsAssignableFrom(method.MethodInfo.ReturnType.GetTypeInfo());
            IWidgetResult result;
            if (isAsync)
            {
                result = await InvokeAsyncCore(method.MethodInfo, context);
            }
            else
            {
                result = InvokeSyncCore(method.MethodInfo, context);
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

            var htmlContent = value as IHtmlContent;
            if (htmlContent != null)
            {
                return new HtmlContentWidgetResult(htmlContent);
            }

            throw new InvalidOperationException($"Widgets only support returning {typeof(string).Name}, {typeof(IHtmlContent).Name} or {typeof(IWidgetResult).Name}");
        }

        /// <summary>
        /// Invokes an asynchronous method.
        /// </summary>
        /// <param name="methodInfo">The method to invoke.</param>
        /// <param name="context">The widget context.</param>
        /// <returns>The widget result.</returns>
        private async Task<IWidgetResult> InvokeAsyncCore(MethodInfo methodInfo, WidgetContext context)
        {
            var widget = _widgetFactory.CreateWidget(context);

            using (_logger.WidgetScope(context))
            {
                var arguments = await _argumentBinder.BindArgumentsAsync(context, methodInfo, context.Arguments);

                _diagnosticSource.BeforeWidget(context, widget);
                _logger.WidgetExecuting(context, arguments);

                var startTimestamp = _logger.IsEnabled(LogLevel.Debug) ? Stopwatch.GetTimestamp() : 0;
                var result = await ControllerActionExecutor.ExecuteAsync(methodInfo, widget, arguments);

                var widgetResult = CoerceToWidgetResult(result);
                _logger.WidgetExecuted(context, startTimestamp, widgetResult);
                _diagnosticSource.AfterWidget(context, widgetResult, widget);

                _widgetFactory.ReleaseWidget(context, widget);

                return widgetResult;
            }
        }

        /// <summary>
        /// Invokes a synchronous method.
        /// </summary>
        /// <param name="methodInfo">The method to invoke.</param>
        /// <param name="context">The widget context.</param>
        /// <returns>The widget result.</returns>
        private IWidgetResult InvokeSyncCore(MethodInfo methodInfo, WidgetContext context)
        {
            var widget = _widgetFactory.CreateWidget(context);

            using (_logger.WidgetScope(context))
            {
                var arguments = _argumentBinder.BindArgumentsAsync(context, methodInfo, context.Arguments).GetAwaiter().GetResult();

                _diagnosticSource.BeforeWidget(context, widget);
                _logger.WidgetExecuting(context, arguments);

                var startTimestamp = _logger.IsEnabled(LogLevel.Debug) ? Stopwatch.GetTimestamp() : 0;
                object result;
                try
                {
                    result = methodInfo.Invoke(widget, arguments);
                }
                catch (TargetInvocationException ex)
                {
                    _widgetFactory.ReleaseWidget(context, widget);

                    var exceptionInfo = ExceptionDispatchInfo.Capture(ex.InnerException);
                    exceptionInfo.Throw();

                    // Unreachable.
                    return null;
                }

                var widgetResult = CoerceToWidgetResult(result);
                _logger.WidgetExecuted(context, startTimestamp, widgetResult);
                _diagnosticSource.AfterWidget(context, widgetResult, widget);

                _widgetFactory.ReleaseWidget(context, widget);

                return widgetResult;
            }
        }

        /// <summary>
        /// Resolves the method that will be executed by the widget.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <returns>The method to execute.</returns>
        private WidgetMethodDescriptor ResolveMethod(WidgetContext context)
        {
            // We need to resolve some state and a widget id.
            var request = context.ViewContext?.HttpContext?.Request;
            string id = null;
            string state = null;
            WidgetHttpMethod httpMethod = WidgetHttpMethod.Get;

            if (request != null)
            {
                httpMethod = string.Equals(request.Method, "post", StringComparison.OrdinalIgnoreCase)
                    ? WidgetHttpMethod.Post : httpMethod;

                if (httpMethod == WidgetHttpMethod.Post)
                {
                    state = request.Form[WidgetConventions.WidgetState];
                    id = request.Form[WidgetConventions.WidgetTarget];

                    if (!string.Equals(id, context.WidgetId, StringComparison.OrdinalIgnoreCase))
                    {
                        // Enforce we only use GET methods.
                        httpMethod = WidgetHttpMethod.Get;
                    }
                }
            }

            /*
             * Rank:
             * Invoke{State}{Method}Async/Invoke{State}{Method}
             * Invoke{State}Async/Invoke{State}
             * Invoke{Method}Async/Invoke{Method}
             * InvokeAsync/Invoke
             */
            
            for (int i = 0; i < context.WidgetDescriptor.Methods.Length; i++)
            {
                var descriptor = context.WidgetDescriptor.Methods[i];

                if (descriptor.HttpMethod != httpMethod && descriptor.HttpMethod != WidgetHttpMethod.Any)
                {
                    continue;
                }

                if (string.Equals(descriptor.State, state))
                {
                    // This method will work for us.
                    return descriptor;
                }
            }

            return null;
        }
    }
}
