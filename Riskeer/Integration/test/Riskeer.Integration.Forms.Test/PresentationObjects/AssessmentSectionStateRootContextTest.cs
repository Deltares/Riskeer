using System;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;

namespace Riskeer.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class AssessmentSectionStateRootContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            var context = new AssessmentSectionStateRootContext(assessmentSection);

            // Assert
            Assert.IsInstanceOf<StateRootContext>(context);
            Assert.AreSame(assessmentSection, context.WrappedData);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionStateRootContext(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("wrappedData", exception.ParamName);
        }
    }
}