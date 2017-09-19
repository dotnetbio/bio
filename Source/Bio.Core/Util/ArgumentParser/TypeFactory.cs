using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;

using Bio.Extensions;

namespace Bio.Util.ArgumentParser
{
    //!! This code is largely taken from MatrixFactory
    /// <summary>
    /// Type Factory.
    /// </summary>
    public static class TypeFactory
    {
        /// <summary>
        /// All Referenced Assemblies.
        /// </summary>
        private static IEnumerable<Assembly> allReferencedAssemblies;

        /// <summary>
        /// All Referenced Assemblies.
        /// </summary>
        private static IEnumerable<Assembly> AllReferencedAssemblies
        {
            get
            {
                if (allReferencedAssemblies == null)
                {
                    var userAssemblies = EnumerateAllUserAssemblyCodeBases().ToHashSet();
                    var systemAssemblies = EnumerateReferencedSystemAssemblies(userAssemblies).ToHashSet();
                    allReferencedAssemblies = userAssemblies.Union(systemAssemblies);
                }
                return allReferencedAssemblies;
            }
        }

        /// <summary>
        /// Type Name And BaseName To Type.
        /// </summary>
        static readonly ConcurrentDictionary<Tuple<string, Type>, Type> TypeNameAndBaseNameToType = new ConcurrentDictionary<Tuple<string, Type>, Type>();

        /// <summary>
        /// Returns the first type in any of the referenced assemblies that matches the type name. If typeName includes the namespace, 
        /// then matches on the fully qualified name. Else, looks for the first type in any of namespaces
        /// that matches typeName AND is a subtype of baseType (use typeof(object) as a default).
        /// </summary>
        /// <param name="typeName">The type name we're searching for. May either be fully qualified or contain only the class name</param>
        /// <param name="baseType">Will only return a type that is a subtype of baseType</param>
        /// <param name="returnType">The type matching typeName, if found, or null.</param>
        /// <returns>true iff the typeName could be resolved into a type.</returns>
        public static bool TryGetType(string typeName, Type baseType, out Type returnType)
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException("typeName");
            if (baseType == null)
                throw new ArgumentNullException("baseType");

            var result = TypeNameAndBaseNameToType.GetOrAdd(Tuple.Create(typeName.ToLower(), baseType), typeAndBase =>
            {
                Type foundType;
                return TryGetTypeInternal(typeAndBase.Item1, typeAndBase.Item2, out foundType) ? foundType : null;
            });

            returnType = result;
            return result != null;
        }

        /// <summary>
        /// Try Get Type Internal.
        /// </summary>
        /// <param name="typeName">Type Name.</param>
        /// <param name="baseType">Base type.</param>
        /// <param name="returnType">Return type.</param>
        /// <returns>True if Gets type internal.</returns>
        private static bool TryGetTypeInternal(string typeName, Type baseType, out Type returnType)
        {
            // rename the built-int shortcuts.
            switch (typeName.ToLower())
            {
                case "int":
                case "int32":
                    returnType = typeof(int);
                    return true;
                case "long":
                case "int64":
                    returnType = typeof(long);
                    return true;
                case "bool":
                case "Boolean":
                    returnType = typeof(bool);
                    return true;
                case "string":
                    returnType = typeof(string);
                    return true;
                case "double":
                    returnType = typeof(double);
                    return true;
                default:
                    break;
            }

            returnType = null;
            Type[] genericTypes;

            if (!TryGetGenericParameters(ref typeName, out genericTypes))
                return false;

            foreach (Assembly assembly in AllReferencedAssemblies)
            {
                returnType = GetType(assembly, baseType, typeName, genericTypes);

                if (returnType != null)
                {
                    //if (genericTypes != null)
                    //{
                    //    returnType = returnType.MakeGenericType(genericTypes);
                    //}
                    return true;
                }
            }



            returnType = null;
            return false;
        }

        /// <summary>
        /// Returns the first type in assembly that matches the type name. If typeName includes the namespace, 
        /// then matches on the fully qualified name. Else, looks for the first type in any of Assembly's namespaces
        /// that matches typeName
        /// </summary>
        /// <param name="assembly">The assembly in which to search</param>
        /// <param name="baseType">Will only return a type that is a subtype of baseType</param>
        /// <param name="typeName">The type name we're searching for. May either be fully qualified or contain only the class name</param>
        /// <param name="genericTypes">genericTypes</param>
        /// <returns>The type, if found, or null.</returns>
        private static Type GetType(Assembly assembly, Type baseType, string typeName, Type[] genericTypes)
        {
            //SpecialFunctions.CheckDate(2010, 4, 13);
            //if (assembly.FullName.StartsWith("ShoViz"))
            //{
            //    return null;
            //}
            try
            {
                // if it's a fully qualified name (with namespace), then use the built in search
                if (typeName.Contains('.'))
                {
                    return assembly.GetType(typeName, throwOnError: false
#if !SILVERLIGHT
, ignoreCase: true);
#else
                        );
#endif
                }

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Type result = type;
                        if (null != genericTypes)
                            result = type.MakeGenericType(genericTypes);

                        if (result.IsSubclassOfOrImplements(baseType))
                            return result;
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// Get Referenced Types.
        /// </summary>
        /// <returns>List of type.</returns>
        public static IEnumerable<Type> GetReferencedTypes()
        {
            foreach (Assembly assembly in AllReferencedAssemblies)
                foreach (Type type in GetAssemblyTypes(assembly))
                    yield return type;

        }

        /// <summary>
        /// Get Assembly Types.
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns>List of type.</returns>
        private static IEnumerable<Type> GetAssemblyTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                // a problem with an assembly. just ignore
            }
            return new Type[0];
        }

        /// <summary>
        /// Checks to see if the type name is generic. If so, modifies the typeName and tries to construct the generic type arguments. 
        /// </summary>
        /// <param name="typeName">The type name. Will be modified if generic.</param>
        /// <param name="genericTypes">List of generic types. Null if this type is not generic.</param>
        /// <returns>True if there was no problem parsing. False is there is a problem.</returns>
        private static bool TryGetGenericParameters(ref string typeName, out Type[] genericTypes)
        {
            genericTypes = null;

            int firstIdx = typeName.IndexOf('<');
            if (firstIdx < 0)
                return true;

            int lastIdx = typeName.LastIndexOf('>');
            Helper.CheckCondition(lastIdx == typeName.Length - 1, "Unbalanced <>");

            string typeListString = typeName.Substring(firstIdx + 1, lastIdx - firstIdx - 1);
            typeName = typeName.Substring(0, firstIdx);
            List<Type> genericTypesAsList = new List<Type>();

            IEnumerable<string> typeArgs;
            try { typeArgs = typeListString.ProtectedSplit('<', '>', false, ','); }
            catch (Exception) { return false; }

            foreach (string typeArgument in typeArgs)
            {
                Type genericArgumentType;
                if (!TryGetType(typeArgument, typeof(object), out genericArgumentType))
                    return false;
                genericTypesAsList.Add(genericArgumentType);
            }
            typeName += "`" + genericTypesAsList.Count;
            genericTypes = genericTypesAsList.ToArray();

            return true;
        }

        /// <summary>
        /// Enumerate AllUser Assembly CodeBases.
        /// </summary>
        /// <returns>List of Assembly.</returns>
        private static IEnumerable<Assembly> EnumerateAllUserAssemblyCodeBases()
        {
            Assembly entryAssembly = SpecialFunctions.GetEntryOrCallingAssembly();

            yield return entryAssembly;

#if SILVERLIGHT
            yield break;
#else

            string exePath = Path.GetDirectoryName(entryAssembly.Location);
            Assembly assembly;
            foreach (string dllName in Directory.EnumerateFiles(exePath, "*.dll").Union(Directory.EnumerateFiles(exePath, "*.exe")))
            {
                assembly = null;
                try
                {
                    assembly = Assembly.LoadFrom(dllName);
                }
                catch
                {
                }
                if (assembly != null)
                {
                    yield return assembly;
                }
            }
#endif
        }

        /// <summary>
        /// Enumerate Referenced SystemAssemblies.
        /// </summary>
        /// <param name="userAssemblies">User Assemblies.</param>
        /// <returns></returns>
        private static IEnumerable<Assembly> EnumerateReferencedSystemAssemblies(IEnumerable<Assembly> userAssemblies)
        {
#if SILVERLIGHT
            yield break;
#else
            HashSet<string> alreadySeen = new HashSet<string>();
            foreach (Assembly userAssembly in userAssemblies)
            {
                if (!alreadySeen.Contains(userAssembly.FullName))
                {
                    alreadySeen.Add(userAssembly.FullName);
                    foreach (AssemblyName assemblyName in userAssembly.GetReferencedAssemblies())
                    {
                        // SpecialFunctions.CheckDate(2010, 4, 13);
                        if (assemblyName.FullName.StartsWith("System.Windows.Forms.DataVisualization"))
                        {
                            continue; //not break;
                        }

                        if (assemblyName.FullName.StartsWith("System") || assemblyName.FullName.StartsWith("mscorlib"))
                        {
                            Assembly systemAssembly = null;
                            try
                            {
                                systemAssembly = Assembly.Load(assemblyName);
                            }
                            catch { }
                            if (systemAssembly != null)
                                yield return systemAssembly;

                        }
                    }
                }
            }
#endif
        }
    }
}
