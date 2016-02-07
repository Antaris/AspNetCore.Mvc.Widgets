namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Provides a description of a Widget.
    /// </summary>
    [DebuggerDisplay("{DisplayName}")]
    public class WidgetDescriptor
    {
        private string _displayName;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <remarks>If a value for this property is not provided, it will default to the full type name.</remarks>
        public string DisplayName
        {
            get
            {
                if (_displayName == null)
                {
                    _displayName = TypeInfo?.FullName;
                }

                return _displayName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Display name must be provided.");
                }

                _displayName = value;
            }
        }

        /// <summary>
        /// Gets or sets the set of GET methods.
        /// </summary>
        public IDictionary<string, MethodInfo> GetMethods { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the set of POST methods.
        /// </summary>
        public IDictionary<string, MethodInfo> PostMethods { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public TypeInfo TypeInfo { get; set; }
    }
}
