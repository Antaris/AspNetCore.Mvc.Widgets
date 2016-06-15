namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides a description of a Widget.
    /// </summary>
    public class WidgetDescriptor
    {
        private string _displayName;

        /// <summary>
        /// Initialises a new instance of <see cref="WidgetDescriptor"/>
        /// </summary>
        public WidgetDescriptor()
        {
            Id = Guid.NewGuid().ToString();
        }

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
                _displayName = Ensure.ArgumentNotNullOrEmpty(value, nameof(value));
            }
        }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets a unique id for this descriptor.
        /// </summary>
        public string Id { get; set; }

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
