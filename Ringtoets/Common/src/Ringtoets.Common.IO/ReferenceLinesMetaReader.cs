﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Core.Common.Utils.Properties;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.IO.Readers;
using Ringtoets.Common.Data.AssessmentSection;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Shape file reader that reads <see cref="ReferenceLineMeta"/> objects based on the line feature in the file.
    /// </summary>
    public class ReferenceLinesMetaReader : IDisposable
    {
        private const string referenceLineIdAttributeKey = "TRAJECT_ID";
        private const string signalingValueAttributeKey = "NORM_SW";
        private const string lowerLimitValueAttributeKey = "NORM_OG";
        private readonly PolylineShapeFileReader polylineShapeFileReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineShapeFileReader"/> class and validates the file.
        /// </summary>
        /// <param name="shapeFilePath">The file path to the shape file.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that does not exist.</item>
        /// <item>The shape file does not contain the required attributes.</item>
        /// </list></exception>
        public ReferenceLinesMetaReader(string shapeFilePath)
        {
            FileUtils.ValidateFilePath(shapeFilePath);
            if (!File.Exists(shapeFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(Resources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            polylineShapeFileReader = OpenPolyLineShapeFile(shapeFilePath);

            ValidateExistenceOfRequiredAttributes();
        }

        /// <summary>
        /// Gets the number of reference lines in the shape file.
        /// </summary>
        public int GetReferenceLinesCount()
        {
            return polylineShapeFileReader.GetNumberOfLines();
        }

        /// <summary>
        /// Reads the current feature in the shape file into a <see cref="ReferenceLineMeta"/>.
        /// </summary>
        /// <returns>The created <see cref="ReferenceLineMeta"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when current feature in the shape file:
        /// <list type="bullet">
        /// <item>Has an empty track id.</item>
        /// <item>Does not contain poly lines.</item>
        /// <item>Contains multiple poly lines.</item>
        /// </list></exception>
        public ReferenceLineMeta ReadReferenceLinesMeta()
        {
            var lineData = ReadMapLineData();
            return lineData == null ? null : CreateReferenceLineMeta(lineData);
        }

        public void Dispose()
        {
            polylineShapeFileReader.Dispose();
        }

        private void ValidateExistenceOfRequiredAttributes()
        {
            var hasAssessmentSectionIdAttribute = polylineShapeFileReader.HasAttribute(referenceLineIdAttributeKey);
            var hasSignalingValueAttribute = polylineShapeFileReader.HasAttribute(signalingValueAttributeKey);
            var hasLowerLimitValueAttribute = polylineShapeFileReader.HasAttribute(lowerLimitValueAttributeKey);

            if (hasAssessmentSectionIdAttribute && hasSignalingValueAttribute && hasLowerLimitValueAttribute)
            {
                return;
            }

            var message = "";
            if (hasAssessmentSectionIdAttribute && hasSignalingValueAttribute)
            {
                // No low limit
                message = string.Format(RingtoetsCommonIOResources.ReferenceLinesMetaReader_File_lacks_required_Attribute_0_,
                                        lowerLimitValueAttributeKey);
                throw new CriticalFileReadException(message);
            }

            if (hasAssessmentSectionIdAttribute && hasLowerLimitValueAttribute)
            {
                // No signaling value
                message = string.Format(RingtoetsCommonIOResources.ReferenceLinesMetaReader_File_lacks_required_Attribute_0_,
                                        signalingValueAttributeKey);
                throw new CriticalFileReadException(message);
            }

            if (hasSignalingValueAttribute && hasLowerLimitValueAttribute)
            {
                // No Assessment Section Id
                message = string.Format(RingtoetsCommonIOResources.ReferenceLinesMetaReader_File_lacks_required_Attribute_0_,
                                        referenceLineIdAttributeKey);
                throw new CriticalFileReadException(message);
            }

            // Multiple attributes not found
            var missingAttributes = new List<string>();
            if (!hasAssessmentSectionIdAttribute)
            {
                missingAttributes.Add(referenceLineIdAttributeKey);
            }
            if (!hasSignalingValueAttribute)
            {
                missingAttributes.Add(signalingValueAttributeKey);
            }
            if (!hasLowerLimitValueAttribute)
            {
                missingAttributes.Add(lowerLimitValueAttributeKey);
            }

            message = string.Format(RingtoetsCommonIOResources.ReferenceLinesMetaReader_File_lacks_required_Attributes_0_,
                                    string.Join("', '", missingAttributes));

            throw new CriticalFileReadException(message);
        }

        private static PolylineShapeFileReader OpenPolyLineShapeFile(string shapeFilePath)
        {
            return new PolylineShapeFileReader(shapeFilePath);
        }

        /// <summary>
        /// Reads a new <see cref="MapLineData"/> from the file.
        /// </summary>
        /// <returns></returns>
        private MapLineData ReadMapLineData()
        {
            return polylineShapeFileReader.ReadLine() as MapLineData;
        }

        private static ReferenceLineMeta CreateReferenceLineMeta(MapLineData lineData)
        {
            var features = lineData.Features.ToArray();

            var feature = features[0];

            var referenceLineId = GetReferenceLineId(feature);
            var signalingValue = GetSignalingValueAttributeKey(feature);
            var lowerLimitValue = GetLowerLimitValueAttribute(feature);
            IEnumerable<Point2D> geometryPoints = GetSectionGeometry(feature);

            var referenceLineMeta = new ReferenceLineMeta
            {
                ReferenceLineId = referenceLineId
            };
            if (lowerLimitValue != null)
            {
                referenceLineMeta.LowerLimitValue = lowerLimitValue.Value;
            }
            if (signalingValue != null)
            {
                referenceLineMeta.SignalingValue = signalingValue.Value;
            }
            referenceLineMeta.SetGeometry(geometryPoints);

            return referenceLineMeta;
        }

        private static IEnumerable<Point2D> GetSectionGeometry(MapFeature lineFeature)
        {
            var mapGeometries = lineFeature.MapGeometries.ToArray();
            if (mapGeometries.Length > 1)
            {
                throw new CriticalFileReadException(RingtoetsCommonIOResources.ReferenceLineReader_File_contains_unsupported_multi_polyline);
            }

            return mapGeometries[0].PointCollections.First().Select(p => new Point2D(p.X, p.Y));
        }

        private static string GetReferenceLineId(MapFeature lineFeature)
        {
            var referenceLineId = Convert.ToString(lineFeature.MetaData[referenceLineIdAttributeKey]);
            if (String.IsNullOrEmpty(referenceLineId))
            {
                throw new CriticalFileReadException(RingtoetsCommonIOResources.ReferenceLinesMetaReader_TrajectId_is_empty);
            }
            return referenceLineId;
        }

        private static int? GetSignalingValueAttributeKey(MapFeature lineFeature)
        {
            return lineFeature.MetaData[signalingValueAttributeKey] as int?;
        }

        private static int? GetLowerLimitValueAttribute(MapFeature lineFeature)
        {
            return lineFeature.MetaData[lowerLimitValueAttributeKey] as int?;
        }
    }
}