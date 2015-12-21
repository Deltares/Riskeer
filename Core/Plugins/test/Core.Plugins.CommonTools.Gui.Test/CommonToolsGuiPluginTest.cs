using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.Swf;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Swf;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Gui.Property;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Gui.Test
{
    [TestFixture]
    public class CommonToolsGuiPluginTest
    {
        [Test]
        public void TestGetObjectProperties()
        {
            var guiPlugin = new CommonToolsGuiPlugin();
            var propertyInfos = guiPlugin.GetPropertyInfos().ToList();

            Assert.AreEqual(3, propertyInfos.Count);

            var projectPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(Project));
            var urlPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(Url));
            var treePropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(TreeFolder));

            Assert.AreEqual(typeof(ProjectProperties), projectPropertyInfo.PropertyType);
            Assert.AreEqual(typeof(UrlProperties), urlPropertyInfo.PropertyType);
            Assert.AreEqual(typeof(TreeFolderProperties), treePropertyInfo.PropertyType);
        }

        [Test]
        public void TestGetViewInfoObjectsContent()
        {
            var guiPlugin = new CommonToolsGuiPlugin();
            var viewInfos = guiPlugin.GetViewInfoObjects().ToList();

            Assert.NotNull(viewInfos);
            Assert.AreEqual(2, viewInfos.Count);
            Assert.IsTrue(viewInfos.Any(vi => vi.DataType == typeof(RichTextFile) && vi.ViewType == typeof(RichTextView)));
            Assert.IsTrue(viewInfos.Any(vi => vi.DataType == typeof(Url) && vi.ViewType == typeof(HtmlPageView)));
        }
    }
}