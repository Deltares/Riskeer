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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.StabilityPointStructures;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.StabilityPointStructures.Data;

namespace Application.Ringtoets.Storage.Test.Create.StabilityPointStructures
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () => sectionResult.Create(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Create_VariousResults_ReturnsEntity(
            [Values(true, false)] bool assessmentLayerOneResult,
            [Values(0.2, 0.523)] double assessmentLayerTwoAResult,
            [Values(3.2, 4.5)] double assessmentLayerThreeResult)
        {
            // Setup
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerOne = assessmentLayerOneResult,
                AssessmentLayerTwoA = (RoundedDouble)assessmentLayerTwoAResult,
                AssessmentLayerThree = (RoundedDouble) assessmentLayerThreeResult
            };

            // Call
            var result = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(Convert.ToByte(assessmentLayerOneResult), result.LayerOne);
            Assert.AreEqual(assessmentLayerTwoAResult, result.LayerTwoA);
            Assert.AreEqual(assessmentLayerThreeResult, result.LayerThree);
        }

        [Test]
        public void Create_WithNaNLevel2aResult_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerTwoA = RoundedDouble.NaN
            };

            // Call
            var result = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNull(result.LayerTwoA);
        }

        [Test]
        public void Create_WithNaNLevel3Result_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerThree = RoundedDouble.NaN
            };

            // Call
            var result = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNull(result.LayerThree);
        }
    }
}