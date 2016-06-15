﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PipingFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = isRelevant,
                Comments = "Some text",
                PipingProbabilityAssessmentInput =
                {
                    A = 0.9876
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            var entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.Piping, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.Comments, entity.Comments);
            CollectionAssert.IsEmpty(entity.StochasticSoilModelEntities);

            var failureMechanismMetaEntity = entity.PipingFailureMechanismMetaEntities.ToArray()[0];
            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.A, failureMechanismMetaEntity.A);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithStochasticSoilModels_ReturnsFailureMechanismEntityWithStochasticSoilModelEntities(bool isRelevant)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.StochasticSoilModels.Add(new StochasticSoilModel(-1, "name", "segmentName"));
            failureMechanism.StochasticSoilModels.Add(new StochasticSoilModel(-1, "name2", "segmentName2"));
            var registry = new PersistenceRegistry();

            // Call
            var entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(2, entity.StochasticSoilModelEntities.Count);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithSections_ReturnsWithFailureMechanismSectionEntitiesAndPipingResultEntities(bool isRelevant)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.AddSection(new TestFailureMechanismSection());
            failureMechanism.AddSection(new TestFailureMechanismSection());
            var registry = new PersistenceRegistry();

            // Call
            var entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(2, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(2, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingSectionResultEntities).Count());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithSurfaceLines_ReturnFailureMechanismEntityWithSurfaceLineEntities(bool isRelevant)
        {
            // Setup
            var random = new Random();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.Add(CreateSurfaceLine(random));

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(failureMechanism.SurfaceLines.Count, entity.SurfaceLineEntities.Count);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCalculationGroup_ReturnFailureMechanismEntityWithCalculationGroupEntities(bool isRelevant)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup("A", true));
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup("B", true));

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(failureMechanism.CalculationsGroup.Name, entity.CalculationGroupEntity.Name);
            Assert.AreEqual(Convert.ToByte(failureMechanism.CalculationsGroup.IsNameEditable), entity.CalculationGroupEntity.IsEditable);
            Assert.AreEqual(0, entity.CalculationGroupEntity.Order);

            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity.CalculationGroupEntity1
                                                                .OrderBy(cge => cge.Order)
                                                                .ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual("A", childGroupEntities[0].Name);
            Assert.AreEqual(1, childGroupEntities[0].IsEditable);
            Assert.AreEqual(0, childGroupEntities[0].Order);
            Assert.AreEqual("B", childGroupEntities[1].Name);
            Assert.AreEqual(1, childGroupEntities[1].IsEditable);
            Assert.AreEqual(1, childGroupEntities[1].Order);
        }

        private RingtoetsPipingSurfaceLine CreateSurfaceLine(Random random)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "A",
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
            };

            var geometryPoints = new Point3D[10];
            for (int i = 0; i < 10; i++)
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