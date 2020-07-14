// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Primitives;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.Storage.Core.Create.StabilityPointStructures;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.StabilityPointStructures
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((StabilityPointStructuresFailureMechanismSectionResult) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_WithDifferentResults_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var random = new Random(21);
            var simpleAssessmentResult = random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>();
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            double tailorMadeAssessmentProbability = random.NextDouble();
            bool useManualAssembly = random.NextBoolean();
            double manualAssemblyProbability = random.NextDouble();

            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                DetailedAssessmentResult = detailedAssessmentResult,
                TailorMadeAssessmentResult = tailorMadeAssessmentResult,
                TailorMadeAssessmentProbability = tailorMadeAssessmentProbability,
                UseManualAssembly = useManualAssembly,
                ManualAssemblyProbability = manualAssemblyProbability
            };

            // Call
            StabilityPointStructuresSectionResultEntity entity = sectionResult.Create();

            // Assert
            Assert.AreEqual(Convert.ToByte(simpleAssessmentResult), entity.SimpleAssessmentResult);
            Assert.AreEqual(Convert.ToByte(detailedAssessmentResult), entity.DetailedAssessmentResult);
            Assert.AreEqual(Convert.ToByte(tailorMadeAssessmentResult), entity.TailorMadeAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentProbability, entity.TailorMadeAssessmentProbability);
            Assert.AreEqual(Convert.ToByte(useManualAssembly), entity.UseManualAssembly);
            Assert.AreEqual(manualAssemblyProbability, entity.ManualAssemblyProbability);

            Assert.IsNull(entity.StabilityPointStructuresCalculationEntityId);
        }

        [Test]
        public void Create_SectionResultWithNaNValues_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                TailorMadeAssessmentProbability = double.NaN,
                ManualAssemblyProbability = double.NaN
            };

            // Call
            StabilityPointStructuresSectionResultEntity entity = sectionResult.Create();

            // Assert
            Assert.IsNull(entity.TailorMadeAssessmentProbability);
            Assert.IsNull(entity.ManualAssemblyProbability);
        }
    }
}