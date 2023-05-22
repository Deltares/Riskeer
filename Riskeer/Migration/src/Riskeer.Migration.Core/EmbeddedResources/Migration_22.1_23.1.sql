/*
Migration script for migrating Riskeer databases.
SourceProject version: 22.1
TargetProject version: 23.1
*/

PRAGMA foreign_keys = OFF;

ATTACH DATABASE "{0}" AS SOURCEPROJECT;

INSERT INTO AssessmentSectionEntity SELECT * FROM [SOURCEPROJECT].AssessmentSectionEntity;
INSERT INTO FailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismSectionEntity;
INSERT INTO FailureMechanismEntity(
    [FailureMechanismEntityId],
    [AssessmentSectionEntityId],
    [CalculationGroupEntityId],
    [FailureMechanismType],
    [InAssembly],
    [FailureMechanismSectionCollectionSourcePath],
    [InAssemblyInputComments],
    [InAssemblyOutputComments],
    [NotInAssemblyComments],
    [CalculationsInputComments],
    [FailureMechanismAssemblyResultProbabilityResultType],
    [FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability])
SELECT
    [FailureMechanismEntityId],
    [AssessmentSectionEntityId],
    [CalculationGroupEntityId],
    [FailureMechanismType],
    [InAssembly],
    [FailureMechanismSectionCollectionSourcePath],
    [InAssemblyInputComments],
    [InAssemblyOutputComments],
    [NotInAssemblyComments],
    [CalculationsInputComments],
    CASE
        WHEN [FailureMechanismAssemblyResultProbabilityResultType] = 2
            THEN 4
        ELSE
            [FailureMechanismAssemblyResultProbabilityResultType]
    END,
    [FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability]
FROM [SOURCEPROJECT].FailureMechanismEntity;
INSERT INTO ClosingStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresFailureMechanismMetaEntity;
INSERT INTO CalculationGroupEntity SELECT * FROM [SOURCEPROJECT].CalculationGroupEntity;
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO SemiProbabilisticPipingCalculationEntity SELECT * FROM [SOURCEPROJECT].SemiProbabilisticPipingCalculationEntity;
INSERT INTO GrassCoverErosionInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity;
INSERT INTO GrassCoverSlipOffInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity;
INSERT INTO PipingSoilLayerEntity SELECT * FROM [SOURCEPROJECT].PipingSoilLayerEntity;
INSERT INTO PipingSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingSoilProfileEntity;
INSERT INTO PipingStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingStochasticSoilProfileEntity;
INSERT INTO PipingScenarioConfigurationPerFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].PipingScenarioConfigurationPerFailureMechanismSectionEntity;
INSERT INTO StochasticSoilModelEntity SELECT * FROM [SOURCEPROJECT].StochasticSoilModelEntity;
INSERT INTO SurfaceLineEntity SELECT * FROM [SOURCEPROJECT].SurfaceLineEntity;
INSERT INTO PipingCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].PipingCharacteristicPointEntity;
INSERT INTO WaterPressureAsphaltCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].WaterPressureAsphaltCoverFailureMechanismMetaEntity;
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverFailureMechanismMetaEntity;
INSERT INTO AdoptableFailureMechanismSectionResultEntity SELECT * FROM [SOURCEPROJECT].AdoptableFailureMechanismSectionResultEntity;
INSERT INTO AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity SELECT * FROM [SOURCEPROJECT].AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity;
INSERT INTO BackgroundDataEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataEntity;
INSERT INTO BackgroundDataMetaEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataMetaEntity;
INSERT INTO ClosingStructureEntity SELECT * FROM [SOURCEPROJECT].ClosingStructureEntity;
INSERT INTO ClosingStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity;
INSERT INTO DikeProfileEntity SELECT * FROM [SOURCEPROJECT].DikeProfileEntity;
INSERT INTO DuneErosionFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity;
INSERT INTO DuneLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationEntity;
INSERT INTO DuneLocationCalculationForTargetProbabilityCollectionEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationForTargetProbabilityCollectionEntity;
INSERT INTO DuneLocationEntity (
    [DuneLocationEntityId],
    [HydraulicLocationEntityId],
    [FailureMechanismEntityId],
    [Name],
    [CoastalAreaId],
    [Offset],
    [Order]
)
SELECT
    [DuneLocationEntityId],
    [HydraulicLocationEntityId],
    [FailureMechanismEntityId],
    [Name],
    [CoastalAreaId],
    [Offset],
    [Order]
FROM [SOURCEPROJECT].DuneLocationEntity
JOIN (
    SELECT HydraulicLocationEntityId, LocationId
    FROM [SOURCEPROJECT].HydraulicLocationEntity
)
USING(LocationId);
INSERT INTO FailureMechanismFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismFailureMechanismSectionEntity;
INSERT INTO ForeshoreProfileEntity SELECT * FROM [SOURCEPROJECT].ForeshoreProfileEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
INSERT INTO GrassCoverSlipOffOutwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsFailureMechanismMetaEntity;
INSERT INTO HeightStructureEntity SELECT * FROM [SOURCEPROJECT].HeightStructureEntity;
INSERT INTO HeightStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
INSERT INTO HeightStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity;
INSERT INTO HydraulicBoundaryDataEntity (
    [HydraulicBoundaryDataEntityId],
    [AssessmentSectionEntityId],
    [HydraulicLocationConfigurationDatabaseFilePath],
    [HydraulicLocationConfigurationDatabaseScenarioName],
    [HydraulicLocationConfigurationDatabaseYear],
    [HydraulicLocationConfigurationDatabaseScope],
    [HydraulicLocationConfigurationDatabaseSeaLevel],
    [HydraulicLocationConfigurationDatabaseRiverDischarge],
    [HydraulicLocationConfigurationDatabaseLakeLevel],
    [HydraulicLocationConfigurationDatabaseWindDirection],
    [HydraulicLocationConfigurationDatabaseWindSpeed],
    [HydraulicLocationConfigurationDatabaseComment]
)
SELECT
    [HydraulicBoundaryDatabaseEntity],
    [AssessmentSectionEntityId],
    [HydraulicLocationConfigurationSettingsFilePath],
    [HydraulicLocationConfigurationSettingsScenarioName],
    [HydraulicLocationConfigurationSettingsYear],
    [HydraulicLocationConfigurationSettingsScope],
    [HydraulicLocationConfigurationSettingsSeaLevel],
    [HydraulicLocationConfigurationSettingsRiverDischarge],
    [HydraulicLocationConfigurationSettingsLakeLevel],
    [HydraulicLocationConfigurationSettingsWindDirection],
    [HydraulicLocationConfigurationSettingsWindSpeed],
    [HydraulicLocationConfigurationSettingsComment]
FROM [SOURCEPROJECT].HydraulicBoundaryDatabaseEntity;
INSERT INTO HydraulicBoundaryDatabaseEntity (
    [HydraulicBoundaryDataEntityId],
    [Version],
    [FilePath],
    [UsePreprocessorClosure],
    [Order]
)
SELECT
    [HydraulicBoundaryDatabaseEntity],
    [Version],
    [FilePath],
    [HydraulicLocationConfigurationSettingsUsePreprocessorClosure],
    0
FROM [SOURCEPROJECT].HydraulicBoundaryDatabaseEntity;
INSERT INTO HydraulicLocationEntity (
    [HydraulicLocationEntityId],
    [HydraulicBoundaryDatabaseEntityId],
    [LocationId],
    [Name],
    [LocationX],
    [LocationY],
    [Order]
)
SELECT
    [HydraulicLocationEntityId],
    [HydraulicBoundaryDatabaseEntityId],
    [LocationId],
    [Name],
    [LocationX],
    [LocationY],
    [Order]
FROM [SOURCEPROJECT].HydraulicLocationEntity
JOIN (
    SELECT HydraulicBoundaryDatabaseEntityId FROM HydraulicBoundaryDatabaseEntity LIMIT 1
);
INSERT INTO HydraulicLocationCalculationCollectionEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationCollectionEntity;
INSERT INTO HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity;
INSERT INTO HydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationEntity;
INSERT INTO HydraulicLocationCalculationForTargetProbabilityCollectionEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationForTargetProbabilityCollectionEntity;
INSERT INTO HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity;
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
INSERT INTO PipingFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingStructureFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingStructureFailureMechanismMetaEntity;
INSERT INTO ProbabilisticPipingCalculationEntity SELECT * FROM [SOURCEPROJECT].ProbabilisticPipingCalculationEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
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
INSERT INTO SpecificFailureMechanismEntity(
    [SpecificFailureMechanismEntityId],
    [AssessmentSectionEntityId],
    [Name],
    [Code],
    [Order],
    [InAssembly],
    [FailureMechanismSectionCollectionSourcePath],
    [InAssemblyInputComments],
    [InAssemblyOutputComments],
    [NotInAssemblyComments],
    [N],
    [FailureMechanismAssemblyResultProbabilityResultType],
    [FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability],
    [ApplyLengthEffectInSection])
SELECT
    [SpecificFailureMechanismEntityId],
    [AssessmentSectionEntityId],
    [Name],
    [Code],
    [Order],
    [InAssembly],
    [FailureMechanismSectionCollectionSourcePath],
    [InAssemblyInputComments],
    [InAssemblyOutputComments],
    [NotInAssemblyComments],
    [N],
    CASE
        WHEN [FailureMechanismAssemblyResultProbabilityResultType] = 2 
            THEN 4
        ELSE
            [FailureMechanismAssemblyResultProbabilityResultType]
    END,
    [FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability],
    [ApplyLengthEffectInSection]
FROM [SOURCEPROJECT].SpecificFailureMechanismEntity;
INSERT INTO SpecificFailureMechanismFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].SpecificFailureMechanismFailureMechanismSectionEntity;
INSERT INTO StabilityPointStructureEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity;
INSERT INTO StochastEntity SELECT * FROM [SOURCEPROJECT].StochastEntity;
INSERT INTO VersionEntity (
    [VersionId],
    [Version],
    [Timestamp],
    [FingerPrint])
SELECT
    [VersionId],
    "23.1",
    [Timestamp],
    [FingerPrint]
FROM [SOURCEPROJECT].VersionEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity;

/*
Outputs that used HydraRing are not migrated
*/
-- ClosingStructuresOutputEntity
-- DuneLocationOutputEntity
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
    'LogMessage' TEXT NOT NULL
    );

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
VALUES ("22.1", "23.1", "Gevolgen van de migratie van versie 22.1 naar versie 23.1:");

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
    "22.1",
    "23.1",
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
INSERT INTO TempFailureMechanisms VALUES (13, 'Macrostabiliteit buitenwaarts');
INSERT INTO TempFailureMechanisms VALUES (14, 'Microstabiliteit');
INSERT INTO TempFailureMechanisms VALUES (15, 'Wateroverdruk bij asfaltbekleding');
INSERT INTO TempFailureMechanisms VALUES (16, 'Grasbekleding afschuiven binnentalud');
INSERT INTO TempFailureMechanisms VALUES (17, 'Sterkte en stabiliteit langsconstructies');
INSERT INTO TempFailureMechanisms VALUES (18, 'Technische innovaties');

CREATE TEMP TABLE TempChanges (
    [FailureMechanismId],
    [FailureMechanismName],
    [msg]
);

INSERT INTO TempChanges
SELECT fme.[FailureMechanismEntityId],
    tfm.[FailureMechanismName],
    "Alle resultaten van dit faalmechanisme die op Automatisch stonden zijn op <selecteer> gezet."
FROM FailureMechanismEntity AS fme
    JOIN TempFailureMechanisms AS tfm USING(FailureMechanismType)
WHERE fme.[FailureMechanismAssemblyResultProbabilityResultType] = 1;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
        [FromVersion],
        [ToVersion],
        [LogMessage]
    ) WITH RECURSIVE FailureMechanismMessages (
        [FailureMechanismId],
        [FailureMechanismName],
        [msg],
        [level]
    ) AS (
        SELECT DISTINCT [FailureMechanismId],
            [FailureMechanismName],
            NULL,
            1
        FROM TempChanges
        UNION
        SELECT [FailureMechanismId],
            NULL,
            [msg],
            2
        FROM TempChanges
        WHERE TempChanges.[FailureMechanismId] IS [FailureMechanismId]
        ORDER BY 1,
            3
    )
SELECT "22.1",
    "23.1",
    CASE
        WHEN [FailureMechanismName] IS NOT NULL THEN "* Faalmechanisme: '" || [FailureMechanismName] || "'"
        ELSE "* + " || [msg]
    END
END
FROM FailureMechanismMessages;

CREATE TEMP TABLE SpecificTempChanges (
    [FailureMechanismId],
    [FailureMechanismName],
    [msg]
);

INSERT INTO SpecificTempChanges
SELECT sfme.[SpecificFailureMechanismEntityId],
    sfme.[Name],
    "Alle resultaten van dit faalmechanisme die op Automatisch stonden zijn op <selecteer> gezet."
FROM SpecificFailureMechanismEntity AS sfme
WHERE sfme.[FailureMechanismAssemblyResultProbabilityResultType] = 1;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
        [FromVersion],
        [ToVersion],
        [LogMessage]
    ) WITH RECURSIVE FailureMechanismMessages (
        [FailureMechanismId],
        [FailureMechanismName],
        [msg],
        [level]
    ) AS (
        SELECT DISTINCT [FailureMechanismId],
            [FailureMechanismName],
            NULL,
            1
        FROM SpecificTempChanges
        UNION
        SELECT [FailureMechanismId],
            NULL,
            [msg],
            2
        FROM SpecificTempChanges
        WHERE SpecificTempChanges.[FailureMechanismId] IS [FailureMechanismId]
        ORDER BY 1,
            3
    )
SELECT "22.1",
    "23.1",
    CASE
        WHEN [FailureMechanismName] IS NOT NULL THEN "* Faalmechanisme: '" || [FailureMechanismName] || "'"
        ELSE "* + " || [msg]
    END
END
FROM FailureMechanismMessages;

DROP TABLE TempLogOutputDeleted;
DROP TABLE TempLogOutputRemaining;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
SELECT "22.1",
       "23.1",
       "* Geen aanpassingen."
    WHERE (
		SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
		WHERE [FromVersion] = "22.1"
	) IS 1;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;