﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
    public class MigrationTo232IntegrationTest
    {
        private const string newVersion = "23.2";

        [Test]
        [TestCaseSource(nameof(GetMigrationProjectsWithMessages))]
        public void Given231Project_WhenUpgradedTo232_ThenProjectAsExpected(string fileName, IEnumerable<string> expectedMessages)
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               fileName);
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Given231Project_WhenUpgradedTo232_ThenProjectAsExpected));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Given231Project_WhenUpgradedTo232_ThenProjectAsExpected), ".log"));
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
                }

                AssertLogDatabase(logFilePath, expectedMessages);
            }
        }

        private static IEnumerable<TestCaseData> GetMigrationProjectsWithMessages()
        {
            yield return new TestCaseData("MigrationTestProject231.risk", new[]
            {
                "* Geen aanpassingen."
            });
        }

        private static void AssertTablesContentMigrated(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string[] tables =
            {
                "AdoptableFailureMechanismSectionResultEntity",
                "AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity",
                "AssessmentSectionEntity",
                "BackgroundDataEntity",
                "BackgroundDataMetaEntity",
                "CalculationGroupEntity",
                "ClosingStructureEntity",
                "ClosingStructuresCalculationEntity",
                "ClosingStructuresFailureMechanismMetaEntity",
                "ClosingStructuresOutputEntity",
                "DikeProfileEntity",
                "DuneErosionFailureMechanismMetaEntity",
                "DuneLocationCalculationEntity",
                "DuneLocationCalculationForTargetProbabilityCollectionEntity",
                "DuneLocationCalculationOutputEntity",
                "DuneLocationEntity",
                "FailureMechanismEntity",
                "FailureMechanismFailureMechanismSectionEntity",
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
                "GrassCoverErosionOutwardsFailureMechanismMetaEntity",
                "GrassCoverErosionOutwardsWaveConditionsCalculationEntity",
                "GrassCoverErosionOutwardsWaveConditionsOutputEntity",
                "GrassCoverSlipOffInwardsFailureMechanismMetaEntity",
                "GrassCoverSlipOffOutwardsFailureMechanismMetaEntity",
                "HeightStructureEntity",
                "HeightStructuresCalculationEntity",
                "HeightStructuresFailureMechanismMetaEntity",
                "HeightStructuresOutputEntity",
                "HydraulicBoundaryDataEntity",
                "HydraulicBoundaryDatabaseEntity",
                "HydraulicLocationCalculationCollectionEntity",
                "HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity",
                "HydraulicLocationCalculationEntity",
                "HydraulicLocationCalculationForTargetProbabilityCollectionEntity",
                "HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity",
                "HydraulicLocationEntity",
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
                "PipingCharacteristicPointEntity",
                "PipingFailureMechanismMetaEntity",
                "PipingScenarioConfigurationPerFailureMechanismSectionEntity",
                "PipingSoilLayerEntity",
                "PipingSoilProfileEntity",
                "PipingStochasticSoilProfileEntity",
                "PipingStructureFailureMechanismMetaEntity",
                "ProbabilisticPipingCalculationEntity",
                "ProbabilisticPipingCalculationOutputEntity",
                "ProjectEntity",
                "SemiProbabilisticPipingCalculationEntity",
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
                "StochasticSoilModelEntity",
                "SubMechanismIllustrationPointEntity",
                "SubMechanismIllustrationPointStochastEntity",
                "SurfaceLineEntity",
                "TopLevelFaultTreeIllustrationPointEntity",
                "TopLevelSubMechanismIllustrationPointEntity",
                "VersionEntity",
                "WaterPressureAsphaltCoverFailureMechanismMetaEntity",
                "WaveImpactAsphaltCoverFailureMechanismMetaEntity",
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

        private static void AssertLogDatabase(string logFilePath, IEnumerable<string> expectedMessages)
        {
            using (var reader = new MigrationLogDatabaseReader(logFilePath))
            {
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                Assert.AreEqual(expectedMessages.Count() + 1, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("23.1", newVersion, $"Gevolgen van de migratie van versie 23.1 naar versie {newVersion}:"),
                    messages[i++]);

                foreach (string expectedMessage in expectedMessages)
                {
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("23.1", newVersion, $"{expectedMessage}"),
                        messages[i++]);
                }
            }
        }

        private static void AssertVersions(MigratedDatabaseReader reader)
        {
            const string validateVersion =
                "SELECT COUNT() = 1 " +
                "FROM [VersionEntity] " +
                "WHERE [Version] = \"23.2\";";
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