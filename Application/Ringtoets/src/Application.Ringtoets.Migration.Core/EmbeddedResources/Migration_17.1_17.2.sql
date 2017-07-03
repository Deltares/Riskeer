/*
Migration script for migrating Ringtoets databases.
SourceProject version: 17.1
TargetProject version: 17.2
*/
PRAGMA foreign_keys = OFF;

ATTACH DATABASE '{0}' AS SOURCEPROJECT;

INSERT INTO AssessmentSectionEntity SELECT * FROM [SOURCEPROJECT].AssessmentSectionEntity;
INSERT INTO BackgroundDataEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataEntity;
INSERT INTO BackgroundDataMetaEntity SELECT * FROM [SOURCEPROJECT].BackgroundDataMetaEntity;
INSERT INTO CalculationGroupEntity SELECT * FROM [SOURCEPROJECT].CalculationGroupEntity;
INSERT INTO CharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].CharacteristicPointEntity;
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
	CASE WHEN Suffix THEN [Id] || 
	SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength)
	|| Suffix ELSE [Id] END as [Id],
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
	FROM (SELECT *, MaxLength - LENGTH([Id]) AS SuffixPreLength, (SELECT CS.rowid
                     FROM [SOURCEPROJECT].ClosingStructureEntity
                     WHERE CS.[ClosingStructureEntityId] > [ClosingStructureEntityId]
                     AND CS.[Id] IS [Id]
                     AND CS.[FailuremechanismEntityId] = [FailuremechanismEntityId]) AS Suffix
	FROM [SOURCEPROJECT].ClosingStructureEntity CS
	JOIN (SELECT MAX(LENGTH([Id])) AS MaxLength FROM [SOURCEPROJECT].ClosingStructureEntity));
INSERT INTO ClosingStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity;
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
	CASE WHEN COUNT([ClosingStructureEntityId]) THEN "" ELSE NULL END,
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "" ELSE NULL END
	FROM [SOURCEPROJECT].ClosingStructuresFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].ClosingStructureEntity USING (FailureMechanismEntityId)
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityID;
INSERT INTO ClosingStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresOutputEntity;
INSERT INTO ClosingStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity;
INSERT INTO DikeProfileEntity SELECT * FROM [SOURCEPROJECT].DikeProfileEntity;
INSERT INTO DuneErosionFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity;
INSERT INTO DuneErosionSectionResultEntity SELECT * FROM [SOURCEPROJECT].DuneErosionSectionResultEntity;
INSERT INTO DuneLocationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationEntity;
INSERT INTO DuneLocationOutputEntity SELECT * FROM [SOURCEPROJECT].DuneLocationOutputEntity;
INSERT INTO FailureMechanismEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismEntity;
INSERT INTO FailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismSectionEntity;
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
	CASE WHEN Suffix THEN [Id] || 
	SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength)
	|| Suffix ELSE [Id] END as [Id],
	[Name],
	[Orientation],
	[BreakWaterType],
	[BreakWaterHeight],
	[GeometryXml],
	[X],
	[Y],
	[X0],
	[Order]
	FROM (SELECT *, MaxLength - LENGTH(Id) as SuffixPreLength, (SELECT FS.rowid
                     FROM [SOURCEPROJECT].ForeshoreProfileEntity
                     WHERE FS.ForeshoreProfileEntityId > ForeshoreProfileEntityId
                     AND FS.Id IS Id
                     AND FS.FailuremechanismEntityId = FailuremechanismEntityId) as Suffix
	FROM [SOURCEPROJECT].ForeshoreProfileEntity FS
	JOIN (SELECT MAX(LENGTH(Id)) as MaxLength FROM [SOURCEPROJECT].ForeshoreProfileEntity));
INSERT INTO GrassCoverErosionInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity;
INSERT INTO GrassCoverErosionInwardsDikeHeightOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsDikeHeightOutputEntity;
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionInwardsOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity;
INSERT INTO GrassCoverErosionInwardsOvertoppingRateOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsOvertoppingRateOutputEntity;
INSERT INTO GrassCoverErosionInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity (
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	[ForeshoreProfileCollectionSourcePath])
SELECT 
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "" ELSE NULL END
	FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityID;
INSERT INTO GrassCoverErosionOutwardsHydraulicLocationEntity(
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[FailureMechanismEntityId],
	[LocationId],
	[Name],
	[LocationX],
	[LocationY],
	[ShouldWaveHeightIllustrationPointsBeCalculated],
	[ShouldWaterLevelIllustrationPointsBeCalculated],
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
INSERT INTO GrassCoverErosionOutwardsHydraulicLocationOutputEntity(
	[GrassCoverErosionOutwardsHydraulicLocationOutputEntityId],
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[HydraulicLocationOutputType],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence])
SELECT
	[GrassCoverErosionOutwardsHydraulicLocationOutputEntityId],
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	NULL,
	[HydraulicLocationOutputType],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
	FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity;
INSERT INTO GrassCoverErosionOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity;
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
	CASE WHEN Suffix THEN [Id] || 
	SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength)
	|| Suffix ELSE [Id] END as [Id],
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
	FROM (SELECT *, MaxLength - LENGTH([Id]) AS SuffixPreLength, (SELECT HS.rowid
                     FROM [SOURCEPROJECT].HeightStructureEntity
                     WHERE HS.[HeightStructureEntityId] > [HeightStructureEntityId]
                     AND HS.[Id] IS [Id]
                     AND HS.[FailuremechanismEntityId] = [FailuremechanismEntityId]) AS Suffix
	FROM [SOURCEPROJECT].HeightStructureEntity HS
	JOIN (SELECT MAX(LENGTH([Id])) AS MaxLength FROM [SOURCEPROJECT].HeightStructureEntity));
INSERT INTO HeightStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
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
	CASE WHEN COUNT([HeightStructureEntityId]) THEN "" ELSE NULL END,
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "" ELSE NULL END
	FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].HeightStructureEntity USING (FailureMechanismEntityId)
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityID;
INSERT INTO HeightStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresOutputEntity;
INSERT INTO HeightStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity;
INSERT INTO HydraulicLocationEntity (
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	[LocationId],
	[Name],
	[LocationX],
	[LocationY],
	[ShouldWaveHeightIllustrationPointsBeCalculated],
	[ShouldWaterLevelIllustrationPointsBeCalculated],
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
INSERT INTO HydraulicLocationOutputEntity(
	[HydraulicLocationEntityOutputId],
	[HydraulicLocationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[HydraulicLocationOutputType],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence])
SELECT 
	[HydraulicLocationEntityOutputId],
	[HydraulicLocationEntityId],
	NULL,
	[HydraulicLocationOutputType],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
	FROM [SOURCEPROJECT].HydraulicLocationOutputEntity;
INSERT INTO MacroStabilityInwardsSectionResultEntity (
	[MacroStabilityInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[LayerOne],
	[LayerTwoA],
	[LayerThree])
SELECT
	[MacrostabilityInwardsSectionResultEntityId],
	[FailureMechanismSectionEntityId],
	[LayerOne],
	[LayerTwoA],
	[LayerThree]
	FROM [SOURCEPROJECT].MacrostabilityInwardsSectionResultEntity;
INSERT INTO MacrostabilityOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].MacrostabilityOutwardsSectionResultEntity;
INSERT INTO MicrostabilitySectionResultEntity SELECT * FROM [SOURCEPROJECT].MicrostabilitySectionResultEntity;
INSERT INTO PipingCalculationEntity SELECT * FROM [SOURCEPROJECT].PipingCalculationEntity;
INSERT INTO PipingCalculationOutputEntity SELECT * FROM [SOURCEPROJECT].PipingCalculationOutputEntity;
INSERT INTO PipingFailureMechanismMetaEntity (
	[PipingFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[A],
	[WaterVolumetricWeight],
	[StochasticSoilModelCollectionSourcePath],
	[SurfacelineCollectionSourcePath]) 
SELECT 
	[PipingFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[A],
	[WaterVolumetricWeight], 
	[StochasticSoilModelSourcePath],
	[SurfacelineSourcePath] 
	FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingSectionResultEntity;
INSERT INTO PipingSemiProbabilisticOutputEntity SELECT * FROM [SOURCEPROJECT].PipingSemiProbabilisticOutputEntity;
INSERT INTO PipingStructureSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingStructureSectionResultEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO SoilLayerEntity SELECT * FROM [SOURCEPROJECT].SoilLayerEntity;
INSERT INTO SoilProfileEntity SELECT * FROM [SOURCEPROJECT].SoilProfileEntity;
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
	CASE WHEN Suffix THEN [Id] || 
	SUBSTR(QUOTE(ZEROBLOB((SuffixPreLength + 1) / 2)), 3, SuffixPreLength)
	|| Suffix ELSE [Id] END as [Id],
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
	FROM (SELECT *, MaxLength - LENGTH([Id]) AS SuffixPreLength, (SELECT SPS.rowid
                     FROM [SOURCEPROJECT].StabilityPointStructureEntity
                     WHERE SPS.[StabilityPointStructureEntityId] > [StabilityPointStructureEntityId]
                     AND SPS.[Id] IS [Id]
                     AND SPS.[FailuremechanismEntityId] = [FailuremechanismEntityId]) AS Suffix
	FROM [SOURCEPROJECT].StabilityPointStructureEntity SPS
	JOIN (SELECT MAX(LENGTH([Id])) AS MaxLength FROM [SOURCEPROJECT].StabilityPointStructureEntity));
INSERT INTO StabilityPointStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
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
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "" ELSE NULL END,
	CASE WHEN COUNT([StabilityPointStructureEntityId]) THEN "" ELSE NULL END
	FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityId)
	LEFT JOIN [SOURCEPROJECT].StabilityPointStructureEntity USING (FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityId;
INSERT INTO StabilityPointStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity;
INSERT INTO StabilityPointStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity;
INSERT INTO StabilityStoneCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverSectionResultEntity;
INSERT INTO StabilityStoneCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsCalculationEntity;
INSERT INTO StabilityStoneCoverWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].StabilityStoneCoverWaveConditionsOutputEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity (
	[FailureMechanismEntityId],
	[ForeshoreProfileCollectionSourcePath])
SELECT 
	[FailureMechanismEntityId],
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "" ELSE NULL END
	FROM [SOURCEPROJECT].FailureMechanismEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityID)
	WHERE FailureMechanismType = 7
	GROUP BY FailureMechanismEntityID;
INSERT INTO StochasticSoilModelEntity(
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
INSERT INTO StochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].StochasticSoilProfileEntity;
INSERT INTO StrengthStabilityLengthwiseConstructionSectionResultEntity SELECT * FROM [SOURCEPROJECT].StrengthStabilityLengthwiseConstructionSectionResultEntity;
INSERT INTO SurfaceLineEntity SELECT * FROM [SOURCEPROJECT].SurfaceLineEntity;
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
INSERT INTO WaveImpactAsphaltCoverWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity;
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity  (
	[FailureMechanismEntityId],
	[ForeshoreProfileCollectionSourcePath])
SELECT 
	[FailureMechanismEntityId],
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "" ELSE NULL END
	FROM [SOURCEPROJECT].FailureMechanismEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityID)
	WHERE FailureMechanismType = 3
	GROUP BY FailureMechanismEntityID;
/*
Insert new data
*/

/* 
Write migration logging 
*/
ATTACH DATABASE '{1}' AS LOGDATABASE;

CREATE TABLE  IF NOT EXISTS [LOGDATABASE].'MigrationLogEntity' 
(
	'MigrationLogEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
	'FromVersion' VARCHAR(20) NOT NULL, 
	'ToVersion' VARCHAR(20) NOT NULL, 
	'LogMessage' TEXT NOT NULL
);
	
INSERT INTO [LOGDATABASE].MigrationLogEntity(
		[FromVersion], 
		[ToVersion], 
		[LogMessage])
VALUES ("17.1", "17.2", "Gevolgen van de migratie van versie 17.1 naar versie 17.2:");

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
	FROM FailureMechanismMessages as fmm
	WHERE fmm.[AssessmentSectionId] IS [AssessmentSectionId]
	ORDER BY 1, 3, 6
)
SELECT 
	"17.1",
	"17.2",
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