--
-- File generated with SQLiteStudio v3.0.7 on Fri Jan 29 11:47:02 2016
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


-- Table: ProjectEntity
DROP TABLE IF EXISTS ProjectEntity;

CREATE TABLE ProjectEntity (
    ProjectEntityId INTEGER           NOT NULL
                                      PRIMARY KEY AUTOINCREMENT,
    Description     TEXT (2147483647),
    LastUpdated     INTEGER           DEFAULT (CURRENT_TIMESTAMP) 
);


-- Table: DikeAssessmentSectionEntity
DROP TABLE IF EXISTS DikeAssessmentSectionEntity;

CREATE TABLE DikeAssessmentSectionEntity (
    DikeAssessmentSectionEntityId INTEGER       PRIMARY KEY AUTOINCREMENT
                                                NOT NULL,
    ProjectEntityId               INTEGER       REFERENCES ProjectEntity (ProjectEntityId) ON DELETE CASCADE
                                                                                           ON UPDATE CASCADE
                                                NOT NULL,
    Name                          VARCHAR (260),
    Norm                          INT
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
