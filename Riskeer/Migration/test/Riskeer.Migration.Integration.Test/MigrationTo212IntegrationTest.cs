// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    public class MigrationTo212IntegrationTest
    {
        private const string newVersion = "21.2";

        [Test]
        public void Given211Project_WhenUpgradedTo212_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               "MigrationTestProject211.risk");
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given211Project_WhenUpgradedTo212_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given211Project_WhenUpgradedTo212_ThenProjectAsExpected), ".log"));
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

                    AssertAssessmentSection(reader, sourceFilePath);
                    AssertFailureMechanism(reader, sourceFilePath);
                    AssertFailureMechanismSection(reader, sourceFilePath);
                    AssertMacroStabilityOutwardsFailurePath(reader, sourceFilePath);
                    AssertMacroStabilityOutwardsSections(reader, sourceFilePath);
                    AssertStrengthStabilityLengthwiseConstructionFailurePath(reader, sourceFilePath);
                    AssertStrengthStabilityLengthwiseConstructionSections(reader, sourceFilePath);
                    AssertTechnicalInnovationFailurePath(reader, sourceFilePath);
                    AssertTechnicalInnovationSections(reader, sourceFilePath);

                    AssertHydraulicBoundaryLocationCalculation(reader, sourceFilePath);
                    AssertHydraulicLocationOutput(reader);

                    AssertPipingFailureMechanism(reader, sourceFilePath);
                    AssertPipingScenarioConfigurationPerFailureMechanismSection(reader, sourceFilePath);
                    AssertPipingFailureMechanismSectionResults(reader, sourceFilePath);
                    AssertSemiProbabilisticPipingOutput(reader, sourceFilePath);

                    AssertGrassCoverErosionInwardsFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertGrassCoverErosionInwardsCalculation(reader, sourceFilePath);
                    AssertGrassCoverErosionInwardsOutput(reader);
                    AssertGrassCoverErosionInwardsSectionResults(reader, sourceFilePath);

                    AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertGrassCoverErosionOutwardsCalculations(reader, sourceFilePath);
                    AssertGrassCoverErosionOutwardsSectionResults(reader, sourceFilePath);

                    AssertStabilityStoneCoverFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertStabilityStoneCoverCalculations(reader, sourceFilePath);

                    AssertWaveImpactAsphaltCoverCalculations(reader, sourceFilePath);
                    AssertWaveImpactAsphaltCoverSectionResults(reader, sourceFilePath);

                    AssertWaveConditionCalculationOutputs(reader);

                    AssertDuneErosionFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertDuneLocationCalculationCollection(reader);
                    AssertDuneLocationCalculation(reader);
                    AssertDuneLocationCalculationOutput(reader);

                    AssertMacroStabilityInwardsOutput(reader);
                    AssertMacroStabilityInwardsFailureMechanismSectionResults(reader, sourceFilePath);

                    AssertHeightStructuresSectionResults(reader, sourceFilePath);
                    AssertClosingStructuresSectionResults(reader, sourceFilePath);
                    AssertStabilityPointStructuresSectionResults(reader, sourceFilePath);

                    AssertStandAloneFailureMechanismMetaEntity(reader, sourceFilePath);

                    AssertPipingStructureFailureMechanismSectionResults(reader, sourceFilePath);
                    AssertDuneErosionFailureMechanismSectionResults(reader, sourceFilePath);
                    AssertStabilityStoneCoverSectionResults(reader, sourceFilePath);
                    AssertMicrostabilitySectionResults(reader, sourceFilePath);
                    AssertWaterPressureAsphaltCoverSectionResults(reader, sourceFilePath);
                    AssertGrassCoverSlipOffOutwardsSectionResults(reader, sourceFilePath);
                    AssertGrassCoverSlipOffInwardsSectionResults(reader, sourceFilePath);
                }

                AssertLogDatabase(logFilePath);
            }
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
                "ClosingStructuresSectionResultEntity",
                "DikeProfileEntity",
                "DuneErosionFailureMechanismMetaEntity",
                "DuneErosionSectionResultEntity",
                "DuneLocationEntity",
                "FailureMechanismSectionEntity",
                "ForeshoreProfileEntity",
                "GrassCoverErosionInwardsFailureMechanismMetaEntity",
                "GrassCoverErosionInwardsSectionResultEntity",
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsSectionResultEntity",
                "GrassCoverSlipOffInwardsSectionResultEntity",
                "GrassCoverSlipOffOutwardsSectionResultEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HeightStructuresSectionResultEntity",
                "HydraulicBoundaryDatabaseEntity",
                "HydraulicLocationEntity",
                "MacroStabilityInwardsCalculationEntity",
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
                "MicrostabilitySectionResultEntity",
                "PipingCharacteristicPointEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingSectionResultEntity",
                "PipingSoilLayerEntity",
                "PipingSoilProfileEntity",
                "PipingStochasticSoilProfileEntity",
                "PipingStructureFailureMechanismMetaEntity",
                "PipingStructureSectionResultEntity",
                "ProbabilisticPipingCalculationEntity",
                "ProjectEntity",
                "SemiProbabilisticPipingCalculationEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresSectionResultEntity",
                "StabilityStoneCoverFailureMechanismMetaEntity",
                "StabilityStoneCoverSectionResultEntity",
                "StochastEntity",
                "StochasticSoilModelEntity",
                "SurfaceLineEntity",
                "VersionEntity",
                "WaterPressureAsphaltCoverSectionResultEntity",
                "WaveImpactAsphaltCoverFailureMechanismMetaEntity",
                "WaveImpactAsphaltCoverSectionResultEntity"
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

                Assert.AreEqual(8, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "Gevolgen van de migratie van versie 21.1 naar versie 21.2:"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "* De oorspronkelijke faalmechanismen zijn omgezet naar het nieuwe formaat.\r\n* Alle toetsoordelen zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "* Traject: 'Traject 12-2 GEKB signaleringsparameter'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "  + Faalmechanisme: 'Grasbekleding erosie kruin en binnentalud'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "    - De waarden van de doelkans voor HBN en overslagdebiet zijn veranderd naar de trajectnorm."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "* Traject: 'Traject 12-2 GEKB omgevingswaarde'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "  + Faalmechanisme: 'Grasbekleding erosie kruin en binnentalud'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("21.1", newVersion, "    - De waarden van de doelkans voor HBN en overslagdebiet zijn veranderd naar de trajectnorm."),
                    messages[i]);
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"21.2\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        private static void AssertAssessmentSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateAssessmentSection =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.AssessmentSectionEntity " +
                ") " +
                "FROM AssessmentSectionEntity NEW " +
                "JOIN SOURCEPROJECT.AssessmentSectionEntity OLD USING(AssessmentSectionEntityId) " +
                "WHERE NEW.[ProjectEntityId] = OLD.[ProjectEntityId] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity1Id] = OLD.[HydraulicLocationCalculationCollectionEntity2Id] " +
                "AND NEW.[HydraulicLocationCalculationCollectionEntity2Id] = OLD.[HydraulicLocationCalculationCollectionEntity3Id] " +
                "AND NEW.[Id] IS OLD.[Id] " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[LowerLimitNorm] = OLD.[LowerLimitNorm] " +
                "AND NEW.[SignalingNorm] = OLD.[SignalingNorm] " +
                "AND NEW.[NormativeNormType] = OLD.[NormativeNormType] " +
                "AND NEW.[Composition] = OLD.[Composition] " +
                "AND NEW.[ReferenceLinePointXml] = OLD.[ReferenceLinePointXml]; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateAssessmentSection);
        }

        private static void AssertFailureMechanism(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanism =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                "WHERE [FailureMechanismType] != 18 AND [FailureMechanismType] != 17 AND [FailureMechanismType] != 13 " +
                ") " +
                "FROM FailureMechanismEntity NEW " +
                "JOIN SOURCEPROJECT.FailureMechanismEntity OLD USING(FailureMechanismEntityId) " +
                "JOIN ( " +
                "SELECT " +
                "[FailureMechanismEntityId], " +
                "CASE " +
                "WHEN [FailureMechanismType] = 14 " +
                "THEN 13 " +
                "WHEN [FailureMechanismType] = 15 " +
                "THEN 14 " +
                "WHEN [FailureMechanismType] = 16 " +
                "THEN 15 " +
                "ELSE [FailureMechanismType] " +
                "END AS RenumberedFailureMechanismType " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                "WHERE FailureMechanismType != 13 AND FailureMechanismType != 17 AND FailureMechanismType != 18" +
                ") USING(FailureMechanismEntityId) " +
                "WHERE NEW.[AssessmentSectionEntityId] = OLD.[AssessmentSectionEntityId] " +
                "AND NEW.[CalculationGroupEntityId] IS OLD.[CalculationGroupEntityId] " +
                "AND NEW.[FailureMechanismType] = RenumberedFailureMechanismType " +
                "AND NEW.[InAssembly] = OLD.[IsRelevant] " +
                "AND NEW.[FailureMechanismSectionCollectionSourcePath] IS OLD.[FailureMechanismSectionCollectionSourcePath] " +
                "AND NEW.[InAssemblyInputComments] IS OLD.[InputComments] " +
                "AND NEW.[InAssemblyOutputComments] IS OLD.[OutputComments] " +
                "AND NEW.[NotInAssemblyComments] IS OLD.[NotRelevantComments] " +
                "AND NEW.[CalculationsInputComments] IS NULL " +
                "AND NEW.[FailurePathAssemblyProbabilityResultType] = 1 " +
                "AND NEW.[ManualFailurePathAssemblyProbability] IS NULL; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanism);
        }

        private static void AssertFailureMechanismSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanismSectionMapping =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.FailureMechanismSectionEntity " +
                ") " +
                "FROM FailureMechanismFailureMechanismSectionEntity NEW " +
                "JOIN SOURCEPROJECT.FailureMechanismSectionEntity OLD USING(FailureMechanismSectionEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId];" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismSectionMapping);

            string validateFailureMechanismSectionEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.FailureMechanismSectionEntity " +
                ") " +
                "FROM FailureMechanismSectionEntity NEW " +
                "JOIN SOURCEPROJECT.FailureMechanismSectionEntity OLD USING(FailureMechanismSectionEntityId) " +
                "WHERE NEW.[Name] = OLD.[Name] " +
                "AND NEW.[FailureMechanismSectionPointXml] = OLD.[FailureMechanismSectionPointXml];" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismSectionEntity);
        }

        private static void AssertMacroStabilityOutwardsFailurePath(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertMigratedSpecificFailurePath(reader, sourceFilePath, 13, "Macrostabiliteit buitenwaarts", "STBU", 1);
        }
        
        private static void AssertMacroStabilityOutwardsSections(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity(reader, "MacroStabilityOutwardsSectionResultEntity", sourceFilePath);
            AssertSpecificFailurePathFailureMechanismSectionEntity(reader, 13, "Macrostabiliteit buitenwaarts", sourceFilePath);
        }

        private static void AssertStrengthStabilityLengthwiseConstructionFailurePath(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertMigratedSpecificFailurePath(reader, sourceFilePath, 17, "Sterkte en stabiliteit langsconstructies", "STKWl", 2);
        }
        
        private static void AssertStrengthStabilityLengthwiseConstructionSections(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity(reader, "StrengthStabilityLengthwiseConstructionSectionResultEntity", sourceFilePath);
            AssertSpecificFailurePathFailureMechanismSectionEntity(reader, 17, "Sterkte en stabiliteit langsconstructies", sourceFilePath);
        }

        private static void AssertTechnicalInnovationFailurePath(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertMigratedSpecificFailurePath(reader, sourceFilePath, 18, "Technische innovaties", "INN", 3);
        }
        
        private static void AssertTechnicalInnovationSections(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity(reader, "TechnicalInnovationSectionResultEntity", sourceFilePath);
            AssertSpecificFailurePathFailureMechanismSectionEntity(reader, 18, "Technische innovaties", sourceFilePath);
        }

        private static void AssertSpecificFailurePathFailureMechanismSectionEntity(MigratedDatabaseReader reader, int failureMechanismType, string failureMechanismName, string sourceFilePath)
        {
            string validateSpecificFailurePathFailureMechanismSectionEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "( " +
                "COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                "JOIN SOURCEPROJECT.FailureMechanismSectionEntity USING(FailureMechanismEntityId) " +
                $"WHERE FailureMechanismType = {failureMechanismType} " +
                ") " +
                ") " +
                "FROM SpecificFailurePathEntity NEW " +
                "JOIN SpecificFailurePathFailureMechanismSectionEntity USING(SpecificFailurePathEntityId) " +
                "JOIN ( " +
                "SELECT " +
                "[AssessmentSectionEntityId], " +
                "[FailureMechanismSectionEntityId] " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                "JOIN SOURCEPROJECT.FailureMechanismSectionEntity USING(FailureMechanismEntityId) " +
                $"WHERE FailureMechanismType = {failureMechanismType} " +
                ") OLD USING (FailureMechanismSectionEntityId) " +
                $"WHERE NEW.[Name] = \"{failureMechanismName}\" " +
                "AND NEW.AssessmentSectionEntityId = OLD.AssessmentSectionEntityId; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateSpecificFailurePathFailureMechanismSectionEntity);
        }

        private static void AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity(MigratedDatabaseReader reader, string failureMechanismSectionResultEntityName, string sourceFilePath)
        {
            string validateNonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"FROM SOURCEPROJECT.{failureMechanismSectionResultEntityName} " +
                ") " +
                "FROM NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity NEW " +
                $"JOIN SOURCEPROJECT.{failureMechanismSectionResultEntityName} OLD USING(FailureMechanismSectionEntityId) " +
                "WHERE NEW.[IsRelevant] = 1 " +
                "AND NEW.[InitialFailureMechanismResultType] = 1 " +
                "AND NEW.[ManualInitialFailureMechanismResultSectionProbability] IS NULL " +
                "AND NEW.[ManualInitialFailureMechanismResultProfileProbability] IS NULL " +
                "AND NEW.[FurtherAnalysisType] = 1 " +
                "AND NEW.[RefinedSectionProbability] IS NULL " +
                "AND NEW.[RefinedProfileProbability] IS NULL; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateNonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity);
        }

        private static void AssertMigratedSpecificFailurePath(MigratedDatabaseReader reader, string sourceFilePath,
                                                              int failureMechanismType,
                                                              string failureMechanismName,
                                                              string failureMechanismCode,
                                                              int order)
        {
            string validateSpecificFailurePathEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                $"WHERE FailureMechanismType = {failureMechanismType} " +
                ") " +
                "FROM SpecificFailurePathEntity NEW " +
                "JOIN (" +
                "SELECT " +
                "[AssessmentSectionEntityId], " +
                "[IsRelevant], " +
                "[FailureMechanismSectionCollectionSourcePath], " +
                "[InputComments], " +
                "[OutputComments], " +
                "[NotRelevantComments] " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                $"WHERE FailureMechanismType = {failureMechanismType} " +
                ") OLD USING (AssessmentSectionEntityId) " +
                $"WHERE NEW.[Name] IS \"{failureMechanismName}\" " +
                $"AND NEW.[Code] IS \"{failureMechanismCode}\" " +
                $"AND NEW.[Order] = {order} " +
                "AND NEW.[InAssembly] = OLD.[IsRelevant] " +
                "AND NEW.[FailureMechanismSectionCollectionSourcePath] IS OLD.[FailureMechanismSectionCollectionSourcePath] " +
                "AND NEW.[InAssemblyInputComments] IS OLD.[InputComments] " +
                "AND NEW.[InAssemblyOutputComments] IS OLD.[OutputComments] " +
                "AND NEW.[NotInAssemblyComments] IS OLD.[NotRelevantComments] " +
                "AND NEW.[N] = 1 " +
                "AND NEW.[FailurePathAssemblyProbabilityResultType] = 1 " +
                "AND NEW.[ManualFailurePathAssemblyProbability] IS NULL " +
                "AND NEW.[ApplyLengthEffectInSection] = 0; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateSpecificFailurePathEntity);
        }

        #region WaveConditions

        private static void AssertWaveConditionCalculationOutputs(MigratedDatabaseReader reader)
        {
            const string validateOutputQueryFormat =
                "SELECT COUNT() = 0 " +
                "FROM [{0}]; ";
            reader.AssertReturnedDataIsValid(string.Format(validateOutputQueryFormat, "GrassCoverErosionOutwardsWaveConditionsOutputEntity"));
            reader.AssertReturnedDataIsValid(string.Format(validateOutputQueryFormat, "StabilityStoneCoverWaveConditionsOutputEntity"));
            reader.AssertReturnedDataIsValid(string.Format(validateOutputQueryFormat, "WaveImpactAsphaltCoverWaveConditionsOutputEntity"));
        }

        #endregion

        #region HeightStructures

        private static void AssertHeightStructuresSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertAdoptableFailureMechanismSectionResults(reader, "HeightStructuresSectionResultEntity", sourceFilePath);
        }

        #endregion

        #region ClosingStructures

        private static void AssertClosingStructuresSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertAdoptableFailureMechanismSectionResults(reader, "ClosingStructuresSectionResultEntity", sourceFilePath);
        }

        #endregion

        #region StabilityPointStructures

        private static void AssertStabilityPointStructuresSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertAdoptableFailureMechanismSectionResults(reader, "StabilityPointStructuresSectionResultEntity", sourceFilePath);
        }

        #endregion

        #region StabilityStoneCover

        private static void AssertStabilityStoneCoverSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "StabilityStoneCoverSectionResultEntity", sourceFilePath);
        }

        #endregion

        #region MacroStabilityInwards

        private static void AssertMacroStabilityInwardsOutput(MigratedDatabaseReader reader)
        {
            const string macroStabilityInwardsCalculationOutputEntityTable =
                "SELECT COUNT() = 0 " +
                "FROM MacroStabilityInwardsCalculationOutputEntity;" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(macroStabilityInwardsCalculationOutputEntityTable);
        }

        private static void AssertMacroStabilityInwardsFailureMechanismSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "MacroStabilityInwardsSectionResultEntity", sourceFilePath);
        }

        #endregion

        #region StabilityStoneCover

        private static void AssertStabilityStoneCoverFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanismEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.StabilityStoneCoverFailureMechanismMetaEntity " +
                ") " +
                "FROM StabilityStoneCoverFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.StabilityStoneCoverFailureMechanismMetaEntity OLD USING(StabilityStoneCoverFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[ForeshoreProfileCollectionSourcePath] IS OLD.[ForeshoreProfileCollectionSourcePath] " +
                "AND NEW.[N] IS OLD.[N] " +
                "AND NEW.[ApplyLengthEffectInSection] = 0;" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismEntity);
        }

        private static void AssertStabilityStoneCoverCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateNormBaseQueryFormat =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.StabilityStoneCoverWaveConditionsCalculationEntity " +
                "WHERE [CategoryType] = {0} " +
                ") " +
                "FROM StabilityStoneCoverWaveConditionsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.StabilityStoneCoverWaveConditionsCalculationEntity OLD USING(StabilityStoneCoverWaveConditionsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] IS NULL " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] = OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CalculationType] = OLD.[CalculationType] " +
                "AND NEW.[WaterLevelType] = {1};" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(string.Format(validateNormBaseQueryFormat, 2, 3));
            reader.AssertReturnedDataIsValid(string.Format(validateNormBaseQueryFormat, 3, 2));

            string validateOtherCalculations =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.StabilityStoneCoverWaveConditionsCalculationEntity " +
                "WHERE [CategoryType] != 2 AND [CategoryType] != 3" +
                ") " +
                "FROM StabilityStoneCoverWaveConditionsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.StabilityStoneCoverWaveConditionsCalculationEntity OLD USING(StabilityStoneCoverWaveConditionsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] IS NULL " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] = OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CalculationType] = OLD.[CalculationType] " +
                "AND NEW.[WaterLevelType] = 1;" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateOtherCalculations);
        }

        #endregion

        #region WaveImpactAsphaltCover

        private static void AssertWaveImpactAsphaltCoverCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateNormBaseQueryFormat =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.WaveImpactAsphaltCoverWaveConditionsCalculationEntity " +
                "WHERE [CategoryType] = {0} " +
                ") " +
                "FROM WaveImpactAsphaltCoverWaveConditionsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.WaveImpactAsphaltCoverWaveConditionsCalculationEntity OLD USING(WaveImpactAsphaltCoverWaveConditionsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] IS NULL " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] = OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[WaterLevelType] = {1};" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(string.Format(validateNormBaseQueryFormat, 2, 3));
            reader.AssertReturnedDataIsValid(string.Format(validateNormBaseQueryFormat, 3, 2));

            string validateOtherCalculations =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.WaveImpactAsphaltCoverWaveConditionsCalculationEntity " +
                "WHERE [CategoryType] != 2 AND [CategoryType] != 3" +
                ") " +
                "FROM WaveImpactAsphaltCoverWaveConditionsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.WaveImpactAsphaltCoverWaveConditionsCalculationEntity OLD USING(WaveImpactAsphaltCoverWaveConditionsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] IS NULL " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] = OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[WaterLevelType] = 1;" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateOtherCalculations);
        }

        private static void AssertWaveImpactAsphaltCoverSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "WaveImpactAsphaltCoverSectionResultEntity", sourceFilePath);
        }

        #endregion

        #region StandAlone

        private static void AssertStandAloneFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanismMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (" +
                "SELECT COUNT()" +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                "WHERE FailureMechanismType = {0}" +
                ") " +
                "FROM {1} NEW " +
                "JOIN FailureMechanismEntity FME USING(FailureMechanismEntityId) " +
                "WHERE FME.[FailureMechanismType] = {2} " +
                "AND NEW.[N] = 1 " +
                "AND NEW.[ApplyLengthEffectInSection] = 0;" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(string.Format(validateFailureMechanismMetaEntity, "16", "GrassCoverSlipOffInwardsFailureMechanismMetaEntity", "15"));
            reader.AssertReturnedDataIsValid(string.Format(validateFailureMechanismMetaEntity, "5", "GrassCoverSlipOffOutwardsFailureMechanismMetaEntity", "5"));
            reader.AssertReturnedDataIsValid(string.Format(validateFailureMechanismMetaEntity, "14", "MicrostabilityFailureMechanismMetaEntity", "13"));
            reader.AssertReturnedDataIsValid(string.Format(validateFailureMechanismMetaEntity, "15", "WaterPressureAsphaltCoverFailureMechanismMetaEntity", "14"));
        }

        private static void AssertPipingStructureFailureMechanismSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableFailureMechanismSectionResults(reader, "PipingStructureSectionResultEntity", sourceFilePath);
        }

        private static void AssertMicrostabilitySectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "MicrostabilitySectionResultEntity", sourceFilePath);
        }

        private static void AssertWaterPressureAsphaltCoverSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "WaterPressureAsphaltCoverSectionResultEntity", sourceFilePath);
        }

        private static void AssertGrassCoverSlipOffOutwardsSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "GrassCoverSlipOffOutwardsSectionResultEntity", sourceFilePath);
        }

        private static void AssertGrassCoverSlipOffInwardsSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "GrassCoverSlipOffInwardsSectionResultEntity", sourceFilePath);
        }

        #endregion

        #region HydraulicBoundaryLocations

        private static void AssertHydraulicBoundaryLocationCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string getRelevantCalculationCollectionsQuery =
                "FROM SOURCEPROJECT.AssessmentSectionEntity ase " +
                "JOIN SOURCEPROJECT.HydraulicLocationCalculationCollectionEntity " +
                "ON ase.HydraulicLocationCalculationCollectionEntity2Id = HydraulicLocationCalculationCollectionEntityId " +
                "OR ase.HydraulicLocationCalculationCollectionEntity3Id = HydraulicLocationCalculationCollectionEntityId ";

            string validateHydraulicLocationCalculationCollection =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"{getRelevantCalculationCollectionsQuery} " +
                ") " +
                "FROM HydraulicLocationCalculationCollectionEntity " +
                "JOIN SOURCEPROJECT.HydraulicLocationCalculationCollectionEntity USING(HydraulicLocationCalculationCollectionEntityId);" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHydraulicLocationCalculationCollection);

            string validateHydraulicLocationCalculations =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"{getRelevantCalculationCollectionsQuery} " +
                "JOIN SOURCEPROJECT.HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                ") " +
                "FROM HydraulicLocationCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.HydraulicLocationCalculationEntity OLD USING(HydraulicLocationCalculationEntityId) " +
                "WHERE NEW.[HydraulicLocationEntityId] = OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[ShouldIllustrationPointsBeCalculated] = OLD.[ShouldIllustrationPointsBeCalculated];" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHydraulicLocationCalculations);

            string validateHydraulicLocationCalculationMapping =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"{getRelevantCalculationCollectionsQuery} " +
                "JOIN SOURCEPROJECT.HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                ") " +
                "FROM HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.HydraulicLocationCalculationEntity OLD USING(HydraulicLocationCalculationEntityId) " +
                "WHERE NEW.[HydraulicLocationCalculationCollectionEntityId] = OLD.[HydraulicLocationCalculationCollectionEntityId];" +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHydraulicLocationCalculationMapping);
        }

        private static void AssertHydraulicLocationOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [HydraulicLocationOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        #endregion

        #region Piping

        private static void AssertPipingFailureMechanism(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanismMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.PipingFailureMechanismMetaEntity " +
                ") " +
                "FROM PipingFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.PipingFailureMechanismMetaEntity OLD USING(PipingFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[A] = OLD.[A] " +
                "AND NEW.[WaterVolumetricWeight] = OLD.[WaterVolumetricWeight] " +
                "AND NEW.[StochasticSoilModelCollectionSourcePath] IS OLD.[StochasticSoilModelCollectionSourcePath] " +
                "AND NEW.[SurfaceLineCollectionSourcePath] IS OLD.[SurfaceLineCollectionSourcePath] " +
                "AND NEW.[PipingScenarioConfigurationType] = 1; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismMetaEntity);
        }

        private static void AssertPipingScenarioConfigurationPerFailureMechanismSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validatePipingScenarioConfigurationPerFailureMechanismSectionEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.FailureMechanismEntity " +
                "JOIN SOURCEPROJECT.FailureMechanismSectionEntity USING(FailureMechanismEntityId) " +
                "WHERE FailureMechanismType = 1 " +
                ") " +
                "FROM PipingScenarioConfigurationPerFailureMechanismSectionEntity NEW " +
                "WHERE NEW.[PipingScenarioConfigurationPerFailureMechanismSectionType] = 1;" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validatePipingScenarioConfigurationPerFailureMechanismSectionEntity);
        }

        private static void AssertPipingFailureMechanismSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "PipingSectionResultEntity", sourceFilePath);
        }
        
        private static void AssertSemiProbabilisticPipingOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.SemiProbabilisticPipingCalculationOutputEntity " +
                "WHERE SemiProbabilisticPipingCalculationEntityId IN (SELECT SemiProbabilisticPipingCalculationEntityId FROM SOURCEPROJECT.SemiProbabilisticPipingCalculationEntity WHERE UseAssessmentLevelManualInput IS 1) " +
                ") " +
                "FROM SemiProbabilisticPipingCalculationOutputEntity NEW " +
                "JOIN SOURCEPROJECT.SemiProbabilisticPipingCalculationOutputEntity OLD " +
                "ON NEW.[SemiProbabilisticPipingCalculationOutputEntityId] = OLD.[SemiProbabilisticPipingCalculationOutputEntityId]" +
                "WHERE NEW.[SemiProbabilisticPipingCalculationEntityId] = OLD.[SemiProbabilisticPipingCalculationEntityId] " +
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
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        #endregion

        # region GrassCoverErosionInwards

        private static void AssertGrassCoverErosionInwardsFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanismEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionInwardsFailureMechanismMetaEntity " +
                ") " +
                "FROM GrassCoverErosionInwardsFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsFailureMechanismMetaEntity OLD USING(GrassCoverErosionInwardsFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[N] IS OLD.[N] " +
                "AND NEW.[DikeProfileCollectionSourcePath] IS OLD.[DikeProfileCollectionSourcePath] " +
                "AND NEW.[ApplyLengthEffectInSection] = 0;" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismEntity);
        }

        private static void AssertGrassCoverErosionInwardsCalculation(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string getNormQuery =
                "JOIN ( " +
                "WITH CalculationGroups AS ( " +
                "SELECT " +
                "CalculationGroupEntityId, " +
                "ParentCalculationGroupEntityId AS OriginalParentId, " +
                "ParentCalculationGroupEntityId AS NextParentId, " +
                "NULL as RootId, " +
                "CASE " +
                "WHEN ParentCalculationGroupEntityId IS NULL " +
                "THEN 1 " +
                "END AS IsRoot " +
                "FROM CalculationGroupEntity " +
                "UNION ALL " +
                "SELECT " +
                "CalculationGroups.CalculationGroupEntityId, " +
                "CalculationGroups.OriginalParentId, " +
                "entity.ParentCalculationGroupEntityId, " +
                "CASE " +
                "WHEN entity.ParentCalculationGroupEntityId IS NULL " +
                "THEN CalculationGroups.NextParentId " +
                "ELSE " +
                "CalculationGroups.RootId " +
                "END, " +
                "NULL " +
                "FROM CalculationGroups " +
                "INNER JOIN CalculationGroupEntity entity " +
                "ON CalculationGroups.NextParentId = entity.CalculationGroupEntityId " +
                ") " +
                "SELECT " +
                "CalculationGroupEntityId as OriginalGroupId, " +
                "CASE " +
                "WHEN IsRoot = 1 " +
                "THEN CalculationGroupEntityId " +
                "ELSE RootId " +
                "END AS FinalGroupId " +
                "FROM CalculationGroups " +
                "WHERE RootId IS NOT NULL OR IsRoot = 1) " +
                "ON OLD.CalculationGroupEntityId = OriginalGroupId " +
                "JOIN ( " +
                "SELECT " +
                "AssessmentSectionEntityId AS failureMechanismAssessmentSectionId, " +
                "CalculationGroupEntityId AS failureMechanismCalculationGroupEntityId " +
                "FROM FailureMechanismEntity) " +
                "ON failureMechanismCalculationGroupEntityId = FinalGroupId " +
                "JOIN ( " +
                "SELECT " +
                "AssessmentSectionEntityId AS sectionId, " +
                "CASE " +
                "WHEN NormativeNormType IS 1 " +
                "THEN LowerLimitNorm " +
                "ELSE SignalingNorm " +
                "END AS Norm " +
                "FROM AssessmentSectionEntity) " +
                "ON sectionId = failureMechanismAssessmentSectionId";

            string validateCalculationWithoutDikeHeightAndOvertoppingRate =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity " +
                "WHERE DikeHeightCalculationType = 1 " +
                "AND OvertoppingRateCalculationType = 1 " +
                ") " +
                "FROM GrassCoverErosionInwardsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity OLD USING(GrassCoverErosionInwardsCalculationEntityId) " +
                $"{getNormQuery} " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[DikeProfileEntityId] IS OLD.[DikeProfileEntityId]" +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[DikeHeight] IS OLD.[DikeHeight] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[CriticalFlowRateMean] IS OLD.[CriticalFlowRateMean] " +
                "AND NEW.[CriticalFlowRateStandardDeviation] IS OLD.[CriticalFlowRateStandardDeviation] " +
                "AND NEW.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldDikeHeightBeCalculated] = 0 " +
                "AND NEW.[DikeHeightTargetProbability] = Norm " +
                "AND NEW.[ShouldDikeHeightIllustrationPointsBeCalculated] = OLD.[ShouldDikeHeightIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldOvertoppingRateBeCalculated] = 0 " +
                "AND NEW.[OvertoppingRateTargetProbability] = Norm " +
                "AND NEW.[ShouldOvertoppingRateIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingRateIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = OLD.[RelevantForScenario] " +
                "AND NEW.[ScenarioContribution] = OLD.[ScenarioContribution]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationWithoutDikeHeightAndOvertoppingRate);

            string validateCalculationWithDikeHeightAndOvertoppingRate =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity " +
                "WHERE DikeHeightCalculationType != 1 " +
                "AND OvertoppingRateCalculationType != 1 " +
                ") " +
                "FROM GrassCoverErosionInwardsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionInwardsCalculationEntity OLD USING(GrassCoverErosionInwardsCalculationEntityId) " +
                $"{getNormQuery} " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[DikeProfileEntityId] IS OLD.[DikeProfileEntityId]" +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[DikeHeight] IS OLD.[DikeHeight] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] IS OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[CriticalFlowRateMean] IS OLD.[CriticalFlowRateMean] " +
                "AND NEW.[CriticalFlowRateStandardDeviation] IS OLD.[CriticalFlowRateStandardDeviation] " +
                "AND NEW.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingOutputIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldDikeHeightBeCalculated] = 1 " +
                "AND NEW.[DikeHeightTargetProbability] = Norm " +
                "AND NEW.[ShouldDikeHeightIllustrationPointsBeCalculated] = OLD.[ShouldDikeHeightIllustrationPointsBeCalculated] " +
                "AND NEW.[ShouldOvertoppingRateBeCalculated] = 1 " +
                "AND NEW.[OvertoppingRateTargetProbability] = Norm " +
                "AND NEW.[ShouldOvertoppingRateIllustrationPointsBeCalculated] = OLD.[ShouldOvertoppingRateIllustrationPointsBeCalculated] " +
                "AND NEW.[RelevantForScenario] = OLD.[RelevantForScenario] " +
                "AND NEW.[ScenarioContribution] = OLD.[ScenarioContribution]; " +
                "DETACH SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateCalculationWithDikeHeightAndOvertoppingRate);
        }

        private static void AssertGrassCoverErosionInwardsOutput(MigratedDatabaseReader reader)
        {
            const string validateDikeHeightOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsDikeHeightOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateDikeHeightOutput);

            const string validateOvertoppingRateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [GrassCoverErosionInwardsOvertoppingRateOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOvertoppingRateOutput);
        }

        private static void AssertGrassCoverErosionInwardsSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "GrassCoverErosionInwardsSectionResultEntity", sourceFilePath);
        }

        #endregion

        # region GrassCoverErosionOutwards

        private static void AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanismEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionOutwardsFailureMechanismMetaEntity " +
                ") " +
                "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionOutwardsFailureMechanismMetaEntity OLD USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[ForeshoreProfileCollectionSourcePath] IS OLD.[ForeshoreProfileCollectionSourcePath] " +
                "AND NEW.[N] IS OLD.[N] " +
                "AND NEW.[ApplyLengthEffectInSection] = 0;" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismEntity);
        }

        private static void AssertGrassCoverErosionOutwardsCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateLowerLimitCalculations =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionOutwardsWaveConditionsCalculationEntity " +
                "WHERE [CategoryType] = 4 " +
                ") " +
                "FROM GrassCoverErosionOutwardsWaveConditionsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionOutwardsWaveConditionsCalculationEntity OLD USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] IS NULL " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] = OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CalculationType] = OLD.[CalculationType] " +
                "AND NEW.[WaterLevelType] = 2;" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateLowerLimitCalculations);

            string validateOtherCalculations =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.GrassCoverErosionOutwardsWaveConditionsCalculationEntity " +
                "WHERE [CategoryType] != 4 " +
                ") " +
                "FROM GrassCoverErosionOutwardsWaveConditionsCalculationEntity NEW " +
                "JOIN SOURCEPROJECT.GrassCoverErosionOutwardsWaveConditionsCalculationEntity OLD USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId) " +
                "WHERE NEW.[CalculationGroupEntityId] = OLD.[CalculationGroupEntityId] " +
                "AND NEW.[ForeshoreProfileEntityId] IS OLD.[ForeshoreProfileEntityId] " +
                "AND NEW.[HydraulicLocationEntityId] IS OLD.[HydraulicLocationEntityId] " +
                "AND NEW.[HydraulicLocationCalculationForTargetProbabilityCollectionEntityId] IS NULL " +
                "AND NEW.\"Order\" = OLD.\"Order\" " +
                "AND NEW.[Name] IS OLD.[Name] " +
                "AND NEW.[Comments] IS OLD.[Comments] " +
                "AND NEW.[UseBreakWater] = OLD.[UseBreakWater] " +
                "AND NEW.[BreakWaterType] = OLD.[BreakWaterType] " +
                "AND NEW.[BreakWaterHeight] IS OLD.[BreakWaterHeight] " +
                "AND NEW.[UseForeshore] = OLD.[UseForeshore] " +
                "AND NEW.[Orientation] IS OLD.[Orientation] " +
                "AND NEW.[UpperBoundaryRevetment] IS OLD.[UpperBoundaryRevetment] " +
                "AND NEW.[LowerBoundaryRevetment] IS OLD.[LowerBoundaryRevetment] " +
                "AND NEW.[UpperBoundaryWaterLevels] IS OLD.[UpperBoundaryWaterLevels] " +
                "AND NEW.[LowerBoundaryWaterLevels] IS OLD.[LowerBoundaryWaterLevels] " +
                "AND NEW.[StepSize] = OLD.[StepSize] " +
                "AND NEW.[CalculationType] = OLD.[CalculationType] " +
                "AND NEW.[WaterLevelType] = 1;" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateOtherCalculations);
        }

        private static void AssertGrassCoverErosionOutwardsSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(reader, "GrassCoverErosionOutwardsSectionResultEntity", sourceFilePath);
        }

        #endregion

        #region DuneErosion

        private static void AssertDuneErosionFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateFailureMechanismEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                "FROM SOURCEPROJECT.DuneErosionFailureMechanismMetaEntity " +
                ") " +
                "FROM DuneErosionFailureMechanismMetaEntity NEW " +
                "JOIN SOURCEPROJECT.DuneErosionFailureMechanismMetaEntity OLD USING(DuneErosionFailureMechanismMetaEntityId) " +
                "WHERE NEW.[FailureMechanismEntityId] = OLD.[FailureMechanismEntityId] " +
                "AND NEW.[N] = OLD.[N];" +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismEntity);
        }

        private static void AssertDuneLocationCalculationCollection(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [DuneLocationCalculationForTargetProbabilityCollectionEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertDuneLocationCalculation(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [DuneLocationCalculationEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertDuneLocationCalculationOutput(MigratedDatabaseReader reader)
        {
            const string validateOutput =
                "SELECT COUNT() = 0 " +
                "FROM [DuneLocationCalculationOutputEntity]; ";
            reader.AssertReturnedDataIsValid(validateOutput);
        }

        private static void AssertDuneErosionFailureMechanismSectionResults(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNonAdoptableFailureMechanismSectionResults(reader, "DuneErosionSectionResultEntity", sourceFilePath);
        }

        #endregion

        #region FailureMechanismSectionResults

        private static void AssertAdoptableFailureMechanismSectionResults(MigratedDatabaseReader reader,
                                                                          string failureMechanismSectionResultEntityName,
                                                                          string sourceFilePath)
        {
            string validateFailureMechanismSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"FROM SOURCEPROJECT.{failureMechanismSectionResultEntityName} " +
                ") " +
                $"FROM {failureMechanismSectionResultEntityName} NEW " +
                $"JOIN SOURCEPROJECT.{failureMechanismSectionResultEntityName} OLD USING({failureMechanismSectionResultEntityName}Id) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[IsRelevant] = 1 " +
                "AND NEW.[InitialFailureMechanismResultType] = 1 " +
                "AND NEW.[ManualInitialFailureMechanismResultSectionProbability] IS NULL " +
                "AND NEW.[FurtherAnalysisType] = 1 " +
                "AND NEW.[RefinedSectionProbability] IS NULL; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismSectionResults);
        }

        private static void AssertAdoptableWithProfileProbabilityFailureMechanismSectionResults(MigratedDatabaseReader reader,
                                                                                                string failureMechanismSectionResultEntityName,
                                                                                                string sourceFilePath)
        {
            string validateFailureMechanismSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"FROM SOURCEPROJECT.{failureMechanismSectionResultEntityName} " +
                ") " +
                $"FROM {failureMechanismSectionResultEntityName} NEW " +
                $"JOIN SOURCEPROJECT.{failureMechanismSectionResultEntityName} OLD USING({failureMechanismSectionResultEntityName}Id) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[IsRelevant] = 1 " +
                "AND NEW.[InitialFailureMechanismResultType] = 1 " +
                "AND NEW.[ManualInitialFailureMechanismResultSectionProbability] IS NULL " +
                "AND NEW.[ManualInitialFailureMechanismResultProfileProbability] IS NULL " +
                "AND NEW.[FurtherAnalysisType] = 1 " +
                "AND NEW.[ProbabilityRefinementType] = 2 " +
                "AND NEW.[RefinedSectionProbability] IS NULL " +
                "AND NEW.[RefinedProfileProbability] IS NULL; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismSectionResults);
        }

        private static void AssertNonAdoptableFailureMechanismSectionResults(MigratedDatabaseReader reader,
                                                                             string failureMechanismSectionResultEntityName,
                                                                             string sourceFilePath)
        {
            string validateFailureMechanismSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"FROM SOURCEPROJECT.{failureMechanismSectionResultEntityName} " +
                ") " +
                $"FROM {failureMechanismSectionResultEntityName} NEW " +
                $"JOIN SOURCEPROJECT.{failureMechanismSectionResultEntityName} OLD USING({failureMechanismSectionResultEntityName}Id) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[IsRelevant] = 1 " +
                "AND NEW.[InitialFailureMechanismResultType] = 1 " +
                "AND NEW.[ManualInitialFailureMechanismResultSectionProbability] IS NULL " +
                "AND NEW.[FurtherAnalysisType] = 1 " +
                "AND NEW.[RefinedSectionProbability] IS NULL; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismSectionResults);
        }

        private static void AssertNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(MigratedDatabaseReader reader,
                                                                                                   string failureMechanismSectionResultEntityName,
                                                                                                   string sourceFilePath)
        {
            string validateFailureMechanismSectionResults =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "(" +
                "SELECT COUNT() " +
                $"FROM SOURCEPROJECT.{failureMechanismSectionResultEntityName} " +
                ") " +
                $"FROM {failureMechanismSectionResultEntityName} NEW " +
                $"JOIN SOURCEPROJECT.{failureMechanismSectionResultEntityName} OLD USING({failureMechanismSectionResultEntityName}Id) " +
                "WHERE NEW.[FailureMechanismSectionEntityId] = OLD.[FailureMechanismSectionEntityId] " +
                "AND NEW.[IsRelevant] = 1 " +
                "AND NEW.[InitialFailureMechanismResultType] = 1 " +
                "AND NEW.[ManualInitialFailureMechanismResultSectionProbability] IS NULL " +
                "AND NEW.[ManualInitialFailureMechanismResultProfileProbability] IS NULL " +
                "AND NEW.[FurtherAnalysisType] = 1 " +
                "AND NEW.[RefinedSectionProbability] IS NULL " +
                "AND NEW.[RefinedProfileProbability] IS NULL; " +
                "DETACH SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateFailureMechanismSectionResults);
        }

        #endregion
    }
}