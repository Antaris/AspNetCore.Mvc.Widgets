namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;

    /// <summary>
    /// Provides a description of a Widget.
    /// </summary>
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
                    _displayName = Type?.FullName;
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
        /// Gets or sets the full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public Type Type { get; set; }
    }
}
