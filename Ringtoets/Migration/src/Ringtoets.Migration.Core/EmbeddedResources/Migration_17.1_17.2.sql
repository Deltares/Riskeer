/*
Migration script for migrating Ringtoets databases.
SourceProject version: 17.1
TargetProject version: 17.2
*/
PRAGMA foreign_keys = OFF;

ATTACH DATABASE "{0}" AS SOURCEPROJECT;

CREATE TEMP TABLE TempAssessmentReturnPeriod
(
	[AssessmentSectionId] TEXT,
	[SignalingReturnPeriod] INTEGER NOT NULL,
	[LowerLimitReturnPeriod] INTEGER NOT NULL
);

INSERT INTO TempAssessmentReturnPeriod VALUES 
	('1-1', 1000, 1000),
	('1-2', 1000, 1000),
	('2-1', 1000, 300),
	('2-2', 1000, 1000),
	('3-1', 3000, 3000),
	('3-2', 1000, 1000),
	('4-1', 300, 300),
	('4-2', 1000, 300),
	('5-1', 3000, 1000),
	('5-2', 3000, 3000),
	('6-1', 3000, 1000),
	('6-2', 3000, 1000),
	('6-3', 3000, 1000),
	('6-4', 3000, 1000),
	('6-5', 3000, 1000),
	('6-6', 3000, 1000),
	('6-7', 10000, 3000),
	('7-1', 3000, 1000),
	('7-2', 3000, 1000),
	('8-1', 30000, 10000),
	('8-2', 30000, 10000),
	('8-3', 30000, 10000),
	('8-4', 30000, 10000),
	('8-5', 3000, 1000),
	('8-6', 3000, 1000),
	('8-7', 3000, 1000),
	('9-1', 1000, 300),
	('9-2', 3000, 1000),
	('10-1', 3000, 1000),
	('10-2', 3000, 1000),
	('10-3', 10000, 3000),
	('11-1', 3000, 1000),
	('11-2', 3000, 1000),
	('11-3', 300, 100),
	('12-1', 1000, 1000),
	('12-2', 3000, 1000),
	('13-1', 3000, 1000),
	('13-2', 3000, 3000),
	('13-3', 3000, 1000),
	('13-4', 3000, 1000),
	('13-5', 3000, 1000),
	('13-6', 3000, 1000),
	('13-7', 3000, 1000),
	('13-8', 3000, 1000),
	('13-9', 3000, 1000),
	('13a-1', 300, 100),
	('13b-1', 300, 100),
	('14-1', 30000, 10000),
	('14-10', 30000, 30000),
	('14-2', 100000, 30000),
	('14-3', 10000, 10000),
	('14-4', 10000, 3000),
	('14-5', 30000, 10000),
	('14-6', 30000, 10000),
	('14-7', 30000, 10000),
	('14-8', 30000, 10000),
	('14-9', 30000, 30000),
	('15-1', 30000, 10000),
	('15-2', 10000, 3000),
	('15-3', 10000, 3000),
	('16-1', 100000, 30000),
	('16-2', 30000, 10000),
	('16-3', 30000, 10000),
	('16-4', 30000, 10000),
	('16-5', 10, 10), -- Signaling return period set to LowerLimit
	('17-1', 3000, 1000),
	('17-2', 3000, 1000),
	('17-3', 100000, 30000),
	('18-1', 10000, 3000),
	('19-1', 100000, 30000),
	('20-1', 30000, 10000),
	('20-2', 10000, 10000),
	('20-3', 30000, 10000),
	('20-4', 1000, 300),
	('21-1', 3000, 1000),
	('21-2', 300, 100),
	('22-1', 3000, 1000),
	('22-2', 10000, 3000),
	('23-1', 3000, 1000),
	('24-1', 10000, 3000),
	('24-2', 1000, 300),
	('24-3', 10000, 10000),
	('25-1', 3000, 1000),
	('25-2', 1000, 300),
	('25-3', 300, 100),
	('25-4', 300, 300),
	('26-1', 3000, 1000),
	('26-2', 3000, 1000),
	('26-3', 10000, 3000),
	('26-4', 1000, 1000),
	('27-1', 3000, 3000),
	('27-2', 10000, 10000),
	('27-3', 3000, 1000),
	('27-4', 1000, 300),
	('28-1', 1000, 300),
	('29-1', 3000, 1000),
	('29-2', 10000, 3000),
	('29-3', 100000, 30000),
	('29-4', 1000, 1000),
	('30-1', 3000, 1000),
	('30-2', 100000, 100000),
	('30-3', 3000, 1000),
	('30-4', 1000000, 1000000),
	('31-1', 30000, 10000),
	('31-2', 10000, 3000),
	('31-3', 300, 100),
	('32-1', 1000, 300),
	('32-2', 1000, 300),
	('32-3', 3000, 1000),
	('32-4', 3000, 1000),
	('33-1', 300, 100),
	('34-1', 1000, 300),
	('34-2', 1000, 300),
	('34-3', 3000, 1000),
	('34-4', 1000, 300),
	('34-5', 300, 100),
	('34a-1', 3000, 1000),
	('35-1', 10000, 3000),
	('35-2', 3000, 1000),
	('36-1', 10000, 3000),
	('36-2', 30000, 10000),
	('36-3', 30000, 10000),
	('36-4', 10000, 3000),
	('36-5', 10000, 3000),
	('36a-1', 3000, 1000),
	('37-1', 10000, 3000),
	('38-1', 30000, 10000),
	('38-2', 10000, 3000),
	('39-1', 3000, 3000),
	('40-1', 30000, 30000),
	('40-2', 10000, 3000),
	('41-1', 30000, 10000),
	('41-2', 10000, 3000),
	('41-3', 3000, 3000),
	('41-4', 10000, 3000),
	('42-1', 10000, 3000),
	('43-1', 30000, 10000),
	('43-2', 10000, 3000),
	('43-3', 30000, 10000),
	('43-4', 30000, 10000),
	('43-5', 30000, 10000),
	('43-6', 30000, 10000),
	('44-1', 30000, 10000),
	('44-2', 300, 100),
	('44-3', 30000, 10000),
	('45-1', 100000, 30000),
	('45-2', 300, 100),
	('45-3', 300, 100),
	('46-1', 300, 100),
	('47-1', 3000, 1000),
	('48-1', 30000, 10000),
	('48-2', 10000, 3000),
	('48-3', 10000, 3000),
	('49-1', 300, 100),
	('49-2', 10000, 3000),
	('50-1', 30000, 10000),
	('50-2', 3000, 1000),
	('51-1', 1000, 300),
	('52-1', 3000, 1000),
	('52-2', 3000, 1000),
	('52-3', 3000, 1000),
	('52-4', 3000, 1000),
	('52a-1', 3000, 1000),
	('53-1', 3000, 1000),
	('53-2', 10000, 3000),
	('53-3', 10000, 3000),
	('54-1', 1000, 300),
	('55-1', 1000, 300),
	('56-1', 300, 100),
	('57-1', 300, 100),
	('58-1', 300, 100),
	('59-1', 300, 100),
	('60-1', 300, 100),
	('61-1', 300, 100),
	('63-1', 300, 100),
	('64-1', 300, 100),
	('65-1', 300, 100),
	('66-1', 300, 100),
	('67-1', 300, 100),
	('68-1', 1000, 300),
	('68-2', 300, 100),
	('69-1', 1000, 300),
	('70-1', 300, 100),
	('71-1', 300, 100),
	('72-1', 300, 100),
	('73-1', 300, 100),
	('74-1', 300, 100),
	('75-1', 300, 100),
	('76-1', 300, 100),
	('76-2', 300, 100),
	('76a-1', 300, 100),
	('77-1', 300, 100),
	('78-1', 300, 100),
	('78a-1', 300, 100),
	('79-1', 300, 100),
	('80-1', 300, 100),
	('81-1', 300, 100),
	('82-1', 300, 100),
	('83-1', 300, 100),
	('85-1', 300, 100),
	('86-1', 300, 100),
	('87-1', 1000, 300),
	('88-1', 300, 100),
	('89-1', 300, 100),
	('90-1', 3000, 1000),
	('91-1', 300, 300),
	('92-1', 300, 100),
	('93-1', 1000, 300),
	('94-1', 300, 100),
	('95-1', 300, 100),
	('201', 10000, 3000),
	('202', 10000, 3000),
	('204a', 10000, 3000),
	('204b', 1000, 300),
	('205', 3000, 1000),
	('206', 10000, 3000),
	('208', 100000, 30000),
	('209', 100000, 30000),
	('210', 100000, 30000),
	('211', 3000, 1000),
	('212', 10000, 3000),
	('213', 10000, 3000),
	('214', 3000, 1000),
	('215', 30000, 10000),
	('216', 3000, 1000),
	('217', 30000, 10000),
	('218', 30000, 10000),
	('219', 30000, 10000),
	('221', 10000, 3000),
	('222', 30000, 10000),
	('223', 30000, 10000),
	('224', 30000, 10000),
	('225', 30000, 10000),
	('226', 3000, 1000),
	('227', 3000, 1000);

INSERT INTO AssessmentSectionEntity (
	[AssessmentSectionEntityId],
	[ProjectEntityId],
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
	CASE
		WHEN [Id] IS NULL
			THEN ""
		ELSE [Id]
	END,
	[Name],
	[Comments],
	CASE
		WHEN SourceAssessmentSection.[LowerLimitReturnPeriod] IS NULL OR [NormativeNormType] == 1 -- If lower limit norm type
			THEN SourceAssessmentSection.[Norm]
		ELSE
			1.0 / SourceAssessmentSection.[LowerLimitReturnPeriod]
	END,
	CASE
		WHEN SourceAssessmentSection.[SignalingReturnPeriod] IS NULL OR [NormativeNormType] == 2 -- If signaling norm type
			THEN SourceAssessmentSection.[Norm]
		ELSE
			1.0 / SourceAssessmentSection.[SignalingReturnPeriod]
	END,
	[NormativeNormType],
	[HydraulicDatabaseVersion],
	[HydraulicDatabaseLocation],
	[Composition],
	[ReferenceLinePointXml],
	[Order]
FROM
(
	SELECT
		CASE
			WHEN [SignalingReturnPeriod] IS NULL OR OriginalReturnPeriod BETWEEN ([SignalingReturnPeriod] - 0.1) AND ([SignalingReturnPeriod] + 0.1) OR
			(
				OriginalReturnPeriod NOT BETWEEN ([SignalingReturnPeriod] - 0.1) AND ([SignalingReturnPeriod] + 0.1) 
				AND OriginalReturnPeriod NOT BETWEEN ([LowerLimitReturnPeriod] - 0.1) AND ([LowerLimitReturnPeriod] + 0.1) 
				AND OriginalReturnPeriod > [LowerLimitReturnPeriod]
			)
				THEN 2 -- Set signaling norm type
		ELSE 1 -- Set lower limit norm type
		END AS [NormativeNormType],
		*
	FROM
	(
		SELECT CAST(1.0 / ASE.[Norm] AS FLOAT) AS OriginalReturnPeriod,
		*
		FROM [SOURCEPROJECT].AssessmentSectionEntity ASE
		LEFT JOIN TempAssessmentReturnPeriod TA ON 
		(
			TA.[AssessmentSectionId] = ASE.[Id]
		)
	)
) AS SourceAssessmentSection;
INSERT INTO BackgroundDataEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataEntity;
INSERT INTO BackgroundDataMetaEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataMetaEntity;
INSERT INTO CalculationGroupEntity SELECT * FROM [SOURCEPROJECT].CalculationGroupEntity;
INSERT INTO PipingCharacteristicPointEntity (
	[PipingCharacteristicPointEntityId],
	[SurfaceLineEntityId],
	[Type],
	[X],
	[Y],
	[Z])
SELECT
	[CharacteristicPointEntityId],
	[SurfaceLineEntityId],
	[Type],
	[X],
	[Y],
	[Z]
	FROM [SOURCEPROJECT].CharacteristicPointEntity;
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
	[ProbabilityOrFrequencyOpenStructureBeforeFlooding],
	[FailureProbabilityOpenStructure],
	[IdenticalApertures],
	[FailureProbabilityReparation],
	[InflowModelType])
SELECT
	[ClosingStructureEntityId],
	[FailureMechanismEntityId],
	[Order],
	[Name],
	CASE 
		WHEN Suffix 
			THEN [Id] || SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength) || Suffix 
		ELSE [Id] 
	END AS [Id],
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
	[ProbabilityOrFrequencyOpenStructureBeforeFlooding],
	[FailureProbabilityOpenStructure],
	[IdenticalApertures],
	[FailureProbabilityReparation],
	[InflowModelType]
	FROM (
		SELECT *,
		MaxLength - LENGTH([Id]) AS SuffixPreLength,
		(
			SELECT CS.rowid
			FROM [SOURCEPROJECT].ClosingStructureEntity
			WHERE CS.[ClosingStructureEntityId] > [ClosingStructureEntityId]
			AND CS.[Id] IS [Id]
			AND CS.[FailuremechanismEntityId] = [FailuremechanismEntityId]
		) AS Suffix
	FROM [SOURCEPROJECT].ClosingStructureEntity CS
	JOIN (
		SELECT MAX(LENGTH([Id])) AS MaxLength
		FROM [SOURCEPROJECT].ClosingStructureEntity
		)
	);
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
	[ProbabilityOrFrequencyOpenStructureBeforeFlooding],
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
	[IdenticalApertures],
	[FailureProbabilityReparation],
	[InflowModelType],
	[FailureProbabilityStructureWithErosion],
	[DeviationWaveDirection],
	[DrainCoefficientMean],
	[ModelFactorSuperCriticalFlowMean],
	[StormDurationMean],
	[FactorStormDurationOpenStructure],
	0
	FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity;
INSERT INTO ClosingStructuresFailureMechanismMetaEntity (
	[ClosingStructuresFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N2A],
	[ClosingStructureCollectionSourcePath],
	[ForeshoreProfileCollectionSourcePath])
SELECT
	[ClosingStructuresFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N2A],
	CASE
		WHEN COUNT([ClosingStructureEntityId])
			THEN ""
		ELSE NULL
	END,
	CASE
		WHEN COUNT([ForeshoreProfileEntityId])
			THEN ""
		ELSE NULL
	END
	FROM [SOURCEPROJECT].ClosingStructuresFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].ClosingStructureEntity USING(FailureMechanismEntityId)
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityID;
INSERT INTO ClosingStructuresSectionResultEntity (
	[ClosingStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[ClosingStructuresCalculationEntityId],
	[LayerOne],
	[LayerThree])
SELECT
	[ClosingStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[ClosingStructuresCalculationEntityId],
	[LayerOne],
	CASE
		WHEN [LayerThree] > 1 OR [LayerThree] < 0 
			THEN NULL
		ELSE
			[LayerThree]
	END
	FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity;
INSERT INTO DikeProfileEntity SELECT * FROM [SOURCEPROJECT].DikeProfileEntity;
INSERT INTO DuneErosionFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity;
INSERT INTO DuneErosionSectionResultEntity SELECT * FROM [SOURCEPROJECT].DuneErosionSectionResultEntity;
INSERT INTO DuneLocationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationEntity;
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
	CASE
		WHEN Suffix
			THEN [Id] || SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength) || Suffix
		ELSE [Id]
	END AS [Id],
	[Name],
	[Orientation],
	[BreakWaterType],
	[BreakWaterHeight],
	[GeometryXml],
	[X],
	[Y],
	[X0],
	[Order]
	FROM (
		SELECT *,
		MaxLength - LENGTH(Id) AS SuffixPreLength,
		(
			SELECT FS.rowid
			FROM [SOURCEPROJECT].ForeshoreProfileEntity
			WHERE FS.ForeshoreProfileEntityId > ForeshoreProfileEntityId
			AND FS.Id IS Id
			AND FS.FailuremechanismEntityId = FailuremechanismEntityId
		) AS Suffix
	FROM [SOURCEPROJECT].ForeshoreProfileEntity FS
	JOIN (
		SELECT MAX(LENGTH(Id)) AS MaxLength 
		FROM [SOURCEPROJECT].ForeshoreProfileEntity
		)
	);
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
	[OvertoppingRateCalculationType],
	[ShouldDikeHeightIllustrationPointsBeCalculated],
	[ShouldOvertoppingRateIllustrationPointsBeCalculated],
	[ShouldOvertoppingOutputIllustrationPointsBeCalculated])
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
	0,
	0,
	0
	FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity;
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionInwardsSectionResultEntity (
	[GrassCoverErosionInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[GrassCoverErosionInwardsCalculationEntityId],
	[LayerOne],
	[LayerThree])
SELECT 
	[GrassCoverErosionInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[GrassCoverErosionInwardsCalculationEntityId],
	[LayerOne],
	CASE
		WHEN [LayerThree] > 1 OR [LayerThree] < 0 
			THEN NULL
		ELSE
			[LayerThree]
	END
	FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity (
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	[ForeshoreProfileCollectionSourcePath])
SELECT
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	CASE
		WHEN COUNT([ForeshoreProfileEntityId])
			THEN ""
		ELSE NULL
	END
	FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityID;
INSERT INTO GrassCoverErosionOutwardsHydraulicLocationEntity (
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[FailureMechanismEntityId],
	[LocationId],
	[Name],
	[LocationX],
	[LocationY],
	[ShouldWaveHeightIllustrationPointsBeCalculated],
	[ShouldDesignWaterLevelIllustrationPointsBeCalculated],
	[Order])
SELECT
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[FailureMechanismEntityId],
	[LocationId],
	[Name],
	[LocationX],
	[LocationY],
	0,
	0,
	[Order]
	FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity;
INSERT INTO GrassCoverErosionOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
INSERT INTO GrassCoverSlipOffInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity;
INSERT INTO GrassCoverSlipOffOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity;
INSERT INTO HeightStructureEntity (
	[HeightStructureEntityId],
	[FailureMechanismEntityId],
	[Order],
	[Name],
	[Id],
	[X],
	[Y],
	[StructureNormalOrientation],
	[LevelCrestStructureMean],
	[LevelCrestStructureStandardDeviation],
	[FlowWidthAtBottomProtectionMean],
	[FlowWidthAtBottomProtectionStandardDeviation],
	[CriticalOvertoppingDischargeMean],
	[CriticalOvertoppingDischargeCoefficientOfVariation],
	[WidthFlowAperturesMean],
	[WidthFlowAperturesStandardDeviation],
	[FailureProbabilityStructureWithErosion],
	[StorageStructureAreaMean],
	[StorageStructureAreaCoefficientOfVariation],
	[AllowedLevelIncreaseStorageMean],
	[AllowedLevelIncreaseStorageStandardDeviation])
SELECT
	[HeightStructureEntityId],
	[FailureMechanismEntityId],
	[Order],
	[Name],
	CASE
		WHEN Suffix
			THEN [Id] || SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength)|| Suffix
		ELSE [Id]
	END AS [Id],
	[X],
	[Y],
	[StructureNormalOrientation],
	[LevelCrestStructureMean],
	[LevelCrestStructureStandardDeviation],
	[FlowWidthAtBottomProtectionMean],
	[FlowWidthAtBottomProtectionStandardDeviation],
	[CriticalOvertoppingDischargeMean],
	[CriticalOvertoppingDischargeCoefficientOfVariation],
	[WidthFlowAperturesMean],
	[WidthFlowAperturesStandardDeviation],
	[FailureProbabilityStructureWithErosion],
	[StorageStructureAreaMean],
	[StorageStructureAreaCoefficientOfVariation],
	[AllowedLevelIncreaseStorageMean],
	[AllowedLevelIncreaseStorageStandardDeviation]
	FROM (
		SELECT *, 
		MaxLength - LENGTH([Id]) AS SuffixPreLength, (
			SELECT HS.rowid
			FROM [SOURCEPROJECT].HeightStructureEntity
			WHERE HS.[HeightStructureEntityId] > [HeightStructureEntityId]
			AND HS.[Id] IS [Id]
			AND HS.[FailuremechanismEntityId] = [FailuremechanismEntityId]
		) AS Suffix
		FROM [SOURCEPROJECT].HeightStructureEntity HS
		JOIN (
			SELECT MAX(LENGTH([Id])) AS MaxLength 
			FROM [SOURCEPROJECT].HeightStructureEntity
		)
	);
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
	0
FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
INSERT INTO HeightStructuresFailureMechanismMetaEntity (
	[HeightStructuresFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	[HeightStructureCollectionSourcePath],
	[ForeshoreProfileCollectionSourcePath])
SELECT
	[HeightStructuresFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	CASE
		WHEN COUNT([HeightStructureEntityId])
			THEN ""
		ELSE NULL
	END,
	CASE
		WHEN COUNT([ForeshoreProfileEntityId])
			THEN ""
		ELSE NULL
	END
	FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].HeightStructureEntity USING(FailureMechanismEntityId)
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING(FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityID;
INSERT INTO HeightStructuresSectionResultEntity (
	[HeightStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[HeightStructuresCalculationEntityId],
	[LayerOne],
	[LayerThree])
SELECT
	[HeightStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[HeightStructuresCalculationEntityId],
	[LayerOne],
	CASE
		WHEN [LayerThree] > 1 OR [LayerThree] < 0 
			THEN NULL
		ELSE
			[LayerThree]
	END
	FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity;
INSERT INTO HydraulicLocationEntity (
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	[LocationId],
	[Name],
	[LocationX],
	[LocationY],
	[ShouldWaveHeightIllustrationPointsBeCalculated],
	[ShouldDesignWaterLevelIllustrationPointsBeCalculated],
	[Order])
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	[LocationId],
	[Name],
	[LocationX],
	[LocationY],
	0,
	0,
	[Order]
	FROM [SOURCEPROJECT].HydraulicLocationEntity;
INSERT INTO MacroStabilityInwardsSectionResultEntity (
	[MacroStabilityInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[LayerOne],
	[LayerThree])
SELECT
	[MacrostabilityInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[LayerOne],
	CASE
		WHEN [LayerThree] > 1 OR [LayerThree] < 0 
			THEN NULL
		ELSE
			[LayerThree]
	END
	FROM [SOURCEPROJECT].MacrostabilityInwardsSectionResultEntity;
INSERT INTO MacrostabilityOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].MacrostabilityOutwardsSectionResultEntity;
INSERT INTO MicrostabilitySectionResultEntity SELECT * FROM [SOURCEPROJECT].MicrostabilitySectionResultEntity;
INSERT INTO PipingCalculationEntity (
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
	[ScenarioContribution],
	[AssessmentLevel],
	[UseAssessmentLevelManualInput])
SELECT
	[PipingCalculationEntityId],
	[CalculationGroupEntityId],
	[SurfaceLineEntityId],
	[StochasticSoilProfileEntityId],
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
	[UseAssessmentLevelManualInput]
	FROM [SOURCEPROJECT].PipingCalculationEntity;
INSERT INTO PipingCalculationOutputEntity
SELECT *
	FROM [SOURCEPROJECT].PipingCalculationOutputEntity
	WHERE PipingCalculationEntityId IN (
		SELECT PipingCalculationEntityId 
		FROM [SOURCEPROJECT].PipingCalculationEntity
		WHERE UseAssessmentLevelManualInput IS 1
	);
INSERT INTO PipingFailureMechanismMetaEntity (
	[PipingFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[A],
	[WaterVolumetricWeight],
	[StochasticSoilModelCollectionSourcePath],
	[SurfaceLineCollectionSourcePath]) 
SELECT
	[PipingFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[A],
	[WaterVolumetricWeight], 
	[StochasticSoilModelSourcePath],
	[SurfaceLineSourcePath] 
	FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingSectionResultEntity (
	[PipingSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[LayerOne],
	[LayerThree])
SELECT
	[PipingSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[LayerOne],
	CASE
		WHEN [LayerThree] > 1 OR [LayerThree] < 0 
			THEN NULL
		ELSE
			[LayerThree]
	END
	FROM [SOURCEPROJECT].PipingSectionResultEntity;
INSERT INTO PipingSemiProbabilisticOutputEntity
SELECT *
	FROM [SOURCEPROJECT].PipingSemiProbabilisticOutputEntity
	WHERE PipingCalculationEntityId IN (
		SELECT PipingCalculationEntityId
		FROM [SOURCEPROJECT].PipingCalculationEntity
		WHERE UseAssessmentLevelManualInput IS 1
	);
INSERT INTO PipingStructureSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingStructureSectionResultEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO PipingSoilLayerEntity (
	[PipingSoilLayerEntityId],
	[PipingSoilProfileEntityId],
	[Top],
	[IsAquifer],
	[Color],
	[MaterialName],
	[BelowPhreaticLevelMean],
	[BelowPhreaticLevelDeviation],
	[BelowPhreaticLevelShift],
	[DiameterD70Mean],
	[DiameterD70CoefficientOfVariation],
	[PermeabilityMean],
	[PermeabilityCoefficientOfVariation],
	[Order])
SELECT
	[SoilLayerEntityId],
	[SoilProfileEntityId],
	[Top],
	[IsAquifer],
	CASE
		WHEN [Color] IS 0
			THEN NULL
		ELSE
			[Color]
	END,
	[MaterialName],
	[BelowPhreaticLevelMean],
	[BelowPhreaticLevelDeviation],
	[BelowPhreaticLevelShift],
	[DiameterD70Mean],
	[DiameterD70CoefficientOfVariation],
	[PermeabilityMean],
	[PermeabilityCoefficientOfVariation],
	[Order]
	FROM [SOURCEPROJECT].SoilLayerEntity;
INSERT INTO PipingSoilProfileEntity (
	[PipingSoilProfileEntityId],
	[Bottom],
	[Name],
	[SourceType])
SELECT
	[SoilProfileEntityId],
	[Bottom],
	CASE
		WHEN [Name] IS NULL
			THEN ""
		ELSE
			[Name]
	END,
	(
		SELECT SSP.[Type]
		FROM [SOURCEPROJECT].StochasticSoilProfileEntity SSP
		WHERE SSP.SoilProfileEntityId IS SP.SoilProfileEntityId
		LIMIT 1
	) AS [SourceType]
	FROM [SOURCEPROJECT].SoilProfileEntity SP;
INSERT INTO StabilityPointStructureEntity (
	[StabilityPointStructureEntityId],
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
	[InflowModelType])
SELECT
	[StabilityPointStructureEntityId],
	[FailureMechanismEntityId],
	[Order],
	[Name],
	CASE
		WHEN Suffix
			THEN [Id] || SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength) || Suffix
		ELSE [Id]
	END AS [Id],
	[X],
	[Y],
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
	[InflowModelType]
	FROM (
		SELECT *, 
		MaxLength - LENGTH([Id]) AS SuffixPreLength, 
		(
			SELECT SPS.rowid
			FROM [SOURCEPROJECT].StabilityPointStructureEntity
			WHERE SPS.[StabilityPointStructureEntityId] > [StabilityPointStructureEntityId]
			AND SPS.[Id] IS [Id]
			AND SPS.[FailuremechanismEntityId] = [FailuremechanismEntityId]
		) AS Suffix
		FROM [SOURCEPROJECT].StabilityPointStructureEntity SPS
		JOIN (
			SELECT MAX(LENGTH([Id])) AS MaxLength 
			FROM [SOURCEPROJECT].StabilityPointStructureEntity
		)
	);
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
	[ModelFactorSuperCriticalFlowMean],
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
	[ModelFactorSuperCriticalFlowMean],
	[FactorStormDurationOpenStructure],
	[DrainCoefficientMean],
	[FailureProbabilityStructureWithErosion],
	0
FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity (
	[StabilityPointStructuresFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	[ForeshoreProfileCollectionSourcePath],
	[StabilityPointStructureCollectionSourcePath])
SELECT 
	[StrengthStabilityPointConstructionFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	CASE
		WHEN COUNT([ForeshoreProfileEntityId])
			THEN ""
		ELSE NULL
	END,
	CASE 
		WHEN COUNT([StabilityPointStructureEntityId])
			THEN ""
		ELSE NULL
	END
	FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityId)
	LEFT JOIN [SOURCEPROJECT].StabilityPointStructureEntity USING (FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityId;
INSERT INTO StabilityPointStructuresSectionResultEntity (
	[StabilityPointStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[StabilityPointStructuresCalculationEntityId],
	[LayerOne],
	[LayerThree])
SELECT 
	[StabilityPointStructuresSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[StabilityPointStructuresCalculationEntityId],
	[LayerOne],
	CASE
		WHEN [LayerThree] > 1 OR [LayerThree] < 0 
			THEN NULL
		ELSE
			[LayerThree]
	END
	FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity;
INSERT INTO StabilityStoneCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity (
	[FailureMechanismEntityId],
	[ForeshoreProfileCollectionSourcePath])
SELECT
	[FailureMechanismEntityId],
	CASE
		WHEN COUNT([ForeshoreProfileEntityId])
			THEN ""
		ELSE NULL
	END
	FROM [SOURCEPROJECT].FailureMechanismEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityID)
	WHERE FailureMechanismType = 7
	GROUP BY FailureMechanismEntityID;
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
	[StochasticSoilModelSegmentPointXml],
	[Order]
	FROM [SOURCEPROJECT].StochasticSoilModelEntity;
INSERT INTO PipingStochasticSoilProfileEntity (
	[PipingStochasticSoilProfileEntityId],
	[PipingSoilProfileEntityId],
	[StochasticSoilModelEntityId],
	[Probability],
	[Order])
SELECT
	[StochasticSoilProfileEntityId],
	[SoilProfileEntityId],
	[StochasticSoilModelEntityId],
	[Probability],
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
	[FailureMechanismEntityId],
	CASE
		WHEN [Name] IS NULL
			THEN ""
		ELSE
			[Name]
	END,
	[ReferenceLineIntersectionX],
	[ReferenceLineIntersectionY],
	[PointsXml],
	[Order]
	FROM [SOURCEPROJECT].SurfaceLineEntity;
INSERT INTO TechnicalInnovationSectionResultEntity SELECT * FROM [SOURCEPROJECT].TechnicalInnovationSectionResultEntity;
INSERT INTO VersionEntity (
	[VersionId],
	[Version],
	[Timestamp],
	[FingerPrint])
SELECT [VersionId],
	"17.2",
	[Timestamp],
	[FingerPrint]
	FROM [SOURCEPROJECT].VersionEntity;
INSERT INTO WaterPressureAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity;
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity (
	[FailureMechanismEntityId],
	[ForeshoreProfileCollectionSourcePath])
SELECT
	[FailureMechanismEntityId],
	CASE
		WHEN COUNT([ForeshoreProfileEntityId])
			THEN ""
			ELSE NULL
	END
	FROM [SOURCEPROJECT].FailureMechanismEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityID)
	WHERE FailureMechanismType = 3
	GROUP BY FailureMechanismEntityID;

/*
Outputs that used HydraRing are not migrated
*/
-- ClosingStructuresOutputEntity
-- DuneLocationOutputEntity
-- GrassCoverErosionInwardsDikeHeightOutputEntity
-- GrassCoverErosionInwardsOutputEntity
-- GrassCoverErosionInwardsOvertoppingRateOutputEntity
-- GrassCoverErosionOutwardsHydraulicLocationOutputEntity
-- GrassCoverErosionOutwardsWaveConditionsOutputEntity
-- HeightStructuresOutputEntity
-- HydraulicLocationOutputEntity
-- StabilityPointStructuresOutputEntity
-- StabilityStoneCoverWaveConditionsOutputEntity
-- WaveImpactAsphaltCoverWaveConditionsOutputEntity

/*
Insert new data
*/
INSERT INTO MacroStabilityInwardsFailureMechanismMetaEntity (
	[FailureMechanismEntityId],
	[A])
SELECT FailureMechanismEntityId,
	0.033
	FROM FailureMechanismEntity WHERE FailureMechanismType = 2;

-- Insert new groups
INSERT INTO CalculationGroupEntity (
	[Name],
	[Order])
SELECT 
	"Berekeningen",
	0
	FROM FailureMechanismEntity
	WHERE FailureMechanismType = 2;

-- Create temp table to store the new calculation group ids
CREATE TEMP TABLE TempCalculationGroupEntity
(
 'CalculationGroupId' INTEGER NOT NULL,
 'FailureMechanismId' INTEGER NOT NULL,
 PRIMARY KEY ('CalculationGroupId', 'FailureMechanismId')
) WITHOUT ROWID;

-- Store the new calculation group ids
INSERT INTO TempCalculationGroupEntity (
	[CalculationGroupId],
	[FailureMechanismId])
SELECT
	last_insert_rowid() - (
		SELECT COUNT() 
		FROM AssessmentSectionEntity ase 
		WHERE fme.AssessmentSectionEntityId >= ase.AssessmentSectionEntityId
	) + 1,
	FailureMechanismEntityId
	FROM FailureMechanismEntity fme
	WHERE fme.FailureMechanismType = 2
	ORDER BY AssessmentSectionEntityId;

-- Link groups to all Macro (2) failure mechanisms
UPDATE FailureMechanismEntity
	SET CalculationGroupEntityId = (
		SELECT CalculationGroupId 
		FROM TempCalculationGroupEntity
		WHERE FailureMechanismId = FailureMechanismEntity.FailureMechanismEntityId
	)
	WHERE FailureMechanismType = 2;

-- Cleanup
DROP TABLE TempCalculationGroupEntity;

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
VALUES ("17.1", "17.2", "Gevolgen van de migratie van versie 17.1 naar versie 17.2:");

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
	FROM [SOURCEPROJECT].PipingSemiProbabilisticOutputEntity
	WHERE PipingCalculationEntityId IN (
		SELECT PipingCalculationEntityId
		FROM [SOURCEPROJECT].PipingCalculationEntity
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
SELECT COUNT()
	FROM [SOURCEPROJECT].PipingSemiProbabilisticOutputEntity
	WHERE PipingCalculationEntityId IN (
		SELECT PipingCalculationEntityId
		FROM [SOURCEPROJECT].PipingCalculationEntity
		WHERE UseAssessmentLevelManualInput IS 1
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
	"17.1",
	"17.2",
	CASE
		WHEN [NrRemaining] > 0
			THEN "* Alle berekende resultaten zijn verwijderd, behalve die van het toetsspoor 'Piping' waarbij het toetspeil handmatig is ingevuld."
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

CREATE TEMP TABLE TempAssessmentSectionChanges
(
	[AssessmentSectionId],
	[AssessmentSectionName],
	[Order],
	[msg]
);

INSERT INTO TempAssessmentSectionChanges
SELECT
	[AssessmentSectionEntityId],
	[Name],
	0,
	"De ondergrens is gelijk gesteld aan 1/" || CAST(ROUND(CAST(1.0 / [LowerLimitNorm] AS FLOAT)) AS INT) ||
	CASE 
		WHEN [NormativeNormType] IS 1
			THEN " (voorheen de waarde van de norm)"
		ELSE ""
	END || "."
	FROM AssessmentSectionEntity

UNION

SELECT
	[AssessmentSectionEntityId],
	[Name],
	1,
	"De signaleringswaarde is gelijk gesteld aan 1/" || CAST(ROUND(CAST(1.0 / [SignalingNorm] AS FLOAT)) AS INT) ||
	CASE 
		WHEN [NormativeNormType] IS 2
			THEN " (voorheen de waarde van de norm)"
		ELSE ""
	END || "."
	FROM AssessmentSectionEntity

UNION

SELECT
	[AssessmentSectionEntityId],
	[Name],
	2,
	"De norm van het dijktraject is gelijk gesteld aan de " ||
	CASE 
		WHEN [NormativeNormType] IS 1
			THEN "ondergrens"
		ELSE "signaleringswaarde"
	END || "."
	FROM AssessmentSectionEntity;

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
	"Het ID van voorlandprofiel '" || source.[Id] || "' is veranderd naar '" || fp.[Id] || "'."
	FROM ForeshoreProfileEntity AS fp
	JOIN [SOURCEPROJECT].ForeshoreProfileEntity AS source ON fp.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fp.[FailureMechanismEntityId]
	WHERE source.[Id] IS NOT fp.[Id];

INSERT INTO TempChanges
SELECT
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Het ID van kunstwerk '" || source.[Id] || "' is veranderd naar '" || hs.[Id] || "'."
	FROM HeightStructureEntity AS hs
	JOIN [SOURCEPROJECT].HeightStructureEntity AS source ON hs.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = hs.[FailureMechanismEntityId]
	WHERE source.[Id] IS NOT hs.[Id];

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Het ID van kunstwerk '" || source.[Id] || "' is veranderd naar '" || cs.[Id] || "'."
	FROM ClosingStructureEntity AS cs
	JOIN [SOURCEPROJECT].ClosingStructureEntity AS source ON cs.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = cs.[FailureMechanismEntityId]
	WHERE source.[Id] IS NOT cs.[Id];

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Het ID van kunstwerk '" || source.[Id] || "' is veranderd naar '" || sps.[Id] || "'."
	FROM StabilityPointStructureEntity AS sps
	JOIN [SOURCEPROJECT].StabilityPointStructureEntity AS source ON sps.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = sps.[FailureMechanismEntityId]
	WHERE source.[Id] IS NOT sps.[Id];

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Het geregistreerde resultaat voor de toets op maat in '" || fms.[Name] || "' ('" || source.[LayerThree] || "') kon niet worden geconverteerd naar een geldige kans en is verwijderd."
	FROM PipingSectionResultEntity as psr
	JOIN [SOURCEPROJECT].PipingSectionResultEntity AS source ON psr.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = psr.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL AND psr.[LayerThree] IS NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Het geregistreerde resultaat voor de toets op maat in '" || fms.[Name] || "' ('" || source.[LayerThree] || "') kon niet worden geconverteerd naar een geldige kans en is verwijderd."
	FROM GrassCoverErosionInwardsSectionResultEntity as gceisr
	JOIN [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity AS source ON gceisr.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = gceisr.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL AND gceisr.[LayerThree] IS NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Het geregistreerde resultaat voor de toets op maat in '" || fms.[Name] || "' ('" || source.[LayerThree] || "') kon niet worden geconverteerd naar een geldige kans en is verwijderd."
	FROM ClosingStructuresSectionResultEntity as cssr
	JOIN [SOURCEPROJECT].ClosingStructuresSectionResultEntity AS source ON cssr.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = cssr.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL AND cssr.[LayerThree] IS NULL;
	
INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Het geregistreerde resultaat voor de toets op maat in '" || fms.[Name] || "' ('" || source.[LayerThree] || "') kon niet worden geconverteerd naar een geldige kans en is verwijderd."
	FROM HeightStructuresSectionResultEntity as hssr
	JOIN [SOURCEPROJECT].HeightStructuresSectionResultEntity AS source ON hssr.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = hssr.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL AND hssr.[LayerThree] IS NULL;
	
INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Het geregistreerde resultaat voor de toets op maat in '" || fms.[Name] || "' ('" || source.[LayerThree] || "') kon niet worden geconverteerd naar een geldige kans en is verwijderd."
	FROM StabilityPointStructuresSectionResultEntity as spssr
	JOIN [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity AS source ON spssr.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = spssr.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL AND spssr.[LayerThree] IS NULL;

INSERT INTO TempChanges
SELECT 
	asfm.[AssessmentSectionId],
	asfm.[AssessmentSectionName],
	asfm.[FailureMechanismId],
	asfm.[FailureMechanismName],
	"Het geregistreerde resultaat voor de toets op maat in '" || fms.[Name] || "' ('" || source.[LayerThree] || "') kon niet worden geconverteerd naar een geldige kans en is verwijderd."
	FROM MacroStabilityInwardsSectionResultEntity as msisr
	JOIN [SOURCEPROJECT].MacroStabilityInwardsSectionResultEntity AS source ON msisr.[rowid] = source.[rowid]
	JOIN FailureMechanismSectionEntity as fms ON fms.[FailureMechanismSectionEntityId] = msisr.[FailureMechanismSectionEntityId]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = fms.[FailureMechanismEntityId]
	WHERE source.[LayerThree] IS NOT NULL AND msisr.[LayerThree] IS NULL;

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
	"17.1",
	"17.2",
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

DROP TABLE TempAssessmentSectionChanges;
DROP TABLE TempFailureMechanisms;
DROP TABLE TempAssessmentReturnPeriod;
DROP TABLE TempAssessmentSectionFailureMechanism;
DROP TABLE TempChanges;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
	[FromVersion], 
	[ToVersion], 
	[LogMessage])
SELECT "17.1",
	"17.2", 
	"* Geen aanpassingen."
	WHERE (
		SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
		WHERE [FromVersion] = "17.1"
	) IS 1;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;