// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.Controls.TreeView;
using Core.Common.Util;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.ProgressDialog;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.Forms.PropertyClasses;
using Riskeer.ClosingStructures.Forms.Views;
using Riskeer.ClosingStructures.IO;
using Riskeer.ClosingStructures.IO.Configurations;
using Riskeer.ClosingStructures.Plugin.FileImporters;
using Riskeer.ClosingStructures.Service;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.Structures;
using Riskeer.Common.Plugin;
using Riskeer.Common.Plugin.FileImporters;
using Riskeer.Common.Service;
using Riskeer.Common.Util;
using Riskeer.Common.Util.Helpers;
using CalculationsStateFailureMechanismContext = Riskeer.ClosingStructures.Forms.PresentationObjects.CalculationsState.ClosingStructuresFailureMechanismContext;
using RegistrationStateFailureMechanismContext = Riskeer.ClosingStructures.Forms.PresentationObjects.RegistrationState.ClosingStructuresFailureMechanismContext;
using CalculationsStateFailureMechanismProperties = Riskeer.ClosingStructures.Forms.PropertyClasses.CalculationsState.ClosingStructuresFailureMechanismProperties;
using RegistrationStateFailureMechanismProperties = Riskeer.ClosingStructures.Forms.PropertyClasses.RegistrationState.ClosingStructuresFailureMechanismProperties;
using CalculationsStateFailureMechanismView = Riskeer.ClosingStructures.Forms.Views.CalculationsState.ClosingStructuresFailureMechanismView;
using RegistrationStateFailureMechanismView = Riskeer.ClosingStructures.Forms.Views.RegistrationState.ClosingStructuresFailureMechanismView;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.ClosingStructures.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="ClosingStructuresFailureMechanism"/>.
    /// </summary>
    public class ClosingStructuresPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<CalculationsStateFailureMechanismContext, CalculationsStateFailureMechanismProperties>
            {
                CreateInstance = context => new CalculationsStateFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<RegistrationStateFailureMechanismContext, RegistrationStateFailureMechanismProperties>
            {
                CreateInstance = context => new RegistrationStateFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<ClosingStructure, ClosingStructureProperties>();
            yield return new PropertyInfo<ClosingStructuresContext, StructureCollectionProperties<ClosingStructure>>
            {
                CreateInstance = context => new StructureCollectionProperties<ClosingStructure>(context.WrappedData)
            };
            yield return new PropertyInfo<ClosingStructuresInputContext, ClosingStructuresInputContextProperties>
            {
                CreateInstance = context => new ClosingStructuresInputContextProperties(
                    context,
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new RiskeerViewInfo<CalculationsStateFailureMechanismContext, CalculationsStateFailureMechanismView>(() => Gui)
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                CreateInstance = context => new CalculationsStateFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new RiskeerViewInfo<RegistrationStateFailureMechanismContext, RegistrationStateFailureMechanismView>(() => Gui)
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                AdditionalDataCheck = context => context.WrappedData.InAssembly,
                CreateInstance = context => new RegistrationStateFailureMechanismView(context.WrappedData, context.Parent),
                CloseForData = CloseRegistrationStateFailureMechanismViewForData
            };

            yield return new RiskeerViewInfo<
                ClosingStructuresFailureMechanismSectionResultContext,
                IObservableEnumerable<AdoptableFailureMechanismSectionResult>,
                StructuresFailureMechanismResultView<ClosingStructuresFailureMechanism, ClosingStructuresInput>>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new StructuresFailureMechanismResultView<ClosingStructuresFailureMechanism, ClosingStructuresInput>(
                    context.WrappedData, (ClosingStructuresFailureMechanism) context.FailureMechanism, context.AssessmentSection,
                    ClosingStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism)
            };

            yield return new RiskeerViewInfo<ClosingStructuresScenariosContext, CalculationGroup, ClosingStructuresScenariosView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Scenarios_DisplayName,
                CreateInstance = context => new ClosingStructuresScenariosView(context.WrappedData, context.ParentFailureMechanism),
                CloseForData = CloseScenariosViewForData
            };

            yield return new RiskeerViewInfo<ClosingStructuresCalculationGroupContext, CalculationGroup, ClosingStructuresCalculationsView>(() => Gui)
            {
                CreateInstance = context => new ClosingStructuresCalculationsView(context.WrappedData, context.FailureMechanism, context.AssessmentSection),
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => context.WrappedData.Name,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CloseForData = CloseCalculationsViewForData
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismStateContextTreeNodeInfo<CalculationsStateFailureMechanismContext>(
                CalculationsStateFailureMechanismChildNodeObjects,
                CalculationsStateFailureMechanismContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateRegistrationStateContextTreeNodeInfo<RegistrationStateFailureMechanismContext>(
                RegistrationStateFailureMechanismEnabledChildNodeObjects,
                RegistrationStateFailureMechanismDisabledChildNodeObjects,
                RegistrationStateFailureMechanismEnabledContextMenuStrip,
                RegistrationStateFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<ClosingStructuresCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<ClosingStructuresCalculationScenarioContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved,
                CalculationType.Probabilistic);

            yield return new TreeNodeInfo<ClosingStructuresFailureMechanismSectionResultContext>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ClosingStructuresContext>
            {
                Text = context => RiskeerCommonFormsResources.StructuresCollection_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = context => context.WrappedData.Cast<object>().ToArray(),
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

            yield return new TreeNodeInfo<ClosingStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RiskeerCommonFormsResources.StructuresIcon,
                ContextMenuStrip = (structure, parentData, treeViewControl) => Gui.Get(structure, treeViewControl)
                                                                                  .AddPropertiesItem()
                                                                                  .Build()
            };

            yield return new TreeNodeInfo<ClosingStructuresInputContext>
            {
                Text = inputContext => RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                Image = inputContext => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ClosingStructuresScenariosContext>
            {
                Text = context => RiskeerCommonFormsResources.Scenarios_DisplayName,
                Image = context => RiskeerCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<ClosingStructuresContext>
            {
                CreateFileImporter = (context, filePath) => CreateClosingStructuresImporter(
                    context,
                    filePath,
                    new ImportMessageProvider(),
                    new ClosingStructureReplaceDataStrategy(context.FailureMechanism)),
                Name = RiskeerCommonFormsResources.StructuresImporter_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.StructuresIcon,
                FileFilterGenerator = CreateClosingStructureFileFilter(),
                IsEnabled = context => context.AssessmentSection.ReferenceLine.Points.Any(),
                VerifyUpdates = context => VerifyStructuresShouldUpdate(
                    context.FailureMechanism,
                    RiskeerCommonIOResources.VerifyStructuresShouldUpdate_When_importing_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<ClosingStructuresCalculationGroupContext>(
                (context, filePath) => new ClosingStructuresCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
                    context.AvailableForeshoreProfiles,
                    context.AvailableStructures));
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<ClosingStructuresContext>
            {
                CreateFileImporter = (context, filePath) => CreateClosingStructuresImporter(
                    context,
                    filePath,
                    new UpdateMessageProvider(),
                    new ClosingStructureUpdateDataStrategy(context.FailureMechanism)),
                Name = RiskeerCommonDataResources.StructureCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.StructuresIcon,
                FileFilterGenerator = CreateClosingStructureFileFilter(),
                IsEnabled = c => c.FailureMechanism.ClosingStructures.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                VerifyUpdates = context => VerifyStructuresShouldUpdate(
                    context.FailureMechanism,
                    RiskeerCommonIOResources.VerifyStructuresShouldUpdate_When_updating_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                ClosingStructuresFailureMechanismSectionsContext, ClosingStructuresFailureMechanism, AdoptableFailureMechanismSectionResult>(
                new AdoptableFailureMechanismSectionResultUpdateStrategy());
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<ClosingStructuresCalculationGroupContext>(
                (context, filePath) => new ClosingStructuresCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<ClosingStructuresCalculationScenarioContext>(
                (context, filePath) => new ClosingStructuresCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                GetInquiryHelper());
        }

        #region ViewInfos

        private static bool CloseRegistrationStateFailureMechanismViewForData(RegistrationStateFailureMechanismView view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as ClosingStructuresFailureMechanism;

            return dataToCloseFor is IAssessmentSection assessmentSection
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        private static bool CloseFailureMechanismResultViewForData(StructuresFailureMechanismResultView<ClosingStructuresFailureMechanism, ClosingStructuresInput> view,
                                                                   object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as ClosingStructuresFailureMechanism;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<ClosingStructuresFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is IFailureMechanismContext<ClosingStructuresFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        private static bool CloseScenariosViewForData(ClosingStructuresScenariosView view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as ClosingStructuresFailureMechanism;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<ClosingStructuresFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is IFailureMechanismContext<ClosingStructuresFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        private static bool CloseCalculationsViewForData(ClosingStructuresCalculationsView view, object dataToCloseFor)
        {
            ClosingStructuresFailureMechanism failureMechanism = null;

            if (dataToCloseFor is CalculationsStateFailureMechanismContext failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<ClosingStructuresFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        #endregion

        #region TreeNodeInfos

        #region CalculationsStateFailureMechanismContext TreeNodeInfo

        private static object[] CalculationsStateFailureMechanismChildNodeObjects(CalculationsStateFailureMechanismContext context)
        {
            ClosingStructuresFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetCalculationsStateFailureMechanismInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup, null, failureMechanism, assessmentSection)
            };
        }

        private static IEnumerable<object> GetCalculationsStateFailureMechanismInputs(ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                new ClosingStructuresContext(failureMechanism.ClosingStructures, failureMechanism, assessmentSection),
                failureMechanism.CalculationsInputComments
            };
        }

        private ContextMenuStrip CalculationsStateFailureMechanismContextMenuStrip(CalculationsStateFailureMechanismContext context,
                                                                                   object parentData,
                                                                                   TreeViewControl treeViewControl)
        {
            IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations = context.WrappedData
                                                                                             .Calculations
                                                                                             .Cast<StructuresCalculation<ClosingStructuresInput>>();
            IInquiryHelper inquiryHelper = GetInquiryHelper();

            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              context,
                              ValidateAllInFailureMechanism,
                              EnableValidateAndCalculateMenuItemForFailureMechanism)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              context,
                              CalculateAllInFailureMechanism,
                              EnableValidateAndCalculateMenuItemForFailureMechanism)
                          .AddSeparator()
                          .AddClearAllCalculationOutputInFailureMechanismItem(context.WrappedData)
                          .AddClearIllustrationPointsOfCalculationsInFailureMechanismItem(() => IllustrationPointsHelper.HasIllustrationPoints(calculations),
                                                                                          CreateChangeHandler(inquiryHelper, calculations))
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static string EnableValidateAndCalculateMenuItemForFailureMechanism(CalculationsStateFailureMechanismContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.Parent);
        }

        private static void ValidateAllInFailureMechanism(CalculationsStateFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<StructuresCalculation<ClosingStructuresInput>>(),
                        context.Parent);
        }

        private void CalculateAllInFailureMechanism(CalculationsStateFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(context.WrappedData, context.Parent));
        }

        #endregion

        #region RegistrationStateFailureMechanismContext TreeNodeInfo

        private static object[] RegistrationStateFailureMechanismEnabledChildNodeObjects(RegistrationStateFailureMechanismContext context)
        {
            ClosingStructuresFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetRegistrationStateFailureMechanismInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetRegistrationStateFailureMechanismOutputs(failureMechanism, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static object[] RegistrationStateFailureMechanismDisabledChildNodeObjects(RegistrationStateFailureMechanismContext context)
        {
            return new object[]
            {
                context.WrappedData.NotInAssemblyComments
            };
        }

        private static IEnumerable<object> GetRegistrationStateFailureMechanismInputs(ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new ClosingStructuresFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                failureMechanism.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetRegistrationStateFailureMechanismOutputs(ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new ClosingStructuresScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new ClosingStructuresFailureMechanismSectionResultContext(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.InAssemblyOutputComments
            };
        }

        private ContextMenuStrip RegistrationStateFailureMechanismEnabledContextMenuStrip(RegistrationStateFailureMechanismContext context,
                                                                                          object parentData,
                                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleInAssemblyOfFailureMechanismItem(context, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private ContextMenuStrip RegistrationStateFailureMechanismDisabledContextMenuStrip(RegistrationStateFailureMechanismContext context,
                                                                                           object parentData,
                                                                                           TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddToggleInAssemblyOfFailureMechanismItem(context, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(RegistrationStateFailureMechanismContext context)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(context);
        }

        #endregion

        #region ClosingStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(ClosingStructuresCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                if (calculationItem is StructuresCalculationScenario<ClosingStructuresInput> calculation)
                {
                    childNodeObjects.Add(new ClosingStructuresCalculationScenarioContext(calculation,
                                                                                         context.WrappedData,
                                                                                         context.FailureMechanism,
                                                                                         context.AssessmentSection));
                }
                else if (calculationItem is CalculationGroup group)
                {
                    childNodeObjects.Add(new ClosingStructuresCalculationGroupContext(group,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(ClosingStructuresCalculationGroupContext context,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            CalculationGroup group = context.WrappedData;
            IInquiryHelper inquiryHelper = GetInquiryHelper();
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));
            bool isNestedGroup = parentData is ClosingStructuresCalculationGroupContext;

            StructuresCalculation<ClosingStructuresInput>[] calculations = group
                                                                           .GetCalculations()
                                                                           .Cast<StructuresCalculation<ClosingStructuresInput>>().ToArray();

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
                builder.AddCustomItem(CreateGenerateClosingStructuresCalculationsItem(context))
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation, CalculationType.Probabilistic)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            builder.AddUpdateForeshoreProfileOfCalculationsItem(calculations, inquiryHelper,
                                                                SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                   .AddCustomItem(CreateUpdateAllStructuresItem(calculations))
                   .AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(
                       context,
                       ValidateAllInCalculationGroup,
                       EnableValidateAndCalculateMenuItemForCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(
                       context,
                       CalculateAllInCalculationGroup,
                       EnableValidateAndCalculateMenuItemForCalculationGroup)
                   .AddSeparator()
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddClearIllustrationPointsOfCalculationsInGroupItem(() => IllustrationPointsHelper.HasIllustrationPoints(calculations),
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

        private StrictContextMenuItem CreateUpdateAllStructuresItem(
            IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = RiskeerCommonFormsResources.StructuresPlugin_CreateUpdateStructureItem_Update_all_calculations_with_Structure_Tooltip;

            StructuresCalculation<ClosingStructuresInput>[] calculationsToUpdate = calculations
                                                                                   .Where(calc => calc.InputParameters.Structure != null
                                                                                                  && !calc.InputParameters.IsStructureInputSynchronized)
                                                                                   .ToArray();

            if (!calculationsToUpdate.Any())
            {
                contextMenuEnabled = false;
                toolTipMessage = RiskeerCommonFormsResources.CreateUpdateContextMenuItem_No_calculations_to_update_ToolTip;
            }

            return new StrictContextMenuItem(RiskeerCommonFormsResources.StructuresPlugin_CreateUpdateStructureItem_Update_all_Structures,
                                             toolTipMessage,
                                             RiskeerCommonFormsResources.UpdateItemIcon,
                                             (o, args) => UpdateStructureDependentDataOfCalculations(calculationsToUpdate))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateStructureDependentDataOfCalculations(IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations)
        {
            string message = RiskeerCommonFormsResources.VerifyUpdate_Confirm_calculation_outputs_cleared;
            if (StructureDependentDataShouldUpdate(calculations, message))
            {
                foreach (StructuresCalculation<ClosingStructuresInput> calculation in calculations)
                {
                    UpdateStructureDerivedCalculationInput(calculation);
                }
            }
        }

        private StrictContextMenuItem CreateGenerateClosingStructuresCalculationsItem(ClosingStructuresCalculationGroupContext nodeData)
        {
            bool structuresAvailable = nodeData.FailureMechanism.ClosingStructures.Any();

            string closingStructuresCalculationGroupContextToolTip = structuresAvailable
                                                                         ? RiskeerCommonFormsResources.Generate_Calculations_for_selected_Structures
                                                                         : RiskeerCommonFormsResources.No_Structures_to_generate_Calculations_for;

            return new StrictContextMenuItem(RiskeerCommonFormsResources.CalculationGroup_Generate_calculations,
                                             closingStructuresCalculationGroupContextToolTip,
                                             RiskeerCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowClosingStructuresSelectionDialog(nodeData))
            {
                Enabled = structuresAvailable
            };
        }

        private void ShowClosingStructuresSelectionDialog(ClosingStructuresCalculationGroupContext nodeData)
        {
            using (var dialog = new StructureSelectionDialog(Gui.MainWindow, nodeData.FailureMechanism.ClosingStructures))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    StructureCalculationConfigurationHelper.GenerateCalculations<ClosingStructure, ClosingStructuresInput>(nodeData.WrappedData, dialog.SelectedItems.Cast<ClosingStructure>());
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void AddCalculation(ClosingStructuresCalculationGroupContext context)
        {
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, RiskeerCommonDataResources.Calculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static void CalculationGroupContextOnNodeRemoved(ClosingStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (ClosingStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);
            parentGroupContext.NotifyObservers();
        }

        private static string EnableValidateAndCalculateMenuItemForCalculationGroup(ClosingStructuresCalculationGroupContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void ValidateAllInCalculationGroup(ClosingStructuresCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<StructuresCalculation<ClosingStructuresInput>>(), context.AssessmentSection);
        }

        private void CalculateAllInCalculationGroup(ClosingStructuresCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                                                     context.FailureMechanism,
                                                                                                                     context.AssessmentSection));
        }

        #endregion

        #region ClosingStructuresCalculationScenarioContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(ClosingStructuresCalculationScenarioContext context)
        {
            StructuresCalculation<ClosingStructuresInput> calculation = context.WrappedData;

            return new object[]
            {
                calculation.Comments,
                new ClosingStructuresInputContext(calculation.InputParameters,
                                                  calculation,
                                                  context.FailureMechanism,
                                                  context.AssessmentSection),
                new StructuresOutputContext(calculation)
            };
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(ClosingStructuresCalculationScenarioContext context,
                                                                    object parentData,
                                                                    TreeViewControl treeViewControl)
        {
            StructuresCalculation<ClosingStructuresInput> calculation = context.WrappedData;
            var changeHandler = new ClearIllustrationPointsOfStructuresCalculationHandler(GetInquiryHelper(), calculation);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));
            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, context)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddUpdateForeshoreProfileOfCalculationItem(calculation,
                                                                      GetInquiryHelper(),
                                                                      SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                          .AddCustomItem(CreateUpdateStructureItem(context))
                          .AddSeparator()
                          .AddValidateCalculationItem(
                              context,
                              Validate,
                              EnableValidateAndCalculateMenuItemForCalculation)
                          .AddPerformCalculationItem<StructuresCalculationScenario<ClosingStructuresInput>, ClosingStructuresCalculationScenarioContext>(
                              context,
                              Calculate,
                              EnableValidateAndCalculateMenuItemForCalculation)
                          .AddSeparator()
                          .AddClearCalculationOutputItem(calculation)
                          .AddClearIllustrationPointsOfCalculationItem(() => IllustrationPointsHelper.HasIllustrationPoints(calculation), changeHandler)
                          .AddDeleteItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static string EnableValidateAndCalculateMenuItemForCalculation(ClosingStructuresCalculationScenarioContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void Validate(ClosingStructuresCalculationScenarioContext context)
        {
            ClosingStructuresCalculationService.Validate(context.WrappedData, context.AssessmentSection);
        }

        private void Calculate(ClosingStructuresCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(context.WrappedData,
                                                                                                                   context.FailureMechanism,
                                                                                                                   context.AssessmentSection));
        }

        private static void CalculationContextOnNodeRemoved(ClosingStructuresCalculationScenarioContext context, object parentData)
        {
            if (parentData is ClosingStructuresCalculationGroupContext calculationGroupContext)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                calculationGroupContext.NotifyObservers();
            }
        }

        private StrictContextMenuItem CreateUpdateStructureItem(ClosingStructuresCalculationScenarioContext context)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = RiskeerCommonFormsResources.Update_Calculation_with_Structure_ToolTip;
            if (context.WrappedData.InputParameters.Structure == null)
            {
                contextMenuEnabled = false;
                toolTipMessage = RiskeerCommonFormsResources.Structure_must_be_selected_ToolTip;
            }
            else if (context.WrappedData.InputParameters.IsStructureInputSynchronized)
            {
                contextMenuEnabled = false;
                toolTipMessage = RiskeerCommonFormsResources.CalculationItem_No_changes_to_update_ToolTip;
            }

            return new StrictContextMenuItem(
                RiskeerCommonFormsResources.Update_Structure_data,
                toolTipMessage,
                RiskeerCommonFormsResources.UpdateItemIcon,
                (o, args) => UpdateStructureDependentDataOfCalculation(context.WrappedData))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateStructureDependentDataOfCalculation(StructuresCalculation<ClosingStructuresInput> calculation)
        {
            string message = RiskeerCommonFormsResources.VerifyUpdate_Confirm_calculation_output_cleared;
            if (StructureDependentDataShouldUpdate(new[]
                {
                    calculation
                }, message))
            {
                UpdateStructureDerivedCalculationInput(calculation);
            }
        }

        private bool StructureDependentDataShouldUpdate(IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations, string query)
        {
            var changeHandler = new CalculationChangeHandler(calculations, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static void UpdateStructureDerivedCalculationInput(StructuresCalculation<ClosingStructuresInput> calculation)
        {
            calculation.InputParameters.SynchronizeStructureInput();

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

        private ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler CreateChangeHandler(
            IInquiryHelper inquiryHelper, IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations)
        {
            return new ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler(inquiryHelper, calculations);
        }

        private static void ValidateAll(IEnumerable<StructuresCalculation<ClosingStructuresInput>> closingStructuresCalculations, IAssessmentSection assessmentSection)
        {
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in closingStructuresCalculations)
            {
                ClosingStructuresCalculationService.Validate(calculation, assessmentSection);
            }
        }

        private static string EnableValidateAndCalculateMenuItem(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        #endregion

        #region ImportInfos

        #region ClosingStructuresImporter

        private static IFileImporter CreateClosingStructuresImporter(ClosingStructuresContext context,
                                                                     string filePath,
                                                                     IImporterMessageProvider importerMessageProvider,
                                                                     IStructureUpdateStrategy<ClosingStructure> structureUpdateStrategy)
        {
            return new ClosingStructuresImporter(context.WrappedData,
                                                 context.AssessmentSection.ReferenceLine,
                                                 filePath,
                                                 importerMessageProvider,
                                                 structureUpdateStrategy);
        }

        private static FileFilterGenerator CreateClosingStructureFileFilter()
        {
            return new FileFilterGenerator(
                RiskeerCommonIOResources.Shape_file_filter_Extension,
                RiskeerCommonIOResources.Shape_file_filter_Description);
        }

        private bool VerifyStructuresShouldUpdate(ClosingStructuresFailureMechanism failureMechanism, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(failureMechanism, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion

        #endregion
    }
}