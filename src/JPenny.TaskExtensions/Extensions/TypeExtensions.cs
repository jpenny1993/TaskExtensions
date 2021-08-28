using System;

namespace JPenny.TaskExtensions.Extensions
{
    internal static class TypeExtensions
    {
        public static Type GetGenericType(this Type type, params Type[] typeArguments)
        {
            return type.MakeGenericType(typeArguments);
        }

        public static TResult CreateInstance<TResult>(this Type type, params object[] constructorArguments)
        {
            return (TResult)Activator.CreateInstance(type, constructorArguments);
        }
    }
}
