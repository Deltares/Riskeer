--
-- File generated with SQLiteStudio v3.0.7 on Mon Jan 25 12:12:02 2016
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


-- Table: DikeAssessmentSectionEntity
DROP TABLE IF EXISTS DikeAssessmentSectionEntity;

CREATE TABLE DikeAssessmentSectionEntity (
    DikeAssessmentSectionEntityId INTEGER        PRIMARY KEY AUTOINCREMENT
                                                 NOT NULL,
    ProjectEntityId               INTEGER        REFERENCES ProjectEntity (ProjectEntityId) ON DELETE CASCADE
                                                                                            ON UPDATE CASCADE
                                                 NOT NULL,
    Name                          VARCHAR (260),
    Norm                          INT
);


-- Table: ProjectEntity
DROP TABLE IF EXISTS ProjectEntity;

CREATE TABLE ProjectEntity (
    ProjectEntityId INTEGER        NOT NULL
                                   PRIMARY KEY AUTOINCREMENT,
    Name            VARCHAR (260),
    Description     TEXT (0),
    LastUpdated     INTEGER        DEFAULT (CURRENT_TIMESTAMP) 
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
