using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelftTools.Utils.Collections;
using NUnit.Framework;
using SharpTestsEx;

namespace DelftTools.Utils.Tests.Collections
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void HasExactOneValue()
        {
            IEnumerable one = Enumerable.Range(1, 1);
            Assert.IsTrue(one.HasExactlyOneValue());

            //has two
            IEnumerable two= Enumerable.Range(1, 2);
            
            two.HasExactlyOneValue()
                .Should().Be.False();
            
            //has none
            Enumerable.Empty<double>().HasExactlyOneValue()
                .Should().Be.False();
        }

        [Test]
        public void PlusShouldAddElements()
        {
            var items = new[] {1, 2, 3};
            var results = items.Plus(4);

            results.Should().Have.SameSequenceAs(new[] {1, 2, 3, 4});

            results = items.Plus(4, 5);

            results.Should().Have.SameSequenceAs(new[] { 1, 2, 3, 4, 5 });
        }
        
        [Test]
        public void ForEach()
        {
            var items = new [] {1, 2, 3};
            
            var results = new List<int>();

            items.ForEach(results.Add);

            results
                .Should("elements should be equal").Have.SameSequenceAs(items);
        }

        [Test]
        public void ForEachWithIndex()
        {
            var items = new[] {1, 2, 3};

            var resultIndices = new List<int>();
            var resultElements = new List<int>();

            items.ForEach((o, i) => { resultElements.Add(o); resultIndices.Add(i); });

            resultElements.Should().Have.SameSequenceAs(items);
            resultIndices.Should().Have.SameSequenceAs(new []{0, 1, 2});
        }

        [Test]
        public void IsMonotonousAscending()
        {
            var items = new[] { 1, 2, 3,3,3,5 };
            Assert.IsTrue(items.IsMonotonousAscending());

            
            Assert.IsFalse(new[] { 1, 2, 1}.IsMonotonousAscending());
        }


        [Test]
        public void SplitInGroupsAndVerify()
        {
            var items0 = new int[0];
            var items1 = new[] { 1, 2, 3, 3, 3, 5 };

            Assert.AreEqual(0, items0.SplitInGroups(5).Count());
            Assert.AreEqual(2, items1.SplitInGroups(5).Count());
            Assert.AreEqual(2, items1.SplitInGroups(3).Count());
            Assert.AreEqual(3, items1.SplitInGroups(2).Count());

            Assert.AreEqual(1, items1.SplitInGroups(5).ToList()[0][0]);
            Assert.AreEqual(5, items1.SplitInGroups(5).ToList()[1][0]);
            
        }
    }
}