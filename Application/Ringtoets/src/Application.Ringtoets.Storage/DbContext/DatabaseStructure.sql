/* ---------------------------------------------------- */
/*  Generated by Enterprise Architect Version 12.0 		*/
/*  Created On : 25-Mar-2016 4:01:03 PM 				*/
/*  DBMS       : SQLite 								*/
/* ---------------------------------------------------- */

/* Drop Tables */

DROP TABLE IF EXISTS 'Version'
;

DROP TABLE IF EXISTS 'ProjectEntity'
;

DROP TABLE IF EXISTS 'DikeAssessmentSectionEntity'
;

DROP TABLE IF EXISTS 'FailureMechanismEntity'
;

DROP TABLE IF EXISTS 'HydraulicLocationEntity'
;

DROP TABLE IF EXISTS 'ReferenceLinePointEntity'
;

/* Create Tables with Primary and Foreign Keys, Check and Unique Constraints */

CREATE TABLE 'Version'
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

CREATE TABLE 'DikeAssessmentSectionEntity'
(
	'DikeAssessmentSectionEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'ProjectEntityId' INTEGER NOT NULL,
	'Name' VARCHAR (260) NOT NULL,
	'Norm' INT (4) NOT NULL,
	'Order' INT (4) NOT NULL,
	'HydraulicDatabaseVersion' TEXT,
	'HydraulicDatabaseLocation' TEXT,
	CONSTRAINT 'FK_AssessmentSection_ProjectEntity' FOREIGN KEY ('ProjectEntityId') REFERENCES 'ProjectEntity' ('ProjectEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'FailureMechanismEntity'
(
	'FailureMechanismEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'DikeAssessmentSectionEntityId' INTEGER NOT NULL,
	'FailureMechanismType' SMALLINT NOT NULL, -- Enumerator for different failure mechanism types (piping, macrostability, dunes, etc)
	CONSTRAINT 'FK_FailureMechanismEntity_AssessmentSectionEntity' FOREIGN KEY ('DikeAssessmentSectionEntityId') REFERENCES 'DikeAssessmentSectionEntity' ('DikeAssessmentSectionEntityId') ON DELETE Cascade ON UPDATE Cascade,
	CONSTRAINT 'UI_AS_FMT' UNIQUE ('FailureMechanismType','DikeAssessmentSectionEntityId')
)
;

CREATE TABLE 'HydraulicLocationEntity'
(
	'HydraulicLocationEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'DikeAssessmentSectionEntityId' INTEGER NOT NULL,
	'LocationId' INTEGER NOT NULL,
	'Name' VARCHAR (260) NOT NULL,
	'LocationX' NUMERIC NOT NULL,
	'LocationY' NUMERIC NOT NULL,
	'DesignWaterLevel' REAL,
	CONSTRAINT 'FK_HydraulicLocationEntity_DikeAssessmentSectionEntity' FOREIGN KEY ('DikeAssessmentSectionEntityId') REFERENCES 'DikeAssessmentSectionEntity' ('DikeAssessmentSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

CREATE TABLE 'ReferenceLinePointEntity'
(
	'ReferenceLinePointEntityId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'DikeAssessmentSectionEntityId' INTEGER NOT NULL,
	'X' NUMERIC NOT NULL,
	'Y' NUMERIC NOT NULL,
	'Order' INT (4) NOT NULL,
	CONSTRAINT 'FK_ReferenceLinePointEntity_DikeAssessmentSectionEntity' FOREIGN KEY ('DikeAssessmentSectionEntityId') REFERENCES 'DikeAssessmentSectionEntity' ('DikeAssessmentSectionEntityId') ON DELETE Cascade ON UPDATE Cascade
)
;

/* Create Indexes and Triggers */

CREATE INDEX 'IXFK_AssessmentSection_ProjectEntity'
 ON 'DikeAssessmentSectionEntity' ('ProjectEntityId' ASC)
;

CREATE INDEX 'IXFK_FailureMechanismEntity_AssessmentSectionEntity'
 ON 'FailureMechanismEntity' ('DikeAssessmentSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_HydraulicLocationEntity_DikeAssessmentSectionEntity'
 ON 'HydraulicLocationEntity' ('DikeAssessmentSectionEntityId' ASC)
;

CREATE INDEX 'IXFK_ReferenceLinePointEntity_DikeAssessmentSectionEntity'
 ON 'ReferenceLinePointEntity' ('DikeAssessmentSectionEntityId' ASC)
;
