// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.GrassCoverErosionInwards;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil.Hydraulics;

namespace Riskeer.Storage.Core.Test.Read.GrassCoverErosionInwards
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
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntity_ReturnCalculation()
        {
            // Setup
            var random = new Random(14);
            var breakWaterType = random.NextEnumValue<BreakWaterType>();
            var dikeHeightCalculationType = random.NextEnumValue<DikeHeightCalculationType>();
            var overtoppingRateCalculationType = random.NextEnumValue<OvertoppingRateCalculationType>();
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                Name = "sodhfksn",
                Comments = "s;ohfgwjo5p09u",
                HydraulicLocationEntity = null,
                DikeProfileEntity = null,
                Orientation = 5.6,
                CriticalFlowRateMean = 3.4,
                CriticalFlowRateStandardDeviation = 1.2,
                UseForeshore = Convert.ToByte(random.NextBoolean()),
                DikeHeight = 2.3,
                UseBreakWater = Convert.ToByte(random.NextBoolean()),
                BreakWaterType = Convert.ToByte(breakWaterType),
                BreakWaterHeight = 5.7,
                DikeHeightCalculationType = Convert.ToByte(dikeHeightCalculationType),
                OvertoppingRateCalculationType = Convert.ToByte(overtoppingRateCalculationType),
                ShouldOvertoppingOutputIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean()),
                ShouldDikeHeightIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean()),
                ShouldOvertoppingRateIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean()),
                RelevantForScenario = Convert.ToByte(random.NextBoolean()),
                ScenarioContribution = 4.5
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Name, calculation.Name);
            Assert.AreEqual(entity.Comments, calculation.Comments.Body);
            AssertBoolean(entity.RelevantForScenario, calculation.IsRelevant);
            Assert.AreEqual(entity.ScenarioContribution, calculation.Contribution);

            GrassCoverErosionInwardsInput input = calculation.InputParameters;
            Assert.AreEqual(entity.Orientation, input.Orientation.Value);
            Assert.AreEqual(entity.CriticalFlowRateMean, input.CriticalFlowRate.Mean.Value);
            Assert.AreEqual(entity.CriticalFlowRateStandardDeviation, input.CriticalFlowRate.StandardDeviation.Value);
            AssertBoolean(entity.UseForeshore, input.UseForeshore);
            Assert.AreEqual(entity.DikeHeight, input.DikeHeight.Value);
            AssertBoolean(entity.UseBreakWater, input.UseBreakWater);
            Assert.AreEqual(breakWaterType, input.BreakWater.Type);
            Assert.AreEqual(entity.BreakWaterHeight, input.BreakWater.Height.Value);
            Assert.AreEqual(dikeHeightCalculationType, input.DikeHeightCalculationType);
            Assert.AreEqual(overtoppingRateCalculationType, input.OvertoppingRateCalculationType);
            AssertBoolean(entity.ShouldOvertoppingOutputIllustrationPointsBeCalculated,
                          input.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            AssertBoolean(entity.ShouldDikeHeightIllustrationPointsBeCalculated,
                          input.ShouldDikeHeightIllustrationPointsBeCalculated);
            AssertBoolean(entity.ShouldOvertoppingRateIllustrationPointsBeCalculated,
                          input.ShouldOvertoppingRateIllustrationPointsBeCalculated);

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
                Name = null,
                Comments = null,
                Orientation = null,
                CriticalFlowRateMean = null,
                CriticalFlowRateStandardDeviation = null,
                DikeHeight = null,
                BreakWaterHeight = null
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
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();
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
                Id = "a",
                DikeGeometryXml = new RoughnessPointCollectionXmlSerializer().ToXml(new RoughnessPoint[0]),
                ForeshoreXml = new Point2DCollectionXmlSerializer().ToXml(new Point2D[0])
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
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 0, 0);
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_ValidEntityWithUnreadHydraulicLocationEntity_ReturnCalculationWithNewHydraulicBoundaryLocation()
        {
            // Setup
            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsTrue(collector.Contains(hydraulicLocationEntity));
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

        private static void AssertBoolean(byte expectedByte, bool actual)
        {
            Assert.AreEqual(Convert.ToBoolean(expectedByte), actual);
        }
    }
}