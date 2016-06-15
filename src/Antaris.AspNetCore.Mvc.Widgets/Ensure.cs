namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System;

    /// <summary>
    /// Provides validation methods for values.
    /// </summary>
    public class Ensure
    {
        const string ParameterNullOrWhiteSpaceFormat = "The parameter '{0}' cannot be null, empty or white space";
        const string ParameterNullOrEmptyFormat = "The parameter '{0}' cannot be null or empty";

        /// <summary>
        /// Ensures the given argument value is not null.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        /// <param name="name">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        /// <returns>The argument value</returns>
        public static T ArgumentNotNull<T>(T argument, string name) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name);
            }

            return argument;
        }

        /// <summary>
        /// Ensures the given argument value is not null or empty.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        /// <param name="name">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument is null or empty.</exception>
        /// <returns>The argument value</returns>
        public static string ArgumentNotNullOrEmpty(string argument, string name)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException(string.Format(ParameterNullOrEmptyFormat, name), name);
            }

            return argument;
        }
    }
}
