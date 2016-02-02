namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the required contract for implementing a widget descriptor provider.
    /// </summary>
    public interface IWidgetDescriptorProvider
    {
        /// <summary>
        /// Gets the available widgets represented by widget descriptors.
        /// </summary>
        /// <returns>The set of widget descriptors.</returns>
        IEnumerable<WidgetDescriptor> GetWidgets();
    }
}
