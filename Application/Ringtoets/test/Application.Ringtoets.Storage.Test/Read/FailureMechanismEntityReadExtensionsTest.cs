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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class FailureMechanismEntityReadExtensionsTest
    {
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
            var collector = new ReadConversionCollector();
            var failureMechanism = new TestFailureMechanism();

            // Call
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

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
                    CreateSimpleFailureMechanismSectionEntity()
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new TestFailureMechanism();

            // Call
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }

        private static FailureMechanismSectionEntity CreateSimpleFailureMechanismSectionEntity()
        {
            var dummyPoints = new[]
            {
                new Point2D(0, 0)
            };
            string dymmyPointXml = new Point2DXmlSerializer().ToXml(dummyPoints);
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                Name = "section",
                FailureMechanismSectionPointXml = dymmyPointXml
            };
            return failureMechanismSectionEntity;
        }

        #region Piping

        [Test]
        public void ReadAsPipingFailureMechanism_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => entity.ReadAsPipingFailureMechanism(null, new ReadConversionCollector());

            // Assert 
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", parameter);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => entity.ReadAsPipingFailureMechanism(new PipingFailureMechanism(), null);

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
                        A = 0.95,
                        UpliftCriticalSafetyFactor = 1.2
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

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
            Assert.AreEqual(probabilityAssessmentInput.UpliftCriticalSafetyFactor, failureMechanism.PipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor.Value);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithStochasticSoilModelsSet_ReturnsNewPipingFailureMechanismWithStochasticSoilModelsSet()
        {
            // Setup
            string emptySegmentPointsXml = new Point2DXmlSerializer().ToXml(new Point2D[0]);
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 3
                },
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointXml = emptySegmentPointsXml,
                        Name = "A",
                        Order = 1
                    },
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointXml = emptySegmentPointsXml,
                        Name = "B",
                        Order = 0
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.StochasticSoilModels.Count);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, failureMechanism.StochasticSoilModels.Select(s => s.Name));
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithSurfaceLines_ReturnsNewPipingFailureMechanismWithSurfaceLinesSet()
        {
            // Setup
            string emptyPointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0]);
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 6
                },
                SurfaceLineEntities =
                {
                    new SurfaceLineEntity
                    {
                        PointsXml = emptyPointsXml,
                        Name = "1",
                        Order = 1
                    },
                    new SurfaceLineEntity
                    {
                        PointsXml = emptyPointsXml,
                        Name = "2",
                        Order = 0
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count);
            CollectionAssert.AreEqual(new[]
            {
                "2",
                "1"
            }, failureMechanism.SurfaceLines.Select(sl => sl.Name));
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithSectionsSet_ReturnsNewPipingFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var pipingSectionResultEntity = new PipingSectionResultEntity
            {
                PipingSectionResultEntityId = entityId,
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            failureMechanismSectionEntity.PipingSectionResultEntities.Add(pipingSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 1
                },
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(entityId, failureMechanism.SectionResults.First().StorageId);
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
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

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

        #endregion

        #region Grass Cover Erosion Inwards

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithCollector_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            var inputId = random.Next(1, 57893);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = "Some comment",
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        GrassCoverErosionInwardsFailureMechanismMetaEntityId = inputId,
                        N = 3
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 456
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.Comments, failureMechanism.Comments);
            Assert.IsEmpty(failureMechanism.Sections);

            Assert.AreEqual(inputId, failureMechanism.GeneralInput.StorageId);
            Assert.AreEqual(3, failureMechanism.GeneralInput.N);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithDikeProfilesSet_ReturnsGrassCoverErosionInwardsFailureMechanismWithDikeProfilesAdded()
        {
            // Setup
            const int id1 = 4578;
            const int id2 = 384729847;
            string emptyDikeGeometryXml = new RoughnessPointXmlSerializer().ToXml(new RoughnessPoint[0]);
            string emptyForeshoreBinaryXml = new Point2DXmlSerializer().ToXml(new Point2D[0]);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        GrassCoverErosionInwardsFailureMechanismMetaEntityId = 2,
                        N = 3
                    }
                },
                DikeProfileEntities =
                {
                    new DikeProfileEntity
                    {
                        DikeProfileEntityId = id1,
                        DikeGeometryXml = emptyDikeGeometryXml,
                        ForeShoreXml = emptyForeshoreBinaryXml
                    },
                    new DikeProfileEntity
                    {
                        DikeProfileEntityId = id2,
                        DikeGeometryXml = emptyDikeGeometryXml,
                        ForeShoreXml = emptyForeshoreBinaryXml
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 567
                }
            };
            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            ReadConversionCollector collector = new ReadConversionCollector();

            // Call
            entity.ReadAsGrassCoverErosionInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.DikeProfiles.Count);
            Assert.AreEqual(id1, failureMechanism.DikeProfiles[0].StorageId);
            Assert.AreEqual(id2, failureMechanism.DikeProfiles[1].StorageId);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithSectionsSet_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var grassCoverErosionInwardsSectionResultEntity = new GrassCoverErosionInwardsSectionResultEntity
            {
                GrassCoverErosionInwardsSectionResultEntityId = entityId,
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            failureMechanismSectionEntity.GrassCoverErosionInwardsSectionResultEntities.Add(grassCoverErosionInwardsSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        GrassCoverErosionInwardsFailureMechanismMetaEntityId = 2,
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 9867
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(entityId, failureMechanism.SectionResults.First().StorageId);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithCalculationGroup_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithCalculationGroupSet()
        {
            // Setup
            var entityId = new Random(984356).Next(1, 502);
            const int rootGroupId = 1;
            const int childGroup1Id = 2;
            const int childGroup2Id = 3;

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
                },
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        GrassCoverErosionInwardsFailureMechanismMetaEntityId = 3456,
                        N = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionInwardsFailureMechanism(failureMechanism, collector);

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

        #endregion
    }
}