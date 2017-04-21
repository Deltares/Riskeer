/*
Migration script for migrating Ringtoets databases.
SourceProject version: 5
TargetProject version: 17.1
*/
PRAGMA foreign_keys = OFF;

ATTACH DATABASE [{0}] AS SOURCEPROJECT;

INSERT INTO AssessmentSectionEntity SELECT * FROM [SOURCEPROJECT].AssessmentSectionEntity;
INSERT INTO CalculationGroupEntity SELECT * FROM [SOURCEPROJECT].CalculationGroupEntity;
INSERT INTO CharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].CharacteristicPointEntity;
INSERT INTO ClosingStructureEntity SELECT * FROM [SOURCEPROJECT].ClosingStructureEntity;
INSERT INTO ClosingStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].ClosingStructureFailureMechanismMetaEntity;
INSERT INTO ClosingStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity;
INSERT INTO ClosingStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity;
INSERT INTO DikeProfileEntity (
	[DikeProfileEntityId],
	[FailureMechanismEntityId],
	[Id],
	[Name],
	[Orientation],
	[BreakWaterType],
	[BreakWaterHeight],
	[ForeshoreXml],
	[DikeGeometryXml],
	[DikeHeight],
	[X],
	[Y],
	[X0],
	[Order])
SELECT
	[DikeProfileEntityId],
	[FailureMechanismEntityId],
	CASE WHEN Suffix THEN [Name] || '(' || Suffix || ')' ELSE [Name] END as [Name],
	CASE WHEN Suffix THEN [Name] || '(' || Suffix || ')' ELSE [Name] END as [Name],
	[Orientation],
	[BreakWaterType],
	[BreakWaterHeight],
	[ForeshoreXml],
	[DikeGeometryXml],
	[DikeHeight],
	[X],
	[Y],
	[X0],
	[Order]
	FROM (SELECT *, (SELECT count(*)
                     FROM [SOURCEPROJECT].DikeProfileEntity
                     WHERE DP.DikeProfileEntityId > DikeProfileEntityId
                     AND DP.Name IS Name
                     AND DP.FailuremechanismEntityId = FailuremechanismEntityId) as Suffix
	FROM [SOURCEPROJECT].DikeProfileEntity DP);
INSERT INTO DuneErosionSectionResultEntity SELECT * FROM [SOURCEPROJECT].DuneErosionSectionResultEntity;
INSERT INTO FailureMechanismEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismEntity;
INSERT INTO FailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismSectionEntity;
INSERT INTO ForeshoreProfileEntity (
	[ForeshoreProfileEntityId],
	[FailureMechanismEntityId],
	[Id],
	[Name],
	[Orientation],
	[BreakWaterType],
	[BreakWaterHeight],
	[GeometryXml],
	[X],
	[Y],
	[X0],
	[Order])
SELECT 
	[ForeshoreProfileEntityId],
	[FailureMechanismEntityId],
	[Name],
	[Name],
	[Orientation],
	[BreakWaterType],
	[BreakWaterHeight],
	[GeometryXml],
	[X],
	[Y],
	[X0],
	[Order]
	FROM [SOURCEPROJECT].ForeshoreProfileEntity;
INSERT INTO GrassCoverErosionInwardsCalculationEntity (
	[GrassCoverErosionInwardsCalculationEntityId],
	[CalculationGroupEntityId],
	[HydraulicLocationEntityId],
	[DikeProfileEntityId],
	[Order],
	[Name],
	[Comments],
	[Orientation],
	[CriticalFlowRateMean],
	[CriticalFlowRateStandardDeviation],
	[UseForeshore],
	[DikeHeightCalculationType],
	[DikeHeight],
	[UseBreakWater],
	[BreakWaterType],
	[BreakWaterHeight],
	[OvertoppingRateCalculationType])
SELECT
	[GrassCoverErosionInwardsCalculationEntityId],
	[CalculationGroupEntityId],
	[HydraulicLocationEntityId],
	[DikeProfileEntityId],
	[Order],
	[Name],
	[Comments],
	[Orientation],
	[CriticalFlowRateMean],
	[CriticalFlowRateStandardDeviation],
	[UseForeshore],
	[DikeHeightCalculationType],
	[DikeHeight],
	[UseBreakWater],
	[BreakWaterType],
	[BreakWaterHeight],
	1
	FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity;
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity (
	[GrassCoverErosionInwardsFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	[DikeProfileCollectionSourcePath])
SELECT 
	[GrassCoverErosionInwardsFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	"Onbekend"
	FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionOutwardsHydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity;
INSERT INTO GrassCoverErosionOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
INSERT INTO GrassCoverSlipOffInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity;
INSERT INTO GrassCoverSlipOffOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity;
INSERT INTO HeightStructureEntity SELECT * FROM [SOURCEPROJECT].HeightStructureEntity;
INSERT INTO HeightStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
INSERT INTO HeightStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity;
INSERT INTO HeightStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity;
INSERT INTO HydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationEntity;
INSERT INTO MacrostabilityInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].MacrostabilityInwardsSectionResultEntity;
INSERT INTO MacrostabilityOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].MacrostabilityOutwardsSectionResultEntity;
INSERT INTO MicrostabilitySectionResultEntity SELECT * FROM [SOURCEPROJECT].MicrostabilitySectionResultEntity;
INSERT INTO PipingCalculationEntity SELECT * FROM [SOURCEPROJECT].PipingCalculationEntity;
INSERT INTO PipingFailureMechanismMetaEntity (
	[PipingFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[A],
	[WaterVolumetricWeight],
	[StochasticSoilModelSourcePath],
	[SurfacelineSourcePath]) 
SELECT 
	[PipingFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[A],
	[WaterVolumetricWeight], 
	"Onbekend", 
	"Onbekend" 
	FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingSectionResultEntity;
INSERT INTO PipingStructureSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingStructureSectionResultEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO SoilLayerEntity 
SELECT 
	[SoilLayerEntityId],
	[SoilProfileEntityId],
	[Top],
	[IsAquifer],
	[Color],
	[MaterialName],
	[BelowPhreaticLevelMean],
	[BelowPhreaticLevelDeviation],
	[DiameterD70Mean],
	ROUND([DiameterD70Deviation] / [DiameterD70Mean], 6),
	[BelowPhreaticLevelShift],
	[PermeabilityMean],
	ROUND([PermeabilityDeviation] / [PermeabilityMean], 6),
	[Order]
	FROM [SOURCEPROJECT].SoilLayerEntity;
INSERT INTO SoilProfileEntity SELECT * FROM [SOURCEPROJECT].SoilProfileEntity;
INSERT INTO StabilityPointStructureEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityPointStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity;
INSERT INTO StabilityStoneCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity;
INSERT INTO StochasticSoilModelEntity 
SELECT 
	[StochasticSoilModelEntityId],
	[FailuremechanismEntityId],
	CASE WHEN Suffix THEN [Name] || '(' || Suffix || ')' ELSE [Name] END as [Name],
	[SegmentName],
	[StochasticSoilModelSegmentPointXml],
	[Order]
	FROM (SELECT *, (SELECT count(*)
                     FROM [SOURCEPROJECT].StochasticSoilModelEntity
                     WHERE SSM.StochasticSoilModelEntityId > StochasticSoilModelEntityId
                     AND SSM.Name IS Name
                     AND SSM.FailuremechanismEntityId = FailuremechanismEntityId) as Suffix
	FROM [SOURCEPROJECT].StochasticSoilModelEntity SSM);
INSERT INTO StochasticSoilProfileEntity 
SELECT
    [StochasticSoilProfileEntityId],
    [SoilProfileEntityId],
    [StochasticSoilmodelEntityId], 
	1,
    CASE
        WHEN [Probability] BETWEEN 0 AND 1 THEN [Probability]
        ELSE 0
    END as [Probability],
    [Order] 
FROM [SOURCEPROJECT].StochasticSoilProfileEntity;
INSERT INTO StrengthStabilityLengthwiseConstructionSectionResultEntity SELECT * FROM [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity;
INSERT INTO SurfaceLineEntity
SELECT 
	[SurfaceLineEntityId],
	[FailuremechanismEntityId],
	CASE WHEN Suffix THEN [Name] || '(' || Suffix || ')' ELSE [Name] END as [Name],
	[ReferenceLineIntersectionX],
	[ReferenceLineIntersectionY],
	[PointsXml],
	[Order]
	FROM (SELECT *, (SELECT count(*)
                     FROM [SOURCEPROJECT].SurfaceLineEntity
                     WHERE SL.SurfaceLineEntityId > SurfaceLineEntityId
                     AND SL.Name IS Name
                     AND SL.FailuremechanismEntityId = FailuremechanismEntityId) as Suffix
	FROM [SOURCEPROJECT].SurfaceLineEntity SL);
INSERT INTO TechnicalInnovationSectionResultEntity SELECT * FROM [SOURCEPROJECT].TechnicalInnovationSectionResultEntity;
INSERT INTO VersionEntity (
	[VersionId], 
	[Version], 
	[Timestamp], 
	[FingerPrint])
SELECT [VersionId],
	"17.1", 
	[Timestamp], 
	[FingerPrint]
	FROM [SOURCEPROJECT].VersionEntity;
INSERT INTO WaterPressureAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity;

/*
Outputs are not migrated
*/
--ClosingStructuresOutputEntity
--GrassCoverErosionInwardsDikeHeightOutputEntity
--GrassCoverErosionInwardsOutputEntity
--GrassCoverErosionOutwardsHydraulicLocationOutputEntity
--GrassCoverErosionOutwardsWaveConditionsOutputEntity
--HeightStructuresOutputEntity
--HydraulicLocationOutputEntity
--PipingCalculationOutputEntity
--PipingSemiProbabilisticOutputEntity
--StabilityPointStructuresOutputEntity
--StabilityStoneCoverWaveConditionsOutputEntity
--WaveImpactAsphaltCoverWaveConditionsOutputEntity

/*
Insert new data
*/
INSERT INTO DuneErosionFailureMechanismMetaEntity (
		[FailureMechanismEntityId], 
		[N])
SELECT FailureMechanismEntityId,
		2.0 
		FROM FailureMechanismEntity WHERE FailureMechanismType = 8;
INSERT INTO BackgroundDataEntity (
		[AssessmentSectionEntityId],
		[Name],
		[IsVisible],
		[Transparency],
		[BackgroundDataType])
SELECT AssessmentSectionEntityId,
		"Bing Maps - Satelliet",
		1,
		0.0,
		2
		FROM AssessmentSectionEntity;
INSERT INTO BackgroundDataMetaEntity(
		[BackgroundDataEntityId],
		[Key],
		[Value])
SELECT BackgroundDataEntityId,
		"WellKnownTileSource",
		"2"
		FROM BackgroundDataEntity;

/* 
Write migration logging 
*/
ATTACH DATABASE [{1}] AS LOGDATABASE;

CREATE TEMP TABLE log_output_deleted (
	'NrDeleted' INTEGER NOT NULL
);

INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].ClosingStructuresOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].GrassCoverErosionInwardsDikeHeightOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].HeightStructuresOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].HydraulicLocationOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].PipingCalculationOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].PipingSemiProbabilisticOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsOutputEntity;
INSERT INTO log_output_deleted SELECT COUNT('x') FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity;

CREATE TABLE  IF NOT EXISTS [LOGDATABASE].'MigrationLogEntity' 
(
	'MigrationLogEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
	'FromVersion' VARCHAR(20), 
	'ToVersion' VARCHAR(20), 
	'LogMessage' TEXT 
);

INSERT INTO [LOGDATABASE].MigrationLogEntity(
		[FromVersion], 
		[ToVersion], 
		[LogMessage])
SELECT	"5", 
		"17.1", 
		"Alle berekende resultaten zijn verwijderd."
		FROM log_output_deleted 
		WHERE [NrDeleted] > 0 
		LIMIT 1;

DROP TABLE log_output_deleted;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;