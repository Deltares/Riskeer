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
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class StrengthStabilityLengthwiseConstructionSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new StrengthStabilityLengthwiseConstructionSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithDecimalParameterValues_ReturnStrengthStabilityLengthwiseConstructionSoilLayerWithDoubleParameterValues(bool layerOne)
        {
            // Setup
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            double layerThree = random.NextDouble();
            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new StrengthStabilityLengthwiseConstructionSectionResultEntity
            {
                StrengthStabilityLengthwiseConstructionSectionResultEntityId = entityId,
                LayerThree = Convert.ToDecimal(layerThree),
                LayerOne = Convert.ToByte(layerOne),
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };

            // Call
            var result = entity.Read(collector);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(entityId, result.StorageId);
            Assert.AreEqual(layerOne, result.AssessmentLayerOne);
            Assert.AreEqual(layerThree, result.AssessmentLayerThree, 1e-6);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithNullParameterValues_ReturnStrengthStabilityLengthwiseConstructionSoilLayerWithNullParameters(bool layerOne)
        {
            // Setup
            var collector = new ReadConversionCollector();
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new StrengthStabilityLengthwiseConstructionSectionResultEntity
            {
                LayerOne = Convert.ToByte(layerOne),
                LayerThree = null,
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };

            // Call
            var layer = entity.Read(collector);

            // Assert
            Assert.IsNaN(layer.AssessmentLayerThree);
        } 
    }
}