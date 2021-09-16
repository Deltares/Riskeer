/*
Migration script for migrating Riskeer databases.
SourceProject version: 21.1
TargetProject version: 21.2
*/

PRAGMA foreign_keys = OFF;

ATTACH DATABASE "{0}" AS SOURCEPROJECT;

INSERT INTO AssessmentSectionEntity (
    [AssessmentSectionEntityId],
    [ProjectEntityId],
    [HydraulicLocationCalculationCollectionEntity1Id],	-- Represents the design water level signaling norm
    [HydraulicLocationCalculationCollectionEntity2Id],	-- Represents the design water level lower limit norm
    [Id],
    [Name],
    [Comments],
    [LowerLimitNorm],
    [SignalingNorm],
    [NormativeNormType],
    [Composition],
    [ReferenceLinePointXml],
    [Order]
) 
SELECT
    [AssessmentSectionEntityId],
    [ProjectEntityId],
    [HydraulicLocationCalculationCollectionEntity2Id],
    [HydraulicLocationCalculationCollectionEntity3Id],
    [Id],
    [Name],
    [Comments],
    [LowerLimitNorm],
    [SignalingNorm],
    [NormativeNormType],
    [Composition],
    [ReferenceLinePointXml],
    [Order]
FROM [SOURCEPROJECT].AssessmentSectionEntity;
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
INSERT INTO DuneLocationCalculationCollectionEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationCollectionEntity;
INSERT INTO DuneLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationEntity;
INSERT INTO DuneLocationCalculationOutputEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationOutputEntity;
INSERT INTO DuneLocationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationEntity;
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
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionInwardsOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity;
INSERT INTO GrassCoverErosionInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity (
    [GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [N],
    [ForeshoreProfileCollectionSourcePath]
)
SELECT
    [GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [N],
    [ForeshoreProfileCollectionSourcePath]
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity;
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
INSERT INTO HydraulicBoundaryDatabaseEntity SELECT * FROM [SOURCEPROJECT].HydraulicBoundaryDatabaseEntity;

INSERT INTO HydraulicLocationCalculationCollectionEntity (
    [HydraulicLocationCalculationCollectionEntityId]                                            
)
SELECT 
    [HydraulicLocationCalculationCollectionEntityId]
FROM [SOURCEPROJECT].AssessmentSectionEntity ase
JOIN [SOURCEPROJECT].HydraulicLocationCalculationCollectionEntity ON ase.HydraulicLocationCalculationCollectionEntity2Id = HydraulicLocationCalculationCollectionEntityId
                                                                  OR ase.HydraulicLocationCalculationCollectionEntity3Id = HydraulicLocationCalculationCollectionEntityId;

INSERT INTO HydraulicLocationCalculationEntity(
    [HydraulicLocationCalculationEntityId],
    [HydraulicLocationEntityId],
    [HydraulicLocationCalculationCollectionEntityId],
    [ShouldIllustrationPointsBeCalculated]
) 
SELECT
    [HydraulicLocationCalculationEntityId],
    [HydraulicLocationEntityId],
    [HydraulicLocationCalculationCollectionEntityId],
    [ShouldIllustrationPointsBeCalculated]
FROM [SOURCEPROJECT].AssessmentSectionEntity ase
JOIN [SOURCEPROJECT].HydraulicLocationCalculationCollectionEntity ON ase.HydraulicLocationCalculationCollectionEntity2Id = HydraulicLocationCalculationCollectionEntityId
                                                                  OR ase.HydraulicLocationCalculationCollectionEntity3Id = HydraulicLocationCalculationCollectionEntityId
JOIN [SOURCEPROJECT].HydraulicLocationCalculationEntity USING(HydraulicLocationCalculationCollectionEntityId);

INSERT INTO HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity (
    [HydraulicLocationCalculationCollectionEntityId],
    [HydraulicLocationCalculationEntityId]                                                                               
)
SELECT
    [HydraulicLocationCalculationCollectionEntityId],
    [HydraulicLocationCalculationEntityId]
FROM HydraulicLocationCalculationCollectionEntity 
JOIN [SOURCEPROJECT].HydraulicLocationCalculationEntity USING(HydraulicLocationCalculationCollectionEntityId);

INSERT INTO HydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationEntity;
INSERT INTO HydraulicLocationOutputEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationOutputEntity;
INSERT INTO IllustrationPointResultEntity SELECT * FROM [SOURCEPROJECT].IllustrationPointResultEntity;
INSERT INTO MacroStabilityInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity;
INSERT INTO MacroStabilityInwardsCalculationOutputEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity;
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
INSERT INTO MacroStabilityOutwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityOutwardsFailureMechanismMetaEntity;
INSERT INTO MacroStabilityOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityOutwardsSectionResultEntity;
INSERT INTO MicrostabilitySectionResultEntity SELECT * FROM [SOURCEPROJECT].MicrostabilitySectionResultEntity;
INSERT INTO PipingCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].PipingCharacteristicPointEntity;
INSERT INTO PipingFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingSectionResultEntity;
INSERT INTO PipingSoilLayerEntity SELECT * FROM [SOURCEPROJECT].PipingSoilLayerEntity;
INSERT INTO PipingSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingSoilProfileEntity;
INSERT INTO PipingStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingStochasticSoilProfileEntity;
INSERT INTO PipingStructureFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingStructureFailureMechanismMetaEntity;
INSERT INTO PipingStructureSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingStructureSectionResultEntity;
INSERT INTO ProbabilisticPipingCalculationEntity SELECT * FROM [SOURCEPROJECT].ProbabilisticPipingCalculationEntity;
INSERT INTO ProbabilisticPipingCalculationOutputEntity SELECT * FROM [SOURCEPROJECT].ProbabilisticPipingCalculationOutputEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO SemiProbabilisticPipingCalculationEntity SELECT * FROM [SOURCEPROJECT].SemiProbabilisticPipingCalculationEntity;
INSERT INTO SemiProbabilisticPipingCalculationOutputEntity SELECT * FROM [SOURCEPROJECT].SemiProbabilisticPipingCalculationOutputEntity;
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
SELECT
    [VersionId],
    "21.2",
    [Timestamp],
    [FingerPrint]
FROM [SOURCEPROJECT].VersionEntity;
INSERT INTO WaterPressureAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverFailureMechanismMetaEntity;
INSERT INTO WaveImpactAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity;

/*
 Map all calculation groups to failure mechanism ids.
 */
CREATE TEMP TABLE RootCalculationGroupFailureMechanismMapping
(
    'CalculationGroupEntityId' INTEGER NOT NULL PRIMARY KEY,
    'FailureMechanismEntityId' INTEGER NOT NULL
);

INSERT INTO RootCalculationGroupFailureMechanismMapping
SELECT OriginalGroupId, FailureMechanismEntityId
FROM
(
    WITH CalculationGroups AS (
        SELECT
            CalculationGroupEntityId,
            ParentCalculationGroupEntityId AS OriginalParentId,
            ParentCalculationGroupEntityId AS NextParentId,
            NULL as RootId,
            CASE
                WHEN ParentCalculationGroupEntityId IS NULL
                    THEN 1
                END AS IsRoot
        FROM CalculationGroupEntity
        UNION ALL
        SELECT
            CalculationGroups.CalculationGroupEntityId,
            CalculationGroups.OriginalParentId,
            entity.ParentCalculationGroupEntityId,
            CASE
                WHEN entity.ParentCalculationGroupEntityId IS NULL
                    THEN CalculationGroups.NextParentId
                ELSE
                    CalculationGroups.RootId
                END,
            NULL
        FROM CalculationGroups
                 INNER JOIN CalculationGroupEntity entity
                            ON CalculationGroups.NextParentId = entity.CalculationGroupEntityId)
    SELECT
        CalculationGroupEntityId as OriginalGroupId,
        CASE
            WHEN IsRoot = 1
                THEN CalculationGroupEntityId
            ELSE RootId
        END AS RootGroupId
    FROM CalculationGroups
    WHERE RootId IS NOT NULL OR IsRoot = 1)
JOIN FailureMechanismEntity ON FailureMechanismEntity.CalculationGroupEntityId = RootGroupId;

INSERT INTO GrassCoverErosionInwardsCalculationEntity(
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
    [DikeHeight],
    [UseBreakWater],
    [BreakWaterType],
    [BreakWaterHeight],
    [ShouldOvertoppingOutputIllustrationPointsBeCalculated],
    [ShouldDikeHeightBeCalculated],
    [DikeHeightTargetProbability],
    [ShouldDikeHeightIllustrationPointsBeCalculated],
    [ShouldOvertoppingRateBeCalculated],
    [OvertoppingRateTargetProbability],
    [ShouldOvertoppingRateIllustrationPointsBeCalculated],
    [RelevantForScenario],
    [ScenarioContribution])
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
    [DikeHeight],
    [UseBreakWater],
    [BreakWaterType],
    [BreakWaterHeight],
    [ShouldOvertoppingOutputIllustrationPointsBeCalculated],
    CASE
        WHEN [DikeHeightCalculationType] IS 1
            THEN 0
        ELSE 1
    END,
    Norm,
    [ShouldDikeHeightIllustrationPointsBeCalculated],
    CASE
        WHEN [OvertoppingRateCalculationType] IS 1
            THEN 0
        ELSE 1
    END,
    Norm,
    [ShouldOvertoppingRateIllustrationPointsBeCalculated],
    [RelevantForScenario],
    [ScenarioContribution]
FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity
JOIN RootCalculationGroupFailureMechanismMapping USING(CalculationGroupEntityId)
JOIN (
    SELECT FailureMechanismEntityId, AssessmentSectionEntityId
    FROM FailureMechanismEntity)
USING(FailureMechanismEntityId)
JOIN (
    SELECT
        AssessmentSectionEntityId AS sectionId,
        CASE
            WHEN NormativeNormType IS 1
                THEN LowerLimitNorm
            ELSE SignalingNorm
        END AS Norm
    FROM AssessmentSectionEntity)
ON sectionId = AssessmentSectionEntityId;

/* 
Write migration logging
*/
ATTACH DATABASE "{1}" AS LOGDATABASE;

CREATE TABLE IF NOT EXISTS [LOGDATABASE].'MigrationLogEntity'
(
    'MigrationLogEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    'FromVersion' VARCHAR(20) NOT NULL,
    'ToVersion' VARCHAR(20) NOT NULL,
    'LogMessage' TEXT NOT NULL);

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
    [AssessmentSectionId],
    [AssessmentSectionName],
    [FailureMechanismId],
    [FailureMechanismName],
    [msg]
);

INSERT INTO TempChanges
SELECT
    asfm.[AssessmentSectionId],
    asfm.[AssessmentSectionName],
    asfm.[FailureMechanismId],
    asfm.[FailureMechanismName],
    "De waarden van de doelkans voor HBN en overslagdebiet zijn veranderd naar de trajectnorm."
FROM GrassCoverErosionInwardsCalculationEntity
JOIN RootCalculationGroupFailureMechanismMapping USING(CalculationGroupEntityId)
JOIN (
    SELECT FailureMechanismEntityId, AssessmentSectionEntityId
    FROM FailureMechanismEntity)
USING(FailureMechanismEntityId)
JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = FailureMechanismEntityId;

CREATE TEMP TABLE TempAssessmentSectionChanges
(
    [AssessmentSectionId],
    [AssessmentSectionName],
    [Order],
    [msg]
);

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
VALUES (
    "21.1",
    "21.2",
    "Gevolgen van de migratie van versie 21.1 naar versie 21.2:");

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
    SELECT
        [FailureMechanismId],
        NULL,
        [AssessmentSectionId],
        [AssessmentSectionName],
        [msg],
        2
    FROM TempChanges
    WHERE TempChanges.[FailureMechanismId] IS [FailureMechanismId]
    ORDER BY [FailureMechanismId], [AssessmentSectionId]),
    AssessmentSectionFailureMechanismMessages
    (
        [AssessmentSectionId],
        [AssessmentSectionName],
        [IsAssessmentSectionHeader],
        [FailureMechanismId],
        [FailureMechanismName],
        [msg],
        [level],
        [Order]
    ) AS (
    SELECT DISTINCT
        [AssessmentSectionId],
        [AssessmentSectionName],
        1,
        NULL,
        NULL,
        NULL,
        1,
        0
    FROM
    (
        SELECT
            [AssessmentSectionId],
            [AssessmentSectionName]
        FROM TempAssessmentSectionChanges
        UNION
        SELECT
            [AssessmentSectionId],
            [AssessmentSectionName]
        FROM FailureMechanismMessages
        WHERE [AssessmentSectionId] IS NOT NULL)
    UNION
    SELECT * FROM
    (
        SELECT
            [AssessmentSectionId],
            [AssessmentSectionName],
            0 AS [IsAssessmentSectionHeader],
            NULL AS [FailureMechanismId],
            NULL,
            [msg],
            1 AS [level],
            [Order]
        FROM TempAssessmentSectionChanges
        UNION
        SELECT
            [AssessmentSectionId],
            NULL,
            0 AS [IsAssessmentSectionHeader],
            fmm.[FailureMechanismId] AS [FailureMechanismId],
            fmm.[FailureMechanismName],
            [msg],
            fmm.[level] AS [level],
            1 AS [Order]
        FROM FailureMechanismMessages AS fmm
        WHERE fmm.[AssessmentSectionId] IS [AssessmentSectionId])
    ORDER BY [AssessmentSectionId], [FailureMechanismId], [level], [IsAssessmentSectionHeader] DESC, [Order])
SELECT
    "21.1",
    "21.2",
    CASE
        WHEN [AssessmentSectionName] IS NOT NULL
            THEN 
                CASE WHEN [IsAssessmentSectionHeader] IS 1
                    THEN "* Traject: '" || [AssessmentSectionName] || "'"
                ELSE
                    " + " || [msg]
                END
        ELSE
            CASE
                WHEN [FailureMechanismName] IS NOT NULL
                    THEN "  + Toetsspoor: '" || [FailureMechanismName] || "'"
                ELSE
                    "    - " || [msg]
            END
    END
FROM AssessmentSectionFailureMechanismMessages;

DROP TABLE RootCalculationGroupFailureMechanismMapping;
DROP TABLE TempFailureMechanisms;
DROP TABLE TempAssessmentSectionFailureMechanism;
DROP TABLE TempAssessmentSectionChanges;
DROP TABLE TempChanges;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
SELECT "21.1",
       "21.2",
       "* Geen aanpassingen."
    WHERE (
        SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
        WHERE [FromVersion] = "21.1"
    ) IS 1;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;
