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
using Application.Ringtoets.Storage.Create.GrassCoverErosionOutwards;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionOutwards
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((GrassCoverErosionOutwardsFailureMechanismSectionResult) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_WithDifferentResults_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var random = new Random(21);
            var simpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
            var detailedAssessmentResultForFactorizedSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForMechanismSpecificLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForFactorizedLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var tailorMadeAssessmentResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            bool useManualAssemblyCategoryGroup = random.NextBoolean();
            var manualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            var sectionResult = new GrassCoverErosionOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                DetailedAssessmentResultForFactorizedSignalingNorm = detailedAssessmentResultForFactorizedSignalingNorm,
                DetailedAssessmentResultForSignalingNorm = detailedAssessmentResultForSignalingNorm,
                DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                DetailedAssessmentResultForLowerLimitNorm = detailedAssessmentResultForLowerLimitNorm,
                DetailedAssessmentResultForFactorizedLowerLimitNorm = detailedAssessmentResultForFactorizedLowerLimitNorm,
                TailorMadeAssessmentResult = tailorMadeAssessmentResult,
                UseManualAssemblyCategoryGroup = useManualAssemblyCategoryGroup,
                ManualAssemblyCategoryGroup = manualAssemblyCategoryGroup
            };

            // Call
            GrassCoverErosionOutwardsSectionResultEntity entity = sectionResult.Create();

            // Assert
            Assert.AreEqual(Convert.ToByte(simpleAssessmentResult), entity.SimpleAssessmentResult);
            Assert.AreEqual(Convert.ToByte(detailedAssessmentResultForFactorizedSignalingNorm), entity.DetailedAssessmentResultForFactorizedSignalingNorm);
            Assert.AreEqual(Convert.ToByte(detailedAssessmentResultForSignalingNorm), entity.DetailedAssessmentResultForSignalingNorm);
            Assert.AreEqual(Convert.ToByte(detailedAssessmentResultForMechanismSpecificLowerLimitNorm), entity.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(Convert.ToByte(detailedAssessmentResultForLowerLimitNorm), entity.DetailedAssessmentResultForLowerLimitNorm);
            Assert.AreEqual(Convert.ToByte(detailedAssessmentResultForFactorizedLowerLimitNorm), entity.DetailedAssessmentResultForFactorizedLowerLimitNorm);
            Assert.AreEqual(Convert.ToByte(tailorMadeAssessmentResult), entity.TailorMadeAssessmentResult);
            Assert.AreEqual(Convert.ToByte(useManualAssemblyCategoryGroup), entity.UseManualAssemblyCategoryGroup);
            Assert.AreEqual(Convert.ToByte(manualAssemblyCategoryGroup), entity.ManualAssemblyCategoryGroup);
        }
    }
}