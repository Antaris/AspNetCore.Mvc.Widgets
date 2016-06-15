namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Provides a default implementation of a widget argument binder.
    /// </summary>
    public class DefaultWidgetArgumentBinder : IWidgetArgumentBinder
    {
        private readonly MvcOptions _options;
        private readonly IModelBinderFactory _modelBinderFactory;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IObjectModelValidator _objectModelValidator;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetArgumentBinder"/>.
        /// </summary>
        /// <param name="options">The options accessor for <see cref="MvcOptions"/></param>
        /// <param name="modelBinderFactory">The model binder factory.</param>
        /// <param name="modelMetadataProvider">The model metadata provider.</param>
        /// <param name="objectModelValidator">The object model validator.</param>
        public DefaultWidgetArgumentBinder(IOptions<MvcOptions> options, IModelBinderFactory modelBinderFactory, IModelMetadataProvider modelMetadataProvider, IObjectModelValidator objectModelValidator)
        {
            _options = Ensure.ArgumentNotNull(options, nameof(options)).Value;
            _modelBinderFactory = Ensure.ArgumentNotNull(modelBinderFactory, nameof(modelBinderFactory));
            _modelMetadataProvider = Ensure.ArgumentNotNull(modelMetadataProvider, nameof(modelMetadataProvider));
            _objectModelValidator = Ensure.ArgumentNotNull(objectModelValidator, nameof(objectModelValidator));
        }

        /// <inheritdocs />
        public async Task<IDictionary<string, object>> BindArgumentsAsync(WidgetContext context, MethodInfo method, IDictionary<string, object> values)
        {
            var actionContext = new ActionContext(
                context.ViewContext.HttpContext,
                context.ViewContext.RouteData,
                new ActionDescriptor());
            var valueProvider = await CreateValueProvider(context, actionContext);

            var arguments = new Dictionary<string, object>(StringComparer.Ordinal);
            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                if (values.ContainsKey(parameter.Name))
                {
                    arguments.Add(parameter.Name, values[parameter.Name]);
                }
                else
                {
                    var result = await BindModelAsync(parameter, context, valueProvider, actionContext);
                    if (result != null && result.Value.IsModelSet)
                    {
                        arguments.Add(parameter.Name, result.Value.Model);
                    }
                    else
                    {
                        arguments.Add(parameter.Name, Activator.CreateInstance(parameter.ParameterType));
                    }
                }
            }

            return arguments;
        }

        public async Task<ModelBindingResult?> BindModelAsync(
            ParameterInfo parameter,
            WidgetContext widgetContext,
            IValueProvider valueProvider,
            ActionContext actionContext)
        {
            var attribute = parameter.GetCustomAttributes().OfType<IBindingSourceMetadata>().FirstOrDefault();
            var bindingInfo = new BindingInfo()
            {
                BindingSource = attribute?.BindingSource
            };
            var operationContext = new OperationBindingContext()
            {
                ActionContext = actionContext,
                InputFormatters = _options.InputFormatters,
                MetadataProvider = _modelMetadataProvider,
                ValidatorProvider = new CompositeModelValidatorProvider(_options.ModelValidatorProviders),
                ValueProvider = valueProvider
            };

            var metadata = _modelMetadataProvider.GetMetadataForType(parameter.ParameterType);
            var binder = _modelBinderFactory.CreateBinder(new ModelBinderFactoryContext()
            {
                BindingInfo = bindingInfo,
                Metadata = metadata,
                CacheToken = parameter,
            });

            var modelBindingContext = DefaultModelBindingContext.CreateBindingContext(
                operationContext,
                metadata,
                bindingInfo,
                parameter.Name);

            var parameterModelName = bindingInfo?.BinderModelName ?? metadata.BinderModelName;
            if (parameterModelName != null)
            {
                // The name was set explicitly, always use that as the prefix.
                modelBindingContext.ModelName = parameterModelName;
            }
            else if (modelBindingContext.ValueProvider.ContainsPrefix(parameter.Name))
            {
                // We have a match for the parameter name, use that as that prefix.
                modelBindingContext.ModelName = parameter.Name;
            }
            else
            {
                // No match, fallback to empty string as the prefix.
                modelBindingContext.ModelName = string.Empty;
            }

            await binder.BindModelAsync(modelBindingContext);

            var modelBindingResult = modelBindingContext.Result;
            if (modelBindingResult != null && modelBindingResult.Value.IsModelSet)
            {
                _objectModelValidator.Validate(
                    actionContext,
                    operationContext.ValidatorProvider,
                    modelBindingContext.ValidationState,
                    modelBindingResult.Value.Key,
                    modelBindingResult.Value.Model);
            }

            return modelBindingResult;
        }

        private async Task<CompositeValueProvider> CreateValueProvider(WidgetContext context, ActionContext actionContext)
        {
            var valueProviderContext = new ValueProviderFactoryContext(actionContext);
            var factories = _options.ValueProviderFactories;

            for (int i = 0; i < factories.Count; i++)
            {
                var factory = factories[i];
                await factory.CreateValueProviderAsync(valueProviderContext);
            }

            return new CompositeValueProvider(valueProviderContext.ValueProviders);
        }
    }
}
