ATTACH DATABASE [{0}] AS SOURCEPROJECT;

CREATE TABLE 'InvalidVersionScriptCalled'
(
	'VersionId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'Version' INTEGER NOT NULL
)
;

INSERT INTO 'InvalidVersionScriptCalled' ('Version') VALUES ("17.0");
INSERT INTO VersionEntity (
	[VersionId], 
	[Version], 
	[Timestamp], 
	[FingerPrint])
SELECT [VersionId],
	"17.0", 
	[Timestamp], 
	[FingerPrint]
	FROM [SOURCEPROJECT].VersionEntity;