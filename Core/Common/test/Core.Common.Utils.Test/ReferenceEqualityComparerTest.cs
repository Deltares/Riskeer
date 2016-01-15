using NUnit.Framework;

namespace Core.Common.Utils.Test
{
    [TestFixture]
    public class ReferenceEqualityComparerTest
    {
        [Test]
        public void HashCode_Object_ReturnHashCode()
        {
            // Setup
            var comparer = new ReferenceEqualityComparer<object>();
            var obj = new object();

            // Call
            var code = comparer.GetHashCode(obj);

            // Assert
            Assert.AreEqual(code, obj.GetHashCode());
        }

        [Test]
        public void HashCode_ObjectHashCodeOverride_ReturnsObjectHashCode()
        {
            // Setup
            var comparer = new ReferenceEqualityComparer<object>();
            var obj = new TestObject();

            // Call
            var code = comparer.GetHashCode(obj);

            // Assert
            Assert.AreNotEqual(code, obj.GetHashCode());
            Assert.AreEqual(code, obj.GetBaseHashCode());
        }

        [Test]
        public void HashCode_DifferentInstance_ReturnDifferentHashCode()
        {
            // Setup
            var comparer = new ReferenceEqualityComparer<object>();

            // Call
            var codeFirst = comparer.GetHashCode(new TestObject());
            var codeOther = comparer.GetHashCode(new TestObject());

            // Assert
            Assert.AreNotEqual(codeFirst, codeOther);
        }

        [Test]
        public void Equals_SameInstance_ReturnTrue() {
            // Setup
            var comparer = new ReferenceEqualityComparer<object>();
            var obj = new object();

            // Call & Assert
            Assert.IsTrue(comparer.Equals(obj, obj));
            Assert.AreEqual(comparer.GetHashCode(obj), comparer.GetHashCode(obj));
        }

        [Test]
        public void Equals_OtherEqualsInstance_ReturnFalse()
        {
            // Setup
            var comparer = new ReferenceEqualityComparer<object>();
            var objectFirst = new TestObject();
            var objectSecond = new TestObject();

            // Call 
            var equals = comparer.Equals(objectFirst, objectSecond);
            
            // Assert
            Assert.IsFalse(equals);
            Assert.IsTrue(objectFirst.Equals(objectSecond));
            Assert.AreNotEqual(comparer.GetHashCode(objectFirst), comparer.GetHashCode(objectSecond));
        }

        class TestObject
        {
            public override bool Equals(object obj)
            {
                return true;
            }

            public override int GetHashCode()
            {
                return 1;
            }

            public int GetBaseHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}