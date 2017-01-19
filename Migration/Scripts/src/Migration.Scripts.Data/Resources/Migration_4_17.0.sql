CREATE TABLE 'InvalidVersionScriptCalled'
(
	'VersionId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	'Version' INTEGER NOT NULL
)
;

INSERT INTO 'InvalidVersionScriptCalled' SET 'Version' = "17.0";