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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Migration.Core;
using Ringtoets.Migration.Core.TestUtil;

namespace Ringtoets.Migration.Integration.Test
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
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Migration.Core,
                                                               "MigrationTestProject173.rtd");
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

                    AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(reader, sourceFilePath);

                    AssertDuneErosionFailureMechanismMetaEntity(reader, sourceFilePath);
                    AssertDuneLocations(reader, sourceFilePath);

                    AssertHeightStructuresSectionResultEntity(reader, sourceFilePath);
                    AssertClosingStructuresSectionResultEntity(reader, sourceFilePath);
                    AssertStabilityPointStructuresSectionResultEntity(reader, sourceFilePath);
                    AssertGrassCoverErosionInwardsSectionResultEntity(reader, sourceFilePath);

                    AssertPipingSectionResultEntity(reader, sourceFilePath);
                    AssertMacroStabilityInwardsSectionResultEntity(reader, sourceFilePath);

                    AssertDuneErosionSectionResultEntity(reader, sourceFilePath);
                    AssertGrassCoverErosionOutwardsSectionResultEntity(reader, sourceFilePath);
                    AssertStabilityStoneCoverSectionResultEntity(reader, sourceFilePath);
                    AssertWaveImpactAsphaltCoverSectionResultEntity(reader, sourceFilePath);

                    AssertGrassCoverSlipOffInwardsSectionResultEntity(reader, sourceFilePath);
                    AssertGrassCoverSlipOffOutwardsSectionResultEntity(reader, sourceFilePath);
                    AssertMacroStabilityOutwardsSectionResultEntity(reader, sourceFilePath);
                    AssertMicrostabilitySectionResultEntity(reader, sourceFilePath);
                    AssertPipingStructureSectionResultEntity(reader, sourceFilePath);
                    AssertStrengthStabilityLengthwiseConstructionSectionResultEntity(reader, sourceFilePath);
                    AssertTechnicalInnovationSectionResultEntity(reader, sourceFilePath);
                    AssertWaterPressureAsphaltCoverSectionResultEntity(reader, sourceFilePath);

                    AssertWaveConditionsCalculations(reader, sourceFilePath);

                    MigratedSerializedDataTestHelper.AssertSerializedMacroStabilityInwardsOutput(reader);
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
                "ClosingStructuresOutputEntity",
                "ClosingStructuresSectionResultEntity",
                "DikeProfileEntity",
                "DuneErosionFailureMechanismMetaEntity",
                "DuneErosionSectionResultEntity",
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

                Assert.AreEqual(61, messages.Count);
                var i = 0;
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "Gevolgen van de migratie van versie 17.3 naar versie 18.1:"),
                    messages[i++]);

                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "* Traject: 'assessmentSectionResults'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Piping'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Hoogte kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Sterkte en stabiliteit langsconstructies'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Technische innovaties'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Wateroverdruk bij asfaltbekleding'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Betrouwbaarheid sluiting kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Macrostabiliteit binnenwaarts'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Macrostabiliteit buitenwaarts'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Golfklappen op asfaltbekleding'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Grasbekleding erosie buitentalud'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Grasbekleding afschuiven binnentalud'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Grasbekleding afschuiven buitentalud'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Microstabiliteit'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Piping bij kunstwerk'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets " +
                                                                "van dit toetsspoor zijn omgezet naar 'NVT'."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Stabiliteit steenzetting'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "  + Toetsspoor: 'Duinafslag'"),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden " +
                                                                "omgezet naar een geldig resultaat en zijn verwijderd."),
                    messages[i++]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                    new MigrationLogMessage("17.3", newVersion, "    - Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet " +
                                                                "naar een geldig resultaat en zijn verwijderd."),
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
                "JOIN [SOURCEPROJECT].AssessmentSectionEntity AS OLD USING(AssessmentSectionEntityId) " +
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
                                                               "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationEntity) " +
                                                               "FROM HydraulicLocationEntity NEW " +
                                                               "JOIN [SourceProject].HydraulicLocationEntity AS OLD USING(HydraulicLocationEntityId) " +
                                                               "WHERE NEW.AssessmentSectionEntityId = OLD.AssessmentSectionEntityId " +
                                                               "AND NEW.LocationId = OLD.LocationId " +
                                                               "AND NEW.Name = OLD.Name " +
                                                               "AND NEW.LocationX = OLD.LocationX " +
                                                               "AND NEW.LocationY = OLD.LocationY " +
                                                               "AND NEW.\"Order\" = OLD.\"Order\"; " +
                                                               "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateMigratedHyraulicBoundaryLocations);

            string validateHydraulicLocationCalculationCount =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = 0 " +
                "FROM " +
                "( " +
                "SELECT " +
                "COUNT(distinct HydraulicLocationCalculationEntityId) AS NrOfCalculationsPerLocation " +
                "FROM [SOURCEPROJECT].HydraulicLocationEntity " +
                "LEFT JOIN HydraulicLocationCalculationEntity USING(HydraulicLocationEntityId) " +
                "GROUP BY HydraulicLocationEntityId " +
                ") " +
                "WHERE NrOfCalculationsPerLocation IS NOT 14; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateHydraulicLocationCalculationCount);

            const string validateHydraulicBoundaryCalculationInputValues = "SELECT " +
                                                                           "COUNT() = 0 " +
                                                                           "FROM HydraulicLocationCalculationEntity " +
                                                                           "WHERE ShouldIllustrationPointsBeCalculated != 0 AND ShouldIllustrationPointsBeCalculated != 1";
            reader.AssertReturnedDataIsValid(validateHydraulicBoundaryCalculationInputValues);

            string validateHydraulicLocationCalculationOutputCount =
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
            reader.AssertReturnedDataIsValid(validateHydraulicLocationCalculationOutputCount);
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

        private static void AssertDuneLocations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var queryGenerator = new DuneErosionFailureMechanismValidationQueryGenerator(sourceFilePath);

            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForMechanismSpecificSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForMechanismSpecificLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetDuneLocationCalculationsCountValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForLowerFactorizedLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDuneLocationCalculationOutputValidationQuery(NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDuneLocationCalculationOutputValidationQuery(NormativeNormType.LowerLimitNorm));

            reader.AssertReturnedDataIsValid(DuneErosionFailureMechanismValidationQueryGenerator.GetNewDuneLocationCalculationOutputValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(DuneErosionFailureMechanismValidationQueryGenerator.GetNewDuneLocationCalculationOutputValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForLowerLimitNorm));
            reader.AssertReturnedDataIsValid(DuneErosionFailureMechanismValidationQueryGenerator.GetNewDuneLocationCalculationOutputValidationQuery(
                                                 DuneErosionFailureMechanismValidationQueryGenerator.CalculationType.CalculationsForLowerFactorizedLimitNorm));
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity) " +
                "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity OLD USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId) " +
                "WHERE new.FailureMechanismEntityId = OLD.FailureMechanismEntityId " +
                "AND NEW.N = OLD.N " +
                "AND NEW.ForeshoreProfileCollectionSourcePath IS OLD.ForeshoreProfileCollectionSourcePath; " +
                "DETACH DATABASE SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        private static void AssertDuneErosionFailureMechanismMetaEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateMetaEntity =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity) " +
                "FROM DuneErosionFailureMechanismMetaEntity NEW " +
                "JOIN [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity OLD USING(DuneErosionFailureMechanismMetaEntityId) " +
                "WHERE new.FailureMechanismEntityId = OLD.FailureMechanismEntityId " +
                "AND NEW.N = OLD.N; " +
                "DETACH DATABASE SOURCEPROJECT;";

            reader.AssertReturnedDataIsValid(validateMetaEntity);
        }

        #region Serializers

        /// <summary>
        /// Testhelper to assert the migrated values of serialized data.
        /// </summary>
        private static class MigratedSerializedDataTestHelper
        {
            private const string oldNamespace = "Application.Ringtoets.Storage.Serializers";

            /// <summary>
            /// Asserts the migrated serialized data related to macro stability inwards output.
            /// </summary>
            /// <param name="reader">The reader to read the migrated database.</param>
            /// <exception cref="AssertionException">Thrown when:
            /// <list type="bullet">
            /// <item>The namespace is still present.</item>
            /// <item>The class name of the serialized data is still present.</item>
            /// </list></exception>
            public static void AssertSerializedMacroStabilityInwardsOutput(MigratedDatabaseReader reader)
            {
                string validateSlidingCurves =
                    "SELECT " +
                    "COUNT() = 0 " +
                    "FROM MacroStabilityInwardsCalculationOutputEntity " +
                    "WHERE LIKE('%MacroStabilityInwardsSliceXmlSerializer%', SlidingCurveSliceXml) " +
                    $"OR LIKE('%{oldNamespace}%', SlidingCurveSliceXml)";

                reader.AssertReturnedDataIsValid(validateSlidingCurves);

                string validateTangentLines =
                    "SELECT " +
                    "COUNT() = 0 " +
                    "FROM MacroStabilityInwardsCalculationOutputEntity " +
                    "WHERE LIKE('%TangentLinesXmlSerializer%', SlipPlaneTangentLinesXml) " +
                    $"OR LIKE('%{oldNamespace}%', SlipPlaneTangentLinesXml)";

                reader.AssertReturnedDataIsValid(validateTangentLines);
            }
        }

        #endregion

        #region Dune Locations

        /// <summary>
        /// Class to generate queries which can be used to assert if the dune locations are correctly migrated.
        /// </summary>
        private class DuneErosionFailureMechanismValidationQueryGenerator
        {
            /// <summary>
            /// Enum to indicate the dune location calculation type.
            /// </summary>
            public enum CalculationType
            {
                /// <summary>
                /// Represents the calculations for the mechanism specific factorized signaling norm.
                /// </summary>
                CalculationsForMechanismSpecificFactorizedSignalingNorm = 1,

                /// <summary>
                /// Represents the calculations for the mechanism specific signaling norm.
                /// </summary>
                CalculationsForMechanismSpecificSignalingNorm = 2,

                /// <summary>
                /// Represents the calculations for the mechanism specific lower limit norm.
                /// </summary>
                CalculationsForMechanismSpecificLowerLimitNorm = 3,

                /// <summary>
                /// Represents the calculations for the lower limit norm.
                /// </summary>
                CalculationsForLowerLimitNorm = 4,

                /// <summary>
                /// Represents the calculations for the factorized lower limit norm.
                /// </summary>
                CalculationsForLowerFactorizedLimitNorm = 5
            }

            private readonly string sourceFilePath;

            /// <summary>
            /// Creates a new instance of <see cref="DuneErosionFailureMechanismValidationQueryGenerator"/>.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the original database.</param>
            /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFilePath"/>
            /// is <c>null</c> or empty.</exception>
            public DuneErosionFailureMechanismValidationQueryGenerator(string sourceFilePath)
            {
                if (string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    throw new ArgumentException(@"Sourcefile path cannot be null or empty",
                                                nameof(sourceFilePath));
                }

                this.sourceFilePath = sourceFilePath;
            }

            /// <summary>
            /// Generates a query to validate the number of created dune location calculations per failure mechanism.
            /// </summary>
            /// <param name="calculationType">The type of calculation that should be validated.</param>
            /// <returns>The query to validate the number of dune location calculations per calculation type.</returns>
            public string GetDuneLocationCalculationsCountValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM " +
                       "( " +
                       "SELECT " +
                       "[DuneErosionFailureMechanismMetaEntityId], " +
                       "COUNT() AS NewCount, " +
                       "OldCount " +
                       GetDuneLocationCalculationsQuery(calculationType) +
                       "LEFT JOIN " +
                       "( " +
                       "SELECT " +
                       "[DuneErosionFailureMechanismMetaEntityId], " +
                       "COUNT() as OldCount " +
                       "FROM [SourceProject].DuneErosionFailureMechanismMetaEntity  " +
                       "JOIN [SourceProject].DuneLocationEntity USING(FailureMechanismEntityId) " +
                       "GROUP BY DuneErosionFailureMechanismMetaEntityId " +
                       ") USING(DuneErosionFailureMechanismMetaEntityId) " +
                       "GROUP BY DuneErosionFailureMechanismMetaEntityId " +
                       "UNION " +
                       "SELECT " +
                       "[DuneErosionFailureMechanismMetaEntityId], " +
                       "NewCount, " +
                       "COUNT() AS OldCount " +
                       "FROM [SourceProject].DuneErosionFailureMechanismMetaEntity " +
                       "JOIN [SourceProject].DuneLocationEntity USING(FailureMechanismEntityId) " +
                       "LEFT JOIN " +
                       "( " +
                       "SELECT " +
                       "[DuneErosionFailureMechanismMetaEntityId],  " +
                       "COUNT() as NewCount " +
                       GetDuneLocationCalculationsQuery(calculationType) +
                       "GROUP BY DuneErosionFailureMechanismMetaEntityId " +
                       ") USING(DuneErosionFailureMechanismMetaEntityId) " +
                       "GROUP BY DuneErosionFailureMechanismMetaEntityId " +
                       ") " +
                       "WHERE NewCount IS NOT OldCount; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the new dune location outputs that are not based on migrated data.
            /// </summary>
            /// <param name="calculationType">The type of calculation on which the output should be validated.</param>
            /// <returns>The query to validate the dune location output.</returns>
            public static string GetNewDuneLocationCalculationOutputValidationQuery(CalculationType calculationType)
            {
                return "SELECT COUNT() = 0 " +
                       GetDuneLocationCalculationsQuery(calculationType) +
                       "JOIN DuneLocationCalculationOutputEntity USING(DuneLocationCalculationEntityId);";
            }

            /// <summary>
            /// Generates a query to validate if the dune location outputs are migrated correctly to the 
            /// corresponding calculation entities.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the dune location outputs.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedDuneLocationCalculationOutputValidationQuery(NormativeNormType normType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = ( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].DuneLocationOutputEntity " +
                       "JOIN [SOURCEPROJECT].DuneLocationEntity USING(DuneLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId)  " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType} " +
                       ") " +
                       GetDuneLocationCalculationsQuery(ConvertToCalculationType(normType)) +
                       "JOIN DuneLocationCalculationOutputEntity NEW USING(DuneLocationCalculationEntityId) " +
                       "JOIN [SOURCEPROJECT].DuneLocationOutputEntity OLD ON OLD.DuneLocationOutputEntityId = NEW.DuneLocationCalculationOutputEntityId " +
                       "WHERE NEW.WaterLevel IS OLD.WaterLevel " +
                       "AND NEW.WaveHeight IS OLD.WaveHeight " +
                       "AND NEW.WavePeriod IS OLD.WavePeriod " +
                       "AND NEW.TargetProbability IS OLD.TargetProbability " +
                       "AND NEW.TargetReliability IS OLD.TargetReliability " +
                       "AND NEW.CalculatedProbability IS OLD.CalculatedProbability " +
                       "AND NEW.CalculatedReliability IS OLD.CalculatedReliability " +
                       "AND NEW.CalculationConvergence = OLD.CalculationConvergence; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            private static string GetDuneLocationCalculationsQuery(CalculationType calculationType)
            {
                return "FROM DuneErosionFailureMechanismMetaEntity fme " +
                       "JOIN DuneLocationCalculationCollectionEntity ON  " +
                       $"DuneLocationCalculationCollectionEntityId = fme.DuneLocationCalculationCollectionEntity{(int) calculationType}Id " +
                       "JOIN DuneLocationCalculationEntity USING(DuneLocationCalculationCollectionEntityId) ";
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding calculation from <see cref="CalculationType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="CalculationType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            private static CalculationType ConvertToCalculationType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.LowerLimitNorm:
                        return CalculationType.CalculationsForMechanismSpecificLowerLimitNorm;
                    case NormativeNormType.SignalingNorm:
                        return CalculationType.CalculationsForMechanismSpecificSignalingNorm;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion

        #region Failure Mechanism Section Result Entities

        private static void AssertHeightStructuresSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity) " +
                "FROM HeightStructuresSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].HeightStructuresSectionResultEntity OLD USING(HeightStructuresSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.HeightStructuresCalculationEntityId IS OLD.HeightStructuresCalculationEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssemblyProbability = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertClosingStructuresSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity) " +
                "FROM ClosingStructuresSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].ClosingStructuresSectionResultEntity OLD USING(ClosingStructuresSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.ClosingStructuresCalculationEntityId IS OLD.ClosingStructuresCalculationEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssemblyProbability = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertStabilityPointStructuresSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity) " +
                "FROM StabilityPointStructuresSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity OLD USING(StabilityPointStructuresSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.StabilityPointStructuresCalculationEntityId IS OLD.StabilityPointStructuresCalculationEntityId " +
                "AND NEW.SimpleAssessmentResult = OLD.LayerOne " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssemblyProbability = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertGrassCoverErosionInwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity) " +
                "FROM GrassCoverErosionInwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity OLD USING(GrassCoverErosionInwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.GrassCoverErosionInwardsCalculationEntityId IS OLD.GrassCoverErosionInwardsCalculationEntityId " +
                "AND NEW.SimpleAssessmentResult = OLD.LayerOne " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssemblyProbability = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertPipingSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].PipingSectionResultEntity) " +
                "FROM PipingSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].PipingSectionResultEntity OLD USING(PipingSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssemblyProbability = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertMacroStabilityInwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].MacroStabilityInwardsSectionResultEntity) " +
                "FROM MacroStabilityInwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].MacroStabilityInwardsSectionResultEntity OLD USING(MacroStabilityInwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND ((OLD.LayerThree IS NULL AND NEW.TailorMadeAssessmentResult = 1) " +
                "OR (OLD.LayerThree IS NOT NULL AND NEW.TailorMadeAssessmentResult = 3)) " +
                "AND NEW.TailorMadeAssessmentProbability IS OLD.LayerThree " +
                "AND NEW.UseManualAssemblyProbability = 0 " +
                "AND NEW.ManualAssemblyProbability IS NULL; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertDuneErosionSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].DuneErosionSectionResultEntity) " +
                "FROM DuneErosionSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].DuneErosionSectionResultEntity OLD USING(DuneErosionSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.SimpleAssessmentResult = OLD.LayerOne " +
                "AND NEW.DetailedAssessmentResultForFactorizedSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForFactorizedLowerLimitNorm = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertGrassCoverErosionOutwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity) " +
                "FROM GrassCoverErosionOutwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity OLD USING(GrassCoverErosionOutwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResultForFactorizedSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForFactorizedLowerLimitNorm = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertStabilityStoneCoverSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity) " +
                "FROM StabilityStoneCoverSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity OLD USING(StabilityStoneCoverSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND NEW.SimpleAssessmentResult = OLD.LayerOne " +
                "AND NEW.DetailedAssessmentResultForFactorizedSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForFactorizedLowerLimitNorm = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertWaveImpactAsphaltCoverSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity) " +
                "FROM WaveImpactAsphaltCoverSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity OLD USING(WaveImpactAsphaltCoverSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResultForFactorizedSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForSignalingNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForLowerLimitNorm = 1 " +
                "AND NEW.DetailedAssessmentResultForFactorizedLowerLimitNorm = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertGrassCoverSlipOffInwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity) " +
                "FROM GrassCoverSlipOffInwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity OLD USING (GrassCoverSlipOffInwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertGrassCoverSlipOffOutwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity) " +
                "FROM GrassCoverSlipOffOutwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity OLD USING (GrassCoverSlipOffOutwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertMacroStabilityOutwardsSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].MacroStabilityOutwardsSectionResultEntity) " +
                "FROM MacroStabilityOutwardsSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].MacroStabilityOutwardsSectionResultEntity OLD USING (MacroStabilityOutwardsSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.DetailedAssessmentProbability IS OLD.LayerTwoA " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentProbability IS NULL " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertMicrostabilitySectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].MicrostabilitySectionResultEntity) " +
                "FROM MicrostabilitySectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].MicrostabilitySectionResultEntity OLD USING (MicrostabilitySectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertPipingStructureSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].PipingStructureSectionResultEntity) " +
                "FROM PipingStructureSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].PipingStructureSectionResultEntity OLD USING (PipingStructureSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.DetailedAssessmentResult = 1 " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertStrengthStabilityLengthwiseConstructionSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity) " +
                "FROM StrengthStabilityLengthwiseConstructionSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity OLD USING (StrengthStabilityLengthwiseConstructionSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertTechnicalInnovationSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].TechnicalInnovationSectionResultEntity) " +
                "FROM TechnicalInnovationSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].TechnicalInnovationSectionResultEntity OLD USING (TechnicalInnovationSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        private static void AssertWaterPressureAsphaltCoverSectionResultEntity(MigratedDatabaseReader reader, string sourceFilePath)
        {
            string validateSectionResult =
                $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                "SELECT  " +
                "COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity) " +
                "FROM WaterPressureAsphaltCoverSectionResultEntity NEW " +
                "JOIN [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity OLD USING (WaterPressureAsphaltCoverSectionResultEntityId) " +
                "WHERE NEW.FailureMechanismSectionEntityId = OLD.FailureMechanismSectionEntityId " +
                "AND ((OLD.LayerOne = 1 AND NEW.SimpleAssessmentResult = 1) " +
                "OR (OLD.LayerOne = 2 AND NEW.SimpleAssessmentResult = 2) " +
                "OR (OLD.LayerOne = 3 AND NEW.SimpleAssessmentResult = 4)) " +
                "AND NEW.TailorMadeAssessmentResult = 1 " +
                "AND NEW.UseManualAssemblyCategoryGroup = 0 " +
                "AND NEW.ManualAssemblyCategoryGroup = 1; " +
                "DETACH DATABASE SOURCEPROJECT;";
            reader.AssertReturnedDataIsValid(validateSectionResult);
        }

        #endregion

        #region  Migrated Hydraulic Boundary Locations on Assessment section

        private static void AssertWaveHeightCalculationEntitiesOnAssessmentSection(MigratedDatabaseReader reader,
                                                                                   HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator queryGenerator)
        {
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaveHeightCalculationsForFactorizedLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationOutputsValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedWaveHeightCalculationOutputsValidationQuery(NormativeNormType.SignalingNorm));

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
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForLowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(
                                                 HydraulicLocationCalculationOnAssessmentSectionValidationQueryGenerator.CalculationType.WaterLevelCalculationsForFactorizedLowerLimitNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType.SignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(NormativeNormType.SignalingNorm));

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
        /// Class to generate queries which can be used to assert if the hydraulic boundary locations 
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
            /// <param name="sourceFilePath">The file path of the original database.</param>
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
            public string GetHydraulicBoundaryLocationCalculationsPerAssessmentSectionCountValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM " +
                       "( " +
                       "SELECT " +
                       "ase.AssessmentSectionEntityId, " +
                       "COUNT() AS NewCount, " +
                       "OldCount " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       "LEFT JOIN  " +
                       "( " +
                       "SELECT   " +
                       "sourceAse.AssessmentSectionEntityId, " +
                       "COUNT(distinct HydraulicLocationEntityId) AS OldCount " +
                       "FROM [SOURCEPROJECT].HydraulicLocationEntity sourceHle " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity sourceAse ON sourceHle.AssessmentSectionEntityId = sourceAse.AssessmentSectionEntityId " +
                       "GROUP BY sourceAse.AssessmentSectionEntityId " +
                       ") USING(AssessmentSectionEntityId) " +
                       "GROUP BY ase.AssessmentSectionEntityId " +
                       "UNION " +
                       "SELECT " +
                       "sourceAse.AssessmentSectionEntityId, " +
                       "NewCount, " +
                       "COUNT(distinct HydraulicLocationEntityId) AS OldCount " +
                       "FROM [SOURCEPROJECT].HydraulicLocationEntity sourceHle " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity sourceAse ON sourceHle.AssessmentSectionEntityId = sourceAse.AssessmentSectionEntityId " +
                       "LEFT JOIN " +
                       "( " +
                       "SELECT " +
                       "ase.AssessmentSectionEntityId, " +
                       "COUNT() AS NewCount " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       "GROUP BY ase.AssessmentSectionEntityId " +
                       ") USING(AssessmentSectionEntityId) " +
                       "GROUP BY sourceAse.AssessmentSectionEntityId " +
                       ") " +
                       "WHERE NewCount IS NOT OldCount; " +
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM AssessmentSectionEntity ase " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON ase.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING(HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity OLD USING(HydraulicLocationEntityId) " +
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() =  " +
                       "( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity sourceHlo " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity USING(HydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity sourceAse USING(AssessmentSectionEntityId) " +
                       $"WHERE sourceHlo.HydraulicLocationOutputType = 1 AND sourceAse.NormativeNormType = {(int) normType} " +
                       ") " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       GetHydraulicLocationCalculationOutputValidationSubQuery() +
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
            /// but unsupported.</exception>
            public string GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToWaveHeightCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM AssessmentSectionEntity ase " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON ase.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING(HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity OLD USING(HydraulicLocationEntityId) " +
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedWaveHeightCalculationOutputsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() =  " +
                       "( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].HydraulicLocationOutputEntity sourceHlo " +
                       "JOIN [SOURCEPROJECT].HydraulicLocationEntity USING(HydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity sourceAse USING(AssessmentSectionEntityId) " +
                       $"WHERE sourceHlo.HydraulicLocationOutputType = 2 AND sourceAse.NormativeNormType = {(int) normType} " +
                       ") " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       GetHydraulicLocationCalculationOutputValidationSubQuery() +
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
            /// <returns>The query to validate the hydraulic boundary location calculation output.</returns>
            public static string GetNewCalculationOutputsValidationQuery(CalculationType calculationType)
            {
                return "SELECT  " +
                       "COUNT() = 0 " +
                       GetHydraulicLocationCalculationsFromCollectionQuery(calculationType) +
                       "JOIN HydraulicLocationOutputEntity USING(HydraulicLocationCalculationEntityId); ";
            }

            private static string GetHydraulicLocationCalculationsFromCollectionQuery(CalculationType calculationType)
            {
                return "FROM AssessmentSectionEntity ase " +
                       $"JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id " +
                       "= hlcce.HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationEntity USING(HydraulicLocationCalculationCollectionEntityId) ";
            }

            private static string GetHydraulicLocationCalculationOutputValidationSubQuery()
            {
                return "JOIN HydraulicLocationOutputEntity NEW USING(HydraulicLocationCalculationEntityId) " +
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
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
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaterLevelCalculationsForMechanismSpecificSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
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
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
                                                 HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator.CalculationType.WaveHeightCalculationsForMechanismSpecificSignalingNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(
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

        /// <summary>
        /// Class to generate queries which can be used to assert if the hydraulic boundary locations 
        /// are correctly migrated on the grass cover erosion outwards failure mechanism level.
        /// </summary>
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

            /// <summary>
            /// Creates a new instance of <see cref="HydraulicLocationOnGrassCoverErosionOutwardsFailureMechanismValidationQueryGenerator"/>.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the original database.</param>
            /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFilePath"/>
            /// is <c>null</c> or empty.</exception>
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
            /// Generates a query to validate the number of created hydraulic boundary location calculations per failure mechanism.
            /// </summary>
            /// <param name="calculationType">The type of calculation that should be validated.</param>
            /// <returns>The query to validate the number of hydraulic boundary location calculations per failure mechanism.</returns>
            public string GetHydraulicBoundaryLocationCalculationsPerFailureMechanismCountValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = 0 " +
                       "FROM " +
                       "( " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "COUNT() AS NewCount, " +
                       "OldCount " +
                       GetHydraulicLocationCalculationsFromFailureMechanismQuery(calculationType) +
                       "LEFT JOIN  " +
                       "( " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "COUNT(distinct GrassCoverErosionOutwardsHydraulicLocationEntityId) AS OldCount " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       "GROUP BY FailureMechanismEntityId " +
                       ") USING(FailureMechanismEntityId) " +
                       "GROUP BY FailureMechanismEntityId " +
                       "UNION " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "NewCount, " +
                       "COUNT(distinct GrassCoverErosionOutwardsHydraulicLocationEntityId) AS OldCount " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       "LEFT JOIN " +
                       "( " +
                       "SELECT " +
                       "[FailureMechanismEntityId], " +
                       "COUNT() AS NewCount " +
                       GetHydraulicLocationCalculationsFromFailureMechanismQuery(calculationType) +
                       "GROUP BY FailureMechanismEntityId " +
                       ") USING(FailureMechanismEntityId) " +
                       "GROUP BY FailureMechanismEntityId" +
                       ") " +
                       "WHERE NewCount IS NOT OldCount; " +
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = 0 " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme  " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON gceofmme.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING(HydraulicLocationCalculationCollectionEntityId)  " +
                       "JOIN HydraulicLocationEntity hl USING(HydraulicLocationEntityId) " +
                       "JOIN FailureMechanismEntity fm USING(FailureMechanismEntityId) " +
                       "JOIN AssessmentSectionEntity ase USING(AssessmentSectionEntityId) " +
                       "JOIN( " +
                       "SELECT " +
                       "LocationId, " +
                       "AssessmentSectionEntityId, " +
                       "ShouldDesignWaterLevelIllustrationPointsBeCalculated " +
                       "FROM[SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       ") OLD ON(OLD.LocationId = hl.LocationId AND OLD.AssessmentSectionEntityId = fm.AssessmentSectionEntityId) " +
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedDesignWaterLevelCalculationOutputsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToDesignWaterLevelCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = ( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                       "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity USING(GrassCoverErosionOutwardsHydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       $"WHERE HydraulicLocationOutputType = 1 AND NormativeNormType = {(int) normType}" +
                       ") " +
                       GetHydraulicLocationCalculationsFromFailureMechanismQuery(calculationType) +
                       GetHydraulicLocationCalculationOutputValidationSubQuery() +
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
            /// but unsupported.</exception>
            public string GetMigratedWaveHeightCalculationsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToWaveHeightCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = 0 " +
                       "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme  " +
                       "JOIN HydraulicLocationCalculationCollectionEntity hlcce " +
                       $"ON gceofmme.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = hlcce.HydraulicLocationCalculationCollectionEntityId  " +
                       "JOIN HydraulicLocationCalculationEntity NEW USING(HydraulicLocationCalculationCollectionEntityId)  " +
                       "JOIN HydraulicLocationEntity hl USING(HydraulicLocationEntityId) " +
                       "JOIN FailureMechanismEntity fm USING(FailureMechanismEntityId) " +
                       "JOIN AssessmentSectionEntity ase USING(AssessmentSectionEntityId) " +
                       "JOIN( " +
                       "SELECT " +
                       "LocationId, " +
                       "AssessmentSectionEntityId, " +
                       "ShouldWaveHeightIllustrationPointsBeCalculated " +
                       "FROM[SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       ") OLD ON(OLD.LocationId = hl.LocationId AND OLD.AssessmentSectionEntityId = fm.AssessmentSectionEntityId) " +
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetMigratedWaveHeightCalculationOutputsValidationQuery(NormativeNormType normType)
            {
                CalculationType calculationType = ConvertToWaveHeightCalculationType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT " +
                       "COUNT() = ( " +
                       "SELECT COUNT() " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                       "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity USING(GrassCoverErosionOutwardsHydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       "JOIN [SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       $"WHERE HydraulicLocationOutputType = 2 AND NormativeNormType = {(int) normType}" +
                       ") " +
                       GetHydraulicLocationCalculationsFromFailureMechanismQuery(calculationType) +
                       GetHydraulicLocationCalculationOutputValidationSubQuery() +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate the new hydraulic boundary location calculations that are not based on migrated data.
            /// </summary>
            /// <param name="calculationType">The type of calculation on which the input should be validated.</param>
            /// <returns>The query to validate the hydraulic boundary location calculation output.</returns>
            public string GetNewCalculationsValidationQuery(CalculationType calculationType)
            {
                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT COUNT() = (SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity) " +
                       GetHydraulicLocationCalculationsFromFailureMechanismQuery(calculationType) +
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
                       GetHydraulicLocationCalculationsFromFailureMechanismQuery(calculationType) +
                       "JOIN HydraulicLocationOutputEntity USING(HydraulicLocationCalculationEntityId); ";
            }

            private static string GetHydraulicLocationCalculationsFromFailureMechanismQuery(CalculationType calculationType)
            {
                return "FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmm " +
                       "JOIN HydraulicLocationCalculationCollectionEntity " +
                       $"ON gceofmm.HydraulicLocationCalculationCollectionEntity{(int) calculationType}Id = HydraulicLocationCalculationCollectionEntityId " +
                       "JOIN HydraulicLocationCalculationEntity USING(HydraulicLocationCalculationCollectionEntityId) " +
                       "JOIN HydraulicLocationEntity hl USING(HydraulicLocationEntityId) " +
                       "JOIN FailureMechanismEntity fm USING(FailureMechanismEntityId) " +
                       "JOIN AssessmentSectionEntity USING(AssessmentSectionEntityId) ";
            }

            private static string GetHydraulicLocationCalculationOutputValidationSubQuery()
            {
                return "JOIN HydraulicLocationOutputEntity NEW USING(HydraulicLocationCalculationEntityId) " +
                       "JOIN ( " +
                       "SELECT " +
                       "LocationId, " +
                       "AssessmentSectionEntityId, " +
                       "GeneralResultSubMechanismIllustrationPointEntityId, " +
                       "Result,  " +
                       "TargetProbability, " +
                       "TargetReliability, " +
                       "CalculatedProbability, " +
                       "CalculatedReliability, " +
                       "CalculationConvergence " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity " +
                       "JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity USING(GrassCoverErosionOutwardsHydraulicLocationEntityId) " +
                       "JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       ") OLD ON (fm.AssessmentSectionEntityId = OLD.AssessmentSectionEntityId AND OLD.LocationId = hl.LocationId)" +
                       "WHERE NEW.GeneralResultSubMechanismIllustrationPointEntityId IS OLD.GeneralResultSubMechanismIllustrationPointEntityId " +
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
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
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
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

        #region Migrated Wave Condition Calculations

        private static void AssertWaveConditionsCalculations(MigratedDatabaseReader reader, string sourceFilePath)
        {
            var queryGenerator = new WaveConditionsCalculationValidationQueryGenerator(sourceFilePath);

            reader.AssertReturnedDataIsValid(queryGenerator.GetGrassCoverErosionOutwardsCalculationValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetGrassCoverErosionOutwardsCalculationValidationQuery(NormativeNormType.SignalingNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetStabilityStoneCoverCalculationValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetStabilityStoneCoverCalculationValidationQuery(NormativeNormType.SignalingNorm));

            reader.AssertReturnedDataIsValid(queryGenerator.GetWaveImpactAsphaltCoverCalculationValidationQuery(NormativeNormType.LowerLimitNorm));
            reader.AssertReturnedDataIsValid(queryGenerator.GetWaveImpactAsphaltCoverCalculationValidationQuery(NormativeNormType.SignalingNorm));
        }

        /// <summary>
        /// Class to generate queries which can be used to assert if the wave conditions calculations
        /// are correctly migrated.
        /// </summary>
        private class WaveConditionsCalculationValidationQueryGenerator
        {
            private readonly string sourceFilePath;

            /// <summary>
            /// Creates a new instance of <see cref="WaveConditionsCalculationValidationQueryGenerator"/>.
            /// </summary>
            /// <param name="sourceFilePath">The file path of the original database.</param>
            /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFilePath"/>
            /// is <c>null</c> or empty.</exception>
            public WaveConditionsCalculationValidationQueryGenerator(string sourceFilePath)
            {
                if (string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    throw new ArgumentException(@"Sourcefile path cannot be null or empty",
                                                nameof(sourceFilePath));
                }

                this.sourceFilePath = sourceFilePath;
            }

            /// <summary>
            /// Generates a query to validate if the grass cover erosion outwards calculations are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the grass cover erosion outwards calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetGrassCoverErosionOutwardsCalculationValidationQuery(NormativeNormType normType)
            {
                FailureMechanismCategoryType categoryType = ConvertToFailureMechanismCategoryType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = " +
                       "(" +
                       "SELECT COUNT() " +
                       "FROM[SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity " +
                       "JOIN[SOURCEPROJECT].CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType}" +
                       ") " +
                       "FROM GrassCoverErosionOutwardsWaveConditionsCalculationEntity NEW " +
                       "JOIN CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       "LEFT JOIN HydraulicLocationEntity hl USING(HydraulicLocationEntityId) " +
                       "JOIN ( " +
                       "SELECT " +
                       "[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId], " +
                       "[LocationId], " +
                       "[AssessmentSectionEntityId], " +
                       "calc.CalculationGroupEntityId, " +
                       "[ForeshoreProfileEntityId], " +
                       "calc.'Order', " +
                       "calc.Name, " +
                       "[Comments], " +
                       "[UseBreakWater], " +
                       "[BreakWaterType], " +
                       "[BreakWaterHeight], " +
                       "[UseForeshore], " +
                       "[Orientation], " +
                       "[UpperBoundaryRevetment], " +
                       "[LowerBoundaryRevetment], " +
                       "[UpperBoundaryWaterLevels], " +
                       "[LowerBoundaryWaterLevels], " +
                       "[StepSize] " +
                       "FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity calc " +
                       "LEFT JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity USING(GrassCoverErosionOutwardsHydraulicLocationEntityId) " +
                       "LEFT JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId) " +
                       ") OLD USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId) " +
                       GetCommonWaveConditionsCalculationPropertiesValidationString(normType) +
                       "AND OLD.LocationId IS hl.LocationId " +
                       $"AND NEW.CategoryType = {(int) categoryType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the stability stone cover calculations are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the migrated stability stone cover calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetStabilityStoneCoverCalculationValidationQuery(NormativeNormType normType)
            {
                AssessmentSectionCategoryType categoryType = ConvertToAssessmentSectionCategoryType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = " +
                       "(" +
                       "SELECT COUNT() " +
                       "FROM[SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity " +
                       "JOIN[SOURCEPROJECT].CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType}" +
                       ") " +
                       "FROM StabilityStoneCoverWaveConditionsCalculationEntity NEW " +
                       "JOIN [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity OLD USING(StabilityStoneCoverWaveConditionsCalculationEntityId) " +
                       GetCommonWaveConditionsCalculationPropertiesValidationString(normType) +
                       "AND NEW.HydraulicLocationEntityId IS OLD.HydraulicLocationEntityId " +
                       $"AND NEW.CategoryType = {(int) categoryType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            /// <summary>
            /// Generates a query to validate if the wave impact asphalt cover calculations are migrated correctly.
            /// </summary>
            /// <param name="normType">The norm type to generate the query for.</param>
            /// <returns>A query to validate the migrated wave impact asphalt cover calculations.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            public string GetWaveImpactAsphaltCoverCalculationValidationQuery(NormativeNormType normType)
            {
                AssessmentSectionCategoryType categoryType = ConvertToAssessmentSectionCategoryType(normType);

                return $"ATTACH DATABASE \"{sourceFilePath}\" AS SOURCEPROJECT; " +
                       "SELECT  " +
                       "COUNT() = " +
                       "(" +
                       "SELECT COUNT() " +
                       "FROM[SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity " +
                       "JOIN[SOURCEPROJECT].CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN[SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId) " +
                       $"WHERE NormativeNormType = {(int) normType}" +
                       ") " +
                       "FROM WaveImpactAsphaltCoverWaveConditionsCalculationEntity NEW " +
                       "JOIN [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity OLD USING(WaveImpactAsphaltCoverWaveConditionsCalculationEntityId) " +
                       GetCommonWaveConditionsCalculationPropertiesValidationString(normType) +
                       "AND NEW.HydraulicLocationEntityId IS OLD.HydraulicLocationEntityId " +
                       $"AND NEW.CategoryType = {(int) categoryType}; " +
                       "DETACH DATABASE SOURCEPROJECT;";
            }

            private static string GetCommonWaveConditionsCalculationPropertiesValidationString(NormativeNormType normType)
            {
                return "JOIN CalculationGroupEntity USING(CalculationGroupEntityId) " +
                       "JOIN FailureMechanismEntity USING(CalculationGroupEntityId) " +
                       "JOIN AssessmentSectionEntity ase USING(AssessmentSectionEntityId)" +
                       $"WHERE ase.NormativeNormType = {(int) normType} " +
                       "AND NEW.CalculationGroupEntityId = OLD.CalculationGroupEntityId " +
                       "AND NEW.ForeshoreProfileEntityId IS OLD.ForeshoreProfileEntityId " +
                       "AND NEW.\"Order\" = OLD.\"Order\" " +
                       "AND NEW.Name IS OLD.Name " +
                       "AND NEW.Comments IS OLD.Comments " +
                       "AND NEW.UseBreakWater = OLD.UseBreakWater " +
                       "AND NEW.BreakWaterType = OLD.BreakWaterType " +
                       "AND NEW.BreakWaterHeight IS OLD.BreakWaterHeight " +
                       "AND NEW.UseForeshore = OLD.UseForeshore " +
                       "AND NEW.Orientation IS OLD.Orientation " +
                       "AND NEW.UpperBoundaryRevetment IS OLD.UpperBoundaryRevetment " +
                       "AND NEW.LowerBoundaryRevetment IS OLD.LowerBoundaryRevetment " +
                       "AND NEW.UpperBoundaryWaterLevels IS OLD.UpperBoundaryWaterLevels " +
                       "AND NEW.LowerBoundaryWaterLevels IS OLD.LowerBoundaryWaterLevels " +
                       "AND NEW.StepSize = OLD.StepSize ";
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding category type from <see cref="Ringtoets.Common.Data.FailureMechanism.FailureMechanismCategoryType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="Ringtoets.Common.Data.FailureMechanism.FailureMechanismCategoryType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            private static FailureMechanismCategoryType ConvertToFailureMechanismCategoryType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.SignalingNorm:
                        return FailureMechanismCategoryType.MechanismSpecificSignalingNorm;
                    case NormativeNormType.LowerLimitNorm:
                        return FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Converts the <see cref="NormativeNormType"/> to the corresponding category type from <see cref="Ringtoets.Common.Data.AssessmentSection.AssessmentSectionCategoryType"/>.
            /// </summary>
            /// <param name="normType">The norm type to convert.</param>
            /// <returns>Returns the converted <see cref="Ringtoets.Common.Data.AssessmentSection.AssessmentSectionCategoryType"/>.</returns>
            /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> 
            /// is an invalid value of <see cref="NormativeNormType"/>.</exception>
            /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
            /// but unsupported.</exception>
            private static AssessmentSectionCategoryType ConvertToAssessmentSectionCategoryType(NormativeNormType normType)
            {
                if (!Enum.IsDefined(typeof(NormativeNormType), normType))
                {
                    throw new InvalidEnumArgumentException(nameof(normType), (int) normType, typeof(NormativeNormType));
                }

                switch (normType)
                {
                    case NormativeNormType.SignalingNorm:
                        return AssessmentSectionCategoryType.SignalingNorm;
                    case NormativeNormType.LowerLimitNorm:
                        return AssessmentSectionCategoryType.LowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion
    }
}