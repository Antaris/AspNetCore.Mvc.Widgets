namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents a list of available widget types.
    /// </summary>
    public class WidgetFeature
    {
        /// <summary>
        /// Gets the list of widget types in an MVC application.
        /// </summary>
        public IList<TypeInfo> Widgets { get; } = new List<TypeInfo>();
    }
}
