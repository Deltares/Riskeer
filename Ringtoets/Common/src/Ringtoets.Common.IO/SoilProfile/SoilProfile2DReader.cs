// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Builders;
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
        /// <item>the <paramref name="databaseFilePath"/> contains invalid characters;</item>
        /// <item>no file could be found at <paramref name="databaseFilePath"/>.</item>
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
        /// <see cref="SoilProfile2D"/> instance from the information.
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
            var soilLayerGeometryLookup = new Dictionary<SoilLayer2DGeometry, Layer2DProperties>();

            long soilProfileId = criticalProperties.ProfileId;
            RequiredProfileProperties properties;

            try
            {
                properties = new RequiredProfileProperties(this, criticalProperties.ProfileName);

                var geometryReader = new SoilLayer2DGeometryReader();
                for (var i = 1; i <= criticalProperties.LayerCount; i++)
                {
                    ReadSoilLayerGeometryFrom(this, geometryReader, criticalProperties.ProfileName, soilLayerGeometryLookup);
                    MoveNext();
                }
            }
            catch (SoilProfileReadException)
            {
                MoveToNextProfile(soilProfileId);
                throw;
            }

            MoveToNextProfile(soilProfileId);
            return new SoilProfile2D(soilProfileId,
                                     criticalProperties.ProfileName,
                                     GetHierarchicallyOrderedSoilLayers(soilLayerGeometryLookup).ToArray(),
                                     GetPreconsolidationStresses(soilProfileId).ToArray())
            {
                IntersectionX = properties.IntersectionX
            };
        }

        /// <summary>
        /// Gets the preconsolidation stresses belonging to the <paramref name="currentSoilProfileId"/>.
        /// </summary>
        /// <param name="currentSoilProfileId">The current soil profile id.</param>
        /// <returns>A collection of the read <see cref="PreconsolidationStress"/> .</returns>
        /// <exception cref="SoilProfileReadException">Thrown when the preconsolidation
        /// stresses could not be read.</exception>
        private IEnumerable<PreconsolidationStress> GetPreconsolidationStresses(long currentSoilProfileId)
        {
            if (!preconsolidationStressReader.HasNext || preconsolidationStressReader.ReadSoilProfileId() != currentSoilProfileId)
            {
                return new PreconsolidationStress[0];
            }

            return preconsolidationStressReader.ReadPreconsolidationStresses();
        }

        private static IEnumerable<SoilLayer2D> GetHierarchicallyOrderedSoilLayers(Dictionary<SoilLayer2DGeometry, Layer2DProperties> soilLayerGeometryLookup)
        {
            SoilLayer2DGeometry[] soilLayerGeometries = soilLayerGeometryLookup.Keys.ToArray();
            SoilLayer2DLoop[] innerLoops = soilLayerGeometries.SelectMany(slg => slg.InnerLoops).ToArray();

            foreach (SoilLayer2DGeometry soilLayerGeometry in soilLayerGeometries)
            {
                if (!IsNestedLayer(innerLoops, soilLayerGeometry))
                {
                    yield return CreateSoilLayer2D(soilLayerGeometry,
                                                   soilLayerGeometryLookup[soilLayerGeometry],
                                                   CreateNestedSoilLayersRecursively(soilLayerGeometryLookup, soilLayerGeometry));
                }
            }
        }

        private static IEnumerable<SoilLayer2D> CreateNestedSoilLayersRecursively(Dictionary<SoilLayer2DGeometry, Layer2DProperties> soilLayerGeometryLookup, SoilLayer2DGeometry soilLayerGeometry)
        {
            var nestedLayers = new List<SoilLayer2D>();
            SoilLayer2DGeometry[] soilLayerGeometries = soilLayerGeometryLookup.Keys.ToArray();

            foreach (SoilLayer2DLoop innerLoop in soilLayerGeometry.InnerLoops)
            {
                SoilLayer2DGeometry nestedSoilLayerGeometry = soilLayerGeometries.First(slg => slg.OuterLoop.Equals(innerLoop));
                SoilLayer2D nestedSoilLayer = CreateSoilLayer2D(nestedSoilLayerGeometry,
                                                                soilLayerGeometryLookup[nestedSoilLayerGeometry],
                                                                CreateNestedSoilLayersRecursively(soilLayerGeometryLookup, nestedSoilLayerGeometry));

                nestedLayers.Add(nestedSoilLayer);
            }

            return StripDuplicateNestedLayers(nestedLayers);
        }

        private static SoilLayer2D CreateSoilLayer2D(SoilLayer2DGeometry soilLayerGeometry, LayerProperties layerProperties, IEnumerable<SoilLayer2D> nestedLayers)
        {
            var soilLayer = new SoilLayer2D(soilLayerGeometry.OuterLoop, nestedLayers);

            SoilLayerHelper.SetSoilLayerBaseProperties(soilLayer, layerProperties);

            return soilLayer;
        }

        private static bool IsNestedLayer(IEnumerable<SoilLayer2DLoop> innerLoops, SoilLayer2DGeometry soilLayerGeometry)
        {
            return innerLoops.Any(il => il.Equals(soilLayerGeometry.OuterLoop));
        }

        private static IEnumerable<SoilLayer2D> StripDuplicateNestedLayers(List<SoilLayer2D> nestedLayers)
        {
            return nestedLayers.Where(nl => !nestedLayers.Except(
                                                             new[]
                                                             {
                                                                 nl
                                                             })
                                                         .SelectMany(GetLayersRecursively)
                                                         .Any(l => l.OuterLoop.Equals(nl.OuterLoop)));
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
        /// Reads a <see cref="SoilLayer2DGeometry"/> from the given <paramref name="databaseReader"/>.
        /// </summary>
        /// <param name="databaseReader">The reader to read a geometry from.</param>
        /// <param name="geometryReader">The geometry reader to use.</param>
        /// <param name="profileName">The name of the profile to read a geometry for.</param>
        /// <param name="soilLayerGeometriesLookup">The lookup to add the read data to.</param>
        /// <exception cref="SoilProfileReadException">Thrown when reading properties of the geometry failed.</exception>
        private static void ReadSoilLayerGeometryFrom(IRowBasedDatabaseReader databaseReader, SoilLayer2DGeometryReader geometryReader, string profileName,
                                                      IDictionary<SoilLayer2DGeometry, Layer2DProperties> soilLayerGeometriesLookup)
        {
            var properties = new Layer2DProperties(databaseReader, profileName);

            try
            {
                soilLayerGeometriesLookup[geometryReader.Read(properties.GeometryValue)] = properties;
            }
            catch (SoilLayerConversionException e)
            {
                throw CreateSoilProfileReadException(databaseReader.Path, profileName, e);
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