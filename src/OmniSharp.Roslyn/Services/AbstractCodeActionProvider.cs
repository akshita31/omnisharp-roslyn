using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeRefactorings;

namespace OmniSharp.Services
{
    public abstract class AbstractCodeActionProvider : ICodeActionProvider
    {
        public string ProviderName { get; }
        public IEnumerable<CodeRefactoringProvider> Refactorings { get; }
        public IEnumerable<CodeFixProvider> CodeFixes { get; }

        public virtual IEnumerable<Assembly> Assemblies { get; protected set; }

        protected AbstractCodeActionProvider(string providerName, ImmutableArray<Assembly> assemblies)
        {
            ProviderName = providerName;

            this.Assemblies = assemblies;

            var features = this.Assemblies
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => !type.GetTypeInfo().IsInterface &&
                               !type.GetTypeInfo().IsAbstract &&
                               !type.GetTypeInfo().ContainsGenericParameters));
            // TODO: handle providers with generic params

            this.Refactorings = features
                .Where(t => typeof(CodeRefactoringProvider).IsAssignableFrom(t))
                .Select(type => CreateInstance<CodeRefactoringProvider>(type))
                .Where(instance => instance != null);

            this.CodeFixes = features
                .Where(t => typeof(CodeFixProvider).IsAssignableFrom(t))
                .Select(type => CreateInstance<CodeFixProvider>(type))
                .Where(instance => instance != null);
        }

        private T CreateInstance<T>(Type type) where T : class
        {
            try
            {
                var defaultCtor = type.GetConstructor(new Type[] { });

                return defaultCtor != null
                    ? (T)Activator.CreateInstance(type)
                    : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create instrance of {type.FullName} in {type.AssemblyQualifiedName}.", ex);
            }
        }
    }
}

