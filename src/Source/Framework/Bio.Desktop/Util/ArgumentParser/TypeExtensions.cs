using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// Type Extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// To Type String.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>To type String.</returns>
        public static string ToTypeString(this Type type)
        {
            if (type == null)
                return "null";

            if (type.Name == "Int32") return "int";
            if (type.Name == "Int64") return "long";
            if (type.Name == "Boolean") return "bool";

            StringBuilder typeString = new StringBuilder(type.Name);
            if (type.IsGenericType)
            {
                typeString.Remove(typeString.Length - 2, 2);
                typeString.Append('<');
                typeString.Append(type.GetGenericArguments().Select(genericType => ToTypeString(genericType)).StringJoin(","));
                typeString.Append('>');
            }
            return typeString.ToString();
        }

        /// <summary>
        /// Get Enum Names.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Array of Enum Names.</returns>
        public static string[] GetEnumNames(this Type type)
        {
            if (type == null)
                return null;

            Helper.CheckCondition(type.IsEnum, "{0} is not an enum type.", type);
#if !SILVERLIGHT
            return Enum.GetNames(type);
#else
            return type.GetFields(BindingFlags.Public | BindingFlags.Static).Select(x => x.Name).ToArray();
#endif
        }

        /// <summary>
        /// Get Implementing Types.
        /// </summary>
        /// <param name="interfaceType">Interface type.</param>
        /// <returns>List of type.</returns>
        public static IEnumerable<Type> GetImplementingTypes(this Type interfaceType)
        {
            if (!interfaceType.IsInterface) throw new ParseException("type {0} is not an interface", interfaceType);
            string interfaceName = interfaceType.Name;
            foreach (Type t in TypeFactory.GetReferencedTypes())
            {
                if (t.IsPublic && t.GetInterface(interfaceName, ignoreCase: true) != null)
                    yield return t;
            }
        }

        /// <summary>
        /// Get Derived Types.
        /// </summary>
        /// <param name="classType">Class Type.</param>
        /// <returns>List of type.</returns>
        public static IEnumerable<Type> GetDerivedTypes(this Type classType)
        {
            foreach (Type t in TypeFactory.GetReferencedTypes())
            {
                if (t.IsPublic && t.IsSubclassOf(classType))
                    yield return t;
            }
        }

        /// <summary>
        /// Implements.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="interfaceType">InterFace type.</param>
        /// <returns>True if Implements.</returns>
        public static bool Implements(this Type type, Type interfaceType)
        {
            if (type == null || interfaceType == null)
                return false;

            return type.GetInterface(interfaceType.Name, ignoreCase: false) != null;
        }

        /// <summary>
        /// Is Subclass Of Or Implements.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="baseType">Base Type.</param>
        /// <returns>True if Subclass Of Or Implements.</returns>
        public static bool IsSubclassOfOrImplements(this Type type, Type baseType)
        {
            if (type == null || baseType == null)
                return false;

            return baseType.IsInterface ? type.Implements(baseType) : type.IsSubclassOf(baseType);
        }

        /// <summary>
        /// Tests if typeOne is an instance of testType. Same as IsSubclassOfOrImplements, but also checks for equality.
        /// </summary>
        /// <param name="typeOne">Type One.</param>
        /// <param name="testType">Test Type.</param>
        /// <returns>True if found to be is instance of.</returns>
        public static bool IsInstanceOf(this Type typeOne, Type testType)
        {
            if (typeOne == null || testType == null)
                return false;

            return typeOne.Equals(testType) || typeOne.IsSubclassOfOrImplements(testType);
        }

        /// <summary>
        /// Get Properties Of Type.
        /// </summary>
        /// <param name="type">type.</param>
        /// <param name="propertyType">Property type.</param>
        /// <returns>List of Property Info.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesOfType(this Type type, Type propertyType)
        {
            if (type == null || propertyType == null)
                return null;

            var result = from p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         where p.PropertyType == propertyType
                         select p;

            return result;
        }

        /// <summary>
        /// Get Fields Of Type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="fieldType">Field type.</param>
        /// <returns>List of field Info.</returns>
        public static IEnumerable<FieldInfo> GetFieldsOfType(this Type type, Type fieldType)
        {
            if (type == null || fieldType == null)
                return null;

            var result = from p in type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                         where p.FieldType == fieldType
                         select p;

            return result;
        }

        /// <summary>
        /// Get Fields And Properties Of Type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="memberType">Member type.</param>
        /// <returns>List of Member Info.</returns>
        public static IEnumerable<MemberInfo> GetFieldsAndPropertiesOfType(this Type type, Type memberType)
        {
            if (type == null || memberType == null)
                return null;

            var result = type.GetFieldsOfType(memberType).Cast<MemberInfo>().Concat(type.GetPropertiesOfType(memberType).Cast<MemberInfo>());

            return result;
        }
    }
}
