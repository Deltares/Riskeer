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

using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Service;
using Ringtoets.Piping.Integration.TestUtils;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.TestUtils
{
    /// <summary>
    /// Class that generates fully configured Ringtoets objects.
    /// </summary>
    public static class TestDataGenerator
    {
        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with all possible configurations for the 
        /// parent and nested calculations of the failure mechanisms.
        /// </summary>
        /// <returns>A fully configured <see cref="AssessmentSection"/> with all possible configuration of 
        /// the parent and nested calculations inside the failure mechanisms.</returns>
        public static AssessmentSection GetAssessmentSectionWithAllCalculationConfigurations()
        {
            return GetAssessmentSectionWithAllCalculationConfigurations(AssessmentSectionComposition.Dike);
        }

        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with a desired <see cref="AssessmentSectionComposition"/>
        /// and with all possible configurations for the parent and nested calculations of the failure mechanisms.
        /// </summary>
        /// <param name="composition">The desired <see cref="AssessmentSectionComposition"/> to initialize the <see cref="AssessmentSection"/> with.</param>
        /// <returns>A fully configured <see cref="AssessmentSection"/> with all possible configurations of 
        /// the parent and nested calculation inside the failure mechanisms.</returns>
        public static AssessmentSection GetAssessmentSectionWithAllCalculationConfigurations(AssessmentSectionComposition composition)
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            {
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(
                    1.1, double.NaN, double.NaN, double.NaN, double.NaN, CalculationConvergence.CalculatedConverged),
                WaveHeightOutput = new HydraulicBoundaryLocationOutput(
                    2.2, double.NaN, double.NaN, double.NaN, double.NaN, CalculationConvergence.CalculatedConverged)
            };

            var assessmentSection = new AssessmentSection(composition)
            {
                ReferenceLine = new ReferenceLine(),
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        hydraulicBoundaryLocation
                    }
                }
            };

            SetFullyConfiguredFailureMechanism(assessmentSection.ClosingStructures, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.GrassCoverErosionInwards, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.GrassCoverErosionOutwards, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.HeightStructures, hydraulicBoundaryLocation);
            PipingTestDataGenerator.ConfigureFailureMechanismWithAllCalculationConfigurations(assessmentSection.PipingFailureMechanism, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.StabilityPointStructures, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.StabilityStoneCover, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.WaveImpactAsphaltCover, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.DuneErosion, hydraulicBoundaryLocation);

            return assessmentSection;
        }

        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with all possible configurations of the parent 
        /// and nested calculations, but without the output of the  <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>,  
        /// and <see cref="DuneLocation"/>.
        /// </summary>
        /// <returns>A fully configured <see cref="AssessmentSection"/> with all possible calculation configurations of 
        /// the parent and nested calculations inside the failure mechanisms, but without the output of the 
        /// <see cref="HydraulicBoundaryLocation"/> and the <see cref="DuneLocation"/>.</returns>
        public static AssessmentSection GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput()
        {
            return GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(AssessmentSectionComposition.Dike);
        }

        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with a desired <see cref="AssessmentSectionComposition"/> and 
        /// possible configurations of the parent and nested calculations, but without the output of the 
        /// <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>, <see cref="HydraulicBoundaryLocation.WaveHeightOutput"/> 
        /// and <see cref="DuneLocation.Output"/>.
        /// </summary>
        /// <param name="composition">The desired <see cref="AssessmentSectionComposition"/> to initialize the <see cref="AssessmentSection"/> with.</param>
        /// <returns>A fully configured <see cref="AssessmentSection"/> with all possible calculation configurations of the parent and 
        /// nested calculations inside the failure mechanisms, but without the output of the <see cref="HydraulicBoundaryLocation"/> and the 
        /// <see cref="DuneLocation"/>.</returns>
        public static AssessmentSection GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(AssessmentSectionComposition composition)
        {
            AssessmentSection assessmentSection = GetAssessmentSectionWithAllCalculationConfigurations(composition);
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase, assessmentSection);

            return assessmentSection;
        }

        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with all possible configurations of the 
        /// parent and nested calculations, but without any calculation output of the failure mechanism.
        /// </summary>
        /// <returns>A fully configured <see cref="AssessmentSection"/> with all possible configurations of the parent and nested calculations
        /// of the failure mechanisms, but without any calculation outputs.</returns>
        public static AssessmentSection GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput()
        {
            return GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(AssessmentSectionComposition.Dike);
        }

        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with a desired <see cref="AssessmentSectionComposition"/> and 
        /// possible configurations of the parent and nested calculations, but without any calculation output of the failure mechanisms.
        /// </summary>
        /// <param name="composition">The desired <see cref="AssessmentSectionComposition"/> to initialize the <see cref="AssessmentSection"/> with.</param>
        /// <returns>A fully configured <see cref="AssessmentSection"/> with all possible configurations of the parent and nested calculations 
        /// of the failure mechanisms, but without any calculation output.</returns>
        public static AssessmentSection GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(AssessmentSectionComposition composition)
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
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="WaveImpactAsphaltCoverFailureMechanism"/> with
        /// all possible parent and nested calculation configurations.
        /// </summary>
        public static WaveImpactAsphaltCoverFailureMechanism GetWaveImpactAsphaltCoverFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="GrassCoverErosionOutwardsFailureMechanism"/> with 
        /// all possible parent and nested calculation configurations.
        /// </summary>
        public static GrassCoverErosionOutwardsFailureMechanism GetGrassCoverErosionOutwardsFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="HeightStructuresFailureMechanism"/> with all possible 
        /// parent and nested calculation configurations.
        /// </summary>
        public static HeightStructuresFailureMechanism GetHeightStructuresFailureMechanismWithAlLCalculationConfigurations()
        {
            var failureMechanism = new HeightStructuresFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="ClosingStructuresFailureMechanism"/> with all possible
        /// parent and nested calculation configurations.
        /// </summary>
        public static ClosingStructuresFailureMechanism GetClosingStructuresFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="StabilityPointStructuresFailureMechanism"/> with all possible
        /// parent and nested calculation configurations.
        /// </summary>
        public static StabilityPointStructuresFailureMechanism GetStabilityPointStructuresFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="GrassCoverErosionInwardsFailureMechanism"/> with all possible
        /// parent and nested calculation configurations.
        /// </summary>
        public static GrassCoverErosionInwardsFailureMechanism GetGrassCoverErosionInwardsFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
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
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new DikeHeightOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged),
                                                            new OvertoppingRateOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged))
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new DikeHeightOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged),
                                                            new OvertoppingRateOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged))
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
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new DikeHeightOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged),
                                                            new OvertoppingRateOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged))
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new DikeHeightOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged),
                                                            new OvertoppingRateOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged))
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
            failureMechanism.AddSection(section1);
            failureMechanism.AddSection(section2);

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

            failureMechanism.ForeshoreProfiles.Add(profile1);
            failureMechanism.ForeshoreProfiles.Add(profile2);
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

            failureMechanism.ForeshoreProfiles.Add(profile1);
            failureMechanism.ForeshoreProfiles.Add(profile2);
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

            failureMechanism.ForeshoreProfiles.Add(profile1);
            failureMechanism.ForeshoreProfiles.Add(profile2);
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
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
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
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocationAndForeshoreProfile = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
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

            failureMechanism.ForeshoreProfiles.Add(profile1);
            failureMechanism.ForeshoreProfiles.Add(profile2);
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
            var subCalculationWithOutputAndHydraulicBoundarLocationAndForeshoreProfile = new StabilityStoneCoverWaveConditionsCalculation
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
                    subCalculationWithOutputAndHydraulicBoundarLocationAndForeshoreProfile,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile
                }
            });
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

            failureMechanism.ForeshoreProfiles.Add(profile1);
            failureMechanism.ForeshoreProfiles.Add(profile2);

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
            var subCalculationWithOutputAndHydraulicBoundarLocationAndForeshoreProfile = new WaveImpactAsphaltCoverWaveConditionsCalculation
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
                    subCalculationWithOutputAndHydraulicBoundarLocationAndForeshoreProfile,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile
                }
            });
        }

        private static void SetFullyConfiguredFailureMechanism(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var internalHydroLocation = new HydraulicBoundaryLocation(hydraulicBoundaryLocation.Id,
                                                                      hydraulicBoundaryLocation.Name,
                                                                      hydraulicBoundaryLocation.Location.X,
                                                                      hydraulicBoundaryLocation.Location.Y)
            {
                WaveHeightOutput = new HydraulicBoundaryLocationOutput(
                    hydraulicBoundaryLocation.WaveHeight + 0.2, double.NaN, double.NaN, double.NaN, double.NaN,
                    hydraulicBoundaryLocation.WaveHeightCalculationConvergence),
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(
                    hydraulicBoundaryLocation.DesignWaterLevel + 0.3, double.NaN, double.NaN, double.NaN, double.NaN,
                    hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence)
            };
            failureMechanism.HydraulicBoundaryLocations.Add(internalHydroLocation);
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

            failureMechanism.ForeshoreProfiles.Add(profile1);
            failureMechanism.ForeshoreProfiles.Add(profile2);

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
            var subCalculationWithOutputAndHydraulicBoundarLocationAndForeshoreProfile = new GrassCoverErosionOutwardsWaveConditionsCalculation
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
                    subCalculationWithOutputAndHydraulicBoundarLocationAndForeshoreProfile,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile
                }
            });
        }

        private static void SetFullyConfiguredFailureMechanism(DuneErosionFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var duneLocation = new DuneLocation(hydraulicBoundaryLocation.Id,
                                                hydraulicBoundaryLocation.Name,
                                                new Point2D(hydraulicBoundaryLocation.Location.X, hydraulicBoundaryLocation.Location.Y),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 7,
                                                    Offset = 20,
                                                    Orientation = 180,
                                                    D50 = 0.00008
                                                });

            var duneLocationWithOutput = new DuneLocation(hydraulicBoundaryLocation.Id,
                                                          hydraulicBoundaryLocation.Name,
                                                          new Point2D(hydraulicBoundaryLocation.Location.X, hydraulicBoundaryLocation.Location.Y),
                                                          new DuneLocation.ConstructionProperties
                                                          {
                                                              CoastalAreaId = 7,
                                                              Offset = 20,
                                                              Orientation = 180,
                                                              D50 = 0.00008
                                                          })
            {
                Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                {
                    WaterLevel = hydraulicBoundaryLocation.DesignWaterLevel + 0.2,
                    WaveHeight = hydraulicBoundaryLocation.WaveHeight + 0.3,
                    WavePeriod = 10
                })
            };

            failureMechanism.DuneLocations.Add(duneLocation);
            failureMechanism.DuneLocations.Add(duneLocationWithOutput);
        }
    }
}