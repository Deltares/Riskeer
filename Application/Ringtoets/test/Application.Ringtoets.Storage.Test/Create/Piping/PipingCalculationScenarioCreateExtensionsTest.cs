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
using Core.Common.TestUtil;
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
                Contribution = (RoundedDouble) contribution,
                Name = name,
                Comments = comments,
                InputParameters =
                {
                    ExitPointL = (RoundedDouble) exitPoint,
                    EntryPointL = (RoundedDouble) entryPoint,
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(-9999.9999, 9999.9999),
                        StandardDeviation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(10.0, 9999.9999),
                        StandardDeviation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999),
                        Shift = (RoundedDouble) random.GetFromRange(1e-6, 10.0)
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble) random.GetFromRange(1e-6, 9999.9999)
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

            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(0, entity.PipingCalculationEntityId);
            Assert.IsNull(entity.CalculationGroupEntity);

            Assert.IsNull(entity.SurfaceLineEntity);
            Assert.IsNull(entity.StochasticSoilProfileEntity);
            Assert.IsNull(entity.HydraulicLocationEntityId);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "A";
            const string comments = "B";
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Name = name,
                Comments = comments
            };

            var registry = new PersistenceRegistry();

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreNotSame(name, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(name, entity.Name);

            Assert.AreNotSame(comments, entity.Comments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(comments, entity.Comments);
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
            SurfaceLineEntity surfaceLineEntity = surfaceLine.Create(registry, 0);

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
        public void Create_HydraulicBoundaryLocation_EntityHasHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2.3, 4.5);

            var registry = new PersistenceRegistry();
            HydraulicLocationEntity hydraulicLocationEntity = hydraulicBoundaryLocation.Create(registry, 0);

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
            StochasticSoilModelEntity soilModelEntity = soilModel.Create(registry, 0);

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
        public void Create_HasCalculationOutput_EntityHasPipingCalculationOutputEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();

            var newOutput = new PipingOutput(1, 2, 3, 4, 5, 6);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = newOutput
            };

            // Call
            PipingCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreEqual(1, entity.PipingCalculationOutputEntities.Count);
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
            Assert.AreEqual(1, entity.PipingSemiProbabilisticOutputEntities.Count);
        }
    }
}