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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.StabilityStoneCover;

namespace Ringtoets.Storage.Core.Test.Read.StabilityStoneCover
{
    [TestFixture]
    public class StabilityStoneCoverSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((StabilityStoneCoverSectionResultEntity) null).Read(
                new StabilityStoneCoverFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new StabilityStoneCoverSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Read_ParameterValues_SectionResultWithParameterValues()
        {
            // Setup
            var random = new Random(21);
            var simpleAssessmentResult = random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>();
            var detailedAssessmentResultForFactorizedSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForMechanismSpecificLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForFactorizedLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>();
            bool useManualAssembly = random.NextBoolean();
            var manualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            var entity = new StabilityStoneCoverSectionResultEntity
            {
                FailureMechanismSectionEntity = new FailureMechanismSectionEntity(),
                SimpleAssessmentResult = Convert.ToByte(simpleAssessmentResult),
                DetailedAssessmentResultForFactorizedSignalingNorm = Convert.ToByte(detailedAssessmentResultForFactorizedSignalingNorm),
                DetailedAssessmentResultForSignalingNorm = Convert.ToByte(detailedAssessmentResultForSignalingNorm),
                DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = Convert.ToByte(detailedAssessmentResultForMechanismSpecificLowerLimitNorm),
                DetailedAssessmentResultForLowerLimitNorm = Convert.ToByte(detailedAssessmentResultForLowerLimitNorm),
                DetailedAssessmentResultForFactorizedLowerLimitNorm = Convert.ToByte(detailedAssessmentResultForFactorizedLowerLimitNorm),
                TailorMadeAssessmentResult = Convert.ToByte(tailorMadeAssessmentResult),
                UseManualAssembly = Convert.ToByte(useManualAssembly),
                ManualAssemblyCategoryGroup = Convert.ToByte(manualAssemblyCategoryGroup)
            };

            var sectionResult = new StabilityStoneCoverFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.AreEqual(simpleAssessmentResult, sectionResult.SimpleAssessmentResult);
            Assert.AreEqual(detailedAssessmentResultForFactorizedSignalingNorm, sectionResult.DetailedAssessmentResultForFactorizedSignalingNorm);
            Assert.AreEqual(detailedAssessmentResultForSignalingNorm, sectionResult.DetailedAssessmentResultForSignalingNorm);
            Assert.AreEqual(detailedAssessmentResultForMechanismSpecificLowerLimitNorm, sectionResult.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(detailedAssessmentResultForLowerLimitNorm, sectionResult.DetailedAssessmentResultForLowerLimitNorm);
            Assert.AreEqual(detailedAssessmentResultForFactorizedLowerLimitNorm, sectionResult.DetailedAssessmentResultForFactorizedLowerLimitNorm);
            Assert.AreEqual(tailorMadeAssessmentResult, sectionResult.TailorMadeAssessmentResult);
            Assert.AreEqual(useManualAssembly, sectionResult.UseManualAssembly);
            Assert.AreEqual(manualAssemblyCategoryGroup, sectionResult.ManualAssemblyCategoryGroup);
        }
    }
}