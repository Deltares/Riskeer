/*
Migration script for migrating Riskeer databases.
SourceProject version: 19.1
TargetProject version: 20.1
*/

PRAGMA foreign_keys = OFF;

ATTACH DATABASE "{0}" AS SOURCEPROJECT;

INSERT INTO AssessmentSectionEntity SELECT * FROM [SOURCEPROJECT].AssessmentSectionEntity;
INSERT INTO BackgroundDataEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataEntity;
INSERT INTO BackgroundDataMetaEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataMetaEntity;
INSERT INTO CalculationGroupEntity SELECT * FROM [SOURCEPROJECT].CalculationGroupEntity;
INSERT INTO ClosingStructureEntity SELECT * FROM [SOURCEPROJECT].ClosingStructureEntity;
INSERT INTO ClosingStructuresCalculationEntity(
	[ClosingStructuresCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	[HydraulicLocationEntityId],
	[ClosingStructureEntityId],
	[Order],
	[Name],
	[Comments],
	[UseBreakWater], 
	[BreakWaterType], 
	[BreakWaterHeight],
	[UseForeshore], 
	[Orientation],
	[StructureNormalOrientation],
	[StorageStructureAreaMean],
	[StorageStructureAreaCoefficientOfVariation],
	[AllowedLevelIncreaseStorageMean],
	[AllowedLevelIncreaseStorageStandardDeviation],
	[WidthFlowAperturesMean],
	[WidthFlowAperturesStandardDeviation],
	[LevelCrestStructureNotClosingMean],
	[LevelCrestStructureNotClosingStandardDeviation],
	[InsideWaterLevelMean],
	[InsideWaterLevelStandardDeviation],
	[ThresholdHeightOpenWeirMean],
	[ThresholdHeightOpenWeirStandardDeviation],
	[AreaFlowAperturesMean],
	[AreaFlowAperturesStandardDeviation],
	[CriticalOvertoppingDischargeMean],
	[CriticalOvertoppingDischargeCoefficientOfVariation],
	[FlowWidthAtBottomProtectionMean],
	[FlowWidthAtBottomProtectionStandardDeviation],
	[ProbabilityOpenStructureBeforeFlooding],
	[FailureProbabilityOpenStructure],
	[IdenticalApertures],
	[FailureProbabilityReparation],
	[InflowModelType], 
	[FailureProbabilityStructureWithErosion],
	[DeviationWaveDirection],
	[DrainCoefficientMean],
    [DrainCoefficientStandardDeviation],
	[ModelFactorSuperCriticalFlowMean],
	[StormDurationMean],
	[FactorStormDurationOpenStructure],
	[ShouldIllustrationPointsBeCalculated],
	[RelevantForScenario],
	[ScenarioContribution])
SELECT 
	[ClosingStructuresCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	[HydraulicLocationEntityId],
	[ClosingStructureEntityId],
	[Order],
	[Name],
	[Comments],
	[UseBreakWater], 
	[BreakWaterType], 
	[BreakWaterHeight],
	[UseForeshore], 
	[Orientation],
	[StructureNormalOrientation],
	[StorageStructureAreaMean],
	[StorageStructureAreaCoefficientOfVariation],
	[AllowedLevelIncreaseStorageMean],
	[AllowedLevelIncreaseStorageStandardDeviation],
	[WidthFlowAperturesMean],
	[WidthFlowAperturesStandardDeviation],
	[LevelCrestStructureNotClosingMean],
	[LevelCrestStructureNotClosingStandardDeviation],
	[InsideWaterLevelMean],
	[InsideWaterLevelStandardDeviation],
	[ThresholdHeightOpenWeirMean],
	[ThresholdHeightOpenWeirStandardDeviation],
	[AreaFlowAperturesMean],
	[AreaFlowAperturesStandardDeviation],
	[CriticalOvertoppingDischargeMean],
	[CriticalOvertoppingDischargeCoefficientOfVariation],
	[FlowWidthAtBottomProtectionMean],
	[FlowWidthAtBottomProtectionStandardDeviation],
	[ProbabilityOpenStructureBeforeFlooding],
	[FailureProbabilityOpenStructure],
	[IdenticalApertures],
	[FailureProbabilityReparation],
	[InflowModelType], 
	[FailureProbabilityStructureWithErosion],
	[DeviationWaveDirection],
	[DrainCoefficientMean],
	0.2,   
	[ModelFactorSuperCriticalFlowMean],
	[StormDurationMean],
	[FactorStormDurationOpenStructure],
	[ShouldIllustrationPointsBeCalculated],
	CASE
		WHEN IsLinkedToSectionResult IS NOT NULL
			THEN 1
		ELSE 0
	END,
	CASE
		WHEN IsLinkedToSectionResult IS NOT NULL
			THEN 1
		ELSE 0
	END
FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity
LEFT JOIN(
	SELECT 
		ClosingStructuresCalculationEntityId,
		CASE
			WHEN ClosingStructuresSectionResultEntityId IS NOT NULL
				THEN 1
			ELSE 0
		END AS IsLinkedToSectionResult
	FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity)
USING(ClosingStructuresCalculationEntityId);
INSERT INTO ClosingStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresFailureMechanismMetaEntity;
INSERT INTO ClosingStructuresSectionResultEntity (
	[ClosingStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssembly],
	[ManualAssemblyProbability])
SELECT 
	[ClosingStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssembly],
	[ManualAssemblyProbability]
FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity;
INSERT INTO DikeProfileEntity SELECT * FROM [SOURCEPROJECT].DikeProfileEntity;
INSERT INTO DuneErosionFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity;
INSERT INTO DuneErosionSectionResultEntity SELECT * FROM [SOURCEPROJECT].DuneErosionSectionResultEntity;
INSERT INTO DuneLocationCalculationCollectionEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationCollectionEntity;
INSERT INTO DuneLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationEntity;
INSERT INTO DuneLocationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationEntity;
INSERT INTO FailureMechanismEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismEntity;
INSERT INTO FailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismSectionEntity;
INSERT INTO ForeshoreProfileEntity SELECT * FROM [SOURCEPROJECT].ForeshoreProfileEntity;
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
	[DikeHeightCalculationType],
	[DikeHeight],
	[UseBreakWater],
	[BreakWaterType],
	[BreakWaterHeight],
	[OvertoppingRateCalculationType],
	[ShouldDikeHeightIllustrationPointsBeCalculated],
	[ShouldOvertoppingRateIllustrationPointsBeCalculated],
	[ShouldOvertoppingOutputIllustrationPointsBeCalculated],
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
	[DikeHeightCalculationType],
	[DikeHeight],
	[UseBreakWater],
	[BreakWaterType],
	[BreakWaterHeight],
	[OvertoppingRateCalculationType],
	[ShouldDikeHeightIllustrationPointsBeCalculated],
	[ShouldOvertoppingRateIllustrationPointsBeCalculated],
	[ShouldOvertoppingOutputIllustrationPointsBeCalculated],
	CASE
		WHEN IsLinkedToSectionResult IS NOT NULL
			THEN 1
		ELSE 0
	END,
	CASE
		WHEN IsLinkedToSectionResult IS NOT NULL
			THEN 1
		ELSE 0
	END
FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity
LEFT JOIN(
	SELECT 
		GrassCoverErosionInwardsCalculationEntityId,
		CASE
			WHEN GrassCoverErosionInwardsSectionResultEntityId IS NOT NULL
				THEN 1
			ELSE 0
		END AS IsLinkedToSectionResult
	FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity)
USING(GrassCoverErosionInwardsCalculationEntityId);
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionInwardsSectionResultEntity (
	[GrassCoverErosionInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssembly],
	[ManualAssemblyProbability])
SELECT 
	[GrassCoverErosionInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssembly],
	[ManualAssemblyProbability]
FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
INSERT INTO GrassCoverSlipOffInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity;
INSERT INTO GrassCoverSlipOffOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity;
INSERT INTO HeightStructureEntity SELECT * FROM [SOURCEPROJECT].HeightStructureEntity;
INSERT INTO HeightStructuresCalculationEntity (
	[HeightStructuresCalculationEntityId],
	[CalculationGroupEntityId],
	[HydraulicLocationEntityId],
	[HeightStructureEntityId],
	[ForeshoreProfileEntityId],
	[Order],
	[Name],
	[Comments],
	[ModelFactorSuperCriticalFlowMean],
	[StructureNormalOrientation],
	[AllowedLevelIncreaseStorageMean],
	[AllowedLevelIncreaseStorageStandardDeviation],
	[StorageStructureAreaMean],
	[StorageStructureAreaCoefficientOfVariation],
	[FlowWidthAtBottomProtectionMean],
	[FlowWidthAtBottomProtectionStandardDeviation],
	[CriticalOvertoppingDischargeMean],
	[CriticalOvertoppingDischargeCoefficientOfVariation],
	[FailureProbabilityStructureWithErosion],
	[WidthFlowAperturesMean],
	[WidthFlowAperturesStandardDeviation],
	[StormDurationMean],
	[LevelCrestStructureMean],
	[LevelCrestStructureStandardDeviation],
	[DeviationWaveDirection],
	[UseBreakWater], 
	[UseForeshore],
	[BreakWaterType],
	[BreakWaterHeight],
	[ShouldIllustrationPointsBeCalculated],
	[RelevantForScenario],
	[ScenarioContribution]) 
SELECT 
	[HeightStructuresCalculationEntityId],
	[CalculationGroupEntityId],
	[HydraulicLocationEntityId],
	[HeightStructureEntityId],
	[ForeshoreProfileEntityId],
	[Order],
	[Name],
	[Comments],
	[ModelFactorSuperCriticalFlowMean],
	[StructureNormalOrientation],
	[AllowedLevelIncreaseStorageMean],
	[AllowedLevelIncreaseStorageStandardDeviation],
	[StorageStructureAreaMean],
	[StorageStructureAreaCoefficientOfVariation],
	[FlowWidthAtBottomProtectionMean],
	[FlowWidthAtBottomProtectionStandardDeviation],
	[CriticalOvertoppingDischargeMean],
	[CriticalOvertoppingDischargeCoefficientOfVariation],
	[FailureProbabilityStructureWithErosion],
	[WidthFlowAperturesMean],
	[WidthFlowAperturesStandardDeviation],
	[StormDurationMean],
	[LevelCrestStructureMean],
	[LevelCrestStructureStandardDeviation],
	[DeviationWaveDirection],
	[UseBreakWater], 
	[UseForeshore],
	[BreakWaterType],
	[BreakWaterHeight],
	[ShouldIllustrationPointsBeCalculated],
	CASE
		WHEN IsLinkedToSectionResult IS NOT NULL
			THEN 1
		ELSE 0
	END,
	CASE
		WHEN IsLinkedToSectionResult IS NOT NULL
			THEN 1
		ELSE 0
	END
FROM [SOURCEPROJECT].HeightStructuresCalculationEntity
LEFT JOIN(
	SELECT 
		HeightStructuresCalculationEntityId,
		CASE
			WHEN HeightStructuresSectionResultEntityId IS NOT NULL
				THEN 1
			ELSE 0
		END AS IsLinkedToSectionResult
	FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity)
USING(HeightStructuresCalculationEntityId);
INSERT INTO HeightStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity;
INSERT INTO HeightStructuresSectionResultEntity (
	[HeightStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssembly],
	[ManualAssemblyProbability])
SELECT 
	[HeightStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssembly],
	[ManualAssemblyProbability]
FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity;
INSERT INTO HydraulicBoundaryDatabaseEntity SELECT * FROM [SOURCEPROJECT].HydraulicBoundaryDatabaseEntity;
INSERT INTO HydraulicLocationCalculationCollectionEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationCollectionEntity;
INSERT INTO HydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationEntity;
INSERT INTO HydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationEntity;
INSERT INTO MacroStabilityInwardsCalculationEntity (
    [MacroStabilityInwardsCalculationEntityId],
    [CalculationGroupEntityId],
    [SurfaceLineEntityId],
    [MacroStabilityInwardsStochasticSoilProfileEntityId],
    [HydraulicLocationEntityId],
    [Order],
    [Name],
    [Comment],
    [RelevantForScenario],
    [ScenarioContribution],
    [AssessmentLevel],
    [UseAssessmentLevelManualInput],
    [SlipPlaneMinimumDepth],
    [SlipPlaneMinimumLength],
    [MaximumSliceWidth],
    [MoveGrid],
    [GridDeterminationType],
    [TangentLineDeterminationType],
    [TangentLineZTop],
    [TangentLineZBottom],
    [TangentLineNumber],
    [LeftGridXLeft],
    [LeftGridXRight],
    [LeftGridNrOfHorizontalPoints],
    [LeftGridZTop],
    [LeftGridZBottom],
    [LeftGridNrOfVerticalPoints],
    [RightGridXLeft],
    [RightGridXRight],
    [RightGridNrOfHorizontalPoints],
    [RightGridZTop],
    [RightGridZBottom],
    [RightGridNrOfVerticalPoints],
    [DikeSoilScenario],
    [WaterLevelRiverAverage],
    [DrainageConstructionPresent],
    [DrainageConstructionCoordinateX],
    [DrainageConstructionCoordinateZ],
    [MinimumLevelPhreaticLineAtDikeTopRiver],
    [MinimumLevelPhreaticLineAtDikeTopPolder],
    [AdjustPhreaticLine3And4ForUplift],
    [LeakageLengthOutwardsPhreaticLine3],
    [LeakageLengthInwardsPhreaticLine3],
    [LeakageLengthOutwardsPhreaticLine4],
    [LeakageLengthInwardsPhreaticLine4],
    [PiezometricHeadPhreaticLine2Outwards],
    [PiezometricHeadPhreaticLine2Inwards],
    [LocationInputExtremeWaterLevelPolder],
    [LocationInputExtremeUseDefaultOffsets],
    [LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver],
    [LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder],
    [LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside],
    [LocationInputExtremePhreaticLineOffsetDikeToeAtPolder],
    [LocationInputExtremePenetrationLength],
    [LocationInputDailyWaterLevelPolder],
    [LocationInputDailyUseDefaultOffsets],
    [LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver],
    [LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder],
    [LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside],
    [LocationInputDailyPhreaticLineOffsetDikeToeAtPolder],
    [CreateZones],
    [ZoningBoundariesDeterminationType],
    [ZoneBoundaryLeft],
    [ZoneBoundaryRight])
SELECT
    [MacroStabilityInwardsCalculationEntityId],
    [CalculationGroupEntityId],
    [SurfaceLineEntityId],
    [MacroStabilityInwardsStochasticSoilProfileEntityId],
    [HydraulicLocationEntityId],
    [Order],
    [Name],
    [Comment],
    [RelevantForScenario],
    CASE
        WHEN [ScenarioContribution] IS NULL
        OR [ScenarioContribution] < 0
            THEN 0
        WHEN [ScenarioContribution] > 1
            THEN 1
        ELSE
            [ScenarioContribution]
    END,
    [AssessmentLevel],
    [UseAssessmentLevelManualInput],
    [SlipPlaneMinimumDepth],
    [SlipPlaneMinimumLength],
    [MaximumSliceWidth],
    [MoveGrid],
    [GridDeterminationType],
    [TangentLineDeterminationType],
    [TangentLineZTop],
    [TangentLineZBottom],
    [TangentLineNumber],
    [LeftGridXLeft],
    [LeftGridXRight],
    [LeftGridNrOfHorizontalPoints],
    [LeftGridZTop],
    [LeftGridZBottom],
    [LeftGridNrOfVerticalPoints],
    [RightGridXLeft],
    [RightGridXRight],
    [RightGridNrOfHorizontalPoints],
    [RightGridZTop],
    [RightGridZBottom],
    [RightGridNrOfVerticalPoints],
    [DikeSoilScenario],
    [WaterLevelRiverAverage],
    [DrainageConstructionPresent],
    [DrainageConstructionCoordinateX],
    [DrainageConstructionCoordinateZ],
    [MinimumLevelPhreaticLineAtDikeTopRiver],
    [MinimumLevelPhreaticLineAtDikeTopPolder],
    [AdjustPhreaticLine3And4ForUplift],
    [LeakageLengthOutwardsPhreaticLine3],
    [LeakageLengthInwardsPhreaticLine3],
    [LeakageLengthOutwardsPhreaticLine4],
    [LeakageLengthInwardsPhreaticLine4],
    [PiezometricHeadPhreaticLine2Outwards],
    [PiezometricHeadPhreaticLine2Inwards],
    [LocationInputExtremeWaterLevelPolder],
    [LocationInputExtremeUseDefaultOffsets],
    [LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver],
    [LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder],
    [LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside],
    [LocationInputExtremePhreaticLineOffsetDikeToeAtPolder],
    [LocationInputExtremePenetrationLength],
    [LocationInputDailyWaterLevelPolder],
    [LocationInputDailyUseDefaultOffsets],
    [LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver],
    [LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder],
    [LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside],
    [LocationInputDailyPhreaticLineOffsetDikeToeAtPolder],
    [CreateZones],
    [ZoningBoundariesDeterminationType],
    [ZoneBoundaryLeft],
    [ZoneBoundaryRight]  
FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity;
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
INSERT INTO SemiProbabilisticPipingCalculationEntity(
    [SemiProbabilisticPipingCalculationEntityId],
    [CalculationGroupEntityId],
    [SurfaceLineEntityId],
    [PipingStochasticSoilProfileEntityId],
    [HydraulicLocationEntityId],
    [Order],
    [Name],
    [Comments],
    [EntryPointL],
    [ExitPointL],
    [PhreaticLevelExitMean],
    [PhreaticLevelExitStandardDeviation],
    [DampingFactorExitMean],
    [DampingFactorExitStandardDeviation],
    [RelevantForScenario],
    [ScenarioContribution],
    [AssessmentLevel],
    [UseAssessmentLevelManualInput])
SELECT
    [PipingCalculationEntityId],
    [CalculationGroupEntityId],
    [SurfaceLineEntityId],
    [PipingStochasticSoilProfileEntityId],
    [HydraulicLocationEntityId],
    [Order],
    [Name],
    [Comments],
    [EntryPointL],
    [ExitPointL],
    [PhreaticLevelExitMean],
    [PhreaticLevelExitStandardDeviation],
    [DampingFactorExitMean],
    [DampingFactorExitStandardDeviation],
    [RelevantForScenario],
    CASE
        WHEN [ScenarioContribution] IS NULL
        OR [ScenarioContribution] < 0
            THEN 0
        WHEN [ScenarioContribution] > 1
            THEN 1
    ELSE
        [ScenarioContribution]
    END,
    [AssessmentLevel],
    [UseAssessmentLevelManualInput]
FROM [SOURCEPROJECT].PipingCalculationEntity;
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
	[PipingCalculationOutputEntityId],
	[PipingCalculationEntityId],
	[Order],
	[HeaveFactorOfSafety],
	[UpliftFactorOfSafety],
	[SellmeijerFactorOfSafety],
	[UpliftEffectiveStress],
	[HeaveGradient],
	[SellmeijerCreepCoefficient],
	[SellmeijerCriticalFall],
	[SellmeijerReducedFall]
FROM [SOURCEPROJECT].PipingCalculationOutputEntity
WHERE PipingCalculationEntityId IN (
    SELECT PipingCalculationEntityId
    FROM [SOURCEPROJECT].PipingCalculationEntity
    WHERE UseAssessmentLevelManualInput IS 1
    );
INSERT INTO PipingCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].PipingCharacteristicPointEntity;
INSERT INTO PipingFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingSectionResultEntity;
INSERT INTO PipingSoilLayerEntity SELECT * FROM [SOURCEPROJECT].PipingSoilLayerEntity;
INSERT INTO PipingSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingSoilProfileEntity;
INSERT INTO PipingStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingStochasticSoilProfileEntity;
INSERT INTO PipingStructureFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingStructureFailureMechanismMetaEntity;
INSERT INTO PipingStructureSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingStructureSectionResultEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO StabilityPointStructureEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity(
	[StabilityPointStructuresCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	[HydraulicLocationEntityId],
	[StabilityPointStructureEntityId],
	[Order],
	[Name],
	[Comments],
	[UseBreakWater], 
	[BreakWaterType], 
	[BreakWaterHeight],
	[UseForeshore], 
	[StructureNormalOrientation],
	[StorageStructureAreaMean],
	[StorageStructureAreaCoefficientOfVariation],
	[AllowedLevelIncreaseStorageMean],
	[AllowedLevelIncreaseStorageStandardDeviation],
	[WidthFlowAperturesMean],
	[WidthFlowAperturesStandardDeviation],
	[InsideWaterLevelMean],
	[InsideWaterLevelStandardDeviation],
	[ThresholdHeightOpenWeirMean],
	[ThresholdHeightOpenWeirStandardDeviation],
	[CriticalOvertoppingDischargeMean],
	[CriticalOvertoppingDischargeCoefficientOfVariation],
	[FlowWidthAtBottomProtectionMean],
	[FlowWidthAtBottomProtectionStandardDeviation],
	[ConstructiveStrengthLinearLoadModelMean],
	[ConstructiveStrengthLinearLoadModelCoefficientOfVariation],
	[ConstructiveStrengthQuadraticLoadModelMean],
	[ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation],
	[BankWidthMean],
	[BankWidthStandardDeviation],
	[InsideWaterLevelFailureConstructionMean],
	[InsideWaterLevelFailureConstructionStandardDeviation],
	[EvaluationLevel],
	[LevelCrestStructureMean],
	[LevelCrestStructureStandardDeviation],
	[VerticalDistance],
	[FailureProbabilityRepairClosure],
	[FailureCollisionEnergyMean],
	[FailureCollisionEnergyCoefficientOfVariation],
	[ShipMassMean],
	[ShipMassCoefficientOfVariation],
	[ShipVelocityMean],
	[ShipVelocityCoefficientOfVariation],
	[LevellingCount],
	[ProbabilityCollisionSecondaryStructure],
	[FlowVelocityStructureClosableMean],
	[StabilityLinearLoadModelMean],
	[StabilityLinearLoadModelCoefficientOfVariation],
	[StabilityQuadraticLoadModelMean],
	[StabilityQuadraticLoadModelCoefficientOfVariation],
	[AreaFlowAperturesMean],
	[AreaFlowAperturesStandardDeviation],
	[InflowModelType], 
	[LoadSchematizationType], 
	[VolumicWeightWater],
	[StormDurationMean],
	[FactorStormDurationOpenStructure],
	[DrainCoefficientMean],
	[DrainCoefficientStandardDeviation],
	[FailureProbabilityStructureWithErosion],
	[ShouldIllustrationPointsBeCalculated],
	[RelevantForScenario],
	[ScenarioContribution])
SELECT 
	[StabilityPointStructuresCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	[HydraulicLocationEntityId],
	[StabilityPointStructureEntityId],
	[Order],
	[Name],
	[Comments],
	[UseBreakWater], 
	[BreakWaterType], 
	[BreakWaterHeight],
	[UseForeshore], 
	[StructureNormalOrientation],
	[StorageStructureAreaMean],
	[StorageStructureAreaCoefficientOfVariation],
	[AllowedLevelIncreaseStorageMean],
	[AllowedLevelIncreaseStorageStandardDeviation],
	[WidthFlowAperturesMean],
	[WidthFlowAperturesStandardDeviation],
	[InsideWaterLevelMean],
	[InsideWaterLevelStandardDeviation],
	[ThresholdHeightOpenWeirMean],
	[ThresholdHeightOpenWeirStandardDeviation],
	[CriticalOvertoppingDischargeMean],
	[CriticalOvertoppingDischargeCoefficientOfVariation],
	[FlowWidthAtBottomProtectionMean],
	[FlowWidthAtBottomProtectionStandardDeviation],
	[ConstructiveStrengthLinearLoadModelMean],
	[ConstructiveStrengthLinearLoadModelCoefficientOfVariation],
	[ConstructiveStrengthQuadraticLoadModelMean],
	[ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation],
	[BankWidthMean],
	[BankWidthStandardDeviation],
	[InsideWaterLevelFailureConstructionMean],
	[InsideWaterLevelFailureConstructionStandardDeviation],
	[EvaluationLevel],
	[LevelCrestStructureMean],
	[LevelCrestStructureStandardDeviation],
	[VerticalDistance],
	[FailureProbabilityRepairClosure],
	[FailureCollisionEnergyMean],
	[FailureCollisionEnergyCoefficientOfVariation],
	[ShipMassMean],
	[ShipMassCoefficientOfVariation],
	[ShipVelocityMean],
	[ShipVelocityCoefficientOfVariation],
	[LevellingCount],
	[ProbabilityCollisionSecondaryStructure],
	[FlowVelocityStructureClosableMean],
	[StabilityLinearLoadModelMean],
	[StabilityLinearLoadModelCoefficientOfVariation],
	[StabilityQuadraticLoadModelMean],
	[StabilityQuadraticLoadModelCoefficientOfVariation],
	[AreaFlowAperturesMean],
	[AreaFlowAperturesStandardDeviation],
	[InflowModelType], 
	[LoadSchematizationType], 
	[VolumicWeightWater],
	[StormDurationMean],
	[FactorStormDurationOpenStructure],
	[DrainCoefficientMean],
	0.2,   
	[FailureProbabilityStructureWithErosion],
	[ShouldIllustrationPointsBeCalculated],
	CASE
		WHEN IsLinkedToSectionResult IS NOT NULL
			THEN 1
		ELSE 0
	END,
	CASE
		WHEN IsLinkedToSectionResult IS NOT NULL
			THEN 1
		ELSE 0
	END
FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity
LEFT JOIN(
	SELECT 
		StabilityPointStructuresCalculationEntityId,
		CASE
			WHEN StabilityPointStructuresSectionResultEntityId IS NOT NULL
				THEN 1
			ELSE 0
		END AS IsLinkedToSectionResult
	FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity)
USING(StabilityPointStructuresCalculationEntityId);
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityPointStructuresSectionResultEntity (
	[StabilityPointStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssembly],
	[ManualAssemblyProbability])
SELECT 
	[StabilityPointStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssembly],
	[ManualAssemblyProbability]
FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity;
INSERT INTO StochastEntity SELECT * FROM [SOURCEPROJECT].StochastEntity;
INSERT INTO StochasticSoilModelEntity SELECT * FROM [SOURCEPROJECT].StochasticSoilModelEntity;
INSERT INTO StrengthStabilityLengthwiseConstructionSectionResultEntity SELECT * FROM [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity;
INSERT INTO SurfaceLineEntity SELECT * FROM [SOURCEPROJECT].SurfaceLineEntity;
INSERT INTO TechnicalInnovationSectionResultEntity SELECT * FROM [SOURCEPROJECT].TechnicalInnovationSectionResultEntity;
INSERT INTO VersionEntity (
	[VersionId],
	[Version],
	[Timestamp],
	[FingerPrint])
SELECT [VersionId],
	"20.1",
	[Timestamp],
	[FingerPrint]
FROM [SOURCEPROJECT].VersionEntity;
INSERT INTO WaterPressureAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverFailureMechanismMetaEntity;
INSERT INTO WaveImpactAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity;
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
-- PipingCalculationOutputEntity where UseManualAssessmentLevel is 0
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

/*
 Outputs that used MacroStabilityInwards kernel are not migrated
 */
 -- MacroStabilityInwardsCalculationOutputEntity

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
VALUES ("19.1", "20.1", "Gevolgen van de migratie van versie 19.1 naar versie 20.1:");

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
INSERT INTO TempLogOutputDeleted
SELECT COUNT()
FROM [SOURCEPROJECT].PipingCalculationOutputEntity
WHERE PipingCalculationEntityId IN (
    SELECT PipingCalculationEntityId
    FROM [SOURCEPROJECT].PipingCalculationEntity
    WHERE UseAssessmentLevelManualInput IS 0
    );

CREATE TEMP TABLE TempLogOutputRemaining
(
	'NrRemaining' INTEGER NOT NULL
);
INSERT INTO TempLogOutputRemaining
SELECT COUNT()
FROM [SOURCEPROJECT].PipingCalculationOutputEntity
WHERE PipingCalculationEntityId IN (
    SELECT PipingCalculationEntityId
    FROM [SOURCEPROJECT].PipingCalculationEntity
    WHERE UseAssessmentLevelManualInput IS 1
    );

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
SELECT
    "19.1",
    "20.1",
    CASE
        WHEN [NrRemaining] > 0
			THEN "* Alle berekende resultaten zijn verwijderd, behalve die van het toetsspoor 'Piping' waarbij de waterstand handmatig is ingevuld."
        ELSE "* Alle berekende resultaten zijn verwijderd."
        END
FROM TempLogOutputDeleted
         LEFT JOIN TempLogOutputRemaining
WHERE [NrDeleted] > 0
    LIMIT 1;

DROP TABLE TempLogOutputDeleted;
DROP TABLE TempLogOutputRemaining;

CREATE TEMP TABLE TempPipingValuesAdjusted 
(
	'NrAdjusted' INTEGER NOT NULL
);

INSERT INTO TempPipingValuesAdjusted SELECT COUNT() FROM [SOURCEPROJECT].PipingCalculationEntity
WHERE (
    [ScenarioContribution] > 1.0
    OR [ScenarioContribution] < 0.0
    OR [ScenarioContribution] IS NULL
    );

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
SELECT
    "19.1",
    "20.1",
    "* Alle scenario bijdragen van van het toetsspoor 'Piping' waarbij de bijdrage groter is dan 100% of kleiner dan 0% zijn aangepast naar respectievelijk 100% en 0%."
FROM TempPipingValuesAdjusted
WHERE [NrAdjusted] > 0
    LIMIT 1;

DROP TABLE TempPipingValuesAdjusted;

CREATE TEMP TABLE TempMacroStabilityInwardsValuesAdjusted 
(
	'NrAdjusted' INTEGER NOT NULL
);

INSERT INTO TempMacroStabilityInwardsValuesAdjusted SELECT COUNT() FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity
WHERE (
    [ScenarioContribution] > 1.0
   OR [ScenarioContribution] < 0.0
   OR [ScenarioContribution] IS NULL
    );

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
[LogMessage])
SELECT
    "19.1",
    "20.1",
    "* Alle scenario bijdragen van het toetsspoor 'Macrostabiliteit Binnenwaarts' waarbij de bijdrage groter is dan 100% of kleiner dan 0% zijn aangepast naar respectievelijk 100% en 0%."
FROM TempMacroStabilityInwardsValuesAdjusted
WHERE [NrAdjusted] > 0
    LIMIT 1;

DROP TABLE TempMacroStabilityInwardsValuesAdjusted;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion],
	[ToVersion],
	[LogMessage])
SELECT 
    "19.1",
	"20.1", 
	"* Geen aanpassingen."
	WHERE (
		SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
		WHERE [FromVersion] = "19.1"
	) IS 1;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;