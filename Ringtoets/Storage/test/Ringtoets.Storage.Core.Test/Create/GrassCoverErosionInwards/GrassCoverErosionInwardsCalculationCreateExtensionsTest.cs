// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.GrassCoverErosionInwards;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => calculation.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        [TestCase("I am comment")]
        [TestCase(null)]
        public void Create_ValidCalculation_ReturnEntity(string comment)
        {
            // Setup
            var random = new Random(12);
            int order = random.Next();
            const string name = "GrassCoverErosionInwardsCalculation Name";
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                Comments =
                {
                    Body = comment
                },
                InputParameters =
                {
                    DikeProfile = null,
                    HydraulicBoundaryLocation = null,
                    DikeHeight = (RoundedDouble) 1.1,
                    Orientation = (RoundedDouble) 2.2,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 3.3,
                        Type = BreakWaterType.Dam
                    },
                    DikeHeightCalculationType = random.NextEnumValue<DikeHeightCalculationType>(),
                    OvertoppingRateCalculationType = random.NextEnumValue<OvertoppingRateCalculationType>(),
                    CriticalFlowRate =
                    {
                        Mean = (RoundedDouble) 4.4,
                        StandardDeviation = (RoundedDouble) 5.5
                    },
                    UseBreakWater = true,
                    UseForeshore = false,
                    ShouldDikeHeightIllustrationPointsBeCalculated = random.NextBoolean(),
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = random.NextBoolean(),
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = random.NextBoolean()
                },
                Output = null
            };

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, order);

            // Assert
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(comment, entity.Comments);
            Assert.AreEqual(order, entity.Order);

            Assert.IsNull(entity.DikeProfileEntity);
            Assert.IsNull(entity.HydraulicLocationEntity);

            GrassCoverErosionInwardsInput input = calculation.InputParameters;
            Assert.AreEqual(input.BreakWater.Height.Value, entity.BreakWaterHeight);
            Assert.AreEqual((short) input.BreakWater.Type, entity.BreakWaterType);
            Assert.AreEqual(Convert.ToByte(input.UseBreakWater), entity.UseBreakWater);
            Assert.AreEqual(input.CriticalFlowRate.Mean.Value, entity.CriticalFlowRateMean);
            Assert.AreEqual(input.CriticalFlowRate.StandardDeviation.Value, entity.CriticalFlowRateStandardDeviation);
            Assert.AreEqual(input.Orientation.Value, entity.Orientation);
            Assert.AreEqual(Convert.ToByte(input.DikeHeightCalculationType), entity.DikeHeightCalculationType);
            Assert.AreEqual(Convert.ToByte(input.OvertoppingRateCalculationType), entity.OvertoppingRateCalculationType);
            Assert.AreEqual(input.DikeHeight.Value, entity.DikeHeight);
            Assert.AreEqual(Convert.ToByte(input.UseForeshore), entity.UseForeshore);

            Assert.AreEqual(Convert.ToByte(input.ShouldDikeHeightIllustrationPointsBeCalculated),
                            entity.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.AreEqual(Convert.ToByte(input.ShouldOvertoppingOutputIllustrationPointsBeCalculated),
                            entity.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(Convert.ToByte(input.ShouldOvertoppingRateIllustrationPointsBeCalculated),
                            entity.ShouldOvertoppingRateIllustrationPointsBeCalculated);

            Assert.IsFalse(entity.GrassCoverErosionInwardsOutputEntities.Any());
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "A";
            const string comment = "B";
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                Comments =
                {
                    Body = comment
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreNotSame(name, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(name, entity.Name);

            Assert.AreNotSame(comment, entity.Comments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(comment, entity.Comments);
        }

        [Test]
        public void Create_NaNParameters_EntityWithNullFields()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    Orientation = RoundedDouble.NaN,
                    CriticalFlowRate =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    DikeHeight = RoundedDouble.NaN,
                    BreakWater =
                    {
                        Height = RoundedDouble.NaN
                    }
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNull(entity.Orientation);
            Assert.IsNull(entity.CriticalFlowRateMean);
            Assert.IsNull(entity.CriticalFlowRateStandardDeviation);
            Assert.IsNull(entity.DikeHeight);
            Assert.IsNull(entity.BreakWaterHeight);
        }

        [Test]
        public void Create_CalculationWithAlreadySavedDikeProfile_ReturnEntityWithDikeProfileEntity()
        {
            // Setup
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            var dikeProfileEntity = new DikeProfileEntity();
            var registry = new PersistenceRegistry();
            registry.Register(dikeProfileEntity, dikeProfile);

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(dikeProfileEntity, entity.DikeProfileEntity);
        }

        [Test]
        public void Create_CalculationWithAlreadySavedHydraulicBoundaryLocation_ReturnEntityWithHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1, 1);
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_CalculationWithOutput_ReturnEntity()
        {
            // Setup
            var random = new Random(456);
            var overtoppingOutput = new OvertoppingOutput(random.NextDouble(), false, random.NextDouble(), null);
            var output = new GrassCoverErosionInwardsOutput(overtoppingOutput, null, null);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = output
            };

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            GrassCoverErosionInwardsOutputEntity outputEntity = entity.GrassCoverErosionInwardsOutputEntities.Single();
            Assert.AreEqual(overtoppingOutput.WaveHeight, outputEntity.WaveHeight);
            Assert.AreEqual(overtoppingOutput.Reliability, outputEntity.Reliability);
            Assert.IsNull(outputEntity.GeneralResultFaultTreeIllustrationPointEntity);
        }
    }
}