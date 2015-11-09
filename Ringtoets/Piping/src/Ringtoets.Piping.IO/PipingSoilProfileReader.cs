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
    /// This class reads a SqLite database file and constructs <see cref="PipingSoilProfile"/> instances from this database.
    /// </summary>
    public class PipingSoilProfileReader : IDisposable
    {
        private const string databaseRequiredVersion = "15.0.5.0";
        private const string pipingMechanismName = "Piping";

        private const string profileCountColumn = "ProfileCount";
        private const string dimensionColumn = "Dimension";
        private const string isAquiferColumn = "IsAquifer";
        private const string profileNameColumn = "ProfileName";
        private const string intersectionXColumn = "IntersectionX";
        private const string bottomColumn = "Bottom";
        private const string topColumn = "Top";
        private const string layerGeometryColumn = "LayerGeometry";
        private const string abovePhreaticLevelColumn = "AbovePhreaticLevel";
        private const string belowPhreaticLevelColumn = "BelowPhreaticLevel";
        private const string dryUnitWeightColumn = "DryUnitWeight";
        private const string layerCountColumn = "LayerCount";
        private const string mechanismParameterName = "mechanism";

        private readonly string databaseFileName;

        private SQLiteConnection connection;
        private SQLiteDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilProfileReader"/> which will use the <paramref name="databaseFilePath"/>
        /// as its source. The reader will not point to any record at the start. Use <see cref="MoveNext"/> to start reading
        /// profiles.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="ArgumentException">Thrown when the given <paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the given <paramref name="databaseFilePath"/> didn't point to an existing file.</exception>
        public PipingSoilProfileReader(string databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);
            if (!File.Exists(databaseFilePath))
            {
                throw new FileNotFoundException(String.Format(Resources.Error_File_0_does_not_exist, databaseFilePath));
            }

            databaseFileName = Path.GetFileName(databaseFilePath);
            OpenConnection(databaseFilePath);
            SetReaderToFirstRecord();
        }

        /// <summary>
        /// Gets the total number of profiles that can be read from the database.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the value <c>true</c> if profiles can be read using the <see cref="PipingSoilProfileReader"/>.
        /// <c>false</c> otherwise.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Reads the information for the next profile from the database and creates a <see cref="PipingSoilProfile"/> instance
        /// of the information.
        /// </summary>
        /// <returns>The next <see cref="PipingSoilProfile"/> from the database, or <c>null</c> if no more profiles can be read.</returns>
        /// <exception cref="PipingSoilProfileReadException">Thrown when reading the profile in the database contained a non-parsable geometry.</exception>
        public PipingSoilProfile ReadProfile()
        {
            if (!HasNext)
            {
                return null;
            }

            var dimensionValue = Read<long>(dimensionColumn);
            return dimensionValue == 1 ? ReadPipingProfile1D() : ReadPipingProfile2D();
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

        /// <summary>
        /// Moves the reader to the next record in the database.
        /// </summary>
        private void MoveNext()
        {
            HasNext = dataReader.Read() || (dataReader.NextResult() && dataReader.Read());
        }

        private PipingSoilProfile ReadPipingProfile1D()
        {
            var profileName = Read<string>(profileNameColumn);
            var layerCount = Read<long>(layerCountColumn);

            double bottom;
            try
            {
                bottom = Read<double>(bottomColumn);
            } catch (InvalidCastException e) {
                SkipRecords((int)layerCount);
                var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_column_1_, profileName, intersectionXColumn);
                throw new PipingSoilProfileReadException(message, e);
            }

            var soilProfileBuilder = new SoilProfileBuilder1D(profileName, bottom);

            for (var i = 1; i <= layerCount; i++)
            {
                soilProfileBuilder.Add(ReadPipingSoilLayer());
                MoveNext();
            }

            return soilProfileBuilder.Build();
        }

        /// <summary>
        /// Reads information for a profile from the database and creates a <see cref="PipingSoilProfile"/> based on the information.
        /// </summary>
        /// <returns>A new <see cref="PipingSoilProfile"/> with information from the database.</returns>
        /// <exception cref="PipingSoilProfileReadException">Thrown when a layer's geometry could not be parsed as XML.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when no valid point to obtain a one-dimensional
        /// intersection from was read from the database, or when after reading layers, no layers were added
        /// to be build.</exception>
        private PipingSoilProfile ReadPipingProfile2D()
        {
            var profileName = Read<string>(profileNameColumn);
            var layerCount = Read<long>(layerCountColumn);

            try
            {
                double intersectionX;
                try
                {
                    intersectionX = TryRead<double>(intersectionXColumn);
                }
                catch (InvalidCastException e)
                {
                    SkipRecords((int)layerCount);
                    var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_column_1_, profileName, intersectionXColumn);
                    throw new PipingSoilProfileReadException(message, e);
                }
                var soilProfileBuilder = new SoilProfileBuilder2D(profileName, intersectionX);

                for (int i = 1; i <= layerCount; i++)
                {
                    try
                    {
                        soilProfileBuilder.Add(ReadPiping2DSoilLayer());
                    }
                    catch (XmlException e)
                    {
                        SkipRecords((int)layerCount + 1 - i);

                        var format = string.Format(Resources.PipingSoilProfileReader_CouldNotParseGeometryOfLayer_0_InProfile_1_, i, profileName);
                        throw new PipingSoilProfileReadException(
                            format, e);
                    }
                    catch (SoilProfileBuilderException e)
                    {
                        SkipRecords((int) layerCount + 1 - i);

                        var format = string.Format(Resources.PipingSoilProfileReader_CouldNotParseGeometryOfLayer_0_InProfile_1_, i, profileName);
                        throw new PipingSoilProfileReadException(
                            format, e);
                    }
                    catch (InvalidCastException e)
                    {
                        SkipRecords((int)layerCount + 1 - i);

                        var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_column_1_, profileName, intersectionXColumn);
                        throw new PipingSoilProfileReadException(message, e);
                    }
                    MoveNext();
                }

                try
                {
                    return soilProfileBuilder.Build();
                }
                catch (SoilProfileBuilderException e)
                {
                    SkipRecords((int) 0 + 1 - 1);
                    var exception = new PipingSoilProfileReadException(
                        string.Format(Resources.PipingSoilProfileReader_CouldNotParseGeometryOfLayer_0_InProfile_1_, 1, profileName), e);

                    throw exception;
                }
            }
            catch (ArgumentException e)
            {
                HandleCriticalBuildException(profileName, e);
            }
            return null;
        }

        private static void HandleCriticalBuildException(string profileName, Exception e)
        {
            var message = string.Format(Resources.PipingSoilProfileReader_Could_not_build_profile_0_from_layer_definitions, profileName);
            throw new CriticalFileReadException(message, e);
        }

        private void SkipRecords(int count)
        {
            for(int i = 0; i < count; i++)
            {
                MoveNext();
            }
        }

        /// <summary>
        /// Reads a value at column <paramref name="columnName"/> from the database.
        /// </summary>
        /// <typeparam name="T">The expected type of value in the column with name <paramref name="columnName"/>.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The read value from the column with name <paramref name="columnName"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the value in the column was not of type <typeparamref name="T"/>.</exception>
        private T TryRead<T>(string columnName)
        {
            var dbValue = dataReader[columnName];
            var isNullable = typeof(T).IsGenericType && typeof(Nullable<>).IsAssignableFrom(typeof(T).GetGenericTypeDefinition());

            if (isNullable && dbValue == DBNull.Value)
            {
                return default(T);
            }

            return (T)dbValue;
        }

        /// <summary>
        /// Reads a value at column <paramref name="columnName"/> from the database.
        /// </summary>
        /// <typeparam name="T">The expected type of value in the column with name <paramref name="columnName"/>.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The read value from the column with name <paramref name="columnName"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the value in the column was not of type <typeparamref name="T"/>.</exception>
        private T Read<T>(string columnName)
        {
            try
            {
                return (T)dataReader[columnName];
            }
            catch (InvalidCastException)
            {
                throw new CriticalFileReadException(string.Format(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column_0_, columnName));
            }
        }

        private PipingSoilLayer ReadPipingSoilLayer()
        {
            var topValue = TryRead<double>(topColumn);
            var isAquiferValue = TryRead<double?>(isAquiferColumn);
            var belowPhreaticLevelValue = TryRead<double?>(belowPhreaticLevelColumn);
            var abovePhreaticLevelValue = TryRead<double?>(abovePhreaticLevelColumn);
            var dryUnitWeightValue = TryRead<double?>(dryUnitWeightColumn);

            var pipingSoilLayer = new PipingSoilLayer(topValue)
            {
                IsAquifer = isAquiferValue,
                BelowPhreaticLevel = belowPhreaticLevelValue,
                AbovePhreaticLevel = abovePhreaticLevelValue,
                DryUnitWeight = dryUnitWeightValue
            };
            return pipingSoilLayer;
        }

        /// <summary>
        /// Reads a soil layer from a 2d profile
        /// </summary>
        /// <returns>A new <see cref="SoilLayer2D"/> instance, based on the information read from the database.</returns>
        /// <exception cref="InvalidCastException">Thrown when a column did not contain a value of the expected type.</exception>
        private SoilLayer2D ReadPiping2DSoilLayer()
        {
            var geometryValue = TryRead<byte[]>(layerGeometryColumn);

            var isAquiferValue = TryRead<double?>(isAquiferColumn);
            var belowPhreaticLevelValue = TryRead<double?>(belowPhreaticLevelColumn);
            var abovePhreaticLevelValue = TryRead<double?>(abovePhreaticLevelColumn);
            var dryUnitWeightValue = TryRead<double?>(dryUnitWeightColumn);

            SoilLayer2D pipingSoilLayer = new PipingSoilLayer2DReader(geometryValue).Read();
            pipingSoilLayer.IsAquifer = isAquiferValue;
            pipingSoilLayer.BelowPhreaticLevel = belowPhreaticLevelValue;
            pipingSoilLayer.AbovePhreaticLevel = abovePhreaticLevelValue;
            pipingSoilLayer.DryUnitWeight = dryUnitWeightValue;

            return pipingSoilLayer;
        }

        private void SetReaderToFirstRecord()
        {
            InitializeDataReader();
            MoveNext();
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

        /// <summary>
        /// Prepares the two queries required for obtaining all the SoilProfile1D and SoilProfile2D with an x defined
        /// to take an intersection from. Since two separate queries are used, the <see cref="dataReader"/> will
        /// have two result sets which the <see cref="MoveNext()"/> method takes into account.
        /// </summary>
        private void InitializeDataReader()
        {

            string versionQuery = string.Format(
                "SELECT Value FROM _Metadata WHERE Key = 'VERSION' AND Value = '{0}';",
                databaseRequiredVersion
                );

            string countQuery = string.Format(string.Join(
                " ",
                "SELECT",
                "(SELECT COUNT(*)",
                "FROM Mechanism as m",
                "JOIN MechanismPointLocation as mpl ON mpl.ME_ID = m.ME_ID",
                "JOIN SoilProfile2D as p2 ON p2.SP2D_ID = mpl.SP2D_ID",
                "WHERE m.ME_Name = @{0})",
                " + ",
                "(SELECT COUNT(*)",
                "FROM SoilProfile1D) as {1};"), mechanismParameterName, profileCountColumn);

            string materialPropertiesQuery = string.Format(
                string.Join(" ",
                            "(SELECT",
                            "m.MA_ID,",
                            "sum(case when pn.PN_Name = 'AbovePhreaticLevel' then pv.PV_Value end) {0},",
                            "sum(case when pn.PN_Name = 'BelowPhreaticLevel' then pv.PV_Value end) {1},",
                            "sum(case when pn.PN_Name = 'DryUnitWeight' then pv.PV_Value end) {2}",
                            "FROM ParameterNames as pn",
                            "JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID",
                            "JOIN Materials as m ON m.MA_ID = pv.MA_ID",
                            "GROUP BY m.MA_ID) as mat ON l.MA_ID = mat.MA_ID"),
                abovePhreaticLevelColumn,
                belowPhreaticLevelColumn,
                dryUnitWeightColumn);

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
                            "{8}",
                            "FROM SoilProfile1D as p",
                            "JOIN {9}",
                            "JOIN SoilLayer1D as l ON l.SP1D_ID = p.SP1D_ID",
                            "LEFT JOIN {10}",
                            "LEFT JOIN {11}",
                            "ORDER BY ProfileName;"),
                dimensionColumn,
                profileNameColumn,
                layerCountColumn,
                bottomColumn,
                topColumn,
                abovePhreaticLevelColumn,
                belowPhreaticLevelColumn,
                dryUnitWeightColumn,
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
                            "{8}",
                            "FROM Mechanism as m",
                            "JOIN MechanismPointLocation as mpl ON mpl.ME_ID = m.ME_ID",
                            "JOIN SoilProfile2D as p ON p.SP2D_ID = mpl.SP2D_ID",
                            "JOIN {9}",
                            "JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID",
                            "LEFT JOIN {10}",
                            "LEFT JOIN {11}",
                            "WHERE m.ME_Name = @{12}",
                            "ORDER BY ProfileName;"),
                dimensionColumn,
                profileNameColumn,
                layerCountColumn,
                layerGeometryColumn,
                intersectionXColumn,
                abovePhreaticLevelColumn,
                belowPhreaticLevelColumn,
                dryUnitWeightColumn,
                isAquiferColumn,
                layer2DCountQuery,
                materialPropertiesQuery,
                layer2DPropertiesQuery,
                mechanismParameterName);

            CreateDataReader(versionQuery + countQuery + query2D + query1D, new SQLiteParameter
            {
                DbType = DbType.String,
                Value = pipingMechanismName,
                ParameterName = mechanismParameterName
            });
        }

        /// <summary>
        /// Creates a new data reader to use in this class.
        /// </summary>
        /// <exception cref="PipingSoilProfileReadException">Thrown when
        /// <list type="bullet">
        ///     <item>Version of the database doesn't match the required version.</item>
        ///     <item>Version of the database could not be read.</item>
        ///     <item>Amount of profiles in database could not be read.</item>
        ///     <item>A query could not be executed on the database schema.</item>
        /// </list>
        /// </exception>
        private void CreateDataReader(string queryString, params SQLiteParameter[] parameters)
        {
            using (var query = new SQLiteCommand(connection)
            {
                CommandText = queryString
            })
            {
                query.Parameters.AddRange(parameters);

                try
                {
                    dataReader = query.ExecuteReader();
                    CheckVersion();
                    GetCount();
                }
                catch (SQLiteException e)
                {
                    Dispose();
                    var exception = new PipingSoilProfileReadException(string.Format(Resources.Error_SoilProfile_read_from_database, databaseFileName), e);
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Checks the version read from the metadata table against the <see cref="databaseRequiredVersion"/>.
        /// </summary>
        /// <exception cref="PipingSoilProfileReadException">Thrown when versions don't match.</exception>
        private void CheckVersion()
        {
            if (!dataReader.HasRows)
            {
                Dispose();
                throw new PipingSoilProfileReadException(string.Format(
                    Resources.PipingSoilProfileReader_Database_file_0_incorrect_version_requires_1,
                    databaseFileName,
                    databaseRequiredVersion));
            }
            dataReader.NextResult();
        }

        private void GetCount()
        {
            dataReader.Read();
            Count = (int)Read<long>(profileCountColumn);
            dataReader.NextResult();
        }
    }
}