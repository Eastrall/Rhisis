using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rhisis.Core.Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Get classes with a custom attribute.
        /// </summary>
        /// <param name="type">Attribute type</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetClassesWithCustomAttribute(Type type)
        {
            return GetRhisisAssemblies().SelectMany(y => y.GetTypes().Where(w => w.GetTypeInfo().GetCustomAttribute(type) != null));
        }

        /// <summary>
        /// Get classes with a custom attribute.
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetClassesWithCustomAttribute<T>() => GetClassesWithCustomAttribute(typeof(T));

        /// <summary>
        /// Get classes that are assignable from a given type. (inherits from)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<TypeInfo> GetClassesAssignableFrom(Type type)
        {
            return from x in GetRhisisAssemblies().SelectMany(x => x.GetTypes())
                   where x.GetInterfaces().Any(i => i.IsAssignableFrom(type))
                   select x.GetTypeInfo();
        }

        /// <summary>
        /// Get classes that are assignable from a given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TypeInfo> GetClassesAssignableFrom<T>() => GetClassesAssignableFrom(typeof(T));

        /// <summary>
        /// Get methods with custom attributes.
        /// </summary>
        /// <param name="type">Attribute type</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetMethodsWithAttributes(Type type)
        {
            return Assembly.GetEntryAssembly()
                .GetTypes()
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(type)?.Count() > 0);
        }

        /// <summary>
        /// Get methods with custom attributes.
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetMethodsWithAttributes<T>() => GetMethodsWithAttributes(typeof(T));

        /// <summary>
        /// Gets rhisis assemblies.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetRhisisAssemblies()
            => AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("Rhisis"));
    }
}
