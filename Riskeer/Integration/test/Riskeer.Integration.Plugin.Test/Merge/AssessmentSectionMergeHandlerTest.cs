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
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
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
                                                                                     new AssessmentSectionMergeData.ConstructionProperties()));

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

            // Call
            handler.PerformMerge(targetAssessmentSection, new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties
                                     {
                                         MergePiping = true,
                                         MergeGrassCoverErosionInwards = true,
                                         MergeMacroStabilityInwards = true,
                                         MergeMacroStabilityOutwards = true,
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
                                         MergeStrengthStabilityLengthwiseConstruction = true,
                                         MergeDuneErosion = true,
                                         MergeTechnicalInnovation = true
                                     }));

            // Assert
            Assert.AreSame(sourceAssessmentSection.Piping, targetAssessmentSection.Piping);
            Assert.AreSame(sourceAssessmentSection.GrassCoverErosionInwards, targetAssessmentSection.GrassCoverErosionInwards);
            Assert.AreSame(sourceAssessmentSection.MacroStabilityInwards, targetAssessmentSection.MacroStabilityInwards);
            Assert.AreSame(sourceAssessmentSection.MacroStabilityOutwards, targetAssessmentSection.MacroStabilityOutwards);
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
            Assert.AreSame(sourceAssessmentSection.StrengthStabilityLengthwiseConstruction, targetAssessmentSection.StrengthStabilityLengthwiseConstruction);
            Assert.AreSame(sourceAssessmentSection.DuneErosion, targetAssessmentSection.DuneErosion);
            Assert.AreSame(sourceAssessmentSection.TechnicalInnovation, targetAssessmentSection.TechnicalInnovation);
        }

        [Test]
        public void PerformMerge_WithNoFailureMechanismsToMerge_FailureMechanismsSame()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            handler.PerformMerge(targetAssessmentSection,
                                 new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties()));

            // Assert
            Assert.AreNotSame(sourceAssessmentSection.Piping, targetAssessmentSection.Piping);
            Assert.AreNotSame(sourceAssessmentSection.GrassCoverErosionInwards, targetAssessmentSection.GrassCoverErosionInwards);
            Assert.AreNotSame(sourceAssessmentSection.MacroStabilityInwards, targetAssessmentSection.MacroStabilityInwards);
            Assert.AreNotSame(sourceAssessmentSection.MacroStabilityOutwards, targetAssessmentSection.MacroStabilityOutwards);
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
            Assert.AreNotSame(sourceAssessmentSection.StrengthStabilityLengthwiseConstruction, targetAssessmentSection.StrengthStabilityLengthwiseConstruction);
            Assert.AreNotSame(sourceAssessmentSection.DuneErosion, targetAssessmentSection.DuneErosion);
            Assert.AreNotSame(sourceAssessmentSection.TechnicalInnovation, targetAssessmentSection.TechnicalInnovation);
        }

        [Test]
        public void PerformMerge_WithAllFailureMechanismsToMerge_LogMessages()
        {
            // Setup
            var handler = new AssessmentSectionMergeHandler();
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            Action call = () => handler.PerformMerge(targetAssessmentSection, new AssessmentSectionMergeData(
                                                         sourceAssessmentSection,
                                                         new AssessmentSectionMergeData.ConstructionProperties
                                                         {
                                                             MergePiping = true,
                                                             MergeGrassCoverErosionInwards = true,
                                                             MergeMacroStabilityInwards = true,
                                                             MergeMacroStabilityOutwards = true,
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
                                                             MergeStrengthStabilityLengthwiseConstruction = true,
                                                             MergeDuneErosion = true,
                                                             MergeTechnicalInnovation = true
                                                         }));

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(19, msgs.Length);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Piping' zijn vervangen.", msgs[1]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Grasbekleding erosie kruin en binnentalud' zijn vervangen.", msgs[2]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Macrostabiliteit binnenwaarts' zijn vervangen.", msgs[3]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Macrostabiliteit buitenwaarts' zijn vervangen.", msgs[4]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Microstabiliteit' zijn vervangen.", msgs[5]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Stabiliteit steenzetting' zijn vervangen.", msgs[6]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Golfklappen op asfaltbekleding' zijn vervangen.", msgs[7]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Wateroverdruk bij asfaltbekleding' zijn vervangen.", msgs[8]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Grasbekleding erosie buitentalud' zijn vervangen.", msgs[9]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Grasbekleding afschuiven buitentalud' zijn vervangen.", msgs[10]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Grasbekleding afschuiven binnentalud' zijn vervangen.", msgs[11]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Hoogte kunstwerk' zijn vervangen.", msgs[12]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Betrouwbaarheid sluiting kunstwerk' zijn vervangen.", msgs[13]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Piping bij kunstwerk' zijn vervangen.", msgs[14]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Sterkte en stabiliteit puntconstructies' zijn vervangen.", msgs[15]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Sterkte en stabiliteit langsconstructies' zijn vervangen.", msgs[16]);
                Assert.AreEqual("Gegevens van toetsspoor 'Duinwaterkering - Duinafslag' zijn vervangen.", msgs[17]);
                Assert.AreEqual("Gegevens van toetsspoor 'Technische innovaties - Technische innovaties' zijn vervangen.", msgs[18]);
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
            sourceAssessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[0]
                }
            });
            sourceAssessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = sourceLocations[1]
                }
            });
            sourceAssessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation
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

        [Test]
        public void GivenEqualUserDefinedTargetProbabilities_WhenMerging_ThenCalculationsMerged()
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
            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability tp in sourceAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                AssertHydraulicBoundaryCalculations(tp.HydraulicBoundaryLocationCalculations, targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                                                                     .First(c => c.TargetProbability.Equals(tp.TargetProbability))
                                                                                                                     .HydraulicBoundaryLocationCalculations, targetLocations);
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability tp in sourceAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                AssertHydraulicBoundaryCalculations(tp.HydraulicBoundaryLocationCalculations, targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                                                                                     .First(c => c.TargetProbability.Equals(tp.TargetProbability))
                                                                                                                     .HydraulicBoundaryLocationCalculations, targetLocations);
            }
        }

        [Test]
        public void GivenDifferentUserDefinedTargetProbabilities_WhenMerging_ThenCalculationsMerged()
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

            sourceAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[0].TargetProbability = 0.01;
            sourceAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities[0].TargetProbability = 0.01;

            // Precondition
            Assert.AreEqual(0, targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Count(c => c.TargetProbability.Equals(0.01)));
            Assert.AreEqual(0, targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Count(c => c.TargetProbability.Equals(0.01)));

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
            Assert.AreEqual(1, targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Count(c => c.TargetProbability.Equals(0.01)));
            Assert.AreEqual(1, targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Count(c => c.TargetProbability.Equals(0.01)));
        }

        private static void AssertHydraulicBoundaryCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations,
                                                                IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations,
                                                                IEnumerable<HydraulicBoundaryLocation> targetLocations)
        {
            Assert.AreEqual(sourceCalculations.Count(), targetLocations.Count());
            Assert.AreEqual(targetCalculations.Count(), sourceCalculations.Count());

            for (var i = 0; i < sourceCalculations.Count(); i++)
            {
                HydraulicBoundaryLocationCalculation sourceCalculation = sourceCalculations.ElementAt(i);
                HydraulicBoundaryLocationCalculation targetCalculation = targetCalculations.ElementAt(i);
                Assert.AreEqual(sourceCalculation.InputParameters.ShouldIllustrationPointsBeCalculated,
                                targetCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
                Assert.AreSame(sourceCalculation.Output, targetCalculation.Output);
                Assert.AreSame(targetLocations.ElementAt(i), targetCalculation.HydraulicBoundaryLocation);
            }
        }

        private static AssessmentSection CreateAssessmentSection(HydraulicBoundaryLocation[] locations)
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Add(new HydraulicBoundaryLocationCalculationsForTargetProbability
            {
                HydraulicBoundaryLocationCalculations =
                {
                    new HydraulicBoundaryLocationCalculation(locations[0])
                }
            });

            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Add(new HydraulicBoundaryLocationCalculationsForTargetProbability
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
                                 new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties()));

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
                                 new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties()));

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
                                 new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties()));

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
                                 new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties()));

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
                                 new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties()));

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
                                 new AssessmentSectionMergeData(
                                     sourceAssessmentSection,
                                     new AssessmentSectionMergeData.ConstructionProperties()));

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
            void Call() => handler.PerformMerge(targetAssessmentSection, new AssessmentSectionMergeData(
                                                    sourceAssessmentSection,
                                                    new AssessmentSectionMergeData.ConstructionProperties()));

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
            void Call() => handler.PerformMerge(targetAssessmentSection, new AssessmentSectionMergeData(
                                                    sourceAssessmentSection,
                                                    new AssessmentSectionMergeData.ConstructionProperties()));

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>("Hydraulische belastingen zijn niet samengevoegd omdat het huidige traject meer gegevens bevat.", LogLevelConstant.Info));
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