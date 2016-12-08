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
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Application.Ringtoets.Storage.TestUtil
{
    /// <summary>
    /// This class can be used to create <see cref="RingtoetsProject"/> instances which have their properties set and can be used in tests.
    /// </summary>
    public static class RingtoetsProjectTestHelper
    {
        /// <summary>
        /// Returns a new complete instance of <see cref="RingtoetsProject"/>.
        /// </summary>
        /// <returns>A new complete instance of <see cref="RingtoetsProject"/>.</returns>
        public static RingtoetsProject GetFullTestProject()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = "assessmentSection",
                HydraulicBoundaryDatabase = GetHydraulicBoundaryDatabase(),
                ReferenceLine = GetReferenceLine()
            };
            PipingFailureMechanism pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
            ConfigurePipingFailureMechanism(pipingFailureMechanism, assessmentSection);
            AddSections(pipingFailureMechanism);
            SetSectionResults(pipingFailureMechanism.SectionResults);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            ConfigureGrassCoverErosionInwardsFailureMechanism(grassCoverErosionInwardsFailureMechanism, assessmentSection);
            AddSections(grassCoverErosionInwardsFailureMechanism);
            SetSectionResults(grassCoverErosionInwardsFailureMechanism.SectionResults,
                              (GrassCoverErosionInwardsCalculation) grassCoverErosionInwardsFailureMechanism.Calculations.First());

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            AddForeshoreProfiles(grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);
            ConfigureGrassCoverErosionOutwardsFailureMechanism(grassCoverErosionOutwardsFailureMechanism);
            AddSections(grassCoverErosionOutwardsFailureMechanism);
            SetSectionResults(grassCoverErosionOutwardsFailureMechanism.SectionResults);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            AddForeshoreProfiles(stabilityStoneCoverFailureMechanism.ForeshoreProfiles);
            ConfigureStabilityStoneCoverFailureMechanism(stabilityStoneCoverFailureMechanism, assessmentSection);
            AddSections(stabilityStoneCoverFailureMechanism);
            SetSectionResults(stabilityStoneCoverFailureMechanism.SectionResults);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            AddForeshoreProfiles(waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);
            ConfigureWaveImpactAsphaltCoverFailureMechanism(waveImpactAsphaltCoverFailureMechanism, assessmentSection);
            AddSections(waveImpactAsphaltCoverFailureMechanism);
            SetSectionResults(waveImpactAsphaltCoverFailureMechanism.SectionResults);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            AddForeshoreProfiles(heightStructuresFailureMechanism.ForeshoreProfiles);
            ConfigureHeightStructuresFailureMechanism(heightStructuresFailureMechanism, assessmentSection);
            AddSections(heightStructuresFailureMechanism);
            SetSectionResults(heightStructuresFailureMechanism.SectionResults,
                              (StructuresCalculation<HeightStructuresInput>) heightStructuresFailureMechanism.Calculations.First());

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            AddForeshoreProfiles(closingStructuresFailureMechanism.ForeshoreProfiles);
            ConfigureClosingStructuresFailureMechanism(closingStructuresFailureMechanism, assessmentSection);
            AddSections(closingStructuresFailureMechanism);
            SetSectionResults(closingStructuresFailureMechanism.SectionResults,
                              (StructuresCalculation<ClosingStructuresInput>) closingStructuresFailureMechanism.Calculations.First());

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            AddForeshoreProfiles(stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            ConfigureStabilityPointStructuresFailureMechanism(stabilityPointStructuresFailureMechanism,
                                                              assessmentSection);
            AddSections(stabilityPointStructuresFailureMechanism);
            SetSectionResults(stabilityPointStructuresFailureMechanism.SectionResults,
                              (StructuresCalculation<StabilityPointStructuresInput>) stabilityPointStructuresFailureMechanism.Calculations.First());

            AddSections(assessmentSection.MacrostabilityInwards);
            SetSectionResults(assessmentSection.MacrostabilityInwards.SectionResults);
            AddSections(assessmentSection.MacrostabilityOutwards);
            SetSectionResults(assessmentSection.MacrostabilityOutwards.SectionResults);
            AddSections(assessmentSection.Microstability);
            SetSectionResults(assessmentSection.Microstability.SectionResults);
            AddSections(assessmentSection.WaterPressureAsphaltCover);
            SetSectionResults(assessmentSection.WaterPressureAsphaltCover.SectionResults);
            AddSections(assessmentSection.GrassCoverSlipOffInwards);
            SetSectionResults(assessmentSection.GrassCoverSlipOffInwards.SectionResults);
            AddSections(assessmentSection.GrassCoverSlipOffOutwards);
            SetSectionResults(assessmentSection.GrassCoverSlipOffOutwards.SectionResults);
            AddSections(assessmentSection.StrengthStabilityLengthwiseConstruction);
            SetSectionResults(assessmentSection.StrengthStabilityLengthwiseConstruction.SectionResults);
            AddSections(assessmentSection.PipingStructure);
            SetSectionResults(assessmentSection.PipingStructure.SectionResults);
            AddSections(assessmentSection.DuneErosion);
            SetSectionResults(assessmentSection.DuneErosion.SectionResults);
            AddSections(assessmentSection.TechnicalInnovation);
            SetSectionResults(assessmentSection.TechnicalInnovation.SectionResults);

            var fullTestProject = new RingtoetsProject
            {
                Name = "tempProjectFile",
                Description = "description",
                AssessmentSections =
                {
                    assessmentSection
                }
            };
            return fullTestProject;
        }

        private static void SetSectionResults(IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<TechnicalInnovationFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = (RoundedDouble) random.NextDouble();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<MacrostabilityInwardsFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = (RoundedDouble) random.NextDouble();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = GetAssessmentLayerTwoAResult();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = GetAssessmentLayerTwoAResult();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<MicrostabilityFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = GetAssessmentLayerTwoAResult();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<PipingStructureFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = GetAssessmentLayerTwoAResult();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<DuneErosionFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = GetAssessmentLayerTwoAResult();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void AddSections(IFailureMechanism failureMechanism)
        {
            failureMechanism.AddSection(new FailureMechanismSection("section 1", new[]
            {
                new Point2D(0, 2),
                new Point2D(2, 3)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("section 2", new[]
            {
                new Point2D(2, 3),
                new Point2D(4, 5)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("section 3", new[]
            {
                new Point2D(4, 5),
                new Point2D(2, 3)
            }));
        }

        private static void AddForeshoreProfiles(ObservableList<ForeshoreProfile> foreshoreProfiles)
        {
            foreshoreProfiles.Add(new ForeshoreProfile(
                                      new Point2D(2, 5), new[]
                                      {
                                          new Point2D(1, 6),
                                          new Point2D(8, 5)
                                      }, new BreakWater(BreakWaterType.Caisson, 2.5), new ForeshoreProfile.ConstructionProperties
                                      {
                                          Name = "FP",
                                          Orientation = 95.5,
                                          X0 = 22.1
                                      }));
            foreshoreProfiles.Add(new ForeshoreProfile(
                                      new Point2D(2, 5), Enumerable.Empty<Point2D>(), null, new ForeshoreProfile.ConstructionProperties()));
        }

        private static ReferenceLine GetReferenceLine()
        {
            IEnumerable<Point2D> points = new[]
            {
                new Point2D(2, 3),
                new Point2D(5, 4),
                new Point2D(5, 8),
                new Point2D(-3, 2)
            };

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);
            return referenceLine;
        }

        private static HydraulicBoundaryDatabase GetHydraulicBoundaryDatabase()
        {
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "/temp/test",
                Version = "1.0"
            };
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(13001, "test", 152.3, 2938.5)
            {
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(12.4, double.NaN,
                                                                             double.NaN, double.NaN, 
                                                                             double.NaN, CalculationConvergence.NotCalculated),
                WaveHeightOutput = new HydraulicBoundaryLocationOutput(2.4, 0, 0, 0, 0, CalculationConvergence.NotCalculated)
            });

            return hydraulicBoundaryDatabase;
        }

        private static AssessmentLayerOneState GetAssessmentLayerOneState()
        {
            var random = new Random(21);
            return (AssessmentLayerOneState) random.Next(1, Enum.GetValues(typeof(AssessmentLayerOneState)).Length + 1);
        }

        private static AssessmentLayerTwoAResult GetAssessmentLayerTwoAResult()
        {
            var random = new Random(21);
            return (AssessmentLayerTwoAResult) random.Next(1, Enum.GetValues(typeof(AssessmentLayerTwoAResult)).Length + 1);
        }

        #region StabilityPointStructures FailureMechanism

        private static void ConfigureStabilityPointStructuresFailureMechanism(StabilityPointStructuresFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N = 8;

            StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure();
            failureMechanism.StabilityPointStructures.Add(stabilityPointStructure);
            failureMechanism.StabilityPointStructures.Add(new TestStabilityPointStructure());

            var random = new Random(56);

            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            HydraulicBoundaryLocation hydroLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "StabilityPoint structures A",
                Children =
                {
                    new StructuresCalculation<StabilityPointStructuresInput>
                    {
                        Name = "Calculation 1",
                        Comments =
                        {
                            Body = "Fully configured for greatness!"
                        },
                        InputParameters =
                        {
                            BreakWater =
                            {
                                Type = BreakWaterType.Dam,
                                Height = (RoundedDouble) random.NextDouble()
                            },
                            DrainCoefficient =
                            {
                                Mean = (RoundedDouble) random.NextDouble()
                            },
                            FactorStormDurationOpenStructure = (RoundedDouble) random.NextDouble(),
                            FailureProbabilityStructureWithErosion = random.NextDouble(),
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = hydroLocation,
                            LoadSchematizationType = LoadSchematizationType.Quadratic,
                            ModelFactorSuperCriticalFlow =
                            {
                                Mean = (RoundedDouble) random.NextDouble()
                            },
                            VolumicWeightWater = (RoundedDouble) random.NextDouble(),
                            UseForeshore = random.NextBoolean(),
                            UseBreakWater = random.NextBoolean(),
                            StormDuration =
                            {
                                Mean = (RoundedDouble) random.NextDouble()
                            },
                            Structure = stabilityPointStructure
                        },
                        Output = new ProbabilityAssessmentOutput(0.7, 0.85, 0.9, 0.10, 0.11)
                    }
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "StabilityPoint structures B"
            });
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<StabilityPointStructuresInput>());
        }

        private static void SetSectionResults(IEnumerable<StabilityPointStructuresFailureMechanismSectionResult> sectionResults,
                                              StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            var random = new Random(21);
            var firstSectionResultHasCalculation = false;
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
                if (!firstSectionResultHasCalculation)
                {
                    sectionResult.Calculation = calculation;
                    firstSectionResultHasCalculation = true;
                }
            }
        }

        #endregion

        #region ClosingStructures FailureMechanism

        private static void ConfigureClosingStructuresFailureMechanism(ClosingStructuresFailureMechanism failureMechanism,
                                                                       IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N2A = 6;

            ClosingStructure closingStructure = new TestClosingStructure();
            failureMechanism.ClosingStructures.Add(closingStructure);
            failureMechanism.ClosingStructures.Add(new TestClosingStructure());

            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            HydraulicBoundaryLocation hydroLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "Closing structures A",
                Children =
                {
                    new StructuresCalculation<ClosingStructuresInput>
                    {
                        InputParameters =
                        {
                            BreakWater =
                            {
                                Type = BreakWaterType.Dam,
                                Height = (RoundedDouble) 4.4
                            },
                            FactorStormDurationOpenStructure = (RoundedDouble) 0.56,
                            FailureProbabilityStructureWithErosion = (RoundedDouble) 0.34,
                            ForeshoreProfile = foreshoreProfile,
                            DeviationWaveDirection = (RoundedDouble) 18.18,
                            DrainCoefficient =
                            {
                                Mean = (RoundedDouble) 19.19
                            },
                            HydraulicBoundaryLocation = hydroLocation,
                            ModelFactorSuperCriticalFlow =
                            {
                                Mean = (RoundedDouble) 13.13
                            },
                            StormDuration =
                            {
                                Mean = (RoundedDouble) 1.1
                            },
                            Structure = closingStructure,
                            UseBreakWater = true,
                            UseForeshore = true
                        },
                        Output = new ProbabilityAssessmentOutput(0.8, 0.95, 0.10, 0.11, 0.12)
                    }
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "Closing structures B"
            });
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());
        }

        private static void SetSectionResults(IEnumerable<ClosingStructuresFailureMechanismSectionResult> sectionResults,
                                              StructuresCalculation<ClosingStructuresInput> calculation)
        {
            var random = new Random(21);
            var firstSectionResultHasCalculation = false;
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
                if (!firstSectionResultHasCalculation)
                {
                    sectionResult.Calculation = calculation;
                    firstSectionResultHasCalculation = true;
                }
            }
        }

        #endregion

        #region HeightStructures FailureMechanism

        private static void ConfigureHeightStructuresFailureMechanism(HeightStructuresFailureMechanism failureMechanism,
                                                                      IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N = 5;

            var hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;

            var heightStructure = new TestHeightStructure();
            failureMechanism.HeightStructures.Add(heightStructure);
            failureMechanism.HeightStructures.Add(new TestHeightStructure());

            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "Height structures A",
                Children =
                {
                    new StructuresCalculation<HeightStructuresInput>
                    {
                        InputParameters =
                        {
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = hydraulicBoundaryLocations[0],
                            ModelFactorSuperCriticalFlow =
                            {
                                Mean = (RoundedDouble) 1.1
                            },
                            StormDuration =
                            {
                                Mean = (RoundedDouble) 1.7
                            },
                            Structure = heightStructure
                        },
                        Output = new ProbabilityAssessmentOutput(0.8, 0.95, 0.10, 0.11, 0.12)
                    }
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "Height structures B"
            });
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<HeightStructuresInput>());
        }

        private static void SetSectionResults(IEnumerable<HeightStructuresFailureMechanismSectionResult> sectionResults,
                                              StructuresCalculation<HeightStructuresInput> calculation)
        {
            var random = new Random(21);
            var firstSectionResultHasCalculation = false;
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
                if (!firstSectionResultHasCalculation)
                {
                    sectionResult.Calculation = calculation;
                    firstSectionResultHasCalculation = true;
                }
            }
        }

        #endregion

        #region Piping FailureMechanism

        private static void ConfigurePipingFailureMechanism(PipingFailureMechanism pipingFailureMechanism, AssessmentSection assessmentSection)
        {
            pipingFailureMechanism.PipingProbabilityAssessmentInput.A = 0.9;

            var referenceLineGeometryPoints = assessmentSection.ReferenceLine.Points.ToArray();

            PipingSoilProfile pipingSoilProfile = new TestPipingSoilProfile();
            PipingSoilLayer pipingSoilLayer = pipingSoilProfile.Layers.First();
            pipingSoilLayer.BelowPhreaticLevelMean = 2.2;
            pipingSoilLayer.BelowPhreaticLevelDeviation = 1.2;
            pipingSoilLayer.BelowPhreaticLevelShift = 3.2;
            pipingSoilLayer.DiameterD70Mean = 2.42;
            pipingSoilLayer.DiameterD70Deviation = 21.002;
            pipingSoilLayer.PermeabilityMean = 0.9982;
            pipingSoilLayer.PermeabilityDeviation = 0.220;
            pipingSoilLayer.Color = Color.HotPink;
            pipingSoilLayer.MaterialName = "HotPinkLayer";

            pipingFailureMechanism.StochasticSoilModels.Add(new StochasticSoilModel(-1, "modelName", "modelSegmentName")
            {
                Geometry =
                {
                    referenceLineGeometryPoints[1],
                    referenceLineGeometryPoints[2],
                    referenceLineGeometryPoints[3]
                },
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, -1)
                    {
                        SoilProfile = pipingSoilProfile
                    },
                    new StochasticSoilProfile(0.8, SoilProfileType.SoilProfile1D, -1)
                    {
                        SoilProfile = new TestPipingSoilProfile()
                    }
                }
            });
            pipingFailureMechanism.SurfaceLines.Add(GetSurfaceLine());

            CalculationGroup pipingCalculationGroup = pipingFailureMechanism.CalculationsGroup;
            pipingCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "A",
                Children =
                {
                    new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
                    {
                        Name = "AA",
                        IsRelevant = true,
                        Contribution = (RoundedDouble) 1.0,
                        Comments =
                        {
                            Body = "Nice comment about this calculation!"
                        },
                        InputParameters =
                        {
                            SurfaceLine = pipingFailureMechanism.SurfaceLines.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                            StochasticSoilModel = pipingFailureMechanism.StochasticSoilModels.First(),
                            StochasticSoilProfile = pipingFailureMechanism.StochasticSoilModels.First()
                                                                          .StochasticSoilProfiles.First(),
                            EntryPointL = (RoundedDouble) 1.0,
                            ExitPointL = (RoundedDouble) 2.0,
                            PhreaticLevelExit =
                            {
                                Mean = (RoundedDouble) 1.1,
                                StandardDeviation = (RoundedDouble) 2.2
                            },
                            DampingFactorExit =
                            {
                                Mean = (RoundedDouble) 3.3,
                                StandardDeviation = (RoundedDouble) 4.4
                            },
                            SaturatedVolumicWeightOfCoverageLayer =
                            {
                                Mean = (RoundedDouble) 7.7,
                                StandardDeviation = (RoundedDouble) 6.6,
                                Shift = (RoundedDouble) 5.5
                            },
                            Diameter70 =
                            {
                                Mean = (RoundedDouble) 8.8,
                                StandardDeviation = (RoundedDouble) 9.9
                            },
                            DarcyPermeability =
                            {
                                Mean = (RoundedDouble) 10.10,
                                StandardDeviation = (RoundedDouble) 11.11
                            }
                        },
                        Output = new TestPipingOutput(),
                        SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(7.7, 8.8, 0.9,
                                                                                    10.10, 11.11, 0.12,
                                                                                    13.13, 14.14, 0.15,
                                                                                    0.16, 17.17,
                                                                                    0.18, 19.19, 20.20)
                    }
                }
            });
            pipingCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "B"
            });
            pipingCalculationGroup.Children.Add(new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
            {
                Name = "C",
                IsRelevant = false,
                Contribution = (RoundedDouble) 0.5,
                Comments =
                {
                    Body = "Another great comment"
                },
                InputParameters =
                {
                    SurfaceLine = pipingFailureMechanism.SurfaceLines.First(),
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                    StochasticSoilModel = pipingFailureMechanism.StochasticSoilModels.First(),
                    StochasticSoilProfile = pipingFailureMechanism.StochasticSoilModels.First()
                                                                  .StochasticSoilProfiles.Skip(1).First(),
                    EntryPointL = (RoundedDouble) 0.3,
                    ExitPointL = (RoundedDouble) 2.3,
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble) 12.12,
                        StandardDeviation = (RoundedDouble) 13.13
                    },
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble) 14.14,
                        StandardDeviation = (RoundedDouble) 15.15
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble) 18.18,
                        StandardDeviation = (RoundedDouble) 17.17,
                        Shift = (RoundedDouble) 16.16
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble) 19.19,
                        StandardDeviation = (RoundedDouble) 20.20
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble) 21.21,
                        StandardDeviation = (RoundedDouble) 22.22
                    }
                },
                Output = null,
                SemiProbabilisticOutput = null
            });
        }

        private static void SetSectionResults(IEnumerable<PipingFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static RingtoetsPipingSurfaceLine GetSurfaceLine()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surfaceline",
                ReferenceLineIntersectionWorldPoint = new Point2D(4.0, 6.0)
            };

            var geometryPoints = new[]
            {
                new Point3D(6.0, 6.0, -2.3),
                new Point3D(5.8, 6.0, -2.3), // Dike toe at river
                new Point3D(5.6, 6.0, 3.4),
                new Point3D(4.2, 6.0, 3.5),
                new Point3D(4.0, 6.0, 0.5), // Dike toe at polder
                new Point3D(3.8, 6.0, 0.5), // Ditch dike side
                new Point3D(3.6, 6.0, 0.2), // Bottom ditch dike side
                new Point3D(3.4, 6.0, 0.25), // Bottom ditch polder side
                new Point3D(3.2, 6.0, 0.5), // Ditch polder side
                new Point3D(3.0, 6.0, 0.5)
            };
            surfaceLine.SetGeometry(geometryPoints);

            surfaceLine.SetDikeToeAtRiverAt(geometryPoints[1]);
            surfaceLine.SetDikeToeAtPolderAt(geometryPoints[4]);
            surfaceLine.SetDitchDikeSideAt(geometryPoints[5]);
            surfaceLine.SetBottomDitchDikeSideAt(geometryPoints[6]);
            surfaceLine.SetBottomDitchPolderSideAt(geometryPoints[7]);
            surfaceLine.SetDitchPolderSideAt(geometryPoints[8]);

            return surfaceLine;
        }

        #endregion

        #region GrassCoverErosionInwards FailureMechanism

        private static void ConfigureGrassCoverErosionInwardsFailureMechanism(GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N = 15;
            var dikeProfile = new DikeProfile(new Point2D(1, 2),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(1, 2), 1),
                                                  new RoughnessPoint(new Point2D(3, 4), 0.5)
                                              },
                                              new[]
                                              {
                                                  new Point2D(5, 6),
                                                  new Point2D(7, 8)
                                              },
                                              new BreakWater(BreakWaterType.Caisson, 15),
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  DikeHeight = 1.1,
                                                  Name = "2.2",
                                                  Orientation = 3.3,
                                                  X0 = 4.4
                                              });
            failureMechanism.DikeProfiles.Add(dikeProfile);
            failureMechanism.DikeProfiles.Add(new DikeProfile(new Point2D(9, 10),
                                                              new[]
                                                              {
                                                                  new RoughnessPoint(new Point2D(11, 12), 1),
                                                                  new RoughnessPoint(new Point2D(13, 14), 0.5)
                                                              },
                                                              new Point2D[0],
                                                              null,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  DikeHeight = 5.5,
                                                                  Name = "6.6",
                                                                  Orientation = 7.7,
                                                                  X0 = 8.8
                                                              }));
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "GEKB A",
                Children =
                {
                    new GrassCoverErosionInwardsCalculation
                    {
                        Name = "Calculation 1",
                        Comments =
                        {
                            Body = "Comments for Calculation 1"
                        },
                        InputParameters =
                        {
                            DikeProfile = dikeProfile,
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0],
                            BreakWater =
                            {
                                Height = (RoundedDouble) (dikeProfile.BreakWater.Height + 0.3),
                                Type = BreakWaterType.Wall
                            },
                            DikeHeight = (RoundedDouble) (dikeProfile.DikeHeight + 0.2),
                            Orientation = dikeProfile.Orientation,
                            CriticalFlowRate =
                            {
                                Mean = (RoundedDouble) 1.1,
                                StandardDeviation = (RoundedDouble) 2.2
                            },
                            CalculateDikeHeight = true,
                            UseForeshore = true,
                            UseBreakWater = true
                        },
                        Output = new GrassCoverErosionInwardsOutput(0.45, true, new ProbabilityAssessmentOutput(0.004, 0.95, 0.00003, 1.1, 4.5), 0.56)
                    }
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "GEKB B"
            });
            failureMechanism.CalculationsGroup.Children.Add(
                new GrassCoverErosionInwardsCalculation
                {
                    Name = "Calculation 2",
                    Comments =
                    {
                        Body = "Comments about Calculation 2"
                    }
                });
        }

        private static void SetSectionResults(IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults,
                                              GrassCoverErosionInwardsCalculation calculation)
        {
            var random = new Random(21);
            var firstSectionResultHasCalculation = false;
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
                if (!firstSectionResultHasCalculation)
                {
                    sectionResult.Calculation = calculation;
                    firstSectionResultHasCalculation = true;
                }
            }
        }

        #endregion

        #region GrassCoverErosionOutwards FailureMechanism

        private static void ConfigureGrassCoverErosionOutwardsFailureMechanism(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            failureMechanism.GeneralInput.N = 15;

            var hydraulicBoundaryLocations = failureMechanism.HydraulicBoundaryLocations;
            hydraulicBoundaryLocations.Add(new HydraulicBoundaryLocation(0, "HL 1", 100, 200));
            hydraulicBoundaryLocations.Add(new HydraulicBoundaryLocation(45, "HL 2", 123, 150));

            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "GCEO A",
                Children =
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
                    {
                        Name = "Calculation 1",
                        Comments =
                        {
                            Body = "Comments for Calculation 1"
                        },
                        InputParameters =
                        {
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = hydraulicBoundaryLocations[0],
                            BreakWater =
                            {
                                Height = (RoundedDouble) (foreshoreProfile.BreakWater.Height + 0.3),
                                Type = BreakWaterType.Wall
                            },
                            Orientation = foreshoreProfile.Orientation,
                            UseForeshore = true,
                            UseBreakWater = true,
                            UpperBoundaryRevetment = (RoundedDouble) 22.3,
                            LowerBoundaryRevetment = (RoundedDouble) (-3.2),
                            UpperBoundaryWaterLevels = (RoundedDouble) 15.3,
                            LowerBoundaryWaterLevels = (RoundedDouble) (-2.4),
                            StepSize = WaveConditionsInputStepSize.Two
                        }
                    }
                }
            });
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "GCEO B"
            });
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(
                new GrassCoverErosionOutwardsWaveConditionsCalculation
                {
                    Name = "Calculation 2",
                    Comments =
                    {
                        Body = "Comments for Calculation 2"
                    },
                    InputParameters =
                    {
                        ForeshoreProfile = null,
                        HydraulicBoundaryLocation = hydraulicBoundaryLocations[1],
                        BreakWater =
                        {
                            Height = (RoundedDouble) (foreshoreProfile.BreakWater.Height + 0.1),
                            Type = BreakWaterType.Dam
                        },
                        Orientation = foreshoreProfile.Orientation,
                        UseForeshore = false,
                        UseBreakWater = false,
                        UpperBoundaryRevetment = (RoundedDouble) 12.3,
                        LowerBoundaryRevetment = (RoundedDouble) (-3.5),
                        UpperBoundaryWaterLevels = (RoundedDouble) 13.3,
                        LowerBoundaryWaterLevels = (RoundedDouble) (-1.9),
                        StepSize = WaveConditionsInputStepSize.One
                    },
                    Output = new GrassCoverErosionOutwardsWaveConditionsOutput(new[]
                    {
                        new WaveConditionsOutput(1, 2, 3, 4, 5, 0.6, 0.7, 0.8, 0.9, CalculationConvergence.NotCalculated),
                        new WaveConditionsOutput(0, 1, 2, 3, 4, 0.5, 0.6, 0.7, 0.8, CalculationConvergence.NotCalculated)
                    })
                });
        }

        private static void SetSectionResults(IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = GetAssessmentLayerTwoAResult();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        #endregion

        #region StabilityStoneCover FailureMechanism

        private static void ConfigureStabilityStoneCoverFailureMechanism(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "SSC A",
                Children =
                {
                    new StabilityStoneCoverWaveConditionsCalculation
                    {
                        Name = "Calculation 1",
                        Comments =
                        {
                            Body = "Comments for Calculation 1"
                        },
                        InputParameters =
                        {
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0],
                            BreakWater =
                            {
                                Height = (RoundedDouble) (foreshoreProfile.BreakWater.Height + 0.3),
                                Type = BreakWaterType.Wall
                            },
                            Orientation = foreshoreProfile.Orientation,
                            UseForeshore = true,
                            UseBreakWater = true,
                            UpperBoundaryRevetment = (RoundedDouble) 22.3,
                            LowerBoundaryRevetment = (RoundedDouble) (-3.2),
                            UpperBoundaryWaterLevels = (RoundedDouble) 15.3,
                            LowerBoundaryWaterLevels = (RoundedDouble) (-2.4),
                            StepSize = WaveConditionsInputStepSize.Two
                        }
                    }
                }
            });
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "SSC B"
            });
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(
                new StabilityStoneCoverWaveConditionsCalculation
                {
                    Name = "Calculation 2",
                    Comments =
                    {
                        Body = "Comments for Calculation 2"
                    },
                    InputParameters =
                    {
                        ForeshoreProfile = null,
                        HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0],
                        BreakWater =
                        {
                            Height = (RoundedDouble) (foreshoreProfile.BreakWater.Height + 0.1),
                            Type = BreakWaterType.Dam
                        },
                        Orientation = foreshoreProfile.Orientation,
                        UseForeshore = false,
                        UseBreakWater = false,
                        UpperBoundaryRevetment = (RoundedDouble) 12.3,
                        LowerBoundaryRevetment = (RoundedDouble) (-3.5),
                        UpperBoundaryWaterLevels = (RoundedDouble) 13.3,
                        LowerBoundaryWaterLevels = (RoundedDouble) (-1.9),
                        StepSize = WaveConditionsInputStepSize.One
                    },
                    Output = new StabilityStoneCoverWaveConditionsOutput(new[]
                    {
                        new WaveConditionsOutput(1, 2, 3, 4, 5, 0.6, 0.7, 0.8, 0.9, CalculationConvergence.NotCalculated),
                        new WaveConditionsOutput(0, 1, 2, 3, 4, 0.5, 0.6, 0.7, 0.8, CalculationConvergence.NotCalculated)
                    }, new[]
                    {
                        new WaveConditionsOutput(10, 9, 8, 7, 6, 0.5, 0.4, 0.3, 0.2, CalculationConvergence.NotCalculated),
                        new WaveConditionsOutput(9, 8, 7, 6, 5, 0.4, 0.3, 0.2, 0.1, CalculationConvergence.NotCalculated)
                    })
                });
        }

        private static void SetSectionResults(IEnumerable<StabilityStoneCoverFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = GetAssessmentLayerTwoAResult();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        #endregion

        #region WaveImpactAsphaltCover FailureMechanism

        private static void ConfigureWaveImpactAsphaltCoverFailureMechanism(WaveImpactAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "WIAC A",
                Children =
                {
                    new WaveImpactAsphaltCoverWaveConditionsCalculation
                    {
                        Name = "Calculation 1",
                        Comments =
                        {
                            Body = "Comments for Calculation 1"
                        },
                        InputParameters =
                        {
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0],
                            BreakWater =
                            {
                                Height = (RoundedDouble) (foreshoreProfile.BreakWater.Height + 0.3),
                                Type = BreakWaterType.Wall
                            },
                            Orientation = foreshoreProfile.Orientation,
                            UseForeshore = true,
                            UseBreakWater = true,
                            UpperBoundaryRevetment = (RoundedDouble) 22.3,
                            LowerBoundaryRevetment = (RoundedDouble) (-3.2),
                            UpperBoundaryWaterLevels = (RoundedDouble) 15.3,
                            LowerBoundaryWaterLevels = (RoundedDouble) (-2.4),
                            StepSize = WaveConditionsInputStepSize.Two
                        }
                    }
                }
            });
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "WIAC B"
            });
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(
                new WaveImpactAsphaltCoverWaveConditionsCalculation
                {
                    Name = "Calculation 2",
                    Comments =
                    {
                        Body = "Comments for Calculation 2"
                    },
                    InputParameters =
                    {
                        ForeshoreProfile = null,
                        HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0],
                        BreakWater =
                        {
                            Height = (RoundedDouble) (foreshoreProfile.BreakWater.Height + 0.1),
                            Type = BreakWaterType.Dam
                        },
                        Orientation = foreshoreProfile.Orientation,
                        UseForeshore = false,
                        UseBreakWater = false,
                        UpperBoundaryRevetment = (RoundedDouble) 12.3,
                        LowerBoundaryRevetment = (RoundedDouble) (-3.5),
                        UpperBoundaryWaterLevels = (RoundedDouble) 13.3,
                        LowerBoundaryWaterLevels = (RoundedDouble) (-1.9),
                        StepSize = WaveConditionsInputStepSize.One
                    },
                    Output = new WaveImpactAsphaltCoverWaveConditionsOutput(new[]
                    {
                        new WaveConditionsOutput(1, 2, 3, 4, 5, 0.6, 0.7, 0.8, 0.9, CalculationConvergence.NotCalculated),
                        new WaveConditionsOutput(0, 1, 2, 3, 4, 0.5, 0.6, 0.7, 0.8, CalculationConvergence.NotCalculated)
                    })
                });
        }

        private static void SetSectionResults(IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = GetAssessmentLayerOneState();
                sectionResult.AssessmentLayerTwoA = GetAssessmentLayerTwoAResult();
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        #endregion
    }
}