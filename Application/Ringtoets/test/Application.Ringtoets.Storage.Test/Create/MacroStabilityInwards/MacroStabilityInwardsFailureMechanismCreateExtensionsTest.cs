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
using Application.Ringtoets.Storage.Create.MacroStabilityInwards;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
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
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet()
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
            MacroStabilityInwardsFailureMechanismMetaEntity failureMechanismMetaEntity = entity.MacroStabilityInwardsFailureMechanismMetaEntities.ToArray()[0];
            Assert.AreEqual(failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A, failureMechanismMetaEntity.A);
            Assert.AreEqual(failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.B, failureMechanismMetaEntity.B);
            Assert.IsNull(failureMechanismMetaEntity.SectionLength);
            Assert.IsNull(failureMechanismMetaEntity.StochasticSoilModelCollectionSourcePath);
            Assert.IsNull(failureMechanismMetaEntity.SurfaceLineCollectionSourcePath);
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
                new MacroStabilityInwardsStochasticSoilModel("name"),
                new MacroStabilityInwardsStochasticSoilModel("name2")
            }, "some/path/to/file");

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(2, entity.StochasticSoilModelEntities.Count);
            for (var i = 0; i < stochasticSoilModels.Count; i++)
            {
                AssertStochasticSoilModel(stochasticSoilModels.ElementAt(i),
                                          entity.StochasticSoilModelEntities.ElementAt(i));
            }

            Assert.AreEqual(1, entity.MacroStabilityInwardsFailureMechanismMetaEntities.Count);
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
            MacroStabilityInwardsSurfaceLineCollection failureMechanismSurfaceLines = failureMechanism.SurfaceLines;

            failureMechanismSurfaceLines.AddRange(new[]
            {
                CreateSurfaceLine(new Random(31))
            }, "path");

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(failureMechanismSurfaceLines.Count, entity.SurfaceLineEntities.Count);
            for (var i = 0; i < failureMechanismSurfaceLines.Count; i++)
            {
                AssertSurfaceLine(failureMechanismSurfaceLines.ElementAt(i), entity.SurfaceLineEntities.ElementAt(i));
            }

            string surfaceLineCollectionSourcePath = entity.MacroStabilityInwardsFailureMechanismMetaEntities
                                                           .Single()
                                                           .SurfaceLineCollectionSourcePath;
            TestHelper.AssertAreEqualButNotSame(failureMechanismSurfaceLines.SourcePath, surfaceLineCollectionSourcePath);
        }

        private static void AssertSurfaceLine(MacroStabilityInwardsSurfaceLine surfaceLine, SurfaceLineEntity entity)
        {
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            Assert.AreEqual(0, entity.PipingCharacteristicPointEntities.Count);
            Assert.AreEqual(14, entity.MacroStabilityInwardsCharacteristicPointEntities.Count);
        }

        private MacroStabilityInwardsSurfaceLine CreateSurfaceLine(Random random)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(nameof(MacroStabilityInwardsSurfaceLine))
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
            };

            var geometryPoints = new Point3D[14];
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
            surfaceLine.SetTrafficLoadInsideAt(geometryPoints[9]);
            surfaceLine.SetTrafficLoadOutsideAt(geometryPoints[10]);
            surfaceLine.SetDikeToeAtRiverAt(geometryPoints[11]);
            surfaceLine.SetDikeToeAtPolderAt(geometryPoints[12]);
            surfaceLine.SetSurfaceLevelInsideAt(geometryPoints[13]);
            surfaceLine.SetSurfaceLevelOutsideAt(geometryPoints[0]);

            return surfaceLine;
        }


        [Test]
        public void Create_WithoutSections_EmptyFailureMechanismSectionEntities()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);
        }

        [Test]
        public void Create_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.AddSection(new TestFailureMechanismSection());

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MacroStabilityInwardsSectionResultEntities).Count());
        }

        private static void AssertStochasticSoilModel(MacroStabilityInwardsStochasticSoilModel model, StochasticSoilModelEntity entity)
        {
            Assert.AreEqual(model.Name, entity.Name);
            Assert.AreEqual(model.StochasticSoilProfiles.Count, entity.MacroStabilityInwardsStochasticSoilProfileEntities.Count);
        }
    }
}