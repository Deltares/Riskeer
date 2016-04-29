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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
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
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Plugin.Properties;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;
using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin
{
    /// <summary>
    /// The GUI plug-in for the <see cref="GrassCoverErosionInwards.Data.GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsGuiPlugin : GuiPlugin
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanismContextProperties>();
            yield return new PropertyInfo<GrassCoverErosionInwardsCalculationContext, GrassCoverErosionInwardsCalculationContextProperties>();
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new DefaultFailureMechanismTreeNodeInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanism>(
                FailureMechanismChildNodeObjects,
                FailureMechanismContextMenuStrip,
                Gui);

            yield return new TreeNodeInfo<GrassCoverErosionInwardsCalculationGroupContext>
            {
                Text = grassCoverErosionInwardsFailureMechanismContext => grassCoverErosionInwardsFailureMechanismContext.WrappedData.Name,
                Image = grassCoverErosionInwardsCalculationGroupContext => RingtoetsCommonFormsResources.GeneralFolderIcon,
                EnsureVisibleOnCreate = grassCoverErosionInwardsCalculationGroupContext => true,
                ChildNodeObjects = CalculationGroupContextChildNodeObjects,
                ContextMenuStrip = CalculationGroupContextContextMenuStrip,
                CanRename = CalculationGroupContextCanRename,
                OnNodeRenamed = CalculationGroupContextOnNodeRenamed,
                CanRemove = CalculationGroupContextCanRemove,
                OnNodeRemoved = CalculationGroupContextOnNodeRemoved,
                CanDrag = CalculationGroupContextCanDrag,
                CanDrop = CalculationGroupContextCanDropOrCanInsert,
                CanInsert = CalculationGroupContextCanDropOrCanInsert,
                OnDrop = CalculationGroupContextOnDrop
            };

            yield return new TreeNodeInfo<GrassCoverErosionInwardsCalculationContext>
            {
                Text = context => context.WrappedData.Name,
                Image = context => GrassCoverErosionInwardsFormsResources.CalculationIcon,
                EnsureVisibleOnCreate = context => true,
                ChildNodeObjects = CalculationContextChildNodeObjects
            };

            yield return new TreeNodeInfo<GrassCoverErosionInwardsInputContext>
            {
                Text = pipingInputContext => GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsInputContext_NodeDisplayName,
                Image = pipingInputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyGrassCoverErosionInwardsOutput>
            {
                Text = emptyPipingOutput => GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ForeColor = emptyPipingOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        private static object[] CalculationContextChildNodeObjects(GrassCoverErosionInwardsCalculationContext calculationContext)
        {
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(calculationContext.WrappedData),
                new GrassCoverErosionInwardsInputContext(calculationContext.WrappedData.InputParameters,
                                                         calculationContext.WrappedData,
                                                         calculationContext.GrassCoverErosionInwardsFailureMechanism,
                                                         calculationContext.AssessmentSection)
            };

            if (!calculationContext.WrappedData.HasOutput)
            {
                childNodes.Add(new EmptyGrassCoverErosionInwardsOutput());
            }

            return childNodes.ToArray();
        }

        private static ExceedanceProbabilityCalculationActivity CreateHydraRingTargetProbabilityCalculationActivity(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                                                                    string hlcdDirectory,
                                                                                                                    GrassCoverErosionInwardsInput inwardsInput,
                                                                                                                    GrassCoverErosionInwardsOutput inwardsOutput
            )
        {
            var hydraulicBoundaryLocationId = (int) hydraulicBoundaryLocation.Id;

            return HydraRingActivityFactory.Create(
                string.Format(Resources.GrassCoverErosionInwardsGuiPlugin_Calculate_overtopping_for_location_0_, hydraulicBoundaryLocationId),
                hlcdDirectory,
                hydraulicBoundaryLocationId.ToString(),
                HydraRingTimeIntegrationSchemeType.FBC,
                HydraRingUncertaintiesType.All,
                new OvertoppingCalculationInput(hydraulicBoundaryLocationId, new HydraRingSection(hydraulicBoundaryLocationId, hydraulicBoundaryLocationId.ToString(), double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN),
                                                inwardsInput.DikeHeight, inwardsInput.CriticalFlowRate.StandardDeviation, inwardsInput.CriticalFlowRate.Mean,
                                                ParseProfilePoints(inwardsInput.DikeGeometry), ParseForeshore(inwardsInput), ParseBreakWater(inwardsInput)
                    ),
                output => { ParseHydraRingOutput(inwardsOutput, output); });
        }

        private static IEnumerable<HydraRingBreakWater> ParseBreakWater(GrassCoverErosionInwardsInput input)
        {
            return input.BreakWaterPresent ?
                       input.BreakWater.Select(water => new HydraRingBreakWater((int) water.Type, water.Height)) :
                       Enumerable.Empty<HydraRingBreakWater>();
        }

        private static IEnumerable<HydraRingForelandPoint> ParseForeshore(GrassCoverErosionInwardsInput input)
        {
            if (!input.ForeshorePresent)
            {
                yield break;
            }
            if (input.ForeshoreGeometry.Any())
            {
                var first = input.ForeshoreGeometry.First();
                yield return new HydraRingForelandPoint(first.StartingPoint.X, first.StartingPoint.Y);
            }

            foreach (var foreshore in input.ForeshoreGeometry)
            {
                yield return new HydraRingForelandPoint(foreshore.EndingPoint.X, foreshore.EndingPoint.Y);
            }
        }

        private static IEnumerable<HydraRingRoughnessProfilePoint> ParseProfilePoints(IEnumerable<RoughnessProfileSection> profileSections)
        {
            if (profileSections.Any())
            {
                var first = profileSections.First();
                yield return new HydraRingRoughnessProfilePoint(first.StartingPoint.X, first.StartingPoint.Y, 0);
            }

            foreach (var profileSection in profileSections)
            {
                yield return new HydraRingRoughnessProfilePoint(profileSection.EndingPoint.X, profileSection.EndingPoint.Y, profileSection.Roughness);
            }
        }

        private static void ParseHydraRingOutput(GrassCoverErosionInwardsOutput grassCoverErosionInwardsOutput, ExceedanceProbabilityCalculationOutput output)
        {
            if (output != null)
            {
                grassCoverErosionInwardsOutput.Probability = (RoundedDouble) output.Beta;
            }
            else
            {
                throw new InvalidOperationException(Resources.GrassCoverErosionInwardsGuiPlugin_Error_during_overtopping_calculation);
            }
        }

        #region GrassCoverErosionInwards TreeNodeInfo

        private object[] FailureMechanismChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext)
        {
            GrassCoverErosionInwardsFailureMechanism wrappedData = grassCoverErosionInwardsFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new GrassCoverErosionInwardsCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static IList GetInputs(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private static IList GetOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var changeRelevancyItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                RingtoetsCommonFormsResources.Checkbox_ticked,
                (sender, args) =>
                {
                    Gui.ViewCommands.RemoveAllViewsForItem(grassCoverErosionInwardsFailureMechanismContext);
                    grassCoverErosionInwardsFailureMechanismContext.WrappedData.IsRelevant = false;
                    grassCoverErosionInwardsFailureMechanismContext.WrappedData.NotifyObservers();
                }
                );

            var addCalculationGroupItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                RingtoetsCommonFormsResources.FailureMechanism_Add_CalculationGroup_Tooltip,
                RingtoetsCommonFormsResources.AddFolderIcon,
                (o, args) => AddCalculationGroup(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
                )
            {
                Enabled = false
            };

            var addCalculationItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation,
                GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsFailureMechanism_Add_GrassCoverErosionInwardsCalculation_Tooltip,
                GrassCoverErosionInwardsFormsResources.CalculationIcon,
                (s, e) => AddCalculation(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
                )
            {
                Enabled = false
            };

            return Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(changeRelevancyItem)
                      .AddSeparator()
                      .AddCustomItem(addCalculationGroupItem)
                      .AddCustomItem(addCalculationItem)
                      .AddSeparator()
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .Build();
        }

        private static void AddCalculationGroup(ICalculatableFailureMechanism failureMechanism)
        {
            var calculation = new CalculationGroup
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, RingtoetsCommonDataResources.CalculationGroup_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.NotifyObservers();
        }

        private static void AddCalculation(ICalculatableFailureMechanism failureMechanism)
        {
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsCalculation_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.NotifyObservers();
        }

        #endregion

        #region CalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(GrassCoverErosionInwardsCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in nodeData.WrappedData.Children)
            {
                var calculation = calculationItem as GrassCoverErosionInwardsCalculation;
                var group = calculationItem as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionInwardsCalculationContext(calculation,
                                                                                        nodeData.GrassCoverErosionInwardsFailureMechanism,
                                                                                        nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                             nodeData.GrassCoverErosionInwardsFailureMechanism,
                                                                                             nodeData.AssessmentSection));
                }
                else
                {
                    childNodeObjects.Add(calculationItem);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(GrassCoverErosionInwardsCalculationGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var group = nodeData.WrappedData;

            var addCalculationGroupItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                RingtoetsCommonFormsResources.FailureMechanism_Add_CalculationGroup_Tooltip,
                RingtoetsCommonFormsResources.AddFolderIcon,
                (o, args) =>
                {
                    var calculation = new CalculationGroup
                    {
                        Name = NamingHelper.GetUniqueName(group.Children, RingtoetsCommonDataResources.CalculationGroup_DefaultName, c => c.Name)
                    };
                    group.Children.Add(calculation);
                    nodeData.WrappedData.NotifyObservers();
                });

            var addCalculationItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation,
                GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsFailureMechanism_Add_GrassCoverErosionInwardsCalculation_Tooltip,
                GrassCoverErosionInwardsFormsResources.CalculationIcon,
                (o, args) =>
                {
                    var calculation = new GrassCoverErosionInwardsCalculation
                    {
                        Name = NamingHelper.GetUniqueName(group.Children, GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsCalculation_DefaultName, c => c.Name)
                    };

                    group.Children.Add(calculation);
                    nodeData.WrappedData.NotifyObservers();
                });

            var builder = Gui.Get(nodeData, treeViewControl);

            if (parentData is GrassCoverErosionInwardsFailureMechanismContext)
            {
                builder
                    .AddOpenItem()
                    .AddSeparator();
            }

            builder
                .AddCustomItem(addCalculationGroupItem)
                .AddCustomItem(addCalculationItem)
                .AddSeparator();

            var isRenamable = CalculationGroupContextCanRename(nodeData, parentData);
            var isRemovable = CalculationGroupContextCanRemove(nodeData, parentData);

            if (isRenamable)
            {
                builder.AddRenameItem();
            }
            if (isRemovable)
            {
                builder.AddDeleteItem();
            }

            if (isRemovable || isRenamable)
            {
                builder.AddSeparator();
            }

            return builder
                .AddImportItem()
                .AddExportItem()
                .AddSeparator()
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }

        private bool CalculationGroupContextCanRename(GrassCoverErosionInwardsCalculationGroupContext nodeData, object parentData)
        {
            return !(parentData is GrassCoverErosionInwardsFailureMechanismContext);
        }

        private void CalculationGroupContextOnNodeRenamed(GrassCoverErosionInwardsCalculationGroupContext nodeData, string newName)
        {
            nodeData.WrappedData.Name = newName;
            nodeData.NotifyObservers();
        }

        private bool CalculationGroupContextCanRemove(GrassCoverErosionInwardsCalculationGroupContext nodeData, object parentNodeData)
        {
            var group = parentNodeData as GrassCoverErosionInwardsCalculationGroupContext;
            return group != null && group.WrappedData.Children.Contains(nodeData.WrappedData);
        }

        private void CalculationGroupContextOnNodeRemoved(GrassCoverErosionInwardsCalculationGroupContext nodeData, object parentNodeData)
        {
            var group = parentNodeData as GrassCoverErosionInwardsCalculationGroupContext;
            if (group != null)
            {
                group.WrappedData.Children.Remove(nodeData.WrappedData);
                group.NotifyObservers();
            }
        }

        private bool CalculationGroupContextCanDrag(GrassCoverErosionInwardsCalculationGroupContext nodeData, object parentData)
        {
            return !(parentData is GrassCoverErosionInwardsFailureMechanismContext);
        }

        private bool CalculationGroupContextCanDropOrCanInsert(object draggedData, object targetData)
        {
            return GetAsICalculationItem(draggedData) != null && NodesHaveSameParentFailureMechanism(draggedData, targetData);
        }

        private static ICalculationBase GetAsICalculationItem(object item)
        {
            var calculationContext = item as GrassCoverErosionInwardsCalculationContext;
            if (calculationContext != null)
            {
                return calculationContext.WrappedData;
            }

            var groupContext = item as GrassCoverErosionInwardsCalculationGroupContext;
            return groupContext != null ? groupContext.WrappedData : null;
        }

        private bool NodesHaveSameParentFailureMechanism(object draggedData, object targetData)
        {
            var sourceFailureMechanism = GetParentFailureMechanism(draggedData);
            var targetFailureMechanism = GetParentFailureMechanism(targetData);

            return ReferenceEquals(sourceFailureMechanism, targetFailureMechanism);
        }

        private static GrassCoverErosionInwardsFailureMechanism GetParentFailureMechanism(object data)
        {
            var calculationContext = data as GrassCoverErosionInwardsCalculationContext;
            if (calculationContext != null)
            {
                return calculationContext.GrassCoverErosionInwardsFailureMechanism;
            }

            var groupContext = data as GrassCoverErosionInwardsCalculationGroupContext;
            return groupContext != null ? groupContext.GrassCoverErosionInwardsFailureMechanism : null;
        }

        private void CalculationGroupContextOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
        {
            ICalculationBase calculationItem = GetAsICalculationItem(droppedData);
            var originalOwnerContext = oldParentData as GrassCoverErosionInwardsCalculationGroupContext;
            var target = newParentData as GrassCoverErosionInwardsCalculationGroupContext;

            if (calculationItem != null && originalOwnerContext != null && target != null)
            {
                var isMoveWithinSameContainer = ReferenceEquals(originalOwnerContext, target);

                DroppingCalculationInContainerStrategy dropHandler = GetDragDropStrategy(isMoveWithinSameContainer, originalOwnerContext, target);
                dropHandler.Execute(droppedData, calculationItem, position, treeViewControl);
            }
        }

        private DroppingCalculationInContainerStrategy GetDragDropStrategy(bool isMoveWithinSameContainer, GrassCoverErosionInwardsCalculationGroupContext originalOwnerContext, GrassCoverErosionInwardsCalculationGroupContext target)
        {
            return isMoveWithinSameContainer
                       ? (DroppingCalculationInContainerStrategy) new DroppingCalculationWithinSameContainer(originalOwnerContext, target)
                       : new DroppingCalculationToNewContainer(originalOwnerContext, target);
        }

        #region Nested Types: DroppingPipingCalculationInContainerStrategy and implementations

        /// <summary>
        /// Strategy pattern implementation for dealing with drag & dropping a <see cref="ICalculation"/>
        /// onto <see cref="CalculationGroup"/> data.
        /// </summary>
        private abstract class DroppingCalculationInContainerStrategy
        {
            protected readonly GrassCoverErosionInwardsCalculationGroupContext target;
            private readonly GrassCoverErosionInwardsCalculationGroupContext originalOwnerContext;

            protected DroppingCalculationInContainerStrategy(GrassCoverErosionInwardsCalculationGroupContext originalOwnerContext, GrassCoverErosionInwardsCalculationGroupContext target)
            {
                this.originalOwnerContext = originalOwnerContext;
                this.target = target;
            }

            /// <summary>
            /// Perform the drag & drop operation.
            /// </summary>
            /// <param name="draggedData">The dragged data.</param>
            /// <param name="calculationBase">The calculation item wrapped by <see cref="draggedData"/>.</param>
            /// <param name="newPosition">The index of the new position within the new owner's collection.</param>
            /// <param name="treeViewControl">The tree view control which is at stake.</param>
            public virtual void Execute(object draggedData, ICalculationBase calculationBase, int newPosition, TreeViewControl treeViewControl)
            {
                MoveCalculationItemToNewOwner(calculationBase, newPosition);

                NotifyObservers();
            }

            /// <summary>
            /// Moves the <see cref="ICalculationBase"/> instance to its new location.
            /// </summary>
            /// <param name="calculationBase">The instance to be relocated.</param>
            /// <param name="position">The index in the new <see cref="CalculationGroup"/>
            /// owner within its <see cref="CalculationGroup.Children"/>.</param>
            protected void MoveCalculationItemToNewOwner(ICalculationBase calculationBase, int position)
            {
                originalOwnerContext.WrappedData.Children.Remove(calculationBase);
                target.WrappedData.Children.Insert(position, calculationBase);
            }

            /// <summary>
            /// Notifies observers of the change in state.
            /// </summary>
            protected virtual void NotifyObservers()
            {
                originalOwnerContext.NotifyObservers();
            }
        }

        /// <summary>
        /// Strategy implementation for rearranging the order of an <see cref="ICalculation"/>
        /// within a <see cref="CalculationGroup"/> through a drag & drop action.
        /// </summary>
        private class DroppingCalculationWithinSameContainer : DroppingCalculationInContainerStrategy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingCalculationWithinSameContainer"/> class.
            /// </summary>
            /// <param name="originalOwnerContext">The calculation group context that is 
            /// the original owner of the dragged item.</param>
            /// <param name="target">The calculation group context that is the target
            /// of the drag & drop operation.</param>
            public DroppingCalculationWithinSameContainer(GrassCoverErosionInwardsCalculationGroupContext originalOwnerContext, GrassCoverErosionInwardsCalculationGroupContext target) :
                base(originalOwnerContext, target) {}
        }

        /// <summary>
        /// Strategy implementation for moving an <see cref="ICalculation"/> from
        /// one <see cref="CalculationGroup"/> to another using a drag & drop action.
        /// </summary>
        private class DroppingCalculationToNewContainer : DroppingCalculationInContainerStrategy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingCalculationToNewContainer"/> class.
            /// </summary>
            /// <param name="originalOwnerContext">The calculation group context that is 
            /// the original owner of the dragged item.</param>
            /// <param name="target">The calculation group context that is the target
            /// of the drag & drop operation.</param>
            public DroppingCalculationToNewContainer(GrassCoverErosionInwardsCalculationGroupContext originalOwnerContext, GrassCoverErosionInwardsCalculationGroupContext target) :
                base(originalOwnerContext, target) {}

            public override void Execute(object draggedData, ICalculationBase calculationBase, int newPosition, TreeViewControl treeViewControl)
            {
                MoveCalculationItemToNewOwner(calculationBase, newPosition);

                NotifyObservers();

                // Try to start a name edit action when an item with the same name was already present
                if (target.WrappedData.Children.Except(new[]
                {
                    calculationBase
                }).Any(c => c.Name.Equals(calculationBase.Name)))
                {
                    treeViewControl.TryRenameNodeForData(draggedData);
                }
            }

            protected override void NotifyObservers()
            {
                base.NotifyObservers();
                target.NotifyObservers();
            }
        }

        # endregion

        #endregion
    }
}