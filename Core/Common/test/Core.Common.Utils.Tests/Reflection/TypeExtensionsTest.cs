using System.ComponentModel;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using SharpTestsEx;

namespace Core.Common.Utils.Tests.Reflection
{
    [TestFixture]
    public class TypeExtensionsTest
    {
        [Test]
        public void GetDisplayName()
        {
            typeof(Person).GetDisplayName()
                          .Should().Be.EqualTo("person");
        }

        [DisplayName(@"person")]
        public class Person {}
    }
}