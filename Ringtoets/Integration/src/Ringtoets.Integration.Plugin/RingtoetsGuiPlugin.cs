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
            yield return new PropertyInfo<DikeAssessmentSection, AssessmentSectionProperties>();
            foreach (var propertyInfo in new PipingGuiPlugin().GetPropertyInfos())
            {
                yield return propertyInfo;
            }
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new AssessmentSectionNodePresenter();
            foreach (var pipingNodePresenter in new PipingGuiPlugin { Gui = Gui }.GetProjectTreeViewNodePresenters())
            {
                yield return pipingNodePresenter;
            }
        }
    }
}