/*
Migration script for migrating Ringtoets databases.
SourceProject version: 17.2
TargetProject version: 17.3
*/
PRAGMA foreign_keys = OFF;

ATTACH DATABASE "{0}" AS SOURCEPROJECT;

INSERT INTO AssessmentSectionEntity SELECT * FROM [SOURCEPROJECT].AssessmentSectionEntity;
INSERT INTO BackgroundDataEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataEntity;
INSERT INTO BackgroundDataMetaEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataMetaEntity;
INSERT INTO CalculationGroupEntity SELECT * FROM [SOURCEPROJECT].CalculationGroupEntity;
INSERT INTO ClosingStructureEntity SELECT * FROM [SOURCEPROJECT].ClosingStructureEntity;
INSERT INTO ClosingStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity;
INSERT INTO ClosingStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresFailureMechanismMetaEntity;
INSERT INTO ClosingStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresOutputEntity;
INSERT INTO ClosingStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity;
INSERT INTO DikeProfileEntity SELECT * FROM [SOURCEPROJECT].DikeProfileEntity;
INSERT INTO DuneErosionFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity;
INSERT INTO DuneErosionSectionResultEntity SELECT * FROM [SOURCEPROJECT].DuneErosionSectionResultEntity;
INSERT INTO DuneLocationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationEntity;
INSERT INTO DuneLocationOutputEntity SELECT * FROM [SOURCEPROJECT].DuneLocationOutputEntity;
INSERT INTO FailureMechanismEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismEntity;
INSERT INTO FailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismSectionEntity;
INSERT INTO FaultTreeIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].FaultTreeIllustrationPointEntity;
INSERT INTO FaultTreeIllustrationPointStochastEntity SELECT * FROM [SOURCEPROJECT].FaultTreeIllustrationPointStochastEntity;
INSERT INTO FaultTreeSubmechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].FaultTreeSubmechanismIllustrationPointEntity;
INSERT INTO ForeshoreProfileEntity SELECT * FROM [SOURCEPROJECT].ForeshoreProfileEntity;
INSERT INTO GeneralResultFaultTreeIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].GeneralResultFaultTreeIllustrationPointEntity;
INSERT INTO GeneralResultFaultTreeIllustrationPointStochastEntity SELECT * FROM [SOURCEPROJECT].GeneralResultFaultTreeIllustrationPointStochastEntity;
INSERT INTO GeneralResultSubMechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].GeneralResultSubMechanismIllustrationPointEntity;
INSERT INTO GeneralResultSubMechanismIllustrationPointStochastEntity SELECT * FROM [SOURCEPROJECT].GeneralResultSubMechanismIllustrationPointStochastEntity;
INSERT INTO GrassCoverErosionInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity;
INSERT INTO GrassCoverErosionInwardsDikeHeightOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsDikeHeightOutputEntity;
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionInwardsOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity;
INSERT INTO GrassCoverErosionInwardsOvertoppingRateOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsOvertoppingRateOutputEntity;
INSERT INTO GrassCoverErosionInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionOutwardsHydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity;
INSERT INTO GrassCoverErosionOutwardsHydraulicLocationOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity;
INSERT INTO GrassCoverErosionOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity;
INSERT INTO GrassCoverSlipOffInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity;
INSERT INTO GrassCoverSlipOffOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity;
INSERT INTO HeightStructureEntity SELECT * FROM [SOURCEPROJECT].HeightStructureEntity;
INSERT INTO HeightStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
INSERT INTO HeightStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity;
INSERT INTO HeightStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresOutputEntity;
INSERT INTO HeightStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity;
INSERT INTO HydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationEntity;
INSERT INTO HydraulicLocationOutputEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationOutputEntity;
INSERT INTO IllustrationPointResultEntity SELECT * FROM [SOURCEPROJECT].IllustrationPointResultEntity;
INSERT INTO MacroStabilityInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity;
INSERT INTO MacroStabilityInwardsCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCharacteristicPointEntity;
INSERT INTO MacroStabilityInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsFailureMechanismMetaEntity;
INSERT INTO MacroStabilityInwardsPreconsolidationStressEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsPreconsolidationStressEntity;
INSERT INTO MacroStabilityInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSectionResultEntity;
INSERT INTO MacroStabilityInwardsSoilLayerOneDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilLayerOneDEntity;
INSERT INTO MacroStabilityInwardsSoilLayerTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilLayerTwoDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileOneDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileOneDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileTwoDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity;
INSERT INTO MacroStabilityInwardsStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsStochasticSoilProfileEntity;
INSERT INTO MacrostabilityOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].MacrostabilityOutwardsSectionResultEntity;
INSERT INTO MicrostabilitySectionResultEntity SELECT * FROM [SOURCEPROJECT].MicrostabilitySectionResultEntity;
INSERT INTO PipingCalculationEntity SELECT * FROM [SOURCEPROJECT].PipingCalculationEntity;
INSERT INTO PipingCalculationOutputEntity SELECT * FROM [SOURCEPROJECT].PipingCalculationOutputEntity;
INSERT INTO PipingCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].PipingCharacteristicPointEntity;
INSERT INTO PipingFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingSectionResultEntity;
INSERT INTO PipingSemiProbabilisticOutputEntity SELECT * FROM [SOURCEPROJECT].PipingSemiProbabilisticOutputEntity;
INSERT INTO PipingSoilLayerEntity SELECT * FROM [SOURCEPROJECT].PipingSoilLayerEntity;
INSERT INTO PipingSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingSoilProfileEntity;
INSERT INTO PipingStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingStochasticSoilProfileEntity;
INSERT INTO PipingStructureSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingStructureSectionResultEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO StabilityPointStructureEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityPointStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity;
INSERT INTO StabilityPointStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity;
INSERT INTO StabilityStoneCoverWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsOutputEntity;
INSERT INTO StochastEntity SELECT * FROM [SOURCEPROJECT].StochastEntity;
INSERT INTO StochasticSoilModelEntity SELECT * FROM [SOURCEPROJECT].StochasticSoilModelEntity;
INSERT INTO StrengthStabilityLengthwiseConstructionSectionResultEntity SELECT * FROM [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity;
INSERT INTO SubMechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].SubMechanismIllustrationPointEntity;
INSERT INTO SubMechanismIllustrationPointStochastEntity SELECT * FROM [SOURCEPROJECT].SubMechanismIllustrationPointStochastEntity;
INSERT INTO SurfaceLineEntity SELECT * FROM [SOURCEPROJECT].SurfaceLineEntity;
INSERT INTO TechnicalInnovationSectionResultEntity SELECT * FROM [SOURCEPROJECT].TechnicalInnovationSectionResultEntity;
INSERT INTO TopLevelFaultTreeIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].TopLevelFaultTreeIllustrationPointEntity;
INSERT INTO TopLevelSubMechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].TopLevelSubMechanismIllustrationPointEntity;
INSERT INTO VersionEntity (
	[VersionId],
	[Version],
	[Timestamp],
	[FingerPrint])
SELECT [VersionId],
	"17.3",
	[Timestamp],
	[FingerPrint]
FROM [SOURCEPROJECT].VersionEntity;
INSERT INTO WaterPressureAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverFailureMechanismMetaEntity;
INSERT INTO WaveImpactAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity;

/*
Following outputs are not migrated:
*/
--MacroStabilityInwardsCalculationOutputEntity
--MacroStabilityInwardsSemiProbabilisticOutputEntity

/* 
Write migration logging
*/
ATTACH DATABASE "{1}" AS LOGDATABASE;

CREATE TABLE IF NOT EXISTS [LOGDATABASE].'MigrationLogEntity'
(
	'MigrationLogEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FromVersion' VARCHAR(20) NOT NULL,
	'ToVersion' VARCHAR(20) NOT NULL,
	'LogMessage' TEXT NOT NULL
);

CREATE TEMP TABLE TempLogOutputDeleted
(
	'NrDeleted' INTEGER NOT NULL
);
INSERT INTO TempLogOutputDeleted
SELECT COUNT()
FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity;
	
INSERT INTO TempLogOutputDeleted
SELECT COUNT()
FROM [SOURCEPROJECT].MacroStabilityInwardsSemiProbabilisticOutputEntity;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion],
	[ToVersion],
	[LogMessage])
VALUES ("17.2", "17.3", "Gevolgen van de migratie van versie 17.2 naar versie 17.3:");

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion], 
	[ToVersion], 
	[LogMessage])
SELECT "17.2",
	"17.3",
	"* Alle berekende resultaten van het faalmechanisme 'Macrostabiliteit binnenwaarts' zijn verwijderd."
	FROM TempLogOutputDeleted
	WHERE [NrDeleted] > 0
	LIMIT 1;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion],
	[ToVersion],
	[LogMessage])
SELECT "17.2",
	"17.3", 
	"* Geen aanpassingen."
	WHERE (
		SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
		WHERE [FromVersion] = "17.2"
	) IS 1;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;