namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides a default implementation of a widget descriptor collection provider.
    /// </summary>
    public class DefaultWidgetDescriptorCollectionProvider : IWidgetDescriptorCollectionProvider
    {
        private readonly IWidgetDescriptorProvider _descriptorProvider;
        private WidgetDescriptorCollection _widgets;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetDescriptorCollectionProvider"/>.
        /// </summary>
        /// <param name="descriptorProvider">The widget descriptor provider.</param>
        public DefaultWidgetDescriptorCollectionProvider(IWidgetDescriptorProvider descriptorProvider)
        {
            if (descriptorProvider == null)
            {
                throw new ArgumentNullException(nameof(descriptorProvider));
            }

            _descriptorProvider = descriptorProvider;
        }

        /// <inheritdoc />
        public WidgetDescriptorCollection Widgets
        {
            get
            {
                if (_widgets == null)
                {
                    _widgets = GetWidgets();
                }

                return _widgets;
            }
        }

        /// <summary>
        /// Gets the available widget descriptors.
        /// </summary>
        /// <returns>The widget descriptor collection.</returns>
        private WidgetDescriptorCollection GetWidgets()
        {
            var descriptors = _descriptorProvider.GetWidgets();

            return new WidgetDescriptorCollection(descriptors.ToArray(), version: 0);
        }
    }
}
