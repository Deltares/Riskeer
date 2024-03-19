/*
Migration script for migrating Riskeer databases.
SourceProject version: 23.1
TargetProject version: 23.2
*/
PRAGMA foreign_keys = OFF;
 
ATTACH DATABASE "{0}" AS SOURCEPROJECT;
 
INSERT INTO AdoptableFailureMechanismSectionResultEntity SELECT * FROM [SOURCEPROJECT].AdoptableFailureMechanismSectionResultEntity;
INSERT INTO AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity SELECT * FROM [SOURCEPROJECT].AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity;
INSERT INTO AssessmentSectionEntity(
    [AssessmentSectionEntityId],
    [ProjectEntityId],
    [HydraulicLocationCalculationCollectionEntity1Id],
    [HydraulicLocationCalculationCollectionEntity2Id],
    [Id],
    [Name],
    [Comments],
    [MaximumAllowableFloodingProbability],
    [SignalFloodingProbability],
    [NormativeProbabilityType],
    [Composition],
    [ReferenceLinePointXml],
    [AreFailureMechanismsCorrelated]) 
SELECT
    [AssessmentSectionEntityId],
    [ProjectEntityId],
    [HydraulicLocationCalculationCollectionEntity1Id],
    [HydraulicLocationCalculationCollectionEntity2Id],
    [Id],
    [Name],
    [Comments],
    [MaximumAllowableFloodingProbability],
    [SignalFloodingProbability],
    [NormativeProbabilityType],
    [Composition],
    [ReferenceLinePointXml],
    0
FROM [SOURCEPROJECT].AssessmentSectionEntity;
INSERT INTO BackgroundDataEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataEntity;
INSERT INTO BackgroundDataMetaEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataMetaEntity;
INSERT INTO CalculationGroupEntity SELECT * FROM [SOURCEPROJECT].CalculationGroupEntity;
INSERT INTO ClosingStructureEntity SELECT * FROM [SOURCEPROJECT].ClosingStructureEntity;
INSERT INTO ClosingStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity;
INSERT INTO ClosingStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresFailureMechanismMetaEntity;
INSERT INTO DikeProfileEntity SELECT * FROM [SOURCEPROJECT].DikeProfileEntity;
INSERT INTO DuneErosionFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity;
INSERT INTO DuneLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationEntity;
INSERT INTO DuneLocationCalculationForTargetProbabilityCollectionEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationForTargetProbabilityCollectionEntity;
INSERT INTO DuneLocationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationEntity;
INSERT INTO FailureMechanismEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismEntity;
INSERT INTO FailureMechanismFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismFailureMechanismSectionEntity;
INSERT INTO FailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismSectionEntity;
INSERT INTO ForeshoreProfileEntity SELECT * FROM [SOURCEPROJECT].ForeshoreProfileEntity;
INSERT INTO GrassCoverErosionInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity;
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity (
    [GrassCoverErosionOutwardsWaveConditionsCalculationEntityId],
    [CalculationGroupEntityId],
    [ForeshoreProfileEntityId],
    [HydraulicLocationEntityId],
    [HydraulicLocationCalculationForTargetProbabilityCollectionEntityId],
    [Order],
    [Name],
    [Comments],
    [UseBreakWater],
    [BreakWaterType],
    [BreakWaterHeight],
    [UseForeshore],
    [Orientation],
    [UpperBoundaryRevetment],
    [LowerBoundaryRevetment],
    [UpperBoundaryWaterLevels],
    [LowerBoundaryWaterLevels],
    [StepSize],
    [CalculationType],
    [WaterLevelType]) 
SELECT
    [GrassCoverErosionOutwardsWaveConditionsCalculationEntityId],
    [CalculationGroupEntityId],
    [ForeshoreProfileEntityId],
    [HydraulicLocationEntityId],
    [HydraulicLocationCalculationForTargetProbabilityCollectionEntityId],
    [Order],
    [Name],
    [Comments],
    [UseBreakWater],
    [BreakWaterType],
    [BreakWaterHeight],
    [UseForeshore],
    [Orientation],
    [UpperBoundaryRevetment],
    [LowerBoundaryRevetment],
    [UpperBoundaryWaterLevels],
    [LowerBoundaryWaterLevels],
    CASE 
        WHEN [StepSize] = 1
            THEN 0.5
        WHEN [StepSize] = 2
            THEN 1
        WHEN [StepSize] = 3
            THEN 2
    END,
    [CalculationType],
    [WaterLevelType]
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
INSERT INTO GrassCoverSlipOffInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverSlipOffOutwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsFailureMechanismMetaEntity;
INSERT INTO HeightStructureEntity SELECT * FROM [SOURCEPROJECT].HeightStructureEntity;
INSERT INTO HeightStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
INSERT INTO HeightStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity;
INSERT INTO HydraulicBoundaryDataEntity SELECT * FROM [SOURCEPROJECT].HydraulicBoundaryDataEntity;
INSERT INTO HydraulicBoundaryDatabaseEntity SELECT * FROM [SOURCEPROJECT].HydraulicBoundaryDatabaseEntity;
INSERT INTO HydraulicLocationCalculationCollectionEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationCollectionEntity;
INSERT INTO HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity;
INSERT INTO HydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationEntity;
INSERT INTO HydraulicLocationCalculationForTargetProbabilityCollectionEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationForTargetProbabilityCollectionEntity;
INSERT INTO HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity;
INSERT INTO HydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationEntity;
INSERT INTO MacroStabilityInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity;
INSERT INTO MacroStabilityInwardsCalculationOutputEntity(
    [MacroStabilityInwardsCalculationOutputEntityId],
    [MacroStabilityInwardsCalculationEntityId],
    [FactorOfStability],
    [ForbiddenZonesXEntryMin],
    [ForbiddenZonesXEntryMax],
    [SlidingCurveLeftSlidingCircleCenterX],
    [SlidingCurveLeftSlidingCircleCenterY],
    [SlidingCurveLeftSlidingCircleRadius],
    [SlidingCurveLeftSlidingCircleIsActive],
    [SlidingCurveLeftSlidingCircleNonIteratedForce],
    [SlidingCurveLeftSlidingCircleIteratedForce],
    [SlidingCurveLeftSlidingCircleDrivingMoment],
    [SlidingCurveLeftSlidingCircleResistingMoment],
    [SlidingCurveRightSlidingCircleCenterX],
    [SlidingCurveRightSlidingCircleCenterY],
    [SlidingCurveRightSlidingCircleRadius],
    [SlidingCurveRightSlidingCircleIsActive],
    [SlidingCurveRightSlidingCircleNonIteratedForce],
    [SlidingCurveRightSlidingCircleIteratedForce],
    [SlidingCurveRightSlidingCircleDrivingMoment],
    [SlidingCurveRightSlidingCircleResistingMoment],
    [SlidingCurveNonIteratedHorizontalForce],
    [SlidingCurveIteratedHorizontalForce],
    [SlidingCurveSliceXML],
    [SlipPlaneLeftGridXLeft],
    [SlipPlaneLeftGridXRight],
    [SlipPlaneLeftGridNrOfHorizontalPoints],
    [SlipPlaneLeftGridZTop],
    [SlipPlaneLeftGridZBottom],
    [SlipPlaneLeftGridNrOfVerticalPoints],
    [SlipPlaneRightGridXLeft],
    [SlipPlaneRightGridXRight],
    [SlipPlaneRightGridNrOfHorizontalPoints],
    [SlipPlaneRightGridZTop],
    [SlipPlaneRightGridZBottom],
    [SlipPlaneRightGridNrOfVerticalPoints],
    [SlipPlaneTangentLinesXml])
SELECT
    [MacroStabilityInwardsCalculationOutputEntityId],
    [MacroStabilityInwardsCalculationEntityId],
    [FactorOfStability],
    [ForbiddenZonesXEntryMin],
    [ForbiddenZonesXEntryMax],
    [SlidingCurveLeftSlidingCircleCenterX],
    [SlidingCurveLeftSlidingCircleCenterY],
    [SlidingCurveLeftSlidingCircleRadius],
    [SlidingCurveLeftSlidingCircleIsActive],
    [SlidingCurveLeftSlidingCircleNonIteratedForce],
    [SlidingCurveLeftSlidingCircleIteratedForce],
    [SlidingCurveLeftSlidingCircleDrivingMoment],
    [SlidingCurveLeftSlidingCircleResistingMoment],
    [SlidingCurveRightSlidingCircleCenterX],
    [SlidingCurveRightSlidingCircleCenterY],
    [SlidingCurveRightSlidingCircleRadius],
    [SlidingCurveRightSlidingCircleIsActive],
    [SlidingCurveRightSlidingCircleNonIteratedForce],
    [SlidingCurveRightSlidingCircleIteratedForce],
    [SlidingCurveRightSlidingCircleDrivingMoment],
    [SlidingCurveRightSlidingCircleResistingMoment],
    [SlidingCurveNonIteratedHorizontalForce],
    [SlidingCurveIteratedHorizontalForce],
    [SlidingCurveSliceXML],
    [SlipPlaneLeftGridXLeft],
    [SlipPlaneLeftGridXRight],
    [SlipPlaneLeftGridNrOfHorizontalPoints],
    [SlipPlaneLeftGridZTop],
    [SlipPlaneLeftGridZBottom],
    [SlipPlaneLeftGridNrOfVerticalPoints],
    [SlipPlaneRightGridXLeft],
    [SlipPlaneRightGridXRight],
    [SlipPlaneRightGridNrOfHorizontalPoints],
    [SlipPlaneRightGridZTop],
    [SlipPlaneRightGridZBottom],
    [SlipPlaneRightGridNrOfVerticalPoints],
    [SlipPlaneTangentLinesXml]
FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity
JOIN [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId)
WHERE UseAssessmentLevelManualInput = 1;
INSERT INTO MacroStabilityInwardsCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCharacteristicPointEntity;
INSERT INTO MacroStabilityInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsFailureMechanismMetaEntity;
INSERT INTO MacroStabilityInwardsPreconsolidationStressEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsPreconsolidationStressEntity;
INSERT INTO MacroStabilityInwardsSoilLayerOneDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilLayerOneDEntity;
INSERT INTO MacroStabilityInwardsSoilLayerTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilLayerTwoDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileOneDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileOneDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileTwoDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity;
INSERT INTO MacroStabilityInwardsStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsStochasticSoilProfileEntity;
INSERT INTO MicrostabilityFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].MicrostabilityFailureMechanismMetaEntity;
INSERT INTO NonAdoptableFailureMechanismSectionResultEntity SELECT * FROM [SOURCEPROJECT].NonAdoptableFailureMechanismSectionResultEntity;
INSERT INTO NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity SELECT * FROM [SOURCEPROJECT].NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity;
INSERT INTO PipingCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].PipingCharacteristicPointEntity;
INSERT INTO PipingFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingScenarioConfigurationPerFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].PipingScenarioConfigurationPerFailureMechanismSectionEntity;
INSERT INTO PipingSoilLayerEntity SELECT * FROM [SOURCEPROJECT].PipingSoilLayerEntity;
INSERT INTO PipingSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingSoilProfileEntity;
INSERT INTO PipingStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingStochasticSoilProfileEntity;
INSERT INTO PipingStructureFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingStructureFailureMechanismMetaEntity;
INSERT INTO ProbabilisticPipingCalculationEntity SELECT * FROM [SOURCEPROJECT].ProbabilisticPipingCalculationEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO SemiProbabilisticPipingCalculationEntity SELECT * FROM [SOURCEPROJECT].SemiProbabilisticPipingCalculationEntity;
INSERT INTO SemiProbabilisticPipingCalculationOutputEntity(
    [SemiProbabilisticPipingCalculationOutputEntityId],
    [SemiProbabilisticPipingCalculationEntityId],
    [Order],
    [HeaveFactorOfSafety],
    [UpliftFactorOfSafety],
    [SellmeijerFactorOfSafety],
    [UpliftEffectiveStress],
    [HeaveGradient],
    [SellmeijerCreepCoefficient],
    [SellmeijerCriticalFall],
    [SellmeijerReducedFall])
SELECT
    [SemiProbabilisticPipingCalculationOutputEntityId],
    [SemiProbabilisticPipingCalculationEntityId],
    sppcoe.[Order],
    [HeaveFactorOfSafety],
    [UpliftFactorOfSafety],
    [SellmeijerFactorOfSafety],
    [UpliftEffectiveStress],
    [HeaveGradient],
    [SellmeijerCreepCoefficient],
    [SellmeijerCriticalFall],
    [SellmeijerReducedFall]
FROM [SOURCEPROJECT].SemiProbabilisticPipingCalculationOutputEntity sppcoe
JOIN [SOURCEPROJECT].SemiProbabilisticPipingCalculationEntity USING(SemiProbabilisticPipingCalculationEntityId)
WHERE UseAssessmentLevelManualInput = 1;
INSERT INTO SpecificFailureMechanismEntity SELECT * FROM [SOURCEPROJECT].SpecificFailureMechanismEntity;
INSERT INTO SpecificFailureMechanismFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].SpecificFailureMechanismFailureMechanismSectionEntity;
INSERT INTO StabilityPointStructureEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity(
    [StabilityStoneCoverWaveConditionsCalculationEntityId],
    [CalculationGroupEntityId],
    [ForeshoreProfileEntityId],
    [HydraulicLocationEntityId],
    [HydraulicLocationCalculationForTargetProbabilityCollectionEntityId],
    [Order],
    [Name],
    [Comments],
    [UseBreakWater],
    [BreakWaterType],
    [BreakWaterHeight],
    [UseForeshore],
    [Orientation],
    [UpperBoundaryRevetment],
    [LowerBoundaryRevetment],
    [UpperBoundaryWaterLevels],
    [LowerBoundaryWaterLevels],
    [StepSize],
    [CalculationType],
    [WaterLevelType])
SELECT
    [StabilityStoneCoverWaveConditionsCalculationEntityId],
    [CalculationGroupEntityId],
    [ForeshoreProfileEntityId],
    [HydraulicLocationEntityId],
    [HydraulicLocationCalculationForTargetProbabilityCollectionEntityId],
    [Order],
    [Name],
    [Comments],
    [UseBreakWater],
    [BreakWaterType],
    [BreakWaterHeight],
    [UseForeshore],
    [Orientation],
    [UpperBoundaryRevetment],
    [LowerBoundaryRevetment],
    [UpperBoundaryWaterLevels],
    [LowerBoundaryWaterLevels],
    CASE 
        WHEN [StepSize] = 1
            THEN 0.5
        WHEN [StepSize] = 2
            THEN 1
        WHEN [StepSize] = 3
            THEN 2
    END,
    [CalculationType],
    [WaterLevelType]
FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity;
INSERT INTO StochastEntity SELECT * FROM [SOURCEPROJECT].StochastEntity;
INSERT INTO StochasticSoilModelEntity SELECT * FROM [SOURCEPROJECT].StochasticSoilModelEntity;
INSERT INTO SurfaceLineEntity SELECT * FROM [SOURCEPROJECT].SurfaceLineEntity;
INSERT INTO WaterPressureAsphaltCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].WaterPressureAsphaltCoverFailureMechanismMetaEntity;
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverFailureMechanismMetaEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsCalculationEntity(
    [WaveImpactAsphaltCoverWaveConditionsCalculationEntityId],
    [CalculationGroupEntityId],
    [ForeshoreProfileEntityId],
    [HydraulicLocationEntityId],
    [HydraulicLocationCalculationForTargetProbabilityCollectionEntityId],
    [Order],
    [Name],
    [Comments],
    [UseBreakWater],
    [BreakWaterType],
    [BreakWaterHeight],
    [UseForeshore],
    [Orientation],
    [UpperBoundaryRevetment],
    [LowerBoundaryRevetment],
    [UpperBoundaryWaterLevels],
    [LowerBoundaryWaterLevels],
    [StepSize],
    [WaterLevelType])
SELECT
    [WaveImpactAsphaltCoverWaveConditionsCalculationEntityId],
    [CalculationGroupEntityId],
    [ForeshoreProfileEntityId],
    [HydraulicLocationEntityId],
    [HydraulicLocationCalculationForTargetProbabilityCollectionEntityId],
    [Order],
    [Name],
    [Comments],
    [UseBreakWater],
    [BreakWaterType],
    [BreakWaterHeight],
    [UseForeshore],
    [Orientation],
    [UpperBoundaryRevetment],
    [LowerBoundaryRevetment],
    [UpperBoundaryWaterLevels],
    [LowerBoundaryWaterLevels],
    CASE
        WHEN [StepSize] = 1
            THEN 0.5
        WHEN [StepSize] = 2
            THEN 1
        WHEN [StepSize] = 3
            THEN 2
    END,
    [WaterLevelType]
FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity;
 
INSERT INTO VersionEntity (
    [VersionId],
    [Version],
    [Timestamp],
    [FingerPrint])
SELECT [VersionId],
    "23.2",
    [Timestamp],
    [FingerPrint]
FROM [SOURCEPROJECT].VersionEntity;

/*
Outputs that used HydraRing are not migrated
*/
-- ClosingStructuresOutputEntity
-- DuneLocationCalculationOutputEntity
-- GrassCoverErosionInwardsDikeHeightOutputEntity
-- GrassCoverErosionInwardsOutputEntity
-- GrassCoverErosionInwardsOvertoppingRateOutputEntity
-- GrassCoverErosionOutwardsWaveConditionsOutputEntity
-- HeightStructuresOutputEntity
-- HydraulicLocationOutputEntity
-- SemiProbabilisticPipingCalculationOutputEntity where UseManualAssessmentLevel is 0
-- ProbabilisticPipingCalculationOutputEntity
-- StabilityPointStructuresOutputEntity
-- StabilityStoneCoverWaveConditionsOutputEntity
-- WaveImpactAsphaltCoverWaveConditionsOutputEntity
-- FaultTreeIllustrationPointEntity
-- FaultTreeIllustrationPointStochastEntity
-- FaultTreeSubMechanismIllustrationPointEntity
-- GeneralResultFaultTreeIllustrationPointEntity
-- GeneralResultFaultTreeIllustrationPointStochastEntity
-- GeneralResultSubMechanismIllustrationPointEntity
-- GeneralResultSubMechanismIllustrationPointStochastEntity
-- IllustrationPointResultEntity
-- SubMechanismIllustrationPointEntity
-- SubMechanismIllustrationPointStochastEntity
-- TopLevelFaultTreeIllustrationPointEntity
-- TopLevelSubMechanismIllustrationPointEntity
-- MacroStabilityInwardsCalculationOutputEntity where UseManualAssessmentLevel is 0

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

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
VALUES (
    "23.1",
    "23.2",
    "Gevolgen van de migratie van versie 23.1 naar versie 23.2:");

CREATE TEMP TABLE TempLogOutputDeleted
(
    'NrDeleted' INTEGER NOT NULL
);

INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].ClosingStructuresOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].DuneLocationCalculationOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsDikeHeightOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsOvertoppingRateOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].HeightStructuresOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].ProbabilisticPipingCalculationOutputEntity;
INSERT INTO TempLogOutputDeleted
SELECT COUNT()
FROM [SOURCEPROJECT].SemiProbabilisticPipingCalculationOutputEntity
JOIN [SOURCEPROJECT].SemiProbabilisticPipingCalculationEntity USING(SemiProbabilisticPipingCalculationEntityId)
WHERE UseAssessmentLevelManualInput = 0;
INSERT INTO TempLogOutputDeleted
SELECT COUNT()
FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity
JOIN [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId)
WHERE UseAssessmentLevelManualInput = 0;

CREATE TEMP TABLE TempLogOutputRemaining
(
    'NrRemaining' INTEGER NOT NULL
);

INSERT INTO TempLogOutputRemaining
SELECT COUNT() +
(
   SELECT COUNT()
   FROM MacroStabilityInwardsCalculationOutputEntity
    JOIN MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId)
   WHERE UseAssessmentLevelManualInput = 1
)
FROM SemiProbabilisticPipingCalculationOutputEntity
JOIN SemiProbabilisticPipingCalculationEntity USING(SemiProbabilisticPipingCalculationEntityId)
WHERE UseAssessmentLevelManualInput = 1;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
SELECT
    "23.1",
    "23.2",
    CASE
        WHEN [NrRemaining] > 0
            THEN "* Alle berekende resultaten zijn verwijderd, behalve die van het faalmechanisme 'Piping' en/of 'Macrostabiliteit binnenwaarts' waarbij de waterstand handmatig is ingevuld."
        ELSE "* Alle berekende resultaten zijn verwijderd."
        END
FROM TempLogOutputDeleted
LEFT JOIN TempLogOutputRemaining
WHERE [NrDeleted] > 0
LIMIT 1;

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
INSERT INTO TempFailureMechanisms VALUES (13, 'Microstabiliteit');
INSERT INTO TempFailureMechanisms VALUES (14, 'Wateroverdruk bij asfaltbekleding');
INSERT INTO TempFailureMechanisms VALUES (15, 'Grasbekleding afschuiven binnentalud');

CREATE TEMP TABLE TempAssessmentSectionFailureMechanism (
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

CREATE TEMP TABLE TempChanges (
    [AssessmentSectionId],
    [AssessmentSectionName],
    [FailureMechanismId],
    [FailureMechanismName],
    [msg]
);

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
[LogMessage]
) WITH RECURSIVE FailureMechanismMessages (
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
    ORDER BY [AssessmentSectionId], [FailureMechanismId]
    ),
    AssessmentSectionFailureMechanismMessages (
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
    FROM (
    SELECT
    [AssessmentSectionId],
    [AssessmentSectionName]
    FROM FailureMechanismMessages
    WHERE [AssessmentSectionId] IS NOT NULL
    )
    UNION
    SELECT *
    FROM (
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
    WHERE fmm.[AssessmentSectionId] IS [AssessmentSectionId]
    )
    ORDER BY
    [AssessmentSectionId],
    [FailureMechanismId],
    [level],
    [IsAssessmentSectionHeader] DESC,
    [Order]
    )
SELECT
    "23.1",
    "23.2",
    CASE
        WHEN [AssessmentSectionName] IS NOT NULL
    THEN 
                CASE
                    WHEN [IsAssessmentSectionHeader] IS 1
                        THEN 
                            "* Traject: '" || [AssessmentSectionName] || "'"
                        ELSE 
                            "  + " || [msg]
END
ELSE
                CASE
                    WHEN [FailureMechanismName] IS NOT NULL
                        THEN
                            "  + Faalmechanisme: '" || [FailureMechanismName] || "'"
                        ELSE
                            "    - " || [msg]
END
END
FROM AssessmentSectionFailureMechanismMessages;

DROP TABLE TempFailureMechanisms;
DROP TABLE TempAssessmentSectionFailureMechanism;
DROP TABLE TempChanges;

DROP TABLE TempLogOutputDeleted;
DROP TABLE TempLogOutputRemaining;
 
INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
SELECT "23.1",
       "23.2",
       "* Geen aanpassingen."
WHERE (
    SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
    WHERE [FromVersion] = "23.1"
) IS 1;
 
DETACH LOGDATABASE;
 
DETACH SOURCEPROJECT;
 
PRAGMA foreign_keys = ON;