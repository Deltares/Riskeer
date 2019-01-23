// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.HeightStructures.Data;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.HeightStructures;
using Riskeer.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.HeightStructures
{
    [TestFixture]
    public class HeightStructuresFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresFailureMechanismSectionResultCreateExtensions.Create(null, new PersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            TestDelegate test = () => sectionResult.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithDifferentResults_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var random = new Random(21);
            var simpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            double tailorMadeAssessmentProbability = random.NextDouble();
            bool useManualAssembly = random.NextBoolean();
            double manualAssemblyProbability = random.NextDouble();

            var sectionResult = new HeightStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                DetailedAssessmentResult = detailedAssessmentResult,
                TailorMadeAssessmentResult = tailorMadeAssessmentResult,
                TailorMadeAssessmentProbability = tailorMadeAssessmentProbability,
                UseManualAssembly = useManualAssembly,
                ManualAssemblyProbability = manualAssemblyProbability
            };

            // Call
            HeightStructuresSectionResultEntity entity = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(Convert.ToByte(simpleAssessmentResult), entity.SimpleAssessmentResult);
            Assert.AreEqual(Convert.ToByte(detailedAssessmentResult), entity.DetailedAssessmentResult);
            Assert.AreEqual(Convert.ToByte(tailorMadeAssessmentResult), entity.TailorMadeAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentProbability, entity.TailorMadeAssessmentProbability);
            Assert.AreEqual(Convert.ToByte(useManualAssembly), entity.UseManualAssembly);
            Assert.AreEqual(manualAssemblyProbability, entity.ManualAssemblyProbability);
            Assert.IsNull(entity.HeightStructuresCalculationEntityId);
        }

        [Test]
        public void Create_SectionResultWithNaNValues_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                TailorMadeAssessmentProbability = double.NaN,
                ManualAssemblyProbability = double.NaN
            };

            // Call
            HeightStructuresSectionResultEntity entity = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNull(entity.TailorMadeAssessmentProbability);
            Assert.IsNull(entity.ManualAssemblyProbability);
        }

        [Test]
        public void Create_CalculationSet_ReturnEntityWithCalculationEntity()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = calculation
            };

            var registry = new PersistenceRegistry();
            var calculationEntity = new HeightStructuresCalculationEntity();
            registry.Register(calculationEntity, calculation);

            // Call
            HeightStructuresSectionResultEntity entity = sectionResult.Create(registry);

            // Assert
            Assert.AreSame(calculationEntity, entity.HeightStructuresCalculationEntity);
        }
    }
}