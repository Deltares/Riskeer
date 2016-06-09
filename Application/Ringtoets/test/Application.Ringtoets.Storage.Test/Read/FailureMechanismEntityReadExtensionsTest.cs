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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Read
{
    public class FailureMechanismEntityReadExtensionsTest
    {
        [Test]
        public void ReadAsPipingFailureMechanism_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => entity.ReadAsPipingFailureMechanism(null);

            // Assert 
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsPipingFailureMechanism_WithCollector_ReturnsNewPipingFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = "Some comment",
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 2
                },
                PipingFailureMechanismMetaEntities = new[]
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        PipingFailureMechanismMetaEntityId = 3,
                        A = new decimal(0.95)
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.Comments, failureMechanism.Comments);
            Assert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsEmpty(failureMechanism.Sections);

            var pipingFailureMechanismMetaEntities = entity.PipingFailureMechanismMetaEntities.ToArray();
            var probabilityAssessmentInput = pipingFailureMechanismMetaEntities[0];
            Assert.AreEqual(probabilityAssessmentInput.PipingFailureMechanismMetaEntityId, failureMechanism.PipingProbabilityAssessmentInput.StorageId);
            Assert.AreEqual(probabilityAssessmentInput.A, failureMechanism.PipingProbabilityAssessmentInput.A);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithStochasticSoilModelsSet_ReturnsNewPipingFailureMechanismWithStochasticSoilModelsSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 3
                },
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity(),
                    new StochasticSoilModelEntity()
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.StochasticSoilModels.Count);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithSurfaceLines_ReturnsNewPipingFailureMechanismWithSurfaceLinesSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 6
                },
                SurfaceLineEntities =
                {
                    new SurfaceLineEntity(),
                    new SurfaceLineEntity()
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithSectionsSet_ReturnsNewPipingFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 1
                },
                FailureMechanismSectionEntities =
                {
                    new FailureMechanismSectionEntity
                    {
                        Name = "section",
                        FailureMechanismSectionPointEntities =
                        {
                            new FailureMechanismSectionPointEntity()
                        }
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithCalculationGroup_ReturnsNewPipingFailureMechanismWithCalculationGroupSet()
        {
            // Setup
            var entityId = new Random(1328).Next(1, 502);
            const int rootGroupId = 5;
            const int childGroup1Id = 7;
            const int childGroup2Id = 9;

            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = rootGroupId,
                    IsEditable = 0,
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            CalculationGroupEntityId = childGroup1Id,
                            IsEditable = 1,
                            Name = "Child1",
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            CalculationGroupEntityId = childGroup2Id,
                            IsEditable = 1,
                            Name = "Child2",
                            Order = 1
                        },
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            PipingFailureMechanism failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.AreEqual(rootGroupId, failureMechanism.CalculationsGroup.StorageId);
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("Child1", child1.Name);
            Assert.AreEqual(childGroup1Id, ((CalculationGroup) child1).StorageId);

            ICalculationBase child2 = failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual("Child2", child2.Name);
            Assert.AreEqual(childGroup2Id, ((CalculationGroup) child2).StorageId);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithCollector_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = "Some comment"
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsGrassCoverErosionInwardsFailureMechanism();

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.Comments, failureMechanism.Comments);
            Assert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithSectionsSet_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismSectionEntities =
                {
                    new FailureMechanismSectionEntity
                    {
                        Name = "section",
                        FailureMechanismSectionPointEntities =
                        {
                            new FailureMechanismSectionPointEntity()
                        }
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsGrassCoverErosionInwardsFailureMechanism();

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsStandAloneFailureMechanism_WithoutSectionsSet_ReturnsNewStandAloneFailureMechanism(bool isRelevant)
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = "Some comment"
            };

            // Call
            var failureMechanism = entity.ReadAsMacroStabilityInwardsFailureMechanism();

            // Assert
            Assert.IsEmpty(failureMechanism.Sections);
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.Comments, failureMechanism.Comments);
            Assert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void ReadAsStandAloneFailureMechanism_WithSectionsSet_ReturnsNewStandAloneFailureMechanismWithFailureMechanismSections()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismSectionEntities =
                {
                    new FailureMechanismSectionEntity
                    {
                        Name = "section",
                        FailureMechanismSectionPointEntities =
                        {
                            new FailureMechanismSectionPointEntity()
                        }
                    }
                }
            };

            // Call
            var failureMechanism = entity.ReadAsMacroStabilityInwardsFailureMechanism();

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }
    }
}