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
        private const int pipingMechanismId = 4;
        private const int profileNameIndex = 0;
        private const int profileLayerGeometryIndex = 1;
        private const int intersectionXIndex = 3;

        private SQLiteConnection connection;
        private SQLiteDataReader dataReader;

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
                var profileName = (string) dataReader[profileNameIndex];
                var intersectionX = (double) dataReader[intersectionXIndex];
                if (!pipingSoilProfiles.ContainsKey(profileName))
                {
                    pipingSoilProfiles.Add(profileName, new SoilProfileBuilder(profileName, intersectionX));
                }

                pipingSoilProfiles[profileName].Add(ReadPipingSoilLayer());
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

        private SoilLayer2D ReadPipingSoilLayer()
        {
            var columnValue = dataReader[profileLayerGeometryIndex];
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
                CommandText = string.Format(
                    "SELECT p.SP2D_Name, l.GeometrySurface, mat.MA_Name, mpl.X " +
                    "FROM MechanismPointLocation as m " +
                    "JOIN MechanismPointLocation as mpl ON p.SP2D_ID = mpl.SP2D_ID " +
                    "JOIN SoilProfile2D as p ON m.SP2D_ID = p.SP2D_ID " +
                    "JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID " +
                    "JOIN Materials as mat ON mat.MA_ID = l.MA_ID " +
                    "WHERE m.ME_ID = @{0} " +
                    "ORDER BY p.SP2D_ID, l.SP2D_ID",
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