/*
Migration script for migrating Riskeer databases.
SourceProject version: 23.1
TargetProject version: 24.1
*/
PRAGMA foreign_keys = OFF;
 
ATTACH DATABASE "{0}" AS SOURCEPROJECT;
 
INSERT INTO AdoptableFailureMechanismSectionResultEntity SELECT * FROM [SOURCEPROJECT].AdoptableFailureMechanismSectionResultEntity;
INSERT INTO AdoptableFailureMechanismSectionResultEntity(
    [FailureMechanismSectionEntityId],
    [IsRelevant],
    [InitialFailureMechanismResultType],
    [ManualInitialFailureMechanismResultSectionProbability],
    [FurtherAnalysisType],
    [RefinedSectionProbability])
SELECT
    [FailureMechanismSectionEntityId],
    [IsRelevant],
    [InitialFailureMechanismResultType],
    [ManualInitialFailureMechanismResultSectionProbability],
    [FurtherAnalysisType],
    CASE 
        WHEN [ProbabilityRefinementType] = 2 OR [ProbabilityRefinementType] = 3
            THEN [RefinedSectionProbability]
        WHEN [ProbabilityRefinementType] = 1
            THEN NULL
    END
FROM [SOURCEPROJECT].AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity;
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
INSERT INTO ClosingStructuresFailureMechanismMetaEntity(
    [ClosingStructuresFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ClosingStructureCollectionSourcePath],
    [ForeshoreProfileCollectionSourcePath])
SELECT
    [ClosingStructuresFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ClosingStructureCollectionSourcePath],
    [ForeshoreProfileCollectionSourcePath]
FROM [SOURCEPROJECT].ClosingStructuresFailureMechanismMetaEntity;
INSERT INTO DikeProfileEntity SELECT * FROM [SOURCEPROJECT].DikeProfileEntity;
INSERT INTO DuneErosionFailureMechanismMetaEntity(
    [DuneErosionFailureMechanismMetaEntityId],
    [FailureMechanismEntityId])
SELECT
    [DuneErosionFailureMechanismMetaEntityId],
    [FailureMechanismEntityId]
FROM [SOURCEPROJECT].DuneErosionFailureMechanismMetaEntity;
INSERT INTO DuneLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationEntity;
INSERT INTO DuneLocationCalculationForTargetProbabilityCollectionEntity SELECT * FROM [SOURCEPROJECT].DuneLocationCalculationForTargetProbabilityCollectionEntity;
INSERT INTO DuneLocationEntity SELECT * FROM [SOURCEPROJECT].DuneLocationEntity;
INSERT INTO FailureMechanismEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismEntity;
INSERT INTO FailureMechanismFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismFailureMechanismSectionEntity;
INSERT INTO FailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].FailureMechanismSectionEntity;
INSERT INTO ForeshoreProfileEntity SELECT * FROM [SOURCEPROJECT].ForeshoreProfileEntity;
INSERT INTO GrassCoverErosionInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].GrassCoverErosionInwardsCalculationEntity;
INSERT INTO GrassCoverErosionInwardsFailureMechanismMetaEntity(
    [GrassCoverErosionInwardsFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [DikeProfileCollectionSourcePath])
SELECT
    [GrassCoverErosionInwardsFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [DikeProfileCollectionSourcePath]
FROM [SOURCEPROJECT].GrassCoverErosionInwardsFailureMechanismMetaEntity;
INSERT INTO GrassCoverErosionOutwardsFailureMechanismMetaEntity(
    [GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ForeshoreProfileCollectionSourcePath])
SELECT
    [GrassCoverErosionOutwardsFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ForeshoreProfileCollectionSourcePath]
FROM [SOURCEPROJECT].GrassCoverErosionOutwardsFailureMechanismMetaEntity;
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
INSERT INTO HeightStructureEntity SELECT * FROM [SOURCEPROJECT].HeightStructureEntity;
INSERT INTO HeightStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].HeightStructuresCalculationEntity;
INSERT INTO HeightStructuresFailureMechanismMetaEntity(
    [HeightStructuresFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [HeightStructureCollectionSourcePath],
    [ForeshoreProfileCollectionSourcePath])
SELECT
    [HeightStructuresFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [HeightStructureCollectionSourcePath],
    [ForeshoreProfileCollectionSourcePath]
FROM [SOURCEPROJECT].HeightStructuresFailureMechanismMetaEntity;
INSERT INTO HydraulicBoundaryDataEntity SELECT * FROM [SOURCEPROJECT].HydraulicBoundaryDataEntity;
INSERT INTO HydraulicBoundaryDatabaseEntity SELECT * FROM [SOURCEPROJECT].HydraulicBoundaryDatabaseEntity;
INSERT INTO HydraulicLocationCalculationCollectionEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationCollectionEntity;
INSERT INTO HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationCollectionHydraulicLocationCalculationEntity;
INSERT INTO HydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationEntity;
INSERT INTO HydraulicLocationCalculationForTargetProbabilityCollectionEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationForTargetProbabilityCollectionEntity;
INSERT INTO HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationCalculationForTargetProbabilityCollectionHydraulicLocationCalculationEntity;
INSERT INTO HydraulicLocationEntity SELECT * FROM [SOURCEPROJECT].HydraulicLocationEntity;
INSERT INTO MacroStabilityInwardsCalculationEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationEntity;
INSERT INTO MacroStabilityInwardsCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsCharacteristicPointEntity;
INSERT INTO MacroStabilityInwardsFailureMechanismMetaEntity(
    [MacroStabilityInwardsFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [StochasticSoilModelCollectionSourcePath],
    [SurfaceLineCollectionSourcePath])
SELECT
    [MacroStabilityInwardsFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [StochasticSoilModelCollectionSourcePath],
    [SurfaceLineCollectionSourcePath]
FROM [SOURCEPROJECT].MacroStabilityInwardsFailureMechanismMetaEntity;
INSERT INTO MacroStabilityInwardsFailureMechanismSectionConfigurationEntity(
    [FailureMechanismSectionEntityId],
    [A])
SELECT
    [FailureMechanismSectionEntityId],
    1
FROM FailureMechanismEntity
JOIN FailureMechanismFailureMechanismSectionEntity USING(FailureMechanismEntityId)
WHERE FailureMechanismType = 2;
INSERT INTO MacroStabilityInwardsPreconsolidationStressEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsPreconsolidationStressEntity;
INSERT INTO MacroStabilityInwardsSoilLayerOneDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilLayerOneDEntity;
INSERT INTO MacroStabilityInwardsSoilLayerTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilLayerTwoDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileOneDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileOneDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileTwoDEntity;
INSERT INTO MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsSoilProfileTwoDSoilLayerTwoDEntity;
INSERT INTO MacroStabilityInwardsStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].MacroStabilityInwardsStochasticSoilProfileEntity;
INSERT INTO NonAdoptableFailureMechanismSectionResultEntity SELECT * FROM [SOURCEPROJECT].NonAdoptableFailureMechanismSectionResultEntity;
INSERT INTO NonAdoptableFailureMechanismSectionResultEntity(
    [FailureMechanismSectionEntityId],
    [IsRelevant],
    [InitialFailureMechanismResultType],
    [ManualInitialFailureMechanismResultSectionProbability],
    [FurtherAnalysisType],
    [RefinedSectionProbability])
SELECT
    [FailureMechanismSectionEntityId],
    [IsRelevant],
    [InitialFailureMechanismResultType],
    [ManualInitialFailureMechanismResultSectionProbability],
    [FurtherAnalysisType],
    [RefinedSectionProbability]
FROM [SOURCEPROJECT].NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity;
INSERT INTO PipingCharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].PipingCharacteristicPointEntity;
INSERT INTO PipingFailureMechanismMetaEntity(
    [PipingFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [WaterVolumetricWeight],
    [StochasticSoilModelCollectionSourcePath],
    [SurfaceLineCollectionSourcePath],
    [ScenarioConfigurationType]) 
SELECT
    [PipingFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [WaterVolumetricWeight],
    [StochasticSoilModelCollectionSourcePath],
    [SurfaceLineCollectionSourcePath],
    [PipingScenarioConfigurationType]
FROM [SOURCEPROJECT].PipingFailureMechanismMetaEntity;
INSERT INTO PipingFailureMechanismSectionConfigurationEntity(
    [PipingFailureMechanismSectionConfigurationEntityId],
    [FailureMechanismSectionEntityId],
    [ScenarioConfigurationType],
    [A])
SELECT
    [PipingScenarioConfigurationPerFailureMechanismSectionEntityId],
    [FailureMechanismSectionEntityId],
    [PipingScenarioConfigurationPerFailureMechanismSectionType],
    1
FROM [SOURCEPROJECT].PipingScenarioConfigurationPerFailureMechanismSectionEntity;
INSERT INTO PipingSoilLayerEntity SELECT * FROM [SOURCEPROJECT].PipingSoilLayerEntity;
INSERT INTO PipingSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingSoilProfileEntity;
INSERT INTO PipingStochasticSoilProfileEntity SELECT * FROM [SOURCEPROJECT].PipingStochasticSoilProfileEntity;
INSERT INTO ProbabilisticPipingCalculationEntity SELECT * FROM [SOURCEPROJECT].ProbabilisticPipingCalculationEntity;
INSERT INTO ProjectEntity SELECT * FROM [SOURCEPROJECT].ProjectEntity;
INSERT INTO SemiProbabilisticPipingCalculationEntity SELECT * FROM [SOURCEPROJECT].SemiProbabilisticPipingCalculationEntity;
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
    [FailureMechanismAssemblyResultProbabilityResultType],
    [FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability])
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
    [FailureMechanismAssemblyResultProbabilityResultType],
    [FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability]
FROM [SOURCEPROJECT].SpecificFailureMechanismEntity;
INSERT INTO SpecificFailureMechanismFailureMechanismSectionEntity SELECT * FROM [SOURCEPROJECT].SpecificFailureMechanismFailureMechanismSectionEntity;
INSERT INTO StabilityPointStructureEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructureEntity;
INSERT INTO StabilityPointStructuresCalculationEntity SELECT * FROM [SOURCEPROJECT].StabilityPointStructuresCalculationEntity;
INSERT INTO StabilityPointStructuresFailureMechanismMetaEntity(
    [StabilityPointStructuresFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ForeshoreProfileCollectionSourcePath],
    [StabilityPointStructureCollectionSourcePath])
SELECT
    [StabilityPointStructuresFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ForeshoreProfileCollectionSourcePath],
    [StabilityPointStructureCollectionSourcePath]
FROM [SOURCEPROJECT].StabilityPointStructuresFailureMechanismMetaEntity;
INSERT INTO StabilityStoneCoverFailureMechanismMetaEntity(
    [StabilityStoneCoverFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ForeshoreProfileCollectionSourcePath])
SELECT
    [StabilityStoneCoverFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ForeshoreProfileCollectionSourcePath]
FROM [SOURCEPROJECT].StabilityStoneCoverFailureMechanismMetaEntity;
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
INSERT INTO WaveImpactAsphaltCoverFailureMechanismMetaEntity(
    [WaveImpactAsphaltCoverFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ForeshoreProfileCollectionSourcePath]) 
SELECT
    [WaveImpactAsphaltCoverFailureMechanismMetaEntityId],
    [FailureMechanismEntityId],
    [ForeshoreProfileCollectionSourcePath]
FROM [SOURCEPROJECT].WaveImpactAsphaltCoverFailureMechanismMetaEntity;
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
    "24.1",
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
-- SemiProbabilisticPipingCalculationOutputEntity
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
-- MacroStabilityInwardsCalculationOutputEntity

/*
Tables that only have length effect columns are not migrated:
*/

-- GrassCoverSlipOffInwardsFailureMechanismMetaEntity
-- GrassCoverSlipOffOutwardsFailureMechanismMetaEntity
-- MicrostabilityFailureMechanismMetaEntity
-- PipingStructureFailureMechanismMetaEntity
-- WaterPressureAsphaltCoverFailureMechanismMetaEntity

/*
Update specific data
*/
UPDATE HydraulicBoundaryDataEntity
SET [HydraulicLocationConfigurationDatabaseScenarioName] = NULL,
    [HydraulicLocationConfigurationDatabaseYear] = NULL,
    [HydraulicLocationConfigurationDatabaseScope] = NULL,
    [HydraulicLocationConfigurationDatabaseSeaLevel] = NULL,
    [HydraulicLocationConfigurationDatabaseRiverDischarge] = NULL,
    [HydraulicLocationConfigurationDatabaseLakeLevel] = NULL,
    [HydraulicLocationConfigurationDatabaseWindDirection] = NULL,
    [HydraulicLocationConfigurationDatabaseWindSpeed] = NULL,
    [HydraulicLocationConfigurationDatabaseComment] = NULL
WHERE [HydraulicLocationConfigurationDatabaseComment] = "Gegenereerd door Riskeer (conform WBI2017)";

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
    "24.1",
    "Gevolgen van de migratie van versie 23.1 naar versie 24.1:");

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
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].SemiProbabilisticPipingCalculationOutputEntity;
INSERT INTO TempLogOutputDeleted SELECT COUNT() FROM [SOURCEPROJECT].MacroStabilityInwardsCalculationOutputEntity;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
SELECT
    "23.1",
    "24.1",
    "* Alle berekende resultaten zijn verwijderd."
FROM TempLogOutputDeleted
WHERE [NrDeleted] > 0
LIMIT 1;

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
[LogMessage])
VALUES ("23.1",
    "24.1",
    "* Omdat alleen faalkansen op vakniveau een rol spelen in de assemblage, zijn de assemblageresultaten voor de faalmechanismen aangepast:
  + De initiële faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Handmatig invullen'.
  + De aangescherpte faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Per doorsnede' of 'Beide'.
  + De assemblagemethode 'Automatisch berekenen o.b.v. slechtste doorsnede of vak' is vervangen door 'Automatisch berekenen o.b.v. slechtste vak'.");

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
    "24.1",
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

INSERT INTO [LOGDATABASE].MigrationLogEntity (
    [FromVersion],
    [ToVersion],
    [LogMessage])
SELECT "23.1",
       "24.1",
       "* Geen aanpassingen."
WHERE (
    SELECT COUNT() FROM [LOGDATABASE].MigrationLogEntity
    WHERE [FromVersion] = "23.1"
) IS 1;
 
DETACH LOGDATABASE;
 
DETACH SOURCEPROJECT;
 
PRAGMA foreign_keys = ON;