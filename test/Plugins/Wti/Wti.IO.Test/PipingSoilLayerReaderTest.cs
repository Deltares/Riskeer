using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using Wti.Data;

namespace Wti.IO.Test
{
    public class PipingSoilLayerReaderTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Constructor_AnyByteArray_ReturnsNewInstance(int size)
        {
            // Call
            var result = new PipingSoilLayerReader(new byte[size]);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void Read_MalformedXmlDocument_ThrowsXmlException()
        {
            // Setup
            var xmlDoc = GetBytes("test");
            var reader = new PipingSoilLayerReader(xmlDoc);

            // Call
            TestDelegate test = () => reader.Read();

            // Assert
            Assert.Throws<XmlException>(test);
        }

        [Test]
        public void Read_XmlDocumentWithoutSaneContent_ReturnsLayerWithoutLoops()
        {
            // Setup
            var xmlDoc = GetBytes("<doc/>");
            var reader = new PipingSoilLayerReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            Assert.IsNull(result.OuterLoop);
            Assert.IsNull(result.InnerLoop);
        }

        [Test]
        public void Read_XmlDocumentWithEmptyOuterLoop_ReturnsLayerWithEmptyOuterLoop()
        {
            // Setup
            var xmlDoc = GetBytes("<OuterLoop/>");
            var reader = new PipingSoilLayerReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            CollectionAssert.IsEmpty(result.OuterLoop);
            Assert.IsNull(result.InnerLoop);
        }

        [Test]
        public void Read_XmlDocumentWithEmptyInnerLoop_ReturnsLayerWithEmptyInnerLoop()
        {
            // Setup
            var xmlDoc = GetBytes("<InnerLoop/>");
            var reader = new PipingSoilLayerReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            Assert.IsNull(result.OuterLoop);
            CollectionAssert.IsEmpty(result.InnerLoop);
        }

        [Test]
        public void Read_XmlDocumentWithEmptyInnerLoopAndOuterLoop_ReturnsLayerWithEmptyInnerLoopAndEmptyOuterLoop()
        {
            // Setup
            var xmlDoc = GetBytes("<root><OuterLoop/><InnerLoop/></root>");
            var reader = new PipingSoilLayerReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            CollectionAssert.IsEmpty(result.OuterLoop);
            CollectionAssert.IsEmpty(result.InnerLoop);
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
            var xmlDoc = GetBytes("<root><OuterLoop><HeadPoint><X>0</X><Y>0.1</Y><Z>1.1</Z></HeadPoint></OuterLoop></root>");
            var reader = new PipingSoilLayerReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            CollectionAssert.AreEqual(new HashSet<Point3D> {new Point3D{X=0,Y=0.1,Z=1.1}}, result.OuterLoop);
        }

        [Test]
        public void Read_XmlDocumentPointInInnerLoop_ReturnsLayerWithInnerLoopWithPoint()
        {
            // Setup
            var xmlDoc = GetBytes("<root><InnerLoop><HeadPoint><X>0</X><Y>0.1</Y><Z>1.1</Z></HeadPoint></InnerLoop></root>");
            var reader = new PipingSoilLayerReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            CollectionAssert.AreEqual(new HashSet<Point3D> { new Point3D { X = 0, Y = 0.1, Z = 1.1 } }, result.InnerLoop);
        }

        [Test]
        public void Read_XmlDocumentPointsInOuterLoop_ReturnsLayerWithOuterLoopWithPoints()
        {
            // Setup
            var xmlDoc = GetBytes("<root><OuterLoop><HeadPoint><X>0</X><Y>0.1</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0.1</Y><Z>1.1</Z></EndPoint></OuterLoop></root>");
            var reader = new PipingSoilLayerReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            CollectionAssert.AreEqual(new HashSet<Point3D> { new Point3D { X = 0, Y = 0.1, Z = 1.1 }, new Point3D { X = 1.0, Y = 0.1, Z = 1.1 } }, result.OuterLoop);
        }

        [Test]
        public void Read_XmlDocumentPointsInInnerLoop_ReturnsLayerWithInnerLoopWithPoints()
        {
            // Setup
            var xmlDoc = GetBytes("<root><InnerLoop><HeadPoint><X>0</X><Y>0.1</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0.1</Y><Z>1.1</Z></EndPoint></InnerLoop></root>");
            var reader = new PipingSoilLayerReader(xmlDoc);

            // Call
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            CollectionAssert.AreEqual(new HashSet<Point3D> { new Point3D { X = 0, Y = 0.1, Z = 1.1 }, new Point3D { X = 1.0, Y = 0.1, Z = 1.1 } }, result.InnerLoop);
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}