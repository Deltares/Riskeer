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
using Application.Ringtoets.Storage.Read.ClosingStructures;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;

namespace Application.Ringtoets.Storage.Test.Read.ClosingStructures
{
    [TestFixture]
    public class ClosingStructuresSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_SectionResultIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new ClosingStructuresSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(null, new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new ClosingStructuresSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(new StructuresFailureMechanismSectionResult<ClosingStructuresInput>(
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
            var layerOne = random.NextEnumValue<AssessmentLayerOneState>();
            double layerThree = random.NextDouble();

            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new ClosingStructuresSectionResultEntity
            {
                LayerThree = layerThree,
                LayerOne = Convert.ToByte(layerOne),
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new StructuresFailureMechanismSectionResult<ClosingStructuresInput>(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.AreEqual(layerOne, sectionResult.AssessmentLayerOne);
            Assert.AreEqual(layerThree, sectionResult.AssessmentLayerThree, 1e-6);
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void Read_EntityWithNullValues_SectionResultWithNaNValues()
        {
            // Setup
            var random = new Random(21);
            var layerOne = random.NextEnumValue<AssessmentLayerOneState>();

            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new ClosingStructuresSectionResultEntity
            {
                LayerThree = null,
                LayerOne = Convert.ToByte(random.NextEnumValue<AssessmentLayerOneState>()),
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new StructuresFailureMechanismSectionResult<ClosingStructuresInput>(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.AreEqual(layerOne, sectionResult.AssessmentLayerOne);
            Assert.IsNaN(sectionResult.AssessmentLayerThree);
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void Read_CalculationEntitySet_ReturnClosingStructuresSectionResultWithCalculation()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var calculationEntity = new ClosingStructuresCalculationEntity();

            var collector = new ReadConversionCollector();
            collector.Read(calculationEntity, calculation);

            var entity = new ClosingStructuresSectionResultEntity
            {
                ClosingStructuresCalculationEntity = calculationEntity
            };
            var sectionResult = new StructuresFailureMechanismSectionResult<ClosingStructuresInput>(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.AreSame(calculation, sectionResult.Calculation);
        }
    }
}