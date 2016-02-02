namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;

    /// <summary>
    /// Decorates a class to mark it as an executable Widget.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WidgetAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the widget name.
        /// </summary>
        public string Name { get; set; }
    }
}
