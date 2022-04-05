﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Primitives;
using Riskeer.Storage.Core.Create.FailureMechanismSectionResults;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.FailureMechanismSectionResults
{
    [TestFixture]
    public class NonAdoptableWithProfileProbabilityFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((NonAdoptableWithProfileProbabilityFailureMechanismSectionResult) null).Create();

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
            var initialFailureMechanismResultType = random.NextEnumValue<NonAdoptableInitialFailureMechanismResultType>();
            double manualProfileProbability = random.NextDouble();
            double manualSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedProfileProbability = random.NextDouble();
            double refinedSectionProbability = random.NextDouble();

            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultProfileProbability = manualProfileProbability,
                ManualInitialFailureMechanismResultSectionProbability = manualSectionProbability,
                FurtherAnalysisType = furtherAnalysisType,
                RefinedProfileProbability = refinedProfileProbability,
                RefinedSectionProbability = refinedSectionProbability
            };

            // Call
            NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity entity = sectionResult.Create();

            // Assert
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.AreEqual(Convert.ToByte(initialFailureMechanismResultType), entity.InitialFailureMechanismResultType);
            Assert.AreEqual(manualProfileProbability, entity.ManualInitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(manualSectionProbability, entity.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(Convert.ToByte(furtherAnalysisType), entity.FurtherAnalysisType);
            Assert.AreEqual(refinedProfileProbability, entity.RefinedProfileProbability);
            Assert.AreEqual(refinedSectionProbability, entity.RefinedSectionProbability);
        }

        [Test]
        public void Create_SectionResultWithNaNValues_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                ManualInitialFailureMechanismResultProfileProbability = double.NaN,
                ManualInitialFailureMechanismResultSectionProbability = double.NaN,
                RefinedProfileProbability = double.NaN,
                RefinedSectionProbability = double.NaN
            };

            // Call
            NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity entity = sectionResult.Create();

            // Assert
            Assert.IsNull(entity.ManualInitialFailureMechanismResultProfileProbability);
            Assert.IsNull(entity.ManualInitialFailureMechanismResultSectionProbability);
            Assert.IsNull(entity.RefinedProfileProbability);
            Assert.IsNull(entity.RefinedSectionProbability);
        }
    }
}