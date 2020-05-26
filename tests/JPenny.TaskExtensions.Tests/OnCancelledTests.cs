using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace JPenny.TaskExtensions.Tests
{
    public interface ICancelledAction
    {
        void CancelledAction();
    }

    public class OnCancelledTests
    {
        private Factory _taskFactory;

        [SetUp]
        public void Setup()
        {
            _taskFactory = new Factory();
        }

        [Test]
        public void Should_run_on_cancelled()
        {
            // Arrange
            var mock = new Mock<ICancelledAction>();
            mock.Setup(x => x.CancelledAction());

            var task = _taskFactory.GetCancelledTask<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.OnCancelled(task, mockObject.CancelledAction);

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await fixture);

            mock.Verify(x => x.CancelledAction(), Times.Once);
        }

        [Test]
        public async Task Should_not_run_on_completion()
        {
            // Arrange
            var mock = new Mock<ICancelledAction>();
            mock.Setup(x => x.CancelledAction());

            var task = _taskFactory.GetNumber();
            var mockObject = mock.Object;

            // Act
            var result = await Tasks.OnCancelled(task, mockObject.CancelledAction);

            // Assert
            mock.Verify(x => x.CancelledAction(), Times.Never);
        }

        [Test]
        public void Should_not_run_on_exception()
        {
            // Arrange
            var mock = new Mock<ICancelledAction>();
            mock.Setup(x => x.CancelledAction());

            var task = _taskFactory.ThrowSystemException<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.OnCancelled(task, mockObject.CancelledAction);

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await fixture);

            mock.Verify(x => x.CancelledAction(), Times.Never);
        }
    }
}