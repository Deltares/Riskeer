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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.HeightStructures;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;

namespace Application.Ringtoets.Storage.Test.Read.HeightStructures
{
    [TestFixture]
    public class HeightStructuresSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_SectionResultIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new HeightStructuresSectionResultEntity();

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
            var entity = new HeightStructuresSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(new HeightStructuresFailureMechanismSectionResult(
                                                      new TestFailureMechanismSection()), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [Combinatorial]
        public void Read_ParameterValues_SectionResultWithParameterValues(
            [Values(AssessmentLayerOneState.NotAssessed, AssessmentLayerOneState.NoVerdict,
                AssessmentLayerOneState.Sufficient)] AssessmentLayerOneState layerOne,
            [Values(AssessmentLayerTwoAResult.NotCalculated, AssessmentLayerTwoAResult.Failed,
                AssessmentLayerTwoAResult.Successful)] AssessmentLayerTwoAResult layerTwoA,
            [Values(1.1, 2.2, null)] double? layerThree)
        {
            // Setup
            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new HeightStructuresSectionResultEntity
            {
                LayerOne = Convert.ToByte(layerOne),
                LayerThree = layerThree,
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.AreEqual(layerOne, sectionResult.AssessmentLayerOne);
            Assert.AreEqual(layerThree ?? double.NaN, sectionResult.AssessmentLayerThree, 1e-6);
            Assert.IsNotNull(sectionResult);
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void Read_CalculationEntitySet_ReturnHeightStructuresSectionResultWithCalculation()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var calculationEntity = new HeightStructuresCalculationEntity();

            var collector = new ReadConversionCollector();
            collector.Read(calculationEntity, calculation);

            var entity = new HeightStructuresSectionResultEntity
            {
                HeightStructuresCalculationEntity = calculationEntity
            };
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.AreSame(calculation, sectionResult.Calculation);
        }
    }
}