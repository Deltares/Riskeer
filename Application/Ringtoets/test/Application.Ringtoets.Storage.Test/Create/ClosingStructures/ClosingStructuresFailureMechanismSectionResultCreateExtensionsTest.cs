﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.ClosingStructures
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((ClosingStructuresFailureMechanismSectionResult) null).Create(new PersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

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
        public void Create_WithDifferentResults_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var random = new Random(21);
            var simpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            double tailorMadeAssessmentProbability = random.NextDouble();
            bool useManualAssemblyProbability = random.NextBoolean();
            double manualAssemblyProbability = random.NextDouble();

            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                DetailedAssessmentResult = detailedAssessmentResult,
                TailorMadeAssessmentResult = tailorMadeAssessmentResult,
                TailorMadeAssessmentProbability = tailorMadeAssessmentProbability,
                UseManualAssemblyProbability = useManualAssemblyProbability,
                ManualAssemblyProbability = manualAssemblyProbability
            };

            // Call
            ClosingStructuresSectionResultEntity entity = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(Convert.ToByte(simpleAssessmentResult), entity.SimpleAssessmentResult);
            Assert.AreEqual(Convert.ToByte(detailedAssessmentResult), entity.DetailedAssessmentResult);
            Assert.AreEqual(Convert.ToByte(tailorMadeAssessmentResult), entity.TailorMadeAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentProbability, entity.TailorMadeAssessmentProbability);
            Assert.AreEqual(Convert.ToByte(useManualAssemblyProbability), entity.UseManualAssemblyProbability);
            Assert.AreEqual(manualAssemblyProbability, entity.ManualAssemblyProbability);
            Assert.IsNull(entity.ClosingStructuresCalculationEntityId);
        }

        [Test]
        public void Create_WithNaNLevel3Result_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                TailorMadeAssessmentProbability = double.NaN,
                ManualAssemblyProbability = double.NaN
            };

            // Call
            ClosingStructuresSectionResultEntity entity = sectionResult.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNull(entity.TailorMadeAssessmentProbability);
            Assert.IsNull(entity.ManualAssemblyProbability);
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
            var calculationEntity = new ClosingStructuresCalculationEntity();
            registry.Register(calculationEntity, calculation);

            // Call
            ClosingStructuresSectionResultEntity entity = sectionResult.Create(registry);

            // Assert
            Assert.AreSame(calculationEntity, entity.ClosingStructuresCalculationEntity);
        }
    }
}