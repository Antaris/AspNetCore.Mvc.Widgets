namespace Microsoft.Extensions.DependencyInjection
{
    using Antaris.AspNetCore.Mvc.Widgets;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Provides extensions for the <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class WidgetServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the MVC widgets services to the services collection.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The collection of services.</returns>
        public static IServiceCollection AddMvcWidgets(this IServiceCollection services)
        {
            services.AddSingleton<IWidgetSelector, DefaultWidgetSelector>();
            services.AddSingleton<IWidgetActivator, DefaultWidgetActivator>();
            services.AddSingleton<IWidgetDescriptorCollectionProvider, DefaultWidgetDescriptorCollectionProvider>();
            services.AddSingleton<IWidgetFactory, DefaultWidgetFactory>();
            services.AddSingleton<IWidgetInvokerFactory, DefaultWidgetInvokerFactory>();
            services.AddSingleton<IWidgetArgumentBinder, DefaultWidgetArgumentBinder>();
            services.AddSingleton<WidgetResultExecutor, WidgetResultExecutor>();

            services.AddTransient<IWidgetDescriptorProvider, DefaultWidgetDescriptorProvider>();
            services.AddTransient<IWidgetHelper, DefaultWidgetHelper>();

            return services;
        }
    }
}