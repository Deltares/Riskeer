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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.ClosingStructures;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;

namespace Application.Ringtoets.Storage.Test.Create.ClosingStructures
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () => sectionResult.Create(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [Combinatorial]
        public void Create_WithDifferentResults_ReturnsEntityWithExpectedResults(
            [Values(AssessmentLayerOneState.NotAssessed, AssessmentLayerOneState.NoVerdict,
                AssessmentLayerOneState.Sufficient)] AssessmentLayerOneState assessmentLayerOneResult,
            [Values(0.2, 0.5)] double assessmentLayerThreeResult)
        {
            // Setup
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerOne = assessmentLayerOneResult,
                AssessmentLayerThree = (RoundedDouble) assessmentLayerThreeResult
            };

            // Call
            ClosingStructuresSectionResultEntity result = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(Convert.ToByte(assessmentLayerOneResult), result.LayerOne);
            Assert.AreEqual(assessmentLayerThreeResult, result.LayerThree);
            Assert.IsNull(result.ClosingStructuresCalculationEntityId);
        }

        [Test]
        public void Create_WithNaNLevel3Result_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerThree = RoundedDouble.NaN
            };

            // Call
            ClosingStructuresSectionResultEntity result = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNull(result.LayerThree);
        }

        [Test]
        public void Create_CalculationSet_ReturnEntityWithCalculationEntity()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                Calculation = calculation
            };

            var registry = new PersistenceRegistry();
            var entity = new ClosingStructuresCalculationEntity();
            registry.Register(entity, calculation);

            // Call
            ClosingStructuresSectionResultEntity result = sectionResult.Create(registry);

            // Assert
            Assert.AreSame(entity, result.ClosingStructuresCalculationEntity);
        }
    }
}