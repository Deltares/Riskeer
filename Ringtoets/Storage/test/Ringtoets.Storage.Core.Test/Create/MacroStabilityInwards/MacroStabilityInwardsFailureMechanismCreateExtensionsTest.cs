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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_WithoutAllPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.MacroStabilityInwards, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(failureMechanism.IsRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.InputComments.Body, entity.InputComments);
            Assert.AreEqual(failureMechanism.OutputComments.Body, entity.OutputComments);
            Assert.AreEqual(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);
            Assert.AreEqual(failureMechanism.FailureMechanismSectionSourcePath, entity.FailureMechanismSectionCollectionSourcePath);

            CollectionAssert.IsEmpty(entity.StochasticSoilModelEntities);
            MacroStabilityInwardsFailureMechanismMetaEntity failureMechanismMetaEntity = entity.MacroStabilityInwardsFailureMechanismMetaEntities.First();
            Assert.AreEqual(failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A, failureMechanismMetaEntity.A);
            Assert.AreEqual(failureMechanism.SurfaceLines.SourcePath, failureMechanismMetaEntity.SurfaceLineCollectionSourcePath);
            Assert.AreEqual(failureMechanism.StochasticSoilModels.SourcePath, failureMechanismMetaEntity.StochasticSoilModelCollectionSourcePath);
        }

        [Test]
        public void Create_WithPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
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
                MacroStabilityInwardsProbabilityAssessmentInput =
                {
                    A = random.NextDouble()
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.MacroStabilityInwards, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(failureMechanism.IsRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.InputComments.Body, entity.InputComments);
            Assert.AreEqual(failureMechanism.OutputComments.Body, entity.OutputComments);
            Assert.AreEqual(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);

            CollectionAssert.IsEmpty(entity.StochasticSoilModelEntities);
            CollectionAssert.IsEmpty(entity.SurfaceLineEntities);
            MacroStabilityInwardsFailureMechanismMetaEntity failureMechanismMetaEntity = entity.MacroStabilityInwardsFailureMechanismMetaEntities.First();
            Assert.AreEqual(failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A, failureMechanismMetaEntity.A);
            Assert.AreEqual(failureMechanism.SurfaceLines.SourcePath, failureMechanismMetaEntity.SurfaceLineCollectionSourcePath);
            Assert.AreEqual(failureMechanism.StochasticSoilModels.SourcePath, failureMechanismMetaEntity.StochasticSoilModelCollectionSourcePath);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
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
            Assert.IsNotNull(entity);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InputComments.Body, entity.InputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.OutputComments.Body, entity.OutputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.NotRelevantComments.Body, entity.NotRelevantComments);
        }

        [Test]
        public void Create_WithStochasticSoilModels_ReturnsFailureMechanismEntityWithStochasticSoilModelEntities()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsStochasticSoilModelCollection stochasticSoilModels = failureMechanism.StochasticSoilModels;

            stochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("name"),
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("name2")
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

            string stochasticSoilModelCollectionSourcePath = entity.MacroStabilityInwardsFailureMechanismMetaEntities
                                                                   .Single()
                                                                   .StochasticSoilModelCollectionSourcePath;
            TestHelper.AssertAreEqualButNotSame(stochasticSoilModels.SourcePath, stochasticSoilModelCollectionSourcePath);
        }

        [Test]
        public void Create_WithSurfaceLines_ReturnFailureMechanismEntityWithSurfaceLineEntities()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsSurfaceLineCollection surfaceLines = failureMechanism.SurfaceLines;

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

            string surfaceLineCollectionSourcePath = entity.MacroStabilityInwardsFailureMechanismMetaEntities
                                                           .Single()
                                                           .SurfaceLineCollectionSourcePath;
            TestHelper.AssertAreEqualButNotSame(surfaceLines.SourcePath, surfaceLineCollectionSourcePath);
        }

        [Test]
        public void Create_WithoutSections_EmptyFailureMechanismSectionEntities()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNotNull(entity);
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);
            Assert.IsNull(entity.FailureMechanismSectionCollectionSourcePath);
        }

        [Test]
        public void Create_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            const string filePath = "failureMechanismSections/file/path";
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            FailureMechanismSection testFailureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            failureMechanism.SetSections(new[]
            {
                testFailureMechanismSection
            }, filePath);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MacroStabilityInwardsSectionResultEntities).Count());
            TestHelper.AssertAreEqualButNotSame(filePath, entity.FailureMechanismSectionCollectionSourcePath);
        }

        [Test]
        public void Create_WithCalculationGroup_ReturnFailureMechanismWithCalculationGroupEntities()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            var calculation = new MacroStabilityInwardsCalculationScenario();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
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

            MacroStabilityInwardsCalculationEntity[] calculationEntities = entity.CalculationGroupEntity.MacroStabilityInwardsCalculationEntities
                                                                                 .OrderBy(ce => ce.Order)
                                                                                 .ToArray();
            Assert.AreEqual(1, calculationEntities.Length);
            MacroStabilityInwardsCalculationEntity calculationEntity = calculationEntities[0];
            Assert.AreEqual(calculation.Name, calculationEntity.Name);
            Assert.AreEqual(1, calculationEntity.Order);
        }

        private static void AssertSurfaceLine(MacroStabilityInwardsSurfaceLine surfaceLine, SurfaceLineEntity entity)
        {
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            Assert.AreEqual(0, entity.PipingCharacteristicPointEntities.Count);
            Assert.AreEqual(12, entity.MacroStabilityInwardsCharacteristicPointEntities.Count);
        }

        private MacroStabilityInwardsSurfaceLine CreateSurfaceLine(Random random)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(nameof(MacroStabilityInwardsSurfaceLine))
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
            };

            var geometryPoints = new Point3D[12];
            for (var i = 0; i < geometryPoints.Length; i++)
            {
                geometryPoints[i] = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            }

            surfaceLine.SetGeometry(geometryPoints);

            surfaceLine.SetDitchPolderSideAt(geometryPoints[1]);
            surfaceLine.SetBottomDitchPolderSideAt(geometryPoints[2]);
            surfaceLine.SetBottomDitchDikeSideAt(geometryPoints[3]);
            surfaceLine.SetDitchDikeSideAt(geometryPoints[4]);
            surfaceLine.SetDikeTopAtPolderAt(geometryPoints[5]);
            surfaceLine.SetDikeTopAtRiverAt(geometryPoints[6]);
            surfaceLine.SetShoulderBaseInsideAt(geometryPoints[7]);
            surfaceLine.SetShoulderTopInsideAt(geometryPoints[8]);
            surfaceLine.SetDikeToeAtRiverAt(geometryPoints[9]);
            surfaceLine.SetDikeToeAtPolderAt(geometryPoints[10]);
            surfaceLine.SetSurfaceLevelInsideAt(geometryPoints[11]);
            surfaceLine.SetSurfaceLevelOutsideAt(geometryPoints[0]);

            return surfaceLine;
        }

        private static void AssertStochasticSoilModel(MacroStabilityInwardsStochasticSoilModel model, StochasticSoilModelEntity entity)
        {
            Assert.AreEqual(model.Name, entity.Name);
            Assert.AreEqual(model.StochasticSoilProfiles.Count(), entity.MacroStabilityInwardsStochasticSoilProfileEntities.Count);
        }
    }
}