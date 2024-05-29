﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.Piping;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.Piping
{
    [TestFixture]
    public class PipingFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            void Call() => failureMechanism.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            var failureMechanism = new PipingFailureMechanism
            {
                InAssembly = random.NextBoolean(),
                InAssemblyInputComments =
                {
                    Body = "Some input text"
                },
                InAssemblyOutputComments =
                {
                    Body = "Some output text"
                },
                NotInAssemblyComments =
                {
                    Body = "Really not in assembly"
                },
                CalculationsInputComments =
                {
                    Body = "Some calculation text"
                },
                GeneralInput =
                {
                    WaterVolumetricWeight = random.NextRoundedDouble(0, 20)
                },
                ScenarioConfigurationType = random.NextEnumValue<PipingScenarioConfigurationType>()
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.Piping, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(failureMechanism.InAssembly), entity.InAssembly);
            Assert.AreEqual(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            Assert.AreEqual(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            Assert.AreEqual(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            Assert.AreEqual(failureMechanism.CalculationsInputComments.Body, entity.CalculationsInputComments);
            CollectionAssert.IsEmpty(entity.StochasticSoilModelEntities);
            CollectionAssert.IsEmpty(entity.SurfaceLineEntities);

            PipingFailureMechanismMetaEntity failureMechanismMetaEntity = entity.PipingFailureMechanismMetaEntities.ToArray()[0];
            Assert.AreEqual(failureMechanism.GeneralInput.WaterVolumetricWeight.Value, failureMechanismMetaEntity.WaterVolumetricWeight);
            Assert.AreEqual(failureMechanism.SurfaceLines.SourcePath, failureMechanismMetaEntity.SurfaceLineCollectionSourcePath);
            Assert.AreEqual(failureMechanism.StochasticSoilModels.SourcePath, failureMechanismMetaEntity.StochasticSoilModelCollectionSourcePath);
            Assert.AreEqual(Convert.ToByte(failureMechanism.ScenarioConfigurationType), failureMechanismMetaEntity.PipingScenarioConfigurationType);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                InAssemblyInputComments =
                {
                    Body = "Some input text"
                },
                InAssemblyOutputComments =
                {
                    Body = "Some output text"
                },
                NotInAssemblyComments =
                {
                    Body = "Really not in assembly"
                },
                CalculationsInputComments =
                {
                    Body = "Some calculation text"
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.CalculationsInputComments.Body, entity.CalculationsInputComments);
        }

        [Test]
        public void Create_WithStochasticSoilModels_ReturnsFailureMechanismEntityWithStochasticSoilModelEntities()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            PipingStochasticSoilModelCollection stochasticSoilModels = failureMechanism.StochasticSoilModels;

            stochasticSoilModels.AddRange(new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("name"),
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("name2")
            }, "some/path/to/file");

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(stochasticSoilModels.Count, entity.StochasticSoilModelEntities.Count);
            for (var i = 0; i < stochasticSoilModels.Count; i++)
            {
                AssertStochasticSoilModel(stochasticSoilModels[i],
                                          entity.StochasticSoilModelEntities.ElementAt(i));
            }

            string stochasticSoilModelCollectionSourcePath = entity.PipingFailureMechanismMetaEntities
                                                                   .Single()
                                                                   .StochasticSoilModelCollectionSourcePath;
            TestHelper.AssertAreEqualButNotSame(stochasticSoilModels.SourcePath, stochasticSoilModelCollectionSourcePath);
        }

        [Test]
        public void Create_WithoutSections_EmptyFailureMechanismSectionEntities()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNotNull(entity);
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);
            Assert.IsNull(entity.FailureMechanismSectionCollectionSourcePath);
        }

        [Test]
        public void Create_WithSections_ReturnsWithFailureMechanismSectionEntitiesAndPipingResultEntities()
        {
            // Setup
            const string filePath = "failureMechanismSections/File/Path";
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, filePath);
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            ICollection<FailureMechanismSectionEntity> failureMechanismSectionEntities = entity.FailureMechanismSectionEntities;
            Assert.AreEqual(1, failureMechanismSectionEntities.Count);
            Assert.AreEqual(1, failureMechanismSectionEntities.SelectMany(fms => fms.AdoptableFailureMechanismSectionResultEntities)
                                                              .Count());
            Assert.AreEqual(1, failureMechanismSectionEntities.SelectMany(fms => fms.PipingScenarioConfigurationPerFailureMechanismSectionEntities)
                                                              .Count());
            TestHelper.AssertAreEqualButNotSame(filePath, entity.FailureMechanismSectionCollectionSourcePath);
        }

        [Test]
        public void Create_WithSurfaceLines_ReturnFailureMechanismEntityWithSurfaceLineEntities()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            PipingSurfaceLineCollection surfaceLines = failureMechanism.SurfaceLines;

            surfaceLines.AddRange(new[]
            {
                CreateSurfaceLine(new Random(31))
            }, "path");

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(surfaceLines.Count, entity.SurfaceLineEntities.Count);
            for (var i = 0; i < surfaceLines.Count; i++)
            {
                AssertSurfaceLine(surfaceLines[i], entity.SurfaceLineEntities.ElementAt(i));
            }

            string surfaceLineCollectionSourcePath = entity.PipingFailureMechanismMetaEntities
                                                           .Single()
                                                           .SurfaceLineCollectionSourcePath;
            TestHelper.AssertAreEqualButNotSame(surfaceLines.SourcePath, surfaceLineCollectionSourcePath);
        }

        [Test]
        public void Create_WithCalculationGroup_ReturnFailureMechanismEntityWithCalculationGroupEntities()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            var semiProbabilisticPipingCalculationScenario = new SemiProbabilisticPipingCalculationScenario();
            var probabilisticPipingCalculationScenario = new ProbabilisticPipingCalculationScenario();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);
            failureMechanism.CalculationsGroup.Children.Add(semiProbabilisticPipingCalculationScenario);
            failureMechanism.CalculationsGroup.Children.Add(probabilisticPipingCalculationScenario);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(failureMechanism.CalculationsGroup.Name, entity.CalculationGroupEntity.Name);
            Assert.AreEqual(0, entity.CalculationGroupEntity.Order);

            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity.CalculationGroupEntity1
                                                                .OrderBy(cge => cge.Order)
                                                                .ToArray();
            Assert.AreEqual(1, childGroupEntities.Length);
            CalculationGroupEntity childGroupEntity = childGroupEntities[0];
            Assert.AreEqual(calculationGroup.Name, childGroupEntity.Name);
            Assert.AreEqual(0, childGroupEntity.Order);

            SemiProbabilisticPipingCalculationEntity[] semiProbabilisticPipingCalculationEntities = entity.CalculationGroupEntity
                                                                                                          .SemiProbabilisticPipingCalculationEntities
                                                                                                          .OrderBy(ce => ce.Order)
                                                                                                          .ToArray();
            Assert.AreEqual(1, semiProbabilisticPipingCalculationEntities.Length);
            SemiProbabilisticPipingCalculationEntity semiProbabilisticPipingCalculationEntity = semiProbabilisticPipingCalculationEntities[0];
            Assert.AreEqual(semiProbabilisticPipingCalculationScenario.Name, semiProbabilisticPipingCalculationEntity.Name);
            Assert.AreEqual(1, semiProbabilisticPipingCalculationEntity.Order);

            ProbabilisticPipingCalculationEntity[] probabilisticPipingCalculationEntities = entity.CalculationGroupEntity
                                                                                                  .ProbabilisticPipingCalculationEntities
                                                                                                  .OrderBy(ce => ce.Order)
                                                                                                  .ToArray();
            Assert.AreEqual(1, probabilisticPipingCalculationEntities.Length);
            ProbabilisticPipingCalculationEntity probabilisticPipingCalculationEntity = probabilisticPipingCalculationEntities[0];
            Assert.AreEqual(probabilisticPipingCalculationScenario.Name, probabilisticPipingCalculationEntity.Name);
            Assert.AreEqual(2, probabilisticPipingCalculationEntity.Order);
        }

        private static void AssertSurfaceLine(PipingSurfaceLine surfaceLine, SurfaceLineEntity entity)
        {
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            Assert.AreEqual(6, entity.PipingCharacteristicPointEntities.Count);
            CollectionAssert.IsEmpty(entity.MacroStabilityInwardsCharacteristicPointEntities);
        }

        private static void AssertStochasticSoilModel(PipingStochasticSoilModel model, StochasticSoilModelEntity entity)
        {
            Assert.AreEqual(model.Name, entity.Name);
            Assert.AreEqual(model.StochasticSoilProfiles.Count(), entity.PipingStochasticSoilProfileEntities.Count);
            CollectionAssert.IsEmpty(entity.MacroStabilityInwardsStochasticSoilProfileEntities);
        }

        private static PipingSurfaceLine CreateSurfaceLine(Random random)
        {
            var surfaceLine = new PipingSurfaceLine(nameof(PipingSurfaceLine))
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
            };

            var geometryPoints = new Point3D[10];
            for (var i = 0; i < 10; i++)
            {
                geometryPoints[i] = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            }

            surfaceLine.SetGeometry(geometryPoints);

            surfaceLine.SetBottomDitchDikeSideAt(geometryPoints[1]);
            surfaceLine.SetBottomDitchPolderSideAt(geometryPoints[2]);
            surfaceLine.SetDikeToeAtPolderAt(geometryPoints[3]);
            surfaceLine.SetDikeToeAtRiverAt(geometryPoints[4]);
            surfaceLine.SetDitchDikeSideAt(geometryPoints[5]);
            surfaceLine.SetDitchPolderSideAt(geometryPoints[7]);

            return surfaceLine;
        }
    }
}