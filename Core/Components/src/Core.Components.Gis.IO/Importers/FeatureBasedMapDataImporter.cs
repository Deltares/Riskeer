// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.Data.Removable;
using Core.Components.Gis.IO.Properties;
using Core.Components.Gis.IO.Readers;
using DotSpatial.Data;

namespace Core.Components.Gis.IO.Importers
{
    /// <summary>
    /// Imports a <see cref="FeatureBasedMapData"/> from a <see cref="Shapefile"/> and stores
    /// it on a <see cref="MapDataCollection"/>.
    /// </summary>
    public class FeatureBasedMapDataImporter : FileImporterBase<MapDataCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="FeatureBasedMapDataImporter"/>.
        /// </summary>
        /// <param name="importTarget">The <see cref="MapDataCollection"/> to add the imported data to.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="importTarget"/>
        /// or <paramref name="filePath"/> is <c>null</c>.</exception>
        public FeatureBasedMapDataImporter(MapDataCollection importTarget, string filePath)
            : base(filePath, importTarget) {}

        protected override bool OnImport()
        {
            ReadResult<FeatureBasedMapData> readResult = ReadFeatureBasedMapData();
            if (readResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            AddFeatureBasedMapDataToMapDataCollection(readResult.Items.First());

            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            Log.Info(Resources.FeatureBasedMapDataImporter_HandleUserCancelingImport_Import_canceled_No_data_changed);
        }

        private void AddFeatureBasedMapDataToMapDataCollection(FeatureBasedMapData importedMapData)
        {
            ImportTarget.Add(RemovableMapDataConverter.FromFeatureBasedMapData(importedMapData));
        }

        private ReadResult<FeatureBasedMapData> ReadFeatureBasedMapData()
        {
            try
            {
                string shapeFileName = Path.GetFileNameWithoutExtension(FilePath);
                Shapefile featureSet = Shapefile.OpenFile(FilePath);

                FeatureBasedMapData importedData;

                switch (featureSet.FeatureType)
                {
                    case FeatureType.Point:
                    case FeatureType.MultiPoint:
                        using (ShapeFileReaderBase reader = new PointShapeFileReader(FilePath))
                        {
                            importedData = reader.ReadShapeFile(shapeFileName);
                        }

                        break;
                    case FeatureType.Line:
                        using (ShapeFileReaderBase reader = new PolylineShapeFileReader(FilePath))
                        {
                            importedData = reader.ReadShapeFile(shapeFileName);
                        }

                        break;
                    case FeatureType.Polygon:
                        using (ShapeFileReaderBase reader = new PolygonShapeFileReader(FilePath))
                        {
                            importedData = reader.ReadShapeFile(shapeFileName);
                        }

                        break;
                    default:
                        throw new CriticalFileReadException(Resources.FeatureBasedMapDataImporter_Import_ShapeFile_Contains_Unsupported_Data);
                }

                return new ReadResult<FeatureBasedMapData>(false)
                {
                    Items = new[]
                    {
                        importedData
                    }
                };
            }
            catch (ArgumentException)
            {
                return HandleCriticalFileReadError(Resources.FeatureBasedMapDataImporter_Import_File_does_not_contain_geometries);
            }
            catch (FileNotFoundException)
            {
                return HandleCriticalFileReadError(Resources.FeatureBasedMapDataImporter_Import_File_does_not_exist_or_misses_needed_files);
            }
            catch (IOException)
            {
                return HandleCriticalFileReadError(Resources.FeatureBasedMapDataImporter_Import_An_error_occurred_when_trying_to_read_the_file);
            }
            catch (CriticalFileReadException e)
            {
                return HandleCriticalFileReadError(e.Message);
            }
            catch (Exception)
            {
                // Because NullReferenceException or NotImplementedException when reading in a corrupt shape file
                // from a third party library is expected, we catch all the exceptions here.
                return HandleCriticalFileReadError(Resources.FeatureBasedMapDataImporter_Import_An_error_occurred_when_trying_to_read_the_file);
            }
        }

        private ReadResult<FeatureBasedMapData> HandleCriticalFileReadError(string message)
        {
            string errorMessage = string.Format(Resources.FeatureBasedMapDataImporter_HandleCriticalFileReadError_Error_0_no_maplayer_imported,
                                                new FileReaderErrorMessageBuilder(FilePath).Build(message));
            Log.Error(errorMessage);
            return new ReadResult<FeatureBasedMapData>(true);
        }
    }
}