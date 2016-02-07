﻿namespace Antaris.AspNetCore.Mvc.Widgets.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNet.Mvc.Infrastructure;
    using Antaris.AspNetCore.Mvc.Widgets.Internal;

    using StateMethodDictionary = System.Collections.Generic.Dictionary<string, System.Reflection.MethodInfo>;
    using System.Threading.Tasks;
    /// <summary>
    /// Provides a default implementation of a widget descriptor provider.
    /// </summary>
    public class DefaultWidgetDescriptorProvider : IWidgetDescriptorProvider
    {
        private readonly IAssemblyProvider _assemblyProvider;
        private const string MethodPrefix = "Invoke";
        private const string AsyncMethodSuffix = "Async";
        private const string MethodGetPattern = "Get";
        private const string MethodPostPattern = "Post";

        /// <summary>
        /// Initialises a new instance of <see cref="DefaultWidgetDescriptorProvider"/>.
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        public DefaultWidgetDescriptorProvider(IAssemblyProvider assemblyProvider)
        {
            if (assemblyProvider == null)
            {
                throw new ArgumentNullException(nameof(assemblyProvider));
            }

            _assemblyProvider = assemblyProvider;
        }

        /// <inheritdoc />
        public IEnumerable<WidgetDescriptor> GetWidgets()
        {
            var types = GetCandidateTypes();

            return types.Where(t => WidgetConventions.IsWidget(t)).Select(CreateDescriptor);
        }

        /// <summary>
        /// Gets the candidate types.
        /// </summary>
        /// <returns>The set of candidate types.</returns>
        protected virtual IEnumerable<TypeInfo> GetCandidateTypes()
        {
            var assembles = _assemblyProvider.CandidateAssemblies;

            return assembles.SelectMany(a => a.ExportedTypes).Select(t => t.GetTypeInfo());
        }

        /// <summary>
        /// Creates a descriptor for the given widget type.
        /// </summary>
        /// <param name="typeInfo">The widget type.</param>
        /// <returns>The widget descriptor.</returns>
        private static WidgetDescriptor CreateDescriptor(TypeInfo typeInfo)
        {
            var descriptor = new WidgetDescriptor
            {
                FullName = WidgetConventions.GetWidgetFullName(typeInfo),
                ShortName = WidgetConventions.GetWidgetName(typeInfo),
                TypeInfo = typeInfo
            };

            var methods = FindMethods(typeInfo);
            descriptor.GetMethods = methods.Item1;
            descriptor.PostMethods = methods.Item2;

            return descriptor;
        }

        /// <summary>
        /// Finds potential methods that can be executed by the widget.
        /// </summary>
        /// <param name="typeInfo">The widget typeinfo.</param>
        /// <returns>The get and post methods.</returns>
        private static Tuple<StateMethodDictionary, StateMethodDictionary> FindMethods(TypeInfo typeInfo)
        {
            var get = new StateMethodDictionary();
            var post = new StateMethodDictionary();

            var methods = typeInfo.DeclaredMethods
                .Where(m => m.Name.StartsWith(MethodPrefix, StringComparison.Ordinal) && !m.IsStatic && m.IsPublic)
                .ToArray();

            /*
             * Possible method names ('Form' as example state):
             *
             * Invoke - Executable through both GET and POST
             * InvokeGet
             * InvokePost
             * InvokeForm - Executable through both GET and POST
             * InvokeFormGet
             * InvokeFormPost
             * InvokeAsync - Executable through both GET and POST
             * InvokeGetAsync
             * InvokePostAsync
             * InvokeFormAsync - Executable through both GET and POST
             * InvokeFormGetAsync
             * InvokeFormPostAsync
             */

            for (int i = 0; i < methods.Length; i++)
            {
                bool? isGet = null;
                var method = methods[i];
                string name = method.Name.Substring(MethodPrefix.Length);
                string state = string.Empty;

                bool isAsync = method.Name.EndsWith(AsyncMethodSuffix, StringComparison.Ordinal);

                if (isAsync)
                {
                    name = name.Substring(0, name.Length - AsyncMethodSuffix.Length);
                }

                if (name.EndsWith(MethodGetPattern, StringComparison.Ordinal))
                {
                    isGet = true;
                    name = name.Substring(0, name.Length - MethodGetPattern.Length);
                }
                else if (name.EndsWith(MethodPostPattern, StringComparison.Ordinal))
                {
                    isGet = false;
                    name = name.Substring(0, name.Length - MethodPostPattern.Length);
                }

                ValidateMethod(method, isAsync);

                state = name;
                if (isGet == null || isGet == true)
                {
                    get.Add(state, method);
                }
                
                if (isGet == null || isGet == false)
                {
                    post.Add(state, method);
                }
            }

            return Tuple.Create(get, post);
        }
        
        /// <summary>
        /// Validates a widget method.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="isAsync">True if the method is an async method, otherwise false.</param>
        private static void ValidateMethod(MethodInfo methodInfo, bool isAsync)
        {
            if (isAsync)
            {
                if (!methodInfo.ReturnType.GetTypeInfo().IsGenericType ||
                    methodInfo.ReturnType.GetGenericTypeDefinition() != typeof(Task<>))
                {
                    throw new InvalidOperationException($"The async method '{methodInfo.Name}' of widget type shoud return a task.");
                }
            }
            else
            {
                if (methodInfo.ReturnType == typeof(void))
                {
                    throw new InvalidOperationException($"The sync method '{methodInfo.Name}' should return a value.");
                }
                else if (methodInfo.ReturnType.IsAssignableFrom(typeof(Task)))
                {
                    throw new InvalidOperationException($"The sync method '{methodInfo.Name}' should not return a task.");
                }
            }
        }
    }
}