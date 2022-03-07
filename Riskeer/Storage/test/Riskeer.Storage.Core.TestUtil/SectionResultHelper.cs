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
    /// <summary>
    /// This class is used to set and assert section results.
    /// </summary>
    public static class SectionResultHelper
    {
        #region Set methods

        /// <summary>
        /// Sets random section results for a <see cref="IAdoptableFailureMechanismSectionResultEntity"/>. 
        /// </summary>
        /// <param name="sectionResult">The <see cref="IAdoptableFailureMechanismSectionResultEntity"/>.</param>
        public static void SetSectionResult(IAdoptableFailureMechanismSectionResultEntity sectionResult)
        {
            var random = new Random(21);

            sectionResult.IsRelevant = Convert.ToByte(random.NextBoolean());
            sectionResult.InitialFailureMechanismResultType = Convert.ToByte(random.NextEnumValue<AdoptableInitialFailureMechanismResultType>());
            sectionResult.ManualInitialFailureMechanismResultSectionProbability = random.NextDouble();
            sectionResult.FurtherAnalysisType = Convert.ToByte(random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>());
            sectionResult.RefinedSectionProbability = random.NextDouble();
        }

        /// <summary>
        /// Sets random section results for a <see cref="IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/>. 
        /// </summary>
        /// <param name="sectionResult">The <see cref="IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/>.</param>
        public static void SetSectionResult(IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResult)
        {
            var random = new Random(21);

            SetSectionResult((IAdoptableFailureMechanismSectionResultEntity) sectionResult);
            sectionResult.ManualInitialFailureMechanismResultProfileProbability = random.NextDouble();
            sectionResult.ProbabilityRefinementType = Convert.ToByte(random.NextEnumValue<ProbabilityRefinementType>());
            sectionResult.RefinedProfileProbability = random.NextDouble();
        }

        /// <summary>
        /// Sets random section results for a <see cref="INonAdoptableFailureMechanismSectionResultEntity"/>. 
        /// </summary>
        /// <param name="sectionResult">The <see cref="INonAdoptableFailureMechanismSectionResultEntity"/>.</param>
        public static void SetSectionResult(INonAdoptableFailureMechanismSectionResultEntity sectionResult)
        {
            var random = new Random(21);

            sectionResult.IsRelevant = Convert.ToByte(random.NextBoolean());
            sectionResult.InitialFailureMechanismResultType = Convert.ToByte(random.NextEnumValue<NonAdoptableInitialFailureMechanismResultType>());
            sectionResult.ManualInitialFailureMechanismResultSectionProbability = random.NextDouble();
            sectionResult.FurtherAnalysisType = Convert.ToByte(random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>());
            sectionResult.RefinedSectionProbability = random.NextDouble();
        }

        /// <summary>
        /// Sets random section results for a <see cref="INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/>. 
        /// </summary>
        /// <param name="sectionResult">The <see cref="INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/>.</param>
        public static void SetSectionResult(INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResult)
        {
            var random = new Random(21);

            SetSectionResult((INonAdoptableFailureMechanismSectionResultEntity) sectionResult);
            sectionResult.ManualInitialFailureMechanismResultProfileProbability = random.NextDouble();
            sectionResult.RefinedProfileProbability = random.NextDouble();
        }

        #endregion

        #region Assert methods

        /// <summary>
        /// Asserts whether the data of the <see cref="IAdoptableFailureMechanismSectionResultEntity"/> and
        /// <see cref="AdoptableFailureMechanismSectionResult"/> are equal.
        /// </summary>
        /// <param name="sectionResultEntity">The <see cref="IAdoptableFailureMechanismSectionResultEntity"/>.</param>
        /// <param name="sectionResult">The <see cref="AdoptableFailureMechanismSectionResult"/>.</param>
        public static void AssertSectionResult(IAdoptableFailureMechanismSectionResultEntity sectionResultEntity, AdoptableFailureMechanismSectionResult sectionResult)
        {
            Assert.AreEqual(Convert.ToBoolean(sectionResultEntity.IsRelevant), sectionResult.IsRelevant);
            Assert.AreEqual((AdoptableInitialFailureMechanismResultType) sectionResultEntity.InitialFailureMechanismResultType, sectionResult.InitialFailureMechanismResultType);
            Assert.AreEqual(sectionResultEntity.ManualInitialFailureMechanismResultSectionProbability.ToNullAsNaN(), sectionResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual((FailureMechanismSectionResultFurtherAnalysisType) sectionResultEntity.FurtherAnalysisType, sectionResult.FurtherAnalysisType);
            Assert.AreEqual(sectionResultEntity.RefinedSectionProbability.ToNullAsNaN(), sectionResult.RefinedSectionProbability);
        }

        /// <summary>
        /// Asserts whether the data of the <see cref="IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/> and
        /// <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/> are equal.
        /// </summary>
        /// <param name="sectionResultEntity">The <see cref="IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/>.</param>
        /// <param name="sectionResult">The <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/>.</param>
        public static void AssertSectionResult(IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResultEntity, AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult)
        {
            AssertSectionResult((IAdoptableFailureMechanismSectionResultEntity) sectionResultEntity, sectionResult);

            Assert.AreEqual(sectionResultEntity.ManualInitialFailureMechanismResultProfileProbability.ToNullAsNaN(), sectionResult.ManualInitialFailureMechanismResultProfileProbability);
            Assert.AreEqual((ProbabilityRefinementType) sectionResultEntity.ProbabilityRefinementType, sectionResult.ProbabilityRefinementType);
            Assert.AreEqual(sectionResultEntity.RefinedProfileProbability.ToNullAsNaN(), sectionResult.RefinedProfileProbability);
        }

        /// <summary>
        /// Asserts whether the data of the <see cref="INonAdoptableFailureMechanismSectionResultEntity"/> and
        /// <see cref="NonAdoptableFailureMechanismSectionResult"/> are equal.
        /// </summary>
        /// <param name="sectionResultEntity">The <see cref="INonAdoptableFailureMechanismSectionResultEntity"/>.</param>
        /// <param name="sectionResult">The <see cref="NonAdoptableFailureMechanismSectionResult"/>.</param>
        public static void AssertSectionResult(INonAdoptableFailureMechanismSectionResultEntity sectionResultEntity, NonAdoptableFailureMechanismSectionResult sectionResult)
        {
            Assert.AreEqual(Convert.ToBoolean(sectionResultEntity.IsRelevant), sectionResult.IsRelevant);
            Assert.AreEqual((NonAdoptableInitialFailureMechanismResultType) sectionResultEntity.InitialFailureMechanismResultType, sectionResult.InitialFailureMechanismResultType);
            Assert.AreEqual(sectionResultEntity.ManualInitialFailureMechanismResultSectionProbability.ToNullAsNaN(), sectionResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual((FailureMechanismSectionResultFurtherAnalysisType) sectionResultEntity.FurtherAnalysisType, sectionResult.FurtherAnalysisType);
            Assert.AreEqual(sectionResultEntity.RefinedSectionProbability.ToNullAsNaN(), sectionResult.RefinedSectionProbability);
        }

        /// <summary>
        /// Asserts whether the data of the <see cref="INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/> and
        /// <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/> are equal.
        /// </summary>
        /// <param name="sectionResultEntity">The <see cref="INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/>.</param>
        /// <param name="sectionResult">The <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/>.</param>
        public static void AssertSectionResult(INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResultEntity, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult)
        {
            AssertSectionResult((INonAdoptableFailureMechanismSectionResultEntity) sectionResultEntity, sectionResult);

            Assert.AreEqual(sectionResultEntity.ManualInitialFailureMechanismResultProfileProbability.ToNullAsNaN(), sectionResult.ManualInitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(sectionResultEntity.RefinedProfileProbability.ToNullAsNaN(), sectionResult.RefinedProfileProbability);
        }

        #endregion
    }
}