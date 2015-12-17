using Core.Common.Gui.Swf;
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
    }
}