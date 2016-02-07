namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the required contract for implementing a widget argument binder.
    /// </summary>
    public interface IWidgetArgumentBinder
    {
        /// <summary>
        /// Binds a set of arguments for the given method.
        /// </summary>
        /// <param name="context">The widget context.</param>
        /// <param name="method">The method to invoke.</param>
        /// <param name="values">The set of invocation-provided values.</param>
        /// <returns>The set of bound arguments as a deferred task result.</returns>
        Task<object[]> BindArgumentsAsync(WidgetContext context, MethodInfo method, IDictionary<string, object> values);
    }
}
