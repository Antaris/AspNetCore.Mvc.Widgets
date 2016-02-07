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

            var methodInfo = ResolveMethod(context.WidgetDescriptor);
            if (methodInfo == null)
            {
                throw new ArgumentNullException("Unable to determine target widget method.");
            }

            var isAsync = typeof(Task).GetTypeInfo().IsAssignableFrom(methodInfo.ReturnType.GetTypeInfo());
            IWidgetResult result;
            if (isAsync)
            {
                result = await InvokeAsyncCore(methodInfo, context);
            }
            else
            {
                result = InvokeSyncCore(methodInfo, context);
            }

            return result.ExecuteAsync(context);
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
                return new ContentWidgetResult(htmlContent);
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
    }
}
