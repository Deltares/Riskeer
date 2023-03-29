﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System.Collections.ObjectModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Integration.Test
{
    public class MigrationTo222IntegrationTest
    {
        private const string newVersion = "23.1";

        [Test]
        public void Given221Project_WhenUpgradedTo231_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               "MigrationTestProject221.risk");
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

                    AssertHydraulicBoundaryData(reader, sourceFilePath);
                    AssertHydraulicBoundaryDatabase(reader, sourceFilePath);
                    AssertHydraulicLocation(reader, sourceFilePath);
                }

                AssertLogDatabase(logFilePath);
            }
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
                "ClosingStructuresOutputEntity",
                "DikeProfileEntity",
                "DuneErosionFailureMechanismMetaEntity",
                "DuneLocationCalculationEntity",
                "DuneLocationCalculationForTargetProbabilityCollectionEntity",
                "DuneLocationCalculationOutputEntity",
                "DuneLocationEntity",
                "FailureMechanismFailureMechanismSectionEntity",
                "FaultTreeIllustrationPointEntity",
                "FaultTreeIllustrationPointStochastEntity",
                "FaultTreeSubmechanismIllustrationPointEntity",
                "ForeshoreProfileEntity",
                "GeneralResultFaultTreeIllustrationPointEntity",
                "GeneralResultFaultTreeIllustrationPointStochastEntity",
                "GeneralResultSubMechanismIllustrationPointEntity",
                "GeneralResultSubMechanismIllustrationPointStochastEntity",
                "GrassCoverErosionInwardsDikeHeightOutputEntity",
                "GrassCoverErosionInwardsOutputEntity",
                "GrassCoverErosionInwardsOvertoppingRateOutputEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "GrassCoverErosionOutwardsWaveConditionsOutputEntity",
                "GrassCoverSlipOffOutwardsFailureMechanismMetaEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HeightStructuresOutputEntity",
                "HydraulicLocationCalculationCollectionEntity",
                "HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity",
                "HydraulicLocationCalculationEntity",
                "HydraulicLocationCalculationForTargetProbabilityCollectionEntity",
                "HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity",
                "HydraulicLocationOutputEntity",
                "IllustrationPointResultEntity",
                "MacroStabilityInwardsCalculationEntity",
                "MacroStabilityInwardsCalculationOutputEntity",
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
                "ProbabilisticPipingCalculationOutputEntity",
                "ProjectEntity",
                "SemiProbabilisticPipingCalculationOutputEntity",
                "SpecificFailureMechanismEntity",
                "SpecificFailureMechanismFailureMechanismSectionEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresOutputEntity",
                "StabilityStoneCoverFailureMechanismMetaEntity",
                "StabilityStoneCoverWaveConditionsCalculationEntity",
                "StabilityStoneCoverWaveConditionsOutputEntity",
                "StochastEntity",
                "SubMechanismIllustrationPointEntity",
                "SubMechanismIllustrationPointStochastEntity",
                "TopLevelFaultTreeIllustrationPointEntity",
                "TopLevelSubMechanismIllustrationPointEntity",
                "VersionEntity",
                "WaveImpactAsphaltCoverWaveConditionsCalculationEntity",
                "WaveImpactAsphaltCoverWaveConditionsOutputEntity"
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

        private static void AssertLogDatabase(string logFilePath)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(2, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("22.1", newVersion, "Gevolgen van de migratie van versie 22.1 naar versie 23.1:"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("22.1", newVersion, "* Geen aanpassingen."),
                    messages[i]);
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