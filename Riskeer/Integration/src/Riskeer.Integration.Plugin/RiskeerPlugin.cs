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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Core.Components.Gis.Data;
using log4net;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Controls;
using Riskeer.Common.Forms.GuiServices;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.IO.ReferenceLines;
using Riskeer.Common.Plugin;
using Riskeer.Common.Service;
using Riskeer.Common.Util;
using Riskeer.Common.Util.TypeConverters;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.DuneErosion.Plugin.Handlers;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.Input;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Forms.Commands;
using Riskeer.Integration.Forms.Dialogs;
using Riskeer.Integration.Forms.Factories;
using Riskeer.Integration.Forms.Merge;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PresentationObjects.StandAlone;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Forms.PropertyClasses.StandAlone;
using Riskeer.Integration.Forms.Views;
using Riskeer.Integration.Forms.Views.SectionResultRows;
using Riskeer.Integration.Forms.Views.SectionResultViews;
using Riskeer.Integration.IO.Exporters;
using Riskeer.Integration.IO.Importers;
using Riskeer.Integration.Plugin.FileImporters;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.Plugin.Merge;
using Riskeer.Integration.Plugin.Properties;
using Riskeer.Integration.Service;
using Riskeer.Integration.Service.Comparers;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RiskeerDataResources = Riskeer.Integration.Data.Properties.Resources;
using RiskeerFormsResources = Riskeer.Integration.Forms.Properties.Resources;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Riskeer.Integration.Plugin
{
    /// <summary>
    /// The plug-in for the Riskeer application.
    /// </summary>
    public class RiskeerPlugin : PluginBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PluginBase));

        #region Failure mechanism associations

        private static readonly IEnumerable<FailureMechanismContextAssociation> failureMechanismAssociations = new[]
        {
            new FailureMechanismContextAssociation(
                typeof(PipingFailureMechanism),
                (mechanism, assessmentSection) => new PipingFailureMechanismContext(
                    (PipingFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(GrassCoverErosionInwardsFailureMechanism),
                (mechanism, assessmentSection) => new GrassCoverErosionInwardsFailureMechanismContext(
                    (GrassCoverErosionInwardsFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(HeightStructuresFailureMechanism),
                (mechanism, assessmentSection) => new HeightStructuresFailureMechanismContext(
                    (HeightStructuresFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(DuneErosionFailureMechanism),
                (mechanism, assessmentSection) => new DuneErosionFailureMechanismContext(
                    (DuneErosionFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(GrassCoverErosionOutwardsFailureMechanism),
                (mechanism, assessmentSection) => new GrassCoverErosionOutwardsFailureMechanismContext(
                    (GrassCoverErosionOutwardsFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(GrassCoverSlipOffInwardsFailureMechanism),
                (mechanism, assessmentSection) => new GrassCoverSlipOffInwardsFailureMechanismContext(
                    (GrassCoverSlipOffInwardsFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(GrassCoverSlipOffOutwardsFailureMechanism),
                (mechanism, assessmentSection) => new GrassCoverSlipOffOutwardsFailureMechanismContext(
                    (GrassCoverSlipOffOutwardsFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(MicrostabilityFailureMechanism),
                (mechanism, assessmentSection) => new MicrostabilityFailureMechanismContext(
                    (MicrostabilityFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(PipingStructureFailureMechanism),
                (mechanism, assessmentSection) => new PipingStructureFailureMechanismContext(
                    (PipingStructureFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(StabilityStoneCoverFailureMechanism),
                (mechanism, assessmentSection) => new StabilityStoneCoverFailureMechanismContext(
                    (StabilityStoneCoverFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(TechnicalInnovationFailureMechanism),
                (mechanism, assessmentSection) => new TechnicalInnovationFailureMechanismContext(
                    (TechnicalInnovationFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(StrengthStabilityLengthwiseConstructionFailureMechanism),
                (mechanism, assessmentSection) => new StrengthStabilityLengthwiseConstructionFailureMechanismContext(
                    (StrengthStabilityLengthwiseConstructionFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(WaveImpactAsphaltCoverFailureMechanism),
                (mechanism, assessmentSection) => new WaveImpactAsphaltCoverFailureMechanismContext(
                    (WaveImpactAsphaltCoverFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(WaterPressureAsphaltCoverFailureMechanism),
                (mechanism, assessmentSection) => new WaterPressureAsphaltCoverFailureMechanismContext(
                    (WaterPressureAsphaltCoverFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(ClosingStructuresFailureMechanism),
                (mechanism, assessmentSection) => new ClosingStructuresFailureMechanismContext(
                    (ClosingStructuresFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(MacroStabilityInwardsFailureMechanism),
                (mechanism, assessmentSection) => new MacroStabilityInwardsFailureMechanismContext(
                    (MacroStabilityInwardsFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(MacroStabilityOutwardsFailureMechanism),
                (mechanism, assessmentSection) => new MacroStabilityOutwardsFailureMechanismContext(
                    (MacroStabilityOutwardsFailureMechanism) mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(StabilityPointStructuresFailureMechanism),
                (mechanism, assessmentSection) => new StabilityPointStructuresFailureMechanismContext(
                    (StabilityPointStructuresFailureMechanism) mechanism,
                    assessmentSection)
            )
        };

        #endregion

        private RiskeerRibbon ribbonCommandHandler;

        private IAssessmentSectionFromFileCommandHandler assessmentSectionFromFileCommandHandler;
        private IHydraulicBoundaryLocationCalculationGuiService hydraulicBoundaryLocationCalculationGuiService;
        private AssessmentSectionMerger assessmentSectionMerger;

        public override IRibbonCommandHandler RibbonCommandHandler => ribbonCommandHandler;

        public override IGui Gui
        {
            get => base.Gui;
            set
            {
                RemoveOnOpenProjectListener(base.Gui);
                base.Gui = value;
                AddOnOpenProjectListener(value);
            }
        }

        public override void Activate()
        {
            base.Activate();

            if (Gui == null)
            {
                throw new InvalidOperationException("Gui cannot be null");
            }

            assessmentSectionFromFileCommandHandler = new AssessmentSectionFromFileCommandHandler(Gui.MainWindow, Gui, Gui.DocumentViewController);
            hydraulicBoundaryLocationCalculationGuiService = new HydraulicBoundaryLocationCalculationGuiService(Gui.MainWindow);
            assessmentSectionMerger = new AssessmentSectionMerger(new AssessmentSectionMergeFilePathProvider(GetInquiryHelper()),
                                                                  new AssessmentSectionProvider(Gui.MainWindow, Gui.ProjectStore),
                                                                  new AssessmentSectionMergeComparer(),
                                                                  new AssessmentSectionMergeDataProviderDialog(Gui.MainWindow),
                                                                  new AssessmentSectionMergeHandler(Gui.ViewCommands));

            ribbonCommandHandler = new RiskeerRibbon
            {
                AddAssessmentSectionButtonCommand = new AddAssessmentSectionCommand(assessmentSectionFromFileCommandHandler)
            };
        }

        /// <summary>
        /// Returns all <see cref="PropertyInfo"/> instances provided for data of <see cref="RiskeerPlugin"/>.
        /// </summary>
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<IProject, RiskeerProjectProperties>();
            yield return new PropertyInfo<IAssessmentSection, AssessmentSectionProperties>();
            yield return new PropertyInfo<BackgroundData, BackgroundDataProperties>
            {
                CreateInstance = data => new BackgroundDataProperties(data)
            };
            yield return new PropertyInfo<HydraulicBoundaryDatabaseContext, HydraulicBoundaryDatabaseProperties>
            {
                CreateInstance = context => new HydraulicBoundaryDatabaseProperties(
                    context.WrappedData,
                    new HydraulicLocationConfigurationDatabaseImportHandler(
                        Gui.MainWindow,
                        new HydraulicLocationConfigurationDatabaseUpdateHandler(context.AssessmentSection), context.WrappedData))
            };
            yield return new PropertyInfo<FailureMechanismContributionContext, AssessmentSectionCompositionProperties>
            {
                CreateInstance = context => new AssessmentSectionCompositionProperties(
                    context.Parent,
                    new AssessmentSectionCompositionChangeHandler(Gui.ViewCommands))
            };
            yield return new PropertyInfo<NormContext, NormProperties>
            {
                CreateInstance = context => new NormProperties(
                    context.WrappedData,
                    new FailureMechanismContributionNormChangeHandler(context.AssessmentSection))
            };
            yield return new PropertyInfo<IFailureMechanismContext<IFailureMechanism>, StandAloneFailureMechanismProperties>
            {
                CreateInstance = context => new StandAloneFailureMechanismProperties(context.WrappedData, context.Parent)
            };
            yield return new PropertyInfo<MacroStabilityOutwardsFailureMechanismContext, MacroStabilityOutwardsFailureMechanismProperties>
            {
                CreateInstance = context => new MacroStabilityOutwardsFailureMechanismProperties(context.WrappedData, context.Parent)
            };
            yield return new PropertyInfo<PipingStructureFailureMechanismContext, PipingStructureFailureMechanismProperties>
            {
                CreateInstance = context => new PipingStructureFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<ICalculationContext<CalculationGroup, IFailureMechanism>, CalculationGroupContextProperties>
            {
                CreateInstance = context => new CalculationGroupContextProperties(context)
            };
            yield return new PropertyInfo<ICalculationContext<ICalculation, IFailureMechanism>, CalculationContextProperties>();
            yield return new PropertyInfo<DesignWaterLevelCalculationsContext, DesignWaterLevelCalculationsProperties>
            {
                CreateInstance = context => new DesignWaterLevelCalculationsProperties(context.WrappedData)
            };
            yield return new PropertyInfo<DesignWaterLevelCalculationContext, DesignWaterLevelCalculationProperties>
            {
                CreateInstance = context => new DesignWaterLevelCalculationProperties(context.WrappedData)
            };
            yield return new PropertyInfo<WaveHeightCalculationsContext, WaveHeightCalculationsProperties>
            {
                CreateInstance = context => new WaveHeightCalculationsProperties(context.WrappedData)
            };
            yield return new PropertyInfo<WaveHeightCalculationContext, WaveHeightCalculationProperties>
            {
                CreateInstance = context => new WaveHeightCalculationProperties(context.WrappedData)
            };
            yield return new PropertyInfo<ForeshoreProfile, ForeshoreProfileProperties>();
            yield return new PropertyInfo<ForeshoreProfilesContext, ForeshoreProfileCollectionProperties>
            {
                CreateInstance = context => new ForeshoreProfileCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<SelectedTopLevelSubMechanismIllustrationPoint, TopLevelSubMechanismIllustrationPointProperties>
            {
                CreateInstance = selected => new TopLevelSubMechanismIllustrationPointProperties(selected.TopLevelSubMechanismIllustrationPoint,
                                                                                                 selected.ClosingSituations)
            };
            yield return new PropertyInfo<SelectedTopLevelFaultTreeIllustrationPoint, TopLevelFaultTreeIllustrationPointProperties>
            {
                CreateInstance = CreateTopLevelFaultTreeIllustrationPointProperties
            };
            yield return new PropertyInfo<IllustrationPointContext<FaultTreeIllustrationPoint>, FaultTreeIllustrationPointProperties>
            {
                CreateInstance = context => new FaultTreeIllustrationPointProperties(context.IllustrationPoint,
                                                                                     context.IllustrationPointNode.Children,
                                                                                     context.WindDirectionName,
                                                                                     context.ClosingSituation)
            };
            yield return new PropertyInfo<IllustrationPointContext<SubMechanismIllustrationPoint>, SubMechanismIllustrationPointProperties>
            {
                CreateInstance = context => new SubMechanismIllustrationPointProperties(context.IllustrationPoint,
                                                                                        context.WindDirectionName,
                                                                                        context.ClosingSituation)
            };
            yield return new PropertyInfo<FailureMechanismSectionsContext, FailureMechanismSectionsProperties>
            {
                CreateInstance = context => new FailureMechanismSectionsProperties(context.WrappedData)
            };
            yield return new PropertyInfo<ReferenceLineContext, ReferenceLineProperties>
            {
                CreateInstance = context => new ReferenceLineProperties(context.WrappedData)
            };
            yield return new PropertyInfo<FailureMechanismAssemblyCategoriesContext, FailureMechanismAssemblyCategoriesProperties>
            {
                CreateInstance = context => new FailureMechanismAssemblyCategoriesProperties(context.GetFailureMechanismCategoriesFunc(),
                                                                                             context.GetFailureMechanismSectionAssemblyCategoriesFunc())
            };
            yield return new PropertyInfo<MacroStabilityOutwardsAssemblyCategoriesContext, FailureMechanismSectionAssemblyCategoriesProperties>
            {
                CreateInstance = context => new FailureMechanismSectionAssemblyCategoriesProperties(context.GetFailureMechanismSectionAssemblyCategoriesFunc())
            };
            yield return new PropertyInfo<DesignWaterLevelCalculationsGroupContext, DesignWaterLevelCalculationsGroupProperties>
            {
                CreateInstance = context =>
                {
                    IEnumerable<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>> calculationsPerCategoryBoundary =
                        DesignWaterLevelCalculationsGroupContextChildNodeObjects(context)
                            .Cast<DesignWaterLevelCalculationsContext>()
                            .Select(childContext => new Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>(childContext.CategoryBoundaryName,
                                                                                                                         childContext.WrappedData));
                    return new DesignWaterLevelCalculationsGroupProperties(context.WrappedData, calculationsPerCategoryBoundary);
                }
            };
            yield return new PropertyInfo<WaveHeightCalculationsGroupContext, WaveHeightCalculationsGroupProperties>
            {
                CreateInstance = context =>
                {
                    IEnumerable<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>> calculationsPerCategoryBoundary =
                        WaveHeightCalculationsGroupContextChildNodeObjects(context)
                            .Cast<WaveHeightCalculationsContext>()
                            .Select(childContext => new Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>(childContext.CategoryBoundaryName,
                                                                                                                         childContext.WrappedData));
                    return new WaveHeightCalculationsGroupProperties(context.WrappedData, calculationsPerCategoryBoundary);
                }
            };
            yield return new PropertyInfo<AssemblyResultCategoriesContext, AssemblyResultCategoriesProperties>
            {
                CreateInstance = context => new AssemblyResultCategoriesProperties(context.GetAssemblyCategoriesFunc(), context.WrappedData)
            };
        }

        /// <summary>
        /// Returns all <see cref="ViewInfo"/> instances provided for data of <see cref="RiskeerPlugin"/>.
        /// </summary>
        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<FailureMechanismContributionContext, FailureMechanismContribution, FailureMechanismContributionView>
            {
                GetViewName = (view, context) => RiskeerDataResources.FailureMechanismContribution_DisplayName,
                GetViewData = context => context.WrappedData,
                Image = RiskeerCommonFormsResources.FailureMechanismContributionIcon,
                CloseForData = CloseFailureMechanismContributionViewForData,
                CreateInstance = context => new FailureMechanismContributionView(context.Parent, Gui.ViewCommands)
            };

            yield return new ViewInfo<NormContext, FailureMechanismContribution, AssessmentSectionAssemblyCategoriesView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.Norms_DisplayName,
                Image = RiskeerCommonFormsResources.NormsIcon,
                CloseForData = CloseAssessmentSectionCategoriesViewForData,
                CreateInstance = context => new AssessmentSectionAssemblyCategoriesView(context.AssessmentSection.FailureMechanismContribution)
            };

            yield return new ViewInfo<DesignWaterLevelCalculationsContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, DesignWaterLevelCalculationsView>
            {
                GetViewName = (view, context) => $"{RiskeerCommonFormsResources.WaterLevelCalculations_DisplayName} - " +
                                                 $"{RiskeerPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName)}",
                GetViewData = context => context.WrappedData,
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseHydraulicBoundaryCalculationsViewForData,
                CreateInstance = context => new DesignWaterLevelCalculationsView(context.WrappedData,
                                                                                 context.AssessmentSection,
                                                                                 context.GetNormFunc,
                                                                                 context.CategoryBoundaryName),
                AfterCreate = (view, context) => { view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService; }
            };

            yield return new ViewInfo<WaveHeightCalculationsContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, WaveHeightCalculationsView>
            {
                GetViewName = (view, context) => $"{RiskeerCommonFormsResources.WaveHeightCalculations_DisplayName} - " +
                                                 $"{RiskeerPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName)}",
                GetViewData = context => context.WrappedData,
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseHydraulicBoundaryCalculationsViewForData,
                CreateInstance = context => new WaveHeightCalculationsView(context.WrappedData,
                                                                           context.AssessmentSection,
                                                                           context.GetNormFunc,
                                                                           context.CategoryBoundaryName),
                AfterCreate = (view, context) => { view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService; }
            };

            yield return new ViewInfo<IAssessmentSection, AssessmentSectionView>
            {
                GetViewName = (view, section) => RiskeerFormsResources.AssessmentSectionMap_DisplayName,
                Image = RiskeerFormsResources.Map,
                CreateInstance = section => new AssessmentSectionView(section)
            };

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<MacroStabilityOutwardsFailureMechanismContext, MacroStabilityOutwardsFailureMechanism, MacroStabilityOutwardsFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<MacroStabilityOutwardsFailureMechanism, MacroStabilityOutwardsFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => MacroStabilityOutwardsAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => MacroStabilityOutwardsAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData, context.Parent),
                    () => MacroStabilityOutwardsAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData, context.Parent),
                    () => MacroStabilityOutwardsAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData, context.Parent)));

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<MicrostabilityFailureMechanismContext, MicrostabilityFailureMechanism, MicrostabilityFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<MicrostabilityFailureMechanism, MicrostabilityFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => MicrostabilityAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => MicrostabilityAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData),
                    () => MicrostabilityAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => MicrostabilityAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<GrassCoverSlipOffOutwardsFailureMechanismContext, GrassCoverSlipOffOutwardsFailureMechanism, GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<GrassCoverSlipOffOutwardsFailureMechanism, GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => GrassCoverSlipOffOutwardsAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffOutwardsAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffOutwardsAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffOutwardsAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<GrassCoverSlipOffInwardsFailureMechanismContext, GrassCoverSlipOffInwardsFailureMechanism, GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<GrassCoverSlipOffInwardsFailureMechanism, GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => GrassCoverSlipOffInwardsAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffInwardsAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffInwardsAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffInwardsAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<PipingStructureFailureMechanismContext, PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => PipingStructureAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => PipingStructureAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData),
                    () => PipingStructureAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => PipingStructureAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithoutDetailedAssessmentViewInfo<StrengthStabilityLengthwiseConstructionFailureMechanismContext, StrengthStabilityLengthwiseConstructionFailureMechanism, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(
                context => new FailureMechanismWithoutDetailedAssessmentView<StrengthStabilityLengthwiseConstructionFailureMechanism, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithoutDetailedAssessmentViewInfo<TechnicalInnovationFailureMechanismContext, TechnicalInnovationFailureMechanism, TechnicalInnovationFailureMechanismSectionResult>(
                context => new FailureMechanismWithoutDetailedAssessmentView<TechnicalInnovationFailureMechanism, TechnicalInnovationFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => TechnicalInnovationAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => TechnicalInnovationAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => TechnicalInnovationAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithoutDetailedAssessmentViewInfo<WaterPressureAsphaltCoverFailureMechanismContext, WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult>(
                context => new FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => WaterPressureAsphaltCoverAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => WaterPressureAsphaltCoverAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => WaterPressureAsphaltCoverAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismResultViewInfo<
                GrassCoverSlipOffInwardsFailureMechanism,
                GrassCoverSlipOffInwardsFailureMechanismSectionResult,
                GrassCoverSlipOffInwardsResultView,
                GrassCoverSlipOffInwardsSectionResultRow,
                FailureMechanismAssemblyCategoryGroupControl>(
                context => new GrassCoverSlipOffInwardsResultView(
                    context.WrappedData,
                    (GrassCoverSlipOffInwardsFailureMechanism) context.FailureMechanism));

            yield return CreateFailureMechanismResultViewInfo<
                GrassCoverSlipOffOutwardsFailureMechanism,
                GrassCoverSlipOffOutwardsFailureMechanismSectionResult,
                GrassCoverSlipOffOutwardsResultView,
                GrassCoverSlipOffOutwardsSectionResultRow,
                FailureMechanismAssemblyCategoryGroupControl>(
                context => new GrassCoverSlipOffOutwardsResultView(
                    context.WrappedData,
                    (GrassCoverSlipOffOutwardsFailureMechanism) context.FailureMechanism));

            yield return CreateFailureMechanismResultViewInfo<
                MicrostabilityFailureMechanism,
                MicrostabilityFailureMechanismSectionResult,
                MicrostabilityResultView,
                MicrostabilitySectionResultRow,
                FailureMechanismAssemblyCategoryGroupControl>(
                context => new MicrostabilityResultView(
                    context.WrappedData,
                    (MicrostabilityFailureMechanism) context.FailureMechanism));

            yield return CreateFailureMechanismResultViewInfo<
                PipingStructureFailureMechanism,
                PipingStructureFailureMechanismSectionResult,
                PipingStructureResultView,
                PipingStructureSectionResultRow,
                FailureMechanismAssemblyCategoryGroupControl>(
                context => new PipingStructureResultView(
                    context.WrappedData,
                    (PipingStructureFailureMechanism) context.FailureMechanism));

            yield return CreateFailureMechanismResultViewInfo<
                TechnicalInnovationFailureMechanism,
                TechnicalInnovationFailureMechanismSectionResult,
                TechnicalInnovationResultView,
                TechnicalInnovationSectionResultRow,
                FailureMechanismAssemblyCategoryGroupControl>(
                context => new TechnicalInnovationResultView(
                    context.WrappedData,
                    (TechnicalInnovationFailureMechanism) context.FailureMechanism));

            yield return CreateFailureMechanismResultViewInfo<
                StrengthStabilityLengthwiseConstructionFailureMechanism,
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult,
                StrengthStabilityLengthwiseConstructionResultView,
                StrengthStabilityLengthwiseConstructionSectionResultRow,
                FailureMechanismAssemblyCategoryGroupControl>(
                context => new StrengthStabilityLengthwiseConstructionResultView(
                    context.WrappedData,
                    (StrengthStabilityLengthwiseConstructionFailureMechanism) context.FailureMechanism));

            yield return CreateFailureMechanismResultViewInfo<
                WaterPressureAsphaltCoverFailureMechanism,
                WaterPressureAsphaltCoverFailureMechanismSectionResult,
                WaterPressureAsphaltCoverResultView,
                WaterPressureAsphaltCoverSectionResultRow,
                FailureMechanismAssemblyCategoryGroupControl>(
                context => new WaterPressureAsphaltCoverResultView(
                    context.WrappedData,
                    (WaterPressureAsphaltCoverFailureMechanism) context.FailureMechanism));

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<MacroStabilityOutwardsFailureMechanismSectionResult>,
                IObservableEnumerable<MacroStabilityOutwardsFailureMechanismSectionResult>,
                MacroStabilityOutwardsResultView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData<MacroStabilityOutwardsFailureMechanism,
                    MacroStabilityOutwardsFailureMechanismSectionResult,
                    MacroStabilityOutwardsResultView,
                    MacroStabilityOutwardsSectionResultRow,
                    FailureMechanismAssemblyCategoryGroupControl>,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new MacroStabilityOutwardsResultView(
                    context.WrappedData,
                    (MacroStabilityOutwardsFailureMechanism) context.FailureMechanism,
                    context.AssessmentSection)
            };

            yield return new ViewInfo<Comment, CommentView>
            {
                GetViewName = (view, comment) => Resources.Comment_DisplayName,
                GetViewData = comment => comment,
                Image = RiskeerCommonFormsResources.EditDocumentIcon,
                CloseForData = CloseCommentViewForData
            };

            yield return new ViewInfo<FailureMechanismSectionsContext, IEnumerable<FailureMechanismSection>, FailureMechanismSectionsView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Image = RiskeerCommonFormsResources.SectionsIcon,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailureMechanismView,
                CreateInstance = context => new FailureMechanismSectionsView(context.WrappedData.Sections, context.WrappedData),
                GetViewData = context => context.WrappedData.Sections
            };

            yield return new ViewInfo<StructuresOutputContext, IStructuresCalculation, GeneralResultFaultTreeIllustrationPointView>
            {
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                GetViewName = (view, context) => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new GeneralResultFaultTreeIllustrationPointView(() => context.WrappedData.Output?.GeneralResult)
            };

            yield return new ViewInfo<AssemblyResultTotalContext, AssessmentSection, AssemblyResultTotalView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.CombinedAssembly_DisplayName,
                Image = Resources.AssemblyResultTotal,
                CloseForData = CloseAssemblyResultTotalViewForData,
                CreateInstance = context => new AssemblyResultTotalView(context.WrappedData)
            };

            yield return new ViewInfo<AssemblyResultPerSectionContext, AssessmentSection, AssemblyResultPerSectionView>
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssemblyResultPerSection_DisplayName,
                Image = Resources.AssemblyResultPerSection,
                CloseForData = CloseAssemblyResultPerSectionViewForData,
                CreateInstance = context => new AssemblyResultPerSectionView(context.WrappedData)
            };

            yield return new ViewInfo<FailureMechanismAssemblyCategoriesContextBase, IFailureMechanism, FailureMechanismAssemblyCategoriesView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.AssemblyCategories_DisplayName,
                Image = RiskeerCommonFormsResources.NormsIcon,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailureMechanismView,
                CreateInstance = context => new FailureMechanismAssemblyCategoriesView(context.WrappedData,
                                                                                       context.AssessmentSection,
                                                                                       context.GetFailureMechanismCategoriesFunc,
                                                                                       context.GetFailureMechanismSectionAssemblyCategoriesFunc)
            };

            yield return new ViewInfo<MacroStabilityOutwardsAssemblyCategoriesContext, MacroStabilityOutwardsFailureMechanism, MacroStabilityOutwardsAssemblyCategoriesView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.AssemblyCategories_DisplayName,
                Image = RiskeerCommonFormsResources.NormsIcon,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailureMechanismView,
                CreateInstance = context => new MacroStabilityOutwardsAssemblyCategoriesView((MacroStabilityOutwardsFailureMechanism) context.WrappedData,
                                                                                             context.AssessmentSection,
                                                                                             context.GetFailureMechanismSectionAssemblyCategoriesFunc)
            };
            yield return new ViewInfo<AssemblyResultCategoriesContext, AssessmentSection, AssemblyResultCategoriesView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.AssemblyCategories_DisplayName,
                Image = RiskeerCommonFormsResources.NormsIcon,
                CloseForData = CloseAssemblyResultCategoriesViewForData,
                CreateInstance = context => new AssemblyResultCategoriesView(context.WrappedData,
                                                                             context.GetAssemblyCategoriesFunc)
            };
            yield return new ViewInfo<AssemblyResultPerSectionMapContext, AssessmentSection, AssemblyResultPerSectionMapView>
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssemblyResultPerSectionMapView_DisplayName,
                Image = Resources.AssemblyResultPerSectionMap,
                CloseForData = CloseAssemblyResultPerSectionMapViewForData,
                CreateInstance = context => new AssemblyResultPerSectionMapView(context.WrappedData)
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<ReferenceLineContext>
            {
                Name = RiskeerCommonDataResources.ReferenceLine_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.ReferenceLineIcon,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                              RiskeerCommonIOResources.Shape_file_filter_Description),
                CreateFileImporter = (context, filePath) => new ReferenceLineImporter(context.WrappedData,
                                                                                      new ReferenceLineUpdateHandler(context.AssessmentSection, Gui.ViewCommands),
                                                                                      filePath)
            };

            yield return new ImportInfo<FailureMechanismSectionsContext>
            {
                Name = RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.SectionsIcon,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                              RiskeerCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => HasGeometry(context.AssessmentSection.ReferenceLine),
                CreateFileImporter = (context, filePath) => new FailureMechanismSectionsImporter(context.WrappedData,
                                                                                                 context.AssessmentSection.ReferenceLine,
                                                                                                 filePath,
                                                                                                 new FailureMechanismSectionReplaceStrategy(context.WrappedData),
                                                                                                 new ImportMessageProvider())
            };

            yield return new ImportInfo<ForeshoreProfilesContext>
            {
                CreateFileImporter = (context, filePath) =>
                    new ForeshoreProfilesImporter(context.WrappedData,
                                                  context.ParentAssessmentSection.ReferenceLine,
                                                  filePath,
                                                  new ForeshoreProfileReplaceDataStrategy(context.ParentFailureMechanism,
                                                                                          context.WrappedData),
                                                  new ImportMessageProvider()),
                Name = Resources.ForeshoreProfilesImporter_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = Resources.Foreshore,
                FileFilterGenerator = CreateForeshoreProfileFileFilterGenerator,
                IsEnabled = context => HasGeometry(context.ParentAssessmentSection.ReferenceLine),
                VerifyUpdates = context => VerifyForeshoreProfileUpdates(context, Resources.RiskeerPlugin_VerifyForeshoreProfileUpdates_When_importing_ForeshoreProfile_definitions_assigned_to_calculations_output_will_be_cleared_confirm)
            };

            yield return new ImportInfo<HydraulicBoundaryDatabaseContext>
            {
                Name = RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = RiskeerCommonFormsResources.DatabaseIcon,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                FileFilterGenerator = new FileFilterGenerator(Resources.HydraulicBoundaryDatabase_FilePath_Extension,
                                                              Resources.HydraulicBoundaryDatabase_file_filter_Description),
                CreateFileImporter = (context, filePath) => new HydraulicBoundaryDatabaseImporter(
                    context.WrappedData, new HydraulicBoundaryDatabaseUpdateHandler(context.AssessmentSection,
                                                                                    new DuneLocationsReplacementHandler(
                                                                                        Gui.ViewCommands, context.AssessmentSection.DuneErosion)),
                    filePath)
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<ReferenceLineContext>
            {
                Name = RiskeerCommonDataResources.ReferenceLine_DisplayName,
                Extension = RiskeerCommonIOResources.Shape_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new ReferenceLineExporter(context.WrappedData, context.AssessmentSection.Id, filePath),
                IsEnabled = context => HasGeometry(context.AssessmentSection.ReferenceLine),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                                                                           RiskeerCommonIOResources.Shape_file_filter_Description))
            };

            yield return new ExportInfo<HydraulicBoundaryDatabaseContext>
            {
                Name = RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Extension = RiskeerCommonIOResources.Shape_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new HydraulicBoundaryLocationsExporter(context.AssessmentSection, filePath),
                IsEnabled = context => context.WrappedData.IsLinked(),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                                                                           RiskeerCommonIOResources.Shape_file_filter_Description))
            };

            yield return new ExportInfo<AssemblyResultsContext>
            {
                Name = RiskeerCommonFormsResources.AssemblyResult_DisplayName,
                Extension = Resources.AssemblyResult_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new AssemblyExporter(context.WrappedData, filePath),
                IsEnabled = context => HasGeometry(context.WrappedData.ReferenceLine),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(Resources.AssemblyResult_file_filter_Extension,
                                                                                                           RiskeerCommonFormsResources.AssemblyResult_DisplayName))
            };
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<ForeshoreProfilesContext>
            {
                CreateFileImporter = (context, filePath) =>
                    new ForeshoreProfilesImporter(context.WrappedData,
                                                  context.ParentAssessmentSection.ReferenceLine,
                                                  filePath,
                                                  new ForeshoreProfileUpdateDataStrategy(context.ParentFailureMechanism, context.WrappedData),
                                                  new UpdateMessageProvider()),
                Name = Resources.ForeshoreProfilesImporter_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = Resources.Foreshore,
                FileFilterGenerator = CreateForeshoreProfileFileFilterGenerator,
                CurrentPath = context => context.WrappedData.SourcePath,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                VerifyUpdates = context => VerifyForeshoreProfileUpdates(context, Resources.RiskeerPlugin_VerifyForeshoreProfileUpdates_When_updating_ForeshoreProfile_definitions_assigned_to_calculations_output_will_be_cleared_confirm)
            };

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                GrassCoverSlipOffInwardsFailureMechanismSectionsContext, GrassCoverSlipOffInwardsFailureMechanism, GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                new GrassCoverSlipOffInwardsFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                GrassCoverSlipOffOutwardsFailureMechanismSectionsContext, GrassCoverSlipOffOutwardsFailureMechanism, GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(
                new GrassCoverSlipOffOutwardsFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                MacroStabilityOutwardsFailureMechanismSectionsContext, MacroStabilityOutwardsFailureMechanism, MacroStabilityOutwardsFailureMechanismSectionResult>(
                new MacroStabilityOutwardsFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                MicrostabilityFailureMechanismSectionsContext, MicrostabilityFailureMechanism, MicrostabilityFailureMechanismSectionResult>(
                new MicrostabilityFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                PipingStructureFailureMechanismSectionsContext, PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult>(
                new PipingStructureFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionsContext, StrengthStabilityLengthwiseConstructionFailureMechanism, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(
                new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                TechnicalInnovationFailureMechanismSectionsContext, TechnicalInnovationFailureMechanism, TechnicalInnovationFailureMechanismSectionResult>(
                new TechnicalInnovationFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                WaterPressureAsphaltCoverFailureMechanismSectionsContext, WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult>(
                new WaterPressureAsphaltCoverFailureMechanismSectionResultUpdateStrategy());
        }

        /// <summary>
        /// Gets the child data instances that have <see cref="ViewInfo"/> definitions of some parent data object.
        /// </summary>
        /// <param name="viewData">The parent data object.</param>
        /// <returns>Sequence of child data.</returns>
        public override IEnumerable<object> GetChildDataWithViewDefinitions(object viewData)
        {
            if (viewData is RiskeerProject project)
            {
                foreach (AssessmentSection item in project.AssessmentSections)
                {
                    yield return item;
                }
            }

            if (viewData is IAssessmentSection assessmentSection)
            {
                yield return assessmentSection.FailureMechanismContribution;
            }
        }

        /// <summary>
        /// Returns all <see cref="TreeNodeInfo"/> instances provided for data of <see cref="RiskeerPlugin"/>.
        /// </summary>
        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<AssessmentSection>
            {
                Text = assessmentSection => assessmentSection.Name,
                Image = assessmentSection => RiskeerFormsResources.AssessmentSectionFolderIcon,
                EnsureVisibleOnCreate = (assessmentSection, parent) => true,
                ExpandOnCreate = assessmentSection => true,
                ChildNodeObjects = AssessmentSectionChildNodeObjects,
                ContextMenuStrip = AssessmentSectionContextMenuStrip,
                CanRename = (assessmentSection, parentData) => true,
                OnNodeRenamed = AssessmentSectionOnNodeRenamed,
                CanRemove = (assessmentSection, parentNodeData) => true,
                OnNodeRemoved = AssessmentSectionOnNodeRemoved
            };

            yield return new TreeNodeInfo<BackgroundData>
            {
                Text = data => Resources.RiskeerPlugin_BackgroundDataContext_Text,
                Image = data => RiskeerFormsResources.Map,
                ContextMenuStrip = BackgroundDataMenuStrip,
                ForeColor = data =>
                {
                    var configuration = data.Configuration as WmtsBackgroundDataConfiguration;

                    if (configuration != null && configuration.IsConfigured)
                    {
                        return Color.FromKnownColor(KnownColor.ControlText);
                    }

                    return Color.FromKnownColor(configuration == null
                                                    ? KnownColor.ControlText
                                                    : KnownColor.GrayText);
                }
            };

            yield return new TreeNodeInfo<ReferenceLineContext>
            {
                Text = context => RiskeerCommonDataResources.ReferenceLine_DisplayName,
                Image = context => RiskeerCommonFormsResources.ReferenceLineIcon,
                ForeColor = context => HasGeometry(context.WrappedData)
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = ReferenceLineContextMenuStrip
            };

            yield return new TreeNodeInfo<NormContext>
            {
                Text = context => RiskeerCommonFormsResources.Norms_DisplayName,
                Image = context => RiskeerCommonFormsResources.NormsIcon,
                ContextMenuStrip = NormContextMenuStrip
            };

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<GrassCoverSlipOffInwardsFailureMechanismContext>(
                GrassCoverSlipOffInwardsFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<GrassCoverSlipOffOutwardsFailureMechanismContext>(
                GrassCoverSlipOffOutwardsFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<MacroStabilityOutwardsFailureMechanismContext>(
                MacroStabilityOutwardsFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<MicrostabilityFailureMechanismContext>(
                MicrostabilityFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<PipingStructureFailureMechanismContext>(
                PipingStructureFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<StrengthStabilityLengthwiseConstructionFailureMechanismContext>(
                StrengthStabilityLengthwiseConstructionFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TechnicalInnovationFailureMechanismContext>(
                TechnicalInnovationFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<WaterPressureAsphaltCoverFailureMechanismContext>(
                WaterPressureAsphaltCoverFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionsContext>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Image = context => RiskeerCommonFormsResources.SectionsIcon,
                ForeColor = context => context.WrappedData.Sections.Any()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = FailureMechanismSectionsContextMenuStrip
            };

            yield return new TreeNodeInfo<CategoryTreeFolder>
            {
                Text = categoryTreeFolder => categoryTreeFolder.Name,
                Image = categoryTreeFolder => GetFolderIcon(categoryTreeFolder.Category),
                ChildNodeObjects = categoryTreeFolder => categoryTreeFolder.Contents.ToArray(),
                ContextMenuStrip = CategoryTreeFolderContextMenu
            };

            yield return new TreeNodeInfo<FailureMechanismContributionContext>
            {
                Text = failureMechanismContribution => RiskeerDataResources.FailureMechanismContribution_DisplayName,
                Image = failureMechanismContribution => RiskeerCommonFormsResources.FailureMechanismContributionIcon,
                ContextMenuStrip = (failureMechanismContribution, parentData, treeViewControl) => Gui.Get(failureMechanismContribution, treeViewControl)
                                                                                                     .AddOpenItem()
                                                                                                     .AddSeparator()
                                                                                                     .AddPropertiesItem()
                                                                                                     .Build()
            };

            yield return new TreeNodeInfo<HydraulicBoundaryDatabaseContext>
            {
                Text = hydraulicBoundaryDatabase => RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = hydraulicBoundaryDatabase => RiskeerCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.IsLinked()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = HydraulicBoundaryDatabaseChildNodeObjects,
                ContextMenuStrip = HydraulicBoundaryDatabaseContextMenuStrip
            };

            yield return new TreeNodeInfo<DesignWaterLevelCalculationsGroupContext>
            {
                Text = context => RiskeerCommonFormsResources.WaterLevelCalculations_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = DesignWaterLevelCalculationsGroupContextMenuStrip,
                ChildNodeObjects = DesignWaterLevelCalculationsGroupContextChildNodeObjects
            };

            yield return new TreeNodeInfo<DesignWaterLevelCalculationsContext>
            {
                Text = context => RiskeerPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName),
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = DesignWaterLevelCalculationsContextMenuStrip
            };

            yield return new TreeNodeInfo<WaveHeightCalculationsGroupContext>
            {
                Text = context => RiskeerCommonFormsResources.WaveHeightCalculations_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = WaveHeightCalculationsGroupContextMenuStrip,
                ChildNodeObjects = WaveHeightCalculationsGroupContextChildNodeObjects
            };

            yield return new TreeNodeInfo<WaveHeightCalculationsContext>
            {
                Text = context => RiskeerPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName),
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = WaveHeightCalculationsContextMenuStrip
            };

            yield return new TreeNodeInfo<ForeshoreProfilesContext>
            {
                Text = context => RiskeerCommonFormsResources.ForeshoreProfiles_DisplayName,
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

            yield return new TreeNodeInfo<DikeProfile>
            {
                Text = dikeProfile => dikeProfile.Name,
                Image = context => RiskeerCommonFormsResources.DikeProfile,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ForeshoreProfile>
            {
                Text = foreshoreProfile => foreshoreProfile.Name,
                Image = context => Resources.Foreshore,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return CreateFailureMechanismSectionResultTreeNodeInfo<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<GrassCoverSlipOffInwardsFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<MicrostabilityFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<PipingStructureFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<TechnicalInnovationFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<WaterPressureAsphaltCoverFailureMechanismSectionResult>();

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<MacroStabilityOutwardsFailureMechanismSectionResult>>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<Comment>
            {
                Text = comment => Resources.Comment_DisplayName,
                Image = context => RiskeerCommonFormsResources.EditDocumentIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<RiskeerProject>
            {
                Text = project => project.Name,
                Image = project => GuiResources.ProjectIcon,
                ChildNodeObjects = nodeData => nodeData.AssessmentSections.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) =>
                {
                    var addItem = new StrictContextMenuItem(
                        RiskeerFormsResources.AddAssessmentSection_DisplayName,
                        RiskeerCommonFormsResources.RiskeerProject_ToolTip,
                        RiskeerFormsResources.AddAssessmentSectionFolder,
                        (s, e) => assessmentSectionFromFileCommandHandler.AddAssessmentSectionFromFile());

                    return Gui.Get(nodeData, treeViewControl)
                              .AddCustomItem(addItem)
                              .AddSeparator()
                              .AddCollapseAllItem()
                              .AddExpandAllItem()
                              .AddSeparator()
                              .AddPropertiesItem()
                              .Build();
                }
            };

            yield return new TreeNodeInfo<StructuresOutputContext>
            {
                Text = output => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                ForeColor = context => context.WrappedData.HasOutput
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                Image = output => RiskeerCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<AssemblyResultTotalContext>
            {
                Text = context => RiskeerCommonFormsResources.CombinedAssembly_DisplayName,
                Image = context => Resources.AssemblyResultTotal,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<AssemblyResultPerSectionContext>
            {
                Text = context => RiskeerFormsResources.AssemblyResultPerSection_DisplayName,
                Image = context => Resources.AssemblyResultPerSection,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<MacroStabilityOutwardsAssemblyCategoriesContext>
            {
                Text = context => RiskeerCommonFormsResources.AssemblyCategories_DisplayName,
                Image = context => RiskeerCommonFormsResources.NormsIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<FailureMechanismAssemblyCategoriesContext>
            {
                Text = context => RiskeerCommonFormsResources.AssemblyCategories_DisplayName,
                Image = context => RiskeerCommonFormsResources.NormsIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<AssemblyResultsContext>
            {
                Text = context => Resources.AssemblyResultsCategoryTreeFolder_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ChildNodeObjects = AssemblyResultsContextChildNodeObjects,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<AssemblyResultCategoriesContext>
            {
                Text = context => RiskeerCommonFormsResources.AssemblyCategories_DisplayName,
                Image = context => RiskeerCommonFormsResources.NormsIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<AssemblyResultPerSectionMapContext>
            {
                Text = context => Resources.AssemblyResultPerSection_Map_DisplayName,
                Image = context => Resources.AssemblyResultPerSectionMap,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        private static ViewInfo<FailureMechanismSectionResultContext<TResult>, IObservableEnumerable<TResult>, TView> CreateFailureMechanismResultViewInfo<
            TFailureMechanism, TResult, TView, TResultRow, TAssemblyResultControl>(
            Func<FailureMechanismSectionResultContext<TResult>, TView> createInstanceFunc)
            where TResult : FailureMechanismSectionResult
            where TView : FailureMechanismResultView<TResult, TResultRow, TFailureMechanism, TAssemblyResultControl>
            where TFailureMechanism : FailureMechanismBase, IHasSectionResults<TResult>
            where TResultRow : FailureMechanismSectionResultRow<TResult>
            where TAssemblyResultControl : AssemblyResultControl, new()
        {
            return new ViewInfo<
                FailureMechanismSectionResultContext<TResult>,
                IObservableEnumerable<TResult>,
                TView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData<TFailureMechanism, TResult, TView, TResultRow, TAssemblyResultControl>,
                GetViewData = context => context.WrappedData,
                CreateInstance = createInstanceFunc
            };
        }

        private TreeNodeInfo<FailureMechanismSectionResultContext<T>> CreateFailureMechanismSectionResultTreeNodeInfo<T>()
            where T : FailureMechanismSectionResult
        {
            return new TreeNodeInfo<FailureMechanismSectionResultContext<T>>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        private static void RemoveOnOpenProjectListener(IProjectOwner projectOwner)
        {
            if (projectOwner != null)
            {
                projectOwner.ProjectOpened -= VerifyHydraulicBoundaryDatabasePath;
            }
        }

        private static void AddOnOpenProjectListener(IProjectOwner projectOwner)
        {
            if (projectOwner != null)
            {
                projectOwner.ProjectOpened += VerifyHydraulicBoundaryDatabasePath;
            }
        }

        private static void VerifyHydraulicBoundaryDatabasePath(IProject project)
        {
            var riskeerProject = project as RiskeerProject;
            if (riskeerProject == null)
            {
                return;
            }

            IEnumerable<AssessmentSection> sectionsWithHydraulicBoundaryDatabaseLinked = riskeerProject.AssessmentSections.Where(i => i.HydraulicBoundaryDatabase.IsLinked());
            foreach (AssessmentSection section in sectionsWithHydraulicBoundaryDatabaseLinked)
            {
                string validationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(
                    section.HydraulicBoundaryDatabase.FilePath,
                    section.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.FilePath,
                    section.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                    section.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessorClosure);

                if (validationProblem != null)
                {
                    log.WarnFormat(
                        RiskeerCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                        validationProblem);
                }
            }
        }

        private static bool HasGeometry(ReferenceLine referenceLine)
        {
            return referenceLine.Points.Any();
        }

        #region PropertyInfos

        private static TopLevelFaultTreeIllustrationPointProperties CreateTopLevelFaultTreeIllustrationPointProperties(SelectedTopLevelFaultTreeIllustrationPoint point)
        {
            return new TopLevelFaultTreeIllustrationPointProperties(point.TopLevelFaultTreeIllustrationPoint,
                                                                    !point.ClosingSituations.HasMultipleUniqueValues(cs => cs));
        }

        #endregion

        private class FailureMechanismContextAssociation
        {
            private readonly Func<IFailureMechanism, IAssessmentSection, object> createFailureMechanismContext;
            private readonly Type failureMechanismType;

            public FailureMechanismContextAssociation(Type failureMechanismType, Func<IFailureMechanism, IAssessmentSection, object> createFailureMechanismContext)
            {
                this.createFailureMechanismContext = createFailureMechanismContext;
                this.failureMechanismType = failureMechanismType;
            }

            public bool Match(IFailureMechanism failureMechanism)
            {
                return failureMechanism.GetType() == failureMechanismType;
            }

            public object Create(IFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            {
                return createFailureMechanismContext(failureMechanism, assessmentSection);
            }
        }

        #region ViewInfos

        #region FailureMechanismWithDetailedAssessmentView ViewInfo

        private static ViewInfo<TFailureMechanismContext, TFailureMechanism, FailureMechanismWithDetailedAssessmentView<TFailureMechanism, TSectionResult>> CreateFailureMechanismWithDetailedAssessmentViewInfo<
            TFailureMechanismContext, TFailureMechanism, TSectionResult>(
            Func<TFailureMechanismContext, FailureMechanismWithDetailedAssessmentView<TFailureMechanism, TSectionResult>> createInstanceFunc)
            where TSectionResult : FailureMechanismSectionResult
            where TFailureMechanism : FailureMechanismBase, IHasSectionResults<TSectionResult>
            where TFailureMechanismContext : IFailureMechanismContext<TFailureMechanism>
        {
            return new ViewInfo<TFailureMechanismContext, TFailureMechanism,
                FailureMechanismWithDetailedAssessmentView<TFailureMechanism, TSectionResult>>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.CalculationIcon,
                CloseForData = CloseFailureMechanismWithDetailedAssessmentViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = createInstanceFunc
            };
        }

        private static bool CloseFailureMechanismWithDetailedAssessmentViewForData<TFailureMechanism, TSectionResult>(
            FailureMechanismWithDetailedAssessmentView<TFailureMechanism, TSectionResult> view, object o)
            where TFailureMechanism : IHasSectionResults<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as IFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        #endregion

        #region FailureMechanismWithoutDetailedAssessmentView ViewInfo

        private static ViewInfo<TFailureMechanismContext, TFailureMechanism, FailureMechanismWithoutDetailedAssessmentView<TFailureMechanism, TSectionResult>> CreateFailureMechanismWithoutDetailedAssessmentViewInfo<
            TFailureMechanismContext, TFailureMechanism, TSectionResult>(
            Func<TFailureMechanismContext, FailureMechanismWithoutDetailedAssessmentView<TFailureMechanism, TSectionResult>> createInstanceFunc)
            where TSectionResult : FailureMechanismSectionResult
            where TFailureMechanism : FailureMechanismBase, IHasSectionResults<TSectionResult>
            where TFailureMechanismContext : IFailureMechanismContext<TFailureMechanism>
        {
            return new ViewInfo<TFailureMechanismContext, TFailureMechanism,
                FailureMechanismWithoutDetailedAssessmentView<TFailureMechanism, TSectionResult>>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.CalculationIcon,
                CloseForData = CloseFailureMechanismWithoutDetailedAssessmentViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = createInstanceFunc
            };
        }

        private static bool CloseFailureMechanismWithoutDetailedAssessmentViewForData<TFailureMechanism, TSectionResult>(
            FailureMechanismWithoutDetailedAssessmentView<TFailureMechanism, TSectionResult> view, object o)
            where TFailureMechanism : IHasSectionResults<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as IFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        #endregion

        #region FailureMechanismContributionContext ViewInfo

        private static bool CloseFailureMechanismContributionViewForData(FailureMechanismContributionView view, object o)
        {
            return o is IAssessmentSection assessmentSection && assessmentSection == view.AssessmentSection;
        }

        #endregion

        #region NormContext ViewInfo

        private static bool CloseAssessmentSectionCategoriesViewForData(AssessmentSectionAssemblyCategoriesView view, object o)
        {
            return o is IAssessmentSection assessmentSection && assessmentSection.FailureMechanismContribution == view.FailureMechanismContribution;
        }

        #endregion

        #region FailureMechanismResults ViewInfo

        private static bool CloseFailureMechanismResultViewForData<TFailureMechanism, TResult, TView, TResultRow, TAssemblyResultControl>(TView view, object dataToCloseFor)
            where TView : FailureMechanismResultView<TResult, TResultRow, TFailureMechanism, TAssemblyResultControl>
            where TFailureMechanism : FailureMechanismBase, IHasSectionResults<TResult>
            where TResult : FailureMechanismSectionResult
            where TResultRow : FailureMechanismSectionResultRow<TResult>
            where TAssemblyResultControl : AssemblyResultControl, new()
        {
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as IFailureMechanism;
            var failureMechanismContext = dataToCloseFor as IFailureMechanismContext<IFailureMechanism>;

            if (assessmentSection != null)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .OfType<IHasSectionResults<FailureMechanismSectionResult>>()
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism is IHasSectionResults<FailureMechanismSectionResult> failureMechanismWithSectionResults &&
                   ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanismWithSectionResults.SectionResults);
        }

        #endregion

        #region HydraulicBoundaryCalculationsView ViewInfo

        private static bool CloseHydraulicBoundaryCalculationsViewForData(HydraulicBoundaryCalculationsView view, object dataToCloseFor)
        {
            IAssessmentSection viewData = view.AssessmentSection;

            return dataToCloseFor is IAssessmentSection assessmentSection && ReferenceEquals(viewData, assessmentSection);
        }

        #endregion

        #region Comment ViewInfo

        private static bool CloseCommentViewForData(CommentView commentView, object o)
        {
            if (o is ICalculationContext<CalculationGroup, IFailureMechanism> calculationGroupContext)
            {
                return GetCommentElements(calculationGroupContext.WrappedData)
                    .Any(commentElement => ReferenceEquals(commentView.Data, commentElement));
            }

            var calculationContext = o as ICalculationContext<ICalculationBase, IFailureMechanism>;
            if (calculationContext?.WrappedData is ICalculation calculation)
            {
                return ReferenceEquals(commentView.Data, calculation.Comments);
            }

            var failureMechanism = o as IFailureMechanism;

            if (o is IFailureMechanismContext<IFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (failureMechanism != null)
            {
                return GetCommentElements(failureMechanism)
                    .Any(commentElement => ReferenceEquals(commentView.Data, commentElement));
            }

            if (o is IAssessmentSection assessmentSection)
            {
                return GetCommentElements(assessmentSection)
                    .Any(commentElement => ReferenceEquals(commentView.Data, commentElement));
            }

            return false;
        }

        private static IEnumerable<Comment> GetCommentElements(CalculationGroup calculationGroup)
        {
            return calculationGroup.GetCalculations().Select(c => c.Comments);
        }

        private static IEnumerable<Comment> GetCommentElements(IAssessmentSection assessmentSection)
        {
            yield return assessmentSection.Comments;
            foreach (Comment comment in assessmentSection.GetFailureMechanisms().SelectMany(GetCommentElements))
            {
                yield return comment;
            }
        }

        private static IEnumerable<Comment> GetCommentElements(IFailureMechanism failureMechanism)
        {
            yield return failureMechanism.InputComments;
            yield return failureMechanism.OutputComments;
            yield return failureMechanism.NotRelevantComments;
            foreach (ICalculation calculation in failureMechanism.Calculations)
            {
                yield return calculation.Comments;
            }
        }

        #endregion

        #region AssemblyResultTotalContext ViewInfo

        private static bool CloseAssemblyResultTotalViewForData(AssemblyResultTotalView view, object o)
        {
            return o is AssessmentSection assessmentSection && assessmentSection == view.AssessmentSection;
        }

        #endregion

        #region AssemblyResultPerSectionContext ViewInfo

        private static bool CloseAssemblyResultPerSectionViewForData(AssemblyResultPerSectionView view, object o)
        {
            return o is AssessmentSection assessmentSection && assessmentSection == view.AssessmentSection;
        }

        #endregion

        #region AssemblyResultCategoriesContext ViewInfo

        private static bool CloseAssemblyResultCategoriesViewForData(AssemblyResultCategoriesView view, object o)
        {
            return o is AssessmentSection assessmentSection && assessmentSection == view.AssessmentSection;
        }

        #endregion

        #region AssemblyResultPerSectionMapContext ViewInfo

        private static bool CloseAssemblyResultPerSectionMapViewForData(AssemblyResultPerSectionMapView view, object o)
        {
            return o is AssessmentSection assessmentSection && assessmentSection == view.AssessmentSection;
        }

        #endregion

        #endregion

        #region TreeNodeInfos

        #region FailureMechanismSectionsContext TreeNodeInfo

        private ContextMenuStrip FailureMechanismSectionsContextMenuStrip(FailureMechanismSectionsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddImportItem()
                      .AddUpdateItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region ReferenceLineContext TreeNodeInfo

        private ContextMenuStrip ReferenceLineContextMenuStrip(ReferenceLineContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region NormContext TreeNodeInfo

        private ContextMenuStrip NormContextMenuStrip(NormContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region BackgroundData treeNodeInfo

        private ContextMenuStrip BackgroundDataMenuStrip(BackgroundData nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var assessmentSection = parentData as IAssessmentSection;

            var mapDataItem = new StrictContextMenuItem(
                Resources.BackgroundData_SelectMapData,
                Resources.BackgroundData_SelectMapData_Tooltip,
                RiskeerCommonFormsResources.MapsIcon, (sender, args) => SelectMapData(assessmentSection, nodeData));

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(mapDataItem)
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        private void SelectMapData(IAssessmentSection assessmentSection, BackgroundData backgroundData)
        {
            if (assessmentSection == null)
            {
                return;
            }

            ImageBasedMapData currentData = BackgroundDataConverter.ConvertFrom(backgroundData);
            using (var dialog = new BackgroundMapDataSelectionDialog(Gui.MainWindow, currentData))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SetSelectedMapData(assessmentSection, dialog.SelectedMapData);
                }
            }
        }

        private static void SetSelectedMapData(IAssessmentSection assessmentSection, ImageBasedMapData selectedMapData)
        {
            BackgroundData newBackgroundData = BackgroundDataConverter.ConvertTo(selectedMapData);

            assessmentSection.BackgroundData.IsVisible = newBackgroundData.IsVisible;
            assessmentSection.BackgroundData.Name = newBackgroundData.Name;
            assessmentSection.BackgroundData.Transparency = newBackgroundData.Transparency;
            assessmentSection.BackgroundData.Configuration = newBackgroundData.Configuration;

            assessmentSection.BackgroundData.NotifyObservers();
        }

        #endregion

        #region AssessmentSection TreeNodeInfo

        private static object[] AssessmentSectionChildNodeObjects(AssessmentSection nodeData)
        {
            var childNodes = new List<object>
            {
                new ReferenceLineContext(nodeData.ReferenceLine, nodeData),
                new NormContext(nodeData.FailureMechanismContribution, nodeData),
                new FailureMechanismContributionContext(nodeData.FailureMechanismContribution, nodeData),
                new HydraulicBoundaryDatabaseContext(nodeData.HydraulicBoundaryDatabase, nodeData),
                nodeData.BackgroundData,
                nodeData.Comments
            };

            childNodes.AddRange(WrapFailureMechanismsInContexts(nodeData));
            childNodes.Add(new AssemblyResultsContext(nodeData));

            return childNodes.ToArray();
        }

        private static IEnumerable<object> WrapFailureMechanismsInContexts(IAssessmentSection assessmentSection)
        {
            return assessmentSection
                   .GetFailureMechanisms()
                   .Select(failureMechanism => failureMechanismAssociations
                                               .First(a => a.Match(failureMechanism))
                                               .Create(failureMechanism, assessmentSection))
                   .ToArray();
        }

        private static void AssessmentSectionOnNodeRenamed(IAssessmentSection nodeData, string newName)
        {
            nodeData.Name = newName;
            nodeData.NotifyObservers();
        }

        private static void AssessmentSectionOnNodeRemoved(IAssessmentSection nodeData, object parentNodeData)
        {
            var parentProject = (RiskeerProject) parentNodeData;
            var assessmentSection = (AssessmentSection) nodeData;
            parentProject.AssessmentSections.Remove(assessmentSection);
            parentProject.NotifyObservers();
        }

        private ContextMenuStrip AssessmentSectionContextMenuStrip(AssessmentSection nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var calculateAllItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                Resources.AssessmentSection_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) => { ActivityProgressDialogRunner.Run(Gui.MainWindow, AssessmentSectionCalculationActivityFactory.CreateActivities(nodeData)); });

            var importItem = new StrictContextMenuItem(
                GuiResources.Import,
                GuiResources.Import_ToolTip,
                GuiResources.ImportIcon,
                (sender, args) => assessmentSectionMerger.StartMerge(nodeData));

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(importItem)
                      .AddSeparator()
                      .AddRenameItem()
                      .AddSeparator()
                      .AddCustomItem(calculateAllItem)
                      .AddSeparator()
                      .AddDeleteItem()
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region StandAloneFailureMechanism TreeNodeInfo

        private static object[] StandAloneFailureMechanismDisabledChildNodeObjects(IFailureMechanismContext<IFailureMechanism> nodeData)
        {
            return new object[]
            {
                nodeData.WrappedData.NotRelevantComments
            };
        }

        private ContextMenuStrip StandAloneFailureMechanismEnabledContextMenuStrip(IFailureMechanismContext<IFailureMechanism> nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(nodeData, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(IFailureMechanismContext<IFailureMechanism> failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip StandAloneFailureMechanismDisabledContextMenuStrip(IFailureMechanismContext<IFailureMechanism> nodeData,
                                                                                    object parentData,
                                                                                    TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(nodeData, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        #endregion

        #region GrassCoverSlipOffInwardsFailureMechanismContext TreeNodeInfo

        private static object[] GrassCoverSlipOffInwardsFailureMechanismEnabledChildNodeObjects(GrassCoverSlipOffInwardsFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetGrassCoverSlipOffInwardsFailureMechanismInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetGrassCoverSlipOffInwardsFailureMechanismOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffInwardsFailureMechanismInputs(GrassCoverSlipOffInwardsFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverSlipOffInwardsFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffInwardsFailureMechanismOutputs(GrassCoverSlipOffInwardsFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region GrassCoverSlipOffOutwardsFailureMechanismContext TreeNodeInfo

        private static object[] GrassCoverSlipOffOutwardsFailureMechanismEnabledChildNodeObjects(GrassCoverSlipOffOutwardsFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetGrassCoverSlipOffOutwardsFailureMechanismInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetGrassCoverSlipOffOutwardsFailureMechanismOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffOutwardsFailureMechanismInputs(GrassCoverSlipOffOutwardsFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverSlipOffOutwardsFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffOutwardsFailureMechanismOutputs(GrassCoverSlipOffOutwardsFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region MacroStabilityOutwardsFailureMechanismContext TreeNodeInfo

        private static object[] MacroStabilityOutwardsFailureMechanismEnabledChildNodeObjects(MacroStabilityOutwardsFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetMacroStabilityOutwardsFailureMechanismInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetMacroStabilityOutwardsFailureMechanismOutputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetMacroStabilityOutwardsFailureMechanismInputs(MacroStabilityOutwardsFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MacroStabilityOutwardsFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetMacroStabilityOutwardsFailureMechanismOutputs(MacroStabilityOutwardsFailureMechanism nodeData,
                                                                                            IAssessmentSection assessmentSection)
        {
            MacroStabilityOutwardsProbabilityAssessmentInput probabilityAssessmentInput = nodeData.MacroStabilityOutwardsProbabilityAssessmentInput;
            return new object[]
            {
                new MacroStabilityOutwardsAssemblyCategoriesContext(nodeData,
                                                                    assessmentSection,
                                                                    () => probabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length)),
                new ProbabilityFailureMechanismSectionResultContext<MacroStabilityOutwardsFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData, assessmentSection),
                nodeData.OutputComments
            };
        }

        #endregion

        #region MicrostabilityFailureMechanismContext TreeNodeInfo

        private static object[] MicrostabilityFailureMechanismEnabledChildNodeObjects(MicrostabilityFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetMicrostabilityFailureMechanismInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetMicrostabilityFailureMechanismOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetMicrostabilityFailureMechanismInputs(MicrostabilityFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MicrostabilityFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetMicrostabilityFailureMechanismOutputs(MicrostabilityFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region PipingStructureFailureMechanismContext TreeNodeInfo

        private static object[] PipingStructureFailureMechanismEnabledChildNodeObjects(PipingStructureFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetPipingStructureFailureMechanismInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetPipingStructureFailureMechanismOutputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetPipingStructureFailureMechanismInputs(PipingStructureFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new PipingStructureFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetPipingStructureFailureMechanismOutputs(PipingStructureFailureMechanism nodeData,
                                                                                     IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(nodeData,
                                                              assessmentSection,
                                                              () => nodeData.N),
                new ProbabilityFailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData, assessmentSection),
                nodeData.OutputComments
            };
        }

        #endregion

        #region StrengthStabilityLengthwiseConstructionFailureMechanismContext TreeNodeInfo

        private static object[] StrengthStabilityLengthwiseConstructionFailureMechanismEnabledChildNodeObjects(
            StrengthStabilityLengthwiseConstructionFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetStrengthStabilityLengthwiseConstructionFailureMechanismInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetStrengthStabilityLengthwiseConstructionFailureMechanismOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetStrengthStabilityLengthwiseConstructionFailureMechanismInputs(StrengthStabilityLengthwiseConstructionFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new StrengthStabilityLengthwiseConstructionFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetStrengthStabilityLengthwiseConstructionFailureMechanismOutputs(StrengthStabilityLengthwiseConstructionFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region TechnicalInnovationFailureMechanismContext TreeNodeInfo

        private static object[] TechnicalInnovationFailureMechanismEnabledChildNodeObjects(TechnicalInnovationFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetTechnicalInnovationFailureMechanismInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetTechnicalInnovationFailureMechanismOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetTechnicalInnovationFailureMechanismInputs(TechnicalInnovationFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new TechnicalInnovationFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetTechnicalInnovationFailureMechanismOutputs(TechnicalInnovationFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region WaterPressureAsphaltCoverFailureMechanismContext TreeNodeInfo

        private static object[] WaterPressureAsphaltCoverFailureMechanismEnabledChildNodeObjects(WaterPressureAsphaltCoverFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetWaterPressureAsphaltCoverFailureMechanismInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetWaterPressureAsphaltCoverFailureMechanismOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetWaterPressureAsphaltCoverFailureMechanismInputs(WaterPressureAsphaltCoverFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new WaterPressureAsphaltCoverFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetWaterPressureAsphaltCoverFailureMechanismOutputs(WaterPressureAsphaltCoverFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region CategoryTreeFolder TreeNodeInfo

        /// <summary>
        /// Gets an <see cref="Image"/> based on <paramref name="category"/>.
        /// </summary>
        /// <param name="category">The tree folder category to retrieve the image for.</param>
        /// <returns>An <see cref="Image"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value of <see cref="TreeFolderCategory"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is an unsupported value of <see cref="TreeFolderCategory"/>.</exception>
        private static Image GetFolderIcon(TreeFolderCategory category)
        {
            if (!Enum.IsDefined(typeof(TreeFolderCategory), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(TreeFolderCategory));
            }

            switch (category)
            {
                case TreeFolderCategory.General:
                    return RiskeerCommonFormsResources.GeneralFolderIcon;
                case TreeFolderCategory.Input:
                    return RiskeerCommonFormsResources.InputFolderIcon;
                case TreeFolderCategory.Output:
                    return RiskeerCommonFormsResources.OutputFolderIcon;
                default:
                    throw new NotSupportedException();
            }
        }

        private ContextMenuStrip CategoryTreeFolderContextMenu(CategoryTreeFolder nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .Build();
        }

        #endregion

        #region HydraulicBoundaryDatabase TreeNodeInfo

        private static object[] HydraulicBoundaryDatabaseChildNodeObjects(HydraulicBoundaryDatabaseContext nodeData)
        {
            if (nodeData.WrappedData.IsLinked())
            {
                return new object[]
                {
                    new DesignWaterLevelCalculationsGroupContext(nodeData.WrappedData.Locations,
                                                                 nodeData.AssessmentSection),
                    new WaveHeightCalculationsGroupContext(nodeData.WrappedData.Locations,
                                                           nodeData.AssessmentSection)
                };
            }

            return new object[0];
        }

        private ContextMenuStrip DesignWaterLevelCalculationsContextMenuStrip(DesignWaterLevelCalculationsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var designWaterLevelItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.DesignWaterLevel_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    IAssessmentSection assessmentSection = nodeData.AssessmentSection;
                    hydraulicBoundaryLocationCalculationGuiService.CalculateDesignWaterLevels(nodeData.WrappedData,
                                                                                              assessmentSection,
                                                                                              nodeData.GetNormFunc(),
                                                                                              nodeData.CategoryBoundaryName);
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.AssessmentSection,
                                                        nodeData.GetNormFunc(),
                                                        designWaterLevelItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                RiskeerPluginHelper.FormatCategoryBoundaryName(nodeData.CategoryBoundaryName),
                () => RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(nodeData.WrappedData));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddCustomItem(designWaterLevelItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => IllustrationPointsHelper.HasIllustrationPoints(nodeData.WrappedData), changeHandler)
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private ContextMenuStrip WaveHeightCalculationsContextMenuStrip(WaveHeightCalculationsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var waveHeightItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.WaveHeight_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    IAssessmentSection assessmentSection = nodeData.AssessmentSection;
                    hydraulicBoundaryLocationCalculationGuiService.CalculateWaveHeights(nodeData.WrappedData,
                                                                                        assessmentSection,
                                                                                        nodeData.GetNormFunc(),
                                                                                        nodeData.CategoryBoundaryName);
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.AssessmentSection,
                                                        nodeData.GetNormFunc(),
                                                        waveHeightItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                RiskeerPluginHelper.FormatCategoryBoundaryName(nodeData.CategoryBoundaryName),
                () => RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(nodeData.WrappedData));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddCustomItem(waveHeightItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => IllustrationPointsHelper.HasIllustrationPoints(nodeData.WrappedData), changeHandler)
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static void SetHydraulicsMenuItemEnabledStateAndTooltip(IAssessmentSection assessmentSection, StrictContextMenuItem menuItem)
        {
            string validationText = HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
            if (!string.IsNullOrEmpty(validationText))
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = validationText;
            }
        }

        private static void SetHydraulicsMenuItemEnabledStateAndTooltip(IAssessmentSection assessmentSection, double norm, StrictContextMenuItem menuItem)
        {
            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, menuItem);
            if (!menuItem.Enabled)
            {
                return;
            }

            TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(norm, logMessage =>
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = logMessage;
            });
        }

        private ContextMenuStrip HydraulicBoundaryDatabaseContextMenuStrip(HydraulicBoundaryDatabaseContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var calculateAllItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.HydraulicLoads_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        Gui.MainWindow,
                        AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateHydraulicBoundaryLocationCalculationActivities(nodeData.AssessmentSection));
                });

            var connectItem = new StrictContextMenuItem(RiskeerFormsResources.HydraulicBoundaryDatabase_Connect,
                                                        RiskeerFormsResources.HydraulicBoundaryDatabase_Connect_ToolTip,
                                                        RiskeerCommonFormsResources.DatabaseIcon,
                                                        (s, e) =>
                                                        {
                                                            var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(Gui.MainWindow, GetInquiryHelper());

                                                            dialog.ShowDialog();
                                                        });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.AssessmentSection,
                                                        calculateAllItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                RiskeerCommonFormsResources.WaterLevel_and_WaveHeight_DisplayName,
                () => RiskeerDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelAndWaveHeightCalculations(nodeData.AssessmentSection));

            AssessmentSection assessmentSection = nodeData.AssessmentSection;
            return builder.AddCustomItem(connectItem)
                          .AddExportItem()
                          .AddSeparator()
                          .AddCustomItem(calculateAllItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => HasIllustrationPoints(assessmentSection), changeHandler)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private ContextMenuStrip DesignWaterLevelCalculationsGroupContextMenuStrip(DesignWaterLevelCalculationsGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;

            IMainWindow guiMainWindow = Gui.MainWindow;
            var designWaterLevelItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.DesignWaterLevel_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        guiMainWindow,
                        AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, designWaterLevelItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                RiskeerCommonFormsResources.WaterLevelCalculations_DisplayName,
                () => RiskeerDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelCalculations(nodeData.AssessmentSection));

            return builder.AddCustomItem(designWaterLevelItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => DesignWaterLevelCalculationsHaveIllustrationPoints(assessmentSection), changeHandler)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private ContextMenuStrip WaveHeightCalculationsGroupContextMenuStrip(WaveHeightCalculationsGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;

            IMainWindow guiMainWindow = Gui.MainWindow;
            var waveHeightItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.WaveHeight_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        guiMainWindow,
                        AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, waveHeightItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                RiskeerCommonFormsResources.WaveHeightCalculations_DisplayName,
                () => RiskeerDataSynchronizationService.ClearIllustrationPointResultsForWaveHeightCalculations(nodeData.AssessmentSection));

            return builder.AddCustomItem(waveHeightItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => WaveHeightCalculationsHaveIllustrationPoints(assessmentSection), changeHandler)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static object[] DesignWaterLevelCalculationsGroupContextChildNodeObjects(DesignWaterLevelCalculationsGroupContext context)
        {
            return new object[]
            {
                new DesignWaterLevelCalculationsContext(context.AssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm,
                                                        context.AssessmentSection,
                                                        () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedSignalingNorm),
                                                        RiskeerCommonDataResources.AssessmentSectionCategoryType_FactorizedSignalingNorm_DisplayName),
                new DesignWaterLevelCalculationsContext(context.AssessmentSection.WaterLevelCalculationsForSignalingNorm,
                                                        context.AssessmentSection,
                                                        () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.SignalingNorm),
                                                        RiskeerCommonDataResources.AssessmentSectionCategoryType_SignalingNorm_DisplayName),
                new DesignWaterLevelCalculationsContext(context.AssessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                                        context.AssessmentSection,
                                                        () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.LowerLimitNorm),
                                                        RiskeerCommonDataResources.AssessmentSectionCategoryType_LowerLimitNorm_DisplayName),
                new DesignWaterLevelCalculationsContext(context.AssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                                                        context.AssessmentSection,
                                                        () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedLowerLimitNorm),
                                                        RiskeerCommonDataResources.AssessmentSectionCategoryType_FactorizedLowerLimitNorm_DisplayName)
            };
        }

        private static object[] WaveHeightCalculationsGroupContextChildNodeObjects(WaveHeightCalculationsGroupContext context)
        {
            return new object[]
            {
                new WaveHeightCalculationsContext(context.AssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm,
                                                  context.AssessmentSection,
                                                  () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedSignalingNorm),
                                                  RiskeerCommonDataResources.AssessmentSectionCategoryType_FactorizedSignalingNorm_DisplayName),
                new WaveHeightCalculationsContext(context.AssessmentSection.WaveHeightCalculationsForSignalingNorm,
                                                  context.AssessmentSection,
                                                  () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.SignalingNorm),
                                                  RiskeerCommonDataResources.AssessmentSectionCategoryType_SignalingNorm_DisplayName),
                new WaveHeightCalculationsContext(context.AssessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                                                  context.AssessmentSection,
                                                  () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.LowerLimitNorm),
                                                  RiskeerCommonDataResources.AssessmentSectionCategoryType_LowerLimitNorm_DisplayName),
                new WaveHeightCalculationsContext(context.AssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                                                  context.AssessmentSection,
                                                  () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedLowerLimitNorm),
                                                  RiskeerCommonDataResources.AssessmentSectionCategoryType_FactorizedLowerLimitNorm_DisplayName)
            };
        }

        private static bool HasIllustrationPoints(IAssessmentSection assessmentSection)
        {
            return DesignWaterLevelCalculationsHaveIllustrationPoints(assessmentSection)
                   || WaveHeightCalculationsHaveIllustrationPoints(assessmentSection);
        }

        private static bool WaveHeightCalculationsHaveIllustrationPoints(IAssessmentSection assessmentSection)
        {
            return IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaveHeightCalculationsForSignalingNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaveHeightCalculationsForLowerLimitNorm);
        }

        private static bool DesignWaterLevelCalculationsHaveIllustrationPoints(IAssessmentSection assessmentSection)
        {
            return IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForSignalingNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
        }

        #endregion

        #region AssemblyResults TreeNodeInfo

        private static object[] AssemblyResultsContextChildNodeObjects(AssemblyResultsContext context)
        {
            AssessmentSection assessmentSection = context.WrappedData;
            return new object[]
            {
                new AssemblyResultCategoriesContext(assessmentSection),
                new AssemblyResultTotalContext(assessmentSection),
                new AssemblyResultPerSectionContext(assessmentSection),
                new AssemblyResultPerSectionMapContext(assessmentSection)
            };
        }

        #endregion

        #endregion

        #region Foreshore Profile Update and ImportInfo

        private static FileFilterGenerator CreateForeshoreProfileFileFilterGenerator =>
            new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                    RiskeerCommonIOResources.Shape_file_filter_Description);

        private bool VerifyForeshoreProfileUpdates(ForeshoreProfilesContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.ParentFailureMechanism, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion
    }
}