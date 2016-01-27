using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Placeholder;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Forms.NodePresenters;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using RingtoetsDataResources = Ringtoets.Integration.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

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
        /// <exception cref="ArgumentNullException">Thrown when <see cref="IContextMenuBuilderProvider"/> is <c>null</c>.</exception>
        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new AssessmentSectionBaseNodePresenter(Gui);
            yield return new FailureMechanismNodePresenter(Gui);
            yield return new PlaceholderWithReadonlyNameNodePresenter(Gui);
            yield return new CategoryTreeFolderNodePresenter(Gui);
            yield return new FailureMechanismContributionNodePresenter(Gui);
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<AssessmentSectionBase>
            {
                Text = assessmentSectionBase => assessmentSectionBase.Name,
                Image = assessmentSectionBase => RingtoetsFormsResources.AssessmentSectionFolderIcon,
                ChildNodeObjects = AssessmentSectionBaseChildNodeObjects,
                ContextMenu = AssessmentSectionBaseContextMenu,
                CanRename = assessmentSectionBase => true,
                OnNodeRenamed = AssessmentSectionBaseOnNodeRenamed,
                CanRemove = (assessmentSectionBase, parentNodeData) => true,
                OnNodeRemoved = AssessmentSectionBaseOnNodeRemoved
            };

            yield return new TreeNodeInfo<FailureMechanismPlaceholder>
            {
                Text = failureMechanismPlaceholder => failureMechanismPlaceholder.Name,
                Image = failureMechanismPlaceholder => RingtoetsFormsResources.FailureMechanismIcon,
                ForegroundColor = failureMechanismPlaceholder => Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = FailureMechanismPlaceholderChildNodeObjects,
                ContextMenu = FailureMechanismPlaceholderContextMenu
            };

            yield return new TreeNodeInfo<PlaceholderWithReadonlyName>
            {
                Text = placeholderWithReadonlyName => placeholderWithReadonlyName.Name,
                Image = placeholderWithReadonlyName => GetIconForPlaceholder(placeholderWithReadonlyName),
                ForegroundColor = placeholderWithReadonlyName => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenu = PlaceholderWithReadonlyNameContextMenu
            };

            yield return new TreeNodeInfo<CategoryTreeFolder>
            {
                Text = categoryTreeFolder => categoryTreeFolder.Name,
                Image = categoryTreeFolder => GetFolderIcon(categoryTreeFolder.Category),
                ChildNodeObjects = categoryTreeFolder => categoryTreeFolder.Contents.Cast<object>().ToArray(),
                ContextMenu = CategoryTreeFolderContextMenu
            };

            yield return new TreeNodeInfo<FailureMechanismContribution>
            {
                Text = failureMechanismContribution => RingtoetsDataResources.FailureMechanismContribution_DisplayName,
                Image = failureMechanismContribution => RingtoetsFormsResources.GenericInputOutputIcon,
                ContextMenu = (failureMechanismContribution, sourceNode) => Gui.Get(sourceNode)
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

        private ContextMenuStrip AssessmentSectionBaseContextMenu(AssessmentSectionBase nodeData, TreeNode node)
        {
            return Gui.Get(node)
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

        private IEnumerable GetInputs(FailureMechanismPlaceholder nodeData)
        {
            yield return nodeData.SectionDivisions;
            yield return nodeData.Locations;
            yield return nodeData.BoundaryConditions;
        }

        private IEnumerable GetOutputs(FailureMechanismPlaceholder nodeData)
        {
            yield return nodeData.AssessmentResult;
        }

        private ContextMenuStrip FailureMechanismPlaceholderContextMenu(FailureMechanismPlaceholder nodeData, TreeNode node)
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

            return Gui.Get(node)
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

        private ContextMenuStrip PlaceholderWithReadonlyNameContextMenu(PlaceholderWithReadonlyName nodeData, TreeNode node)
        {
            IContextMenuBuilder menuBuilder = Gui.Get(node);

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

        private ContextMenuStrip CategoryTreeFolderContextMenu(CategoryTreeFolder nodeData, TreeNode node)
        {
            return Gui.Get(node)
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .Build();
        }

        # endregion
    }
}