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
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Storage.Core.Test.Read.MacroStabilityOutwards
{
    [TestFixture]
    public class MacroStabilityOutwardsSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            TestDelegate call = () => ((MacroStabilityOutwardsSectionResultEntity) null).Read(sectionResult);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new MacroStabilityOutwardsSectionResultEntity();

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
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            double detailedAssessmentProbability = random.NextDouble();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>();
            double tailorMadeAssessmentProbability = random.NextDouble();
            bool useManualAssemblyCategoryGroup = random.NextBoolean();
            var manualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            
            var entity = new MacroStabilityOutwardsSectionResultEntity
            {
                SimpleAssessmentResult = Convert.ToByte(simpleAssessmentResult),
                DetailedAssessmentResult = Convert.ToByte(detailedAssessmentResult),
                DetailedAssessmentProbability = detailedAssessmentProbability,
                TailorMadeAssessmentResult = Convert.ToByte(tailorMadeAssessmentResult),
                TailorMadeAssessmentProbability = tailorMadeAssessmentProbability,
                UseManualAssemblyCategoryGroup = Convert.ToByte(useManualAssemblyCategoryGroup),
                ManualAssemblyCategoryGroup = Convert.ToByte(manualAssemblyCategoryGroup)
            };
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.AreEqual(simpleAssessmentResult, sectionResult.SimpleAssessmentResult);
            Assert.AreEqual(detailedAssessmentResult, sectionResult.DetailedAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentResult, sectionResult.TailorMadeAssessmentResult);
            Assert.AreEqual(useManualAssemblyCategoryGroup, sectionResult.UseManualAssemblyCategoryGroup);
            Assert.AreEqual(manualAssemblyCategoryGroup, sectionResult.ManualAssemblyCategoryGroup);
        }

        [Test]
        public void Read_EntityWithNullValues_SetsSectionResultWithNaNValues()
        {
            // Setup
            var entity = new MacroStabilityOutwardsSectionResultEntity();
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.IsNaN(sectionResult.DetailedAssessmentProbability);
            Assert.IsNaN(sectionResult.TailorMadeAssessmentProbability);
        }
    }
}