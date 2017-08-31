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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayer2DGeometryReaderTest
    {
        [Test]
        public void Constructor_ReturnsNewInstance()
        {
            // Call
            var result = new SoilLayer2DGeometryReader();

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void Read_NullByteArray_ThrowsArgumentNullException()
        {
            // Setup
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read((byte[]) null);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "De geometrie is leeg.");
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Read_NullXmlDocument_ThrowsArgumentNullException()
        {
            // Setup
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read((XDocument) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("geometry", paramName);
        }

        [Test]
        public void Read_XmlDocumentWithoutSaneContent_ThrowsSoilLayerConversionException()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument("<doc/>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("Het XML-document dat de geometrie beschrijft voor de laag is niet geldig.", exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWithNoInnerLoops_ThrowsSoilLayerConversionException()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument("<GeometrySurface><OuterLoop/></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("Het XML-document dat de geometrie beschrijft voor de laag is niet geldig.", exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWithNoOuterLoop_ThrowsSoilLayerConversionException()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument("<GeometrySurface><InnerLoops><InnerLoop/></InnerLoops></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("Het XML-document dat de geometrie beschrijft voor de laag is niet geldig.", exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWithEmptyInnerLoopAndOuterLoop_ReturnsLayerWithEmptyInnerLoopAndEmptyOuterLoop()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument("<GeometrySurface><OuterLoop/><InnerLoops><GeometryLoop/></InnerLoops></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2D result = reader.Read(xmlDoc);

            // Assert
            Assert.NotNull(result);
            CollectionAssert.IsEmpty(result.OuterLoop);
            Assert.AreEqual(1, result.InnerLoops.Count());
            CollectionAssert.IsEmpty(result.InnerLoops.ElementAt(0));
        }

        [Test]
        [TestCase("x")]
        [TestCase("")]
        public void Read_XmlDocumentWithInvalidPointCoordinate_ThrowsSoilLayerConversionException(string incorrectNumber)
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument(
                "<GeometrySurface><OuterLoop><CurveList><GeometryCurve>" +
                $"<HeadPoint><X>{incorrectNumber}</X><Z>1.2</Z></HeadPoint>" +
                "<EndPoint><X>1.2</X><Z>1.2</Z></EndPoint>" +
                "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("Coördinaat van een punt bevat ongeldige waarde.", exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWithOverflowingPointCoordinate_ThrowsSoilLayerConversionException()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument(
                "<GeometrySurface><OuterLoop><CurveList><GeometryCurve>" +
                $"<HeadPoint><X>{double.MaxValue}</X><Z>1.2</Z></HeadPoint>" +
                "<EndPoint><X>1.2</X><Z>1.2</Z></EndPoint>" +
                "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("Coördinaat van een punt bevat ongeldige waarde.", exception.Message);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Read_NLXmlDocumentPointInOuterLoop_ReturnsLayerWithOuterLoopWithPoint()
        {
            Read_XmlDocumentPointInOuterLoop_ReturnsLayerWithOuterLoopWithPoint();
        }

        [Test]
        [SetCulture("en-US")]
        public void Read_ENXmlDocumentPointInOuterLoop_ReturnsLayerWithOuterLoopWithPoint()
        {
            Read_XmlDocumentPointInOuterLoop_ReturnsLayerWithOuterLoopWithPoint();
        }

        [Test]
        public void Read_XmlDocumentPointsInInnerLoop_ReturnsLayerWithInnerLoopWithSegment()
        {
            // Setup
            var random = new Random(22);
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            double x1 = random.NextDouble();
            double x2 = random.NextDouble();
            double y1 = random.NextDouble();
            double y2 = random.NextDouble();

            string x1String = XmlConvert.ToString(x1);
            string x2String = XmlConvert.ToString(x2);
            string y1String = XmlConvert.ToString(y1);
            string y2String = XmlConvert.ToString(y2);
            XDocument xmlDoc = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop/><InnerLoops><GeometryLoop><CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                              "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                              "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList></GeometryLoop></InnerLoops></GeometrySurface>",
                              x1String, y1String, x2String, y2String));
            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2D result = reader.Read(xmlDoc);

            // Assert
            Assert.NotNull(result);

            var expectedSegment = new Segment2D(new Point2D(x1, y1), new Point2D(x2, y2));
            var expectedCollection = new[]
            {
                new List<Segment2D>
                {
                    expectedSegment,
                    expectedSegment
                }
            };
            CollectionAssert.AreEqual(expectedCollection, result.InnerLoops);
        }

        [Test]
        public void Read_XmlDocumentSinglePointOuterLoopGeometryCurve_ThrowsSoilLayerConversionException()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument(
                "<GeometrySurface><OuterLoop><CurveList><GeometryCurve>" +
                "<EndPoint><X>1</X><Y>0.1</Y><Z>1.1</Z></EndPoint>" +
                "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            Assert.Throws<SoilLayerConversionException>(test);
        }

        [Test]
        public void Read_XmlDocumentSinglePointInnerLoopGeometryCurve_ThrowsSoilLayerConversionException()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument(
                "<GeometrySurface><InnerLoops><InnerLoop><GeometryCurve>" +
                "<HeadPoint><X>0</X><Y>0.1</Y><Z>1.1</Z></HeadPoint>" +
                "</GeometryCurve></InnerLoop></InnerLoops></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            Assert.Throws<SoilLayerConversionException>(test);
        }

        [Test]
        public void Read_XmlDocumentEqualSegments_ReturnsTwoEqualSegments()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument(
                "<GeometrySurface><OuterLoop><CurveList>" +
                "<GeometryCurve>" +
                "<HeadPoint><X>0</X><Y>0</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0</Y><Z>1.1</Z></EndPoint>" +
                "</GeometryCurve>" +
                "<GeometryCurve>" +
                "<HeadPoint><X>0</X><Y>0</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0</Y><Z>1.1</Z></EndPoint>" +
                "</GeometryCurve>" +
                "</CurveList></OuterLoop><InnerLoops/></GeometrySurface>");

            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2D result = reader.Read(xmlDoc);

            // Assert
            Assert.AreEqual(2, result.OuterLoop.Count());
            Assert.AreEqual(result.OuterLoop.ElementAt(0), result.OuterLoop.ElementAt(1));
        }

        private static void Read_XmlDocumentPointInOuterLoop_ReturnsLayerWithOuterLoopWithPoint()
        {
            // Setup
            var random = new Random(22);
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            double x1 = random.NextDouble();
            double x2 = random.NextDouble();
            double y1 = random.NextDouble();
            double y2 = random.NextDouble();

            string x1String = XmlConvert.ToString(x1);
            string x2String = XmlConvert.ToString(x2);
            string y1String = XmlConvert.ToString(y1);
            string y2String = XmlConvert.ToString(y2);
            XDocument bytes = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop><CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                              "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                              "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>",
                              x1String, y1String, x2String, y2String));

            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2D result = reader.Read(bytes);

            // Assert
            Assert.NotNull(result);
            var expectedSegment = new Segment2D(new Point2D(x1, y1), new Point2D(x2, y2));
            CollectionAssert.AreEqual(new List<Segment2D>
            {
                expectedSegment,
                expectedSegment
            }, result.OuterLoop);
        }

        /// <summary>
        /// Takes a <paramref name="xmlString"/> which describes an XML document and returns 
        /// an <see cref="XDocument"/> from this.
        /// </summary>
        /// <param name="xmlString">The <see cref="string"/> to convert to an <see cref="XDocument"/>.</param>
        /// <returns>The <see cref="XDocument"/> constructed from <paramref name="xmlString"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="xmlString"/> is <c>null</c>.</exception>
        /// <exception cref="XmlException">Thrown when <paramref name="xmlString"/> does not describe
        /// a valid XML document.</exception>
        private static XDocument GetXmlDocument(string xmlString)
        {
            return XDocument.Load(new MemoryStream(GetByteArray(xmlString)));
        }

        /// <summary>
        /// Takes a <paramref name="str"/> and returns an <see cref="Array"/> of <see cref="byte"/>
        /// which contains the same information as the original <paramref name="str"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to convert to an <see cref="Array"/> of 
        /// <see cref="byte"/>.</param>
        /// <returns>The <see cref="Array"/> of <see cref="byte"/> constructed from 
        /// <paramref name="str"/>.</returns>
        private static byte[] GetByteArray(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}