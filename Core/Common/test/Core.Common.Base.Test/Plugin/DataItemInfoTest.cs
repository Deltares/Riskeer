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
            // call
            var info = new DataItemInfo();

            // assert
            Assert.IsNull(info.ValueType);
            Assert.IsNull(info.Name);
            Assert.IsNull(info.Category);
            Assert.IsNull(info.Image);
            Assert.IsNull(info.AdditionalOwnerCheck);
            Assert.IsNull(info.CreateData);
        }

        [Test]
        public void DefaultConstructor_Generic_ExpectedValues()
        {
            // call
            var info = new DataItemInfo<double>();

            // assert
            Assert.AreEqual(typeof(double), info.ValueType);
            Assert.IsNull(info.Name);
            Assert.IsNull(info.Category);
            Assert.IsNull(info.Image);
            Assert.IsNull(info.AdditionalOwnerCheck);
            Assert.IsNull(info.CreateData);
        }

        [Test]
        public void GetSetAutomaticProperties_NonGeneric_ExpectedBehavior()
        {
            // setup & call
            var info = new DataItemInfo
            {
                ValueType = typeof(double),
                Name = "Some double",
                Category = "Nice category",
                Image = new Bitmap(16, 16),
                AdditionalOwnerCheck = o => true,
                CreateData = o => 1.2,
            };

            // assert
            Assert.AreEqual(typeof(double), info.ValueType);
            Assert.AreEqual("Some double", info.Name);
            Assert.AreEqual("Nice category", info.Category);
            Assert.IsNotNull(info.Image);
            Assert.IsTrue(info.AdditionalOwnerCheck(null));
            Assert.AreEqual(1.2, info.CreateData(null));
        }

        [Test]
        public void GetSetAutomaticProperties_Generic_ExpectedBehavior()
        {
            // setup & call
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(16, 16),
                AdditionalOwnerCheck = o => false,
                CreateData = o => -1,
            };

            // assert
            Assert.AreEqual(typeof(int), info.ValueType);
            Assert.AreEqual("Some integer", info.Name);
            Assert.AreEqual("Better category", info.Category);
            Assert.IsNotNull(info.Image);
            Assert.IsFalse(info.AdditionalOwnerCheck(null));
            Assert.AreEqual(-1, info.CreateData(null));
        }

        [Test]
        public void ImplicitConversion_FromGenericToNonGeneric_ShouldCopyValues()
        {
            // setup
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(16, 16),
                AdditionalOwnerCheck = o => false,
                CreateData = o => -1,
            };

            // call
            var nonGenericInfo = (DataItemInfo) info;

            // assert
            Assert.AreEqual(info.ValueType, nonGenericInfo.ValueType);
            Assert.AreEqual(info.Name, nonGenericInfo.Name);
            Assert.AreEqual(info.Category, nonGenericInfo.Category);
            Assert.AreEqual(info.AdditionalOwnerCheck(1), nonGenericInfo.AdditionalOwnerCheck(1));
            Assert.AreEqual(info.CreateData(null), nonGenericInfo.CreateData(null));
        }

        [Test]
        public void ImplicitConversion_FromGenericToNonGenericWithoutMethodsSet_MethodsShouldBeNull()
        {
            // setup
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(16, 16)
            };

            // call
            var nonGenericInfo = (DataItemInfo) info;

            // assert
            Assert.AreEqual(info.ValueType, nonGenericInfo.ValueType);
            Assert.AreEqual(info.Name, nonGenericInfo.Name);
            Assert.AreEqual(info.Category, nonGenericInfo.Category);
            Assert.IsNull(nonGenericInfo.AdditionalOwnerCheck);
            Assert.IsNull(nonGenericInfo.CreateData);
        }
    }
}