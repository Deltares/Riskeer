using System.ComponentModel;
using Core.Common.Utils.Reflection;
using NUnit.Framework;

namespace Core.Common.Utils.Tests.Reflection
{
    [TestFixture]
    public class TypeExtensionsTest
    {
        [Test]
        public void GetDisplayName()
        {
            Assert.AreEqual("person", typeof(Person).GetDisplayName());
        }

        [DisplayName(@"person")]
        public class Person {}
    }
}