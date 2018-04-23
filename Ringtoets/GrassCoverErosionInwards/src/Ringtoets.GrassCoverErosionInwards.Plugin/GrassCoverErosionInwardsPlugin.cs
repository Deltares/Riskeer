﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.Plugin;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.IO.Configurations;
using Ringtoets.GrassCoverErosionInwards.Plugin.FileImporters;
using Ringtoets.GrassCoverErosionInwards.Plugin.Properties;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.GrassCoverErosionInwards.Util;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanismProperties>
            {
                CreateInstance = context => new GrassCoverErosionInwardsFailureMechanismProperties(
                    context.WrappedData,
                    new FailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism>())
            };
            yield return new PropertyInfo<DikeProfile, DikeProfileProperties>();
            yield return new PropertyInfo<GrassCoverErosionInwardsInputContext, GrassCoverErosionInwardsInputContextProperties>
            {
                CreateInstance = context => new GrassCoverErosionInwardsInputContextProperties(
                    context,
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
            yield return new PropertyInfo<DikeProfilesContext, DikeProfileCollectionProperties>
            {
                CreateInstance = context => new DikeProfileCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<GrassCoverErosionInwardsOutputContext, GrassCoverErosionInwardsOutputProperties>
            {
                CreateInstance = context => new GrassCoverErosionInwardsOutputProperties(context.WrappedData.Output,
                                                                                         context.FailureMechanism,
                                                                                         context.AssessmentSection)
            };
            yield return new PropertyInfo<OvertoppingOutputContext, OvertoppingOutputProperties>
            {
                CreateInstance = context => new OvertoppingOutputProperties(context.WrappedData.Output.OvertoppingOutput,
                                                                            context.FailureMechanism,
                                                                            context.AssessmentSection)
            };
            yield return new PropertyInfo<DikeHeightOutputContext, DikeHeightOutputProperties>
            {
                CreateInstance = context => new DikeHeightOutputProperties(context.WrappedData.Output.DikeHeightOutput)
            };
            yield return new PropertyInfo<OvertoppingRateOutputContext, OvertoppingRateOutputProperties>
            {
                CreateInstance = context => new OvertoppingRateOutputProperties(context.WrappedData.Output.OvertoppingRateOutput)
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<GrassCoverErosionInwardsCalculationGroupContext>(
                (context, filePath) => new GrassCoverErosionInwardsCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
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
                IsEnabled = context => context.ParentAssessmentSection.ReferenceLine != null,
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
                IsEnabled = context => context.WrappedData.SourcePath != null,
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
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseGrassCoverErosionInwardsFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new GrassCoverErosionInwardsFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                GrassCoverErosionInwardsScenariosContext,
                CalculationGroup,
                GrassCoverErosionInwardsScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                AfterCreate = (view, context) => view.FailureMechanism = context.ParentFailureMechanism,
                CloseForData = CloseScenariosViewForData,
                Image = RingtoetsCommonFormsResources.ScenariosIcon
            };

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>,
                IObservableEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult>,
                GrassCoverErosionInwardsFailureMechanismResultView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new GrassCoverErosionInwardsFailureMechanismResultView(
                    context.WrappedData,
                    (GrassCoverErosionInwardsFailureMechanism) context.FailureMechanism,
                    context.AssessmentSection)
            };

            yield return new ViewInfo<GrassCoverErosionInwardsInputContext, GrassCoverErosionInwardsCalculation, GrassCoverErosionInwardsInputView>
            {
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                GetViewName = (view, context) => RingtoetsCommonFormsResources.Calculation_Input,
                GetViewData = context => context.Calculation,
                CloseForData = CloseInputViewForData
            };

            yield return new ViewInfo<OvertoppingOutputContext, GrassCoverErosionInwardsCalculation, OvertoppingOutputGeneralResultFaultTreeIllustrationPointView>
            {
                Image = RingtoetsCommonFormsResources.GeneralOutputIcon,
                GetViewName = (view, context) => Resources.OvertoppingOutput_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RingtoetsPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new OvertoppingOutputGeneralResultFaultTreeIllustrationPointView(
                    () => context.WrappedData.Output?.OvertoppingOutput.GeneralResult)
            };

            yield return new ViewInfo<DikeHeightOutputContext, GrassCoverErosionInwardsCalculation, DikeHeightOutputGeneralResultFaultTreeIllustrationPointView>
            {
                Image = RingtoetsCommonFormsResources.GeneralOutputIcon,
                GetViewName = (view, context) => GrassCoverErosionInwardsFormsResources.DikeHeight_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RingtoetsPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new DikeHeightOutputGeneralResultFaultTreeIllustrationPointView(
                    () => context.WrappedData.Output?.DikeHeightOutput?.GeneralResult)
            };

            yield return new ViewInfo<OvertoppingRateOutputContext, GrassCoverErosionInwardsCalculation, OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView>
            {
                Image = RingtoetsCommonFormsResources.GeneralOutputIcon,
                GetViewName = (view, context) => GrassCoverErosionInwardsFormsResources.OvertoppingRate_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RingtoetsPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView(
                    () => context.WrappedData.Output?.OvertoppingRateOutput?.GeneralResult)
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
                ForeColor = context => context.WrappedData.Any()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
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

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>>
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

            yield return new TreeNodeInfo<GrassCoverErosionInwardsOutputContext>
            {
                Text = context => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = context => Resources.OutputIcon,
                ForeColor = context => context.WrappedData.HasOutput
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = OutputChildNodeObjects,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<OvertoppingOutputContext>
            {
                Text = output => Resources.OvertoppingOutput_DisplayName,
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = context => context.WrappedData.Output?.OvertoppingOutput != null
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<DikeHeightOutputContext>
            {
                Text = output => GrassCoverErosionInwardsFormsResources.DikeHeight_DisplayName,
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = context => context.WrappedData.Output?.DikeHeightOutput != null
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<OvertoppingRateOutputContext>
            {
                Text = output => GrassCoverErosionInwardsFormsResources.OvertoppingRate_DisplayName,
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = context => context.WrappedData.Output?.OvertoppingRateOutput != null
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
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

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
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
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        #region GrassCoverErosionInwardsInputView ViewInfo

        private static bool CloseInputViewForData(GrassCoverErosionInwardsInputView view, object o)
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

        private static void ValidateAll(IEnumerable<GrassCoverErosionInwardsCalculation> grassCoverErosionInwardsCalculations, IAssessmentSection assessmentSection)
        {
            foreach (GrassCoverErosionInwardsCalculation calculation in grassCoverErosionInwardsCalculations)
            {
                GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);
            }
        }

        #region GrassCoverErosionInwardsOutputContext TreeNodeInfo

        private static object[] OutputChildNodeObjects(GrassCoverErosionInwardsOutputContext context)
        {
            GrassCoverErosionInwardsCalculation calculation = context.WrappedData;

            return new object[]
            {
                new OvertoppingOutputContext(calculation, context.FailureMechanism, context.AssessmentSection),
                new DikeHeightOutputContext(calculation),
                new OvertoppingRateOutputContext(calculation)
            };
        }

        #endregion

        #region GrassCoverErosionInwardsFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            GrassCoverErosionInwardsFailureMechanism wrappedData = context.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, context.Parent), TreeFolderCategory.Input),
                new GrassCoverErosionInwardsCalculationGroupContext(wrappedData.CalculationsGroup, null, wrappedData, context.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData, context.Parent), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext)
        {
            return new object[]
            {
                grassCoverErosionInwardsFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverErosionInwardsScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new ProbabilityFailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
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
                                                                                        context.WrappedData,
                                                                                        context.FailureMechanism,
                                                                                        context.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                             context.WrappedData,
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

            GrassCoverErosionInwardsCalculation[] calculations = context.WrappedData
                                                                        .GetCalculations()
                                                                        .OfType<GrassCoverErosionInwardsCalculation>()
                                                                        .ToArray();
            StrictContextMenuItem updateDikeProfileItem = CreateUpdateDikeProfileItem(calculations);

            builder.AddImportItem()
                   .AddExportItem()
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddDuplicateCalculationItem(group, context)
                       .AddSeparator();
            }
            else
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

        private StrictContextMenuItem CreateUpdateDikeProfileItem(IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_all_calculations_with_DikeProfile_Tooltip;

            GrassCoverErosionInwardsCalculation[] calculationsToUpdate = calculations
                                                                         .Where(calc => calc.InputParameters.DikeProfile != null
                                                                                        && !calc.InputParameters.IsDikeProfileInputSynchronized)
                                                                         .ToArray();

            if (!calculationsToUpdate.Any())
            {
                contextMenuEnabled = false;
                toolTipMessage = RingtoetsCommonFormsResources.CreateUpdateContextMenuItem_No_calculations_to_update_ToolTip;
            }

            return new StrictContextMenuItem(Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_all_DikeProfiles,
                                             toolTipMessage,
                                             RingtoetsCommonFormsResources.UpdateItemIcon,
                                             (o, args) => UpdateDikeProfileDependentDataOfAllCalculations(calculationsToUpdate))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateDikeProfileDependentDataOfAllCalculations(IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            string message = RingtoetsCommonFormsResources.VerifyUpdate_Confirm_calculation_outputs_cleared;
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
                RingtoetsCommonFormsResources.GenerateScenariosIcon, (o, args) => ShowDikeProfileSelectionDialog(nodeData))
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
            GrassCoverErosionInwardsCalculation calculation = context.WrappedData;

            return new object[]
            {
                calculation.Comments,
                new GrassCoverErosionInwardsInputContext(calculation.InputParameters,
                                                         calculation,
                                                         context.FailureMechanism,
                                                         context.AssessmentSection),
                new GrassCoverErosionInwardsOutputContext(calculation,
                                                          context.FailureMechanism,
                                                          context.AssessmentSection)
            };
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(GrassCoverErosionInwardsCalculationContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            GrassCoverErosionInwardsCalculation calculation = context.WrappedData;

            StrictContextMenuItem updateDikeProfile = CreateUpdateDikeProfileItem(context);

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, context)
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
            var contextMenuEnabled = true;
            string toolTipMessage = Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_calculation_with_DikeProfile_ToolTip;
            if (context.WrappedData.InputParameters.DikeProfile == null)
            {
                contextMenuEnabled = false;
                toolTipMessage = Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_calculation_no_DikeProfile_ToolTip;
            }
            else if (context.WrappedData.InputParameters.IsDikeProfileInputSynchronized)
            {
                contextMenuEnabled = false;
                toolTipMessage = RingtoetsCommonFormsResources.CalculationItem_No_changes_to_update_ToolTip;
            }

            return new StrictContextMenuItem(
                Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_DikeProfile_data,
                toolTipMessage,
                RingtoetsCommonFormsResources.UpdateItemIcon,
                (o, args) => UpdateDikeProfileDependentDataOfCalculation(context.WrappedData))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateDikeProfileDependentDataOfCalculation(GrassCoverErosionInwardsCalculation calculation)
        {
            string message = RingtoetsCommonFormsResources.VerifyUpdate_Confirm_calculation_output_cleared;
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
            calculation.InputParameters.SynchronizeDikeProfileInput();

            var affectedObjects = new List<IObservable>
            {
                calculation.InputParameters
            };

            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));

            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        #endregion

        #region Dike Profiles Importer

        private static FileFilterGenerator DikeProfileImporterFileFilterGenerator()
        {
            return new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                           RingtoetsCommonIOResources.Shape_file_filter_Description);
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