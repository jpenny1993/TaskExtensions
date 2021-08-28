using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

using Utility = JPenny.TaskExtensions.Extensions.TaskExtensions;

namespace JPenny.TaskExtensions.Tests
{
    public class OnExceptionOfTypeTests
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
            var mock = new Mock<IAction<Exception>>();
            mock.Setup(x => x.Action(It.IsAny<Exception>()));

            var task = _taskFactory.GetCancelledTask<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Utility.OnException(task, mockObject.Action);

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await fixture);

            mock.Verify(x => x.Action(It.IsAny<Exception>()), Times.Never);
        }

        [Test]
        public async Task Should_not_run_on_completion()
        {
            // Arrange
            var mock = new Mock<IAction<Exception>>();
            mock.Setup(x => x.Action(It.IsAny<Exception>()));

            var task = _taskFactory.GetNumber();
            var mockObject = mock.Object;

            // Act
            var result = await Utility.OnException(task, mockObject.Action);

            // Assert
            mock.Verify(x => x.Action(It.IsAny<Exception>()), Times.Never);
        }

        [Test]
        public void Should_run_on_exception()
        {
            // Arrange
            var mock = new Mock<IAction<Exception>>();
            mock.Setup(x => x.Action(It.IsAny<Exception>()));

            var task = _taskFactory.ThrowSystemException<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Utility.OnException(task, mockObject.Action);

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await fixture);

            mock.Verify(x => x.Action(It.IsAny<Exception>()), Times.Once);
        }
    }
}