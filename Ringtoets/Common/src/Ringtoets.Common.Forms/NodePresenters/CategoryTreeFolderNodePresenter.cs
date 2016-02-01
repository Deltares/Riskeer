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
//using Ringtoets.Common.Forms.PresentationObjects;
//using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
//using TreeNode = Core.Common.Controls.TreeView.TreeNode;
//
//namespace Ringtoets.Common.Forms.NodePresenters
//{
//    /// <summary>
//    /// Node presenter for <see cref="CategoryTreeFolder"/>.
//    /// </summary>
//    public class CategoryTreeFolderNodePresenter : RingtoetsNodePresenterBase<CategoryTreeFolder>
//    {
//        /// <summary>
//        /// Creates a new instance of <see cref="CategoryTreeFolderNodePresenter"/>, which uses the 
//        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
//        /// </summary>
//        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
//        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
//        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
//        public CategoryTreeFolderNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }
//
//        protected override void UpdateNode(TreeNode parentNode, TreeNode node, CategoryTreeFolder nodeData)
//        {
//            node.Text = nodeData.Name;
//            node.Image = GetFolderIcon(nodeData.Category);
//            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
//        }
//
//        protected override IEnumerable GetChildNodeObjects(CategoryTreeFolder nodeData)
//        {
//            return nodeData.Contents;
//        }
//
//        protected override ContextMenuStrip GetContextMenu(TreeNode node, CategoryTreeFolder nodeData)
//        {
//            return contextMenuBuilderProvider
//                .Get(node)
//                .AddExpandAllItem()
//                .AddCollapseAllItem()
//                .Build();
//        }
//
//        private Image GetFolderIcon(TreeFolderCategory category)
//        {
//            switch (category)
//            {
//                case TreeFolderCategory.General:
//                    return RingtoetsCommonFormsResources.GeneralFolderIcon;
//                case TreeFolderCategory.Input:
//                    return RingtoetsCommonFormsResources.InputFolderIcon;
//                case TreeFolderCategory.Output:
//                    return RingtoetsCommonFormsResources.OutputFolderIcon;
//                default:
//                    throw new NotImplementedException();
//            }
//        }
//    }
//}