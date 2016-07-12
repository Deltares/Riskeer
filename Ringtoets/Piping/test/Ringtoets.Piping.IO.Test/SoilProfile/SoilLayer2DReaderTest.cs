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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using Core.Common.Base.Geometry;

using NUnit.Framework;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.IO.Test.TestHelpers;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayer2DReaderTest
    {
        [Test]
        public void Constructor_Always_ReturnsNewInstance()
        {
            // Call
            var result = new SoilLayer2DReader();

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void Read_MalformedXmlDocument_ThrowsSoilLayer2DConversionException()
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetByteArray("test");
            var reader = new SoilLayer2DReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml, exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWithoutSaneContent_ThrowsSoilLayer2DConversionException()
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetXmlDocument("<doc/>");
            var reader = new SoilLayer2DReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml, exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWithNoInnerLoops_ThrowsSoilLayer2DConversionException()
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetXmlDocument("<GeometrySurface><OuterLoop/></GeometrySurface>");
            var reader = new SoilLayer2DReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml, exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWithNoOuterLoop_ThrowsSoilLayer2DConversionException()
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetXmlDocument("<GeometrySurface><InnerLoops><InnerLoop/></InnerLoops></GeometrySurface>");
            var reader = new SoilLayer2DReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml, exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWithEmptyInnerLoopAndOuterLoop_ReturnsLayerWithEmptyInnerLoopAndEmptyOuterLoop()
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetXmlDocument("<GeometrySurface><OuterLoop/><InnerLoops><InnerLoop/></InnerLoops></GeometrySurface>");
            var reader = new SoilLayer2DReader();

            // Call
            var result = reader.Read(xmlDoc);

            // Assert
            Assert.NotNull(result);
            CollectionAssert.IsEmpty(result.OuterLoop);
            Assert.AreEqual(1, result.InnerLoops.Count());
            CollectionAssert.IsEmpty(result.InnerLoops.ElementAt(0));
        }

        [Test]
        [TestCase("x")]
        [TestCase("")]
        public void Read_XmlDocumentWithInvalidPointCoordinate_ThrowsSoilLayer2DConversionException(string incorrectNumber)
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetXmlDocument(string.Format(
                "<GeometrySurface><OuterLoop><CurveList><GeometryCurve>" +
                "<HeadPoint><X>{0}</X><Z>1.2</Z></HeadPoint>" +
                "<EndPoint><X>1.2</X><Z>1.2</Z></EndPoint>" +
                "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>",incorrectNumber));
            var reader = new SoilLayer2DReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual(Resources.SoilLayer2DReader_Could_not_parse_point_location, exception.Message);
        }

        [Test]
        public void Read_XmlDocumentWitOverflowingPointCoordinate_ThrowsSoilLayer2DConversionException()
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetXmlDocument(string.Format(
                "<GeometrySurface><OuterLoop><CurveList><GeometryCurve>" +
                "<HeadPoint><X>{0}</X><Z>1.2</Z></HeadPoint>" +
                "<EndPoint><X>1.2</X><Z>1.2</Z></EndPoint>" +
                "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>",double.MaxValue));
            var reader = new SoilLayer2DReader();

            // Call
            TestDelegate test = () => reader.Read(xmlDoc);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual(Resources.SoilLayer2DReader_Could_not_parse_point_location, exception.Message);
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

        private void Read_XmlDocumentPointInOuterLoop_ReturnsLayerWithOuterLoopWithPoint()
        {
            // Setup
            var random = new Random(22);
            var invariantCulture = CultureInfo.InvariantCulture;

            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();

            var x1String = x1.ToString(invariantCulture);
            var x2String = x2.ToString(invariantCulture);
            var y1String = y1.ToString(invariantCulture);
            var y2String = y2.ToString(invariantCulture);
            var parsedX1 = double.Parse(x1String, invariantCulture);
            var parsedX2 = double.Parse(x2String, invariantCulture);
            var parsedY1 = double.Parse(y1String, invariantCulture);
            var parsedY2 = double.Parse(y2String, invariantCulture);
            var bytes = StringGeometryHelper.GetXmlDocument(string.Format(invariantCulture, "<GeometrySurface><OuterLoop><CurveList><GeometryCurve>" +
                                                                                            "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                                                                                            "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                                                                                            "</GeometryCurve><GeometryCurve>" +
                                                                                            "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                                                                                            "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                                                                                            "</GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>",
                                                                          x1String, y1String, x2String, y2String));
            var xmlDoc = bytes;
            var reader = new SoilLayer2DReader();

            // Call
            var result = reader.Read(xmlDoc);

            // Assert
            Assert.NotNull(result);
            Segment2D expectedSegment = new Segment2D(new Point2D(parsedX1, parsedY1), new Point2D(parsedX2, parsedY2));
            CollectionAssert.AreEqual(new List<Segment2D> {expectedSegment, expectedSegment}, result.OuterLoop);
        }

        [Test]
        public void Read_XmlDocumentPointsInInnerLoop_ReturnsLayerWithInnerLoopWithSegment()
        {
            // Setup
            var random = new Random(22);
            var invariantCulture = CultureInfo.InvariantCulture;

            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();

            var x1String = x1.ToString(invariantCulture);
            var x2String = x2.ToString(invariantCulture);
            var y1String = y1.ToString(invariantCulture);
            var y2String = y2.ToString(invariantCulture);
            var parsedX1 = double.Parse(x1String, invariantCulture);
            var parsedX2 = double.Parse(x2String, invariantCulture);
            var parsedY1 = double.Parse(y1String, invariantCulture);
            var parsedY2 = double.Parse(y2String, invariantCulture);
            var xmlDoc = StringGeometryHelper.GetXmlDocument(string.Format(invariantCulture, "<GeometrySurface><OuterLoop/><InnerLoops><InnerLoop><CurveList><GeometryCurve>" +
                                                                                             "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                                                                                             "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                                                                                             "</GeometryCurve><GeometryCurve>" +
                                                                                             "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                                                                                             "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                                                                                             "</GeometryCurve></CurveList></InnerLoop></InnerLoops></GeometrySurface>", x1String, y1String, x2String, y2String));
            var reader = new SoilLayer2DReader();

            // Call
            var result = reader.Read(xmlDoc);

            // Assert
            Assert.NotNull(result);
            Segment2D expectedSegment = new Segment2D(new Point2D(parsedX1, parsedY1), new Point2D(parsedX2, parsedY2));
            var expectedCollection = new Collection<List<Segment2D>> { new List<Segment2D> { expectedSegment, expectedSegment } };
            CollectionAssert.AreEqual(expectedCollection, result.InnerLoops);
        }

        [Test]
        public void Read_XmlDocumentSinglePointOuterLoopGeometryCurve_ThrowsSoilLayer2DConversionException()
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetXmlDocument("<GeometrySurface><OuterLoop><CurveList><GeometryCurve><EndPoint><X>1</X><Y>0.1</Y><Z>1.1</Z></EndPoint></GeometryCurve></CurveList></OuterLoop><InnerLoops/></GeometrySurface>");
            var reader = new SoilLayer2DReader();

            // Call
            TestDelegate test = () => { reader.Read(xmlDoc); };

            // Assert
            Assert.Throws<SoilLayerConversionException>(test);
        }

        [Test]
        public void Read_XmlDocumentSinglePointInnerLoopGeometryCurve_ThrowsSoilLayer2DConversionException()
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetXmlDocument("<GeometrySurface><InnerLoops><InnerLoop><GeometryCurve><HeadPoint><X>0</X><Y>0.1</Y><Z>1.1</Z></HeadPoint></GeometryCurve></InnerLoop></InnerLoops></GeometrySurface>");
            var reader = new SoilLayer2DReader();

            // Call
            TestDelegate test = () => { reader.Read(xmlDoc); };

            // Assert
            Assert.Throws<SoilLayerConversionException>(test);
        }

        [Test]
        public void Read_XmlDocumentEqualSegments_ReturnsTwoEqualSegments()
        {
            // Setup
            var xmlDoc = StringGeometryHelper.GetXmlDocument("<GeometrySurface><OuterLoop><CurveList>" +
                                                             "<GeometryCurve>" +
                                                             "<HeadPoint><X>0</X><Y>0</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0</Y><Z>1.1</Z></EndPoint>" +
                                                             "</GeometryCurve>" +
                                                             "<GeometryCurve>" +
                                                             "<HeadPoint><X>0</X><Y>0</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0</Y><Z>1.1</Z></EndPoint>" +
                                                             "</GeometryCurve>" +
                                                             "</CurveList></OuterLoop><InnerLoops/></GeometrySurface>");

            var reader = new SoilLayer2DReader();

            // Call
            var result = reader.Read(xmlDoc);

            // Assert
            Assert.AreEqual(2, result.OuterLoop.Count());
            Assert.AreEqual(result.OuterLoop.ElementAt(0), result.OuterLoop.ElementAt(1));
        }
    }
}