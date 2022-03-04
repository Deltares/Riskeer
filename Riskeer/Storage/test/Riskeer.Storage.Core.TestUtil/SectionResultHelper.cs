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
using Riskeer.Common.Primitives;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.TestUtil
{
    public static class SectionResultHelper
    {
        public static void SetSectionResult(IAdoptableFailureMechanismSectionResultEntity sectionResult)
        {
            var random = new Random(21);

            sectionResult.IsRelevant = Convert.ToByte(random.NextBoolean());
            sectionResult.InitialFailureMechanismResultType = Convert.ToByte(random.NextEnumValue<AdoptableInitialFailureMechanismResultType>());
            sectionResult.ManualInitialFailureMechanismResultSectionProbability = random.NextDouble();
            sectionResult.FurtherAnalysisType = Convert.ToByte(random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>());
            sectionResult.RefinedSectionProbability = random.NextDouble();
        }

        public static void AssertSectionResults(IAdoptableFailureMechanismSectionResultEntity sectionResultEntity, AdoptableFailureMechanismSectionResult sectionResult)
        {
            Assert.AreEqual(Convert.ToBoolean(sectionResultEntity.IsRelevant), sectionResult.IsRelevant);
            Assert.AreEqual((AdoptableInitialFailureMechanismResultType) sectionResultEntity.InitialFailureMechanismResultType, sectionResult.InitialFailureMechanismResultType);
            Assert.AreEqual(sectionResultEntity.ManualInitialFailureMechanismResultSectionProbability.ToNullAsNaN(), sectionResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual((FailureMechanismSectionResultFurtherAnalysisType) sectionResultEntity.FurtherAnalysisType, sectionResult.FurtherAnalysisType);
            Assert.AreEqual(sectionResultEntity.RefinedSectionProbability.ToNullAsNaN(), sectionResult.RefinedSectionProbability);
        }

        public static void SetSectionResult(IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResult)
        {
            var random = new Random(21);

            SetSectionResult((IAdoptableFailureMechanismSectionResultEntity) sectionResult);
            sectionResult.ManualInitialFailureMechanismResultProfileProbability = random.NextDouble();
            sectionResult.ProbabilityRefinementType = Convert.ToByte(random.NextEnumValue<ProbabilityRefinementType>());
            sectionResult.RefinedProfileProbability = random.NextDouble();
        }

        public static void AssertSectionResults(IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResultEntity, AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult)
        {
            AssertSectionResults((IAdoptableFailureMechanismSectionResultEntity) sectionResultEntity, sectionResult);

            Assert.AreEqual(sectionResultEntity.ManualInitialFailureMechanismResultProfileProbability.ToNullAsNaN(), sectionResult.ManualInitialFailureMechanismResultProfileProbability);
            Assert.AreEqual((ProbabilityRefinementType) sectionResultEntity.ProbabilityRefinementType, sectionResult.ProbabilityRefinementType);
            Assert.AreEqual(sectionResultEntity.RefinedProfileProbability.ToNullAsNaN(), sectionResult.RefinedProfileProbability);
        }

        public static void SetSectionResult(INonAdoptableFailureMechanismSectionResultEntity sectionResult)
        {
            var random = new Random(21);

            sectionResult.IsRelevant = Convert.ToByte(random.NextBoolean());
            sectionResult.InitialFailureMechanismResultType = Convert.ToByte(random.NextEnumValue<NonAdoptableInitialFailureMechanismResultType>());
            sectionResult.ManualInitialFailureMechanismResultSectionProbability = random.NextDouble();
            sectionResult.FurtherAnalysisType = Convert.ToByte(random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>());
            sectionResult.RefinedSectionProbability = random.NextDouble();
        }

        public static void AssertSectionResults(INonAdoptableFailureMechanismSectionResultEntity sectionResultEntity, NonAdoptableFailureMechanismSectionResult sectionResult)
        {
            Assert.AreEqual(Convert.ToBoolean(sectionResultEntity.IsRelevant), sectionResult.IsRelevant);
            Assert.AreEqual((NonAdoptableInitialFailureMechanismResultType) sectionResultEntity.InitialFailureMechanismResultType, sectionResult.InitialFailureMechanismResultType);
            Assert.AreEqual(sectionResultEntity.ManualInitialFailureMechanismResultSectionProbability.ToNullAsNaN(), sectionResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual((FailureMechanismSectionResultFurtherAnalysisType) sectionResultEntity.FurtherAnalysisType, sectionResult.FurtherAnalysisType);
            Assert.AreEqual(sectionResultEntity.RefinedSectionProbability.ToNullAsNaN(), sectionResult.RefinedSectionProbability);
        }

        public static void SetSectionResult(INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResult)
        {
            var random = new Random(21);

            SetSectionResult((INonAdoptableFailureMechanismSectionResultEntity) sectionResult);
            sectionResult.ManualInitialFailureMechanismResultProfileProbability = random.NextDouble();
            sectionResult.RefinedProfileProbability = random.NextDouble();
        }

        public static void AssertSectionResults(INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResultEntity, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult)
        {
            AssertSectionResults((INonAdoptableFailureMechanismSectionResultEntity) sectionResultEntity, sectionResult);

            Assert.AreEqual(sectionResultEntity.ManualInitialFailureMechanismResultProfileProbability.ToNullAsNaN(), sectionResult.ManualInitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(sectionResultEntity.RefinedProfileProbability.ToNullAsNaN(), sectionResult.RefinedProfileProbability);
        }
    }
}