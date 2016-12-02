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
using Application.Ringtoets.Storage.Create.HeightStructures;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;

namespace Application.Ringtoets.Storage.Test.Create.HeightStructures
{
    [TestFixture]
    public class HeightStructuresFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () => sectionResult.Create(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Create_WithDifferentResults_ReturnsEntityWithExpectedResults(
            [Values(AssessmentLayerOneState.NotAssessed, AssessmentLayerOneState.NoVerdict,
                AssessmentLayerOneState.Sufficient)] AssessmentLayerOneState assessmentLayerOneResult,
            [Values(3.2, 4.5)] double assessmentLayerThreeResult)
        {
            // Setup
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerOne = assessmentLayerOneResult,
                AssessmentLayerThree = (RoundedDouble) assessmentLayerThreeResult
            };

            // Call
            HeightStructuresSectionResultEntity result = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(Convert.ToByte(assessmentLayerOneResult), result.LayerOne);
            Assert.AreEqual(assessmentLayerThreeResult, result.LayerThree);
            Assert.IsNull(result.HeightStructuresCalculationEntityId);
        }

        [Test]
        public void Create_WithNaNLevel3Result_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerThree = RoundedDouble.NaN
            };

            // Call
            HeightStructuresSectionResultEntity result = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNull(result.LayerThree);
        }

        [Test]
        public void Create_CalculationSet_ReturnEntityWithCalculationEntity()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                Calculation = calculation
            };

            var registry = new PersistenceRegistry();
            var entity = new HeightStructuresCalculationEntity();
            registry.Register(entity, calculation);

            // Call
            HeightStructuresSectionResultEntity result = sectionResult.Create(registry);

            // Assert
            Assert.AreSame(entity, result.HeightStructuresCalculationEntity);
        }
    }
}