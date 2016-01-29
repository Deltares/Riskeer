// Copyright (C) Stichting Deltares 2016. All rights preserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

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