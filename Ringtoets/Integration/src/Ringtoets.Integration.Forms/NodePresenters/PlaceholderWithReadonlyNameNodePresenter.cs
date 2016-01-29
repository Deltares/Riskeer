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
//using System.Drawing;
//using System.Windows.Forms;
//using Core.Common.Gui;
//using Core.Common.Gui.ContextMenu;
//using Ringtoets.Common.Forms.NodePresenters;
//using Ringtoets.Common.Placeholder;
//using Ringtoets.Integration.Forms.Properties;
//using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
//using TreeNode = Core.Common.Controls.TreeView.TreeNode;
//
//namespace Ringtoets.Integration.Forms.NodePresenters
//{
//    /// <summary>
//    /// Node presenter class for <see cref="PlaceholderWithReadonlyName"/>
//    /// </summary>
//    public class PlaceholderWithReadonlyNameNodePresenter : RingtoetsNodePresenterBase<PlaceholderWithReadonlyName>
//    {
//        /// <summary>
//        /// Creates a new instance of <see cref="PlaceholderWithReadonlyNameNodePresenter"/>, which uses the 
//        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
//        /// </summary>
//        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
//        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
//        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
//        public PlaceholderWithReadonlyNameNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }
//
//        protected override void UpdateNode(TreeNode parentNode, TreeNode node, PlaceholderWithReadonlyName nodeData)
//        {
//            node.Text = nodeData.Name;
//            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
//            node.Image = GetIconForPlaceholder(nodeData);
//        }
//
//        protected override ContextMenuStrip GetContextMenu(TreeNode node, PlaceholderWithReadonlyName nodeData)
//        {
//            IContextMenuBuilder menuBuilder = contextMenuBuilderProvider.Get(node);
//
//            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
//            {
//                menuBuilder.AddOpenItem();
//            }
//            
//            if (nodeData is OutputPlaceholder)
//            {
//                var clearItem = new StrictContextMenuItem(
//                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase,
//                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip,
//                    RingtoetsCommonFormsResources.ClearIcon,
//                    null)
//                {
//                    Enabled = false
//                };
//
//                menuBuilder.AddCustomItem(clearItem);
//            }
//
//            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
//            {
//                menuBuilder.AddSeparator();
//            }
//            return menuBuilder.AddImportItem()
//                              .AddExportItem()
//                              .AddSeparator()
//                              .AddPropertiesItem()
//                              .Build();
//        }
//
//        private static Bitmap GetIconForPlaceholder(PlaceholderWithReadonlyName nodeData)
//        {
//            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
//            {
//                return Resources.GenericInputOutputIcon;
//            }
//            return Resources.PlaceholderIcon;
//        }
//    }
//}