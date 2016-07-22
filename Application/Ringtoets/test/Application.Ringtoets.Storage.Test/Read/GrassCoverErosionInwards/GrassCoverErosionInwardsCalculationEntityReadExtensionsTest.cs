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

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;

using NUnit.Framework;

using Ringtoets.GrassCoverErosionInwards.Data;

using Application.Ringtoets.Storage.Read.GrassCoverErosionInwards;

using Core.Common.Base.Geometry;

using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Read.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase(true, BreakWaterType.Dam)]
        [TestCase(false, BreakWaterType.Wall)]
        public void Read_ValidEntity_ReturnCalculation(bool flagValue, BreakWaterType type)
        {
            // Setup
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = 457,
                Name = "sodhfksn",
                Comments = "s;ohfgwjo5p09u",
                GrassCoverErosionInwardsOutputEntity = null,
                HydraulicLocationEntity = null,
                DikeProfileEntity = null,
                Orientation = 5.6,
                CriticalFlowRateMean = 3.4,
                CriticalFlowRateStandardDeviation = 1.2,
                UseForeshore = Convert.ToByte(flagValue),
                DikeHeight = 2.3,
                UseBreakWater = Convert.ToByte(flagValue),
                BreakWaterType = Convert.ToInt16(type),
                BreakWaterHeight = 5.7,
                CalculateDikeHeight = Convert.ToByte(flagValue)
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.GrassCoverErosionInwardsCalculationEntityId, calculation.StorageId);
            Assert.AreEqual(entity.Name, calculation.Name);
            Assert.AreEqual(entity.Comments, calculation.Comments);

            GrassCoverErosionInwardsInput input = calculation.InputParameters;
            Assert.AreEqual(entity.Orientation, input.Orientation.Value);
            Assert.AreEqual(entity.CriticalFlowRateMean, input.CriticalFlowRate.Mean.Value);
            Assert.AreEqual(entity.CriticalFlowRateStandardDeviation, input.CriticalFlowRate.StandardDeviation.Value);
            Assert.AreEqual(flagValue, input.UseForeshore);
            Assert.AreEqual(entity.DikeHeight, input.DikeHeight.Value);
            Assert.AreEqual(flagValue, input.UseBreakWater);
            Assert.AreEqual(type, input.BreakWater.Type);
            Assert.AreEqual(entity.BreakWaterHeight, input.BreakWater.Height.Value);
            Assert.AreEqual(flagValue, input.CalculateDikeHeight);

            Assert.IsNull(input.DikeProfile);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnCalculationWithNaNOrNull()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = 123456,
                Name = null,
                Comments = null,
                Orientation = null,
                CriticalFlowRateMean = null,
                CriticalFlowRateStandardDeviation = null,
                DikeHeight = null,
                BreakWaterHeight = null,
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.GrassCoverErosionInwardsCalculationEntityId, calculation.StorageId);
            Assert.IsNull(calculation.Name);
            Assert.IsNull(calculation.Comments);

            GrassCoverErosionInwardsInput input = calculation.InputParameters;
            Assert.IsNaN(input.Orientation);
            Assert.IsNaN(input.CriticalFlowRate.Mean);
            Assert.IsNaN(input.CriticalFlowRate.StandardDeviation);
            Assert.IsNaN(input.DikeHeight);
            Assert.IsNaN(input.BreakWater.Height.Value);

            Assert.IsNull(input.DikeProfile);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void Create_ValidEntityWithAlreadyReadDikeProfileEntity_ReturnCalculationWithReadDikeProfile()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());
            var dikeProfileEntity = new DikeProfileEntity
            {
                DikeProfileEntityId = 7465
            };
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                DikeProfileEntity = dikeProfileEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(dikeProfileEntity, dikeProfile);

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(dikeProfile, calculation.InputParameters.DikeProfile);
        }

        [Test]
        public void Create_ValidEntityWithUnreadDikeProfileEntity_ReturnCalculationWithNewDikeProfile()
        {
            // Setup
            var dikeProfileEntity = new DikeProfileEntity
            {
                DikeProfileEntityId = 7465,
                DikeGeometryData = new RoughnessPointBinaryConverter().ToBytes(new RoughnessPoint[0]),
                ForeShoreData = new Point2DBinaryConverter().ToBytes(new Point2D[0])
            };
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                DikeProfileEntity = dikeProfileEntity
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.InputParameters.DikeProfile);
            Assert.IsTrue(collector.Contains(dikeProfileEntity));
        }

        [Test]
        public void Create_ValidEntityWithAlreadyReadHydraulicLocationEntity_ReturnCalculationWithReadHydraulicBoundaryLocation()
        {
            // Setup
            var hydroLocation = new HydraulicBoundaryLocation(1, "A", 0, 0);
            var hydroLocationEntity = new HydraulicLocationEntity();
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                HydraulicLocationEntity = hydroLocationEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydroLocationEntity, hydroLocation);

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydroLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Create_ValidEntityWithUnreadHydraulicLocationEntity_ReturnCalculationWithNewHydraulicBoundaryLocation()
        {
            // Setup
            var hydroLocationEntity = new HydraulicLocationEntity
            {
                Name = "A"
            };
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                HydraulicLocationEntity = hydroLocationEntity
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsTrue(collector.Contains(hydroLocationEntity));
        }

        [Test]
        public void Read_ValidEntityWithOutputEntity_ReturnCalculationWithOutput()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = 456,
                GrassCoverErosionInwardsOutputEntity = new GrassCoverErosionInwardsOutputEntity
                {
                    GrassCoverErosionInwardsOutputId = 9745,
                    ProbabilisticOutputEntity = new ProbabilisticOutputEntity
                    {
                        ProbabilisticOutputEntityId = 3245
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(calculation.HasOutput);
        }
    }
}