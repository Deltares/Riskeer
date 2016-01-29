//using System.Drawing;
//using System.Windows.Forms;
//using Core.Common.Gui;
//using Ringtoets.Common.Forms.NodePresenters;
//using Ringtoets.Integration.Data.Contribution;
//using Ringtoets.Integration.Forms.Properties;
//using TreeNode = Core.Common.Controls.TreeView.TreeNode;
//
//namespace Ringtoets.Integration.Forms.NodePresenters
//{
//    public class FailureMechanismContributionNodePresenter : RingtoetsNodePresenterBase<FailureMechanismContribution>
//    {
//        public FailureMechanismContributionNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) {}
//
//        protected override void UpdateNode(TreeNode parentNode, TreeNode node, FailureMechanismContribution nodeData)
//        {
//            node.Text = Data.Properties.Resources.FailureMechanismContribution_DisplayName;
//            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
//            node.Image = Resources.GenericInputOutputIcon;
//        }
//
//        protected override ContextMenuStrip GetContextMenu(TreeNode node, FailureMechanismContribution nodeData)
//        {
//            return contextMenuBuilderProvider
//                .Get(node)
//                .AddOpenItem()
//                .AddSeparator()
//                .AddExportItem()
//                .Build();
//        }
//    }
//}