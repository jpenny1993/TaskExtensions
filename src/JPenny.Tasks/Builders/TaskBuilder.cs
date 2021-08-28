using System;
using System.Collections.Generic;
using JPenny.Tasks.Resolvers;

namespace JPenny.Tasks.Builders
{
    public abstract class TaskBuilder
    {
        protected ITaskResolver MainTask { get; set; }

        protected ITaskResolver CancelledTask { get; set; }

        protected ITaskResolver CompletedTask { get; set; }

        protected IDictionary<Type, Action<Exception>> ExceptionHandlers { get; } = new Dictionary<Type, Action<Exception>>();

        protected void AddExceptionHandler<TException>(Action<TException> onException)
            where TException : Exception
        {
            var type = typeof(TException);
            if (ExceptionHandlers.ContainsKey(type))
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

            ExceptionHandlers.Add(type, action);
        }

        internal TBuilder ApplyOptions<TBuilder>(Action<TBuilder> taskBuilder)
            where TBuilder : TaskBuilder
        {
            var self = (TBuilder)this;
            taskBuilder(self);
            return self;
        }
    }
}
