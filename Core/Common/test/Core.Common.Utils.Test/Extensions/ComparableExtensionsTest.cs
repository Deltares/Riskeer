using System;

using Core.Common.Utils.Extensions;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Extensions
{
    [TestFixture]
    public class ComparableExtensionsTest
    {
        [Test]
        public void NullIsSmallerThanAnything()
        {
            IComparable b = null;
            Assert.IsTrue(b.IsSmaller("anything"));
        }

        [Test]
        public void NullIsNotSmallerThanNull()
        {
            IComparable b = null;
            Assert.IsFalse(b.IsSmaller(null));
        }

        [Test]
        public void NullIsNotBiggerThanAnything()
        {
            IComparable b = null;
            Assert.IsFalse(b.IsBigger("anything"));
        }

        [Test]
        public void NullIsNotBiggerThanNull()
        {
            IComparable b = null;
            Assert.IsFalse(b.IsBigger((null)));
        }
    }
}