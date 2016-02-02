namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc.Controllers;
    using Microsoft.AspNet.Mvc.Infrastructure;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.AspNet.Routing;
    using Antaris.AspNetCore.Mvc.Widgets.Internal;

    /// <summary>
    /// Provides a default implementation of a widget invoker.
    /// </summary>
    public class DefaultWidgetInvoker : IWidgetInvoker
    {
        private readonly ITypeActivatorCache _typeActivatorCache;
        private readonly IWidgetActivator _widgetActivator;
        private readonly IWidgetArgumentBinder _argumentBinder;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetInvoker"/>.
        /// </summary>
        /// <param name="typeActivatorCache">The type activator cache.</param>
        /// <param name="widgetActivator">The widget activator.</param>
        /// <param name="argumentBinder">The widget argument binder.</param>
        public DefaultWidgetInvoker(ITypeActivatorCache typeActivatorCache, IWidgetActivator widgetActivator, IWidgetArgumentBinder argumentBinder)
        {
            if (typeActivatorCache == null)
            {
                throw new ArgumentNullException(nameof(typeActivatorCache));
            }

            if (widgetActivator == null)
            {
                throw new ArgumentNullException(nameof(widgetActivator));
            }

            if (argumentBinder == null)
            {
                throw new ArgumentNullException(nameof(argumentBinder));
            }

            _typeActivatorCache = typeActivatorCache;
            _widgetActivator = widgetActivator;
            _argumentBinder = argumentBinder;
        }

        /// <inheritdoc />
        public void Invoke(WidgetContext context)
        {
            var method = WidgetMethodSelector.FindSyncMethod(context, context.WidgetDescriptor.Type.GetTypeInfo());

            if (method == null)
            {
                throw new InvalidOperationException("Cannot find an appropriate method.");
            }

            var result = InvokeSyncCore(method, context);

            result.Execute(context);
        }

        /// <inheritdoc />
        public async Task InvokeAsync(WidgetContext context)
        {
            IWidgetResult result;
            var asyncMethod = WidgetMethodSelector.FindAsyncMethod(context, context.WidgetDescriptor.Type.GetTypeInfo());

            if (asyncMethod == null)
            {
                var syncMethod = WidgetMethodSelector.FindSyncMethod(context, context.WidgetDescriptor.Type.GetTypeInfo());

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
        /// Creates a widget instance.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <returns>The wiget instance.</returns>
        private object CreateWidget(WidgetContext context)
        {
            var services = context.ViewContext.HttpContext.RequestServices;
            var widget = _typeActivatorCache.CreateInstance<object>(
                services,
                context.WidgetDescriptor.Type);
            _widgetActivator.Activate(widget, context);

            return widget;
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

            var htmlStringResult = value as HtmlString;
            if (htmlStringResult != null)
            {
                return new ContentWidgetResult(htmlStringResult);
            }

            throw new InvalidOperationException($"Widgets only support returning {typeof(string).Name}, {typeof(HtmlString).Name} or {typeof(IWidgetResult).Name}");
        }

        /// <summary>
        /// Invokes an asynchronous method.
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="context">The widget context.</param>
        /// <returns>The widget result.</returns>
        private async Task<IWidgetResult> InvokeAsyncCore(MethodInfo method, WidgetContext context)
        {
            var widget = CreateWidget(context);

            var arguments = await GetArgumentsAsync(context, method, context.Values);

            var result = await ControllerActionExecutor.ExecuteAsync(method, widget, arguments);
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
            var widget = CreateWidget(context);
            object result = null;

            var arguments = GetArgumentsAsync(context, method, context.Values).GetAwaiter().GetResult();

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
        private async Task<object[]> GetArgumentsAsync(WidgetContext context, MethodInfo method, RouteValueDictionary values)
        {
            var arguments = await _argumentBinder.BindArgumentsAsync(context, method, values);
            return arguments.Values.ToArray();
        }
    }
}
