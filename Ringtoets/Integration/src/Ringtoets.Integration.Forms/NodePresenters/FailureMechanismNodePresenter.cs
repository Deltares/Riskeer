// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// All rights reserved.

//using System;
//using System.Collections;
//using System.Drawing;
//using System.Windows.Forms;
//using Core.Common.Gui;
//using Core.Common.Gui.ContextMenu;
//using Ringtoets.Common.Forms.NodePresenters;
//using Ringtoets.Common.Forms.PresentationObjects;
//using Ringtoets.Integration.Data.Placeholders;
//using Ringtoets.Integration.Forms.Properties;
//
//using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
//using TreeNode = Core.Common.Controls.TreeView.TreeNode;
//
//namespace Ringtoets.Integration.Forms.NodePresenters
//{
//    public class FailureMechanismNodePresenter : RingtoetsNodePresenterBase<FailureMechanismPlaceholder>
//    {
//        /// <summary>
//        /// Creates a new instance of <see cref="FailureMechanismNodePresenter"/>, which uses the 
//        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
//        /// </summary>
//        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
//        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
//        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
//        public FailureMechanismNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }
//
//        protected override void UpdateNode(TreeNode parentNode, TreeNode node, FailureMechanismPlaceholder nodeData)
//        {
//            node.Text = nodeData.Name;
//            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
//            node.Image = Resources.FailureMechanismIcon;
//        }
//
//        protected override IEnumerable GetChildNodeObjects(FailureMechanismPlaceholder nodeData)
//        {
//            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(nodeData), TreeFolderCategory.Input);
//            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(nodeData), TreeFolderCategory.Output);
//        }
//
//        protected override ContextMenuStrip GetContextMenu(TreeNode node, FailureMechanismPlaceholder nodeData)
//        {
//            var calculateItem = new StrictContextMenuItem(
//                RingtoetsCommonFormsResources.Calculate_all,
//                RingtoetsCommonFormsResources.Calculate_all_ToolTip,
//                RingtoetsCommonFormsResources.CalculateAllIcon,
//                null)
//            {
//                Enabled = false
//            };
//            var clearOutputItem = new StrictContextMenuItem(
//                RingtoetsCommonFormsResources.Clear_all_output,
//                RingtoetsCommonFormsResources.Clear_all_output_ToolTip,
//                RingtoetsCommonFormsResources.ClearIcon, null
//                )
//            {
//                Enabled = false
//            };
//
//            return contextMenuBuilderProvider.Get(node)
//                                             .AddCustomItem(calculateItem)
//                                             .AddCustomItem(clearOutputItem)
//                                             .AddSeparator()
//                                             .AddImportItem()
//                                             .AddExportItem()
//                                             .AddSeparator()
//                                             .AddExpandAllItem()
//                                             .AddCollapseAllItem()
//                                             .AddSeparator()
//                                             .AddPropertiesItem()
//                                             .Build();
//        }
//
//        private IEnumerable GetInputs(FailureMechanismPlaceholder nodeData)
//        {
//            yield return nodeData.SectionDivisions;
//            yield return nodeData.Locations;
//            yield return nodeData.BoundaryConditions;
//        }
//
//        private IEnumerable GetOutputs(FailureMechanismPlaceholder nodeData)
//        {
//            yield return nodeData.AssessmentResult;
//        }
//    }
//}