﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Read.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.TestUtil;

using NUnit.Framework;

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Read.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_SectionResultIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsSectionResultEntity();

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
            var entity = new GrassCoverErosionInwardsSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection()), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithDecimalParameterValues_ReturnGrassCoverErosionInwardsSectionResultWithDoubleParameterValues(bool layerOne)
        {
            // Setup
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            double layerThree = random.NextDouble();
            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = entityId,
                LayerThree = Convert.ToDecimal(layerThree),
                LayerOne = Convert.ToByte(layerOne),
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.IsNotNull(sectionResult);
            Assert.AreEqual(entityId, sectionResult.StorageId);
            Assert.AreEqual(layerOne, sectionResult.AssessmentLayerOne);
            Assert.AreEqual(layerThree, sectionResult.AssessmentLayerThree, 1e-6);

            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithNullParameterValues_ReturnGrassCoverErosionInwardsSectionResultWithNullParameters(bool layerOne)
        {
            // Setup
            var collector = new ReadConversionCollector();
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new GrassCoverErosionInwardsSectionResultEntity
            {
                LayerOne = Convert.ToByte(layerOne),
                LayerThree = null,
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.IsNaN(sectionResult.AssessmentLayerThree);
        }

        [Test]
        public void Read_CalculationEntitySet_ReturnGrassCoverErosionInwardsSectionResultWithCalculation()
        {
            // Setup
            var random = new Random(21);
            var entityId = random.Next(1, 502);

            var calculation = new GrassCoverErosionInwardsCalculation();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            var calculationEntity = new GrassCoverErosionInwardsCalculationEntity();

            var collector = new ReadConversionCollector();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            collector.Read(calculationEntity, calculation);

            var entity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = entityId,
                FailureMechanismSectionEntity = failureMechanismSectionEntity,
                GrassCoverErosionInwardsCalculationEntity = calculationEntity
            };
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.AreSame(calculation, sectionResult.Calculation);
        }
    }
}