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

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayer2DGeometryReaderTest
    {
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
        public void Read_XmlDocumentWithoutInnerLoopsTag_ThrowsSoilLayerConversionException()
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
        public void Read_XmlDocumentWithoutOuterLoopTag_ThrowsSoilLayerConversionException()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument("<GeometrySurface><InnerLoops/></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("Het XML-document dat de geometrie beschrijft voor de laag is niet geldig.", exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWithEmptyInnerLoopsAndEmptyOuterLoop_ReturnsLayerGeometryWithoutInnerLoopsAndWithEmptyOuterLoop()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument("<GeometrySurface><OuterLoop/><InnerLoops/></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2DGeometry geometry = reader.Read(xmlDoc);

            // Assert
            Assert.NotNull(geometry);
            CollectionAssert.IsEmpty(geometry.OuterLoop.Segments);
            Assert.AreEqual(0, geometry.InnerLoops.Count());
        }

        [Test]
        public void Read_XmlDocumentWithEmptyInnerLoopAndEmptyOuterLoop_ReturnsLayerGeometryWithEmptyInnerLoopAndEmptyOuterLoop()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument("<GeometrySurface><OuterLoop/><InnerLoops><GeometryLoop/></InnerLoops></GeometrySurface>");
            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2DGeometry geometry = reader.Read(xmlDoc);

            // Assert
            Assert.NotNull(geometry);
            CollectionAssert.IsEmpty(geometry.OuterLoop.Segments);
            Assert.AreEqual(1, geometry.InnerLoops.Count());
            CollectionAssert.IsEmpty(geometry.InnerLoops.ElementAt(0).Segments);
        }

        [Test]
        public void Read_XmlDocumentNoHeadPoint_ThrowsSoilLayerConversionException()
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
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("Het XML-document dat de geometrie beschrijft voor de laag is niet geldig.", exception.Message);
        }

        [Test]
        public void Read_XmlDocumentNoEndPoint_ThrowsSoilLayerConversionException()
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
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("Het XML-document dat de geometrie beschrijft voor de laag is niet geldig.", exception.Message);
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
        public void Read_XmlDocumentWithOneSegment_ThrowsSoilLayerConversionException()
        {
            // Setup
            XDocument xmlDoc = GetXmlDocument(
                "<GeometrySurface><OuterLoop><CurveList>" +
                "<GeometryCurve>" +
                "<HeadPoint><X>0</X><Y>0</Y><Z>1.25</Z></HeadPoint><EndPoint><X>111</X><Y>0</Y><Z>1.25</Z></EndPoint>" +
                "</GeometryCurve>" +
                "</CurveList></OuterLoop><InnerLoops/></GeometrySurface>");

            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Read_NLXmlDocumentWithOuterLoop_ReturnsLayerGeometryWithExpectedOuterLoop()
        {
            Read_XmlDocumentWithOuterLoop_ReturnsLayerGeometryWithExpectedOuterLoop();
        }

        [Test]
        [SetCulture("en-US")]
        public void Read_ENXmlDocumentWithOuterLoop_ReturnsLayerGeometryWithExpectedOuterLoop()
        {
            Read_XmlDocumentWithOuterLoop_ReturnsLayerGeometryWithExpectedOuterLoop();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Read_NLXmlDocumentWithInnerLoop_ReturnsLayerGeometryWithExpectedInnerLoop()
        {
            Read_XmlDocumentWithInnerLoop_ReturnsLayerGeometryWithExpectedInnerLoop();
        }

        [Test]
        [SetCulture("en-US")]
        public void Read_ENXmlDocumentWithInnerLoop_ReturnsLayerGeometryWithExpectedInnerLoop()
        {
            Read_XmlDocumentWithInnerLoop_ReturnsLayerGeometryWithExpectedInnerLoop();
        }

        [Test]
        public static void Read_XmlDocumentWithTwoEqualSegmentsInOuterLoop_ReturnsLayerGeometryWithExpectedOuterLoop()
        {
            // Setup
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            const double point1 = 1.1;
            const double point2 = 2.2;

            string pointString1 = XmlConvert.ToString(point1);
            string pointString2 = XmlConvert.ToString(point2);
            XDocument bytes = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop><CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>",
                              pointString1, pointString2));

            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2DGeometry geometry = reader.Read(bytes);

            // Assert
            Assert.NotNull(geometry);
            Assert.AreEqual(new SoilLayer2DLoop(new[]
            {
                new Segment2D(new Point2D(point1, point1), new Point2D(point2, point2)),
                new Segment2D(new Point2D(point2, point2), new Point2D(point1, point1))
            }), geometry.OuterLoop);
        }

        [Test]
        public static void Read_XmlDocumentWithTwoEqualSegmentsInInnerLoop_ReturnsLayerGeometryWithExpectedInnerLoop()
        {
            // Setup
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            const double point1 = 1.1;
            const double point2 = 2.2;

            string pointString1 = XmlConvert.ToString(point1);
            string pointString2 = XmlConvert.ToString(point2);
            XDocument bytes = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop/><InnerLoops><GeometryLoop>" +
                              "<CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList></GeometryLoop></InnerLoops></GeometrySurface>",
                              pointString1, pointString2));

            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2DGeometry geometry = reader.Read(bytes);

            // Assert
            Assert.NotNull(geometry);
            Assert.AreEqual(new SoilLayer2DLoop(new[]
            {
                new Segment2D(new Point2D(point1, point1), new Point2D(point2, point2)),
                new Segment2D(new Point2D(point2, point2), new Point2D(point1, point1))
            }), geometry.InnerLoops.ElementAt(0));
        }

        [Test]
        public static void Read_XmlDocumentWithScrambledSegmentsInOuterLoop_ReturnsLayerGeometryWithOuterLoopWithSortedSegments(
            [Values(true, false)] bool firstSegmentInverted,
            [Values(true, false)] bool secondSegmentInverted)
        {
            // Setup
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            double point1 = firstSegmentInverted ? 2.2 : 1.1;
            double point2 = firstSegmentInverted ? 1.1 : 2.2;
            double point3 = secondSegmentInverted ? 3.3 : 2.2;
            double point4 = secondSegmentInverted ? 2.2 : 3.3;
            const double point5 = 3.3;
            const double point6 = 1.1;

            string pointString1 = XmlConvert.ToString(point1);
            string pointString2 = XmlConvert.ToString(point2);
            string pointString3 = XmlConvert.ToString(point3);
            string pointString4 = XmlConvert.ToString(point4);
            string pointString5 = XmlConvert.ToString(point5);
            string pointString6 = XmlConvert.ToString(point6);

            XDocument bytes = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop>" +
                              "<CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{2}</X><Y>0.1</Y><Z>{2}</Z></HeadPoint>" +
                              "<EndPoint><X>{3}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{4}</X><Y>0.1</Y><Z>{4}</Z></HeadPoint>" +
                              "<EndPoint><X>{5}</X><Y>0.1</Y><Z>{5}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>",
                              pointString1, pointString2, pointString3, pointString4, pointString5, pointString6));

            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2DGeometry geometry = reader.Read(bytes);

            // Assert
            Assert.NotNull(geometry);
            Assert.AreEqual(new SoilLayer2DLoop(new[]
            {
                new Segment2D(new Point2D(1.1, 1.1), new Point2D(2.2, 2.2)),
                new Segment2D(new Point2D(2.2, 2.2), new Point2D(3.3, 3.3)),
                new Segment2D(new Point2D(3.3, 3.3), new Point2D(1.1, 1.1))
            }), geometry.OuterLoop);
        }

        [Test]
        public static void Read_XmlDocumentWithScrambledSegmentsInInnerLoop_ReturnsLayerGeometryWithInnerLoopWithSortedSegments(
            [Values(true, false)] bool firstSegmentInverted,
            [Values(true, false)] bool secondSegmentInverted)
        {
            // Setup
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            double point1 = firstSegmentInverted ? 2.2 : 1.1;
            double point2 = firstSegmentInverted ? 1.1 : 2.2;
            double point3 = secondSegmentInverted ? 3.3 : 2.2;
            double point4 = secondSegmentInverted ? 2.2 : 3.3;
            const double point5 = 3.3;
            const double point6 = 1.1;

            string pointString1 = XmlConvert.ToString(point1);
            string pointString2 = XmlConvert.ToString(point2);
            string pointString3 = XmlConvert.ToString(point3);
            string pointString4 = XmlConvert.ToString(point4);
            string pointString5 = XmlConvert.ToString(point5);
            string pointString6 = XmlConvert.ToString(point6);

            XDocument bytes = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop/><InnerLoops><GeometryLoop>" +
                              "<CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{2}</X><Y>0.1</Y><Z>{2}</Z></HeadPoint>" +
                              "<EndPoint><X>{3}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{4}</X><Y>0.1</Y><Z>{4}</Z></HeadPoint>" +
                              "<EndPoint><X>{5}</X><Y>0.1</Y><Z>{5}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList></GeometryLoop></InnerLoops></GeometrySurface>",
                              pointString1, pointString2, pointString3, pointString4, pointString5, pointString6));

            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2DGeometry geometry = reader.Read(bytes);

            // Assert
            Assert.NotNull(geometry);
            Assert.AreEqual(new SoilLayer2DLoop(new[]
            {
                new Segment2D(new Point2D(1.1, 1.1), new Point2D(2.2, 2.2)),
                new Segment2D(new Point2D(2.2, 2.2), new Point2D(3.3, 3.3)),
                new Segment2D(new Point2D(3.3, 3.3), new Point2D(1.1, 1.1))
            }), geometry.InnerLoops.ElementAt(0));
        }

        [Test]
        public static void Read_XmlDocumentWithSegmentsInOuterLoopThatAreNotConnected_ThrowsSoilLayerConversionException()
        {
            // Setup
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            const double point1 = 1.1;
            const double point2 = 2.2;
            const double point3 = 3.3;
            const double point4 = 4.4;

            string pointString1 = XmlConvert.ToString(point1);
            string pointString2 = XmlConvert.ToString(point2);
            string pointString3 = XmlConvert.ToString(point3);
            string pointString4 = XmlConvert.ToString(point4);

            XDocument bytes = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop>" +
                              "<CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{2}</X><Y>0.1</Y><Z>{2}</Z></HeadPoint>" +
                              "<EndPoint><X>{3}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>",
                              pointString1, pointString2, pointString3, pointString4));

            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(bytes);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public static void Read_XmlDocumentWithSegmentsInInnerLoopThatAreNotConnected_ThrowsSoilLayerConversionException()
        {
            // Setup
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            const double point1 = 1.1;
            const double point2 = 2.2;
            const double point3 = 3.3;
            const double point4 = 4.4;

            string pointString1 = XmlConvert.ToString(point1);
            string pointString2 = XmlConvert.ToString(point2);
            string pointString3 = XmlConvert.ToString(point3);
            string pointString4 = XmlConvert.ToString(point4);

            XDocument bytes = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop/><InnerLoops><GeometryLoop>" +
                              "<CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{2}</X><Y>0.1</Y><Z>{2}</Z></HeadPoint>" +
                              "<EndPoint><X>{3}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList></GeometryLoop></InnerLoops></GeometrySurface>",
                              pointString1, pointString2, pointString3, pointString4));

            var reader = new SoilLayer2DGeometryReader();

            // Call
            TestDelegate test = () => reader.Read(bytes);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        private static void Read_XmlDocumentWithOuterLoop_ReturnsLayerGeometryWithExpectedOuterLoop()
        {
            // Setup
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            const double point1 = 1.1;
            const double point2 = 2.2;
            const double point3 = 3.3;

            string pointString1 = XmlConvert.ToString(point1);
            string pointString2 = XmlConvert.ToString(point2);
            string pointString3 = XmlConvert.ToString(point3);
            XDocument bytes = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop><CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                              "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{2}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{2}</X><Y>0.1</Y><Z>{2}</Z></HeadPoint>" +
                              "<EndPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>",
                              pointString1, pointString2, pointString3));

            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2DGeometry geometry = reader.Read(bytes);

            // Assert
            Assert.NotNull(geometry);
            Assert.AreEqual(new SoilLayer2DLoop(new[]
            {
                new Segment2D(new Point2D(point1, point1), new Point2D(point2, point2)),
                new Segment2D(new Point2D(point2, point2), new Point2D(point3, point3)),
                new Segment2D(new Point2D(point3, point3), new Point2D(point1, point1))
            }), geometry.OuterLoop);
        }

        private static void Read_XmlDocumentWithInnerLoop_ReturnsLayerGeometryWithExpectedInnerLoop()
        {
            // Setup
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            const double point1 = 1.1;
            const double point2 = 2.2;
            const double point3 = 3.3;

            string pointString1 = XmlConvert.ToString(point1);
            string pointString2 = XmlConvert.ToString(point2);
            string pointString3 = XmlConvert.ToString(point3);
            XDocument bytes = GetXmlDocument(
                string.Format(invariantCulture,
                              "<GeometrySurface><OuterLoop/>" +
                              "<InnerLoops><GeometryLoop>" +
                              "<CurveList><GeometryCurve>" +
                              "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></HeadPoint>" +
                              "<EndPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{1}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                              "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{2}</Z></EndPoint>" +
                              "</GeometryCurve><GeometryCurve>" +
                              "<HeadPoint><X>{2}</X><Y>0.1</Y><Z>{2}</Z></HeadPoint>" +
                              "<EndPoint><X>{0}</X><Y>0.1</Y><Z>{0}</Z></EndPoint>" +
                              "</GeometryCurve></CurveList>" +
                              "</GeometryLoop></InnerLoops></GeometrySurface>",
                              pointString1, pointString2, pointString3));

            var reader = new SoilLayer2DGeometryReader();

            // Call
            SoilLayer2DGeometry geometry = reader.Read(bytes);

            // Assert
            Assert.NotNull(geometry);
            Assert.AreEqual(new SoilLayer2DLoop(new[]
            {
                new Segment2D(new Point2D(point1, point1), new Point2D(point2, point2)),
                new Segment2D(new Point2D(point2, point2), new Point2D(point3, point3)),
                new Segment2D(new Point2D(point3, point3), new Point2D(point1, point1))
            }), geometry.InnerLoops.ElementAt(0));
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
        /// Takes <paramref name="str"/> and returns an <see cref="Array"/> of <see cref="byte"/>
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