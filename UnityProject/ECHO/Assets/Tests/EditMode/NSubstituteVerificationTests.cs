using NUnit.Framework;
using NSubstitute;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace EchoZero.Tests.EditMode
{
    /// <summary>
    /// Smoke test that confirms NSubstitute 6.2.0 is correctly installed
    /// and usable from the EditMode test assembly.
    ///
    /// Run this first after adding NSubstitute.dll to Assets/Plugins/NSubstitute/.
    /// If it passes, NSubstitute is working. If it fails to compile, check:
    ///   - NSubstitute.dll is in Assets/Plugins/NSubstitute/
    ///   - The .meta file sets the DLL to Editor Only
    ///   - EchoZero.Tests.EditMode.asmdef references NSubstitute.dll under precompiledReferences
    ///
    /// TASK: TASK-001 acceptance criterion
    /// </summary>
    [TestFixture]
    public class NSubstituteVerificationTests
    {
        public interface IVerificationService
        {
            string Greet(string name);
            int Add(int a, int b);
        }

        [Test]
        public void NSubstitute_CanCreateSubstitute()
        {
            var sub = Substitute.For<IVerificationService>();
            Assert.IsNotNull(sub, "Substitute.For<T> must return a non-null substitute.");
        }

        [Test]
        public void NSubstitute_CanConfigureReturnValue()
        {
            var sub = Substitute.For<IVerificationService>();
            sub.Greet("ECHO").Returns("Hello, ECHO.");

            var result = sub.Greet("ECHO");

            Assert.AreEqual("Hello, ECHO.", result,
                "Configured return value must be returned by the substitute.");
        }

        [Test]
        public void NSubstitute_CanVerifyCallReceived()
        {
            var sub = Substitute.For<IVerificationService>();
            sub.Add(1, 2).Returns(3);

            _ = sub.Add(1, 2);

            sub.Received(1).Add(1, 2);
            // If NSubstitute is broken this line throws — test will fail with a useful message.
        }

        [Test]
        public void NSubstitute_CanVerifyCallNotReceived()
        {
            var sub = Substitute.For<IVerificationService>();
            sub.DidNotReceive().Greet(Arg.Any<string>());
        }
    }
}
