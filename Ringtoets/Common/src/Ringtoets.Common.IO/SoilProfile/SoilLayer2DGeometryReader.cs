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
using System.Collections.ObjectModel;
using System.IO;
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
    /// which contains information about the geometry of a soil layer.
    /// </summary>
    internal class SoilLayer2DGeometryReader
    {
        private const string outerLoopElementName = "OuterLoop";
        private const string innerLoopsElementName = "InnerLoops";
        private const string innerLoopElementName = "GeometryLoop";
        private const string endPointElementName = "EndPoint";
        private const string headPointElementName = "HeadPoint";
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
        /// Reads a new <see cref="SoilLayer2D"/> using the <paramref name="geometry"/> as the source of the 
        /// geometry for a soil layer.
        /// </summary>
        /// <param name="geometry">An <see cref="Array"/> of <see cref="byte"/> which contains the information
        /// of a soil layer in an XML document.</param>
        /// <returns>A new <see cref="SoilLayer2D"/> with information taken from the XML document.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> is <c>null</c>.</exception>
        /// <exception cref="SoilLayerConversionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="geometry"/> is not valid XML.</item>
        /// <item><paramref name="geometry"/> does not pass schema validation.</item>
        /// </list>
        /// </exception>
        /// <seealso cref="Read(XDocument)"/>
        public SoilLayer2D Read(byte[] geometry)
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
        /// Reads a new <see cref="SoilLayer2D"/> using the <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry">An <see cref="XmlDocument"/> which contains the information 
        /// of a soil layer in an XML document.</param>
        /// <returns>A new <see cref="SoilLayer2D"/> with information taken from the XML document.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> is <c>null</c>.</exception>
        /// <exception cref="SoilLayerConversionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="geometry"/> is not valid XML.</item>
        /// <item><paramref name="geometry"/> does not pass schema validation.</item>
        /// </list>
        /// </exception>
        /// <seealso cref="Read(byte[])"/>
        public SoilLayer2D Read(XDocument geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }
            ValidateToSchema(geometry);

            return ParseLayer(geometry);
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
        /// Parses the XML element to create a 2D soil layer.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <exception cref="SoilLayerConversionException">XML for inner or outer geometry loops is invalid.</exception>
        private static SoilLayer2D ParseLayer(XDocument geometry)
        {
            var soilLayer = new SoilLayer2D();

            XElement xmlOuterLoop = geometry.XPathSelectElement($"//{outerLoopElementName}");
            IEnumerable<XElement> xmlInnerLoops = geometry.XPathSelectElements($"//{innerLoopsElementName}//{innerLoopElementName}");

            if (xmlOuterLoop != null)
            {
                soilLayer.OuterLoop = ParseGeometryLoop(xmlOuterLoop);
            }
            foreach (XElement loop in xmlInnerLoops)
            {
                soilLayer.AddInnerLoop(ParseGeometryLoop(loop));
            }
            return soilLayer;
        }

        /// <summary>
        /// Parses the XML element to create a collection of <see cref="Segment2D"/> describing
        /// a geometric loop.
        /// </summary>
        /// <param name="loop">The geometric loop element.</param>
        /// <exception cref="SoilLayerConversionException">XML for any geometry curve is invalid.</exception>
        private static IEnumerable<Segment2D> ParseGeometryLoop(XElement loop)
        {
            var loops = new Collection<Segment2D>();
            IEnumerable<XElement> curves = loop.XPathSelectElements($".//{geometryCurveElementName}");

            foreach (XElement curve in curves)
            {
                loops.Add(ParseGeometryCurve(curve));
            }
            return loops;
        }

        /// <summary>
        /// Parses the XML element to create a <see cref="Segment2D"/>.
        /// </summary>
        /// <param name="curve">The geometry curve element.</param>
        /// <exception cref="SoilLayerConversionException">XML for geometry curve is invalid.</exception>
        private static Segment2D ParseGeometryCurve(XElement curve)
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
        /// Parses the XML element to create a <see cref="Point2D"/>.
        /// </summary>
        /// <param name="point">The 2D point element.</param>
        /// <exception cref="SoilLayerConversionException">Thrown when any of the following occurs:
        /// <list type="bullet">
        /// <item>A coordinate value cannot be parsed.</item>
        /// <item>XML for 2D point is invalid.</item>
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
    }
}