using System.Collections.Generic;

using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Forms;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.NodePresenters;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Plugin
{
    /// <summary>
    /// The GUI plugin for the Ringtoets application.
    /// </summary>
    public class RingtoetsGuiPlugin : GuiPlugin
    {
        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new RingtoetsRibbon();
            }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<AssessmentSectionBase, AssessmentSectionBaseProperties>();
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new AssessmentSectionBaseNodePresenter();
            yield return new FailureMechanismNodePresenter((IContextMenuProvider)Gui, Gui.CommandHandler);
            yield return new PlaceholderWithReadonlyNameNodePresenter((IContextMenuProvider)Gui, Gui.CommandHandler);
            yield return new CategoryTreeFolderNodePresenter();
        }
    }
}