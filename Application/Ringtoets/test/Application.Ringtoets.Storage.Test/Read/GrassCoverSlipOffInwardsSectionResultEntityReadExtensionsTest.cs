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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class GrassCoverSlipOffInwardsSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new GrassCoverSlipOffInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate test = () => ((GrassCoverSlipOffInwardsSectionResultEntity) null).Read(sectionResult);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new GrassCoverSlipOffInwardsSectionResultEntity();

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
            var manualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            bool useManualAssemblyCategoryGroup = random.NextBoolean();

            var entity = new GrassCoverSlipOffInwardsSectionResultEntity
            {
                SimpleAssessmentResult = Convert.ToByte(simpleAssessmentResult),
                DetailedAssessmentResult = Convert.ToByte(detailedAssessmentResult),
                TailorMadeAssessmentResult = Convert.ToByte(tailorMadeAssessmentResult),
                UseManualAssemblyCategoryGroup = Convert.ToByte(useManualAssemblyCategoryGroup),
                ManualAssemblyCategoryGroup = Convert.ToByte(manualAssemblyCategoryGroup)
            };
            var sectionResult = new GrassCoverSlipOffInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.AreEqual(simpleAssessmentResult, sectionResult.SimpleAssessmentResult);
            Assert.AreEqual(detailedAssessmentResult, sectionResult.DetailedAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentResult, sectionResult.TailorMadeAssessmentResult);
            Assert.AreEqual(useManualAssemblyCategoryGroup, sectionResult.UseManualAssemblyCategoryGroup);
            Assert.AreEqual(manualAssemblyCategoryGroup, sectionResult.ManualAssemblyCategoryGroup);
        }
    }
}