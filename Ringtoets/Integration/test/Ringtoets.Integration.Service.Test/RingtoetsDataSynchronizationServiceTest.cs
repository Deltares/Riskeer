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
using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.TestUtils;
using Ringtoets.Piping.Data;
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
            IEnumerable<IObservable> affectedItems = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms()
                .SelectMany(f => f.Calculations)
                .Where(c => c.HasOutput));

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
            IEnumerable<IObservable> affectedItems = RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            Assert.IsTrue(assessmentSection.ClosingStructures.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>()
                                            .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.GrassCoverErosionInwards.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                            .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.GrassCoverErosionOutwards.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                            .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.HeightStructures.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>()
                                            .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.PipingFailureMechanism.Calculations.Cast<PipingCalculation>()
                                            .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.StabilityPointStructures.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                            .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.StabilityStoneCover.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                            .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.WaveImpactAsphaltCover.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                            .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));

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
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase,
                                                                                                                                assessmentSection.GrassCoverErosionOutwards);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            CollectionAssert.AreEqual(new[]
            {
                hydraulicBoundaryLocation
            }, affectedObjects);
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
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase, assessmentSection.GrassCoverErosionOutwards);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
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
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase, assessmentSection.GrassCoverErosionOutwards);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
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
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            CollectionAssert.AreEqual(new[]
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            }, affectedObjects);
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
            IEnumerable<IObservable> affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(affected);
        }

        [Test]
        [TestCase(3.5, double.NaN)]
        [TestCase(double.NaN, 8.3)]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithDataAndGrassCoverErosionOutwardsLocationWithData_ClearDataAndReturnTrue(
            double designWaterLevel, double waveHeight)
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 0, 0)
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

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new HydraulicBoundaryLocation(2, "test2", 0, 0)
            {
                DesignWaterLevel = (RoundedDouble) designWaterLevel,
                WaveHeight = (RoundedDouble) waveHeight
            };;

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    grassCoverErosionOutwardsHydraulicBoundaryLocation
                }
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            CollectionAssert.AreEquivalent(new[]
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation,
                hydraulicBoundaryLocation
            }, affectedObjects);
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
            RingtoetsDataSynchronizationService.ClearReferenceLine(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            PipingFailureMechanism pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
            CollectionAssert.IsEmpty(pipingFailureMechanism.Sections);
            CollectionAssert.IsEmpty(pipingFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(pipingFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(pipingFailureMechanism.SurfaceLines);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.DikeProfiles);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.ForeshoreProfiles);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.ClosingStructures);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.HeightStructures);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.StabilityPointStructures);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.Sections);
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.SectionResults);

            MacrostabilityInwardsFailureMechanism macrostabilityInwardsFailureMechanism = assessmentSection.MacrostabilityInwards;
            CollectionAssert.IsEmpty(macrostabilityInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macrostabilityInwardsFailureMechanism.SectionResults);

            MacrostabilityOutwardsFailureMechanism macrostabilityOutwardsFailureMechanism = assessmentSection.MacrostabilityOutwards;
            CollectionAssert.IsEmpty(macrostabilityOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macrostabilityOutwardsFailureMechanism.SectionResults);

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            CollectionAssert.IsEmpty(microstabilityFailureMechanism.Sections);
            CollectionAssert.IsEmpty(microstabilityFailureMechanism.SectionResults);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            CollectionAssert.IsEmpty(waterPressureAsphaltCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(waterPressureAsphaltCoverFailureMechanism.SectionResults);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            CollectionAssert.IsEmpty(grassCoverSlipOffOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverSlipOffOutwardsFailureMechanism.SectionResults);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            CollectionAssert.IsEmpty(grassCoverSlipOffInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverSlipOffInwardsFailureMechanism.SectionResults);

            StrengthStabilityLengthwiseConstructionFailureMechanism stabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            CollectionAssert.IsEmpty(stabilityLengthwiseConstructionFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityLengthwiseConstructionFailureMechanism.SectionResults);

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            CollectionAssert.IsEmpty(pipingStructureFailureMechanism.Sections);
            CollectionAssert.IsEmpty(pipingStructureFailureMechanism.SectionResults);

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            CollectionAssert.IsEmpty(technicalInnovationFailureMechanism.Sections);
            CollectionAssert.IsEmpty(technicalInnovationFailureMechanism.SectionResults);

            Assert.IsNull(assessmentSection.ReferenceLine);
        }

        [Test]
        public void ClearReferenceLine_FullyConfiguredAssessmentSection_AllAffectedObservableObjectsReturned()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();

            // Call
            var observables = RingtoetsDataSynchronizationService.ClearReferenceLine(assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(39, observables.Length);

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
            CollectionAssert.Contains(observables, pipingFailureMechanism);
            CollectionAssert.Contains(observables, pipingFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(observables, pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(observables, pipingFailureMechanism.SurfaceLines);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism);
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism.DikeProfiles);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism);
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism);
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism);
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism.ForeshoreProfiles);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.ClosingStructures);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.HeightStructures);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.StabilityPointStructures);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.Sections);
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, duneErosionFailureMechanism);

            MacrostabilityInwardsFailureMechanism macrostabilityInwardsFailureMechanism = assessmentSection.MacrostabilityInwards;
            CollectionAssert.Contains(observables, macrostabilityInwardsFailureMechanism);

            MacrostabilityOutwardsFailureMechanism macrostabilityOutwardsFailureMechanism = assessmentSection.MacrostabilityOutwards;
            CollectionAssert.Contains(observables, macrostabilityOutwardsFailureMechanism);

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            CollectionAssert.Contains(observables, microstabilityFailureMechanism);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            CollectionAssert.Contains(observables, waterPressureAsphaltCoverFailureMechanism);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            CollectionAssert.Contains(observables, grassCoverSlipOffOutwardsFailureMechanism);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            CollectionAssert.Contains(observables, grassCoverSlipOffInwardsFailureMechanism);

            StrengthStabilityLengthwiseConstructionFailureMechanism stabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            CollectionAssert.Contains(observables, stabilityLengthwiseConstructionFailureMechanism);

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            CollectionAssert.Contains(observables, pipingStructureFailureMechanism);

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            CollectionAssert.Contains(observables, technicalInnovationFailureMechanism);

            Assert.IsNull(assessmentSection.ReferenceLine);
            CollectionAssert.Contains(observables, assessmentSection);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityStoneCoverFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityStoneCoverFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = new StabilityStoneCoverFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredStabilityStoneCoverFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredStabilityStoneCoverFailureMechanism();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            StabilityStoneCoverWaveConditionsCalculation[] calculations = failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                                                          .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                                                                          .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + calculations.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_WaveImpactAsphaltCoverFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_WaveImpactAsphaltCoverFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredWaveImpactAsphaltCoverFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredWaveImpactAsphaltCoverFailureMechanism();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            WaveImpactAsphaltCoverWaveConditionsCalculation[] calculations = failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                                                             .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                                                                             .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + calculations.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_GrassCoverErosionOutwardsFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_GrassCoverErosionOutwardsFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredGrassCoverErosionOutwardsFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredGrassCoverErosionOutwardsFailureMechanism();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculations = failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                                                .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                                                                                .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + calculations.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_HeightStructuresFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_HeightStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = new HeightStructuresFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredHeightStructuresFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredHeightStructuresFailureMechanism();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            StructuresCalculation<HeightStructuresInput>[] calculations = failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>()
                                                                                          .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                                                                          .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + calculations.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_ClosingStructuresFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            ClosingStructuresFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_ClosingStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            ClosingStructuresFailureMechanism failureMechanism = new ClosingStructuresFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredClosingStructuresFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            ClosingStructuresFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredClosingStructuresFailureMechanism();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            StructuresCalculation<ClosingStructuresInput>[] calculations = failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>()
                                                                                           .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                                                                           .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + calculations.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityPointStructuresFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityPointStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = new StabilityPointStructuresFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredStabilityPointStructuresFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredStabilityPointStructuresFailureMechanism();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            StructuresCalculation<StabilityPointStructuresInput>[] calculations = failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                                                                                  .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                                                                                  .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + calculations.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
        }

        [Test]
        public void RemoveDikeProfile_GrassCoverErosionInwardsFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = null;
            DikeProfile profile = new TestDikeProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveDikeProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveDikeProfile_GrassCoverErosionInwardsFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveDikeProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveDikeProfile_FullyConfiguredGrassCoverErosionInwardsFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredGrassCoverErosionInwardsFailureMechanism();
            DikeProfile profile = failureMechanism.DikeProfiles[0];
            GrassCoverErosionInwardsCalculation[] calculations = failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                                                                 .Where(c => ReferenceEquals(c.InputParameters.DikeProfile, profile))
                                                                                 .ToArray();
            int originalNumberOfSectionResultAssignments = failureMechanism.SectionResults.Count(sr => sr.Calculation != null);
            GrassCoverErosionInwardsFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults
                                                                                                     .Where(sr => calculations.Contains(sr.Calculation))
                                                                                                     .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveDikeProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.DikeProfiles, profile);
            foreach (GrassCoverErosionInwardsCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.DikeProfile);
            }
            foreach (GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult in sectionResults)
            {
                Assert.IsNull(sectionResult.Calculation);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + calculations.Length + sectionResults.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.DikeProfiles);
            foreach (GrassCoverErosionInwardsCalculation calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
            foreach (GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult in sectionResults)
            {
                CollectionAssert.Contains(array, sectionResult);
            }
            Assert.AreEqual(originalNumberOfSectionResultAssignments - sectionResults.Length, failureMechanism.SectionResults.Count(sr => sr.Calculation != null),
                            "Other section results with a different calculation/dikeprofile should still have their association.");
        }
    }
}