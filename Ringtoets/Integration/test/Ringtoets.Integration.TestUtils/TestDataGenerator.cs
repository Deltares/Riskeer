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
        /// Gets a fully configured <see cref="AssessmentSection"/>.
        /// </summary>
        /// <returns>A fully configured <see cref="AssessmentSection"/>.</returns>
        public static AssessmentSection GetFullyConfiguredAssessmentSection()
        {
            return GetFullyConfiguredAssessmentSection(AssessmentSectionComposition.Dike);
        }

        /// <summary>
        /// Gets a fully configured <see cref="AssessmentSection"/> with a desired <see cref="AssessmentSectionComposition"/>.
        /// </summary>
        /// <param name="composition">The desired <see cref="AssessmentSectionComposition"/> to initialize the <see cref="AssessmentSection"/> with.</param>
        /// <returns>A fully configured <see cref="AssessmentSection"/>.</returns>
        public static AssessmentSection GetFullyConfiguredAssessmentSection(AssessmentSectionComposition composition)
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
            PipingTestDataGenerator.SetFullyConfiguredFailureMechanism(assessmentSection.PipingFailureMechanism, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.StabilityPointStructures, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.StabilityStoneCover, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.WaveImpactAsphaltCover, hydraulicBoundaryLocation);
            SetFullyConfiguredFailureMechanism(assessmentSection.DuneErosion, hydraulicBoundaryLocation);

            return assessmentSection;
        }

        /// <summary>
        /// Gets a fully configured <see cref="StabilityStoneCoverFailureMechanism"/>.
        /// </summary>
        public static StabilityStoneCoverFailureMechanism GetFullyConfiguredStabilityStoneCoverFailureMechanism()
        {
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
        /// </summary>
        public static WaveImpactAsphaltCoverFailureMechanism GetFullyConfiguredWaveImpactAsphaltCoverFailureMechanism()
        {
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        public static GrassCoverErosionOutwardsFailureMechanism GetFullyConfiguredGrassCoverErosionOutwardsFailureMechanism()
        {
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="HeightStructuresFailureMechanism"/>.
        /// </summary>
        public static HeightStructuresFailureMechanism GetFullyConfiguredHeightStructuresFailureMechanism()
        {
            var failureMechanism = new HeightStructuresFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="ClosingStructuresFailureMechanism"/>.
        /// </summary>
        public static ClosingStructuresFailureMechanism GetFullyConfiguredClosingStructuresFailureMechanism()
        {
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="StabilityPointStructuresFailureMechanism"/>.
        /// </summary>
        public static StabilityPointStructuresFailureMechanism GetFullyConfiguredStabilityPointStructuresFailureMechanism()
        {
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a fully configured <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        public static GrassCoverErosionInwardsFailureMechanism GetFullyConfiguredGrassCoverErosionInwardsFailureMechanism()
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
                                                   Name = "B",
                                                   DikeHeight = 3,
                                                   Orientation = 20,
                                                   X0 = 0
                                               });

            failureMechanism.DikeProfiles.Add(dikeprofile1);
            failureMechanism.DikeProfiles.Add(dikeprofile2);

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeprofile1
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new DikeHeightAssessmentOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged))
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeprofile2
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new DikeHeightAssessmentOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged))
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
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeprofile2
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new DikeHeightAssessmentOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged))
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeprofile1
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new DikeHeightAssessmentOutput(0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged))
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
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
                                                            {
                                                                Children =
                                                                {
                                                                    subCalculation,
                                                                    subCalculationWithOutput,
                                                                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                                                                    subCalculationWithHydraulicBoundaryLocation
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

            failureMechanism.SectionResults.ElementAt(0).Calculation = calculationWithOutput;
            failureMechanism.SectionResults.ElementAt(1).Calculation = subCalculationWithOutput;
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
            var calculationWithOutput = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var calculationWithForeshoreProfile = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
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
            var subCalculationWithOutput = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile2
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculation<TCalculationInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
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
            failureMechanism.CalculationsGroup.Children.Add(calculationWithForeshoreProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
                                                            {
                                                                Children =
                                                                {
                                                                    subCalculation,
                                                                    subCalculationWithOutput,
                                                                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                                                                    subCalculationWithHydraulicBoundaryLocation
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
                                                    Name = "B",
                                                    Orientation = 50,
                                                    X0 = 10
                                                });

            failureMechanism.ForeshoreProfiles.Add(profile1);
            failureMechanism.ForeshoreProfiles.Add(profile2);

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var calculationWithOutput = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
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
            var calculationWithOutputAndHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
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
            var subCalculationWithOutput = new StabilityStoneCoverWaveConditionsCalculation
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
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
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
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithForeshoreProfile);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
                                                                         {
                                                                             Children =
                                                                             {
                                                                                 subCalculation,
                                                                                 subCalculationWithOutput,
                                                                                 subCalculationWithOutputAndHydraulicBoundaryLocation,
                                                                                 subCalculationWithHydraulicBoundaryLocation
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
                                                    Name = "B",
                                                    Orientation = 50,
                                                    X0 = 10
                                                });

            failureMechanism.ForeshoreProfiles.Add(profile1);
            failureMechanism.ForeshoreProfiles.Add(profile2);

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationWithOutput = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
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
            var calculationWithOutputAndHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
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
            var subCalculationWithOutput = new WaveImpactAsphaltCoverWaveConditionsCalculation
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
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile1
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
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithForeshoreProfile);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
                                                                         {
                                                                             Children =
                                                                             {
                                                                                 subCalculation,
                                                                                 subCalculationWithOutput,
                                                                                 subCalculationWithOutputAndHydraulicBoundaryLocation,
                                                                                 subCalculationWithHydraulicBoundaryLocation
                                                                             }
                                                                         });
        }

        private static void SetFullyConfiguredFailureMechanism(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            HydraulicBoundaryLocation internalHydroLocation = new HydraulicBoundaryLocation(hydraulicBoundaryLocation.Id,
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
                                                    Name = "B",
                                                    Orientation = 50,
                                                    X0 = 10
                                                });

            failureMechanism.ForeshoreProfiles.Add(profile1);
            failureMechanism.ForeshoreProfiles.Add(profile2);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var calculationWithOutput = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = internalHydroLocation,
                    ForeshoreProfile = profile1
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithForeshoreProfile = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = internalHydroLocation,
                    ForeshoreProfile = profile1
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = internalHydroLocation,
                    ForeshoreProfile = profile2
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = internalHydroLocation
                }
            };

            var subCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var subCalculationWithOutput = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = internalHydroLocation,
                    ForeshoreProfile = profile2
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = internalHydroLocation,
                    ForeshoreProfile = profile1
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = internalHydroLocation
                }
            };

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutput);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithForeshoreProfile);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
                                                                         {
                                                                             Children =
                                                                             {
                                                                                 subCalculation,
                                                                                 subCalculationWithOutput,
                                                                                 subCalculationWithOutputAndHydraulicBoundaryLocation,
                                                                                 subCalculationWithHydraulicBoundaryLocation
                                                                             }
                                                                         });
        }

        private static void SetFullyConfiguredFailureMechanism(DuneErosionFailureMechanism failureMechanism,
                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            DuneLocation duneLocation = new DuneLocation(hydraulicBoundaryLocation.Id,
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
        }
    }
}