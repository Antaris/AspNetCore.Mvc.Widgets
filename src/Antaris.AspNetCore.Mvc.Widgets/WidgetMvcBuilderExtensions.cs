namespace Microsoft.Extensions.DependencyInjection
{
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Provides extensions for the <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the MVC widgets services to the MVC services registration.
        /// </summary>
        /// <param name="services">The mvc builder.</param>
        /// <returns>The mvc builder.</returns>
        public static IMvcBuilder AddMvcWidgets(this IMvcBuilder builder)
        {
            builder.Services.AddMvcWidgets();

            builder.PartManager.FeatureProviders.Add(new WidgetFeatureProvider());

            return builder;
        }

        /// <summary>
        /// Adds the MVC widgets services to the MVC services registration.
        /// </summary>
        /// <param name="services">The mvc core builder.</param>
        /// <returns>The mvc core builder.</returns>
        public static IMvcCoreBuilder AddMvcWidgets(this IMvcCoreBuilder builder)
        {
            builder.Services.AddMvcWidgets();

            builder.PartManager.FeatureProviders.Add(new WidgetFeatureProvider());

            return builder;
        }
    }
}