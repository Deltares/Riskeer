using System.Linq;

using NUnit.Framework;

using Ringtoets.Piping.Forms.Helpers;

namespace Ringtoets.Piping.Forms.Test.Helpers
{
    [TestFixture]
    public class NamingHelperTest
    {
        [Test]
        public void GetUniqueName_EmptyCollection_ReturnNameBase()
        {
            // Setup
            const string nameBase = "The basic name";

            var existingObjects = Enumerable.Empty<ObjectWithName>();

            // Call
            var name = NamingHelper.GetUniqueName(existingObjects, nameBase, namedObject => namedObject.Name);

            // Assert
            Assert.AreEqual(nameBase, name);
        }

        [Test]
        public void GetUniqueName_CollectionWithNamedObjectMatchingNameBase_ReturnNameBaseAppendedWithPostfixIncrement()
        {
            // Setup
            const string nameBase = "The basic name";

            var existingObjects = new[] { new ObjectWithName(nameBase) };

            // Call
            var name = NamingHelper.GetUniqueName(existingObjects, nameBase, namedObject => namedObject.Name);

            // Assert
            Assert.AreEqual(nameBase + " (1)", name);
        }

        [Test]
        public void GetUniqueName_CollectionWithNamedObjectMatchingNameBaseAndPostFix_ReturnNameBaseAppendedWithNextPostfixIncrement()
        {
            // Setup
            const string nameBase = "The basic name";

            var existingObjects = new[]
            {
                new ObjectWithName(nameBase),
                new ObjectWithName(nameBase + " (3)"),
                new ObjectWithName(nameBase + " (1)"),
                new ObjectWithName(nameBase + " (2)"),
            };

            // Call
            var name = NamingHelper.GetUniqueName(existingObjects, nameBase, namedObject => namedObject.Name);

            // Assert
            Assert.AreEqual(nameBase + " (4)", name);
        }

        [Test]
        public void GetUniqueName_CollectionWithNamedObjectNotMatchingNameBase_ReturnNameBase()
        {
            // Setup
            const string nameBase = "The basic name";

            var existingObjects = new[] { new ObjectWithName("Something original!") };

            // Call
            var name = NamingHelper.GetUniqueName(existingObjects, nameBase, namedObject => namedObject.Name);

            // Assert
            Assert.AreEqual(nameBase, name);
        }

        private class ObjectWithName
        {
            public ObjectWithName(string name)
            {
                Name = name;
            }

            public string Name { get; private set; }
        }
    }
}