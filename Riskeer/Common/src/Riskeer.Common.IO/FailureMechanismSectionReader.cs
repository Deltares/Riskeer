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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Readers;
using Riskeer.Common.Data.FailureMechanism;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Common.IO
{
    /// <summary>
    /// Reads <see cref="FailureMechanismSection"/> instances from a shapefile containing
    /// one or multiple line features.
    /// </summary>
    public class FailureMechanismSectionReader : IDisposable
    {
        private const string sectionNameAttributeKey = "Vaknaam";
        private readonly PolylineShapeFileReader polylineShapeFileReader;
        private readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismSectionReader"/> class.
        /// </summary>
        /// <param name="shapeFilePath">The shape file path.</param>
        /// <exception cref="ArgumentException"><paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that doesn't exist.</item>
        /// <item>The shapefile has non-line geometries in it.</item>
        /// <item>An unexpected error occurred when reading the shapefile.</item>
        /// </list>
        /// </exception>
        public FailureMechanismSectionReader(string shapeFilePath)
        {
            IOUtils.ValidateFilePath(shapeFilePath);
            filePath = shapeFilePath;
            polylineShapeFileReader = new PolylineShapeFileReader(filePath);
        }

        /// <summary>
        /// Gets the number of failure mechanism sections in the shapefile.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the shapefile does not have
        /// a required attribute defined.</exception>
        public int GetFailureMechanismSectionCount()
        {
            ValidateExistenceOfRequiredAttributes();
            return polylineShapeFileReader.GetNumberOfFeatures();
        }

        /// <summary>
        /// Reads and consumes an entry in the shapefile, using the data to create a new
        /// instance of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The <see cref="FailureMechanismSection"/> read from the file, or <c>null</c>
        /// when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when either:
        /// <list type="bullet">
        /// <item>the shapefile does not have a required attribute defined.</item>
        /// <item>the element read from the file is a multi-polyline.</item>
        /// </list></exception>
        public FailureMechanismSection ReadFailureMechanismSection()
        {
            ValidateExistenceOfRequiredAttributes();

            MapLineData lineData = ReadMapLineData();
            return lineData == null ? null : CreateFailureMechanismSection(lineData);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                polylineShapeFileReader.Dispose();
            }
        }

        /// <summary>
        /// Validates the existence of required attributes.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the shapefile does not have
        /// a required attribute defined.</exception>
        private void ValidateExistenceOfRequiredAttributes()
        {
            if (!polylineShapeFileReader.HasAttribute(sectionNameAttributeKey))
            {
                throw CreateCriticalFileReadException(
                    string.Format(RiskeerCommonIOResources.FailureMechanismSectionReader_File_lacks_required_Attribute_0_,
                                  sectionNameAttributeKey));
            }
        }

        /// <summary>
        /// Reads a new <see cref="MapLineData"/> from the file.
        /// </summary>
        /// <returns></returns>
        private MapLineData ReadMapLineData()
        {
            return polylineShapeFileReader.ReadFeature() as MapLineData;
        }

        private FailureMechanismSection CreateFailureMechanismSection(MapLineData lineData)
        {
            MapFeature[] features = lineData.Features.ToArray();
            if (features.Length > 1)
            {
                throw CreateCriticalFileReadException(RiskeerCommonIOResources.FailureMechanismSectionReader_File_has_unsupported_multiPolyline);
            }

            MapFeature feature = features[0];

            string name = GetSectionName(feature);
            Point2D[] geometryPoints = GetSectionGeometry(feature).ToArray();
            return new FailureMechanismSection(name, geometryPoints);
        }

        private string GetSectionName(MapFeature lineFeature)
        {
            var sectionName = lineFeature.MetaData[sectionNameAttributeKey] as string;

            if (string.IsNullOrWhiteSpace(sectionName))
            {
                throw CreateCriticalFileReadException(RiskeerCommonIOResources.FailureMechanismSectionReader_File_has_section_without_sectionName);
            }

            return sectionName;
        }

        private IEnumerable<Point2D> GetSectionGeometry(MapFeature lineFeature)
        {
            MapGeometry[] mapGeometries = lineFeature.MapGeometries.ToArray();
            if (mapGeometries.Length > 1)
            {
                throw CreateCriticalFileReadException(RiskeerCommonIOResources.FailureMechanismSectionReader_File_has_unsupported_multiPolyline);
            }

            return mapGeometries[0].PointCollections.First().Select(p => new Point2D(p));
        }

        private CriticalFileReadException CreateCriticalFileReadException(string message)
        {
            string wrappedMessage = new FileReaderErrorMessageBuilder(filePath).Build(message);
            return new CriticalFileReadException(wrappedMessage);
        }
    }
}