using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml;
using NUnit.Framework;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Test
{
    public class PipingSoilLayer2DReaderTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Constructor_AnyByteArray_ReturnsNewInstance(int size)
        {
            // Call
            var result = new PipingSoilLayer2DReader(new byte[size]);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void Read_MalformedXmlDocument_ThrowsXmlException()
        {
            // Setup
            var xmlDoc = GetBytes("test");
            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            TestDelegate test = () => reader.Read();

            // Assert
            Assert.Throws<XmlException>(test);
        }

        [Test]
        public void Read_XmlDocumentWithoutSaneContent_ReturnsLayerWithoutOuterLoopAndEmptyInnerLoops()
        {
            // Setup
            var xmlDoc = GetBytes("<doc/>");
            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            Assert.IsNull(result.OuterLoop);
            CollectionAssert.IsEmpty(result.InnerLoops);
        }

        [Test]
        public void Read_XmlDocumentWithEmptyOuterLoop_ReturnsLayerWithEmptyOuterLoop()
        {
            // Setup
            var xmlDoc = GetBytes("<OuterLoop/>");
            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            CollectionAssert.IsEmpty(result.OuterLoop);
            CollectionAssert.IsEmpty(result.InnerLoops);
        }

        [Test]
        public void Read_XmlDocumentWithEmptyInnerLoop_ReturnsLayerWithOneEmptyInnerLoop()
        {
            // Setup
            var xmlDoc = GetBytes("<InnerLoop/>");
            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            Assert.IsNull(result.OuterLoop);
            Assert.AreEqual(1, result.InnerLoops.Count());
            CollectionAssert.IsEmpty(result.InnerLoops.ElementAt(0));
        }

        [Test]
        public void Read_XmlDocumentWithEmptyInnerLoopAndOuterLoop_ReturnsLayerWithEmptyInnerLoopAndEmptyOuterLoop()
        {
            // Setup
            var xmlDoc = GetBytes("<root><OuterLoop/><InnerLoop/></root>");
            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            CollectionAssert.IsEmpty(result.OuterLoop);
            Assert.AreEqual(1, result.InnerLoops.Count());
            CollectionAssert.IsEmpty(result.InnerLoops.ElementAt(0));
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
            var sometempvarforbassie = string.Format(invariantCulture, "<root><OuterLoop><GeometryCurve>" +
                                                                        "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                                                                        "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                                                                        "</GeometryCurve><GeometryCurve>" +
                                                                        "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                                                                        "<EndPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></EndPoint>" +
                                                                        "</GeometryCurve></OuterLoop></root>",
                                                                        x1String, y1String, x2String, y2String);
            var bytes = GetBytes(sometempvarforbassie);
            var xmlDoc = bytes;
            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            var result = reader.Read();

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
            var xmlDoc = GetBytes(string.Format(invariantCulture, "<root><InnerLoop><GeometryCurve>" +
                                  "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                                  "<HeadPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></HeadPoint>" +
                                  "</GeometryCurve><GeometryCurve>" +
                                  "<HeadPoint><X>{0}</X><Y>0.1</Y><Z>{1}</Z></HeadPoint>" +
                                  "<HeadPoint><X>{2}</X><Y>0.1</Y><Z>{3}</Z></HeadPoint>" +
                                  "</GeometryCurve></InnerLoop></root>", x1String, y1String, x2String, y2String));
            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            Segment2D expectedSegment = new Segment2D(new Point2D(parsedX1, parsedY1), new Point2D(parsedX2, parsedY2));
            var expectedCollection = new Collection<List<Segment2D>> { new List<Segment2D> { expectedSegment, expectedSegment } };
            CollectionAssert.AreEqual(expectedCollection, result.InnerLoops);
        }

        [Test]
        public void Read_XmlDocumentSinglePointOuterLoopGeometryCurve_ThrowsXmlException()
        {
            // Setup
            var xmlDoc = GetBytes("<root><OuterLoop><GeometryCurve><EndPoint><X>1</X><Y>0.1</Y><Z>1.1</Z></EndPoint></GeometryCurve></OuterLoop></root>");
            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            TestDelegate test = () => { reader.Read(); };

            // Assert
            Assert.Throws<XmlException>(test);
        }

        [Test]
        public void Read_XmlDocumentSinglePointInnerLoopGeometryCurve_ThrowsXmlException()
        {
            // Setup
            var xmlDoc = GetBytes("<root><InnerLoop><GeometryCurve><HeadPoint><X>0</X><Y>0.1</Y><Z>1.1</Z></HeadPoint></GeometryCurve></InnerLoop></root>");
            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            TestDelegate test = () => { reader.Read(); };

            // Assert
            Assert.Throws<XmlException>(test);
        }

        [Test]
        public void Read_XmlDocumentEqualSegments_ReturnsTwoEqualSegments()
        {
            // Setup
            var xmlDoc = GetBytes("<root><OuterLoop>" +
                                  "<GeometryCurve>" +
                                  "<HeadPoint><X>0</X><Y>0</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0</Y><Z>1.1</Z></EndPoint>" +
                                  "</GeometryCurve>" +
                                  "<GeometryCurve>" +
                                  "<HeadPoint><X>0</X><Y>0</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0</Y><Z>1.1</Z></EndPoint>" +
                                  "</GeometryCurve>" +
                                  "</OuterLoop></root>");

            var reader = new PipingSoilLayer2DReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.AreEqual(2, result.OuterLoop.Count);
            Assert.AreEqual(result.OuterLoop[0], result.OuterLoop[1]);
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}