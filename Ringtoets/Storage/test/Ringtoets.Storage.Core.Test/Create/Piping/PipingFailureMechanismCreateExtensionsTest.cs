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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Primitives;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.Create.Piping;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.Piping
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
            TestDelegate test = () => failureMechanism.Create(null);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = random.NextBoolean(),
                InputComments =
                {
                    Body = "Some input text"
                },
                OutputComments =
                {
                    Body = "Some output text"
                },
                NotRelevantComments =
                {
                    Body = "Really not relevant"
                },
                PipingProbabilityAssessmentInput =
                {
                    A = random.NextDouble()
                },
                GeneralInput =
                {
                    WaterVolumetricWeight = random.NextRoundedDouble(0, 20)
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.Piping, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(failureMechanism.IsRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.InputComments.Body, entity.InputComments);
            Assert.AreEqual(failureMechanism.OutputComments.Body, entity.OutputComments);
            Assert.AreEqual(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);
            CollectionAssert.IsEmpty(entity.StochasticSoilModelEntities);
            CollectionAssert.IsEmpty(entity.SurfaceLineEntities);

            PipingFailureMechanismMetaEntity failureMechanismMetaEntity = entity.PipingFailureMechanismMetaEntities.ToArray()[0];
            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.A, failureMechanismMetaEntity.A);
            Assert.AreEqual(failureMechanism.GeneralInput.WaterVolumetricWeight.Value, failureMechanismMetaEntity.WaterVolumetricWeight);
            Assert.AreEqual(failureMechanism.SurfaceLines.SourcePath, failureMechanismMetaEntity.SurfaceLineCollectionSourcePath);
            Assert.AreEqual(failureMechanism.StochasticSoilModels.SourcePath, failureMechanismMetaEntity.StochasticSoilModelCollectionSourcePath);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                InputComments =
                {
                    Body = "Some input text"
                },
                OutputComments =
                {
                    Body = "Some output text"
                },
                NotRelevantComments =
                {
                    Body = "Really not relevant"
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InputComments.Body, entity.InputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.OutputComments.Body, entity.OutputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);
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
        public void Create_WithSections_ReturnsWithFailureMechanismSectionEntitiesAndPipingResultEntities()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingSectionResultEntities).Count());
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);
            failureMechanism.CalculationsGroup.Children.Add(calculation);

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

            PipingCalculationEntity[] calculationEntities = entity.CalculationGroupEntity.PipingCalculationEntities
                                                                  .OrderBy(ce => ce.Order)
                                                                  .ToArray();
            Assert.AreEqual(1, calculationEntities.Length);
            PipingCalculationEntity calculationEntity = calculationEntities[0];
            Assert.AreEqual(calculation.Name, calculationEntity.Name);
            Assert.AreEqual(1, calculationEntity.Order);
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

        private PipingSurfaceLine CreateSurfaceLine(Random random)
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