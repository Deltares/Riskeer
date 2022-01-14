// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((AdoptableWithProfileProbabilityFailureMechanismSectionResult) null).CreateForMacroStabilityInwards();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_WithDifferentResults_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            var initialFailureMechanismResultType = random.NextEnumValue<AdoptableInitialFailureMechanismResultType>();
            double manualProfileProbability = random.NextDouble();
            double manualSectionProbability = random.NextDouble();
            bool furtherAnalysisNeeded = random.NextBoolean();
            var probabilityRefinementType = random.NextEnumValue<ProbabilityRefinementType>();
            double refinedProfileProbability = random.NextDouble();
            double refinedSectionProbability = random.NextDouble();

            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResult = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultProfileProbability = manualProfileProbability,
                ManualInitialFailureMechanismResultSectionProbability = manualSectionProbability,
                FurtherAnalysisNeeded = furtherAnalysisNeeded,
                ProbabilityRefinementType = probabilityRefinementType,
                RefinedProfileProbability = refinedProfileProbability,
                RefinedSectionProbability = refinedSectionProbability
            };

            // Call
            MacroStabilityInwardsSectionResultEntity entity = sectionResult.CreateForMacroStabilityInwards();

            // Assert
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.AreEqual(Convert.ToByte(initialFailureMechanismResultType), entity.AdoptableInitialFailureMechanismResultType);
            Assert.AreEqual(manualProfileProbability, entity.ManualInitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(manualSectionProbability, entity.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(Convert.ToByte(furtherAnalysisNeeded), entity.FurtherAnalysisNeeded);
            Assert.AreEqual(Convert.ToByte(probabilityRefinementType), entity.ProbabilityRefinementType);
            Assert.AreEqual(refinedProfileProbability, entity.RefinedProfileProbability);
            Assert.AreEqual(refinedSectionProbability, entity.RefinedSectionProbability);
        }

        [Test]
        public void Create_SectionResultWithNaNValues_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                ManualInitialFailureMechanismResultProfileProbability = double.NaN,
                ManualInitialFailureMechanismResultSectionProbability = double.NaN,
                RefinedProfileProbability = double.NaN,
                RefinedSectionProbability = double.NaN
            };

            // Call
            MacroStabilityInwardsSectionResultEntity entity = sectionResult.CreateForMacroStabilityInwards();

            // Assert
            Assert.IsNull(entity.ManualInitialFailureMechanismResultProfileProbability);
            Assert.IsNull(entity.ManualInitialFailureMechanismResultSectionProbability);
            Assert.IsNull(entity.RefinedProfileProbability);
            Assert.IsNull(entity.RefinedSectionProbability);
        }
    }
}