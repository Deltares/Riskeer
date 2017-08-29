// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.Views;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.Views
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            // Call
            var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<WaveImpactAsphaltCoverFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.AreEqual(row.AssessmentLayerThree, result.AssessmentLayerThree);

            TestHelper.AssertTypeConverter<WaveImpactAsphaltCoverFailureMechanismSectionResultRow, NoValueRoundedDoubleConverter>(
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.AssessmentLayerThree));
        }

        [Test]
        public void AssessmentLayerTwoA_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            const AssessmentLayerTwoAResult newValue = AssessmentLayerTwoAResult.Successful;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoA = newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerThree_ValueSet_ReturnExpectedValue()
        {
            // Setup
            var random = new Random(21);
            RoundedDouble assessmentLayerThree = random.NextRoundedDouble();

            var sectionResult = new WaveImpactAsphaltCoverFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(sectionResult);

            // Call
            row.AssessmentLayerThree = assessmentLayerThree;

            // Assert
            Assert.AreEqual(assessmentLayerThree, sectionResult.AssessmentLayerThree);
        }
    }
}