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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class AssessmentSectionEntityReadExtensionsTest
    {
        [Test]
        public void Read_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new AssessmentSectionEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune)]
        public void Read_WithCollector_ReturnsNewAssessmentSection(AssessmentSectionComposition assessmentSectionComposition)
        {
            // Setup
            const string testId = "testId";
            const string testName = "testName";
            const string comments = "Some text";
            const double norm = 0.05;
            var entity = new AssessmentSectionEntity
            {
                Id = testId,
                Name = testName,
                Composition = (short) assessmentSectionComposition,
                Comments = comments,
                Norm = norm
            };
            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.IsNotNull(section);
            Assert.AreEqual(testId, section.Id);
            Assert.AreEqual(testName, section.Name);
            Assert.AreEqual(comments, section.Comments.Body);
            Assert.AreEqual(norm, section.FailureMechanismContribution.Norm);
            Assert.AreEqual(assessmentSectionComposition, section.Composition);
            Assert.IsNull(section.HydraulicBoundaryDatabase);
            Assert.IsNull(section.ReferenceLine);
        }

        [Test]
        public void Read_WithReferenceLineEntities_ReturnsNewAssessmentSectionWithReferenceLineSet()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var random = new Random(21);
            double firstX = random.NextDouble();
            double firstY = random.NextDouble();
            double secondX = random.NextDouble();
            double secondY = random.NextDouble();

            var points = new[]
            {
                new Point2D(firstX, firstY),
                new Point2D(secondX, secondY)
            };
            entity.ReferenceLinePointXml = new Point2DXmlSerializer().ToXml(points);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            CollectionAssert.AreEqual(points, section.ReferenceLine.Points);
            Assert.AreEqual(Math2D.Length(points), section.PipingFailureMechanism.PipingProbabilityAssessmentInput.SectionLength);
        }

        [Test]
        public void Read_WithHydraulicDatabaseLocation_ReturnsNewAssessmentSectionWithHydraulicDatabaseSet()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var testLocation = "testLocation";
            var testVersion = "testVersion";
            entity.HydraulicDatabaseLocation = testLocation;
            entity.HydraulicDatabaseVersion = testVersion;
            entity.HydraulicLocationEntities.Add(new HydraulicLocationEntity
            {
                Name = "A",
                Order = 1
            });
            entity.HydraulicLocationEntities.Add(new HydraulicLocationEntity
            {
                Name = "B",
                Order = 0
            });

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.HydraulicBoundaryDatabase.Locations.Count);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, section.HydraulicBoundaryDatabase.Locations.Select(l => l.Name));
            Assert.AreEqual(testLocation, section.HydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(testVersion, section.HydraulicBoundaryDatabase.Version);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithPipingFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInPipingFailureMechanism(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            var parameterA = random.NextDouble()/10;
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        A = parameterA,
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.PipingFailureMechanism.IsRelevant);
            Assert.AreEqual(inputComments, section.PipingFailureMechanism.InputComments.Body);
            Assert.AreEqual(outputComments, section.PipingFailureMechanism.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.PipingFailureMechanism.NotRelevantComments.Body);
            Assert.AreEqual(parameterA, section.PipingFailureMechanism.PipingProbabilityAssessmentInput.A);
        }

        [Test]
        public void Read_WithPipingWithStochasticSoilModels_ReturnsPipingWithStochasticSoilModels()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            string emptySegmentPointsXml = new Point2DXmlSerializer().ToXml(new Point2D[0]);
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity(),
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointXml = emptySegmentPointsXml
                    },
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointXml = emptySegmentPointsXml
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.PipingFailureMechanism.StochasticSoilModels.Count);
        }

        [Test]
        public void Read_WithPipingWithSurfaceLines_ReturnsPipingWithSurfaceLines()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            string emptyPointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0]);
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity(),
                SurfaceLineEntities =
                {
                    new SurfaceLineEntity
                    {
                        PointsXml = emptyPointsXml
                    },
                    new SurfaceLineEntity
                    {
                        PointsXml = emptyPointsXml
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.PipingFailureMechanism.SurfaceLines.Count);
        }

        [Test]
        public void Read_WithPipingWithCalculationGroups_ReturnsPipingWithCalculationGroups()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            IList<ICalculationBase> childCalculationGroups = section.PipingFailureMechanism.CalculationsGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        public void Read_WithPipingWithFailureMechanismSections_ReturnsPipingWithFailureMechanismSections()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            };
            var sectionA = failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(0);
            var sectionB = failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(1);
            sectionA.PipingSectionResultEntities.Add(new PipingSectionResultEntity
            {
                FailureMechanismSectionEntity = sectionA
            });
            sectionB.PipingSectionResultEntities.Add(new PipingSectionResultEntity
            {
                FailureMechanismSectionEntity = sectionB
            });

            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.PipingFailureMechanism.Sections.Count());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithGrassCoverErosionInwardsWithProperties_ReturnsGrassCoverErosionInwardsWithProperties(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotRelevantText = "Really not relevant";

            var n = new Random(21).Next(1, 20);
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = originalInput,
                OutputComments = originalOutput,
                NotRelevantComments = originalNotRelevantText,
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = n
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.GrassCoverErosionInwards.IsRelevant);
            Assert.AreEqual(originalInput, section.GrassCoverErosionInwards.InputComments.Body);
            Assert.AreEqual(originalOutput, section.GrassCoverErosionInwards.OutputComments.Body);
            Assert.AreEqual(originalNotRelevantText, section.GrassCoverErosionInwards.NotRelevantComments.Body);
            Assert.AreEqual(n, section.GrassCoverErosionInwards.GeneralInput.N);
        }

        [Test]
        public void Read_WithGrassCoverErosionInwardsWithCalculationGroups_ReturnsGrassCoverErosionInwardsWithCalculationGroups()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            IList<ICalculationBase> childCalculationGroups = section.GrassCoverErosionInwards.CalculationsGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        public void Read_WithGrassCoverErosionInwardsWithFailureMechanismSection_ReturnsGrassCoverErosionInwardsWithFailureMechanismSections()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var rootGroupEntity = new CalculationGroupEntity();
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities(),
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = rootGroupEntity
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.GrassCoverErosionInwards.Sections.Count());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithGrassCoverErosionOutwardsWithProperties_ReturnsGrassCoverErosionOutwardsWithProperties(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            int n = new Random(21).Next(1, 20);
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentErosionOutwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = n
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.GrassCoverErosionOutwards.IsRelevant);
            Assert.AreEqual(inputComments, section.GrassCoverErosionOutwards.InputComments.Body);
            Assert.AreEqual(outputComments, section.GrassCoverErosionOutwards.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.GrassCoverErosionOutwards.NotRelevantComments.Body);
            Assert.AreEqual(n, section.GrassCoverErosionOutwards.GeneralInput.N);
        }

        [Test]
        public void Read_WithGrassCoverErosionOutwardsWithWaveConditionsCalculationGroups_ReturnsGrassCoverErosionOutwardsWithWaveConditionsCalculationGroups()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentErosionOutwards,
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            IList<ICalculationBase> childCalculationGroups = section.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        public void Read_WithGrassCoverErosionOutwardsWithForeshoreProfile_ReturnsGrassCoverErosionOutwardsWithForeshoreProfiles()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var profileAName = "profileA";
            var profileBName = "profileB";
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentErosionOutwards,
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = 2
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Order = 1,
                        Name = profileAName,
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    },
                    new ForeshoreProfileEntity
                    {
                        Order = 0,
                        Name = profileBName,
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            ObservableList<ForeshoreProfile> foreshoreProfiles = section.GrassCoverErosionOutwards.ForeshoreProfiles;
            CollectionAssert.AreEqual(new[]
            {
                profileBName,
                profileAName
            }, foreshoreProfiles.Select(fp => fp.Name));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithStabilityStoneCoverWithProperties_ReturnsStabilityStoneCoverWithProperties(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityStoneRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.StabilityStoneCover.IsRelevant);
            Assert.AreEqual(inputComments, section.StabilityStoneCover.InputComments.Body);
            Assert.AreEqual(outputComments, section.StabilityStoneCover.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.StabilityStoneCover.NotRelevantComments.Body);
        }

        [Test]
        public void Read_WithStabilityStoneCoverWithWaveConditionsCalculationGroups_ReturnsStabilityStoneCoverWithWaveConditionsCalculationGroups()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityStoneRevetment,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            IList<ICalculationBase> childCalculationGroups = section.StabilityStoneCover.WaveConditionsCalculationGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithStabilityStoneCoverWithForeshoreProfiles_ReturnsStabilityStoneCoverWithForeshoreProfiles(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var profileAName = "profileA";
            var profileBName = "profileB";
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityStoneRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Order = 1,
                        Name = profileAName,
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    },
                    new ForeshoreProfileEntity
                    {
                        Order = 0,
                        Name = profileBName,
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    }
                },
                IsRelevant = Convert.ToByte(isRelevant)
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            ObservableList<ForeshoreProfile> foreshoreProfiles = section.StabilityStoneCover.ForeshoreProfiles;
            CollectionAssert.AreEqual(new[]
            {
                profileBName,
                profileAName
            }, foreshoreProfiles.Select(fp => fp.Name));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithWaveImpactAsphaltCoverWithProperties_ReturnsWaveImpactAsphaltCoverWithProperties(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.WaveImpactOnAsphaltRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.WaveImpactAsphaltCover.IsRelevant);
            Assert.AreEqual(inputComments, section.WaveImpactAsphaltCover.InputComments.Body);
            Assert.AreEqual(outputComments, section.WaveImpactAsphaltCover.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.WaveImpactAsphaltCover.NotRelevantComments.Body);
        }

        [Test]
        public void Read_WithWaveImpactAsphaltCoverWithForeshoreProfiles_ReturnsWaveImpactAsphaltCoverWithForeshoreProfiles()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var profileAName = "profileA";
            var profileBName = "profileB";
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.WaveImpactOnAsphaltRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Order = 1,
                        Name = profileAName,
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    },
                    new ForeshoreProfileEntity
                    {
                        Order = 0,
                        Name = profileBName,
                        GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            ObservableList<ForeshoreProfile> foreshoreProfiles = section.WaveImpactAsphaltCover.ForeshoreProfiles;
            CollectionAssert.AreEqual(new[]
            {
                profileBName,
                profileAName
            }, foreshoreProfiles.Select(fp => fp.Name));
        }

        [Test]
        public void Read_WithWaveImpactAsphaltCoverWithWaveConditionsCalculationGroups_ReturnsWaveImpactAsphaltCoverWithWaveConditionsCalculationGroups()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.WaveImpactOnAsphaltRevetment,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            IList<ICalculationBase> childCalculationGroups = section.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithHeightStructuresWithProperties_ReturnsHeightStructuresWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StructureHeight,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = 5
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.HeightStructures.IsRelevant);
            Assert.AreEqual(inputComments, section.HeightStructures.InputComments.Body);
            Assert.AreEqual(outputComments, section.HeightStructures.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.HeightStructures.NotRelevantComments.Body);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithClosingStructuresWithProperties_ReturnsClosingStructuresWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.ReliabilityClosingOfStructure,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                ClosingStructureFailureMechanismMetaEntities =
                {
                    new ClosingStructureFailureMechanismMetaEntity
                    {
                        N2A = 3
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.ClosingStructures.IsRelevant);
            Assert.AreEqual(inputComments, section.ClosingStructures.InputComments.Body);
            Assert.AreEqual(outputComments, section.ClosingStructures.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.ClosingStructures.NotRelevantComments.Body);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithStabilityPointStructuresWithProperties_ReturnsStabilityPointStructuresWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityPointStructures,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity
                    {
                        N = 5
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.StabilityPointStructures.IsRelevant);
            Assert.AreEqual(inputComments, section.StabilityPointStructures.InputComments.Body);
            Assert.AreEqual(outputComments, section.StabilityPointStructures.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.StabilityPointStructures.NotRelevantComments.Body);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithStandAloneFailureMechanisms_ReturnsNewAssessmentSectionWithFailureMechanismsSet(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            FailureMechanismEntity macrostabilityInwards = CreateFailureMechanismEntity(
                isRelevant, FailureMechanismType.MacrostabilityInwards);
            FailureMechanismEntity macrostabilityOutwards = CreateFailureMechanismEntity(
                isRelevant, FailureMechanismType.MacrostabilityOutwards);
            FailureMechanismEntity microstability = CreateFailureMechanismEntity(
                isRelevant, FailureMechanismType.Microstability);
            FailureMechanismEntity strengthAndStabilityParallelConstruction = CreateFailureMechanismEntity(
                isRelevant, FailureMechanismType.StrengthAndStabilityParallelConstruction);
            FailureMechanismEntity waterOverpressureAsphaltRevetment = CreateFailureMechanismEntity(
                isRelevant, FailureMechanismType.WaterOverpressureAsphaltRevetment);
            FailureMechanismEntity grassRevetmentSlidingOutwards = CreateFailureMechanismEntity(
                isRelevant, FailureMechanismType.GrassRevetmentSlidingOutwards);
            FailureMechanismEntity grassRevetmentSlidingInwards = CreateFailureMechanismEntity(
                isRelevant, FailureMechanismType.GrassRevetmentSlidingInwards);
            FailureMechanismEntity duneErosion = CreateFailureMechanismEntity(
                isRelevant, FailureMechanismType.DuneErosion);
            FailureMechanismEntity technicalInnovations = CreateFailureMechanismEntity(
                isRelevant, FailureMechanismType.TechnicalInnovations);

            entity.FailureMechanismEntities.Add(macrostabilityInwards);
            entity.FailureMechanismEntities.Add(macrostabilityOutwards);
            entity.FailureMechanismEntities.Add(microstability);
            entity.FailureMechanismEntities.Add(strengthAndStabilityParallelConstruction);
            entity.FailureMechanismEntities.Add(waterOverpressureAsphaltRevetment);
            entity.FailureMechanismEntities.Add(grassRevetmentSlidingOutwards);
            entity.FailureMechanismEntities.Add(grassRevetmentSlidingInwards);
            entity.FailureMechanismEntities.Add(duneErosion);
            entity.FailureMechanismEntities.Add(technicalInnovations);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            AssertFailureMechanismEqual(isRelevant, 2, macrostabilityInwards.InputComments,
                                        macrostabilityInwards.OutputComments, macrostabilityInwards.NotRelevantComments,
                                        section.MacrostabilityInwards);
            AssertFailureMechanismEqual(isRelevant, 2, macrostabilityOutwards.InputComments,
                                        macrostabilityOutwards.OutputComments, macrostabilityOutwards.NotRelevantComments,
                                        section.MacrostabilityOutwards);
            AssertFailureMechanismEqual(isRelevant, 2, microstability.InputComments,
                                        microstability.OutputComments, microstability.NotRelevantComments, section.Microstability);
            AssertFailureMechanismEqual(isRelevant, 2, strengthAndStabilityParallelConstruction.InputComments,
                                        strengthAndStabilityParallelConstruction.OutputComments,
                                        strengthAndStabilityParallelConstruction.NotRelevantComments,
                                        section.StrengthStabilityLengthwiseConstruction);
            AssertFailureMechanismEqual(isRelevant, 2, waterOverpressureAsphaltRevetment.InputComments,
                                        waterOverpressureAsphaltRevetment.OutputComments,
                                        waterOverpressureAsphaltRevetment.NotRelevantComments,
                                        section.WaterPressureAsphaltCover);
            AssertFailureMechanismEqual(isRelevant, 2, grassRevetmentSlidingOutwards.InputComments,
                                        grassRevetmentSlidingOutwards.OutputComments, grassRevetmentSlidingOutwards.NotRelevantComments,
                                        section.GrassCoverSlipOffOutwards);
            AssertFailureMechanismEqual(isRelevant, 2, duneErosion.InputComments, duneErosion.OutputComments,
                                        duneErosion.NotRelevantComments, section.DuneErosion);
            AssertFailureMechanismEqual(isRelevant, 2, technicalInnovations.InputComments,
                                        technicalInnovations.OutputComments, technicalInnovations.NotRelevantComments,
                                        section.TechnicalInnovation);
        }

        private static AssessmentSectionEntity CreateAssessmentSectionEntity()
        {
            return new AssessmentSectionEntity
            {
                Norm = 1.0/30000
            };
        }

        private static FailureMechanismEntity CreateFailureMechanismEntity(bool isRelevant,
                                                                           FailureMechanismType failureMechanismType)
        {
            return new FailureMechanismEntity
            {
                FailureMechanismType = (short) failureMechanismType,
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = string.Concat("InputComment", failureMechanismType.ToString()),
                OutputComments = string.Concat("OutputComment", failureMechanismType.ToString()),
                NotRelevantComments = string.Concat("NotRelevantComment", failureMechanismType.ToString()),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            };
        }

        private static void AssertFailureMechanismEqual(bool expectedIsRelevant, int expectedSectionCount,
                                                        string expectedInputComments, string expectedOutputComments,
                                                        string expectedNotRelevantComments, IFailureMechanism failureMechanism)
        {
            Assert.AreEqual(expectedIsRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(expectedInputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(expectedOutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(expectedNotRelevantComments, failureMechanism.NotRelevantComments.Body);
            Assert.AreEqual(expectedSectionCount, failureMechanism.Sections.Count());
        }

        private static FailureMechanismSectionEntity[] CreateFailureMechanismSectionEntities()
        {
            var dummyPointData = new[]
            {
                new Point2D(0.0, 0.0)
            };
            string dummyPointXml = new Point2DXmlSerializer().ToXml(dummyPointData);
            return new[]
            {
                new FailureMechanismSectionEntity
                {
                    Name = "",
                    FailureMechanismSectionPointXml = dummyPointXml
                },
                new FailureMechanismSectionEntity
                {
                    Name = "",
                    FailureMechanismSectionPointXml = dummyPointXml
                }
            };
        }
    }
}