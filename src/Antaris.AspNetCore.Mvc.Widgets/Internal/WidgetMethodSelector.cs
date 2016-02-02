namespace Antaris.AspNetCore.Mvc.Widgets.Internal
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Provides services for matching widget methods for execution.
    /// </summary>
    public static class WidgetMethodSelector
    {
        private static readonly string[] SyncMethodNames = new[]
        {
            "Invoke{0}{1}",
            "Invoke{0}",
            "Invoke{1}",
            "Invoke"
        };

        private static readonly string[] AsyncMethodNames = new[]
        {
            "Invoke{0}{1}Async",
            "Invoke{0}Async",
            "Invoke{1}Async",
            "InvokeAsync"
        };

        /// <summary>
        /// Finds an asynchronous method to execute.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <param name="widgetType">The widget type.</param>
        /// <returns>The asynchronous method.</returns>
        public static MethodInfo FindAsyncMethod(WidgetContext context, TypeInfo widgetType)
        {
            string httpMethod = ResolveHttpMethod(context);
            string state = string.Empty; // Resolve a widget state?
            MethodInfo method = null;

            for (int i = 0; i < AsyncMethodNames.Length; i++)
            {
                string name = string.Format(AsyncMethodNames[i], state, httpMethod);
                method = GetMethod(name, widgetType);
                if (method != null)
                {
                    break;
                }
            }

            if (method == null)
            {
                return null;
            }

            if (!method.ReturnType.GetTypeInfo().IsGenericType
                || method.ReturnType.GetGenericTypeDefinition() != typeof(Task<>))
            {
                throw new InvalidOperationException($"Async method '{method.Name}' must return a task.");
            }

            return method;
        }

        /// <summary>
        /// Finds a synchronous method to execute.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <param name="widgetType">The widget type.</param>
        /// <returns>The synchronous method.</returns>
        public static MethodInfo FindSyncMethod(WidgetContext context, TypeInfo widgetType)
        {
            string httpMethod = ResolveHttpMethod(context);
            string state = string.Empty; // Resolve a widget state?
            MethodInfo method = null;

            for (int i = 0; i < SyncMethodNames.Length; i++)
            {
                string name = string.Format(SyncMethodNames[i], state, httpMethod);
                method = GetMethod(name, widgetType);
                if (method != null)
                {
                    break;
                }
            }

            if (method == null)
            {
                return null;
            }

            if (method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException($"Sync method '{method.Name}' should return a value.");
            }

            if (method.ReturnType.IsAssignableFrom(typeof(Task)))
            {
                throw new InvalidOperationException($"Sync method '{method.Name}' cannot return a task.");
            }

            return method;
        }

        /// <summary>
        /// Gets the method for the given name.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="widgetType">The widget type.</param>
        /// <returns>The method instance.</returns>
        private static MethodInfo GetMethod(string name, TypeInfo widgetType)
        {
#if NET451
            return widgetType.AsType().GetMethod(name, BindingFlags.Public | BindingFlags.Instance);
#else
            var method = widgetType.AsType().GetMethod(name);
            return method != null && method.IsStatic ? null : method;
#endif
        }

        /// <summary>
        /// Resolves the HTTP method used to discover which action to execute.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <returns>The HTTP method.</returns>
        private static string ResolveHttpMethod(WidgetContext context)
        {
            string httpMethod = context.ViewContext?.HttpContext?.Request?.Method;
            if (string.Equals(httpMethod, "get", StringComparison.OrdinalIgnoreCase))
            {
                httpMethod = "Get";
            }
            else
            {
                httpMethod = "Post";
            }

            if (httpMethod == "Post" && !string.IsNullOrEmpty(context.WidgetId))
            {
                // MA - We only want to use Post if we are the active widget.
                // TODO - This needs to come from the value provider.
                var form = context.ViewContext?.HttpContext?.Request?.Form;
                if (!string.Equals(context.WidgetId, form[WidgetConventions.WidgetTarget], StringComparison.CurrentCultureIgnoreCase))
                {
                    httpMethod = "Get";
                }
            }

            return httpMethod;
        }
    }
}
