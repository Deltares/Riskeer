using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms;
using Core.Common.Utils;
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
            var guiPlugin = new CommonToolsGuiPlugin();
            var propertyInfos = guiPlugin.GetPropertyInfos().ToList();

            Assert.AreEqual(2, propertyInfos.Count);

            var projectPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(Project));
            var urlPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(Url));

            Assert.AreEqual(typeof(ProjectProperties), projectPropertyInfo.PropertyType);
            Assert.AreEqual(typeof(UrlProperties), urlPropertyInfo.PropertyType);
        }

        [Test]
        public void GetCommonToolsGuiPluginProperties_Always_ReturnViews()
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