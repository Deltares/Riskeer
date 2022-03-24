// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;
using Riskeer.Integration.Plugin.Merge;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Plugin.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergeHandlerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var handler = new AssessmentSectionMergeHandler();

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionMergeHandler>(handler);
        }

        [Test]
        public void PerformMerge_TargetAssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();

            // Call
            void Call() => handler.PerformMerge(null, new AssessmentSectionMergeData(new AssessmentSection(AssessmentSectionComposition.Dike),
                                                                                     CreateDefaultConstructionProperties()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("targetAssessmentSection", exception.ParamName);
        }

        [Test]
        public void PerformMerge_MergeDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();

            // Call
            void Call() => handler.PerformMerge(new AssessmentSection(AssessmentSectionComposition.Dike), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("mergeData", exception.ParamName);
        }

        [Test]
        public void PerformMerge_WithAllFailureMechanismsToMerge_SetNewFailureMechanisms()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mergeData = new AssessmentSectionMergeData(sourceAssessmentSection, new AssessmentSectionMergeData.ConstructionProperties
            {
                MergePiping = true,
                MergeGrassCoverErosionInwards = true,
                MergeMacroStabilityInwards = true,
                MergeMicrostability = true,
                MergeStabilityStoneCover = true,
                MergeWaveImpactAsphaltCover = true,
                MergeWaterPressureAsphaltCover = true,
                MergeGrassCoverErosionOutwards = true,
                MergeGrassCoverSlipOffOutwards = true,
                MergeGrassCoverSlipOffInwards = true,
                MergeHeightStructures = true,
                MergeClosingStructures = true,
                MergePipingStructure = true,
                MergeStabilityPointStructures = true,
                MergeDuneErosion = true
            });

            // Call
            handler.PerformMerge(targetAssessmentSection, mergeData);

            // Assert
            Assert.AreSame(sourceAssessmentSection.Piping, targetAssessmentSection.Piping);
            Assert.AreSame(sourceAssessmentSection.GrassCoverErosionInwards, targetAssessmentSection.GrassCoverErosionInwards);
            Assert.AreSame(sourceAssessmentSection.MacroStabilityInwards, targetAssessmentSection.MacroStabilityInwards);
            Assert.AreSame(sourceAssessmentSection.Microstability, targetAssessmentSection.Microstability);
            Assert.AreSame(sourceAssessmentSection.StabilityStoneCover, targetAssessmentSection.StabilityStoneCover);
            Assert.AreSame(sourceAssessmentSection.WaveImpactAsphaltCover, targetAssessmentSection.WaveImpactAsphaltCover);
            Assert.AreSame(sourceAssessmentSection.WaterPressureAsphaltCover, targetAssessmentSection.WaterPressureAsphaltCover);
            Assert.AreSame(sourceAssessmentSection.GrassCoverErosionOutwards, targetAssessmentSection.GrassCoverErosionOutwards);
            Assert.AreSame(sourceAssessmentSection.GrassCoverSlipOffOutwards, targetAssessmentSection.GrassCoverSlipOffOutwards);
            Assert.AreSame(sourceAssessmentSection.GrassCoverSlipOffInwards, targetAssessmentSection.GrassCoverSlipOffInwards);
            Assert.AreSame(sourceAssessmentSection.HeightStructures, targetAssessmentSection.HeightStructures);
            Assert.AreSame(sourceAssessmentSection.ClosingStructures, targetAssessmentSection.ClosingStructures);
            Assert.AreSame(sourceAssessmentSection.PipingStructure, targetAssessmentSection.PipingStructure);
            Assert.AreSame(sourceAssessmentSection.StabilityPointStructures, targetAssessmentSection.StabilityPointStructures);
            Assert.AreSame(sourceAssessmentSection.DuneErosion, targetAssessmentSection.DuneErosion);
        }

        [Test]
        public void PerformMerge_WithNoFailureMechanismsToMerge_FailureMechanismsSame()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mergeData = new AssessmentSectionMergeData(sourceAssessmentSection, CreateDefaultConstructionProperties());

            // Call
            handler.PerformMerge(targetAssessmentSection, mergeData);

            // Assert
            Assert.AreNotSame(sourceAssessmentSection.Piping, targetAssessmentSection.Piping);
            Assert.AreNotSame(sourceAssessmentSection.GrassCoverErosionInwards, targetAssessmentSection.GrassCoverErosionInwards);
            Assert.AreNotSame(sourceAssessmentSection.MacroStabilityInwards, targetAssessmentSection.MacroStabilityInwards);
            Assert.AreNotSame(sourceAssessmentSection.Microstability, targetAssessmentSection.Microstability);
            Assert.AreNotSame(sourceAssessmentSection.StabilityStoneCover, targetAssessmentSection.StabilityStoneCover);
            Assert.AreNotSame(sourceAssessmentSection.WaveImpactAsphaltCover, targetAssessmentSection.WaveImpactAsphaltCover);
            Assert.AreNotSame(sourceAssessmentSection.WaterPressureAsphaltCover, targetAssessmentSection.WaterPressureAsphaltCover);
            Assert.AreNotSame(sourceAssessmentSection.GrassCoverErosionOutwards, targetAssessmentSection.GrassCoverErosionOutwards);
            Assert.AreNotSame(sourceAssessmentSection.GrassCoverSlipOffOutwards, targetAssessmentSection.GrassCoverSlipOffOutwards);
            Assert.AreNotSame(sourceAssessmentSection.GrassCoverSlipOffInwards, targetAssessmentSection.GrassCoverSlipOffInwards);
            Assert.AreNotSame(sourceAssessmentSection.HeightStructures, targetAssessmentSection.HeightStructures);
            Assert.AreNotSame(sourceAssessmentSection.ClosingStructures, targetAssessmentSection.ClosingStructures);
            Assert.AreNotSame(sourceAssessmentSection.PipingStructure, targetAssessmentSection.PipingStructure);
            Assert.AreNotSame(sourceAssessmentSection.StabilityPointStructures, targetAssessmentSection.StabilityPointStructures);
            Assert.AreNotSame(sourceAssessmentSection.DuneErosion, targetAssessmentSection.DuneErosion);
        }

        [Test]
        public void PerformMerge_WithAllFailureMechanismsToMerge_LogMessages()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            void Call() => handler.PerformMerge(targetAssessmentSection,
                                                new AssessmentSectionMergeData(sourceAssessmentSection, new AssessmentSectionMergeData.ConstructionProperties
                                                {
                                                    MergePiping = true,
                                                    MergeGrassCoverErosionInwards = true,
                                                    MergeMacroStabilityInwards = true,
                                                    MergeMicrostability = true,
                                                    MergeStabilityStoneCover = true,
                                                    MergeWaveImpactAsphaltCover = true,
                                                    MergeWaterPressureAsphaltCover = true,
                                                    MergeGrassCoverErosionOutwards = true,
                                                    MergeGrassCoverSlipOffOutwards = true,
                                                    MergeGrassCoverSlipOffInwards = true,
                                                    MergeHeightStructures = true,
                                                    MergeClosingStructures = true,
                                                    MergePipingStructure = true,
                                                    MergeStabilityPointStructures = true,
                                                    MergeDuneErosion = true
                                                }));

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(16, msgs.Length);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Piping' zijn vervangen.", msgs[1]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Grasbekleding erosie kruin en binnentalud' zijn vervangen.", msgs[2]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Macrostabiliteit binnenwaarts' zijn vervangen.", msgs[3]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Microstabiliteit' zijn vervangen.", msgs[4]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Stabiliteit steenzetting' zijn vervangen.", msgs[5]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Golfklappen op asfaltbekleding' zijn vervangen.", msgs[6]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Wateroverdruk bij asfaltbekleding' zijn vervangen.", msgs[7]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Grasbekleding erosie buitentalud' zijn vervangen.", msgs[8]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Grasbekleding afschuiven buitentalud' zijn vervangen.", msgs[9]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Grasbekleding afschuiven binnentalud' zijn vervangen.", msgs[10]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Hoogte kunstwerk' zijn vervangen.", msgs[11]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Betrouwbaarheid sluiting kunstwerk' zijn vervangen.", msgs[12]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Piping bij kunstwerk' zijn vervangen.", msgs[13]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Sterkte en stabiliteit puntconstructies' zijn vervangen.", msgs[14]);
                Assert.AreEqual("Gegevens van het generieke faalmechanisme 'Duinafslag' zijn vervangen.", msgs[15]);
            });
        }

        [Test]
        public void PerformMerge_WithFailureMechanismsToMerge_FailureMechanismsSame()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                SpecificFailureMechanisms =
                {
                    new SpecificFailureMechanism()
                }
            };
            IFailureMechanism[] originalFailureMechanisms = targetAssessmentSection.SpecificFailureMechanisms.ToArray();

            var failureMechanismsToMerge = new[]
            {
                new SpecificFailureMechanism(),
                new SpecificFailureMechanism()
            };
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            sourceAssessmentSection.SpecificFailureMechanisms.AddRange(failureMechanismsToMerge);

            var constructionProperties = new AssessmentSectionMergeData.ConstructionProperties();
            constructionProperties.MergeSpecificFailureMechanisms.AddRange(failureMechanismsToMerge);
            var mergeData = new AssessmentSectionMergeData(sourceAssessmentSection, constructionProperties);

            // Call
            handler.PerformMerge(targetAssessmentSection, mergeData);

            // Assert
            IEnumerable<IFailureMechanism> expectedFailureMechanisms = originalFailureMechanisms.Concat(failureMechanismsToMerge);
            CollectionAssert.AreEqual(expectedFailureMechanisms, targetAssessmentSection.SpecificFailureMechanisms);
        }

        [Test]
        public void PerformMerge_WithNoFailureMechanismsToMerge_FailureMechanismsNotSame()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                SpecificFailureMechanisms =
                {
                    new SpecificFailureMechanism()
                }
            };
            IFailureMechanism[] originalFailureMechanisms = targetAssessmentSection.SpecificFailureMechanisms.ToArray();

            var failureMechanismsToMerge = new[]
            {
                new SpecificFailureMechanism(),
                new SpecificFailureMechanism()
            };
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            sourceAssessmentSection.SpecificFailureMechanisms.AddRange(failureMechanismsToMerge);

            var mergeData = new AssessmentSectionMergeData(sourceAssessmentSection, new AssessmentSectionMergeData.ConstructionProperties());

            // Call
            handler.PerformMerge(targetAssessmentSection, mergeData);

            // Assert
            CollectionAssert.AreEqual(originalFailureMechanisms, targetAssessmentSection.SpecificFailureMechanisms);
        }

        [Test]
        public void PerformMerge_WithFailureMechanismsToMergeNotInSourceAssessmentSection_ThrowsArgumentException()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var failureMechanismsToMerge = new[]
            {
                new SpecificFailureMechanism(),
                new SpecificFailureMechanism()
            };
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var constructionProperties = new AssessmentSectionMergeData.ConstructionProperties();
            constructionProperties.MergeSpecificFailureMechanisms.AddRange(failureMechanismsToMerge);
            var mergeData = new AssessmentSectionMergeData(sourceAssessmentSection, constructionProperties);

            // Call
            void Call() => handler.PerformMerge(targetAssessmentSection, mergeData);

            // Assert
            const string expectedMessage = "MergeSpecificFailurePaths must contain items of the assessment section in mergeData.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void PerformMerge_WithFailureMechanismsToMerge_LogsMessages()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                SpecificFailureMechanisms =
                {
                    new SpecificFailureMechanism()
                }
            };

            var failureMechanismsToMerge = new[]
            {
                new SpecificFailureMechanism
                {
                    Name = "Path 1"
                },
                new SpecificFailureMechanism
                {
                    Name = "Path 2"
                }
            };
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            sourceAssessmentSection.SpecificFailureMechanisms.AddRange(failureMechanismsToMerge);

            var constructionProperties = new AssessmentSectionMergeData.ConstructionProperties();
            constructionProperties.MergeSpecificFailureMechanisms.AddRange(failureMechanismsToMerge);
            var mergeData = new AssessmentSectionMergeData(sourceAssessmentSection, constructionProperties);

            // Call
            void Call() => handler.PerformMerge(targetAssessmentSection, mergeData);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                int nrOfFailureMechanisms = failureMechanismsToMerge.Length;
                Assert.AreEqual(nrOfFailureMechanisms + 1, msgs.Length);
                for (int i = 0; i < nrOfFailureMechanisms; i++)
                {
                    string failureMechanismName = failureMechanismsToMerge[i].Name;
                    Assert.AreEqual($"Faalmechanisme '{failureMechanismName}' en de bijbehorende gegevens zijn toegevoegd aan de lijst van specifieke faalmechanismen.", msgs[i + 1]);
                }
            });
        }

        [Test]
        public void GivenFailureMechanismWithWithCalculations_WhenCalculationHasReferenceToHydraulicBoundaryLocation_ThenReferenceUpdated()
        {
            // Given
            var handler = new AssessmentSectionMergeHandler();

            var targetLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "location 1", 1, 1),
                new HydraulicBoundaryLocation(2, "location 2", 2, 2)
            };

            var sourceLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "location 1", 1, 1),
                new HydraulicBoundaryLocation(2, "location 2", 2, 2)
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(targetLocations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(sourceLocations);
            sourceAssessmentSection.Piping.CalculationsGroup.Children.Add(new TestPipingCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[0]
                }
            });
            sourceAssessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[1]
                }
            });
            sourceAssessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[0]
                }
            });
            sourceAssessmentSection.HeightStructures.CalculationsGroup.Children.Add(new StructuresCalculationScenario<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[1]
                }
            });
            sourceAssessmentSection.ClosingStructures.CalculationsGroup.Children.Add(new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[0]
                }
            });
            sourceAssessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[1]
                }
            });
            sourceAssessmentSection.StabilityStoneCover.CalculationsGroup.Children.Add(new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[0]
                }
            });
            sourceAssessmentSection.WaveImpactAsphaltCover.CalculationsGroup.Children.Add(new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[1]
                }
            });
            sourceAssessmentSection.GrassCoverErosionOutwards.CalculationsGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[0]
                }
            });

            // When
            handler.PerformMerge(targetAssessmentSection, new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties
                                     {
                                         MergePiping = true,
                                         MergeGrassCoverErosionInwards = true,
                                         MergeMacroStabilityInwards = true,
                                         MergeStabilityStoneCover = true,
                                         MergeWaveImpactAsphaltCover = true,
                                         MergeGrassCoverErosionOutwards = true,
                                         MergeHeightStructures = true,
                                         MergeClosingStructures = true,
                                         MergeStabilityPointStructures = true
                                     }));

            // Then
            var pipingCalculation = (TestPipingCalculationScenario) targetAssessmentSection.Piping.Calculations.Single();
            Assert.AreSame(targetLocations[0], pipingCalculation.InputParameters.HydraulicBoundaryLocation);

            var grassInwardsCalculation = (GrassCoverErosionInwardsCalculationScenario) targetAssessmentSection.GrassCoverErosionInwards.Calculations.Single();
            Assert.AreSame(targetLocations[1], grassInwardsCalculation.InputParameters.HydraulicBoundaryLocation);

            var macroStabilityInwardsCalculation = (MacroStabilityInwardsCalculation) targetAssessmentSection.MacroStabilityInwards.Calculations.Single();
            Assert.AreSame(targetLocations[0], macroStabilityInwardsCalculation.InputParameters.HydraulicBoundaryLocation);

            var heightStructuresCalculation = (StructuresCalculation<HeightStructuresInput>) targetAssessmentSection.HeightStructures.Calculations.Single();
            Assert.AreSame(targetLocations[1], heightStructuresCalculation.InputParameters.HydraulicBoundaryLocation);

            var closingStructuresCalculation = (StructuresCalculationScenario<ClosingStructuresInput>) targetAssessmentSection.ClosingStructures.Calculations.Single();
            Assert.AreSame(targetLocations[0], closingStructuresCalculation.InputParameters.HydraulicBoundaryLocation);

            var stabilityPointStructuresCalculation = (StructuresCalculationScenario<StabilityPointStructuresInput>) targetAssessmentSection.StabilityPointStructures.Calculations.Single();
            Assert.AreSame(targetLocations[1], stabilityPointStructuresCalculation.InputParameters.HydraulicBoundaryLocation);

            var stabilityStoneCoverCalculation = (StabilityStoneCoverWaveConditionsCalculation) targetAssessmentSection.StabilityStoneCover.Calculations.Single();
            Assert.AreSame(targetLocations[0], stabilityStoneCoverCalculation.InputParameters.HydraulicBoundaryLocation);

            var waveImpactAsphaltCoverCalculation = (WaveImpactAsphaltCoverWaveConditionsCalculation) targetAssessmentSection.WaveImpactAsphaltCover.Calculations.Single();
            Assert.AreSame(targetLocations[1], waveImpactAsphaltCoverCalculation.InputParameters.HydraulicBoundaryLocation);

            var grassOutwardsCalculation = (GrassCoverErosionOutwardsWaveConditionsCalculation) targetAssessmentSection.GrassCoverErosionOutwards.Calculations.Single();
            Assert.AreSame(targetLocations[0], grassOutwardsCalculation.InputParameters.HydraulicBoundaryLocation);
        }

        private static AssessmentSection CreateAssessmentSection(HydraulicBoundaryLocation[] locations, double targetProbability = 0.1)
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Add(new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability)
            {
                HydraulicBoundaryLocationCalculations =
                {
                    new HydraulicBoundaryLocationCalculation(locations[0])
                }
            });

            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Add(new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability)
            {
                HydraulicBoundaryLocationCalculations =
                {
                    new HydraulicBoundaryLocationCalculation(locations[0])
                }
            });

            assessmentSection.HydraulicBoundaryDatabase.Locations.AddRange(locations);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(locations);

            return assessmentSection;
        }

        private static AssessmentSectionMergeData.ConstructionProperties CreateDefaultConstructionProperties()
        {
            return new AssessmentSectionMergeData.ConstructionProperties();
        }

        #region HydraulicBoundaryLocationCalculations

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculationFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenTargetAssessmentSectionHasOutput_ThenCalculationsNotChanged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Given
            HydraulicBoundaryLocation[] locations =
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(targetCalculations);
            sourceCalculations.ForEachElementDo(c => c.InputParameters.ShouldIllustrationPointsBeCalculated = true);

            var handler = new AssessmentSectionMergeHandler();

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => !c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));

            // When
            handler.PerformMerge(targetAssessmentSection,
                                 new AssessmentSectionMergeData(sourceAssessmentSection, CreateDefaultConstructionProperties()));

            // Then
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => !c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculationFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenBothAssessmentSectionsHaveOutput_ThenCalculationsNotChanged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Given
            HydraulicBoundaryLocation[] locations =
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(targetCalculations);
            SetOutput(sourceCalculations);
            sourceCalculations.ForEachElementDo(c => c.InputParameters.ShouldIllustrationPointsBeCalculated = true);

            var handler = new AssessmentSectionMergeHandler();

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => !c.Output.HasGeneralResult));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));

            // When
            handler.PerformMerge(targetAssessmentSection,
                                 new AssessmentSectionMergeData(sourceAssessmentSection, CreateDefaultConstructionProperties()));

            // Then
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => !c.Output.HasGeneralResult));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculationFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenSourceAssessmentSectionHasOutput_ThenCalculationDataMerged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Given
            HydraulicBoundaryLocation[] locations =
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(sourceCalculations);
            sourceCalculations.ForEachElementDo(c => c.InputParameters.ShouldIllustrationPointsBeCalculated = true);

            var handler = new AssessmentSectionMergeHandler();

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => !c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));
            Assert.IsTrue(sourceCalculations.All(c => c.InputParameters.ShouldIllustrationPointsBeCalculated));

            // When
            handler.PerformMerge(targetAssessmentSection,
                                 new AssessmentSectionMergeData(sourceAssessmentSection, CreateDefaultConstructionProperties()));

            // Then
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.InputParameters.ShouldIllustrationPointsBeCalculated));
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculationFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenTargetAssessmentSectionHasOutputWithIllustrationPoints_ThenCalculationsNotChanged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Given
            HydraulicBoundaryLocation[] locations =
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(targetCalculations, true);
            SetOutput(sourceCalculations);

            var handler = new AssessmentSectionMergeHandler();

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => !c.Output.HasGeneralResult));

            // When
            handler.PerformMerge(targetAssessmentSection,
                                 new AssessmentSectionMergeData(sourceAssessmentSection, CreateDefaultConstructionProperties()));

            // Then
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => !c.Output.HasGeneralResult));
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculationFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenSourceAssessmentSectionHasOutputWithIllustrationPoints_ThenCalculationsMerged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Given
            HydraulicBoundaryLocation[] locations =
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(targetCalculations);
            SetOutput(sourceCalculations, true);

            var handler = new AssessmentSectionMergeHandler();

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => c.Output.HasGeneralResult));

            // When
            handler.PerformMerge(targetAssessmentSection,
                                 new AssessmentSectionMergeData(sourceAssessmentSection, CreateDefaultConstructionProperties()));

            // Then
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.Output.HasGeneralResult));
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculationFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenBothAssessmentSectionsHaveOutputAndIllustrationPoints_ThenCalculationsNotChanged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Given
            HydraulicBoundaryLocation[] locations =
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(targetCalculations, true);
            SetOutput(sourceCalculations, true);
            sourceCalculations.ForEachElementDo(c => c.InputParameters.ShouldIllustrationPointsBeCalculated = true);

            var handler = new AssessmentSectionMergeHandler();

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => c.Output.HasGeneralResult));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));

            // When
            handler.PerformMerge(targetAssessmentSection,
                                 new AssessmentSectionMergeData(sourceAssessmentSection, CreateDefaultConstructionProperties()));

            // Then
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => c.Output.HasGeneralResult));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculationFuncs))]
        public void PerformMerge_HydraulicBoundaryLocationCalculationsMerged_ObserversNotifiedAndMessageLogged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Twice();
            mocks.ReplayAll();

            HydraulicBoundaryLocation[] locations =
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            targetCalculations.ForEachElementDo(calculation => calculation.Attach(observer));

            SetOutput(targetCalculations);
            SetOutput(sourceCalculations, true);

            var handler = new AssessmentSectionMergeHandler();

            // Call
            void Call() => handler.PerformMerge(targetAssessmentSection,
                                                new AssessmentSectionMergeData(sourceAssessmentSection, CreateDefaultConstructionProperties()));

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>("Hydraulische belastingen zijn samengevoegd.", LogLevelConstant.Info));
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculationFuncs))]
        public void PerformMerge_HydraulicBoundaryLocationCalculationsNotMerged_ObserversNotNotifiedAndMessageLogged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            HydraulicBoundaryLocation[] locations =
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            targetCalculations.ForEachElementDo(calculation => calculation.Attach(observer));

            SetOutput(targetCalculations);
            SetOutput(sourceCalculations);

            var handler = new AssessmentSectionMergeHandler();

            // Call
            void Call() => handler.PerformMerge(targetAssessmentSection,
                                                new AssessmentSectionMergeData(sourceAssessmentSection, CreateDefaultConstructionProperties()));

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>("Hydraulische belastingen zijn niet samengevoegd omdat het huidige traject meer gegevens bevat.", LogLevelConstant.Info));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenEqualCalculationsForUserDefinedTargetProbabilities_WhenMerging_ThenObserversNotNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();
            var handler = new AssessmentSectionMergeHandler();

            var targetLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "location 1", 1, 1),
                new HydraulicBoundaryLocation(2, "location 2", 2, 2)
            };

            var sourceLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "location 1", 1, 1),
                new HydraulicBoundaryLocation(2, "location 2", 2, 2)
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(targetLocations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(sourceLocations);

            targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Attach(observer);
            targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Attach(observer);

            // When
            handler.PerformMerge(targetAssessmentSection, new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties
                                     {
                                         MergePiping = false,
                                         MergeGrassCoverErosionInwards = false,
                                         MergeMacroStabilityInwards = false,
                                         MergeStabilityStoneCover = false,
                                         MergeWaveImpactAsphaltCover = false,
                                         MergeGrassCoverErosionOutwards = false,
                                         MergeHeightStructures = false,
                                         MergeClosingStructures = false,
                                         MergeStabilityPointStructures = false
                                     }));

            // Then
            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> waterLevelTargetProbabilities =
                targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities;
            Assert.AreEqual(1, waterLevelTargetProbabilities.Count);
            Assert.AreEqual(0.1, waterLevelTargetProbabilities.ElementAt(0).TargetProbability);

            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> waveHeightTargetProbabilities =
                targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities;
            Assert.AreEqual(1, waveHeightTargetProbabilities.Count);
            Assert.AreEqual(0.1, waveHeightTargetProbabilities.ElementAt(0).TargetProbability);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenDifferentCalculationsForUserDefinedTargetProbabilities_WhenMerging_ThenCalculationsMergedAndObserversNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Twice();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler();

            var targetLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "location 1", 1, 1),
                new HydraulicBoundaryLocation(2, "location 2", 2, 2)
            };

            var sourceLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "location 1", 1, 1),
                new HydraulicBoundaryLocation(2, "location 2", 2, 2)
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(targetLocations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(sourceLocations, 0.01);

            SetOutput(sourceAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.ElementAt(0).HydraulicBoundaryLocationCalculations, true);
            SetOutput(sourceAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.ElementAt(0).HydraulicBoundaryLocationCalculations, true);

            targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Attach(observer);
            targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Attach(observer);

            // When
            handler.PerformMerge(targetAssessmentSection, new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties
                                     {
                                         MergePiping = false,
                                         MergeGrassCoverErosionInwards = false,
                                         MergeMacroStabilityInwards = false,
                                         MergeStabilityStoneCover = false,
                                         MergeWaveImpactAsphaltCover = false,
                                         MergeGrassCoverErosionOutwards = false,
                                         MergeHeightStructures = false,
                                         MergeClosingStructures = false,
                                         MergeStabilityPointStructures = false
                                     }));

            // Then
            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> waterLevelTargetProbabilities =
                targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities;
            Assert.AreEqual(2, waterLevelTargetProbabilities.Count);
            Assert.AreEqual(0.1, waterLevelTargetProbabilities.ElementAt(0).TargetProbability);
            Assert.AreEqual(0.01, waterLevelTargetProbabilities.ElementAt(1).TargetProbability);
            Assert.IsTrue(waterLevelTargetProbabilities.ElementAt(1).HydraulicBoundaryLocationCalculations.All(c => c.HasOutput));
            Assert.IsTrue(waterLevelTargetProbabilities.ElementAt(1).HydraulicBoundaryLocationCalculations.All(c => c.Output.HasGeneralResult));

            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> waveHeightTargetProbabilities =
                targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities;
            Assert.AreEqual(2, waveHeightTargetProbabilities.Count);
            Assert.AreEqual(0.1, waveHeightTargetProbabilities.ElementAt(0).TargetProbability);
            Assert.AreEqual(0.01, waveHeightTargetProbabilities.ElementAt(1).TargetProbability);
            Assert.IsTrue(waveHeightTargetProbabilities.ElementAt(1).HydraulicBoundaryLocationCalculations.All(c => c.HasOutput));
            Assert.IsTrue(waveHeightTargetProbabilities.ElementAt(1).HydraulicBoundaryLocationCalculations.All(c => c.Output.HasGeneralResult));

            mocks.VerifyAll();
        }

        private static void SetOutput(IEnumerable<HydraulicBoundaryLocationCalculation> calculations, bool illustrationPoints = false)
        {
            foreach (HydraulicBoundaryLocationCalculation calculation in calculations)
            {
                calculation.Output = illustrationPoints
                                         ? new TestHydraulicBoundaryLocationCalculationOutput(new TestGeneralResultSubMechanismIllustrationPoint())
                                         : new TestHydraulicBoundaryLocationCalculationOutput();
            }
        }

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryLocationCalculationFuncs()
        {
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaterLevelCalculationsForSignalingNorm));
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaterLevelCalculationsForLowerLimitNorm));
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                .SelectMany(c => c.HydraulicBoundaryLocationCalculations)));
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                                .SelectMany(c => c.HydraulicBoundaryLocationCalculations)));
        }

        #endregion
    }
}