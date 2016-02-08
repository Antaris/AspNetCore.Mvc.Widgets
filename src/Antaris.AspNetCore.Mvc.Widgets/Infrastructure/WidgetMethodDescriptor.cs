namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Reflection;

    /// <summary>
    /// Represents a widget method.
    /// </summary>
    public class WidgetMethodDescriptor
    {
        /// <summary>
        /// Gets or sets whether the method is async.
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// Gets or sets the intended HTTP method for this widget.
        /// </summary>
        public WidgetHttpMethod HttpMethod { get; set; }

        /// <summary>
        /// Gets or sets the method info.
        /// </summary>
        public MethodInfo MethodInfo { get; set; }

        /// <summary>
        /// Gets or sets the method rank.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets whether the name of the state the method represents.
        /// </summary>
        public string State { get; set; }
    }

    public enum WidgetHttpMethod
    {
        Any,
        Get,
        Post
    }
}