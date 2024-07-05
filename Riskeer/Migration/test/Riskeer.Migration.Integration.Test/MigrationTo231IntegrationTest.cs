// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Integration.Test
{
    public class MigrationTo231IntegrationTest
    {
        private const string newVersion = "23.1";

        [Test]
        [TestCaseSource(nameof(GetMigrationProjectsWithMessages))]
        public void Given221Project_WhenUpgradedTo231_ThenProjectAsExpected(string fileName, IEnumerable<string> expectedMessages)
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               fileName);
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given221Project_WhenUpgradedTo231_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given221Project_WhenUpgradedTo231_ThenProjectAsExpected), ".log"));
            var migrator = new ProjectFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                // When
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Then
                using (var reader = new MigratedDatabaseReader(targetFilePath))
                {
                    AssertTablesContentMigrated(reader, sourceFilePath);

                    AssertVersions(reader);
                    AssertDatabase(reader);

                    AssertFailureMechanismWithResultProbabilityTypeManual(reader, sourceFilePath);
                    AssertFailureMechanismWithResultProbabilityTypeAutomatic(reader, sourceFilePath);

                    AssertDuneLocation(reader, sourceFilePath);

                    AssertHydraulicBoundaryData(reader, sourceFilePath);
                    AssertHydraulicBoundaryDatabase(reader, sourceFilePath);
                    AssertHydraulicLocation(reader, sourceFilePath);

                    AssertHydraulicLocationOutput(reader);
                    AssertDuneLocationOutput(reader);

                    AssertGrassCoverErosionInwardsOutput(reader);

                    AssertGrassCoverErosionOutwardsOutput(reader);
                    AssertStabilityStoneCoverOutput(reader);
                    AssertWaveImpactAsphaltCoverOutput(reader);

                    AssertClosingStructuresOutput(reader);
                    AssertHeightStructuresOutput(reader);
                    AssertStabilityPointStructuresOutput(reader);
                    AssertMacroStabilityInwardsOutput(reader, sourceFilePath);
                    AssertPipingOutput(reader, sourceFilePath);

                    AssertIllustrationPointResults(reader);
                }

                AssertLogDatabase(logFilePath, expectedMessages);
            }
        }

        private static IEnumerable<TestCaseData> GetMigrationProjectsWithMessages()
        {
            yield return new TestCaseData("MigrationTestProject221NoOutput.risk", new[]
            {
                "* Geen aanpassingen."
            });

            yield return new TestCaseData("MigrationTestProject221MacroStabilityInwardsNoManualAssessmentLevels.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd."
            });

            yield return new TestCaseData("MigrationTestProject221PipingNoManualAssessmentLevels.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd."
            });

            // This file contains all configured failure mechanisms (except Dunes and MacroStabilityInwards) with output.
            // The mechanisms Dunes and MacroStabilityInwards have different assessment sections, and are therefore put in different test files.
            yield return new TestCaseData("MigrationTestProject221WithOutput.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd, behalve die van het faalmechanisme 'Piping' en/of 'Macrostabiliteit binnenwaarts' waarbij de waterstand handmatig is ingevuld."
            });

            yield return new TestCaseData("MigrationTestProject221DunesWithOutput.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd."
            });

            yield return new TestCaseData("MigrationTestProject221MacroStabilityInwardsWithOutput.risk", new[]
            {
                "* Alle berekende resultaten zijn verwijderd, behalve die van het faalmechanisme 'Piping' en/of 'Macrostabiliteit binnenwaarts' waarbij de waterstand handmatig is ingevuld."
            });

            yield return new TestCaseData("MigrationTestProject221WithFailureMechanismAssemblyResultsAutomatic.risk", new[]
            {
                "* Traject: 'Traject 12-2'",
                "  + Faalmechanisme: 'Piping'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Grasbekleding erosie kruin en binnentalud'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Hoogte kunstwerk'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Wateroverdruk bij asfaltbekleding'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Betrouwbaarheid sluiting kunstwerk'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Macrostabiliteit binnenwaarts'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Golfklappen op asfaltbekleding'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Grasbekleding erosie buitentalud'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Grasbekleding afschuiven binnentalud'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Grasbekleding afschuiven buitentalud'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Microstabiliteit'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Piping bij kunstwerk'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Stabiliteit steenzetting'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Duinafslag'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Sterkte en stabiliteit puntconstructies'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd.",
                "  + Faalmechanisme: 'Faalmechanisme Automatisch'",
                "    - De automatisch berekende faalkans van het faalmechanisme is verwijderd."
            });

            yield return new TestCaseData("MigrationTestProject221WithFailureMechanismAssemblyResultsManual.risk", new[]
            {
                "* Geen aanpassingen."
            });
        }

        private static void AssertFailureMechanismWithResultProbabilityTypeManual(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanism =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                "WHERE [FailureMechanismAssemblyResultProbabilityResultType] = 2 " +
                ") " +
                "FROM FailureMechanismEntity NEW " +
                "JOIN SOURCEPROJECT.FailureMechanismEntity OLD USING(FailureMechanismEntityId) " +
                "WHERE NEW.[AssessmentSectionEntityId] = OLD.[AssessmentSectionEntityId] " +
                "AND NEW.[CalculationGroupEntityId] IS OLD.[CalculationGroupEntityId] " +
                "AND NEW.[FailureMechanismType] = OLD.[FailureMechanismType] " +
                "AND NEW.[InAssembly] = OLD.[InAssembly] " +
                "AND NEW.[FailureMechanismSectionCollectionSourcePath] IS OLD.[FailureMechanismSectionCollectionSourcePath] " +
                "AND NEW.[InAssemblyInputComments] IS OLD.[InAssemblyInputComments] " +
                "AND NEW.[InAssemblyOutputComments] IS OLD.[InAssemblyOutputComments] " +
                "AND NEW.[NotInAssemblyComments] IS OLD.[NotInAssemblyComments] " +
                "AND NEW.[CalculationsInputComments] IS OLD.[CalculationsInputComments] " +
                "AND NEW.[FailureMechanismAssemblyResultProbabilityResultType] = 4 " +
                "AND NEW.[FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability] IS OLD.[FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability]; " +
                "DETACH SOURCEPROJECT";
            reader.AssertReturnedDataIsValid(validateFailureMechanism);
        }

        private static void AssertFailureMechanismWithResultProbabilityTypeAutomatic(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanism =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                "WHERE [FailureMechanismAssemblyResultProbabilityResultType] = 1 " +
                ") " +
                "FROM FailureMechanismEntity NEW " +
                "JOIN SOURCEPROJECT.FailureMechanismEntity OLD USING(FailureMechanismEntityId) " +
                "WHERE NEW.[AssessmentSectionEntityId] = OLD.[AssessmentSectionEntityId] " +
                "AND NEW.[CalculationGroupEntityId] IS OLD.[CalculationGroupEntityId] " +
                "AND NEW.[FailureMechanismType] = OLD.[FailureMechanismType] " +
                "AND NEW.[InAssembly] = OLD.[InAssembly] " +
                "AND NEW.[FailureMechanismSectionCollectionSourcePath] IS OLD.[FailureMechanismSectionCollectionSourcePath] " +
                "AND NEW.[InAssemblyInputComments] IS OLD.[InAssemblyInputComments] " +
                "AND NEW.[InAssemblyOutputComments] IS OLD.[InAssemblyOutputComments] " +
                "AND NEW.[NotInAssemblyComments] IS OLD.[NotInAssemblyComments] " +
                "AND NEW.[CalculationsInputComments] IS OLD.[CalculationsInputComments] " +
                "AND NEW.[FailureMechanismAssemblyResultProbabilityResultType] = 1 " +
                "AND NEW.[FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability] IS OLD.[FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability]; " +
                "DETACH SOURCEPROJECT";
            reader.AssertReturnedDataIsValid(validateFailureMechanism);
        }

        private static void AssertDuneLocation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateDuneLocation =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.DuneLocationEntity " +
                ") " +
                "FROM DuneLocationEntity NEW " +
                "JOIN SOURCEPROJECT.DuneLocationEntity OLD USING(DuneLocationEntityId) " +
                "JOIN (" +
                "SELECT HydraulicLocationEntityId, LocationId " +
                "FROM SOURCEPROJECT.HydraulicLocationEntity " +
                ") HYDRAULICLOCATION " +
                "USING(LocationId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] = HYDRAULICLOCATION.[HydraulicLocationEntityId] " +
                "AND NEW.[Name] = OLD.[Name]" +
                "AND NEW.[CoastalAreaId] = OLD.[CoastalAreaId] " +
                "AND NEW.[Offset] IS OLD.[Offset] " +
                "AND NEW.\"Order\" = OLD.\"Order\"; " +
                "DETACH SOURCEPROJECT";

            reader.AssertReturnedDataIsValid(validateDuneLocation);
        }

        private static void AssertHydraulicBoundaryData(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateHydraulicBoundaryData =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.HydraulicBoundaryDatabaseEntity " +
                ") " +
                "FROM HydraulicBoundaryDataEntity NEW " +
                "JOIN SOURCEPROJECT.HydraulicBoundaryDatabaseEntity OLD ON NEW.HydraulicBoundaryDataEntityId = OLD.HydraulicBoundaryDatabaseEntity " +
                "WHERE NEW.[AssessmentSectionEntityId] = OLD.[AssessmentSectionEntityId] " +
                "AND NEW.[HydraulicLocationConfigurationDatabaseFilePath] = OLD.[HydraulicLocationConfigurationSettingsFilePath]" +
                "AND NEW.[HydraulicLocationConfigurationDatabaseScenarioName] = OLD.[HydraulicLocationConfigurationSettingsScenarioName] " +
                "AND NEW.[HydraulicLocationConfigurationDatabaseYear] = OLD.[HydraulicLocationConfigurationSettingsYear] " +
                "AND NEW.[HydraulicLocationConfigurationDatabaseScope] = OLD.[HydraulicLocationConfigurationSettingsScope] " +
                "AND NEW.[HydraulicLocationConfigurationDatabaseSeaLevel] IS OLD.[HydraulicLocationConfigurationSettingsSeaLevel] " +
                "AND NEW.[HydraulicLocationConfigurationDatabaseRiverDischarge] IS OLD.[HydraulicLocationConfigurationSettingsRiverDischarge] " +
                "AND NEW.[HydraulicLocationConfigurationDatabaseLakeLevel] IS OLD.[HydraulicLocationConfigurationSettingsLakeLevel] " +
                "AND NEW.[HydraulicLocationConfigurationDatabaseWindDirection] IS OLD.[HydraulicLocationConfigurationSettingsWindDirection] " +
                "AND NEW.[HydraulicLocationConfigurationDatabaseWindSpeed] IS OLD.[HydraulicLocationConfigurationSettingsWindSpeed] " +
                "AND NEW.[HydraulicLocationConfigurationDatabaseComment] IS OLD.[HydraulicLocationConfigurationSettingsComment]; " +
                "DETACH SOURCEPROJECT";

            reader.AssertReturnedDataIsValid(validateHydraulicBoundaryData);
        }

        private static void AssertHydraulicBoundaryDatabase(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateHydraulicBoundaryDatabase =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.HydraulicBoundaryDatabaseEntity " +
                ") " +
                "FROM HydraulicBoundaryDatabaseEntity NEW " +
                "JOIN SOURCEPROJECT.HydraulicBoundaryDatabaseEntity OLD ON NEW.HydraulicBoundaryDataEntityId = OLD.HydraulicBoundaryDatabaseEntity " +
                "WHERE NEW.[Version] = OLD.[Version] " +
                "AND NEW.[FilePath] = OLD.[FilePath] " +
                "AND NEW.[UsePreprocessorClosure] = OLD.[HydraulicLocationConfigurationSettingsUsePreprocessorClosure] " +
                "AND NEW.\"Order\" = 0; " +
                "DETACH SOURCEPROJECT";

            reader.AssertReturnedDataIsValid(validateHydraulicBoundaryDatabase);
        }

        private static void AssertHydraulicLocation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateHydraulicLocation =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.HydraulicLocationEntity " +
                ") " +
                "FROM HydraulicLocationEntity NEW " +
                "JOIN ( " +
                "SELECT " +
                "[HydraulicBoundaryDatabaseEntityId] AS HBDId " +
                "FROM HydraulicBoundaryDatabaseEntity" +
                ") " +
                "ON NEW.HydraulicBoundaryDatabaseEntityId = HBDId " +
                "JOIN SOURCEPROJECT.HydraulicLocationEntity OLD USING(HydraulicLocationEntityId) " +
                "WHERE NEW.[LocationId] = OLD.[LocationId] " +
                "AND NEW.[Name] = OLD.[Name] " +
                "AND NEW.[LocationX] IS OLD.[LocationX] " +
                "AND NEW.[LocationY] IS OLD.[LocationY] " +
                "AND NEW.\"Order\" = OLD.\"Order\"; " +
                "DETACH SOURCEPROJECT";

            reader.AssertReturnedDataIsValid(validateHydraulicLocation);
        }

        private static void AssertHydraulicLocationOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [HydraulicLocationOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertDuneLocationOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [DuneLocationCalculationOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertGrassCoverErosionInwardsOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);

            const string validateDikeHeightOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsDikeHeightOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateDikeHeightOutput);

            const string validateOvertoppingRateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsOvertoppingRateOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOvertoppingRateOutput);
        }

        private static void AssertGrassCoverErosionOutwardsOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionOutwardsWaveConditionsOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertStabilityStoneCoverOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityStoneCoverWaveConditionsOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertWaveImpactAsphaltCoverOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [WaveImpactAsphaltCoverWaveConditionsOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertClosingStructuresOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [ClosingStructuresOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertHeightStructuresOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [HeightStructuresOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertStabilityPointStructuresOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityPointStructuresOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertMacroStabilityInwardsOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMacroStabilityInwardsOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.MacroStabilityInwardsCalculationOutputEntity " +
                "JOIN SOURCEPROJECT.MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId)" +
                "WHERE UseAssessmentLevelManualInput = 1" +
                ")" +
                "FROM MacroStabilityInwardsCalculationOutputEntity NEW " +
                "JOIN SOURCEPROJECT.MacroStabilityInwardsCalculationOutputEntity OLD " +
                "USING (MacroStabilityInwardsCalculationOutputEntityId)" +
                "WHERE NEW.[MacroStabilityInwardsCalculationEntityId] = OLD.[MacroStabilityInwardsCalculationEntityId] " +
                "AND NEW.[FactorOfStability] IS OLD.[FactorOfStability] " +
                "AND NEW.[ForbiddenZonesXEntryMin] IS OLD.[ForbiddenZonesXEntryMin] " +
                "AND NEW.[ForbiddenZonesXEntryMax] IS OLD.[ForbiddenZonesXEntryMax] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleCenterX] IS OLD.[SlidingCurveLeftSlidingCircleCenterX] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleCenterY] IS OLD.[SlidingCurveLeftSlidingCircleCenterY] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleRadius] IS OLD.[SlidingCurveLeftSlidingCircleRadius] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleIsActive] = OLD.[SlidingCurveLeftSlidingCircleIsActive] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleNonIteratedForce] IS OLD.[SlidingCurveLeftSlidingCircleNonIteratedForce] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleIteratedForce] IS OLD.[SlidingCurveLeftSlidingCircleIteratedForce] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleDrivingMoment] IS OLD.[SlidingCurveLeftSlidingCircleDrivingMoment] " +
                "AND NEW.[SlidingCurveLeftSlidingCircleResistingMoment] IS OLD.[SlidingCurveLeftSlidingCircleResistingMoment] " +
                "AND NEW.[SlidingCurveRightSlidingCircleCenterX] IS OLD.[SlidingCurveRightSlidingCircleCenterX] " +
                "AND NEW.[SlidingCurveRightSlidingCircleCenterY] IS OLD.[SlidingCurveRightSlidingCircleCenterY] " +
                "AND NEW.[SlidingCurveRightSlidingCircleRadius] IS OLD.[SlidingCurveRightSlidingCircleRadius] " +
                "AND NEW.[SlidingCurveRightSlidingCircleIsActive] = OLD.[SlidingCurveRightSlidingCircleIsActive] " +
                "AND NEW.[SlidingCurveRightSlidingCircleNonIteratedForce] IS OLD.[SlidingCurveRightSlidingCircleNonIteratedForce] " +
                "AND NEW.[SlidingCurveRightSlidingCircleIteratedForce] IS OLD.[SlidingCurveRightSlidingCircleIteratedForce] " +
                "AND NEW.[SlidingCurveRightSlidingCircleDrivingMoment] IS OLD.[SlidingCurveRightSlidingCircleDrivingMoment] " +
                "AND NEW.[SlidingCurveRightSlidingCircleResistingMoment] IS OLD.[SlidingCurveRightSlidingCircleResistingMoment] " +
                "AND NEW.[SlidingCurveNonIteratedHorizontalForce] IS OLD.[SlidingCurveNonIteratedHorizontalForce] " +
                "AND NEW.[SlidingCurveIteratedHorizontalForce] IS OLD.[SlidingCurveIteratedHorizontalForce] " +
                "AND NEW.[SlidingCurveSliceXML] = OLD.[SlidingCurveSliceXML] " +
                "AND NEW.[SlipPlaneLeftGridXLeft] IS OLD.[SlipPlaneLeftGridXLeft] " +
                "AND NEW.[SlipPlaneLeftGridXRight] IS OLD.[SlipPlaneLeftGridXRight] " +
                "AND NEW.[SlipPlaneLeftGridNrOfHorizontalPoints] = OLD.[SlipPlaneLeftGridNrOfHorizontalPoints] " +
                "AND NEW.[SlipPlaneLeftGridZTop] IS OLD.[SlipPlaneLeftGridZTop] " +
                "AND NEW.[SlipPlaneLeftGridZBottom] IS OLD.[SlipPlaneLeftGridZBottom] " +
                "AND NEW.[SlipPlaneLeftGridNrOfVerticalPoints] = OLD.[SlipPlaneLeftGridNrOfVerticalPoints] " +
                "AND NEW.[SlipPlaneRightGridXLeft] IS OLD.[SlipPlaneRightGridXLeft] " +
                "AND NEW.[SlipPlaneRightGridXRight] IS OLD.[SlipPlaneRightGridXRight] " +
                "AND NEW.[SlipPlaneRightGridNrOfHorizontalPoints] = OLD.[SlipPlaneRightGridNrOfHorizontalPoints] " +
                "AND NEW.[SlipPlaneRightGridZTop] IS OLD.[SlipPlaneRightGridZTop] " +
                "AND NEW.[SlipPlaneRightGridZBottom] IS OLD.[SlipPlaneRightGridZBottom] " +
                "AND NEW.[SlipPlaneRightGridNrOfVerticalPoints] = OLD.[SlipPlaneRightGridNrOfVerticalPoints] " +
                "AND NEW.[SlipPlaneTangentLinesXml] = OLD.[SlipPlaneTangentLinesXml]; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateMacroStabilityInwardsOutput);
        }

        private static void AssertPipingOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string validateProbabilisticCalculationOutput =
                "SELECT COUNT() = 0 " +
                "FROM ProbabilisticPipingCalculationOutputEntity;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateProbabilisticCalculationOutput);

            string validateSemiProbabilisticOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.SemiProbabilisticPipingCalculationOutputEntity " +
                "JOIN SOURCEPROJECT.SemiProbabilisticPipingCalculationEntity USING(SemiProbabilisticPipingCalculationEntityId) " +
                "WHERE UseAssessmentLevelManualInput = 1" +
                ") " +
                "FROM SemiProbabilisticPipingCalculationOutputEntity NEW " +
                "JOIN SOURCEPROJECT.SemiProbabilisticPipingCalculationOutputEntity OLD " +
                "USING (SemiProbabilisticPipingCalculationOutputEntityId)" +
                "WHERE NEW.[SemiProbabilisticPipingCalculationEntityId] = OLD.[SemiProbabilisticPipingCalculationEntityId]" +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[HeaveFactorOfSafety] IS OLD.[HeaveFactorOfSafety] " +
                "AND NEW.[UpliftFactorOfSafety] IS OLD.[UpliftFactorOfSafety] " +
                "AND NEW.[SellmeijerFactorOfSafety] IS OLD.[SellmeijerFactorOfSafety] " +
                "AND NEW.[UpliftEffectiveStress] IS OLD.[UpliftEffectiveStress] " +
                "AND NEW.[HeaveGradient] IS OLD.[HeaveGradient] " +
                "AND NEW.[SellmeijerCreepCoefficient] IS OLD.[SellmeijerCreepCoefficient] " +
                "AND NEW.[SellmeijerCriticalFall] IS OLD.[SellmeijerCriticalFall] " +
                "AND NEW.[SellmeijerReducedFall] IS OLD.[SellmeijerReducedFall]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSemiProbabilisticOutput);
        }

        private static void AssertIllustrationPointResults(MigratedDatabaseReader reader)
        {
            const string validateFaultTreeIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [FaultTreeIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateFaultTreeIllustrationPoint);

            const string validateFaultTreeIllustrationPointStochast =
                "SELECT COUNT() = 0 " +
                "FROM [FaultTreeIllustrationPointStochastEntity]; ";
            reader.AssertReturnedDataIsValid(validateFaultTreeIllustrationPointStochast);

            const string validateFaultTreeSubmechanismIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [FaultTreeSubmechanismIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateFaultTreeSubmechanismIllustrationPoint);

            const string validateGeneralResultFaultTreeIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [GeneralResultFaultTreeIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateGeneralResultFaultTreeIllustrationPoint);

            const string validateGeneralResultFaultTreeIllustrationPointStochast =
                "SELECT COUNT() = 0 " +
                "FROM [GeneralResultFaultTreeIllustrationPointStochastEntity]; ";
            reader.AssertReturnedDataIsValid(validateGeneralResultFaultTreeIllustrationPointStochast);

            const string validateGeneralResultSubMechanismIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [GeneralResultSubMechanismIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateGeneralResultSubMechanismIllustrationPoint);

            const string validateGeneralResultSubMechanismIllustrationPointStochast =
                "SELECT COUNT() = 0 " +
                "FROM [GeneralResultSubMechanismIllustrationPointStochastEntity]; ";
            reader.AssertReturnedDataIsValid(validateGeneralResultSubMechanismIllustrationPointStochast);

            const string validateIllustrationPointResult =
                "SELECT COUNT() = 0 " +
                "FROM [IllustrationPointResultEntity]; ";
            reader.AssertReturnedDataIsValid(validateIllustrationPointResult);

            const string validateSubMechanismIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [SubMechanismIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateSubMechanismIllustrationPoint);

            const string validateSubMechanismIllustrationPointStochast =
                "SELECT COUNT() = 0 " +
                "FROM [SubMechanismIllustrationPointStochastEntity]; ";
            reader.AssertReturnedDataIsValid(validateSubMechanismIllustrationPointStochast);

            const string validateTopLevelFaultTreeIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [TopLevelFaultTreeIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateTopLevelFaultTreeIllustrationPoint);

            const string validateTopLevelSubMechanismIllustrationPoint =
                "SELECT COUNT() = 0 " +
                "FROM [TopLevelSubMechanismIllustrationPointEntity]; ";
            reader.AssertReturnedDataIsValid(validateTopLevelSubMechanismIllustrationPoint);
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string[] tables =
            {
                "AssessmentSectionEntity",
                "FailureMechanismSectionEntity",
                "FailureMechanismEntity",
                "ClosingStructuresFailureMechanismMetaEntity",
                "CalculationGroupEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "SemiProbabilisticPipingCalculationEntity",
                "GrassCoverErosionInwardsCalculationEntity",
                "GrassCoverSlipOffInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "PipingSoilLayerEntity",
                "PipingSoilProfileEntity",
                "PipingStochasticSoilProfileEntity",
                "PipingScenarioConfigurationPerFailureMechanismSectionEntity",
                "StochasticSoilModelEntity",
                "SurfaceLineEntity",
                "PipingCharacteristicPointEntity",
                "WaterPressureAsphaltCoverFailureMechanismMetaEntity",
                "WaveImpactAsphaltCoverFailureMechanismMetaEntity",
                "AdoptableFailureMechanismSectionResultEntity",
                "AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity",
                "BackgroundDataEntity",
                "BackgroundDataMetaEntity",
                "ClosingStructureEntity",
                "ClosingStructuresCalculationEntity",
                "DikeProfileEntity",
                "DuneErosionFailureMechanismMetaEntity",
                "DuneLocationCalculationEntity",
                "DuneLocationCalculationForTargetProbabilityCollectionEntity",
                "FailureMechanismFailureMechanismSectionEntity",
                "ForeshoreProfileEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "GrassCoverSlipOffOutwardsFailureMechanismMetaEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HydraulicLocationCalculationCollectionEntity",
                "HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity",
                "HydraulicLocationCalculationEntity",
                "HydraulicLocationCalculationForTargetProbabilityCollectionEntity",
                "HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity",
                "MacroStabilityInwardsCalculationEntity",
                "MacroStabilityInwardsCharacteristicPointEntity",
                "MacroStabilityInwardsFailureMechanismMetaEntity",
                "MacroStabilityInwardsPreconsolidationStressEntity",
                "MacroStabilityInwardsSoilLayerOneDEntity",
                "MacroStabilityInwardsSoilLayerTwoDEntity",
                "MacroStabilityInwardsSoilProfileOneDEntity",
                "MacroStabilityInwardsSoilProfileTwoDEntity",
                "MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity",
                "MacroStabilityInwardsStochasticSoilProfileEntity",
                "MicrostabilityFailureMechanismMetaEntity",
                "NonAdoptableFailureMechanismSectionResultEntity",
                "NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingStructureFailureMechanismMetaEntity",
                "ProbabilisticPipingCalculationEntity",
                "ProjectEntity",
                "SpecificFailureMechanismEntity",
                "SpecificFailureMechanismFailureMechanismSectionEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityStoneCoverFailureMechanismMetaEntity",
                "StabilityStoneCoverWaveConditionsCalculationEntity",
                "StochastEntity",
                "VersionEntity",
                "WaveImpactAsphaltCoverWaveConditionsCalculationEntity"
            };

            foreach (string table in tables)
            {
                string validateMigratedTable =
                    $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                    $"SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].{table}) " +
                    $"FROM {table};" +
                    "DETACH SOURCEPROJECT;";
                reader.AssertReturnedDataIsValid(validateMigratedTable);
            }
        }

        private static void AssertLogDatabase(string logFilePath, IEnumerable<string> expectedMessages)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(expectedMessages.Count() + 1, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("22.1", newVersion, "Gevolgen van de migratie van versie 22.1 naar versie 23.1:"),
                    messages[i++]);

                foreach (string expectedMessage in expectedMessages)
                {
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("22.1", newVersion, $"{expectedMessage}"),
                        messages[i++]);
                }
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"23.1\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }
    }
}