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
using System.Collections.ObjectModel;
using System.ComponentModel;
using Application.Ringtoets.Migration.Core;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    public class MigrationTo181IntegrationTest
    {
        /// <summary>
        /// Enum to indicate the normative norm type.
        /// </summary>
        private enum NormativeNormType
        {
            /// <summary>
            /// Represents the lower limit norm.
            /// </summary>
            LowerLimitNorm = 1,

            /// <summary>
            /// Represents the signaling norm.
            /// </summary>
            SignalingNorm = 2
        }

        private const string newVersion = "18.1";

        [Test]
        public void Given173Project_WhenUpgradedTo181_ThenProjectAsExpected()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core,
                                                               "FullTestProject173.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given173Project_WhenUpgradedTo181_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given173Project_WhenUpgradedTo181_ThenProjectAsExpected), ".log"));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
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
                    AssertHydraulicBoundaryLocationsProperties(reader, sourceFilePath);
                    AssertHydraulicBoundaryLocationsOnAssessmentSection(reader, sourceFilePath);
                    AssertHydraulicBoundaryLocationsOnGrassCoverErosionOutwardsFailureMechanism(reader, sourceFilePath);

                    AssertPipingSoilLayers(reader);
                    AssertHydraRingPreprocessor(reader);
                    AssertStabilityStoneCoverFailureMechanism(reader);
                    AssertMacroStabilityOutwardsFailureMechanism(reader);
                    AssertPipingStructureFailureMechanism(reader);
                    AssertWaveImpactAsphaltCoverFailureMechanism(reader);
                    AssertGrassCoverErosionInwardsOutput(reader, sourceFilePath);
                    AssertClosingStructuresOutput(reader, sourceFilePath);
                    AssertHeightStructuresOutput(reader, sourceFilePath);
                    AssertStabilityPointStructuresOutput(reader, sourceFilePath);

                    AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(reader, sourceFilePath );
                }

                AssertLogDatabase(logFilePath);
            }
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var tables = new[]
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
                "DuneLocationEntity",
                "DuneLocationOutputEntity",
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
                "HydraRingPreprocessorEntity",
                "HydraulicLocationEntity",
                "IllustrationPointResultEntity",
                "MacroStabilityInwardsCalculationEntity",
                "MacroStabilityInwardsCalculationOutputEntity",
                "MacroStabilityInwardsCharacteristicPointEntity",
                "MacroStabilityInwardsFailureMechanismMetaEntity",
                "MacroStabilityInwardsPreconsolidationStressEntity",
                "MacroStabilityInwardsSectionResultEntity",
                "MacroStabilityInwardsSemiProbabilisticOutputEntity",
                "MacroStabilityInwardsSoilLayerOneDEntity",
                "MacroStabilityInwardsSoilLayerTwoDEntity",
                "MacroStabilityInwardsSoilProfileOneDEntity",
                "MacroStabilityInwardsSoilProfileTwoDEntity",
                "MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity",
                "MacroStabilityInwardsStochasticSoilProfileEntity",
                "MacroStabilityOutwardsSectionResultEntity",
                "MicrostabilitySectionResultEntity",
                "PipingCalculationEntity",
                "PipingCalculationOutputEntity",
                "PipingCharacteristicPointEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingSectionResultEntity",
                "PipingSemiProbabilisticOutputEntity",
                "PipingSoilLayerEntity",
                "PipingSoilProfileEntity",
                "PipingStochasticSoilProfileEntity",
                "PipingStructureSectionResultEntity",
                "ProjectEntity",
                "StabilityPointStructureEntity",
                "StabilityPointStructuresCalculationEntity",
                "StabilityPointStructuresFailureMechanismMetaEntity",
                "StabilityPointStructuresOutputEntity",
                "StabilityPointStructuresSectionResultEntity",
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
                    new MigrationLogMessage("17.3", newVersion, "Gevolgen van de migratie van versie 17.3 naar versie 18.1:"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "* Traject: 'PipingSoilLayer'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Piping'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-0.125' voor de variatiecoëfficiënt van parameter 'd70' van ondergrondlaag 'DiameterD70Variation' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-1.0' voor het gemiddelde van parameter 'Doorlatendheid' van ondergrondlaag 'PermeabilityMean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-1.0e-06' voor het gemiddelde van parameter 'd70' van ondergrondlaag 'DiameterD70Mean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-10.0' voor de standaardafwijking van parameter 'Verzadigd gewicht' van ondergrondlaag 'BelowPhreaticLevelDeviation' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '-10.0' voor de variatiecoëfficiënt van parameter 'Doorlatendheid' van ondergrondlaag 'PermeabilityVariation' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '0.0' voor het gemiddelde van parameter 'Verzadigd gewicht' van ondergrondlaag 'BelowPhreaticLevelMean' is ongeldig en is veranderd naar NaN."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - De waarde '15.0' voor de verschuiving van parameter 'Verzadigd gewicht' van ondergrondlaag 'BelowPhreaticLevelShift' is ongeldig en is veranderd naar NaN."),
                    messages[i]);
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"18.1\";";
            reader.AssertReturnedDataIsValid(validateVersion);
        }

        private static void AssertDatabase(MigratedDatabaseReader reader)
        {
            const string validateForeignKeys =
                "PRAGMA foreign_keys;";
            reader.AssertReturnedDataIsValid(validateForeignKeys);
        }

        private static void AssertHydraRingPreprocessor(MigratedDatabaseReader reader)
        {
            const string validatePreprocessorSettings =
                "SELECT COUNT() = 0 " +
                "FROM [HydraRingPreprocessorEntity];";
            reader.AssertReturnedDataIsValid(validatePreprocessorSettings);
        }

        private static void AssertPipingSoilLayers(MigratedDatabaseReader reader)
        {
            const string validateBelowPhreaticLevel =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [BelowPhreaticLevelMean] < [BelowPhreaticLevelShift] " +
                "OR [BelowPhreaticLevelMean] <= 0 " +
                "OR [BelowPhreaticLevelDeviation] < 0;";
            reader.AssertReturnedDataIsValid(validateBelowPhreaticLevel);

            const string validateDiameter70 =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [DiameterD70Mean] <= 0 " +
                "OR [DiameterD70CoefficientOfVariation] < 0;";
            reader.AssertReturnedDataIsValid(validateDiameter70);

            const string validatePermeability =
                "SELECT COUNT() = 0 " +
                "FROM PipingSoilLayerEntity " +
                "WHERE [PermeabilityMean] <= 0 " +
                "OR [PermeabilityCoefficientOfVariation] < 0;";
            reader.AssertReturnedDataIsValid(validatePermeability);
        }

        private static void AssertStabilityStoneCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateStabilityStoneCoverFailureMechanisms =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityStoneCoverFailureMechanismMetaEntity] " +
                "WHERE [N] IS NOT 4;";
            reader.AssertReturnedDataIsValid(validateStabilityStoneCoverFailureMechanisms);
        }

        private static void AssertMacroStabilityOutwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateMacroStabilityOutwardsFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 13) " +
                "FROM [MacroStabilityOutwardsFailureMechanismMetaEntity] " +
                "WHERE [A] = 0.033 " +
                "AND [FailureMechanismEntityId] IN " +
                "(SELECT [FailureMechanismEntityId] FROM [FailureMechanismEntity] WHERE [FailureMechanismType] = 13);";
            reader.AssertReturnedDataIsValid(validateMacroStabilityOutwardsFailureMechanisms);
        }

        private static void AssertPipingStructureFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validatePipingStructureFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 11) " +
                "FROM [PipingStructureFailureMechanismMetaEntity] " +
                "WHERE [N] = 1.0 " +
                "AND [FailureMechanismEntityId] IN " +
                "(SELECT [FailureMechanismEntityId] FROM [FailureMechanismEntity] WHERE [FailureMechanismType] = 11);";
            reader.AssertReturnedDataIsValid(validatePipingStructureFailureMechanisms);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateWaveImpactAsphaltCoverFailureMechanisms =
                "SELECT COUNT() = 0 " +
                "FROM [WaveImpactAsphaltCoverFailureMechanismMetaEntity] " +
                "WHERE [DeltaL] IS NOT 1000;";
            reader.AssertReturnedDataIsValid(validateWaveImpactAsphaltCoverFailureMechanisms);
        }

        private static void AssertGrassCoverErosionInwardsOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateGrassCoverErosionInwardsOutputEntities =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity) " +
                "FROM GrassCoverErosionInwardsOutputEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity OLD USING(GrassCoverErosionInwardsOutputEntityId) " +
                "WHERE NEW.GrassCoverErosionInwardsCalculationEntityId = OLD.GrassCoverErosionInwardsCalculationEntityId " +
                "AND NEW.GeneralResultFaultTreeIllustrationPointEntityId IS OLD.GeneralResultFaultTreeIllustrationPointEntityId " +
                "AND NEW.IsOvertoppingDominant = OLD.IsOvertoppingDominant " +
                "AND NEW.WaveHeight IS OLD.WaveHeight " +
                "AND NEW.Reliability IS OLD.Reliability;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateGrassCoverErosionInwardsOutputEntities);
        }

        private static void AssertClosingStructuresOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateClosingStructuresOutputEntities =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].ClosingStructuresOutputEntity) " +
                "FROM ClosingStructuresOutputEntity NEW " +
                "JOIN [SOURCEPROJECT].ClosingStructuresOutputEntity OLD USING(ClosingStructuresOutputEntityId) " +
                "WHERE NEW.ClosingStructuresCalculationEntityId = OLD.ClosingStructuresCalculationEntityId " +
                "AND NEW.GeneralResultFaultTreeIllustrationPointEntityId IS OLD.GeneralResultFaultTreeIllustrationPointEntityId " +
                "AND NEW.Reliability IS OLD.Reliability;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateClosingStructuresOutputEntities);
        }

        private static void AssertHeightStructuresOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateHeightStructuresOutputEntities =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HeightStructuresOutputEntity) " +
                "FROM HeightStructuresOutputEntity NEW " +
                "JOIN [SOURCEPROJECT].HeightStructuresOutputEntity OLD USING(HeightStructuresOutputEntityId) " +
                "WHERE NEW.HeightStructuresCalculationEntityId = OLD.HeightStructuresCalculationEntityId " +
                "AND NEW.GeneralResultFaultTreeIllustrationPointEntityId IS OLD.GeneralResultFaultTreeIllustrationPointEntityId " +
                "AND NEW.Reliability IS OLD.Reliability;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHeightStructuresOutputEntities);
        }

        private static void AssertStabilityPointStructuresOutput(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateStabilityPointStructuresOutputEntities =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity) " +
                "FROM StabilityPointStructuresOutputEntity NEW " +
                "JOIN [SOURCEPROJECT].StabilityPointStructuresOutputEntity OLD ON OLD.StabilityPointStructuresOutputEntity = NEW.StabilityPointStructuresOutputEntityId " +
                "WHERE NEW.StabilityPointStructuresCalculationEntityId = OLD.StabilityPointStructuresCalculationEntityId " +
                "AND NEW.GeneralResultFaultTreeIllustrationPointEntityId IS OLD.GeneralResultFaultTreeIllustrationPointEntityId " +
                "AND NEW.Reliability IS OLD.Reliability;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateStabilityPointStructuresOutputEntities);
        }

        private static void AssertAssessmentSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateAssessmentSectionEntities =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].AssessmentSectionEntity) " +
                "FROM AssessmentSectionEntity NEW " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity AS OLD USING (AssessmentSectionEntityId) " +
                "WHERE NEW.ProjectEntityId = OLD.ProjectEntityId " +
                "AND NEW.Id = OLD.Id " +
                "AND NEW.Name IS OLD.Name " +
                "AND NEW.Comments IS OLD.Comments " +
                "AND NEW.LowerLimitNorm = OLD.LowerLimitNorm " +
                "AND NEW.SignalingNorm = OLD.SignalingNorm " +
                "AND NEW.NormativeNormType = OLD.NormativeNormType " +
                "AND NEW.HydraulicDatabaseVersion IS OLD.HydraulicDatabaseVersion " +
                "AND NEW.HydraulicDatabaseLocation IS OLD.HydraulicDatabaseLocation " +
                "AND NEW.Composition = OLD.Composition " +
                "AND NEW.ReferenceLinePointXml IS OLD.ReferenceLinePointXml " +
                "AND NEW.\"Order\" = OLD.\"Order\"; " +
                "DETACH DATABASE SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateAssessmentSectionEntities);
        }

        private static void AssertHydraulicBoundaryLocationsProperties(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMigratedHyraulicBoundaryLocations = $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                                                               "SELECT " +
                                                               "COUNT() = (SELECT COUNT() FROM[SOURCEPROJECT].HydraulicLocationEntity) " +
                                                               "FROM HydraulicLocationEntity NEW " +
                                                               "JOIN[SourceProject].HydraulicLocationEntity AS OLD USING(HydraulicLocationEntityId) " +
                                                               "WHERE NEW.AssessmentSectionEntityId = OLD.AssessmentSectionEntityId " +
                                                               "AND NEW.LocationId = OLD.LocationId " +
                                                               "AND NEW.Name = OLD.Name " +
                                                               "AND NEW.LocationX = OLD.LocationX " +
                                                               "AND NEW.LocationY = OLD.LocationY " +
                                                               "AND NEW.\"Order\" = OLD.\"Order\"; " +
                                                               "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMigratedHyraulicBoundaryLocations);

            string validateNrOfHydraulicLocationCalculations =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = 0 " +
                "FROM " +
                "( " +
                "SELECT " +
                "COUNT(distinct HydraulicLocationCalculationEntityId) AS NrOfCalculationsPerLocation " +
                "FROM [SOURCEPROJECT].HydraulicLocationEntity " +
                "LEFT JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationEntityId) " +
                "GROUP BY HydraulicLocationEntityId " +
                ") " +
                "WHERE NrOfCalculationsPerLocation IS NOT 14; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateNrOfHydraulicLocationCalculations);

            const string validateHydraulicBoundaryCalculationInputValues = "SELECT " +
                                                                           "COUNT() = 0 " +
                                                                           "FROM HydraulicLocationCalculationEntity " +
                                                                           "WHERE ShouldIllustrationPointsBeCalculated != 0 AND ShouldIllustrationPointsBeCalculated != 1";
            reader.AssertReturnedDataIsValid(validateHydraulicBoundaryCalculationInputValues);

            string validateNrOfHydraulicLocationCalculationOutputs =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() + " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                ") " +
                "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity " +
                ") " +
                "FROM HydraulicLocationOutputEntity; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateNrOfHydraulicLocationCalculationOutputs);
        }

        private static void AssertHydraulicBoundaryLocationsOnAssessmentSection(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var queryGenerator = new HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator(sourceFilePath);
            AssertDesignWaterLevelCalculationEntitiesOnAssessmentSection(reader, queryGenerator);
            AssertWaveHeightCalculationEntitiesOnAssessmentSection(reader, queryGenerator);
        }

        private static void AssertHydraulicBoundaryLocationsOnGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var queryGenerator = new HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator(sourceFilePath);

            AssertDesignWaterLevelCalculationEntitiesOnGrassCoverErosionOutwardsFailureMechanism(reader, queryGenerator);
            AssertWaveHeightCalculationEntitiesOnGrassCoverErosionOutwardsFailureMechanism(reader, queryGenerator);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity) " +
                "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity new " +
                "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity source USING (GrassCoverErosionOutwardsFailureMechanismMetaEntityId) " +
                "WHERE new.FailureMechanismEntityId = source.FailureMechanismEntityId " +
                "AND new.N = source.N " +
                "AND new.ForeshoreProfileCollectionSourcePath IS source.ForeshoreProfileCollectionSourcePath; " +
                "DETACH DATABASE SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        #region  Migrated Hydraulic Boundary Locations on Assessment section

        private static void AssertWaveHeightCalculationEntitiesOnAssessmentSection(MigratedDatabaseReader reader,
                                                                                   HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator queryGenerator)
        {
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerAssessmentSectionValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerAssessmentSectionValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerAssessmentSectionValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerAssessmentSectionValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(
                                                 NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(
                                                 NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationOutputsValidationQuery(
                                                 NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationOutputsValidationQuery(
                                                 NormativeNormType.SignalingNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.GetNewCalculationOutputsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedLowerLimitNorm));
            reader.AssertReturnedDataIsValid(HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.GetNewCalculationOutputsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedSignalingNorm));
        }

        private static void AssertDesignWaterLevelCalculationEntitiesOnAssessmentSection(MigratedDatabaseReader reader,
                                                                                         HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator queryGenerator)
        {
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerAssessmentSectionValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerAssessmentSectionValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerAssessmentSectionValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerAssessmentSectionValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(
                                                 NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(
                                                 NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(
                                                 NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(
                                                 NormativeNormType.SignalingNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.GetNewCalculationOutputsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedLowerLimitNorm));
            reader.AssertReturnedDataIsValid(HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.GetNewCalculationOutputsValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedSignalingNorm));
        }

        /// <summary>
        /// Class to generate queries which can be used if the hydraulic boundary locations 
        /// are correctly migrated on the assessment section level.
        /// </summary>
        private class HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator
        {
            /// <summary>
            /// Enum to indicate the hydraulic location calculation type.
            /// </summary>
            public enum CalculationType
            {
                /// <summary>
                /// Represents the water level calculations for the factorized signaling norm.
                /// </summary>
                WaterLevelCalculationsForFactorizedSignalingNorm = 1,

                /// <summary>
                /// Represents the water level calculations for the signaling norm.
                /// </summary>
                WaterLevelCalculationsForSignalingNorm = 2,

                /// <summary>
                /// Represents the water level calculations for the lower limit norm.
                /// </summary>
                WaterLevelCalculationsForLowerLimitNorm = 3,

                /// <summary>
                /// Represents the water level calculations for the factorized lower limit norm.
                /// </summary>
                WaterLevelCalculationsForFactorizedLowerLimitNorm = 4,

                /// <summary>
                /// Represents the wave height calculations for the factorized signaling norm.
                /// </summary>
                WaveHeightCalculationsForFactorizedSignalingNorm = 5,

                /// <summary>
                /// Represents the wave height calculations for the signaling norm.
                /// </summary>
                WaveHeightCalculationsForSignalingNorm = 6,

                /// <summary>
                /// Represents the wave height calculations for the lower limit norm.
                /// </summary>
                WaveHeightCalculationsForLowerLimitNorm = 7,

                /// <summary>
                /// Represents the wave height calculations for the factorized lower limit norm.
                /// </summary>
                WaveHeightCalculationsForFactorizedLowerLimitNorm = 8
            }

            private readonly string sourceFilePath;

            /// <summary>
            /// Creates a new instance of <see cref="HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator"/>.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the database to be verified.</param>
            /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFilePath"/>
            /// is <c>null</c> or empty.</exception>
            public HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator(string sourceFilePath)
            {
                if (string.IsNullOrEmpty(sourceFilePath))
                {
                    throw new ArgumentException(@"Sourcefile path cannot be null or empty",
                                                nameof(sourceFilePath));
                }

                this.sourceFilePath = sourceFilePath;
            }

            /// <summary>
            /// Generates a query to validate the number of created hydraulic boundary location calculations per assessment section.
            /// </summary>
            /// <param name="calculationType">The type of calculation that should be validated.</param>
            /// <returns>The query to validate the number of hydraulic boundary location calculations per assessment section.</returns>
            public string GetNrOfHydraulicBoundaryLocationCalculationsPerAssessmentSectionValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM " +
                       "(" +
                       "SELECT " +
                       "sourceAse.AssessmentSectionEntityId, " +
                       "COUNT(distinct HydraulicLocationEntityId) AS OldCount, " +
                       "NewCount " +
                       "FROM [SOURCEPROJECT].HydraulicLocationEntity sourceHle " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity sourceAse ON sourceHle.AssessmentSectionEntityId = sourceAse.AssessmentSectionEntityId " +
                       "LEFT JOIN " +
                       "(" +
                       "SELECT " +
                       "ase.AssessmentSectionEntityId, " +
                       "COUNT(distinct HydraulicLocationEntityId) AS NewCount " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       "GROUP BY ase.AssessmentSectionEntityId " +
                       ") USING(AssessmentSectionEntityId) " +
                       "GROUP BY sourceAse.AssessmentSectionEntityId " +
                       ") " +
                       "WHERE OldCount IS NOT NewCount; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary calculation input for the design water level calculations 
            /// are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation input for the design water level calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is an unsupported value,
            /// but is unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM AssessmentSectionEntity ase " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON ase.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING (HydraulicLocationCalculationEntityId) " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity OLD USING (HydraulicLocationEntityId) " +
                       $"WHERE OLD.ShouldDesignWaterLevelIllustrationPointsBeCalculated != NEW.ShouldIllustrationPointsBeCalculated AND ase.NormativeNormType = {(int) normType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary location calculation outputs related to the design water level calculations
            /// are migrated correctly to the corresponding calculation entities.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation outputs.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is an unsupported value,
            /// but is unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() =  " +
                       "( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity sourceHlo " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity USING (HydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity sourceAse USING (AssessmentSectionEntityId) " +
                       $"WHERE sourceHlo.HydraulicLocationOutputType = 1 AND sourceAse.NormativeNormType = {(int) normType} " +
                       ") " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       GetHydralicLocationCalculationOutputValidationSubQuery() +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary calculation input for the wave height calculations 
            /// are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation input for the wave height calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but is unsupported.</exception>
            public string GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToWaveHeightCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM AssessmentSectionEntity ase " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON ase.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING (HydraulicLocationCalculationEntityId) " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity OLD USING (HydraulicLocationEntityId) " +
                       $"WHERE OLD.ShouldWaveHeightIllustrationPointsBeCalculated != NEW.ShouldIllustrationPointsBeCalculated AND ase.NormativeNormType = {(int) normType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary location calculation outputs related to the wave height calculations 
            /// are migrated correctly to the corresponding calculation entities.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation outputs.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is an unsupported value,
            /// but is unsupported.</exception>
            public string GetMigratedWaveHeightCalculationOutputsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() =  " +
                       "( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity sourceHlo " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity USING (HydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity sourceAse USING (AssessmentSectionEntityId) " +
                       $"WHERE sourceHlo.HydraulicLocationOutputType = 2 AND sourceAse.NormativeNormType = {(int) normType} " +
                       ") " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       GetHydralicLocationCalculationOutputValidationSubQuery() +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the new hydraulic boundary location calculations that are not based on migrated data.
            /// </summary>
            /// <param name="calculationType">The type of calculation on which the input should be validated.</param>
            /// <returns>The query to validate the hydraulic boundary location calculation input.</returns>
            public string GetNewCalculationsValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationEntity) " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       "WHERE ShouldIllustrationPointsBeCalculated = 0;" +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the new hydraulic boundary location calculation outputs that are not based on migrated data.
            /// </summary>
            /// <param name="calculationType">The type of calculation on which the output should be validated.</param>
            /// <returns>The query to validate the hydraulic boundary location calculation input.</returns>
            public static string GetNewCalculationOutputsValidationQuery(CalculationType calculationType)
            {
                return "SELECT  " +
                       "COUNT() = 0 " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       "JOIN HydraulicLocationOutputEntity USING (HydraulicLocationCalculationEntityId); ";
            }

            private static string GetHydraulicLocationCalculationsFromCollectionQuery(CalculationType calculationType)
            {
                return "FROM AssessmentSectionEntity ase " +
                       $"JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id " +
                       "= hlcce.HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId) ";
            }

            private static string GetHydralicLocationCalculationOutputValidationSubQuery()
            {
                return "JOIN HydraulicLocationOutputEntity NEW USING (HydraulicLocationCalculationEntityId) " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity OLD ON " +
                       "NEW.GeneralResultSubMechanismIllustrationPointEntityId IS OLD.GeneralResultSubMechanismIllustrationPointEntityId " +
                       "AND NEW.Result IS OLD.Result " +
                       "AND NEW.TargetProbability IS OLD.TargetProbability " +
                       "AND NEW.TargetReliability IS OLD.TargetReliability " +
                       "AND NEW.CalculatedProbability IS OLD.CalculatedProbability " +
                       "AND NEW.CalculatedReliability IS OLD.CalculatedReliability " +
                       "AND NEW.CalculationConvergence = OLD.CalculationConvergence; ";
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding design water level calculation from <see cref="CalculationType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="CalculationType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is an unsupported value,
            /// but is unsupported.</exception>
            private static CalculationType ConvertToDesignWaterLevelCalculationType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.LowerLimitNorm:
                        return CalculationType.WaterLevelCalculationsForLowerLimitNorm;
                    case NormativeNormType.SignalingNorm:
                        return CalculationType.WaterLevelCalculationsForSignalingNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding wave height calculation from <see cref="CalculationType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="CalculationType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is an unsupported value,
            /// but is unsupported.</exception>
            private static CalculationType ConvertToWaveHeightCalculationType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.LowerLimitNorm:
                        return CalculationType.WaveHeightCalculationsForLowerLimitNorm;
                    case NormativeNormType.SignalingNorm:
                        return CalculationType.WaveHeightCalculationsForSignalingNorm;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion

        #region Migrated Hydraulic Boundary Locations on Grass Cover Erosion Outwards Failure Mechanism

        private static void AssertDesignWaterLevelCalculationEntitiesOnGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader,
                                                                                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator queryGenerator)
        {
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerFailureMechanism(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerFailureMechanism(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerFailureMechanism(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(NormativeNormType.LowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.GetNewCalculationOutputsValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm));
        }

        private static void AssertWaveHeightCalculationEntitiesOnGrassCoverErosionOutwardsFailureMechanism(MigratedDatabaseReader reader,
                                                                                                           HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator queryGenerator)
        {
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerFailureMechanism(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerFailureMechanism(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetNrOfHydraulicBoundaryLocationCalculationsPerFailureMechanism(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationOutputsValidationQuery(NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationOutputsValidationQuery(NormativeNormType.LowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetNewCalculationsValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.GetNewCalculationOutputsValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm));
        }

        private class HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator
        {
            /// <summary>
            /// Enum to indicate the hydraulic location calculation type.
            /// </summary>
            public enum CalculationType
            {
                /// <summary>
                /// Represents the water level calculations for the mechanism specific factorized signaling norm.
                /// </summary>
                WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm = 1,

                /// <summary>
                /// Represents the water level calculations for the mechanism specific signaling norm.
                /// </summary>
                WaterLevelCalculationsForMechanismSpecificSignalingNorm = 2,

                /// <summary>
                /// Represents the water level calculations for the mechanism specific lower limit norm.
                /// </summary>
                WaterLevelCalculationsForMechanismSpecificLowerLimitNorm = 3,

                /// <summary>
                /// Represents the wave height calculations for the mechanism specific factorized signaling norm.
                /// </summary>
                WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm = 4,

                /// <summary>
                /// Represents the wave height calculations for the mechanism specific signaling norm.
                /// </summary>
                WaveHeightCalculationsForMechanismSpecificSignalingNorm = 5,

                /// <summary>
                /// Represents the wave height calculations for the mechanism specific lower limit norm.
                /// </summary>
                WaveHeightCalculationsForMechanismSpecificLowerLimitNorm = 6
            }

            private readonly string sourceFilePath;

            public HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator(string sourceFilePath)
            {
                if (string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    throw new ArgumentException(@"Sourcefile path cannot be null or empty",
                                                nameof(sourceFilePath));
                }

                this.sourceFilePath = sourceFilePath;
            }

            /// <summary>
            /// Generates a query to validate the number of created hydraulic boundary location calculations per failure mechanism section.
            /// </summary>
            /// <param name="calculationType">The type of calculation that should be validated.</param>
            /// <returns>The query to validate the number of hydraulic boundary location calculations per assessment section.</returns>
            public string GetNrOfHydraulicBoundaryLocationCalculationsPerFailureMechanism(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = 0 " +
                       "FROM " +
                       "( " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "COUNT(distinct GrassCoverErosionOutwardsHydraulicLocationEntityId) as OldCount, " +
                       "NEWCount " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING (FailureMechanismEntityId) " +
                       "LEFT JOIN " +
                       "( " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "COUNT(distinct HydraulicLocationEntityId) AS NewCount " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity " +
                       "JOIN HydraulicLocationCalculationCollectionEntity " +
                       $"ON HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId) " +
                       "JOIN FailureMechanismEntity USING (FailureMechanismEntityId) " +
                       "GROUP BY GrassCoverErosionOutwardsFailureMechanismMetaEntityId " +
                       ") USING (FailureMechanismEntityId) " +
                       "GROUP BY FailureMechanismEntityId " +
                       ") " +
                       "WHERE OldCount IS NOT NewCount; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary calculation input for the wave height calculations 
            /// are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation input for the wave height calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but is unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = 0 " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme  " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON gceofmme.HydraulicLocationCalculationCollectionEntity{(int)calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING (HydraulicLocationCalculationEntityId)  " +
                       "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity OLD ON OLD.GrassCoverErosionOutwardsHydraulicLocationEntityId = NEW.HydraulicLocationEntityId  " +
                       "JOIN FailureMechanismEntity USING (FailureMechanismEntityId) " +
                       "JOIN AssessmentSectionEntity ase USING (AssessmentSectionEntityId) " +
                       $"WHERE OLD.ShouldDesignWaterLevelIllustrationPointsBeCalculated != NEW.ShouldIllustrationPointsBeCalculated AND ase.NormativeNormType = {(int)normType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary location calculation outputs related to the design water level calculations
            /// are migrated correctly to the corresponding calculation entities.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation outputs.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is an unsupported value,
            /// but is unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = ( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                       "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity USING (GrassCoverErosionOutwardsHydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING (FailureMechanismEntityId) " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                       $"WHERE HydraulicLocationOutputType = 1 AND NormativeNormType = {(int) normType}" +
                       ") " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON gceofmme.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId) " +
                       "JOIN HydraulicLocationOutputEntity NEW USING (HydraulicLocationCalculationEntityId) " +
                       "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity OLD ON NEW.GeneralResultSubMechanismIllustrationPointEntityId IS OLD.GeneralResultSubMechanismIllustrationPointEntityId " +
                       "AND NEW.Result IS OLD.Result " +
                       "AND NEW.TargetProbability IS OLD.TargetProbability " +
                       "AND NEW.TargetReliability IS OLD.TargetReliability " +
                       "AND NEW.CalculatedProbability IS OLD.CalculatedProbability " +
                       "AND NEW.CalculatedReliability IS OLD.CalculatedReliability " +
                       "AND NEW.CalculationConvergence = OLD.CalculationConvergence; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary calculation input for the wave height calculations 
            /// are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation input for the wave height calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but is unsupported.</exception>
            public string GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToWaveHeightCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = 0 " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme  " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON gceofmme.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING (HydraulicLocationCalculationEntityId)  " +
                       "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity OLD ON OLD.GrassCoverErosionOutwardsHydraulicLocationEntityId = NEW.HydraulicLocationEntityId  " +
                       "JOIN FailureMechanismEntity USING (FailureMechanismEntityId) " +
                       "JOIN AssessmentSectionEntity ase USING (AssessmentSectionEntityId) " +
                       $"WHERE OLD.ShouldWaveHeightIllustrationPointsBeCalculated != NEW.ShouldIllustrationPointsBeCalculated AND ase.NormativeNormType = {(int) normType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the hydraulic boundary location calculation outputs related to the design water level calculations
            /// are migrated correctly to the corresponding calculation entities.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the hydraulic boundary location calculation outputs.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is an unsupported value,
            /// but is unsupported.</exception>
            public string GetMigratedWaveHeightCalculationOutputsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToWaveHeightCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = ( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                       "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity USING (GrassCoverErosionOutwardsHydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING (FailureMechanismEntityId) " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                       $"WHERE HydraulicLocationOutputType = 2 AND NormativeNormType = {(int) normType}" +
                       ") " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON gceofmme.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId) " +
                       "JOIN HydraulicLocationOutputEntity NEW USING (HydraulicLocationCalculationEntityId) " +
                       "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity OLD ON NEW.GeneralResultSubMechanismIllustrationPointEntityId IS OLD.GeneralResultSubMechanismIllustrationPointEntityId " +
                       "AND NEW.Result IS OLD.Result " +
                       "AND NEW.TargetProbability IS OLD.TargetProbability " +
                       "AND NEW.TargetReliability IS OLD.TargetReliability " +
                       "AND NEW.CalculatedProbability IS OLD.CalculatedProbability " +
                       "AND NEW.CalculatedReliability IS OLD.CalculatedReliability " +
                       "AND NEW.CalculationConvergence = OLD.CalculationConvergence; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the new hydraulic boundary location calculations that are not based on migrated data.
            /// </summary>
            /// <param name="calculationType">The type of calculation on which the input should be validated.</param>
            /// <returns>The query to validate the hydraulic boundary location calculation input.</returns>
            public string GetNewCalculationsValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity) " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity " +
                       "JOIN HydraulicLocationCalculationCollectionEntity " +
                       $"ON HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationCalculationEntity hlce USING (HydraulicLocationCalculationEntityId) " +
                       "WHERE hlce.ShouldIllustrationPointsBeCalculated = 0;" +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the new hydraulic boundary location calculation outputs that are not based on migrated data.
            /// </summary>
            /// <param name="calculationType">The type of calculation on which the output should be validated.</param>
            /// <returns>The query to validate the hydraulic boundary location calculation input.</returns>
            public static string GetNewCalculationOutputsValidationQuery(CalculationType calculationType)
            {
                return "SELECT  " +
                       "COUNT() = 0 " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity " +
                       "JOIN HydraulicLocationCalculationCollectionEntity " +
                       $"ON HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId) " +
                       "JOIN HydraulicLocationOutputEntity USING (HydraulicLocationCalculationEntityId); ";
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding design water level calculation from <see cref="CalculationType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="CalculationType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is an unsupported value,
            /// but is unsupported.</exception>
            private static CalculationType ConvertToDesignWaterLevelCalculationType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.LowerLimitNorm:
                        return CalculationType.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm;
                    case NormativeNormType.SignalingNorm:
                        return CalculationType.WaterLevelCalculationsForMechanismSpecificSignalingNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding wave height calculation from <see cref="CalculationType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="CalculationType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is an unsupported value,
            /// but is unsupported.</exception>
            private static CalculationType ConvertToWaveHeightCalculationType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.LowerLimitNorm:
                        return CalculationType.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm;
                    case NormativeNormType.SignalingNorm:
                        return CalculationType.WaveHeightCalculationsForMechanismSpecificSignalingNorm;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion
    }
}