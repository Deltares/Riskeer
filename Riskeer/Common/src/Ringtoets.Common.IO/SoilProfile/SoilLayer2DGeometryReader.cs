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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using Core.Common.Base.Geometry;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class is responsible for reading an array of bytes and interpret this as an XML document,
    /// which contains information about the geometry of a 2D soil layer.
    /// </summary>
    internal class SoilLayer2DGeometryReader
    {
        private const string outerLoopElementName = "OuterLoop";
        private const string innerLoopsElementName = "InnerLoops";
        private const string innerLoopElementName = "GeometryLoop";
        private const string headPointElementName = "HeadPoint";
        private const string endPointElementName = "EndPoint";
        private const string geometryCurveElementName = "GeometryCurve";
        private const string xElementName = "X";
        private const string zElementName = "Z";

        private readonly XmlSchemaSet schema;

        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2DGeometryReader"/>.
        /// </summary>
        public SoilLayer2DGeometryReader()
        {
            schema = LoadXmlSchema();
        }

        /// <summary>
        /// Reads a <see cref="SoilLayer2DGeometry"/> from <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry">An <see cref="Array"/> of <see cref="byte"/> which contains the information
        /// of a soil layer 2D geometry as an XML document.</param>
        /// <returns>A <see cref="SoilLayer2DGeometry"/> with information taken from the XML document.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> is <c>null</c>.</exception>
        /// <exception cref="SoilLayerConversionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="geometry"/> is not valid XML;</item>
        /// <item><paramref name="geometry"/> does not pass the schema validation.</item>
        /// </list>
        /// </exception>
        /// <seealso cref="Read(XDocument)"/>
        public SoilLayer2DGeometry Read(byte[] geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry), Resources.SoilLayer2DGeometryReader_Geometry_is_null);
            }

            try
            {
                using (var stream = new MemoryStream(geometry))
                {
                    return Read(XDocument.Load(stream));
                }
            }
            catch (XmlException e)
            {
                throw new SoilLayerConversionException(Resources.SoilLayer2DGeometryReader_Geometry_contains_no_valid_xml, e);
            }
        }

        /// <summary>
        /// Reads a <see cref="SoilLayer2DGeometry"/> from <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry">An <see cref="XmlDocument"/> which contains the information
        /// of a soil layer 2D geometry.</param>
        /// <returns>A <see cref="SoilLayer2D"/> with information taken from the XML document.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> is <c>null</c>.</exception>
        /// <exception cref="SoilLayerConversionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="geometry"/> is not valid XML;</item>
        /// <item><paramref name="geometry"/> does not pass the schema validation.</item>
        /// </list>
        /// </exception>
        /// <seealso cref="Read(byte[])"/>
        public SoilLayer2DGeometry Read(XDocument geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            ValidateToSchema(geometry);

            return ParseGeometry(geometry);
        }

        /// <summary>
        /// Validates the <paramref name="document"/> to the <see cref="schema"/>.
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/> to validate.</param>
        /// <exception cref="SoilLayerConversionException">Thrown when the validation failed.</exception>
        private void ValidateToSchema(XDocument document)
        {
            try
            {
                document.Validate(schema, null);
            }
            catch (XmlSchemaValidationException e)
            {
                throw new SoilLayerConversionException(Resources.SoilLayer2DGeometryReader_Geometry_contains_no_valid_xml, e);
            }
        }

        private static XmlSchemaSet LoadXmlSchema()
        {
            var xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add(XmlSchema.Read(new StringReader(Resources.XmlGeometrySchema), null));
            return xmlSchemaSet;
        }

        /// <summary>
        /// Parses <paramref name="geometry"/> in order to create a 2D soil layer geometry.
        /// </summary>
        /// <param name="geometry">An <see cref="XmlDocument"/> which contains the information
        /// of a 2D soil layer geometry.</param>
        /// <returns>A <see cref="SoilLayer2DGeometry"/> with data set for the outer loop and the inner loops.</returns>
        /// <exception cref="SoilLayerConversionException">Thrown when the XML for the outer
        /// or inner geometry loops is invalid.</exception>
        private static SoilLayer2DGeometry ParseGeometry(XDocument geometry)
        {
            XElement xmlOuterLoop = geometry.XPathSelectElement($"//{outerLoopElementName}");
            IEnumerable<XElement> xmlInnerLoops = geometry.XPathSelectElements($"//{innerLoopsElementName}//{innerLoopElementName}");

            return new SoilLayer2DGeometry(ParseLoop(xmlOuterLoop), xmlInnerLoops.Select(ParseLoop).ToArray());
        }

        /// <summary>
        /// Parses <paramref name="loop"/> in order to create a <see cref="SoilLayer2DLoop"/>.
        /// </summary>
        /// <param name="loop">An <see cref="XElement"/> which contains the information
        /// of a 2D soil layer loop.</param>
        /// <returns>A <see cref="SoilLayer2DLoop"/>.</returns>
        /// <exception cref="SoilLayerConversionException">Thrown when the XML:
        /// <list type="bullet">
        /// <item>for any geometry curve is invalid;</item>
        /// <item>only contains one curve;</item>
        /// <item>contains disconnected curves.</item>
        /// </list>
        /// </exception>
        private static SoilLayer2DLoop ParseLoop(XElement loop)
        {
            IEnumerable<XElement> curves = loop.XPathSelectElements($".//{geometryCurveElementName}");

            Segment2D[] unsortedSegments = curves.Select(ParseCurve).ToArray();

            if (unsortedSegments.Length == 1)
            {
                throw new SoilLayerConversionException(Resources.Loop_contains_disconnected_segments);
            }

            return new SoilLayer2DLoop(GetSortedSegments(unsortedSegments).ToArray());
        }

        /// <summary>
        /// Parses <paramref name="curve"/> in order to create a <see cref="Segment2D"/>.
        /// </summary>
        /// <param name="curve">An <see cref="XElement"/> which contains the information
        /// of a 2D segment.</param>
        /// <returns>A <see cref="Segment2D"/>.</returns>
        /// <exception cref="SoilLayerConversionException">Thrown when the XML for the curve
        /// is invalid.</exception>
        private static Segment2D ParseCurve(XElement curve)
        {
            XElement headDefinition = curve.Element(headPointElementName);
            XElement endDefinition = curve.Element(endPointElementName);
            if (headDefinition == null || endDefinition == null)
            {
                throw new SoilLayerConversionException(Resources.SoilLayer2DGeometryReader_Geometry_contains_no_valid_xml);
            }

            return new Segment2D(ParsePoint(headDefinition), ParsePoint(endDefinition));
        }

        /// <summary>
        /// Parses <paramref name="point"/> in order to create a <see cref="Point2D"/>.
        /// </summary>
        /// <param name="point">An <see cref="XElement"/> which contains the information
        /// of a 2D point.</param>
        /// <returns>A <see cref="Point2D"/>.</returns>
        /// <exception cref="SoilLayerConversionException">Thrown when any of the following occurs:
        /// <list type="bullet">
        /// <item>A coordinate value cannot be parsed.</item>
        /// <item>The XML for the point is invalid.</item>
        /// </list></exception>
        private static Point2D ParsePoint(XElement point)
        {
            XElement xElement = point.Element(xElementName);
            XElement yElement = point.Element(zElementName);
            if (xElement == null || yElement == null)
            {
                throw new SoilLayerConversionException(Resources.SoilLayer2DGeometryReader_Geometry_contains_no_valid_xml);
            }

            try
            {
                double x = XmlConvert.ToDouble(xElement.Value);
                double y = XmlConvert.ToDouble(yElement.Value);
                return new Point2D(x, y);
            }
            catch (SystemException e) when (e is ArgumentNullException
                                            || e is FormatException
                                            || e is OverflowException)
            {
                throw new SoilLayerConversionException(Resources.SoilLayer2DGeometryReader_Could_not_parse_point_location, e);
            }
        }

        /// <summary>
        /// Creates sorted segments from <paramref name="unsortedSegments"/>.
        /// </summary>
        /// <param name="unsortedSegments">The unsorted segments to get the sorted segments from.</param>
        /// <returns>An array of sorted segments.</returns>
        /// <exception cref="SoilLayerConversionException">Thrown when 
        /// <param name="unsortedSegments"/> contains disconnected segments.</exception>
        private static IEnumerable<Segment2D> GetSortedSegments(Segment2D[] unsortedSegments)
        {
            Point2D[] sortedPoints = GetSortedPoints(unsortedSegments);
            int sortedPointsLength = sortedPoints.Length;

            for (var i = 0; i < sortedPointsLength; i++)
            {
                yield return new Segment2D(sortedPoints[i],
                                           i == sortedPointsLength - 1
                                               ? sortedPoints[0]
                                               : sortedPoints[i + 1]);
            }
        }

        /// <summary>
        /// Gets sorted points from <paramref name="segments"/>.
        /// </summary>
        /// <param name="segments">The segments to get the sorted points from.</param>
        /// <returns>An array of sorted points.</returns>
        /// <exception cref="SoilLayerConversionException">Thrown when 
        /// <param name="segments"/> contains disconnected segments.</exception>
        private static Point2D[] GetSortedPoints(Segment2D[] segments)
        {
            var sortedPoints = new List<Point2D>();

            for (var index = 0; index < segments.Length; ++index)
            {
                if (index == 0)
                {
                    sortedPoints.Add(segments[index].FirstPoint);
                    sortedPoints.Add(segments[index].SecondPoint);
                }
                else if (segments[index].FirstPoint.Equals(sortedPoints[sortedPoints.Count - 1]))
                {
                    sortedPoints.Add(segments[index].SecondPoint);
                }
                else if (segments[index].SecondPoint.Equals(sortedPoints[sortedPoints.Count - 1]))
                {
                    sortedPoints.Add(segments[index].FirstPoint);
                }
                else
                {
                    if (sortedPoints.Count == 2)
                    {
                        sortedPoints.Reverse();
                    }

                    if (segments[index].FirstPoint.Equals(sortedPoints[sortedPoints.Count - 1]))
                    {
                        sortedPoints.Add(segments[index].SecondPoint);
                    }
                    else if (segments[index].SecondPoint.Equals(sortedPoints[sortedPoints.Count - 1]))
                    {
                        sortedPoints.Add(segments[index].FirstPoint);
                    }
                    else
                    {
                        throw new SoilLayerConversionException(Resources.Loop_contains_disconnected_segments);
                    }
                }
            }

            if (sortedPoints.Count <= 0 || !sortedPoints[0].Equals(sortedPoints[sortedPoints.Count - 1]))
            {
                return sortedPoints.ToArray();
            }

            sortedPoints.RemoveAt(sortedPoints.Count - 1);

            return sortedPoints.ToArray();
        }
    }
}