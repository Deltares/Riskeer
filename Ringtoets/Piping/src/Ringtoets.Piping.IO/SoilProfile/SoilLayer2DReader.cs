using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class is responsible for reading an array of bytes and interpret this as a XML document, which contains information about
    /// the geometry of a <see cref="PipingSoilLayer"/>.
    /// </summary>
    internal class SoilLayer2DReader
    {
        private const string outerLoopElementName = "OuterLoop";
        private const string innerLoopElementName = "InnerLoop";
        private const string endPointElementName = "EndPoint";
        private const string headPointElementName = "HeadPoint";
        private const string geometryCurveElementName = "GeometryCurve";
        private const string xElementName = "X";
        private const string zElementName = "Z";

        private readonly XmlSchemaSet schema;

        /// <summary>
        /// Constructs an instance of <see cref="SoilLayer2DReader"/>.
        /// </summary>
        /// <exception cref="SoilLayer2DConversionException">Thrown when the XML-schema 
        /// could not be loaded.</exception>
        internal SoilLayer2DReader()
        {
            schema = LoadXmlSchema();
        }

        /// <summary>
        /// Reads a new <see cref="SoilLayer2D"/> using the <paramref name="geometry"/> as the source of the 
        /// geometry for a <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="geometry">An <see cref="Array"/> of <see cref="byte"/> which contains the information
        /// of a <see cref="PipingSoilLayer"/> in an XML document.</param>
        /// <returns>A new <see cref="SoilLayer2D"/> with information taken from the XML document.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> is null.</exception>
        /// <exception cref="SoilLayer2DConversionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="geometry"/> is not valid XML.</item>
        /// <item><paramref name="geometry"/> does not validate to the schema.</item>
        /// </list>
        /// </exception>
        internal SoilLayer2D Read(byte[] geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException("geometry", Resources.SoilLayer2DReader_Geometry_is_null);
            }
            try
            {
                return Read(XDocument.Load(new MemoryStream(geometry)));
            }
            catch (XmlException e)
            {
                throw new SoilLayer2DConversionException(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml, e);
            }
        }

        /// <summary>
        /// Reads a new <see cref="SoilLayer2D"/> using the <paramref name="geometry"/> as the source of the 
        /// geometry for a <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="geometry">An <see cref="XmlDocument"/> which contains the information of a <see cref="PipingSoilLayer"/>
        /// in an XML document.</param>
        /// <returns>A new <see cref="SoilLayer2D"/> with information taken from the XML document.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> is null.</exception>
        /// <exception cref="SoilLayer2DConversionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="geometry"/> is not valid XML.</item>
        /// <item><paramref name="geometry"/> does not validate to the schema.</item>
        /// </list>
        /// </exception>
        internal SoilLayer2D Read(XDocument geometry)
        {
            ValidateToSchema(geometry);

            return ParseLayer(geometry);
        }

        private SoilLayer2D ParseLayer(XDocument geometry)
        {
            var pipingSoilLayer = new SoilLayer2D();

            var xmlOuterLoop = geometry.XPathSelectElement(string.Format("//{0}", outerLoopElementName));
            var xmlInnerLoops = geometry.XPathSelectElements(string.Format("//{0}", innerLoopElementName));

            if (xmlOuterLoop != null)
            {
                pipingSoilLayer.OuterLoop = ParseGeometryLoop(xmlOuterLoop);
            }
            foreach (XElement loop in xmlInnerLoops)
            {
                pipingSoilLayer.AddInnerLoop(ParseGeometryLoop(loop));
            }
            return pipingSoilLayer;
        }

        /// <summary>
        /// Adds the validation schema for the geometry to the <paramref name="document"/>.
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/> to add the validation schema to.</param>
        /// <exception cref="SoilLayer2DConversionException">Thrown when:
        /// <list type="bullet">
        /// <item>The validation schema could not be correctly loaded.</item>
        /// <item>The validation failed.</item>
        /// </list>
        /// </exception>
        private void ValidateToSchema(XDocument document)
        {
            try
            {
                document.Validate(schema, null);
            }
            catch (InvalidOperationException e)
            {
                throw new SoilLayer2DConversionException(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml, e);
            }
            catch (XmlSchemaValidationException e)
            {
                throw new SoilLayer2DConversionException(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml, e);
            }
        }

        private XmlSchemaSet LoadXmlSchema()
        {
            var schemaFile = GetType().Assembly.GetManifestResourceStream("Ringtoets.Piping.IO.SoilProfile.XmlGeometrySchema.xsd");
            if (schemaFile == null)
            {
                throw new SoilLayer2DConversionException(Resources.SoilLayer2DReader_Schema_file_could_not_be_loaded);
            }
            try
            {
                var xmlSchema = new XmlSchemaSet();
                xmlSchema.Add(XmlSchema.Read(schemaFile, null));
                return xmlSchema;
            }
            catch (XmlSchemaException e)
            {
                throw new SoilLayer2DConversionException(Resources.SoilLayer2DReader_Schema_file_could_not_be_loaded, e);
            }
        }

        private IEnumerable<Segment2D> ParseGeometryLoop(XElement loop)
        {
            var loops = new Collection<Segment2D>();
            var curves = loop.XPathSelectElements(string.Format("//{0}", geometryCurveElementName));

            foreach (XElement curve in curves)
            {
                loops.Add(ParseGeometryCurve(curve));
            }
            return loops;
        }

        private Segment2D ParseGeometryCurve(XElement curve)
        {
            var headDefinition = curve.Element(headPointElementName);
            var endDefinition = curve.Element(endPointElementName);
            if (headDefinition != null && endDefinition != null)
            {
                return new Segment2D(
                    ParsePoint(headDefinition),
                    ParsePoint(endDefinition)
                    );
            }
            throw new SoilLayer2DConversionException(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml);
        }

        private Point2D ParsePoint(XElement point)
        {
            var x = point.Element(xElementName);
            var y = point.Element(zElementName);
            if (x != null && y != null)
            {
                return new Point2D
                {
                    X = double.Parse(x.Value, CultureInfo.InvariantCulture),
                    Y = double.Parse(y.Value, CultureInfo.InvariantCulture)
                };
            }
            throw new SoilLayer2DConversionException(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml);
        }
    }
}