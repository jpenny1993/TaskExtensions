using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

using Utility = JPenny.Tasks.Extensions.TaskExtensions;

namespace JPenny.Tasks.Tests
{
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
            var mock = new Mock<IAction>();
            mock.Setup(x => x.Action());

            var task = _taskFactory.GetCancelledTask<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Utility.OnCancelled(task, mockObject.Action);

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await fixture);

            mock.Verify(x => x.Action(), Times.Once);
        }

        [Test]
        public async Task Should_not_run_on_completion()
        {
            // Arrange
            var mock = new Mock<IAction>();
            mock.Setup(x => x.Action());

            var task = _taskFactory.GetNumber();
            var mockObject = mock.Object;

            // Act
            var result = await Utility.OnCancelled(task, mockObject.Action);

            // Assert
            mock.Verify(x => x.Action(), Times.Never);
        }

        [Test]
        public void Should_not_run_on_exception()
        {
            // Arrange
            var mock = new Mock<IAction>();
            mock.Setup(x => x.Action());

            var task = _taskFactory.ThrowSystemException<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Utility.OnCancelled(task, mockObject.Action);

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await fixture);

            mock.Verify(x => x.Action(), Times.Never);
        }
    }
}