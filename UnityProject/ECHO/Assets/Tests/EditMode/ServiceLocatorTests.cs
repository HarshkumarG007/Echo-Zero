using System;
using NUnit.Framework;
using EchoZero.Core;

namespace EchoZero.Tests.EditMode
{
    /// <summary>
    /// Unit tests for ServiceLocator.
    /// TASK: TASK-002
    /// </summary>
    [TestFixture]
    public class ServiceLocatorTests
    {
        private interface ITestService { string Name { get; } }
        private class TestService : ITestService { public string Name => "TestService"; }
        private class OtherService : ITestService { public string Name => "OtherService"; }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
        }

        [Test]
        public void Register_ThenGet_ReturnsSameInstance()
        {
            var svc = new TestService();
            ServiceLocator.Register<ITestService>(svc);
            var result = ServiceLocator.Get<ITestService>();
            Assert.AreSame(svc, result, "Get<T> must return the exact registered instance.");
        }

        [Test]
        public void Get_WithoutRegister_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(
                () => ServiceLocator.Get<ITestService>(),
                "Get<T> must throw when T is not registered.");
        }

        [Test]
        public void Register_Twice_SecondReplacesFirst()
        {
            var first  = new TestService();
            var second = new OtherService();
            ServiceLocator.Register<ITestService>(first);
            ServiceLocator.Register<ITestService>(second);
            var result = ServiceLocator.Get<ITestService>();
            Assert.AreSame(second, result, "Second registration must replace the first.");
        }

        [Test]
        public void Register_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => ServiceLocator.Register<ITestService>(null),
                "Registering null must throw ArgumentNullException.");
        }

        [Test]
        public void TryGet_Registered_ReturnsTrueAndService()
        {
            var svc = new TestService();
            ServiceLocator.Register<ITestService>(svc);
            var found = ServiceLocator.TryGet<ITestService>(out var result);
            Assert.IsTrue(found);
            Assert.AreSame(svc, result);
        }

        [Test]
        public void TryGet_NotRegistered_ReturnsFalse()
        {
            var found = ServiceLocator.TryGet<ITestService>(out var result);
            Assert.IsFalse(found);
            Assert.IsNull(result);
        }

        [Test]
        public void Clear_ThenGet_ThrowsInvalidOperationException()
        {
            ServiceLocator.Register<ITestService>(new TestService());
            ServiceLocator.Clear();
            Assert.Throws<InvalidOperationException>(
                () => ServiceLocator.Get<ITestService>(),
                "After Clear(), Get<T> must throw.");
        }
    }
}
