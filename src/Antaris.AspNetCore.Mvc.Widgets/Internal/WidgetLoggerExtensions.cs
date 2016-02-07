namespace Antaris.AspNetCore.Mvc.Widgets.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.AspNet.Mvc.ViewFeatures;
    using Microsoft.AspNet.Mvc.ViewEngines;
    using Microsoft.Extensions.Logging;
    using Infrastructure;
    internal static class WidgetsLoggerExtensions
    {
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
        private static readonly string[] EmptyArguments = new string[0];

        private static readonly Action<ILogger, string, string[], Exception> _widgetExecuting;
        private static readonly Action<ILogger, string, double, string, Exception> _widgetExecuted;

        private static readonly Action<ILogger, string, Exception> _partialViewFound;
        private static readonly Action<ILogger, string, IEnumerable<string>, Exception> _partialViewNotFound;
        private static readonly Action<ILogger, string, Exception> _partialViewResultExecuting;

        private static readonly Action<ILogger, string, Exception> _antiforgeryTokenInvalid;

        private static readonly Action<ILogger, string, Exception> _widgetResultExecuting;

        private static readonly Action<ILogger, string, Exception> _viewResultExecuting;
        private static readonly Action<ILogger, string, Exception> _viewFound;
        private static readonly Action<ILogger, string, IEnumerable<string>, Exception> _viewNotFound;

        static WidgetsLoggerExtensions()
        {
            _widgetExecuting = LoggerMessage.Define<string, string[]>(
                LogLevel.Debug,
                1,
                "Executing view component {WidgetName} with arguments ({Arguments}).");

            _widgetExecuted = LoggerMessage.Define<string, double, string>(
                LogLevel.Debug,
                2,
                "Executed view component {WidgetName} in {ElapsedMilliseconds}ms and returned " +
                "{WidgetResult}");

            _partialViewResultExecuting = LoggerMessage.Define<string>(
                LogLevel.Information,
                1,
                "Executing PartialViewResult, running view at path {Path}.");

            _partialViewFound = LoggerMessage.Define<string>(
                LogLevel.Debug,
                2,
                "The partial view '{PartialViewName}' was found.");

            _partialViewNotFound = LoggerMessage.Define<string, IEnumerable<string>>(
                LogLevel.Error,
                3,
                "The partial view '{PartialViewName}' was not found. Searched locations: {SearchedViewLocations}");

            _antiforgeryTokenInvalid = LoggerMessage.Define<string>(
                LogLevel.Information,
                1,
                "Antiforgery token validation failed. {Message}");

            _widgetResultExecuting = LoggerMessage.Define<string>(
                LogLevel.Information,
                1,
                "Executing WidgetResult, running {WidgetName}.");

            _viewResultExecuting = LoggerMessage.Define<string>(
                LogLevel.Information,
                1,
                "Executing ViewResult, running view at path {Path}.");

            _viewFound = LoggerMessage.Define<string>(
                LogLevel.Debug,
                2,
                "The view '{ViewName}' was found.");

            _viewNotFound = LoggerMessage.Define<string, IEnumerable<string>>(
                LogLevel.Error,
                3,
                "The view '{ViewName}' was not found. Searched locations: {SearchedViewLocations}");
        }

        public static IDisposable WidgetScope(this ILogger logger, WidgetContext context)
        {
            return logger.BeginScopeImpl(new WidgetLogScope(context.WidgetDescriptor));
        }

        public static void WidgetExecuting(
            this ILogger logger,
            WidgetContext context,
            object[] arguments)
        {
            var formattedArguments = GetFormattedArguments(arguments);
            _widgetExecuting(logger, context.WidgetDescriptor.DisplayName, formattedArguments, null);
        }

        private static string[] GetFormattedArguments(object[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
            {
                return EmptyArguments;
            }

            var formattedArguments = new string[arguments.Length];
            for (var i = 0; i < formattedArguments.Length; i++)
            {
                formattedArguments[i] = Convert.ToString(arguments[i]);
            }

            return formattedArguments;
        }

        public static void WidgetExecuted(
            this ILogger logger,
            WidgetContext context,
            long startTimestamp,
            object result)
        {
            // Don't log if logging wasn't enabled at start of request as time will be wildly wrong.
            if (startTimestamp != 0)
            {
                var currentTimestamp = Stopwatch.GetTimestamp();
                var elapsed = new TimeSpan((long)(TimestampToTicks * (currentTimestamp - startTimestamp)));

                _widgetExecuted(
                    logger,
                    context.WidgetDescriptor.DisplayName,
                    elapsed.TotalMilliseconds,
                    Convert.ToString(result),
                    null);
            }
        }

        public static void PartialViewFound(
            this ILogger logger,
            string partialViewName)
        {
            _partialViewFound(logger, partialViewName, null);
        }

        public static void PartialViewNotFound(
            this ILogger logger,
            string partialViewName,
            IEnumerable<string> searchedLocations)
        {
            _partialViewNotFound(logger, partialViewName, searchedLocations, null);
        }

        public static void PartialViewResultExecuting(this ILogger logger, IView view)
        {
            _partialViewResultExecuting(logger, view.Path, null);
        }

        public static void AntiforgeryTokenInvalid(this ILogger logger, string message, Exception exception)
        {
            _antiforgeryTokenInvalid(logger, message, exception);
        }

        public static void WidgetResultExecuting(this ILogger logger, string WidgetName)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                _widgetResultExecuting(logger, WidgetName, null);
            }
        }

        public static void WidgetResultExecuting(this ILogger logger, Type WidgetType)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                _widgetResultExecuting(logger, WidgetType.Name, null);
            }
        }

        public static void ViewResultExecuting(this ILogger logger, IView view)
        {
            _viewResultExecuting(logger, view.Path, null);
        }

        public static void ViewFound(this ILogger logger, string viewName)
        {
            _viewFound(logger, viewName, null);
        }

        public static void ViewNotFound(this ILogger logger, string viewName,
            IEnumerable<string> searchedLocations)
        {
            _viewNotFound(logger, viewName, searchedLocations, null);
        }

        private class WidgetLogScope : IReadOnlyList<KeyValuePair<string, object>>
        {
            private readonly WidgetDescriptor _descriptor;

            public WidgetLogScope(WidgetDescriptor descriptor)
            {
                _descriptor = descriptor;
            }

            public KeyValuePair<string, object> this[int index]
            {
                get
                {
                    if (index == 0)
                    {
                        return new KeyValuePair<string, object>("WidgetName", _descriptor.DisplayName);
                    }
                    else if (index == 1)
                    {
                        return new KeyValuePair<string, object>("WidgetId", _descriptor.Id);
                    }
                    throw new IndexOutOfRangeException(nameof(index));
                }
            }

            public int Count
            {
                get
                {
                    return 2;
                }
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                for (int i = 0; i < Count; ++i)
                {
                    yield return this[i];
                }
            }

            public override string ToString()
            {
                return _descriptor.DisplayName;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}

