using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
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

        private const string dimensionColumn = "Dimension";
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
        private const string layerCountColumn = "LayerCount";
        private const string mechanismParameterName = "mechanism";

        private SQLiteConnection connection;
        private SQLiteDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilProfileReader"/> which will use the <paramref name="dbFile"/>
        /// as its source. The reader will not point to any record at the start. Use <see cref="Next"/> to start reading
        /// profiles.
        /// </summary>
        /// <param name="dbFile">The path of the database file to open.</param>
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

            OpenConnection(dbFile);
        }

        /// <summary>
        /// Prepares the next layer from the database.
        /// </summary>
        /// <returns>False if there are no more rows to be read. True otherwise.</returns>
        /// <exception cref="PipingSoilProfileReadException">Thrown when reading the layer entry from the database failed.</exception>
        /// <exception cref="XmlException">Thrown when parsing the geometry of a 2d soil layer failed.</exception>
        public PipingSoilProfile ReadProfile()
        {
            if (dataReader == null)
            {
                throw new InvalidOperationException(GetType() + " was not initialized using Next()");
            }

            return TryRead<long>(dimensionColumn) == 1 ? ReadPipingProfile1D().Build() : ReadPipingProfile2D().Build();
        }

        /// <summary>
        /// Moves the reader to the next record in the database.
        /// </summary>
        /// <returns><c>true</c> if there was another record. Otherwise, <c>false</c>.</returns>
        /// <remarks>You should never make two calls to this method without calling <see cref="ReadProfile"/> in between.</remarks>
        public bool Next()
        {
            InitializeReader();
            if (!dataReader.Read())
            {
                if (dataReader.NextResult())
                {
                    return dataReader.Read();
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
            }
            connection.Close();
            connection.Dispose();
        }

        private ISoilProfileBuilder ReadPipingProfile1D()
        {
            var profileName = TryRead<string>(profileNameColumn);
            var bottom = TryRead<double>(bottomColumn);
            var layerCount = TryRead<long>(layerCountColumn);

            var soilProfileBuilder = new SoilProfileBuilder1D(profileName, bottom);

            for (int i = 1; i <= layerCount; i++)
            {
                soilProfileBuilder.Add(ReadPipingSoilLayer());
                if (i < layerCount)
                {
                    Next();
                }
            }
            return soilProfileBuilder;
        }

        private ISoilProfileBuilder ReadPipingProfile2D()
        {
            var profileName = TryRead<string>(profileNameColumn);
            var intersectionX = TryRead<double>(intersectionXColumn);
            var layerCount = TryRead<long>(layerCountColumn);

            var soilProfileBuilder = new SoilProfileBuilder2D(profileName, intersectionX);

            for (int i = 1; i <= layerCount; i++)
            {
                soilProfileBuilder.Add(ReadPiping2DSoilLayer());
                if (i < layerCount)
                {
                    Next();
                }
            }
            return soilProfileBuilder;
        }

        private void InitializeReader()
        {
            if (dataReader == null)
            {
                PrepareQueries();
            }
        }

        private T TryRead<T>(string columnName)
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

        private void OpenConnection(string dbFile)
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                FailIfMissing = true,
                DataSource = dbFile,
                ReadOnly = true,
                ForeignKeys = true
            };

            connection = new SQLiteConnection(connectionStringBuilder.ConnectionString);
            Connect();
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

        private PipingSoilLayer ReadPipingSoilLayer()
        {
            var columnValue = TryRead<double>(topColumn);
            var isAquiferValue = TryRead<double>(isAquiferColumn);
            var pipingSoilLayer = new PipingSoilLayer(columnValue)
            {
                IsAquifer = isAquiferValue.Equals(1.0)
            };
            return pipingSoilLayer;
        }

        private SoilLayer2D ReadPiping2DSoilLayer()
        {
            var geometry = TryRead<byte[]>(layerGeometryColumn);
            var isAquiferValue = TryRead<double>(isAquiferColumn);

            SoilLayer2D pipingSoilLayer = new PipingSoilLayer2DReader(geometry).Read();
            pipingSoilLayer.IsAquifer = isAquiferValue.Equals(1.0);

            return pipingSoilLayer;
        }

        private void PrepareQueries()
        {
            string materialPropertiesQuery = string.Format(
                string.Join(" ",
                            "(SELECT",
                            "m.MA_ID,",
                            "sum(case when pn.PN_Name = 'AbovePhreaticLevel' then pv.PV_Value end) {0},",
                            "sum(case when pn.PN_Name = 'BelowPhreaticLevel' then pv.PV_Value end) {1},",
                            "sum(case when pn.PN_Name = 'PermeabKx' then pv.PV_Value end) {2},",
                            "sum(case when pn.PN_Name = 'DiameterD70' then pv.PV_Value end) {3},",
                            "sum(case when pn.PN_Name = 'WhitesConstant' then pv.PV_Value end) {4},",
                            "sum(case when pn.PN_Name = 'BeddingAngle' then pv.PV_Value end) {5}",
                            "FROM ParameterNames as pn",
                            "JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID",
                            "JOIN Materials as m ON m.MA_ID = pv.MA_ID",
                            "GROUP BY m.MA_ID) as mat ON l.MA_ID = mat.MA_ID"),
                abovePhreaticLevelColumn,
                belowPhreaticLevelColumn,
                permeabKxColumn,
                diameterD70Column,
                whitesConstantColumn,
                beddingAngleColumn);

            string layer1DCountQuery = string.Format(
                string.Join(" ",
                            "(SELECT SP1D_ID, COUNT(*) as {0}",
                            "FROM SoilLayer1D",
                            "GROUP BY SP1D_ID) lc ON  lc.SP1D_ID = p.SP1D_ID"), layerCountColumn);

            string layer2DCountQuery = string.Format(
                string.Join(" ",
                            "(SELECT SP2D_ID, COUNT(*) as {0}",
                            "FROM SoilLayer2D",
                            "GROUP BY SP2D_ID) lc ON  lc.SP2D_ID = p.SP2D_ID"), layerCountColumn);

            string layer1DPropertiesQuery = string.Format(
                string.Join(" ",
                            "(SELECT",
                            "pv.SL1D_ID,",
                            "sum(case when pn.PN_Name = 'IsAquifer' then pv.PV_Value end) {0}",
                            "FROM ParameterNames as pn",
                            "JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID",
                            "GROUP BY pv.SL1D_ID) as lpv ON lpv.SL1D_ID = l.SL1D_ID"
                    ), isAquiferColumn);

            string layer2DPropertiesQuery = string.Format(
                string.Join(" ",
                            "(SELECT",
                            "pv.SL2D_ID,",
                            "sum(case when pn.PN_Name = 'IsAquifer' then pv.PV_Value end) {0}",
                            "FROM ParameterNames as pn",
                            "JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID",
                            "GROUP BY pv.SL2D_ID) as lpv ON lpv.SL2D_ID = l.SL2D_ID"
                    ), isAquiferColumn);

            var query1D = string.Format(
                string.Join(" ", "SELECT",
                            "1 as {0},",
                            "p.SP1D_Name as {1},",
                            "lc.{2},",
                            "p.BottomLevel as {3},",
                            "l.TopLevel as {4},",
                            "{5},",
                            "{6},",
                            "{7},",
                            "{8},",
                            "{9},",
                            "{10},",
                            "{11}",
                            "FROM SoilProfile1D as p",
                            "JOIN {12}",
                            "JOIN SoilLayer1D as l ON l.SP1D_ID = p.SP1D_ID",
                            "LEFT JOIN {13}",
                            "JOIN {14}",
                            "ORDER BY ProfileName;"),
                dimensionColumn,
                profileNameColumn,
                layerCountColumn,
                bottomColumn,
                topColumn,
                abovePhreaticLevelColumn,
                belowPhreaticLevelColumn,
                permeabKxColumn,
                diameterD70Column,
                whitesConstantColumn,
                beddingAngleColumn,
                isAquiferColumn,
                layer1DCountQuery,
                materialPropertiesQuery,
                layer1DPropertiesQuery);

            var query2D = string.Format(
                string.Join(" ",
                            "SELECT",
                            "2 as {0},",
                            "p.SP2D_Name as {1},",
                            "lc.{2},",
                            "l.GeometrySurface as {3}, ",
                            "mpl.X as {4},",
                            "{5},",
                            "{6},",
                            "{7},",
                            "{8},",
                            "{9},",
                            "{10},",
                            "{11}",
                            "FROM Mechanism as m",
                            "JOIN MechanismPointLocation as mpl ON mpl.ME_ID = m.ME_ID",
                            "JOIN SoilProfile2D as p ON p.SP2D_ID = mpl.SP2D_ID",
                            "JOIN {12}",
                            "JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID",
                            "LEFT JOIN {13}",
                            "JOIN {14}",
                            "WHERE m.ME_Name = @{15}",
                            "ORDER BY ProfileName;"),
                dimensionColumn,
                profileNameColumn,
                layerCountColumn,
                layerGeometryColumn,
                intersectionXColumn,
                abovePhreaticLevelColumn,
                belowPhreaticLevelColumn,
                permeabKxColumn,
                diameterD70Column,
                whitesConstantColumn,
                beddingAngleColumn,
                isAquiferColumn,
                layer2DCountQuery,
                materialPropertiesQuery,
                layer2DPropertiesQuery,
                mechanismParameterName);

            dataReader = CreateDataReader(query2D + query1D, new SQLiteParameter
            {
                DbType = DbType.String,
                Value = pipingMechanismName,
                ParameterName = mechanismParameterName
            });
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