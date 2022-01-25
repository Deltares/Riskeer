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
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using Riskeer.AssemblyTool.Data;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Primitives;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Data.TestUtil;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Storage.Core.TestUtil
{
    /// <summary>
    /// This class can be used to create <see cref="RiskeerProject"/> instances which have their properties set and can be used
    /// in tests.
    /// </summary>
    public static class RiskeerProjectTestHelper
    {
        /// <summary>
        /// Returns a new complete instance of <see cref="RiskeerProject"/>.
        /// </summary>
        /// <returns>A new complete instance of <see cref="RiskeerProject"/>.</returns>
        public static RiskeerProject GetFullTestProject()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = "assessmentSection",
                HydraulicBoundaryDatabase =
                {
                    FilePath = "/temp/test",
                    Version = "1.0"
                },
                Id = "12-2",
                FailureMechanismContribution =
                {
                    LowerLimitNorm = 1.0 / 10,
                    SignalingNorm = 1.0 / 1000000,
                    NormativeNorm = NormType.Signaling
                }
            };
            SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(2, 3),
                new Point2D(5, 4),
                new Point2D(5, 8),
                new Point2D(-3, 2)
            });

            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            hydraulicBoundaryLocations.AddRange(GetHydraulicBoundaryLocations());

            var random = new Random(21);
            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.AddRange(new[]
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1))
            });

            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.AddRange(new[]
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1))
            });

            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            ConfigureHydraulicBoundaryLocationCalculations(assessmentSection);

            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            ConfigureMacroStabilityInwardsFailureMechanism(macroStabilityInwardsFailureMechanism, assessmentSection);
            SetSections(macroStabilityInwardsFailureMechanism);
            SetSectionResults(macroStabilityInwardsFailureMechanism.SectionResults);

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.Piping;
            ConfigurePipingFailureMechanism(pipingFailureMechanism, assessmentSection);
            SetSections(pipingFailureMechanism);
            SetSectionResults(pipingFailureMechanism.SectionResults);
            SetSectionConfigurations(pipingFailureMechanism.ScenarioConfigurationsPerFailureMechanismSection);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            ConfigureGrassCoverErosionInwardsFailureMechanism(grassCoverErosionInwardsFailureMechanism, assessmentSection);
            SetSections(grassCoverErosionInwardsFailureMechanism);
            SetSectionResults(grassCoverErosionInwardsFailureMechanism.SectionResults);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            AddForeshoreProfiles(grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);
            ConfigureGrassCoverErosionOutwardsFailureMechanism(grassCoverErosionOutwardsFailureMechanism, assessmentSection);
            SetSections(grassCoverErosionOutwardsFailureMechanism);
            SetSectionResults(grassCoverErosionOutwardsFailureMechanism.SectionResults);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            AddForeshoreProfiles(stabilityStoneCoverFailureMechanism.ForeshoreProfiles);
            ConfigureStabilityStoneCoverFailureMechanism(stabilityStoneCoverFailureMechanism, assessmentSection);
            SetSections(stabilityStoneCoverFailureMechanism);
            SetSectionResults(stabilityStoneCoverFailureMechanism.SectionResults);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            AddForeshoreProfiles(waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);
            ConfigureWaveImpactAsphaltCoverFailureMechanism(waveImpactAsphaltCoverFailureMechanism, assessmentSection);
            SetSections(waveImpactAsphaltCoverFailureMechanism);
            SetSectionResults(waveImpactAsphaltCoverFailureMechanism.SectionResults);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            AddForeshoreProfiles(heightStructuresFailureMechanism.ForeshoreProfiles);
            ConfigureHeightStructuresFailureMechanism(heightStructuresFailureMechanism, assessmentSection);
            SetSections(heightStructuresFailureMechanism);
            SetSectionResults(heightStructuresFailureMechanism.SectionResults);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            AddForeshoreProfiles(closingStructuresFailureMechanism.ForeshoreProfiles);
            ConfigureClosingStructuresFailureMechanism(closingStructuresFailureMechanism, assessmentSection);
            SetSections(closingStructuresFailureMechanism);
            SetSectionResults(closingStructuresFailureMechanism.SectionResults);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            ConfigureDuneErosionFailureMechanism(duneErosionFailureMechanism);
            SetSections(duneErosionFailureMechanism);
            SetSectionResults(duneErosionFailureMechanism.SectionResults);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            AddForeshoreProfiles(stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            ConfigureStabilityPointStructuresFailureMechanism(stabilityPointStructuresFailureMechanism,
                                                              assessmentSection);
            SetSections(stabilityPointStructuresFailureMechanism);
            SetSectionResults(stabilityPointStructuresFailureMechanism.SectionResults);

            MacroStabilityOutwardsFailureMechanism macroStabilityOutwardsFailureMechanism = assessmentSection.MacroStabilityOutwards;
            ConfigureMacroStabilityOutwardsFailureMechanism(macroStabilityOutwardsFailureMechanism);
            SetSections(macroStabilityOutwardsFailureMechanism);
            SetSectionResults(macroStabilityOutwardsFailureMechanism.SectionResultsOld);

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            ConfigurePipingStructureFailureMechanism(pipingStructureFailureMechanism);
            SetSections(pipingStructureFailureMechanism);
            SetSectionResults(pipingStructureFailureMechanism.SectionResults);

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            SetGeneralInput(microstabilityFailureMechanism, random.Next());
            SetSections(microstabilityFailureMechanism);
            SetSectionResults(microstabilityFailureMechanism.SectionResultsOld);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            SetGeneralInput(waterPressureAsphaltCoverFailureMechanism, random.Next());
            SetSections(waterPressureAsphaltCoverFailureMechanism);
            SetSectionResults(waterPressureAsphaltCoverFailureMechanism.SectionResultsOld);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            SetGeneralInput(grassCoverSlipOffInwardsFailureMechanism, random.Next());
            SetSections(grassCoverSlipOffInwardsFailureMechanism);
            SetSectionResults(grassCoverSlipOffInwardsFailureMechanism.SectionResultsOld);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            SetGeneralInput(grassCoverSlipOffOutwardsFailureMechanism, random.Next());
            SetSections(grassCoverSlipOffOutwardsFailureMechanism);
            SetSectionResults(grassCoverSlipOffOutwardsFailureMechanism.SectionResultsOld);

            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            SetGeneralInput(strengthStabilityLengthwiseConstructionFailureMechanism, random.Next());
            SetSections(strengthStabilityLengthwiseConstructionFailureMechanism);
            SetSectionResults(strengthStabilityLengthwiseConstructionFailureMechanism.SectionResultsOld);

            SetSectionResults(assessmentSection.DuneErosion.SectionResultsOld);

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            SetGeneralInput(technicalInnovationFailureMechanism, random.Next());
            SetSections(technicalInnovationFailureMechanism);
            SetSectionResults(technicalInnovationFailureMechanism.SectionResultsOld);

            var i = 0;
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm =>
            {
                SetComments(fm);
                SetFailurePathAssemblyResults(fm, i++);
            });

            IEnumerable<SpecificFailurePath> failurePaths = Enumerable.Repeat(new SpecificFailurePath(), random.Next(1, 10))
                                                                      .ToArray();
            SetSpecificFailurePaths(failurePaths);
            assessmentSection.SpecificFailurePaths.AddRange(failurePaths);
            assessmentSection.SpecificFailurePaths.ForEach(SetComments);

            var fullTestProject = new RiskeerProject(assessmentSection)
            {
                Description = "description"
            };
            return fullTestProject;
        }

        private static void SetGeneralInput(IHasGeneralInput failureMechanism, int seed)
        {
            var random = new Random(seed);
            failureMechanism.GeneralInput.N = random.NextRoundedDouble(1, 20);
        }

        private static void SetHydraulicBoundaryLocationConfigurationSettings(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.SetValues("some\\Path\\ToHlcd",
                                                                                       "ScenarioName",
                                                                                       1337,
                                                                                       "Scope",
                                                                                       false,
                                                                                       "SeaLevel",
                                                                                       "RiverDischarge",
                                                                                       "LakeLevel",
                                                                                       "WindDirection",
                                                                                       "WindSpeed",
                                                                                       "Comment");
        }

        private static void SetSectionResults(IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultOld> sectionResults)
        {
            var random = new Random(39);
            foreach (StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultOld sectionResult in sectionResults)
            {
                sectionResult.SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
                sectionResult.TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();
                sectionResult.UseManualAssembly = random.NextBoolean();
                sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();
            }
        }

        private static void SetSectionResults(IEnumerable<TechnicalInnovationFailureMechanismSectionResultOld> sectionResults)
        {
            var random = new Random(39);
            foreach (TechnicalInnovationFailureMechanismSectionResultOld sectionResult in sectionResults)
            {
                sectionResult.SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
                sectionResult.TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();
                sectionResult.UseManualAssembly = random.NextBoolean();
                sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();
            }
        }

        private static void SetSectionResults(IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResultOld> sectionResults)
        {
            var random = new Random(39);
            foreach (WaterPressureAsphaltCoverFailureMechanismSectionResultOld sectionResult in sectionResults)
            {
                sectionResult.SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
                sectionResult.TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();
                sectionResult.UseManualAssembly = random.NextBoolean();
                sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();
            }
        }

        private static void SetSectionResults(IEnumerable<MacroStabilityOutwardsFailureMechanismSectionResultOld> sectionResults)
        {
            var random = new Random(21);
            foreach (MacroStabilityOutwardsFailureMechanismSectionResultOld sectionResult in sectionResults)
            {
                sectionResult.SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
                sectionResult.DetailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
                sectionResult.DetailedAssessmentProbability = random.NextDouble();
                sectionResult.TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>();
                sectionResult.TailorMadeAssessmentProbability = random.NextDouble();
                sectionResult.UseManualAssembly = random.NextBoolean();
                sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();
            }
        }

        private static void SetSectionResults(IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResultOld> sectionResults)
        {
            var random = new Random(39);
            foreach (GrassCoverSlipOffInwardsFailureMechanismSectionResultOld sectionResult in sectionResults)
            {
                sectionResult.SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
                sectionResult.DetailedAssessmentResult = random.NextEnumValue<DetailedAssessmentResultType>();
                sectionResult.TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();
                sectionResult.UseManualAssembly = random.NextBoolean();
                sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();
            }
        }

        private static void SetSectionResults(IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld> sectionResults)
        {
            var random = new Random(39);
            foreach (GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld sectionResult in sectionResults)
            {
                sectionResult.SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
                sectionResult.DetailedAssessmentResult = random.NextEnumValue<DetailedAssessmentResultType>();
                sectionResult.TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();
                sectionResult.UseManualAssembly = random.NextBoolean();
                sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();
            }
        }

        private static void SetSectionResults(IEnumerable<MicrostabilityFailureMechanismSectionResultOld> sectionResults)
        {
            var random = new Random(39);
            foreach (MicrostabilityFailureMechanismSectionResultOld sectionResult in sectionResults)
            {
                sectionResult.SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
                sectionResult.DetailedAssessmentResult = random.NextEnumValue<DetailedAssessmentResultType>();
                sectionResult.TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();
                sectionResult.UseManualAssembly = random.NextBoolean();
                sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();
            }
        }

        private static void SetSections(IFailurePath failurePath)
        {
            failurePath.SetSections(new[]
            {
                new FailureMechanismSection("section 1", new[]
                {
                    new Point2D(0, 2),
                    new Point2D(2, 3)
                }),
                new FailureMechanismSection("section 2", new[]
                {
                    new Point2D(2, 3),
                    new Point2D(4, 5)
                }),
                new FailureMechanismSection("section 3", new[]
                {
                    new Point2D(4, 5),
                    new Point2D(2, 3)
                })
            }, "failureMechanismSections/File/Path");
        }

        private static void AddForeshoreProfiles(ForeshoreProfileCollection foreshoreProfiles)
        {
            var foreshoreProfile1 = new ForeshoreProfile(
                new Point2D(2, 5), new[]
                {
                    new Point2D(1, 6),
                    new Point2D(8, 5)
                }, new BreakWater(BreakWaterType.Caisson, 2.5),
                new ForeshoreProfile.ConstructionProperties
                {
                    Id = "fpid",
                    Name = "FP",
                    Orientation = 95.5,
                    X0 = 22.1
                });
            var foreshoreProfile2 = new ForeshoreProfile(
                new Point2D(2, 5), Enumerable.Empty<Point2D>(), null,
                new ForeshoreProfile.ConstructionProperties
                {
                    Id = "fpid2"
                });

            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile1,
                foreshoreProfile2
            }, "some/path/to/foreshoreprofile");
        }

        private static IEnumerable<HydraulicBoundaryLocation> GetHydraulicBoundaryLocations()
        {
            yield return new HydraulicBoundaryLocation(13001, "test", 152.3, 2938.5);
            yield return new HydraulicBoundaryLocation(13002, "test2", 135.2, 5293.8);
            yield return new HydraulicBoundaryLocation(13003, "test3", 132.5, 5293.8);
        }

        private static void ConfigureHydraulicBoundaryLocationCalculations(AssessmentSection assessmentSection)
        {
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            HydraulicBoundaryLocation hydraulicLocationWithoutIllustrationPoints = hydraulicBoundaryLocations.ElementAt(0);
            ConfigureCalculationsWithOutput(assessmentSection, hydraulicLocationWithoutIllustrationPoints);

            HydraulicBoundaryLocation hydraulicLocationWithIllustrationPoints = hydraulicBoundaryLocations.ElementAt(1);
            ConfigureCalculationsWithOutput(assessmentSection, hydraulicLocationWithIllustrationPoints);
        }

        private static void ConfigureCalculationsWithOutput(AssessmentSection assessmentSection,
                                                            HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var random = new Random(21);
            HydraulicBoundaryLocationCalculation signalingNormCalculation = assessmentSection.WaterLevelCalculationsForSignalingNorm
                                                                                             .Single(calc => ReferenceEquals(calc.HydraulicBoundaryLocation, hydraulicBoundaryLocation));
            ConfigureDesignWaterLevelCalculation(signalingNormCalculation, random.NextBoolean());

            HydraulicBoundaryLocationCalculation lowerLimitNormCalculation = assessmentSection.WaterLevelCalculationsForLowerLimitNorm
                                                                                              .Single(calc => ReferenceEquals(calc.HydraulicBoundaryLocation, hydraulicBoundaryLocation));
            ConfigureDesignWaterLevelCalculation(lowerLimitNormCalculation, random.NextBoolean());

            foreach (HydraulicBoundaryLocationCalculation calculation in assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                                          .Select(c => c.HydraulicBoundaryLocationCalculations.Single(calc => ReferenceEquals(calc.HydraulicBoundaryLocation, hydraulicBoundaryLocation))))
            {
                ConfigureDesignWaterLevelCalculation(calculation, random.NextBoolean());
            }

            foreach (HydraulicBoundaryLocationCalculation calculation in assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                                                          .Select(c => c.HydraulicBoundaryLocationCalculations.Single(calc => ReferenceEquals(calc.HydraulicBoundaryLocation, hydraulicBoundaryLocation))))
            {
                ConfigureWaveHeightCalculation(calculation, random.NextBoolean());
            }
        }

        private static void ConfigureDesignWaterLevelCalculation(HydraulicBoundaryLocationCalculation designWaterLevelCalculation,
                                                                 bool hasIllustrationPoints)
        {
            designWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated = hasIllustrationPoints;
            designWaterLevelCalculation.Output = GetDesignWaterLevelOutput(hasIllustrationPoints);
        }

        private static void ConfigureWaveHeightCalculation(HydraulicBoundaryLocationCalculation waveHeightCalculation,
                                                           bool hasIllustrationPoints)
        {
            waveHeightCalculation.InputParameters.ShouldIllustrationPointsBeCalculated = hasIllustrationPoints;
            waveHeightCalculation.Output = GetWaveHeightOutput(hasIllustrationPoints);
        }

        private static HydraulicBoundaryLocationCalculationOutput GetWaveHeightOutput(bool hasIllustrationPoints)
        {
            GeneralResult<TopLevelSubMechanismIllustrationPoint> illustrationPoints = hasIllustrationPoints
                                                                                          ? GetConfiguredGeneralResultTopLevelSubMechanismIllustrationPoint()
                                                                                          : null;

            return new HydraulicBoundaryLocationCalculationOutput(2.4, 0, 0, 0, 0, CalculationConvergence.CalculatedNotConverged, illustrationPoints);
        }

        private static HydraulicBoundaryLocationCalculationOutput GetDesignWaterLevelOutput(bool hasIllustrationPoints)
        {
            GeneralResult<TopLevelSubMechanismIllustrationPoint> illustrationPoints = hasIllustrationPoints
                                                                                          ? GetConfiguredGeneralResultTopLevelSubMechanismIllustrationPoint()
                                                                                          : null;

            return new HydraulicBoundaryLocationCalculationOutput(12.4, double.NaN,
                                                                  double.NaN, double.NaN,
                                                                  double.NaN, CalculationConvergence.CalculatedConverged, illustrationPoints);
        }

        private static GeneralResult<TopLevelSubMechanismIllustrationPoint> GetConfiguredGeneralResultTopLevelSubMechanismIllustrationPoint()
        {
            var illustrationPointResult = new IllustrationPointResult("Description of result", "-", 5);
            var subMechanismIllustrationPointStochast = new SubMechanismIllustrationPointStochast("Name of a stochast", "-", 10, 9, 8);

            var illustrationPoint = new SubMechanismIllustrationPoint("Name of illustrationPoint", 3, new[]
            {
                subMechanismIllustrationPointStochast
            }, new[]
            {
                illustrationPointResult
            });

            var windDirection = new WindDirection("60", 60);
            var topLevelIllustrationPoint = new TopLevelSubMechanismIllustrationPoint(windDirection,
                                                                                      "Closing situation",
                                                                                      illustrationPoint);

            var governingWindDirection = new WindDirection("SSE", 120);
            var stochast = new Stochast("Name of a stochast", 13, 37);
            return new GeneralResult<TopLevelSubMechanismIllustrationPoint>(governingWindDirection,
                                                                            new[]
                                                                            {
                                                                                stochast
                                                                            },
                                                                            new[]
                                                                            {
                                                                                topLevelIllustrationPoint
                                                                            });
        }

        private static StructuresOutput GetStructuresOutputWithIllustrationPoints()
        {
            var random = new Random(56);
            var output = new StructuresOutput(
                random.NextDouble(),
                GetConfiguredGeneralResultFaultTreeIllustrationPoint());
            return output;
        }

        private static GeneralResult<TopLevelFaultTreeIllustrationPoint> GetConfiguredGeneralResultFaultTreeIllustrationPoint()
        {
            return new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                new WindDirection("GoverningWindDirection", 180),
                new[]
                {
                    new Stochast("Stochast", 0.1, 0.9)
                },
                new[]
                {
                    new TopLevelFaultTreeIllustrationPoint(
                        new WindDirection("WindDirection", 120),
                        "ClosingSituation",
                        new IllustrationPointNode(
                            new FaultTreeIllustrationPoint(
                                "FaultTreeIllustrationPoint",
                                0.5,
                                new[]
                                {
                                    new Stochast("Stochast", 0.1, 0.9)
                                }, CombinationType.And
                            ))
                    )
                }
            );
        }

        private static void SetComments(IFailurePath failurePath)
        {
            failurePath.InAssemblyInputComments.Body = $"Input comment {failurePath.Name}";
            failurePath.InAssemblyOutputComments.Body = $"Output comment {failurePath.Name}";
            failurePath.NotInAssemblyComments.Body = $"Not in assembly comment {failurePath.Name}";
        }

        private static void SetComments(IFailureMechanism failureMechanism)
        {
            SetComments((IFailurePath) failureMechanism);
            failureMechanism.CalculationsInputComments.Body = $"Calculations input comment: {failureMechanism.Name}";
        }

        private static void SetFailurePathAssemblyResults(IFailurePath failurePath, int seed)
        {
            var random = new Random(seed);
            FailurePathAssemblyResult assemblyResult = failurePath.AssemblyResult;
            assemblyResult.ProbabilityResultType = random.NextEnumValue<FailurePathAssemblyProbabilityResultType>();
            assemblyResult.ManualFailurePathAssemblyProbability = random.NextDouble();
        }

        private static void SetSectionResults(IEnumerable<AdoptableFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (AdoptableFailureMechanismSectionResult sectionResult in sectionResults)
            {
                sectionResult.IsRelevant = random.NextBoolean();
                sectionResult.InitialFailureMechanismResultType = random.NextEnumValue<AdoptableInitialFailureMechanismResultType>();
                sectionResult.ManualInitialFailureMechanismResultSectionProbability = random.NextDouble();
                sectionResult.FurtherAnalysisNeeded = random.NextBoolean();
                sectionResult.RefinedSectionProbability = random.NextDouble();
            }
        }
        
        private static void SetSectionResults(IEnumerable<AdoptableWithProfileProbabilityFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);

            foreach (AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult in sectionResults)
            {
                sectionResult.IsRelevant = random.NextBoolean();
                sectionResult.InitialFailureMechanismResultType = random.NextEnumValue<AdoptableInitialFailureMechanismResultType>();
                sectionResult.ManualInitialFailureMechanismResultProfileProbability = random.NextDouble();
                sectionResult.ManualInitialFailureMechanismResultSectionProbability = random.NextDouble();
                sectionResult.FurtherAnalysisNeeded = random.NextBoolean();
                sectionResult.ProbabilityRefinementType = random.NextEnumValue<ProbabilityRefinementType>();
                sectionResult.RefinedProfileProbability = random.NextDouble();
                sectionResult.RefinedSectionProbability = random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<NonAdoptableFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (NonAdoptableFailureMechanismSectionResult sectionResult in sectionResults)
            {
                sectionResult.IsRelevant = random.NextBoolean();
                sectionResult.InitialFailureMechanismResultType = random.NextEnumValue<NonAdoptableInitialFailureMechanismResultType>();
                sectionResult.ManualInitialFailureMechanismResultSectionProbability = random.NextDouble();
                sectionResult.FurtherAnalysisNeeded = random.NextBoolean();
                sectionResult.RefinedSectionProbability = random.NextDouble();
            }
        }
        
        private static void SetSectionResults(IEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);

            foreach (NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult in sectionResults)
            {
                sectionResult.IsRelevant = random.NextBoolean();
                sectionResult.InitialFailureMechanismResultType = random.NextEnumValue<NonAdoptableInitialFailureMechanismResultType>();
                sectionResult.ManualInitialFailureMechanismResultProfileProbability = random.NextDouble();
                sectionResult.ManualInitialFailureMechanismResultSectionProbability = random.NextDouble();
                sectionResult.FurtherAnalysisNeeded = random.NextBoolean();
                sectionResult.RefinedProfileProbability = random.NextDouble();
                sectionResult.RefinedSectionProbability = random.NextDouble();
            }
        }

        #region MacroStabilityOutwards FailureMechanism

        private static void ConfigureMacroStabilityOutwardsFailureMechanism(MacroStabilityOutwardsFailureMechanism macroStabilityOutwardsFailureMechanism)
        {
            macroStabilityOutwardsFailureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.A = 0.6;
        }

        #endregion

        #region PipingStructure FailureMechanism

        private static void ConfigurePipingStructureFailureMechanism(PipingStructureFailureMechanism pipingStructureFailureMechanism)
        {
            pipingStructureFailureMechanism.GeneralInput.N = (RoundedDouble) 12.5;
        }

        #endregion

        #region Specific FailurePath

        private static void SetSpecificFailurePaths(IEnumerable<SpecificFailurePath> specificFailurePaths)
        {
            var i = 0;
            foreach (SpecificFailurePath failurePath in specificFailurePaths)
            {
                var random = new Random(i);
                failurePath.Input.N = random.NextRoundedDouble(1, 20);

                failurePath.Name = $"Path {i}";
                failurePath.InAssembly = random.NextBoolean();
                failurePath.InAssemblyInputComments.Body = $"Input comment path: {i}";
                failurePath.InAssemblyOutputComments.Body = $"Output comment path: {i}";
                failurePath.NotInAssemblyComments.Body = $"NotInAssembly comment path: {i}";

                SetSections(failurePath);
                SetFailurePathAssemblyResults(failurePath, i);
                i++;
            }
        }

        #endregion

        #region StabilityPointStructures FailureMechanism

        private static void ConfigureStabilityPointStructuresFailureMechanism(StabilityPointStructuresFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N = (RoundedDouble) 8.0;

            StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure("id structure1");
            failureMechanism.StabilityPointStructures.AddRange(new[]
            {
                stabilityPointStructure,
                new TestStabilityPointStructure("id structure2")
            }, "path");

            var random = new Random(56);

            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "StabilityPoint structures A",
                Children =
                {
                    new StructuresCalculationScenario<StabilityPointStructuresInput>
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
                                Height = random.NextRoundedDouble()
                            },
                            DrainCoefficient =
                            {
                                Mean = random.NextRoundedDouble()
                            },
                            FactorStormDurationOpenStructure = random.NextRoundedDouble(),
                            FailureProbabilityStructureWithErosion = random.NextDouble(),
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                            LoadSchematizationType = LoadSchematizationType.Quadratic,
                            VolumicWeightWater = random.NextRoundedDouble(),
                            UseForeshore = random.NextBoolean(),
                            UseBreakWater = random.NextBoolean(),
                            StormDuration =
                            {
                                Mean = random.NextRoundedDouble(0.1, 1.0)
                            },
                            Structure = stabilityPointStructure,
                            ShouldIllustrationPointsBeCalculated = false
                        },
                        Output = new StructuresOutput(0.11, null)
                    },
                    new StructuresCalculationScenario<StabilityPointStructuresInput>
                    {
                        Name = "Calculation 2",
                        Comments =
                        {
                            Body = "Fully configured for with general result"
                        },
                        InputParameters =
                        {
                            BreakWater =
                            {
                                Type = BreakWaterType.Dam,
                                Height = random.NextRoundedDouble()
                            },
                            DrainCoefficient =
                            {
                                Mean = random.NextRoundedDouble()
                            },
                            FactorStormDurationOpenStructure = random.NextRoundedDouble(),
                            FailureProbabilityStructureWithErosion = random.NextDouble(),
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                            LoadSchematizationType = LoadSchematizationType.Quadratic,
                            VolumicWeightWater = random.NextRoundedDouble(),
                            UseForeshore = random.NextBoolean(),
                            UseBreakWater = random.NextBoolean(),
                            StormDuration =
                            {
                                Mean = random.NextRoundedDouble(0.1, 1.0)
                            },
                            Structure = stabilityPointStructure,
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = GetStructuresOutputWithIllustrationPoints()
                    }
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "StabilityPoint structures B"
            });
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculationScenario<StabilityPointStructuresInput>());
        }

        #endregion

        #region ClosingStructures FailureMechanism

        private static void ConfigureClosingStructuresFailureMechanism(ClosingStructuresFailureMechanism failureMechanism,
                                                                       IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N2A = 6;

            ClosingStructure closingStructure = new TestClosingStructure("structureA");
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                closingStructure,
                new TestClosingStructure("structureB")
            }, @"C:\Folder");

            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "Closing structures A",
                Children =
                {
                    new StructuresCalculationScenario<ClosingStructuresInput>
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
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
                            UseForeshore = true,
                            ShouldIllustrationPointsBeCalculated = false
                        },
                        Output = new StructuresOutput(0.11, null)
                    },
                    new StructuresCalculationScenario<ClosingStructuresInput>
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
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
                            UseForeshore = true,
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = GetStructuresOutputWithIllustrationPoints()
                    }
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "Closing structures B"
            });
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculationScenario<ClosingStructuresInput>());
        }

        #endregion

        #region DuneErosion FailureMechanism

        private static void ConfigureDuneErosionFailureMechanism(DuneErosionFailureMechanism failureMechanism)
        {
            failureMechanism.GeneralInput.N = (RoundedDouble) 5.5;

            var random = new Random(21);
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.AddRange(new[]
            {
                new DuneLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                new DuneLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                new DuneLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1))
            });

            SetDuneLocations(failureMechanism);
        }

        private static void SetSectionResults(IEnumerable<DuneErosionFailureMechanismSectionResultOld> sectionResults)
        {
            var random = new Random(42);
            foreach (DuneErosionFailureMechanismSectionResultOld sectionResult in sectionResults)
            {
                sectionResult.SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>();
                sectionResult.DetailedAssessmentResultForFactorizedSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
                sectionResult.DetailedAssessmentResultForSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
                sectionResult.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
                sectionResult.DetailedAssessmentResultForLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
                sectionResult.DetailedAssessmentResultForFactorizedLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
                sectionResult.TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>();
                sectionResult.UseManualAssembly = random.NextBoolean();
                sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            }
        }

        private static void SetDuneLocations(DuneErosionFailureMechanism failureMechanism)
        {
            var locationOne = new DuneLocation(12, "DuneLocation", new Point2D(790, 456),
                                               new DuneLocation.ConstructionProperties());
            var locationTwo = new DuneLocation(13, "DuneLocation", new Point2D(791, 457),
                                               new DuneLocation.ConstructionProperties());
            failureMechanism.SetDuneLocations(new[]
            {
                locationOne,
                locationTwo
            });

            ConfigureDuneLocationCalculations(failureMechanism);
        }

        private static void ConfigureDuneLocationCalculations(DuneErosionFailureMechanism failureMechanism)
        {
            var random = new Random(21);
            IEnumerable<DuneLocationCalculation> calculations = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities
                                                                                .SelectMany(targetProbability => targetProbability.DuneLocationCalculations);
            foreach (DuneLocationCalculation calculation in calculations)
            {
                if (random.NextBoolean())
                {
                    SetCalculationOutput(calculation);
                }
            }
        }

        private static void SetCalculationOutput(DuneLocationCalculation calculation)
        {
            calculation.Output = new DuneLocationCalculationOutput(CalculationConvergence.CalculatedConverged,
                                                                   new DuneLocationCalculationOutput.ConstructionProperties
                                                                   {
                                                                       WaterLevel = 10,
                                                                       WaveHeight = 20,
                                                                       WavePeriod = 30,
                                                                       TargetProbability = 0.4,
                                                                       TargetReliability = 50,
                                                                       CalculatedProbability = 0.6,
                                                                       CalculatedReliability = 70
                                                                   });
        }

        #endregion

        #region HeightStructures FailureMechanism

        private static void ConfigureHeightStructuresFailureMechanism(HeightStructuresFailureMechanism failureMechanism,
                                                                      IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N = (RoundedDouble) 5.0;

            List<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;

            var heightStructure = new TestHeightStructure();
            failureMechanism.HeightStructures.AddRange(new[]
            {
                heightStructure,
                new TestHeightStructure("IdB", "Structure B")
            }, @"/temp/structures");

            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "Height structures A",
                Children =
                {
                    new StructuresCalculationScenario<HeightStructuresInput>
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
                            Structure = heightStructure,
                            ShouldIllustrationPointsBeCalculated = false
                        },
                        Output = new StructuresOutput(0.11, null)
                    },
                    new StructuresCalculationScenario<HeightStructuresInput>
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
                            Structure = heightStructure,
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = GetStructuresOutputWithIllustrationPoints()
                    }
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "Height structures B"
            });
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculationScenario<HeightStructuresInput>());
        }
        
        #endregion

        #region Piping FailureMechanism

        private static void ConfigurePipingFailureMechanism(PipingFailureMechanism pipingFailureMechanism, AssessmentSection assessmentSection)
        {
            pipingFailureMechanism.PipingProbabilityAssessmentInput.A = 0.9;

            Point2D[] referenceLineGeometryPoints = assessmentSection.ReferenceLine.Points.ToArray();

            var pipingSoilProfile = new PipingSoilProfile("SoilProfile1D", 0.0, new[]
            {
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = true,
                    BelowPhreaticLevel = new LogNormalDistribution
                    {
                        Mean = (RoundedDouble) 3.2,
                        StandardDeviation = (RoundedDouble) 1.2,
                        Shift = (RoundedDouble) 2.2
                    },
                    DiameterD70 = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 2.42,
                        CoefficientOfVariation = (RoundedDouble) 21.002
                    },
                    Permeability = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.9982,
                        CoefficientOfVariation = (RoundedDouble) 0.220
                    },
                    Color = Color.HotPink,
                    MaterialName = "HotPinkLayer"
                },
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false,
                    Color = Color.Empty,
                    MaterialName = "EmptyLayer"
                },
                new PipingSoilLayer(3.0)
                {
                    IsAquifer = false,
                    Color = Color.FromArgb(0),
                    MaterialName = "ColorZeroLayer"
                }
            }, SoilProfileType.SoilProfile1D);

            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new PipingStochasticSoilModel("modelName", new[]
                {
                    referenceLineGeometryPoints[1],
                    referenceLineGeometryPoints[2],
                    referenceLineGeometryPoints[3]
                }, new[]
                {
                    new PipingStochasticSoilProfile(0.2, pipingSoilProfile),
                    new PipingStochasticSoilProfile(0.8, PipingSoilProfileTestFactory.CreatePipingSoilProfile("SoilProfile2D",
                                                                                                              SoilProfileType.SoilProfile2D))
                })
            }, "some/path/to/stochasticSoilModelFile");
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                GetSurfaceLine()
            }, "some/path/to/surfaceLineFile");

            CalculationGroup pipingCalculationGroup = pipingFailureMechanism.CalculationsGroup;
            pipingCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "A",
                Children =
                {
                    new SemiProbabilisticPipingCalculationScenario
                    {
                        Name = "Semi-probabilistic with HydraulicBoundaryLocation",
                        IsRelevant = true,
                        Contribution = (RoundedDouble) 1.0,
                        Comments =
                        {
                            Body = "Calculation with hydraulic boundary location and output"
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
                            }
                        },
                        Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
                    },
                    new SemiProbabilisticPipingCalculationScenario
                    {
                        Name = "Semi-probabilistic with manual input",
                        IsRelevant = true,
                        Contribution = (RoundedDouble) 1.0,
                        Comments =
                        {
                            Body = "Calculation with manual assessment level and output"
                        },
                        InputParameters =
                        {
                            SurfaceLine = pipingFailureMechanism.SurfaceLines.First(),
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
                            UseAssessmentLevelManualInput = true,
                            AssessmentLevel = (RoundedDouble) 6.0
                        },
                        Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
                    },
                    new ProbabilisticPipingCalculationScenario
                    {
                        Name = "Probabilistic with HydraulicBoundaryLocation",
                        IsRelevant = true,
                        Contribution = (RoundedDouble) 1.0,
                        Comments =
                        {
                            Body = "Calculation with hydraulic boundary location and output"
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
                            ShouldProfileSpecificIllustrationPointsBeCalculated = true,
                            ShouldSectionSpecificIllustrationPointsBeCalculated = true
                        },
                        Output = new ProbabilisticPipingOutput(PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(),
                                                               PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput())
                    }
                }
            });
            pipingCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "B"
            });
            pipingCalculationGroup.Children.Add(new SemiProbabilisticPipingCalculationScenario
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
                    }
                },
                Output = null
            });
            pipingCalculationGroup.Children.Add(new ProbabilisticPipingCalculationScenario
            {
                Name = "D",
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
                    ShouldProfileSpecificIllustrationPointsBeCalculated = false,
                    ShouldSectionSpecificIllustrationPointsBeCalculated = false
                },
                Output = null
            });
        }

        private static void SetSectionConfigurations(IEnumerable<PipingScenarioConfigurationPerFailureMechanismSection> sectionConfigurations)
        {
            var random = new Random(21);
            foreach (PipingScenarioConfigurationPerFailureMechanismSection sectionConfiguration in sectionConfigurations)
            {
                sectionConfiguration.ScenarioConfigurationType = random.NextEnumValue<PipingScenarioConfigurationPerFailureMechanismSectionType>();
            }
        }

        private static PipingSurfaceLine GetSurfaceLine()
        {
            var surfaceLine = new PipingSurfaceLine("Surface line")
            {
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

        #region MacroStabilityInwards FailureMechanism

        private static void ConfigureMacroStabilityInwardsFailureMechanism(MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism,
                                                                           AssessmentSection assessmentSection)
        {
            macroStabilityInwardsFailureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A = 0.9;
            Point2D[] referenceLineGeometryPoints = assessmentSection.ReferenceLine.Points.ToArray();

            var soilLayer1D = new MacroStabilityInwardsSoilLayer1D(5)
            {
                Data =
                {
                    IsAquifer = true,
                    MaterialName = "SeaShellLayer",
                    Color = Color.SeaShell,
                    UsePop = true,
                    ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated,
                    AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.03,
                        CoefficientOfVariation = (RoundedDouble) 0.02,
                        Shift = (RoundedDouble) 0.01
                    },
                    BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.06,
                        CoefficientOfVariation = (RoundedDouble) 0.05,
                        Shift = (RoundedDouble) 0.04
                    },
                    Cohesion = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.07,
                        CoefficientOfVariation = (RoundedDouble) 0.08
                    },
                    FrictionAngle = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.09,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.11,
                        CoefficientOfVariation = (RoundedDouble) 0.12
                    },
                    StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.13,
                        CoefficientOfVariation = (RoundedDouble) 0.14
                    },
                    Pop = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.15,
                        CoefficientOfVariation = (RoundedDouble) 0.16
                    }
                }
            };

            var soilProfile1D = new MacroStabilityInwardsSoilProfile1D("MacroStabilityInwardsSoilProfile1D",
                                                                       -10,
                                                                       new[]
                                                                       {
                                                                           soilLayer1D
                                                                       });

            var soilLayer2D = new MacroStabilityInwardsSoilLayer2D(
                new Ring(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }),
                new MacroStabilityInwardsSoilLayerData(), new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(3, 3),
                        new Point2D(4, 4)
                    }))
                    {
                        Data =
                        {
                            IsAquifer = true,
                            MaterialName = "Zero",
                            Color = Color.FromArgb(0),
                            UsePop = true,
                            ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.CPhi,
                            AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
                            {
                                Mean = (RoundedDouble) 15.901,
                                CoefficientOfVariation = (RoundedDouble) 5.902,
                                Shift = (RoundedDouble) 5.903
                            },
                            BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                            {
                                Mean = (RoundedDouble) 5.906,
                                CoefficientOfVariation = (RoundedDouble) 5.905,
                                Shift = (RoundedDouble) 5.904
                            },
                            Cohesion = new VariationCoefficientLogNormalDistribution
                            {
                                Mean = (RoundedDouble) 5.907,
                                CoefficientOfVariation = (RoundedDouble) 5.908
                            },
                            FrictionAngle = new VariationCoefficientLogNormalDistribution
                            {
                                Mean = (RoundedDouble) 5.909,
                                CoefficientOfVariation = (RoundedDouble) 5.91
                            },
                            ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
                            {
                                Mean = (RoundedDouble) 5.911,
                                CoefficientOfVariation = (RoundedDouble) 5.912
                            },
                            StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
                            {
                                Mean = (RoundedDouble) 5.913,
                                CoefficientOfVariation = (RoundedDouble) 5.914
                            },
                            Pop = new VariationCoefficientLogNormalDistribution
                            {
                                Mean = (RoundedDouble) 5.915,
                                CoefficientOfVariation = (RoundedDouble) 5.916
                            }
                        }
                    },
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(-10, -100),
                        new Point2D(10, 100)
                    }))
                    {
                        Data =
                        {
                            MaterialName = "Empty",
                            Color = Color.Empty,
                            ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.SuCalculated
                        }
                    }
                })
            {
                Data =
                {
                    IsAquifer = false,
                    MaterialName = "GainsboroLayer",
                    Color = Color.Gainsboro,
                    UsePop = false,
                    ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.SuCalculated,
                    AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 10.901,
                        CoefficientOfVariation = (RoundedDouble) 0.902,
                        Shift = (RoundedDouble) 0.903
                    },
                    BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.906,
                        CoefficientOfVariation = (RoundedDouble) 0.905,
                        Shift = (RoundedDouble) 0.904
                    },
                    Cohesion = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.907,
                        CoefficientOfVariation = (RoundedDouble) 0.908
                    },
                    FrictionAngle = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.909,
                        CoefficientOfVariation = (RoundedDouble) 0.91
                    },
                    ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.911,
                        CoefficientOfVariation = (RoundedDouble) 0.912
                    },
                    StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.913,
                        CoefficientOfVariation = (RoundedDouble) 0.914
                    },
                    Pop = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.915,
                        CoefficientOfVariation = (RoundedDouble) 0.916
                    }
                }
            };

            var soilProfile2D = new MacroStabilityInwardsSoilProfile2D("MacroStabilityInwardsSoilProfile2D", new[]
            {
                soilLayer2D
            }, new[]
            {
                new MacroStabilityInwardsPreconsolidationStress(new Point2D(1, 2), new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 3,
                    CoefficientOfVariation = (RoundedDouble) 4
                })
            });

            macroStabilityInwardsFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new MacroStabilityInwardsStochasticSoilModel("MacroStabilityInwards model name", new[]
                {
                    referenceLineGeometryPoints[1],
                    referenceLineGeometryPoints[2],
                    referenceLineGeometryPoints[3]
                }, new[]
                {
                    new MacroStabilityInwardsStochasticSoilProfile(0.3, soilProfile1D),
                    new MacroStabilityInwardsStochasticSoilProfile(0.7, soilProfile2D)
                })
            }, "some/path/to/stochasticSoilModelFile");
            macroStabilityInwardsFailureMechanism.SurfaceLines.AddRange(new[]
            {
                GetMacroStabilityInwardsSurfaceLine()
            }, "some/path/to/surfaceLineFile");

            CalculationGroup macroStabilityInwardsCalculationGroup = macroStabilityInwardsFailureMechanism.CalculationsGroup;
            macroStabilityInwardsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "Calculation group with calculation",
                Children =
                {
                    new MacroStabilityInwardsCalculationScenario
                    {
                        Name = "Calculation with hydraulic boundary location and output",
                        IsRelevant = true,
                        Contribution = (RoundedDouble) 1.0,
                        Comments =
                        {
                            Body = "Nice comment about this calculation!"
                        },
                        InputParameters =
                        {
                            SurfaceLine = macroStabilityInwardsFailureMechanism.SurfaceLines.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                            StochasticSoilModel = macroStabilityInwardsFailureMechanism.StochasticSoilModels.First(),
                            StochasticSoilProfile = macroStabilityInwardsFailureMechanism.StochasticSoilModels.First()
                                                                                         .StochasticSoilProfiles.First(),
                            SlipPlaneMinimumDepth = (RoundedDouble) 0.2,
                            SlipPlaneMinimumLength = (RoundedDouble) 0.3,
                            MaximumSliceWidth = (RoundedDouble) 0.4,
                            MoveGrid = true,
                            DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand,
                            WaterLevelRiverAverage = (RoundedDouble) 0.6,
                            DrainageConstructionPresent = true,
                            XCoordinateDrainageConstruction = (RoundedDouble) 0.7,
                            ZCoordinateDrainageConstruction = (RoundedDouble) 0.8,
                            LocationInputExtreme =
                            {
                                WaterLevelPolder = (RoundedDouble) 0.9,
                                UseDefaultOffsets = false,
                                PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 1.0,
                                PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 1.1,
                                PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 1.2,
                                PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 1.3,
                                PenetrationLength = (RoundedDouble) 1.4
                            },
                            LocationInputDaily =
                            {
                                WaterLevelPolder = (RoundedDouble) 1.5,
                                UseDefaultOffsets = true,
                                PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 1.6,
                                PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 1.7,
                                PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 1.8,
                                PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 1.9
                            },
                            AdjustPhreaticLine3And4ForUplift = false,
                            LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) 2.0,
                            LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) 2.1,
                            LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) 2.2,
                            LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) 2.3,
                            PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) 2.4,
                            PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) 2.5,
                            GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual,
                            TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.Specified,
                            TangentLineZTop = (RoundedDouble) 2.7,
                            TangentLineZBottom = (RoundedDouble) 2.6,
                            TangentLineNumber = 2,
                            LeftGrid =
                            {
                                XLeft = (RoundedDouble) 2.8,
                                XRight = (RoundedDouble) 2.9,
                                NumberOfHorizontalPoints = 3,
                                ZTop = (RoundedDouble) 3.1,
                                ZBottom = (RoundedDouble) 3.0,
                                NumberOfVerticalPoints = 4
                            },
                            RightGrid =
                            {
                                XLeft = (RoundedDouble) 3.2,
                                XRight = (RoundedDouble) 3.3,
                                NumberOfHorizontalPoints = 5,
                                ZTop = (RoundedDouble) 3.5,
                                ZBottom = (RoundedDouble) 3.4,
                                NumberOfVerticalPoints = 6
                            },
                            CreateZones = false,
                            ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Manual,
                            ZoneBoundaryLeft = (RoundedDouble) 10,
                            ZoneBoundaryRight = (RoundedDouble) 12
                        },
                        Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                    },
                    new MacroStabilityInwardsCalculationScenario
                    {
                        Name = "Calculation with manual assessment level and output",
                        IsRelevant = true,
                        Contribution = (RoundedDouble) 1.0,
                        Comments =
                        {
                            Body = "Nice comment about this calculation!"
                        },
                        InputParameters =
                        {
                            SurfaceLine = macroStabilityInwardsFailureMechanism.SurfaceLines.First(),
                            StochasticSoilModel = macroStabilityInwardsFailureMechanism.StochasticSoilModels.First(),
                            StochasticSoilProfile = macroStabilityInwardsFailureMechanism.StochasticSoilModels.First()
                                                                                         .StochasticSoilProfiles.First(),
                            UseAssessmentLevelManualInput = true,
                            AssessmentLevel = (RoundedDouble) 0.1,
                            SlipPlaneMinimumDepth = (RoundedDouble) 0.2,
                            SlipPlaneMinimumLength = (RoundedDouble) 0.3,
                            MaximumSliceWidth = (RoundedDouble) 0.4,
                            MoveGrid = true,
                            DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand,
                            WaterLevelRiverAverage = (RoundedDouble) 0.6,
                            DrainageConstructionPresent = true,
                            XCoordinateDrainageConstruction = (RoundedDouble) 0.7,
                            ZCoordinateDrainageConstruction = (RoundedDouble) 0.8,
                            LocationInputExtreme =
                            {
                                WaterLevelPolder = (RoundedDouble) 0.9,
                                UseDefaultOffsets = false,
                                PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 1.0,
                                PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 1.1,
                                PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 1.2,
                                PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 1.3,
                                PenetrationLength = (RoundedDouble) 1.4
                            },
                            LocationInputDaily =
                            {
                                WaterLevelPolder = (RoundedDouble) 1.5,
                                UseDefaultOffsets = true,
                                PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 1.6,
                                PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 1.7,
                                PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 1.8,
                                PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 1.9
                            },
                            AdjustPhreaticLine3And4ForUplift = false,
                            LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) 2.0,
                            LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) 2.1,
                            LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) 2.2,
                            LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) 2.3,
                            PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) 2.4,
                            PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) 2.5,
                            GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual,
                            TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.Specified,
                            TangentLineZTop = (RoundedDouble) 2.7,
                            TangentLineZBottom = (RoundedDouble) 2.6,
                            TangentLineNumber = 2,
                            LeftGrid =
                            {
                                XLeft = (RoundedDouble) 2.8,
                                XRight = (RoundedDouble) 2.9,
                                NumberOfHorizontalPoints = 3,
                                ZTop = (RoundedDouble) 3.1,
                                ZBottom = (RoundedDouble) 3.0,
                                NumberOfVerticalPoints = 4
                            },
                            RightGrid =
                            {
                                XLeft = (RoundedDouble) 3.2,
                                XRight = (RoundedDouble) 3.3,
                                NumberOfHorizontalPoints = 5,
                                ZTop = (RoundedDouble) 3.5,
                                ZBottom = (RoundedDouble) 3.4,
                                NumberOfVerticalPoints = 6
                            },
                            CreateZones = false,
                            ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Manual,
                            ZoneBoundaryLeft = (RoundedDouble) 5.4,
                            ZoneBoundaryRight = (RoundedDouble) 6.5
                        },
                        Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                    }
                }
            });
            macroStabilityInwardsCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "Group without calculations"
            });
            macroStabilityInwardsCalculationGroup.Children.Add(new MacroStabilityInwardsCalculationScenario
            {
                Name = "Scenario without output",
                IsRelevant = false,
                Contribution = (RoundedDouble) 0.5,
                Comments =
                {
                    Body = "Another great comment"
                },
                InputParameters =
                {
                    SurfaceLine = macroStabilityInwardsFailureMechanism.SurfaceLines.First(),
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                    StochasticSoilModel = macroStabilityInwardsFailureMechanism.StochasticSoilModels.First(),
                    StochasticSoilProfile = macroStabilityInwardsFailureMechanism.StochasticSoilModels.First()
                                                                                 .StochasticSoilProfiles.Skip(1).First(),
                    SlipPlaneMinimumDepth = (RoundedDouble) 10.2,
                    SlipPlaneMinimumLength = (RoundedDouble) 10.3,
                    MaximumSliceWidth = (RoundedDouble) 10.4,
                    WaterLevelRiverAverage = (RoundedDouble) 10.6,
                    XCoordinateDrainageConstruction = (RoundedDouble) 10.7,
                    ZCoordinateDrainageConstruction = (RoundedDouble) 10.8,
                    LocationInputExtreme =
                    {
                        WaterLevelPolder = (RoundedDouble) 10.9,
                        PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 20.0,
                        PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 20.1,
                        PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 20.2,
                        PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 20.3,
                        PenetrationLength = (RoundedDouble) 20.4
                    },
                    LocationInputDaily =
                    {
                        WaterLevelPolder = (RoundedDouble) 20.5,
                        PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 20.6,
                        PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 20.7,
                        PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 20.8,
                        PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 20.9
                    },
                    LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) 40.0,
                    LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) 40.1,
                    LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) 40.2,
                    LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) 40.3,
                    PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) 40.4,
                    PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) 40.5,
                    TangentLineZTop = (RoundedDouble) 40.7,
                    TangentLineZBottom = (RoundedDouble) 40.6,
                    TangentLineNumber = 2,
                    LeftGrid =
                    {
                        XLeft = (RoundedDouble) 40.8,
                        XRight = (RoundedDouble) 40.9,
                        NumberOfHorizontalPoints = 3,
                        ZTop = (RoundedDouble) 30.1,
                        ZBottom = (RoundedDouble) 30.0,
                        NumberOfVerticalPoints = 4
                    },
                    RightGrid =
                    {
                        XLeft = (RoundedDouble) 30.2,
                        XRight = (RoundedDouble) 30.3,
                        NumberOfHorizontalPoints = 5,
                        ZTop = (RoundedDouble) 30.5,
                        ZBottom = (RoundedDouble) 30.4,
                        NumberOfVerticalPoints = 6
                    },
                    ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic,
                    ZoneBoundaryLeft = (RoundedDouble) 1,
                    ZoneBoundaryRight = (RoundedDouble) 2
                },
                Output = null
            });
        }

        private static MacroStabilityInwardsSurfaceLine GetMacroStabilityInwardsSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("MacroStabilityInwards surface line")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(4.4, 6.6)
            };

            var geometryPoints = new[]
            {
                new Point3D(1.0, 6.0, -2.3),
                new Point3D(2.8, 6.0, -2.3),
                new Point3D(3.6, 6.0, 3.4),
                new Point3D(4.2, 6.0, 3.5),
                new Point3D(5.0, 6.0, 0.5),
                new Point3D(6.8, 6.0, 0.5),
                new Point3D(7.6, 6.0, 0.2),
                new Point3D(8.4, 6.0, 0.25),
                new Point3D(9.2, 6.0, 0.5),
                new Point3D(10.0, 6.0, 0.5),
                new Point3D(11.0, 6.0, -2.3),
                new Point3D(12.8, 6.0, -2.3),
                new Point3D(13.6, 6.0, 3.4)
            };
            surfaceLine.SetGeometry(geometryPoints);

            surfaceLine.SetSurfaceLevelOutsideAt(geometryPoints[12]);
            surfaceLine.SetDikeToeAtRiverAt(geometryPoints[11]);
            surfaceLine.SetDikeTopAtPolderAt(geometryPoints[10]);
            surfaceLine.SetDikeTopAtRiverAt(geometryPoints[9]);
            surfaceLine.SetShoulderBaseInsideAt(geometryPoints[8]);
            surfaceLine.SetShoulderTopInsideAt(geometryPoints[7]);
            surfaceLine.SetDikeToeAtPolderAt(geometryPoints[6]);
            surfaceLine.SetDitchDikeSideAt(geometryPoints[5]);
            surfaceLine.SetBottomDitchDikeSideAt(geometryPoints[4]);
            surfaceLine.SetBottomDitchPolderSideAt(geometryPoints[3]);
            surfaceLine.SetDitchPolderSideAt(geometryPoints[2]);
            surfaceLine.SetSurfaceLevelInsideAt(geometryPoints[1]);

            return surfaceLine;
        }

        #endregion

        #region GrassCoverErosionInwards FailureMechanism

        private static void ConfigureGrassCoverErosionInwardsFailureMechanism(GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N = (RoundedDouble) 15.0;
            var dikeProfile1 = new DikeProfile(new Point2D(1, 2),
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
                                                   Id = "id",
                                                   DikeHeight = 1.1,
                                                   Name = "2.2",
                                                   Orientation = 3.3,
                                                   X0 = 4.4
                                               });
            var dikeProfile2 = new DikeProfile(new Point2D(9, 10),
                                               new[]
                                               {
                                                   new RoughnessPoint(new Point2D(11, 12), 1),
                                                   new RoughnessPoint(new Point2D(13, 14), 0.5)
                                               },
                                               new Point2D[0],
                                               null,
                                               new DikeProfile.ConstructionProperties
                                               {
                                                   Id = "id2",
                                                   DikeHeight = 5.5,
                                                   Name = "6.6",
                                                   Orientation = 7.7,
                                                   X0 = 8.8
                                               });
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile1,
                dikeProfile2
            }, "some/path/to/dikeprofiles");

            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "GEKB A",
                Children =
                {
                    new GrassCoverErosionInwardsCalculationScenario
                    {
                        Name = "Calculation 1",
                        Comments =
                        {
                            Body = "Comments for Calculation 1"
                        },
                        InputParameters =
                        {
                            DikeProfile = dikeProfile1,
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0],
                            BreakWater =
                            {
                                Height = (RoundedDouble) (dikeProfile1.BreakWater.Height + 0.3),
                                Type = BreakWaterType.Wall
                            },
                            DikeHeight = (RoundedDouble) (dikeProfile1.DikeHeight + 0.2),
                            Orientation = dikeProfile1.Orientation,
                            CriticalFlowRate =
                            {
                                Mean = (RoundedDouble) 1.1,
                                StandardDeviation = (RoundedDouble) 2.2
                            },
                            ShouldDikeHeightBeCalculated = true,
                            DikeHeightTargetProbability = assessmentSection.FailureMechanismContribution.Norm,
                            ShouldOvertoppingRateBeCalculated = true,
                            OvertoppingRateTargetProbability = assessmentSection.FailureMechanismContribution.Norm,
                            UseForeshore = true,
                            UseBreakWater = true
                        },
                        Output = new GrassCoverErosionInwardsOutput(new OvertoppingOutput(0.45, true, 1.1, null),
                                                                    new DikeHeightOutput(0.56, 0.05, 2, 0.06, 3, CalculationConvergence.CalculatedConverged, null),
                                                                    new OvertoppingRateOutput(0.57, 0.07, 4, 0.08, 5, CalculationConvergence.CalculatedConverged, null))
                    },
                    new GrassCoverErosionInwardsCalculationScenario
                    {
                        Name = "Calculation 2",
                        Comments =
                        {
                            Body = "Comments for Calculation 2"
                        },
                        InputParameters =
                        {
                            DikeProfile = dikeProfile1,
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0],
                            BreakWater =
                            {
                                Height = (RoundedDouble) (dikeProfile1.BreakWater.Height + 0.3),
                                Type = BreakWaterType.Wall
                            },
                            DikeHeight = (RoundedDouble) (dikeProfile1.DikeHeight + 0.2),
                            Orientation = dikeProfile1.Orientation,
                            CriticalFlowRate =
                            {
                                Mean = (RoundedDouble) 1.1,
                                StandardDeviation = (RoundedDouble) 2.2
                            },
                            UseForeshore = true,
                            UseBreakWater = true,
                            ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                            ShouldDikeHeightBeCalculated = true,
                            DikeHeightTargetProbability = assessmentSection.FailureMechanismContribution.Norm,
                            ShouldDikeHeightIllustrationPointsBeCalculated = true,
                            ShouldOvertoppingRateBeCalculated = true,
                            OvertoppingRateTargetProbability = assessmentSection.FailureMechanismContribution.Norm,
                            ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                        },
                        Output = new GrassCoverErosionInwardsOutput(new OvertoppingOutput(0.45, true, 1.1, GetConfiguredGeneralResultFaultTreeIllustrationPoint()),
                                                                    new DikeHeightOutput(0.56, 0.05, 2, 0.06, 3, CalculationConvergence.CalculatedConverged, GetConfiguredGeneralResultFaultTreeIllustrationPoint()),
                                                                    new OvertoppingRateOutput(0.57, 0.07, 4, 0.08, 5, CalculationConvergence.CalculatedConverged, GetConfiguredGeneralResultFaultTreeIllustrationPoint()))
                    }
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Name = "GEKB B"
            });
            failureMechanism.CalculationsGroup.Children.Add(
                new GrassCoverErosionInwardsCalculationScenario
                {
                    Name = "Calculation 2",
                    Comments =
                    {
                        Body = "Comments about Calculation 2"
                    },
                    InputParameters =
                    {
                        DikeHeightTargetProbability = assessmentSection.FailureMechanismContribution.Norm,
                        OvertoppingRateTargetProbability = assessmentSection.FailureMechanismContribution.Norm
                    }
                });
        }

        #endregion

        #region GrassCoverErosionOutwards FailureMechanism

        private static void ConfigureGrassCoverErosionOutwardsFailureMechanism(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                               IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N = (RoundedDouble) 15.0;
            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First();
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
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
                            StepSize = WaveConditionsInputStepSize.Two,
                            CalculationType = GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact,
                            WaterLevelType = WaveConditionsInputWaterLevelType.None
                        }
                    },
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
                    {
                        Name = "Calculation 2",
                        Comments =
                        {
                            Body = "Comments for Calculation 2"
                        },
                        InputParameters =
                        {
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                            BreakWater =
                            {
                                Height = (RoundedDouble) (foreshoreProfile.BreakWater.Height + 0.3),
                                Type = BreakWaterType.Wall
                            },
                            CalculationsTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.First(),
                            Orientation = foreshoreProfile.Orientation,
                            UseForeshore = true,
                            UseBreakWater = true,
                            UpperBoundaryRevetment = (RoundedDouble) 22.3,
                            LowerBoundaryRevetment = (RoundedDouble) (-3.2),
                            UpperBoundaryWaterLevels = (RoundedDouble) 15.3,
                            LowerBoundaryWaterLevels = (RoundedDouble) (-2.4),
                            StepSize = WaveConditionsInputStepSize.Two,
                            CalculationType = GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact,
                            WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability
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
                    Name = "Calculation 3",
                    Comments =
                    {
                        Body = "Comments for Calculation 3"
                    },
                    InputParameters =
                    {
                        ForeshoreProfile = null,
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
                        StepSize = WaveConditionsInputStepSize.One,
                        CalculationType = GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact,
                        WaterLevelType = WaveConditionsInputWaterLevelType.Signaling
                    },
                    Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(
                        new[]
                        {
                            new WaveConditionsOutput(1, 2, 3, 4, 5, 0.6, 0.7, 0.8, 0.9, CalculationConvergence.NotCalculated),
                            new WaveConditionsOutput(0, 1, 2, 3, 4, 0.5, 0.6, 0.7, 0.8, CalculationConvergence.NotCalculated)
                        },
                        new[]
                        {
                            new WaveConditionsOutput(10, 20, 30, 40, 50, 0.4, 0.5, 0.6, 0.7, CalculationConvergence.NotCalculated),
                            new WaveConditionsOutput(0, 10, 20, 30, 40, 0.7, 0.6, 0.5, 0.4, CalculationConvergence.NotCalculated)
                        },
                        new[]
                        {
                            new WaveConditionsOutput(10, 20, 30, 40, 50, 0.4, 0.5, 0.6, 0.7, CalculationConvergence.NotCalculated),
                            new WaveConditionsOutput(0, 10, 20, 30, 40, 0.7, 0.6, 0.5, 0.4, CalculationConvergence.NotCalculated)
                        })
                });
        }

        #endregion

        #region StabilityStoneCover FailureMechanism

        private static void ConfigureStabilityStoneCoverFailureMechanism(StabilityStoneCoverFailureMechanism failureMechanism,
                                                                         IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralInput.N = (RoundedDouble) 15.0;

            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
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
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
                            StepSize = WaveConditionsInputStepSize.Two,
                            CalculationType = StabilityStoneCoverWaveConditionsCalculationType.Columns,
                            WaterLevelType = WaveConditionsInputWaterLevelType.None
                        }
                    },
                    new StabilityStoneCoverWaveConditionsCalculation
                    {
                        Name = "Calculation 2",
                        Comments =
                        {
                            Body = "Comments for Calculation 2"
                        },
                        InputParameters =
                        {
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                            BreakWater =
                            {
                                Height = (RoundedDouble) (foreshoreProfile.BreakWater.Height + 0.3),
                                Type = BreakWaterType.Wall
                            },
                            CalculationsTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.First(),
                            Orientation = foreshoreProfile.Orientation,
                            UseForeshore = true,
                            UseBreakWater = true,
                            UpperBoundaryRevetment = (RoundedDouble) 22.3,
                            LowerBoundaryRevetment = (RoundedDouble) (-3.2),
                            UpperBoundaryWaterLevels = (RoundedDouble) 15.3,
                            LowerBoundaryWaterLevels = (RoundedDouble) (-2.4),
                            StepSize = WaveConditionsInputStepSize.Two,
                            CalculationType = StabilityStoneCoverWaveConditionsCalculationType.Columns,
                            WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability
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
                    Name = "Calculation 3",
                    Comments =
                    {
                        Body = "Comments for Calculation 3"
                    },
                    InputParameters =
                    {
                        ForeshoreProfile = null,
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
                        StepSize = WaveConditionsInputStepSize.One,
                        WaterLevelType = WaveConditionsInputWaterLevelType.Signaling
                    },
                    Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create(new[]
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

        #endregion

        #region WaveImpactAsphaltCover FailureMechanism

        private static void ConfigureWaveImpactAsphaltCoverFailureMechanism(WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                                            IAssessmentSection assessmentSection)
        {
            failureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL = (RoundedDouble) 1337.0;

            ForeshoreProfile foreshoreProfile = failureMechanism.ForeshoreProfiles[0];
            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
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
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
                            StepSize = WaveConditionsInputStepSize.Two,
                            WaterLevelType = WaveConditionsInputWaterLevelType.None
                        }
                    },
                    new WaveImpactAsphaltCoverWaveConditionsCalculation
                    {
                        Name = "Calculation 2",
                        Comments =
                        {
                            Body = "Comments for Calculation 2"
                        },
                        InputParameters =
                        {
                            ForeshoreProfile = foreshoreProfile,
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                            BreakWater =
                            {
                                Height = (RoundedDouble) (foreshoreProfile.BreakWater.Height + 0.3),
                                Type = BreakWaterType.Wall
                            },
                            CalculationsTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Last(),
                            Orientation = foreshoreProfile.Orientation,
                            UseForeshore = true,
                            UseBreakWater = true,
                            UpperBoundaryRevetment = (RoundedDouble) 22.3,
                            LowerBoundaryRevetment = (RoundedDouble) (-3.2),
                            UpperBoundaryWaterLevels = (RoundedDouble) 15.3,
                            LowerBoundaryWaterLevels = (RoundedDouble) (-2.4),
                            StepSize = WaveConditionsInputStepSize.Two,
                            WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability
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
                    Name = "Calculation 3",
                    Comments =
                    {
                        Body = "Comments for Calculation 3"
                    },
                    InputParameters =
                    {
                        ForeshoreProfile = null,
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
                        StepSize = WaveConditionsInputStepSize.One,
                        WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit
                    },
                    Output = new WaveImpactAsphaltCoverWaveConditionsOutput(new[]
                    {
                        new WaveConditionsOutput(1, 2, 3, 4, 5, 0.6, 0.7, 0.8, 0.9, CalculationConvergence.NotCalculated),
                        new WaveConditionsOutput(0, 1, 2, 3, 4, 0.5, 0.6, 0.7, 0.8, CalculationConvergence.NotCalculated)
                    })
                });
        }

        #endregion
    }
}