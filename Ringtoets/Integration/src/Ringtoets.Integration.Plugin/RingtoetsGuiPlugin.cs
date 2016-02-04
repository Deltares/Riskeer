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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Placeholder;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using RingtoetsDataResources = Ringtoets.Integration.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

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

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<AssessmentSectionBase>
            {
                Text = assessmentSectionBase => assessmentSectionBase.Name,
                Image = assessmentSectionBase => RingtoetsFormsResources.AssessmentSectionFolderIcon,
                EnsureVisibleOnCreate = assessmentSectionBase => true,
                ChildNodeObjects = AssessmentSectionBaseChildNodeObjects,
                ContextMenuStrip = AssessmentSectionBaseContextMenuStrip,
                CanRename = (assessmentSectionBase, parentData) => true,
                OnNodeRenamed = AssessmentSectionBaseOnNodeRenamed,
                CanRemove = (assessmentSectionBase, parentNodeData) => true,
                OnNodeRemoved = AssessmentSectionBaseOnNodeRemoved
            };

            yield return new TreeNodeInfo<FailureMechanismPlaceholder>
            {
                Text = failureMechanismPlaceholder => failureMechanismPlaceholder.Name,
                Image = failureMechanismPlaceholder => RingtoetsFormsResources.FailureMechanismIcon,
                ForeColor = failureMechanismPlaceholder => Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = FailureMechanismPlaceholderChildNodeObjects,
                ContextMenuStrip = FailureMechanismPlaceholderContextMenuStrip
            };

            yield return new TreeNodeInfo<PlaceholderWithReadonlyName>
            {
                Text = placeholderWithReadonlyName => placeholderWithReadonlyName.Name,
                Image = placeholderWithReadonlyName => GetIconForPlaceholder(placeholderWithReadonlyName),
                ForeColor = placeholderWithReadonlyName => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = PlaceholderWithReadonlyNameContextMenuStrip
            };

            yield return new TreeNodeInfo<CategoryTreeFolder>
            {
                Text = categoryTreeFolder => categoryTreeFolder.Name,
                Image = categoryTreeFolder => GetFolderIcon(categoryTreeFolder.Category),
                ChildNodeObjects = categoryTreeFolder => categoryTreeFolder.Contents.Cast<object>().ToArray(),
                ContextMenuStrip = CategoryTreeFolderContextMenu
            };

            yield return new TreeNodeInfo<FailureMechanismContribution>
            {
                Text = failureMechanismContribution => RingtoetsDataResources.FailureMechanismContribution_DisplayName,
                Image = failureMechanismContribution => RingtoetsFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (failureMechanismContribution, parentData, treeViewControl) => Gui.Get(failureMechanismContribution, treeViewControl)
                                                                                                     .AddOpenItem()
                                                                                                     .AddSeparator()
                                                                                                     .AddExportItem()
                                                                                                     .Build()
            };
        }

        # region AssessmentSectionBase

        private object[] AssessmentSectionBaseChildNodeObjects(AssessmentSectionBase nodeData)
        {
            var childNodes = new List<object>
            {
                nodeData.ReferenceLine,
                nodeData.FailureMechanismContribution,
                nodeData.HydraulicBoundaryDatabase
            };

            childNodes.AddRange(nodeData.GetFailureMechanisms());

            return childNodes.ToArray();
        }

        private void AssessmentSectionBaseOnNodeRenamed(AssessmentSectionBase nodeData, string newName)
        {
            nodeData.Name = newName;
            nodeData.NotifyObservers();
        }

        private void AssessmentSectionBaseOnNodeRemoved(AssessmentSectionBase nodeData, object parentNodeData)
        {
            var parentProject = (Project) parentNodeData;

            parentProject.Items.Remove(nodeData);
            parentProject.NotifyObservers();
        }

        private ContextMenuStrip AssessmentSectionBaseContextMenuStrip(AssessmentSectionBase nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddRenameItem()
                      .AddDeleteItem()
                      .AddSeparator()
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        # endregion

        # region FailureMechanismPlaceholder

        private object[] FailureMechanismPlaceholderChildNodeObjects(FailureMechanismPlaceholder nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetInputs(nodeData),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetOutputs(nodeData),
                                       TreeFolderCategory.Output)
            };
        }

        private IList GetInputs(FailureMechanismPlaceholder nodeData)
        {
            return new ArrayList
            {
                nodeData.SectionDivisions,
                nodeData.Locations,
                nodeData.BoundaryConditions
            };
        }

        private IList GetOutputs(FailureMechanismPlaceholder nodeData)
        {
            return new ArrayList
            {
                nodeData.AssessmentResult
            };
        }

        private ContextMenuStrip FailureMechanismPlaceholderContextMenuStrip(FailureMechanismPlaceholder nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var calculateItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                RingtoetsCommonFormsResources.Calculate_all_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                null)
            {
                Enabled = false
            };
            var clearOutputItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Clear_all_output,
                RingtoetsCommonFormsResources.Clear_all_output_ToolTip,
                RingtoetsCommonFormsResources.ClearIcon, null
                )
            {
                Enabled = false
            };

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(calculateItem)
                      .AddCustomItem(clearOutputItem)
                      .AddSeparator()
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        # endregion

        # region PlaceholderWithReadonlyName

        private static Bitmap GetIconForPlaceholder(PlaceholderWithReadonlyName nodeData)
        {
            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                return RingtoetsFormsResources.GenericInputOutputIcon;
            }
            return RingtoetsFormsResources.PlaceholderIcon;
        }

        private ContextMenuStrip PlaceholderWithReadonlyNameContextMenuStrip(PlaceholderWithReadonlyName nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IContextMenuBuilder menuBuilder = Gui.Get(nodeData, treeViewControl);

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                menuBuilder.AddOpenItem();
            }

            if (nodeData is OutputPlaceholder)
            {
                var clearItem = new StrictContextMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase,
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip,
                    RingtoetsCommonFormsResources.ClearIcon,
                    null)
                {
                    Enabled = false
                };

                menuBuilder.AddCustomItem(clearItem);
            }

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                menuBuilder.AddSeparator();
            }
            return menuBuilder.AddImportItem()
                              .AddExportItem()
                              .AddSeparator()
                              .AddPropertiesItem()
                              .Build();
        }

        # endregion

        # region CategoryTreeFolder

        private Image GetFolderIcon(TreeFolderCategory category)
        {
            switch (category)
            {
                case TreeFolderCategory.General:
                    return RingtoetsCommonFormsResources.GeneralFolderIcon;
                case TreeFolderCategory.Input:
                    return RingtoetsCommonFormsResources.InputFolderIcon;
                case TreeFolderCategory.Output:
                    return RingtoetsCommonFormsResources.OutputFolderIcon;
                default:
                    throw new NotImplementedException();
            }
        }

        private ContextMenuStrip CategoryTreeFolderContextMenu(CategoryTreeFolder nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .Build();
        }

        # endregion
    }
}