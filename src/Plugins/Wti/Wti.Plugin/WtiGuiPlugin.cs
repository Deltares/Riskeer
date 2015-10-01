using System.Collections.Generic;

using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;

using Mono.Addins;
using Wti.Controller;
using Wti.Data;
using Wti.Forms.NodePresenters;
using Wti.Forms.PropertyClasses;

namespace Wti.Plugin
{
    [Extension(typeof(IPlugin))]
    public class WtiGuiPlugin : GuiPlugin
    {
        public override string Name
        {
            get
            {
                return Properties.Resources.WtiApplicationGuiName;
            }
        }

        public override string DisplayName
        {
            get
            {
                return Properties.Resources.wtiGuiPluginDisplayName;
            }
        }

        public override string Description
        {
            get
            {
                return Properties.Resources.wtiGuiPluginDescription;
            }
        }

        public override string Version
        {
            get
            {
                return "0.5.0.0";
            }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo{ ObjectType = typeof(WtiProject), PropertyType = typeof(WtiProjectProperties)};
            yield return new PropertyInfo{ ObjectType = typeof(PipingData), PropertyType = typeof(PipingDataProperties)};
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new WtiProjectNodePresenter();
            yield return new PipingDataNodeController().NodePresenter;
        }

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new WtiRibbon();
            }
        }
    }
}