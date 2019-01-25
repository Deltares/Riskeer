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
    public class MigrationTo191IntegrationTest
    {
        private const string newVersion = "19.1";

        [Test]
        public void Given181Project_WhenUpgradedTo182_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Migration.Core,
                                                               "MigrationTestProject181.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given181Project_WhenUpgradedTo182_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given181Project_WhenUpgradedTo182_ThenProjectAsExpected), ".log"));
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

                    AssertAssessmentSection(reader,sourceFilePath);
                    AssertBackgroundData(reader, sourceFilePath);

                    AssertPipingSoilLayers(reader);
                }

                AssertLogDatabase(logFilePath);
            }
        }

        private static void AssertAssessmentSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateAssessmentSection =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM SOURCEPROJECT.AssessmentSectionEntity) " +
                "FROM AssessmentSectionEntity NEW " +
                "JOIN SOURCEPROJECT.AssessmentSectionEntity OLD USING(AssessmentSectionEntityId) " +
                "WHERE NEW.[ProjectEntityId] = OLD.[ProjectEntityId] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity1Id] = OLD.[HydraulicLocationCalculationCollectionEntity1Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity2Id] = OLD.[HydraulicLocationCalculationCollectionEntity2Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity3Id] = OLD.[HydraulicLocationCalculationCollectionEntity3Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity4Id] = OLD.[HydraulicLocationCalculationCollectionEntity4Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity5Id] = OLD.[HydraulicLocationCalculationCollectionEntity5Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity6Id] = OLD.[HydraulicLocationCalculationCollectionEntity6Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity7Id] = OLD.[HydraulicLocationCalculationCollectionEntity7Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity8Id] = OLD.[HydraulicLocationCalculationCollectionEntity8Id] " +
                "AND NEW.[Id] IS OLD.[Id] " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[LowerLimitNorm] = OLD.[LowerLimitNorm] " +
                "AND NEW.[SignalingNorm] = OLD.[SignalingNorm] " +
                "AND NEW.[NormativeNormType] = OLD.[NormativeNormType] " +
                "AND NEW.[Composition] = OLD.[Composition] " +
                "AND NEW.[ReferenceLinePointXml] IS OLD.[ReferenceLinePointXml] " +
                "AND NEW.\"Order\" =  OLD.\"Order\"; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateAssessmentSection);

            string validateHydraulicDatabase =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.AssessmentSectionEntity " +
                "WHERE HydraulicDatabaseLocation IS NOT NULL " +
                ") " +
                "FROM HydraulicBoundaryDatabaseEntity NEW " +
                "JOIN SOURCEPROJECT.AssessmentSectionEntity OLD USING(AssessmentSectionEntityId) " +
                "WHERE NEW.[Version] = OLD.[HydraulicDatabaseVersion] " +
                "AND NEW.[FilePath] = OLD.[HydraulicDatabaseLocation] " +
                "AND NEW.[HydraulicLocationConfigurationSettingsFilePath] = rtrim(OLD.[HydraulicDatabaseLocation], replace(OLD.[HydraulicDatabaseLocation], '\\', '')) || 'hlcd.sqlite' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsScenarioName] = 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsYear] = 2023 " +
                "AND NEW.[HydraulicLocationConfigurationSettingsSeaLevel] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsRiverDischarge] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsLakeLevel] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsWindDirection] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsWindSpeed] IS 'Conform WBI2017' " +
                "AND NEW.[HydraulicLocationConfigurationSettingsComment] IS 'Gegenereerd door Ringtoets (conform WBI2017)'; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHydraulicDatabase);
        }

        private static void AssertBackgroundData(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateTransparency =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT SUM([IsInvalid]) = 0 " +
                "FROM " +
                "(" +
                "SELECT " +
                "CASE WHEN (NEW.[Transparency] = OLD.[Transparency] AND OLD.[Transparency] > 0) " +
                "OR (NEW.[Transparency] = 0.6 AND OLD.[Transparency] = 0) " +
                "THEN 0 " +
                "ELSE 1 " +
                "END AS [IsInvalid] " +
                "FROM BackgroundDataEntity NEW " +
                "JOIN [SOURCEPROJECT].BackgroundDataEntity OLD USING(BackgroundDataEntityId) " +
                "); " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateTransparency);

            string validateBackgroundData =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].BackgroundDataEntity) " +
                "FROM BackgroundDataEntity NEW " +
                "JOIN [SOURCEPROJECT].BackgroundDataEntity OLD USING(BackgroundDataEntityId) " +
                "WHERE NEW.[AssessmentSectionEntityId] = OLD.[AssessmentSectionEntityId] " +
                "AND NEW.[Name] = OLD.[Name] " +
                "AND NEW.[IsVisible] = OLD.[IsVisible] " +
                "AND NEW.[BackgroundDataType] = OLD.[BackgroundDataType]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateBackgroundData);
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string[] tables =
            {
                "AssessmentSectionEntity",
                "BackgroundDataEntity",
                "BackgroundDataMetaEntity",
                "CalculationGroupEntity",
                "ClosingStructureEntity",
                "ClosingStructuresCalculationEntity",
                "ClosingStructuresFailureMechanismMetaEntity",
                "ClosingStructuresOutputEntity",
                "ClosingStructuresSectionResultEntity",
                "DikeProfileEntity",
                "DuneErosionFailureMechanismMetaEntity",
                "DuneErosionSectionResultEntity",
                "DuneLocationCalculationCollectionEntity",
                "DuneLocationCalculationEntity",
                "DuneLocationCalculationOutputEntity",
                "DuneLocationEntity",
                "FailureMechanismEntity",
                "FailureMechanismSectionEntity",
                "FaultTreeIllustrationPointEntity",
                "FaultTreeIllustrationPointStochastEntity",
                "FaultTreeSubmechanismIllustrationPointEntity",
                "ForeshoreProfileEntity",
                "GeneralResultFaultTreeIllustrationPointEntity",
                "GeneralResultFaultTreeIllustrationPointStochastEntity",
                "GeneralResultSubMechanismIllustrationPointEntity",
                "GeneralResultSubMechanismIllustrationPointStochastEntity",
                "GrassCoverErosionInwardsCalculationEntity",
                "GrassCoverErosionInwardsDikeHeightOutputEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionInwardsOutputEntity",
                "GrassCoverErosionInwardsOvertoppingRateOutputEntity",
                "GrassCoverErosionInwardsSectionResultEntity",
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsSectionResultEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "GrassCoverErosionOutwardsWaveConditionsOutputEntity",
                "GrassCoverSlipOffInwardsSectionResultEntity",
                "GrassCoverSlipOffOutwardsSectionResultEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HeightStructuresOutputEntity",
                "HeightStructuresSectionResultEntity",
                "HydraulicLocationCalculationCollectionEntity",
                "HydraulicLocationCalculationEntity",
                "HydraulicLocationEntity",
                "HydraulicLocationOutputEntity",
                "IllustrationPointResultEntity",
                "MacroStabilityInwardsCalculationEntity",
                "MacroStabilityInwardsCalculationOutputEntity",
                "MacroStabilityInwardsCharacteristicPointEntity",
                "MacroStabilityInwardsFailureMechanismMetaEntity",
                "MacroStabilityInwardsPreconsolidationStressEntity",
                "MacroStabilityInwardsSectionResultEntity",
                "MacroStabilityInwardsSoilLayerOneDEntity",
                "MacroStabilityInwardsSoilLayerTwoDEntity",
                "MacroStabilityInwardsSoilProfileOneDEntity",
                "MacroStabilityInwardsSoilProfileTwoDEntity",
                "MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity",
                "MacroStabilityInwardsStochasticSoilProfileEntity",
                "MacroStabilityOutwardsFailureMechanismMetaEntity",
                "MacroStabilityOutwardsSectionResultEntity",
                "MicrostabilitySectionResultEntity",
                "PipingCalculationEntity",
                "PipingCalculationOutputEntity",
                "PipingCharacteristicPointEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingSectionResultEntity",
                "PipingSoilLayerEntity",
                "PipingSoilProfileEntity",
                "PipingStochasticSoilProfileEntity",
                "PipingStructureFailureMechanismMetaEntity",
                "PipingStructureSectionResultEntity",
                "ProjectEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresOutputEntity",
                "StabilityPointStructuresSectionResultEntity",
                "StabilityStoneCoverFailureMechanismMetaEntity",
                "StabilityStoneCoverSectionResultEntity",
                "StabilityStoneCoverWaveConditionsCalculationEntity",
                "StabilityStoneCoverWaveConditionsOutputEntity",
                "StochastEntity",
                "StochasticSoilModelEntity",
                "StrengthStabilityLengthwiseConstructionSectionResultEntity",
                "SubMechanismIllustrationPointEntity",
                "SubMechanismIllustrationPointStochastEntity",
                "SurfaceLineEntity",
                "TechnicalInnovationSectionResultEntity",
                "TopLevelFaultTreeIllustrationPointEntity",
                "TopLevelSubMechanismIllustrationPointEntity",
                "VersionEntity",
                "WaterPressureAsphaltCoverSectionResultEntity",
                "WaveImpactAsphaltCoverFailureMechanismMetaEntity",
                "WaveImpactAsphaltCoverSectionResultEntity",
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

                Assert.AreEqual(10, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "Gevolgen van de migratie van versie 18.1 naar versie 19.1:"),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'BackgroundData-DefaultValue'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + De waarde voor de transparantie van de achtergrondkaart is aangepast naar 0.60."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'PipingSoilLayer'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + Toetsspoor: 'Piping'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "    - De waarde '0.0049' voor het gemiddelde van parameter 'Verzadigd gewicht' van ondergrondlaag 'BelowPhreaticLevelMean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "    - De waarde '4.9e-07' voor het gemiddelde van parameter 'Doorlatendheid' van ondergrondlaag 'PermeabilityMean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "    - De waarde '4.9e-07' voor het gemiddelde van parameter 'd70' van ondergrondlaag 'DiameterD70Mean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "* Traject: 'WithHydraulicDatabase'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("18.1", newVersion, "  + Er worden standaardwaarden conform WBI2017 voor de HLCD bestand informatie gebruikt."),
                    messages[i]);
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"19.1\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        private static void AssertPipingSoilLayers(MigratedDatabaseReader reader)
        {
            const string validateBelowPhreaticLevel =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [BelowPhreaticLevelMean] < [BelowPhreaticLevelShift] " +
                "OR [BelowPhreaticLevelMean] < 0.005;";
            reader.AssertReturnedDataIsValid(validateBelowPhreaticLevel);

            const string validateDiameter70 =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [DiameterD70Mean] < 0.0000005;";
            reader.AssertReturnedDataIsValid(validateDiameter70);

            const string validatePermeability =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [PermeabilityMean] < 0.0000005;";
            reader.AssertReturnedDataIsValid(validatePermeability);
        }
    }
}