using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using Wti.Data;

namespace Wti.IO
{
    /// <summary>
    /// This class reads a SqLite database file and constructs <see cref="PipingSoilProfile"/> from this database.
    /// The database is created with the DSoilModel application.
    /// </summary>
    public class PipingSoilProfilesReader : IDisposable
    {
        private readonly SQLiteConnection connection;
        private readonly int PIPING_MECHANISM_ID = 4;
        private int profileIdIndex;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilProfilesReader"/> which will use the <paramref name="dbFile"/>
        /// as its source.
        /// </summary>
        /// <param name="dbFile"></param>
        public PipingSoilProfilesReader(string dbFile)
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

        /// <summary>
        /// Attempts to initialize a connection with the data source.
        /// </summary>
        /// <returns>True if the resulting connection can be opened. False otherwise.</returns>
        public bool Connect()
        {
            try
            {
                connection.Open();
                return connection.State == ConnectionState.Open;
            }
            catch (SQLiteException)
            {
                connection.Dispose();
                return false;
            }
        }

        public ICollection<PipingSoilProfile> ReadSoilProfiles()
        {
            var pipingSoilProfiles = new Dictionary<long, PipingSoilProfile>();
            var soilProfile2DReader = GetSoilProfile2DReader();

            while (soilProfile2DReader.Read())
            {
                PipingSoilProfile soilProfile = ReadSoilProfile(soilProfile2DReader);
                if (!pipingSoilProfiles.ContainsKey(soilProfile.Id))
                {
                    pipingSoilProfiles.Add(soilProfile.Id, soilProfile);
                }
                else
                {
                    soilProfile = pipingSoilProfiles[soilProfile.Id];
                }
                soilProfile.Add(ReadSoilLayer(soilProfile2DReader));
            }

            return pipingSoilProfiles.Values;
        }

        private PipingSoilProfile ReadSoilProfile(SQLiteDataReader reader)
        {
            long id = (long) reader[0];
            string name = (string) reader[1];
            return new PipingSoilProfile(id, name);
        }

        private PipingSoilLayer ReadSoilLayer(SQLiteDataReader reader)
        {
            long id = (long)reader[2];
            var pipingSoilLayer = new PipingSoilLayer(id);

            byte[] geometryStream = (byte[])reader[3];
            XmlTextReader xmlTextReader = new XmlTextReader(new MemoryStream(geometryStream));

            while (xmlTextReader.Read())
            {
                switch (xmlTextReader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an Element.
                        Console.Write("<" + xmlTextReader.Name);
                        Console.WriteLine(">");
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        Console.WriteLine(xmlTextReader.Value);
                        break;
                    case XmlNodeType.EndElement: //Display end of element.
                        Console.Write("</" + xmlTextReader.Name);
                        Console.WriteLine(">");
                        break;
                }
            }

            return pipingSoilLayer;
        }

        private SQLiteDataReader GetSoilProfile2DReader()
        {
            var query = new SQLiteCommand(connection);
            var parameter = new SQLiteParameter
            {
                DbType = DbType.Int32,
                Value = PIPING_MECHANISM_ID,
                ParameterName = "mechanism"
            };

            query.CommandText = "SELECT p.SP2D_ID, p.SP2D_Name, l.SL2D_ID, l.GeometrySurface FROM MechanismPointLocation as m JOIN SoilProfile2D as p ON m.SP2D_ID = p.SP2D_ID JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID WHERE m.ME_ID = @mechanism";
            query.Parameters.Add(parameter);
            return query.ExecuteReader();
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}