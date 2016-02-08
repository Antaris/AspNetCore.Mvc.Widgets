namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Diagnostics;
    using Microsoft.AspNet.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides a default implementation of a widget invoker factory.
    /// </summary>
    public class DefaultWidgetInvokerFactory : IWidgetInvokerFactory
    {
        private readonly IWidgetFactory _widgetFactory;
        private readonly IWidgetArgumentBinder _argumentBinder;
        private readonly DiagnosticSource _diagnosticSource;
        private readonly ILogger _logger;

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetInvokerFactory"/>.
        /// </summary>
        /// <param name="widgetFactory">The factory used to create widget instances.</param>
        /// <param name="argumentBinder">The argument binder.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="loggerFactory">The factory used to create logger instances.</param>
        public DefaultWidgetInvokerFactory(
            IWidgetFactory widgetFactory,
            IWidgetArgumentBinder argumentBinder,
            DiagnosticSource diagnosticSource,
            ILoggerFactory loggerFactory)
        {
            if (widgetFactory == null)
            {
                throw new ArgumentNullException(nameof(widgetFactory));
            }

            if (argumentBinder == null)
            {
                throw new ArgumentNullException(nameof(argumentBinder));
            }

            if (diagnosticSource == null)
            {
                throw new ArgumentNullException(nameof(diagnosticSource));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _widgetFactory = widgetFactory;
            _argumentBinder = argumentBinder;
            _diagnosticSource = diagnosticSource;

            _logger = loggerFactory.CreateLogger<DefaultWidgetInvoker>();
        }

        /// <inheritdoc />
        public IWidgetInvoker CreateInstance(WidgetContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new DefaultWidgetInvoker(_widgetFactory, _argumentBinder, _diagnosticSource, _logger);
        }
    }
}
