/*
Migration script for migrating Ringtoets databases.
SourceProject version: 17.1
TargetProject version: 17.2
*/
PRAGMA foreign_keys = OFF;

ATTACH DATABASE [{0}] AS SOURCEPROJECT;

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
	FROM (SELECT *, MaxLength - LENGTH([Id]) AS SuffixPreLength, (SELECT count(*)
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
	CASE WHEN COUNT([ClosingStructureEntityId]) THEN "Onbekend" ELSE NULL END,
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "Onbekend" ELSE NULL END
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
	CASE WHEN Suffix THEN [Name] || '(' || Suffix || ')' ELSE [Name] END as [Name],
	[Orientation],
	[BreakWaterType],
	[BreakWaterHeight],
	[GeometryXml],
	[X],
	[Y],
	[X0],
	[Order]
	FROM (SELECT *, MaxLength - LENGTH(NAME) as SuffixPreLength, (SELECT count(*)
                     FROM [SOURCEPROJECT].ForeshoreProfileEntity
                     WHERE FS.ForeshoreProfileEntityId > ForeshoreProfileEntityId
                     AND FS.Name IS Name
                     AND FS.FailuremechanismEntityId = FailuremechanismEntityId) as Suffix
	FROM [SOURCEPROJECT].ForeshoreProfileEntity FS
	JOIN (SELECT MAX(LENGTH(Name)) as MaxLength FROM [SOURCEPROJECT].ForeshoreProfileEntity));
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
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "Onbekend" ELSE NULL END
	FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityID;
INSERT INTO GrassCoverErosionOutwardsHydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity;
INSERT INTO GrassCoverErosionOutwardsHydraulicLocationOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity;
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
	FROM (SELECT *, MaxLength - LENGTH([Id]) AS SuffixPreLength, (SELECT count(*)
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
	CASE WHEN COUNT([HeightStructureEntityId]) THEN "Onbekend" ELSE NULL END,
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "Onbekend" ELSE NULL END
	FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].HeightStructureEntity USING (FailureMechanismEntityId)
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityId)
	GROUP BY FailureMechanismEntityID;
INSERT INTO HeightStructuresOutputEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresOutputEntity;
INSERT INTO HeightStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity;
INSERT INTO HydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationEntity;
INSERT INTO HydraulicLocationOutputEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationOutputEntity;
INSERT INTO MacrostabilityInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].MacrostabilityInwardsSectionResultEntity;
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
	FROM (SELECT *, MaxLength - LENGTH([Id]) AS SuffixPreLength, (SELECT count(*)
                     FROM [SOURCEPROJECT].StabilityPointStructureEntity
                     WHERE SPS.[StabilityPointStructureEntityId] > [StabilityPointStructureEntityId]
                     AND SPS.[Id] IS [Id]
                     AND SPS.[FailuremechanismEntityId] = [FailuremechanismEntityId]) AS Suffix
	FROM [SOURCEPROJECT].StabilityPointStructureEntity SPS
	JOIN (SELECT MAX(LENGTH([Id])) AS MaxLength FROM [SOURCEPROJECT].StabilityPointStructureEntity));
SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity (
	[StabilityPointStructuresFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	[ForeshoreProfileCollectionSourcePath])
SELECT 
	[StrengthStabilityPointConstructionFailureMechanismMetaEntityId],
	[FailureMechanismEntityId],
	[N],
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "Onbekend" ELSE NULL END
	FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityId)
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
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "Onbekend" ELSE NULL END
	FROM [SOURCEPROJECT].FailureMechanismEntity
	LEFT JOIN [SOURCEPROJECT].ForeshoreProfileEntity USING (FailureMechanismEntityID)
	WHERE FailureMechanismType = 7
	GROUP BY FailureMechanismEntityID;
INSERT INTO StochasticSoilModelEntity SELECT * FROM [SOURCEPROJECT].StochasticSoilModelEntity;
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
	CASE WHEN COUNT([ForeshoreProfileEntityId]) THEN "Onbekend" ELSE NULL END
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
ATTACH DATABASE [{1}] AS LOGDATABASE;

CREATE TEMP TABLE log_output_deleted (
	'NrDeleted' INTEGER NOT NULL
);

CREATE TABLE  IF NOT EXISTS [LOGDATABASE].'MigrationLogEntity' 
(
	'MigrationLogEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
	'FromVersion' VARCHAR(20) NOT NULL, 
	'ToVersion' VARCHAR(20) NOT NULL, 
	'LogMessage' TEXT NOT NULL
);

DROP TABLE log_output_deleted;

DETACH LOGDATABASE;

DETACH SOURCEPROJECT;

PRAGMA foreign_keys = ON;