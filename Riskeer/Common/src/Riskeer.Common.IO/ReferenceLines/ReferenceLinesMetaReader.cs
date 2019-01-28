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
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Core.Common.Util.Properties;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Readers;
using Ringtoets.Common.Data.AssessmentSection;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Common.IO.ReferenceLines
{
    /// <summary>
    /// Shape file reader that reads <see cref="ReferenceLineMeta"/> objects based on the line feature in the file.
    /// </summary>
    public static class ReferenceLinesMetaReader
    {
        private const string assessmentsectionIdAttributeKey = "TRAJECT_ID";
        private const string signalingValueAttributeKey = "NORM_SW";
        private const string lowerLimitValueAttributeKey = "NORM_OG";

        /// <summary>
        /// Reads the current features in the shape file into a collection of <see cref="ReferenceLineMeta"/> objects.
        /// </summary>
        /// <param name="shapeFilePath">The file path to the shape file.</param>
        /// <returns>The created collection of <see cref="ReferenceLineMeta"/> objects.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when current feature in the shape file:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that does not exist.</item>
        /// <item>The shape file does not contain the mandatory attributes.</item>
        /// <item>Has an empty <see cref="assessmentsectionIdAttributeKey"/> attribute.</item>
        /// <item>The shape file has non-line geometries in it.</item>
        /// </list></exception>
        public static IEnumerable<ReferenceLineMeta> ReadReferenceLinesMetas(string shapeFilePath)
        {
            ValidateFilePath(shapeFilePath);

            using (PolylineShapeFileReader reader = OpenPolyLineShapeFile(shapeFilePath))
            {
                ValidateExistenceOfRequiredAttributes(reader, shapeFilePath);

                return ReadReferenceLinesMetas(reader);
            }
        }

        private static IEnumerable<ReferenceLineMeta> ReadReferenceLinesMetas(PolylineShapeFileReader reader)
        {
            var referenceLinesMetas = new List<ReferenceLineMeta>();
            ReferenceLineMeta referenceLinesMeta;
            do
            {
                referenceLinesMeta = ReadReferenceLinesMeta(reader);
                if (referenceLinesMeta != null)
                {
                    referenceLinesMetas.Add(referenceLinesMeta);
                }
            } while (referenceLinesMeta != null);

            return referenceLinesMetas;
        }

        private static ReferenceLineMeta ReadReferenceLinesMeta(PolylineShapeFileReader reader)
        {
            MapLineData lineData = ReadMapLineData(reader);
            return lineData == null ? null : CreateReferenceLineMeta(lineData);
        }

        /// <summary>
        /// Validates the <paramref name="shapeFilePath"/>.
        /// </summary>
        /// <param name="shapeFilePath">The file path to the shape file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="shapeFilePath"/> does not exist.</exception>
        private static void ValidateFilePath(string shapeFilePath)
        {
            IOUtils.ValidateFilePath(shapeFilePath);
            if (!File.Exists(shapeFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(Resources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }
        }

        /// <summary>
        /// Validates the file by checking if all mandatory attributes are present in the shape file.
        /// </summary>
        /// <param name="polylineShapeFileReader">The opened shape file reader.</param>
        /// <param name="shapeFilePath">The file path to the shape file.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the shape file lacks one of the mandatory attributes.</exception>
        private static void ValidateExistenceOfRequiredAttributes(PolylineShapeFileReader polylineShapeFileReader, string shapeFilePath)
        {
            IEnumerable<string> missingAttributes = GetMissingAttributes(polylineShapeFileReader);
            if (missingAttributes.Any())
            {
                string message = string.Format(RingtoetsCommonIOResources.ReferenceLinesMetaReader_File_0_lacks_required_Attribute_1_,
                                               shapeFilePath, string.Join("', '", missingAttributes));
                throw new CriticalFileReadException(message);
            }
        }

        private static IEnumerable<string> GetMissingAttributes(PolylineShapeFileReader polylineShapeFileReader)
        {
            if (!polylineShapeFileReader.HasAttribute(assessmentsectionIdAttributeKey))
            {
                yield return assessmentsectionIdAttributeKey;
            }

            if (!polylineShapeFileReader.HasAttribute(signalingValueAttributeKey))
            {
                yield return signalingValueAttributeKey;
            }

            if (!polylineShapeFileReader.HasAttribute(lowerLimitValueAttributeKey))
            {
                yield return lowerLimitValueAttributeKey;
            }
        }

        /// <summary>
        /// Opens a new <see cref="PolylineShapeFileReader"/> to <paramref name="shapeFilePath"/>.
        /// </summary>
        /// <param name="shapeFilePath">Path to the shape file to read.</param>
        /// <returns>A new instance of the <see cref="PolylineShapeFileReader"/> class.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that does not exist.</item>
        /// <item>The shape file has non-line geometries in it.</item>
        /// </list>
        /// </exception>
        private static PolylineShapeFileReader OpenPolyLineShapeFile(string shapeFilePath)
        {
            return new PolylineShapeFileReader(shapeFilePath);
        }

        private static MapLineData ReadMapLineData(PolylineShapeFileReader polylineShapeFileReader)
        {
            return (MapLineData) polylineShapeFileReader.ReadFeature();
        }

        /// <summary>
        /// Creates a new <see cref="ReferenceLineMeta"/> from the <paramref name="lineData"/>.
        /// </summary>
        /// <param name="lineData">The <see cref="MapFeature"/> to create a <see cref="ReferenceLineMeta"/> from.</param>
        /// <returns>The newly created <see cref="ReferenceLineMeta"/>.</returns>
        private static ReferenceLineMeta CreateReferenceLineMeta(MapLineData lineData)
        {
            MapFeature feature = lineData.Features.First();

            string assessmentSectionId = GetAssessmentSectionId(feature);
            int? signalingValue = ParseNormValue(feature.MetaData[signalingValueAttributeKey]);
            int? lowerLimitValue = ParseNormValue(feature.MetaData[lowerLimitValueAttributeKey]);
            IEnumerable<Point2D> geometryPoints = GetSectionGeometry(feature);

            var referenceLineMeta = new ReferenceLineMeta
            {
                AssessmentSectionId = assessmentSectionId
            };
            if (lowerLimitValue != null)
            {
                referenceLineMeta.LowerLimitValue = lowerLimitValue.Value;
            }

            if (signalingValue != null)
            {
                referenceLineMeta.SignalingValue = signalingValue.Value;
            }

            referenceLineMeta.ReferenceLine.SetGeometry(geometryPoints);

            return referenceLineMeta;
        }

        /// <summary>
        /// Gets the geometry from the <paramref name="lineFeature"/>.
        /// </summary>
        /// <param name="lineFeature">The <see cref="MapFeature"/> to get the geometry from.</param>
        /// <returns>A <see cref="Point2D"/> collection that represents the <paramref name="lineFeature"/>'s geometry.</returns>
        private static IEnumerable<Point2D> GetSectionGeometry(MapFeature lineFeature)
        {
            MapGeometry[] mapGeometries = lineFeature.MapGeometries.ToArray();
            if (mapGeometries.Length != 1)
            {
                return Enumerable.Empty<Point2D>();
            }

            return mapGeometries[0].PointCollections.First().Select(p => new Point2D(p)).ToArray();
        }

        private static string GetAssessmentSectionId(MapFeature lineFeature)
        {
            return Convert.ToString(lineFeature.MetaData[assessmentsectionIdAttributeKey]);
        }

        private static int? ParseNormValue(object readObject)
        {
            try
            {
                return readObject == null ? (int?) null : Convert.ToInt32(readObject);
            }
            catch (Exception exception)
            {
                if (exception is InvalidCastException || exception is FormatException || exception is OverflowException)
                {
                    return null;
                }

                throw;
            }
        }
    }
}