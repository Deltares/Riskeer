using System.Collections.Generic;

using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;

using Mono.Addins;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Plugin
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

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new WtiRibbon();
            }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo
            {
                ObjectType = typeof(WtiProject), PropertyType = typeof(WtiProjectProperties)
            };
            yield return new PropertyInfo
            {
                ObjectType = typeof(PipingCalculationInputs), PropertyType = typeof(PipingCalculationInputsProperties)
            };
            yield return new PropertyInfo
            {
                ObjectType = typeof(PipingOutput), PropertyType = typeof(PipingOutputProperties)
            };
            yield return new PropertyInfo
            {
                ObjectType = typeof(RingtoetsPipingSurfaceLine), PropertyType = typeof(PipingSurfaceLineProperties)
            };
            yield return new PropertyInfo<PipingSoilProfile, PipingSoilProfileProperties>();
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new WtiProjectNodePresenter();
            yield return new PipingCalculationInputsNodePresenter();
            yield return new PipingFailureMechanismNodePresenter();
            yield return new PipingSurfaceLineCollectionNodePresenter
            {
                ImportSurfaceLinesAction = Gui.CommandHandler.ImportToGuiSelection
            };
            yield return new PipingSurfaceLineNodePresenter();
            yield return new PipingSoilProfileCollectionNodePresenter
            {
                ImportSoilProfilesAction = Gui.CommandHandler.ImportToGuiSelection
            };
            yield return new PipingSoilProfileNodePresenter();
            yield return new PipingOutputNodePresenter();
        }
    }
}