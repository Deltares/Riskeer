--
-- File generated with SQLiteStudio v3.0.7 on Thu Feb 11 11:54:23 2016
--
-- Text encoding used: windows-1252
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Version
DROP TABLE IF EXISTS Version;

CREATE TABLE Version (
    VersionId   INTEGER      PRIMARY KEY AUTOINCREMENT
                             NOT NULL,
    FromVersion VARCHAR (16),
    ToVersion   VARCHAR (16),
    Timestamp   NUMERIC
);


-- Table: DuneAssessmentSectionEntity
DROP TABLE IF EXISTS DuneAssessmentSectionEntity;

CREATE TABLE DuneAssessmentSectionEntity (
    DuneAssessmentSectionEntityId INTEGER       PRIMARY KEY AUTOINCREMENT
                                                NOT NULL,
    ProjectEntityId               INTEGER       REFERENCES ProjectEntity (ProjectEntityId) ON DELETE CASCADE
                                                                                           ON UPDATE CASCADE
                                                NOT NULL,
    Name                          VARCHAR (260) NOT NULL,
    Norm                          INT           NOT NULL,
    [Order]                       INT           NOT NULL
);


-- Table: DikeAssessmentSectionEntity
DROP TABLE IF EXISTS DikeAssessmentSectionEntity;

CREATE TABLE DikeAssessmentSectionEntity (
    DikeAssessmentSectionEntityId INTEGER       PRIMARY KEY AUTOINCREMENT
                                                NOT NULL,
    ProjectEntityId               INTEGER       REFERENCES ProjectEntity (ProjectEntityId) ON DELETE CASCADE
                                                                                           ON UPDATE CASCADE
                                                NOT NULL,
    Name                          VARCHAR (260) NOT NULL,
    Norm                          INT           NOT NULL,
    [Order]                       INT (0)       NOT NULL
);


-- Table: ProjectEntity
DROP TABLE IF EXISTS ProjectEntity;

CREATE TABLE ProjectEntity (
    ProjectEntityId INTEGER           NOT NULL
                                      PRIMARY KEY AUTOINCREMENT,
    Description     TEXT (2147483647),
    LastUpdated     INTEGER           DEFAULT (CURRENT_TIMESTAMP) 
);


-- Table: FailureMechanismEntity
DROP TABLE IF EXISTS FailureMechanismEntity;

CREATE TABLE FailureMechanismEntity (
    FailureMechanismEntityId      INTEGER NOT NULL
                                          PRIMARY KEY AUTOINCREMENT,
    DikeAssessmentSectionEntityId INTEGER REFERENCES DikeAssessmentSectionEntity (DikeAssessmentSectionEntityId) ON DELETE CASCADE
                                                                                                                 ON UPDATE CASCADE
                                          NOT NULL,
    FailureMechanismType          INT (2) NOT NULL,
    UNIQUE (
        DikeAssessmentSectionEntityId,
        FailureMechanismType
    )
    ON CONFLICT REPLACE
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
