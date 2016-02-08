namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a default implementation of a widget selector.
    /// </summary>
    public class DefaultWidgetSelector : IWidgetSelector
    {
        private readonly IWidgetDescriptorCollectionProvider _descriptorProvider;
        private WidgetDescriptorCache _cache;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetSelector"/>.
        /// </summary>
        /// <param name="descriptorProvider">The descriptor provider.</param>
        public DefaultWidgetSelector(IWidgetDescriptorCollectionProvider descriptorProvider)
        {
            _descriptorProvider = descriptorProvider;
        }

        /// <inheritdoc />
        public WidgetDescriptor SelectWidget(string widgetName)
        {
            var collection = _descriptorProvider.Widgets;
            if (_cache == null || _cache.Version != collection.Version)
            {
                _cache = new WidgetDescriptorCache(collection);
            }

            if (widgetName.Contains('.'))
            {
                return _cache.SelectByFullName(widgetName);
            }

            return _cache.SelectByShortName(widgetName);
        }

        private class WidgetDescriptorCache
        {
            private readonly ILookup<string, WidgetDescriptor> _lookupByShortName;
            private readonly ILookup<string, WidgetDescriptor> _lookupByFullName;

            public WidgetDescriptorCache(WidgetDescriptorCollection collection)
            {
                Version = collection.Version;

                _lookupByShortName = collection.Items.ToLookup(c => c.ShortName, c => c);
                _lookupByFullName = collection.Items.ToLookup(c => c.FullName, c => c);
            }

            public int Version { get; }

            public WidgetDescriptor SelectByShortName(string name)
            {
                return Select(_lookupByShortName, name);
            }

            public WidgetDescriptor SelectByFullName(string name)
            {
                return Select(_lookupByFullName, name);
            }

            private static WidgetDescriptor Select(ILookup<string, WidgetDescriptor> candidates, string name)
            {
                var matches = candidates[name];

                var count = matches.Count();
                if (count == 0)
                {
                    return null;
                }
                else if (count == 1)
                {
                    return matches.Single();
                }
                else
                {
                    var matchedTypes = new List<string>();
                    foreach (var candidate in matches)
                    {
                        matchedTypes.Add($"Type: {candidate.TypeInfo.FullName}, Name: {candidate.FullName}");
                    }

                    var typeNames = string.Join(Environment.NewLine, matchedTypes);
                    throw new InvalidOperationException($"The widget name matched multiple types:{Environment.NewLine}{typeNames}");
                }
            }
        }
    }
}
