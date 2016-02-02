namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.ModelBinding.Validation;
    using Microsoft.AspNet.Routing;
    using Microsoft.Extensions.OptionsModel;

    /// <summary>
    /// Provides a default implementation of a widget argument binder.
    /// </summary>
    public class DefaultWidgetArgumentBinder : IWidgetArgumentBinder
    {
        private readonly MvcOptions _options;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IObjectModelValidator _objectModelValidator;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetArgumentBinder"/>.
        /// </summary>
        /// <param name="options">The options accessor for <see cref="MvcOptions"/></param>
        /// <param name="modelMetadataProvider">The model metadata provider.</param>
        /// <param name="objectModelValidator">The object model validator.</param>
        public DefaultWidgetArgumentBinder(IOptions<MvcOptions> options, IModelMetadataProvider modelMetadataProvider, IObjectModelValidator objectModelValidator)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (modelMetadataProvider == null)
            {
                throw new ArgumentNullException(nameof(modelMetadataProvider));
            }

            if (objectModelValidator == null)
            {
                throw new ArgumentNullException(nameof(objectModelValidator));
            }
            
            _options = options.Value;
            _modelMetadataProvider = modelMetadataProvider;
            _objectModelValidator = objectModelValidator;
        }

        /// <inheritdocs />
        public async Task<IDictionary<string, object>> BindArgumentsAsync(WidgetContext context, MethodInfo method, IDictionary<string, object> values)
        {
            var bindingContext = new OperationBindingContext
            {
                HttpContext = context.ViewContext.HttpContext,
                InputFormatters = _options.InputFormatters,
                MetadataProvider = _modelMetadataProvider,
                ModelBinder = new CompositeModelBinder(_options.ModelBinders),
                ValidatorProvider = new CompositeModelValidatorProvider(_options.ModelValidatorProviders),
                ValueProvider = await CompositeValueProvider.CreateAsync(_options.ValueProviderFactories, new ValueProviderFactoryContext(context.ViewContext.HttpContext, context.ViewContext.RouteData.Values))
            };

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
                    var attribute = parameter.GetCustomAttributes().OfType<IBindingSourceMetadata>().FirstOrDefault();
                    var bindingInfo = new BindingInfo()
                    {
                        BindingSource = attribute?.BindingSource
                    };

                    var metadata = _modelMetadataProvider.GetMetadataForType(parameter.ParameterType);
                    var modelBindingContext = ModelBindingContext.CreateBindingContext(
                        bindingContext,
                        context.ModelState,
                        metadata,
                        bindingInfo,
                        parameter.Name);

                    var result = await bindingContext.ModelBinder.BindModelAsync(modelBindingContext);
                    if (result.IsModelSet)
                    {
                        _objectModelValidator.Validate(bindingContext.ValidatorProvider, context.ModelState, modelBindingContext.ValidationState, result.Key, result.Model);
                        arguments.Add(parameter.Name, result.Model);
                    }
                    else
                    {
                        arguments.Add(parameter.Name, Activator.CreateInstance(parameter.ParameterType));
                    }
                }
            }

            return arguments;
        }
    }
}
