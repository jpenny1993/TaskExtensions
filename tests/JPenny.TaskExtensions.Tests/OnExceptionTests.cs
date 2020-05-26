using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace JPenny.TaskExtensions.Tests
{
    public interface IExceptionAction
    {
        void ExceptionAction(AggregateException ex);
    }

    public class OnExceptionTests
    {
        private Factory _taskFactory;

        [SetUp]
        public void Setup()
        {
            _taskFactory = new Factory();
        }

        [Test]
        public void Should_not_run_on_cancelled()
        {
            // Arrange
            var mock = new Mock<IExceptionAction>();
            mock.Setup(x => x.ExceptionAction(It.IsAny<AggregateException>()));

            var task = _taskFactory.GetCancelledTask<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.OnException(task, mockObject.ExceptionAction);

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await fixture);

            mock.Verify(x => x.ExceptionAction(It.IsAny<AggregateException>()), Times.Never);
        }

        [Test]
        public async Task Should_not_run_on_completion()
        {
            // Arrange
            var mock = new Mock<IExceptionAction>();
            mock.Setup(x => x.ExceptionAction(It.IsAny<AggregateException>()));

            var task = _taskFactory.GetNumber();
            var mockObject = mock.Object;

            // Act
            var result = await Tasks.OnException(task, mockObject.ExceptionAction);

            // Assert
            mock.Verify(x => x.ExceptionAction(It.IsAny<AggregateException>()), Times.Never);
        }

        [Test]
        public void Should_run_on_exception()
        {
            // Arrange
            var mock = new Mock<IExceptionAction>();
            mock.Setup(x => x.ExceptionAction(It.IsAny<AggregateException>()));

            var task = _taskFactory.ThrowSystemException<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.OnException(task, mockObject.ExceptionAction);

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await fixture);

            mock.Verify(x => x.ExceptionAction(It.IsAny<AggregateException>()), Times.Once);
        }
    }
}