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
            string soilProfile2DQuery = SoilDatabaseQueryBuilder.GetSoilProfile2DQuery();

            try
            {
                dataReader = CreateDataReader(soilProfile2DQuery);
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
                throw new SoilProfileReadException(Resources.SoilProfile_Cannot_construct_SoilProfile_without_layers, properties.ProfileName);
            }

            for (var i = 1; i <= properties.LayerCount; i++)
            {
                soilLayers.Add(ReadSoilLayerFrom(this, properties.ProfileName));
                MoveNext();
            }

            return new SoilProfile2D(properties.ProfileId,
                                     properties.ProfileName,
                                     soilLayers)
            {
                IntersectionX = properties.IntersectionX
            };
        }

        /// <summary>
        /// Reads a <see cref="SoilLayer2D"/> from the given <paramref name="reader"/>.
        /// </summary>
        /// <exception cref="SoilProfileReadException">Thrown when reading properties of the layers failed.</exception>
        private static SoilLayer2D ReadSoilLayerFrom(IRowBasedDatabaseReader reader, string profileName)
        {
            var properties = new Layer2DProperties(reader, profileName);
            byte[] geometryValue = properties.GeometryValue;

            SoilLayer2D soilLayer;
            try
            {
                soilLayer = new SoilLayer2DGeometryReader().Read(geometryValue);
            }
            catch (SoilLayerConversionException e)
            {
                throw CreateSoilProfileReadException(reader.Path, profileName, e);
            }

            if (soilLayer != null)
            {
                soilLayer.IsAquifer = properties.IsAquifer.HasValue && properties.IsAquifer.Value.Equals(1.0);
                soilLayer.MaterialName = properties.MaterialName ?? string.Empty;
                soilLayer.Color = SoilLayerColorConverter.Convert(properties.Color);

                soilLayer.BelowPhreaticLevelDistribution = properties.BelowPhreaticLevelDistribution;
                soilLayer.BelowPhreaticLevelShift = properties.BelowPhreaticLevelShift ?? double.NaN;
                soilLayer.BelowPhreaticLevelMean = properties.BelowPhreaticLevelMean ?? double.NaN;
                soilLayer.BelowPhreaticLevelDeviation = properties.BelowPhreaticLevelDeviation ?? double.NaN;

                soilLayer.DiameterD70Distribution = properties.DiameterD70Distribution;
                soilLayer.DiameterD70Shift = properties.DiameterD70Shift ?? double.NaN;
                soilLayer.DiameterD70Mean = properties.DiameterD70Mean ?? double.NaN;
                soilLayer.DiameterD70CoefficientOfVariation = properties.DiameterD70CoefficientOfVariation ?? double.NaN;

                soilLayer.PermeabilityDistribution = properties.PermeabilityDistribution;
                soilLayer.PermeabilityShift = properties.PermeabilityShift ?? double.NaN;
                soilLayer.PermeabilityMean = properties.PermeabilityMean ?? double.NaN;
                soilLayer.PermeabilityCoefficientOfVariation = properties.PermeabilityCoefficientOfVariation ?? double.NaN;
            }
            return soilLayer;
        }

        private static SoilProfileReadException CreateSoilProfileReadException(string filePath, string profileName, Exception innerException)
        {
            string message = new FileReaderErrorMessageBuilder(filePath)
                .WithSubject(string.Format(Resources.SoilProfileReader_SoilProfileName_0_, profileName))
                .Build(innerException.Message);
            return new SoilProfileReadException(message, profileName, innerException);
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
                    IntersectionX = reader.ReadOrDefault<double?>(readColumn) ?? double.NaN;

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