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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Service.Test
{
    [TestFixture]
    public class RingtoetsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithoutAssessmentSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithAssessmentSection_ClearsFailureMechanismCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            IEnumerable<ICalculation> expectedAffectedItems = assessmentSection.GetFailureMechanisms()
                                                                               .SelectMany(f => f.Calculations)
                                                                               .Where(c => c.HasOutput)
                                                                               .ToList();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms().SelectMany(f => f.Calculations).Where(c => c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_VariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedCalculations()
        {
            // Setup
            var assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            var expectedAffectedItems = new List<ICalculation>();
            expectedAffectedItems.AddRange(assessmentSection.ClosingStructures.Calculations
                                                            .Cast<StructuresCalculation<ClosingStructuresInput>>()
                                                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionInwards.Calculations
                                                            .Cast<GrassCoverErosionInwardsCalculation>()
                                                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                            .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.HeightStructures.Calculations
                                                            .Cast<StructuresCalculation<HeightStructuresInput>>()
                                                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.PipingFailureMechanism.Calculations
                                                            .Cast<PipingCalculation>()
                                                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.StabilityPointStructures.Calculations
                                                            .Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                            .Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                            .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection);

            // Assert
            Assert.IsFalse(assessmentSection.ClosingStructures.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>()
                                            .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            Assert.IsFalse(assessmentSection.GrassCoverErosionInwards.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                            .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            Assert.IsFalse(assessmentSection.GrassCoverErosionOutwards.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                            .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            Assert.IsFalse(assessmentSection.HeightStructures.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>()
                                            .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            Assert.IsFalse(assessmentSection.PipingFailureMechanism.Calculations.Cast<PipingCalculation>()
                                            .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            Assert.IsFalse(assessmentSection.StabilityPointStructures.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                            .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            Assert.IsFalse(assessmentSection.StabilityStoneCover.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                            .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            Assert.IsFalse(assessmentSection.WaveImpactAsphaltCover.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                            .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(null, failureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        [TestCase(1.0, 3.0)]
        [TestCase(3.8, double.NaN)]
        [TestCase(double.NaN, 6.9)]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithData_ClearsDataAndReturnsTrue(
            double waveHeight, double designWaterLevel)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0)
            {
                WaveHeight = (RoundedDouble) waveHeight,
                DesignWaterLevel = (RoundedDouble) designWaterLevel
            };
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase, assessmentSection.GrassCoverErosionOutwards);

            // Assert
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            Assert.IsTrue(affected);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryDatabaseWithoutLocations_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase, assessmentSection.GrassCoverErosionOutwards);

            // Assert
            Assert.IsFalse(affected);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithoutWaveHeightAndDesignWaterLevel_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase, assessmentSection.GrassCoverErosionOutwards);

            // Assert
            Assert.IsFalse(affected);
        }

        [Test]
        [TestCase(3.5, double.NaN)]
        [TestCase(double.NaN, 8.3)]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithoutDataAndGrassCoverErosionOutwardsLocationWithData_ClearDataAndReturnTrue(
            double designWaterLevel, double waveHeight)
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = hydraulicBoundaryLocation;
            grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel = (RoundedDouble) designWaterLevel;
            grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight = (RoundedDouble) waveHeight;

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    grassCoverErosionOutwardsHydraulicBoundaryLocation
                }
            };

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, failureMechanism);

            // Assert
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            Assert.IsTrue(affected);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithoutDataAndGrassCoverErosionOutwardsLocationWithoutData_ReturnFalse()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    hydraulicBoundaryLocation
                }
            };

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, failureMechanism);

            // Assert
            Assert.IsFalse(affected);
        }

        [Test]
        [TestCase(3.5, double.NaN)]
        [TestCase(double.NaN, 8.3)]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithDataAndGrassCoverErosionOutwardsLocationWithData_ClearDataAndReturnTrue(
            double designWaterLevel, double waveHeight)
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0)
            {
                DesignWaterLevel = (RoundedDouble) designWaterLevel,
                WaveHeight = (RoundedDouble) waveHeight
            };
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = hydraulicBoundaryLocation;

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    grassCoverErosionOutwardsHydraulicBoundaryLocation
                }
            };

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, failureMechanism);

            // Assert
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            Assert.IsTrue(affected);
        }

        [Test]
        public void ClearReferenceLine_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.ClearReferenceLine(null);

            // Assert
            string parmaName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", parmaName);
        }

        [Test]
        public void ClearReferenceLine_FullyConfiguredAssessmentSection_AllReferenceLineDependentDataCleared()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();

            // Call
            var observables = RingtoetsDataSynchronizationService.ClearReferenceLine(assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(39, observables.Length);

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
            CollectionAssert.IsEmpty(pipingFailureMechanism.Sections);
            CollectionAssert.IsEmpty(pipingFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, pipingFailureMechanism);
            CollectionAssert.IsEmpty(pipingFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, pipingFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(observables, pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(pipingFailureMechanism.SurfaceLines);
            CollectionAssert.Contains(observables, pipingFailureMechanism.SurfaceLines);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.DikeProfiles);
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism.DikeProfiles);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism.ForeshoreProfiles);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.ClosingStructures);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.ClosingStructures);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.HeightStructures);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.HeightStructures);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.StabilityPointStructures);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.StabilityPointStructures);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.Sections);
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, duneErosionFailureMechanism);

            MacrostabilityInwardsFailureMechanism macrostabilityInwardsFailureMechanism = assessmentSection.MacrostabilityInwards;
            CollectionAssert.IsEmpty(macrostabilityInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macrostabilityInwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, macrostabilityInwardsFailureMechanism);

            MacrostabilityOutwardsFailureMechanism macrostabilityOutwardsFailureMechanism = assessmentSection.MacrostabilityOutwards;
            CollectionAssert.IsEmpty(macrostabilityOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macrostabilityOutwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, macrostabilityOutwardsFailureMechanism);

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            CollectionAssert.IsEmpty(microstabilityFailureMechanism.Sections);
            CollectionAssert.IsEmpty(microstabilityFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, microstabilityFailureMechanism);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            CollectionAssert.IsEmpty(waterPressureAsphaltCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(waterPressureAsphaltCoverFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, waterPressureAsphaltCoverFailureMechanism);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            CollectionAssert.IsEmpty(grassCoverSlipOffOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverSlipOffOutwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, grassCoverSlipOffOutwardsFailureMechanism);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            CollectionAssert.IsEmpty(grassCoverSlipOffInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverSlipOffInwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, grassCoverSlipOffInwardsFailureMechanism);

            StrengthStabilityLengthwiseConstructionFailureMechanism stabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            CollectionAssert.IsEmpty(stabilityLengthwiseConstructionFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityLengthwiseConstructionFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, stabilityLengthwiseConstructionFailureMechanism);

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            CollectionAssert.IsEmpty(pipingStructureFailureMechanism.Sections);
            CollectionAssert.IsEmpty(pipingStructureFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, pipingStructureFailureMechanism);

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            CollectionAssert.IsEmpty(technicalInnovationFailureMechanism.Sections);
            CollectionAssert.IsEmpty(technicalInnovationFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, technicalInnovationFailureMechanism);

            Assert.IsNull(assessmentSection.ReferenceLine);
            CollectionAssert.Contains(observables, assessmentSection);
        }
    }
}