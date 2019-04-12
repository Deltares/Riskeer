 /*
Migration script for migrating Ringtoets database to Riskeer database.
SourceProject version: 18.1
TargetProject version: 19.1
*/
PRAGMA foreign_keys = OFF;

ATTACH DATABASE "{0}" AS SOURCEPROJECT;

INSERT INTO AssessmentSectionEntity (
	[AssessmentSectionEntityId],
	[ProjectEntityId],
	[HydraulicLocationCalculationCollectionEntity1Id],
	[HydraulicLocationCalculationCollectionEntity2Id],
	[HydraulicLocationCalculationCollectionEntity3Id],
	[HydraulicLocationCalculationCollectionEntity4Id],
	[HydraulicLocationCalculationCollectionEntity5Id],
	[HydraulicLocationCalculationCollectionEntity6Id],
	[HydraulicLocationCalculationCollectionEntity7Id],
	[HydraulicLocationCalculationCollectionEntity8Id],
	[Id],
	[Name],
	[Comments],
	[LowerLimitNorm],
	[SignalingNorm],
	[NormativeNormType],
	[Composition],
	[ReferenceLinePointXml],
	[Order])
SELECT 
	[AssessmentSectionEntityId],
	[ProjectEntityId],
	[HydraulicLocationCalculationCollectionEntity1Id],
	[HydraulicLocationCalculationCollectionEntity2Id],
	[HydraulicLocationCalculationCollectionEntity3Id],
	[HydraulicLocationCalculationCollectionEntity4Id],
	[HydraulicLocationCalculationCollectionEntity5Id],
	[HydraulicLocationCalculationCollectionEntity6Id],
	[HydraulicLocationCalculationCollectionEntity7Id],
	[HydraulicLocationCalculationCollectionEntity8Id],
	[Id],
	[Name],
	[Comments],
	[LowerLimitNorm],
	[SignalingNorm],
	[NormativeNormType],
	[Composition],
	[ReferenceLinePointXml],
	[Order]
FROM SOURCEPROJECT.AssessmentSectionEntity;
INSERT INTO BackgroundDataEntity (
	[BackgroundDataEntityId],
	[AssessmentSectionEntityId],
	[Name],
	[IsVisible],
	[Transparency],
	[BackgroundDataType])
SELECT 
	[BackgroundDataEntityId],
	[AssessmentSectionEntityId],
	[Name],
	[IsVisible],
	CASE 
		WHEN [Transparency] > 0 
			THEN [Transparency]
	ELSE 
		0.6
	END,
	[BackgroundDataType]
FROM [SOURCEPROJECT].BackgroundDataEntity;
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
	[ModelFactorSuperCriticalFlowMean],
	[StormDurationMean],
	[FactorStormDurationOpenStructure],
	[ShouldIllustrationPointsBeCalculated])
SELECT 
	[ClosingStructuresCalculationEntityId],
	[CalculationGroupEntityId],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[ForeshoreProfileEntityId]
	END,
	[HydraulicLocationEntityId],
	[ClosingStructureEntityId],
	[Order],
	[Name],
	[Comments],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseBreakWater]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 3
		ELSE 
			[BreakWaterType]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[BreakWaterHeight]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseForeshore]
	END,
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
	[ModelFactorSuperCriticalFlowMean],
	[StormDurationMean],
	[FactorStormDurationOpenStructure],
	[ShouldIllustrationPointsBeCalculated]
FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity
LEFT JOIN (
	SELECT 
		ForeshoreProfileEntityId,
		CASE 
			WHEN (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / (LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) = 1
				THEN 0
			ELSE 1
		END AS ValidForeshoreProfile
	FROM [SOURCEPROJECT].ForeshoreProfileEntity
) USING(ForeshoreProfileEntityId);
INSERT INTO ClosingStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresFailureMechanismMetaEntity;
INSERT INTO ClosingStructuresOutputEntity(
	[ClosingStructuresOutputEntityId],
	[ClosingStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability])
SELECT 
	[ClosingStructuresOutputEntityId],
	[ClosingStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability]
FROM [SOURCEPROJECT].ClosingStructuresOutputEntity
JOIN [SOURCEPROJECT].ClosingStructuresCalculationEntity USING(ClosingStructuresCalculationEntityId)
WHERE ForeshoreProfileEntityId IS NULL 
UNION
SELECT 
	[ClosingStructuresOutputEntityId],
	[ClosingStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability]
FROM [SOURCEPROJECT].ClosingStructuresOutputEntity
JOIN [SOURCEPROJECT].ClosingStructuresCalculationEntity USING(ClosingStructuresCalculationEntityId)
JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(ForeshoreProfileEntityId)
WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / 
(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) != 1;
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
INSERT INTO ForeshoreProfileEntity(
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
	[Id],
	[Name],
	[Orientation],
	[BreakWaterType], 
	[BreakWaterHeight],
	[GeometryXml],
	[X],
	[Y],
	[X0],
	[Order]
FROM [SOURCEPROJECT].ForeshoreProfileEntity
WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / (LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) != 1;
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
INSERT INTO GrassCoverErosionOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity (
	[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	[HydraulicLocationEntityId],
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
	[CategoryType],
	[CalculationType])
SELECT 
	[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId],
	[CalculationGroupEntityId],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[ForeshoreProfileEntityId]
	END,
	[HydraulicLocationEntityId],
	[Order],
	[Name],
	[Comments],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseBreakWater]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 3
		ELSE 
			[BreakWaterType]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[BreakWaterHeight]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseForeshore]
	END,
	[Orientation],
	[UpperBoundaryRevetment],
	[LowerBoundaryRevetment],
	[UpperBoundaryWaterLevels],
	[LowerBoundaryWaterLevels],
	[StepSize],
	[CategoryType],
	CASE
		WHEN [HasOutput] = 1 
			THEN 2
		ELSE 3
	END
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity
JOIN (
	SELECT 
	    [GrassCoverErosionOutwardsWaveConditionsCalculationEntityId],
	    CASE 
			WHEN GrassCoverErosionOutwardsWaveConditionsOutputEntityId IS NOT NULL
				THEN 1
			ELSE 0 
	    END AS HasOutput
	FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity
	LEFT JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId)
	GROUP BY GrassCoverErosionOutwardsWaveConditionsCalculationEntityId
) USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId)
LEFT JOIN (
	SELECT 
		ForeshoreProfileEntityId,
		CASE 
			WHEN (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / (LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) = 1
				THEN 0
			ELSE 1
		END AS ValidForeshoreProfile
	FROM [SOURCEPROJECT].ForeshoreProfileEntity
) USING(ForeshoreProfileEntityId);
INSERT INTO GrassCoverErosionOutwardsWaveConditionsOutputEntity(
	[GrassCoverErosionOutwardsWaveConditionsOutputEntityId],
	[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId],
	[Order],
	[OutputType],
	[WaterLevel],
	[WaveHeight],
	[WavePeakPeriod],
	[WaveAngle],
	[WaveDirection],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence])
SELECT 
	[GrassCoverErosionOutwardsWaveConditionsOutputEntityId],
	[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId],
	output.[Order],
	2,
	[WaterLevel],
	[WaveHeight],
	[WavePeakPeriod],
	[WaveAngle],
	[WaveDirection],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity output
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId)
WHERE ForeshoreProfileEntityId IS NULL 
UNION
SELECT 
	[GrassCoverErosionOutwardsWaveConditionsOutputEntityId],
	[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId],
	output.[Order],
	2,
	[WaterLevel],
	[WaveHeight],
	[WavePeakPeriod],
	[WaveAngle],
	[WaveDirection],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity output
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity USING(GrassCoverErosionOutwardsWaveConditionsCalculationEntityId)
JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(ForeshoreProfileEntityId)
WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / 
(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) != 1;
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
	[ShouldIllustrationPointsBeCalculated]) 
SELECT 
	[HeightStructuresCalculationEntityId],
	[CalculationGroupEntityId],
	[HydraulicLocationEntityId],
	[HeightStructureEntityId],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[ForeshoreProfileEntityId]
	END,
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
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseBreakWater]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseForeshore]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 3
		ELSE 
			[BreakWaterType]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[BreakWaterHeight]
	END,
	[ShouldIllustrationPointsBeCalculated]
FROM [SOURCEPROJECT].HeightStructuresCalculationEntity
LEFT JOIN (
	SELECT 
		ForeshoreProfileEntityId,
		CASE 
			WHEN (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / (LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) = 1
				THEN 0
			ELSE 1
		END AS ValidForeshoreProfile
	FROM [SOURCEPROJECT].ForeshoreProfileEntity
) USING(ForeshoreProfileEntityId);
INSERT INTO HeightStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity;
INSERT INTO HeightStructuresOutputEntity(
	[HeightStructuresOutputEntityId],
	[HeightStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability])
SELECT 
	[HeightStructuresOutputEntityId],
	[HeightStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability]
FROM [SOURCEPROJECT].HeightStructuresOutputEntity
JOIN [SOURCEPROJECT].HeightStructuresCalculationEntity USING(HeightStructuresCalculationEntityId)
WHERE ForeshoreProfileEntityId IS NULL 
UNION
SELECT 
	[HeightStructuresOutputEntityId],
	[HeightStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability]
FROM [SOURCEPROJECT].HeightStructuresOutputEntity
JOIN [SOURCEPROJECT].HeightStructuresCalculationEntity USING(HeightStructuresCalculationEntityId)
JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(ForeshoreProfileEntityId)
WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / 
(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) != 1;
INSERT INTO HeightStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity;
INSERT INTO HydraulicLocationCalculationCollectionEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationCollectionEntity;
INSERT INTO HydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationEntity;
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
INSERT INTO PipingCalculationEntity SELECT * FROM [SOURCEPROJECT].PipingCalculationEntity;
INSERT INTO PipingCalculationOutputEntity SELECT * FROM [SOURCEPROJECT].PipingCalculationOutputEntity;
INSERT INTO PipingCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].PipingCharacteristicPointEntity;
INSERT INTO PipingFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingSectionResultEntity;
INSERT INTO PipingSoilLayerEntity(
	[PipingSoilLayerEntityId],
	[PipingSoilProfileEntityId],
	[Top],
	[IsAquifer], 
	[Color], 
	[MaterialName],
	[BelowPhreaticLevelMean] ,
	[BelowPhreaticLevelDeviation],
	[BelowPhreaticLevelShift],
	[DiameterD70Mean],
	[DiameterD70CoefficientOfVariation],
	[PermeabilityMean],
	[PermeabilityCoefficientOfVariation],
	[Order])
SELECT 
	[PipingSoilLayerEntityId],
	[PipingSoilProfileEntityId],
	[Top],
	[IsAquifer], 
	[Color], 
	[MaterialName],
	CASE
		WHEN [BelowPhreaticLevelMean] < 0.005
			THEN NULL
		ELSE
			[BelowPhreaticLevelMean]
	END,
	[BelowPhreaticLevelDeviation],
	[BelowPhreaticLevelShift],
	CASE
		WHEN [DiameterD70Mean] <= 0.0000005
			THEN NULL
		ELSE
			[DiameterD70Mean]
	END,
	[DiameterD70CoefficientOfVariation],
	CASE
		WHEN [PermeabilityMean] <= 0.0000005
			THEN NULL
		ELSE
			[PermeabilityMean]
	END,
	[PermeabilityCoefficientOfVariation],
	[Order]
FROM [SOURCEPROJECT].PipingSoilLayerEntity;
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
	[FailureProbabilityStructureWithErosion],
	[ShouldIllustrationPointsBeCalculated])
SELECT 
	[StabilityPointStructuresCalculationEntityId],
	[CalculationGroupEntityId],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[ForeshoreProfileEntityId]
	END,
	[HydraulicLocationEntityId],
	[StabilityPointStructureEntityId],
	[Order],
	[Name],
	[Comments],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseBreakWater]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 3
		ELSE 
			[BreakWaterType]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[BreakWaterHeight]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseForeshore]
	END,
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
	[FailureProbabilityStructureWithErosion],
	[ShouldIllustrationPointsBeCalculated]
FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity
LEFT JOIN (
	SELECT 
		ForeshoreProfileEntityId,
		CASE 
			WHEN (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / (LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) = 1
				THEN 0
			ELSE 1
		END AS ValidForeshoreProfile
	FROM [SOURCEPROJECT].ForeshoreProfileEntity
) USING(ForeshoreProfileEntityId);
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityPointStructuresOutputEntity(
	[StabilityPointStructuresOutputEntityId],
	[StabilityPointStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability])
SELECT 
	[StabilityPointStructuresOutputEntityId],
	[StabilityPointStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability]
FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity
JOIN [SOURCEPROJECT].StabilityPointStructuresCalculationEntity USING(StabilityPointStructuresCalculationEntityId)
WHERE ForeshoreProfileEntityId IS NULL 
UNION
SELECT 
	[StabilityPointStructuresOutputEntityId],
	[StabilityPointStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability]
FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity
JOIN [SOURCEPROJECT].StabilityPointStructuresCalculationEntity USING(StabilityPointStructuresCalculationEntityId)
JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(ForeshoreProfileEntityId)
WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / 
(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) != 1;
INSERT INTO StabilityPointStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity (
	[StabilityStoneCoverWaveConditionsCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	[HydraulicLocationEntityId],
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
	[CategoryType],
	[CalculationType])
SELECT [StabilityStoneCoverWaveConditionsCalculationEntityId],
	[CalculationGroupEntityId],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[ForeshoreProfileEntityId]
	END,
	[HydraulicLocationEntityId],
	[Order],
	[Name],
	[Comments],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseBreakWater]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 3
		ELSE 
			[BreakWaterType]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[BreakWaterHeight]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseForeshore]
	END,
	[Orientation],
	[UpperBoundaryRevetment],
	[LowerBoundaryRevetment],
	[UpperBoundaryWaterLevels],
	[LowerBoundaryWaterLevels],
	[StepSize],
	[CategoryType],
	3 
FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity
LEFT JOIN (
	SELECT 
		ForeshoreProfileEntityId,
		CASE 
			WHEN (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / (LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) = 1
				THEN 0
			ELSE 1
		END AS ValidForeshoreProfile
	FROM [SOURCEPROJECT].ForeshoreProfileEntity
) USING(ForeshoreProfileEntityId);
INSERT INTO StabilityStoneCoverWaveConditionsOutputEntity(
	[StabilityStoneCoverWaveConditionsOutputEntityId],
	[StabilityStoneCoverWaveConditionsCalculationEntityId],
	[Order],
	[OutputType],
	[WaterLevel],
	[WaveHeight],
	[WavePeakPeriod],
	[WaveAngle],
	[WaveDirection],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence])
SELECT 
	[StabilityStoneCoverWaveConditionsOutputEntityId],
	[StabilityStoneCoverWaveConditionsCalculationEntityId],
	output.[Order],
	[OutputType],
	[WaterLevel],
	[WaveHeight],
	[WavePeakPeriod],
	[WaveAngle],
	[WaveDirection],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsOutputEntity output
JOIN [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity USING(StabilityStoneCoverWaveConditionsCalculationEntityId)
WHERE ForeshoreProfileEntityId IS NULL 
UNION
SELECT 
	[StabilityStoneCoverWaveConditionsOutputEntityId],
	[StabilityStoneCoverWaveConditionsCalculationEntityId],
	output.[Order],
	[OutputType],
	[WaterLevel],
	[WaveHeight],
	[WavePeakPeriod],
	[WaveAngle],
	[WaveDirection],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsOutputEntity output
JOIN [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity USING(StabilityStoneCoverWaveConditionsCalculationEntityId)
JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(ForeshoreProfileEntityId)
WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / 
(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) != 1;
INSERT INTO StochastEntity SELECT * FROM [SOURCEPROJECT].StochastEntity;
INSERT INTO StochasticSoilModelEntity SELECT * FROM [SOURCEPROJECT].StochasticSoilModelEntity;
INSERT INTO StrengthStabilityLengthwiseConstructionSectionResultEntity SELECT * FROM [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity;
INSERT INTO SubMechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].SubMechanismIllustrationPointEntity;
INSERT INTO SubMechanismIllustrationPointStochastEntity SELECT * FROM [SOURCEPROJECT].SubMechanismIllustrationPointStochastEntity;
INSERT INTO SurfaceLineEntity SELECT * FROM [SOURCEPROJECT].SurfaceLineEntity;
INSERT INTO TechnicalInnovationSectionResultEntity SELECT * FROM [SOURCEPROJECT].TechnicalInnovationSectionResultEntity;
INSERT INTO TopLevelFaultTreeIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].TopLevelFaultTreeIllustrationPointEntity;
INSERT INTO TopLevelSubMechanismIllustrationPointEntity SELECT * FROM [SOURCEPROJECT].TopLevelSubMechanismIllustrationPointEntity;
INSERT INTO WaterPressureAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity;
INSERT INTO VersionEntity (
	[VersionId],
	[Version],
	[Timestamp],
	[FingerPrint])
SELECT [VersionId],
	"19.1",
	[Timestamp],
	[FingerPrint]
FROM [SOURCEPROJECT].VersionEntity;
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverFailureMechanismMetaEntity;
INSERT INTO WaveImpactAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsCalculationEntity(
	[WaveImpactAsphaltCoverWaveConditionsCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	[HydraulicLocationEntityId],
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
	[CategoryType])
SELECT 
	[WaveImpactAsphaltCoverWaveConditionsCalculationEntityId],
	[CalculationGroupEntityId],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[ForeshoreProfileEntityId]
	END,
	[HydraulicLocationEntityId],
	[Order],
	[Name],
	[Comments],
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseBreakWater]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 3
		ELSE 
			[BreakWaterType]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN NULL
		ELSE 
			[BreakWaterHeight]
	END,
	CASE 
		WHEN ValidForeshoreProfile = 0
			THEN 0
		ELSE 
			[UseForeshore]
	END,
	[Orientation],
	[UpperBoundaryRevetment],
	[LowerBoundaryRevetment],
	[UpperBoundaryWaterLevels],
	[LowerBoundaryWaterLevels],
	[StepSize], 
	[CategoryType] 
FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity
LEFT JOIN (
	SELECT 
		ForeshoreProfileEntityId,
		CASE 
			WHEN (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / (LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) = 1
				THEN 0
			ELSE 1
		END AS ValidForeshoreProfile
	FROM [SOURCEPROJECT].ForeshoreProfileEntity
) USING(ForeshoreProfileEntityId);
INSERT INTO WaveImpactAsphaltCoverWaveConditionsOutputEntity(
	[WaveImpactAsphaltCoverWaveConditionsOutputEntityId],
	[WaveImpactAsphaltCoverWaveConditionsCalculationEntityId],
	[Order],
	[WaterLevel],
	[WaveHeight],
	[WavePeakPeriod],
	[WaveAngle],
	[WaveDirection],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence])
SELECT 
	[WaveImpactAsphaltCoverWaveConditionsOutputEntityId],
	[WaveImpactAsphaltCoverWaveConditionsCalculationEntityId],
	output.[Order],
	[WaterLevel],
	[WaveHeight],
	[WavePeakPeriod],
	[WaveAngle],
	[WaveDirection],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity output
JOIN [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity USING(WaveImpactAsphaltCoverWaveConditionsCalculationEntityId)
WHERE ForeshoreProfileEntityId IS NULL 
UNION
SELECT 
	[WaveImpactAsphaltCoverWaveConditionsOutputEntityId],
	[WaveImpactAsphaltCoverWaveConditionsCalculationEntityId],
	output.[Order],
	[WaterLevel],
	[WaveHeight],
	[WavePeakPeriod],
	[WaveAngle],
	[WaveDirection],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity output
JOIN [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity USING(WaveImpactAsphaltCoverWaveConditionsCalculationEntityId)
JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(ForeshoreProfileEntityId)
WHERE (LENGTH(GeometryXML) - LENGTH(REPLACE(REPLACE(GeometryXML, '<SerializablePoint2D>', ''), '</SerializablePoint2D>', ''))) / 
(LENGTH('<SerializablePoint2D>') + LENGTH('</SerializablePoint2D>')) != 1;

/*
Insert new data
*/

-- Directory retrieval was taken from https://stackoverflow.com/questions/21388820/how-to-get-the-last-index-of-a-substring-in-sqlite
INSERT INTO HydraulicBoundaryDatabaseEntity (
	[AssessmentSectionEntityId],
	[Version],
	[FilePath],
	[HydraulicLocationConfigurationSettingsFilePath],
	[HydraulicLocationConfigurationSettingsUsePreprocessorClosure],
	[HydraulicLocationConfigurationSettingsScenarioName],
	[HydraulicLocationConfigurationSettingsYear],
	[HydraulicLocationConfigurationSettingsScope],
	[HydraulicLocationConfigurationSettingsSeaLevel],
	[HydraulicLocationConfigurationSettingsRiverDischarge],
	[HydraulicLocationConfigurationSettingsLakeLevel],
	[HydraulicLocationConfigurationSettingsWindDirection],
	[HydraulicLocationConfigurationSettingsWindSpeed],
	[HydraulicLocationConfigurationSettingsComment])
SELECT 
	[AssessmentSectionEntityId],
	[HydraulicDatabaseVersion],
	[HydraulicDatabaseLocation],
	rtrim([HydraulicDatabaseLocation], replace([HydraulicDatabaseLocation], '\', '')) || 'hlcd.sqlite',
	0,
	"WBI2017",
	2023,
	"WBI2017",
	"Conform WBI2017",
	"Conform WBI2017",
	"Conform WBI2017",
	"Conform WBI2017",
	"Conform WBI2017",
	"Gegenereerd door Riskeer (conform WBI2017)"
FROM SOURCEPROJECT.AssessmentSectionEntity
WHERE [HydraulicDatabaseLocation] IS NOT NULL;

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

CREATE TEMP TABLE TempAssessmentSectionChanges
(
	[AssessmentSectionId],
	[AssessmentSectionName],
	[Order],
	[msg]
);

INSERT INTO TempAssessmentSectionChanges
SELECT
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	0,
	"Er worden standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie."
FROM HydraulicBoundaryDatabaseEntity
JOIN AssessmentSectionEntity AS ase USING(AssessmentSectionEntityId)
JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[AssessmentSectionId] = ase.AssessmentSectionEntityId;

INSERT INTO TempAssessmentSectionChanges
SELECT
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	1,
	"De waarde voor de transparantie van de achtergrondkaart is aangepast naar 0.60."
FROM AssessmentSectionEntity AS ase
JOIN BackgroundDataEntity AS bd USING(AssessmentSectionEntityId)
JOIN [SOURCEPROJECT].BackgroundDataEntity AS source USING(BackgroundDataEntityId)
JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[AssessmentSectionId] = ase.AssessmentSectionEntityId
WHERE source.[Transparency] != bd.Transparency;

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
	"De waarde '" || source.[BelowPhreaticLevelMean] ||  "' voor het gemiddelde van parameter 'Verzadigd gewicht' van ondergrondlaag '" || source.[MaterialName] || "' is ongeldig en is veranderd naar NaN."
	FROM PipingSoilLayerEntity as psl
	JOIN PipingSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN PipingStochasticSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN StochasticSoilModelEntity USING(StochasticSoilModelEntityId)
	JOIN [SOURCEPROJECT].PipingSoilLayerEntity AS source ON psl.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = [FailureMechanismEntityId]
	WHERE source.[BelowPhreaticLevelMean] IS NOT psl.[BelowPhreaticLevelMean];

INSERT INTO TempChanges
SELECT
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"De waarde '" || source.[DiameterD70Mean] ||  "' voor het gemiddelde van parameter 'd70' van ondergrondlaag '" || source.[MaterialName] || "' is ongeldig en is veranderd naar NaN."
	FROM PipingSoilLayerEntity as psl
	JOIN PipingSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN PipingStochasticSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN StochasticSoilModelEntity USING(StochasticSoilModelEntityId)
	JOIN [SOURCEPROJECT].PipingSoilLayerEntity AS source ON psl.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = [FailureMechanismEntityId]
	WHERE source.[DiameterD70Mean] IS NOT psl.[DiameterD70Mean];

INSERT INTO TempChanges
SELECT
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"De waarde '" || source.[PermeabilityMean] ||  "' voor het gemiddelde van parameter 'Doorlatendheid' van ondergrondlaag '" || source.[MaterialName] || "' is ongeldig en is veranderd naar NaN."
	FROM PipingSoilLayerEntity as psl
	JOIN PipingSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN PipingStochasticSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN StochasticSoilModelEntity USING(StochasticSoilModelEntityId)
	JOIN [SOURCEPROJECT].PipingSoilLayerEntity AS source ON psl.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = [FailureMechanismEntityId]
	WHERE source.[PermeabilityMean] IS NOT psl.[PermeabilityMean];

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion],
	[ToVersion],
	[LogMessage])
VALUES ("18.1", "19.1", "Gevolgen van de migratie van versie 18.1 naar versie 19.1:");

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
	ORDER BY [FailureMechanismId], [AssessmentSectionId]
),
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
		WHERE [AssessmentSectionId] IS NOT NULL
	)

	UNION

	SELECT
		*
	FROM 
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
			WHERE fmm.[AssessmentSectionId] IS [AssessmentSectionId]

	) ORDER BY [AssessmentSectionId], [FailureMechanismId], [level], [IsAssessmentSectionHeader] DESC, [Order]
)
SELECT 
	"18.1",
	"19.1",
	CASE 
		WHEN [AssessmentSectionName] IS NOT NULL
			THEN 
				CASE WHEN [IsAssessmentSectionHeader] IS 1
					THEN "* Traject: '" || [AssessmentSectionName] || "'"
				ELSE
					"  + " || [msg]
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

DROP TABLE TempFailureMechanisms;
DROP TABLE TempAssessmentSectionFailureMechanism;
DROP TABLE TempAssessmentSectionChanges;
DROP TABLE TempChanges;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion], 
	[ToVersion], 
	[LogMessage])
SELECT "18.1",
	"19.1", 
	"* Geen aanpassingen."
	WHERE (
		SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
		WHERE [FromVersion] = "18.1"
	) IS 1;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;