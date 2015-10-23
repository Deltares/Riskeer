﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Xml;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO
{
    /// <summary>
    /// This class reads a SqLite database file and constructs <see cref="PipingSoilProfile"/> from this database.
    /// The database is created with the DSoilModel application.
    /// </summary>
    public class PipingSoilProfileReader : IDisposable
    {
        private const int pipingMechanismId = 4;

        private SQLiteConnection connection;
        private SQLiteDataReader dataReader;
        private const string profileNameColumn = "ProfileName";
        private const string intersectionXColumn = "IntersectionX";
        private const string layerGeometryColumn = "LayerGeometry";
        private const string abovePhreaticLevelColumn = "AbovePhreaticLevel";
        private const string belowPhreaticLevelColumn = "BelowPhreaticLevel";
        private const string permeabKxColumn = "PermeabKx";
        private const string diameterD70Column = "DiameterD70";
        private const string whitesConstantColumn = "WhitesConstant";
        private const string beddingAngleColumn = "BeddingAngle";

        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilProfileReader"/> which will use the <paramref name="dbFile"/>
        /// as its source.
        /// </summary>
        /// <param name="dbFile"></param>
        public PipingSoilProfileReader(string dbFile)
        {
            if (String.IsNullOrEmpty(dbFile))
            {
                throw new ArgumentException(Resources.Error_PathMustBeSpecified);
            }
            if (!File.Exists(dbFile))
            {
                throw new FileNotFoundException(String.Format(Resources.Error_File_0_does_not_exist, dbFile));
            }

            PrepareConnection(dbFile);
            Connect();
        }

        /// <summary>
        /// Creates instances of <see cref="PipingSoilProfile"/> based on the database file of the <see cref="PipingSoilProfileReader"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="PipingSoilProfileReadException">Thrown when reading soil profile entries from the database failed.</exception>
        /// <exception cref="XmlException">Thrown when parsing the geometry of a soil layer failed.</exception>
        public IEnumerable<PipingSoilProfile> Read()
        {
            var pipingSoilProfiles = new Dictionary<string, SoilProfileBuilder>();

            CreateDataReader();

            while (dataReader.Read())
            {
                var profileName = (string)dataReader[profileNameColumn];
                var intersectionX = dataReader[intersectionXColumn] as double?;
                if (intersectionX != null)
                {
                    if (!pipingSoilProfiles.ContainsKey(profileName))
                    {
                        pipingSoilProfiles.Add(profileName, new SoilProfileBuilder(profileName, intersectionX.Value));
                    }

                    pipingSoilProfiles[profileName].Add(ReadPiping2DSoilLayer());
                }
            }

            return pipingSoilProfiles.Select(keyValue => keyValue.Value.Build());
        }

        public void Dispose()
        {
            dataReader.Dispose();
            connection.Dispose();
        }

        private void PrepareConnection(string dbFile)
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                FailIfMissing = true,
                DataSource = dbFile,
                ReadOnly = true,
                ForeignKeys = true
            };

            connection = new SQLiteConnection(connectionStringBuilder.ConnectionString);
        }

        private void Connect()
        {
            try
            {
                connection.Open();
            }
            catch (SQLiteException)
            {
                connection.Dispose();
            }
        }

        private SoilLayer2D ReadPiping2DSoilLayer()
        {
            var columnValue = dataReader[layerGeometryColumn];
            var geometry = (byte[]) columnValue;

            return new PipingSoilLayer2DReader(geometry).Read();
        }

        /// <summary>
        /// Creates a new data reader to use in this class, based on a query which returns all the known soil layers for which its profile has a X coordinate defined for piping.
        /// </summary>
        private void CreateDataReader()
        {
            var mechanismParameterName = "mechanism";
            var query = new SQLiteCommand(connection)
            {
                CommandText = string.Format(string.Join(" ",
                    "SELECT",
                      "p.SP2D_Name as {0},",
                      "l.GeometrySurface as {1},",
                      "mpl.X as {2},",
                      "sum(case when mat.PN_Name = 'AbovePhreaticLevel' then mat.PV_Value end) {3},",
                      "sum(case when mat.PN_Name = 'BelowPhreaticLevel' then mat.PV_Value end) {4},",
                      "sum(case when mat.PN_Name = 'PermeabKx' then mat.PV_Value end) {5},",
                      "sum(case when mat.PN_Name = 'DiameterD70' then mat.PV_Value end) {6},",
                      "sum(case when mat.PN_Name = 'WhitesConstant' then mat.PV_Value end) {7},",
                      "sum(case when mat.PN_Name = 'BeddingAngle' then mat.PV_Value end) {8}",
                    "FROM MechanismPointLocation as m",
                    "JOIN MechanismPointLocation as mpl ON p.SP2D_ID = mpl.SP2D_ID",
                    "JOIN SoilProfile2D as p ON m.SP2D_ID = p.SP2D_ID",
                    "JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID",
                    "JOIN (",
	                    "SELECT m.MA_ID, pn.PN_Name, pv.PV_Value",
	                    "FROM ParameterNames as pn",
	                    "JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID",
	                    "JOIN Materials as m ON m.MA_ID = pv.MA_ID) as mat ON l.MA_ID = mat.MA_ID",
                    "WHERE m.ME_ID = @{9}",
                    "GROUP BY l.SL2D_ID",
                    "UNION",
                    "SELECT",
                      "p.SP1D_Name as {0},",
                      "null as {1},",
                      "null as {2},",
                      "sum(case when mat.PN_Name = 'AbovePhreaticLevel' then mat.PV_Value end) {3},",
                      "sum(case when mat.PN_Name = 'BelowPhreaticLevel' then mat.PV_Value end) {4},",
                      "sum(case when mat.PN_Name = 'PermeabKx' then mat.PV_Value end) {5},",
                      "sum(case when mat.PN_Name = 'DiameterD70' then mat.PV_Value end) {6},",
                      "sum(case when mat.PN_Name = 'WhitesConstant' then mat.PV_Value end) {7},",
                      "sum(case when mat.PN_Name = 'BeddingAngle' then mat.PV_Value end) {8}",
                    "FROM SoilProfile1D as p",
                      "JOIN SoilLayer1D as l ON l.SP1D_ID = p.SP1D_ID",
                      "JOIN (",
	                    "SELECT m.MA_ID, pn.PN_Name, pv.PV_Value",
	                    "FROM ParameterNames as pn",
	                    "JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID",
	                    "JOIN Materials as m ON m.MA_ID = pv.MA_ID) as mat ON l.MA_ID = mat.MA_ID",
                    "GROUP BY l.SL1D_ID",
                    "ORDER BY ProfileName"
                ),
                profileNameColumn,
                layerGeometryColumn,
                intersectionXColumn,
                abovePhreaticLevelColumn,
                belowPhreaticLevelColumn,
                permeabKxColumn,
                diameterD70Column,
                whitesConstantColumn,
                beddingAngleColumn,
                mechanismParameterName)
            };
            query.Parameters.Add(new SQLiteParameter
            {
                DbType = DbType.Int32,
                Value = pipingMechanismId,
                ParameterName = mechanismParameterName
            });

            try
            {
                dataReader = query.ExecuteReader();
            }
            catch (SQLiteException e)
            {
                throw new PipingSoilProfileReadException(string.Format(Resources.Error_SoilProfileReadFromDatabase, connection.DataSource), e);
            }
        }
    }
}