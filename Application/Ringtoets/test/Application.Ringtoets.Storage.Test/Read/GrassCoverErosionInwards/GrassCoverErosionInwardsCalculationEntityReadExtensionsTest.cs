// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;

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
        public void Read_ValidEntity_ReturnCalculation()
        {
            // Setup
            var random = new Random(14);
            bool flagValue = random.NextBoolean();
            BreakWaterType breakWaterType = random.NextEnumValue<BreakWaterType>();
            DikeHeightCalculationType dikeHeightCalculationType = random.NextEnumValue<DikeHeightCalculationType>();
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                Name = "sodhfksn",
                Comments = "s;ohfgwjo5p09u",
                HydraulicLocationEntity = null,
                DikeProfileEntity = null,
                Orientation = 5.6,
                CriticalFlowRateMean = 3.4,
                CriticalFlowRateStandardDeviation = 1.2,
                UseForeshore = Convert.ToByte(flagValue),
                DikeHeight = 2.3,
                UseBreakWater = Convert.ToByte(flagValue),
                BreakWaterType = Convert.ToInt16(breakWaterType),
                BreakWaterHeight = 5.7,
                DikeHeightCalculationType = Convert.ToByte(dikeHeightCalculationType)
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Name, calculation.Name);
            Assert.AreEqual(entity.Comments, calculation.Comments.Body);

            GrassCoverErosionInwardsInput input = calculation.InputParameters;
            Assert.AreEqual(entity.Orientation, input.Orientation.Value);
            Assert.AreEqual(entity.CriticalFlowRateMean, input.CriticalFlowRate.Mean.Value);
            Assert.AreEqual(entity.CriticalFlowRateStandardDeviation, input.CriticalFlowRate.StandardDeviation.Value);
            Assert.AreEqual(flagValue, input.UseForeshore);
            Assert.AreEqual(entity.DikeHeight, input.DikeHeight.Value);
            Assert.AreEqual(flagValue, input.UseBreakWater);
            Assert.AreEqual(breakWaterType, input.BreakWater.Type);
            Assert.AreEqual(entity.BreakWaterHeight, input.BreakWater.Height.Value);
            Assert.AreEqual(dikeHeightCalculationType, input.DikeHeightCalculationType);

            Assert.IsNull(input.DikeProfile);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void Read_EntityNotReadBefore_RegisterEntity()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsCalculationEntity();

            var collector = new ReadConversionCollector();

            // Precondition
            Assert.IsFalse(collector.Contains(entity));

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(entity));
            Assert.AreSame(calculation, collector.Get(entity));
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnCalculationWithNaNOrNull()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
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
            Assert.IsNull(calculation.Name);
            Assert.IsNull(calculation.Comments.Body);

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
        public void Read_ValidEntityWithAlreadyReadDikeProfileEntity_ReturnCalculationWithReadDikeProfile()
        {
            // Setup
            DikeProfile dikeProfile = new TestDikeProfile();
            var dikeProfileEntity = new DikeProfileEntity();
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
        public void Read_ValidEntityWithUnreadDikeProfileEntity_ReturnCalculationWithNewDikeProfile()
        {
            // Setup
            var dikeProfileEntity = new DikeProfileEntity
            {
                DikeGeometryXml = new RoughnessPointXmlSerializer().ToXml(new RoughnessPoint[0]),
                ForeshoreXml = new Point2DXmlSerializer().ToXml(new Point2D[0])
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
        public void Read_ValidEntityWithAlreadyReadHydraulicLocationEntity_ReturnCalculationWithReadHydraulicBoundaryLocation()
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
        public void Read_ValidEntityWithUnreadHydraulicLocationEntity_ReturnCalculationWithNewHydraulicBoundaryLocation()
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
                GrassCoverErosionInwardsOutputEntities =
                {
                    new GrassCoverErosionInwardsOutputEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(calculation.HasOutput);
        }

        [Test]
        public void Read_CalculationEntityAlreadyRead_ReturnReadCalculation()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsOutputEntities =
                {
                    new GrassCoverErosionInwardsOutputEntity()
                }
            };

            var calculation = new GrassCoverErosionInwardsCalculation();

            var collector = new ReadConversionCollector();
            collector.Read(entity, calculation);

            // Call
            GrassCoverErosionInwardsCalculation returnedCalculation = entity.Read(collector);

            // Assert
            Assert.AreSame(calculation, returnedCalculation);
        }
    }
}