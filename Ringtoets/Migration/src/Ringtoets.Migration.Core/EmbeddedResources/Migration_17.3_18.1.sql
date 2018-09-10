 /*
Migration script for migrating Ringtoets databases.
SourceProject version: 17.3
TargetProject version: 18.1
*/
PRAGMA foreign_keys = OFF;

ATTACH DATABASE "{0}" AS SOURCEPROJECT;

INSERT INTO BackgroundDataEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataEntity;
INSERT INTO BackgroundDataMetaEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataMetaEntity;
INSERT INTO CalculationGroupEntity SELECT * FROM [SOURCEPROJECT].CalculationGroupEntity;
INSERT INTO ClosingStructureEntity (
	[ClosingStructureEntityId],
	[FailureMechanismEntityId],
	[Order],
	[Name],
	[Id],
	[X],
	[Y],
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
	[InflowModelType])
SELECT
	[ClosingStructureEntityId],
	[FailureMechanismEntityId],
	[Order],
	[Name],
	[Id],
	[X],
	[Y],
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
	CASE
		WHEN [ProbabilityOrFrequencyOpenStructureBeforeFlooding] > 1
			THEN NULL
		ELSE
			[ProbabilityOrFrequencyOpenStructureBeforeFlooding]
	END,
	[FailureProbabilityOpenStructure],
	CASE
		WHEN [IdenticalApertures] < 1
			THEN 1
		ELSE 
			[IdenticalApertures]
	END,
	[FailureProbabilityReparation],
	[InflowModelType]
FROM [SOURCEPROJECT].ClosingStructureEntity;
INSERT INTO ClosingStructuresCalculationEntity (
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
	[ProbabilityOrFrequencyOpenStructureBeforeFlooding],
	[FailureProbabilityOpenStructure],
	CASE
		WHEN [IdenticalApertures] < 1
			THEN 1
		ELSE 
			[IdenticalApertures]
	END,
	[FailureProbabilityReparation],
	[InflowModelType],
	[FailureProbabilityStructureWithErosion],
	[DeviationWaveDirection],
	[DrainCoefficientMean],
	[ModelFactorSuperCriticalFlowMean],
	[StormDurationMean],
	[FactorStormDurationOpenStructure],
	[ShouldIllustrationPointsBeCalculated]
	FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity;
INSERT INTO ClosingStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresFailureMechanismMetaEntity;
INSERT INTO ClosingStructuresSectionResultEntity (
	[ClosingStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[ClosingStructuresCalculationEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssemblyProbability],
	[ManualAssemblyProbability])
SELECT
	[ClosingStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[ClosingStructuresCalculationEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	CASE
		WHEN [LayerThree] IS NOT NULL
			THEN 3
		ELSE
			1
	END,
	[LayerThree],
	0,
	NULL
FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity;
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
	[Id],
	[Name],
	[Orientation],
	[BreakWaterType],
	[BreakWaterHeight],
	REPLACE( 
		REPLACE(ForeshoreXml, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''),  
		'Point2DXmlSerializer.SerializablePoint2D', 
		'SerializablePoint2D' 
	),
	REPLACE(
		REPLACE(DikeGeometryXml, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''),
		'RoughnessPointXmlSerializer.SerializableRoughnessPoint',
		'SerializableRoughnessPoint'
	),
	[DikeHeight],
	[X],
	[Y],
	[X0],
	[Order]
FROM [SOURCEPROJECT].DikeProfileEntity;
INSERT INTO DuneErosionSectionResultEntity (
	[DuneErosionSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResultForFactorizedSignalingNorm],
	[DetailedAssessmentResultForSignalingNorm],
	[DetailedAssessmentResultForMechanismSpecificLowerLimitNorm],
	[DetailedAssessmentResultForLowerLimitNorm],
	[DetailedAssessmentResultForFactorizedLowerLimitNorm],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT 
	[DuneErosionSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[LayerOne],
	1,
	1,
	1,
	1,
	1,
	1,
	0,
	1
FROM [SOURCEPROJECT].DuneErosionSectionResultEntity;
INSERT INTO DuneLocationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationEntity;
INSERT INTO FailureMechanismEntity (
	[FailureMechanismEntityId],
	[AssessmentSectionEntityId],
	[CalculationGroupEntityId],
	[FailureMechanismType],
	[IsRelevant],
	[FailureMechanismSectionCollectionSourcePath],
	[InputComments],
	[OutputComments],
	[NotRelevantComments])
SELECT 
	[FailureMechanismEntityId],
	[AssessmentSectionEntityId],
	[CalculationGroupEntityId],
	[FailureMechanismType],
	[IsRelevant],
	CASE WHEN COUNT([FailureMechanismSectionEntityId]) THEN "" ELSE NULL END,
	[InputComments],
	[OutputComments],
	[NotRelevantComments]
FROM [SOURCEPROJECT].FailureMechanismEntity
LEFT JOIN [SOURCEPROJECT].FailureMechanismSectionEntity USING (FailureMechanismEntityId)
GROUP BY FailureMechanismEntityId;
INSERT INTO FailureMechanismSectionEntity (
	[FailureMechanismSectionEntityId],
	[FailureMechanismEntityId],
	[Name],
	[FailureMechanismSectionPointXml])
SELECT 
	[FailureMechanismSectionEntityId],
	[FailureMechanismEntityId],
	[Name],
	REPLACE( 
		REPLACE(FailureMechanismSectionPointXml, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''),  
		'Point2DXmlSerializer.SerializablePoint2D', 
		'SerializablePoint2D' 
	)
FROM [SOURCEPROJECT].FailureMechanismSectionEntity;
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
	[Id],
	[Name],
	[Orientation],
	[BreakWaterType],
	[BreakWaterHeight],
	REPLACE( 
		REPLACE(GeometryXml, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''),  
		'Point2DXmlSerializer.SerializablePoint2D', 
		'SerializablePoint2D' 
	),
	[X],
	[Y],
	[X0],
	[Order]
FROM [SOURCEPROJECT].ForeshoreProfileEntity;
INSERT INTO GrassCoverErosionInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity;
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionInwardsSectionResultEntity (
	[GrassCoverErosionInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[GrassCoverErosionInwardsCalculationEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssemblyProbability],
	[ManualAssemblyProbability])
SELECT
	[GrassCoverErosionInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[GrassCoverErosionInwardsCalculationEntityId],
	[LayerOne],
	1,
	CASE
		WHEN [LayerThree] IS NOT NULL
			THEN 3
		ELSE
			1
	END,
	[LayerThree],
	0,
	NULL
FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsSectionResultEntity (
	[GrassCoverErosionOutwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResultForFactorizedSignalingNorm],
	[DetailedAssessmentResultForSignalingNorm],
	[DetailedAssessmentResultForMechanismSpecificLowerLimitNorm],
	[DetailedAssessmentResultForLowerLimitNorm],
	[DetailedAssessmentResultForFactorizedLowerLimitNorm],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT 
	[GrassCoverErosionOutwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	1,
	1,
	1,
	1,
	1,
	0,
	1
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity;
INSERT INTO GrassCoverSlipOffInwardsSectionResultEntity (
	[GrassCoverSlipOffInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT
	[GrassCoverSlipOffInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	1,
	0,
	1
FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity;
INSERT INTO GrassCoverSlipOffOutwardsSectionResultEntity (
	[GrassCoverSlipOffOutwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT
	[GrassCoverSlipOffOutwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	1,
	0,
	1
FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity;
INSERT INTO HeightStructureEntity SELECT * FROM [SOURCEPROJECT].HeightStructureEntity;
INSERT INTO HeightStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
INSERT INTO HeightStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity;
INSERT INTO HeightStructuresSectionResultEntity (
	[HeightStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[HeightStructuresCalculationEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssemblyProbability],
	[ManualAssemblyProbability])
SELECT
	[HeightStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[HeightStructuresCalculationEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	CASE
		WHEN [LayerThree] IS NOT NULL
			THEN 3
		ELSE
			1
	END,
	[LayerThree],
	0,
	NULL
FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity;
INSERT INTO HydraulicLocationEntity (
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	[LocationId],
	[Name],
	[LocationX],
	[LocationY],
	[Order])
SELECT 
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	[LocationId],
	[Name],
	[LocationX],
	[LocationY],
	[Order]
FROM [SOURCEPROJECT].HydraulicLocationEntity;
INSERT INTO IllustrationPointResultEntity SELECT * FROM [SOURCEPROJECT].IllustrationPointResultEntity;
INSERT INTO MacroStabilityInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity;
INSERT INTO MacroStabilityInwardsCalculationOutputEntity (
	[MacroStabilityInwardsCalculationOutputEntityId],
	[MacroStabilityInwardsCalculationEntityId],
	[FactorOfStability],
	[ZValue],
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
	[ZValue],
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
	REPLACE(
		REPLACE(SlidingCurveSliceXML, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''), 
		'MacroStabilityInwardsSliceXmlSerializer.SerializableMacroStabilityInwardsSlice',
		'SerializableMacroStabilityInwardsSlice'
	),
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
	REPLACE(
		REPLACE(SlipPlaneTangentLinesXml, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''), 
		'TangentLinesXmlSerializer.SerializableTangentLine',
		'SerializableTangentLine'
	)
FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity
JOIN [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId)
WHERE [UseAssessmentLevelManualInput] = 1;
INSERT INTO MacroStabilityInwardsCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCharacteristicPointEntity;
INSERT INTO MacroStabilityInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsFailureMechanismMetaEntity;
INSERT INTO MacroStabilityInwardsPreconsolidationStressEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsPreconsolidationStressEntity;
INSERT INTO MacroStabilityInwardsSectionResultEntity (
	[MacroStabilityInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssemblyProbability],
	[ManualAssemblyProbability])
SELECT
	[MacroStabilityInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	CASE
		WHEN [LayerThree] IS NOT NULL
			THEN 3
		ELSE
			1
	END,
	[LayerThree],
	0,
	NULL
FROM [SOURCEPROJECT].MacroStabilityInwardsSectionResultEntity;
INSERT INTO MacroStabilityInwardsSoilLayerOneDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilLayerOneDEntity;
INSERT INTO MacroStabilityInwardsSoilLayerTwoDEntity (
	[MacroStabilityInwardsSoilLayerTwoDEntityId],
	[ParentMacroStabilityInwardsSoilLayerTwoDEntityId],
	[IsAquifer],
	[Color],
	[MaterialName],
	[ShearStrengthModel],
	[UsePop],
	[AbovePhreaticLevelMean],
	[AbovePhreaticLevelCoefficientOfVariation],
	[AbovePhreaticLevelShift],
	[BelowPhreaticLevelMean],
	[BelowPhreaticLevelCoefficientOfVariation],
	[BelowPhreaticLevelShift],
	[CohesionMean],
	[CohesionCoefficientOfVariation],
	[FrictionAngleMean],
	[FrictionAngleCoefficientOfVariation],
	[ShearStrengthRatioMean],
	[ShearStrengthRatioCoefficientOfVariation],
	[StrengthIncreaseExponentMean],
	[StrengthIncreaseExponentCoefficientOfVariation],
	[PopMean],
	[PopCoefficientOfVariation],
	[OuterRingXml],
	[Order])
SELECT 
	[MacroStabilityInwardsSoilLayerTwoDEntityId],
	[ParentMacroStabilityInwardsSoilLayerTwoDEntityId],
	[IsAquifer],
	[Color],
	[MaterialName],
	[ShearStrengthModel],
	[UsePop],
	[AbovePhreaticLevelMean],
	[AbovePhreaticLevelCoefficientOfVariation],
	[AbovePhreaticLevelShift],
	[BelowPhreaticLevelMean],
	[BelowPhreaticLevelCoefficientOfVariation],
	[BelowPhreaticLevelShift],
	[CohesionMean],
	[CohesionCoefficientOfVariation],
	[FrictionAngleMean],
	[FrictionAngleCoefficientOfVariation],
	[ShearStrengthRatioMean],
	[ShearStrengthRatioCoefficientOfVariation],
	[StrengthIncreaseExponentMean],
	[StrengthIncreaseExponentCoefficientOfVariation],
	[PopMean],
	[PopCoefficientOfVariation],
	REPLACE( 
		REPLACE(OuterRingXml, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''),  
		'Point2DXmlSerializer.SerializablePoint2D', 
		'SerializablePoint2D' 
	),
	[Order]
FROM [SOURCEPROJECT].MacroStabilityInwardsSoilLayerTwoDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileOneDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileOneDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileTwoDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity;
INSERT INTO MacroStabilityInwardsStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsStochasticSoilProfileEntity;
INSERT INTO MacroStabilityOutwardsSectionResultEntity (
	[MacroStabilityOutwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[DetailedAssessmentProbability],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT
	[MacroStabilityOutwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	[LayerTwoA],
	1,
	NULL,
	0,
	1
FROM [SOURCEPROJECT].MacroStabilityOutwardsSectionResultEntity;
INSERT INTO MicrostabilitySectionResultEntity (
	[MicrostabilitySectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT
	[MicrostabilitySectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	1,
	0,
	1
FROM [SOURCEPROJECT].MicrostabilitySectionResultEntity;
INSERT INTO PipingCalculationEntity SELECT * FROM [SOURCEPROJECT].PipingCalculationEntity;
INSERT INTO PipingCalculationOutputEntity 
SELECT *
	FROM [SOURCEPROJECT].PipingCalculationOutputEntity
	WHERE PipingCalculationEntityId IN (
		SELECT PipingCalculationEntityId 
		FROM [SOURCEPROJECT].PipingCalculationEntity
		WHERE UseAssessmentLevelManualInput IS 1
	);
INSERT INTO PipingCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].PipingCharacteristicPointEntity;
INSERT INTO PipingFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingSectionResultEntity (
	[PipingSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssemblyProbability],
	[ManualAssemblyProbability])
SELECT
	[PipingSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	CASE
		WHEN [LayerThree] IS NOT NULL
			THEN 3
		ELSE
			1
	END,
	[LayerThree],
	0,
	NULL
FROM [SOURCEPROJECT].PipingSectionResultEntity;
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
		WHEN [BelowPhreaticLevelMean] <= 0
			THEN NULL
		ELSE
			[BelowPhreaticLevelMean]
	END,
	CASE
		WHEN [BelowPhreaticLevelDeviation] < 0
			THEN NULL
		ELSE
			[BelowPhreaticLevelDeviation]
	END,
	CASE
		WHEN [BelowPhreaticLevelMean] < [BelowPhreaticLevelShift]
			THEN NULL
		ELSE
			[BelowPhreaticLevelShift]
	END,
	CASE
		WHEN [DiameterD70Mean] <= 0
			THEN NULL
		ELSE
			[DiameterD70Mean]
	END,
	CASE
		WHEN [DiameterD70CoefficientOfVariation] < 0
			THEN NULL
		ELSE
			[DiameterD70CoefficientOfVariation]
	END,
	CASE
		WHEN [PermeabilityMean] <= 0
			THEN NULL
		ELSE
			[PermeabilityMean]
	END,
	CASE
		WHEN [PermeabilityCoefficientOfVariation] < 0
			THEN NULL
		ELSE
			[PermeabilityCoefficientOfVariation]
	END,
	[Order]
FROM [SOURCEPROJECT].PipingSoilLayerEntity;
INSERT INTO PipingSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingSoilProfileEntity;
INSERT INTO PipingStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingStochasticSoilProfileEntity;
INSERT INTO PipingStructureSectionResultEntity (
	[PipingStructureSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT
	[PipingStructureSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	1,
	0,
	1
FROM [SOURCEPROJECT].PipingStructureSectionResultEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO StabilityPointStructureEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity (
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
	CASE
		WHEN [VerticalDistance] < 0
			THEN NULL 
		ELSE 
			[VerticalDistance]
	END,
	[FailureProbabilityRepairClosure],
	[FailureCollisionEnergyMean],
	[FailureCollisionEnergyCoefficientOfVariation],
	[ShipMassMean],
	[ShipMassCoefficientOfVariation],
	[ShipVelocityMean],
	[ShipVelocityCoefficientOfVariation],
	CASE 
		WHEN [LevellingCount] < 0
			THEN 0
		ELSE 
			[LevellingCount]
	END,
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
FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityPointStructuresSectionResultEntity (
	[StabilityPointStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[StabilityPointStructuresCalculationEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResult],
	[TailorMadeAssessmentResult],
	[TailorMadeAssessmentProbability],
	[UseManualAssemblyProbability],
	[ManualAssemblyProbability])
SELECT
	[StabilityPointStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[StabilityPointStructuresCalculationEntityId],
	[LayerOne],
	1,
	CASE
		WHEN [LayerThree] IS NOT NULL
			THEN 3
		ELSE
			1
	END,
	[LayerThree],
	0,
	NULL
FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity (
	[StabilityStoneCoverFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[ForeshoreProfileCollectionSourcePath],
	[N])
SELECT [StabilityStoneCoverFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[ForeshoreProfileCollectionSourcePath],
	"4"
FROM [SOURCEPROJECT].StabilityStoneCoverFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverSectionResultEntity (
	[StabilityStoneCoverSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResultForFactorizedSignalingNorm],
	[DetailedAssessmentResultForSignalingNorm],
	[DetailedAssessmentResultForMechanismSpecificLowerLimitNorm],
	[DetailedAssessmentResultForLowerLimitNorm],
	[DetailedAssessmentResultForFactorizedLowerLimitNorm],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT 
	[StabilityStoneCoverSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[LayerOne],
	1,
	1,
	1,
	1,
	1,
	1,
	0,
	1
FROM [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity(
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
	[CategoryType]
) 
SELECT 
	[StabilityStoneCoverWaveConditionsCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	[HydraulicLocationEntityId],
	calc.'Order',
	calc.Name,
	calc.Comments,
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
	CASE
		WHEN [NormativeNormType] = 1 
			THEN 3
		ELSE 
			2
	END
FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity calc
JOIN [SOURCEPROJECT].CalculationGroupEntity USING(CalculationGroupEntityId)
JOIN [SOURCEPROJECT].FailureMechanismEntity USING(CalculationGroupEntityId)
JOIN [SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId);
INSERT INTO StochasticSoilModelEntity (
	[StochasticSoilModelEntityId],
	[FailureMechanismEntityId],
	[Name],
	[StochasticSoilModelSegmentPointXml],
	[Order])
SELECT 
	[StochasticSoilModelEntityId],
	[FailureMechanismEntityId],
	[Name],
	REPLACE( 
		REPLACE(StochasticSoilModelSegmentPointXml, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''),  
		'Point2DXmlSerializer.SerializablePoint2D', 
		'SerializablePoint2D' 
	),
	[Order]
FROM [SOURCEPROJECT].StochasticSoilModelEntity;
INSERT INTO StrengthStabilityLengthwiseConstructionSectionResultEntity (
	[StrengthStabilityLengthwiseConstructionSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT
	[StrengthStabilityLengthwiseConstructionSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	0,
	1
FROM [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity;
INSERT INTO SurfaceLineEntity(
	[SurfaceLineEntityId],
	[FailureMechanismEntityId],
	[Name],
	[ReferenceLineIntersectionX],
	[ReferenceLineIntersectionY],
	[PointsXml],
	[Order])
SELECT 
	[SurfaceLineEntityId],
	[FailureMechanismEntityId],
	[Name],
	[ReferenceLineIntersectionX],
	[ReferenceLineIntersectionY],
	REPLACE(
		REPLACE(PointsXml, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''), 
		'Point3DXmlSerializer.SerializablePoint3D',
		'SerializablePoint3D'
	),
	[Order]
FROM [SOURCEPROJECT].SurfaceLineEntity;
INSERT INTO TechnicalInnovationSectionResultEntity (
	[TechnicalInnovationSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT
	[TechnicalInnovationSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	0,
	1
FROM [SOURCEPROJECT].TechnicalInnovationSectionResultEntity;
INSERT INTO VersionEntity (
	[VersionId],
	[Version],
	[Timestamp],
	[FingerPrint])
SELECT [VersionId],
	"18.1",
	[Timestamp],
	[FingerPrint]
FROM [SOURCEPROJECT].VersionEntity;
INSERT INTO WaterPressureAsphaltCoverSectionResultEntity (
	[WaterPressureAsphaltCoverSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT
	[WaterPressureAsphaltCoverSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	0,
	1
FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity (
	[WaveImpactAsphaltCoverFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[ForeshoreProfileCollectionSourcePath],
	[DeltaL])
SELECT [WaveImpactAsphaltCoverFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[ForeshoreProfileCollectionSourcePath],
	"1000"
FROM [SOURCEPROJECT].WaveImpactAsphaltCoverFailureMechanismMetaEntity;
INSERT INTO WaveImpactAsphaltCoverSectionResultEntity (
	[WaveImpactAsphaltCoverSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[SimpleAssessmentResult],
	[DetailedAssessmentResultForFactorizedSignalingNorm],
	[DetailedAssessmentResultForSignalingNorm],
	[DetailedAssessmentResultForMechanismSpecificLowerLimitNorm],
	[DetailedAssessmentResultForLowerLimitNorm],
	[DetailedAssessmentResultForFactorizedLowerLimitNorm],
	[TailorMadeAssessmentResult],
	[UseManualAssemblyCategoryGroup],
	[ManualAssemblyCategoryGroup])
SELECT 
	[WaveImpactAsphaltCoverSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	CASE
		WHEN [LayerOne] = 3
			THEN 4
		ELSE
			[LayerOne]
	END,
	1,
	1,
	1,
	1,
	1,
	1,
	0,
	1
FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsCalculationEntity (
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
	[CategoryType]
) 
SELECT 
	[WaveImpactAsphaltCoverWaveConditionsCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	[HydraulicLocationEntityId],
	calc.'Order',
	calc.Name,
	calc.Comments,
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
	CASE
		WHEN [NormativeNormType] = 1 
			THEN 3
		ELSE 
			2
	END
FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity calc
JOIN [SOURCEPROJECT].CalculationGroupEntity USING(CalculationGroupEntityId)
JOIN [SOURCEPROJECT].FailureMechanismEntity USING(CalculationGroupEntityId)
JOIN [SOURCEPROJECT].AssessmentSectionEntity USING(AssessmentSectionEntityId);

/*
Outputs that used HydraRing are not migrated
*/
-- ClosingStructuresOutputEntity
-- GrassCoverErosionOutwardsWaveConditionsOutputEntity
-- GrassCoverErosionInwardsDikeHeightOutputEntity
-- GrassCoverErosionInwardsOutputEntity
-- GrassCoverErosionInwardsOvertoppingRateOutputEntity
-- HeightStructuresOutputEntity
-- StabilityPointStructuresOutputEntity
-- StabilityStoneCoverWaveConditionsOutputEntity
-- WaveImpactAsphaltCoverWaveConditionsOutputEntity
-- TopLevelFaultTreeIllustrationPointEntity
-- TopLevelSubMechanismIllustrationPointEntity
-- GeneralResultFaultTreeIllustrationPointEntity
-- GeneralResultFaultTreeIllustrationPointStochastEntity
-- GeneralResultSubMechanismIllustrationPointEntity
-- GeneralResultSubMechanismIllustrationPointStochastEntity
-- SubMechanismIllustrationPointEntity
-- SubMechanismIllustrationPointStochastEntity
-- FaultTreeIllustrationPointEntity
-- FaultTreeIllustrationPointStochastEntity
-- FaultTreeSubmechanismIllustrationPointEntity
-- StochastEntity
-- (Conditional) MacroStabilityInwardsCalculationOutputEntity
-- (Conditional) PipingCalculationOutputEntity

/*
Insert new data
*/
INSERT INTO MacroStabilityOutwardsFailureMechanismMetaEntity (
	[FailureMechanismEntityId],
	[A])
SELECT FailureMechanismEntityId,
	0.033
FROM FailureMechanismEntity WHERE FailureMechanismType = 13;
	
INSERT INTO PipingStructureFailureMechanismMetaEntity (
	[FailureMechanismEntityId],
	[N])
SELECT FailureMechanismEntityId,
	1.0
FROM FailureMechanismEntity WHERE FailureMechanismType = 11;

-- Migrate the Hydraulic Boundary Locations and associated data
/*
Note, the following conventions are used for the calculation type on AssessmentSectionEntity:
- The water level calculations for the factorized signaling norm = 0.
- The water level calculations for the signaling norm = 1.
- The water level calculations for the lower limit norm = 2.
- The water level calculations for the factorized lower limit norm = 3.
- The wave height calculations for the factorized signaling norm = 4.
- The wave height calculations for the signaling norm = 5.
- The wave height calculations for the lower limit norm = 6.
- The wave height calculations for the factorized lower limit norm = 7.
For grass cover erosion outwards:
- The design water level failure mechanism specific factorized signaling norm = 8.
- The design water level failure mechanism specific signaling norm = 9.
- The design water level failure mechanism specific lower limit norm = 10.
- The wave height failure mechanism specific factorized signaling norm = 11.
- The wave height failure mechanism specific signaling norm = 12.
- The wave height failure mechanism specific lower limit norm = 13.
*/
CREATE TEMP TABLE TempCalculationTypes ( 'CalculationType' TINYINT (1) NOT NULL);
INSERT INTO TempCalculationTypes VALUES (0);
INSERT INTO TempCalculationTypes VALUES (1);
INSERT INTO TempCalculationTypes VALUES (2);
INSERT INTO TempCalculationTypes VALUES (3);
INSERT INTO TempCalculationTypes VALUES (4);
INSERT INTO TempCalculationTypes VALUES (5);
INSERT INTO TempCalculationTypes VALUES (6);
INSERT INTO TempCalculationTypes VALUES (7);
INSERT INTO TempCalculationTypes VALUES (8);
INSERT INTO TempCalculationTypes VALUES (9);
INSERT INTO TempCalculationTypes VALUES (10);
INSERT INTO TempCalculationTypes VALUES (11);
INSERT INTO TempCalculationTypes VALUES (12);
INSERT INTO TempCalculationTypes VALUES (13);

-- Migrate the hydraulic boundary location calculations on assessment section level
-- Create the calculation collections
CREATE TEMP TABLE TempHydraulicLocationCalculationCollectionEntity
(
	'HydraulicLocationCalculationCollectionEntityId' INTEGER NOT NULL,
	'AssessmentSectionEntityId' INTEGER,
	'GrassCoverErosionOutwardsFailureMechanismMetaEntityId' INTEGER,
	'CalculationType' TINYINT (1) NOT NULL,
	PRIMARY KEY
	(
		'HydraulicLocationCalculationCollectionEntityId' AUTOINCREMENT
	)
);

INSERT INTO TempHydraulicLocationCalculationCollectionEntity (
	[AssessmentSectionEntityId],
	[CalculationType])
SELECT
	[AssessmentSectionEntityId],
	[CalculationType]
FROM [SOURCEPROJECT].AssessmentSectionEntity
JOIN (
	SELECT * 
	FROM TempCalculationTypes
	WHERE CalculationType >= 0 AND CalculationType <= 7
);

INSERT INTO TempHydraulicLocationCalculationCollectionEntity (
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[CalculationType])
SELECT
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[CalculationType]
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity
JOIN (
	SELECT * 
	FROM TempCalculationTypes
	WHERE CalculationType >= 8 AND CalculationType <= 13
);

INSERT INTO HydraulicLocationCalculationCollectionEntity (
	[HydraulicLocationCalculationCollectionEntityId])
SELECT
	[HydraulicLocationCalculationCollectionEntityId]
FROM TempHydraulicLocationCalculationCollectionEntity;

-- Create the calculation entities
CREATE TEMP TABLE TempHydraulicLocationCalculationEntity
(
	'HydraulicLocationCalculationEntityId' INTEGER NOT NULL,
	'HydraulicLocationEntityId' INTEGER NOT NULL,
	'AssessmentSectionEntityId' INTEGER,
	'GrassCoverErosionOutwardsFailureMechanismMetaEntityId' INTEGER,
	'CalculationType' TINYINT (1) NOT NULL,
	PRIMARY KEY
	(
		'HydraulicLocationCalculationEntityId' AUTOINCREMENT
	)
);

-- Create the calculations for the Hydraulic Boundary Locations on AssessmentSection level
INSERT INTO TempHydraulicLocationCalculationEntity (
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	[CalculationType])
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	[CalculationType]
FROM HydraulicLocationEntity
JOIN (
	SELECT * 
	FROM TempCalculationTypes
	WHERE CalculationType >= 0 AND CalculationType <= 7
);

-- Create a lookup table to match the Hydraulic Boundary Locations on Grass Cover Erosion Outwards Failure Mechanism
-- with the Hydraulic Boundary Locations in the Assessment Section level, because the primary keys of the 
-- Hydraulic Boundary Locations between these two levels might differ. 
-- The translation between the Hydraulic Boundary Locations is based on the LocationID (which is assumed to be unique
-- for every Hydraulic Boundary Location) and the AssessmentSection (which is necessary because multiple assessment 
-- sections can contain the same Hydraulic Boundary Locations)
CREATE TEMP TABLE TempGrassCoverErosionOutwardsHydraulicBoundaryLocationLookupTable (
	'GrassCoverErosionOutwardsHydraulicLocationEntityId' INTEGER NOT NULL UNIQUE,
	'HydraulicLocationEntityId' INTEGER NOT NULL UNIQUE
);

INSERT INTO TempGrassCoverErosionOutwardsHydraulicBoundaryLocationLookupTable (
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[HydraulicLocationEntityId]
)
SELECT 
	GrassCoverErosionOutwardsHydraulicLocationEntityId,
	HydraulicLocationEntityId
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity gceohl
JOIN [SOURCEPROJECT].FailureMechanismEntity fm USING(FailureMechanismEntityId)
JOIN [SOURCEPROJECT].HydraulicLocationEntity hl ON (hl.LocationId = gceohl.LocationId AND hl.AssessmentSectionEntityId = fm.AssessmentSectionEntityId);

-- Create the calculations for the Hydraulic Boundary Locations on Grass Cover Erosion Outwards Failure Mechanism level
INSERT INTO TempHydraulicLocationCalculationEntity (
	[HydraulicLocationEntityId],
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[CalculationType])
SELECT
	[HydraulicLocationEntityId],
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId], 
	[CalculationType]
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity
JOIN [SOURCEPROJECT].FailureMechanismEntity USING(FailureMechanismEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity USING(FailureMechanismEntityId)
JOIN TempGrassCoverErosionOutwardsHydraulicBoundaryLocationLookupTable USING(GrassCoverErosionOutwardsHydraulicLocationEntityId)
JOIN (
	SELECT * 
	FROM TempCalculationTypes
	WHERE CalculationType >= 8 AND CalculationType <= 13
);

-- Insert the calculations 
INSERT INTO HydraulicLocationCalculationEntity (
	[HydraulicLocationCalculationEntityId],
	[HydraulicLocationEntityId],
	[HydraulicLocationCalculationCollectionEntityId],
	[ShouldIllustrationPointsBeCalculated])
SELECT
	[HydraulicLocationCalculationEntityId],
	[HydraulicLocationEntityId],
	[HydraulicLocationCalculationCollectionEntityId],
	0
FROM TempHydraulicLocationCalculationCollectionEntity collections
JOIN TempHydraulicLocationCalculationEntity calculations 
ON calculations.CalculationType = collections.CalculationType
AND (calculations.AssessmentSectionEntityId = collections.AssessmentSectionEntityId 
	OR calculations.GrassCoverErosionOutwardsFailureMechanismMetaEntityId = collections.GrassCoverErosionOutwardsFailureMechanismMetaEntityId);

-- Migration for the Hydraulic Boundary Locations on Assessment Section Level
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
	[HydraulicDatabaseVersion],
	[HydraulicDatabaseLocation],
	[Composition],
	[ReferenceLinePointXml],
	[Order])
SELECT
	[AssessmentSectionEntityId],
	[ProjectEntityId],
	[WaterLevelCalculationsForFactorizedSignalingNormId],
	[WaterLevelCalculationsForSignalingNormId],
	[WaterLevelCalculationsForLowerLimitNormId],
	[WaterLevelCalculationsForFactorizedLowerLimitNormId],
	[WaveHeightCalculationsForFactorizedSignalingNormId],
	[WaveHeightCalculationsForSignalingNormId],
	[WaveHeightCalculationsForLowerLimitNormId],
	[WaveHeightCalculationsForFactorizedLowerLimitNormId],
	[Id],
	[Name],
	[Comments],
	[LowerLimitNorm],
	[SignalingNorm],
	[NormativeNormType],
	[HydraulicDatabaseVersion],
	[HydraulicDatabaseLocation],
	[Composition],
	REPLACE( 
		REPLACE(ReferenceLinePointXml, ' xmlns="http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers"', ''),  
		'Point2DXmlSerializer.SerializablePoint2D', 
		'SerializablePoint2D' 
	), 
	[Order]
FROM [SOURCEPROJECT].AssessmentSectionEntity
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForFactorizedSignalingNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 0
) USING(AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForSignalingNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 1
) USING(AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForLowerLimitNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 2
) USING(AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForFactorizedLowerLimitNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 3
) USING(AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFactorizedSignalingNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 4
) USING(AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForSignalingNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 5
) USING(AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForLowerLimitNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 6
) USING(AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFactorizedLowerLimitNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 7
) USING(AssessmentSectionEntityId);

-- Update the calculation inputs
UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM AssessmentSectionEntity ase
		JOIN HydraulicLocationCalculationCollectionEntity hlcce 
		ON ase.HydraulicLocationCalculationCollectionEntity2Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		OR ase.HydraulicLocationCalculationCollectionEntity3Id = hlcce.HydraulicLocationCalculationCollectionEntityId 
		OR ase.HydraulicLocationCalculationCollectionEntity6Id = hlcce.HydraulicLocationCalculationCollectionEntityId 
		OR ase.HydraulicLocationCalculationCollectionEntity7Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationEntity USING(HydraulicLocationCalculationCollectionEntityId)
		JOIN [SOURCEPROJECT].HydraulicLocationEntity USING(HydraulicLocationEntityId)
		WHERE(ShouldDesignWaterLevelIllustrationPointsBeCalculated = 1 AND NormativeNormType = 2 AND ase.HydraulicLocationCalculationCollectionEntity2Id = hlcce.HydraulicLocationCalculationCollectionEntityId)
		OR (ShouldDesignWaterLevelIllustrationPointsBeCalculated = 1 AND NormativeNormType = 1 AND ase.HydraulicLocationCalculationCollectionEntity3Id = hlcce.HydraulicLocationCalculationCollectionEntityId)
		OR (ShouldWaveHeightIllustrationPointsBeCalculated = 1 AND NormativeNormType = 2 AND ase.HydraulicLocationCalculationCollectionEntity6Id = hlcce.HydraulicLocationCalculationCollectionEntityId)
		OR (ShouldWaveHeightIllustrationPointsBeCalculated = 1 AND NormativeNormType = 1 AND ase.HydraulicLocationCalculationCollectionEntity7Id = hlcce.HydraulicLocationCalculationCollectionEntityId)
);

-- Migration for the Hydraulic Boundary Locations on Grass Cover Erosion Outwards Failure Mechanism Level
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity (
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[HydraulicLocationCalculationCollectionEntity1Id],
	[HydraulicLocationCalculationCollectionEntity2Id],
	[HydraulicLocationCalculationCollectionEntity3Id],
	[HydraulicLocationCalculationCollectionEntity4Id],
	[HydraulicLocationCalculationCollectionEntity5Id],
	[HydraulicLocationCalculationCollectionEntity6Id],
	[N],
	[ForeshoreProfileCollectionSourcePath])
SELECT
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[WaterLevelCalculationsForFailureMechanismSpecificFactorizedSignalingNormId],
	[WaterLevelCalculationsForFailureMechanismSpecificSignalingNormId],
	[WaterLevelCalculationsForFailureMechanismSpecificLowerLimitNormId],
	[WaveHeightCalculationsForFailureMechanismSpecificFactorizedSignalingNormId],
	[WaveHeightCalculationsForFailureMechanismSpecificSignalingNormId],
	[WaveHeightCalculationsForFailureMechanismSpecificLowerLimitNormId],
	[N],
	[ForeshoreProfileCollectionSourcePath]
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForFailureMechanismSpecificFactorizedSignalingNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 8
) USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForFailureMechanismSpecificSignalingNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 9
) USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForFailureMechanismSpecificLowerLimitNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 10
) USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFailureMechanismSpecificFactorizedSignalingNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 11
) USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFailureMechanismSpecificSignalingNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 12
) USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFailureMechanismSpecificLowerLimitNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 13
) USING(GrassCoverErosionOutwardsFailureMechanismMetaEntityId);

-- Update the calculation inputs
UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme
		JOIN FailureMechanismEntity fm USING(FailureMechanismEntityId)
		JOIN AssessmentSectionEntity USING(AssessmentSectionEntityId)
		JOIN HydraulicLocationCalculationCollectionEntity hlcce 
		ON gceofmme.HydraulicLocationCalculationCollectionEntity2Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		OR gceofmme.HydraulicLocationCalculationCollectionEntity3Id = hlcce.HydraulicLocationCalculationCollectionEntityId 
		OR gceofmme.HydraulicLocationCalculationCollectionEntity5Id = hlcce.HydraulicLocationCalculationCollectionEntityId 
		OR gceofmme.HydraulicLocationCalculationCollectionEntity6Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationEntity USING(HydraulicLocationCalculationCollectionEntityId)
		JOIN HydraulicLocationEntity hl USING(HydraulicLocationEntityId)
		JOIN TempGrassCoverErosionOutwardsHydraulicBoundaryLocationLookupTable USING(HydraulicLocationEntityId)
		JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity USING(GrassCoverErosionOutwardsHydraulicLocationEntityId)
		WHERE (ShouldDesignWaterLevelIllustrationPointsBeCalculated = 1 AND NormativeNormType = 2 AND gceofmme.HydraulicLocationCalculationCollectionEntity2Id = hlcce.HydraulicLocationCalculationCollectionEntityId)
		OR (ShouldDesignWaterLevelIllustrationPointsBeCalculated = 1 AND NormativeNormType = 1 AND gceofmme.HydraulicLocationCalculationCollectionEntity3Id = hlcce.HydraulicLocationCalculationCollectionEntityId)
		OR (ShouldWaveHeightIllustrationPointsBeCalculated = 1 AND NormativeNormType = 2 AND gceofmme.HydraulicLocationCalculationCollectionEntity5Id = hlcce.HydraulicLocationCalculationCollectionEntityId)
		OR (ShouldWaveHeightIllustrationPointsBeCalculated = 1 AND NormativeNormType = 1 AND gceofmme.HydraulicLocationCalculationCollectionEntity6Id = hlcce.HydraulicLocationCalculationCollectionEntityId)
);

-- Migrate the grass cover erosion outwards wave conditions calculations
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
	[CategoryType])
SELECT
	[GrassCoverErosionOutwardsWaveConditionsCalculationEntityId],
	[CalculationGroupEntityId],
	[ForeshoreProfileEntityId],
	lookup.HydraulicLocationEntityId,
	calc.'Order',
	calc.Name,
	calc.Comments,
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
	CASE
		WHEN [NormativeNormType] = 1
			THEN 3
		ELSE
			2
	END
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity calc
LEFT JOIN TempGrassCoverErosionOutwardsHydraulicBoundaryLocationLookupTable lookup USING(GrassCoverErosionOutwardsHydraulicLocationEntityId)
JOIN CalculationGroupEntity USING(CalculationGroupEntityId)
JOIN FailureMechanismEntity USING(CalculationGroupEntityId)
JOIN AssessmentSectionEntity USING(AssessmentSectionEntityId);

-- Migrate dune locations and associated data.
/*
Note, the following conventions are used for the calculation type on AssessmentSectionEntity:
- The dune calculations for the mechanism specific factorized signaling norm = 0.
- The dune calculations for the mechanism specific signaling norm = 1.
- The dune calculations for the mechanism specific lower limit norm = 2.
- The dune calculations for the lower limit norm = 3.
- The dune calculations for the factorized lower limit norm = 4.
*/
CREATE TEMP TABLE TempDuneCalculationTypes ( 'CalculationType' TINYINT (1) NOT NULL);
INSERT INTO TempDuneCalculationTypes VALUES (0);
INSERT INTO TempDuneCalculationTypes VALUES (1);
INSERT INTO TempDuneCalculationTypes VALUES (2);
INSERT INTO TempDuneCalculationTypes VALUES (3);
INSERT INTO TempDuneCalculationTypes VALUES (4);

-- Create the calculation collections
CREATE TEMP TABLE TempDuneLocationCalculationCollectionEntity
(
	'DuneLocationCalculationCollectionEntityId' INTEGER NOT NULL,
	'DuneErosionFailureMechanismMetaEntityId' INTEGER NOT NULL,
	'CalculationType' TINYINT (1) NOT NULL,
	PRIMARY KEY
	(
		'DuneLocationCalculationCollectionEntityId' AUTOINCREMENT
	)
);

INSERT INTO TempDuneLocationCalculationCollectionEntity (
	[DuneErosionFailureMechanismMetaEntityId],
	[CalculationType])
SELECT 
	[DuneErosionFailureMechanismMetaEntityId],
	[CalculationType]
FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity
JOIN (
	SELECT *
	FROM TempDuneCalculationTypes
);

INSERT INTO DuneLocationCalculationCollectionEntity(
	[DuneLocationCalculationCollectionEntityId])
SELECT 
	[DuneLocationCalculationCollectionEntityId]
FROM TempDuneLocationCalculationCollectionEntity;

-- Create the calculation entities
CREATE TEMP TABLE TempDuneLocationCalculationEntity
(
	'DuneLocationCalculationEntityId' INTEGER NOT NULL,
	'DuneLocationEntityId' INTEGER NOT NULL,
	'DuneErosionFailureMechanismMetaEntityId' INTEGER,
	'CalculationType' TINYINT (1) NOT NULL,
	PRIMARY KEY
	(
		'DuneLocationCalculationEntityId' AUTOINCREMENT
	)
);

INSERT INTO TempDuneLocationCalculationEntity(
	[DuneLocationEntityId],
	[DuneErosionFailureMechanismMetaEntityId],
	[CalculationType])
SELECT 
	[DuneLocationEntityId],
	[DuneErosionFailureMechanismMetaEntityId],
	[CalculationType]
FROM [SourceProject].FailureMechanismEntity 
JOIN [SourceProject].DuneLocationEntity USING(FailureMechanismEntityId)
JOIN [SourceProject].DuneErosionFailureMechanismMetaEntity USING(FailureMechanismEntityId)
JOIN (
	SELECT *
	FROM TempDuneCalculationTypes
);

INSERT INTO DuneLocationCalculationEntity(
	[DuneLocationCalculationEntityId],
	[DuneLocationEntityId],
	[DuneLocationCalculationCollectionEntityId])
SELECT
	[DuneLocationCalculationEntityId],
	[DuneLocationEntityId],
	[DuneLocationCalculationCollectionEntityId]
FROM TempDuneLocationCalculationCollectionEntity collections
JOIN TempDuneLocationCalculationEntity calculations 
ON collections.CalculationType = calculations.CalculationType
AND collections.DuneErosionFailureMechanismMetaEntityId = calculations.DuneErosionFailureMechanismMetaEntityId;

-- Migrate the dune erosion failure mechanism meta entity
INSERT INTO DuneErosionFailureMechanismMetaEntity (
	DuneErosionFailureMechanismMetaEntityId,
	FailureMechanismEntityId,
	DuneLocationCalculationCollectionEntity1Id,
	DuneLocationCalculationCollectionEntity2Id,
	DuneLocationCalculationCollectionEntity3Id,
	DuneLocationCalculationCollectionEntity4Id,
	DuneLocationCalculationCollectionEntity5Id,
	N
)
SELECT 
	[DuneErosionFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[CalculationsForMechanismSpecificFactorizedSignalingNorm],
	[CalculationsForMechanismSpecificSignalingNorm],
	[CalculationsForMechanismSpecificLowerLimitNorm],
	[CalculationsForLowerLimitNorm],
	[CalculationsForFactorizedLowerLimitNorm],
	[N]
FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity
JOIN (
	SELECT
		DuneLocationCalculationCollectionEntityId AS CalculationsForMechanismSpecificFactorizedSignalingNorm,
		DuneErosionFailureMechanismMetaEntityId
	FROM TempDuneLocationCalculationCollectionEntity WHERE CalculationType = 0
) USING (DuneErosionFailureMechanismMetaEntityId)
JOIN (
	SELECT
		DuneLocationCalculationCollectionEntityId AS CalculationsForMechanismSpecificSignalingNorm,
		DuneErosionFailureMechanismMetaEntityId
	FROM TempDuneLocationCalculationCollectionEntity WHERE CalculationType = 1
) USING (DuneErosionFailureMechanismMetaEntityId)
JOIN (
	SELECT
		DuneLocationCalculationCollectionEntityId AS CalculationsForMechanismSpecificLowerLimitNorm,
		DuneErosionFailureMechanismMetaEntityId
	FROM TempDuneLocationCalculationCollectionEntity WHERE CalculationType = 2
) USING (DuneErosionFailureMechanismMetaEntityId)
JOIN (
	SELECT
		DuneLocationCalculationCollectionEntityId AS CalculationsForLowerLimitNorm,
		DuneErosionFailureMechanismMetaEntityId
	FROM TempDuneLocationCalculationCollectionEntity WHERE CalculationType = 3
) USING (DuneErosionFailureMechanismMetaEntityId)
JOIN (
	SELECT
		DuneLocationCalculationCollectionEntityId AS CalculationsForFactorizedLowerLimitNorm,
		DuneErosionFailureMechanismMetaEntityId
	FROM TempDuneLocationCalculationCollectionEntity WHERE CalculationType = 4
) USING (DuneErosionFailureMechanismMetaEntityId);

-- Cleanup
DROP TABLE TempCalculationTypes;
DROP TABLE TempHydraulicLocationCalculationEntity;
DROP TABLE TempHydraulicLocationCalculationCollectionEntity;
DROP TABLE TempGrassCoverErosionOutwardsHydraulicBoundaryLocationLookupTable;

DROP TABLE TempDuneCalculationTypes;
DROP TABLE TempDuneLocationCalculationCollectionEntity;
DROP TABLE TempDuneLocationCalculationEntity;

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
VALUES ("17.3", "18.1", "Gevolgen van de migratie van versie 17.3 naar versie 18.1:");

CREATE TEMP TABLE TempLogOutputDeleted 
(
	'NrDeleted' INTEGER NOT NULL
);

INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].ClosingStructuresOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].DuneLocationOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsDikeHeightOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionInwardsOvertoppingRateOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].HeightStructuresOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].HydraulicLocationOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity;
INSERT INTO TempLogOutputDeleted
SELECT COUNT()
	FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity
	WHERE MacroStabilityInwardsCalculationEntityId IN (
		SELECT MacroStabilityInwardsCalculationEntityId
		FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity
		WHERE UseAssessmentLevelManualInput IS 0
	);
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
SELECT COUNT() + 
(
    SELECT COUNT()
    FROM [SOURCEPROJECT].PipingCalculationOutputEntity
    JOIN [SOURCEPROJECT].PipingCalculationEntity USING(PipingCalculationEntityId)
    WHERE UseAssessmentLevelManualInput = 1 
)
FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity
JOIN [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity USING(MacroStabilityInwardsCalculationEntityId)
WHERE UseAssessmentLevelManualInput = 1;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion],
	[ToVersion],
	[LogMessage])
SELECT
	"17.3",
	"18.1",
	CASE
		WHEN [NrRemaining] > 0
			THEN "* Alle berekende resultaten zijn verwijderd, behalve die van het toetsspoor 'Piping' en 'Macrostabiliteit binnenwaarts' waarbij de waterstand handmatig is ingevuld."
			ELSE "* Alle berekende resultaten zijn verwijderd."
	END
	FROM TempLogOutputDeleted
	LEFT JOIN TempLogOutputRemaining
	WHERE [NrDeleted] > 0
	LIMIT 1;

DROP TABLE TempLogOutputDeleted;
DROP TABLE TempLogOutputRemaining;

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
	JOIN FailureMechanismEntity USING(AssessmentSectionEntityId)
	JOIN TempFailureMechanisms USING(FailureMechanismType);

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
	"De waarde '" || source.[BelowPhreaticLevelDeviation] ||  "' voor de standaardafwijking van parameter 'Verzadigd gewicht' van ondergrondlaag '" || source.[MaterialName] || "' is ongeldig en is veranderd naar NaN."
	FROM PipingSoilLayerEntity as psl
	JOIN PipingSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN PipingStochasticSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN StochasticSoilModelEntity USING(StochasticSoilModelEntityId)
	JOIN [SOURCEPROJECT].PipingSoilLayerEntity AS source ON psl.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = [FailureMechanismEntityId]
	WHERE source.[BelowPhreaticLevelDeviation] IS NOT psl.[BelowPhreaticLevelDeviation];

INSERT INTO TempChanges
SELECT
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"De waarde '" || source.[BelowPhreaticLevelShift] ||  "' voor de verschuiving van parameter 'Verzadigd gewicht' van ondergrondlaag '" || source.[MaterialName] || "' is ongeldig en is veranderd naar NaN."
	FROM PipingSoilLayerEntity as psl
	JOIN PipingSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN PipingStochasticSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN StochasticSoilModelEntity USING(StochasticSoilModelEntityId)
	JOIN [SOURCEPROJECT].PipingSoilLayerEntity AS source ON psl.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = [FailureMechanismEntityId]
	WHERE source.[BelowPhreaticLevelShift] IS NOT psl.[BelowPhreaticLevelShift];

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
	"De waarde '" || source.[DiameterD70CoefficientOfVariation] || "' voor de variatieco" || CAST(X'c3ab' AS TEXT) || "ffici" || CAST(X'c3ab' AS TEXT) || "nt van parameter 'd70' van ondergrondlaag '" || source.[MaterialName] || "' is ongeldig en is veranderd naar NaN."
	FROM PipingSoilLayerEntity as psl
	JOIN PipingSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN PipingStochasticSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN StochasticSoilModelEntity USING(StochasticSoilModelEntityId)
	JOIN [SOURCEPROJECT].PipingSoilLayerEntity AS source ON psl.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = [FailureMechanismEntityId]
	WHERE source.[DiameterD70CoefficientOfVariation] IS NOT psl.[DiameterD70CoefficientOfVariation];

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

INSERT INTO TempChanges
SELECT
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"De waarde '" || source.[PermeabilityCoefficientOfVariation] || "' voor de variatieco" || CAST(X'c3ab' AS TEXT) || "ffici" || CAST(X'c3ab' AS TEXT) || "nt van parameter 'Doorlatendheid' van ondergrondlaag '" || source.[MaterialName] || "' is ongeldig en is veranderd naar NaN."
	FROM PipingSoilLayerEntity as psl
	JOIN PipingSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN PipingStochasticSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN StochasticSoilModelEntity USING(StochasticSoilModelEntityId)
	JOIN [SOURCEPROJECT].PipingSoilLayerEntity AS source ON psl.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = [FailureMechanismEntityId]
	WHERE source.[PermeabilityCoefficientOfVariation] IS NOT psl.[PermeabilityCoefficientOfVariation];

/*
* Log changed simple assessment results
*/
INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM ClosingStructuresSectionResultEntity as sre
	JOIN [SOURCEPROJECT].ClosingStructuresSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM GrassCoverErosionOutwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM GrassCoverSlipOffInwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM GrassCoverSlipOffOutwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM HeightStructuresSectionResultEntity as sre
	JOIN [SOURCEPROJECT].HeightStructuresSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM MacroStabilityInwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].MacroStabilityInwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM MacroStabilityOutwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].MacroStabilityOutwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM MicrostabilitySectionResultEntity as sre
	JOIN [SOURCEPROJECT].MicrostabilitySectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM PipingSectionResultEntity as sre
	JOIN [SOURCEPROJECT].PipingSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM PipingStructureSectionResultEntity as sre
	JOIN [SOURCEPROJECT].PipingStructureSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM StrengthStabilityLengthwiseConstructionSectionResultEntity as sre
	JOIN [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM TechnicalInnovationSectionResultEntity as sre
	JOIN [SOURCEPROJECT].TechnicalInnovationSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;
	
INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM WaterPressureAsphaltCoverSectionResultEntity as sre
	JOIN [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;
	
INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten met de waarde 'Voldoende / Niet relevant' voor de eenvoudige toets van dit toetsspoor zijn omgezet naar 'NVT'."
	FROM WaveImpactAsphaltCoverSectionResultEntity as sre
	JOIN [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerOne] = 2;

/*
* Log removed detailed assessment results
*/
INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM StabilityStoneCoverSectionResultEntity as sre
	JOIN [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerTwoA] != 1;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM WaveImpactAsphaltCoverSectionResultEntity as sre
	JOIN [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerTwoA] != 1;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM GrassCoverErosionOutwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerTwoA] != 1;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM DuneErosionSectionResultEntity as sre
	JOIN [SOURCEPROJECT].DuneErosionSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerTwoA] != 1;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM MicrostabilitySectionResultEntity as sre
	JOIN [SOURCEPROJECT].MicrostabilitySectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerTwoA] != 1;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM GrassCoverSlipOffOutwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerTwoA] != 1;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM GrassCoverSlipOffInwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerTwoA] != 1;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de gedetailleerde toets van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM PipingStructureSectionResultEntity as sre
	JOIN [SOURCEPROJECT].PipingStructureSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerTwoA] != 1;

/*
* Log removed tailor made assessment results
*/
INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM StabilityStoneCoverSectionResultEntity as sre
	JOIN [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM WaveImpactAsphaltCoverSectionResultEntity as sre
	JOIN [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM GrassCoverErosionOutwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM DuneErosionSectionResultEntity as sre
	JOIN [SOURCEPROJECT].DuneErosionSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM MacroStabilityOutwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].MacroStabilityOutwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM MicrostabilitySectionResultEntity as sre
	JOIN [SOURCEPROJECT].MicrostabilitySectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM WaterPressureAsphaltCoverSectionResultEntity as sre
	JOIN [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM GrassCoverSlipOffOutwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM GrassCoverSlipOffInwardsSectionResultEntity as sre
	JOIN [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM PipingStructureSectionResultEntity as sre
	JOIN [SOURCEPROJECT].PipingStructureSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM StrengthStabilityLengthwiseConstructionSectionResultEntity as sre
	JOIN [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Alle resultaten voor de toets op maat van dit toetsspoor konden niet worden omgezet naar een geldig resultaat en zijn verwijderd."
	FROM TechnicalInnovationSectionResultEntity as sre
	JOIN [SOURCEPROJECT].TechnicalInnovationSectionResultEntity AS source ON sre.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = sre.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"De waarde van '" || source.[ProbabilityOrFrequencyOpenStructureBeforeFlooding] || "' van parameter 'Kans op open staan bij naderend hoogwater' van kunstwerk '" || source.[Name] || "' is ongeldig en is veranderd naar NaN."
	FROM ClosingStructureEntity AS cs
	JOIN [SOURCEPROJECT].ClosingStructureEntity AS source ON cs.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = cs.[FailureMechanismEntityId]
	WHERE source.[ProbabilityOrFrequencyOpenStructureBeforeFlooding] IS NOT cs.[ProbabilityOpenStructureBeforeFlooding];

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"De waarde van '" || source.[IdenticalApertures] || "' van parameter 'Aantal identieke doorstroomopeningen' van kunstwerk '" || source.[Name] || "' is ongeldig en is veranderd naar 1."
	FROM ClosingStructureEntity AS cs
	JOIN [SOURCEPROJECT].ClosingStructureEntity AS source ON cs.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = cs.[FailureMechanismEntityId]
	WHERE source.[IdenticalApertures] != cs.[IdenticalApertures];

/*
* Log changes that are applied to closing and stability point structure calculations
*/
CREATE TEMP TABLE TempMigratedStructuresCalculationGroupMapping
(
	'CalculationGroupEntityId' INTEGER NOT NULL,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	PRIMARY KEY
	(
		'CalculationGroupEntityId'
	)
);

INSERT INTO TempMigratedStructuresCalculationGroupMapping(
	[CalculationGroupEntityId],
	[FailureMechanismEntityId]
)
SELECT
	[CalculationGroupEntityId],
	[FailureMechanismEntityId]
FROM
(
WITH RECURSIVE CalculationGroupEntities(CalculationGroupEntityId, ParentCalculationGroupEntityId, FailureMechanismEntityId) AS (
SELECT
	CalculationGroupEntityId,
	NULL,
	FailureMechanismEntityId
FROM FailureMechanismEntity
WHERE FailureMechanismEntity.FailureMechanismType = 10
OR FailureMechanismEntity.FailureMechanismType = 12
UNION ALL
SELECT
	CalculationGroupEntity.CalculationGroupEntityId,
	CalculationGroupEntity.ParentCalculationGroupEntityId,
	CalculationGroupEntities.FailureMechanismEntityId
FROM CalculationGroupEntity, CalculationGroupEntities
WHERE CalculationGroupEntity.ParentCalculationGroupEntityId = CalculationGroupEntities.CalculationGroupEntityId
)
SELECT
	CalculationGroupEntities.CalculationGroupEntityId AS CalculationGroupEntityId,
	CalculationGroupEntities.FailureMechanismEntityId AS FailureMechanismEntityId
FROM CalculationGroupEntities
ORDER BY FailureMechanismEntityId
);

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"De waarde van '" || source.[IdenticalApertures] || "' van parameter 'Aantal identieke doorstroomopeningen' van berekening '" || source.[Name] || "' is ongeldig en is veranderd naar 1."
	FROM ClosingStructuresCalculationEntity AS cs
	JOIN [SOURCEPROJECT].ClosingStructuresCalculationEntity AS source ON cs.[rowid] = source.[rowid]
	JOIN TempMigratedStructuresCalculationGroupMapping mapping USING(CalculationGroupEntityId)
 	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = mapping.[FailureMechanismEntityId]
	WHERE source.[IdenticalApertures] != cs.[IdenticalApertures];

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"De waarde van '" || source.[LevellingCount] || "' van parameter 'Aantal nivelleringen per jaar' van berekening '" || source.[Name] || "' is ongeldig en is veranderd naar 0."
	FROM StabilityPointStructuresCalculationEntity AS sps
	JOIN [SOURCEPROJECT].StabilityPointStructuresCalculationEntity AS source ON sps.[rowid] = source.[rowid]
	JOIN TempMigratedStructuresCalculationGroupMapping mapping USING(CalculationGroupEntityId)
 	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = mapping.[FailureMechanismEntityId]
	WHERE source.[LevellingCount] != sps.[LevellingCount];

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"De waarde van '" || source.[VerticalDistance] || "' van parameter 'Afstand onderkant wand en teen van de dijk/berm' van berekening '" || source.[Name] || "' is ongeldig en is veranderd naar NaN."
	FROM StabilityPointStructuresCalculationEntity AS sps
	JOIN [SOURCEPROJECT].StabilityPointStructuresCalculationEntity AS source ON sps.[rowid] = source.[rowid]
	JOIN TempMigratedStructuresCalculationGroupMapping mapping USING(CalculationGroupEntityId)
 	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = mapping.[FailureMechanismEntityId]
	WHERE source.[VerticalDistance] IS NOT sps.[VerticalDistance];

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
	"17.3", 
	"18.1",
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
DROP TABLE TempMigratedStructuresCalculationGroupMapping;
DROP TABLE TempChanges;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion], 
	[ToVersion], 
	[LogMessage])
SELECT "17.3",
	"18.1", 
	"* Geen aanpassingen."
	WHERE (
		SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
		WHERE [FromVersion] = "17.3"
	) IS 1;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;