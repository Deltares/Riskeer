using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismSectionResultContextTest
    {
        [Test]
        public void Constructor_WithSectionResultsAndFailureMechanism_PropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            var sectionResult = CreateFailureMechanismSectionResult();

            mocks.ReplayAll();

            // Call
            var context = new FailureMechanismSectionResultContext(new[]
            {
                sectionResult
            }, failureMechanismMock);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                sectionResult
            }, context.SectionResults);
            Assert.AreSame(failureMechanismMock, context.FailureMechanism);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismSectionResultListNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismSectionResultContext(null, failureMechanismMock);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResults", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = CreateFailureMechanismSectionResult();

            // Call
            TestDelegate call = () => new FailureMechanismSectionResultContext(new[]
            {
                sectionResult
            }, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        private static FailureMechanismSectionResult CreateFailureMechanismSectionResult()
        {
            var points = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            var section = new FailureMechanismSection("test", points);
            var sectionResult = new FailureMechanismSectionResult(section);
            return sectionResult;
        }
    }
}