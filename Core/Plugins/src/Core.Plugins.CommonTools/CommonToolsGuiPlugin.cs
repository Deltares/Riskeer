using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Swf;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Properties;
using Core.Plugins.CommonTools.Property;
using PropertyInfo = Core.Common.Gui.PropertyInfo;

namespace Core.Plugins.CommonTools
{
    public class CommonToolsGuiPlugin : GuiPlugin
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<Url, UrlProperties>();
            yield return new PropertyInfo<Project, ProjectProperties>();
            yield return new PropertyInfo<TreeFolder, TreeFolderProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<RichTextFile, RichTextView>
            {
                Image = Common.Gui.Properties.Resources.key,
                GetViewName = (v, o) => o != null ? o.Name : ""
            };
            yield return new ViewInfo<Url, HtmlPageView>
            {
                Image = Resources.home,
                Description = Resources.CommonToolsGuiPlugin_GetViewInfoObjects_Browser,
                GetViewName = (v, o) => o != null ? o.Name : ""
            };
        }
    }
}