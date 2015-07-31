using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bio.Registration
{
    /// <summary>
    /// This attribute identifies a registered part (alphabet, algorithm, parser, formatter)
    /// The declared type must implement the proper interface to be identified correctly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class BioRegisterAttribute : Attribute
    {
        /// <summary>
        /// The registered type.
        /// </summary>
        public Type DeclaredType { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="declaredType">Declared type</param>
        public BioRegisterAttribute(Type declaredType)
        {
            DeclaredType = declaredType;
        }
    }

    /// <summary>
    /// This is used to lookup registered parts for .NET Bio 2.0
    /// </summary>
    public static class BioRegistrationService
    {
        private static List<BioRegisterAttribute> locatedParts;

        /// <summary>
        /// Locates the registered parts for .NET Bio 2.0
        /// </summary>
        /// <typeparam name="T">Type to look for.</typeparam>
        /// <returns>Enumerable of the given types.</returns>
        public static IEnumerable<Type> LocateRegisteredParts<T>()
        {
            if (locatedParts == null)
            {
                locatedParts = new List<BioRegisterAttribute>();
                var assemblies = PlatformManager.Services.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    locatedParts.AddRange(assembly.GetCustomAttributes<BioRegisterAttribute>());
                }
            }

            return
                locatedParts.Where(part => typeof(T).GetTypeInfo().IsAssignableFrom(part.DeclaredType.GetTypeInfo()))
                    .Select(part => part.DeclaredType);
        }
    }
}
