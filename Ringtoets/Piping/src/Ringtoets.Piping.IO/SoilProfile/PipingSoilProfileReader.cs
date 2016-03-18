// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Data;
using System.Data.SQLite;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class reads a SqLite database file and constructs <see cref="PipingSoilProfile"/> instances from this database.
    /// </summary>
    public class PipingSoilProfileReader : SqLiteDatabaseReaderBase, IRowBasedDatabaseReader
    {
        private const string pipingMechanismName = "Piping";
        private const string mechanismParameterName = "mechanism";

        private SQLiteDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilProfileReader"/> which will use the <paramref name="databaseFilePath"/>
        /// as its source. The reader will not point to any record at the start. Use <see cref="MoveNext"/> to start reading
        /// profiles.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Preparing the queries to read from the database failed.</item>
        /// </list>
        /// </exception>
        public PipingSoilProfileReader(string databaseFilePath) : base(databaseFilePath)
        {
            VerifyVersion(databaseFilePath);
            InitializeReader();
        }

        /// <summary>
        /// Gets the total number of profiles that can be read from the database.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not more soil profiles can be read using 
        /// the <see cref="PipingSoilProfileReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Reads the information for the next profile from the database and creates a 
        /// <see cref="PipingSoilProfile"/> instance of the information.
        /// </summary>
        /// <returns>The next <see cref="PipingSoilProfile"/> from the database, or <c>null</c> if no more profiles can be read.</returns>
        /// <exception cref="PipingSoilProfileReadException">Thrown when reading the profile in the database contained a non-parsable geometry.</exception>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">Thrown when the database returned incorrect values for required properties.</exception>
        public PipingSoilProfile ReadProfile()
        {
            if (!HasNext)
            {
                return null;
            }

            try
            {
                return ReadPipingSoilProfile();
            }
            catch (InvalidCastException e)
            {
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column);
                throw new CriticalFileReadException(message, e);
            }
        }

        public override void Dispose()
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
            }
            base.Dispose();
        }

        /// <summary>
        /// Moves the reader to the next record in the database.
        /// </summary>
        public void MoveNext()
        {
            HasNext = MoveNext(dataReader);
        }

        /// <summary>
        /// Reads the value in the column with name <paramref name="columnName"/> from the 
        /// current row that's being pointed at.
        /// </summary>
        /// <typeparam name="T">The type of object to read.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The value in the column, or <c>null</c> if the value was <see cref="DBNull.Value"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the value in the column could not be casted to type <typeparamref name="T"/>.</exception>
        public T? ReadOrNull<T>(string columnName) where T : struct
        {
            var valueObject = dataReader[columnName];
            if (valueObject.Equals(DBNull.Value))
            {
                return null;
            }
            return (T) valueObject;
        }

        /// <summary>
        /// Reads a value at column <paramref name="columnName"/> from the database.
        /// </summary>
        /// <typeparam name="T">The expected type of value in the column with name <paramref name="columnName"/>.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The read value from the column with name <paramref name="columnName"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the value in the column was not of type <typeparamref name="T"/>.</exception>
        public T Read<T>(string columnName)
        {
            return (T) dataReader[columnName];
        }

        private void VerifyVersion(string databaseFilePath)
        {
            using (var versionReader = new SoilDatabaseVersionReader(databaseFilePath))
            {
                try
                {
                    versionReader.VerifyVersion();
                }
                catch (CriticalFileReadException)
                {
                    CloseConnection();
                    throw;
                }
            }
        }

        /// <summary>
        /// Reads a <see cref="PipingSoilProfile"/> from the database.
        /// </summary>
        /// <returns>A new <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="PipingSoilProfileReadException">Thrown when a recoverable error occurred while reading from the database.</exception>
        /// <exception cref="InvalidCastException">Thrown when recovering from the <see cref="PipingSoilProfileReadException"/> failed.</exception>
        private PipingSoilProfile ReadPipingSoilProfile()
        {
            try
            {
                var dimensionValue = Read<long>(SoilProfileDatabaseColumns.Dimension);
                return dimensionValue == 1 ? SoilProfile1DReader.ReadFrom(this) : SoilProfile2DReader.ReadFrom(this);
            }
            catch (PipingSoilProfileReadException e)
            {
                MoveToNextProfile(e.ProfileName);
                throw;
            }
        }

        /// <summary>
        /// Steps through the result rows until a row is read which' profile name differs from <paramref name="profileName"/>.
        /// </summary>
        /// <param name="profileName">The name of the profile to skip.</param>
        private void MoveToNextProfile(string profileName)
        {
            while (HasNext && Read<string>(SoilProfileDatabaseColumns.ProfileName).Equals(profileName))
            {
                MoveNext();
            }
        }

        /// <summary>
        /// Prepares a new data reader with queries for obtaining the profiles and updates the reader
        /// so that it points to the first row of the result set.
        /// </summary>
        /// <exception cref="PipingSoilProfileReadException">Thrown when
        /// <list type="bullet">
        ///     <item>Version of the database does not match the required version.</item>
        ///     <item>Version of the database could not be read.</item>
        ///     <item>Amount of profiles in database could not be read.</item>
        /// </list>
        /// </exception>
        /// <exception cref="CriticalFileReadException">A query could not be executed on the database schema.</exception>
        private void InitializeReader()
        {
            try
            {
                PrepareReader();
                GetCount();
            }
            catch (SQLiteException e)
            {
                Dispose();
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.Error_SoilProfile_read_from_database);
                throw new CriticalFileReadException(message, e);
            }
            MoveNext();
        }

        /// <summary>
        /// Prepares the two queries required for obtaining all the SoilProfile1D and SoilProfile2D with an x defined
        /// to take an intersection from. Since two separate queries are used, the <see cref="dataReader"/> will
        /// have two result sets which the <see cref="MoveNext()"/> method takes into account.
        /// </summary>
        /// <exception cref="SQLiteException">A query could not be executed on the database schema.</exception>
        private void PrepareReader()
        {
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
                "FROM SoilProfile1D) as {1};"), mechanismParameterName, SoilProfileDatabaseColumns.ProfileCount);

            string materialPropertiesQuery = string.Format(
                string.Join(" ",
                            "(SELECT",
                            "m.MA_ID,",
                            "max(case when pn.PN_Name = 'AbovePhreaticLevel' then pv.PV_Value end) {0},",
                            "max(case when pn.PN_Name = 'BelowPhreaticLevel' then pv.PV_Value end) {1},",
                            "max(case when pn.PN_Name = 'DryUnitWeight' then pv.PV_Value end) {2}",
                            "FROM ParameterNames as pn",
                            "JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID",
                            "JOIN Materials as m ON m.MA_ID = pv.MA_ID",
                            "GROUP BY m.MA_ID) as mat ON l.MA_ID = mat.MA_ID"),
                SoilProfileDatabaseColumns.AbovePhreaticLevel,
                SoilProfileDatabaseColumns.BelowPhreaticLevel,
                SoilProfileDatabaseColumns.DryUnitWeight);

            string layer1DCountQuery = string.Format(
                string.Join(" ",
                            "(SELECT SP1D_ID, COUNT(*) as {0}",
                            "FROM SoilLayer1D",
                            "GROUP BY SP1D_ID) lc ON  lc.SP1D_ID = p.SP1D_ID"), SoilProfileDatabaseColumns.LayerCount);

            string layer2DCountQuery = string.Format(
                string.Join(" ",
                            "(SELECT SP2D_ID, COUNT(*) as {0}",
                            "FROM SoilLayer2D",
                            "GROUP BY SP2D_ID) lc ON  lc.SP2D_ID = p.SP2D_ID"), SoilProfileDatabaseColumns.LayerCount);

            string layer1DPropertiesQuery = string.Format(
                string.Join(" ",
                            "(SELECT",
                            "pv.SL1D_ID,",
                            "max(case when pn.PN_Name = 'IsAquifer' then pv.PV_Value end) {0}",
                            "FROM ParameterNames as pn",
                            "JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID",
                            "GROUP BY pv.SL1D_ID) as lpv ON lpv.SL1D_ID = l.SL1D_ID"
                    ), SoilProfileDatabaseColumns.IsAquifer);

            string layer2DPropertiesQuery = string.Format(
                string.Join(" ",
                            "(SELECT",
                            "pv.SL2D_ID,",
                            "max(case when pn.PN_Name = 'IsAquifer' then pv.PV_Value end) {0}",
                            "FROM ParameterNames as pn",
                            "JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID",
                            "GROUP BY pv.SL2D_ID) as lpv ON lpv.SL2D_ID = l.SL2D_ID"
                    ), SoilProfileDatabaseColumns.IsAquifer);

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
                            "p.SP1D_ID as {9}",
                            "FROM SoilProfile1D as p",
                            "JOIN {10}",
                            "JOIN SoilLayer1D as l ON l.SP1D_ID = p.SP1D_ID",
                            "LEFT JOIN {11}",
                            "LEFT JOIN {12}",
                            "ORDER BY ProfileName;"),
                SoilProfileDatabaseColumns.Dimension,
                SoilProfileDatabaseColumns.ProfileName,
                SoilProfileDatabaseColumns.LayerCount,
                SoilProfileDatabaseColumns.Bottom,
                SoilProfileDatabaseColumns.Top,
                SoilProfileDatabaseColumns.AbovePhreaticLevel,
                SoilProfileDatabaseColumns.BelowPhreaticLevel,
                SoilProfileDatabaseColumns.DryUnitWeight,
                SoilProfileDatabaseColumns.IsAquifer,
                SoilProfileDatabaseColumns.SoilProfileId,
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
                            "p.SP2D_ID as {9}",
                            "FROM Mechanism as m",
                            "JOIN MechanismPointLocation as mpl ON mpl.ME_ID = m.ME_ID",
                            "JOIN SoilProfile2D as p ON p.SP2D_ID = mpl.SP2D_ID",
                            "JOIN {10}",
                            "JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID",
                            "LEFT JOIN {11}",
                            "LEFT JOIN {12}",
                            "WHERE m.ME_Name = @{13}",
                            "ORDER BY ProfileName;"),
                SoilProfileDatabaseColumns.Dimension,
                SoilProfileDatabaseColumns.ProfileName,
                SoilProfileDatabaseColumns.LayerCount,
                SoilProfileDatabaseColumns.LayerGeometry,
                SoilProfileDatabaseColumns.IntersectionX,
                SoilProfileDatabaseColumns.AbovePhreaticLevel,
                SoilProfileDatabaseColumns.BelowPhreaticLevel,
                SoilProfileDatabaseColumns.DryUnitWeight,
                SoilProfileDatabaseColumns.IsAquifer,
                SoilProfileDatabaseColumns.SoilProfileId,
                layer2DCountQuery,
                materialPropertiesQuery,
                layer2DPropertiesQuery,
                mechanismParameterName);

            dataReader = CreateDataReader(countQuery + query2D + query1D, new SQLiteParameter
            {
                DbType = DbType.String,
                Value = pipingMechanismName,
                ParameterName = mechanismParameterName
            });
        }

        private void GetCount()
        {
            dataReader.Read();
            Count = (int) Read<long>(SoilProfileDatabaseColumns.ProfileCount);
            dataReader.NextResult();
        }
    }
}