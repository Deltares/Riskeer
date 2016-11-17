// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Create.GrassCoverSlipOffInwards;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverSlipOffInwards
{
    [TestFixture]
    public class GrassCoverSlipOffInwardsFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_VariousResults_ReturnsEntity(
            [Values(AssessmentLayerOneState.NotAssessed, AssessmentLayerOneState.NeedsDetailedAssessment,
                AssessmentLayerOneState.Sufficient)] AssessmentLayerOneState assessmentLayerOneResult,
            [Values(AssessmentLayerTwoAResult.NotCalculated, AssessmentLayerTwoAResult.Failed)] AssessmentLayerTwoAResult assessmentLayerTwoAResult,
            [Values(3.2, 4.5)] double assessmentLayerThreeResult)
        {
            // Setup
            var sectionResult = new GrassCoverSlipOffInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerOne = assessmentLayerOneResult,
                AssessmentLayerTwoA = assessmentLayerTwoAResult,
                AssessmentLayerThree = (RoundedDouble) assessmentLayerThreeResult
            };

            // Call
            var result = sectionResult.Create();

            // Assert
            Assert.AreEqual(Convert.ToByte(assessmentLayerOneResult), result.LayerOne);
            Assert.AreEqual(Convert.ToByte(assessmentLayerTwoAResult), result.LayerTwoA);
            Assert.AreEqual(assessmentLayerThreeResult, result.LayerThree);
        }

        [Test]
        public void Create_WithNaNLevel3Result_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new GrassCoverSlipOffInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerThree = RoundedDouble.NaN
            };

            // Call
            var result = sectionResult.Create();

            // Assert
            Assert.IsNull(result.LayerThree);
        }
    }
}