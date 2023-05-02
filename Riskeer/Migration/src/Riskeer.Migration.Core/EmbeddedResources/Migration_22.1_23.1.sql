/*
Migration script for migrating Riskeer databases.
SourceProject version: 22.1
TargetProject version: 23.1
*/

PRAGMA foreign_keys = OFF;

ATTACH DATABASE "{0}" AS SOURCEPROJECT;

INSERT INTO AssessmentSectionEntity SELECT * FROM [SOURCEPROJECT].AssessmentSectionEntity;
INSERT INTO FailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismSectionEntity;
INSERT INTO FailureMechanismEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismEntity;
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
INSERT INTO ClosingStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresOutputEntity;
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
    [Orientation],
    [D50],
    [Order]
)
SELECT
    [DuneLocationEntityId],
    [HydraulicLocationEntityId],
    [FailureMechanismEntityId],
    [Name],
    [CoastalAreaId],
    [Offset],
    [Orientation],
    [D50],
    [Order]
FROM [SOURCEPROJECT].DuneLocationEntity
JOIN (
    SELECT HydraulicLocationEntityId, LocationId
    FROM [SOURCEPROJECT].HydraulicLocationEntity
)
USING(LocationId);
INSERT INTO FailureMechanismFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismFailureMechanismSectionEntity;
INSERT INTO FaultTreeIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].FaultTreeIllustrationPointEntity;
INSERT INTO FaultTreeIllustrationPointStochastEntity SELECT * FROM [SOURCEPROJECT].FaultTreeIllustrationPointStochastEntity;
INSERT INTO FaultTreeSubmechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].FaultTreeSubmechanismIllustrationPointEntity;
INSERT INTO ForeshoreProfileEntity SELECT * FROM [SOURCEPROJECT].ForeshoreProfileEntity;
INSERT INTO GeneralResultFaultTreeIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].GeneralResultFaultTreeIllustrationPointEntity;
INSERT INTO GeneralResultFaultTreeIllustrationPointStochastEntity SELECT * FROM [SOURCEPROJECT].GeneralResultFaultTreeIllustrationPointStochastEntity;
INSERT INTO GeneralResultSubMechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].GeneralResultSubMechanismIllustrationPointEntity;
INSERT INTO GeneralResultSubMechanismIllustrationPointStochastEntity SELECT * FROM [SOURCEPROJECT].GeneralResultSubMechanismIllustrationPointStochastEntity;
INSERT INTO GrassCoverErosionInwardsDikeHeightOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsDikeHeightOutputEntity;
INSERT INTO GrassCoverErosionInwardsOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity;
INSERT INTO GrassCoverErosionInwardsOvertoppingRateOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsOvertoppingRateOutputEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity;
INSERT INTO GrassCoverSlipOffOutwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsFailureMechanismMetaEntity;
INSERT INTO HeightStructureEntity SELECT * FROM [SOURCEPROJECT].HeightStructureEntity;
INSERT INTO HeightStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
INSERT INTO HeightStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity;
INSERT INTO HeightStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresOutputEntity;
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
INSERT INTO IllustrationPointResultEntity SELECT * FROM [SOURCEPROJECT].IllustrationPointResultEntity;
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
INSERT INTO ProbabilisticPipingCalculationOutputEntity SELECT * FROM [SOURCEPROJECT].ProbabilisticPipingCalculationOutputEntity;
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
INSERT INTO SpecificFailureMechanismEntity SELECT * FROM [SOURCEPROJECT].SpecificFailureMechanismEntity;
INSERT INTO SpecificFailureMechanismFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].SpecificFailureMechanismFailureMechanismSectionEntity;
INSERT INTO StabilityPointStructureEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityPointStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity;
INSERT INTO StabilityStoneCoverWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsOutputEntity;
INSERT INTO StochastEntity SELECT * FROM [SOURCEPROJECT].StochastEntity;
INSERT INTO SubMechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].SubMechanismIllustrationPointEntity;
INSERT INTO SubMechanismIllustrationPointStochastEntity SELECT * FROM [SOURCEPROJECT].SubMechanismIllustrationPointStochastEntity;
INSERT INTO TopLevelFaultTreeIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].TopLevelFaultTreeIllustrationPointEntity;
INSERT INTO TopLevelSubMechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].TopLevelSubMechanismIllustrationPointEntity;
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
INSERT INTO WaveImpactAsphaltCoverWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity;

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
SELECT COUNT()
FROM [SOURCEPROJECT].SemiProbabilisticPipingCalculationOutputEntity
JOIN [SOURCEPROJECT].SemiProbabilisticPipingCalculationEntity USING(SemiProbabilisticPipingCalculationEntityId)
WHERE UseAssessmentLevelManualInput = 1;
INSERT INTO TempLogOutputRemaining
SELECT COUNT()
FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity
JOIN [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId)
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
			THEN "* Alle berekende resultaten zijn verwijderd, behalve die van het faalmechanisme 'Piping' en 'Macrostabiliteit Binnenwaards' waarbij de waterstand handmatig is ingevuld."
        ELSE "* Alle berekende resultaten zijn verwijderd."
        END
FROM TempLogOutputDeleted
         LEFT JOIN TempLogOutputRemaining
WHERE [NrDeleted] > 0
    LIMIT 1;

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