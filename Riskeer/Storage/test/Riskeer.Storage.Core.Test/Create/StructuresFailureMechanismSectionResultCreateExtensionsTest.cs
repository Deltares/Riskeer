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
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class StructuresFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((AdoptableFailureMechanismSectionResult) null).Create<TestStructuresFailureMechanismSectionResultEntity>();

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
            double manualSectionProbability = random.NextDouble();
            bool furtherAnalysisNeeded = random.NextBoolean();
            double refinedSectionProbability = random.NextDouble();

            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResult = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultSectionProbability = manualSectionProbability,
                FurtherAnalysisNeeded = furtherAnalysisNeeded,
                RefinedSectionProbability = refinedSectionProbability
            };

            // Call
            var entity = sectionResult.Create<TestStructuresFailureMechanismSectionResultEntity>();

            // Assert
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.AreEqual(Convert.ToByte(initialFailureMechanismResultType), entity.InitialFailureMechanismResultType);
            Assert.AreEqual(manualSectionProbability, entity.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(Convert.ToByte(furtherAnalysisNeeded), entity.FurtherAnalysisNeeded);
            Assert.AreEqual(refinedSectionProbability, entity.RefinedSectionProbability);
        }

        [Test]
        public void Create_SectionResultWithNaNValues_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                ManualInitialFailureMechanismResultSectionProbability = double.NaN,
                RefinedSectionProbability = double.NaN
            };

            // Call
            var entity = sectionResult.Create<TestStructuresFailureMechanismSectionResultEntity>();

            // Assert
            Assert.IsNull(entity.ManualInitialFailureMechanismResultSectionProbability);
            Assert.IsNull(entity.RefinedSectionProbability);
        }

        private class TestStructuresFailureMechanismSectionResultEntity : IStructuresSectionResultEntity
        {
            public byte IsRelevant { get; set; }
            public byte InitialFailureMechanismResultType { get; set; }
            public double? ManualInitialFailureMechanismResultSectionProbability { get; set; }
            public byte FurtherAnalysisNeeded { get; set; }
            public double? RefinedSectionProbability { get; set; }
            public FailureMechanismSectionEntity FailureMechanismSectionEntity { get; set; }
        }
    }
}