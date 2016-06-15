namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Antaris.AspNetCore.Mvc.Widgets.Internal;

    /// <summary>
    /// Discovers widgets from a list of <see cref="ApplicationPart"/> instances.
    /// </summary>
    public class WidgetFeatureProvider : IApplicationFeatureProvider<WidgetFeature>
    {
        /// <inheritdoc />
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, WidgetFeature feature)
        {
            Ensure.ArgumentNotNull(parts, nameof(parts));
            Ensure.ArgumentNotNull(feature, nameof(feature));

            foreach (var type in parts.OfType<IApplicationPartTypeProvider>().SelectMany(p => p.Types))
            {
                if (WidgetConventions.IsWidget(type) && !feature.Widgets.Contains(type))
                {
                    feature.Widgets.Add(type);
                }
            }
        }
    }
}
