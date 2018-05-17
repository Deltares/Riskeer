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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.TestUtil.Test
{
    [TestFixture]
    public class TestDataGeneratorTest
    {
        [Test]
        public void GetAssessmentSectionWithAllCalculationConfigurations_DefaultComposition_ReturnsWithAllPossibleCalculationConfigurations()
        {
            // Call
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            // Assert
            Assert.AreEqual(AssessmentSectionComposition.Dike, assessmentSection.Composition);
            AssertFailureMechanismsHaveAllPossibleCalculationConfigurations(assessmentSection);
            AssertHydraulicBoundaryOutput(assessmentSection, true);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();
            AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithOutputs(duneErosionFailureMechanism);
            AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithoutOutputs(duneErosionFailureMechanism);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune)]
        public void GetAssessmentSectionWithAllCalculationConfigurations_CompositionGiven_ReturnWithCompositionAndAllPossibleCalculationConfigurations(
            AssessmentSectionComposition composition)
        {
            // Call 
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations(composition);

            // Assert
            Assert.AreEqual(composition, assessmentSection.Composition);
            AssertFailureMechanismsHaveAllPossibleCalculationConfigurations(assessmentSection);
            AssertHydraulicBoundaryOutput(assessmentSection, true);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();
            AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithOutputs(duneErosionFailureMechanism);
            AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithoutOutputs(duneErosionFailureMechanism);
        }

        [Test]
        public void GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput_DefaultComposition_ReturnWithOnlyCalculationOutputsAndComposition()
        {
            // Call 
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput();

            // Assert
            Assert.AreEqual(AssessmentSectionComposition.Dike, assessmentSection.Composition);
            AssertFailureMechanismsHaveAllPossibleCalculationConfigurations(assessmentSection);
            AssertHydraulicBoundaryOutput(assessmentSection, false);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();
            AssertDuneErosionFailureAssertDuneErosionFailureMechanismCalculationConfigurations(duneErosionFailureMechanism, false);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput_Composition_CalculationsAndComposition(Dike)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput_Composition_CalculationsAndComposition(DikeAndDune)")]
        [TestCase(AssessmentSectionComposition.Dune, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput_Composition_CalculationsAndComposition(Dune)")]
        public void GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput_CompositionGiven_ReturnWithOnlyCalculationOutputsAndComposition(
            AssessmentSectionComposition composition)
        {
            // Call 
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(composition);

            // Assert
            Assert.AreEqual(composition, assessmentSection.Composition);
            AssertFailureMechanismsHaveAllPossibleCalculationConfigurations(assessmentSection);
            AssertHydraulicBoundaryOutput(assessmentSection, false);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();
            AssertDuneErosionFailureAssertDuneErosionFailureMechanismCalculationConfigurations(duneErosionFailureMechanism, false);
        }

        [Test]
        public void GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput_DefaultComposition_ReturnWithOnlyHydraulicBoundaryLocationAndDuneLocationOutputsAndComposition()
        {
            // Call 
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput();

            // Assert
            Assert.AreEqual(AssessmentSectionComposition.Dike, assessmentSection.Composition);
            AssertFailureMechanismsHaveAllCalculationConfigurationsWithoutCalculationOutputs(assessmentSection);
            Assert.False(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(calc => calc.HasOutput));
            AssertHydraulicBoundaryOutput(assessmentSection, true);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();
            AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithOutputs(duneErosionFailureMechanism);
            AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithoutOutputs(duneErosionFailureMechanism);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput_WithComposition_CompositionSetAndOnlyHydraulicOutputs(Dike)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput_CompositionGiven_CompositionSetAndOnlyHydraulicOutputs(DikeAndDune)")]
        [TestCase(AssessmentSectionComposition.Dune, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput_CompositionGiven_CompositionSetAndOnlyHydraulicOutputs(Dune)")]
        public void GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput_CompositionGiven_CompositionSetAndReturnWithOnlyHydraulicBoundaryLocationAndDuneLocationOutputs(
            AssessmentSectionComposition composition)
        {
            // Call 
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(composition);

            // Assert
            Assert.AreEqual(composition, assessmentSection.Composition);
            AssertFailureMechanismsHaveAllCalculationConfigurationsWithoutCalculationOutputs(assessmentSection);
            Assert.False(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(calc => calc.HasOutput));
            AssertHydraulicBoundaryOutput(assessmentSection, true);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();
            AssertNumberOfDuneErosionFailureMechanismCalculations(duneErosionFailureMechanism);
            AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithoutOutputs(duneErosionFailureMechanism);
            AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithOutputs(duneErosionFailureMechanism);
        }

        private static void AssertHydraulicBoundaryOutput(AssessmentSection assessmentSection, bool hasOutput)
        {
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForSignalingNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForSignalingNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForLowerLimitNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.All(c => c.HasOutput == hasOutput));

            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            Assert.IsTrue(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.All(calc => calc.HasOutput == hasOutput));
            Assert.IsTrue(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.All(calc => calc.HasOutput == hasOutput));
            Assert.IsTrue(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.All(calc => calc.HasOutput == hasOutput));
            Assert.IsTrue(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.All(calc => calc.HasOutput == hasOutput));
            Assert.IsTrue(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.All(calc => calc.HasOutput == hasOutput));
            Assert.IsTrue(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.All(calc => calc.HasOutput == hasOutput));
        }

        private static void AssertFailureMechanismsHaveAllPossibleCalculationConfigurations(IAssessmentSection assessmentSection)
        {
            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
                var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
                var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
                var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
                var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
                var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
                var duneErosionFailureMechanism = failureMechanism as DuneErosionFailureMechanism;
                var macroStabilityInwardsFailureMechanism = failureMechanism as MacroStabilityInwardsFailureMechanism;

                if (closingStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(closingStructuresFailureMechanism);
                    AssertClosingStructuresFailureMechanismCalculationConfigurationsWithOutputs(closingStructuresFailureMechanism);
                    AssertClosingStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(closingStructuresFailureMechanism);
                }

                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    AssertHasDikeProfiles(grassCoverErosionInwardsFailureMechanism);
                    AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithOutputs(grassCoverErosionInwardsFailureMechanism);
                    AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithoutOutputs(grassCoverErosionInwardsFailureMechanism);
                }

                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(grassCoverErosionOutwardsFailureMechanism);
                    AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithOutputs(grassCoverErosionOutwardsFailureMechanism);
                    AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithoutOutputs(grassCoverErosionOutwardsFailureMechanism);
                }

                if (heightStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(heightStructuresFailureMechanism);
                    AssertHeightStructuresFailureMechanismCalculationConfigurationsWithOutputs(heightStructuresFailureMechanism);
                    AssertHeightStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(heightStructuresFailureMechanism);
                }

                if (pipingFailureMechanism != null)
                {
                    PipingTestDataGeneratorHelper.AssertHasStochasticSoilModels(pipingFailureMechanism);
                    PipingTestDataGeneratorHelper.AssertHasSurfaceLines(pipingFailureMechanism);
                    PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(pipingFailureMechanism);
                    PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(pipingFailureMechanism);
                }

                if (stabilityPointStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(stabilityPointStructuresFailureMechanism);
                    AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithOutputs(stabilityPointStructuresFailureMechanism);
                    AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(stabilityPointStructuresFailureMechanism);
                }

                if (stabilityStoneCoverFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(stabilityStoneCoverFailureMechanism);
                    AssertStabilityStoneCoverFailureMechanismCalculationConfigurationsWithOutputs(stabilityStoneCoverFailureMechanism);
                    AssertStabilityStoneCoverFailureMechanismCalculationConfigurationsWithoutOutputs(stabilityStoneCoverFailureMechanism);
                }

                if (waveImpactAsphaltCoverFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(waveImpactAsphaltCoverFailureMechanism);
                    AssertWaveImpactAsphaltCoverFailureMechanismCalculationConfigurationsWithOutputs(waveImpactAsphaltCoverFailureMechanism);
                    AssertWaveImpactAsphaltCoverFailureMechanismCalculationConfigurationsWithoutOutputs(waveImpactAsphaltCoverFailureMechanism);
                }

                if (macroStabilityInwardsFailureMechanism != null)
                {
                    MacroStabilityInwardsTestDataGeneratorHelper.AssertHasStochasticSoilModels(macroStabilityInwardsFailureMechanism);
                    MacroStabilityInwardsTestDataGeneratorHelper.AssertHasSurfaceLines(macroStabilityInwardsFailureMechanism);
                    MacroStabilityInwardsTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(macroStabilityInwardsFailureMechanism);
                    MacroStabilityInwardsTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(macroStabilityInwardsFailureMechanism);
                }

                AssertHasFailureMechanismSections(failureMechanism);
            }
        }

        private static void AssertFailureMechanismsHaveAllCalculationConfigurationsWithoutCalculationOutputs(IAssessmentSection assessmentSection)
        {
            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
                var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
                var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
                var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
                var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
                var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
                var duneErosionFailureMechanism = failureMechanism as DuneErosionFailureMechanism;
                var macroStabilityInwardsFailureMechanism = failureMechanism as MacroStabilityInwardsFailureMechanism;

                if (closingStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(closingStructuresFailureMechanism);
                    AssertClosingStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(closingStructuresFailureMechanism);
                }

                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    AssertHasDikeProfiles(grassCoverErosionInwardsFailureMechanism);
                    AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithoutOutputs(grassCoverErosionInwardsFailureMechanism);
                }

                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(grassCoverErosionOutwardsFailureMechanism);
                    AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithoutOutputs(grassCoverErosionOutwardsFailureMechanism);
                }

                if (heightStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(heightStructuresFailureMechanism);
                    AssertHeightStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(heightStructuresFailureMechanism);
                }

                if (pipingFailureMechanism != null)
                {
                    PipingTestDataGeneratorHelper.AssertHasStochasticSoilModels(pipingFailureMechanism);
                    PipingTestDataGeneratorHelper.AssertHasSurfaceLines(pipingFailureMechanism);
                    PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(pipingFailureMechanism);
                }

                if (stabilityPointStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(stabilityPointStructuresFailureMechanism);
                    AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(stabilityPointStructuresFailureMechanism);
                }

                if (stabilityStoneCoverFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(stabilityStoneCoverFailureMechanism);
                    AssertStabilityStoneCoverFailureMechanismCalculationConfigurationsWithoutOutputs(stabilityStoneCoverFailureMechanism);
                }

                if (waveImpactAsphaltCoverFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(waveImpactAsphaltCoverFailureMechanism);
                    AssertWaveImpactAsphaltCoverFailureMechanismCalculationConfigurationsWithoutOutputs(waveImpactAsphaltCoverFailureMechanism);
                }

                if (duneErosionFailureMechanism != null)
                {
                    AssertNumberOfDuneErosionFailureMechanismCalculations(duneErosionFailureMechanism);
                }

                if (macroStabilityInwardsFailureMechanism != null)
                {
                    MacroStabilityInwardsTestDataGeneratorHelper.AssertHasStochasticSoilModels(macroStabilityInwardsFailureMechanism);
                    MacroStabilityInwardsTestDataGeneratorHelper.AssertHasSurfaceLines(macroStabilityInwardsFailureMechanism);
                    MacroStabilityInwardsTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(macroStabilityInwardsFailureMechanism);
                }

                AssertHasFailureMechanismSections(failureMechanism);
            }
        }

        private static void AssertHasFailureMechanismSections(IFailureMechanism failureMechanism)
        {
            IEnumerable<FailureMechanismSection> sections = failureMechanism.Sections;
            Assert.AreEqual(2, sections.Count());

            var failureMechanismHasSectionResults = failureMechanism as IHasSectionResults<FailureMechanismSectionResult>;
            if (failureMechanismHasSectionResults != null)
            {
                Assert.AreEqual(sections.Count(), failureMechanismHasSectionResults.SectionResults.Count());
            }
        }

        #region Grass Cover Erosion Inwards

        [Test]
        public void GetGrassCoverErosionInwardsFailureMechanismWithAllCalculationConfigurations_ReturnsAllPossibleCalculationConfigurations()
        {
            // Call 
            GrassCoverErosionInwardsFailureMechanism failureMechanism = TestDataGenerator.GetGrassCoverErosionInwardsFailureMechanismWithAllCalculationConfigurations();

            // Assert
            AssertHasDikeProfiles(failureMechanism);
            AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithOutputs(failureMechanism);
            AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithoutOutputs(failureMechanism);
            AssertHasFailureMechanismSections(failureMechanism);
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertGrassCoverErosionInwardsCalculationGroupWithOutput(calculationRoot.OfType<GrassCoverErosionInwardsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertGrassCoverErosionInwardsCalculationGroupWithOutput(nestedCalculations.Children.OfType<GrassCoverErosionInwardsCalculation>());
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithoutOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertGrassCoverErosionInwardsCalculationGroupWithoutOutput(calculationRoot.OfType<GrassCoverErosionInwardsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertGrassCoverErosionInwardsCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<GrassCoverErosionInwardsCalculation>());
        }

        private static void AssertGrassCoverErosionInwardsCalculationGroupWithOutput(IEnumerable<GrassCoverErosionInwardsCalculation> children)
        {
            AssertCalculationConfig(children, true, false, true);
            AssertCalculationConfig(children, true, true, true);
        }

        private static void AssertGrassCoverErosionInwardsCalculationGroupWithoutOutput(IEnumerable<GrassCoverErosionInwardsCalculation> children)
        {
            AssertCalculationConfig(children, false, false, false);
            AssertCalculationConfig(children, true, false, false);
            AssertCalculationConfig(children, true, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<GrassCoverErosionInwardsCalculation> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => calc.InputParameters.HydraulicBoundaryLocation != null == hasHydraulicBoundaryLocation
                                                           && calc.InputParameters.DikeProfile != null == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        private static void AssertHasDikeProfiles(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.DikeProfiles);
        }

        #endregion

        #region Stability Stone Cover 

        [Test]
        public void GetStabilityStoneCoverFailureMechanismWithAllCalculationConfigurations_ReturnsAllPossibleCalculationConfigurations()
        {
            // Call 
            StabilityStoneCoverFailureMechanism failureMechanism = TestDataGenerator.GetStabilityStoneCoverFailureMechanismWithAllCalculationConfigurations();

            // Assert
            AssertHasForeshoreProfiles(failureMechanism);
            AssertStabilityStoneCoverFailureMechanismCalculationConfigurationsWithOutputs(failureMechanism);
            AssertStabilityStoneCoverFailureMechanismCalculationConfigurationsWithoutOutputs(failureMechanism);
            AssertHasFailureMechanismSections(failureMechanism);
        }

        private static void AssertStabilityStoneCoverFailureMechanismCalculationConfigurationsWithOutputs(
            StabilityStoneCoverFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertStabilityStoneCoverWaveConditionsCalculationGroupWithOutput(
                calculationRoot.OfType<StabilityStoneCoverWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertStabilityStoneCoverWaveConditionsCalculationGroupWithOutput(
                nestedCalculations.Children.OfType<StabilityStoneCoverWaveConditionsCalculation>());
        }

        private static void AssertStabilityStoneCoverFailureMechanismCalculationConfigurationsWithoutOutputs(StabilityStoneCoverFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertStabilityStoneCoverWaveCalculationGroupWithoutOutput(calculationRoot.OfType<StabilityStoneCoverWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertStabilityStoneCoverWaveCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<StabilityStoneCoverWaveConditionsCalculation>());
        }

        private static void AssertStabilityStoneCoverWaveConditionsCalculationGroupWithOutput(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, true, false, true);
            AssertCalculationConfig(children, true, true, true);
        }

        private static void AssertStabilityStoneCoverWaveCalculationGroupWithoutOutput(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, false, false, false);
            AssertCalculationConfig(children, true, false, false);
            AssertCalculationConfig(children, true, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => calc.InputParameters.HydraulicBoundaryLocation != null == hasHydraulicBoundaryLocation
                                                           && calc.InputParameters.ForeshoreProfile != null == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        private static void AssertHasForeshoreProfiles(StabilityStoneCoverFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region Wave Impact Asphalt Cover

        [Test]
        public void GetWaveImpactAsphaltCoverFailureMechanismWithAllCalculationConfigurations_ReturnsAllPossibleCalculationConfigurations()
        {
            // Call 
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = TestDataGenerator.GetWaveImpactAsphaltCoverFailureMechanismWithAllCalculationConfigurations();

            // Assert
            AssertHasForeshoreProfiles(failureMechanism);
            AssertWaveImpactAsphaltCoverFailureMechanismCalculationConfigurationsWithOutputs(failureMechanism);
            AssertWaveImpactAsphaltCoverFailureMechanismCalculationConfigurationsWithoutOutputs(failureMechanism);
            AssertHasFailureMechanismSections(failureMechanism);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanismCalculationConfigurationsWithOutputs(
            WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertWaveImpactAsphaltCoverCalculationGroupWithOutput(
                calculationRoot.OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertWaveImpactAsphaltCoverCalculationGroupWithOutput(
                nestedCalculations.Children.OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>());
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanismCalculationConfigurationsWithoutOutputs(
            WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertWaveImpactAsphaltCoverCalculationGroupWithoutOutput(calculationRoot.OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertWaveImpactAsphaltCoverCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>());
        }

        private static void AssertWaveImpactAsphaltCoverCalculationGroupWithOutput(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, true, false, true);
            AssertCalculationConfig(children, true, true, true);
        }

        private static void AssertWaveImpactAsphaltCoverCalculationGroupWithoutOutput(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, false, false, false);
            AssertCalculationConfig(children, true, false, false);
            AssertCalculationConfig(children, true, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => calc.InputParameters.HydraulicBoundaryLocation != null == hasHydraulicBoundaryLocation
                                                           && calc.InputParameters.ForeshoreProfile != null == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        private static void AssertHasForeshoreProfiles(WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region Grass Cover Erosion Outwards

        [Test]
        public void GetGrassCoverErosionOutwardsFailureMechanismWithAllCalculationConfigurations_ReturnsAllPossibleCalculationConfigurations()
        {
            // Call 
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = TestDataGenerator.GetGrassCoverErosionOutwardsFailureMechanismWithAllCalculationConfigurations();

            // Assert
            Assert.IsTrue(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.All(calc => calc.HasOutput));
            Assert.IsTrue(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.All(calc => calc.HasOutput));
            Assert.IsTrue(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.All(calc => calc.HasOutput));
            Assert.IsTrue(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.All(calc => calc.HasOutput));
            Assert.IsTrue(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.All(calc => calc.HasOutput));
            Assert.IsTrue(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.All(calc => calc.HasOutput));

            AssertHasForeshoreProfiles(failureMechanism);
            AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithOutputs(failureMechanism);
            AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithoutOutputs(failureMechanism);
            AssertHasFailureMechanismSections(failureMechanism);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithOutputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertGrassCoverErosionCalculationGroupWithOutput(calculationRoot.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertGrassCoverErosionCalculationGroupWithOutput(nestedCalculations.Children.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>());
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithoutOutputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertGrassCoverErosionCalculationGroupWithoutOutput(calculationRoot.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertGrassCoverErosionCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>());
        }

        private static void AssertGrassCoverErosionCalculationGroupWithOutput(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, true, false, true);
            AssertCalculationConfig(children, true, true, true);
        }

        private static void AssertGrassCoverErosionCalculationGroupWithoutOutput(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, false, false, false);
            AssertCalculationConfig(children, true, false, false);
            AssertCalculationConfig(children, true, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => calc.InputParameters.HydraulicBoundaryLocation != null == hasHydraulicBoundaryLocation
                                                           && calc.InputParameters.ForeshoreProfile != null == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        private static void AssertHasForeshoreProfiles(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region Dune Erosion

        private static void AssertNumberOfDuneErosionFailureMechanismCalculations(DuneErosionFailureMechanism failureMechanism)
        {
            int expectedCalculationsCount = failureMechanism.DuneLocations.Count;
            Assert.AreEqual(expectedCalculationsCount, failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.Count());
            Assert.AreEqual(expectedCalculationsCount, failureMechanism.CalculationsForMechanismSpecificSignalingNorm.Count());
            Assert.AreEqual(expectedCalculationsCount, failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.Count());
            Assert.AreEqual(expectedCalculationsCount, failureMechanism.CalculationsForLowerLimitNorm.Count());
            Assert.AreEqual(expectedCalculationsCount, failureMechanism.CalculationsForFactorizedLowerLimitNorm.Count());
        }

        private static void AssertDuneErosionFailureAssertDuneErosionFailureMechanismCalculationConfigurations(DuneErosionFailureMechanism failureMechanism, bool hasOutput)
        {
            if (hasOutput)
            {
                Assert.True(failureMechanism.DuneLocations.All(dl => dl.Calculation.Output != null));

                Assert.True(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.All(calc => calc.Output != null));
                Assert.True(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.All(calc => calc.Output != null));
                Assert.True(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.All(calc => calc.Output != null));
                Assert.True(failureMechanism.CalculationsForLowerLimitNorm.All(calc => calc.Output != null));
                Assert.True(failureMechanism.CalculationsForFactorizedLowerLimitNorm.All(calc => calc.Output != null));
            }
            else
            {
                Assert.True(failureMechanism.DuneLocations.All(dl => dl.Calculation.Output == null));

                Assert.True(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.All(calc => calc.Output == null));
                Assert.True(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.All(calc => calc.Output == null));
                Assert.True(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.All(calc => calc.Output == null));
                Assert.True(failureMechanism.CalculationsForLowerLimitNorm.All(calc => calc.Output == null));
                Assert.True(failureMechanism.CalculationsForFactorizedLowerLimitNorm.All(calc => calc.Output == null));
            }
        }

        private static void AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithOutputs(DuneErosionFailureMechanism failureMechanism)
        {
            Assert.True(failureMechanism.DuneLocations.Any(dl => dl.Calculation.Output != null));

            Assert.True(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.Any(calc => calc.Output != null));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.Any(calc => calc.Output != null));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.Any(calc => calc.Output != null));
            Assert.True(failureMechanism.CalculationsForLowerLimitNorm.Any(calc => calc.Output != null));
            Assert.True(failureMechanism.CalculationsForFactorizedLowerLimitNorm.Any(calc => calc.Output != null));
        }

        private static void AssertDuneErosionFailureMechanismContainsCalculationConfigurationsWithoutOutputs(DuneErosionFailureMechanism failureMechanism)
        {
            Assert.True(failureMechanism.DuneLocations.Any(dl => dl.Calculation.Output == null));

            Assert.True(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.Any(calc => calc.Output == null));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.Any(calc => calc.Output == null));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.Any(calc => calc.Output == null));
            Assert.True(failureMechanism.CalculationsForLowerLimitNorm.Any(calc => calc.Output == null));
            Assert.True(failureMechanism.CalculationsForFactorizedLowerLimitNorm.Any(calc => calc.Output == null));
        }

        #endregion

        #region Structures

        #region ClosingStructures

        [Test]
        public void GetClosingStructuresFailureMechanismWithAllCalculationConfigurations_ReturnsAllPossibleCalculationConfigurations()
        {
            // Call 
            ClosingStructuresFailureMechanism failureMechanism = TestDataGenerator.GetClosingStructuresFailureMechanismWithAllCalculationConfigurations();

            // Assert
            AssertHasForeshoreProfiles(failureMechanism);
            AssertClosingStructuresFailureMechanismCalculationConfigurationsWithOutputs(failureMechanism);
            AssertClosingStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(failureMechanism);
            AssertHasFailureMechanismSections(failureMechanism);
        }

        private static void AssertClosingStructuresFailureMechanismCalculationConfigurationsWithOutputs(ClosingStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithOutputs<ClosingStructure, ClosingStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<ClosingStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithOutputs<ClosingStructure, ClosingStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<ClosingStructuresInput>>());
        }

        private static void AssertClosingStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(ClosingStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<ClosingStructure, ClosingStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<ClosingStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<ClosingStructure, ClosingStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<ClosingStructuresInput>>());
        }

        private static void AssertHasForeshoreProfiles(ClosingStructuresFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region StabilityPointStructures

        [Test]
        public void GetStabilityPointStructuresFailureMechanismWithAllCalculationConfigurations_ReturnsAllPossibleCalculationConfigurations()
        {
            // Call 
            StabilityPointStructuresFailureMechanism failureMechanism = TestDataGenerator.GetStabilityPointStructuresFailureMechanismWithAllCalculationConfigurations();

            // Assert
            AssertHasForeshoreProfiles(failureMechanism);
            AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithOutputs(failureMechanism);
            AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(failureMechanism);
            AssertHasFailureMechanismSections(failureMechanism);
        }

        private static void AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithOutputs(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithOutputs<StabilityPointStructure, StabilityPointStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<StabilityPointStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithOutputs<StabilityPointStructure, StabilityPointStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<StabilityPointStructuresInput>>());
        }

        private static void AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<StabilityPointStructure, StabilityPointStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<StabilityPointStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<StabilityPointStructure, StabilityPointStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<StabilityPointStructuresInput>>());
        }

        private static void AssertHasForeshoreProfiles(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region HeightStructures

        [Test]
        public void GetHeightStructuresFailureMechanismWithAllCalculationConfigurations_ReturnsAllPossibleCalculationConfigurations()
        {
            // Call 
            HeightStructuresFailureMechanism failureMechanism = TestDataGenerator.GetHeightStructuresFailureMechanismWithAlLCalculationConfigurations();

            // Assert
            AssertHasForeshoreProfiles(failureMechanism);
            AssertHeightStructuresFailureMechanismCalculationConfigurationsWithOutputs(failureMechanism);
            AssertHeightStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(failureMechanism);
            AssertHasFailureMechanismSections(failureMechanism);
        }

        private static void AssertHeightStructuresFailureMechanismCalculationConfigurationsWithOutputs(HeightStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithOutputs<HeightStructure, HeightStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<HeightStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithOutputs<HeightStructure, HeightStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<HeightStructuresInput>>());
        }

        private static void AssertHeightStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(HeightStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<HeightStructure, HeightStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<HeightStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<HeightStructure, HeightStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<HeightStructuresInput>>());
        }

        private static void AssertHasForeshoreProfiles(HeightStructuresFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        private static void AssertStructuresCalculationGroupConfigurationWithOutputs<TStructureBase, TCalculationInput>(
            IEnumerable<StructuresCalculation<TCalculationInput>> children)
            where TStructureBase : StructureBase
            where TCalculationInput : StructuresInputBase<TStructureBase>, new()
        {
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, true, false, true);
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, true, true, true);
        }

        private static void AssertStructuresCalculationGroupConfigurationWithoutOutputs<TStructureBase, TCalculationInput>(
            IEnumerable<StructuresCalculation<TCalculationInput>> children)
            where TStructureBase : StructureBase
            where TCalculationInput : StructuresInputBase<TStructureBase>, new()
        {
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, false, false, false);
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, true, false, false);
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, true, true, false);
        }

        private static void AssertCalculationConfig<TStructureBase, TCalculationInput>(
            IEnumerable<StructuresCalculation<TCalculationInput>> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
            where TStructureBase : StructureBase
            where TCalculationInput : StructuresInputBase<TStructureBase>, new()
        {
            Assert.NotNull(children.FirstOrDefault(calc => calc.InputParameters.HydraulicBoundaryLocation != null == hasHydraulicBoundaryLocation
                                                           && calc.InputParameters.ForeshoreProfile != null == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        #endregion
    }
}