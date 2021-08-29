using System;
using System.Collections.Generic;

namespace JPenny.Tasks.Extensions
{
    internal static class DictionaryExceptions
    {
        internal static void AddExceptionHandler<TException>(this IDictionary<Type, Action<Exception>> dicitonary, Action<TException> onException)
            where TException : Exception
        {
            var type = typeof(TException);
            if (dicitonary.ContainsKey(type))
            {
                throw new ArgumentOutOfRangeException($"Unable to add multiple exception handlers of type {type.FullName}.");
            }

            var action = new Action<Exception>(error =>
            {
                if (error is TException ex)
                {
                    onException(ex);
                }
            });

            dicitonary.Add(type, action);
        }
    }
}
