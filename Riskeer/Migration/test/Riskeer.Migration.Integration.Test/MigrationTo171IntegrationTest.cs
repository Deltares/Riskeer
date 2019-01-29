// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.ObjectModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Integration.Test
{
    [TestFixture]
    public class MigrationTo171IntegrationTest
    {
        private const string newVersion = "17.1";

        [Test]
        public void Given164Project_WhenUpgradedTo171_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               "MigrationTestProject164.rtd");
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given164Project_WhenUpgradedTo171_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given164Project_WhenUpgradedTo171_ThenProjectAsExpected), ".log"));
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

                    AssertDuneErosionFailureMechanism(reader);
                    AssertClosingStructuresFailureMechanism(reader, sourceFilePath);
                    AssertGrassCoverErosionInwardsFailureMechanism(reader);
                    AssertGrassCoverErosionOutwardsFailureMechanism(reader);
                    AssertHeightStructuresFailureMechanism(reader);
                    AssertPipingFailureMechanism(reader);
                    AssertStabilityPointStructuresFailureMechanism(reader);
                    AssertStabilityStoneCoverFailureMechanism(reader);
                    AssertWaveImpactAsphaltCoverFailureMechanism(reader);

                    AssertHydraulicBoundaryLocations(reader);
                    AssertDikeProfiles(reader);
                    AssertForeshoreProfiles(reader);
                    AssertStochasticSoilModels(reader);
                    AssertSurfaceLines(reader);
                    AssertSoilLayers(reader, sourceFilePath);
                    AssertBackgroundData(reader);

                    AssertVersions(reader);
                    AssertDatabase(reader);
                }

                AssertLogDatabase(logFilePath);
            }
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string[] tables =
            {
                "AssessmentSectionEntity",
                "CalculationGroupEntity",
                "CharacteristicPointEntity",
                "ClosingStructureEntity",
                "ClosingStructuresCalculationEntity",
                "ClosingStructuresSectionResultEntity",
                "DikeProfileEntity",
                "DuneErosionSectionResultEntity",
                "FailureMechanismEntity",
                "FailureMechanismSectionEntity",
                "ForeshoreProfileEntity",
                "GrassCoverErosionInwardsCalculationEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionInwardsSectionResultEntity",
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsHydraulicLocationEntity",
                "GrassCoverErosionOutwardsSectionResultEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "GrassCoverSlipOffInwardsSectionResultEntity",
                "GrassCoverSlipOffOutwardsSectionResultEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HeightStructuresSectionResultEntity",
                "HydraulicLocationEntity",
                "MacrostabilityInwardsSectionResultEntity",
                "MacrostabilityOutwardsSectionResultEntity",
                "MicrostabilitySectionResultEntity",
                "PipingCalculationEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingSectionResultEntity",
                "PipingStructureSectionResultEntity",
                "ProjectEntity",
                "SoilLayerEntity",
                "SoilProfileEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresSectionResultEntity",
                "StabilityStoneCoverSectionResultEntity",
                "StabilityStoneCoverWaveConditionsCalculationEntity",
                "StochasticSoilModelEntity",
                "StochasticSoilProfileEntity",
                "StrengthStabilityLengthwiseConstructionSectionResultEntity",
                "SurfaceLineEntity",
                "TechnicalInnovationSectionResultEntity",
                "VersionEntity",
                "WaterPressureAsphaltCoverSectionResultEntity",
                "WaveImpactAsphaltCoverSectionResultEntity",
                "WaveImpactAsphaltCoverWaveConditionsCalculationEntity"
            };

            foreach (string table in tables)
            {
                string validateMigratedTable =
                    $"ATTACH DATABASE[{sourceFilePath}] AS SOURCEPROJECT; " +
                    $"SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].{table}) " +
                    $"FROM {table};" +
                    "DETACH SOURCEPROJECT;";
                reader.AssertReturnedDataIsValid(validateMigratedTable);
            }
        }

        private static void AssertDikeProfiles(MigratedDatabaseReader reader)
        {
            const string validateDikeProfiles =
                "SELECT COUNT(DISTINCT(Id)) = COUNT() " +
                "FROM DikeProfileEntity " +
                "GROUP BY [FailureMechanismEntityId]";
            reader.AssertReturnedDataIsValid(validateDikeProfiles);
        }

        private static void AssertForeshoreProfiles(MigratedDatabaseReader reader)
        {
            const string validateForeshoreProfiles =
                "SELECT COUNT() = 0 " +
                "FROM ForeshoreProfileEntity " +
                "WHERE Id != Name;";
            reader.AssertReturnedDataIsValid(validateForeshoreProfiles);
        }

        private static void AssertStochasticSoilModels(MigratedDatabaseReader reader)
        {
            const string validateStochasticSoilModels =
                "SELECT COUNT(DISTINCT(Name)) = COUNT() " +
                "FROM StochasticSoilModelEntity;";
            reader.AssertReturnedDataIsValid(validateStochasticSoilModels);

            AssertStochasticSoilProfiles(reader);
        }

        private static void AssertStochasticSoilProfiles(MigratedDatabaseReader reader)
        {
            const string validateStochasticSoilProfiles =
                "SELECT " +
                "(SELECT COUNT() = (SELECT COUNT() FROM StochasticSoilProfileEntity WHERE [Type] = 1) FROM StochasticSoilProfileEntity) " +
                "AND " +
                "(SELECT COUNT() = 0 FROM StochasticSoilProfileEntity WHERE [Probability] NOT BETWEEN 0 AND 1 OR [Probability] IS NULL);";
            reader.AssertReturnedDataIsValid(validateStochasticSoilProfiles);
        }

        private static void AssertSoilLayers(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSoilLayers =
                $"ATTACH DATABASE[{sourceFilePath}] AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].SoilLayerEntity) " +
                "FROM SoilLayerEntity AS NEW " +
                "LEFT JOIN [SOURCEPROJECT].SoilLayerEntity AS OLD ON NEW.[SoilLayerEntityId] = OLD.[SoilLayerEntityId] " +
                "WHERE ((NEW.[DiameterD70CoefficientOfVariation] IS NULL AND OLD.[DiameterD70Deviation] IS NULL) " +
                "OR NEW.[DiameterD70CoefficientOfVariation] = ROUND(OLD.[DiameterD70Deviation] / OLD.[DiameterD70Mean], 6)) " +
                "AND ((NEW.[PermeabilityCoefficientOfVariation] IS NULL AND OLD.[PermeabilityDeviation] IS NULL) " +
                "OR NEW.[PermeabilityCoefficientOfVariation] = ROUND(OLD.[PermeabilityDeviation] / OLD.[PermeabilityMean], 6)); " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSoilLayers);
        }

        private static void AssertSurfaceLines(MigratedDatabaseReader reader)
        {
            const string validateSurfaceLines =
                "SELECT COUNT(DISTINCT(Name)) = COUNT() " +
                "FROM SurfaceLineEntity;";
            reader.AssertReturnedDataIsValid(validateSurfaceLines);
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM VersionEntity " +
                "WHERE [Version] = \"17.1\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        private static void AssertBackgroundData(MigratedDatabaseReader reader)
        {
            const string validateBackgroundData =
                "SELECT COUNT() = " +
                "(SELECT COUNT() FROM AssessmentSectionEntity) " +
                "FROM BackgroundDataEntity " +
                "WHERE [Name] = \"Bing Maps - Satelliet\" " +
                "AND [IsVisible] = 1 " +
                "AND [Transparency] = 0.0 " +
                "AND [BackgroundDataType] = 2 " +
                "AND [AssessmentSectionEntityId] IN " +
                "(SELECT [AssessmentSectionEntityId] FROM AssessmentSectionEntity);";
            reader.AssertReturnedDataIsValid(validateBackgroundData);

            const string validateBackgroundMetaData =
                "SELECT COUNT() = " +
                "(SELECT COUNT() FROM BackgroundDataEntity) " +
                "FROM BackgroundDataMetaEntity " +
                "WHERE [Key] = \"WellKnownTileSource\" " +
                "AND [Value] = \"2\" " +
                "AND [BackgroundDataEntityId] IN " +
                "(SELECT BackgroundDataEntityId FROM BackgroundDataEntity);";
            reader.AssertReturnedDataIsValid(validateBackgroundMetaData);
        }

        private static void AssertLogDatabase(string logFilePath)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(6, messages.Count);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("5", newVersion, "Gevolgen van de migratie van versie 16.4 naar versie 17.1:"),
                    messages[0]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("5", newVersion, "* Alle berekende resultaten zijn verwijderd."),
                    messages[1]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("5", newVersion, "* Traject: 'assessmentSection'"),
                    messages[2]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("5", newVersion, "  + Toetsspoor: 'Grasbekleding erosie kruin en binnentalud'"),
                    messages[3]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("5", newVersion, "    - De naam van dijkprofiel '1' is veranderd naar '102' en wordt ook gebruikt als ID."),
                    messages[4]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("5", newVersion, "    - De naam van dijkprofiel '10' is veranderd naar '104' en wordt ook gebruikt als ID."),
                    messages[5]);
            }
        }

        private static void AssertClosingStructuresFailureMechanism(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanisms =
                $"ATTACH DATABASE[{sourceFilePath}] AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].ClosingStructureFailureMechanismMetaEntity) " +
                "FROM ClosingStructuresFailureMechanismMetaEntity;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM ClosingStructuresOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertDuneErosionFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 8) " +
                "FROM DuneErosionFailureMechanismMetaEntity " +
                "WHERE N = 2.0 " +
                "AND FailureMechanismEntityId IN " +
                "(SELECT FailureMechanismEntityId FROM FailureMechanismEntity WHERE FailureMechanismType = 8);";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsFailureMechanismMetaEntity] " +
                "WHERE [DikeProfileCollectionSourcePath] != \"\";";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);

            const string validateCalculations =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsCalculationEntity] " +
                "WHERE [OvertoppingRateCalculationType] != 1;";
            reader.AssertReturnedDataIsValid(validateCalculations);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsDikeHeightOutputEntity] " +
                "JOIN [GrassCoverErosionInwardsOutputEntity];";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                "JOIN GrassCoverErosionOutwardsWaveConditionsOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertHeightStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM HeightStructuresOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertHydraulicBoundaryLocations(MigratedDatabaseReader reader)
        {
            const string validateOutputs =
                "SELECT COUNT() = 0 " +
                "FROM HydraulicLocationOutputEntity";
            reader.AssertReturnedDataIsValid(validateOutputs);
        }

        private static void AssertPipingFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = 0 " +
                "FROM [PipingFailureMechanismMetaEntity] " +
                "WHERE [StochasticSoilModelSourcePath] != \"\"" +
                "OR [SurfaceLineSourcePath] != \"\";";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);

            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM PipingCalculationOutputEntity " +
                "JOIN PipingSemiProbabilisticOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertStabilityPointStructuresFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM StabilityPointStructuresOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertStabilityStoneCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM StabilityStoneCoverWaveConditionsOutputEntity";
            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateCalculationOutputs =
                "SELECT COUNT() = 0 " +
                "FROM WaveImpactAsphaltCoverWaveConditionsOutputEntity";

            reader.AssertReturnedDataIsValid(validateCalculationOutputs);
        }
    }
}