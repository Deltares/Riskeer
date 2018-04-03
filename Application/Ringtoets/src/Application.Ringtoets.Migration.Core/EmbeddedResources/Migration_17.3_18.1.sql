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
INSERT INTO ClosingStructureEntity SELECT * FROM [SOURCEPROJECT].ClosingStructureEntity;
INSERT INTO ClosingStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresCalculationEntity;
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
FROM [SOURCEPROJECT].ClosingStructuresOutputEntity;
INSERT INTO ClosingStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].ClosingStructuresSectionResultEntity;
INSERT INTO DikeProfileEntity SELECT * FROM [SOURCEPROJECT].DikeProfileEntity;
INSERT INTO DuneErosionFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity;
INSERT INTO DuneErosionSectionResultEntity SELECT * FROM [SOURCEPROJECT].DuneErosionSectionResultEntity;
INSERT INTO DuneLocationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationEntity;
INSERT INTO DuneLocationOutputEntity SELECT * FROM [SOURCEPROJECT].DuneLocationOutputEntity;
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
INSERT INTO GrassCoverErosionInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity;
INSERT INTO GrassCoverErosionInwardsDikeHeightOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsDikeHeightOutputEntity;
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionInwardsOutputEntity(
	[GrassCoverErosionInwardsOutputEntityId],
	[GrassCoverErosionInwardsCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Order],
	[IsOvertoppingDominant],
	[WaveHeight],
	[Reliability])
SELECT 
	[GrassCoverErosionInwardsOutputEntityId],
	[GrassCoverErosionInwardsCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Order],
	[IsOvertoppingDominant],
	[WaveHeight],
	[Reliability]
FROM [SOURCEPROJECT].GrassCoverErosionInwardsOutputEntity;
INSERT INTO GrassCoverErosionInwardsOvertoppingRateOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsOvertoppingRateOutputEntity;
INSERT INTO GrassCoverErosionInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsSectionResultEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
INSERT INTO GrassCoverErosionOutwardsWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionOutwardsWaveConditionsOutputEntity;
INSERT INTO GrassCoverSlipOffInwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffInwardsSectionResultEntity;
INSERT INTO GrassCoverSlipOffOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].GrassCoverSlipOffOutwardsSectionResultEntity;
INSERT INTO HeightStructureEntity SELECT * FROM [SOURCEPROJECT].HeightStructureEntity;
INSERT INTO HeightStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
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
FROM [SOURCEPROJECT].HeightStructuresOutputEntity;
INSERT INTO HeightStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresSectionResultEntity;
INSERT INTO HydraRingPreprocessorEntity SELECT * FROM [SOURCEPROJECT].HydraRingPreprocessorEntity;
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
INSERT INTO MacrostabilityOutwardsSectionResultEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityOutwardsSectionResultEntity;
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
INSERT INTO PipingStructureSectionResultEntity SELECT * FROM [SOURCEPROJECT].PipingStructureSectionResultEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO StabilityPointStructureEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityPointStructuresOutputEntity(
	[StabilityPointStructuresOutputEntityId],
	[StabilityPointStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability])
SELECT 
	[StabilityPointStructuresOutputEntity],
	[StabilityPointStructuresCalculationEntityId],
	[GeneralResultFaultTreeIllustrationPointEntityId],
	[Reliability]
FROM [SOURCEPROJECT].StabilityPointStructuresOutputEntity;
INSERT INTO StabilityPointStructuresSectionResultEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresSectionResultEntity;
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
SELECT [VersionId],
	"18.1",
	[Timestamp],
	[FingerPrint]
FROM [SOURCEPROJECT].VersionEntity;
INSERT INTO WaterPressureAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaterPressureAsphaltCoverSectionResultEntity;
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
INSERT INTO WaveImpactAsphaltCoverSectionResultEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverSectionResultEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsCalculationEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsCalculationEntity;
INSERT INTO WaveImpactAsphaltCoverWaveConditionsOutputEntity SELECT * FROM [SOURCEPROJECT].WaveImpactAsphaltCoverWaveConditionsOutputEntity;

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

-- Migrate the hydraulic boundary location calculations on assessment section level
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
-- UNION ALL is used to repeate the operation for the calculations eight times
INSERT INTO TempHydraulicLocationCalculationEntity (
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	[CalculationType])
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	0
FROM HydraulicLocationEntity
UNION ALL
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	1
FROM HydraulicLocationEntity
UNION ALL
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	2
FROM HydraulicLocationEntity
UNION ALL
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	3
FROM HydraulicLocationEntity
UNION ALL
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	4
FROM HydraulicLocationEntity
UNION ALL
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	5
FROM HydraulicLocationEntity
UNION ALL
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	6
FROM HydraulicLocationEntity
UNION ALL
SELECT
	[HydraulicLocationEntityId],
	[AssessmentSectionEntityId],
	7
FROM HydraulicLocationEntity;

-- Create the calculations for the Hydraulic Boundary Locations on Grass Cover Erosion Outwards Failure Mechanism level
-- Note: it is assumed that the HBL entities of the grass cover erosion outwards failure mechanism have the same PK value
-- as the HBL entities on assessment section level.
-- Otherwise, joints will have to be used for each
INSERT INTO TempHydraulicLocationCalculationEntity (
	[HydraulicLocationEntityId],
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[CalculationType])
SELECT
	GrassCoverErosionOutwardsHydraulicLocationEntityId,
	GrassCoverErosionOutwardsFailureMechanismMetaEntityId,
	8
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity
JOIN [SOURCEPROJECT].FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity USING (FailureMechanismEntityId)

UNION ALL

SELECT
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	9
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity
JOIN [SOURCEPROJECT].FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity USING (FailureMechanismEntityId)

UNION ALL

SELECT
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	10
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity
JOIN [SOURCEPROJECT].FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity USING (FailureMechanismEntityId)

UNION ALL

SELECT
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	11
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity
JOIN [SOURCEPROJECT].FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity USING (FailureMechanismEntityId)

UNION ALL

SELECT
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	12
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity
JOIN [SOURCEPROJECT].FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity USING (FailureMechanismEntityId)

UNION ALL

SELECT
	[GrassCoverErosionOutwardsHydraulicLocationEntityId],
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	13
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity
JOIN [SOURCEPROJECT].FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity USING (FailureMechanismEntityId);

INSERT INTO HydraulicLocationCalculationEntity (
	[HydraulicLocationCalculationEntityId],
	[HydraulicLocationEntityId],
	[ShouldIllustrationPointsBeCalculated])
SELECT
	[HydraulicLocationCalculationEntityId],
	[HydraulicLocationEntityId],
	0
FROM TempHydraulicLocationCalculationEntity;

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
	0
FROM [SOURCEPROJECT].AssessmentSectionEntity
UNION ALL
SELECT
	[AssessmentSectionEntityId],
	1
FROM [SOURCEPROJECT].AssessmentSectionEntity
UNION ALL
SELECT
	[AssessmentSectionEntityId],
	2
FROM [SOURCEPROJECT].AssessmentSectionEntity
UNION ALL
SELECT
	[AssessmentSectionEntityId],
	3
FROM [SOURCEPROJECT].AssessmentSectionEntity
UNION ALL
SELECT
	[AssessmentSectionEntityId],
	4
FROM [SOURCEPROJECT].AssessmentSectionEntity
UNION ALL
SELECT
	[AssessmentSectionEntityId],
	5
FROM [SOURCEPROJECT].AssessmentSectionEntity
UNION ALL
SELECT
	[AssessmentSectionEntityId],
	6
FROM [SOURCEPROJECT].AssessmentSectionEntity
UNION ALL
SELECT
	[AssessmentSectionEntityId],
	7
FROM [SOURCEPROJECT].AssessmentSectionEntity;

INSERT INTO TempHydraulicLocationCalculationCollectionEntity (
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	[CalculationType])
SELECT
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	8
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity

UNION ALL

SELECT
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	9
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity

UNION ALL

SELECT
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	10
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity

UNION ALL

SELECT
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	11
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity

UNION ALL

SELECT
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	12
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity

UNION ALL

SELECT
	[GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
	13
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity;

INSERT INTO HydraulicLocationCalculationCollectionEntity (
	[HydraulicLocationCalculationCollectionEntityId])
SELECT
	[HydraulicLocationCalculationCollectionEntityId]
FROM TempHydraulicLocationCalculationCollectionEntity;

-- Map the calculations into the collections and add them to the association table
INSERT INTO HydraulicLocationCalculationCollectionToHydraulicCalculationEntity (
	[HydraulicLocationCalculationCollectionEntityId],
	[HydraulicLocationCalculationEntityId])
SELECT
	[HydraulicLocationCalculationCollectionEntityId],
	[HydraulicLocationCalculationEntityId]
FROM TempHydraulicLocationCalculationEntity calculationTable
JOIN TempHydraulicLocationCalculationCollectionEntity calculationCollectionTable
ON calculationTable.AssessmentSectionEntityId = calculationCollectionTable.AssessmentSectionEntityId
AND calculationTable.CalculationType = calculationCollectionTable.CalculationType;

INSERT INTO HydraulicLocationCalculationCollectionToHydraulicCalculationEntity (
	[HydraulicLocationCalculationCollectionEntityId],
	[HydraulicLocationCalculationEntityId])
SELECT
	[HydraulicLocationCalculationCollectionEntityId],
	[HydraulicLocationCalculationEntityId]
FROM TempHydraulicLocationCalculationEntity calculationTable
JOIN TempHydraulicLocationCalculationCollectionEntity calculationCollectionTable
ON calculationTable.GrassCoverErosionOutwardsFailureMechanismMetaEntityId = calculationCollectionTable.GrassCoverErosionOutwardsFailureMechanismMetaEntityId
AND calculationTable.CalculationType = calculationCollectionTable.CalculationType;

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
	[ReferenceLinePointXml],
	[Order]
FROM [SOURCEPROJECT].AssessmentSectionEntity
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForFactorizedSignalingNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 0
) USING (AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForSignalingNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 1
) USING (AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForLowerLimitNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 2
) USING (AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForFactorizedLowerLimitNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 3
) USING (AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFactorizedSignalingNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 4
) USING (AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForSignalingNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 5
) USING (AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForLowerLimitNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 6
) USING (AssessmentSectionEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFactorizedLowerLimitNormId,
		AssessmentSectionEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 7
) USING (AssessmentSectionEntityId);

-- Update the calculation inputs
UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM AssessmentSectionEntity ase
		JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity2Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
		JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
		JOIN [SOURCEPROJECT].HydraulicLocationEntity AS source USING (HydraulicLocationEntityId)
		WHERE ase.NormativeNormType = 2 AND source.ShouldDesignWaterLevelIllustrationPointsBeCalculated = 1
);

UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM AssessmentSectionEntity ase
		JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity3Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
		JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
		JOIN [SOURCEPROJECT].HydraulicLocationEntity AS source USING (HydraulicLocationEntityId)
		WHERE ase.NormativeNormType = 1 AND source.ShouldDesignWaterLevelIllustrationPointsBeCalculated = 1
);

UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM AssessmentSectionEntity ase
		JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity6Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
		JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
		JOIN [SOURCEPROJECT].HydraulicLocationEntity AS source USING (HydraulicLocationEntityId)
		WHERE ase.NormativeNormType = 2 AND source.ShouldWaveHeightIllustrationPointsBeCalculated = 1
);

UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM AssessmentSectionEntity ase
		JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity7Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
		JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
		JOIN [SOURCEPROJECT].HydraulicLocationEntity AS source USING (HydraulicLocationEntityId)
		WHERE ase.NormativeNormType = 1 AND source.ShouldWaveHeightIllustrationPointsBeCalculated = 1
);

--Migrate the outputs on AssessmentSection level
INSERT INTO HydraulicLocationOutputEntity (
	[HydraulicLocationOutputEntityId],
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence])
SELECT
	[HydraulicLocationEntityOutputId],
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM AssessmentSectionEntity ase
JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity2Id = hlcce.HydraulicLocationCalculationCollectionEntityId
JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
JOIN HydraulicLocationEntity USING (HydraulicLocationEntityId)
JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity USING (HydraulicLocationEntityId)
WHERE HydraulicLocationOutputType = 1 AND NormativeNormType = 2

UNION ALL

SELECT
	[HydraulicLocationEntityOutputId],
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM AssessmentSectionEntity ase
JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity3Id = hlcce.HydraulicLocationCalculationCollectionEntityId
JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
JOIN HydraulicLocationEntity USING (HydraulicLocationEntityId)
JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity USING (HydraulicLocationEntityId)
WHERE HydraulicLocationOutputType = 1 AND NormativeNormType = 1

UNION ALL

SELECT
	[HydraulicLocationEntityOutputId],
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM AssessmentSectionEntity ase
JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity6Id = hlcce.HydraulicLocationCalculationCollectionEntityId
JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
JOIN HydraulicLocationEntity USING (HydraulicLocationEntityId)
JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity USING (HydraulicLocationEntityId)
WHERE HydraulicLocationOutputType = 2 AND NormativeNormType = 2

UNION ALL

SELECT
	[HydraulicLocationEntityOutputId],
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM AssessmentSectionEntity ase
JOIN HydraulicLocationCalculationCollectionEntity hlcce ON ase.HydraulicLocationCalculationCollectionEntity7Id = hlcce.HydraulicLocationCalculationCollectionEntityId
JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
JOIN HydraulicLocationEntity USING (HydraulicLocationEntityId)
JOIN [SOURCEPROJECT].HydraulicLocationOutputEntity USING (HydraulicLocationEntityId)
WHERE HydraulicLocationOutputType = 2 AND NormativeNormType = 1;

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
) USING (GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForFailureMechanismSpecificSignalingNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 9
) USING (GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaterLevelCalculationsForFailureMechanismSpecificLowerLimitNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 10
) USING (GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFailureMechanismSpecificFactorizedSignalingNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 11
) USING (GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFailureMechanismSpecificSignalingNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 12
) USING (GrassCoverErosionOutwardsFailureMechanismMetaEntityId)
JOIN (
	SELECT
		HydraulicLocationCalculationCollectionEntityId AS WaveHeightCalculationsForFailureMechanismSpecificLowerLimitNormId,
		GrassCoverErosionOutwardsFailureMechanismMetaEntityId
	FROM TempHydraulicLocationCalculationCollectionEntity WHERE CalculationType = 13
) USING (GrassCoverErosionOutwardsFailureMechanismMetaEntityId);

-- Update the calculation inputs
UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme
		JOIN HydraulicLocationCalculationCollectionEntity hlcce ON gceofmme.HydraulicLocationCalculationCollectionEntity2Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
		JOIN HydraulicLocationCalculationEntity hlce USING (HydraulicLocationCalculationEntityId)
		JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity source ON source.GrassCoverErosionOutwardsHydraulicLocationEntityId = hlce.HydraulicLocationEntityId
		JOIN FailureMechanismEntity USING (FailureMechanismEntityId)
		JOIN AssessmentSectionEntity ase USING (AssessmentSectionEntityId)
		WHERE ase.NormativeNormType = 2 AND source.ShouldDesignWaterLevelIllustrationPointsBeCalculated = 1
);

UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme
		JOIN HydraulicLocationCalculationCollectionEntity hlcce ON gceofmme.HydraulicLocationCalculationCollectionEntity3Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
		JOIN HydraulicLocationCalculationEntity hlce USING (HydraulicLocationCalculationEntityId)
		JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity source ON source.GrassCoverErosionOutwardsHydraulicLocationEntityId = hlce.HydraulicLocationEntityId
		JOIN FailureMechanismEntity USING (FailureMechanismEntityId)
		JOIN AssessmentSectionEntity ase USING (AssessmentSectionEntityId)
		WHERE ase.NormativeNormType = 1 AND source.ShouldDesignWaterLevelIllustrationPointsBeCalculated = 1
);

UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme
		JOIN HydraulicLocationCalculationCollectionEntity hlcce ON gceofmme.HydraulicLocationCalculationCollectionEntity5Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
		JOIN HydraulicLocationCalculationEntity hlce USING (HydraulicLocationCalculationEntityId)
		JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity source ON source.GrassCoverErosionOutwardsHydraulicLocationEntityId = hlce.HydraulicLocationEntityId
		JOIN FailureMechanismEntity USING (FailureMechanismEntityId)
		JOIN AssessmentSectionEntity ase USING (AssessmentSectionEntityId)
		WHERE ase.NormativeNormType = 2 AND source.ShouldWaveHeightIllustrationPointsBeCalculated = 1
);

UPDATE HydraulicLocationCalculationEntity
	SET [ShouldIllustrationPointsBeCalculated] = 1
	WHERE HydraulicLocationCalculationEntityId IN (
		SELECT
			HydraulicLocationCalculationEntityId
		FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme
		JOIN HydraulicLocationCalculationCollectionEntity hlcce ON gceofmme.HydraulicLocationCalculationCollectionEntity6Id = hlcce.HydraulicLocationCalculationCollectionEntityId
		JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
		JOIN HydraulicLocationCalculationEntity hlce USING (HydraulicLocationCalculationEntityId)
		JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity source ON source.GrassCoverErosionOutwardsHydraulicLocationEntityId = hlce.HydraulicLocationEntityId
		JOIN FailureMechanismEntity USING (FailureMechanismEntityId)
		JOIN AssessmentSectionEntity ase USING (AssessmentSectionEntityId)
		WHERE ase.NormativeNormType = 1 AND source.ShouldWaveHeightIllustrationPointsBeCalculated = 1
);

-- Insert outputs
INSERT INTO HydraulicLocationOutputEntity (
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence])
SELECT
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme
JOIN FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId)
JOIN HydraulicLocationCalculationCollectionEntity ON gceofmme.HydraulicLocationCalculationCollectionEntity2Id = HydraulicLocationCalculationCollectionEntityId
JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity ON GrassCoverErosionOutwardsHydraulicLocationEntityId = HydraulicLocationEntityId
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity USING (GrassCoverErosionOutwardsHydraulicLocationEntityId)
WHERE NormativeNormType = 2 AND HydraulicLocationOutputType = 1

UNION ALL

SELECT
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme
JOIN FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId)
JOIN HydraulicLocationCalculationCollectionEntity ON gceofmme.HydraulicLocationCalculationCollectionEntity3Id = HydraulicLocationCalculationCollectionEntityId
JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity ON GrassCoverErosionOutwardsHydraulicLocationEntityId = HydraulicLocationEntityId
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity USING (GrassCoverErosionOutwardsHydraulicLocationEntityId)
WHERE NormativeNormType = 1 AND HydraulicLocationOutputType = 1

UNION ALL

SELECT
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme
JOIN FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId)
JOIN HydraulicLocationCalculationCollectionEntity ON gceofmme.HydraulicLocationCalculationCollectionEntity5Id = HydraulicLocationCalculationCollectionEntityId
JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity ON GrassCoverErosionOutwardsHydraulicLocationEntityId = HydraulicLocationEntityId
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity USING (GrassCoverErosionOutwardsHydraulicLocationEntityId)
WHERE NormativeNormType = 2 AND HydraulicLocationOutputType = 2

UNION ALL

SELECT
	[HydraulicLocationCalculationEntityId],
	[GeneralResultSubMechanismIllustrationPointEntityId],
	[Result],
	[TargetProbability],
	[TargetReliability],
	[CalculatedProbability],
	[CalculatedReliability],
	[CalculationConvergence]
FROM GrassCoverErosionOutwardsFailureMechanismMetaEntity gceofmme
JOIN FailureMechanismEntity USING (FailureMechanismEntityId)
JOIN AssessmentSectionEntity USING (AssessmentSectionEntityId)
JOIN HydraulicLocationCalculationCollectionEntity ON gceofmme.HydraulicLocationCalculationCollectionEntity6Id = HydraulicLocationCalculationCollectionEntityId
JOIN HydraulicLocationCalculationCollectionToHydraulicCalculationEntity USING (HydraulicLocationCalculationCollectionEntityId)
JOIN HydraulicLocationCalculationEntity USING (HydraulicLocationCalculationEntityId)
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationEntity ON GrassCoverErosionOutwardsHydraulicLocationEntityId = HydraulicLocationEntityId
JOIN [SOURCEPROJECT].GrassCoverErosionOutwardsHydraulicLocationOutputEntity USING (GrassCoverErosionOutwardsHydraulicLocationEntityId)
WHERE NormativeNormType = 1 AND HydraulicLocationOutputType = 2;

-- Cleanup
DROP TABLE TempHydraulicLocationCalculationEntity;
DROP TABLE TempHydraulicLocationCalculationCollectionEntity;

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
	"De waarde '" || source.[BelowPhreaticLevelMean] ||  "' voor het gemiddelde van parameter 'Verzadigd gewicht' van ondergrondlaag '" || source.[MaterialName] || "' is ongeldig en is veranderd naar NaN."
	FROM PipingSoilLayerEntity as psl
	JOIN PipingSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN PipingStochasticSoilProfileEntity USING(PipingSoilProfileEntityId)
	JOIN StochasticSoilModelEntity USING (StochasticSoilModelEntityId)
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
	JOIN StochasticSoilModelEntity USING (StochasticSoilModelEntityId)
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
	JOIN StochasticSoilModelEntity USING (StochasticSoilModelEntityId)
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
	JOIN StochasticSoilModelEntity USING (StochasticSoilModelEntityId)
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
	JOIN StochasticSoilModelEntity USING (StochasticSoilModelEntityId)
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
	JOIN StochasticSoilModelEntity USING (StochasticSoilModelEntityId)
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
	JOIN StochasticSoilModelEntity USING (StochasticSoilModelEntityId)
	JOIN [SOURCEPROJECT].PipingSoilLayerEntity AS source ON psl.[rowid] = source.[rowid]
	JOIN TempAssessmentSectionFailureMechanism AS asfm ON asfm.[FailureMechanismId] = [FailureMechanismEntityId]
	WHERE source.[PermeabilityCoefficientOfVariation] IS NOT psl.[PermeabilityCoefficientOfVariation];

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