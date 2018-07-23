/*
Migration script for migrating Ringtoets databases.
SourceProject version: 5
TargetProject version: 17.1
*/
PRAGMA foreign_keys = OFF;

ATTACH DATABASE "{0}" AS SOURCEPROJECT;

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
	CASE WHEN Suffix THEN [Name] || 
	SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength)
	|| Suffix ELSE [Name] END as [Id],
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
	[Order]
	FROM (SELECT *, MaxLength - LENGTH(NAME) as SuffixPreLength, (SELECT DP.rowid
                     FROM [SOURCEPROJECT].DikeProfileEntity
                     WHERE DP.DikeProfileEntityId > DikeProfileEntityId
                     AND DP.Name IS Name
                     AND DP.FailuremechanismEntityId = FailuremechanismEntityId) as Suffix
	FROM [SOURCEPROJECT].DikeProfileEntity DP
	JOIN (SELECT MAX(LENGTH(Name)) as MaxLength FROM [SOURCEPROJECT].DikeProfileEntity));
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
	CASE WHEN COUNT([DikeProfileEntityId]) THEN "" ELSE NULL END
	FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].DikeProfileEntity USING (FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityId;
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
	[SurfaceLineSourcePath]) 
SELECT 
	[PipingFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[A],
	[WaterVolumetricWeight], 
	CASE WHEN COUNT([StochasticSoilModelEntityId]) THEN "" ELSE NULL END,
	CASE WHEN COUNT([SurfaceLineEntityId]) THEN "" ELSE NULL END
	FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].StochasticSoilModelEntity USING (FailureMechanismEntityId)
	LEFT JOIN [SOURCEPROJECT].SurfaceLineEntity USING (FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityId;
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
INSERT INTO StochasticSoilModelEntity (
	[StochasticSoilModelEntityId],
	[FailureMechanismEntityId],
	[Name],
	[SegmentName],
	[StochasticSoilModelSegmentPointXml],
	[Order])
SELECT 
	[StochasticSoilModelEntityId],
	[FailuremechanismEntityId],
	CASE WHEN Suffix THEN [Name] || ' (' || SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength) || Suffix || ')' ELSE [Name] END AS [Name],
	[SegmentName],
	[StochasticSoilModelSegmentPointXml],
	[Order]
	FROM 
	(
		SELECT *, MAX(MaxLength - LENGTH(NAME), 0) as SuffixPreLength, 
		(
			SELECT COUNT() 
			FROM [SOURCEPROJECT].StochasticSoilModelEntity
			WHERE SSM.StochasticSoilModelEntityId > StochasticSoilModelEntityId
			AND SSM.Name IS Name
			AND SSM.FailuremechanismEntityId = FailuremechanismEntityId
		) AS Suffix 
		FROM [SOURCEPROJECT].StochasticSoilModelEntity SSM
		JOIN (
			SELECT MAX(LENGTH(Name)-3) as MaxLength FROM [SOURCEPROJECT].StochasticSoilModelEntity
		)
	);
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
INSERT INTO SurfaceLineEntity (
	[SurfaceLineEntityId],
	[FailureMechanismEntityId],
	[Name],
	[ReferenceLineIntersectionX],
	[ReferenceLineIntersectionY],
	[PointsXml],
	[Order])
SELECT 
	[SurfaceLineEntityId],
	[FailuremechanismEntityId],
	CASE WHEN Suffix THEN [Name] || ' (' || SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength) || Suffix || ')' ELSE [Name] END AS [Name],
	[ReferenceLineIntersectionX],
	[ReferenceLineIntersectionY],
	[PointsXml],
	[Order]	
	FROM 
	(
		SELECT *, MAX(MaxLength - LENGTH(NAME), 0) as SuffixPreLength, 
		(
			SELECT COUNT() 
			FROM [SOURCEPROJECT].SurfaceLineEntity
			WHERE SL.SurfaceLineEntityId > SurfaceLineEntityId
			AND SL.Name IS Name
			AND SL.FailuremechanismEntityId = FailuremechanismEntityId
	) AS Suffix 
		FROM [SOURCEPROJECT].SurfaceLineEntity SL
		JOIN (
			SELECT MAX(LENGTH(Name)-3) as MaxLength FROM [SOURCEPROJECT].SurfaceLineEntity
		)
	);
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
ATTACH DATABASE "{1}" AS LOGDATABASE;

CREATE TEMP TABLE TempLogOutputDeleted 
(
	'NrDeleted' INTEGER NOT NULL
);

INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].ClosingStructuresOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsDikeHeightOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].HeightStructuresOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].PipingCalculationOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].PipingSemiProbabilisticOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity;

CREATE TABLE IF NOT EXISTS [LOGDATABASE].'MigrationLogEntity' 
(
	'MigrationLogEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
	'FromVersion' VARCHAR(20) NOT NULL, 
	'ToVersion' VARCHAR(20) NOT NULL, 
	'LogMessage' TEXT NOT NULL
);

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion], 
	[ToVersion], 
	[LogMessage])
VALUES ("5", 
		"17.1", 
		"Gevolgen van de migratie van versie 16.4 naar versie 17.1:");

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion], 
	[ToVersion], 
	[LogMessage])
SELECT	"5", 
	"17.1", 
	"* Alle berekende resultaten zijn verwijderd."
	FROM TempLogOutputDeleted 
	WHERE [NrDeleted] > 0 
	LIMIT 1;

DROP TABLE TempLogOutputDeleted;

CREATE TEMP TABLE TempFailureMechanisms 
(
	'FailureMechanismType' INTEGER NOT NULL, 
	'FailureMechanismName' VARCHAR(255) NOT NULL
);

INSERT INTO TempFailureMechanisms VALUES (1, 'Piping');
INSERT INTO TempFailureMechanisms VALUES (2, 'Macrostabiliteit binnenwaarts');
INSERT INTO TempFailureMechanisms VALUES (3, 'Golfklappen op asfaltbekleding');
INSERT INTO TempFailureMechanisms VALUES (4, 'Grasbekleding erosie buitentalud');
INSERT INTO TempFailureMechanisms VALUES (5, 'Grasbekleding afschuiven buitentalud');
INSERT INTO TempFailureMechanisms VALUES (6, 'Grasbekleding erosie kruin en binnentalud');
INSERT INTO TempFailureMechanisms VALUES (7, 'Stabiliteit steenzetting');
INSERT INTO TempFailureMechanisms VALUES (8, 'Duinafslag');
INSERT INTO TempFailureMechanisms VALUES (9, 'Hoogte kunstwerk');
INSERT INTO TempFailureMechanisms VALUES (10, 'Betrouwbaarheid sluiting kunstwerk');
INSERT INTO TempFailureMechanisms VALUES (11, 'Piping bij kunstwerk');
INSERT INTO TempFailureMechanisms VALUES (12, 'Sterkte en stabiliteit puntconstructies');
INSERT INTO TempFailureMechanisms VALUES (13, 'Macrostabiliteit buitenwaarts');
INSERT INTO TempFailureMechanisms VALUES (14, 'Microstabiliteit');
INSERT INTO TempFailureMechanisms VALUES (15, 'Wateroverdruk bij asfaltbekleding');
INSERT INTO TempFailureMechanisms VALUES (16, 'Grasbekleding afschuiven binnentalud');
INSERT INTO TempFailureMechanisms VALUES (17, 'Sterkte en stabiliteit langsconstructies');
INSERT INTO TempFailureMechanisms VALUES (18, 'Technische innovaties');

CREATE TEMP TABLE TempAssessmentSectionFailureMechanism
(
	[AssessmentSectionId], 
	[AssessmentSectionName], 
	[FailureMechanismId], 
	[FailureMechanismName]
);

INSERT INTO TempAssessmentSectionFailureMechanism
SELECT 
	[AssessmentSectionEntityId], 
	[Name], 
	[FailureMechanismEntityId], 
	[FailureMechanismName]
	FROM AssessmentSectionEntity
	JOIN FailureMechanismEntity USING (AssessmentSectionEntityId)
	JOIN TempFailureMechanisms USING (FailureMechanismType);

CREATE TEMP TABLE TempChanges
(
	AssessmentSectionId, 
	AssessmentSectionName, 
	FailureMechanismId, 
	FailureMechanismName, 
	msg
);

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId], 
	asfm.[AssessmentSectionName], 
	asfm.[FailureMechanismId], 
	asfm.[FailureMechanismName], 
	"De naam van dijkprofiel '" || source.[Name] || "' is veranderd naar '" || dp.[Id] || "' en wordt ook gebruikt als ID."
	FROM DikeProfileEntity AS dp
	JOIN [SOURCEPROJECT].DikeProfileEntity AS source ON dp.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = dp.[FailureMechanismEntityId]
	WHERE source.[Name] IS NOT dp.[Id];

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion], 
	[ToVersion], 
	[LogMessage])
WITH RECURSIVE
FailureMechanismMessages
(
	[FailureMechanismId], 
	[FailureMechanismName], 
	[AssessmentSectionId], 
	[AssessmentSectionName], 
	[msg], 
	[level]
) AS (
SELECT DISTINCT 
	[FailureMechanismId], 
	[FailureMechanismName], 
	[AssessmentSectionId], 
	[AssessmentSectionName], 
	NULL, 
	1
	FROM TempChanges
	UNION
SELECT [FailureMechanismId], 
	NULL, 
	[AssessmentSectionId], 
	[AssessmentSectionName], 
	[msg], 
	2
	FROM TempChanges
	WHERE TempChanges.[FailureMechanismId] IS [FailureMechanismId]
	ORDER BY 1, 3
),
AssessmentSectionFailureMechanismMessages
(
	[AssessmentSectionId], 
	[AssessmentSectionName], 
	[FailureMechanismId], 
	[FailureMechanismName], 
	[msg], 
	[level]
) AS (
SELECT DISTINCT 
	[AssessmentSectionId], 
	[AssessmentSectionName], 
	NULL, 
	NULL,
	NULL, 
	0
	FROM FailureMechanismMessages
	WHERE [AssessmentSectionId] IS NOT NULL
	UNION
SELECT 
	[AssessmentSectionId], 
	NULL, 
	fmm.[FailureMechanismId], 
	fmm.[FailureMechanismName], 
	[msg], 
	fmm.[level]
	FROM FailureMechanismMessages AS fmm
	WHERE fmm.[AssessmentSectionId] IS [AssessmentSectionId]
	ORDER BY 1, 3, 6
)
SELECT 
	"5", 
	"17.1",
	CASE WHEN [AssessmentSectionName] IS NOT NULL 
		THEN "* Traject: '" || [AssessmentSectionName] || "'" 
	ELSE 
		CASE WHEN [FailureMechanismName] IS NOT NULL 
			THEN "  + Toetsspoor: '" || [FailureMechanismName] || "'" 
		ELSE
			"    - " || [msg] 
		END 
	END
FROM AssessmentSectionFailureMechanismMessages;

DROP TABLE TempFailureMechanisms;
DROP TABLE TempAssessmentSectionFailureMechanism;
DROP TABLE TempChanges;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion], 
	[ToVersion], 
	[LogMessage])
SELECT "5", 
	"17.1", 
	"* Geen aanpassingen."
	WHERE (
		SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
		WHERE [FromVersion] = "5"
	) IS 1;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;