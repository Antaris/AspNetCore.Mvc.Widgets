namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a collection of widget descriptors.
    /// </summary>
    public class WidgetDescriptorCollection
    {
        /// <summary>
        /// Initialises a new instance of <see cref="WidgetDescriptorCollection"/>.
        /// </summary>
        /// <param name="items">The set of widget descriptor items.</param>
        /// <param name="version">The collection version.</param>
        public WidgetDescriptorCollection(IEnumerable<WidgetDescriptor> items, int version)
        {
            Items = new List<WidgetDescriptor>(items ?? Enumerable.Empty<WidgetDescriptor>());
            Version = version;
        }

        /// <summary>
        /// Gets the set of items.
        /// </summary>
        public IReadOnlyList<WidgetDescriptor> Items { get; }

        /// <summary>
        /// Gets the collection version.
        /// </summary>
        public int Version { get; }
    }
}
