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
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

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
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant"
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new TestFailureMechanism();

            // Call
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

            // Assert
            Assert.IsEmpty(failureMechanism.Sections);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
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
            string dummyPointXml = new Point2DXmlSerializer().ToXml(dummyPoints);
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                Name = "section",
                FailureMechanismSectionPointXml = dummyPointXml
            };
            return failureMechanismSectionEntity;
        }

        #region Dune Erosion

        [Test]
        public void ReadAsDuneErosionFailureMechanism_WithMeta_ReturnFailureMechanismWithGeneralInputSet()
        {
            // Setup
            const int generalInputN = 3;

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                DuneErosionFailureMechanismMetaEntities =
                {
                    new DuneErosionFailureMechanismMetaEntity
                    {
                        N = generalInputN
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            entity.ReadAsDuneErosionFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(generalInputN, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
        }

        [Test]
        public void ReadAsDuneErosionFailureMechanism_WithSectionsSet_ReturnsNewDuneErosionFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var duneErosionSectionResultEntity = new DuneErosionSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            failureMechanismSectionEntity.DuneErosionSectionResultEntities.Add(duneErosionSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                DuneErosionFailureMechanismMetaEntities =
                {
                    new DuneErosionFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            entity.ReadAsDuneErosionFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }

        [Test]
        public void ReadAsDuneErosionFailureMechanism_WithHydraulicBoundaryLocations_ReturnsNewDuneErosionFailureMechanismWithLocationsSet()
        {
            // Setup
            const string locationAName = "DuneLocation A";
            const string locationBName = "DuneLocation B";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                DuneErosionFailureMechanismMetaEntities =
                {
                    new DuneErosionFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                DuneLocationEntities =
                {
                    new DuneLocationEntity
                    {
                        Order = 1,
                        Name = locationBName
                    },
                    new DuneLocationEntity
                    {
                        Order = 0,
                        Name = locationAName
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            entity.ReadAsDuneErosionFailureMechanism(failureMechanism, collector);

            // Assert
            ObservableList<DuneLocation> duneLocations = failureMechanism.DuneLocations;
            Assert.AreEqual(2, duneLocations.Count);

            Assert.AreEqual(locationAName, duneLocations[0].Name);
            Assert.AreEqual(locationBName, duneLocations[1].Name);
        }

        #endregion

        #region Piping

        [Test]
        public void ReadAsPipingFailureMechanism_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => entity.ReadAsPipingFailureMechanism(null, new ReadConversionCollector());

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
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
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant",
                CalculationGroupEntity = new CalculationGroupEntity(),
                PipingFailureMechanismMetaEntities = new[]
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        A = 0.95,
                        WaterVolumetricWeight = 5.48
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
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
            Assert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsEmpty(failureMechanism.Sections);

            var pipingFailureMechanismMetaEntities = entity.PipingFailureMechanismMetaEntities.ToArray();
            var pipingFailureMechanismMetaEntity = pipingFailureMechanismMetaEntities[0];
            Assert.AreEqual(pipingFailureMechanismMetaEntity.A, failureMechanism.PipingProbabilityAssessmentInput.A);
            Assert.AreEqual(pipingFailureMechanismMetaEntity.WaterVolumetricWeight, failureMechanism.GeneralInput.WaterVolumetricWeight.Value);

            Assert.IsNull(pipingFailureMechanismMetaEntity.StochasticSoilModelSourcePath);
            Assert.IsNull(pipingFailureMechanismMetaEntity.SurfaceLineSourcePath);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithStochasticSoilModelsSet_ReturnsNewPipingFailureMechanismWithStochasticSoilModelsSet()
        {
            // Setup
            string emptySegmentPointsXml = new Point2DXmlSerializer().ToXml(new Point2D[0]);
            var sourcePath = "some/Path";
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
                },
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        StochasticSoilModelSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.StochasticSoilModels.Count);
            Assert.AreEqual(sourcePath, failureMechanism.StochasticSoilModels.SourcePath);
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
            var sourcePath = "some/path";
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
                },
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        SurfaceLineSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.SurfaceLines.Count);
            Assert.AreEqual(sourcePath, failureMechanism.SurfaceLines.SourcePath);
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
                },
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity()
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
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Name = "Child1",
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Name = "Child2",
                            Order = 1
                        },
                    }
                },
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity()
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
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant",
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
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
            Assert.IsEmpty(failureMechanism.Sections);

            Assert.AreEqual(3, failureMechanism.GeneralInput.N);
            Assert.IsNull(failureMechanism.DikeProfiles.SourcePath);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithDikeProfilesSet_ReturnsGrassCoverErosionInwardsFailureMechanismWithDikeProfilesAdded()
        {
            // Setup
            string emptyDikeGeometryXml = new RoughnessPointXmlSerializer().ToXml(new RoughnessPoint[0]);
            string emptyForeshoreBinaryXml = new Point2DXmlSerializer().ToXml(new Point2D[0]);
            const string sourcePath = "some/path/to/my/dikeprofiles";
            var entity = new FailureMechanismEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = 3,
                        DikeProfileCollectionSourcePath = sourcePath
                    }
                },
                DikeProfileEntities =
                {
                    new DikeProfileEntity
                    {
                        Id = "idA",
                        DikeGeometryXml = emptyDikeGeometryXml,
                        ForeshoreXml = emptyForeshoreBinaryXml
                    },
                    new DikeProfileEntity
                    {
                        Id = "idB",
                        DikeGeometryXml = emptyDikeGeometryXml,
                        ForeshoreXml = emptyForeshoreBinaryXml
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
            Assert.AreEqual(sourcePath, failureMechanism.DikeProfiles.SourcePath);
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
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Name = "Child1",
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
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
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant",
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
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
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
        public void ReadAsGrassCoverErosionOutwardsFailureMechanism_WithWaveConditionsCalculationGroup_ReturnsNewGrassCoverErosionOutwardsFailureMechanismWithCalculationGroupSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Name = "Child1",
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
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
        public void ReadAsGrassCoverErosionOutwardsFailureMechanism_WithHydraulicBoundaryLocations_ReturnsNewGrassCoverErosionOutwardsFailureMechanismWithLocationsSet()
        {
            // Setup
            var locationAName = "Location A";
            var locationBName = "Location B";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                GrassCoverErosionOutwardsHydraulicLocationEntities =
                {
                    new GrassCoverErosionOutwardsHydraulicLocationEntity
                    {
                        Order = 1,
                        Name = locationBName
                    },
                    new GrassCoverErosionOutwardsHydraulicLocationEntity
                    {
                        Order = 0,
                        Name = locationAName
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionOutwardsFailureMechanism(failureMechanism, collector);

            // Assert
            var hydraulicBoundaryLocations = failureMechanism.HydraulicBoundaryLocations;
            Assert.AreEqual(2, hydraulicBoundaryLocations.Count);

            Assert.AreEqual(locationAName, hydraulicBoundaryLocations[0].Name);
            Assert.AreEqual(locationBName, hydraulicBoundaryLocations[1].Name);
        }

        #endregion

        #region Stability Stone Cover

        [Test]
        public void ReadAsStabilityStoneCoverFailureMechanism_WithWaveConditionsCalculationGroup_ReturnsNewStabilityStoneCoverFailureMechanismWithCalculationGroupSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Name = "Child1",
                            Order = 1
                        },
                        new CalculationGroupEntity
                        {
                            Name = "Child2",
                            Order = 0
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
            Assert.AreEqual("Child2", child1.Name);

            ICalculationBase child2 = failureMechanism.WaveConditionsCalculationGroup.Children[1];
            Assert.AreEqual("Child1", child2.Name);
        }

        [Test]
        public void ReadAsStabilityStoneCoverFailureMechanism_WithForeshoreProfiles_ReturnsNewStabilityStoneCoverFailureMechanismWithForeshoreProfilesSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Id = "Child1",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 0
                    },
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            entity.ReadAsStabilityStoneCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);

            ForeshoreProfile child1 = failureMechanism.ForeshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = failureMechanism.ForeshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);
        }

        #endregion

        #region Wave Impact Asphalt Cover

        [Test]
        public void ReadAsWaveImpactAsphaltCoverFailureMechanism_WithWaveConditionsCalculationGroup_ReturnsNewWaveImpactAsphaltCoverFailureMechanismWithCalculationGroupSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    Name = "Berekeningen",
                    Order = 0,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Name = "Child1",
                            Order = 1
                        },
                        new CalculationGroupEntity
                        {
                            Name = "Child2",
                            Order = 0
                        },
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaveImpactAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.WaveConditionsCalculationGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.WaveConditionsCalculationGroup.Children[0];
            Assert.AreEqual("Child2", child1.Name);

            ICalculationBase child2 = failureMechanism.WaveConditionsCalculationGroup.Children[1];
            Assert.AreEqual("Child1", child2.Name);
        }

        [Test]
        public void ReadAsWaveImpactAsphaltCoverFailureMechanism_WithForeshoreProfiles_ReturnsNewWaveImpactAsphaltCoverFailureMechanismWithForeshoreProfilesSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Id = "Child1",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 0
                    },
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaveImpactAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);

            ForeshoreProfile child1 = failureMechanism.ForeshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = failureMechanism.ForeshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);
        }

        #endregion

        #region Height Structures

        [Test]
        public void ReadAsHeightStructuresFailureMechanism_WithForeshoreProfiles_ReturnFailureMechanismWithForeshoreProfilesSet()
        {
            // Setup
            const int generalInputN = 7;
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Id = "Child1",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 0
                    },
                },
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = generalInputN
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            entity.ReadAsHeightStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);

            ForeshoreProfile child1 = failureMechanism.ForeshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = failureMechanism.ForeshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);

            Assert.AreEqual(generalInputN, failureMechanism.GeneralInput.N);
        }

        [Test]
        public void ReadAsHeightStructuresFailureMechanism_WithHeightStructures_ReturnFailureMechanismWithHeightStructuresSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                HeightStructureEntities =
                {
                    new HeightStructureEntity
                    {
                        Order = 2,
                        Name = "Child1",
                        Id = "a"
                    },
                    new HeightStructureEntity
                    {
                        Order = 1,
                        Name = "Child2",
                        Id = "b"
                    }
                },
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = 7
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            entity.ReadAsHeightStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.HeightStructures.Count);

            HeightStructure child1 = failureMechanism.HeightStructures[0];
            Assert.AreEqual("Child2", child1.Name);

            HeightStructure child2 = failureMechanism.HeightStructures[1];
            Assert.AreEqual("Child1", child2.Name);
        }

        #endregion

        #region Closing Structures

        [Test]
        public void ReadAsClosingStructuresFailureMechanism_WithForeshoreProfiles_ReturnFailureMechanismWithForeshoreProfilesSet()
        {
            // Setup
            const int generalInputN2A = 3;

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Id = "Child1",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 0
                    }
                },
                ClosingStructuresFailureMechanismMetaEntities =
                {
                    new ClosingStructuresFailureMechanismMetaEntity
                    {
                        N2A = generalInputN2A
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            entity.ReadAsClosingStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);

            ForeshoreProfile child1 = failureMechanism.ForeshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = failureMechanism.ForeshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);

            Assert.AreEqual(generalInputN2A, failureMechanism.GeneralInput.N2A);
        }

        [Test]
        public void ReadAsClosingStructuresFailureMechanism_WithClosingStructures_ReturnFailureMechanismWithClosingStructuresSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                ClosingStructureEntities =
                {
                    new ClosingStructureEntity
                    {
                        Order = 2,
                        Name = "Child1",
                        Id = "a"
                    },
                    new ClosingStructureEntity
                    {
                        Order = 1,
                        Name = "Child2",
                        Id = "b"
                    }
                },
                ClosingStructuresFailureMechanismMetaEntities =
                {
                    new ClosingStructuresFailureMechanismMetaEntity()
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            entity.ReadAsClosingStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.ClosingStructures.Count);

            ClosingStructure child1 = failureMechanism.ClosingStructures[0];
            Assert.AreEqual("Child2", child1.Name);

            ClosingStructure child2 = failureMechanism.ClosingStructures[1];
            Assert.AreEqual("Child1", child2.Name);
        }

        [Test]
        public void ReadAsClosingStructuresFailureMechanism_WithCalculationsAndGroups_ReturnFailureMechanismWithCalculationsAndGroups()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Name = "A",
                            Order = 1
                        }
                    },
                    ClosingStructuresCalculationEntities =
                    {
                        new ClosingStructuresCalculationEntity
                        {
                            Name = "B",
                            Order = 0
                        }
                    }
                },
                ClosingStructuresFailureMechanismMetaEntities =
                {
                    new ClosingStructuresFailureMechanismMetaEntity()
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            entity.ReadAsClosingStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase expectedCalculation = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("B", expectedCalculation.Name);
            Assert.IsInstanceOf<StructuresCalculation<ClosingStructuresInput>>(expectedCalculation);

            ICalculationBase expectedCalculationGroup = failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual("A", expectedCalculationGroup.Name);
            Assert.IsInstanceOf<CalculationGroup>(expectedCalculationGroup);
        }

        #endregion

        #region Stability Point Structures

        [Test]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithForeshoreProfiles_ReturnFailureMechanismWithForeshoreProfilesSet()
        {
            // Setup
            const int generalInputN = 5;

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Id = "Child1",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 0
                    }
                },
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity
                    {
                        N = generalInputN
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            entity.ReadAsStabilityPointStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.ForeshoreProfiles.Count);

            ForeshoreProfile child1 = failureMechanism.ForeshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = failureMechanism.ForeshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);

            Assert.AreEqual(generalInputN, failureMechanism.GeneralInput.N);
        }

        [Test]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithStabilityPointStructures_ReturnFailureMechanismWithStabilityPointStructuresSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                StabilityPointStructureEntities =
                {
                    new StabilityPointStructureEntity
                    {
                        Order = 2,
                        Name = "Child1",
                        Id = "a"
                    },
                    new StabilityPointStructureEntity
                    {
                        Order = 1,
                        Name = "Child2",
                        Id = "b"
                    }
                },
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity
                    {
                        N = 7
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            entity.ReadAsStabilityPointStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.StabilityPointStructures.Count);

            StabilityPointStructure child1 = failureMechanism.StabilityPointStructures[0];
            Assert.AreEqual("Child2", child1.Name);

            StabilityPointStructure child2 = failureMechanism.StabilityPointStructures[1];
            Assert.AreEqual("Child1", child2.Name);
        }

        [Test]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithCalculationsAndGroups_ReturnFailureMechanismWithCalculationsAndGroups()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Name = "A",
                            Order = 1
                        }
                    },
                    StabilityPointStructuresCalculationEntities =
                    {
                        new StabilityPointStructuresCalculationEntity
                        {
                            Name = "B",
                            Order = 0
                        }
                    }
                },
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity
                    {
                        N = 2
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            entity.ReadAsStabilityPointStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase expectedCalculation = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("B", expectedCalculation.Name);
            Assert.IsInstanceOf<StructuresCalculation<StabilityPointStructuresInput>>(expectedCalculation);

            ICalculationBase expectedCalculationGroup = failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual("A", expectedCalculationGroup.Name);
            Assert.IsInstanceOf<CalculationGroup>(expectedCalculationGroup);
        }

        #endregion
    }
}