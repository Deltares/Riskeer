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
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Plugin.Properties;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.HydraRing.IO;

using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin
{
    /// <summary>
    /// The GUI plug-in for the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsGuiPlugin : GuiPlugin
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanismContextProperties>();
            yield return new PropertyInfo<GrassCoverErosionInwardsInputContext, GrassCoverErosionInwardsInputContextProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>,
                IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult>,
                GrassCoverErosionInwardsFailureMechanismResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
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
                Text = context => BaseResources.GrassCoverErosionInwardsGuiPlugin_DikeProfilesContext_DisplayName,
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

            yield return new TreeNodeInfo<DikeProfile>
            {
                Text = dikeProfile => dikeProfile.Name,
                Image = context => Resources.DikeProfile,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<GrassCoverErosionInwardsCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<GrassCoverErosionInwardsCalculationContext>(
                GrassCoverErosionInwardsFormsResources.CalculationIcon,
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionInwardsInputContext>
            {
                Text = inputContext => GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsInputContext_NodeDisplayName,
                Image = inputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ProbabilityAssessmentOutput>
            {
                Text = output => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateEmptyProbabilityAssessmentOutputTreeNodeInfo(
                EmptyProbabilityAssessmentOutputContextMenuStrip);
        }       

        private void CalculateAll(GrassCoverErosionInwardsFailureMechanism failureMechanism, IEnumerable<GrassCoverErosionInwardsCalculation> calculations, IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, calculations.Select(calc => new GrassCoverErosionInwardsCalculationActivity(calc,
                                                                                                                                         Path.GetDirectoryName(assessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                                                                                         failureMechanism,
                                                                                                                                         assessmentSection)).ToList());
        }

        private static string AllDataAvailable(IAssessmentSection assessmentSection, GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (!failureMechanism.Sections.Any())
            {
                return RingtoetsCommonFormsResources.GuiPlugin_AllDataAvailable_No_failure_mechanism_sections_imported;
            }

            if (assessmentSection.HydraulicBoundaryDatabase == null)
            {
                return RingtoetsCommonFormsResources.GuiPlugin_AllDataAvailable_No_hydraulic_boundary_database_imported;
            }

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(RingtoetsCommonFormsResources.Hydraulic_boundary_database_connection_failed_0_, validationProblem);
            }

            return null;
        }

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

        #region EmptyProbabilityAssessmentOutput TreeNodeInfo

        private ContextMenuStrip EmptyProbabilityAssessmentOutputContextMenuStrip(EmptyProbabilityAssessmentOutput output, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(output, treeViewControl));
            return builder.AddExportItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        #endregion

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
                new FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddPerformAllCalculationsInFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext, CalculateAll, EnablePerformAllCalculationsInFailureMechanism)
                          .AddClearAllCalculationOutputInFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
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

        private static string EnablePerformAllCalculationsInFailureMechanism(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            return AllDataAvailable(context.Parent, context.WrappedData);
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
            var group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            var isNestedGroup = parentData is GrassCoverErosionInwardsCalculationGroupContext;

            if (!isNestedGroup)
            {
                builder.AddOpenItem()
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation)
                   .AddSeparator()
                   .AddPerformAllCalculationsInGroupItem(group, context, CalculateAll, EnablePerformAllCalculationsInGroup)
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem()
                       .AddDeleteItem()
                       .AddSeparator();
            }

            return builder.AddImportItem()
                          .AddExportItem()
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static void CalculationGroupContextOnNodeRemoved(GrassCoverErosionInwardsCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (GrassCoverErosionInwardsCalculationGroupContext)parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);
            parentGroupContext.NotifyObservers();
        }

        private static void AddCalculation(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsCalculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static string EnablePerformAllCalculationsInGroup(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            return AllDataAvailable(context.AssessmentSection, context.FailureMechanism);
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

            return builder.AddPerformCalculationItem(calculation, context, Calculate, EnablePerformCalculation)
                          .AddClearCalculationOutputItem(calculation)
                          .AddSeparator()
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

        private static string EnablePerformCalculation(GrassCoverErosionInwardsCalculationContext context)
        {
            return AllDataAvailable(context.AssessmentSection, context.FailureMechanism);
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
                calculationGroupContext.NotifyObservers();
            }
        }

        #endregion
    }
}