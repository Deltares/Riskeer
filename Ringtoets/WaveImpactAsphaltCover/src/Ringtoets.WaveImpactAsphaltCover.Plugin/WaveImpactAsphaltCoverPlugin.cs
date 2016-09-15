﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;
using Ringtoets.Revetment.Service;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses;
using Ringtoets.WaveImpactAsphaltCover.Forms.Views;
using Ringtoets.WaveImpactAsphaltCover.IO;
using Ringtoets.WaveImpactAsphaltCover.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using WaveImpactAsphaltCoverDataResources = Ringtoets.WaveImpactAsphaltCover.Data.Properties.Resources;
using WaveImpactAsphaltCoverFormsResources = Ringtoets.WaveImpactAsphaltCover.Forms.Properties.Resources;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
    /// </summary>
    public class WaveImpactAsphaltCoverPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<WaveImpactAsphaltCoverFailureMechanismContext, WaveImpactAsphaltCoverFailureMechanismContextProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>,
                IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult>,
                WaveImpactAsphaltCoverFailureMechanismResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<WaveImpactAsphaltCoverFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext>(
                WaveConditionsCalculationGroupContextChildNodeObjects,
                WaveConditionsCalculationGroupContextContextMenuStrip,
                WaveConditionsCalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<WaveImpactAsphaltCoverWaveConditionsCalculationContext>(
                WaveImpactAsphaltCoverFormsResources.CalculationIcon,
                WaveConditionsCalculationContextChildNodeObjects,
                WaveConditionsCalculationContextContextMenuStrip,
                WaveConditionsCalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<WaveImpactAsphaltCoverWaveConditionsCalculationInputContext>
            {
                Text = context => RingtoetsCommonFormsResources.Calculation_Input,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyWaveImpactAsphaltCoverOutput>
            {
                Text = emptyPipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyPipingOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<WaveImpactAsphaltCoverWaveConditionsOutput>
            {
                Text = emptyPipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext>
            {
                CreateFileExporter = (context, filePath) => new WaveImpactAsphaltCoverWaveConditionsExporter(context.WrappedData.GetCalculations().Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>(), filePath),
                IsEnabled = context => context.WrappedData.GetCalculations().Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>().Any(c => c.HasOutput),
                FileFilter = RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter
            };

            yield return new ExportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationContext>
            {
                CreateFileExporter = (context, filePath) => new WaveImpactAsphaltCoverWaveConditionsExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                IsEnabled = context => context.WrappedData.HasOutput,
                FileFilter = RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter
            };
        }

        #region ViewInfos

        #region FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>

        private static bool CloseFailureMechanismResultViewForData(WaveImpactAsphaltCoverFailureMechanismResultView view, object dataToCloseFor)
        {
            var viewData = view.Data;
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as WaveImpactAsphaltCoverFailureMechanism;
            var failureMechanismContext = dataToCloseFor as IFailureMechanismContext<WaveImpactAsphaltCoverFailureMechanism>;

            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<WaveImpactAsphaltCoverFailureMechanism>()
                    .Any(fm => ReferenceEquals(viewData, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #endregion

        #region TreeNodeInfos

        #region WaveImpactAsphaltCoverFailureMechanismContext

        private object[] FailureMechanismEnabledChildNodeObjects(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext)
        {
            WaveImpactAsphaltCoverFailureMechanism wrappedData = failureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, failureMechanismContext.Parent), TreeFolderCategory.Input),
                new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(wrappedData.WaveConditionsCalculationGroup, wrappedData, failureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(failureMechanismContext.WrappedData)
            };
        }

        private static IList GetInputs(WaveImpactAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private IList GetOutputs(WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        #endregion

        #endregion

        #region WaveImpactAsphaltCover TreeNodeOnfo

        private object[] WaveConditionsCalculationGroupContextChildNodeObjects(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                var calculation = item as WaveImpactAsphaltCoverWaveConditionsCalculation;
                var group = item as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                                    nodeData.FailureMechanism,
                                                                                                    nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(group,
                                                                                                         nodeData.FailureMechanism,
                                                                                                         nodeData.AssessmentSection));
                }
                else
                {
                    childNodeObjects.Add(item);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip WaveConditionsCalculationGroupContextContextMenuStrip(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData,
                                                                                       object parentData, TreeViewControl treeViewControl)
        {
            var group = nodeData.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var isNestedGroup = parentData is WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGenerateWaveConditionsCalculationsItem(nodeData);

            if (!isNestedGroup)
            {
                builder.AddCustomItem(generateCalculationsItem);
            }

            builder.AddExportItem()
                   .AddSeparator()
                   .AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(nodeData, AddWaveConditionsCalculation);

            if (!isNestedGroup)
            {
                builder.AddSeparator()
                       .AddRemoveAllChildrenItem(group, Gui.ViewCommands);
            }

            builder.AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(nodeData,
                                                          c => ValidateAll(
                                                              c.WrappedData.GetCalculations().OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>(),
                                                              c.AssessmentSection.HydraulicBoundaryDatabase),
                                                          ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(group, nodeData, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup)
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

        private string ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private string ValidateAllDataAvailableAndGetErrorMessageForCalculation(WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            if (!failureMechanism.Sections.Any())
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported;
            }

            if (assessmentSection.HydraulicBoundaryDatabase == null)
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported;
            }

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                                     validationProblem);
            }
            
            return null;
        }

        private StrictContextMenuItem CreateGenerateWaveConditionsCalculationsItem(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData)
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = nodeData.AssessmentSection.HydraulicBoundaryDatabase;
            bool locationsAvailable = hydraulicBoundaryDatabase != null && hydraulicBoundaryDatabase.Locations.Any();

            string waveImpactAsphaltCoverWaveConditionsCalculationGroupContextToolTip = locationsAvailable
                                                                                         ? RingtoetsCommonFormsResources.CalculationGroup_CreateGenerateHydraulicBoundaryCalculationsItem_ToolTip
                                                                                         : RingtoetsCommonFormsResources.CalculationGroup_No_HRD_To_Generate_ToolTip;

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationsGroup_Generate_calculations,
                                             waveImpactAsphaltCoverWaveConditionsCalculationGroupContextToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => { ShowHydraulicBoundaryLocationSelectionDialog(nodeData); })
            {
                Enabled = locationsAvailable
            };
        }

        private void ShowHydraulicBoundaryLocationSelectionDialog(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData)
        {
            using (var dialog = new HydraulicBoundaryLocationSelectionDialog(Gui.MainWindow, nodeData.AssessmentSection.HydraulicBoundaryDatabase.Locations))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateWaveImpactAsphaltCoverWaveConditionsCalculations(dialog.SelectedItems, nodeData.WrappedData.Children);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateWaveImpactAsphaltCoverWaveConditionsCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, IList<ICalculationBase> calculationCollection)
        {
            WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(
                hydraulicBoundaryLocations,
                calculationCollection);
        }

        private void AddWaveConditionsCalculation(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData)
        {
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children,
                                                  WaveImpactAsphaltCoverDataResources.WaveImpactAsphaltCoverWaveConditionsCalculation_DefaultName,
                                                  c => c.Name)
            };
            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private void ValidateAll(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> calculations, HydraulicBoundaryDatabase database)
        {
            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in calculations)
            {
                WaveConditionsCalculationService.Instance.Validate(calculation.InputParameters, database, calculation.Name);
            }
        }

        private void CalculateAll(CalculationGroup group, WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext context)
        {
            var calculations = group.GetCalculations().OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>().ToArray();

            CalculateAll(calculations, context.FailureMechanism, context.AssessmentSection);
        }

        private void CalculateAll(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> calculations,
                                  WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                  IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                calculations
                    .Select(calculation => new WaveImpactAsphaltCoverWaveConditionsCalculationActivity(calculation,
                                                                                                       Path.GetDirectoryName(assessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                                                       failureMechanism,
                                                                                                       assessmentSection))
                    .ToList());

            foreach (var calculation in calculations)
            {
                calculation.NotifyObservers();
            }
        }

        private void WaveConditionsCalculationGroupContextOnNodeRemoved(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        #endregion

        #region WaveImpactAsphaltCoverWaveConditionsCalculationContext

        private object[] WaveConditionsCalculationContextChildNodeObjects(WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(context.WrappedData),
                new WaveImpactAsphaltCoverWaveConditionsCalculationInputContext(context.WrappedData.InputParameters,
                                                                                context.FailureMechanism,
                                                                                context.AssessmentSection)
            };

            if (context.WrappedData.HasOutput)
            {
                childNodes.Add(context.WrappedData.Output);
            }
            else
            {
                childNodes.Add(new EmptyWaveImpactAsphaltCoverOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip WaveConditionsCalculationContextContextMenuStrip(WaveImpactAsphaltCoverWaveConditionsCalculationContext nodeData,
                                                                                  object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = nodeData.WrappedData;

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddValidateCalculationItem(nodeData,
                                                      c => ValidateAll(
                                                          new[]
                                                          {
                                                              c.WrappedData
                                                          },
                                                          c.AssessmentSection.HydraulicBoundaryDatabase),
                                                      ValidateAllDataAvailableAndGetErrorMessageForCalculation)
                          .AddPerformCalculationItem(calculation, nodeData, PerformCalculation)
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

        private void PerformCalculation(WaveImpactAsphaltCoverWaveConditionsCalculation calculation,
                                        WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             new WaveImpactAsphaltCoverWaveConditionsCalculationActivity(calculation,
                                                                                                         Path.GetDirectoryName(context.AssessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                                                         context.FailureMechanism,
                                                                                                         context.AssessmentSection));
            calculation.NotifyObservers();
        }

        private void WaveConditionsCalculationContextOnNodeRemoved(WaveImpactAsphaltCoverWaveConditionsCalculationContext nodeData, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                bool successfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);
                if (successfullyRemovedData)
                {
                    calculationGroupContext.NotifyObservers();
                }
            }
        }

        #endregion
    }
}