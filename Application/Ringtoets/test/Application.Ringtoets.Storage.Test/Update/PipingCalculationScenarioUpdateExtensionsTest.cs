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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class PipingCalculationScenarioUpdateExtensionsTest
    {
        [Test]
        public void Update_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            TestDelegate call = () => calculation.Update(null, context);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_RingtoetsEntitiesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => calculation.Update(registry, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_PipingCalculationScenarioNotSavedYet_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            TestDelegate call = () => calculation.Update(registry, context);

            // Assert
            Assert.Throws<EntityNotFoundException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationScenarioChanged_EntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var entity  = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 456
            };
            context.PipingCalculationEntities.Add(entity);

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                IsRelevant = true,
                Contribution = (RoundedDouble)0.56,
                Name = "New name!",
                Comments = "Better comments!",
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble)3.3,
                        StandardDeviation = (RoundedDouble) 4.4
                    },
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)5.5,
                        StandardDeviation = (RoundedDouble)6.6
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)7.7,
                        StandardDeviation = (RoundedDouble)8.8,
                        Shift = (RoundedDouble)9.9
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)10.10,
                        StandardDeviation = (RoundedDouble)11.11
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)12.12,
                        StandardDeviation = (RoundedDouble)13.13
                    }
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            Assert.AreEqual(Convert.ToByte(calculation.IsRelevant), entity.RelevantForScenario);
            Assert.AreEqual(Convert.ToDecimal(calculation.Contribution), entity.ScenarioContribution);
            Assert.AreEqual(calculation.Name, entity.Name);
            Assert.AreEqual(calculation.Comments, entity.Comments);

            PipingInput inputParameters = calculation.InputParameters;
            Assert.AreEqual(ToNullableDecimal(inputParameters.EntryPointL), entity.EntryPointL);
            Assert.AreEqual(ToNullableDecimal(inputParameters.ExitPointL), entity.ExitPointL);

            Assert.AreEqual(inputParameters.PhreaticLevelExit.Mean.Value, entity.PhreaticLevelExitMean);
            Assert.AreEqual(inputParameters.PhreaticLevelExit.StandardDeviation.Value, entity.PhreaticLevelExitStandardDeviation);
            Assert.AreEqual(inputParameters.DampingFactorExit.Mean.Value, entity.DampingFactorExitMean);
            Assert.AreEqual(inputParameters.DampingFactorExit.StandardDeviation.Value, entity.DampingFactorExitStandardDeviation);
            Assert.AreEqual(inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean.Value, entity.SaturatedVolumicWeightOfCoverageLayerMean);
            Assert.AreEqual(inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation.Value, entity.SaturatedVolumicWeightOfCoverageLayerStandardDeviation);
            Assert.AreEqual(inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift.Value, entity.SaturatedVolumicWeightOfCoverageLayerShift);
            Assert.AreEqual(inputParameters.Diameter70.Mean.Value, entity.Diameter70Mean);
            Assert.AreEqual(inputParameters.Diameter70.StandardDeviation.Value, entity.Diameter70StandardDeviation);
            Assert.AreEqual(inputParameters.DarcyPermeability.Mean.Value, entity.DarcyPermeabilityMean);
            Assert.AreEqual(inputParameters.DarcyPermeability.StandardDeviation.Value, entity.DarcyPermeabilityStandardDeviation);

            registry.RemoveUntouched(context);
            CollectionAssert.Contains(context.PipingCalculationEntities, entity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationScenarioWithReplacedSurfaceLine_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var originalSurfaceLineEntity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = 345,
                Name = "Original"
            };
            var replacementSurfaceLineEntity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = 897,
                Name = "Replacement"
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453,
                SurfaceLineEntity = originalSurfaceLineEntity
            };
            context.PipingCalculationEntities.Add(entity);
            context.SurfaceLineEntities.Add(originalSurfaceLineEntity);
            context.SurfaceLineEntities.Add(replacementSurfaceLineEntity);

            var replacementSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = replacementSurfaceLineEntity.Name,
                StorageId = replacementSurfaceLineEntity.SurfaceLineEntityId,
                ReferenceLineIntersectionWorldPoint = new Point2D(1, 2)
            };
            replacementSurfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });

            var registry = new PersistenceRegistry();
            registry.Register(originalSurfaceLineEntity, new RingtoetsPipingSurfaceLine());
            registry.Register(replacementSurfaceLineEntity, replacementSurfaceLine);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SurfaceLine = replacementSurfaceLine
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            Assert.IsNotNull(entity.SurfaceLineEntity);
            Assert.AreSame(replacementSurfaceLineEntity, entity.SurfaceLineEntity);

            registry.RemoveUntouched(context);
            CollectionAssert.Contains(context.PipingCalculationEntities, entity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationScenarioWithClearedSurfaceLine_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var surfaceLineEntity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = 345,
                Name = "Original"
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453,
                SurfaceLineEntity = surfaceLineEntity
            };
            context.PipingCalculationEntities.Add(entity);

            var registry = new PersistenceRegistry();
            registry.Register(surfaceLineEntity, new RingtoetsPipingSurfaceLine());

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SurfaceLine = null
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            Assert.IsNull(entity.SurfaceLineEntity);

            registry.RemoveUntouched(context);
            CollectionAssert.Contains(context.PipingCalculationEntities, entity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationScenarioWithReplacedHydraulicBoundarLocation_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var originalHydraulicLocationEntity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 345,
                Name = "Original"
            };
            var replacementHydraulicLocationEntity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 897,
                Name = "Replacement"
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453,
                HydraulicLocationEntity = originalHydraulicLocationEntity
            };
            context.PipingCalculationEntities.Add(entity);
            context.HydraulicLocationEntities.Add(originalHydraulicLocationEntity);
            context.HydraulicLocationEntities.Add(replacementHydraulicLocationEntity);

            var replacementBoundaryLocation = new HydraulicBoundaryLocation(1, replacementHydraulicLocationEntity.Name, 1, 2)
            {
                StorageId = replacementHydraulicLocationEntity.HydraulicLocationEntityId,
            };

            var registry = new PersistenceRegistry();
            registry.Register(originalHydraulicLocationEntity,
                              new HydraulicBoundaryLocation(2, originalHydraulicLocationEntity.Name, 3, 4));
            registry.Register(replacementHydraulicLocationEntity, replacementBoundaryLocation);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    },
                    HydraulicBoundaryLocation = replacementBoundaryLocation
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            Assert.AreSame(replacementHydraulicLocationEntity, entity.HydraulicLocationEntity);

            registry.RemoveUntouched(context);
            CollectionAssert.Contains(context.PipingCalculationEntities, entity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationScenarioWithClearedHydraulicBoundaryLocation_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var hydraulicLocationEntity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 5675,
                Name = "Original"
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453,
                HydraulicLocationEntity = hydraulicLocationEntity
            };
            context.PipingCalculationEntities.Add(entity);
            context.HydraulicLocationEntities.Add(hydraulicLocationEntity);

            var registry = new PersistenceRegistry();
            registry.Register(hydraulicLocationEntity, new HydraulicBoundaryLocation(1, hydraulicLocationEntity.Name, 1, 3));

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    },
                    HydraulicBoundaryLocation = null
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            Assert.IsNull(entity.HydraulicLocationEntity);

            registry.RemoveUntouched(context);
            CollectionAssert.Contains(context.PipingCalculationEntities, entity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationScenarioWithReplacedStochasticSoilProfile_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var originalStochasticSoilProfileEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 490865
            };
            var replacementStochasticSoilProfileEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 57,
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453,
                StochasticSoilProfileEntity = originalStochasticSoilProfileEntity
            };
            context.PipingCalculationEntities.Add(entity);
            context.StochasticSoilProfileEntities.Add(originalStochasticSoilProfileEntity);
            context.StochasticSoilProfileEntities.Add(replacementStochasticSoilProfileEntity);

            var replacementSurfaceLine = new StochasticSoilProfile(0.6, SoilProfileType.SoilProfile1D, 34)
            {
                StorageId = replacementStochasticSoilProfileEntity.StochasticSoilProfileEntityId,
            };

            var registry = new PersistenceRegistry();
            registry.Register(originalStochasticSoilProfileEntity,
                              new StochasticSoilProfile(0.6, SoilProfileType.SoilProfile2D, 131));
            registry.Register(replacementStochasticSoilProfileEntity, replacementSurfaceLine);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    },
                    StochasticSoilProfile = replacementSurfaceLine
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            Assert.AreSame(replacementStochasticSoilProfileEntity, entity.StochasticSoilProfileEntity);

            registry.RemoveUntouched(context);
            CollectionAssert.Contains(context.PipingCalculationEntities, entity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationScenarioWithClearedStochasticSoilProfile_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var stochasticSoilProfileEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 6,
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453,
                StochasticSoilProfileEntity = stochasticSoilProfileEntity
            };
            context.PipingCalculationEntities.Add(entity);

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    },
                    StochasticSoilProfile = null
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            Assert.IsNull(entity.StochasticSoilProfileEntity);

            registry.RemoveUntouched(context);
            CollectionAssert.Contains(context.PipingCalculationEntities, entity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationWithNewPipingOutput_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453
            };
            context.PipingCalculationEntities.Add(entity);

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                Output = new PipingOutput(1,1,1,1,1,1),
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    }
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            Assert.AreEqual(1, entity.PipingCalculationOutputEntities.Count);

            PipingCalculationOutputEntity pipingCalculationOutputEntity = entity.PipingCalculationOutputEntities.First();
            pipingCalculationOutputEntity.PipingCalculationOutputEntityId = 495876;
            registry.TransferIds();
            Assert.AreEqual(pipingCalculationOutputEntity.PipingCalculationOutputEntityId, calculation.Output.StorageId,
                "New PipingCalculationOutputEntity should be registered to PersistenceRegistry.");
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationWithUnchangedPipingOutput_PipingCalculationOutputEntityIsRegistered()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculationOutputEntity = new PipingCalculationOutputEntity
            {
                PipingCalculationOutputEntityId = 45
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453
            };
            entity.PipingCalculationOutputEntities.Add(calculationOutputEntity);
            context.PipingCalculationEntities.Add(entity);
            context.PipingCalculationOutputEntities.Add(calculationOutputEntity);

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                Output = new PipingOutput(1, 1, 1, 1, 1, 1)
                {
                    StorageId = calculationOutputEntity.PipingCalculationOutputEntityId
                },
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    }
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            CollectionAssert.Contains(context.PipingCalculationOutputEntities, calculationOutputEntity);
            registry.RemoveUntouched(context);
            CollectionAssert.Contains(context.PipingCalculationOutputEntities, calculationOutputEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationWithRemovedPipingOutput_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculationOutputEntity = new PipingCalculationOutputEntity
            {
                PipingCalculationOutputEntityId = 45
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453
            };
            entity.PipingCalculationOutputEntities.Add(calculationOutputEntity);
            context.PipingCalculationEntities.Add(entity);
            context.PipingCalculationOutputEntities.Add(calculationOutputEntity);

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                Output = null,
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    }
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            CollectionAssert.IsEmpty(entity.PipingCalculationOutputEntities);

            registry.RemoveUntouched(context);
            CollectionAssert.DoesNotContain(context.PipingCalculationOutputEntities, calculationOutputEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationWithNewSemiProbabilisticOutput_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453
            };
            context.PipingCalculationEntities.Add(entity);

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(1, 1, 1,
                                                                            1, 1, 1,
                                                                            1, 1, 1,
                                                                            1, 1,
                                                                            1, 1, 1),
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    }
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            Assert.AreEqual(1, entity.PipingSemiProbabilisticOutputEntities.Count);

            PipingSemiProbabilisticOutputEntity semiProbabilisticOutputEntity = entity.PipingSemiProbabilisticOutputEntities.First();
            semiProbabilisticOutputEntity.PipingSemiProbabilisticOutputEntityId = 546;
            registry.TransferIds();
            Assert.AreEqual(semiProbabilisticOutputEntity.PipingSemiProbabilisticOutputEntityId, calculation.SemiProbabilisticOutput.StorageId,
                "New PipingSemiProbabilisticOutputEntity should be registered to PersistenceRegistry.");
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationWithUnchangedSemiProbabilisticOutput_PipingSemiProbabilisticOutputEntityIsRegistered()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var semiProbabilisticOutputEntity = new PipingSemiProbabilisticOutputEntity
            {
                PipingSemiProbabilisticOutputEntityId = 549876
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453
            };
            entity.PipingSemiProbabilisticOutputEntities.Add(semiProbabilisticOutputEntity);
            context.PipingCalculationEntities.Add(entity);
            context.PipingSemiProbabilisticOutputEntities.Add(semiProbabilisticOutputEntity);

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(1, 1, 1,
                                                                            1, 1, 1,
                                                                            1, 1, 1,
                                                                            1, 1,
                                                                            1, 1, 1)
                {
                    StorageId = semiProbabilisticOutputEntity.PipingSemiProbabilisticOutputEntityId
                },
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    }
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            CollectionAssert.Contains(context.PipingSemiProbabilisticOutputEntities, semiProbabilisticOutputEntity);
            registry.RemoveUntouched(context);
            CollectionAssert.Contains(context.PipingSemiProbabilisticOutputEntities, semiProbabilisticOutputEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_PipingCalculationWithRemovedSemiProbabilisticOutput_PipingCalculationEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var semiProbabilisticOutputEntity = new PipingSemiProbabilisticOutputEntity
            {
                PipingSemiProbabilisticOutputEntityId = 54976
            };
            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = 453
            };
            entity.PipingSemiProbabilisticOutputEntities.Add(semiProbabilisticOutputEntity);
            context.PipingCalculationEntities.Add(entity);
            context.PipingSemiProbabilisticOutputEntities.Add(semiProbabilisticOutputEntity);

            var registry = new PersistenceRegistry();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                StorageId = entity.PipingCalculationEntityId,
                SemiProbabilisticOutput = null,
                InputParameters =
                {
                    EntryPointL = (RoundedDouble)1.1,
                    ExitPointL = (RoundedDouble)2.2,
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)1
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1
                    }
                }
            };

            // Call
            calculation.Update(registry, context);

            // Assert
            CollectionAssert.IsEmpty(entity.PipingSemiProbabilisticOutputEntities);

            registry.RemoveUntouched(context);
            CollectionAssert.DoesNotContain(context.PipingSemiProbabilisticOutputEntities, semiProbabilisticOutputEntity);
            mocks.VerifyAll();
        }
        
        private decimal? ToNullableDecimal(RoundedDouble roundedDoubleValue)
        {
            if (double.IsNaN(roundedDoubleValue))
            {
                return null;
            }
            return Convert.ToDecimal(roundedDoubleValue);
        }
    }
}