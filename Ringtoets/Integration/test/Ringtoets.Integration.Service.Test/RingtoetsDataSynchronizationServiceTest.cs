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
using Ringtoets.Piping.Data;
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
            var assessmentSection = GetFullyConfiguredAssessmentSection();
            var expectedAffectedItems = new List<ICalculation>();
            expectedAffectedItems.AddRange(assessmentSection.ClosingStructures.Calculations
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionInwards.Calculations
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.HeightStructures.Calculations
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.PipingFailureMechanism.Calculations
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.StabilityPointStructures.Calculations
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                            .Where(c => c.HasOutput));

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            Assert.IsFalse(assessmentSection.ClosingStructures.Calculations.Any(c => c.HasOutput));
            Assert.IsFalse(assessmentSection.GrassCoverErosionInwards.Calculations.Any(c => c.HasOutput));
            Assert.IsFalse(assessmentSection.GrassCoverErosionOutwards.Calculations.Any(c => c.HasOutput));
            Assert.IsFalse(assessmentSection.HeightStructures.Calculations.Any(c => c.HasOutput));
            Assert.IsFalse(assessmentSection.PipingFailureMechanism.Calculations.Any(c => c.HasOutput));
            Assert.IsFalse(assessmentSection.StabilityPointStructures.Calculations.Any(c => c.HasOutput));
            Assert.IsFalse(assessmentSection.StabilityStoneCover.Calculations.Any(c => c.HasOutput));
            Assert.IsFalse(assessmentSection.WaveImpactAsphaltCover.Calculations.Any(c => c.HasOutput));

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
            var assessmentSection = GetFullyConfiguredAssessmentSection();
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

        private static AssessmentSection GetFullyConfiguredAssessmentSection()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            SetFullyConfiguredStructuresFailureMechanism<ClosingStructuresInput, ClosingStructure>(
                assessmentSection.ClosingStructures, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.GrassCoverErosionInwards, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.GrassCoverErosionOutwards, hydraulicBoundaryLocation);
            SetFullyConfiguredStructuresFailureMechanism<HeightStructuresInput, HeightStructure>(
                assessmentSection.HeightStructures, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.PipingFailureMechanism, hydraulicBoundaryLocation);
            SetFullyConfiguredStructuresFailureMechanism<StabilityPointStructuresInput, StabilityPointStructure>(
                assessmentSection.StabilityPointStructures, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.StabilityStoneCover, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.WaveImpactAsphaltCover, hydraulicBoundaryLocation);

            return assessmentSection;
        }

        private static void SetFullyConfiguredFailureMechanism(PipingFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var calculation = new PipingCalculation(new GeneralPipingInput());
            var calculationWithOutput = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestPipingOutput()
            };
            var calculationWithHydraulicBoundaryLocation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new PipingCalculation(new GeneralPipingInput());
            var subCalculationWithOutput = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestPipingOutput()
            };
            var subCalculationWithHydraulicBoundaryLocation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(subCalculation);
            calculationGroup.Children.Add(subCalculationWithOutput);
            calculationGroup.Children.Add(subCalculationWithOutputAndHydraulicBoundaryLocation);
            calculationGroup.Children.Add(subCalculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);
        }

        private static void SetFullyConfiguredFailureMechanism(GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            var calculationWithHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new GrassCoverErosionInwardsCalculation();
            var subCalculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            var subCalculationWithHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(subCalculation);
            calculationGroup.Children.Add(subCalculationWithOutput);
            calculationGroup.Children.Add(subCalculationWithOutputAndHydraulicBoundaryLocation);
            calculationGroup.Children.Add(subCalculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);
        }

        private static void SetFullyConfiguredStructuresFailureMechanism<TCalculationInput, TStructureBase>(
            ICalculatableFailureMechanism failureMechanism,
            HydraulicBoundaryLocation hydraulicBoundaryLocation)
            where TStructureBase : StructureBase
            where TCalculationInput : StructuresInputBase<TStructureBase>, new()
        {
            var calculation = new StructuresCalculation<TCalculationInput>();
            var calculationWithOutput = new StructuresCalculation<TCalculationInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var calculationWithHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new StructuresCalculation<TCalculationInput>();
            var subCalculationWithOutput = new StructuresCalculation<TCalculationInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var subCalculationWithHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(subCalculation);
            calculationGroup.Children.Add(subCalculationWithOutput);
            calculationGroup.Children.Add(subCalculationWithOutputAndHydraulicBoundaryLocation);
            calculationGroup.Children.Add(subCalculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);
        }

        private static void SetFullyConfiguredFailureMechanism(StabilityStoneCoverFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var calculationWithOutput = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            var subCalculationWithOutput = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutput);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(subCalculation);
            calculationGroup.Children.Add(subCalculationWithOutput);
            calculationGroup.Children.Add(subCalculationWithOutputAndHydraulicBoundaryLocation);
            calculationGroup.Children.Add(subCalculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationGroup);
        }

        private static void SetFullyConfiguredFailureMechanism(WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationWithOutput = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var subCalculationWithOutput = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutput);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(subCalculation);
            calculationGroup.Children.Add(subCalculationWithOutput);
            calculationGroup.Children.Add(subCalculationWithOutputAndHydraulicBoundaryLocation);
            calculationGroup.Children.Add(subCalculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationGroup);
        }

        private static void SetFullyConfiguredFailureMechanism(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var calculationWithOutput = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var subCalculationWithOutput = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutput);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(subCalculation);
            calculationGroup.Children.Add(subCalculationWithOutput);
            calculationGroup.Children.Add(subCalculationWithOutputAndHydraulicBoundaryLocation);
            calculationGroup.Children.Add(subCalculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationGroup);
        }
    }
}