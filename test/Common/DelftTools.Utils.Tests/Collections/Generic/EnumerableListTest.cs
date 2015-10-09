using System;
using System.Linq;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections.Generic;
using log4net;
using log4net.Core;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Collections.Generic
{
    [TestFixture]
    public class EnumerableListTest
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EnumerableListTest));

        [Test]
        public void EnumerableListItemCountShouldBeFast()
        {
            var items = new EventedList<object>();
            var editor = new EnumerableListEditor(items);

            var strings = new EnumerableList<string>
            {
                Enumerable = items.OfType<string>(),
                Editor = editor
            };

            for (int i = 0; i < 100000; i++)
            {
                strings.Add(i.ToString());
            }

            //actual limit of 450 is more like a max on my machine but buildagents do this one very slow.
            int count = 0;

            // timings are unstable (reflection?)
            TestHelper.AssertIsFasterThan(1200, () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    count += strings.Count;
                }
            });

            Assert.AreEqual(100*100000, count);
        }

        [Test]
        public void EnumerableListWithCacheItemCountShouldBeFast()
        {
            EventedList<object> f = new EventedList<object>();
            var sampleFeatures = f.OfType<string>();

            var editor = new EnumerableListEditor(f);
            var features = new EnumerableList<string>
            {
                Enumerable = sampleFeatures,
                Editor = editor,
                CollectionChangeSource = f
            };
            long numValues = 10000;
            long numCalculations = 15000;
            for (int i = 0; i < numValues; i++)
            {
                features.Add(i.ToString());
            }

            EnumerableList<string> featureCollection = features;

            long count = 0;
            Action action = delegate
            {
                for (int i = 0; i < numCalculations; i++)
                {
                    count += featureCollection.Count();
                }
            };

            TestHelper.AssertIsFasterThan(30, action);

            Assert.AreEqual(numCalculations*numValues, count);
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            LogHelper.ConfigureLogging();
            LogHelper.SetLoggingLevel(Level.Info);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogHelper.ResetLogging();
        }
    }

    public class EnumerableListEditor : IEnumerableListEditor
    {
        public EnumerableListEditor(EventedList<object> eventedList)
        {
            EventedList = eventedList;
        }

        public EventedList<object> EventedList { get; set; }

        public void OnAdd(object o)
        {
            EventedList.Add(o);
        }

        public void OnRemove(object o) {}

        public void OnInsert(int index, object value) {}

        public void OnRemoveAt(int index) {}

        public void OnReplace(int index, object o) {}

        public void OnClear() {}
    }
}