using System;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class ReferenceLineContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mocks.ReplayAll();

            // Call
            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            // Assert
            Assert.IsInstanceOf<Observable>(referenceLineContext);
            Assert.AreSame(referenceLine, referenceLineContext.WrappedData);
            Assert.AreSame(assessmentSection, referenceLineContext.Parent);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ReferenceLineContext(null);

            // Assert
            string expectedMessage = "Kan geen presentatie object maken voor een referentielijn zonder een traject als eigenaar.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Equals_ContextObjectsHaveSameParent_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context1 = new ReferenceLineContext(assessmentSection);
            var context2 = new ReferenceLineContext(assessmentSection);

            // Call
            var contextsAreEqual1 = context1.Equals(context2);
            var contextsAreEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(contextsAreEqual1);
            Assert.IsTrue(contextsAreEqual2);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_TwoContextsWithDifferentParents_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection1 = mocks.Stub<IAssessmentSection>();
            var assessmentSection2 = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context1 = new ReferenceLineContext(assessmentSection1);
            var context2 = new ReferenceLineContext(assessmentSection2);

            // Call
            var contextsAreEqual1 = context1.Equals(context2);
            var contextsAreEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(contextsAreEqual1);
            Assert.IsFalse(contextsAreEqual2);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ComparingToItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new ReferenceLineContext(assessmentSection);

            // Call
            var isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ComparingToNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new ReferenceLineContext(assessmentSection);

            // Call
            var isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
            mocks.VerifyAll();
        }

        [Test]
        public void GetHashCode_TwoContextInstancesEqualToEachOther_ReturnIdenticalHashes()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new ReferenceLineContext(assessmentSection);

            var otherContext = new ReferenceLineContext(assessmentSection);
            // Precondition
            Assert.True(context.Equals(otherContext));

            // Call
            int contextHashCode = context.GetHashCode();
            int otherContextHashCode = otherContext.GetHashCode();

            // Assert
            Assert.AreEqual(contextHashCode, otherContextHashCode);
            mocks.VerifyAll();
        }
    }
}