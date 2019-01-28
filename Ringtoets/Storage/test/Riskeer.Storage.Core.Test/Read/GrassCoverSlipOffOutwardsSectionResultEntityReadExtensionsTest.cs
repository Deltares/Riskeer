﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class GrassCoverSlipOffOutwardsSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new GrassCoverSlipOffOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            TestDelegate call = () => ((GrassCoverSlipOffOutwardsSectionResultEntity) null).Read(sectionResult);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new GrassCoverSlipOffOutwardsSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Read_ParameterValues_SetsSectionResultWithParameterValues()
        {
            // Setup
            var random = new Random(31);
            var simpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentResultType>();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();
            bool useManualAssembly = random.NextBoolean();
            var manualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            var entity = new GrassCoverSlipOffOutwardsSectionResultEntity
            {
                SimpleAssessmentResult = Convert.ToByte(simpleAssessmentResult),
                DetailedAssessmentResult = Convert.ToByte(detailedAssessmentResult),
                TailorMadeAssessmentResult = Convert.ToByte(tailorMadeAssessmentResult),
                UseManualAssembly = Convert.ToByte(useManualAssembly),
                ManualAssemblyCategoryGroup = Convert.ToByte(manualAssemblyCategoryGroup)
            };
            var sectionResult = new GrassCoverSlipOffOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.AreEqual(simpleAssessmentResult, sectionResult.SimpleAssessmentResult);
            Assert.AreEqual(detailedAssessmentResult, sectionResult.DetailedAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentResult, sectionResult.TailorMadeAssessmentResult);
            Assert.AreEqual(useManualAssembly, sectionResult.UseManualAssembly);
            Assert.AreEqual(manualAssemblyCategoryGroup, sectionResult.ManualAssemblyCategoryGroup);
        }
    }
}