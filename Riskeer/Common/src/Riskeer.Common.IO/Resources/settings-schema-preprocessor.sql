BEGIN TRANSACTION;
CREATE TABLE "TimeIntegrationSettings" (
                "LocationID" INTEGER NOT NULL,
                "CalculationTypeID" INTEGER NOT NULL,
                "TimeIntegrationSchemeID" INTEGER NOT NULL,
	CONSTRAINT timeintegrationsettings_pk PRIMARY KEY ("LocationID", "CalculationTypeID"),
	CONSTRAINT calculationtypes_timeintegrationsettings_fk FOREIGN KEY ("CalculationTypeID") REFERENCES CalculationTypes ("CalculationTypeID") ON DELETE NO ACTION ON UPDATE NO ACTION
);
CREATE TABLE "NumericsSettings" (
                "LocationId" INTEGER NOT NULL,
                "MechanismID" INTEGER NOT NULL,
                "SubMechanismID" INTEGER NOT NULL,
                "CalculationMethod" INTEGER NOT NULL,
                "FORM_StartMethod" INTEGER NOT NULL,
                "FORM_NIterations" INTEGER NOT NULL,
                "FORM_RelaxationFactor" REAL NOT NULL,
                "FORM_EpsBeta" REAL NOT NULL,
                "FORM_EpsHOH" REAL NOT NULL,
                "FORM_EpsZFunc" REAL NOT NULL,
                "DS_StartMethod" INTEGER NOT NULL,
                "DS_Min" INTEGER NOT NULL,
                "DS_Max" INTEGER NOT NULL,
                "DS_VarCoefficient" REAL NOT NULL,
                "NI_UMin" REAL NOT NULL,
                "NI_UMax" REAL NOT NULL,
                "NI_NumberSteps" INTEGER NOT NULL,
	CONSTRAINT numericsettings_pk PRIMARY KEY ("LocationId", "MechanismID", "SubMechanismID")
);
CREATE TABLE "General" (
                "CreationDate" VARCHAR(20),
                "HRDName" VARCHAR(60) NOT NULL,
                "HRDCreationDate" VARCHAR(60),
                "HRDTrackID" INTEGER NOT NULL,
                "HRDNameRegion" VARCHAR(20) NOT NULL
);
CREATE TABLE "ExcludedWindDirections" (
                "CalculationTypeID" INTEGER NOT NULL,
                "LocationID" INTEGER NOT NULL,
                "HRDWindDirectionID" INTEGER NOT NULL,
	CONSTRAINT excludedwinddirections_pk PRIMARY KEY ("CalculationTypeID", "LocationID", "HRDWindDirectionID"),
	CONSTRAINT calculationtypes_excludedwinddirections_fk FOREIGN KEY ("CalculationTypeID") REFERENCES CalculationTypes ("CalculationTypeID") ON DELETE NO ACTION ON UPDATE NO ACTION
);
CREATE TABLE "ExcludedLocations" (
                "LocationID" INTEGER NOT NULL,
	CONSTRAINT excludedlocations_pk PRIMARY KEY ("LocationID")
);
CREATE TABLE "DesignTablesSettings" (
                "LocationID" INTEGER NOT NULL,
                "CalculationTypeID" INTEGER NOT NULL,
                "Min" REAL NOT NULL,
                "Max" REAL NOT NULL,
	CONSTRAINT designtablessettings_pk PRIMARY KEY ("LocationID", "CalculationTypeID"),
	CONSTRAINT calculationtypes_designtablessettings_fk FOREIGN KEY ("CalculationTypeID") REFERENCES CalculationTypes ("CalculationTypeID") ON DELETE NO ACTION ON UPDATE NO ACTION
);
CREATE TABLE "CalculationTypes" (
                "CalculationTypeID" INTEGER NOT NULL,
                "CalculationType" INTEGER NOT NULL,
	CONSTRAINT calculationtypes_pk PRIMARY KEY ("CalculationTypeID")
);
CREATE TABLE "ExcludedLocationsPreprocessor" (
                "LocationID" INTEGER NOT NULL,
             CONSTRAINT excludedPreprocessorLocations_pk PRIMARY KEY ("LocationID")
);
CREATE TABLE "PreprocessorSettings" (
                "LocationID" INTEGER NOT NULL,
                "MinValueRunPreprocessor" REAL NOT NULL,
                "MaxValueRunPreprocessor" REAL NOT NULL,
             CONSTRAINT preprocessorSettingsLocatoins_pk PRIMARY KEY ("LocationID")
);
COMMIT;
