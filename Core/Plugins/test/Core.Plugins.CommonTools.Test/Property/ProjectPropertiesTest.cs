using System;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Utils.PropertyBag;
using Core.Plugins.CommonTools.Property;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Test.Property
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
        public void Description_WithData_ReturnsDescription()
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

        [Test]
        public void GetProperties_Always_ReturnsTwoProperties()
        {
            // Setup
            var data = new Project();

            var bag = new DynamicPropertyBag(new ProjectProperties
            {
                Data = data
            });

            // Call
            var properties = bag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(2, properties.Count);
        }
    }
}