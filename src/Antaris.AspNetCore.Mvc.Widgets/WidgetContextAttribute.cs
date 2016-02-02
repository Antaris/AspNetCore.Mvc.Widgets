namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;

    /// <summary>
    /// Decorates a property with an injection target for a widget context instance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class WidgetContextAttribute : Attribute
    {
    }
}
