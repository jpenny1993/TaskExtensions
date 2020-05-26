using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace JPenny.TaskExtensions.Tests
{
    public interface IFinallyAction
    {
        Task<int> FinallyAction(Task<int> t);
    }

    public class FinallyTests
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
            var mock = new Mock<IFinallyAction>();
            mock.Setup(x => x.FinallyAction(It.IsAny<Task<int>>()));

            var task = _taskFactory.GetCancelledTask<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.Finally(task, x => mockObject.FinallyAction(x));

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await fixture);

            mock.Verify(x => x.FinallyAction(It.IsAny<Task<int>>()), Times.Once);
        }

        [Test]
        public async Task Should_run_on_completion()
        {
            // Arrange
            var mock = new Mock<IFinallyAction>();
            mock.Setup(x => x.FinallyAction(It.IsAny<Task<int>>()));

            var task = _taskFactory.GetNumber();
            var mockObject = mock.Object;

            // Act
            var result = await Tasks.Finally(task, x => mockObject.FinallyAction(x));

            // Assert
            mock.Verify(x => x.FinallyAction(It.IsAny<Task<int>>()), Times.Once);
        }

        [Test]
        public void Should_run_on_exception()
        {
            // Arrange
            var mock = new Mock<IFinallyAction>();
            mock.Setup(x => x.FinallyAction(It.IsAny<Task<int>>()));

            var task = _taskFactory.ThrowSystemException<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.Finally(task, x => mockObject.FinallyAction(x));

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await fixture);

            mock.Verify(x => x.FinallyAction(It.IsAny<Task<int>>()), Times.Once);
        }
    }
}