using System;
using System.Collections.Generic;
using JPenny.Tasks.Resolvers;

namespace JPenny.Tasks.Builders
{
    public abstract class TaskBuilderBase
    {
        protected ITaskResolver MainTask { get; set; }

        protected ITaskResolver CancelledTask { get; set; }

        protected ITaskResolver CompletedTask { get; set; }

        protected IDictionary<Type, Action<Exception>> ExceptionHandlers { get; } = new Dictionary<Type, Action<Exception>>();

        internal TBuilder ApplyOptions<TBuilder>(Action<TBuilder> taskBuilder)
            where TBuilder : TaskBuilderBase
        {
            var self = (TBuilder)this;
            taskBuilder(self);
            return self;
        }
    }
}
