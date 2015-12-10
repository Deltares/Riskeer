using System.Windows.Forms;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.NodePresenters
{
    public class FailureMechanismContributionNodePresenter : RingtoetsNodePresenterBase<FailureMechanismContribution>
    {
        public FailureMechanismContributionNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) {}

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, FailureMechanismContribution nodeData)
        {
            node.Text = Data.Properties.Resources.FailureMechanismContribution_DisplayName;
            node.Image = Resources.GenericInputOutputIcon;
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode node, FailureMechanismContribution nodeData)
        {
            return contextMenuBuilderProvider
                .Get(node)
                .AddOpenItem()
                .Build();
        }
    }
}