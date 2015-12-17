using System;
using System.Collections.Generic;

using Core.Common.Controls;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Forms;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Forms.NodePresenters;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;

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

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<FailureMechanismContribution, FailureMechanismContributionView>
            {
                GetViewName = (v,o) => Resources.FailureMechanismContribution_DisplayName,
                Image = Forms.Properties.Resources.GenericInputOutputIcon,
                CloseForData = (v, o) =>
                {
                    var assessmentSection = o as AssessmentSectionBase;
                    return assessmentSection != null && assessmentSection.FailureMechanismContribution == v.Data;
                }
            };
        }

        public override IEnumerable<object> GetChildDataWithViewDefinitions(object dataObject)
        {
            var assessmentSection = dataObject as AssessmentSectionBase;
            if (assessmentSection != null)
            {
                yield return assessmentSection.FailureMechanismContribution;
            }
        }

        /// <summary>
        /// Get the <see cref="ITreeNodePresenter"/> defined for the <see cref="RingtoetsGuiPlugin"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ITreeNodePresenter"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="IGui.ContextMenuProvider"/> is <c>null</c>.</exception>
        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new AssessmentSectionBaseNodePresenter(Gui.ContextMenuProvider);
            yield return new FailureMechanismNodePresenter(Gui.ContextMenuProvider);
            yield return new PlaceholderWithReadonlyNameNodePresenter(Gui.ContextMenuProvider);
            yield return new CategoryTreeFolderNodePresenter(Gui.ContextMenuProvider);
            yield return new FailureMechanismContributionNodePresenter(Gui.ContextMenuProvider);
        }
    }
}