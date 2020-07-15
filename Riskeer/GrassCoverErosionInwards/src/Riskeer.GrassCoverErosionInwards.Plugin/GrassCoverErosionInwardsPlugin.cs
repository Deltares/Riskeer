// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.Plugin;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms;
using Riskeer.GrassCoverErosionInwards.Forms.ChangeHandlers;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionInwards.Forms.Views;
using Riskeer.GrassCoverErosionInwards.IO.Configurations;
using Riskeer.GrassCoverErosionInwards.Plugin.FileImporters;
using Riskeer.GrassCoverErosionInwards.Plugin.Properties;
using Riskeer.GrassCoverErosionInwards.Service;
using Riskeer.GrassCoverErosionInwards.Util;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Riskeer.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Plugin
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
            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<GrassCoverErosionInwardsCalculationGroupContext>(
                (context, filePath) => new GrassCoverErosionInwardsCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
                    context.AvailableDikeProfiles));

            yield return new ImportInfo<DikeProfilesContext>
            {
                CreateFileImporter = (context, filePath) => new DikeProfilesImporter(context.WrappedData,
                                                                                     context.ParentAssessmentSection.ReferenceLine,
                                                                                     filePath,
                                                                                     new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(context.ParentFailureMechanism),
                                                                                     new ImportMessageProvider()),
                Name = RiskeerCommonIOResources.DikeProfilesImporter_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.DikeProfile,
                FileFilterGenerator = DikeProfileImporterFileFilterGenerator(),
                IsEnabled = context => context.ParentAssessmentSection.ReferenceLine.Points.Any(),
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
                Name = RiskeerCommonIOResources.DikeProfilesImporter_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.DikeProfile,
                FileFilterGenerator = DikeProfileImporterFileFilterGenerator(),
                CurrentPath = context => context.WrappedData.SourcePath,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                VerifyUpdates = context => VerifyDikeProfilesShouldUpdate(context,
                                                                          Resources.GrassCoverErosionInwardsPlugin_VerifyDikeProfileUpdate_When_updating_Calculation_with_DikeProfile_data_output_will_be_cleared_confirm)
            };

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                GrassCoverErosionInwardsFailureMechanismSectionsContext, GrassCoverErosionInwardsFailureMechanism, GrassCoverErosionInwardsFailureMechanismSectionResult>(
                new GrassCoverErosionInwardsFailureMechanismSectionResultUpdateStrategy());
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<GrassCoverErosionInwardsCalculationGroupContext>(
                (context, filePath) => new GrassCoverErosionInwardsCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<GrassCoverErosionInwardsCalculationScenarioContext>(
                (context, filePath) => new GrassCoverErosionInwardsCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                GetInquiryHelper());
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.CalculationIcon,
                CloseForData = CloseFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new GrassCoverErosionInwardsFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                GrassCoverErosionInwardsScenariosContext,
                CalculationGroup,
                GrassCoverErosionInwardsScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Scenarios_DisplayName,
                CreateInstance = context => new GrassCoverErosionInwardsScenariosView(context.WrappedData, context.ParentFailureMechanism, context.AssessmentSection),
                CloseForData = CloseScenariosViewForData,
                Image = RiskeerCommonFormsResources.ScenariosIcon
            };

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>,
                IObservableEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult>,
                GrassCoverErosionInwardsFailureMechanismResultView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new GrassCoverErosionInwardsFailureMechanismResultView(
                    context.WrappedData,
                    (GrassCoverErosionInwardsFailureMechanism) context.FailureMechanism,
                    context.AssessmentSection)
            };

            yield return new ViewInfo<GrassCoverErosionInwardsInputContext, GrassCoverErosionInwardsCalculation, GrassCoverErosionInwardsInputView>
            {
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                GetViewData = context => context.Calculation,
                CloseForData = CloseInputViewForData
            };

            yield return new ViewInfo<OvertoppingOutputContext, GrassCoverErosionInwardsCalculation, OvertoppingOutputGeneralResultFaultTreeIllustrationPointView>
            {
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                GetViewName = (view, context) => Resources.OvertoppingOutput_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new OvertoppingOutputGeneralResultFaultTreeIllustrationPointView(
                    () => context.WrappedData.Output?.OvertoppingOutput.GeneralResult)
            };

            yield return new ViewInfo<DikeHeightOutputContext, GrassCoverErosionInwardsCalculation, DikeHeightOutputGeneralResultFaultTreeIllustrationPointView>
            {
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                GetViewName = (view, context) => GrassCoverErosionInwardsFormsResources.DikeHeight_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new DikeHeightOutputGeneralResultFaultTreeIllustrationPointView(
                    () => context.WrappedData.Output?.DikeHeightOutput?.GeneralResult)
            };

            yield return new ViewInfo<OvertoppingRateOutputContext, GrassCoverErosionInwardsCalculation, OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView>
            {
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                GetViewName = (view, context) => GrassCoverErosionInwardsFormsResources.OvertoppingRate_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView(
                    () => context.WrappedData.Output?.OvertoppingRateOutput?.GeneralResult)
            };

            yield return new ViewInfo<GrassCoverErosionInwardsCalculationGroupContext, CalculationGroup, GrassCoverErosionInwardsCalculationsView>
            {
                CreateInstance = context => new GrassCoverErosionInwardsCalculationsView(context.WrappedData,context.FailureMechanism, context.AssessmentSection),
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.GeneralFolderIcon,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CloseForData = CloseCalculationsViewForData,
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<GrassCoverErosionInwardsFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<DikeProfilesContext>
            {
                Text = context => RiskeerCommonFormsResources.DikeProfiles_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
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

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<GrassCoverErosionInwardsCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<GrassCoverErosionInwardsCalculationScenarioContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<GrassCoverErosionInwardsScenariosContext>
            {
                Text = context => RiskeerCommonFormsResources.Scenarios_DisplayName,
                Image = context => RiskeerCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionInwardsInputContext>
            {
                Text = inputContext => RiskeerCommonFormsResources.Calculation_Input,
                Image = inputContext => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionInwardsOutputContext>
            {
                Text = context => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
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
                Image = output => RiskeerCommonFormsResources.GeneralOutputIcon,
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
                Image = output => RiskeerCommonFormsResources.GeneralOutputIcon,
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
                Image = output => RiskeerCommonFormsResources.GeneralOutputIcon,
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

        #region ViewInfos

        private static bool CloseFailureMechanismViewForData(GrassCoverErosionInwardsFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as GrassCoverErosionInwardsFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        private static bool CloseScenariosViewForData(GrassCoverErosionInwardsScenariosView view, object removedData)
        {
            var failureMechanism = removedData as GrassCoverErosionInwardsFailureMechanism;

            if (removedData is GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (removedData is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<GrassCoverErosionInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

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

        private static bool CloseInputViewForData(GrassCoverErosionInwardsInputView view, object o)
        {
            if (o is GrassCoverErosionInwardsCalculationScenarioContext calculationContext)
            {
                return ReferenceEquals(view.Data, calculationContext.WrappedData);
            }

            IEnumerable<GrassCoverErosionInwardsCalculation> calculations = null;

            if (o is GrassCoverErosionInwardsCalculationGroupContext calculationGroupContext)
            {
                calculations = calculationGroupContext.WrappedData.GetCalculations()
                                                      .OfType<GrassCoverErosionInwardsCalculation>();
            }

            var failureMechanism = o as GrassCoverErosionInwardsFailureMechanism;

            if (o is GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (o is IAssessmentSection assessmentSection)
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

        private static bool CloseCalculationsViewForData(GrassCoverErosionInwardsCalculationsView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as GrassCoverErosionInwardsFailureMechanism;

            if (o is GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                                            .OfType<GrassCoverErosionInwardsFailureMechanism>()
                                                                            .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        #endregion

        #region TreeNodeInfos

        #region GrassCoverErosionInwardsFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            GrassCoverErosionInwardsFailureMechanism wrappedData = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, assessmentSection), TreeFolderCategory.Input),
                new GrassCoverErosionInwardsCalculationGroupContext(wrappedData.CalculationsGroup, null, wrappedData, assessmentSection),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                failureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverErosionInwardsFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralInput.N),
                new GrassCoverErosionInwardsScenariosContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection),
                new ProbabilityFailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations = failureMechanismContext.WrappedData
                                                                                                                           .Calculations
                                                                                                                           .Cast<GrassCoverErosionInwardsCalculation>();
            IInquiryHelper inquiryHelper = GetInquiryHelper();

            var builder = new RiskeerContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));
            return builder
                   .AddOpenItem()
                   .AddSeparator()
                   .AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                   .AddSeparator()
                   .AddValidateAllCalculationsInFailureMechanismItem(
                       failureMechanismContext,
                       ValidateAllInFailureMechanism,
                       EnableValidateAndCalculateMenuItemForFailureMechanism)
                   .AddPerformAllCalculationsInFailureMechanismItem(
                       failureMechanismContext,
                       CalculateAllInFailureMechanism,
                       EnableValidateAndCalculateMenuItemForFailureMechanism)
                   .AddSeparator()
                   .AddClearAllCalculationOutputInFailureMechanismItem(failureMechanismContext.WrappedData)
                   .AddClearIllustrationPointsOfCalculationsInFailureMechanismItem(
                       () => GrassCoverErosionInwardsIllustrationPointsHelper.HasIllustrationPoints(calculations),
                       CreateChangeHandler(inquiryHelper, calculations))
                   .AddSeparator()
                   .AddCollapseAllItem()
                   .AddExpandAllItem()
                   .AddSeparator()
                   .AddPropertiesItem()
                   .Build();
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext,
                                                                    RemoveAllViewsForItem)
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

        private static string EnableValidateAndCalculateMenuItemForFailureMechanism(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.Parent);
        }

        private static void ValidateAllInFailureMechanism(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<GrassCoverErosionInwardsCalculation>(), context.WrappedData, context.Parent);
        }

        private void CalculateAllInFailureMechanism(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData, context.Parent));
        }

        #endregion

        #region GrassCoverErosionInwardsCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                var calculation = calculationItem as GrassCoverErosionInwardsCalculationScenario;
                var group = calculationItem as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionInwardsCalculationScenarioContext(calculation,
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
            bool isNestedGroup = parentData is GrassCoverErosionInwardsCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGenerateCalculationsItem(context);

            GrassCoverErosionInwardsCalculation[] calculations = context.WrappedData
                                                                        .GetCalculations()
                                                                        .OfType<GrassCoverErosionInwardsCalculation>()
                                                                        .ToArray();
            StrictContextMenuItem updateDikeProfileItem = CreateUpdateAllDikeProfilesItem(calculations);

            IInquiryHelper inquiryHelper = GetInquiryHelper();

            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            if (!isNestedGroup)
            {
                builder.AddOpenItem()
                       .AddSeparator();
            }

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
                       ValidateAllInCalculationGroup,
                       EnableValidateAndCalculateMenuItemForCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(
                       group,
                       context,
                       CalculateAllInCalculationGroup,
                       EnableValidateAndCalculateMenuItemForCalculationGroup)
                   .AddSeparator()
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddClearIllustrationPointsOfCalculationsInGroupItem(() => GrassCoverErosionInwardsIllustrationPointsHelper.HasIllustrationPoints(calculations),
                                                                        CreateChangeHandler(inquiryHelper, calculations));

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

        private StrictContextMenuItem CreateUpdateAllDikeProfilesItem(IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
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
                toolTipMessage = RiskeerCommonFormsResources.CreateUpdateContextMenuItem_No_calculations_to_update_ToolTip;
            }

            return new StrictContextMenuItem(Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_all_DikeProfiles,
                                             toolTipMessage,
                                             RiskeerCommonFormsResources.UpdateItemIcon,
                                             (o, args) => UpdateDikeProfileDependentDataOfAllCalculations(calculationsToUpdate))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateDikeProfileDependentDataOfAllCalculations(IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            string message = RiskeerCommonFormsResources.VerifyUpdate_Confirm_calculation_outputs_cleared;
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
                RiskeerCommonFormsResources.CalculationGroup_Generate_calculations,
                calculationGroupGenerateCalculationsToolTip,
                RiskeerCommonFormsResources.GenerateScenariosIcon, (o, args) => ShowDikeProfileSelectionDialog(nodeData))
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
                GenerateCalculations(nodeData.WrappedData, view.SelectedItems);
            }

            nodeData.NotifyObservers();
        }

        private static void GenerateCalculations(CalculationGroup target, IEnumerable<DikeProfile> dikeProfiles)
        {
            foreach (DikeProfile profile in dikeProfiles)
            {
                var calculation = new GrassCoverErosionInwardsCalculationScenario
                {
                    Name = NamingHelper.GetUniqueName(target.Children, profile.Name, c => c.Name),
                    InputParameters =
                    {
                        DikeProfile = profile
                    }
                };
                target.Children.Add(calculation);
            }
        }

        private static void AddCalculation(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            var calculation = new GrassCoverErosionInwardsCalculationScenario
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, RiskeerCommonDataResources.Calculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static void CalculationGroupContextOnNodeRemoved(GrassCoverErosionInwardsCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (GrassCoverErosionInwardsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        private static string EnableValidateAndCalculateMenuItemForCalculationGroup(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void ValidateAllInCalculationGroup(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>(),
                        context.FailureMechanism,
                        context.AssessmentSection);
        }

        private void CalculateAllInCalculationGroup(CalculationGroup group, GrassCoverErosionInwardsCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                               context.FailureMechanism,
                                                                                               context.AssessmentSection));
        }

        #endregion

        #region GrassCoverErosionInwardsCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(GrassCoverErosionInwardsCalculationScenarioContext context)
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

        private ContextMenuStrip CalculationContextContextMenuStrip(GrassCoverErosionInwardsCalculationScenarioContext context, object parentData, TreeViewControl treeViewControl)
        {
            GrassCoverErosionInwardsCalculation calculation = context.WrappedData;
            var changeHandler = new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandler(GetInquiryHelper(),
                                                                                                              calculation);

            StrictContextMenuItem updateDikeProfile = CreateUpdateDikeProfileItem(context);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));
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
                              EnableValidateAndCalculateMenuItemForCalculation)
                          .AddPerformCalculationItem(
                              calculation,
                              context,
                              Calculate,
                              EnableValidateAndCalculateMenuItemForCalculation)
                          .AddSeparator()
                          .AddClearCalculationOutputItem(calculation)
                          .AddClearIllustrationPointsOfCalculationItem(() => GrassCoverErosionInwardsIllustrationPointsHelper.HasIllustrationPoints(calculation),
                                                                       changeHandler)
                          .AddDeleteItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static string EnableValidateAndCalculateMenuItemForCalculation(GrassCoverErosionInwardsCalculationScenarioContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void Validate(GrassCoverErosionInwardsCalculationScenarioContext context)
        {
            GrassCoverErosionInwardsCalculationService.Validate(context.WrappedData, context.FailureMechanism, context.AssessmentSection);
        }

        private void Calculate(GrassCoverErosionInwardsCalculation calculation, GrassCoverErosionInwardsCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                          context.FailureMechanism,
                                                                                                                          context.AssessmentSection));
        }

        private static void CalculationContextOnNodeRemoved(GrassCoverErosionInwardsCalculationScenarioContext context, object parentData)
        {
            if (parentData is GrassCoverErosionInwardsCalculationGroupContext calculationGroupContext)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                calculationGroupContext.NotifyObservers();
            }
        }

        private StrictContextMenuItem CreateUpdateDikeProfileItem(GrassCoverErosionInwardsCalculationScenarioContext context)
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
                toolTipMessage = RiskeerCommonFormsResources.CalculationItem_No_changes_to_update_ToolTip;
            }

            return new StrictContextMenuItem(
                Resources.GrassCoverErosionInwardsPlugin_CreateUpdateDikeProfileItem_Update_DikeProfile_data,
                toolTipMessage,
                RiskeerCommonFormsResources.UpdateItemIcon,
                (o, args) => UpdateDikeProfileDependentDataOfCalculation(context.WrappedData))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateDikeProfileDependentDataOfCalculation(GrassCoverErosionInwardsCalculation calculation)
        {
            string message = RiskeerCommonFormsResources.VerifyUpdate_Confirm_calculation_output_cleared;
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
            var changeHandler = new CalculationChangeHandler(calculations, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static void UpdateDikeProfileDerivedCalculationInput(GrassCoverErosionInwardsCalculation calculation)
        {
            calculation.InputParameters.SynchronizeDikeProfileInput();

            var affectedObjects = new List<IObservable>
            {
                calculation.InputParameters
            };

            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));

            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        #endregion

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

        private static ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandler CreateChangeHandler(
            IInquiryHelper inquiryHelper, IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            return new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandler(inquiryHelper, calculations);
        }

        private static void ValidateAll(IEnumerable<GrassCoverErosionInwardsCalculation> calculations,
                                        GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                        IAssessmentSection assessmentSection)
        {
            foreach (GrassCoverErosionInwardsCalculation calculation in calculations)
            {
                GrassCoverErosionInwardsCalculationService.Validate(calculation, failureMechanism, assessmentSection);
            }
        }

        private static string EnableValidateAndCalculateMenuItem(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        #endregion

        #region ImportInfos

        #region Dike Profiles Importer

        private static FileFilterGenerator DikeProfileImporterFileFilterGenerator()
        {
            return new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                           RiskeerCommonIOResources.Shape_file_filter_Description);
        }

        private bool VerifyDikeProfilesShouldUpdate(DikeProfilesContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.ParentFailureMechanism, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion

        #endregion
    }
}