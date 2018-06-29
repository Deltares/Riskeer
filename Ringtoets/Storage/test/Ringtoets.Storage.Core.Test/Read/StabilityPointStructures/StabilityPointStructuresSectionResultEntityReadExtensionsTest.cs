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
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;
using Ringtoets.Storage.Core.Read.StabilityPointStructures;
using Ringtoets.Storage.Core.TestUtil;

namespace Ringtoets.Storage.Core.Test.Read.StabilityPointStructures
{
    [TestFixture]
    public class StabilityPointStructuresSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((StabilityPointStructuresSectionResultEntity) null).Read(
                new StabilityPointStructuresFailureMechanismSectionResult(new TestFailureMechanismSection()),
                new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new StabilityPointStructuresSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(null, new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new StabilityPointStructuresSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(new StabilityPointStructuresFailureMechanismSectionResult(
                                                      new TestFailureMechanismSection()), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_ParameterValues_SectionResultWithParameterValues()
        {
            // Setup
            var random = new Random(21);
            var simpleAssessmentResult = random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>();
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            double tailorMadeAssessmentProbability = random.NextDouble();
            bool useManualAssemblyProbability = random.NextBoolean();
            double manualAssemblyProbability = random.NextDouble();

            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new StabilityPointStructuresSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity,
                SimpleAssessmentResult = Convert.ToByte(simpleAssessmentResult),
                DetailedAssessmentResult = Convert.ToByte(detailedAssessmentResult),
                TailorMadeAssessmentResult = Convert.ToByte(tailorMadeAssessmentResult),
                TailorMadeAssessmentProbability = tailorMadeAssessmentProbability,
                UseManualAssemblyProbability = Convert.ToByte(useManualAssemblyProbability),
                ManualAssemblyProbability = manualAssemblyProbability
            };
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.AreEqual(simpleAssessmentResult, sectionResult.SimpleAssessmentResult);
            Assert.AreEqual(detailedAssessmentResult, sectionResult.DetailedAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentResult, sectionResult.TailorMadeAssessmentResult);
            Assert.AreEqual(tailorMadeAssessmentProbability, sectionResult.TailorMadeAssessmentProbability, 1e-6);
            Assert.AreEqual(useManualAssemblyProbability, sectionResult.UseManualAssemblyProbability);
            Assert.AreEqual(manualAssemblyProbability, sectionResult.ManualAssemblyProbability, 1e-6);
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void Read_EntityWithNullValues_SectionResultWithNaNValues()
        {
            // Setup
            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new StabilityPointStructuresSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.IsNaN(sectionResult.TailorMadeAssessmentProbability);
            Assert.IsNaN(sectionResult.ManualAssemblyProbability);
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void Read_CalculationEntitySet_ReturnStabilityPointStructuresSectionResultWithCalculation()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var calculationEntity = new StabilityPointStructuresCalculationEntity();

            var collector = new ReadConversionCollector();
            collector.Read(calculationEntity, calculation);

            var entity = new StabilityPointStructuresSectionResultEntity
            {
                StabilityPointStructuresCalculationEntity = calculationEntity
            };
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.AreSame(calculation, sectionResult.Calculation);
        }
    }
}