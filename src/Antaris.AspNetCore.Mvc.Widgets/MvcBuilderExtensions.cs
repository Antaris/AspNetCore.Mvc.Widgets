namespace Microsoft.Extensions.DependencyInjection
{
    using Antaris.AspNetCore.Mvc.Widgets;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Provides extensions for the <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the MVC widgets services to the services collection.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The collection of services.</returns>
        public static IMvcBuilder AddMvcWidgets(this IMvcBuilder builder)
        {
            builder.Services.AddSingleton<IWidgetSelector, DefaultWidgetSelector>();
            builder.Services.AddSingleton<IWidgetActivator, DefaultWidgetActivator>();
            builder.Services.AddSingleton<IWidgetDescriptorCollectionProvider, DefaultWidgetDescriptorCollectionProvider>();
            builder.Services.AddSingleton<IWidgetFactory, DefaultWidgetFactory>();
            builder.Services.AddSingleton<IWidgetInvokerFactory, DefaultWidgetInvokerFactory>();
            builder.Services.AddSingleton<IWidgetArgumentBinder, DefaultWidgetArgumentBinder>();
            builder.Services.AddSingleton<WidgetResultExecutor, WidgetResultExecutor>();

            builder.Services.AddTransient<IWidgetDescriptorProvider, DefaultWidgetDescriptorProvider>();
            builder.Services.AddTransient<IWidgetHelper, DefaultWidgetHelper>();

            builder.PartManager.FeatureProviders.Add(new WidgetFeatureProvider());

            return builder;
        }
    }
}