using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace JPenny.TaskExtensions.Tests
{
    public interface ISuccessAction
    {
        void SuccessAction(int x);
    }

    public class OnSuccessTests
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
            var mock = new Mock<ISuccessAction>();
            mock.Setup(x => x.SuccessAction(It.IsAny<int>()));

            var task = _taskFactory.GetCancelledTask<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.OnSuccess(task, mockObject.SuccessAction);

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await fixture);

            mock.Verify(x => x.SuccessAction(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task Should_run_on_completion()
        {
            // Arrange
            var mock = new Mock<ISuccessAction>();
            mock.Setup(x => x.SuccessAction(It.IsAny<int>()));

            var task = _taskFactory.GetNumber();
            var mockObject = mock.Object;

            // Act
            var result = await Tasks.OnSuccess(task, mockObject.SuccessAction);

            // Assert
            mock.Verify(x => x.SuccessAction(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Should_not_run_on_exception()
        {
            // Arrange
            var mock = new Mock<ISuccessAction>();
            mock.Setup(x => x.SuccessAction(It.IsAny<int>()));

            var task = _taskFactory.ThrowSystemException<int>();
            var mockObject = mock.Object;

            // Act
            var fixture = Tasks.OnSuccess(task, mockObject.SuccessAction);

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await fixture);

            mock.Verify(x => x.SuccessAction(It.IsAny<int>()), Times.Never);
        }
    }
}