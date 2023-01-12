﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.Piping;
using Riskeer.Storage.Core.Create.Piping.SemiProbabilistic;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.Piping.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingCalculationScenarioCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new SemiProbabilisticPipingCalculationScenario();

            // Call
            void Call() => calculation.Create(null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        [TestCase(true, false, 0.0, "A", "<Comments>", 2.2, 0.0, 5.8, 123, 827364)]
        [TestCase(false, true, 1.0, null, null, double.NaN, double.NaN, double.NaN, 980754, 231)]
        public void Create_SemiProbabilisticPipingCalculationScenarioWithPropertiesSet_ReturnSemiProbabilisticPipingCalculationEntity(
            bool isRelevant, bool useAssessmentLevelManualInput, double contribution, string name, string comments,
            double exitPoint, double entryPoint, double assessmentLevel, int order, int randomSeed)
        {
            // Setup
            var random = new Random(randomSeed);

            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                IsRelevant = isRelevant,
                Contribution = (RoundedDouble) contribution,
                Name = name,
                Comments =
                {
                    Body = comments
                },
                InputParameters =
                {
                    ExitPointL = (RoundedDouble) exitPoint,
                    EntryPointL = (RoundedDouble) entryPoint,
                    PhreaticLevelExit =
                    {
                        Mean = random.NextRoundedDouble(-9999.9999, 9999.9999),
                        StandardDeviation = random.NextRoundedDouble(1e-6, 9999.9999)
                    },
                    DampingFactorExit =
                    {
                        Mean = random.NextRoundedDouble(1e-6, 9999.9999),
                        StandardDeviation = random.NextRoundedDouble(1e-6, 9999.9999)
                    },
                    UseAssessmentLevelManualInput = useAssessmentLevelManualInput,
                    AssessmentLevel = (RoundedDouble) assessmentLevel
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            SemiProbabilisticPipingCalculationEntity entity = calculation.Create(registry, order);

            // Assert
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.RelevantForScenario);
            Assert.AreEqual(calculation.Contribution, entity.ScenarioContribution);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(comments, entity.Comments);

            Assert.AreEqual(exitPoint.ToNaNAsNull(), entity.ExitPointL);
            Assert.AreEqual(entryPoint.ToNaNAsNull(), entity.EntryPointL);

            SemiProbabilisticPipingInput input = calculation.InputParameters;
            Assert.AreEqual(input.PhreaticLevelExit.Mean.Value, entity.PhreaticLevelExitMean);
            Assert.AreEqual(input.PhreaticLevelExit.StandardDeviation.Value, entity.PhreaticLevelExitStandardDeviation);
            Assert.AreEqual(input.DampingFactorExit.Mean.Value, entity.DampingFactorExitMean);
            Assert.AreEqual(input.DampingFactorExit.StandardDeviation.Value, entity.DampingFactorExitStandardDeviation);

            Assert.AreEqual(Convert.ToByte(input.UseAssessmentLevelManualInput), entity.UseAssessmentLevelManualInput);
            Assert.AreEqual(input.AssessmentLevel.ToNaNAsNull(), entity.AssessmentLevel);

            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(0, entity.SemiProbabilisticPipingCalculationEntityId);
            Assert.IsNull(entity.CalculationGroupEntity);

            Assert.IsNull(entity.SurfaceLineEntity);
            Assert.IsNull(entity.PipingStochasticSoilProfileEntity);
            Assert.IsNull(entity.HydraulicLocationEntity);
            CollectionAssert.IsEmpty(entity.SemiProbabilisticPipingCalculationOutputEntities);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "A";
            const string comments = "B";
            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                Name = name,
                Comments =
                {
                    Body = comments
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            SemiProbabilisticPipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(name, entity.Name);
            TestHelper.AssertAreEqualButNotSame(comments, entity.Comments);
        }

        [Test]
        public void Create_HasSurfaceLineSet_EntityHasSurfaceLineEntity()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty)
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(1.1, 2.2)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(3.3, 6.6, 1.0)
            });

            var registry = new PersistenceRegistry();
            SurfaceLineEntity surfaceLineEntity = surfaceLine.Create(registry, 0);

            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            // Call
            SemiProbabilisticPipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(surfaceLineEntity, entity.SurfaceLineEntity);
        }

        [Test]
        public void Create_HydraulicBoundaryLocation_EntityHasHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2.3, 4.5);

            var registry = new PersistenceRegistry();
            HydraulicLocationEntity hydraulicLocationEntity = hydraulicBoundaryLocation.Create(registry, 0);

            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            // Call
            SemiProbabilisticPipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_StochasticSoilProfileSet_EntityHasStochasticSoilProfileEntity()
        {
            // Setup
            PipingSoilProfile soilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.6, soilProfile);

            PipingStochasticSoilModel soilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("A", new[]
            {
                stochasticSoilProfile
            });

            var registry = new PersistenceRegistry();
            StochasticSoilModelEntity soilModelEntity = soilModel.Create(registry, 0);

            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = soilModel,
                    StochasticSoilProfile = stochasticSoilProfile
                }
            };

            // Call
            SemiProbabilisticPipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            PipingStochasticSoilProfileEntity expectedStochasticSoilProfileEntity = soilModelEntity.PipingStochasticSoilProfileEntities.First();
            Assert.AreSame(expectedStochasticSoilProfileEntity, entity.PipingStochasticSoilProfileEntity);
            Assert.IsTrue(registry.Contains(soilModel));
        }

        [Test]
        public void Create_HasCalculationOutput_EntityHasPipingCalculationOutputEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();

            SemiProbabilisticPipingOutput newOutput = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();
            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                Output = newOutput
            };

            // Call
            SemiProbabilisticPipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            SemiProbabilisticPipingCalculationOutputEntity outputEntity = entity.SemiProbabilisticPipingCalculationOutputEntities.SingleOrDefault();

            Assert.IsNotNull(outputEntity);
            Assert.AreEqual(newOutput.HeaveFactorOfSafety, outputEntity.HeaveFactorOfSafety);
            Assert.AreEqual(newOutput.SellmeijerFactorOfSafety, outputEntity.SellmeijerFactorOfSafety);
            Assert.AreEqual(newOutput.UpliftFactorOfSafety, outputEntity.UpliftFactorOfSafety);
            Assert.AreEqual(newOutput.UpliftEffectiveStress, outputEntity.UpliftEffectiveStress, newOutput.UpliftEffectiveStress.GetAccuracy());
            Assert.AreEqual(newOutput.HeaveGradient, outputEntity.HeaveGradient, newOutput.HeaveGradient.GetAccuracy());
            Assert.AreEqual(newOutput.SellmeijerCreepCoefficient, outputEntity.SellmeijerCreepCoefficient, newOutput.SellmeijerCreepCoefficient.GetAccuracy());
            Assert.AreEqual(newOutput.SellmeijerCriticalFall, outputEntity.SellmeijerCriticalFall, newOutput.SellmeijerCriticalFall.GetAccuracy());
            Assert.AreEqual(newOutput.SellmeijerReducedFall, outputEntity.SellmeijerReducedFall, newOutput.SellmeijerReducedFall.GetAccuracy());
        }
    }
}