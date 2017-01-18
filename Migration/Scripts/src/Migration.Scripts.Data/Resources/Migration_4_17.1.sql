ATTACH [{0}] AS SOURCEPROJECT;

INSERT INTO VersionEntity (
  [VersionId],
  [Version],
  [Timestamp],
  [FingerPrint])
SELECT
  [VersionId],
  "17.1",
  [Timestamp],
  [FingerPrint]
FROM [SOURCEPROJECT].VersionEntity;

INSERT INTO ProjectEntity
SELECT * FROM [SOURCEPROJECT].ProjectEntity;

INSERT INTO AssessmentSectionEntity (
  [AssessmentSectionEntityId],
  [ProjectEntityId],
  [Id],
  [Name],
  [Comments],
  [Norm],
  [HydraulicDatabaseVersion],
  [HydraulicDatabaseLocation],
  [Composition],
  [ReferenceLinePointXml],
  [Order])
SELECT 
  [AssessmentSectionEntityId],
  [ProjectEntityId],
  [Id],
  [Name],
  [Comments],
  [Norm],
  [HydraulicDatabaseVersion],
  [HydraulicDatabaseLocation],
  [Composition],
  [ReferenceLinePointXml],
  [Order] 
FROM [SOURCEPROJECT].AssessmentSectionEntity;

INSERT INTO HydraulicLocationEntity
SELECT * FROM [SOURCEPROJECT].HydraulicLocationEntity;

INSERT INTO HydraulicLocationOutputEntity
SELECT * FROM [SOURCEPROJECT].HydraulicLocationOutputEntity;