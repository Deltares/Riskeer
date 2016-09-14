/* ---------------------------------------------------- */
/*  Generated by Enterprise Architect Version 12.0 		*/
/*  Created On : 14-sep-2016 14:06:31 				*/
/*  DBMS       : SQLite 								*/
/* ---------------------------------------------------- */

/* Drop Tables */

DROP TABLE IF EXISTS 'VersionEntity'
;

DROP TABLE IF EXISTS 'PipingFailureMechanismMetaEntity'
;

DROP TABLE IF EXISTS 'ProjectEntity'
;

DROP TABLE IF EXISTS 'AssessmentSectionEntity'
;

DROP TABLE IF EXISTS 'FailureMechanismEntity'
;

DROP TABLE IF EXISTS 'FailureMechanismSectionEntity'
;

DROP TABLE IF EXISTS 'CalculationGroupEntity'
;

DROP TABLE IF EXISTS 'HydraulicLocationEntity'
;

DROP TABLE IF EXISTS 'PipingCalculationEntity'
;

DROP TABLE IF EXISTS 'GrassCoverErosionInwardsFailureMechanismMetaEntity'
;

DROP TABLE IF EXISTS 'GrassCoverErosionInwardsCalculationEntity'
;

DROP TABLE IF EXISTS 'GrassCoverErosionOutwardsFailureMechanismMetaEntity'
;

DROP TABLE IF EXISTS 'SoilLayerEntity'
;

DROP TABLE IF EXISTS 'SoilProfileEntity'
;

DROP TABLE IF EXISTS 'StochasticSoilProfileEntity'
;

DROP TABLE IF EXISTS 'StochasticSoilModelEntity'
;

DROP TABLE IF EXISTS 'SurfaceLineEntity'
;

DROP TABLE IF EXISTS 'CharacteristicPointEntity'
;

DROP TABLE IF EXISTS 'PipingCalculationOutputEntity'
;

DROP TABLE IF EXISTS 'ProbabilisticOutputEntity'
;

DROP TABLE IF EXISTS 'PipingSemiProbabilisticOutputEntity'
;

DROP TABLE IF EXISTS 'PipingSectionResultEntity'
;

DROP TABLE IF EXISTS 'GrassCoverErosionInwardsSectionResultEntity'
;

DROP TABLE IF EXISTS 'HeightStructuresSectionResultEntity'
;

DROP TABLE IF EXISTS 'StrengthStabilityLengthwiseConstructionSectionResultEntity'
;

DROP TABLE IF EXISTS 'TechnicalInnovationSectionResultEntity'
;

DROP TABLE IF EXISTS 'WaterPressureAsphaltCoverSectionResultEntity'
;

DROP TABLE IF EXISTS 'ClosingStructureSectionResultEntity'
;

DROP TABLE IF EXISTS 'GrassCoverErosionOutwardsSectionResultEntity'
;

DROP TABLE IF EXISTS 'GrassCoverSlipOffInwardsSectionResultEntity'
;

DROP TABLE IF EXISTS 'GrassCoverSlipOffOutwardsSectionResultEntity'
;

DROP TABLE IF EXISTS 'MacrostabilityInwardsSectionResultEntity'
;

DROP TABLE IF EXISTS 'MacrostabilityOutwardsSectionResultEntity'
;

DROP TABLE IF EXISTS 'WaveImpactAsphaltCoverSectionResultEntity'
;

DROP TABLE IF EXISTS 'MicrostabilitySectionResultEntity'
;

DROP TABLE IF EXISTS 'PipingStructureSectionResultEntity'
;

DROP TABLE IF EXISTS 'DuneErosionSectionResultEntity'
;

DROP TABLE IF EXISTS 'StabilityStoneCoverSectionResultEntity'
;

DROP TABLE IF EXISTS 'StrengthStabilityPointConstructionSectionResultEntity'
;

DROP TABLE IF EXISTS 'DikeProfileEntity'
;

DROP TABLE IF EXISTS 'GrassCoverErosionInwardsOutputEntity'
;

/* Create Tables with Primary and Foreign Keys, Check and Unique Constraints */

CREATE TABLE 'VersionEntity'
(
	'VersionId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'Version' INTEGER NOT NULL,
	'Timestamp' DATETIME NOT NULL,
	'FingerPrint' BLOB NOT NULL
)
;

CREATE TABLE 'PipingFailureMechanismMetaEntity'
(
	'PipingFailureMechanismMetaEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	'A' REAL NOT NULL,
	'UpliftCriticalSafetyFactor' REAL NOT NULL,
	CONSTRAINT 'FK_PipingFailureMechanismMetaEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'ProjectEntity'
(
	'ProjectEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'Description' VARCHAR (260)
)
;

CREATE TABLE 'AssessmentSectionEntity'
(
	'AssessmentSectionEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'Id' TEXT,
	'ProjectEntityId' INTEGER NOT NULL,
	'Name' VARCHAR (260),
	'Comments' TEXT,
	'Norm' INT (4) NOT NULL,
	'HydraulicDatabaseVersion' TEXT,
	'HydraulicDatabaseLocation' TEXT,
	'Composition' SMALLINT NOT NULL, -- Enum: Dike = 0, Dune = 1, DikeAndDune = 2
	'ReferenceLinePointXml' TEXT,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_AssessmentSectionEntity_ProjectEntity' FOREIGN KEY ('ProjectEntityId') REFERENCES 'ProjectEntity' ('ProjectEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'FailureMechanismEntity'
(
	'FailureMechanismEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'AssessmentSectionEntityId' INTEGER NOT NULL,
	'CalculationGroupEntityId' INTEGER,
	'FailureMechanismType' SMALLINT NOT NULL, -- Enumerator for different failure mechanism types (piping, macrostability, dunes, etc)  1 = Piping 2 = Macrostabiliteit binnenwaarts 3= Golfklappen op asfaltbekleding 4= Grasbekleding erosie buitentalud 5 = Grasbekleding afschuiven buitentalud 6 = Grasbekleding erosie kruin en binnentalud 7 = Stabiliteit steenzetting 8 = Duinafslag 9 = Hoogte kunstwerk 10 = Betrouwbaarheid sluiten kunstwerk 11 = Piping bij kunstwerk 12 = Sterkte en stabiliteit puntconstructires 13 = Macrostabiliteit buitenwaarts 14 = Microstabiliteit 15 = Wateroverdruk bij asfaltbekleding 16 = Grasbekleding afschuiven binnentalud 17 = Sterkte en stabiliteit langsconstructires 18 = Technische innovaties
	'IsRelevant' TINYINT (1) NOT NULL, -- true or false
	'Comments' TEXT,
	CONSTRAINT 'FK_FailureMechanismEntity_AssessmentSectionEntity' FOREIGN KEY ('AssessmentSectionEntityId') REFERENCES 'AssessmentSectionEntity' ('AssessmentSectionEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'FK_FailureMechanismEntity_CalculationGroupEntity' FOREIGN KEY ('CalculationGroupEntityId') REFERENCES 'CalculationGroupEntity' ('CalculationGroupEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'UI_AssessmentSectionEntityId_FailureMechanismType' UNIQUE ('AssessmentSectionEntityId','FailureMechanismType')
)
;

CREATE TABLE 'FailureMechanismSectionEntity'
(
	'FailureMechanismSectionEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	'Name' VARCHAR (260) NOT NULL,
	'FailureMechanismSectionPointXml' TEXT NOT NULL,
	CONSTRAINT 'FK_FailureMechanismSectionEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'CalculationGroupEntity'
(
	'CalculationGroupEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'ParentCalculationGroupEntityId' INTEGER,
	'Name' VARCHAR (260),
	'IsEditable' TINYINT (1) NOT NULL, -- true or false
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_CalculationGroupEntity_CalculationGroupEntity' FOREIGN KEY ('ParentCalculationGroupEntityId') REFERENCES 'CalculationGroupEntity' ('CalculationGroupEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'HydraulicLocationEntity'
(
	'HydraulicLocationEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'AssessmentSectionEntityId' INTEGER NOT NULL,
	'LocationId' INTEGER NOT NULL,
	'Name' VARCHAR (260) NOT NULL,
	'LocationX' REAL,
	'LocationY' REAL,
	'DesignWaterLevel' REAL,
	'WaveHeight' REAL,
	'Order' INT (4) NOT NULL,
	'DesignWaterLevelCalculationConvergence' TINYINT (1) NOT NULL, -- NotCalculated, CalculatedNotConverged or CalculatedConverged
	'WaveHeightCalculationConvergence' TINYINT (1) NOT NULL, -- NotCalculated, CalculatedNotConverged or CalculatedConverged
	CONSTRAINT 'FK_HydraulicLocationEntity_AssessmentSectionEntity' FOREIGN KEY ('AssessmentSectionEntityId') REFERENCES 'AssessmentSectionEntity' ('AssessmentSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'PipingCalculationEntity'
(
	'PipingCalculationEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'CalculationGroupEntityId' INTEGER NOT NULL,
	'SurfaceLineEntityId' INTEGER,
	'StochasticSoilProfileEntityId' INTEGER,
	'HydraulicLocationEntityId' INTEGER,
	'PipingCalculationOutputEntityId' INTEGER,
	'PipingSemiProbabilisticOutputEntityId' INTEGER,
	'Order' INT (4) NOT NULL,
	'Name' VARCHAR (260),
	'Comments' TEXT,
	'EntryPointL' REAL,
	'ExitPointL' REAL,
	'PhreaticLevelExitMean' REAL,
	'PhreaticLevelExitStandardDeviation' REAL,
	'DampingFactorExitMean' REAL,
	'DampingFactorExitStandardDeviation' REAL,
	'RelevantForScenario' TINYINT (1) NOT NULL,
	'ScenarioContribution' REAL,
	CONSTRAINT 'FK_PipingCalculationEntity_CalculationGroupEntity' FOREIGN KEY ('CalculationGroupEntityId') REFERENCES 'CalculationGroupEntity' ('CalculationGroupEntityId') ON DELETE No Action ON UPDATE No Action,
	CONSTRAINT 'FK_PipingCalculationEntity_HydraulicLocationEntity' FOREIGN KEY ('HydraulicLocationEntityId') REFERENCES 'HydraulicLocationEntity' ('HydraulicLocationEntityId') ON DELETE No Action ON UPDATE No Action,
	CONSTRAINT 'FK_PipingCalculationEntity_PipingCalculationOutputEntity' FOREIGN KEY ('PipingCalculationOutputEntityId') REFERENCES 'PipingCalculationOutputEntity' ('PipingCalculationOutputEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'FK_PipingCalculationEntity_PipingSemiProbabilisticOutputEntity' FOREIGN KEY ('PipingSemiProbabilisticOutputEntityId') REFERENCES 'PipingSemiProbabilisticOutputEntity' ('PipingSemiProbabilisticOutputEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'FK_PipingCalculationEntity_StochasticSoilProfileEntity' FOREIGN KEY ('StochasticSoilProfileEntityId') REFERENCES 'StochasticSoilProfileEntity' ('StochasticSoilProfileEntityId') ON DELETE No Action ON UPDATE No Action,
	CONSTRAINT 'FK_PipingCalculationEntity_SurfaceLineEntity' FOREIGN KEY ('SurfaceLineEntityId') REFERENCES 'SurfaceLineEntity' ('SurfaceLineEntityId') ON DELETE No Action ON UPDATE No Action
)
;

CREATE TABLE 'GrassCoverErosionInwardsFailureMechanismMetaEntity'
(
	'GrassCoverErosionInwardsFailureMechanismMetaEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	'N' INT (4) NOT NULL,
	CONSTRAINT 'FK_GrassCoverErosionInwardsFailureMechanismMetaEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'GrassCoverErosionInwardsCalculationEntity'
(
	'GrassCoverErosionInwardsCalculationEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'CalculationGroupEntityId' INTEGER NOT NULL,
	'HydraulicLocationEntityId' INTEGER,
	'DikeProfileEntityId' INTEGER,
	'GrassCoverErosionInwardsOutputEntityId' INTEGER,
	'Order' INT (4) NOT NULL,
	'Name' VARCHAR (260),
	'Comments' TEXT,
	'Orientation' REAL,
	'CriticalFlowRateMean' REAL,
	'CriticalFlowRateStandardDeviation' REAL,
	'UseForeshore' TINYINT (1) NOT NULL,
	'DikeHeight' REAL,
	'UseBreakWater' TINYINT (1) NOT NULL,
	'BreakWaterType' SMALLINT NOT NULL,
	'BreakWaterHeight' REAL,
	'CalculateDikeHeight' TINYINT (1) NOT NULL,
	CONSTRAINT 'FK_GrassCoverErosionInwardsCalculationEntity_CalculationGroupEntity' FOREIGN KEY ('CalculationGroupEntityId') REFERENCES 'CalculationGroupEntity' ('CalculationGroupEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'FK_GrassCoverErosionInwardsCalculationEntity_DikeProfileEntity' FOREIGN KEY ('DikeProfileEntityId') REFERENCES 'DikeProfileEntity' ('DikeProfileEntityId') ON DELETE Set Null ON UPDATE Cascade,
	CONSTRAINT 'FK_GrassCoverErosionInwardsCalculationEntity_GrassCoverErosionInwardsOutputEntity' FOREIGN KEY ('GrassCoverErosionInwardsOutputEntityId') REFERENCES 'GrassCoverErosionInwardsOutputEntity' ('GrassCoverErosionInwardsOutputEntityId') ON DELETE Set Null ON UPDATE Cascade,
	CONSTRAINT 'FK_GrassCoverErosionInwardsCalculationEntity_HydraulicLocationEntity' FOREIGN KEY ('HydraulicLocationEntityId') REFERENCES 'HydraulicLocationEntity' ('HydraulicLocationEntityId') ON DELETE Set Null ON UPDATE Cascade
)
;

CREATE TABLE 'GrassCoverErosionOutwardsFailureMechanismMetaEntity'
(
	'GrassCoverErosionOutwardsFailureMechanismMetaEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	'N' INT (4) NOT NULL,
	CONSTRAINT 'FK_GrassCoverErosionOutwardsFailureMechanismMetaEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'SoilLayerEntity'
(
	'SoilLayerEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'SoilProfileEntityId' INTEGER NOT NULL,
	'Top' REAL,
	'IsAquifer' TINYINT (1) NOT NULL, -- true or false
	'Color' INTEGER NOT NULL, -- ARGB value of Color.
	'MaterialName' TEXT NOT NULL,
	'BelowPhreaticLevelMean' REAL,
	'BelowPhreaticLevelDeviation' REAL,
	'DiameterD70Mean' REAL,
	'DiameterD70Deviation' REAL,
	'BelowPhreaticLevelShift' REAL,
	'PermeabilityMean' REAL,
	'PermeabilityDeviation' REAL,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_SoilLayerEntity_SoilProfileEntity' FOREIGN KEY ('SoilProfileEntityId') REFERENCES 'SoilProfileEntity' ('SoilProfileEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'SoilProfileEntity'
(
	'SoilProfileEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'Bottom' REAL,
	'Name' TEXT
)
;

CREATE TABLE 'StochasticSoilProfileEntity'
(
	'StochasticSoilProfileEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'SoilProfileEntityId' INTEGER NOT NULL,
	'StochasticSoilModelEntityId' INTEGER NOT NULL,
	'Probability' REAL,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_StochasticSoilProfileEntity_SoilProfileEntity' FOREIGN KEY ('SoilProfileEntityId') REFERENCES 'SoilProfileEntity' ('SoilProfileEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'FK_StochasticSoilProfileEntity_StochasticSoilModelEntity' FOREIGN KEY ('StochasticSoilModelEntityId') REFERENCES 'StochasticSoilModelEntity' ('StochasticSoilModelEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'StochasticSoilModelEntity'
(
	'StochasticSoilModelEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	'Name' TEXT,
	'SegmentName' TEXT,
	'StochasticSoilModelSegmentPointXml' TEXT NOT NULL,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_StochasticSoilModelEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'SurfaceLineEntity'
(
	'SurfaceLineEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	'Name' VARCHAR (260),
	'ReferenceLineIntersectionX' REAL,
	'ReferenceLineIntersectionY' REAL,
	'PointsXml' TEXT NOT NULL,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_SurfaceLineEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE No Action ON UPDATE No Action
)
;

CREATE TABLE 'CharacteristicPointEntity'
(
	'CharacteristicPointEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'SurfaceLineEntityId' INTEGER NOT NULL,
	'Type' SMALLINT NOT NULL, -- Enum: 1 = DikeToeAtRiver 2 = DikeToeAtPolder 3 = DitchDikeSide 4 = BottomDitchDikeSide 5 = BottomDitchPolderSide 6 = DitchPolderSide
	'X' REAL,
	'Y' REAL,
	'Z' REAL,
	CONSTRAINT 'FK_SurfaceLinePointEntity_SurfaceLineEntity' FOREIGN KEY ('SurfaceLineEntityId') REFERENCES 'SurfaceLineEntity' ('SurfaceLineEntityId') ON DELETE No Action ON UPDATE No Action
)
;

CREATE TABLE 'PipingCalculationOutputEntity'
(
	'PipingCalculationOutputEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'HeaveFactorOfSafety' REAL,
	'HeaveZValue' REAL,
	'UpliftFactorOfSafety' REAL,
	'UpliftZValue' REAL,
	'SellmeijerFactorOfSafety' REAL,
	'SellmeijerZValue' REAL
)
;

CREATE TABLE 'ProbabilisticOutputEntity'
(
	'ProbabilisticOutputEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'RequiredProbability' REAL,
	'RequiredReliability' REAL,
	'Probability' REAL,
	'Reliability' REAL,
	'FactorOfSafety' REAL
)
;

CREATE TABLE 'PipingSemiProbabilisticOutputEntity'
(
	'PipingSemiProbabilisticOutputEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'UpliftFactorOfSafety' REAL,
	'UpliftProbability' REAL,
	'HeaveFactorOfSafety' REAL,
	'HeaveReliability' REAL,
	'HeaveProbability' REAL,
	'SellmeijerFactorOfSafety' REAL,
	'SellmeijerReliability' REAL,
	'SellmeijerProbability' REAL,
	'RequiredProbability' REAL,
	'RequiredReliability' REAL,
	'PipingProbability' REAL,
	'PipingReliability' REAL,
	'PipingFactorOfSafety' REAL
)
;

CREATE TABLE 'PipingSectionResultEntity'
(
	'PipingSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerThree' REAL,
	CONSTRAINT 'FK_PipingSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'GrassCoverErosionInwardsSectionResultEntity'
(
	'GrassCoverErosionInwardsSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'GrassCoverErosionInwardsCalculationEntityId' INTEGER,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerThree' REAL,
	CONSTRAINT 'FK_GrassCoverErosionInwardsSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'FK_GrassCoverErosionInwardsSectionResultEntity_GrassCoverErosionInwardsCalculationEntity' FOREIGN KEY ('GrassCoverErosionInwardsCalculationEntityId') REFERENCES 'GrassCoverErosionInwardsCalculationEntity' ('GrassCoverErosionInwardsCalculationEntityId') ON DELETE Set Null ON UPDATE Cascade
)
;

CREATE TABLE 'HeightStructuresSectionResultEntity'
(
	'HeightStructuresSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerThree' REAL,
	CONSTRAINT 'FK_HeightStructuresSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'StrengthStabilityLengthwiseConstructionSectionResultEntity'
(
	'StrengthStabilityLengthwiseConstructionSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerThree' REAL,
	CONSTRAINT 'FK_StrengthStabilityLengthwiseConstructionSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'TechnicalInnovationSectionResultEntity'
(
	'TechnicalInnovationSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerThree' REAL,
	CONSTRAINT 'FK_TechnicalInnovationSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'WaterPressureAsphaltCoverSectionResultEntity'
(
	'WaterPressureAsphaltCoverSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerThree' REAL,
	CONSTRAINT 'FK_WaterPressureAsphaltCoverSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'ClosingStructureSectionResultEntity'
(
	'ClosingStructureSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerTwoA' REAL,
	'LayerThree' REAL,
	CONSTRAINT 'FK_ClosingStructureSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'GrassCoverErosionOutwardsSectionResultEntity'
(
	'GrassCoverErosionOutwardsSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerTwoA' TINYINT (1) NOT NULL, -- Enum: 1 = NotCalculated 2 = Failed 3 = Succesful
	'LayerThree' REAL,
	CONSTRAINT 'FK_GrassCoverErosionOutwardsSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'GrassCoverSlipOffInwardsSectionResultEntity'
(
	'GrassCoverSlipOffInwardsSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerTwoA' TINYINT (1) NOT NULL, -- Enum: 1 = NotCalculated 2 = Failed 3 = Succesful
	'LayerThree' REAL,
	CONSTRAINT 'FK_GrassCoverSlipOffInwardsSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'GrassCoverSlipOffOutwardsSectionResultEntity'
(
	'GrassCoverSlipOffOutwardsSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerTwoA' TINYINT (1) NOT NULL, -- Enum: 1 = NotCalculated 2 = Failed 3 = Succesful
	'LayerThree' REAL,
	CONSTRAINT 'FK_GrassCoverSlipOffOutwardsSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'MacrostabilityInwardsSectionResultEntity'
(
	'MacrostabilityInwardsSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerTwoA' REAL,
	'LayerThree' REAL,
	CONSTRAINT 'FK_MacrostabilityInwardsSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'MacrostabilityOutwardsSectionResultEntity'
(
	'MacrostabilityOutwardsSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerTwoA' REAL,
	'LayerThree' REAL,
	CONSTRAINT 'FK_MacrostabilityOutwardsSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'WaveImpactAsphaltCoverSectionResultEntity'
(
	'WaveImpactAsphaltCoverSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerTwoA' REAL,
	'LayerThree' REAL,
	CONSTRAINT 'FK_WaveImpactAsphaltCoverSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'MicrostabilitySectionResultEntity'
(
	'MicrostabilitySectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerTwoA' TINYINT (1) NOT NULL, -- Enum: 1 = NotCalculated 2 = Failed 3 = Succesful
	'LayerThree' REAL,
	CONSTRAINT 'FK_MicrostabilitySectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'PipingStructureSectionResultEntity'
(
	'PipingStructureSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerOne' TINYINT (1) NOT NULL, -- true or false
	'LayerTwoA' TINYINT (1) NOT NULL, -- Enum: 1 = NotCalculated 2 = Failed 3 = Succesful
	'LayerThree' REAL,
	CONSTRAINT 'FK_PipingStructureSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'DuneErosionSectionResultEntity'
(
	'DuneErosionSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerTwoA' TINYINT (1) NOT NULL, -- Enum: 1 = NotCalculated 2 = Failed 3 = Succesful
	'LayerThree' REAL,
	CONSTRAINT 'FK_DuneErosionSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'StabilityStoneCoverSectionResultEntity'
(
	'StabilityStoneCoverSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerTwoA' TINYINT (1) NOT NULL, -- Enum: 1 = NotCalculated 2 = Failed 3 = Succesful
	'LayerThree' REAL,
	CONSTRAINT 'FK_StabilityStoneCoverSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'StrengthStabilityPointConstructionSectionResultEntity'
(
	'StrengthStabilityPointConstructionSectionResultEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'LayerTwoA' REAL,
	'LayerThree' REAL,
	CONSTRAINT 'FK_StrengthStabilityPointConstructionSectionResultEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'DikeProfileEntity'
(
	'DikeProfileEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	'Name' VARCHAR (260) NOT NULL,
	'Orientation' REAL,
	'BreakWaterType' TINYINT (1), -- Enum: 1 = Wall 2 = Caisson 3 = Dam
	'BreakWaterHeight' REAL,
	'ForeShoreXml' TEXT NOT NULL,
	'DikeGeometryXml' TEXT NOT NULL,
	'DikeHeight' REAL,
	'X' REAL,
	'Y' REAL,
	'X0' REAL,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_DikeProfileEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'GrassCoverErosionInwardsOutputEntity'
(
	'GrassCoverErosionInwardsOutputEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'ProbabilisticOutputEntityId' INTEGER NOT NULL,
	'IsOvertoppingDominant' TINYINT (1) NOT NULL,
	'WaveHeight' REAL,
	'DikeHeight' REAL, -- REAL mapped as string, or DBNull when the value is null.
	'IsDikeHeightCalculated' TINYINT (1) NOT NULL,
	CONSTRAINT 'FK_GrassCoverErosionInwardsOutput_ProbabilisticOutputEntity' FOREIGN KEY ('ProbabilisticOutputEntityId') REFERENCES 'ProbabilisticOutputEntity' ('ProbabilisticOutputEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

/* Create Indexes and Triggers */

CREATE INDEX 'IXFK_PipingFailureMechanismMetaEntity_FailureMechanismEntity'
 ON 'PipingFailureMechanismMetaEntity' ('FailureMechanismEntityId' ASC)
;

CREATE INDEX 'IXFK_AssessmentSectionEntity_ProjectEntity'
 ON 'AssessmentSectionEntity' ('ProjectEntityId' ASC)
;

CREATE INDEX 'IXFK_FailureMechanismEntity_AssessmentSectionEntity'
 ON 'FailureMechanismEntity' ('AssessmentSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_FailureMechanismEntity_CalculationGroupEntity'
 ON 'FailureMechanismEntity' ('CalculationGroupEntityId' ASC)
;

CREATE INDEX 'IXFK_FailureMechanismSectionEntity_FailureMechanismEntity'
 ON 'FailureMechanismSectionEntity' ('FailureMechanismEntityId' ASC)
;

CREATE INDEX 'IXFK_CalculationGroupEntity_CalculationGroupEntity'
 ON 'CalculationGroupEntity' ('ParentCalculationGroupEntityId' ASC)
;

CREATE INDEX 'IXFK_HydraulicLocationEntity_AssessmentSectionEntity'
 ON 'HydraulicLocationEntity' ('AssessmentSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_PipingCalculationEntity_PipingCalculationOutputEntity'
 ON 'PipingCalculationEntity' ('PipingCalculationOutputEntityId' ASC)
;

CREATE INDEX 'IXFK_PipingCalculationEntity_PipingSemiProbabilisticOutputEntity'
 ON 'PipingCalculationEntity' ('PipingSemiProbabilisticOutputEntityId' ASC)
;

CREATE INDEX 'IXFK_PipingCalculationEntity_StochasticSoilProfileEntity'
 ON 'PipingCalculationEntity' ('StochasticSoilProfileEntityId' ASC)
;

CREATE INDEX 'IXFK_PipingCalculationEntity_SurfaceLineEntity'
 ON 'PipingCalculationEntity' ('SurfaceLineEntityId' ASC)
;

CREATE INDEX 'IXFK_PipingCalculationEntity_HydraulicLocationEntity'
 ON 'PipingCalculationEntity' ('HydraulicLocationEntityId' ASC)
;

CREATE INDEX 'IXFK_PipingCalculationEntity_CalculationGroupEntity'
 ON 'PipingCalculationEntity' ('CalculationGroupEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionInwardsFailureMechanismMetaEntity_FailureMechanismEntity'
 ON 'GrassCoverErosionInwardsFailureMechanismMetaEntity' ('FailureMechanismEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionInwardsCalculationEntity_CalculationGroupEntity'
 ON 'GrassCoverErosionInwardsCalculationEntity' ('CalculationGroupEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionInwardsCalculationEntity_DikeProfileEntity'
 ON 'GrassCoverErosionInwardsCalculationEntity' ('DikeProfileEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionInwardsCalculationEntity_GrassCoverErosionInwardsOutputEntity'
 ON 'GrassCoverErosionInwardsCalculationEntity' ('GrassCoverErosionInwardsOutputEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionInwardsCalculationEntity_HydraulicLocationEntity'
 ON 'GrassCoverErosionInwardsCalculationEntity' ('HydraulicLocationEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionOutwardsFailureMechanismMetaEntity_FailureMechanismEntity'
 ON 'GrassCoverErosionOutwardsFailureMechanismMetaEntity' ('FailureMechanismEntityId' ASC)
;

CREATE INDEX 'IXFK_StochasticSoilProfileEntity_SoilProfileEntity'
 ON 'StochasticSoilProfileEntity' ('SoilProfileEntityId' ASC)
;

CREATE INDEX 'IXFK_StochasticSoilProfileEntity_StochasticSoilModelEntity'
 ON 'StochasticSoilProfileEntity' ('StochasticSoilModelEntityId' ASC)
;

CREATE INDEX 'IXFK_StochasticSoilModelEntity_FailureMechanismEntity'
 ON 'StochasticSoilModelEntity' ('FailureMechanismEntityId' ASC)
;

CREATE INDEX 'IXFK_SurfaceLineEntity_FailureMechanismEntity'
 ON 'SurfaceLineEntity' ('FailureMechanismEntityId' ASC)
;

CREATE INDEX 'IXFK_SurfaceLinePointEntity_SurfaceLineEntity'
 ON 'CharacteristicPointEntity' ('SurfaceLineEntityId' ASC)
;

CREATE INDEX 'IXFK_PipingSectionResultEntity_FailureMechanismSectionEntity'
 ON 'PipingSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionInwardsSectionResultEntity_FailureMechanismSectionEntity'
 ON 'GrassCoverErosionInwardsSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionInwardsSectionResultEntity_GrassCoverErosionInwardsCalculationEntity'
 ON 'GrassCoverErosionInwardsSectionResultEntity' ('GrassCoverErosionInwardsCalculationEntityId' ASC)
;

CREATE INDEX 'IXFK_HeightStructuresSectionResultEntity_FailureMechanismSectionEntity'
 ON 'HeightStructuresSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_StrengthStabilityLengthwiseConstructionSectionResultEntity_FailureMechanismSectionEntity'
 ON 'StrengthStabilityLengthwiseConstructionSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_TechnicalInnovationSectionResultEntity_FailureMechanismSectionEntity'
 ON 'TechnicalInnovationSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_WaterPressureAsphaltCoverSectionResultEntity_FailureMechanismSectionEntity'
 ON 'WaterPressureAsphaltCoverSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_ClosingStructureSectionResultEntity_FailureMechanismSectionEntity'
 ON 'ClosingStructureSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionOutwardsSectionResultEntity_FailureMechanismSectionEntity'
 ON 'GrassCoverErosionOutwardsSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverSlipOffInwardsSectionResultEntity_FailureMechanismSectionEntity'
 ON 'GrassCoverSlipOffInwardsSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverSlipOffOutwardsSectionResultEntity_FailureMechanismSectionEntity'
 ON 'GrassCoverSlipOffOutwardsSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_MacrostabilityInwardsSectionResultEntity_FailureMechanismSectionEntity'
 ON 'MacrostabilityInwardsSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_MacrostabilityOutwardsSectionResultEntity_FailureMechanismSectionEntity'
 ON 'MacrostabilityOutwardsSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_WaveImpactAsphaltCoverSectionResultEntity_FailureMechanismSectionEntity'
 ON 'WaveImpactAsphaltCoverSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_MicrostabilitySectionResultEntity_FailureMechanismSectionEntity'
 ON 'MicrostabilitySectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_PipingStructureSectionResultEntity_FailureMechanismSectionEntity'
 ON 'PipingStructureSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_DuneErosionSectionResultEntity_FailureMechanismSectionEntity'
 ON 'DuneErosionSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_StabilityStoneCoverSectionResultEntity_FailureMechanismSectionEntity'
 ON 'StabilityStoneCoverSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_StrengthStabilityPointConstructionSectionResultEntity_FailureMechanismSectionEntity'
 ON 'StrengthStabilityPointConstructionSectionResultEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_DikeProfileEntity_FailureMechanismEntity'
 ON 'DikeProfileEntity' ('FailureMechanismEntityId' ASC)
;

CREATE INDEX 'IXFK_GrassCoverErosionInwardsOutput_ProbabilisticOutputEntity'
 ON 'GrassCoverErosionInwardsOutputEntity' ('ProbabilisticOutputEntityId' ASC)
;
