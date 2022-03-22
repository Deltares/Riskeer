﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil;
using Riskeer.Storage.Core.TestUtil.MacroStabilityInwards;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class FailureMechanismEntityReadExtensionsTest
    {
        private static FailureMechanismSectionEntity CreateSimpleFailureMechanismSectionEntity()
        {
            var dummyPoints = new[]
            {
                new Point2D(0, 0)
            };
            string dummyPointXml = new Point2DCollectionXmlSerializer().ToXml(dummyPoints);
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                Name = "section",
                FailureMechanismSectionPointXml = dummyPointXml
            };
            return failureMechanismSectionEntity;
        }

        #region GrassCoverSlipOffInwards

        [Test]
        public void ReadAsGrassCoverSlipOffInwardsFailureMechanism_WithPropertiesSet_SetsGrassCoverSlipOffInwardsFailureMechanismProperties()
        {
            // Setup
            var random = new Random(31);
            bool inAssembly = random.NextBoolean();
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                GrassCoverSlipOffInwardsFailureMechanismMetaEntities = new[]
                {
                    new GrassCoverSlipOffInwardsFailureMechanismMetaEntity
                    {
                        N = random.NextRoundedDouble(1.0, 20.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverSlipOffInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            GrassCoverSlipOffInwardsFailureMechanismMetaEntity metaEntity = entity.GrassCoverSlipOffInwardsFailureMechanismMetaEntities.Single();
            Assert.AreEqual(metaEntity.N, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
        }

        [Test]
        public void ReadAsGrassCoverSlipOffInwardsFailureMechanism_WithSectionsSet_ReturnsNewGrassCoverSlipOffInwardsFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            const string filePath = "failureMechanismSections/File/Path";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var grassCoverSlipOffInwardsSectionResultEntity = new GrassCoverSlipOffInwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(grassCoverSlipOffInwardsSectionResultEntity);

            failureMechanismSectionEntity.GrassCoverSlipOffInwardsSectionResultEntities.Add(grassCoverSlipOffInwardsSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                GrassCoverSlipOffInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverSlipOffInwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverSlipOffInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverSlipOffInwardsSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion

        #region WaterPressureAsphaltCover

        [Test]
        public void ReadAsWaterPressureAsphaltCoverFailureMechanism_WithPropertiesSet_SetsWaterPressureAsphaltCoverFailureMechanismProperties()
        {
            // Setup
            var random = new Random(31);
            bool inAssembly = random.NextBoolean();
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                WaterPressureAsphaltCoverFailureMechanismMetaEntities = new[]
                {
                    new WaterPressureAsphaltCoverFailureMechanismMetaEntity
                    {
                        N = random.NextRoundedDouble(1.0, 20.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaterPressureAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            WaterPressureAsphaltCoverFailureMechanismMetaEntity metaEntity = entity.WaterPressureAsphaltCoverFailureMechanismMetaEntities.Single();
            Assert.AreEqual(metaEntity.N, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
        }

        [Test]
        public void ReadAsWaterPressureAsphaltCoverFailureMechanism_WithSectionsSet_ReturnsNewWaterPressureAsphaltCoverFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            const string filePath = "failureMechanismSections/File/Path";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var waterPressureAsphaltCoverSectionResultEntity = new WaterPressureAsphaltCoverSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(waterPressureAsphaltCoverSectionResultEntity);

            failureMechanismSectionEntity.WaterPressureAsphaltCoverSectionResultEntities.Add(waterPressureAsphaltCoverSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                WaterPressureAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaterPressureAsphaltCoverFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaterPressureAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.WaterPressureAsphaltCoverSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion

        #region GrassCoverSlipOffOutwards

        [Test]
        public void ReadAsGrassCoverSlipOffOutwardsFailureMechanism_WithPropertiesSet_SetsGrassCoverSlipOffOutwardsFailureMechanismProperties()
        {
            // Setup
            var random = new Random(31);
            bool inAssembly = random.NextBoolean();
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                GrassCoverSlipOffOutwardsFailureMechanismMetaEntities = new[]
                {
                    new GrassCoverSlipOffOutwardsFailureMechanismMetaEntity
                    {
                        N = random.NextRoundedDouble(1.0, 20.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverSlipOffOutwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            GrassCoverSlipOffOutwardsFailureMechanismMetaEntity metaEntity = entity.GrassCoverSlipOffOutwardsFailureMechanismMetaEntities.Single();
            Assert.AreEqual(metaEntity.N, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
        }

        [Test]
        public void ReadAsGrassCoverSlipOffOutwardsFailureMechanism_WithSectionsSet_ReturnsNewGrassCoverSlipOffOutwardsFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            const string filePath = "failureMechanismSections/File/Path";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var grassCoverSlipOffOutwardsSectionResultEntity = new GrassCoverSlipOffOutwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(grassCoverSlipOffOutwardsSectionResultEntity);

            failureMechanismSectionEntity.GrassCoverSlipOffOutwardsSectionResultEntities.Add(grassCoverSlipOffOutwardsSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                GrassCoverSlipOffOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverSlipOffOutwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();

            // Call
            entity.ReadAsGrassCoverSlipOffOutwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverSlipOffOutwardsSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion

        #region Microstability

        [Test]
        public void ReadAsMicrostabilityFailureMechanism_WithPropertiesSet_SetsMicrostabilityFailureMechanismProperties()
        {
            // Setup
            var random = new Random(31);
            bool inAssembly = random.NextBoolean();
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                MicrostabilityFailureMechanismMetaEntities = new[]
                {
                    new MicrostabilityFailureMechanismMetaEntity
                    {
                        N = random.NextRoundedDouble(1.0, 20.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MicrostabilityFailureMechanism();

            // Call
            entity.ReadAsMicrostabilityFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            MicrostabilityFailureMechanismMetaEntity metaEntity = entity.MicrostabilityFailureMechanismMetaEntities.Single();
            Assert.AreEqual(metaEntity.N, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
        }

        [Test]
        public void ReadAsMicrostabilityFailureMechanism_WithSectionsSet_ReturnsNewMicrostabilityFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            const string filePath = "failureMechanismSections/File/Path";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var microStabilitySectionResultEntity = new MicrostabilitySectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(microStabilitySectionResultEntity);

            failureMechanismSectionEntity.MicrostabilitySectionResultEntities.Add(microStabilitySectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                MicrostabilityFailureMechanismMetaEntities =
                {
                    new MicrostabilityFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MicrostabilityFailureMechanism();

            // Call
            entity.ReadAsMicrostabilityFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MicrostabilitySectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion

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
            const string filePath = "failureMechanismSections/File/Path";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var duneErosionSectionResultEntity = new DuneErosionSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(duneErosionSectionResultEntity);

            failureMechanismSectionEntity.DuneErosionSectionResultEntities.Add(duneErosionSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
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
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.DuneErosionSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        [Test]
        public void ReadAsDuneErosionFailureMechanism_WitDuneLocations_ReturnsNewDuneErosionFailureMechanismWithLocationsSet()
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
            IObservableEnumerable<DuneLocation> duneLocations = failureMechanism.DuneLocations;
            Assert.AreEqual(2, duneLocations.Count());

            Assert.AreEqual(locationAName, duneLocations.ElementAt(0).Name);
            Assert.AreEqual(locationBName, duneLocations.ElementAt(1).Name);
        }

        [Test]
        public void ReadAsDuneErosionFailureMechanism_WithDuneLocationCalculations_ReturnsNewDuneErosionFailureMechanismWithLocationsAndCalculationsSet()
        {
            // Setup
            var duneLocationEntityA = new DuneLocationEntity
            {
                Order = 1,
                Name = "DuneLocation A"
            };

            var duneErosionFailureMechanismMetaEntity = new DuneErosionFailureMechanismMetaEntity
            {
                N = 1
            };

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                DuneErosionFailureMechanismMetaEntities =
                {
                    duneErosionFailureMechanismMetaEntity
                },
                DuneLocationEntities =
                {
                    duneLocationEntityA
                }
            };

            SetDuneLocationCalculationForTargetProbabilityCollectionEntities(duneErosionFailureMechanismMetaEntity, duneLocationEntityA);

            var duneLocation = new TestDuneLocation();
            var collector = new ReadConversionCollector();
            collector.Read(duneLocationEntityA, duneLocation);

            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            entity.ReadAsDuneErosionFailureMechanism(failureMechanism, collector);

            // Assert
            IEnumerable<DuneLocationCalculationForTargetProbabilityCollectionEntity> sortedCollectionEntities =
                entity.DuneErosionFailureMechanismMetaEntities.Single()
                      .DuneLocationCalculationForTargetProbabilityCollectionEntities
                      .OrderBy(e => e.Order);
            AssertDuneLocationCalculationsForTargetProbability(sortedCollectionEntities,
                                                               duneLocation,
                                                               failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities);
        }

        private static void AssertDuneLocationCalculationsForTargetProbability(
            IEnumerable<DuneLocationCalculationForTargetProbabilityCollectionEntity> expectedCalculationCollectionEntities,
            DuneLocation expectedHydraulicBoundaryLocation,
            IEnumerable<DuneLocationCalculationsForTargetProbability> actualCalculationCollections)
        {
            int expectedNrOfEntities = expectedCalculationCollectionEntities.Count();
            Assert.AreEqual(expectedNrOfEntities, actualCalculationCollections.Count());

            for (var j = 0; j < expectedNrOfEntities; j++)
            {
                AssertDuneLocationCalculationsForTargetProbability(expectedCalculationCollectionEntities.ElementAt(j),
                                                                   expectedHydraulicBoundaryLocation,
                                                                   actualCalculationCollections.ElementAt(j));
            }
        }

        private static void AssertDuneLocationCalculationsForTargetProbability(DuneLocationCalculationForTargetProbabilityCollectionEntity expectedCalculationCollectionEntity,
                                                                               DuneLocation expectedHydraulicBoundaryLocation,
                                                                               DuneLocationCalculationsForTargetProbability actualCalculations)
        {
            Assert.AreEqual(expectedCalculationCollectionEntity.TargetProbability, actualCalculations.TargetProbability);
            ICollection<DuneLocationCalculationEntity> expectedCalculations = expectedCalculationCollectionEntity.DuneLocationCalculationEntities;
            int expectedNrOfCalculations = expectedCalculations.Count;
            Assert.AreEqual(expectedNrOfCalculations, actualCalculations.DuneLocationCalculations.Count);

            for (var i = 0; i < expectedNrOfCalculations; i++)
            {
                DuneLocationCalculation actualCalculation = actualCalculations.DuneLocationCalculations[i];
                AssertDuneLocationCalculation(expectedCalculations.ElementAt(i),
                                              expectedHydraulicBoundaryLocation,
                                              actualCalculation);
            }
        }

        private static void AssertDuneLocationCalculation(DuneLocationCalculationEntity expectedCalculationEntity,
                                                          DuneLocation expectedDuneLocation,
                                                          DuneLocationCalculation actualCalculation)
        {
            Assert.AreSame(expectedDuneLocation, actualCalculation.DuneLocation);

            DuneLocationCalculationOutputEntity expectedOutput = expectedCalculationEntity.DuneLocationCalculationOutputEntities.SingleOrDefault();
            if (expectedOutput != null)
            {
                Assert.IsNotNull(actualCalculation.Output);
            }
            else
            {
                Assert.IsNull(actualCalculation.Output);
            }
        }

        private static void SetDuneLocationCalculationForTargetProbabilityCollectionEntities(DuneErosionFailureMechanismMetaEntity entity,
                                                                                             DuneLocationEntity duneLocationEntity)
        {
            entity.DuneLocationCalculationForTargetProbabilityCollectionEntities = new List<DuneLocationCalculationForTargetProbabilityCollectionEntity>();

            var random = new Random(21);
            int nrOfCollections = random.Next(1, 10);
            for (int i = nrOfCollections; i >= 0; i--)
            {
                entity.DuneLocationCalculationForTargetProbabilityCollectionEntities.Add(CreateDuneLocationCalculationForTargetProbabilityCollectionEntity(duneLocationEntity, i));
            }
        }

        private static DuneLocationCalculationForTargetProbabilityCollectionEntity CreateDuneLocationCalculationForTargetProbabilityCollectionEntity(
            DuneLocationEntity duneLocationEntity,
            int seed)
        {
            var random = new Random(seed);
            return new DuneLocationCalculationForTargetProbabilityCollectionEntity
            {
                TargetProbability = random.NextDouble(0, 0.1),
                Order = seed,
                DuneLocationCalculationEntities =
                {
                    new DuneLocationCalculationEntity
                    {
                        DuneLocationEntity = duneLocationEntity
                    }
                }
            };
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
            void Call() => ((FailureMechanismEntity) null).ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            void Call() => entity.ReadAsPipingFailureMechanism(null, new ReadConversionCollector());

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", parameter);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            void Call() => entity.ReadAsPipingFailureMechanism(new PipingFailureMechanism(), null);

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithProperties_SetsPipingFailureMechanismWithProperties()
        {
            // Setup
            var random = new Random(31);
            bool inAssembly = random.NextBoolean();
            PipingScenarioConfigurationType pipingScenarioConfigurationType = random.NextEnumValue<PipingScenarioConfigurationType>();
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                PipingFailureMechanismMetaEntities = new[]
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        A = random.NextDouble(),
                        WaterVolumetricWeight = random.NextDouble(),
                        PipingScenarioConfigurationType = Convert.ToByte(pipingScenarioConfigurationType)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            entity.ReadAsPipingFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            PipingFailureMechanismMetaEntity pipingFailureMechanismMetaEntity = entity.PipingFailureMechanismMetaEntities.Single();
            Assert.AreEqual(pipingFailureMechanismMetaEntity.A, failureMechanism.PipingProbabilityAssessmentInput.A);
            Assert.AreEqual(pipingFailureMechanismMetaEntity.WaterVolumetricWeight, failureMechanism.GeneralInput.WaterVolumetricWeight,
                            failureMechanism.GeneralInput.WaterVolumetricWeight.GetAccuracy());
            Assert.AreEqual(pipingScenarioConfigurationType, failureMechanism.ScenarioConfigurationType);

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
            var random = new Random(21);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            };
            string segmentPointsXml = new Point2DCollectionXmlSerializer().ToXml(geometry);
            const string sourcePath = "some/Path";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointXml = segmentPointsXml,
                        PipingStochasticSoilProfileEntities =
                        {
                            PipingStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                        },
                        Name = "A",
                        Order = 1
                    },
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointXml = segmentPointsXml,
                        PipingStochasticSoilProfileEntities =
                        {
                            PipingStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                        },
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
            string emptyPointsXml = new Point3DCollectionXmlSerializer().ToXml(new Point3D[0]);
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
            const string filePath = "failureMechanismSections/File/Path";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var pipingSectionResultEntity = new PipingSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(pipingSectionResultEntity);

            var pipingScenarioConfigurationPerFailureMechanismSectionEntity = new PipingScenarioConfigurationPerFailureMechanismSectionEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            failureMechanismSectionEntity.PipingSectionResultEntities.Add(pipingSectionResultEntity);
            failureMechanismSectionEntity.PipingScenarioConfigurationPerFailureMechanismSectionEntities.Add(pipingScenarioConfigurationPerFailureMechanismSectionEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
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
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count,
                            failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
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
            void Call() => ((FailureMechanismEntity) null).ReadAsMacroStabilityInwardsFailureMechanism(failureMechanism, collector);

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            void Call() => entity.ReadAsMacroStabilityInwardsFailureMechanism(null, new ReadConversionCollector());

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", parameter);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            void Call() => entity.ReadAsMacroStabilityInwardsFailureMechanism(new MacroStabilityInwardsFailureMechanism(), null);

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithPropertiesSet_SetsMacroStabilityInwardsFailureMechanismProperties()
        {
            // Setup
            var random = new Random(31);
            bool inAssembly = random.NextBoolean();
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculations text",
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
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
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

            MacroStabilityInwardsFailureMechanismMetaEntity metaEntity = entity.MacroStabilityInwardsFailureMechanismMetaEntities.Single();
            Assert.AreEqual(metaEntity.A, failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A);
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
            var random = new Random(21);
            string segmentPointsXml = new Point2DCollectionXmlSerializer().ToXml(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            });

            const string sourcePath = "some/Path";
            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointXml = segmentPointsXml,
                        MacroStabilityInwardsStochasticSoilProfileEntities =
                        {
                            MacroStabilityInwardsStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                        },
                        Name = "A",
                        Order = 1
                    },
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointXml = segmentPointsXml,
                        MacroStabilityInwardsStochasticSoilProfileEntities =
                        {
                            MacroStabilityInwardsStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                        },
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
            string emptyPointsXml = new Point3DCollectionXmlSerializer().ToXml(new Point3D[0]);
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
            const string filePath = "failureMechanismSections/File/Path";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var macroStabilityInwardsSectionResultEntity = new MacroStabilityInwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(macroStabilityInwardsSectionResultEntity);
            failureMechanismSectionEntity.MacroStabilityInwardsSectionResultEntities.Add(macroStabilityInwardsSectionResultEntity);

            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
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
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count,
                            failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MacroStabilityInwardsSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        [Test]
        public void ReadAsMacroStabilityInwardsFailureMechanism_WithCalculationsAndGroups_ReturnFailureMechanismWithCalculationAndGroups()
        {
            var entity = new FailureMechanismEntity
            {
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity()
                },
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
                    MacroStabilityInwardsCalculationEntities =
                    {
                        new MacroStabilityInwardsCalculationEntity
                        {
                            Name = "B",
                            TangentLineNumber = 1,
                            Order = 0,
                            LeftGridNrOfHorizontalPoints = 5,
                            LeftGridNrOfVerticalPoints = 5,
                            RightGridNrOfHorizontalPoints = 5,
                            RightGridNrOfVerticalPoints = 5,
                            ScenarioContribution = 0.1
                        }
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            entity.ReadAsMacroStabilityInwardsFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase expectedCalculation = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("B", expectedCalculation.Name);
            Assert.IsInstanceOf<MacroStabilityInwardsCalculationScenario>(expectedCalculation);

            ICalculationBase expectedCalculationGroup = failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual("A", expectedCalculationGroup.Name);
            Assert.IsInstanceOf<CalculationGroup>(expectedCalculationGroup);
        }

        #endregion

        #region Grass Cover Erosion Inwards

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithCollector_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithPropertiesSet(bool inAssembly)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = new Random(39).NextRoundedDouble(1.0, 20.0)
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
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            RoundedDouble actualN = failureMechanism.GeneralInput.N;
            Assert.AreEqual(entity.GrassCoverErosionInwardsFailureMechanismMetaEntities.Single().N, actualN, actualN.GetAccuracy());

            Assert.IsNull(failureMechanism.DikeProfiles.SourcePath);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithDikeProfilesSet_ReturnsGrassCoverErosionInwardsFailureMechanismWithDikeProfilesAdded()
        {
            // Setup
            string emptyDikeGeometryXml = new RoughnessPointCollectionXmlSerializer().ToXml(new RoughnessPoint[0]);
            string emptyForeshoreBinaryXml = new Point2DCollectionXmlSerializer().ToXml(new Point2D[0]);
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
            const string filePath = "failureMechanismSections/File/Path";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var grassCoverErosionInwardsSectionResultEntity = new GrassCoverErosionInwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(grassCoverErosionInwardsSectionResultEntity);
            failureMechanismSectionEntity.GrassCoverErosionInwardsSectionResultEntities.Add(grassCoverErosionInwardsSectionResultEntity);

            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
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
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath, failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionInwardsSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
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
        public void ReadAsGrassCoverErosionOutwardsFailureMechanism_WithCollector_ReturnsNewGrassCoverErosionOutwardsFailureMechanismWithPropertiesSet(bool inAssembly)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = new Random(39).NextRoundedDouble(1.0, 20.0)
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
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            RoundedDouble actualN = failureMechanism.GeneralInput.N;
            Assert.AreEqual(entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single().N, actualN, actualN.GetAccuracy());
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
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
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
            const string filePath = "failureMechanismSections/File/Path";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var grassCoverErosionOutwardsSectionResultEntity = new GrassCoverErosionOutwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(grassCoverErosionOutwardsSectionResultEntity);

            failureMechanismSectionEntity.GrassCoverErosionOutwardsSectionResultEntities.Add(grassCoverErosionOutwardsSectionResultEntity);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
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
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath, failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionOutwardsSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
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
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("Child1", child1.Name);

            ICalculationBase child2 = failureMechanism.CalculationsGroup.Children[1];
            Assert.AreEqual("Child2", child2.Name);
        }

        #endregion

        #region Stability Stone Cover

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsStabilityStoneCoverFailureMechanism_WithCollector_ReturnsNewStabilityStoneCoverFailureMechanismWithPropertiesSet(bool inAssembly)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity
                    {
                        N = new Random(39).NextRoundedDouble(1.0, 20.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            entity.ReadAsStabilityStoneCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            RoundedDouble actualN = failureMechanism.GeneralInput.N;
            Assert.AreEqual(entity.StabilityStoneCoverFailureMechanismMetaEntities.Single().N, actualN, actualN.GetAccuracy());

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
                    new StabilityStoneCoverFailureMechanismMetaEntity
                    {
                        N = 4.2
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            entity.ReadAsStabilityStoneCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("Child2", child1.Name);

            ICalculationBase child2 = failureMechanism.CalculationsGroup.Children[1];
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
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        N = 1.2
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
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 0
                    }
                },
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        N = 8.123
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

        [Test]
        public void ReadAsStabilityStoneCoverFailureMechanism_WithSectionsSet_StabilityStoneCoverFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            const string filePath = "failureMechanismSections/FilePath";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var stabilityStoneCoverSectionResultEntity = new StabilityStoneCoverSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(stabilityStoneCoverSectionResultEntity);
            failureMechanismSectionEntity.StabilityStoneCoverSectionResultEntities.Add(stabilityStoneCoverSectionResultEntity);

            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity
                    {
                        N = 2
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            entity.ReadAsStabilityStoneCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count,
                            failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StabilityStoneCoverSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion

        #region Wave Impact Asphalt Cover

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsWaveImpactAsphaltCoverFailureMechanism_WithCollector_ReturnsNewWaveImpactAsphaltCoverFailureMechanismWithPropertiesSet(bool inAssembly)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity
                    {
                        DeltaL = new Random(39).NextRoundedDouble(1.0, 2000.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaveImpactAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            RoundedDouble actualDeltaL = failureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL;
            Assert.AreEqual(entity.WaveImpactAsphaltCoverFailureMechanismMetaEntities.Single().DeltaL, actualDeltaL, actualDeltaL.GetAccuracy());

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
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity
                    {
                        DeltaL = new Random(39).NextRoundedDouble(1.0, 2000.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaveImpactAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);

            ICalculationBase child1 = failureMechanism.CalculationsGroup.Children[0];
            Assert.AreEqual("Child2", child1.Name);

            ICalculationBase child2 = failureMechanism.CalculationsGroup.Children[1];
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
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        DeltaL = new Random(39).NextRoundedDouble(1.0, 2000.0)
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
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 0
                    }
                },
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        DeltaL = new Random(39).NextRoundedDouble(1.0, 2000.0)
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

        [Test]
        public void ReadAsWaveImpactAsphaltCoverFailureMechanism_WithSectionsSet_WaveImpactAsphaltCoverFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            const string filePath = "failureMechanismSections/FilePath";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var waveImpactAsphaltCoverSectionResultEntity = new WaveImpactAsphaltCoverSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(waveImpactAsphaltCoverSectionResultEntity);
            failureMechanismSectionEntity.WaveImpactAsphaltCoverSectionResultEntities.Add(waveImpactAsphaltCoverSectionResultEntity);

            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity
                    {
                        DeltaL = new Random(39).NextRoundedDouble(1.0, 2000.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            entity.ReadAsWaveImpactAsphaltCoverFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count,
                            failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.WaveImpactAsphaltCoverSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion

        #region Height Structures

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsHeightStructuresFailureMechanism_WithCollector_ReturnsNewHeightStructuresFailureMechanismWithPropertiesSet(bool inAssembly)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = new Random(39).NextRoundedDouble(1.0, 20.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            entity.ReadAsHeightStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            RoundedDouble actualN = failureMechanism.GeneralInput.N;
            Assert.AreEqual(entity.HeightStructuresFailureMechanismMetaEntities.Single().N, actualN, actualN.GetAccuracy());

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
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
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

            RoundedDouble actualN = failureMechanism.GeneralInput.N;
            Assert.AreEqual(entity.HeightStructuresFailureMechanismMetaEntities.Single().N, actualN, actualN.GetAccuracy());
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

        [Test]
        public void ReadAsHeightStructuresFailureMechanism_WithSectionsSet_HeightStructuresFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            const string filePath = "failureMechanismSections/FilePath";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var heightStructuresSectionResultEntity = new HeightStructuresSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(heightStructuresSectionResultEntity);
            failureMechanismSectionEntity.HeightStructuresSectionResultEntities.Add(heightStructuresSectionResultEntity);

            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = 2
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            entity.ReadAsHeightStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count,
                            failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.HeightStructuresSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion

        #region Closing Structures

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsClosingStructuresFailureMechanism_WithCollector_ReturnsNewClosingStructuresFailureMechanismWithPropertiesSet(bool inAssembly)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
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
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
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
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
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
                            Order = 0,
                            IdenticalApertures = 1,
                            ScenarioContribution = 0.1
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

        [Test]
        public void ReadAsClosingStructuresFailureMechanism_WithSectionsSet_ClosingStructuresFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            const string filePath = "failureMechanismSections/FilePath";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var closingStructuresSectionResultEntity = new ClosingStructuresSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(closingStructuresSectionResultEntity);
            failureMechanismSectionEntity.ClosingStructuresSectionResultEntities.Add(closingStructuresSectionResultEntity);

            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
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
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count,
                            failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.ClosingStructuresSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion

        #region Stability Point Structures

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithCollector_ReturnsNewStabilityPointStructuresFailureMechanismWithPropertiesSet(bool inAssembly)
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity
                    {
                        N = new Random(39).NextRoundedDouble(1.0, 20.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            entity.ReadAsStabilityPointStructuresFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            RoundedDouble actualN = failureMechanism.GeneralInput.N;
            Assert.AreEqual(entity.StabilityPointStructuresFailureMechanismMetaEntities.Single().N, actualN, actualN.GetAccuracy());
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
            const double generalInputN = 5.0;
            const string fileLocation = "some/location/to/foreshoreprofiles";

            var entity = new FailureMechanismEntity
            {
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Id = "Child1",
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
                        Order = 1
                    },
                    new ForeshoreProfileEntity
                    {
                        Id = "Child2",
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>()),
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

            Assert.AreEqual(generalInputN, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N);
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
                            Order = 0,
                            ScenarioContribution = 0.1
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

        [Test]
        public void ReadAsStabilityPointStructuresFailureMechanism_WithSectionsSet_StabilityPointStructuresFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            const string filePath = "failureMechanismSections/FilePath";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var stabilityPointStructuresSectionResultEntity = new StabilityPointStructuresSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(stabilityPointStructuresSectionResultEntity);
            failureMechanismSectionEntity.StabilityPointStructuresSectionResultEntities.Add(stabilityPointStructuresSectionResultEntity);

            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
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
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count,
                            failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StabilityPointStructuresSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion

        #region PipingStructure

        [Test]
        public void ReadAsPipingStructureFailureMechanism_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingStructureFailureMechanism();
            var collector = new ReadConversionCollector();

            // Call
            void Call() => ((FailureMechanismEntity) null).ReadAsPipingStructureFailureMechanism(failureMechanism, collector);

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void ReadAsPipingStructureFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            void Call() => entity.ReadAsPipingStructureFailureMechanism(null, new ReadConversionCollector());

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", parameter);
        }

        [Test]
        public void ReadAsPipingStructureFailureMechanism_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            void Call() => entity.ReadAsPipingStructureFailureMechanism(new PipingStructureFailureMechanism(), null);

            // Assert 
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void ReadAsPipingStructureFailureMechanism_WithPropertiesSet_SetsPipingStructureFailureMechanismProperties()
        {
            // Setup
            var random = new Random(31);
            bool inAssembly = random.NextBoolean();
            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Really not in assembly",
                CalculationsInputComments = "Some calculation text",
                CalculationGroupEntity = new CalculationGroupEntity(),
                PipingStructureFailureMechanismMetaEntities = new[]
                {
                    new PipingStructureFailureMechanismMetaEntity
                    {
                        N = random.NextRoundedDouble(1.0, 20.0)
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingStructureFailureMechanism();

            // Call
            entity.ReadAsPipingStructureFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            PipingStructureFailureMechanismMetaEntity metaEntity = entity.PipingStructureFailureMechanismMetaEntities.Single();
            Assert.AreEqual(metaEntity.N, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
        }

        [Test]
        public void ReadAsPipingStructureFailureMechanism_WithSectionsSet_PipingStructureFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            const string filePath = "failureMechanismSections/FilePath";

            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var pipingStructureSectionResultEntity = new PipingStructureSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(pipingStructureSectionResultEntity);
            failureMechanismSectionEntity.PipingStructureSectionResultEntities.Add(pipingStructureSectionResultEntity);

            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                PipingStructureFailureMechanismMetaEntities =
                {
                    new PipingStructureFailureMechanismMetaEntity
                    {
                        N = 1.0
                    }
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new PipingStructureFailureMechanism();

            // Call
            entity.ReadAsPipingStructureFailureMechanism(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count,
                            failureMechanism.Sections.Count());
            Assert.AreEqual(entity.FailureMechanismSectionCollectionSourcePath,
                            failureMechanism.FailureMechanismSectionSourcePath);

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingStructureSectionResultEntities).Single(), failureMechanism.SectionResults.Single());
        }

        #endregion
    }
}