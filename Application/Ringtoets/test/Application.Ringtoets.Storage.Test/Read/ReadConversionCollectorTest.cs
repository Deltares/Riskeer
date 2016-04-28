using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class ReadConversionCollectorTest
    {

        [Test]
        public void Contains_WithoutEntity_ArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Contains(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Contains_SoilProfileAdded_True()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();
            collector.Read(entity, new TestPipingSoilProfile());

            // Call
            var result = collector.Contains(entity);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_NoSoilProfileAdded_False()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();

            // Call
            var result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_OtherSoilProfileEntityAdded_False()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();
            collector.Read(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            var result = collector.Contains(entity);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithoutEntity_ArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Get(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Get_SoilProfileAdded_ReturnsEntity()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var profile = new TestPipingSoilProfile();
            var entity = new SoilProfileEntity();
            collector.Read(entity, profile);

            // Call
            var result = collector.Get(entity);

            // Assert
            Assert.AreSame(profile, result);
        }

        [Test]
        public void Get_NoSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Get_OtherSoilProfileAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var entity = new SoilProfileEntity();
            collector.Read(new SoilProfileEntity(), new TestPipingSoilProfile());

            // Call
            TestDelegate test = () => collector.Get(entity);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Read_WithNullSoilProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(null, new TestPipingSoilProfile());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_WithNullProfileEntity_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => collector.Read(new SoilProfileEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", paramName);
        }
    }
}