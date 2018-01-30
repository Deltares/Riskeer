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

using System.Collections.ObjectModel;
using Application.Ringtoets.Migration.Core;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    public class MigrationTo181IntegrationTest
    {
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

                    AssertHydraulicBoundaryLocations(reader, sourceFilePath);

                    AssertPipingSoilLayers(reader);
                    AssertHydraRingPreprocessor(reader);
                    AssertStabilityStoneCoverFailureMechanism(reader);
                    AssertMacroStabilityOutwardsFailureMechanism(reader);
                    AssertPipingStructureFailureMechanism(reader);
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
                "GrassCoverErosionOutwardsHydraulicLocationEntity",
                "GrassCoverErosionOutwardsHydraulicLocationOutputEntity",
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
                "HydraulicLocationOutputEntity",
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
            const string validateStabilityStoneCoverFailureMechanism =
                "SELECT COUNT() = 0 " +
                "FROM [StabilityStoneCoverFailureMechanismMetaEntity] " +
                "WHERE [N] IS NOT 4;";
            reader.AssertReturnedDataIsValid(validateStabilityStoneCoverFailureMechanism);
        }

        private static void AssertMacroStabilityOutwardsFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 13) " +
                "FROM [MacroStabilityOutwardsFailureMechanismMetaEntity] " +
                "WHERE [A] = 0.033 " +
                "AND [FailureMechanismEntityId] IN " +
                "(SELECT [FailureMechanismEntityId] FROM [FailureMechanismEntity] WHERE [FailureMechanismType] = 13);";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);
        }

        private static void AssertPipingStructureFailureMechanism(MigratedDatabaseReader reader)
        {
            const string validateFailureMechanisms =
                "SELECT COUNT() = (SELECT COUNT() FROM FailureMechanismEntity WHERE FailureMechanismType = 11) " +
                "FROM [PipingStructureFailureMechanismMetaEntity] " +
                "WHERE [N] = 1.0 " +
                "AND [FailureMechanismEntityId] IN " +
                "(SELECT [FailureMechanismEntityId] FROM [FailureMechanismEntity] WHERE [FailureMechanismType] = 11);";
            reader.AssertReturnedDataIsValid(validateFailureMechanisms);
        }

        #region Migrated Hydraulic Boundary Locations

        private static void AssertHydraulicBoundaryLocations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateHydraulicBoundaryLocationEntities =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationEntity) " +
                "FROM HydraulicLocationEntity NEW " +
                "JOIN [SOURCEPROJECT].HydraulicLocationEntity OLD USING(HydraulicLocationEntityId) " +
                "WHERE NEW.AssessmentSectionEntityId = OLD.AssessmentSectionEntityId " +
                "AND NEW.LocationId = OLD.LocationId " +
                "AND NEW.Name = OLD.Name " +
                "AND NEW.LocationX IS OLD.LocationX " +
                "AND NEW.LocationY IS OLD.LocationY;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHydraulicBoundaryLocationEntities);

            string validateNrOfHydraulicBoundaryCalculationEntities =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() * 8 FROM [SOURCEPROJECT].HydraulicLocationEntity) " +
                "FROM HydraulicLocationCalculationEntity; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateNrOfHydraulicBoundaryCalculationEntities);

            string validateNrOfHydraulicBoundaryCalculationOutputEntities =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationOutputEntity) " +
                "FROM HydraulicLocationOutputEntity NEW " +
                "JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity OLD ON NEW.HydraulicLocationOutputEntityId = OLD.HydraulicLocationEntityOutputId " +
                "WHERE NEW.GeneralResultSubMechanismIllustrationPointEntityId IS OLD.GeneralResultSubMechanismIllustrationPointEntityId " +
                "AND NEW.Result IS OLD.Result " +
                "AND NEW.TargetProbability IS OLD.TargetProbability " +
                "AND NEW.TargetReliability IS OLD.TargetReliability " +
                "AND NEW.CalculatedProbability IS OLD.CalculatedProbability " +
                "AND NEW.CalculatedReliability IS OLD.CalculatedReliability " +
                "AND NEW.CalculationConvergence IS OLD.CalculationConvergence; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateNrOfHydraulicBoundaryCalculationOutputEntities);

            AssertMigratedHydraulicLocationCalculations(reader, sourceFilePath);
        }

        private static void AssertNewHydraulicLocationCalculations(MigratedDatabaseReader reader, string sourceFilePath, int entityCalculationNumber)
        {
            string validateNewHydraulicCalculations =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationEntity) " +
                "FROM HydraulicLocationEntity AS NEW " +
                $"JOIN HydraulicLocationCalculationEntity hlce ON NEW.HydraulicLocationCalculationEntity{entityCalculationNumber}Id = hlce.HydraulicLocationCalculationEntityId " +
                "WHERE hlce.ShouldIllustrationPointsBeCalculated = 0;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateNewHydraulicCalculations);
        }

        private static void AssertMigratedHydraulicLocationCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertDesignWaterLevelCalculations(reader, sourceFilePath);

            AssertWaveHeightCalculations(reader, sourceFilePath);

            AssertMigratedHydraulicLocationCalculationOutputs(reader, sourceFilePath);
        }

        private static void AssertMigratedHydraulicLocationCalculationOutputs(MigratedDatabaseReader reader, string sourceFilePath)
        {
            const string validateOutputNewCalculations =
                "SELECT COUNT() = 0 " +
                "FROM HydraulicLocationEntity hl " +
                "JOIN HydraulicLocationCalculationEntity calc1 ON calc1.HydraulicLocationCalculationEntityId = hl.HydraulicLocationCalculationEntity1Id " +
                "JOIN HydraulicLocationCalculationEntity calc4 ON calc4.HydraulicLocationCalculationEntityId = hl.HydraulicLocationCalculationEntity4Id " +
                "JOIN HydraulicLocationCalculationEntity calc5 ON calc5.HydraulicLocationCalculationEntityId = hl.HydraulicLocationCalculationEntity5Id " +
                "JOIN HydraulicLocationCalculationEntity calc8 ON calc8.HydraulicLocationCalculationEntityId = hl.HydraulicLocationCalculationEntity8Id " +
                "JOIN HydraulicLocationOutputEntity USING(HydraulicLocationCalculationEntityId);";
            reader.AssertReturnedDataIsValid(validateOutputNewCalculations);

            string validateDesignWaterLevelCalculationsWithSignalingNormOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity " +
                "JOIN [SOURCEPROJECT].HydraulicLocationEntity USING (HydraulicLocationEntityId) " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE NormativeNormType = 2 AND HydraulicLocationOutputType = 1 " +
                ") " +
                "FROM HydraulicLocationEntity NEWHL " +
                "JOIN HydraulicLocationCalculationEntity calc ON calc.HydraulicLocationCalculationEntityId = NEWHL.HydraulicLocationCalculationEntity2Id " +
                "JOIN HydraulicLocationOutputEntity NEW USING(HydraulicLocationCalculationEntityId) " +
                "JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity OLD ON NEW.HydraulicLocationOutputEntityId = OLD.HydraulicLocationEntityOutputId " +
                "WHERE OLD.HydraulicLocationEntityId = NEWHL.HydraulicLocationEntityId;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateDesignWaterLevelCalculationsWithSignalingNormOutput);

            string validateDesignWaterLevelCalculationsWithLowerLimitNormOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity " +
                "JOIN [SOURCEPROJECT].HydraulicLocationEntity USING (HydraulicLocationEntityId) " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE NormativeNormType = 1 AND HydraulicLocationOutputType = 1 " +
                ") " +
                "FROM HydraulicLocationEntity NEWHL " +
                "JOIN HydraulicLocationCalculationEntity calc ON calc.HydraulicLocationCalculationEntityId = NEWHL.HydraulicLocationCalculationEntity3Id " +
                "JOIN HydraulicLocationOutputEntity NEW USING(HydraulicLocationCalculationEntityId) " +
                "JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity OLD ON NEW.HydraulicLocationOutputEntityId = OLD.HydraulicLocationEntityOutputId " +
                "WHERE OLD.HydraulicLocationEntityId = NEWHL.HydraulicLocationEntityId;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateDesignWaterLevelCalculationsWithLowerLimitNormOutput);

            string validateWaveHeightCalculationsWithSignalingNormOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity " +
                "JOIN [SOURCEPROJECT].HydraulicLocationEntity USING (HydraulicLocationEntityId) " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE NormativeNormType = 2 AND HydraulicLocationOutputType = 2 " +
                ") " +
                "FROM HydraulicLocationEntity NEWHL " +
                "JOIN HydraulicLocationCalculationEntity calc ON calc.HydraulicLocationCalculationEntityId = NEWHL.HydraulicLocationCalculationEntity6Id " +
                "JOIN HydraulicLocationOutputEntity NEW USING(HydraulicLocationCalculationEntityId) " +
                "JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity OLD ON NEW.HydraulicLocationOutputEntityId = OLD.HydraulicLocationEntityOutputId " +
                "WHERE OLD.HydraulicLocationEntityId = NEWHL.HydraulicLocationEntityId;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateWaveHeightCalculationsWithSignalingNormOutput);

            string validateWaveHeightCalculationsWithLowerLimitNormOutput =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity " +
                "JOIN [SOURCEPROJECT].HydraulicLocationEntity USING (HydraulicLocationEntityId) " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE NormativeNormType = 1 AND HydraulicLocationOutputType = 2 " +
                ") " +
                "FROM HydraulicLocationEntity NEWHL " +
                "JOIN HydraulicLocationCalculationEntity calc ON calc.HydraulicLocationCalculationEntityId = NEWHL.HydraulicLocationCalculationEntity7Id " +
                "JOIN HydraulicLocationOutputEntity NEW USING(HydraulicLocationCalculationEntityId) " +
                "JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity OLD ON NEW.HydraulicLocationOutputEntityId = OLD.HydraulicLocationEntityOutputId " +
                "WHERE OLD.HydraulicLocationEntityId = NEWHL.HydraulicLocationEntityId;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateWaveHeightCalculationsWithLowerLimitNormOutput);
        }

        private static void AssertDesignWaterLevelCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNewHydraulicLocationCalculations(reader, sourceFilePath, 1);
            string validateDesignWaterLevelCalculationsWithSignalingNorm =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].HydraulicLocationEntity " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE NormativeNormType = 2 " +
                ") " +
                "FROM HydraulicLocationEntity AS NEW " +
                "JOIN [SOURCEPROJECT].HydraulicLocationEntity AS OLD ON OLD.HydraulicLocationEntityId = NEW.HydraulicLocationEntityId " +
                "JOIN HydraulicLocationCalculationEntity hlce ON NEW.HydraulicLocationCalculationEntity2Id = hlce.HydraulicLocationCalculationEntityId " +
                "JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE hlce.ShouldIllustrationPointsBeCalculated = OLD.ShouldDesignWaterLevelIllustrationPointsBeCalculated AND NormativeNormType = 2;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateDesignWaterLevelCalculationsWithSignalingNorm);
            string validateDesignWaterLevelCalculationsWithLowerLimitNorm =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].HydraulicLocationEntity " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE NormativeNormType = 1 " +
                ") " +
                "FROM HydraulicLocationEntity AS NEW " +
                "JOIN [SOURCEPROJECT].HydraulicLocationEntity AS OLD ON OLD.HydraulicLocationEntityId = NEW.HydraulicLocationEntityId " +
                "JOIN HydraulicLocationCalculationEntity hlce ON NEW.HydraulicLocationCalculationEntity3Id = hlce.HydraulicLocationCalculationEntityId " +
                "JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE hlce.ShouldIllustrationPointsBeCalculated = OLD.ShouldWaveHeightIllustrationPointsBeCalculated AND NormativeNormType = 1;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateDesignWaterLevelCalculationsWithLowerLimitNorm);
            AssertNewHydraulicLocationCalculations(reader, sourceFilePath, 4);
        }

        private static void AssertWaveHeightCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            AssertNewHydraulicLocationCalculations(reader, sourceFilePath, 5);
            string validateWaveHeightCalculationsWithSignalingNorm =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].HydraulicLocationEntity " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE NormativeNormType = 2 " +
                ") " +
                "FROM HydraulicLocationEntity AS NEW " +
                "JOIN [SOURCEPROJECT].HydraulicLocationEntity AS OLD ON OLD.HydraulicLocationEntityId = NEW.HydraulicLocationEntityId " +
                "JOIN HydraulicLocationCalculationEntity hlce ON NEW.HydraulicLocationCalculationEntity6Id = hlce.HydraulicLocationCalculationEntityId " +
                "JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE hlce.ShouldIllustrationPointsBeCalculated = OLD.ShouldWaveHeightIllustrationPointsBeCalculated AND NormativeNormType = 2;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateWaveHeightCalculationsWithSignalingNorm);
            string validateWaveHeightCalculationsWithLowerLimitNorm =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT COUNT() = " +
                "( " +
                "SELECT COUNT() " +
                "FROM [SOURCEPROJECT].HydraulicLocationEntity " +
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE NormativeNormType = 1 " +
                ") " +
                "FROM HydraulicLocationEntity AS NEW " +
                "JOIN [SOURCEPROJECT].HydraulicLocationEntity AS OLD ON OLD.HydraulicLocationEntityId = NEW.HydraulicLocationEntityId " +
                "JOIN HydraulicLocationCalculationEntity hlce ON NEW.HydraulicLocationCalculationEntity7Id = hlce.HydraulicLocationCalculationEntityId " +
                "JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                "WHERE hlce.ShouldIllustrationPointsBeCalculated = OLD.ShouldDesignWaterLevelIllustrationPointsBeCalculated AND NormativeNormType = 1;" +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateWaveHeightCalculationsWithLowerLimitNorm);
            AssertNewHydraulicLocationCalculations(reader, sourceFilePath, 8);
        }

        #endregion
    }
}