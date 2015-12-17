using Core.Common.Base.Data;
using Core.Plugins.CommonTools.Gui.Property;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Gui.Test.Property
{
    [TestFixture]
    public class ProjectPropertiesTest
    {
        [Test]
        public void Name_WithData_ReturnsName()
        {
            // Setup
            var project = new Project();
            var properties = new ProjectProperties
            {
                Data = project
            };

            var testName = "some name";
            project.Name = testName;

            // Call
            var result = properties.Name;

            // Assert
            Assert.AreEqual(testName, result);
        }

        [Test]
        public void Description_WithData_SameAsData()
        {
            // Setup
            var project = new Project();
            var properties = new ProjectProperties
            {
                Data = project
            };

            var testDescription = "some description";
            var anotherDescription = "another description";

            project.Description = testDescription;

            // Call
            var result = properties.Description;

            // Assert
            Assert.AreEqual(testDescription, result);

            // Call
            properties.Description = anotherDescription;

            // Assert
            Assert.AreEqual(anotherDescription, project.Description);
        }
    }
}