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
using Riskeer.Common.Data;
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
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.Structures;
using Riskeer.Common.Service;
using Riskeer.Common.Util;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.PropertyClasses;
using Riskeer.StabilityPointStructures.Forms.Views;
using Riskeer.StabilityPointStructures.IO;
using Riskeer.StabilityPointStructures.IO.Configurations;
using Riskeer.StabilityPointStructures.Plugin.FileImporters;
using Riskeer.StabilityPointStructures.Service;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="StabilityPointStructuresFailureMechanism"/>.
    /// </summary>
    public class StabilityPointStructuresPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<StabilityPointStructuresFailureMechanismContext, StabilityPointStructuresFailureMechanismProperties>
            {
                CreateInstance = context => new StabilityPointStructuresFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<StabilityPointStructure, StabilityPointStructureProperties>();
            yield return new PropertyInfo<StabilityPointStructuresInputContext, StabilityPointStructuresInputContextProperties>
            {
                CreateInstance = context => new StabilityPointStructuresInputContextProperties(context, new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
            yield return new PropertyInfo<StabilityPointStructuresContext, StructureCollectionProperties<StabilityPointStructure>>
            {
                CreateInstance = context => new StructureCollectionProperties<StabilityPointStructure>(context.WrappedData)
            };
            yield return new PropertyInfo<StabilityPointStructuresOutputContext, StabilityPointStructuresOutputProperties>
            {
                CreateInstance = context => new StabilityPointStructuresOutputProperties(context.WrappedData.Output,
                                                                                         context.FailureMechanism,
                                                                                         context.AssessmentSection)
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<StabilityPointStructuresFailureMechanismContext, StabilityPointStructuresFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.CalculationIcon,
                CloseForData = CloseStabilityPointStructuresFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new StabilityPointStructuresFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>,
                IObservableEnumerable<StabilityPointStructuresFailureMechanismSectionResult>,
                StabilityPointStructuresFailureMechanismResultView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new StabilityPointStructuresFailureMechanismResultView(
                    context.WrappedData,
                    (StabilityPointStructuresFailureMechanism) context.FailureMechanism, context.AssessmentSection)
            };

            yield return new ViewInfo<StabilityPointStructuresScenariosContext, CalculationGroup, StabilityPointStructuresScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Scenarios_DisplayName,
                Image = RiskeerCommonFormsResources.ScenariosIcon,
                CreateInstance = context => new StabilityPointStructuresScenariosView(context.WrappedData, context.ParentFailureMechanism, context.AssessmentSection),
                CloseForData = CloseScenariosViewForData
            };

            yield return new ViewInfo<StabilityPointStructuresCalculationGroupContext, CalculationGroup, StabilityPointStructuresCalculationsView>
            {
                CreateInstance = context => new StabilityPointStructuresCalculationsView(context.WrappedData, context.FailureMechanism, context.AssessmentSection),
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.GeneralFolderIcon,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CloseForData = CloseCalculationsViewForData
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<StabilityPointStructuresFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StabilityPointStructuresContext>
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

            yield return new TreeNodeInfo<StabilityPointStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RiskeerCommonFormsResources.StructuresIcon,
                ContextMenuStrip = (structure, parentData, treeViewControl) => Gui.Get(structure, treeViewControl)
                                                                                  .AddPropertiesItem()
                                                                                  .Build()
            };

            yield return new TreeNodeInfo<StabilityPointStructuresScenariosContext>
            {
                Text = context => RiskeerCommonFormsResources.Scenarios_DisplayName,
                Image = context => RiskeerCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<StabilityPointStructuresCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<StabilityPointStructuresCalculationScenarioContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<StabilityPointStructuresInputContext>
            {
                Text = inputContext => RiskeerCommonFormsResources.Calculation_Input,
                Image = inputContext => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<StabilityPointStructuresContext>
            {
                CreateFileImporter = (context, filePath) => CreateStabilityPointStructuresImporter(
                    context,
                    filePath,
                    new ImportMessageProvider(),
                    new StabilityPointStructureReplaceDataStrategy(context.FailureMechanism)),
                Name = RiskeerCommonFormsResources.StructuresImporter_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.StructuresIcon,
                FileFilterGenerator = CreateStabilityPointStructureFileFilter(),
                IsEnabled = context => context.AssessmentSection.ReferenceLine.Points.Any(),
                VerifyUpdates = context => VerifyStructuresShouldUpdate(
                    context.FailureMechanism,
                    RiskeerCommonIOResources.VerifyStructuresShouldUpdate_When_importing_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<StabilityPointStructuresCalculationGroupContext>(
                (context, filePath) => new StabilityPointStructuresCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
                    context.AvailableForeshoreProfiles,
                    context.AvailableStructures));
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<StabilityPointStructuresCalculationGroupContext>(
                (context, filePath) => new StabilityPointStructuresCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<StabilityPointStructuresCalculationScenarioContext>(
                (context, filePath) => new StabilityPointStructuresCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                GetInquiryHelper());
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<StabilityPointStructuresContext>
            {
                CreateFileImporter = (context, filePath) => CreateStabilityPointStructuresImporter(
                    context,
                    filePath,
                    new UpdateMessageProvider(),
                    new StabilityPointStructureUpdateDataStrategy(context.FailureMechanism)),
                Name = RiskeerCommonDataResources.StructureCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.StructuresIcon,
                FileFilterGenerator = CreateStabilityPointStructureFileFilter(),
                IsEnabled = context => context.WrappedData.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                VerifyUpdates = context => VerifyStructuresShouldUpdate(
                    context.FailureMechanism,
                    RiskeerCommonIOResources.VerifyStructuresShouldUpdate_When_updating_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                StabilityPointStructuresFailureMechanismSectionsContext, StabilityPointStructuresFailureMechanism, StabilityPointStructuresFailureMechanismSectionResult>(
                new StabilityPointStructuresFailureMechanismSectionResultUpdateStrategy());
        }

        #region ViewInfos

        private static bool CloseStabilityPointStructuresFailureMechanismViewForData(StabilityPointStructuresFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as StabilityPointStructuresFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        private static bool CloseFailureMechanismResultViewForData(StabilityPointStructuresFailureMechanismResultView view, object viewData)
        {
            var assessmentSection = viewData as IAssessmentSection;
            var failureMechanism = viewData as StabilityPointStructuresFailureMechanism;
            var failureMechanismContext = viewData as IFailureMechanismContext<StabilityPointStructuresFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .OfType<StabilityPointStructuresFailureMechanism>()
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        private static bool CloseScenariosViewForData(StabilityPointStructuresScenariosView view, object removedData)
        {
            var failureMechanism = removedData as StabilityPointStructuresFailureMechanism;

            if (removedData is StabilityPointStructuresFailureMechanismContext failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (removedData is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<StabilityPointStructuresFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        private static bool CloseCalculationsViewForData(StabilityPointStructuresCalculationsView view, object o)
        {
            
            var failureMechanism = o as StabilityPointStructuresFailureMechanism;

            if (o is StabilityPointStructuresFailureMechanismContext failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (o is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<StabilityPointStructuresFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        #endregion

        #region TreeNodeInfos

        #region StabilityPointStructuresFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(StabilityPointStructuresFailureMechanismContext context)
        {
            StabilityPointStructuresFailureMechanism wrappedData = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetInputs(wrappedData, assessmentSection),
                                       TreeFolderCategory.Input),
                new StabilityPointStructuresCalculationGroupContext(wrappedData.CalculationsGroup,
                                                                    null,
                                                                    wrappedData,
                                                                    assessmentSection),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetOutputs(wrappedData, assessmentSection),
                                       TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(StabilityPointStructuresFailureMechanismContext stabilityPointStructuresFailureMechanismContext)
        {
            return new object[]
            {
                stabilityPointStructuresFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(StabilityPointStructuresFailureMechanism failureMechanism,
                                                     IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new StabilityPointStructuresFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(StabilityPointStructuresFailureMechanism failureMechanism,
                                                      IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralInput.N),
                new StabilityPointStructuresScenariosContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection),
                new ProbabilityFailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(StabilityPointStructuresFailureMechanismContext failureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations = failureMechanismContext.WrappedData
                                                                                                                    .Calculations
                                                                                                                    .Cast<StructuresCalculation<StabilityPointStructuresInput>>();
            IInquiryHelper inquiryHelper = GetInquiryHelper();
            var builder = new RiskeerContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));
            return builder.AddOpenItem()
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
                          .AddClearIllustrationPointsOfCalculationsInFailureMechanismItem(() => IllustrationPointsHelper.HasIllustrationPoints(calculations),
                                                                                          CreateChangeHandler(inquiryHelper, calculations))
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(StabilityPointStructuresFailureMechanismContext stabilityPointStructuresFailureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(stabilityPointStructuresFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(stabilityPointStructuresFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(StabilityPointStructuresFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private static string EnableValidateAndCalculateMenuItemForFailureMechanism(StabilityPointStructuresFailureMechanismContext context)
        {
            return EnableValidateAndCalculateMenu(context.Parent);
        }

        private static void ValidateAllInFailureMechanism(StabilityPointStructuresFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<StructuresCalculation<StabilityPointStructuresInput>>(),
                        context.Parent);
        }

        private void CalculateAllInFailureMechanism(StabilityPointStructuresFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(context.WrappedData, context.Parent));
        }

        #endregion

        #region StabilityPointStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(StabilityPointStructuresCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                if (calculationItem is StructuresCalculationScenario<StabilityPointStructuresInput> calculation)
                {
                    childNodeObjects.Add(new StabilityPointStructuresCalculationScenarioContext(calculation,
                                                                                                context.WrappedData,
                                                                                                context.FailureMechanism,
                                                                                                context.AssessmentSection));
                }
                else if (calculationItem is CalculationGroup group)
                {
                    childNodeObjects.Add(new StabilityPointStructuresCalculationGroupContext(group,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(StabilityPointStructuresCalculationGroupContext context,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            CalculationGroup group = context.WrappedData;
            IInquiryHelper inquiryHelper = GetInquiryHelper();

            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));
            bool isNestedGroup = parentData is StabilityPointStructuresCalculationGroupContext;

            StructuresCalculation<StabilityPointStructuresInput>[] calculations = group
                                                                                  .GetCalculations()
                                                                                  .OfType<StructuresCalculation<StabilityPointStructuresInput>>()
                                                                                  .ToArray();

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
                builder.AddCustomItem(CreateGenerateStabilityPointStructuresCalculationsItem(context))
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation)
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
                       group,
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
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = RiskeerCommonFormsResources.StructuresPlugin_CreateUpdateStructureItem_Update_all_calculations_with_Structure_Tooltip;

            StructuresCalculation<StabilityPointStructuresInput>[] calculationsToUpdate = calculations
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

        private void UpdateStructureDependentDataOfCalculations(StructuresCalculation<StabilityPointStructuresInput>[] calculations)
        {
            string message = RiskeerCommonFormsResources.VerifyUpdate_Confirm_calculation_outputs_cleared;
            if (StructureDependentDataShouldUpdate(calculations, message))
            {
                foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculations)
                {
                    UpdateStructureDerivedCalculationInput(calculation);
                }
            }
        }

        private StrictContextMenuItem CreateGenerateStabilityPointStructuresCalculationsItem(StabilityPointStructuresCalculationGroupContext nodeData)
        {
            StructureCollection<StabilityPointStructure> stabilityPointStructures = nodeData.FailureMechanism.StabilityPointStructures;
            bool structuresAvailable = stabilityPointStructures.Any();

            string stabilityPointStructuresCalculationGroupContextToolTip = structuresAvailable
                                                                                ? RiskeerCommonFormsResources.Generate_Calculations_for_selected_Structures
                                                                                : RiskeerCommonFormsResources.No_Structures_to_generate_Calculations_for;

            return new StrictContextMenuItem(RiskeerCommonFormsResources.CalculationGroup_Generate_calculations,
                                             stabilityPointStructuresCalculationGroupContextToolTip,
                                             RiskeerCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowStabilityPointStructuresSelectionDialog(nodeData))
            {
                Enabled = structuresAvailable
            };
        }

        private void ShowStabilityPointStructuresSelectionDialog(StabilityPointStructuresCalculationGroupContext nodeData)
        {
            using (var dialog = new StructureSelectionDialog(Gui.MainWindow, nodeData.FailureMechanism.StabilityPointStructures))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    StructureCalculationConfigurationHelper.GenerateCalculations<StabilityPointStructure, StabilityPointStructuresInput>(nodeData.WrappedData, dialog.SelectedItems.Cast<StabilityPointStructure>());
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void AddCalculation(StabilityPointStructuresCalculationGroupContext context)
        {
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, RiskeerCommonDataResources.Calculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static void CalculationGroupContextOnNodeRemoved(StabilityPointStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (StabilityPointStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        private static string EnableValidateAndCalculateMenuItemForCalculationGroup(StabilityPointStructuresCalculationGroupContext context)
        {
            return EnableValidateAndCalculateMenu(context.AssessmentSection);
        }

        private static void ValidateAllInCalculationGroup(StabilityPointStructuresCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<StructuresCalculation<StabilityPointStructuresInput>>(),
                        context.AssessmentSection);
        }

        private void CalculateAllInCalculationGroup(CalculationGroup group, StabilityPointStructuresCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(group,
                                                                                                                            context.FailureMechanism,
                                                                                                                            context.AssessmentSection));
        }

        #endregion

        #region StabilityPointStructuresCalculationScenarioContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(StabilityPointStructuresCalculationScenarioContext context)
        {
            StructuresCalculation<StabilityPointStructuresInput> calculation = context.WrappedData;

            return new object[]
            {
                calculation.Comments,
                new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                         calculation,
                                                         context.FailureMechanism,
                                                         context.AssessmentSection),
                new StabilityPointStructuresOutputContext(calculation, context.FailureMechanism, context.AssessmentSection)
            };
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(StabilityPointStructuresCalculationScenarioContext context,
                                                                    object parentData,
                                                                    TreeViewControl treeViewControl)
        {
            StructuresCalculation<StabilityPointStructuresInput> calculation = context.WrappedData;
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
                          .AddPerformCalculationItem(
                              calculation,
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

        private static string EnableValidateAndCalculateMenuItemForCalculation(StabilityPointStructuresCalculationScenarioContext context)
        {
            return EnableValidateAndCalculateMenu(context.AssessmentSection);
        }

        private static void Validate(StabilityPointStructuresCalculationScenarioContext context)
        {
            StabilityPointStructuresCalculationService.Validate(context.WrappedData, context.AssessmentSection);
        }

        private void Calculate(StructuresCalculation<StabilityPointStructuresInput> calculation, StabilityPointStructuresCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                          context.FailureMechanism,
                                                                                                                          context.AssessmentSection));
        }

        private static void CalculationContextOnNodeRemoved(StabilityPointStructuresCalculationScenarioContext context, object parentData)
        {
            if (parentData is StabilityPointStructuresCalculationGroupContext calculationGroupContext)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                calculationGroupContext.NotifyObservers();
            }
        }

        private StrictContextMenuItem CreateUpdateStructureItem(StabilityPointStructuresCalculationScenarioContext context)
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

        private void UpdateStructureDependentDataOfCalculation(StructuresCalculation<StabilityPointStructuresInput> calculation)
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

        private bool StructureDependentDataShouldUpdate(IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations, string query)
        {
            var changeHandler = new CalculationChangeHandler(calculations, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static void UpdateStructureDerivedCalculationInput(ICalculation<StabilityPointStructuresInput> calculation)
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
            IInquiryHelper inquiryHelper, IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations)
        {
            return new ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler(inquiryHelper, calculations);
        }

        private static string EnableValidateAndCalculateMenu(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        private static void ValidateAll(IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations, IAssessmentSection assessmentSection)
        {
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculations)
            {
                StabilityPointStructuresCalculationService.Validate(calculation, assessmentSection);
            }
        }

        #endregion

        #region ImporterInfos

        private static StabilityPointStructuresImporter CreateStabilityPointStructuresImporter(StabilityPointStructuresContext context,
                                                                                               string filePath,
                                                                                               IImporterMessageProvider messageProvider,
                                                                                               IStructureUpdateStrategy<StabilityPointStructure> updateStrategy)
        {
            return new StabilityPointStructuresImporter(context.WrappedData,
                                                        context.AssessmentSection.ReferenceLine,
                                                        filePath,
                                                        messageProvider,
                                                        updateStrategy);
        }

        private static FileFilterGenerator CreateStabilityPointStructureFileFilter()
        {
            return new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                           RiskeerCommonIOResources.Shape_file_filter_Description);
        }

        private bool VerifyStructuresShouldUpdate(IFailureMechanism failureMechanism, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(failureMechanism, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion
    }
}