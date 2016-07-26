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
using System.Linq;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

using NUnit.Framework;

using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.Piping
{
    [TestFixture]
    public class PipingCalculationScenarioCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            TestDelegate call = () => calculation.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual(paramName, "registry");
        }

        [Test]
        [TestCase(true, 0.0, "A", "<Comments>", 2.2, 0.0, 123, 827364)]
        [TestCase(false, 1.0, null, null, double.NaN, double.NaN, 980754, 231)]
        public void Create_PipingCalculationScenarioWithPropertiesSet_ReturnPipingCalculationEntity(
            bool isRelevant, double contribution, string name, string comments,
            double exitPoint, double entryPoint, int order, int randomSeed)
        {
            // Setup
            var random = new Random(randomSeed);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                IsRelevant = isRelevant,
                Contribution = (RoundedDouble)contribution,
                Name = name,
                Comments = comments,
                InputParameters =
                {
                    ExitPointL = (RoundedDouble)exitPoint,
                    EntryPointL = (RoundedDouble)entryPoint,
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble)GetRandomDoubleFromRange(random, -9999.9999, 9999.9999),
                        StandardDeviation = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        Shift = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble)GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, order);

            // Assert
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.RelevantForScenario);
            Assert.AreEqual(contribution, entity.ScenarioContribution);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(comments, entity.Comments);

            Assert.AreEqual(exitPoint.ToNaNAsNull(), entity.ExitPointL);
            Assert.AreEqual(entryPoint.ToNaNAsNull(), entity.EntryPointL);

            PipingInput input = calculation.InputParameters;
            Assert.AreEqual(input.PhreaticLevelExit.Mean.Value, entity.PhreaticLevelExitMean);
            Assert.AreEqual(input.PhreaticLevelExit.StandardDeviation.Value, entity.PhreaticLevelExitStandardDeviation);
            Assert.AreEqual(input.DampingFactorExit.Mean.Value, entity.DampingFactorExitMean);
            Assert.AreEqual(input.DampingFactorExit.StandardDeviation.Value, entity.DampingFactorExitStandardDeviation);
            Assert.AreEqual(input.SaturatedVolumicWeightOfCoverageLayer.Mean.Value, entity.SaturatedVolumicWeightOfCoverageLayerMean);
            Assert.AreEqual(input.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation.Value, entity.SaturatedVolumicWeightOfCoverageLayerStandardDeviation);
            Assert.AreEqual(input.SaturatedVolumicWeightOfCoverageLayer.Shift.Value, entity.SaturatedVolumicWeightOfCoverageLayerShift);
            Assert.AreEqual(input.Diameter70.Mean.Value, entity.Diameter70Mean);
            Assert.AreEqual(input.Diameter70.StandardDeviation.Value, entity.Diameter70StandardDeviation);
            Assert.AreEqual(input.DarcyPermeability.Mean.Value, entity.DarcyPermeabilityMean);
            Assert.AreEqual(input.DarcyPermeability.StandardDeviation.Value, entity.DarcyPermeabilityStandardDeviation);
            
            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(0, entity.PipingCalculationEntityId);
            Assert.IsNull(entity.CalculationGroupEntity);

            Assert.IsNull(entity.SurfaceLineEntity);
            Assert.IsNull(entity.StochasticSoilProfileEntity);
            Assert.IsNull(entity.HydraulicLocationEntityId);
        }

        [Test]
        public void Create_HasSurfaceLineSet_EntityHasSurfaceLineEntity()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(1.1, 2.2)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0), 
                new Point3D(3.3, 6.6, 1.0) 
            });

            var registry = new PersistenceRegistry();
            SurfaceLineEntity surfaceLineEntity = surfaceLine.Create(registry);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(surfaceLineEntity, entity.SurfaceLineEntity);
        }

        [Test]
        public void Create_HasSurfaceLineSetButInstanceNotSaved_EntityHasNewSurfaceLineEntity()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(1.1, 2.2)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.0), 
                new Point3D(3.3, 6.6, 1.0) 
            });

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.True(registry.Contains(surfaceLine));
            Assert.AreSame(registry.Get(surfaceLine), entity.SurfaceLineEntity);
        }

        [Test]
        public void Create_HydraulicBoundaryLocation_EntityHasHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2.3, 4.5);

            var registry = new PersistenceRegistry();
            HydraulicLocationEntity hydraulicLocationEntity = hydraulicBoundaryLocation.Create(registry);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_HasHydraulicBoundaryLocationSetButInstanceNotSaved_EntityHasNewHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "a", 1.1, 2.2);
            var registry = new PersistenceRegistry();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.True(registry.Contains(hydraulicBoundaryLocation));
            Assert.AreSame(registry.Get(hydraulicBoundaryLocation), entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_StochasticSoilProfileSet_EntityHasStochasticSoilProfileEntity()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();
            var stochasticSoilProfile = new StochasticSoilProfile(0.6, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = soilProfile
            };

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            var registry = new PersistenceRegistry();
            StochasticSoilModelEntity soilModelEntity = soilModel.Create(registry);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    StochasticSoilModel = soilModel,
                    StochasticSoilProfile = stochasticSoilProfile
                }
            };

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            var expectedStochasticSoilProfileEntity = soilModelEntity.StochasticSoilProfileEntities.First();
            Assert.AreSame(expectedStochasticSoilProfileEntity, entity.StochasticSoilProfileEntity);
            Assert.IsTrue(registry.Contains(soilModel));
        }

        [Test]
        public void Create_HasStochasticSoilProfileSetButInstanceNotSaved_EntityHasNewStochasticSoilProfileEntity()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();
            var stochasticSoilProfile = new StochasticSoilProfile(0.6, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = soilProfile
            };

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            var registry = new PersistenceRegistry();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    StochasticSoilModel = soilModel,
                    StochasticSoilProfile = stochasticSoilProfile
                }
            };

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.True(registry.Contains(soilModel));
            Assert.True(registry.Contains(stochasticSoilProfile));
            Assert.AreSame(registry.Get(stochasticSoilProfile), entity.StochasticSoilProfileEntity);
        }

        [Test]
        public void Create_HasCalculationOutput_EntityHasPipingCalculationOutputEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();

            var newOutput = new PipingOutput(1,2,3,4,5,6);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = newOutput
            };

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity.PipingCalculationOutputEntity);
        }

        [Test]
        public void Create_HasPipingSemiProbabilisticOutput_EntityHasPipingSemiProbabilisticOutputEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();

            var newOutput = new PipingSemiProbabilisticOutput(1, 2, 0.3,
                                                              4, 5, 0.6,
                                                              7, 8, 0.9,
                                                              1.0, 11,
                                                              0.2, 13, 14);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                SemiProbabilisticOutput = newOutput
            };

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity.PipingSemiProbabilisticOutputEntity);
        }

        private double GetRandomDoubleFromRange(Random random, double lowerLimit, double upperLimit)
        {
            double difference = upperLimit - lowerLimit;
            return lowerLimit + random.NextDouble() * difference;
        }
    }
}