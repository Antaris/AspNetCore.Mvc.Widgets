namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNet.Mvc.Infrastructure;
    using Antaris.AspNetCore.Mvc.Widgets.Internal;

    /// <summary>
    /// Provides a default implementation of a widget descriptor provider.
    /// </summary>
    public class DefaultWidgetDescriptorProvider : IWidgetDescriptorProvider
    {
        private readonly IAssemblyProvider _assemblyProvider;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetDescriptorProvider"/>.
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        public DefaultWidgetDescriptorProvider(IAssemblyProvider assemblyProvider)
        {
            if (assemblyProvider == null)
            {
                throw new ArgumentNullException(nameof(assemblyProvider));
            }

            _assemblyProvider = assemblyProvider;
        }

        /// <inheritdoc />
        public IEnumerable<WidgetDescriptor> GetWidgets()
        {
            var types = GetCandidateTypes();

            return types.Where(t => WidgetConventions.IsWidget(t)).Select(CreateDescriptor);
        }

        /// <summary>
        /// Gets the candidate types.
        /// </summary>
        /// <returns>The set of candidate types.</returns>
        protected virtual IEnumerable<TypeInfo> GetCandidateTypes()
        {
            var assembles = _assemblyProvider.CandidateAssemblies;

            return assembles.SelectMany(a => a.ExportedTypes).Select(t => t.GetTypeInfo());
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
                Type = typeInfo.AsType()
            };

            return descriptor;
        }
    }
}