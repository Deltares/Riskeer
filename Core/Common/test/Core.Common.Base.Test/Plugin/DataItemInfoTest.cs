using System.Drawing;
using Core.Common.Base.Plugin;
using NUnit.Framework;

namespace Core.Common.Base.Test.Plugin
{
    [TestFixture]
    public class DataItemInfoTest
    {
        [Test]
        public void DefaultConstructor_NonGeneric_ExpectedValues()
        {
            // Call
            var dataItemInfo = new DataItemInfo();

            // Assert
            Assert.IsNull(dataItemInfo.ValueType);
            Assert.AreEqual("", dataItemInfo.Name);
            Assert.AreEqual("", dataItemInfo.Category);
            Assert.IsNull(dataItemInfo.Image);
            Assert.IsNull(dataItemInfo.AdditionalOwnerCheck);
            Assert.IsNull(dataItemInfo.CreateData);
        }

        [Test]
        public void DefaultConstructor_Generic_ExpectedValues()
        {
            // Call
            var dataItemInfo = new DataItemInfo<double>();

            // Assert
            Assert.AreEqual(typeof(double), dataItemInfo.ValueType);
            Assert.AreEqual("", dataItemInfo.Name);
            Assert.AreEqual("", dataItemInfo.Category);
            Assert.IsNull(dataItemInfo.Image);
            Assert.IsNull(dataItemInfo.AdditionalOwnerCheck);
            Assert.IsNull(dataItemInfo.CreateData);
        }

        [Test]
        public void GetSetAutomaticProperties_NonGeneric_ExpectedBehavior()
        {
            // Setup / Call
            var dataItemInfo = new DataItemInfo
            {
                ValueType = typeof(double),
                Name = "Some double",
                Category = "Nice category",
                Image = new Bitmap(16, 16),
                AdditionalOwnerCheck = o => true,
                CreateData = o => 1.2,
            };

            // Assert
            Assert.AreEqual(typeof(double), dataItemInfo.ValueType);
            Assert.AreEqual("Some double", dataItemInfo.Name);
            Assert.AreEqual("Nice category", dataItemInfo.Category);
            Assert.IsNotNull(dataItemInfo.Image);
            Assert.IsTrue(dataItemInfo.AdditionalOwnerCheck(null));
            Assert.AreEqual(1.2, dataItemInfo.CreateData(null));
        }

        [Test]
        public void GetSetAutomaticProperties_Generic_ExpectedBehavior()
        {
            // Setup / Call
            var dataItemInfo = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(16, 16),
                AdditionalOwnerCheck = o => false,
                CreateData = o => -1,
            };

            // assert
            Assert.AreEqual(typeof(int), dataItemInfo.ValueType);
            Assert.AreEqual("Some integer", dataItemInfo.Name);
            Assert.AreEqual("Better category", dataItemInfo.Category);
            Assert.IsNotNull(dataItemInfo.Image);
            Assert.IsFalse(dataItemInfo.AdditionalOwnerCheck(null));
            Assert.AreEqual(-1, dataItemInfo.CreateData(null));
        }

        [Test]
        public void ImplicitConversion_FromGenericToNonGeneric_ShouldCopyValues()
        {
            // Setup
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(16, 16),
                AdditionalOwnerCheck = o => false,
                CreateData = o => -1,
            };

            // Call
            var nonGenericInfo = (DataItemInfo) info;

            // Assert
            Assert.AreEqual(info.ValueType, nonGenericInfo.ValueType);
            Assert.AreEqual(info.Name, nonGenericInfo.Name);
            Assert.AreEqual(info.Category, nonGenericInfo.Category);
            Assert.AreEqual(info.AdditionalOwnerCheck(1), nonGenericInfo.AdditionalOwnerCheck(1));
            Assert.AreEqual(info.CreateData(null), nonGenericInfo.CreateData(null));
        }

        [Test]
        public void ImplicitConversion_FromGenericToNonGenericWithoutMethodsSet_MethodsShouldBeNull()
        {
            // Setup
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(16, 16)
            };

            // Call
            var nonGenericInfo = (DataItemInfo) info;

            // Assert
            Assert.AreEqual(info.ValueType, nonGenericInfo.ValueType);
            Assert.AreEqual(info.Name, nonGenericInfo.Name);
            Assert.AreEqual(info.Category, nonGenericInfo.Category);
            Assert.IsNull(nonGenericInfo.AdditionalOwnerCheck);
            Assert.IsNull(nonGenericInfo.CreateData);
        }
    }
}