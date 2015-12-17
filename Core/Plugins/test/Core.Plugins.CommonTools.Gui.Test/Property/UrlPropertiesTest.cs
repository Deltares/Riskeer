using System;
using Core.Common.Base.Data;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Gui.Property;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Gui.Test.Property
{
    [TestFixture]
    public class UrlPropertiesTest
    {
        [Test]
        public void Name_WithData_SameAsData()
        {
            // Setup
            var someName = "some name";
            var otherName = "some other name";

            var url = new Url(someName, string.Empty);
            var properties = new UrlProperties
            {
                Data = url
            };

            // Call & Assert
            Assert.AreEqual(someName, properties.Name);

            // Call
            properties.Name = otherName;

            // Assert
            Assert.AreEqual(otherName, url.Name);
        }

        [Test]
        public void Path_WithData_ReturnsPath()
        {
            // Setup
            var somePath = "some path";
            var otherPath = "some path";

            var url = new Url(string.Empty, somePath);
            var properties = new UrlProperties
            {
                Data = url
            };

            // Call & Assert
            Assert.AreEqual(otherPath, properties.Path);

            // Call
            properties.Path = otherPath;

            // Assert
            Assert.AreEqual(otherPath, url.Path);
        }

        [Test]
        public void Path_WithData_SameAsData()
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