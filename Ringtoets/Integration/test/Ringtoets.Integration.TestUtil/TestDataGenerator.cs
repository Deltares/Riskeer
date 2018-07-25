// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Service;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.TestUtil
{
    /// <summary>
    /// Class that generates fully configured Ringtoets objects.
    /// </summary>
    public static class TestDataGenerator
    {
        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with a desired <see cref="AssessmentSectionComposition"/> and
        /// with all possible configurations for the parent and nested calculations of the failure mechanisms.
        /// </summary>
        /// <param name="composition">The desired <see cref="AssessmentSectionComposition"/> to initialize the <see cref="AssessmentSection"/> with.</param>
        /// <returns>The configured <see cref="AssessmentSection"/>.</returns>
        public static AssessmentSection GetAssessmentSectionWithAllCalculationConfigurations(
            AssessmentSectionComposition composition = AssessmentSectionComposition.Dike)
        {
            var random = new Random(21);
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var assessmentSection = new AssessmentSection(composition)
            {
                ReferenceLine = new ReferenceLine(),
                HydraulicBoundaryDatabase =
                {
                    Locations =
                    {
                        hydraulicBoundaryLocation
                    }
                }
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            assessmentSection.WaterLevelCalculationsForSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            assessmentSection.WaveHeightCalculationsForSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            assessmentSection.WaveHeightCalculationsForLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            SetFullyConfiguredFailureMechanism(assessmentSection.ClosingStructures, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.GrassCoverErosionInwards, hydraulicBoundaryLocation);
            MacroStabilityInwardsTestDataGenerator.ConfigureFailureMechanismWithAllCalculationConfigurations(assessmentSection.MacroStabilityInwards, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.GrassCoverErosionOutwards, hydraulicBoundaryLocation, random);
            SetFullyConfiguredFailureMechanism(assessmentSection.HeightStructures, hydraulicBoundaryLocation);
            PipingTestDataGenerator.ConfigureFailureMechanismWithAllCalculationConfigurations(assessmentSection.Piping, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.StabilityPointStructures, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.StabilityStoneCover, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.WaveImpactAsphaltCover, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.DuneErosion);
            SetFullyConfiguredFailureMechanism(assessmentSection.GrassCoverSlipOffInwards);
            SetFullyConfiguredFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards);
            SetFullyConfiguredFailureMechanism(assessmentSection.MacroStabilityOutwards);
            SetFullyConfiguredFailureMechanism(assessmentSection.Microstability);
            SetFullyConfiguredFailureMechanism(assessmentSection.PipingStructure);
            SetFullyConfiguredFailureMechanism(assessmentSection.StrengthStabilityLengthwiseConstruction);
            SetFullyConfiguredFailureMechanism(assessmentSection.TechnicalInnovation);
            SetFullyConfiguredFailureMechanism(assessmentSection.WaterPressureAsphaltCover);

            return assessmentSection;
        }

        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with a desired <see cref="AssessmentSectionComposition"/> and
        /// with all possible configurations of the parent and nested calculations, but without design water level output,
        /// wave height output and dune location calculation output.
        /// </summary>
        /// <param name="composition">The desired <see cref="AssessmentSectionComposition"/> to initialize the <see cref="AssessmentSection"/> with.</param>
        /// <returns>The configured <see cref="AssessmentSection"/>.</returns>
        public static AssessmentSection GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(
            AssessmentSectionComposition composition = AssessmentSectionComposition.Dike)
        {
            AssessmentSection assessmentSection = GetAssessmentSectionWithAllCalculationConfigurations(composition);
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection);

            return assessmentSection;
        }

        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with a desired <see cref="AssessmentSectionComposition"/> and
        /// with all possible configurations of the parent and nested calculations, but without any calculation output of the
        /// failure mechanisms.
        /// </summary>
        /// <param name="composition">The desired <see cref="AssessmentSectionComposition"/> to initialize the <see cref="AssessmentSection"/> with.</param>
        /// <returns>The configured <see cref="AssessmentSection"/>.</returns>
        public static AssessmentSection GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(
            AssessmentSectionComposition composition = AssessmentSectionComposition.Dike)
        {
            AssessmentSection assessmentSection = GetAssessmentSectionWithAllCalculationConfigurations(composition);
            RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            return assessmentSection;
        }

        /// <summary>
        /// Gets a fully configured <see cref="StabilityStoneCoverFailureMechanism"/> with all
        /// possible parent and nested calculation configurations.
        /// </summary>
        public static StabilityStoneCoverFailureMechanism GetStabilityStoneCoverFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            SetFullyConfiguredFailureMechanism(failureMechanism, new TestHydraulicBoundaryLocation());

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="WaveImpactAsphaltCoverFailureMechanism"/> with
        /// all possible parent and nested calculation configurations.
        /// </summary>
        public static WaveImpactAsphaltCoverFailureMechanism GetWaveImpactAsphaltCoverFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            SetFullyConfiguredFailureMechanism(failureMechanism, new TestHydraulicBoundaryLocation());

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="GrassCoverErosionOutwardsFailureMechanism"/> with 
        /// all possible parent and nested calculation configurations.
        /// </summary>
        public static GrassCoverErosionOutwardsFailureMechanism GetGrassCoverErosionOutwardsFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            SetFullyConfiguredFailureMechanism(failureMechanism, new TestHydraulicBoundaryLocation(), new Random(21));

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="HeightStructuresFailureMechanism"/> with all possible 
        /// parent and nested calculation configurations.
        /// </summary>
        public static HeightStructuresFailureMechanism GetHeightStructuresFailureMechanismWithAlLCalculationConfigurations()
        {
            var failureMechanism = new HeightStructuresFailureMechanism();
            SetFullyConfiguredFailureMechanism(failureMechanism, new TestHydraulicBoundaryLocation());

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="ClosingStructuresFailureMechanism"/> with all possible
        /// parent and nested calculation configurations.
        /// </summary>
        public static ClosingStructuresFailureMechanism GetClosingStructuresFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new ClosingStructuresFailureMechanism();
            SetFullyConfiguredFailureMechanism(failureMechanism, new TestHydraulicBoundaryLocation());

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="StabilityPointStructuresFailureMechanism"/> with all possible
        /// parent and nested calculation configurations.
        /// </summary>
        public static StabilityPointStructuresFailureMechanism GetStabilityPointStructuresFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            SetFullyConfiguredFailureMechanism(failureMechanism, new TestHydraulicBoundaryLocation());

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="GrassCoverErosionInwardsFailureMechanism"/> with all possible
        /// parent and nested calculation configurations.
        /// </summary>
        public static GrassCoverErosionInwardsFailureMechanism GetGrassCoverErosionInwardsFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            SetFullyConfiguredFailureMechanism(failureMechanism, new TestHydraulicBoundaryLocation());

            return failureMechanism;
        }

        /// <summary>
        /// Gets an assessment section where all failure mechanism have sections set.
        /// </summary>
        /// <param name="composition">The desired <see cref="AssessmentSectionComposition"/>
        /// to initialize the <see cref="AssessmentSection"/> with.</param>
        /// <returns>The configured <see cref="AssessmentSection"/>.</returns>
        public static AssessmentSection GetAssessmenSectionWithAllFailureMechanismSectionsAndResults(
            AssessmentSectionComposition composition = AssessmentSectionComposition.Dike)
        {
            var assessmentSection = new AssessmentSection(composition)
            {
                ReferenceLine = GetReferenceLine()
            };

            IEnumerable<IFailureMechanism> failureMechanisms = assessmentSection.GetFailureMechanisms();
            for (var i = 0; i < failureMechanisms.Count(); i++)
            {
                AddFailureMechanismSections(failureMechanisms.ElementAt(i), i);
            }

            return assessmentSection;
        }

        private static ReferenceLine GetReferenceLine()
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(-1, -1),
                new Point2D(5, 5),
                new Point2D(10, 10),
                new Point2D(-3, 2)
            });
            return referenceLine;
        }

        private static void SetFullyConfiguredFailureMechanism(GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var dikeprofile1 = new DikeProfile(new Point2D(0, 0),
                                               new[]
                                               {
                                                   new RoughnessPoint(new Point2D(0, 0), 0.5),
                                                   new RoughnessPoint(new Point2D(1, 1), 0.6),
                                                   new RoughnessPoint(new Point2D(4, 5), 0.8),
                                                   new RoughnessPoint(new Point2D(10, 1), 1.0)
                                               },
                                               new[]
                                               {
                                                   new Point2D(0, 0),
                                                   new Point2D(1, 1),
                                                   new Point2D(4, 5),
                                                   new Point2D(10, 1)
                                               },
                                               new BreakWater(BreakWaterType.Caisson, 1.3),
                                               new DikeProfile.ConstructionProperties
                                               {
                                                   Id = "aid",
                                                   Name = "A",
                                                   DikeHeight = 5,
                                                   Orientation = 20,
                                                   X0 = 0
                                               });
            var dikeprofile2 = new DikeProfile(new Point2D(10, 10),
                                               new[]
                                               {
                                                   new RoughnessPoint(new Point2D(0, 1), 1.0),
                                                   new RoughnessPoint(new Point2D(1, 2), 0.6),
                                                   new RoughnessPoint(new Point2D(4, 2), 0.5),
                                                   new RoughnessPoint(new Point2D(10, 0), 0.8)
                                               },
                                               new[]
                                               {
                                                   new Point2D(0, 1),
                                                   new Point2D(1, 2),
                                                   new Point2D(4, 2),
                                                   new Point2D(10, 0)
                                               },
                                               new BreakWater(BreakWaterType.Caisson, 1.3),
                                               new DikeProfile.ConstructionProperties
                                               {
                                                   Id = "bid",
                                                   Name = "B",
                                                   DikeHeight = 3,
                                                   Orientation = 20,
                                                   X0 = 0
                                               });

            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeprofile1,
                dikeprofile2
            }, "some/path/to/dikeprofiles");

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationWithOutputAndDikeProfileAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeprofile1
                },
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0),
                                                            new TestDikeHeightOutput(0, CalculationConvergence.CalculatedConverged),
                                                            new TestOvertoppingRateOutput(0, CalculationConvergence.CalculatedConverged))
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0),
                                                            new TestDikeHeightOutput(0, CalculationConvergence.CalculatedConverged),
                                                            new TestOvertoppingRateOutput(0, CalculationConvergence.CalculatedConverged))
            };
            var calculationWithHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var calculationWithDikeProfileAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeprofile2
                }
            };

            var subCalculation = new GrassCoverErosionInwardsCalculation();
            var subCalculationWithOutputAndDikeProfileAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeprofile2
                },
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0),
                                                            new TestDikeHeightOutput(0, CalculationConvergence.CalculatedConverged),
                                                            new TestOvertoppingRateOutput(0, CalculationConvergence.CalculatedConverged))
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0),
                                                            new TestDikeHeightOutput(0, CalculationConvergence.CalculatedConverged),
                                                            new TestOvertoppingRateOutput(0, CalculationConvergence.CalculatedConverged))
            };
            var subCalculationWithHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var subCalculationWithDikeProfileAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeprofile2
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndDikeProfileAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithDikeProfileAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutputAndDikeProfileAndHydraulicBoundaryLocation,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithDikeProfileAndHydraulicBoundaryLocation
                }
            });

            AddFailureMechanismSections(failureMechanism);

            failureMechanism.SectionResults.ElementAt(0).Calculation = calculationWithOutputAndDikeProfileAndHydraulicBoundaryLocation;
            failureMechanism.SectionResults.ElementAt(1).Calculation = subCalculationWithOutputAndDikeProfileAndHydraulicBoundaryLocation;
        }

        private static void SetFullyConfiguredFailureMechanism(HeightStructuresFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var profile1 = new ForeshoreProfile(new Point2D(0, 0),
                                                new[]
                                                {
                                                    new Point2D(0, 0),
                                                    new Point2D(10, 0)
                                                },
                                                new BreakWater(BreakWaterType.Caisson, 1.1), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "aid",
                                                    Name = "A",
                                                    Orientation = 30,
                                                    X0 = 0
                                                });
            var profile2 = new ForeshoreProfile(new Point2D(10, 10),
                                                new[]
                                                {
                                                    new Point2D(0, 2),
                                                    new Point2D(20, 2)
                                                },
                                                new BreakWater(BreakWaterType.Dam, 2.2), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "bid",
                                                    Name = "B",
                                                    Orientation = 50,
                                                    X0 = 10
                                                });

            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile1,
                profile2
            }, "path");

            AddFailureMechanismSections(failureMechanism);

            SetConfiguredStructuresCalculations<HeightStructuresInput, HeightStructure>(failureMechanism, hydraulicBoundaryLocation, profile1, profile2);
        }

        private static void SetFullyConfiguredFailureMechanism(ClosingStructuresFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var profile1 = new ForeshoreProfile(new Point2D(0, 0),
                                                new[]
                                                {
                                                    new Point2D(0, 0),
                                                    new Point2D(10, 0)
                                                },
                                                new BreakWater(BreakWaterType.Caisson, 1.1), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "aid",
                                                    Name = "A",
                                                    Orientation = 30,
                                                    X0 = 0
                                                });
            var profile2 = new ForeshoreProfile(new Point2D(10, 10),
                                                new[]
                                                {
                                                    new Point2D(0, 2),
                                                    new Point2D(20, 2)
                                                },
                                                new BreakWater(BreakWaterType.Dam, 2.2), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "bid",
                                                    Name = "B",
                                                    Orientation = 50,
                                                    X0 = 10
                                                });

            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile1,
                profile2
            }, "some/path/to/foreshoreprofiles");

            AddFailureMechanismSections(failureMechanism);

            SetConfiguredStructuresCalculations<ClosingStructuresInput, ClosingStructure>(failureMechanism, hydraulicBoundaryLocation, profile1, profile2);
        }

        private static void SetFullyConfiguredFailureMechanism(StabilityPointStructuresFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var profile1 = new ForeshoreProfile(new Point2D(0, 0),
                                                new[]
                                                {
                                                    new Point2D(0, 0),
                                                    new Point2D(10, 0)
                                                },
                                                new BreakWater(BreakWaterType.Caisson, 1.1), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "aid",
                                                    Name = "A",
                                                    Orientation = 30,
                                                    X0 = 0
                                                });
            var profile2 = new ForeshoreProfile(new Point2D(10, 10),
                                                new[]
                                                {
                                                    new Point2D(0, 2),
                                                    new Point2D(20, 2)
                                                },
                                                new BreakWater(BreakWaterType.Dam, 2.2), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "bid",
                                                    Name = "B",
                                                    Orientation = 50,
                                                    X0 = 10
                                                });

            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile1,
                profile2
            }, "some/path/to/foreshoreprofiles");

            AddFailureMechanismSections(failureMechanism);

            SetConfiguredStructuresCalculations<StabilityPointStructuresInput, StabilityPointStructure>(failureMechanism, hydraulicBoundaryLocation, profile1, profile2);
        }

        private static void SetConfiguredStructuresCalculations<TCalculationInput, TStructureBase>(
            ICalculatableFailureMechanism failureMechanism,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            ForeshoreProfile profile1,
            ForeshoreProfile profile2)
            where TStructureBase : StructureBase
            where TCalculationInput : StructuresInputBase<TStructureBase>, new()
        {
            var calculation = new StructuresCalculation<TCalculationInput>();
            var calculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithHydraulicBoundaryLocationAndForeshoreProfile = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfiles = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new StructuresCalculation<TCalculationInput>();
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestStructuresOutput()
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
                },
                Output = new TestStructuresOutput()
            };
            var subCalculationWithHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocationAndForeshoreProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfiles);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile
                }
            });
        }

        private static void SetFullyConfiguredFailureMechanism(StabilityStoneCoverFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var profile1 = new ForeshoreProfile(new Point2D(0, 0),
                                                new[]
                                                {
                                                    new Point2D(0, 0),
                                                    new Point2D(10, 0)
                                                },
                                                new BreakWater(BreakWaterType.Caisson, 1.1), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "aid",
                                                    Name = "A",
                                                    Orientation = 30,
                                                    X0 = 0
                                                });
            var profile2 = new ForeshoreProfile(new Point2D(10, 10),
                                                new[]
                                                {
                                                    new Point2D(0, 2),
                                                    new Point2D(20, 2)
                                                },
                                                new BreakWater(BreakWaterType.Dam, 2.2), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "bid",
                                                    Name = "B",
                                                    Orientation = 50,
                                                    X0 = 10
                                                });

            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile1,
                profile2
            }, "path");
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var calculationWithOutputAndHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithForeshoreProfile = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
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
            var subCalculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
                },
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
            var subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
                }
            };

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithForeshoreProfile);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile
                }
            });

            AddFailureMechanismSections(failureMechanism);
        }

        private static void SetFullyConfiguredFailureMechanism(WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var profile1 = new ForeshoreProfile(new Point2D(0, 0),
                                                new[]
                                                {
                                                    new Point2D(0, 0),
                                                    new Point2D(10, 0)
                                                },
                                                new BreakWater(BreakWaterType.Caisson, 1.1), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "aid",
                                                    Name = "A",
                                                    Orientation = 30,
                                                    X0 = 0
                                                });
            var profile2 = new ForeshoreProfile(new Point2D(10, 10),
                                                new[]
                                                {
                                                    new Point2D(0, 2),
                                                    new Point2D(20, 2)
                                                },
                                                new BreakWater(BreakWaterType.Dam, 2.2), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "bid",
                                                    Name = "B",
                                                    Orientation = 50,
                                                    X0 = 10
                                                });

            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile1,
                profile2
            }, "path");

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationWithOutputAndHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithForeshoreProfile = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
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
            var subCalculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
                },
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
            var subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
                }
            };

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithForeshoreProfile);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile
                }
            });

            AddFailureMechanismSections(failureMechanism);
        }

        private static void SetFullyConfiguredFailureMechanism(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                               Random random)
        {
            failureMechanism.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            var profile1 = new ForeshoreProfile(new Point2D(0, 0),
                                                new[]
                                                {
                                                    new Point2D(0, 0),
                                                    new Point2D(10, 0)
                                                },
                                                new BreakWater(BreakWaterType.Caisson, 1.1), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "aid",
                                                    Name = "A",
                                                    Orientation = 30,
                                                    X0 = 0
                                                });
            var profile2 = new ForeshoreProfile(new Point2D(10, 10),
                                                new[]
                                                {
                                                    new Point2D(0, 2),
                                                    new Point2D(20, 2)
                                                },
                                                new BreakWater(BreakWaterType.Dam, 2.2), new ForeshoreProfile.ConstructionProperties
                                                {
                                                    Id = "bid",
                                                    Name = "B",
                                                    Orientation = 50,
                                                    X0 = 10
                                                });

            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile1,
                profile2
            }, "path");

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithHydraulicBoundaryLocationAndhForeshoreProfile = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
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
            var subCalculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
                },
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
            var subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
                }
            };

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocationAndhForeshoreProfile);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile
                }
            });

            AddFailureMechanismSections(failureMechanism);
        }

        private static void SetFullyConfiguredFailureMechanism(DuneErosionFailureMechanism failureMechanism)
        {
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation()
            });

            failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestDuneLocationCalculationOutput();
            failureMechanism.CalculationsForMechanismSpecificSignalingNorm.First().Output = new TestDuneLocationCalculationOutput();
            failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.First().Output = new TestDuneLocationCalculationOutput();
            failureMechanism.CalculationsForLowerLimitNorm.First().Output = new TestDuneLocationCalculationOutput();
            failureMechanism.CalculationsForFactorizedLowerLimitNorm.First().Output = new TestDuneLocationCalculationOutput();

            AddFailureMechanismSections(failureMechanism);
        }

        private static void SetFullyConfiguredFailureMechanism(IFailureMechanism failureMechanism)
        {
            AddFailureMechanismSections(failureMechanism);
        }

        private static void AddFailureMechanismSections(IFailureMechanism failureMechanism)
        {
            var section1 = new FailureMechanismSection("1",
                                                       new[]
                                                       {
                                                           new Point2D(-1, -1),
                                                           new Point2D(5, 5)
                                                       });
            var section2 = new FailureMechanismSection("2",
                                                       new[]
                                                       {
                                                           new Point2D(5, 5),
                                                           new Point2D(15, 15)
                                                       });
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section1,
                section2
            });
        }

        private static void AddFailureMechanismSections(IFailureMechanism failureMechanism, int numberOfSections)
        {
            var startPoint = new Point2D(-1, -1);
            var endPoint = new Point2D(15, 15);
            double endPointStepsX = (endPoint.X - startPoint.X) / numberOfSections;
            double endPointStepsY = (endPoint.Y - startPoint.Y) / numberOfSections;

            for (var i = 1; i <= numberOfSections; i++)
            {
                endPoint = new Point2D(startPoint.X + endPointStepsX, startPoint.Y + endPointStepsY);
                FailureMechanismTestHelper.SetSections(failureMechanism, new[]
                {
                    new FailureMechanismSection(i.ToString(),
                                                new[]
                                                {
                                                    startPoint,
                                                    endPoint
                                                })
                });
                startPoint = endPoint;
            }
        }
    }
}