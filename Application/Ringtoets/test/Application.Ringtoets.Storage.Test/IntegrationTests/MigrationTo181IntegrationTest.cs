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
                    AssertWaveImpactAsphaltCoverFailureMechanism(reader);
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
            const string validateWaveImpactAsphaltCoverFailureMechanism =
                "SELECT COUNT() = 0 " +
                "FROM [WaveImpactAsphaltCoverFailureMechanismMetaEntity] " +
                "WHERE [DeltaL] IS NOT 1000;";
            reader.AssertReturnedDataIsValid(validateWaveImpactAsphaltCoverFailureMechanism);
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
                HydraulicLocationValidationQueryGenerator.GetOutputValidationQuery(
                    sourceFilePath,
                    HydraulicLocationValidationQueryGenerator.NormativeNormType.SignalingNorm,
                    HydraulicLocationValidationQueryGenerator.HydraulicLocationOutputType.DesignWaterLevel,
                    2);
            reader.AssertReturnedDataIsValid(validateDesignWaterLevelCalculationsWithSignalingNormOutput);

            string validateDesignWaterLevelCalculationsWithLowerLimitNormOutput =
                HydraulicLocationValidationQueryGenerator.GetOutputValidationQuery(
                    sourceFilePath,
                    HydraulicLocationValidationQueryGenerator.NormativeNormType.LowerLimitNorm,
                    HydraulicLocationValidationQueryGenerator.HydraulicLocationOutputType.DesignWaterLevel,
                    3);
            reader.AssertReturnedDataIsValid(validateDesignWaterLevelCalculationsWithLowerLimitNormOutput);

            string validateWaveHeightCalculationsWithSignalingNormOutput =
                HydraulicLocationValidationQueryGenerator.GetOutputValidationQuery(
                    sourceFilePath,
                    HydraulicLocationValidationQueryGenerator.NormativeNormType.SignalingNorm,
                    HydraulicLocationValidationQueryGenerator.HydraulicLocationOutputType.WaveHeight,
                    6);
            reader.AssertReturnedDataIsValid(validateWaveHeightCalculationsWithSignalingNormOutput);

            string validateWaveHeightCalculationsWithLowerLimitNormOutput =
                HydraulicLocationValidationQueryGenerator.GetOutputValidationQuery(
                    sourceFilePath,
                    HydraulicLocationValidationQueryGenerator.NormativeNormType.LowerLimitNorm,
                    HydraulicLocationValidationQueryGenerator.HydraulicLocationOutputType.WaveHeight,
                    7);
            reader.AssertReturnedDataIsValid(validateWaveHeightCalculationsWithLowerLimitNormOutput);
        }

        private static void AssertDesignWaterLevelCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculation1Entities = 
                HydraulicLocationValidationQueryGenerator.GetNewCalculationsValidationQuery(sourceFilePath, 1);
            reader.AssertReturnedDataIsValid(validateCalculation1Entities);

            string validateDesignWaterLevelCalculationsWithSignalingNorm =
                HydraulicLocationValidationQueryGenerator.GetDesignWaterLevelCalculationValidationQuery(
                    sourceFilePath,
                    HydraulicLocationValidationQueryGenerator.NormativeNormType.SignalingNorm,
                    2);
            reader.AssertReturnedDataIsValid(validateDesignWaterLevelCalculationsWithSignalingNorm);
            string validateDesignWaterLevelCalculationsWithLowerLimitNorm =
                HydraulicLocationValidationQueryGenerator.GetDesignWaterLevelCalculationValidationQuery(
                    sourceFilePath,
                    HydraulicLocationValidationQueryGenerator.NormativeNormType.LowerLimitNorm,
                    3);
            reader.AssertReturnedDataIsValid(validateDesignWaterLevelCalculationsWithLowerLimitNorm);

            string validateCalculation4Entities =
                HydraulicLocationValidationQueryGenerator.GetNewCalculationsValidationQuery(sourceFilePath, 4);
            reader.AssertReturnedDataIsValid(validateCalculation4Entities);
        }

        private static void AssertWaveHeightCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateCalculation5Entities =
                HydraulicLocationValidationQueryGenerator.GetNewCalculationsValidationQuery(sourceFilePath, 5);

            reader.AssertReturnedDataIsValid(validateCalculation5Entities);
            string validateWaveHeightCalculationsWithSignalingNorm =
                HydraulicLocationValidationQueryGenerator.GetWaveHeightCalculationValidationQuery(
                    sourceFilePath,
                    HydraulicLocationValidationQueryGenerator.NormativeNormType.SignalingNorm,
                    6);
            reader.AssertReturnedDataIsValid(validateWaveHeightCalculationsWithSignalingNorm);
            string validateWaveHeightCalculationsWithLowerLimitNorm =
                HydraulicLocationValidationQueryGenerator.GetWaveHeightCalculationValidationQuery(
                    sourceFilePath,
                    HydraulicLocationValidationQueryGenerator.NormativeNormType.LowerLimitNorm,
                    7);
            reader.AssertReturnedDataIsValid(validateWaveHeightCalculationsWithLowerLimitNorm);

            string validateCalculation8Entities = 
                HydraulicLocationValidationQueryGenerator.GetNewCalculationsValidationQuery(sourceFilePath, 8);
            reader.AssertReturnedDataIsValid(validateCalculation8Entities);
        }

        /// <summary>
        /// Class to generate queries which can be used if the hydraulic boundary locations 
        /// are correctly migrated.
        /// </summary>
        private static class HydraulicLocationValidationQueryGenerator
        {
            /// <summary>
            /// Enum to indicate the hydraulic location output types.
            /// </summary>
            public enum HydraulicLocationOutputType
            {
                /// <summary>
                /// Represents an output for a design water level 
                /// calculation.
                /// </summary>
                DesignWaterLevel = 1,

                /// <summary>
                /// Represents an output for a wave height calculation.
                /// </summary>
                WaveHeight = 2
            }

            /// <summary>
            /// Enum to indicate the norm which was used.
            /// </summary>
            public enum NormativeNormType
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

            /// <summary>
            /// Generates a query to validate the migrated hydraulic boundary location output.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the database to be verified.</param>
            /// <param name="normType">The <see cref="NormativeNormType"/> on which the output was calculated for.</param>
            /// <param name="outputType">The <see cref="HydraulicLocationOutputType"/> which the output represents.</param>
            /// <param name="calculationEntityNumber">The calculation on which the outputs should be set.</param>
            /// <returns>The query to validate the migrated hydraulic boundary location output.</returns>
            public static string GetOutputValidationQuery(string sourceFilePath,
                                                          NormativeNormType normType,
                                                          HydraulicLocationOutputType outputType,
                                                          int calculationEntityNumber)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT;" +
                       "SELECT COUNT() = " +
                       "( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity USING (HydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType} AND HydraulicLocationOutputType = {(int) outputType} " +
                       ") " +
                       "FROM HydraulicLocationEntity NEWHL " +
                       $"JOIN HydraulicLocationCalculationEntity calc ON calc.HydraulicLocationCalculationEntityId = NEWHL.HydraulicLocationCalculationEntity{calculationEntityNumber}Id " +
                       "JOIN HydraulicLocationOutputEntity NEW USING(HydraulicLocationCalculationEntityId) " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity OLD ON NEW.HydraulicLocationOutputEntityId = OLD.HydraulicLocationEntityOutputId " +
                       "WHERE OLD.HydraulicLocationEntityId = NEWHL.HydraulicLocationEntityId;" +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the migrated wave height calculations.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the database to be verified.</param>
            /// <param name="normType">The <see cref="NormativeNormType"/> for which the input was set.</param>
            /// <param name="calculationEntityNumber">The calculation on which the input should be validated.</param>
            /// <returns>The query to validate the migrated hydraulic boundary location calculation input.</returns>
            public static string GetWaveHeightCalculationValidationQuery(string sourceFilePath,
                                                                         NormativeNormType normType,
                                                                         int calculationEntityNumber)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = " +
                       "( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].HydraulicLocationEntity " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType}  " +
                       ") " +
                       "FROM HydraulicLocationEntity AS NEW " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity AS OLD ON OLD.HydraulicLocationEntityId = NEW.HydraulicLocationEntityId " +
                       $"JOIN HydraulicLocationCalculationEntity hlce ON NEW.HydraulicLocationCalculationEntity{calculationEntityNumber}Id = hlce.HydraulicLocationCalculationEntityId " +
                       "JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                       $"WHERE hlce.ShouldIllustrationPointsBeCalculated = OLD.ShouldWaveHeightIllustrationPointsBeCalculated AND NormativeNormType = {(int) normType};" +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the migrated design water level calculations.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the database to be verified.</param>
            /// <param name="normType">The <see cref="NormativeNormType"/> for which the input was set.</param>
            /// <param name="calculationEntityNumber">The calculation on which the input should be validated.</param>
            /// <returns>The query to validate the migrated hydraulic boundary location calculation input.</returns>
            public static string GetDesignWaterLevelCalculationValidationQuery(string sourceFilePath,
                                                                               NormativeNormType normType,
                                                                               int calculationEntityNumber)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = " +
                       "( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].HydraulicLocationEntity " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType}  " +
                       ") " +
                       "FROM HydraulicLocationEntity AS NEW " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity AS OLD ON OLD.HydraulicLocationEntityId = NEW.HydraulicLocationEntityId " +
                       $"JOIN HydraulicLocationCalculationEntity hlce ON NEW.HydraulicLocationCalculationEntity{calculationEntityNumber}Id = hlce.HydraulicLocationCalculationEntityId " +
                       "JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId) " +
                       $"WHERE hlce.ShouldIllustrationPointsBeCalculated = OLD.ShouldDesignWaterLevelIllustrationPointsBeCalculated AND NormativeNormType = {(int) normType};" +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the new hydraulic boundary location calculations that are not based on migrated data.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the database to be verified.</param>
            /// <param name="calculationEntityNumber">The calculation on which the input should be validated.</param>
            /// <returns>The query to validate the migrated hydraulic boundary location calculation input.</returns>
            public static string GetNewCalculationsValidationQuery(string sourceFilePath,
                                                                   int calculationEntityNumber)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationEntity) " +
                       "FROM HydraulicLocationEntity AS NEW " +
                       $"JOIN HydraulicLocationCalculationEntity hlce ON NEW.HydraulicLocationCalculationEntity{calculationEntityNumber}Id = hlce.HydraulicLocationCalculationEntityId " +
                       "WHERE hlce.ShouldIllustrationPointsBeCalculated = 0;" +
                       "DETACH DATABASE SOURCEPROJECT;";
            }
        }

        #endregion
    }
}