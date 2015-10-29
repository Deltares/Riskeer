using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils.Aop.Markers;
using Core.Common.Utils.Reflection;
using NUnit.Framework;

namespace Core.Common.Utils.Tests.Reflection
{
    [TestFixture]
    public class AutoClonerTest
    {
        [Test]
        public void AutoCloneClassWithValuesArrayAsField()
        {
            var inst = new ClassWithValueArray
            {
                values = new[]
                {
                    1.0,
                    3.0
                }
            };
            var clone = TypeUtils.DeepClone(inst);
            Assert.AreEqual(inst.values.Length, clone.values.Length);
            Assert.AreEqual(inst.values[0], clone.values[0]);
        }

        [Test]
        public void AutoCloneClassWithBaseClass()
        {
            var otherName = "onm";
            var name = "nm";

            var deepPeer = new TypeUtilsTest.SuperCloneTestClassWithReference();
            var inst = new TypeUtilsTest.SuperCloneTestClassWithReference
            {
                OtherName = otherName,
                Name = name,
                Peer = new TypeUtilsTest.SuperCloneTestClassWithReference
                {
                    Peer = deepPeer
                }
            };

            var clone = TypeUtils.DeepClone(inst);

            Assert.AreEqual(otherName, clone.OtherName);
            Assert.AreEqual(name, clone.Name);
            Assert.AreNotSame(inst, clone);
            Assert.IsNotNull(clone.Peer);
            var clonedDeepPeer = ((TypeUtilsTest.SuperCloneTestClassWithReference) clone.Peer).Peer;
            Assert.IsNotNull(clonedDeepPeer);
            Assert.AreNotSame(inst.Peer, clone.Peer);
            Assert.AreNotSame(deepPeer, clonedDeepPeer);
        }

        [Test]
        public void AutoCloneWithExternalAggregation()
        {
            // create small graph with aggregation
            var set = new Set
            {
                Elements =
                {
                    new Element
                    {
                        Name = "a"
                    },
                    new Element
                    {
                        Name = "b"
                    }
                }
            };
            set.Elements.ForEach(e => e.Set = set);

            // clone only subgraph
            var clonedElement = TypeUtils.DeepClone(set.Elements.First());

            // make sure objects outside graph weren't cloned
            Assert.AreSame(set, clonedElement.Set);
        }

        [Test]
        public void AutoCloneWithInternalAggregation()
        {
            // create small graph with aggregation
            var set = new Set
            {
                Elements =
                {
                    new Element
                    {
                        Name = "a"
                    },
                    new Element
                    {
                        Name = "b"
                    }
                }
            };
            set.Elements.ForEach(e => e.Set = set);
            set.RootElement = set.Elements.First();

            // clone entire graph
            var clonedSet = TypeUtils.DeepClone(set);

            // make sure internal aggregation was cloned correctly
            Assert.AreNotSame(clonedSet.RootElement, set.RootElement);
            Assert.AreSame(clonedSet.Elements.First(), clonedSet.RootElement);
        }

        private class ClassWithValueArray
        {
            public double[] values;
        }

        public class Element
        {
            [Aggregation]
            public Set Set { get; set; }

            public string Name { get; set; }
        }

        public class Set
        {
            public Set()
            {
                Elements = new List<Element>();
            }

            public List<Element> Elements { get; set; }

            [Aggregation]
            public Element RootElement { get; set; }
        }
    }
}