// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a DSoil database file and reads 1d profiles from this database.
    /// </summary>
    public class SoilProfile1DReader : SqLiteDatabaseReaderBase, IRowBasedDatabaseReader
    {
        private IDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="SoilProfile1DReader"/>, which will use the 
        /// <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public SoilProfile1DReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Gets a value indicating whether or not more soil profiles can be read using 
        /// the <see cref="SoilProfile1DReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Initializes the database reader.
        /// </summary>
        public void Initialize()
        {
            PrepareReader();
            MoveNext();
        }

        /// <summary>
        /// Reads the information for the next soil profile from the database and creates a 
        /// <see cref="SoilProfile1D"/> instance of the information.
        /// </summary>
        /// <returns>The next <see cref="SoilProfile1D"/> from the database, or <c>null</c> 
        /// if no more soil profile can be read.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the database returned incorrect 
        /// values for required properties.</exception>
        public SoilProfile1D ReadSoilProfile()
        {
            try
            {
                SoilProfile1D soilProfile = TryReadSoilProfile();
                MoveNext();
                return soilProfile;
            }
            catch (SystemException exception) when (exception is FormatException ||
                                                    exception is OverflowException ||
                                                    exception is InvalidCastException)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.SoilProfileReader_Error_reading_soil_profile_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        public void MoveNext()
        {
            HasNext = MoveNext(dataReader);
        }

        public T Read<T>(string columnName)
        {
            return (T) dataReader[columnName];
        }

        public T ReadOrDefault<T>(string columnName)
        {
            object valueObject = dataReader[columnName];
            if (valueObject.Equals(DBNull.Value))
            {
                return default(T);
            }
            return (T) valueObject;
        }

        protected override void Dispose(bool disposing)
        {
            if (dataReader != null)
            {
                dataReader.Close();
                dataReader.Dispose();
                dataReader = null;
            }
            base.Dispose(disposing);
        }

        private SoilProfile1D TryReadSoilProfile()
        {
            var criticalProperties = new RequiredProfileProperties(this);
            var soilProfile = new SoilProfile1D(criticalProperties.ProfileName, criticalProperties.Bottom, new []{new SoilLayer1D(1000) });

            for (var i = 1; i <= criticalProperties.LayerCount; i++)
            {
                MoveNext();
            }

            return soilProfile;
        }

        private void PrepareReader()
        {
            string subQueryGetNumberOfLayerProfile1D =
                "SELECT " +
                "SP1D_ID, " +
                $"COUNT(*) AS {SoilProfileTableDefinitions.LayerCount} " +
                "FROM SoilLayer1D " +
                "GROUP BY SP1D_ID";

            string subQueryGetMaterialPropertiesOfLayer =
                "SELECT " +
                "mat.MA_ID, " +
                $"mat.MA_Name AS {SoilProfileTableDefinitions.MaterialName}, " +
                $"max(case when pn.PN_Name = 'Color' then pv.PV_Value end) {SoilProfileTableDefinitions.Color}, " +
                $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Dist_Type end) {SoilProfileTableDefinitions.BelowPhreaticLevelDistribution}, " +
                $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Shift end) {SoilProfileTableDefinitions.BelowPhreaticLevelShift}, " +
                $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Mean end) {SoilProfileTableDefinitions.BelowPhreaticLevelMean}, " +
                $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Deviation end) {SoilProfileTableDefinitions.BelowPhreaticLevelDeviation}, " +
                $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Dist_Type end) {SoilProfileTableDefinitions.PermeabilityDistribution}, " +
                $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Shift end) {SoilProfileTableDefinitions.PermeabilityShift}, " +
                $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Mean end) {SoilProfileTableDefinitions.PermeabilityMean}, " +
                $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Variation end) {SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation}, " +
                $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Dist_Type end) {SoilProfileTableDefinitions.DiameterD70Distribution}, " +
                $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Shift end) {SoilProfileTableDefinitions.DiameterD70Shift}, " +
                $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Mean end) {SoilProfileTableDefinitions.DiameterD70Mean}, " +
                $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Variation end) {SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation} " +
                "FROM ParameterNames AS pn " +
                "LEFT JOIN ParameterValues AS pv USING(PN_ID) " +
                "LEFT JOIN Stochast AS s USING(PN_ID) " +
                "JOIN Materials AS mat " +
                "WHERE pv.MA_ID = mat.MA_ID OR s.MA_ID = mat.MA_ID " +
                "GROUP BY mat.MA_ID ";

            string subQueryGetLayerPropertiesOfLayer1D =
                "SELECT " +
                "SL1D_ID, " +
                $"PV_Value AS {SoilProfileTableDefinitions.IsAquifer} " +
                "FROM ParameterNames " +
                "JOIN LayerParameterValues USING(PN_ID) " +
                $"WHERE PN_NAME = '{SoilProfileTableDefinitions.IsAquifer}'";

            string querySoilProfile1D =
                "SELECT " +
                $"1 AS {SoilProfileTableDefinitions.Dimension}, " +
                $"sp1d.SP1D_Name AS {SoilProfileTableDefinitions.ProfileName}, " +
                $"layerCount.{SoilProfileTableDefinitions.LayerCount}, " +
                $"sp1d.BottomLevel AS {SoilProfileTableDefinitions.Bottom}, " +
                $"sl1d.TopLevel AS {SoilProfileTableDefinitions.Top}, " +
                $"{SoilProfileTableDefinitions.MaterialName}, " +
                $"{SoilProfileTableDefinitions.IsAquifer}, " +
                $"{SoilProfileTableDefinitions.Color}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDistribution}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelShift}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelMean}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDeviation}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Distribution}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Shift}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Mean}, " +
                $"{SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.PermeabilityDistribution}, " +
                $"{SoilProfileTableDefinitions.PermeabilityShift}, " +
                $"{SoilProfileTableDefinitions.PermeabilityMean}, " +
                $"{SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation}, " +
                $"sp1d.SP1D_ID AS {SoilProfileTableDefinitions.SoilProfileId} " +
                "FROM Segment AS segment " +
                "JOIN (SELECT SSM_ID, SP1D_ID, SP2D_ID FROM StochasticSoilProfile GROUP BY SSM_ID, SP1D_ID, SP2D_ID) ssp USING(SSM_ID) " +
                "JOIN SoilProfile1D sp1d USING (SP1D_ID) " +
                $"JOIN ({subQueryGetNumberOfLayerProfile1D}) {SoilProfileTableDefinitions.LayerCount} USING (SP1D_ID) " +
                "JOIN SoilLayer1D sl1d USING (SP1D_ID) " +
                $"LEFT JOIN ({subQueryGetMaterialPropertiesOfLayer}) materialProperties USING(MA_ID) " +
                $"LEFT JOIN ({subQueryGetLayerPropertiesOfLayer1D}) layerProperties USING(SL1D_ID) " +
                "GROUP BY sp1d.SP1D_ID, sl1d.SL1D_ID;";

            try
            {
                dataReader = CreateDataReader(querySoilProfile1D);
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.SoilProfileReader_Error_reading_soil_profile_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        private class RequiredProfileProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="RequiredProfileProperties"/>, which contains properties
            /// that are required to create a complete <see cref="SoilProfile1D"/>. If these properties
            /// cannot be read, then the reader can proceed to the next profile.
            /// </summary>
            /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> to read the required profile property values from.</param>
            /// <exception cref="SoilProfileReadException">Thrown when the values in the database could not be 
            /// casted to the expected column types.</exception>
            internal RequiredProfileProperties(IRowBasedDatabaseReader reader)
            {
                const string readColumn = SoilProfileTableDefinitions.Bottom;
                try
                {
                    ProfileName = reader.Read<string>(SoilProfileTableDefinitions.ProfileName);
                    Bottom = reader.Read<double>(readColumn);
                    LayerCount = reader.Read<long>(SoilProfileTableDefinitions.LayerCount);
                    ProfileId = reader.Read<long>(SoilProfileTableDefinitions.SoilProfileId);
                }
                catch (InvalidCastException e)
                {
                    string message = new FileReaderErrorMessageBuilder(reader.Path)
                        .WithSubject(string.Format(Resources.SoilProfileReader_SoilProfileName_0_, ProfileName))
                        .Build(string.Format(Resources.SoilProfileReader_Profile_has_invalid_value_on_Column_0_, readColumn));
                    throw new SoilProfileReadException(message, ProfileName, e);
                }
            }

            /// <summary>
            /// The bottom of the profile.
            /// </summary>
            public double Bottom { get; }

            /// <summary>
            /// The name of the profile to read.
            /// </summary>
            public string ProfileName { get; }

            /// <summary>
            /// The number of layers that the profile has to read.
            /// </summary>
            public long LayerCount { get; }

            /// <summary>
            /// Gets the database identifier of the profile.
            /// </summary>
            public long ProfileId { get; }
        }
    }
}