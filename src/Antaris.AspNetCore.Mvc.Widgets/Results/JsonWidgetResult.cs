namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.OptionsModel;
    using Newtonsoft.Json;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Renders a JSON result from a widget.
    /// </summary>
    public class JsonWidgetResult : IWidgetResult
    {
        private readonly JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Initialises a new instance of <see cref="JsonWidgetResult"/>.
        /// </summary>
        /// <param name="value">The value to serialize as JSON.</param>
        /// <param name="serializerSettings">[Optional] The serializer settings.</param>
        public JsonWidgetResult(object value, JsonSerializerSettings serializerSettings = null)
        {
            Value = value;
            _serializerSettings = serializerSettings;
        }

        /// <summary>
        /// Gets the value to serialize as JSON.
        /// </summary>
        public object Value { get; }

        /// <inheritdoc />
        public void Execute(WidgetContext context)
        {
            var serializerSettings = _serializerSettings;
            if (serializerSettings == null)
            {
                serializerSettings = context.ViewContext.HttpContext.RequestServices.GetRequiredService<IOptions<MvcJsonOptions>>().Value.SerializerSettings;
            }

            using (var jsonWriter = new JsonTextWriter(context.Writer))
            {
                jsonWriter.CloseOutput = false;

                var jsonSerializer = JsonSerializer.Create(serializerSettings);
                jsonSerializer.Serialize(jsonWriter, Value);
            }
        }

        /// <inheritdoc />
        public Task ExecuteAsync(WidgetContext context)
        {
            Execute(context);
            return Task.FromResult(true);
        }
    }
}
