/* ---------------------------------------------------- */
/*  Generated by Enterprise Architect Version 12.0 		*/
/*  Created On : 06-apr-2016 9:02:20 				*/
/*  DBMS       : SQLite 								*/
/* ---------------------------------------------------- */

/* Drop Tables */

DROP TABLE IF EXISTS 'VersionEntity'
;

DROP TABLE IF EXISTS 'ProjectEntity'
;

DROP TABLE IF EXISTS 'AssessmentSectionEntity'
;

DROP TABLE IF EXISTS 'FailureMechanismEntity'
;

DROP TABLE IF EXISTS 'HydraulicLocationEntity'
;

DROP TABLE IF EXISTS 'ReferenceLinePointEntity'
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

CREATE TABLE 'AssessmentSectionEntity'
(
	'AssessmentSectionEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'ProjectEntityId' INTEGER NOT NULL,
	'Name' VARCHAR (260) NOT NULL,
	'Norm' INT (4) NOT NULL,
	'Order' INT (4) NOT NULL,
	'HydraulicDatabaseVersion' TEXT,
	'HydraulicDatabaseLocation' TEXT,
	'Composition' SMALLINT NOT NULL, -- Enum: Dike = 0, Dune = 1, DikeAndDune = 2
	CONSTRAINT 'FK_AssessmentSectionEntity_ProjectEntity' FOREIGN KEY ('ProjectEntityId') REFERENCES 'ProjectEntity' ('ProjectEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'FailureMechanismEntity'
(
	'FailureMechanismEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'AssessmentSectionEntityId' INTEGER NOT NULL,
	'FailureMechanismType' SMALLINT NOT NULL, -- Enumerator for different failure mechanism types (piping, macrostability, dunes, etc)
	CONSTRAINT 'FK_FailureMechanismEntity_AssessmentSectionEntity' FOREIGN KEY ('AssessmentSectionEntityId') REFERENCES 'AssessmentSectionEntity' ('AssessmentSectionEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'UI_AssessmentSectionEntityId_FailureMechanismType' UNIQUE ('FailureMechanismType','AssessmentSectionEntityId')
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

/* Create Indexes and Triggers */

CREATE INDEX 'IXFK_AssessmentSectionEntity_ProjectEntity'
 ON 'AssessmentSectionEntity' ('ProjectEntityId' ASC)
;

CREATE INDEX 'IXFK_FailureMechanismEntity_AssessmentSectionEntity'
 ON 'FailureMechanismEntity' ('AssessmentSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_HydraulicLocationEntity_AssessmentSectionEntity'
 ON 'HydraulicLocationEntity' ('AssessmentSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_ReferenceLinePointEntity_AssessmentSectionEntity'
 ON 'ReferenceLinePointEntity' ('AssessmentSectionEntityId' ASC)
;
