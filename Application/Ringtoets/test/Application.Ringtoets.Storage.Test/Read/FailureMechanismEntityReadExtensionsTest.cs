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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityStoneCover.Data;

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
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = "Some comment"
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new TestFailureMechanism();

            // Call
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

            // Assert
            Assert.IsEmpty(failureMechanism.Sections);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.Comments, failureMechanism.Comments);
            Assert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void ReadAsStandAloneFailureMechanism_WithSectionsSet_ReturnsNewStandAloneFailureMechanismWithFailureMechanismSections()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
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
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = "Some comment",
                CalculationGroupEntity = new CalculationGroupEntity(),
                PipingFailureMechanismMetaEntities = new[]
                {
                    new PipingFailureMechanismMetaEntity
                    {
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
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.Comments, failureMechanism.Comments);
            Assert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsEmpty(failureMechanism.Sections);

            var pipingFailureMechanismMetaEntities = entity.PipingFailureMechanismMetaEntities.ToArray();
            var probabilityAssessmentInput = pipingFailureMechanismMetaEntities[0];
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
                CalculationGroupEntity = new CalculationGroupEntity(),
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
                CalculationGroupEntity = new CalculationGroupEntity(),
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
            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var pipingSectionResultEntity = new PipingSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            failureMechanismSectionEntity.PipingSectionResultEntities.Add(pipingSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
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
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithCalculationGroup_ReturnsNewPipingFailureMechanismWithCalculationGroupSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    IsEditable = 0,
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            IsEditable = 1,
                            Name = "Child1",
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
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
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("Child1", child1.Name);

            ICalculationBase child2 = failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual("Child2", child2.Name);
        }

        #endregion

        #region Grass Cover Erosion Inwards

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithCollector_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = "Some comment",
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = 3
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.Comments, failureMechanism.Comments);
            Assert.IsEmpty(failureMechanism.Sections);

            Assert.AreEqual(3, failureMechanism.GeneralInput.N);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithDikeProfilesSet_ReturnsGrassCoverErosionInwardsFailureMechanismWithDikeProfilesAdded()
        {
            // Setup
            string emptyDikeGeometryXml = new RoughnessPointXmlSerializer().ToXml(new RoughnessPoint[0]);
            string emptyForeshoreBinaryXml = new Point2DXmlSerializer().ToXml(new Point2D[0]);
            var entity = new FailureMechanismEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = 3
                    }
                },
                DikeProfileEntities =
                {
                    new DikeProfileEntity
                    {
                        DikeGeometryXml = emptyDikeGeometryXml,
                        ForeShoreXml = emptyForeshoreBinaryXml
                    },
                    new DikeProfileEntity
                    {
                        DikeGeometryXml = emptyDikeGeometryXml,
                        ForeShoreXml = emptyForeshoreBinaryXml
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            ReadConversionCollector collector = new ReadConversionCollector();

            // Call
            entity.ReadAsGrassCoverErosionInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.DikeProfiles.Count);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithSectionsSet_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var grassCoverErosionInwardsSectionResultEntity = new GrassCoverErosionInwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            failureMechanismSectionEntity.GrassCoverErosionInwardsSectionResultEntities.Add(grassCoverErosionInwardsSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithCalculationGroup_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithCalculationGroupSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    IsEditable = 0,
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            IsEditable = 1,
                            Name = "Child1",
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
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
                        N = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("Child1", child1.Name);

            ICalculationBase child2 = failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual("Child2", child2.Name);
        }

        #endregion

        #region Grass Cover Erosion Outwards

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsGrassCoverErosionOutwardsFailureMechanism_WithCollector_ReturnsNewGrassCoverErosionOutwardsFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = "Some comment",
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = 3
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionOutwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.Comments, failureMechanism.Comments);
            Assert.IsEmpty(failureMechanism.Sections);

            Assert.AreEqual(3, failureMechanism.GeneralInput.N);
        }

        [Test]
        public void ReadAsGrassCoverErosionOutwardsFailureMechanism_WithSectionsSet_ReturnsNewGrassCoverErosionOutwardsFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var grassCoverErosionOutwardsSectionResultEntity = new GrassCoverErosionOutwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            failureMechanismSectionEntity.GrassCoverErosionOutwardsSectionResultEntities.Add(grassCoverErosionOutwardsSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionOutwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }

        [Test]
        public void ReadAsGrassCoverErosionOutwardsFailureMechanism_WithWaveConditionsCalculationGroup_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithCalculationGroupSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    IsEditable = 0,
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            IsEditable = 1,
                            Name = "Child1",
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            IsEditable = 1,
                            Name = "Child2",
                            Order = 1
                        },
                    }
                },
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionOutwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.WaveConditionsCalculationGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.WaveConditionsCalculationGroup.Children[0];
            Assert.AreEqual("Child1", child1.Name);

            ICalculationBase child2 = failureMechanism.WaveConditionsCalculationGroup.Children[1];
            Assert.AreEqual("Child2", child2.Name);
        }

        [Test]
        public void ReadAsStabilityStoneCoverFailureMechanism_WithWaveConditionsCalculationGroup_ReturnsNewStabilityStoneCoverFailureMechanismWithCalculationGroupSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    IsEditable = 0,
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            IsEditable = 1,
                            Name = "Child1",
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            IsEditable = 1,
                            Name = "Child2",
                            Order = 1
                        },
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            entity.ReadAsStabilityStoneCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.WaveConditionsCalculationGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.WaveConditionsCalculationGroup.Children[0];
            Assert.AreEqual("Child1", child1.Name);

            ICalculationBase child2 = failureMechanism.WaveConditionsCalculationGroup.Children[1];
            Assert.AreEqual("Child2", child2.Name);
        }

        #endregion
    }
}