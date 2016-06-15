namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Antaris.AspNetCore.Mvc.Widgets.Internal;

    /// <summary>
    /// Provides a default implementation of a widget descriptor provider.
    /// </summary>
    public class DefaultWidgetDescriptorProvider : IWidgetDescriptorProvider
    {
        private readonly ApplicationPartManager _partManager;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetDescriptorProvider"/>.
        /// </summary>
        /// <param name="partManager">The application part manager.</param>
        public DefaultWidgetDescriptorProvider(ApplicationPartManager partManager)
        {
            _partManager = Ensure.ArgumentNotNull(partManager, nameof(partManager));
        }

        /// <inheritdoc />
        public IEnumerable<WidgetDescriptor> GetWidgets()
        {
            return GetCandidateTypes().Select(CreateDescriptor);
        }

        /// <summary>
        /// Gets the candidate <see cref="TypeInfo"/> instances provided by the <see cref="ApplicationPartManager"/>.
        /// </summary>
        /// <returns>A list of <see cref="TypeInfo"/> instances.</returns>
        protected virtual IEnumerable<TypeInfo> GetCandidateTypes()
        {
            var feature = new WidgetFeature();
            _partManager.PopulateFeature(feature);
            return feature.Widgets;
        }

        /// <summary>
        /// Creates a descriptor for the given widget type.
        /// </summary>
        /// <param name="typeInfo">The widget type.</param>
        /// <returns>The widget descriptor.</returns>
        private static WidgetDescriptor CreateDescriptor(TypeInfo typeInfo)
        {
            var descriptor = new WidgetDescriptor
            {
                FullName = WidgetConventions.GetWidgetFullName(typeInfo),
                ShortName = WidgetConventions.GetWidgetName(typeInfo),
                TypeInfo = typeInfo
            };

            return descriptor;
        }
    }
}