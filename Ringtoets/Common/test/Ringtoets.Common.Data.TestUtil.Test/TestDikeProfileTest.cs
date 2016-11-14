using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestDikeProfileTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Call
            var testProfile = new TestDikeProfile();

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.IsNull(testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "A";

            // Call
            var testProfile = new TestDikeProfile(name);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual(name, testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithPoint_ExpectedValues()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            var testProfile = new TestDikeProfile(point);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.IsNull(testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(point, testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithNameAndPoint_ExpectedValues()
        {
            // Setup
            const string name = "N";
            var point = new Point2D(-12.34, 7.78);

            // Call
            var testProfile = new TestDikeProfile(name, point);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual(name, testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(point, testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }
    }
}