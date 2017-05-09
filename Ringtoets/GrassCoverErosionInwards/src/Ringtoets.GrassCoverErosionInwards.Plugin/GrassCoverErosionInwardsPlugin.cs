// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Utils;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.IO.Exporters;
using Ringtoets.GrassCoverErosionInwards.IO.Importers;
using Ringtoets.GrassCoverErosionInwards.Plugin.FileImporters;
using Ringtoets.GrassCoverErosionInwards.Plugin.Properties;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.GrassCoverErosionInwards.Utils;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanismContextProperties>
            {
                CreateInstance = context => new GrassCoverErosionInwardsFailureMechanismContextProperties(
                    context,
                    new FailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism>())
            };
            yield return new PropertyInfo<DikeProfile, DikeProfileProperties>();
            yield return new PropertyInfo<GrassCoverErosionInwardsInputContext, GrassCoverErosionInwardsInputContextProperties>
            {
                CreateInstance = context => new GrassCoverErosionInwardsInputContextProperties(
                    context,
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
            yield return new PropertyInfo<GrassCoverErosionInwardsOutput, GrassCoverErosionInwardsOutputProperties>();
            yield return new PropertyInfo<DikeProfilesContext, DikeProfileCollectionProperties>
            {
                CreateInstance = context => new DikeProfileCollectionProperties(context.WrappedData)
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<GrassCoverErosionInwardsCalculationGroupContext>(
                (context, filePath) => new GrassCoverErosionInwardsCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AvailableHydraulicBoundaryLocations,
                    context.AvailableDikeProfiles,
                    context.FailureMechanism));

            yield return new ImportInfo<DikeProfilesContext>
            {
                CreateFileImporter = (context, filePath) => new DikeProfilesImporter(context.WrappedData,
                                                                                     context.ParentAssessmentSection.ReferenceLine,
                                                                                     filePath,
                                                                                     new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(context.ParentFailureMechanism),
                                                                                     new ImportMessageProvider()),
                Name = RingtoetsCommonIOResources.DikeProfilesImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.DikeProfile,
                FileFilterGenerator = DikeProfileImporterFileFilterGenerator(),
                IsEnabled = IsDikeProfileImporterEnabled,
                VerifyUpdates = context => VerifyDikeProfilesShouldUpdate(context,
                                                                          Resources.GrassCoverErosionInwardsPlugin_VerifyDikeProfileImport_When_importing_DikeProfiles_Calculation_output_will_be_cleared_confirm)
            };
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<DikeProfilesContext>
            {
                CreateFileImporter = (context, filePath) => new DikeProfilesImporter(context.WrappedData,
                                                                                     context.ParentAssessmentSection.ReferenceLine,
                                                                                     filePath,
                                                                                     new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(context.ParentFailureMechanism),
                                                                                     new UpdateMessageProvider()),
                Name = RingtoetsCommonIOResources.DikeProfilesImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.DikeProfile,
                FileFilterGenerator = DikeProfileImporterFileFilterGenerator(),
                CurrentPath = context => context.WrappedData.SourcePath,
                IsEnabled = IsDikeProfileImporterEnabled,
                VerifyUpdates = context => VerifyDikeProfilesShouldUpdate(context,
                                                                          Resources.GrassCoverErosionInwardsPlugin_VerifyDikeProfileUpdate_When_updating_Calculation_with_DikeProfile_data_output_will_be_cleared_confirm)
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RingtoetsExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<GrassCoverErosionInwardsCalculationGroupContext>(
                (context, filePath) => new GrassCoverErosionInwardsCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo<GrassCoverErosionInwardsCalculationContext>(
                (context, filePath) => new GrassCoverErosionInwardsCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath));
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanismView>
            {
                GetViewName = (view, mechanism) => mechanism.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseGrassCoverErosionInwardsFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

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
                GetViewName = (view, results) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
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
                Text = context => RingtoetsCommonFormsResources.DikeProfiles_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.Any() ?
                                           Color.FromKnownColor(KnownColor.ControlText) :
                                           Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = context => context.WrappedData
                                                     .Cast<object>()
                                                     .ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddUpdateItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
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
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                calculations.Select(calc =>
                                        new GrassCoverErosionInwardsCalculationActivity(
                                            calc,
                                            assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                            failureMechanism,
                                            assessmentSection)).ToArray());
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, IFailureMechanism failureMechanism)
        {
            if (failureMechanism.Contribution <= 0.0)
            {
                return RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero;
            }

            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        #region GrassCoverErosionInwardsFailureMechanismView ViewInfo

        private static bool CloseGrassCoverErosionInwardsFailureMechanismViewForData(GrassCoverErosionInwardsFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as GrassCoverErosionInwardsFailureMechanism;

            var viewFailureMechanismContext = (GrassCoverErosionInwardsFailureMechanismContext) view.Data;
            GrassCoverErosionInwardsFailureMechanism viewFailureMechanism = viewFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewFailureMechanism, failureMechanism);
        }

        #endregion

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

        private static bool CloseInputViewForData(GrassCoverErosionInwardsInputView view, object o)
        {
            var calculationContext = o as GrassCoverErosionInwardsCalculationContext;
            if (calculationContext != null)
            {
                return ReferenceEquals(view.Data, calculationContext.WrappedData);
            }
            var calculation = o as GrassCoverErosionInwardsCalculation;
            if (calculation != null)
            {
                return ReferenceEquals(view.Data, calculation);
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

        private static void ValidateAll(IEnumerable<GrassCoverErosionInwardsCalculation> grassCoverErosionInwardsCalculations, IAssessmentSection assessmentSection)
        {
            foreach (GrassCoverErosionInwardsCalculation calculation in grassCoverErosionInwardsCalculations)
            {
                GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);
            }
        }

        #region GrassCoverErosionInwardsFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext)
        {
            GrassCoverErosionInwardsFailureMechanism wrappedData = grassCoverErosionInwardsFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new GrassCoverErosionInwardsCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext)
        {
            return new object[]
            {
                grassCoverErosionInwardsFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IList GetInputs(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IList GetOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new GrassCoverErosionInwardsScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl));

            return builder
                .AddOpenItem()
                .AddSeparator()
                .AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext, RemoveAllViewsForItem)
                .AddSeparator()
                .AddValidateAllCalculationsInFailureMechanismItem(
                    grassCoverErosionInwardsFailureMechanismContext,
                    ValidateAll,
                    ValidateAllDataAvailableAndGetErrorMessage)
                .AddPerformAllCalculationsInFailureMechanismItem(
                    grassCoverErosionInwardsFailureMechanismContext,
                    CalculateAll,
                    ValidateAllDataAvailableAndGetErrorMessage)
                .AddSeparator()
                .AddClearAllCalculationOutputInFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
                .AddSeparator()
                .AddCollapseAllItem()
                .AddExpandAllItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }

        private void RemoveAllViewsForItem(GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext,
                                                                    RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent, context.WrappedData);
        }

        private static void ValidateAll(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<GrassCoverErosionInwardsCalculation>(), context.Parent);
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
            bool isNestedGroup = parentData is GrassCoverErosionInwardsCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGenerateCalculationsItem(context);
            StrictContextMenuItem updateDikeProfileItem = CreateUpdateDikeProfileItem(context);

            builder.AddImportItem()
                   .AddExportItem()
                   .AddSeparator();

            if (!isNestedGroup)
            {
                builder.AddCustomItem(generateCalculationsItem)
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            builder.AddCustomItem(updateDikeProfileItem)
                   .AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(
                       context,
                       ValidateAll,
                       ValidateAllDataAvailableAndGetErrorMessage)
                   .AddPerformAllCalculationsInGroupItem(
                       group,
                       context,
                       CalculateAll,
                       ValidateAllDataAvailableAndGetErrorMessage)
                   .AddSeparator()
                   .AddClearAllCalculationOutputInGroupItem(group);

            if (isNestedGroup)
            {
                builder.AddDeleteItem();
            }
            else
            {
                builder.AddRemoveAllChildrenItem();
            }

            return builder.AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private StrictContextMenuItem CreateUpdateDikeProfileItem(GrassCoverErosionInwardsCalculationGroupContext nodeData)
        {
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations = nodeData.WrappedData
                                                                                    .GetCalculations()
                                                                                    .OfType<GrassCoverErosionInwardsCalculation>();

            var isContextMenuItemEnabled = true;
            string toolTipText =
                Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_all_calculations_with_DikeProfile_Tooltip;
            if (!calculations.Any())
            {
                toolTipText = RingtoetsCommonFormsResources.CreateUpdateContextMenuItem_No_calculations_to_update_ToolTip;
                isContextMenuItemEnabled = false;
            }
            else if (calculations.All(calc => calc.InputParameters.DikeProfile == null))
            {
                toolTipText = Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_No_calculations_with_DikeProfile_ToolTip;
                isContextMenuItemEnabled = false;
            }

            return new StrictContextMenuItem(Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_all_DikeProfiles,
                                             toolTipText,
                                             RingtoetsCommonFormsResources.UpdateItemIcon,
                                             (o, args) => UpdateDikeProfileDependentDataOfAllCalculations(nodeData))
            {
                Enabled = isContextMenuItemEnabled
            };
        }

        private void UpdateDikeProfileDependentDataOfAllCalculations(GrassCoverErosionInwardsCalculationGroupContext nodeData)
        {
            GrassCoverErosionInwardsCalculation[] calculations = nodeData.WrappedData
                                                                         .GetCalculations()
                                                                         .OfType<GrassCoverErosionInwardsCalculation>()
                                                                         .ToArray();

            string message =
                Resources.GrassCoverErosionInwardsPlugin_VerifyDikeProfileUpdate_Confirm_calculation_outputs_cleared_when_updating_DikeProfile_dependent_data;
            if (DikeProfileDependentDataShouldUpdate(calculations, message))
            {
                foreach (GrassCoverErosionInwardsCalculation calculation in calculations)
                {
                    UpdateDikeProfileDerivedCalculationInput(calculation);
                }
            }
        }

        private StrictContextMenuItem CreateGenerateCalculationsItem(GrassCoverErosionInwardsCalculationGroupContext nodeData)
        {
            bool isDikeProfileAvailable = nodeData.AvailableDikeProfiles.Any();

            string calculationGroupGenerateCalculationsToolTip = isDikeProfileAvailable
                                                                     ? Resources.GrassCoverErosionInwardsPlugin_CreateGenerateCalculationsItem_ToolTip
                                                                     : Resources.GrassCoverErosionInwardsPlugin_CreateGenerateCalculationsItem_NoDikeLocations_ToolTip;

            var generateCalculationsItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
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

        private static void GenerateCalculations(CalculationGroup target, GrassCoverErosionInwardsFailureMechanism failureMechanism, IEnumerable<DikeProfile> dikeProfiles)
        {
            foreach (DikeProfile profile in dikeProfiles)
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
            }

            GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(
                failureMechanism.SectionResults,
                failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>());
        }

        private static void CalculationGroupContextOnNodeRemoved(GrassCoverErosionInwardsCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (GrassCoverErosionInwardsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);

            GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(
                context.FailureMechanism.SectionResults,
                context.FailureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>().ToArray());

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

        private static string ValidateAllDataAvailableAndGetErrorMessage(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private static void ValidateAll(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>(), context.AssessmentSection);
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
                context.WrappedData.Comments,
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

            StrictContextMenuItem updateDikeProfile = CreateUpdateDikeProfileItem(context);

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddRenameItem()
                          .AddCustomItem(updateDikeProfile)
                          .AddSeparator()
                          .AddValidateCalculationItem(
                              context,
                              Validate,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddPerformCalculationItem(
                              calculation,
                              context,
                              Calculate,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddSeparator()
                          .AddClearCalculationOutputItem(calculation)
                          .AddDeleteItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(GrassCoverErosionInwardsCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private static void Validate(GrassCoverErosionInwardsCalculationContext context)
        {
            GrassCoverErosionInwardsCalculationService.Validate(context.WrappedData, context.AssessmentSection);
        }

        private void Calculate(GrassCoverErosionInwardsCalculation calculation, GrassCoverErosionInwardsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, new GrassCoverErosionInwardsCalculationActivity(calculation,
                                                                                                             context.AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                             context.FailureMechanism,
                                                                                                             context.AssessmentSection));
        }

        private static void CalculationContextOnNodeRemoved(GrassCoverErosionInwardsCalculationContext context, object parentData)
        {
            var calculationGroupContext = parentData as GrassCoverErosionInwardsCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(
                    context.FailureMechanism.SectionResults,
                    context.FailureMechanism.Calculations.OfType<GrassCoverErosionInwardsCalculation>());
                calculationGroupContext.NotifyObservers();
            }
        }

        private StrictContextMenuItem CreateUpdateDikeProfileItem(GrassCoverErosionInwardsCalculationContext context)
        {
            GrassCoverErosionInwardsInput inputParameters = context.WrappedData.InputParameters;
            bool hasDikeProfile = inputParameters.DikeProfile != null;

            string toolTipMessage = hasDikeProfile
                                        ? Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_calculation_with_DikeProfile_ToolTip
                                        : Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_calculation_no_DikeProfile_ToolTip;

            return new StrictContextMenuItem(
                Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_DikeProfile_data,
                toolTipMessage,
                RingtoetsCommonFormsResources.UpdateItemIcon,
                (o, args) => UpdateDikeProfileDependentDataOfCalculation(context.WrappedData))
            {
                Enabled = hasDikeProfile
            };
        }

        private void UpdateDikeProfileDependentDataOfCalculation(GrassCoverErosionInwardsCalculation calculation)
        {
            string message =
                Resources.GrassCoverErosionInwardsPlugin_VerifyDikeProfileUpdate_Confirm_calculation_output_cleared_when_updating_DikeProfile_dependent_data;
            if (DikeProfileDependentDataShouldUpdate(new[]
            {
                calculation
            }, message))
            {
                UpdateDikeProfileDerivedCalculationInput(calculation);
            }
        }

        private bool DikeProfileDependentDataShouldUpdate(IEnumerable<GrassCoverErosionInwardsCalculation> calculations, string query)
        {
            var changeHandler = new CalculationChangeHandler(calculations,
                                                             query,
                                                             new DialogBasedInquiryHelper(Gui.MainWindow));

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static void UpdateDikeProfileDerivedCalculationInput(GrassCoverErosionInwardsCalculation calculation)
        {
            GrassCoverErosionInwardsInput inputParameters = calculation.InputParameters;
            bool currentUseBreakWater = inputParameters.UseBreakWater;
            BreakWater currentBreakWater = inputParameters.BreakWater;
            RoundedDouble currentOrientation = inputParameters.Orientation;
            RoundedDouble currentDikeHeight = inputParameters.DikeHeight;
            bool currentUseForeshore = inputParameters.UseForeshore;

            // Reapply the dike profile will update the derived inputs
            inputParameters.DikeProfile = inputParameters.DikeProfile;

            var affectedObjects = new List<IObservable>();
            if (IsDerivedInputUpdated(currentUseBreakWater,
                                      currentBreakWater,
                                      currentOrientation,
                                      currentDikeHeight,
                                      currentUseForeshore,
                                      inputParameters))
            {
                affectedObjects.Add(inputParameters);
                affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));
            }

            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        private static bool IsDerivedInputUpdated(bool currentUseBreakWater,
                                                  BreakWater currentBreakWater,
                                                  RoundedDouble currentOrientation,
                                                  RoundedDouble currentDikeHeight,
                                                  bool currentUseForeshore,
                                                  GrassCoverErosionInwardsInput actualInput)
        {
            return currentUseBreakWater != actualInput.UseBreakWater
                   || !Equals(currentBreakWater, actualInput.BreakWater)
                   || currentOrientation != actualInput.Orientation
                   || currentDikeHeight != actualInput.DikeHeight
                   || currentUseForeshore != actualInput.UseForeshore;
        }

        #endregion

        #region Dike Profiles Importer

        private static FileFilterGenerator DikeProfileImporterFileFilterGenerator()
        {
            return new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                           RingtoetsCommonIOResources.Shape_file_filter_Description);
        }

        private static bool IsDikeProfileImporterEnabled(DikeProfilesContext context)
        {
            return context.ParentAssessmentSection.ReferenceLine != null;
        }

        private bool VerifyDikeProfilesShouldUpdate(DikeProfilesContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.ParentFailureMechanism,
                                                                             query,
                                                                             new DialogBasedInquiryHelper(Gui.MainWindow));

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion
    }
}