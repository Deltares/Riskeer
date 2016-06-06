/* ---------------------------------------------------- */
/*  Generated by Enterprise Architect Version 12.0 		*/
/*  Created On : 06-Jun-2016 09:07:52 				*/
/*  DBMS       : SQLite 								*/
/* ---------------------------------------------------- */

/* Drop Tables */

DROP TABLE IF EXISTS 'VersionEntity'
;

DROP TABLE IF EXISTS 'ProjectEntity'
;

DROP TABLE IF EXISTS 'CharacteristicPointEntity'
;

DROP TABLE IF EXISTS 'AssessmentSectionEntity'
;

DROP TABLE IF EXISTS 'FailureMechanismSectionEntity'
;

DROP TABLE IF EXISTS 'FailureMechanismEntity'
;

DROP TABLE IF EXISTS 'FailureMechanismSectionPointEntity'
;

DROP TABLE IF EXISTS 'CalculationGroupEntity'
;

DROP TABLE IF EXISTS 'HydraulicLocationEntity'
;

DROP TABLE IF EXISTS 'PipingCalculationEntity'
;

DROP TABLE IF EXISTS 'ReferenceLinePointEntity'
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

DROP TABLE IF EXISTS 'SurfaceLinePointEntity'
;

DROP TABLE IF EXISTS 'PipingFailureMechanismMetaEntity'
;

DROP TABLE IF EXISTS 'StochasticSoilModelSegmentPointEntity'
;

/* Create Tables with Primary and Foreign Keys, Check and Unique Constraints */

CREATE TABLE 'VersionEntity'
(
	'VersionId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FromVersion' TEXT,
	'ToVersion' TEXT,
	'Timestamp' NUMERIC
)
;

CREATE TABLE 'ProjectEntity'
(
	'ProjectEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'Description' VARCHAR (260)
)
;

CREATE TABLE 'CharacteristicPointEntity'
(
	'CharacteristicPointEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'CharacteristicPointType' SMALLINT NOT NULL, -- Enum: 1 = DikeToeAtRiver 2 = DikeToeAtPolder 3 = DitchDikeSide 4 = BottomDitchDikeSide 5 = BottomDitchPolderSide 6 = DitchPolderSide
	'SurfaceLinePointEntityId' INTEGER NOT NULL,
	CONSTRAINT 'FK_CharacteristicPointEntity_SurfaceLinePointEntity' FOREIGN KEY ('SurfaceLinePointEntityId') REFERENCES 'SurfaceLinePointEntity' ('SurfaceLinePointEntityId') ON DELETE No Action ON UPDATE No Action
)
;

CREATE TABLE 'AssessmentSectionEntity'
(
	'AssessmentSectionEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'ProjectEntityId' INTEGER NOT NULL,
	'Name' VARCHAR (260),
	'Comments' TEXT,
	'Norm' INT (4) NOT NULL,
	'HydraulicDatabaseVersion' TEXT,
	'HydraulicDatabaseLocation' TEXT,
	'Composition' SMALLINT NOT NULL, -- Enum: Dike = 0, Dune = 1, DikeAndDune = 2
	CONSTRAINT 'FK_AssessmentSectionEntity_ProjectEntity' FOREIGN KEY ('ProjectEntityId') REFERENCES 'ProjectEntity' ('ProjectEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'FailureMechanismSectionEntity'
(
	'FailureMechanismSectionEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	'Name' VARCHAR (260) NOT NULL,
	CONSTRAINT 'FK_FailureMechanismSectionEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'FailureMechanismEntity'
(
	'FailureMechanismEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'AssessmentSectionEntityId' INTEGER NOT NULL,
	'CalculationGroupEntityId' INTEGER,
	'FailureMechanismType' SMALLINT NOT NULL, -- Enumerator for different failure mechanism types (piping, macrostability, dunes, etc)  1 = Piping 2 = Macrostabiliteit binnenwaarts 3= Golfklappen op asfaltbekleding 4= Grasbekleding erosie buitentalud 5 = Grasbekleding afschuiven buitentalud 6 = Grasbekleding erosie kruin en binnentalud 7 = Stabiliteit steenzetting 8 = Duinafslag 9 = Hoogte kunstwerk 10 = Betrouwbaarheid sluiten kunstwerk 11 = Piping bij kunstwerk 12 = Sterkte en stabiliteit puntconstructires 13 = Macrostabiliteit buitenwaarts 14 = Microstabiliteit 15 = Wateroverdruk bij asfaltbekleding 16 = Grasbekleding afschuiven binnentalud 17 = Sterkte en stabiliteit langsconstructires 18 = Technische innovaties
	'IsRelevant' TINYINT (1) NOT NULL, -- true or false
	CONSTRAINT 'FK_FailureMechanismEntity_AssessmentSectionEntity' FOREIGN KEY ('AssessmentSectionEntityId') REFERENCES 'AssessmentSectionEntity' ('AssessmentSectionEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'FK_FailureMechanismEntity_CalculationGroupEntity' FOREIGN KEY ('CalculationGroupEntityId') REFERENCES 'CalculationGroupEntity' ('CalculationGroupEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'UI_AssessmentSectionEntityId_FailureMechanismType' UNIQUE ('AssessmentSectionEntityId','FailureMechanismType')
)
;

CREATE TABLE 'FailureMechanismSectionPointEntity'
(
	'FailureMechanismSectionPointEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismSectionEntityId' INTEGER NOT NULL,
	'X' NUMERIC NOT NULL,
	'Y' NUMERIC NOT NULL,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_FailureMechanismSectionPointEntity_FailureMechanismSectionEntity' FOREIGN KEY ('FailureMechanismSectionEntityId') REFERENCES 'FailureMechanismSectionEntity' ('FailureMechanismSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
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
	'LocationX' NUMERIC NOT NULL,
	'LocationY' NUMERIC NOT NULL,
	'DesignWaterLevel' REAL,
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
	'Order' INT (4) NOT NULL,
	'Name' VARCHAR (260),
	'Comments' TEXT,
	'EntryPointL' NUMERIC,
	'ExitPointL' NUMERIC,
	'PhreaticLevelExitMean' NUMERIC NOT NULL,
	'PhreaticLevelExitStandardDeviation' NUMERIC NOT NULL,
	'Diameter70Mean' NUMERIC NOT NULL,
	'Diameter70StandardDeviation' NUMERIC NOT NULL,
	'DarcyPermeabilityMean' NUMERIC NOT NULL,
	'DarcyPermeabilityStandardDeviation' NUMERIC NOT NULL,
	'DampingFactorExitMean' NUMERIC NOT NULL,
	'DampingFactorExitStandardDeviation' NUMERIC NOT NULL,
	'SaturatedVolumicWeightOfCoverageLayerMean' NUMERIC NOT NULL,
	'SaturatedVolumicWeightOfCoverageLayerStandardDeviation' NUMERIC NOT NULL,
	'SaturatedVolumicWeightOfCoverageLayerShift' NUMERIC NOT NULL,
	'RelevantForScenario' TINYINT (1) NOT NULL,
	'ScenarioContribution' NUMERIC NOT NULL,
	CONSTRAINT 'FK_PipingCalculationEntity_CalculationGroupEntity' FOREIGN KEY ('CalculationGroupEntityId') REFERENCES 'CalculationGroupEntity' ('CalculationGroupEntityId') ON DELETE No Action ON UPDATE No Action,
	CONSTRAINT 'FK_PipingCalculationEntity_HydraulicLocationEntity' FOREIGN KEY ('HydraulicLocationEntityId') REFERENCES 'HydraulicLocationEntity' ('HydraulicLocationEntityId') ON DELETE No Action ON UPDATE No Action,
	CONSTRAINT 'FK_PipingCalculationEntity_StochasticSoilProfileEntity' FOREIGN KEY ('StochasticSoilProfileEntityId') REFERENCES 'StochasticSoilProfileEntity' ('StochasticSoilProfileEntityId') ON DELETE No Action ON UPDATE No Action,
	CONSTRAINT 'FK_PipingCalculationEntity_SurfaceLineEntity' FOREIGN KEY ('SurfaceLineEntityId') REFERENCES 'SurfaceLineEntity' ('SurfaceLineEntityId') ON DELETE No Action ON UPDATE No Action
)
;

CREATE TABLE 'ReferenceLinePointEntity'
(
	'ReferenceLinePointEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'AssessmentSectionEntityId' INTEGER NOT NULL,
	'X' NUMERIC NOT NULL,
	'Y' NUMERIC NOT NULL,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_ReferenceLinePointEntity_AssessmentSectionEntity' FOREIGN KEY ('AssessmentSectionEntityId') REFERENCES 'AssessmentSectionEntity' ('AssessmentSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'SoilLayerEntity'
(
	'SoilLayerEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'SoilProfileEntityId' INTEGER NOT NULL,
	'Top' NUMERIC NOT NULL,
	'IsAquifer' TINYINT (1) NOT NULL, -- true or false
	CONSTRAINT 'FK_SoilLayerEntity_SoilProfileEntity' FOREIGN KEY ('SoilProfileEntityId') REFERENCES 'SoilProfileEntity' ('SoilProfileEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'SoilProfileEntity'
(
	'SoilProfileEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'Bottom' NUMERIC NOT NULL,
	'Name' TEXT
)
;

CREATE TABLE 'StochasticSoilProfileEntity'
(
	'StochasticSoilProfileEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'SoilProfileEntityId' INTEGER NOT NULL,
	'StochasticSoilModelEntityId' INTEGER NOT NULL,
	'Probability' NUMERIC NOT NULL,
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
	CONSTRAINT 'FK_StochasticSoilModelEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'SurfaceLineEntity'
(
	'SurfaceLineEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER NOT NULL,
	'Name' VARCHAR (260),
	'ReferenceLineIntersectionX' NUMERIC NOT NULL,
	'ReferenceLineIntersectionY' NUMERIC NOT NULL,
	CONSTRAINT 'FK_SurfaceLineEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE No Action ON UPDATE No Action
)
;

CREATE TABLE 'SurfaceLinePointEntity'
(
	'SurfaceLinePointEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'SurfaceLineEntityId' INTEGER NOT NULL,
	'X' NUMERIC NOT NULL,
	'Y' NUMERIC NOT NULL,
	'Order' INT (4) NOT NULL,
	'Z' NUMERIC NOT NULL,
	CONSTRAINT 'FK_SurfaceLinePointEntity_SurfaceLineEntity' FOREIGN KEY ('SurfaceLineEntityId') REFERENCES 'SurfaceLineEntity' ('SurfaceLineEntityId') ON DELETE No Action ON UPDATE No Action
)
;

CREATE TABLE 'PipingFailureMechanismMetaEntity'
(
	'PipingFailureMechanismMetaEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'FailureMechanismEntityId' INTEGER,
	'A' NUMERIC NOT NULL,
	CONSTRAINT 'FK_PipingFailureMechanismMetaEntity_FailureMechanismEntity' FOREIGN KEY ('FailureMechanismEntityId') REFERENCES 'FailureMechanismEntity' ('FailureMechanismEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'StochasticSoilModelSegmentPointEntity'
(
	'StochasticSoilModelSegmentPointEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'StochasticSoilModelEntityId' INTEGER NOT NULL,
	'X' NUMERIC NOT NULL,
	'Y' NUMERIC NOT NULL,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_StochasticSoilModelSegmentPointEntity_StochasticSoilModelEntity' FOREIGN KEY ('StochasticSoilModelEntityId') REFERENCES 'StochasticSoilModelEntity' ('StochasticSoilModelEntityId') ON DELETE No Action ON UPDATE No Action
)
;

/* Create Indexes and Triggers */

CREATE INDEX 'IXFK_CharacteristicPointEntity_SurfaceLinePointEntity'
 ON 'CharacteristicPointEntity' ('SurfaceLinePointEntityId' ASC)
;

CREATE INDEX 'IXFK_AssessmentSectionEntity_ProjectEntity'
 ON 'AssessmentSectionEntity' ('ProjectEntityId' ASC)
;

CREATE INDEX 'IXFK_FailureMechanismSectionEntity_FailureMechanismEntity'
 ON 'FailureMechanismSectionEntity' ('FailureMechanismEntityId' ASC)
;

CREATE INDEX 'IXFK_FailureMechanismEntity_AssessmentSectionEntity'
 ON 'FailureMechanismEntity' ('AssessmentSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_FailureMechanismEntity_CalculationGroupEntity'
 ON 'FailureMechanismEntity' ('CalculationGroupEntityId' ASC)
;

CREATE INDEX 'IXFK_FailureMechanismSectionPointEntity_FailureMechanismSectionEntity'
 ON 'FailureMechanismSectionPointEntity' ('FailureMechanismSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_CalculationGroupEntity_CalculationGroupEntity'
 ON 'CalculationGroupEntity' ('ParentCalculationGroupEntityId' ASC)
;

CREATE INDEX 'IXFK_HydraulicLocationEntity_AssessmentSectionEntity'
 ON 'HydraulicLocationEntity' ('AssessmentSectionEntityId' ASC)
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

CREATE INDEX 'IXFK_ReferenceLinePointEntity_AssessmentSectionEntity'
 ON 'ReferenceLinePointEntity' ('AssessmentSectionEntityId' ASC)
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
 ON 'SurfaceLinePointEntity' ('SurfaceLineEntityId' ASC)
;

CREATE INDEX 'IXFK_PipingFailureMechanismMetaEntity_FailureMechanismEntity'
 ON 'PipingFailureMechanismMetaEntity' ('FailureMechanismEntityId' ASC)
;
