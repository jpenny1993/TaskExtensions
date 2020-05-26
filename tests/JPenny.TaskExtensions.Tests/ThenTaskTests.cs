using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace JPenny.TaskExtensions.Tests
{
    public class ThenTaskTests
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
            var mock = new Mock<IAction<int, Task<string>>>();
            mock.Setup(x => x.Action(It.IsAny<int>()))
                .Returns(_taskFactory.GetString());

            var task = _taskFactory.GetCancelledTask<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.Then(task, mockObject.Action);

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await fixture);

            mock.Verify(x => x.Action(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task Should_run_on_completion()
        {
            // Arrange
            var mock = new Mock<IAction<int, Task<string>>>();
            mock.Setup(x => x.Action(It.IsAny<int>()))
                .Returns(_taskFactory.GetString());

            var task = _taskFactory.GetNumber();
            var mockObject = mock.Object;

            // Act
            var result = await Tasks.Then(task, mockObject.Action);

            // Assert
            mock.Verify(x => x.Action(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Should_not_run_on_exception()
        {
            // Arrange
            var mock = new Mock<IAction<int, Task<string>>>();
            mock.Setup(x => x.Action(It.IsAny<int>()))
                .Returns(_taskFactory.GetString());

            var task = _taskFactory.ThrowSystemException<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.Then(task, mockObject.Action);

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await fixture);

            mock.Verify(x => x.Action(It.IsAny<int>()), Times.Never);
        }
    }
}