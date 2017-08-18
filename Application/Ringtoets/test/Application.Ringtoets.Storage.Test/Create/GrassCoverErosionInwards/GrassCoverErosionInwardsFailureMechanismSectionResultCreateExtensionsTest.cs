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
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () => sectionResult.Create(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [Combinatorial]
        public void Create_WithDifferentResults_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var random = new Random(21);
            var assessmentLayerOneResult = random.NextEnumValue<AssessmentLayerOneState>();
            RoundedDouble assessmentLayerThreeResult = random.NextRoundedDouble();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerOne = assessmentLayerOneResult,
                AssessmentLayerThree = assessmentLayerThreeResult
            };

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsSectionResultEntity entity = sectionResult.Create(registry);

            // Assert
            Assert.AreEqual(Convert.ToByte(assessmentLayerOneResult), entity.LayerOne);
            Assert.AreEqual(assessmentLayerThreeResult, entity.LayerThree);
            Assert.IsNull(entity.GrassCoverErosionInwardsCalculationEntity);
        }

        [Test]
        public void Create_WithNaNLevel3Result_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerThree = RoundedDouble.NaN
            };

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsSectionResultEntity entity = sectionResult.Create(registry);

            // Assert
            Assert.IsNull(entity.LayerThree);
        }

        [Test]
        public void Create_CalculationSet_ReturnEntityWithCalculationEntity()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                Calculation = calculation
            };

            var registry = new PersistenceRegistry();
            var entity = new GrassCoverErosionInwardsCalculationEntity();
            registry.Register(entity, calculation);

            // Call
            GrassCoverErosionInwardsSectionResultEntity result = sectionResult.Create(registry);

            // Assert
            Assert.AreSame(entity, result.GrassCoverErosionInwardsCalculationEntity);
        }
    }
}