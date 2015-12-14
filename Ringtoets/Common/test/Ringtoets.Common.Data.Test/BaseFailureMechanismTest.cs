using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Properties;

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
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-1)]
        [TestCase(100+1e-6)]
        public void Contribution_ValueOutsideValidRegion_ThrowsArgumentException(double value)
        {
            // Setup
            var failureMechanism = mockRepository.StrictMock<BaseFailureMechanism>();

            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => failureMechanism.Contribution = value;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, Resources.FailureMechanism_Contribution_Value_should_be_in_interval_0_100);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(100)]
        [TestCase(50)]
        [TestCase(1e-9)]
        public void Contribution_ValueIntsideValidRegion_DoesNotThrow(double value)
        {
            // Setup
            var failureMechanism = mockRepository.StrictMock<BaseFailureMechanism>();

            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => failureMechanism.Contribution = value;

            // Assert
            Assert.DoesNotThrow(test);
            mockRepository.VerifyAll();
        }
    }
}