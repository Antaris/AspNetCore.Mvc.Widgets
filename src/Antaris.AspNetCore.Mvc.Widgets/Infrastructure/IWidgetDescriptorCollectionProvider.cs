namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    /// <summary>
    /// Defines the required contract for implementing a widget descriptor collection provider.
    /// </summary>
    public interface IWidgetDescriptorCollectionProvider
    {
        /// <summary>
        /// Gets the collection of widget descriptors.
        /// </summary>
        WidgetDescriptorCollection Widgets { get; }
    }
}
