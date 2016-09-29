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
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Forms.Views;
using Ringtoets.HeightStructures.IO;
using Ringtoets.HeightStructures.Service;
using Ringtoets.HydraRing.IO;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using HeightStructuresDataResources = Ringtoets.HeightStructures.Data.Properties.Resources;
using HeightStructuresFormsResources = Ringtoets.HeightStructures.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.HeightStructures.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="HeightStructuresFailureMechanism"/>.
    /// </summary>
    public class HeightStructuresPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<HeightStructuresFailureMechanismContext, HeightStructuresFailureMechanismContextProperties>();
            yield return new PropertyInfo<HeightStructuresInputContext, HeightStructuresInputContextProperties>();
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<HeightStructuresCollectionContext>
            {
                CreateFileImporter = (context, filePath) => new HeightStructuresImporter(context.WrappedData,
                                                                                         context.AssessmentSection.ReferenceLine,
                                                                                         filePath),
                Name = RingtoetsCommonFormsResources.StructuresImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.StructuresIcon,
                FileFilter = RingtoetsCommonIOResources.DataTypeDisplayName_shape_file_filter,
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>,
                IEnumerable<HeightStructuresFailureMechanismSectionResult>,
                HeightStructuresFailureMechanismResultView>
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
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<HeightStructuresFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<HeightStructuresCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<HeightStructuresCalculationContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<HeightStructuresInputContext>
            {
                Text = inputContext => HeightStructuresFormsResources.HeightStructuresInputContext_NodeDisplayName,
                Image = inputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<HeightStructuresCollectionContext>
            {
                Text = context => RingtoetsCommonFormsResources.StructuresCollection_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.Any()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = context => context.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddSeparator()
                                                                                 .AddExpandAllItem()
                                                                                 .AddCollapseAllItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<HeightStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RingtoetsCommonFormsResources.StructuresIcon,
                ContextMenuStrip = (structure, parentData, treeViewControl) => Gui.Get(structure, treeViewControl)
                                                                                  .AddPropertiesItem()
                                                                                  .Build()
            };

            yield return new TreeNodeInfo<ProbabilityAssessmentOutput>
            {
                Text = output => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateEmptyProbabilityAssessmentOutputTreeNodeInfo(
                EmptyProbabilityAssessmentOutputContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        private void CalculateAll(HeightStructuresFailureMechanism failureMechanism,
                                  IEnumerable<HeightStructuresCalculation> calculations,
                                  IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             calculations.Select(calc => new HeightStructuresCalculationActivity(calc,
                                                                                                                 Path.GetDirectoryName(assessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                                                                 failureMechanism,
                                                                                                                 assessmentSection)).ToArray());
        }

        private void ValidateAll(IEnumerable<HeightStructuresCalculation> heightStructuresCalculations, IAssessmentSection assessmentSection)
        {
            foreach (var calculation in heightStructuresCalculations)
            {
                HeightStructuresCalculationService.Validate(calculation, assessmentSection);
            }
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, HeightStructuresFailureMechanism failureMechanism)
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
                return string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                                     validationProblem);
            }

            return null;
        }

        #region TreeNodeInfos

        #region EmptyProbabilityAssessmentOutput TreeNodeInfo

        private ContextMenuStrip EmptyProbabilityAssessmentOutputContextMenuStrip(EmptyProbabilityAssessmentOutput output, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(output, treeViewControl));
            return builder.AddPropertiesItem()
                          .Build();
        }

        #endregion

        #region HeightStructuresFailureMechanismResultView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(HeightStructuresFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as HeightStructuresFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<HeightStructuresFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<HeightStructuresFailureMechanism>()
                    .Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #region HeightStructuresFailureMechanismContext TreeNodeInfo

        private object[] FailureMechanismEnabledChildNodeObjects(HeightStructuresFailureMechanismContext context)
        {
            HeightStructuresFailureMechanism wrappedData = context.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, context.Parent), TreeFolderCategory.Input),
                new HeightStructuresCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, context.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static IList GetInputs(HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, assessmentSection),
                new HeightStructuresCollectionContext(failureMechanism.HeightStructures, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private static IList GetOutputs(HeightStructuresFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(HeightStructuresFailureMechanismContext context)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(context.WrappedData)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(HeightStructuresFailureMechanismContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(context, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              context,
                              c => ValidateAll(c.WrappedData.Calculations.OfType<HeightStructuresCalculation>(), c.Parent),
                              ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism)
                          .AddPerformAllCalculationsInFailureMechanismItem(context, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism)
                          .AddClearAllCalculationOutputInFailureMechanismItem(context.WrappedData)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(HeightStructuresFailureMechanismContext context)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(context);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(HeightStructuresFailureMechanismContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(context, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism(HeightStructuresFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent, context.WrappedData);
        }

        private void CalculateAll(HeightStructuresFailureMechanismContext context)
        {
            CalculateAll(context.WrappedData, context.WrappedData.Calculations.OfType<HeightStructuresCalculation>(), context.Parent);
        }

        #endregion

        #region HeightStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(HeightStructuresCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                var calculation = calculationItem as HeightStructuresCalculation;
                var group = calculationItem as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new HeightStructuresCalculationContext(calculation,
                                                                                context.FailureMechanism,
                                                                                context.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new HeightStructuresCalculationGroupContext(group,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(HeightStructuresCalculationGroupContext context, object parentData, TreeViewControl treeViewControl)
        {
            var group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            var isNestedGroup = parentData is HeightStructuresCalculationGroupContext;

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
                       c => ValidateAll(c.WrappedData.GetCalculations().OfType<HeightStructuresCalculation>(), c.AssessmentSection),
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

        private static void CalculationGroupContextOnNodeRemoved(HeightStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (HeightStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);
            parentGroupContext.NotifyObservers();
        }

        private static void AddCalculation(HeightStructuresCalculationGroupContext context)
        {
            var calculation = new HeightStructuresCalculation
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, HeightStructuresDataResources.HeightStructuresCalculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationsInGroup(HeightStructuresCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private void CalculateAll(CalculationGroup group, HeightStructuresCalculationGroupContext context)
        {
            CalculateAll(context.FailureMechanism, group.GetCalculations().OfType<HeightStructuresCalculation>(), context.AssessmentSection);
        }

        #endregion

        #region HeightStructuresCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(HeightStructuresCalculationContext context)
        {
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(context.WrappedData),
                new HeightStructuresInputContext(context.WrappedData.InputParameters,
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

        private ContextMenuStrip CalculationContextContextMenuStrip(HeightStructuresCalculationContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            HeightStructuresCalculation calculation = context.WrappedData;

            return builder.AddValidateCalculationItem(
                context,
                c => HeightStructuresCalculationService.Validate(c.WrappedData, c.AssessmentSection),
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

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculation(HeightStructuresCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private void Calculate(HeightStructuresCalculation calculation, HeightStructuresCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, new HeightStructuresCalculationActivity(calculation,
                                                                                                     Path.GetDirectoryName(context.AssessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                                                     context.FailureMechanism,
                                                                                                     context.AssessmentSection));
        }

        private void CalculationContextOnNodeRemoved(HeightStructuresCalculationContext context, object parentData)
        {
            var calculationGroupContext = parentData as HeightStructuresCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                calculationGroupContext.NotifyObservers();
            }
        }

        #endregion

        #endregion
    }
}