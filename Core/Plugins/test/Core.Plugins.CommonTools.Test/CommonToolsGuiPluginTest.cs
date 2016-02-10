using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms;
using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Properties;
using Core.Plugins.CommonTools.Property;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Test
{
    [TestFixture]
    public class CommonToolsGuiPluginTest
    {
        [Test]
        public void GetCommonToolsGuiPluginProperties_Always_ReturnProperties()
        {
            // Setup
            var guiPlugin = new CommonToolsGuiPlugin();

            // Call
            var propertyInfos = guiPlugin.GetPropertyInfos().ToList();

            // Assert
            Assert.AreEqual(2, propertyInfos.Count);

            var projectPropertyInfo = propertyInfos.First(pi => pi.DataType == typeof(Project));
            var urlPropertyInfo = propertyInfos.First(pi => pi.DataType == typeof(WebLink));

            Assert.AreEqual(typeof(ProjectProperties), projectPropertyInfo.PropertyObjectType);
            Assert.AreEqual(typeof(WebLinkProperties), urlPropertyInfo.PropertyObjectType);
        }

        [Test]
        public void GetCommonToolsGuiPluginProperties_Always_ReturnViews()
        {
            // Setup
            var guiPlugin = new CommonToolsGuiPlugin();

            // Call
            var viewInfos = guiPlugin.GetViewInfos().ToArray();

            // Assert
            Assert.NotNull(viewInfos);
            Assert.AreEqual(2, viewInfos.Length);

            var richTextFileInfo = viewInfos.First(vi => vi.DataType == typeof(RichTextFile));
            var webLinkInfo = viewInfos.First(vi => vi.DataType == typeof(WebLink));

            Assert.AreEqual(richTextFileInfo.ViewType, typeof(RichTextView));
            Assert.IsNull(richTextFileInfo.Description);
            TestHelper.AssertImagesAreEqual(Common.Gui.Properties.Resources.key, richTextFileInfo.Image);

            Assert.AreEqual(webLinkInfo.ViewType, typeof(HtmlPageView));
            Assert.AreEqual(webLinkInfo.Description, Resources.CommonToolsGuiPlugin_GetViewInfoObjects_Browser);
            TestHelper.AssertImagesAreEqual(Resources.HomeIcon, webLinkInfo.Image);
        }

        [Test]
        public void RichTextFileViewInfoName_WithoutData_EmptyString()
        {
            // Setup
            var guiPlugin = new CommonToolsGuiPlugin();

            var info = guiPlugin.GetViewInfos().First(vi => vi.DataType == typeof(RichTextFile));
            
            // Call
            var name = info.GetViewName(null, null);

            // Assert
            Assert.IsEmpty(name);
        }

        [Test]
        public void RichTextFileViewInfoName_WithData_NameOfData()
        {
            // Setup
            var expected = "SomeName";
            var guiPlugin = new CommonToolsGuiPlugin();

            var info = guiPlugin.GetViewInfos().First(vi => vi.DataType == typeof(RichTextFile));
            var richTextFile = new RichTextFile
            {
                Name = expected
            };
            
            // Call
            var name = info.GetViewName(null, richTextFile);

            // Assert
            Assert.AreEqual(expected, name);
        }

        [Test]
        public void HtmlPageViewInfoName_WithoutData_EmptyString()
        {
            // Setup
            var guiPlugin = new CommonToolsGuiPlugin();

            var info = guiPlugin.GetViewInfos().First(vi => vi.DataType == typeof(WebLink));

            // Call
            var name = info.GetViewName(null, null);

            // Assert
            Assert.IsEmpty(name);
        }

        [Test]
        public void HtmlPageViewInfoName_WithData_NameOfData()
        {
            // Setup
            var expected = "SomeName";
            var guiPlugin = new CommonToolsGuiPlugin();

            var info = guiPlugin.GetViewInfos().First(vi => vi.DataType == typeof(WebLink));
            var webLink = new WebLink(expected, null);

            // Call
            var name = info.GetViewName(null, webLink);

            // Assert
            Assert.AreEqual(expected, name);
        }
    }
}