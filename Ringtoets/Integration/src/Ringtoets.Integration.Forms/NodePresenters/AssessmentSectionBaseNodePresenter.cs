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

//using System;
//using System.Collections;
//using System.Drawing;
//using System.Windows.Forms;
//using Core.Common.Base.Data;
//using Core.Common.Gui;
//using Ringtoets.Common.Forms.NodePresenters;
//using Ringtoets.Integration.Data;
//using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
//using TreeNode = Core.Common.Controls.TreeView.TreeNode;
//
//namespace Ringtoets.Integration.Forms.NodePresenters
//{
//    /// <summary>
//    /// Node presenter for <see cref="Ringtoets.Integration.Data.AssessmentSectionBase"/> items in the tree view.
//    /// </summary>
//    public class AssessmentSectionBaseNodePresenter : RingtoetsNodePresenterBase<AssessmentSectionBase>
//    {
//        /// <summary>
//        /// Creates a new instance of <see cref="AssessmentSectionBaseNodePresenter"/>, which uses the 
//        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
//        /// </summary>
//        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
//        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
//        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
//        public AssessmentSectionBaseNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }
//
//        public override bool CanRenameNode(TreeNode node)
//        {
//            return true;
//        }
//
//        public override bool CanRenameNodeTo(TreeNode node, string newName)
//        {
//            return true;
//        }
//
//        protected override void UpdateNode(TreeNode parentNode, TreeNode node, AssessmentSectionBase nodeData)
//        {
//            node.Text = nodeData.Name;
//            node.Image = RingtoetsFormsResources.AssessmentSectionFolderIcon;
//            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
//        }
//
//        protected override IEnumerable GetChildNodeObjects(AssessmentSectionBase nodeData)
//        {
//            yield return nodeData.ReferenceLine;
//            yield return nodeData.FailureMechanismContribution;
//            yield return nodeData.HydraulicBoundaryDatabase;
//            foreach (var failureMechanism in nodeData.GetFailureMechanisms())
//            {
//                yield return failureMechanism;
//            }
//        }
//
//        protected override void OnNodeRenamed(AssessmentSectionBase nodeData, string newName)
//        {
//            nodeData.Name = newName;
//            nodeData.NotifyObservers();
//        }
//
//        protected override bool CanRemove(object parentNodeData, AssessmentSectionBase nodeData)
//        {
//            return true;
//        }
//
//        protected override bool RemoveNodeData(object parentNodeData, AssessmentSectionBase nodeData)
//        {
//            var parentProject = (Project) parentNodeData;
//
//            parentProject.Items.Remove(nodeData);
//            parentProject.NotifyObservers();
//
//            return true;
//        }
//
//        protected override ContextMenuStrip GetContextMenu(TreeNode node, AssessmentSectionBase nodeData)
//        {
//            return contextMenuBuilderProvider
//                .Get(node)
//                .AddRenameItem()
//                .AddDeleteItem()
//                .AddSeparator()
//                .AddImportItem()
//                .AddExportItem()
//                .AddSeparator()
//                .AddExpandAllItem()
//                .AddCollapseAllItem()
//                .AddSeparator()
//                .AddPropertiesItem()
//                .Build();
//        }
//    }
//}