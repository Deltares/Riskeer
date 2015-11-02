using System.Drawing;
using Core.Common.Base.Tests.Properties;
using Core.Common.BaseDelftTools;
using NUnit.Framework;

namespace Core.Common.Base.Tests.Shell.Core
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
            Assert.IsNull(info.AddExampleData);
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
            Assert.IsNull(info.AddExampleData);
        }

        [Test]
        public void GetSetAutomaticProperties_NonGeneric_ExpectedBehavior()
        {
            // setup & call
            bool addExampleDataCalled = false;
            var info = new DataItemInfo
            {
                ValueType = typeof(double),
                Name = "Some double",
                Category = "Nice category",
                Image = new Bitmap(Resources.alarm_clock_blue),
                AdditionalOwnerCheck = o => true,
                CreateData = o => 1.2,
                AddExampleData = delegate { addExampleDataCalled = true; }
            };
            info.AddExampleData(null);

            // assert
            Assert.AreEqual(typeof(double), info.ValueType);
            Assert.AreEqual("Some double", info.Name);
            Assert.AreEqual("Nice category", info.Category);
            Assert.IsNotNull(info.Image);
            Assert.IsTrue(info.AdditionalOwnerCheck(null));
            Assert.AreEqual(1.2, info.CreateData(null));
            Assert.IsTrue(addExampleDataCalled);
        }

        [Test]
        public void GetSetAutomaticProperties_Generic_ExpectedBehavior()
        {
            // setup & call
            bool addExampleDataNotCalled = true;
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(Resources.alarm_clock_blue),
                AdditionalOwnerCheck = o => false,
                CreateData = o => -1,
                AddExampleData = delegate { addExampleDataNotCalled = false; }
            };
            info.AddExampleData(1);

            // assert
            Assert.AreEqual(typeof(int), info.ValueType);
            Assert.AreEqual("Some integer", info.Name);
            Assert.AreEqual("Better category", info.Category);
            Assert.IsNotNull(info.Image);
            Assert.IsFalse(info.AdditionalOwnerCheck(null));
            Assert.AreEqual(-1, info.CreateData(null));
            Assert.IsFalse(addExampleDataNotCalled);
        }

        [Test]
        public void ImplicitConversion_FromGenericToNonGeneric_ShouldCopyValues()
        {
            // setup
            bool addExampleDataCalled = false;
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(Resources.alarm_clock_blue),
                AdditionalOwnerCheck = o => false,
                CreateData = o => -1,
                AddExampleData = delegate { addExampleDataCalled = true; }
            };

            // call
            var nonGenericInfo = (DataItemInfo) info;

            // assert
            Assert.AreEqual(info.ValueType, nonGenericInfo.ValueType);
            Assert.AreEqual(info.Name, nonGenericInfo.Name);
            Assert.AreEqual(info.Category, nonGenericInfo.Category);
            Assert.AreEqual(info.AdditionalOwnerCheck(1), nonGenericInfo.AdditionalOwnerCheck(1));
            Assert.AreEqual(info.CreateData(null), nonGenericInfo.CreateData(null));
            nonGenericInfo.AddExampleData(1);
            Assert.IsTrue(addExampleDataCalled);
        }

        [Test]
        public void ImplicitConversion_FromGenericToNonGenericWithoutMethodsSet_MethodsShouldBeNull()
        {
            // setup
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(Resources.alarm_clock_blue)
            };

            // call
            var nonGenericInfo = (DataItemInfo) info;

            // assert
            Assert.AreEqual(info.ValueType, nonGenericInfo.ValueType);
            Assert.AreEqual(info.Name, nonGenericInfo.Name);
            Assert.AreEqual(info.Category, nonGenericInfo.Category);
            Assert.IsNull(nonGenericInfo.AdditionalOwnerCheck);
            Assert.IsNull(nonGenericInfo.CreateData);
            Assert.IsNull(nonGenericInfo.AddExampleData);
        }
    }
}