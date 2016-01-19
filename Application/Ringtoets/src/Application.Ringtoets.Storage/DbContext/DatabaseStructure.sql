--
-- File generated with SQLiteStudio v3.0.7 on Fri Jan 15 14:48:04 2016
--
-- Text encoding used: windows-1252
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Version
CREATE TABLE IF NOT EXISTS Version (VersionId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, FromVersion VARCHAR (50), ToVersion VARCHAR (50), Timestamp NUMERIC);

-- Table: ProjectEntity
CREATE TABLE IF NOT EXISTS ProjectEntity (ProjectEntityId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Name VARCHAR (1024), Description VARCHAR (1024), LastUpdated INTEGER DEFAULT (CURRENT_TIMESTAMP));

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
