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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Storage.Core.Create.MacroStabilityInwards;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((MacroStabilityInwardsFailureMechanismSectionResult) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_WithDifferentResults_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var random = new Random(39);
            var simpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            double tailorMadeAssessmentProbability = random.NextDouble();
            bool useManualAssemblyProbability = random.NextBoolean();
            double manualAssemblyProbability = random.NextDouble();

            var sectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                DetailedAssessmentResult = detailedAssessmentResult,
                TailorMadeAssessmentResult = tailorMadeAssessmentResult,
                TailorMadeAssessmentProbability = tailorMadeAssessmentProbability,
                UseManualAssemblyProbability = useManualAssemblyProbability,
                ManualAssemblyProbability = manualAssemblyProbability
            };

            // Call
            MacroStabilityInwardsSectionResultEntity entity = sectionResult.Create();

            // Assert
            Assert.AreEqual(Convert.ToByte(simpleAssessmentResult), entity.SimpleAssessmentResult);
            Assert.AreEqual(Convert.ToByte(detailedAssessmentResult), entity.DetailedAssessmentResult);
            Assert.AreEqual(Convert.ToByte(tailorMadeAssessmentResult), entity.TailorMadeAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentProbability, entity.TailorMadeAssessmentProbability);
            Assert.AreEqual(Convert.ToByte(useManualAssemblyProbability), entity.UseManualAssemblyProbability);
            Assert.AreEqual(manualAssemblyProbability, entity.ManualAssemblyProbability);
        }

        [Test]
        public void Create_SectionResultWithNaNValues_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                TailorMadeAssessmentProbability = double.NaN,
                ManualAssemblyProbability = double.NaN
            };

            // Call
            MacroStabilityInwardsSectionResultEntity result = sectionResult.Create();

            // Assert
            Assert.IsNull(result.TailorMadeAssessmentProbability);
            Assert.IsNull(result.ManualAssemblyProbability);
        }
    }
}