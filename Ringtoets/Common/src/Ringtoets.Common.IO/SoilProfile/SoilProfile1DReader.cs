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
using Ringtoets.Common.IO.Exceptions;
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
        /// <exception cref="SoilProfileReadException">Thrown when reading properties of the profile failed.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when the database returned incorrect 
        /// values for required properties.</exception>
        public SoilProfile1D ReadSoilProfile()
        {
            try
            {
                SoilProfile1D soilProfile = TryReadSoilProfile();
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

        /// <summary>
        /// Tries to read and create a <see cref="SoilProfile1D"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SoilProfileReadException">Thrown when reading properties of the profile failed.</exception>
        private SoilProfile1D TryReadSoilProfile()
        {
            var properties = new RequiredProfileProperties(this);

            var soilLayers = new List<SoilLayer1D>();
            for (var i = 1; i <= properties.LayerCount; i++)
            {
                soilLayers.Add(ReadSoilLayerFrom(this, properties.ProfileName));
                MoveNext();
            }

            try
            {
                return new SoilProfile1D(properties.ProfileId,
                                         properties.ProfileName,
                                         properties.Bottom,
                                         soilLayers);
            }
            catch (ArgumentException exception)
            {
                throw new SoilProfileReadException(
                    Resources.SoilProfile1DReader_ReadSoilProfile_Failed_to_construct_profile_from_read_data,
                    properties.ProfileName,
                    exception);
            }
        }

        private void PrepareReader()
        {
            string soilProfile1DQuery = SoilDatabaseQueryBuilder.GetSoilProfile1DQuery();

            try
            {
                dataReader = CreateDataReader(soilProfile1DQuery);
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.SoilProfileReader_Error_reading_soil_profile_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Reads a <see cref="SoilLayer1D"/> from the given <paramref name="reader"/>.
        /// </summary>
        /// <exception cref="SoilProfileReadException">Thrown when reading properties of the layers failed.</exception>
        private static SoilLayer1D ReadSoilLayerFrom(IRowBasedDatabaseReader reader, string profileName)
        {
            var properties = new Layer1DProperties(reader, profileName);
            var soilLayer = new SoilLayer1D(properties.Top)
            {
                MaterialName = properties.MaterialName ?? string.Empty
            };

            if (properties.IsAquifer.HasValue)
            {
                soilLayer.IsAquifer = properties.IsAquifer.Value.Equals(1.0);
            }
            if (properties.Color.HasValue)
            {
                soilLayer.Color = SoilLayerColorConverter.Convert(properties.Color);
            }
            if (properties.BelowPhreaticLevelDistribution.HasValue)
            {
                soilLayer.BelowPhreaticLevelDistribution = properties.BelowPhreaticLevelDistribution.Value;
            }
            if (properties.BelowPhreaticLevelShift.HasValue)
            {
                soilLayer.BelowPhreaticLevelShift = properties.BelowPhreaticLevelShift.Value;
            }
            if (properties.BelowPhreaticLevelMean.HasValue)
            {
                soilLayer.BelowPhreaticLevelMean = properties.BelowPhreaticLevelMean.Value;
            }
            if (properties.BelowPhreaticLevelDeviation.HasValue)
            {
                soilLayer.BelowPhreaticLevelDeviation = properties.BelowPhreaticLevelDeviation.Value;
            }
            if (properties.DiameterD70Distribution.HasValue)
            {
                soilLayer.DiameterD70Distribution = properties.DiameterD70Distribution.Value;
            }
            if (properties.DiameterD70Shift.HasValue)
            {
                soilLayer.DiameterD70Shift = properties.DiameterD70Shift.Value;
            }
            if (properties.DiameterD70Mean.HasValue)
            {
                soilLayer.DiameterD70Mean = properties.DiameterD70Mean.Value;
            }
            if (properties.DiameterD70CoefficientOfVariation.HasValue)
            {
                soilLayer.DiameterD70CoefficientOfVariation = properties.DiameterD70CoefficientOfVariation.Value;
            }
            if (properties.PermeabilityDistribution.HasValue)
            {
                soilLayer.PermeabilityDistribution = properties.PermeabilityDistribution.Value;
            }
            if (properties.PermeabilityShift.HasValue)
            {
                soilLayer.PermeabilityShift = properties.PermeabilityShift.Value;
            }
            if (properties.PermeabilityMean.HasValue)
            {
                soilLayer.PermeabilityMean = properties.PermeabilityMean.Value;
            }
            if (properties.PermeabilityCoefficientOfVariation.HasValue)
            {
                soilLayer.PermeabilityCoefficientOfVariation = properties.PermeabilityCoefficientOfVariation.Value;
            }

            return soilLayer;
        }

        private class Layer1DProperties : LayerProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="Layer1DProperties"/> which contains properties
            /// that are required to create a complete <see cref="SoilLayer1D"/>. If these properties
            /// cannot be read, then the reader can proceed to the next profile.
            /// </summary>
            /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> to read the required layer property values from.</param>
            /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
            /// <exception cref="SoilProfileReadException">Thrown when the values in the database could not be 
            /// casted to the expected column types.</exception>
            internal Layer1DProperties(IRowBasedDatabaseReader reader, string profileName)
                : base(reader, profileName)
            {
                const string readColumn = SoilProfileTableDefinitions.Top;
                try
                {
                    Top = reader.Read<double>(readColumn);
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
            /// Gets the top level of the 1D soil layer.
            /// </summary>
            public double Top { get; }
        }

        private class RequiredProfileProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="RequiredProfileProperties"/> which contains properties
            /// that are required to create a complete <see cref="SoilProfile1D"/>. If these properties
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

                    readColumn = SoilProfileTableDefinitions.Bottom;
                    Bottom = reader.Read<double>(readColumn);

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