﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
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
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
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
        public void ReadAsPipingFailureMechanism_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => ((FailureMechanismEntity) null).ReadAsPipingFailureMechanism(failureMechanism,
                                                                                                   collector);

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
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
        public void ReadAsPipingFailureMechanism_CollectorNull_ThrowsArgumentNullException()
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
        public void ReadAsPipingFailureMechanism_WithProperties_SetsPipingFailureMechanismWithProperties()
        {
            // Setup
            var random = new Random(31);
            bool isRelevant = random.NextBoolean();
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
                        A = random.NextDouble(),
                        WaterVolumetricWeight = random.NextDouble()
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
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            PipingFailureMechanismMetaEntity pipingFailureMechanismMetaEntity = entity.PipingFailureMechanismMetaEntities.Single();
            Assert.AreEqual(pipingFailureMechanismMetaEntity.A, failureMechanism.PipingProbabilityAssessmentInput.A);
            Assert.AreEqual(pipingFailureMechanismMetaEntity.WaterVolumetricWeight, failureMechanism.GeneralInput.WaterVolumetricWeight,
                            failureMechanism.GeneralInput.WaterVolumetricWeight.GetAccuracy());

            Assert.IsNull(pipingFailureMechanismMetaEntity.StochasticSoilModelCollectionSourcePath);
            Assert.IsNull(pipingFailureMechanismMetaEntity.SurfaceLineCollectionSourcePath);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithoutStochasticSoilModelsWithSourcePath_SetsFailureMechanismStochasticSoilModelsSourcePath()
        {
            // Setup
            const string sourcePath = "some/Path";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        StochasticSoilModelCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            PipingStochasticSoilModelCollection stochasticSoilModels = failureMechanism.StochasticSoilModels;
            Assert.AreEqual(sourcePath, stochasticSoilModels.SourcePath);
            CollectionAssert.IsEmpty(stochasticSoilModels);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithStochasticSoilModelsSet_SetsPipingFailureMechanismWithStochasticSoilModels()
        {
            // Setup
            string emptySegmentPointsXml = new Point2DXmlSerializer().ToXml(new Point2D[0]);
            const string sourcePath = "some/Path";
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
                        StochasticSoilModelCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.StochasticSoilModelEntities.Count, failureMechanism.StochasticSoilModels.Count);
            Assert.AreEqual(sourcePath, failureMechanism.StochasticSoilModels.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, failureMechanism.StochasticSoilModels.Select(s => s.Name));
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithoutSurfaceLinesWithSourcePath_SetsFailureMechanismSurfaceLinesSourcePath()
        {
            // Setup
            const string sourcePath = "some/path";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        SurfaceLineCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            PipingSurfaceLineCollection surfaceLines = failureMechanism.SurfaceLines;
            Assert.AreEqual(sourcePath, surfaceLines.SourcePath);
            CollectionAssert.IsEmpty(surfaceLines);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithSurfaceLines_SetsPipingFailureMechanismSurfaceLines()
        {
            // Setup
            string emptyPointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0]);
            const string sourcePath = "some/path";
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
                        SurfaceLineCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.SurfaceLineEntities.Count, failureMechanism.SurfaceLines.Count);
            Assert.AreEqual(sourcePath, failureMechanism.SurfaceLines.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                "2",
                "1"
            }, failureMechanism.SurfaceLines.Select(sl => sl.Name));
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithSectionsSet_SetsPipingFailureMechanismWithFailureMechanismSections()
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
            Assert.AreEqual(failureMechanismSectionEntity.PipingSectionResultEntities.Count,
                            failureMechanism.Sections.Count());
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithCalculationGroup_SetsPipingFailureMechanismCalculationGroup()
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
                        }
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
            Assert.AreEqual(entity.CalculationGroupEntity.CalculationGroupEntity1.Count,
                            failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("Child1", child1.Name);

            ICalculationBase child2 = failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual("Child2", child2.Name);
        }

        #endregion

        #region MacroStabilityInwards

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => ((FailureMechanismEntity) null).ReadAsMacroStabilityInwardsFailureMechanism(
                failureMechanism,
                collector);

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => entity.ReadAsMacroStabilityInwardsFailureMechanism(
                null, new ReadConversionCollector());

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", parameter);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => entity.ReadAsMacroStabilityInwardsFailureMechanism(
                new MacroStabilityInwardsFailureMechanism(), null);

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithPropertiesSet_SetsMacroStabilityInwardsFailureMechanismProperties()
        {
            // Setup
            var random = new Random(31);
            bool isRelevant = random.NextBoolean();
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant",
                CalculationGroupEntity = new CalculationGroupEntity(),
                MacroStabilityInwardsFailureMechanismMetaEntities = new[]
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity
                    {
                        A = random.NextDouble()
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            entity.ReadAsMacroStabilityInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            MacroStabilityInwardsFailureMechanismMetaEntity metaEntity = entity.MacroStabilityInwardsFailureMechanismMetaEntities.Single();
            Assert.AreEqual(metaEntity.A, failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A);

            Assert.IsNull(metaEntity.StochasticSoilModelCollectionSourcePath);
            Assert.IsNull(metaEntity.SurfaceLineCollectionSourcePath);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithNullPropertiesSet_SetsMacroStabilityInwardsFailureMechanismPropertiesToNaN()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                MacroStabilityInwardsFailureMechanismMetaEntities = new[]
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity()
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            entity.ReadAsMacroStabilityInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);

            MacroStabilityInwardsFailureMechanismMetaEntity metaEntity = entity.MacroStabilityInwardsFailureMechanismMetaEntities.First();
            Assert.AreEqual(metaEntity.A, failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A);
            Assert.IsNaN(failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.SectionLength);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithoutStochasticSoilModelsWithSourcePath_FailureMechanismWithStochasticSoilModelsSourcePathSet()
        {
            // Setup
            const string sourcePath = "some/Path";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity
                    {
                        StochasticSoilModelCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            entity.ReadAsMacroStabilityInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            MacroStabilityInwardsStochasticSoilModelCollection stochasticSoilModels = failureMechanism.StochasticSoilModels;
            Assert.AreEqual(sourcePath, stochasticSoilModels.SourcePath);
            CollectionAssert.IsEmpty(stochasticSoilModels);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithStochasticSoilModelsSet_MacroStabilityInwardsFailureMechanismWithStochasticSoilModelsSet()
        {
            // Setup
            string emptySegmentPointsXml = new Point2DXmlSerializer().ToXml(new Point2D[0]);
            const string sourcePath = "some/Path";
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
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity
                    {
                        StochasticSoilModelCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            entity.ReadAsMacroStabilityInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.StochasticSoilModelEntities.Count, failureMechanism.StochasticSoilModels.Count);
            Assert.AreEqual(sourcePath, failureMechanism.StochasticSoilModels.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, failureMechanism.StochasticSoilModels.Select(s => s.Name));
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithoutSurfaceLinesWithSourcePath_FailureMechanismWithSurfaceLinesSourcePathSet()
        {
            // Setup
            const string sourcePath = "some/path";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity
                    {
                        SurfaceLineCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            entity.ReadAsMacroStabilityInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            MacroStabilityInwardsSurfaceLineCollection surfaceLines = failureMechanism.SurfaceLines;
            Assert.AreEqual(sourcePath, surfaceLines.SourcePath);
            CollectionAssert.IsEmpty(surfaceLines);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithSurfaceLines_MacroStabilityInwardsFailureMechanismWithSurfaceLinesSet()
        {
            // Setup
            string emptyPointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0]);
            const string sourcePath = "some/path";
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
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity
                    {
                        SurfaceLineCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            entity.ReadAsMacroStabilityInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.SurfaceLineEntities.Count, failureMechanism.SurfaceLines.Count);
            Assert.AreEqual(sourcePath, failureMechanism.SurfaceLines.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                "2",
                "1"
            }, failureMechanism.SurfaceLines.Select(sl => sl.Name));
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithSectionsSet_MacroStabilityInwardsFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            failureMechanismSectionEntity.MacroStabilityInwardsSectionResultEntities.Add(new MacroStabilityInwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            });
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity()
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            entity.ReadAsMacroStabilityInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(failureMechanismSectionEntity.MacroStabilityInwardsSectionResultEntities.Count,
                            failureMechanism.Sections.Count());
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
            CollectionAssert.IsEmpty(failureMechanism.Sections);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var collector = new ReadConversionCollector();

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
                        }
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
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            Assert.AreEqual(3, failureMechanism.GeneralInput.N);
            Assert.IsNull(failureMechanism.ForeshoreProfiles.SourcePath);
        }

        [Test]
        public void ReadAsGrassCoverErosionOutwardsFailureMechanism_WithoutForeshoreProfilesWithSourcePath_ReturnsFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string fileLocation = "some/path/to/foreshoreProfiles";

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        N = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionOutwardsFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);
            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void ReadAsGrassCoverErosionOutwardsFailureMechanism_WithForeshoreProfilesAndSourcePath_ReturnsFailureMechanismWithForeshoreProfilesAndSourcePathSet()
        {
            // Setup
            const string fileLocation = "some/path/to/foreshoreProfiles";

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
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        N = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverErosionOutwardsFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(2, foreshoreProfiles.Count);
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);

            ForeshoreProfile child1 = foreshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = foreshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);
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
                        }
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
            const string locationAName = "Location A";
            const string locationBName = "Location B";
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
            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = failureMechanism.HydraulicBoundaryLocations;
            Assert.AreEqual(2, hydraulicBoundaryLocations.Count);

            Assert.AreEqual(locationAName, hydraulicBoundaryLocations[0].Name);
            Assert.AreEqual(locationBName, hydraulicBoundaryLocations[1].Name);
        }

        #endregion

        #region Stability Stone Cover

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsStabilityStoneCoverFailureMechanism_WithCollector_ReturnsNewStabilityStoneCoverFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant",
                CalculationGroupEntity = new CalculationGroupEntity(),
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity()
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            entity.ReadAsStabilityStoneCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            Assert.IsNull(failureMechanism.ForeshoreProfiles.SourcePath);
        }

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
                        }
                    }
                },
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity()
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
        public void ReadAsStabilityStoneCoverFailureMechanism_WithoutForeshoreProfilesWithSourcePath_ReturnsFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string fileLocation = "some/path/to/foreshoreProfiles";

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            entity.ReadAsStabilityStoneCoverFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);
            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void ReadAsStabilityStoneCoverFailureMechanism_WithForeshoreProfilesAndSourcePath_ReturnsFailureMechanismWithForeshoreProfilesAndSourcePathSet()
        {
            // Setup
            const string fileLocation = "some/path/to/foreshoreProfiles";
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
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            entity.ReadAsStabilityStoneCoverFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(2, foreshoreProfiles.Count);
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);

            ForeshoreProfile child1 = foreshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = foreshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);
        }

        #endregion

        #region Wave Impact Asphalt Cover

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsWaveImpactAsphaltCoverFailureMechanism_WithCollector_ReturnsNewWaveImpactAsphaltCoverFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant",
                CalculationGroupEntity = new CalculationGroupEntity(),
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity()
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaveImpactAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            Assert.IsNull(failureMechanism.ForeshoreProfiles.SourcePath);
        }

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
                        }
                    }
                },
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity()
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
        public void ReadAsWaveImpactAsphaltCoverFailureMechanism_WithoutForeshoreProfilesWithSourcePath_ReturnsFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string fileLocation = "some/path/to/foreshoreProfiles";

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaveImpactAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);
            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void ReadAsWaveImpactAsphaltCoverFailureMechanism_WithForeshoreProfilesAndSourcePath_ReturnsFailureMechanismWithForeshoreProfilesAndSourcePathSet()
        {
            // Setup
            const string fileLocation = "some/path/to/foreshoreProfiles";

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
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaveImpactAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(2, foreshoreProfiles.Count);
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);

            ForeshoreProfile child1 = foreshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = foreshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);
        }

        #endregion

        #region Height Structures

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsHeightStructuresFailureMechanism_WithCollector_ReturnsNewHeightStructuresFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant",
                CalculationGroupEntity = new CalculationGroupEntity(),
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            entity.ReadAsHeightStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            Assert.IsNull(failureMechanism.ForeshoreProfiles.SourcePath);
        }

        [Test]
        public void ReadAsHeightStructuresFailureMechanism_WithoutForeshoreProfilesWithSourcePath_ReturnsFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string fileLocation = "some/path/to/foreshoreProfiles";

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        N = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            entity.ReadAsHeightStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);
            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void ReadAsHeightStructuresFailureMechanism_WithForeshoreProfilesAndSourcePath_ReturnFailureMechanismWithForeshoreProfilesAndSourcePathSet()
        {
            // Setup
            const int generalInputN = 7;
            const string fileLocation = "some/location/to/foreshoreProfiles";

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
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = generalInputN,
                        ForeshoreProfileCollectionSourcePath = fileLocation
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            entity.ReadAsHeightStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(2, foreshoreProfiles.Count);
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);

            ForeshoreProfile child1 = foreshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = foreshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);

            Assert.AreEqual(generalInputN, failureMechanism.GeneralInput.N);
        }

        [Test]
        public void ReadAsHeightStructuresFailureMechanism_WithoutHeightStructuresWithSourcePath_ReturnsFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string path = "path/to/closingStructues";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = 7,
                        HeightStructureCollectionSourcePath = path
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            entity.ReadAsHeightStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            StructureCollection<HeightStructure> heightStructures =
                failureMechanism.HeightStructures;
            Assert.AreEqual(0, heightStructures.Count);
            Assert.AreEqual(path, heightStructures.SourcePath);
        }

        [Test]
        public void ReadAsHeightStructuresFailureMechanism_WithHeightStructures_ReturnFailureMechanismWithHeightStructuresSet()
        {
            // Setup
            const string sourcePath = "Some path";
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
                        N = 7,
                        HeightStructureCollectionSourcePath = sourcePath
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

            Assert.AreEqual(sourcePath, failureMechanism.HeightStructures.SourcePath);
        }

        [Test]
        public void ReadAsHeightStructuresFailureMechanism_WithoutStructuresWithPath_ReturnFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string sourcePath = "Some path";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = 7,
                        HeightStructureCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            entity.ReadAsHeightStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(sourcePath, failureMechanism.HeightStructures.SourcePath);
            CollectionAssert.IsEmpty(failureMechanism.HeightStructures);
        }

        #endregion

        #region Closing Structures

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsClosingStructuresFailureMechanism_WithCollector_ReturnsNewClosingStructuresFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant",
                CalculationGroupEntity = new CalculationGroupEntity(),
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
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            Assert.IsNull(failureMechanism.ForeshoreProfiles.SourcePath);
        }

        [Test]
        public void ReadAsClosingStructuresFailureMechanism_WithoutForeshoreProfilesWithSourcePath_ReturnsFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string fileLocation = "some/path/to/foreshoreProfiles";

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                ClosingStructuresFailureMechanismMetaEntities =
                {
                    new ClosingStructuresFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        N2A = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            entity.ReadAsClosingStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);
            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void ReadAsClosingStructuresFailureMechanism_WithForeshoreProfiles_ReturnFailureMechanismWithForeshoreProfilesSet()
        {
            // Setup
            const int generalInputN2A = 3;
            const string fileLocation = "some/location/to/foreshoreprofiles";

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
                        N2A = generalInputN2A,
                        ForeshoreProfileCollectionSourcePath = fileLocation
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            entity.ReadAsClosingStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(2, foreshoreProfiles.Count);
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);

            ForeshoreProfile child1 = foreshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = foreshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);

            Assert.AreEqual(generalInputN2A, failureMechanism.GeneralInput.N2A);
        }

        [Test]
        public void ReadAsClosingStructuresFailureMechanism_WithoutClosingStructuresWithSourcePath_ReturnsFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string path = "path/to/closingStructues";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                ClosingStructuresFailureMechanismMetaEntities =
                {
                    new ClosingStructuresFailureMechanismMetaEntity
                    {
                        ClosingStructureCollectionSourcePath = path
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            entity.ReadAsClosingStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            StructureCollection<ClosingStructure> closingStructures =
                failureMechanism.ClosingStructures;
            Assert.AreEqual(0, closingStructures.Count);
            Assert.AreEqual(path, closingStructures.SourcePath);
        }

        [Test]
        public void ReadAsClosingStructuresFailureMechanism_WithClosingStructures_ReturnFailureMechanismWithClosingStructuresSet()
        {
            // Setup
            const string sourcePath = "some/path";
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
                    new ClosingStructuresFailureMechanismMetaEntity
                    {
                        ClosingStructureCollectionSourcePath = sourcePath
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            entity.ReadAsClosingStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            StructureCollection<ClosingStructure> closingStructures = failureMechanism.ClosingStructures;
            Assert.AreEqual(2, closingStructures.Count);
            Assert.AreEqual(sourcePath, closingStructures.SourcePath);

            ClosingStructure child1 = closingStructures[0];
            Assert.AreEqual("Child2", child1.Name);

            ClosingStructure child2 = closingStructures[1];
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
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithCollector_ReturnsNewStabilityPointStructuresFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Really not relevant",
                CalculationGroupEntity = new CalculationGroupEntity(),
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            entity.ReadAsStabilityPointStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(entity.InputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, failureMechanism.NotRelevantComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            Assert.IsNull(failureMechanism.ForeshoreProfiles.SourcePath);
        }

        [Test]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithoutForeshoreProfilesWithSourcePath_ReturnsFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string fileLocation = "some/path/to/foreshoreProfiles";

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        N = 1
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            entity.ReadAsStabilityPointStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);
            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithForeshoreProfiles_ReturnFailureMechanismWithForeshoreProfilesSet()
        {
            // Setup
            const int generalInputN = 5;
            const string fileLocation = "some/location/to/foreshoreprofiles";

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
                        N = generalInputN,
                        ForeshoreProfileCollectionSourcePath = fileLocation
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            entity.ReadAsStabilityPointStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            Assert.AreEqual(2, foreshoreProfiles.Count);
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);

            ForeshoreProfile child1 = foreshoreProfiles[0];
            Assert.AreEqual("Child2", child1.Id);

            ForeshoreProfile child2 = foreshoreProfiles[1];
            Assert.AreEqual("Child1", child2.Id);

            Assert.AreEqual(generalInputN, failureMechanism.GeneralInput.N);
        }

        [Test]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithoutStabilityPointStructuresWithSourcePath_ReturnsFailureMechanismWithSourcePathSet()
        {
            // Setup
            const string path = "path/to/stabilityPointStructues";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity
                    {
                        N = 7,
                        StabilityPointStructureCollectionSourcePath = path
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            entity.ReadAsStabilityPointStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            StructureCollection<StabilityPointStructure> stabilityPointStructures =
                failureMechanism.StabilityPointStructures;
            Assert.AreEqual(0, stabilityPointStructures.Count);
            Assert.AreEqual(path, stabilityPointStructures.SourcePath);
        }

        [Test]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithStabilityPointStructures_ReturnFailureMechanismWithStabilityPointStructuresSet()
        {
            // Setup
            const string path = "path/to/stabilityPointStructures";
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
                        N = 7,
                        StabilityPointStructureCollectionSourcePath = path
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            entity.ReadAsStabilityPointStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            StructureCollection<StabilityPointStructure> stabilityPointStructures =
                failureMechanism.StabilityPointStructures;
            Assert.AreEqual(2, stabilityPointStructures.Count);
            Assert.AreEqual(path, stabilityPointStructures.SourcePath);

            StabilityPointStructure child1 = stabilityPointStructures[0];
            Assert.AreEqual("Child2", child1.Name);

            StabilityPointStructure child2 = stabilityPointStructures[1];
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