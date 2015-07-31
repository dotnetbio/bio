using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// ParseAction
    /// </summary>
    public enum ParseAction
    {
        /// <summary>
        /// Specifies that a field is required when parsing.
        /// </summary>
        Required,
        /// <summary>
        /// Specifies that an element is optional. Note that all public fields are optional by default. This allows you to mark private or protected fields as parsable
        /// </summary>
        Optional,
        /// <summary>
        /// Specifies that a field should not be parsed. This only is useful for public fields that would otherwise be automatically parsed.
        /// </summary>
        Ignore,
        /// <summary>
        /// Specifies that the string used to construct this argument should be stored here. Note that this MUST be of type string.
        /// </summary>
        ArgumentString,
        /// <summary>
        /// Behaves like the params keyword for methods: sucks up all the final arguments and constructs a list out of them. They must all be the same type, as
        /// specified by the type of the list that this attribute is attached to. This can only be placed on a member of type List. This is considered an optional
        /// argument, in the sense that if there are no arguments left, an empty list will be returned. It's up to the parsable type to decide if it wants to check
        /// that the list is non-empty in its FinalizeParse method.
        /// </summary>
        Params
    };

    /// <summary>
    /// Parse Attribute class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ParseAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="action">Parse Action.</param>
        public ParseAttribute(ParseAction action) : this(action, null) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="action">Parse Action.</param>
        /// <param name="parseType">Parse type.</param>
        public ParseAttribute(ParseAction action, Type parseType)
        {
            Action = action;
            ParseTypeOrNull = parseType;
        }

        /// <summary>
        /// Parse Action.
        /// </summary>
        public ParseAction Action { get; private set; }

        /// <summary>
        ///  Parse Type Or Null.
        /// </summary>
        public Type ParseTypeOrNull { get; private set; }

        /// <summary>
        /// Use ConstructorArguments syntax to hard code settings.
        /// </summary>
        public string DefaultParameters { get; set; }
    }

    /// <summary>
    /// Labels a Collection type as not being parsed as a collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ParseAsNonCollectionAttribute : Attribute { }

    /// <summary>
    /// Marks a class so that only fields and properties that have explicit Parse attributes SET IN THE CURRENT CLASS OR A DERIVED CLASS will be parsed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DoNotParseInheritedAttribute : Attribute { }

    /// <summary>
    /// Parse Extensions class.
    /// </summary>
    public static class ParseExtensions
    {
        /// <summary>
        /// Default Optional Attribute.
        /// </summary>
        private static ParseAttribute DefaultOptionalAttribute = new ParseAttribute(ParseAction.Optional);

        /// <summary>
        /// Default Ignore Attribute
        /// </summary>
        private static ParseAttribute DefaultIgnoreAttribute = new ParseAttribute(ParseAction.Ignore);

        /// <summary>
        /// Determines if the ParseExplicit attribute has been set.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool DoNotParseInherited(this Type type)
        {
            return Attribute.IsDefined(type, typeof(DoNotParseInheritedAttribute));
        }

        /// <summary>
        /// Get Parse Type Or Null.
        /// </summary>
        /// <param name="member">The Member.</param>
        /// <returns>Type of Parse.</returns>
        public static Type GetParseTypeOrNull(this MemberInfo member)
        {
            ParseAttribute parseAttribute = (ParseAttribute)Attribute.GetCustomAttribute(member, typeof(ParseAttribute));
            return parseAttribute == null ? null : parseAttribute.ParseTypeOrNull;
        }

        /// <summary>
        /// Get Default Parameters Or Null.
        /// </summary>
        /// <param name="member">The Member.</param>
        /// <returns>Default Parameter or null.</returns>
        public static string GetDefaultParametersOrNull(this MemberInfo member)
        {
            ParseAttribute parseAttribute = (ParseAttribute)Attribute.GetCustomAttribute(member, typeof(ParseAttribute));
            return parseAttribute == null ? null : parseAttribute.DefaultParameters;
        }

        //static ThreadLocal<Cache<MemberInfo, ParseAttribute>> _parseAttributeCache =
        //    new ThreadLocal<Cache<MemberInfo, ParseAttribute>>(
        //        () => new Cache<MemberInfo, ParseAttribute>(maxSize: 10000, recoverySize: 100));
        /// <summary>
        /// Parse Attribute Cache.
        /// </summary>
        static ConcurrentDictionary<MemberInfo, ParseAttribute> _parseAttributeCache = new ConcurrentDictionary<MemberInfo, ParseAttribute>();

        /// <summary>
        /// Get Parse Attribute.
        /// </summary>
        /// <param name="member">The Member.</param>
        /// <param name="actualTypeInheritanceHierarchy">Actual Type Inheritance Hierarchy.</param>
        /// <returns>Parse Attribute.</returns>
        public static ParseAttribute GetParseAttribute(this MemberInfo member, Type[] actualTypeInheritanceHierarchy)
        {
            ParseAttribute pa = _parseAttributeCache.GetOrAdd(member, (m) => GetParseAttributeInternal(m, actualTypeInheritanceHierarchy));
            //ParseAttribute pa;
            //if (!_parseAttributeCache.Value.TryGetValue(member, out pa))
            //{
            //    pa = GetParseAttributeInternal(member, actualTypeInheritanceHierarchy);
            //    _parseAttributeCache.Value.Add(member, pa);
            //}
            return pa;
        }

        /// <summary>
        /// Gets the default or declared parse attribute for the specified member.
        /// </summary>
        /// <param name="member">The Member.</param>
        /// <param name="actualTypeInheritanceHierarchy">actualTypeInheritanceHierarchy</param>
        /// <returns>Parse Attribute.</returns>
        private static ParseAttribute GetParseAttributeInternal(this MemberInfo member, Type[] actualTypeInheritanceHierarchy)
        {
            // march up the stack, starting at the actual type's base. If we find the member before we find a ParseExplicit, we're ok to parse this.
            // If we find a parseExplicity first, then we know we can't parse this member.  If a member is declared in the first class that we see ParseExplicit, 
            // keep it, because we define ParseExplicit as "keep for this and all derived types"
            for (int i = actualTypeInheritanceHierarchy.Length - 1; i >= 0; i--)
            {
                if (member.DeclaringType.Equals(actualTypeInheritanceHierarchy[i]))
                    break;
                if (actualTypeInheritanceHierarchy[i].DoNotParseInherited())
                    return DefaultIgnoreAttribute;
            }

            ParseAttribute parseAttribute = (ParseAttribute)Attribute.GetCustomAttribute(member, typeof(ParseAttribute));
            if (IsIndexer(member))
            {
                Helper.CheckCondition<ParseException>(parseAttribute == null || parseAttribute.Action == ParseAction.Ignore, "Can't parse an Indexer.");
                return DefaultIgnoreAttribute;
            }

            PropertyInfo property = member as PropertyInfo;
            if (parseAttribute == null)
            {
                FieldInfo field = member as FieldInfo;
                if (field != null)
                {
                    return field.IsPublic ? DefaultOptionalAttribute : DefaultIgnoreAttribute;
                }
                else if (property != null)
                {
                    parseAttribute = property.GetGetMethod() != null && property.GetSetMethod() != null && property.GetGetMethod().IsPublic && property.GetSetMethod().IsPublic ? // either will be null if don't exist or non-public
                        DefaultOptionalAttribute : DefaultIgnoreAttribute;
                }
                else
                {
                    parseAttribute = DefaultIgnoreAttribute;
                }
            }
            return parseAttribute;
        }

        /// <summary>
        /// Is Indexer.
        /// </summary>
        /// <param name="member">The Member.</param>
        /// <returns>Member Indexer.</returns>
        public static bool IsIndexer(this MemberInfo member)
        {
            if (member == null)
                return false;

            return member is PropertyInfo && ((PropertyInfo)member).GetIndexParameters().Length > 0;
        }

        /// <summary>
        /// Returns true if and only if the type has a public default constructor, or is an interface or abstract class, in which case a derived type may be parsed.
        /// </summary>
        public static bool IsConstructable(this Type t)
        {
            if (t == null)
                return false;

            if ((t.IsInterface || t.IsAbstract))
                return true;

            return t.HasPublicDefaultConstructor();
        }

        /// <summary>
        /// Has Public Default Constructor.
        /// </summary>
        /// <param name="t">Type of default constructor.</param>
        /// <returns>True if it has public default constructor.</returns>
        public static bool HasPublicDefaultConstructor(this Type t)
        {
            if (t == null)
                return false;

            var constructor = t.GetConstructor(Type.EmptyTypes);
            return constructor != null && constructor.IsPublic;
        }

        /// <summary>
        /// Is collection.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="type">Type of interface.</param>
        /// <returns>True if interface type matches.</returns>
        public static bool IsCollection<T>(this Type type)
        {
            if (type == null)
                return false;

            var interfaces = type.GetInterfaces().Where(interface1 => interface1.ToString().StartsWith("IEnumerable"));

            if (IsCollection(type))
            {
                string targetString = string.Format("IEnumerable<{0}>", typeof(T).ToTypeString());
                return type.GetInterfaces().Any(interface1 => interface1.ToTypeString().Equals(targetString));
            }
            else
                return false;
        }

        /// <summary>
        ///  Is collection.
        /// </summary>
        /// <param name="type">Type of interface.</param>
        /// <returns>True if interface type matches.</returns>
        public static bool IsCollection(this Type type)
        {
            if (type == null)
                return false;

            bool result =
#if !SILVERLIGHT
                type.FindInterfaces(Module.FilterTypeNameIgnoreCase, "ICollection*").Length > 0;
#else
                type.GetInterfaces().Any(interface1 => interface1.ToString().StartsWith("ICollection"));
#endif
            return result;
        }

        /// <summary>
        /// Parse As Collection.
        /// </summary>
        /// <param name="parseType">Parse type.</param>
        /// <returns>True if parse as collection.</returns>
        public static bool ParseAsCollection(this Type parseType)
        {
            bool result = !Attribute.IsDefined(parseType, typeof(ParseAsNonCollectionAttribute)) &&
                parseType.IsCollection()
                && !parseType.HasParseMethod();
            return result;
        }

        /// <summary>
        /// Get Inheritance Hierarchy.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>List of types.</returns>
        public static Type[] GetInheritanceHierarchy(this Type type)
        {
            var result = new Stack<Type>();
            while (type != null)
            {
                result.Push(type);
                type = type.BaseType;
            }
            return result.ToArray();
        }

        /// <summary>
        /// To Parse String.
        /// </summary>
        /// <param name="obj">The Object.</param>
        /// <param name="parseTypeOrNull">Parse Type Or Null.</param>
        /// <param name="suppressDefaults">Suppress Defaults</param>
        /// <returns></returns>
        public static string ToParseString(this object obj, Type parseTypeOrNull = null, bool suppressDefaults = false)
        {
            if (obj == null)
                return null;

            Type t = obj.GetType();
            if (t.HasParseMethod() || !t.IsConstructable())
                return obj.ToString();
            else if (obj is IEnumerable)
            {
                return "(" + ((IEnumerable)obj).StringJoin(",") + ")";
            }
            else if (parseTypeOrNull == null)
                return ConstructorArguments.ToString(obj);    // can only get here if t is constructable.
            else
            {
                object valueAsParseType = ArgumentCollection.ImplicitlyCastValueToType(obj, parseTypeOrNull);
                return ConstructorArguments.ToString(valueAsParseType, suppressDefaults);
            }
        }
    }
}
