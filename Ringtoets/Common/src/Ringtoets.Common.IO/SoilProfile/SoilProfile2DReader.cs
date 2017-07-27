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
using System.Collections.Generic;
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
    /// This class reads a DSoil database file and reads 2d profiles from this database.
    /// </summary>
    public class SoilProfile2DReader : SqLiteDatabaseReaderBase, IRowBasedDatabaseReader
    {
        private IDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="SoilProfile2DReader"/>, which will use the 
        /// <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public SoilProfile2DReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Gets a value indicating whether or not more soil profiles can be read using 
        /// the <see cref="SoilProfile2DReader"/>.
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
        /// <see cref="SoilProfile2D"/> instance of the information.
        /// </summary>
        /// <returns>The next <see cref="SoilProfile2D"/> from the database, or <c>null</c> 
        /// if no more soil profile can be read.</returns>
        /// <exception cref="SoilProfileReadException">Thrown when reading properties of the profile failed.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when the database returned incorrect 
        /// values for required properties.</exception>
        public SoilProfile2D ReadSoilProfile()
        {
            try
            {
                return TryReadSoilProfile();
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

        private void PrepareReader()
        {
            string subQueryGetNumberOfLayerProfile2D =
                $"SELECT SP2D_ID, COUNT(*) as {SoilProfileTableDefinitions.LayerCount} " +
                "FROM SoilLayer2D " +
                "GROUP BY SP2D_ID";
            string subQueryGetMaterialPropertiesOfLayer =
                "SELECT " +
                "mat.MA_ID, " +
                $"mat.MA_Name as {SoilProfileTableDefinitions.MaterialName}, " +
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

            string subQueryGetLayerPropertiesOfLayer2D =
                "SELECT " +
                "SL2D_ID, " +
                $"PV_Value as {SoilProfileTableDefinitions.IsAquifer} " +
                "FROM ParameterNames " +
                "JOIN LayerParameterValues USING(PN_ID) " +
                $"WHERE PN_NAME = '{SoilProfileTableDefinitions.IsAquifer}'";

            string querySoilProfile21D =
                "SELECT " +
                $"sp2d.SP2D_Name as {SoilProfileTableDefinitions.ProfileName}, " +
                $"layerCount.{SoilProfileTableDefinitions.LayerCount}, " +
                $"sl2d.GeometrySurface as {SoilProfileTableDefinitions.LayerGeometry}, " +
                $"mpl.X as {SoilProfileTableDefinitions.IntersectionX}, " +
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
                $"sp2d.SP2D_ID as {SoilProfileTableDefinitions.SoilProfileId} " +
                "FROM Mechanism AS m " +
                "JOIN Segment AS segment USING(ME_ID) " +
                "JOIN (SELECT SSM_ID, SP1D_ID, SP2D_ID FROM StochasticSoilProfile GROUP BY SSM_ID, SP1D_ID, SP2D_ID) ssp USING(SSM_ID) " +
                "JOIN SoilProfile2D sp2d USING (SP2D_ID) " +
                "JOIN (" +
                subQueryGetNumberOfLayerProfile2D +
                ") layerCount USING (SP2D_ID) " +
                "JOIN SoilLayer2D sl2d USING (SP2D_ID) " +
                "LEFT JOIN MechanismPointLocation mpl USING(ME_ID, SP2D_ID) " +
                "LEFT JOIN (" +
                subQueryGetMaterialPropertiesOfLayer +
                ") materialProperties USING(MA_ID) " +
                "LEFT JOIN (" +
                subQueryGetLayerPropertiesOfLayer2D +
                ") layerProperties USING(SL2D_ID) " +
                "GROUP BY sp2d.SP2D_ID, sl2d.SL2D_ID;";

            try
            {
                dataReader = CreateDataReader(querySoilProfile21D);
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.SoilProfileReader_Error_reading_soil_profile_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        private SoilProfile2D TryReadSoilProfile()
        {
            var properties = new RequiredProfileProperties(this);

            var soilLayers = new List<SoilLayer2D>();

            if (properties.LayerCount == 0)
            {
                MoveNext();
            }
            else
            {
                for (var i = 1; i <= properties.LayerCount; i++)
                {
                    soilLayers.Add(ReadSoilLayerFrom(this, properties.ProfileName));
                    MoveNext();
                }
            }

            return new SoilProfile2D(properties.ProfileId,
                                     properties.ProfileName,
                                     soilLayers);
        }

        /// <summary>
        /// Reads a <see cref="SoilLayer2D"/> from the given <paramref name="reader"/>.
        /// </summary>
        /// <exception cref="SoilProfileReadException">Thrown when reading properties of the layers failed.</exception>
        private static SoilLayer2D ReadSoilLayerFrom(IRowBasedDatabaseReader reader, string profileName)
        {
            var properties = new Layer2DProperties(reader, profileName);

            return new SoilLayer2D();
        }

        private class Layer2DProperties : LayerProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="Layer2DProperties"/>, which contains properties
            /// that are required to create a complete <see cref="SoilLayer2D"/>. If these properties
            /// cannot be read, then the reader can proceed to the next profile.
            /// </summary>
            /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> to read the required layer property values from.</param>
            /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
            /// <exception cref="SoilProfileReadException">Thrown when the values in the database could not be 
            /// casted to the expected column types.</exception>
            internal Layer2DProperties(IRowBasedDatabaseReader reader, string profileName)
                : base(reader, profileName)
            {
                const string readColumn = SoilProfileTableDefinitions.LayerGeometry;
                try
                {
                    GeometryValue = reader.Read<byte[]>(readColumn);
                }
                catch (InvalidCastException e)
                {
                    string message = new FileReaderErrorMessageBuilder(reader.Path)
                        .WithSubject(string.Format(Resources.SoilProfileReader_SoilProfileName_0_, profileName))
                        .Build(string.Format(Resources.SoilProfileReader_Profile_has_invalid_value_on_Column_0_, readColumn));
                    throw new SoilProfileReadException(message, profileName, e);
                }
            }

            /// <summary>
            /// Gets the geometry for the layer.
            /// </summary>
            public byte[] GeometryValue { get; }
        }

        private class RequiredProfileProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="RequiredProfileProperties"/>, which contains properties
            /// that are required to create a complete <see cref="SoilProfile2D"/>. If these properties
            /// cannot be read, then the reader can proceed to the next profile.
            /// </summary>
            /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> to read the required profile property values from.</param>
            /// <exception cref="SoilProfileReadException">Thrown when the values in the database could not be 
            /// casted to the expected column types.</exception>
            internal RequiredProfileProperties(IRowBasedDatabaseReader reader)
            {
                string readColumn = SoilProfileTableDefinitions.ProfileName;
                try
                {
                    ProfileName = reader.Read<string>(SoilProfileTableDefinitions.ProfileName);

                    readColumn = SoilProfileTableDefinitions.IntersectionX;
                    IntersectionX = reader.Read<double>(readColumn);

                    readColumn = SoilProfileTableDefinitions.LayerCount;
                    LayerCount = reader.Read<long>(readColumn);

                    readColumn = SoilProfileTableDefinitions.SoilProfileId;
                    ProfileId = reader.Read<long>(readColumn);
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
            /// The 1d intersection of the profile.
            /// </summary>
            public double IntersectionX { get; }

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