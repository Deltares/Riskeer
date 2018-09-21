// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;

namespace Ringtoets.Storage.Core.Test.Read
{
    [TestFixture]
    public class WaterPressureAsphaltCoverSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new WaterPressureAsphaltCoverFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            TestDelegate call = () => ((WaterPressureAsphaltCoverSectionResultEntity) null).Read(sectionResult);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new WaterPressureAsphaltCoverSectionResultEntity();

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
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();
            bool useManualAssembly = random.NextBoolean();
            var manualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            var entity = new WaterPressureAsphaltCoverSectionResultEntity
            {
                SimpleAssessmentResult = Convert.ToByte(simpleAssessmentResult),
                TailorMadeAssessmentResult = Convert.ToByte(tailorMadeAssessmentResult),
                UseManualAssembly = Convert.ToByte(useManualAssembly),
                ManualAssemblyCategoryGroup = Convert.ToByte(manualAssemblyCategoryGroup)
            };
            var sectionResult = new WaterPressureAsphaltCoverFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.AreEqual(simpleAssessmentResult, sectionResult.SimpleAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentResult, sectionResult.TailorMadeAssessmentResult);
            Assert.AreEqual(useManualAssembly, sectionResult.UseManualAssembly);
            Assert.AreEqual(manualAssemblyCategoryGroup, sectionResult.ManualAssemblyCategoryGroup);
        }
    }
}