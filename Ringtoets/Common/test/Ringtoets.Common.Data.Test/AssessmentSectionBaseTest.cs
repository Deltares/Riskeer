using System;
using System.Collections.Generic;

using Core.Common.Base;

using NUnit.Framework;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class AssessmentSectionBaseTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Call
            var assessmentSection = new SimpleAssessmentSection();

            // Assert
            Assert.IsInstanceOf<Observable>(assessmentSection);
            Assert.AreEqual(String.Empty, assessmentSection.Name);
            Assert.IsNull(assessmentSection.ReferenceLine);
            Assert.IsNull(assessmentSection.FailureMechanismContribution);
            Assert.IsNull(assessmentSection.HydraulicBoundaryDatabase);
        }

        [Test]
        public void SimpleProperties_SetNewValue_GetNewlySetValue()
        {
            // Setup
            var assessmentSection = new SimpleAssessmentSection();

            var newReferenceLine = new ReferenceLine();

            // Call
            assessmentSection.ReferenceLine = newReferenceLine;

            // Assert
            Assert.AreSame(newReferenceLine, assessmentSection.ReferenceLine);
        }

        private class SimpleAssessmentSection : AssessmentSectionBase {
            public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                throw new NotImplementedException();
            }
        }
    }
}