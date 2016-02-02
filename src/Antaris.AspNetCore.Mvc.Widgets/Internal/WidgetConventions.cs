namespace Antaris.AspNetCore.Mvc.Widgets.Internal
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides convention-based methods supporting Widget discovery.
    /// </summary>
    public static class WidgetConventions
    {
        public const string WidgetSuffix = "Widget";
        public const string WidgetTarget = "__posttarget";
        public const string WidgetState = "__poststate";

        /// <summary>
        /// Gets the short name for a widget.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <returns>The widget short name.</returns>
        public static string GetShortNameByConvention(TypeInfo widgetType)
        {
            if (widgetType == null)
            {
                throw new ArgumentNullException(nameof(widgetType));
            }

            if (widgetType.Name.EndsWith(WidgetSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return widgetType.Name.Substring(0, widgetType.Name.Length - WidgetSuffix.Length);
            }

            return widgetType.Name;
        }

        /// <summary>
        /// Gets the widget full name for the given widget type.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <returns>The widget full name.</returns>
        public static string GetWidgetFullName(TypeInfo widgetType)
        {
            if (widgetType == null)
            {
                throw new ArgumentNullException(nameof(widgetType));
            }

            var attr = widgetType.GetCustomAttribute<WidgetAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.Name))
            {
                return attr.Name;
            }

            var shortName = GetShortNameByConvention(widgetType);
            if (string.IsNullOrEmpty(widgetType.Namespace))
            {
                return shortName;
            }

            return $"{widgetType.Namespace}.{shortName}";
        }

        /// <summary>
        /// Gets the widget name for the given widget type.
        /// </summary>
        /// <param name="widgetType">The widget type.</param>
        /// <returns>The widget name.</returns>
        public static string GetWidgetName(TypeInfo widgetType)
        {
            if (widgetType == null)
            {
                throw new ArgumentNullException(nameof(widgetType));
            }

            var attr = widgetType.GetCustomAttribute<WidgetAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.Name))
            {
                var idx = attr.Name.LastIndexOf('.');
                if (idx >= 0)
                {
                    return attr.Name.Substring(idx + 1);
                }

                return attr.Name;
            }

            return GetShortNameByConvention(widgetType);
        }

        /// <summary>
        /// Determines if the given type represents a widget.
        /// </summary>
        /// <param name="typeInfo">The candidate type.</param>
        /// <returns>True if the candidate type represents a widget, otherwise false.</returns>
        public static bool IsWidget(TypeInfo typeInfo)
        {
            if (!typeInfo.IsClass
                || !typeInfo.IsPublic
                || typeInfo.IsAbstract
                || typeInfo.ContainsGenericParameters)
            {
                return false;
            }

            return typeInfo.Name.EndsWith(WidgetSuffix, StringComparison.OrdinalIgnoreCase)
                || typeInfo.GetCustomAttribute<WidgetAttribute>() != null;
        }
    }
}
