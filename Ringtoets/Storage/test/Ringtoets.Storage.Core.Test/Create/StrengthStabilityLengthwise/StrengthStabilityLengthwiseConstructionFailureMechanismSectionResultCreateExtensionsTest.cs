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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Storage.Core.Create.StrengthStabilityLengthwise;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.StrengthStabilityLengthwise
{
    [TestFixture]
    public class StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult) null).Create();

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
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();
            bool useManualAssemblyCategoryGroup = random.NextBoolean();
            var manualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                TailorMadeAssessmentResult = tailorMadeAssessmentResult,
                UseManualAssemblyCategoryGroup = useManualAssemblyCategoryGroup,
                ManualAssemblyCategoryGroup = manualAssemblyCategoryGroup
            };

            // Call
            StrengthStabilityLengthwiseConstructionSectionResultEntity entity = sectionResult.Create();

            // Assert
            Assert.AreEqual(Convert.ToByte(simpleAssessmentResult), entity.SimpleAssessmentResult);
            Assert.AreEqual(Convert.ToByte(tailorMadeAssessmentResult), entity.TailorMadeAssessmentResult);
            Assert.AreEqual(Convert.ToByte(useManualAssemblyCategoryGroup), entity.UseManualAssemblyCategoryGroup);
            Assert.AreEqual(Convert.ToByte(manualAssemblyCategoryGroup), entity.ManualAssemblyCategoryGroup);
        }
    }
}