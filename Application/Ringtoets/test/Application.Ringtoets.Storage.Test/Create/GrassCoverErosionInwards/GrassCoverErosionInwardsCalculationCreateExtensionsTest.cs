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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
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
                    UseForeshore = false
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
            DikeProfile dikeProfile = new TestDikeProfile();
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
            var hydroLocation = new HydraulicBoundaryLocation(1, "A", 1, 1);
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydroLocation
                }
            };

            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(hydraulicLocationEntity, hydroLocation);

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_CalculationWithOutput_ReturnEntity()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new GrassCoverErosionInwardsOvertoppingOutput(
                                                                1, true, new ProbabilityAssessmentOutput(1, 1, 1, 1, 1)),
                                                            new TestDikeHeightOutput(2),
                                                            new TestOvertoppingRateOutput(3))
            };

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreEqual(1, entity.GrassCoverErosionInwardsOutputEntities.Count);
        }
    }
}