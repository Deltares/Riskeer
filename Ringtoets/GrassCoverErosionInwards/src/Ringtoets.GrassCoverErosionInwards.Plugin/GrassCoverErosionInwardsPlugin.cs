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

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.GrassCoverErosionInwards.Utils;
using Ringtoets.HydraRing.IO;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using GrassCoverErosionInwardsPluginResources = Ringtoets.GrassCoverErosionInwards.Plugin.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanismContextProperties>();
            yield return new PropertyInfo<DikeProfile, DikeProfileProperties>();
            yield return new PropertyInfo<GrassCoverErosionInwardsInputContext, GrassCoverErosionInwardsInputContextProperties>();
            yield return new PropertyInfo<GrassCoverErosionInwardsOutput, GrassCoverErosionInwardsOutputProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                GrassCoverErosionInwardsScenariosContext,
                CalculationGroup,
                GrassCoverErosionInwardsScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, calculationGroup) => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                AfterCreate = (view, context) => view.FailureMechanism = context.ParentFailureMechanism,
                CloseForData = CloseScenariosViewForData,
                Image = RingtoetsCommonFormsResources.ScenariosIcon
            };

            yield return new ViewInfo<
                FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>,
                IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult>,
                GrassCoverErosionInwardsFailureMechanismResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };

            yield return new ViewInfo<GrassCoverErosionInwardsInputContext, GrassCoverErosionInwardsCalculation, GrassCoverErosionInwardsInputView>
            {
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                GetViewName = (view, input) => RingtoetsCommonFormsResources.Calculation_Input,
                GetViewData = context => context.Calculation,
                CloseForData = CloseInputViewForData
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<GrassCoverErosionInwardsFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<DikeProfilesContext>
            {
                Text = context => GrassCoverErosionInwardsPluginResources.GrassCoverErosionInwardsPlugin_DikeProfilesContext_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.Any() ?
                                           Color.FromKnownColor(KnownColor.ControlText) :
                                           Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = context => context.WrappedData
                                                     .Cast<object>()
                                                     .ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .Build()
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<GrassCoverErosionInwardsCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<GrassCoverErosionInwardsCalculationContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<GrassCoverErosionInwardsScenariosContext>
            {
                Text = context => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionInwardsInputContext>
            {
                Text = inputContext => RingtoetsCommonFormsResources.Calculation_Input,
                Image = inputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionInwardsOutput>
            {
                Text = output => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        private void CalculateAll(GrassCoverErosionInwardsFailureMechanism failureMechanism, IEnumerable<GrassCoverErosionInwardsCalculation> calculations, IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, calculations.Select(
                calc => new GrassCoverErosionInwardsCalculationActivity(calc,
                                                                        Path.GetDirectoryName(assessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                        failureMechanism,
                                                                        assessmentSection)).ToArray());
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (!failureMechanism.Sections.Any())
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported;
            }

            if (assessmentSection.HydraulicBoundaryDatabase == null)
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported;
            }

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_, validationProblem);
            }

            return null;
        }

        #region GrassCoverErosionInwardsScenariosView ViewInfo

        private static bool CloseScenariosViewForData(GrassCoverErosionInwardsScenariosView view, object removedData)
        {
            var failureMechanism = removedData as GrassCoverErosionInwardsFailureMechanism;

            var failureMechanismContext = removedData as GrassCoverErosionInwardsFailureMechanismContext;
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            var assessmentSection = removedData as IAssessmentSection;
            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<GrassCoverErosionInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        #endregion

        #region GrassCoverErosionInwardsFailureMechanismResultView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(GrassCoverErosionInwardsFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as GrassCoverErosionInwardsFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<GrassCoverErosionInwardsFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<GrassCoverErosionInwardsFailureMechanism>()
                    .Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #region GrassCoverErosionInwardsInputView  ViewInfo

        private bool CloseInputViewForData(GrassCoverErosionInwardsInputView view, object o)
        {
            var calculationContext = o as GrassCoverErosionInwardsCalculationContext;
            if (calculationContext != null)
            {
                return ReferenceEquals(view.Data, calculationContext.WrappedData);
            }

            IEnumerable<GrassCoverErosionInwardsCalculation> calculations = null;

            var calculationGroupContext = o as GrassCoverErosionInwardsCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculations = calculationGroupContext.WrappedData.GetCalculations()
                                                      .OfType<GrassCoverErosionInwardsCalculation>();
            }

            var failureMechanism = o as GrassCoverErosionInwardsFailureMechanism;

            var failureMechanismContext = o as GrassCoverErosionInwardsFailureMechanismContext;
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            var assessmentSection = o as IAssessmentSection;
            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<GrassCoverErosionInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (failureMechanism != null)
            {
                calculations = failureMechanism.CalculationsGroup.GetCalculations()
                                               .OfType<GrassCoverErosionInwardsCalculation>();
            }

            return calculations != null && calculations.Any(ci => ReferenceEquals(view.Data, ci));
        }

        #endregion

        private void ValidateAll(IEnumerable<GrassCoverErosionInwardsCalculation> grassCoverErosionInwardsCalculations, IAssessmentSection assessmentSection)
        {
            foreach (var calculation in grassCoverErosionInwardsCalculations)
            {
                new GrassCoverErosionInwardsCalculationService().Validate(calculation, assessmentSection);
            }
        }

        #region GrassCoverErosionInwardsFailureMechanismContext TreeNodeInfo

        private object[] FailureMechanismEnabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext)
        {
            GrassCoverErosionInwardsFailureMechanism wrappedData = grassCoverErosionInwardsFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new GrassCoverErosionInwardsCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
            };
        }

        private static IList GetInputs(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private static IList GetOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new GrassCoverErosionInwardsScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              grassCoverErosionInwardsFailureMechanismContext,
                              fm => ValidateAll(fm.WrappedData.Calculations.OfType<GrassCoverErosionInwardsCalculation>(), grassCoverErosionInwardsFailureMechanismContext.Parent),
                              ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism)
                          .AddPerformAllCalculationsInFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism)
                          .AddClearAllCalculationOutputInFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent, context.WrappedData);
        }

        private void CalculateAll(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            CalculateAll(context.WrappedData, context.WrappedData.Calculations.OfType<GrassCoverErosionInwardsCalculation>(), context.Parent);
        }

        #endregion

        #region GrassCoverErosionInwardsCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                var calculation = calculationItem as GrassCoverErosionInwardsCalculation;
                var group = calculationItem as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionInwardsCalculationContext(calculation,
                                                                                        context.FailureMechanism,
                                                                                        context.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                             context.FailureMechanism,
                                                                                             context.AssessmentSection));
                }
                else
                {
                    childNodeObjects.Add(calculationItem);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(GrassCoverErosionInwardsCalculationGroupContext context, object parentData, TreeViewControl treeViewControl)
        {
            CalculationGroup group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            var isNestedGroup = parentData is GrassCoverErosionInwardsCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGenerateCalculationsItem(context);

            if (!isNestedGroup)
            {
                builder.AddCustomItem(generateCalculationsItem)
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation);

            if (!isNestedGroup)
            {
                builder.AddSeparator()
                       .AddRemoveAllChildrenItem(group, Gui.ViewCommands);
            }

            builder.AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(
                       context,
                       c => ValidateAll(c.WrappedData.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>(), c.AssessmentSection),
                       ValidateAllDataAvailableAndGetErrorMessageForCalculationsInGroup)
                   .AddPerformAllCalculationsInGroupItem(group, context, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationsInGroup)
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem()
                       .AddDeleteItem()
                       .AddSeparator();
            }

            return builder.AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private StrictContextMenuItem CreateGenerateCalculationsItem(GrassCoverErosionInwardsCalculationGroupContext nodeData)
        {
            bool isDikeProfileAvailable = nodeData.AvailableDikeProfiles.Any();

            string calculationGroupGenerateCalculationsToolTip = isDikeProfileAvailable
                                                                     ? GrassCoverErosionInwardsPluginResources.GrassCoverErosionInwardsPlugin_CreateGenerateCalculationsItem_ToolTip
                                                                     : GrassCoverErosionInwardsPluginResources.GrassCoverErosionInwardsPlugin_CreateGenerateCalculationsItem_NoDikeLocations_ToolTip;

            var generateCalculationsItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
                calculationGroupGenerateCalculationsToolTip,
                RingtoetsCommonFormsResources.GenerateScenariosIcon, (o, args) => { ShowDikeProfileSelectionDialog(nodeData); })
            {
                Enabled = isDikeProfileAvailable
            };
            return generateCalculationsItem;
        }

        private void ShowDikeProfileSelectionDialog(GrassCoverErosionInwardsCalculationGroupContext nodeData)
        {
            using (var view = new GrassCoverErosionInwardsDikeProfileSelectionDialog(Gui.MainWindow, nodeData.AvailableDikeProfiles))
            {
                view.ShowDialog();
                GenerateCalculations(nodeData.WrappedData, nodeData.FailureMechanism, view.SelectedItems);
            }
            nodeData.NotifyObservers();
        }

        private void GenerateCalculations(CalculationGroup target, GrassCoverErosionInwardsFailureMechanism failureMechanism, IEnumerable<DikeProfile> dikeProfiles)
        {
            foreach (var profile in dikeProfiles)
            {
                var calculation = new GrassCoverErosionInwardsCalculation
                {
                    Name = NamingHelper.GetUniqueName(target.Children, profile.Name, c => c.Name),
                    InputParameters =
                    {
                        DikeProfile = profile
                    }
                };
                target.Children.Add(calculation);
                AssignUnassignCalculations.Update(failureMechanism.SectionResults, calculation);
            }
        }

        private static void CalculationGroupContextOnNodeRemoved(GrassCoverErosionInwardsCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (GrassCoverErosionInwardsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);
            parentGroupContext.NotifyObservers();
        }

        private static void AddCalculation(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, RingtoetsCommonDataResources.Calculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationsInGroup(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private void CalculateAll(CalculationGroup group, GrassCoverErosionInwardsCalculationGroupContext context)
        {
            CalculateAll(context.FailureMechanism, group.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>(), context.AssessmentSection);
        }

        #endregion

        #region GrassCoverErosionInwardsCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(GrassCoverErosionInwardsCalculationContext context)
        {
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(context.WrappedData),
                new GrassCoverErosionInwardsInputContext(context.WrappedData.InputParameters,
                                                         context.WrappedData,
                                                         context.FailureMechanism,
                                                         context.AssessmentSection)
            };

            if (context.WrappedData.HasOutput)
            {
                childNodes.Add(context.WrappedData.Output);
            }
            else
            {
                childNodes.Add(new EmptyProbabilityAssessmentOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(GrassCoverErosionInwardsCalculationContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            GrassCoverErosionInwardsCalculation calculation = context.WrappedData;

            return builder.AddValidateCalculationItem(
                context,
                c => new GrassCoverErosionInwardsCalculationService().Validate(c.WrappedData, c.AssessmentSection),
                ValidateAllDataAvailableAndGetErrorMessageForCalculation)
                          .AddPerformCalculationItem(calculation, context, Calculate, ValidateAllDataAvailableAndGetErrorMessageForCalculation)
                          .AddClearCalculationOutputItem(calculation)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddDeleteItem()
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculation(GrassCoverErosionInwardsCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private void Calculate(GrassCoverErosionInwardsCalculation calculation, GrassCoverErosionInwardsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, new GrassCoverErosionInwardsCalculationActivity(calculation,
                                                                                                             Path.GetDirectoryName(context.AssessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                                                             context.FailureMechanism,
                                                                                                             context.AssessmentSection));
        }

        private void CalculationContextOnNodeRemoved(GrassCoverErosionInwardsCalculationContext context, object parentData)
        {
            var calculationGroupContext = parentData as GrassCoverErosionInwardsCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                AssignUnassignCalculations.Delete(context.FailureMechanism.SectionResults, context.WrappedData, context.FailureMechanism.Calculations.OfType<GrassCoverErosionInwardsCalculation>());
                calculationGroupContext.NotifyObservers();
            }
        }

        #endregion
    }
}