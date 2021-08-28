using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JPenny.TaskExtensions.Extensions;
using JPenny.TaskExtensions.Tasks;

namespace JPenny.TaskExtensions
{
    public sealed class TaskOptionsBuilder
    {
        private IPipelineTask _previousTask;

        private Type _resultType;

        private ITaskResolver _taskResolver = TaskResolver.Default;

        private Task _onCancelled = Task.CompletedTask;

        private Task _onCompleted = Task.CompletedTask;

        private IDictionary<Type, Action<Exception>> _exceptionHandlers = new Dictionary<Type, Action<Exception>>();

        internal TaskOptionsBuilder()
        {
            _previousTask = null;
        }

        internal TaskOptionsBuilder(IPipelineTask previousTask)
        {
            _previousTask = previousTask;
        }

        public TaskOptionsBuilder Action(Task task)
        {
            _taskResolver = new TaskResolver(() => task);
            _resultType = null;
            return this;
        }

        public TaskOptionsBuilder Action(Action action, CancellationToken cancellationToken = default)
        {
            _taskResolver = new TaskResolver(() => new Task(action, cancellationToken));
            _resultType = null;
            return this;
        }

        public TaskOptionsBuilder Action<TResult>(Func<Task> taskResolver)
        {

            _taskResolver = new TaskResolver(taskResolver);
            _resultType = null;
            return this;
        }

        public TaskOptionsBuilder Action<TResult>(Task<TResult> task)
        {
            _taskResolver = new TaskResolver(() => task);
            _resultType = typeof(TResult);
            return this;
        }

        public TaskOptionsBuilder Action<TResult>(Func<Task<TResult>> taskResolver)
        {

            _taskResolver = new TaskResolver(taskResolver);
            _resultType = typeof(TResult);
            return this;
        }

        public TaskOptionsBuilder Action<TPreviousResult>(Func<TPreviousResult, Task> taskResolver)
        {
            if (_previousTask == default)
            {
                throw new IndexOutOfRangeException("Must create at least one pipeline task first.");
            }

            var suggestedTaskType = typeof(IPipelineTask<TPreviousResult>);
            var previousTaskType = _previousTask.GetType();

            if (!suggestedTaskType.IsAssignableFrom(previousTaskType))
            {
                throw new InvalidCastException($"Unable to cast previous task type of {previousTaskType} to {suggestedTaskType}.");
            }

            var previousTask = (IPipelineTask<TPreviousResult>)_previousTask;
            _taskResolver = new TaskResolver<TPreviousResult>(previousTask, taskResolver);
            _resultType = null;

            return this;
        }

        public TaskOptionsBuilder Action<TPreviousResult, TResult>(Func<TPreviousResult, Task<TResult>> taskResolver)
        {
            if (_previousTask == default)
            {
                throw new IndexOutOfRangeException("Must create at least one pipeline task first.");
            }

            var suggestedTaskType = typeof(IPipelineTask<TPreviousResult>);
            var previousTaskType = _previousTask.GetType();

            if (!suggestedTaskType.IsAssignableFrom(previousTaskType))
            {
                throw new InvalidCastException($"Unable to cast previous task type of {previousTaskType} to {suggestedTaskType}.");
            }

            var previousTask = (IPipelineTask<TPreviousResult>)_previousTask;
            _taskResolver = new TaskResolver<TPreviousResult, TResult>(previousTask, taskResolver);
            _resultType = typeof(TResult);

            return this;
        }

        public TaskOptionsBuilder Action<TPreviousResult>(Action<TPreviousResult> action)
        {
            if (_previousTask == default)
            {
                throw new IndexOutOfRangeException("Must create at least one pipeline task first.");
            }

            var suggestedType = typeof(IPipelineTask<TPreviousResult>);
            var previousType = _previousTask.GetType();
            if (!suggestedType.IsAssignableFrom(previousType))
            {
                throw new InvalidCastException($"Unable to cast previous task type of {previousType} to {suggestedType}.");
            }

            var previousTask = (IPipelineTask<TPreviousResult>)_previousTask;
            Func<TPreviousResult, Task> taskResolverFunc = (previous) =>
            {
                return new Task(() => action(previous));
            };
            _taskResolver = new TaskResolver<TPreviousResult>(previousTask, taskResolverFunc);
            _resultType = null;

            return this;
        }

        public TaskOptionsBuilder Catch(Action<Exception> onException) => Catch<Exception>(onException);

        public TaskOptionsBuilder Catch<TException>(Action<TException> onException)
            where TException : Exception
        {
            var type = typeof(TException);
            if (_exceptionHandlers.ContainsKey(type))
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

            _exceptionHandlers.Add(type, action);
            return this;
        }

        public TaskOptionsBuilder OnCancelled(Action cancelledAction, CancellationToken cancellationToken = default)
        {
            _onCancelled = new Task(cancelledAction, cancellationToken);
            return this;
        }

        public TaskOptionsBuilder OnCancelled(Task cancelledAction)
        {
            _onCancelled = cancelledAction;
            return this;
        }

        public TaskOptionsBuilder OnCompleted(Action completedAction, CancellationToken cancellationToken = default)
        {
            _onCompleted = new Task(completedAction, cancellationToken);
            return this;
        }

        public TaskOptionsBuilder OnCompleted(Task completedAction)
        {
            _onCompleted = completedAction;
            return this;
        }

        internal IPipelineTask Build()
        {
            if (_resultType != null)
            {
                return typeof(ResultantTask<>)
                    .GetGenericType(_resultType)
                    .CreateInstance<IPipelineTask>(
                        _taskResolver,
                        _exceptionHandlers,
                        _onCancelled,
                        _onCompleted);
            }

            return new VoidTask
            {
                TaskProvider = _taskResolver,
                ExceptionHandlers = _exceptionHandlers,
                CancelledAction = _onCancelled,
                CompletedAction = _onCompleted
            };
        }
    }
}
