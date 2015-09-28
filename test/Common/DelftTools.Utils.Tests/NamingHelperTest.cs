using System;
using System.Collections.Generic;
using System.Linq;
using DelftTools.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class NamingHelperTest
    {
        private MockRepository mocks;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            mocks = new MockRepository();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void GetUniqueNameWithInvalidFilter()
        {
            NamingHelper.GetUniqueName("invalidfilter", new INameable[] { }, null);
        }

        [Test]
        public void GetUniqueName()
        {
            var item1 = mocks.Stub<INameable>();
            var item2 = mocks.Stub<INameable>();
            var item3 = mocks.Stub<INameable>();

            item1.Name = "one (1)";
            item2.Name = "one";
            item3.Name = "INameable1";

            var namedItems = new List<INameable>(new[] { item1, item2, item3 });

            Assert.AreEqual("INameable2", NamingHelper.GetUniqueName(null, namedItems, typeof(INameable)));
            Assert.AreEqual("one (2)", NamingHelper.GetUniqueName("one ({0})", namedItems, typeof(INameable)));
        }

        [Test]
        public void GetUniqueNameWithIgnoreCaseFalse()
        {
            var item1 = mocks.Stub<INameable>();
            var item2 = mocks.Stub<INameable>();

            item1.Name = "one (1)";
            item2.Name = "Inameable1";

            var namedItems = new List<INameable>(new[] { item1, item2 });

            Assert.AreEqual("INameable1", NamingHelper.GetUniqueName(null, namedItems, typeof(INameable), false));
            Assert.AreEqual("One (1)", NamingHelper.GetUniqueName("One ({0})", namedItems, typeof(INameable), false));
        }

        [Test]
        public void GetUniqueNameWithIgnoreCaseTrue()
        {
            var item1 = mocks.Stub<INameable>();
            var item2 = mocks.Stub<INameable>();

            item1.Name = "one (1)";
            item2.Name = "Inameable1";

            var namedItems = new List<INameable>(new[] { item1, item2 });

            Assert.AreEqual("INameable2", NamingHelper.GetUniqueName(null, namedItems, typeof(INameable)));
            Assert.AreEqual("One (2)", NamingHelper.GetUniqueName("One ({0})", namedItems, typeof(INameable)));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GenerateUniqueNameFromList_NameNotInCollection_ReturnOriginalName(bool ignoreCase)
        {
            // setup
            var names = new[] { "test 1", "test 2" };

            // call
            var uniqueName = NamingHelper.GenerateUniqueNameFromList("haha", ignoreCase, names);

            // assert
            Assert.AreEqual("haha", uniqueName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GenerateUniqueNameFromList_NameInCollection_ReturnOriginalName(bool ignoreCase)
        {
            // setup
            var names = new[] { "test 1", "test 2" };

            // call
            var uniqueName = NamingHelper.GenerateUniqueNameFromList("test {0}", ignoreCase, names);

            // assert
            Assert.AreEqual("test 3", uniqueName);
        }

        [Test]
        public void MakeNamesUnique()
        {
            var nameables = Enumerable.Range(0, 1000).Select(i =>
                {
                    var nameable = mocks.Stub<INameable>();
                    nameable.Name = string.Format("Nameable {0}", i);
                    return nameable;
                });

            var nameablesList = new List<INameable>(nameables);

            var first = mocks.Stub<INameable>();
            first.Name = nameablesList.First().Name;

            var last = mocks.Stub<INameable>();
            first.Name = nameablesList.Last().Name;

            var middle = mocks.Stub<INameable>();
            middle.Name = nameablesList[500].Name;

            nameablesList.Insert(0, first);
            nameablesList.Insert(500, middle);
            nameablesList.Insert(nameablesList.Count, last);

            mocks.ReplayAll();

            NamingHelper.MakeNamesUnique(nameables);

            var uniqueNamesCount = nameablesList.Distinct().Count();
            Assert.AreEqual(uniqueNamesCount, nameablesList.Count);
        }

        [Test]
        [Category(TestCategory.Performance)]
        public void MakeNamesUniqueForWorstCaseShouldBeQuick()
        {
            var nameables = Enumerable.Range(0, 10000).Select(i =>
            {
                var nameable = mocks.Stub<INameable>();
                nameable.Name = "Nameable";
                return nameable;
            });

            var nameablesList = new List<INameable>(nameables);
            
            mocks.ReplayAll();

            TestHelper.AssertIsFasterThan(600, () => NamingHelper.MakeNamesUnique(nameables));
            
            var uniqueNamesCount = nameablesList.Distinct().Count();
            Assert.AreEqual(uniqueNamesCount, nameablesList.Count);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
        }
    }
}	