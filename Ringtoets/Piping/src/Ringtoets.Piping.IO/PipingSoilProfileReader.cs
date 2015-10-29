using System;
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
        private const string pipingMechanismName = "Piping";

        private const string isAquiferColumn = "IsAquifer";
        private const string profileNameColumn = "ProfileName";
        private const string intersectionXColumn = "IntersectionX";
        private const string bottomColumn = "Bottom";
        private const string topColumn = "Top";
        private const string layerGeometryColumn = "LayerGeometry";
        private const string abovePhreaticLevelColumn = "AbovePhreaticLevel";
        private const string belowPhreaticLevelColumn = "BelowPhreaticLevel";
        private const string permeabKxColumn = "PermeabKx";
        private const string diameterD70Column = "DiameterD70";
        private const string whitesConstantColumn = "WhitesConstant";
        private const string beddingAngleColumn = "BeddingAngle";
        private const string mechanismParameterName = "mechanism";

        private SQLiteConnection connection;

        private string query1D;
        private string query2D;

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
            PrepareQueries();
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
            var pipingSoilProfileBuilders = new Dictionary<string, ISoilProfileBuilder>();

            using (var dataReader = CreateDataReader(query2D, new SQLiteParameter
            {
                DbType = DbType.String,
                Value = pipingMechanismName,
                ParameterName = mechanismParameterName
            }))
            {
                ReadPipingSoilLayers2D(dataReader, pipingSoilProfileBuilders);
            }
            using (var dataReader = CreateDataReader(query1D))
            {
                ReadPipingSoilLayers(dataReader, pipingSoilProfileBuilders);
            }

            return pipingSoilProfileBuilders.Select(keyValue => keyValue.Value.Build());
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }

        private void ReadPipingSoilLayers2D(SQLiteDataReader dataReader, IDictionary<string, ISoilProfileBuilder> pipingSoilProfileBuilders)
        {
            while (dataReader.Read())
            {
                var profileName = TryRead<string>(dataReader, profileNameColumn);
                var intersectionX = TryRead<double>(dataReader, intersectionXColumn);

                Func<SoilProfileBuilder2D> create = () => new SoilProfileBuilder2D(profileName, intersectionX);

                var soilProfileBuilder = GetSoilProfileBuilder(profileName, pipingSoilProfileBuilders, create);

                soilProfileBuilder.Add(ReadPiping2DSoilLayer(dataReader));
            }
        }

        private void ReadPipingSoilLayers(SQLiteDataReader dataReader, IDictionary<string, ISoilProfileBuilder> pipingSoilProfileBuilders)
        {
            while (dataReader.Read())
            {
                var profileName = TryRead<string>(dataReader, profileNameColumn);
                var bottom = TryRead<double>(dataReader, bottomColumn);

                Func<SoilProfileBuilder1D> create = () => new SoilProfileBuilder1D(profileName, bottom);

                var soilProfileBuilder = GetSoilProfileBuilder(profileName, pipingSoilProfileBuilders, create);

                soilProfileBuilder.Add(ReadPipingSoilLayer(dataReader));
            }
        }

        private static T GetSoilProfileBuilder<T>(string profileName, IDictionary<string, ISoilProfileBuilder> pipingSoilProfileBuilders, Func<T> createInstance) where T : ISoilProfileBuilder
        {
            try
            {
                if (!pipingSoilProfileBuilders.ContainsKey(profileName))
                {
                    pipingSoilProfileBuilders.Add(profileName, createInstance());
                }
                return (T) pipingSoilProfileBuilders[profileName];
            }
            catch (InvalidCastException e)
            {
                throw new PipingSoilProfileReadException(Resources.Error_CannotCombine2DAnd1DLayersInProfile, e);
            }
        }

        private T TryRead<T>(SQLiteDataReader dataReader, string columnName)
        {
            try
            {
                return (T) dataReader[columnName];
            }
            catch (InvalidCastException e)
            {
                throw new PipingSoilProfileReadException(String.Format(Resources.PipingSoilProfileReader_InvalidValueOnColumn, columnName), e);
            }
        }

        private PipingSoilLayer ReadPipingSoilLayer(SQLiteDataReader dataReader)
        {
            var columnValue = TryRead<double>(dataReader, topColumn);
            var pipingSoilLayer = new PipingSoilLayer(columnValue);
            return pipingSoilLayer;
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

        private SoilLayer2D ReadPiping2DSoilLayer(SQLiteDataReader dataReader)
        {
            var geometry = TryRead<byte[]>(dataReader, layerGeometryColumn);

            return new PipingSoilLayer2DReader(geometry).Read();
        }

        private void PrepareQueries()
        {
            string sharedSelectColumns = string.Format(string.Join(" ",
                                                                   "sum(case when lpv.PN_Name = 'IsAquifer' then lpv.PV_Value end) {0},",
                                                                   "sum(case when mat.PN_Name = 'AbovePhreaticLevel' then mat.PV_Value end) {1},",
                                                                   "sum(case when mat.PN_Name = 'BelowPhreaticLevel' then mat.PV_Value end) {2},",
                                                                   "sum(case when mat.PN_Name = 'PermeabKx' then mat.PV_Value end) {3},",
                                                                   "sum(case when mat.PN_Name = 'DiameterD70' then mat.PV_Value end) {4},",
                                                                   "sum(case when mat.PN_Name = 'WhitesConstant' then mat.PV_Value end) {5},",
                                                                   "sum(case when mat.PN_Name = 'BeddingAngle' then mat.PV_Value end) {6}"),
                                                       isAquiferColumn,
                                                       abovePhreaticLevelColumn,
                                                       belowPhreaticLevelColumn,
                                                       permeabKxColumn,
                                                       diameterD70Column,
                                                       whitesConstantColumn,
                                                       beddingAngleColumn
                );

            query1D = string.Format(string.Join(" ", "SELECT",
                                                "p.SP1D_Name as {0},",
                                                "p.BottomLevel as {1},",
                                                "l.TopLevel as {2},",
                                                sharedSelectColumns,
                                                "FROM SoilProfile1D as p",
                                                "JOIN SoilLayer1D as l ON l.SP1D_ID = p.SP1D_ID",
                                                "JOIN (",
                                                "SELECT m.MA_ID, pn.PN_Name, pv.PV_Value",
                                                "FROM ParameterNames as pn",
                                                "JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID",
                                                "JOIN Materials as m ON m.MA_ID = pv.MA_ID) as mat ON l.MA_ID = mat.MA_ID",
                                                "JOIN (",
                                                "SELECT pv.SL1D_ID, pn.PN_Name, pv.PV_Value",
                                                "FROM ParameterNames as pn",
                                                "JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID) as lpv ON lpv.SL1D_ID = l.SL1D_ID",
                                                "GROUP BY l.SL1D_ID",
                                                "ORDER BY ProfileName"),
                                    profileNameColumn,
                                    bottomColumn,
                                    topColumn);

            query2D = string.Format(string.Join(" ", "SELECT",
                                                "p.SP2D_Name as {0},",
                                                "l.GeometrySurface as {1}, ",
                                                "mpl.X as {2},",
                                                sharedSelectColumns,
                                                "FROM Mechanism as m",
                                                "JOIN MechanismPointLocation as mpl ON mpl.ME_ID = m.ME_ID",
                                                "JOIN SoilProfile2D as p ON p.SP2D_ID = mpl.SP2D_ID ",
                                                "JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID",
                                                "JOIN (",
                                                "SELECT m.MA_ID, pn.PN_Name, pv.PV_Value",
                                                "FROM ParameterNames as pn",
                                                "JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID",
                                                "JOIN Materials as m ON m.MA_ID = pv.MA_ID) as mat ON l.MA_ID = mat.MA_ID",
                                                "JOIN (",
                                                "SELECT pv.SL2D_ID, pn.PN_Name, pv.PV_Value",
                                                "FROM ParameterNames as pn",
                                                "JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID) as lpv ON lpv.SL2D_ID = l.SL2D_ID",
                                                "WHERE m.ME_Name = @{3}",
                                                "GROUP BY l.SL2D_ID"),
                                    profileNameColumn,
                                    layerGeometryColumn,
                                    intersectionXColumn,
                                    mechanismParameterName);
        }

        /// <summary>
        /// Creates a new data reader to use in this class, based on a query which returns all the known soil layers for which its profile has a X coordinate defined for piping.
        /// </summary>
        private SQLiteDataReader CreateDataReader(string queryString, params SQLiteParameter[] parameters)
        {
            using (var query = new SQLiteCommand(connection)
            {
                CommandText = queryString
            })
            {
                query.Parameters.AddRange(parameters);

                try
                {
                    return query.ExecuteReader();
                }
                catch (SQLiteException e)
                {
                    throw new PipingSoilProfileReadException(string.Format(Resources.Error_SoilProfileReadFromDatabase, connection.DataSource), e);
                }
            }
        }
    }
}