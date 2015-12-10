using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class BaseFailureMechanismTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        [TestCase(101)]
        [TestCase(-1)]
        [TestCase(-1e-6)]
        [TestCase(100+1e-6)]
        public void Contribution_ValueOutsideValidRegion_ThrowsArgumentException(double value)
        {
            // Setup
            var failureMechanism = mockRepository.StrictMock<BaseFailureMechanism>();

            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => failureMechanism.Contribution = value;

            // Assert
            Assert.Throws<ArgumentException>(test);
            mockRepository.VerifyAll();
        }
    }
}