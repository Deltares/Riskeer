using System;
using System.ComponentModel;
using Core.Common.Gui.Swf;
using Core.Common.Utils.PropertyBag;
using Core.Plugins.CommonTools.Gui.Property;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Gui.Test.Property
{
    [TestFixture]
    public class TreeFolderPropertiesTest
    {
        [Test]
        public void Text_WithData_ReturnsText()
        {
            // Setup
            var someText = "some name";

            var folder = new TreeFolder(null, null, someText, FolderImageType.None);
            var properties = new TreeFolderProperties
            {
                Data = folder
            };

            // Call
            var result = properties.Text;

            // Assert
            Assert.AreEqual(someText, result);
        }

        [Test]
        public void GetProperties_Always_ReturnsOneProperty()
        {
            // Setup
            var data = new TreeFolder(null, null, string.Empty, FolderImageType.None);

            var bag = new DynamicPropertyBag(new TreeFolderProperties
            {
                Data = data
            });

            // Call
            var properties = bag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(1, properties.Count);
        }
    }
}