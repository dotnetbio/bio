using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bio.Extensions
{
    /// <summary>
    /// Reflection extensions.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Method to retrieve all the interfaces of the given Type and it's base classes.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetInterfaces(this Type t)
        {
            return InternalInterface(t).Distinct();
        }

        private static IEnumerable<Type> InternalInterface(Type t)
        {
            TypeInfo ti = t.GetTypeInfo();

            foreach (var intf in ti.ImplementedInterfaces)
                yield return intf;

            if (ti.BaseType != null)
            {
                foreach (var intf in InternalInterface(ti.BaseType))
                    yield return intf;
            }
        }

    }
}
