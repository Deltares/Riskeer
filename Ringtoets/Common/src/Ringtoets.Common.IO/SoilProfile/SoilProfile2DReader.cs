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
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a D-Soil Model file and reads 2d profiles from this database.
    /// </summary>
    public class SoilProfile2DReader : SqLiteDatabaseReaderBase, IRowBasedDatabaseReader
    {
        private IDataReader dataReader;
        private PreconsolidationStressReader preconsolidationStressReader;

        /// <summary>
        /// Creates a new instance of <see cref="SoilProfile2DReader"/> which will use the 
        /// <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters;</item>
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

            if (preconsolidationStressReader != null)
            {
                preconsolidationStressReader.Dispose();
                preconsolidationStressReader = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Steps through the result rows until a row is read which' profile id differs from <paramref name="soilProfileId"/>.
        /// </summary>
        /// <param name="soilProfileId">The id of the profile to skip.</param>
        private void MoveToNextProfile(long soilProfileId)
        {
            while (HasNext && Read<long>(SoilProfileTableDefinitions.SoilProfileId).Equals(soilProfileId))
            {
                MoveNext();
            }
        }

        private void PrepareReader()
        {
            string soilProfile2DQuery = SoilDatabaseQueryBuilder.GetSoilProfile2DQuery();

            try
            {
                dataReader = CreateDataReader(soilProfile2DQuery);

                preconsolidationStressReader = new PreconsolidationStressReader(Path);
                preconsolidationStressReader.Initialize();
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.SoilProfileReader_Error_reading_soil_profile_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Tries to read and create a <see cref="SoilProfile2D"/>.
        /// </summary>
        /// <returns>The read <see cref="SoilProfile2D"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when encountering an unrecoverable error 
        /// while reading the profile.</exception>
        /// <exception cref="SoilProfileReadException">Thrown when reading properties of the profile failed.</exception>
        private SoilProfile2D TryReadSoilProfile()
        {
            var criticalProperties = new CriticalProfileProperties(this);
            var soilLayerGeometries = new List<SoilLayer2DGeometry>();
            var stresses = new List<PreconsolidationStress>();
            long soilProfileId = criticalProperties.ProfileId;

            RequiredProfileProperties properties;

            try
            {
                properties = new RequiredProfileProperties(this, criticalProperties.ProfileName);

                for (var i = 1; i <= criticalProperties.LayerCount; i++)
                {
                    soilLayerGeometries.Add(ReadSoilLayerGeometryFrom(this, criticalProperties.ProfileName));
                    MoveNext();
                }

                stresses.AddRange(GetPreconsolidationStresses(soilProfileId));
            }
            catch (SoilProfileReadException)
            {
                MoveToNextProfile(soilProfileId);
                throw;
            }

            try
            {
                return new SoilProfile2D(soilProfileId,
                                         criticalProperties.ProfileName,
                                         GetSoilLayers(soilLayerGeometries),
                                         stresses)
                {
                    IntersectionX = properties.IntersectionX
                };
            }
            catch (ArgumentException exception)
            {
                MoveToNextProfile(soilProfileId);
                throw new SoilProfileReadException(
                    Resources.SoilProfile1DReader_ReadSoilProfile_Failed_to_construct_profile_from_read_data,
                    criticalProperties.ProfileName,
                    exception);
            }
        }

        /// <summary>
        /// Gets the preconsolidation stresses belonging to the <paramref name="currentSoilProfileId"/>.
        /// </summary>
        /// <param name="currentSoilProfileId">The current soil profile id.</param>
        /// <returns>A collection of the read <see cref="PreconsolidationStress"/> .</returns>
        /// <exception cref="SoilProfileReadException">Thrown when the preconsolidation
        /// stresses could not be read.</exception>
        private PreconsolidationStress[] GetPreconsolidationStresses(long currentSoilProfileId)
        {
            if (!preconsolidationStressReader.HasNext || preconsolidationStressReader.ReadSoilProfileId() != currentSoilProfileId)
            {
                return new PreconsolidationStress[0];
            }

            return preconsolidationStressReader.ReadPreconsolidationStresses().ToArray();
        }

        private static IEnumerable<SoilLayer2D> GetSoilLayers(List<SoilLayer2DGeometry> soilLayerGeometries)
        {
            SoilLayer2DLoop[] innerLoops = soilLayerGeometries.SelectMany(slg => slg.InnerLoops).ToArray();

            foreach (SoilLayer2DGeometry soilLayerGeometry in soilLayerGeometries)
            {
                if (IsNestedLayer(innerLoops, soilLayerGeometry))
                {
                    continue;
                }

                SoilLayer2D soilLayer = CreateSoilLayer2D(soilLayerGeometry);

                CreateNestedSoilLayersRecursively(soilLayerGeometries, soilLayerGeometry, soilLayer);

                yield return soilLayer;
            }
        }

        private static void CreateNestedSoilLayersRecursively(List<SoilLayer2DGeometry> soilLayerGeometries, SoilLayer2DGeometry soilLayerGeometry, SoilLayer2D soilLayer)
        {
            var nestedLayers = new List<SoilLayer2D>();

            foreach (SoilLayer2DLoop innerLoop in soilLayerGeometry.InnerLoops)
            {
                SoilLayer2DGeometry nestedSoilLayerGeometry = soilLayerGeometries.First(slg => slg.OuterLoop.Segments.SequenceEqual(innerLoop.Segments));
                SoilLayer2D nestedSoilLayer = CreateSoilLayer2D(nestedSoilLayerGeometry);

                CreateNestedSoilLayersRecursively(soilLayerGeometries, nestedSoilLayerGeometry, nestedSoilLayer);

                nestedLayers.Add(nestedSoilLayer);
            }

            soilLayer.NestedLayers = StripDuplicateNestedLayers(nestedLayers);
        }

        private static SoilLayer2D CreateSoilLayer2D(SoilLayer2DGeometry soilLayerGeometry)
        {
            var soilLayer = new SoilLayer2D(soilLayerGeometry.OuterLoop, soilLayerGeometry.InnerLoops);

            SoilLayerHelper.SetSoilLayerBaseProperties(soilLayer, soilLayerGeometry.LayerProperties);

            return soilLayer;
        }

        private static bool IsNestedLayer(IEnumerable<SoilLayer2DLoop> innerLoops, SoilLayer2DGeometry soilLayerGeometry)
        {
            return innerLoops.Any(il => il.Segments.SequenceEqual(soilLayerGeometry.OuterLoop.Segments));
        }

        private static IEnumerable<SoilLayer2D> StripDuplicateNestedLayers(List<SoilLayer2D> nestedLayers)
        {
            return nestedLayers.Where(nl => !nestedLayers.Except(
                                                             new[]
                                                             {
                                                                 nl
                                                             })
                                                         .SelectMany(GetLayersRecursively)
                                                         .Any(l => l.OuterLoop.Segments.SequenceEqual(nl.OuterLoop.Segments)));
        }

        private static IEnumerable<SoilLayer2D> GetLayersRecursively(SoilLayer2D soilLayer)
        {
            var layers = new List<SoilLayer2D>
            {
                soilLayer
            };

            foreach (SoilLayer2D nestedLayer in soilLayer.NestedLayers)
            {
                layers.AddRange(GetLayersRecursively(nestedLayer));
            }

            return layers;
        }

        /// <summary>
        /// Reads a <see cref="SoilLayer2DGeometry"/> from the given <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The reader to read the geometry from.</param>
        /// <param name="profileName">The name of the profile to read the geometry for.</param>
        /// <returns>A <see cref="SoilLayer2DGeometry"/>.</returns>
        /// <exception cref="SoilProfileReadException">Thrown when reading properties of the geometry failed.</exception>
        private static SoilLayer2DGeometry ReadSoilLayerGeometryFrom(IRowBasedDatabaseReader reader, string profileName)
        {
            var properties = new Layer2DProperties(reader, profileName);

            try
            {
                SoilLayer2DGeometry soilLayerGeometry = new SoilLayer2DGeometryReader().Read(properties.GeometryValue);

                soilLayerGeometry.LayerProperties = properties;

                return soilLayerGeometry;
            }
            catch (SoilLayerConversionException e)
            {
                throw CreateSoilProfileReadException(reader.Path, profileName, e);
            }
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
            /// Creates a new instance of <see cref="Layer2DProperties"/> which contains properties
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
                    string message = string.Format(Resources.SoilProfileReader_Profile_Name_0_has_invalid_value_on_Column_1,
                                                   profileName,
                                                   readColumn);
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
            /// Creates a new instance of <see cref="RequiredProfileProperties"/> which contains properties
            /// that are required to create a complete <see cref="SoilProfile2D"/>. If these properties
            /// cannot be read, then the reader can proceed to the next profile.
            /// </summary>
            /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> to read the required 
            /// profile property values from.</param>
            /// <param name="profileName">The profile name used in generating exceptions messages 
            /// if casting failed.</param>
            /// <exception cref="SoilProfileReadException">Thrown when the values in the database could not be 
            /// casted to the expected column types.</exception>
            internal RequiredProfileProperties(IRowBasedDatabaseReader reader, string profileName)
            {
                try
                {
                    IntersectionX = reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.IntersectionX) ?? double.NaN;
                }
                catch (InvalidCastException e)
                {
                    string message = string.Format(Resources.SoilProfileReader_Profile_Name_0_has_invalid_value_on_Column_1,
                                                   profileName,
                                                   SoilProfileTableDefinitions.IntersectionX);
                    throw new SoilProfileReadException(message, profileName, e);
                }
            }

            /// <summary>
            /// The 1d intersection of the profile.
            /// </summary>
            public double IntersectionX { get; }
        }
    }
}