using System;
using System.Collections.Generic;

using Core.Common.Base;

using NUnit.Framework;

using Ringtoets.Common.Data;

namespace Ringtoets.Integration.Data.Test
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
            Assert.AreEqual("Referentielijn", assessmentSection.ReferenceLine.Name);
            Assert.IsNull(assessmentSection.FailureMechanismContribution);
        }

        private class SimpleAssessmentSection : AssessmentSectionBase {
            public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                throw new NotImplementedException();
            }
        }
    }
}