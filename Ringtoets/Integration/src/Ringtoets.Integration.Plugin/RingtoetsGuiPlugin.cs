using System.Collections.Generic;

using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Forms;

using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.NodePresenters;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Piping.Plugin;

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
                return new PipingRibbon();
            }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<DikeAssessmentSection, AssessmentSectionProperties>();
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new AssessmentSectionNodePresenter();
        }
    }
}