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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Core.Components.Gis.Data;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.ProgressDialog;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using log4net;
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
using Riskeer.Common.Forms.Helpers;
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
using Riskeer.Common.Util.Helpers;
using Riskeer.Common.Util.TypeConverters;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.DuneErosion.Plugin.Handlers;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.Input;
using Riskeer.Integration.Data.StandAlone.SectionResults;
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
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using CoreGuiResources = Core.Gui.Properties.Resources;
using FontFamily = System.Windows.Media.FontFamily;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;
using RiskeerCommonUtilResources = Riskeer.Common.Util.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonPluginResources = Riskeer.Common.Plugin.Properties.Resources;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;
using RiskeerDataResources = Riskeer.Integration.Data.Properties.Resources;
using RiskeerFormsResources = Riskeer.Integration.Forms.Properties.Resources;
using RiskeerIOResources = Riskeer.Integration.IO.Properties.Resources;

namespace Riskeer.Integration.Plugin
{
    /// <summary>
    /// The plug-in for the Riskeer application.
    /// </summary>
    public class RiskeerPlugin : PluginBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PluginBase));

        private static readonly FontFamily fontFamily = new FontFamily(
            new Uri($"{PackUriHelper.UriSchemePack}://application:,,,/Riskeer.Integration.Plugin;component/Resources/"),
            "./#Symbols");

        private static readonly IDictionary<IView, IEnumerable<IObserver>> observersForViewTitles = new Dictionary<IView, IEnumerable<IObserver>>();

        private IHydraulicBoundaryLocationCalculationGuiService hydraulicBoundaryLocationCalculationGuiService;
        private AssessmentSectionMerger assessmentSectionMerger;

        public override IGui Gui
        {
            get => base.Gui;
            set
            {
                if (base.Gui != null)
                {
                    base.Gui.ProjectOpened -= VerifyHydraulicBoundaryDatabasePath;
                    base.Gui.ViewHost.ViewOpened -= OnViewOpened;
                    base.Gui.ViewHost.ViewClosed -= OnViewClosed;
                }

                base.Gui = value;

                if (value != null)
                {
                    value.ProjectOpened += VerifyHydraulicBoundaryDatabasePath;
                    base.Gui.ViewHost.ViewOpened += OnViewOpened;
                    base.Gui.ViewHost.ViewClosed += OnViewClosed;
                }
            }
        }

        public override void Activate()
        {
            base.Activate();

            if (Gui == null)
            {
                throw new InvalidOperationException("Gui cannot be null");
            }

            hydraulicBoundaryLocationCalculationGuiService = new HydraulicBoundaryLocationCalculationGuiService(Gui.MainWindow);
            assessmentSectionMerger = new AssessmentSectionMerger(new AssessmentSectionMergeFilePathProvider(GetInquiryHelper()),
                                                                  new AssessmentSectionProvider(Gui.MainWindow, Gui.ProjectStore),
                                                                  new AssessmentSectionMergeComparer(),
                                                                  new AssessmentSectionMergeDataProviderDialog(Gui.MainWindow),
                                                                  new AssessmentSectionMergeHandler());
        }

        public override IEnumerable<StateInfo> GetStateInfos()
        {
            yield return new StateInfo(Resources.RiskeerPlugin_GetStateInfos_AssessmentSection, "\uE900", fontFamily, project =>
            {
                if (project is RiskeerProject riskeerProject)
                {
                    AssessmentSection assessmentSection = riskeerProject.AssessmentSections.First();

                    return new AssessmentSectionStateRootContext(assessmentSection);
                }

                return null;
            });

            yield return new StateInfo(Resources.RiskeerPlugin_GetStateInfos_HydraulicLoads, "\uE901", fontFamily, project =>
            {
                if (project is RiskeerProject riskeerProject)
                {
                    return new HydraulicLoadsStateRootContext(riskeerProject.AssessmentSections.First());
                }

                return null;
            });

            yield return new StateInfo(Resources.RiskeerPlugin_GetStateInfos_Calculations, "\uE902", fontFamily, project =>
            {
                if (project is RiskeerProject riskeerProject)
                {
                    return new CalculationsStateRootContext(riskeerProject.AssessmentSections.First());
                }

                return null;
            });

            yield return new StateInfo(Resources.RiskeerPlugin_GetStateInfos_FailurePaths, "\uE903", fontFamily, project =>
            {
                if (project is RiskeerProject riskeerProject)
                {
                    return new FailurePathsStateRootContext(riskeerProject.AssessmentSections.First());
                }

                return null;
            });
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<StateRootContext, AssessmentSectionProperties>
            {
                CreateInstance = context => new AssessmentSectionProperties(context.WrappedData, new AssessmentSectionCompositionChangeHandler(Gui.ViewCommands))
            };
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
            yield return new PropertyInfo<NormContext, NormProperties>
            {
                CreateInstance = context => new NormProperties(
                    context.WrappedData,
                    new FailureMechanismContributionNormChangeHandler(context.AssessmentSection))
            };
            yield return new PropertyInfo<IFailurePathContext<IFailureMechanism>, StandAloneFailurePathProperties>
            {
                CreateInstance = context => new StandAloneFailurePathProperties(context.WrappedData, context.Parent)
            };
            yield return new PropertyInfo<SpecificFailurePathContext, SpecificFailurePathProperties>
            {
                CreateInstance = context => new SpecificFailurePathProperties(context.WrappedData)
            };
            yield return new PropertyInfo<MacroStabilityOutwardsFailurePathContext, MacroStabilityOutwardsFailurePathProperties>
            {
                CreateInstance = context => new MacroStabilityOutwardsFailurePathProperties(context.WrappedData, context.Parent)
            };
            yield return new PropertyInfo<PipingStructureFailurePathContext, PipingStructureFailurePathProperties>
            {
                CreateInstance = context => new PipingStructureFailurePathProperties(context.WrappedData)
            };
            yield return new PropertyInfo<ICalculationContext<CalculationGroup, IFailureMechanism>, CalculationGroupContextProperties>
            {
                CreateInstance = context => new CalculationGroupContextProperties(context)
            };
            yield return new PropertyInfo<ICalculationContext<ICalculation, IFailureMechanism>, CalculationContextProperties>();
            yield return new PropertyInfo<WaterLevelCalculationsForNormTargetProbabilityContext, WaterLevelCalculationsForNormTargetProbabilityProperties>
            {
                CreateInstance = context => new WaterLevelCalculationsForNormTargetProbabilityProperties(context.WrappedData, context.GetNormFunc())
            };
            yield return new PropertyInfo<WaterLevelCalculationsForUserDefinedTargetProbabilityContext, WaterLevelCalculationsForUserDefinedTargetProbabilityProperties>
            {
                CreateInstance = context => new WaterLevelCalculationsForUserDefinedTargetProbabilityProperties(
                    context.WrappedData,
                    new WaterLevelHydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler(context.WrappedData, context.AssessmentSection))
            };
            yield return new PropertyInfo<WaveHeightCalculationsForUserDefinedTargetProbabilityContext, WaveHeightCalculationsForUserDefinedTargetProbabilityProperties>
            {
                CreateInstance = context => new WaveHeightCalculationsForUserDefinedTargetProbabilityProperties(
                    context.WrappedData,
                    new HydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler(context.WrappedData))
            };
            yield return new PropertyInfo<DesignWaterLevelCalculationContext, DesignWaterLevelCalculationProperties>
            {
                CreateInstance = context => new DesignWaterLevelCalculationProperties(context.WrappedData)
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
            yield return new PropertyInfo<AssemblyResultCategoriesContext, AssemblyResultCategoriesProperties>
            {
                CreateInstance = context => new AssemblyResultCategoriesProperties(context.GetAssemblyCategoriesFunc(), context.WrappedData)
            };
            yield return new PropertyInfo<StructuresOutputContext, StructuresOutputProperties>
            {
                CreateInstance = context => new StructuresOutputProperties(context.WrappedData.Output)
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<NormContext, FailureMechanismContribution, AssessmentSectionAssemblyCategoriesView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.Norms_DisplayName,
                Image = RiskeerCommonFormsResources.NormsIcon,
                CloseForData = (view, dataToCloseFor) => dataToCloseFor is IAssessmentSection assessmentSection
                                                         && assessmentSection.FailureMechanismContribution == view.FailureMechanismContribution,
                CreateInstance = context => new AssessmentSectionAssemblyCategoriesView(context.AssessmentSection.FailureMechanismContribution)
            };

            yield return new ViewInfo<WaterLevelCalculationsForNormTargetProbabilityContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, DesignWaterLevelCalculationsView>
            {
                GetViewName = (view, context) => GetWaterLevelCalculationsForNormTargetProbabilitiesViewName(context.WrappedData, context.AssessmentSection),
                GetViewData = context => context.WrappedData,
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseHydraulicBoundaryCalculationsViewForData,
                CreateInstance = context => new DesignWaterLevelCalculationsView(context.WrappedData,
                                                                                 context.AssessmentSection,
                                                                                 context.GetNormFunc,
                                                                                 () => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(context.WrappedData, context.AssessmentSection)),
                AfterCreate = (view, context) =>
                {
                    view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService;
                }
            };

            yield return new ViewInfo<WaterLevelCalculationsForUserDefinedTargetProbabilityContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, DesignWaterLevelCalculationsView>
            {
                GetViewName = (view, context) => GetWaterLevelCalculationsForUserDefinedTargetProbabilitiesViewName(context.WrappedData, context.AssessmentSection),
                GetViewData = context => context.WrappedData.HydraulicBoundaryLocationCalculations,
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseForWaterLevelCalculationsForUserDefinedTargetProbabilityContextData,
                CreateInstance = context => new DesignWaterLevelCalculationsView(context.WrappedData.HydraulicBoundaryLocationCalculations,
                                                                                 context.AssessmentSection,
                                                                                 () => context.WrappedData.TargetProbability,
                                                                                 () => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(context.WrappedData.HydraulicBoundaryLocationCalculations,
                                                                                                                                                                                    context.AssessmentSection)),
                AfterCreate = (view, context) =>
                {
                    view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService;
                }
            };

            yield return new ViewInfo<WaveHeightCalculationsForUserDefinedTargetProbabilityContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, WaveHeightCalculationsView>
            {
                GetViewName = (view, context) => GetWaveHeightCalculationsForUserDefinedTargetProbabilitiesViewName(context.WrappedData, context.AssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities),
                GetViewData = context => context.WrappedData.HydraulicBoundaryLocationCalculations,
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseForWaveHeightCalculationsForUserDefinedTargetProbabilityContextData,
                CreateInstance = context => new WaveHeightCalculationsView(context.WrappedData.HydraulicBoundaryLocationCalculations,
                                                                           context.AssessmentSection,
                                                                           () => context.WrappedData.TargetProbability,
                                                                           () => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(context.WrappedData,
                                                                                                                                                                    context.AssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                                                                                                                                                    probability => probability.TargetProbability)),
                AfterCreate = (view, context) =>
                {
                    view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService;
                }
            };

            yield return new ViewInfo<AssessmentSectionStateRootContext, AssessmentSectionReferenceLineView>
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssessmentSectionMap_DisplayName,
                Image = RiskeerFormsResources.Map,
                CreateInstance = context => new AssessmentSectionReferenceLineView(context.WrappedData),
                CloseForData = CloseAssessmentSectionViewForData
            };

            yield return new ViewInfo<HydraulicLoadsStateRootContext, AssessmentSectionExtendedView>
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssessmentSectionMap_DisplayName,
                Image = RiskeerFormsResources.Map,
                CreateInstance = context => new AssessmentSectionExtendedView(context.WrappedData),
                CloseForData = CloseAssessmentSectionViewForData
            };

            yield return new ViewInfo<CalculationsStateRootContext, AssessmentSectionExtendedView>
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssessmentSectionMap_DisplayName,
                Image = RiskeerFormsResources.Map,
                CreateInstance = context => new AssessmentSectionExtendedView(context.WrappedData),
                CloseForData = CloseAssessmentSectionViewForData
            };

            yield return new ViewInfo<FailurePathsStateRootContext, AssessmentSectionExtendedView>
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssessmentSectionMap_DisplayName,
                Image = RiskeerFormsResources.Map,
                CreateInstance = context => new AssessmentSectionExtendedView(context.WrappedData),
                CloseForData = CloseAssessmentSectionViewForData
            };

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<MacroStabilityOutwardsFailurePathContext, MacroStabilityOutwardsFailureMechanism, MacroStabilityOutwardsFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<MacroStabilityOutwardsFailureMechanism, MacroStabilityOutwardsFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => MacroStabilityOutwardsAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => MacroStabilityOutwardsAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData, context.Parent),
                    () => MacroStabilityOutwardsAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData, context.Parent),
                    () => MacroStabilityOutwardsAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData, context.Parent)));

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<MicrostabilityFailurePathContext, MicrostabilityFailureMechanism, MicrostabilityFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<MicrostabilityFailureMechanism, MicrostabilityFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => MicrostabilityAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => MicrostabilityAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData),
                    () => MicrostabilityAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => MicrostabilityAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<GrassCoverSlipOffOutwardsFailurePathContext, GrassCoverSlipOffOutwardsFailureMechanism, GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<GrassCoverSlipOffOutwardsFailureMechanism, GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => GrassCoverSlipOffOutwardsAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffOutwardsAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffOutwardsAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffOutwardsAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<GrassCoverSlipOffInwardsFailurePathContext, GrassCoverSlipOffInwardsFailureMechanism, GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<GrassCoverSlipOffInwardsFailureMechanism, GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => GrassCoverSlipOffInwardsAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffInwardsAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffInwardsAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => GrassCoverSlipOffInwardsAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithDetailedAssessmentViewInfo<PipingStructureFailurePathContext, PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult>(
                context => new FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => PipingStructureAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => PipingStructureAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(context.WrappedData),
                    () => PipingStructureAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => PipingStructureAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithoutDetailedAssessmentViewInfo<StrengthStabilityLengthwiseConstructionFailurePathContext, StrengthStabilityLengthwiseConstructionFailureMechanism, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(
                context => new FailureMechanismWithoutDetailedAssessmentView<StrengthStabilityLengthwiseConstructionFailureMechanism, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithoutDetailedAssessmentViewInfo<TechnicalInnovationFailurePathContext, TechnicalInnovationFailureMechanism, TechnicalInnovationFailureMechanismSectionResult>(
                context => new FailureMechanismWithoutDetailedAssessmentView<TechnicalInnovationFailureMechanism, TechnicalInnovationFailureMechanismSectionResult>(
                    context.WrappedData,
                    context.Parent,
                    () => TechnicalInnovationAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(context.WrappedData),
                    () => TechnicalInnovationAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(context.WrappedData),
                    () => TechnicalInnovationAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(context.WrappedData)));

            yield return CreateFailureMechanismWithoutDetailedAssessmentViewInfo<WaterPressureAsphaltCoverFailurePathContext, WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult>(
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

            yield return new ViewInfo<SpecificFailurePathContext, SpecificFailurePathView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                CreateInstance = context => new SpecificFailurePathView(context.WrappedData, context.Parent),
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailurePathView
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
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailurePathView,
                CreateInstance = context => new FailureMechanismSectionsView(context.WrappedData.Sections, context.WrappedData),
                GetViewData = context => context.WrappedData.Sections
            };

            yield return new ViewInfo<StructuresOutputContext, IStructuresCalculation, GeneralResultFaultTreeIllustrationPointView>
            {
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                GetViewName = (view, context) => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new GeneralResultFaultTreeIllustrationPointView(
                    context.WrappedData, () => context.WrappedData.Output?.GeneralResult)
            };

            yield return new ViewInfo<AssemblyResultTotalContext, AssessmentSection, AssemblyResultTotalView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.CombinedAssembly_DisplayName,
                Image = Resources.AssemblyResultTotal,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
                CreateInstance = context => new AssemblyResultTotalView(context.WrappedData)
            };

            yield return new ViewInfo<AssemblyResultPerSectionContext, AssessmentSection, AssemblyResultPerSectionView>
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssemblyResultPerSection_DisplayName,
                Image = Resources.AssemblyResultPerSection,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
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
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
                CreateInstance = context => new AssemblyResultCategoriesView(context.WrappedData,
                                                                             context.GetAssemblyCategoriesFunc)
            };
            yield return new ViewInfo<AssemblyResultPerSectionMapContext, AssessmentSection, AssemblyResultPerSectionMapView>
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssemblyResultPerSectionMapView_DisplayName,
                Image = Resources.AssemblyResultPerSectionMap,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
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
                Name = context => RiskeerCommonDataResources.ReferenceLine_DisplayName,
                Extension = RiskeerCommonIOResources.Shape_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new ReferenceLineExporter(context.WrappedData, context.AssessmentSection.Id, filePath),
                IsEnabled = context => HasGeometry(context.AssessmentSection.ReferenceLine),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                                                                           RiskeerCommonIOResources.Shape_file_filter_Description))
            };

            yield return new ExportInfo<HydraulicBoundaryDatabaseContext>
            {
                Name = context => RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Extension = RiskeerCommonIOResources.Zip_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new HydraulicBoundaryLocationCalculationsExporter(context.AssessmentSection, filePath),
                IsEnabled = context => context.WrappedData.IsLinked(),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonIOResources.Zip_file_filter_Extension,
                                                                                                           RiskeerCommonIOResources.Zip_file_filter_Description))
            };

            yield return new ExportInfo<AssemblyResultsContext>
            {
                Name = context => RiskeerCommonFormsResources.AssemblyResult_DisplayName,
                Extension = Resources.AssemblyResult_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new AssemblyExporter(context.WrappedData, filePath),
                IsEnabled = context => HasGeometry(context.WrappedData.ReferenceLine),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(Resources.AssemblyResult_file_filter_Extension,
                                                                                                           RiskeerCommonFormsResources.AssemblyResult_DisplayName))
            };

            yield return CreateHydraulicBoundaryLocationCalculationsForTargetProbabilityExportInfo<
                WaterLevelCalculationsForUserDefinedTargetProbabilityContext>(HydraulicBoundaryLocationCalculationsType.WaterLevel,
                                                                              RiskeerIOResources.WaterLevels_DisplayName);
            yield return CreateHydraulicBoundaryLocationCalculationsForTargetProbabilityExportInfo<
                WaveHeightCalculationsForUserDefinedTargetProbabilityContext>(HydraulicBoundaryLocationCalculationsType.WaveHeight,
                                                                              RiskeerIOResources.WaveHeights_DisplayName);

            yield return new ExportInfo<WaterLevelCalculationsForNormTargetProbabilityContext>
            {
                Name = context => $"{RiskeerIOResources.WaterLevels_DisplayName} ({ProbabilityFormattingHelper.Format(context.GetNormFunc())})",
                Extension = RiskeerCommonIOResources.Shape_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter(
                    context.WrappedData, filePath, HydraulicBoundaryLocationCalculationsType.WaterLevel),
                IsEnabled = context => true,
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                                                                           RiskeerCommonIOResources.Shape_file_filter_Description))
            };

            yield return new ExportInfo<WaterLevelCalculationsForNormTargetProbabilitiesGroupContext>
            {
                Name = context => RiskeerCommonUtilResources.WaterLevelCalculationsForNormTargetProbabilities_DisplayName,
                Extension = RiskeerCommonIOResources.Zip_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                    new[]
                    {
                        new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                            context.AssessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                            context.AssessmentSection.FailureMechanismContribution.LowerLimitNorm),
                        new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                            context.AssessmentSection.WaterLevelCalculationsForSignalingNorm,
                            context.AssessmentSection.FailureMechanismContribution.SignalingNorm)
                    }, HydraulicBoundaryLocationCalculationsType.WaterLevel, filePath),
                IsEnabled = context => true,
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonIOResources.Zip_file_filter_Extension,
                                                                                                           RiskeerCommonIOResources.Zip_file_filter_Description))
            };

            yield return CreateHydraulicBoundaryLocationCalculationsForTargetProbabilityGroupExportInfo
                <WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext>(
                    RiskeerCommonUtilResources.WaterLevelCalculationsForUserDefinedTargetProbabilities_DisplayName,
                    HydraulicBoundaryLocationCalculationsType.WaterLevel,
                    context => context.WrappedData
                                      .Select(tp => new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                                                  tp.HydraulicBoundaryLocationCalculations, tp.TargetProbability))
                                      .ToArray());

            yield return CreateHydraulicBoundaryLocationCalculationsForTargetProbabilityGroupExportInfo
                <WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext>(
                    RiskeerCommonUtilResources.WaveHeightCalculationsForUserDefinedTargetProbabilities_DisplayName,
                    HydraulicBoundaryLocationCalculationsType.WaveHeight,
                    context => context.WrappedData
                                      .Select(tp => new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                                                  tp.HydraulicBoundaryLocationCalculations, tp.TargetProbability))
                                      .ToArray());
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

            yield return new UpdateInfo<SpecificFailurePathSectionsContext>
            {
                CreateFileImporter = (context, filePath) =>
                    new FailureMechanismSectionsImporter(context.WrappedData,
                                                         context.AssessmentSection.ReferenceLine,
                                                         filePath,
                                                         new FailureMechanismSectionReplaceStrategy(context.WrappedData),
                                                         new UpdateMessageProvider()),
                Name = RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.SectionsIcon,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                              RiskeerCommonIOResources.Shape_file_filter_Description),
                CurrentPath = context => context.WrappedData.FailureMechanismSectionSourcePath,
                IsEnabled = context => context.WrappedData.FailureMechanismSectionSourcePath != null
            };
        }

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

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return CreateStateRootTreeNodeInfo<AssessmentSectionStateRootContext>(
                AssessmentSectionStateRootContextChildNodeObjects,
                AssessmentSectionStateRootContextMenuStrip);

            yield return CreateStateRootTreeNodeInfo<HydraulicLoadsStateRootContext>(
                HydraulicLoadsStateRootContextChildNodeObjects,
                HydraulicLoadsStateRootContextMenuStrip);

            yield return CreateStateRootTreeNodeInfo<CalculationsStateRootContext>(
                CalculationsStateRootContextChildNodeObjects,
                CalculationsStateRootContextMenuStrip);

            yield return CreateStateRootTreeNodeInfo<FailurePathsStateRootContext>(
                FailurePathsStateRootContextChildNodeObjects,
                FailurePathsStateRootContextMenuStrip);

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

            yield return RiskeerTreeNodeInfoFactory.CreateFailurePathContextTreeNodeInfo<GrassCoverSlipOffInwardsFailurePathContext>(
                GrassCoverSlipOffInwardsFailurePathEnabledChildNodeObjects,
                StandAloneFailurePathDisabledChildNodeObjects,
                StandAloneFailurePathEnabledContextMenuStrip,
                StandAloneFailurePathDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailurePathContextTreeNodeInfo<GrassCoverSlipOffOutwardsFailurePathContext>(
                GrassCoverSlipOffOutwardsFailurePathEnabledChildNodeObjects,
                StandAloneFailurePathDisabledChildNodeObjects,
                StandAloneFailurePathEnabledContextMenuStrip,
                StandAloneFailurePathDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailurePathContextTreeNodeInfo<MacroStabilityOutwardsFailurePathContext>(
                MacroStabilityOutwardsFailurePathEnabledChildNodeObjects,
                StandAloneFailurePathDisabledChildNodeObjects,
                StandAloneFailurePathEnabledContextMenuStrip,
                StandAloneFailurePathDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailurePathContextTreeNodeInfo<MicrostabilityFailurePathContext>(
                MicrostabilityFailurePathEnabledChildNodeObjects,
                StandAloneFailurePathDisabledChildNodeObjects,
                StandAloneFailurePathEnabledContextMenuStrip,
                StandAloneFailurePathDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailurePathContextTreeNodeInfo<PipingStructureFailurePathContext>(
                PipingStructureFailurePathEnabledChildNodeObjects,
                StandAloneFailurePathDisabledChildNodeObjects,
                StandAloneFailurePathEnabledContextMenuStrip,
                StandAloneFailurePathDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailurePathContextTreeNodeInfo<StrengthStabilityLengthwiseConstructionFailurePathContext>(
                StrengthStabilityLengthwiseConstructionFailurePathEnabledChildNodeObjects,
                StandAloneFailurePathDisabledChildNodeObjects,
                StandAloneFailurePathEnabledContextMenuStrip,
                StandAloneFailurePathDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailurePathContextTreeNodeInfo<TechnicalInnovationFailurePathContext>(
                TechnicalInnovationFailurePathEnabledChildNodeObjects,
                StandAloneFailurePathDisabledChildNodeObjects,
                StandAloneFailurePathEnabledContextMenuStrip,
                StandAloneFailurePathDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailurePathContextTreeNodeInfo<WaterPressureAsphaltCoverFailurePathContext>(
                WaterPressureAsphaltCoverFailurePathEnabledChildNodeObjects,
                StandAloneFailurePathDisabledChildNodeObjects,
                StandAloneFailurePathEnabledContextMenuStrip,
                StandAloneFailurePathDisabledContextMenuStrip);

            yield return CreateSpecificFailurePathTreeNodeInfo();

            yield return new TreeNodeInfo<GenericFailurePathsContext>
            {
                Text = context => Resources.GenericFailurePaths_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ChildNodeObjects = GenericFailurePathChildNodeObjects,
                ContextMenuStrip = GenericFailurePathsContextMenuStrip,
                ExpandOnCreate = context => true
            };

            yield return new TreeNodeInfo<SpecificFailurePathsContext>
            {
                Text = context => Resources.SpecificFailurePaths_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ChildNodeObjects = SpecificFailurePathsChildNodeObjects,
                ContextMenuStrip = SpecificFailurePathsContextMenuStrip,
                CanInsert = SpecificFailurePathsContext_CanDropOrInsert,
                CanDrop = SpecificFailurePathsContext_CanDropOrInsert,
                OnDrop = SpecificFailurePathsContext_OnDrop,
                ExpandOnCreate = context => true
            };

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

            yield return new TreeNodeInfo<WaterLevelCalculationsForNormTargetProbabilitiesGroupContext>
            {
                Text = context => RiskeerCommonUtilResources.WaterLevelCalculationsForNormTargetProbabilities_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = WaterLevelCalculationsForNormTargetProbabilitiesGroupContextMenuStrip,
                ChildNodeObjects = WaterLevelCalculationsForNormTargetProbabilitiesGroupContextChildNodes
            };

            yield return new TreeNodeInfo<WaterLevelCalculationsForNormTargetProbabilityContext>
            {
                Text = context => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(context.WrappedData,
                                                                                                                               context.AssessmentSection),
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = WaterLevelCalculationsForNormTargetProbabilityContextMenuStrip
            };

            yield return new TreeNodeInfo<WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext>
            {
                Text = context => RiskeerCommonUtilResources.WaterLevelCalculationsForUserDefinedTargetProbabilities_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContextMenuStrip,
                ChildNodeObjects = WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContextChildNodes,
                CanInsert = WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext_CanDropOrInsert,
                CanDrop = WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext_CanDropOrInsert,
                OnDrop = WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext_OnDrop,
                OnRemoveChildNodesConfirmationText = context => Resources.RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_TargetProbabilities
            };

            yield return new TreeNodeInfo<WaterLevelCalculationsForUserDefinedTargetProbabilityContext>
            {
                Text = context => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(context.WrappedData.HydraulicBoundaryLocationCalculations,
                                                                                                                               context.AssessmentSection),
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                EnsureVisibleOnCreate = (context, o) => true,
                OnRemoveConfirmationText = context => Resources.RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_TargetProbability,
                CanRemove = (context, o) => true,
                OnNodeRemoved = WaterLevelHydraulicBoundaryCalculationsForUserDefinedTargetProbabilityOnNodeRemoved,
                ContextMenuStrip = WaterLevelCalculationsForUserDefinedTargetProbabilityContextMenuStrip,
                CanDrag = (context, o) => true
            };

            yield return new TreeNodeInfo<WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext>
            {
                Text = context => RiskeerCommonUtilResources.WaveHeightCalculationsForUserDefinedTargetProbabilities_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContextMenuStrip,
                ChildNodeObjects = WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContextChildNodes,
                CanInsert = WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext_CanDropOrInsert,
                CanDrop = WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext_CanDropOrInsert,
                OnDrop = WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext_OnDrop,
                OnRemoveChildNodesConfirmationText = context => Resources.RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_TargetProbabilities
            };

            yield return new TreeNodeInfo<WaveHeightCalculationsForUserDefinedTargetProbabilityContext>
            {
                Text = context => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(context.WrappedData,
                                                                                                                     context.AssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                                                                                                     probability => probability.TargetProbability),
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                EnsureVisibleOnCreate = (context, o) => true,
                OnRemoveConfirmationText = context => Resources.RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_TargetProbability,
                CanRemove = (context, o) => true,
                OnNodeRemoved = HydraulicBoundaryCalculationsForUserDefinedTargetProbabilityOnNodeRemoved,
                ContextMenuStrip = WaveHeightCalculationsForUserDefinedTargetProbabilityContextMenuStrip,
                CanDrag = (context, o) => true
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
                ExpandOnCreate = context => true,
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

        private ExportInfo<T> CreateHydraulicBoundaryLocationCalculationsForTargetProbabilityExportInfo<T>(
            HydraulicBoundaryLocationCalculationsType calculationsType, string displayName)
            where T : HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext
        {
            return new ExportInfo<T>
            {
                Name = context => $"{displayName} ({ProbabilityFormattingHelper.Format(context.WrappedData.TargetProbability)})",
                Extension = RiskeerCommonIOResources.Shape_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter(
                    context.WrappedData.HydraulicBoundaryLocationCalculations, filePath, calculationsType),
                IsEnabled = context => true,
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                                                                           RiskeerCommonIOResources.Shape_file_filter_Description))
            };
        }

        private ExportInfo<T> CreateHydraulicBoundaryLocationCalculationsForTargetProbabilityGroupExportInfo<T>(
            string displayName, HydraulicBoundaryLocationCalculationsType calculationsType,
            Func<T, IEnumerable<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>>> locationCalculationsForTargetProbabilitiesFunc)
            where T : HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext
        {
            return new ExportInfo<T>
            {
                Name = context => displayName,
                Extension = RiskeerCommonIOResources.Zip_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                    locationCalculationsForTargetProbabilitiesFunc(context), calculationsType, filePath),
                IsEnabled = context => locationCalculationsForTargetProbabilitiesFunc(context).Any(),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonIOResources.Zip_file_filter_Extension,
                                                                                                           RiskeerCommonIOResources.Zip_file_filter_Description))
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

        private void OnViewOpened(object sender, ViewChangeEventArgs e)
        {
            IEnumerable<IObserver> observers = Enumerable.Empty<IObserver>();
            if (e.View is DesignWaterLevelCalculationsView designWaterLevelCalculationsView)
            {
                observers = CreateObservers(designWaterLevelCalculationsView);
            }
            else if (e.View is WaveHeightCalculationsView waveHeightCalculationsView)
            {
                observers = CreateObservers(waveHeightCalculationsView);
            }
            else if (e.View is SpecificFailurePathView specificFailurePathView)
            {
                observers = CreateObservers(specificFailurePathView);
            }

            if (!observers.Any())
            {
                return;
            }

            observersForViewTitles[e.View] = observers;
        }

        private IEnumerable<IObserver> CreateObservers(DesignWaterLevelCalculationsView designWaterLevelCalculationsView)
        {
            IAssessmentSection assessmentSection = designWaterLevelCalculationsView.AssessmentSection;

            object viewData = designWaterLevelCalculationsView.Data;
            if (ReferenceEquals(viewData, assessmentSection.WaterLevelCalculationsForSignalingNorm)
                || ReferenceEquals(viewData, assessmentSection.WaterLevelCalculationsForLowerLimitNorm))
            {
                IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculationsForUserDefinedTargetProbability =
                    ReferenceEquals(viewData, assessmentSection.WaterLevelCalculationsForSignalingNorm)
                        ? assessmentSection.WaterLevelCalculationsForSignalingNorm
                        : assessmentSection.WaterLevelCalculationsForLowerLimitNorm;

                Func<string> getTitleFunc = () => GetWaterLevelCalculationsForNormTargetProbabilitiesViewName(calculationsForUserDefinedTargetProbability, assessmentSection);

                return new[]
                {
                    CreateViewTitleObserver(designWaterLevelCalculationsView, assessmentSection.FailureMechanismContribution, getTitleFunc)
                };
            }
            else
            {
                IObservableEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> userDefinedTargetProbabilities =
                    assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities;
                HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForUserDefinedTargetProbability =
                    userDefinedTargetProbabilities.SingleOrDefault(calculations => ReferenceEquals(calculations.HydraulicBoundaryLocationCalculations, designWaterLevelCalculationsView.Data));

                if (calculationsForUserDefinedTargetProbability != null)
                {
                    Func<string> getTitleFunc = () => GetWaterLevelCalculationsForUserDefinedTargetProbabilitiesViewName(calculationsForUserDefinedTargetProbability, assessmentSection);

                    return new[]
                    {
                        CreateViewTitleObserver(designWaterLevelCalculationsView, assessmentSection.FailureMechanismContribution, getTitleFunc),
                        CreateViewTitleObserver(designWaterLevelCalculationsView, (IObservable) userDefinedTargetProbabilities, getTitleFunc),
                        CreateViewTitleObserver(designWaterLevelCalculationsView, userDefinedTargetProbabilities, getTitleFunc)
                    };
                }
            }

            return Enumerable.Empty<IObserver>();
        }

        private IEnumerable<IObserver> CreateObservers(WaveHeightCalculationsView waveHeightCalculationsView)
        {
            IAssessmentSection assessmentSection = waveHeightCalculationsView.AssessmentSection;
            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> userDefinedTargetProbabilities =
                assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities;

            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForUserDefinedTargetProbability =
                userDefinedTargetProbabilities.SingleOrDefault(calculations => ReferenceEquals(calculations.HydraulicBoundaryLocationCalculations, waveHeightCalculationsView.Data));

            if (calculationsForUserDefinedTargetProbability != null)
            {
                Func<string> getTitleFunc = () => GetWaveHeightCalculationsForUserDefinedTargetProbabilitiesViewName(calculationsForUserDefinedTargetProbability,
                                                                                                                     userDefinedTargetProbabilities);
                return new[]
                {
                    CreateViewTitleObserver(waveHeightCalculationsView, (IObservable) userDefinedTargetProbabilities, getTitleFunc),
                    CreateViewTitleObserver(waveHeightCalculationsView, userDefinedTargetProbabilities, getTitleFunc)
                };
            }

            return Enumerable.Empty<IObserver>();
        }

        private IEnumerable<IObserver> CreateObservers(SpecificFailurePathView specificFailurePathView)
        {
            IFailurePath failurePath = specificFailurePathView.FailurePath;
            return new[]
            {
                CreateViewTitleObserver(specificFailurePathView, failurePath, () => failurePath.Name)
            };
        }

        private static void OnViewClosed(object sender, ViewChangeEventArgs e)
        {
            if (observersForViewTitles.TryGetValue(e.View, out IEnumerable<IObserver> observersForViewTitle))
            {
                foreach (IObserver observer in observersForViewTitle)
                {
                    var disposableObserver = observer as IDisposable;
                    disposableObserver?.Dispose();
                }

                observersForViewTitles.Remove(e.View);
            }
        }

        /// <summary>
        /// Creates an observer to update the title of the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The <see cref="IView"/> to create the <see cref="IObserver"/> for.</param>
        /// <param name="observable">The <see cref="IObservable"/> to observe.</param>
        /// <param name="getTitleFunc">The title that should be set on the <paramref name="view"/>.</param>
        /// <returns>An <see cref="IObserver"/>.</returns>
        private IObserver CreateViewTitleObserver(IView view,
                                                  IObservable observable,
                                                  Func<string> getTitleFunc)
        {
            return new Observer(() => Gui.ViewHost.SetTitle(view, getTitleFunc()))
            {
                Observable = observable
            };
        }

        /// <summary>
        /// Creates an observer to update the title of the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The <see cref="IView"/> to create the <see cref="IObserver"/> for.</param>
        /// <param name="targetProbabilities">The collection of <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/> to observe.</param>
        /// <param name="getTitleFunc">The title that should be set on the <paramref name="view"/>.</param>
        /// <returns>An <see cref="IObserver"/>.</returns>
        private IObserver CreateViewTitleObserver(IView view,
                                                  IObservableEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities,
                                                  Func<string> getTitleFunc)
        {
            return new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability>, HydraulicBoundaryLocationCalculationsForTargetProbability>(
                () => Gui.ViewHost.SetTitle(view, getTitleFunc()), targetProbability => targetProbability)
            {
                Observable = targetProbabilities
            };
        }

        private static string GetWaterLevelCalculationsForNormTargetProbabilitiesViewName(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                                          IAssessmentSection assessmentSection)
        {
            return $"{RiskeerCommonUtilResources.WaterLevelCalculationsForNormTargetProbabilities_DisplayName} - " +
                   $"{TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(calculations, assessmentSection)}";
        }

        private static string GetWaterLevelCalculationsForUserDefinedTargetProbabilitiesViewName(
            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForUserDefinedTargetProbability,
            IAssessmentSection assessmentSection)
        {
            if (!assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Contains(calculationsForUserDefinedTargetProbability))
            {
                return null;
            }

            string targetProbability = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(calculationsForUserDefinedTargetProbability.HydraulicBoundaryLocationCalculations,
                                                                                                                                    assessmentSection);
            return $"{RiskeerCommonUtilResources.WaterLevelCalculationsForUserDefinedTargetProbabilities_DisplayName} - {targetProbability}";
        }

        private static string GetWaveHeightCalculationsForUserDefinedTargetProbabilitiesViewName(
            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForUserDefinedTargetProbability,
            IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> calculationsForUserDefinedTargetProbabilities)
        {
            if (!calculationsForUserDefinedTargetProbabilities.Contains(calculationsForUserDefinedTargetProbability))
            {
                return null;
            }

            string targetProbability = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(calculationsForUserDefinedTargetProbability,
                                                                                                                          calculationsForUserDefinedTargetProbabilities,
                                                                                                                          probability => probability.TargetProbability);
            return $"{RiskeerCommonUtilResources.WaveHeightCalculationsForUserDefinedTargetProbabilities_DisplayName} - {targetProbability}";
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

        private static bool WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext_CanDropOrInsert(object draggedData, object targetData)
        {
            var waterLevelCalculationsForTargetProbabilitiesGroupContext = (WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext) targetData;

            return draggedData is WaterLevelCalculationsForUserDefinedTargetProbabilityContext waterLevelCalculationsForTargetProbabilityContext
                   && waterLevelCalculationsForTargetProbabilitiesGroupContext.WrappedData.Contains(waterLevelCalculationsForTargetProbabilityContext.WrappedData);
        }

        private static void WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext_OnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
        {
            var waterLevelCalculationsForTargetProbabilitiesGroupContext = (WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext) newParentData;
            var waterLevelCalculationsForTargetProbabilityContext = (WaterLevelCalculationsForUserDefinedTargetProbabilityContext) droppedData;

            waterLevelCalculationsForTargetProbabilitiesGroupContext.WrappedData.Remove(
                waterLevelCalculationsForTargetProbabilityContext.WrappedData);
            waterLevelCalculationsForTargetProbabilitiesGroupContext.WrappedData.Insert(
                position, waterLevelCalculationsForTargetProbabilityContext.WrappedData);

            waterLevelCalculationsForTargetProbabilitiesGroupContext.WrappedData.NotifyObservers();
        }

        private static bool WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext_CanDropOrInsert(object draggedData, object targetData)
        {
            var waveHeightCalculationsForTargetProbabilitiesGroupContext = (WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext) targetData;

            return draggedData is WaveHeightCalculationsForUserDefinedTargetProbabilityContext waveHeightCalculationsForTargetProbabilityContext
                   && waveHeightCalculationsForTargetProbabilitiesGroupContext.WrappedData.Contains(waveHeightCalculationsForTargetProbabilityContext.WrappedData);
        }

        private static void WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext_OnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
        {
            var waveHeightCalculationsForTargetProbabilitiesGroupContext = (WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext) newParentData;
            var waveHeightCalculationsForTargetProbabilityContext = (WaveHeightCalculationsForUserDefinedTargetProbabilityContext) droppedData;

            waveHeightCalculationsForTargetProbabilitiesGroupContext.WrappedData.Remove(
                waveHeightCalculationsForTargetProbabilityContext.WrappedData);
            waveHeightCalculationsForTargetProbabilitiesGroupContext.WrappedData.Insert(
                position, waveHeightCalculationsForTargetProbabilityContext.WrappedData);

            waveHeightCalculationsForTargetProbabilitiesGroupContext.WrappedData.NotifyObservers();
        }

        #region ViewInfos

        #region FailureMechanismWithDetailedAssessmentView ViewInfo

        private static ViewInfo<TFailureMechanismContext, TFailureMechanism, FailureMechanismWithDetailedAssessmentView<TFailureMechanism, TSectionResult>> CreateFailureMechanismWithDetailedAssessmentViewInfo<
            TFailureMechanismContext, TFailureMechanism, TSectionResult>(
            Func<TFailureMechanismContext, FailureMechanismWithDetailedAssessmentView<TFailureMechanism, TSectionResult>> createInstanceFunc)
            where TSectionResult : FailureMechanismSectionResult
            where TFailureMechanism : FailureMechanismBase, IHasSectionResults<TSectionResult>
            where TFailureMechanismContext : IFailurePathContext<TFailureMechanism>
        {
            return new ViewInfo<TFailureMechanismContext, TFailureMechanism,
                FailureMechanismWithDetailedAssessmentView<TFailureMechanism, TSectionResult>>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = createInstanceFunc
            };
        }

        #endregion

        #region FailureMechanismWithoutDetailedAssessmentView ViewInfo

        private static ViewInfo<TFailureMechanismContext, TFailureMechanism, FailureMechanismWithoutDetailedAssessmentView<TFailureMechanism, TSectionResult>> CreateFailureMechanismWithoutDetailedAssessmentViewInfo<
            TFailureMechanismContext, TFailureMechanism, TSectionResult>(
            Func<TFailureMechanismContext, FailureMechanismWithoutDetailedAssessmentView<TFailureMechanism, TSectionResult>> createInstanceFunc)
            where TSectionResult : FailureMechanismSectionResult
            where TFailureMechanism : FailureMechanismBase, IHasSectionResults<TSectionResult>
            where TFailureMechanismContext : IFailurePathContext<TFailureMechanism>
        {
            return new ViewInfo<TFailureMechanismContext, TFailureMechanism,
                FailureMechanismWithoutDetailedAssessmentView<TFailureMechanism, TSectionResult>>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = createInstanceFunc
            };
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
            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                return assessmentSection.GetFailureMechanisms()
                                        .OfType<IHasSectionResults<FailureMechanismSectionResult>>()
                                        .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (dataToCloseFor is IFailurePathContext<IFailureMechanism> failureMechanismContext)
            {
                return failureMechanismContext.WrappedData is IHasSectionResults<FailureMechanismSectionResult> failureMechanismWithSectionResults
                       && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanismWithSectionResults.SectionResults);
            }

            return false;
        }

        #endregion

        #region StateRootContext ViewInfo

        private static bool CloseAssessmentSectionViewForData(AssessmentSectionReferenceLineView view, object dataToCloseFor)
        {
            return ReferenceEquals(view.AssessmentSection, dataToCloseFor);
        }

        #endregion

        #region HydraulicBoundaryCalculationsView ViewInfo

        private static bool CloseHydraulicBoundaryCalculationsViewForData(HydraulicBoundaryCalculationsView view, object dataToCloseFor)
        {
            return ReferenceEquals(view.AssessmentSection, dataToCloseFor);
        }

        private static bool CloseForWaterLevelCalculationsForUserDefinedTargetProbabilityContextData(DesignWaterLevelCalculationsView view, object dataToCloseFor)
        {
            if (dataToCloseFor is WaterLevelCalculationsForUserDefinedTargetProbabilityContext context
                && ReferenceEquals(context.WrappedData.HydraulicBoundaryLocationCalculations, view.Data))
            {
                return true;
            }

            return CloseHydraulicBoundaryCalculationsViewForData(view, dataToCloseFor);
        }

        private static bool CloseForWaveHeightCalculationsForUserDefinedTargetProbabilityContextData(WaveHeightCalculationsView view, object dataToCloseFor)
        {
            if (dataToCloseFor is WaveHeightCalculationsForUserDefinedTargetProbabilityContext context
                && ReferenceEquals(context.WrappedData.HydraulicBoundaryLocationCalculations, view.Data))
            {
                return true;
            }

            return CloseHydraulicBoundaryCalculationsViewForData(view, dataToCloseFor);
        }

        #endregion

        #region Comment ViewInfo

        private static bool CloseCommentViewForData(CommentView commentView, object dataToCloseFor)
        {
            if (dataToCloseFor is ICalculationContext<CalculationGroup, IFailureMechanism> calculationGroupContext)
            {
                return GetCommentElements(calculationGroupContext.WrappedData)
                    .Any(commentElement => ReferenceEquals(commentView.Data, commentElement));
            }

            var calculationContext = dataToCloseFor as ICalculationContext<ICalculationBase, IFailureMechanism>;
            if (calculationContext?.WrappedData is ICalculation calculation)
            {
                return ReferenceEquals(commentView.Data, calculation.Comments);
            }

            if (dataToCloseFor is IFailurePathContext<IFailureMechanism> failureMechanismContext)
            {
                return GetCommentElements(failureMechanismContext.WrappedData)
                    .Any(commentElement => ReferenceEquals(commentView.Data, commentElement));
            }

            if (dataToCloseFor is IFailurePathContext<IFailurePath> failurePathContext)
            {
                return GetCommentElements(failurePathContext.WrappedData)
                    .Any(commentElement => ReferenceEquals(commentView.Data, commentElement));
            }

            if (dataToCloseFor is IAssessmentSection assessmentSection)
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

            foreach (Comment comment in assessmentSection.SpecificFailurePaths.SelectMany(GetCommentElements))
            {
                yield return comment;
            }
        }

        private static IEnumerable<Comment> GetCommentElements(IFailurePath failurePath)
        {
            yield return failurePath.InputComments;
            yield return failurePath.OutputComments;
            yield return failurePath.NotRelevantComments;
        }

        private static IEnumerable<Comment> GetCommentElements(IFailureMechanism failureMechanism)
        {
            foreach (Comment comment in GetCommentElements((IFailurePath) failureMechanism))
            {
                yield return comment;
            }

            foreach (ICalculation calculation in failureMechanism.Calculations)
            {
                yield return calculation.Comments;
            }
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
            var assessmentSectionStateRootContext = (AssessmentSectionStateRootContext) parentData;

            var mapDataItem = new StrictContextMenuItem(
                Resources.BackgroundData_SelectMapData,
                Resources.BackgroundData_SelectMapData_Tooltip,
                RiskeerCommonFormsResources.MapsIcon, (sender, args) => SelectMapData(assessmentSectionStateRootContext.WrappedData, nodeData));

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(mapDataItem)
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        private void SelectMapData(IAssessmentSection assessmentSection, BackgroundData backgroundData)
        {
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

        #region StateRootContext TreeNodeInfo

        private static TreeNodeInfo<TContext> CreateStateRootTreeNodeInfo<TContext>(
            Func<TContext, object[]> childNodeObjectsFunc,
            Func<TContext, object, TreeViewControl, ContextMenuStrip> contextMenuStripFunc)
            where TContext : StateRootContext
        {
            return new TreeNodeInfo<TContext>
            {
                Text = context => context.WrappedData.Name,
                Image = context => RiskeerFormsResources.AssessmentSectionFolderIcon,
                EnsureVisibleOnCreate = (context, parent) => true,
                ExpandOnCreate = context => true,
                ChildNodeObjects = childNodeObjectsFunc,
                ContextMenuStrip = contextMenuStripFunc,
                CanRename = (context, parentData) => true,
                OnNodeRenamed = StateRootContextOnNodeRenamed,
                CanRemove = (context, parentNodeData) => false
            };
        }

        private static void StateRootContextOnNodeRenamed<TContext>(TContext nodeData, string newName)
            where TContext : StateRootContext
        {
            nodeData.WrappedData.Name = newName;
            nodeData.WrappedData.NotifyObservers();
        }

        #endregion

        #region AssessmentSectionStateRootContext TreeNodeInfo

        private static object[] AssessmentSectionStateRootContextChildNodeObjects(AssessmentSectionStateRootContext nodeData)
        {
            return new object[]
            {
                new ReferenceLineContext(nodeData.WrappedData.ReferenceLine, nodeData.WrappedData),
                new NormContext(nodeData.WrappedData.FailureMechanismContribution, nodeData.WrappedData),
                nodeData.WrappedData.BackgroundData,
                nodeData.WrappedData.Comments
            };
        }

        private ContextMenuStrip AssessmentSectionStateRootContextMenuStrip(AssessmentSectionStateRootContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var importItem = new StrictContextMenuItem(
                CoreGuiResources.Import,
                CoreGuiResources.Import_ToolTip,
                CoreGuiResources.ImportIcon,
                (sender, args) => assessmentSectionMerger.StartMerge(nodeData.WrappedData));

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(importItem)
                      .AddSeparator()
                      .AddRenameItem()
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region HydraulicLoadsStateRootContext TreeNodeInfo

        private static object[] HydraulicLoadsStateRootContextChildNodeObjects(HydraulicLoadsStateRootContext nodeData)
        {
            AssessmentSection assessmentSection = nodeData.WrappedData;

            return new object[]
            {
                new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, assessmentSection),
                new StabilityStoneCoverHydraulicLoadsContext(assessmentSection.StabilityStoneCover, assessmentSection),
                new WaveImpactAsphaltCoverHydraulicLoadsContext(assessmentSection.WaveImpactAsphaltCover, assessmentSection),
                new GrassCoverErosionOutwardsHydraulicLoadsContext(assessmentSection.GrassCoverErosionOutwards, assessmentSection),
                new DuneErosionHydraulicLoadsContext(assessmentSection.DuneErosion, assessmentSection)
            };
        }

        private ContextMenuStrip HydraulicLoadsStateRootContextMenuStrip(HydraulicLoadsStateRootContext nodeData,
                                                                         object parentData, TreeViewControl treeViewControl)
        {
            var calculateAllItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.HydraulicLoads_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        Gui.MainWindow,
                        AssessmentSectionCalculationActivityFactory.CreateHydraulicLoadCalculationActivities(nodeData.WrappedData));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.WrappedData, calculateAllItem);

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddRenameItem()
                      .AddSeparator()
                      .AddCustomItem(calculateAllItem)
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region CalculationsStateRootContext TreeNodeInfo

        private static object[] CalculationsStateRootContextChildNodeObjects(CalculationsStateRootContext nodeData)
        {
            AssessmentSection assessmentSection = nodeData.WrappedData;

            return new object[]
            {
                new PipingCalculationsContext(assessmentSection.Piping, assessmentSection),
                new GrassCoverErosionInwardsCalculationsContext(assessmentSection.GrassCoverErosionInwards, assessmentSection),
                new MacroStabilityInwardsCalculationsContext(assessmentSection.MacroStabilityInwards, assessmentSection),
                new HeightStructuresCalculationsContext(assessmentSection.HeightStructures, assessmentSection),
                new ClosingStructuresCalculationsContext(assessmentSection.ClosingStructures, assessmentSection),
                new StabilityPointStructuresCalculationsContext(assessmentSection.StabilityPointStructures, assessmentSection)
            };
        }

        private ContextMenuStrip CalculationsStateRootContextMenuStrip(CalculationsStateRootContext nodeData,
                                                                       object parentData, TreeViewControl treeViewControl)
        {
            var calculateAllItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                Resources.AssessmentSection_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        Gui.MainWindow, AssessmentSectionCalculationActivityFactory.CreateCalculationActivities(nodeData.WrappedData));
                });

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddRenameItem()
                      .AddSeparator()
                      .AddCustomItem(calculateAllItem)
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region FailurePathsStateRootContext TreeNodeInfo

        private static object[] FailurePathsStateRootContextChildNodeObjects(FailurePathsStateRootContext nodeData)
        {
            AssessmentSection assessmentSection = nodeData.WrappedData;

            return new object[]
            {
                new GenericFailurePathsContext(assessmentSection),
                new SpecificFailurePathsContext(assessmentSection.SpecificFailurePaths, assessmentSection),
                new AssemblyResultsContext(assessmentSection)
            };
        }

        private ContextMenuStrip FailurePathsStateRootContextMenuStrip(FailurePathsStateRootContext nodeData,
                                                                       object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddRenameItem()
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region GenericFailurePaths TreeNodeInfo

        private static object[] GenericFailurePathChildNodeObjects(GenericFailurePathsContext nodeData)
        {
            AssessmentSection assessmentSection = nodeData.WrappedData;
            return new object[]
            {
                new PipingFailurePathContext(assessmentSection.Piping, assessmentSection),
                new GrassCoverErosionInwardsFailurePathContext(assessmentSection.GrassCoverErosionInwards, assessmentSection),
                new MacroStabilityInwardsFailurePathContext(assessmentSection.MacroStabilityInwards, assessmentSection),
                new MacroStabilityOutwardsFailurePathContext(assessmentSection.MacroStabilityOutwards, assessmentSection),
                new MicrostabilityFailurePathContext(assessmentSection.Microstability, assessmentSection),
                new StabilityStoneCoverFailurePathContext(assessmentSection.StabilityStoneCover, assessmentSection),
                new WaveImpactAsphaltCoverFailurePathContext(assessmentSection.WaveImpactAsphaltCover, assessmentSection),
                new WaterPressureAsphaltCoverFailurePathContext(assessmentSection.WaterPressureAsphaltCover, assessmentSection),
                new GrassCoverErosionOutwardsFailurePathContext(assessmentSection.GrassCoverErosionOutwards, assessmentSection),
                new GrassCoverSlipOffOutwardsFailurePathContext(assessmentSection.GrassCoverSlipOffOutwards, assessmentSection),
                new GrassCoverSlipOffInwardsFailurePathContext(assessmentSection.GrassCoverSlipOffInwards, assessmentSection),
                new HeightStructuresFailurePathContext(assessmentSection.HeightStructures, assessmentSection),
                new ClosingStructuresFailurePathContext(assessmentSection.ClosingStructures, assessmentSection),
                new PipingStructureFailurePathContext(assessmentSection.PipingStructure, assessmentSection),
                new StabilityPointStructuresFailurePathContext(assessmentSection.StabilityPointStructures, assessmentSection),
                new StrengthStabilityLengthwiseConstructionFailurePathContext(assessmentSection.StrengthStabilityLengthwiseConstruction, assessmentSection),
                new DuneErosionFailurePathContext(assessmentSection.DuneErosion, assessmentSection),
                new TechnicalInnovationFailurePathContext(assessmentSection.TechnicalInnovation, assessmentSection)
            };
        }

        private ContextMenuStrip GenericFailurePathsContextMenuStrip(GenericFailurePathsContext nodeData,
                                                                     object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .Build();
        }

        #endregion

        #region SpecificFailurePaths TreeNodeInfo

        private static object[] SpecificFailurePathsChildNodeObjects(SpecificFailurePathsContext nodeData)
        {
            return nodeData.WrappedData
                           .Cast<SpecificFailurePath>()
                           .Select(sfp => new SpecificFailurePathContext(sfp, nodeData.AssessmentSection))
                           .Cast<object>()
                           .ToArray();
        }

        private ContextMenuStrip SpecificFailurePathsContextMenuStrip(SpecificFailurePathsContext nodeData,
                                                                      object parentData,
                                                                      TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddCustomItem(CreateAddSpecificFailurePathItem(nodeData))
                          .AddSeparator()
                          .AddRemoveAllChildrenItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static StrictContextMenuItem CreateAddSpecificFailurePathItem(SpecificFailurePathsContext nodeData)
        {
            return new StrictContextMenuItem(Resources.RiskeerPlugin_ContextMenuStrip_Add_SpecificFailurePath,
                                             Resources.RiskeerPlugin_ContextMenuStrip_Add_SpecificFailurePath_Tooltip,
                                             RiskeerCommonFormsResources.FailureMechanismIcon,
                                             (sender, args) => AddSpecificFailurePath(nodeData));
        }

        private static void AddSpecificFailurePath(SpecificFailurePathsContext nodeData)
        {
            ObservableList<IFailurePath> failurePaths = nodeData.WrappedData;
            var newFailurePath = new SpecificFailurePath
            {
                Name = NamingHelper.GetUniqueName(failurePaths,
                                                  RiskeerDataResources.SpecificFailurePath_Name_DefaultName,
                                                  fp => fp.Name)
            };
            failurePaths.Add(newFailurePath);
            failurePaths.NotifyObservers();
        }

        private static bool SpecificFailurePathsContext_CanDropOrInsert(object draggedData, object targetData)
        {
            var failurePathsContext = (SpecificFailurePathsContext) targetData;

            return draggedData is SpecificFailurePathContext failurePathContext
                   && failurePathsContext.WrappedData.Contains(failurePathContext.WrappedData);
        }

        private static void SpecificFailurePathsContext_OnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
        {
            var failurePathsContext = (SpecificFailurePathsContext) newParentData;
            var failurePathContext = (SpecificFailurePathContext) droppedData;

            failurePathsContext.WrappedData.Remove(failurePathContext.WrappedData);
            failurePathsContext.WrappedData.Insert(position, failurePathContext.WrappedData);

            failurePathsContext.WrappedData.NotifyObservers();
        }

        #endregion

        #region SpecificFailurePath TreeNodeInfo

        private TreeNodeInfo CreateSpecificFailurePathTreeNodeInfo()
        {
            TreeNodeInfo<SpecificFailurePathContext> treeNodeInfo =
                RiskeerTreeNodeInfoFactory.CreateFailurePathContextTreeNodeInfo<SpecificFailurePathContext>(
                    SpecificFailurePathEnabledChildNodeObjects,
                    SpecificFailurePathDisabledChildNodeObjects,
                    SpecificFailurePathEnabledContextMenuStrip,
                    SpecificFailurePathDisabledContextMenuStrip);
            treeNodeInfo.CanRename = (context, o) => true;
            treeNodeInfo.OnNodeRenamed = SpecificFailurePathContextOnNodeRenamed;
            treeNodeInfo.CanRemove = (context, o) => true;
            treeNodeInfo.OnNodeRemoved = SpecificFailurePathContextOnNodeRemoved;
            treeNodeInfo.CanDrag = (context, o) => true;

            return treeNodeInfo;
        }

        private static void SpecificFailurePathContextOnNodeRenamed(SpecificFailurePathContext nodeData, string newName)
        {
            nodeData.WrappedData.Name = newName;
            nodeData.WrappedData.NotifyObservers();
        }

        private static void SpecificFailurePathContextOnNodeRemoved(SpecificFailurePathContext nodeData, object parentNodeData)
        {
            var specificFailurePathsContext = (SpecificFailurePathsContext) parentNodeData;
            ObservableList<IFailurePath> failurePaths = specificFailurePathsContext.WrappedData;

            failurePaths.Remove(nodeData.WrappedData);
            failurePaths.NotifyObservers();
        }

        private static object[] SpecificFailurePathDisabledChildNodeObjects(SpecificFailurePathContext nodeData)
        {
            return new object[]
            {
                nodeData.WrappedData.NotRelevantComments
            };
        }

        private static object[] SpecificFailurePathEnabledChildNodeObjects(SpecificFailurePathContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetSpecificFailurePathInputs(nodeData),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetSpecificFailureMechanismPathOutputs(nodeData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetSpecificFailurePathInputs(SpecificFailurePathContext nodeData)
        {
            return new object[]
            {
                new SpecificFailurePathSectionsContext(nodeData.WrappedData, nodeData.Parent),
                nodeData.WrappedData.InputComments
            };
        }

        private static IEnumerable<object> GetSpecificFailureMechanismPathOutputs(SpecificFailurePathContext nodeData)
        {
            return new object[]
            {
                nodeData.WrappedData.OutputComments
            };
        }

        private ContextMenuStrip SpecificFailurePathEnabledContextMenuStrip(SpecificFailurePathContext nodeData,
                                                                            object parentData,
                                                                            TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailurePathItem(nodeData, RemoveAllViewsForFailurePathContext)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddSeparator()
                          .AddDeleteItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForFailurePathContext(SpecificFailurePathContext failurePathContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failurePathContext);
        }

        private ContextMenuStrip SpecificFailurePathDisabledContextMenuStrip(SpecificFailurePathContext nodeData,
                                                                             object parentData,
                                                                             TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddToggleRelevancyOfFailurePathItem(nodeData, RemoveAllViewsForFailurePathContext)
                          .AddSeparator()
                          .AddRenameItem()
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

        #region StandAloneFailurePath TreeNodeInfo

        private static object[] StandAloneFailurePathDisabledChildNodeObjects(IFailurePathContext<IFailureMechanism> nodeData)
        {
            return new object[]
            {
                nodeData.WrappedData.NotRelevantComments
            };
        }

        private ContextMenuStrip StandAloneFailurePathEnabledContextMenuStrip(IFailurePathContext<IFailureMechanism> nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailurePathItem(nodeData, RemoveAllViewsForFailureMechanismContext)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForFailureMechanismContext(IFailurePathContext<IFailureMechanism> failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip StandAloneFailurePathDisabledContextMenuStrip(IFailurePathContext<IFailureMechanism> nodeData,
                                                                               object parentData,
                                                                               TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddToggleRelevancyOfFailurePathItem(nodeData, RemoveAllViewsForFailureMechanismContext)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        #endregion

        #region GrassCoverSlipOffInwardsFailurePathContext TreeNodeInfo

        private static object[] GrassCoverSlipOffInwardsFailurePathEnabledChildNodeObjects(GrassCoverSlipOffInwardsFailurePathContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetGrassCoverSlipOffInwardsFailurePathInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetGrassCoverSlipOffInwardsFailurePathOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffInwardsFailurePathInputs(GrassCoverSlipOffInwardsFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverSlipOffInwardsFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffInwardsFailurePathOutputs(GrassCoverSlipOffInwardsFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region GrassCoverSlipOffOutwardsFailurePathContext TreeNodeInfo

        private static object[] GrassCoverSlipOffOutwardsFailurePathEnabledChildNodeObjects(GrassCoverSlipOffOutwardsFailurePathContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetGrassCoverSlipOffOutwardsFailurePathInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetGrassCoverSlipOffOutwardsFailurePathOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffOutwardsFailurePathInputs(GrassCoverSlipOffOutwardsFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverSlipOffOutwardsFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffOutwardsFailurePathOutputs(GrassCoverSlipOffOutwardsFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region MacroStabilityOutwardsFailurePathContext TreeNodeInfo

        private static object[] MacroStabilityOutwardsFailurePathEnabledChildNodeObjects(MacroStabilityOutwardsFailurePathContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetMacroStabilityOutwardsFailurePathInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetMacroStabilityOutwardsFailurePathOutputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetMacroStabilityOutwardsFailurePathInputs(MacroStabilityOutwardsFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MacroStabilityOutwardsFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetMacroStabilityOutwardsFailurePathOutputs(MacroStabilityOutwardsFailureMechanism nodeData,
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

        #region MicrostabilityFailurePathContext TreeNodeInfo

        private static object[] MicrostabilityFailurePathEnabledChildNodeObjects(MicrostabilityFailurePathContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetMicrostabilityFailurePathInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetMicrostabilityFailurePathOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetMicrostabilityFailurePathInputs(MicrostabilityFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MicrostabilityFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetMicrostabilityFailurePathOutputs(MicrostabilityFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region PipingStructureFailurePathContext TreeNodeInfo

        private static object[] PipingStructureFailurePathEnabledChildNodeObjects(PipingStructureFailurePathContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetPipingStructureFailurePathInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetPipingStructureFailurePathOutputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetPipingStructureFailurePathInputs(PipingStructureFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new PipingStructureFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetPipingStructureFailurePathOutputs(PipingStructureFailureMechanism nodeData,
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

        #region StrengthStabilityLengthwiseConstructionFailurePathContext TreeNodeInfo

        private static object[] StrengthStabilityLengthwiseConstructionFailurePathEnabledChildNodeObjects(
            StrengthStabilityLengthwiseConstructionFailurePathContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetStrengthStabilityLengthwiseConstructionFailurePathInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetStrengthStabilityLengthwiseConstructionFailurePathOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetStrengthStabilityLengthwiseConstructionFailurePathInputs(StrengthStabilityLengthwiseConstructionFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new StrengthStabilityLengthwiseConstructionFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetStrengthStabilityLengthwiseConstructionFailurePathOutputs(StrengthStabilityLengthwiseConstructionFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region TechnicalInnovationFailurePathContext TreeNodeInfo

        private static object[] TechnicalInnovationFailurePathEnabledChildNodeObjects(TechnicalInnovationFailurePathContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetTechnicalInnovationFailurePathInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetTechnicalInnovationFailurePathOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetTechnicalInnovationFailurePathInputs(TechnicalInnovationFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new TechnicalInnovationFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetTechnicalInnovationFailurePathOutputs(TechnicalInnovationFailureMechanism nodeData)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData),
                nodeData.OutputComments
            };
        }

        #endregion

        #region WaterPressureAsphaltCoverFailurePathContext TreeNodeInfo

        private static object[] WaterPressureAsphaltCoverFailurePathEnabledChildNodeObjects(WaterPressureAsphaltCoverFailurePathContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetWaterPressureAsphaltCoverFailurePathInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetWaterPressureAsphaltCoverFailurePathOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetWaterPressureAsphaltCoverFailurePathInputs(WaterPressureAsphaltCoverFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new WaterPressureAsphaltCoverFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetWaterPressureAsphaltCoverFailurePathOutputs(WaterPressureAsphaltCoverFailureMechanism nodeData)
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
                    new WaterLevelCalculationsForNormTargetProbabilitiesGroupContext(nodeData.WrappedData.Locations,
                                                                                     nodeData.AssessmentSection),
                    new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(nodeData.AssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                                                                                            nodeData.AssessmentSection),
                    new WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext(nodeData.AssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                                                                            nodeData.AssessmentSection)
                };
            }

            return new object[0];
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

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.AssessmentSection,
                                                        calculateAllItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                RiskeerCommonFormsResources.HydraulicLoads_DisplayName,
                () => RiskeerDataSynchronizationService.ClearIllustrationPointResultsForWaterLevelAndWaveHeightCalculations(nodeData.AssessmentSection));

            AssessmentSection assessmentSection = nodeData.AssessmentSection;
            return builder.AddImportItem(RiskeerFormsResources.HydraulicBoundaryDatabase_Connect,
                                         RiskeerFormsResources.HydraulicBoundaryDatabase_Connect_ToolTip,
                                         RiskeerCommonFormsResources.DatabaseIcon)
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

        private ContextMenuStrip WaterLevelCalculationsForNormTargetProbabilitiesGroupContextMenuStrip(WaterLevelCalculationsForNormTargetProbabilitiesGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;

            IMainWindow guiMainWindow = Gui.MainWindow;
            var waterLevelCalculationItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.WaterLevel_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        guiMainWindow,
                        AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaterLevelCalculationActivitiesForNormTargetProbabilities(assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, waterLevelCalculationItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                RiskeerCommonUtilResources.WaterLevelCalculationsForNormTargetProbabilities_DisplayName,
                () => RiskeerDataSynchronizationService.ClearIllustrationPointResultsOfWaterLevelCalculationsForNormTargetProbabilities(nodeData.AssessmentSection));

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddCustomItem(waterLevelCalculationItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => WaterLevelCalculationsForNormTargetProbabilitiesHaveIllustrationPoints(assessmentSection), changeHandler)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static object[] WaterLevelCalculationsForNormTargetProbabilitiesGroupContextChildNodes(WaterLevelCalculationsForNormTargetProbabilitiesGroupContext context)
        {
            return new object[]
            {
                new WaterLevelCalculationsForNormTargetProbabilityContext(context.AssessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                                                          context.AssessmentSection,
                                                                          () => context.AssessmentSection.FailureMechanismContribution.LowerLimitNorm),
                new WaterLevelCalculationsForNormTargetProbabilityContext(context.AssessmentSection.WaterLevelCalculationsForSignalingNorm,
                                                                          context.AssessmentSection,
                                                                          () => context.AssessmentSection.FailureMechanismContribution.SignalingNorm)
            };
        }

        private ContextMenuStrip WaterLevelCalculationsForNormTargetProbabilityContextMenuStrip(WaterLevelCalculationsForNormTargetProbabilityContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var waterLevelCalculationItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.WaterLevel_Calculate_All_ToolTip,
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
                                                                                              TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(nodeData.WrappedData, assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.AssessmentSection, waterLevelCalculationItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(nodeData.WrappedData, nodeData.AssessmentSection),
                () => RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(nodeData.WrappedData));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddExportItem()
                          .AddSeparator()
                          .AddCustomItem(waterLevelCalculationItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => IllustrationPointsHelper.HasIllustrationPoints(nodeData.WrappedData), changeHandler)
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private ContextMenuStrip WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContextMenuStrip(WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;

            var addTargetProbabilityItem = new StrictContextMenuItem(
                RiskeerCommonPluginResources.ContextMenuStrip_Add_TargetProbability,
                RiskeerCommonPluginResources.ContextMenuStrip_Add_TargetProbability_ToolTip,
                RiskeerCommonFormsResources.GenericInputOutputIcon,
                (sender, args) =>
                {
                    HydraulicBoundaryLocationCalculationsForTargetProbability hydraulicBoundaryLocationCalculationsForTargetProbability = CreateHydraulicBoundaryLocationCalculationsForTargetProbability(assessmentSection);

                    nodeData.WrappedData.Add(hydraulicBoundaryLocationCalculationsForTargetProbability);
                    nodeData.WrappedData.NotifyObservers();
                });

            IMainWindow guiMainWindow = Gui.MainWindow;
            var waterLevelCalculationItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.WaterLevel_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        guiMainWindow,
                        AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaterLevelCalculationActivitiesForUserDefinedTargetProbabilities(assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, waterLevelCalculationItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                RiskeerCommonUtilResources.WaterLevelCalculationsForUserDefinedTargetProbabilities_DisplayName,
                () => RiskeerDataSynchronizationService.ClearIllustrationPointResultsOfWaterLevelCalculationsForUserDefinedTargetProbabilities(nodeData.AssessmentSection));

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddCustomItem(addTargetProbabilityItem)
                          .AddSeparator()
                          .AddCustomItem(waterLevelCalculationItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => WaterLevelCalculationsForUserDefinedTargetProbabilitiesHaveIllustrationPoints(assessmentSection), changeHandler)
                          .AddRemoveAllChildrenItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static object[] WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContextChildNodes(WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext context)
        {
            return context.WrappedData
                          .Select(calculations => (object) new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(calculations, context.AssessmentSection))
                          .ToArray();
        }

        private ContextMenuStrip WaterLevelCalculationsForUserDefinedTargetProbabilityContextMenuStrip(WaterLevelCalculationsForUserDefinedTargetProbabilityContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            string displayNameForWaterLevelCalculations = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(nodeData.WrappedData.HydraulicBoundaryLocationCalculations,
                                                                                                                                                       nodeData.AssessmentSection);
            return HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContextMenuStrip(
                nodeData, treeViewControl, (calculation, section) =>
                    hydraulicBoundaryLocationCalculationGuiService.CalculateDesignWaterLevels(
                        calculation.HydraulicBoundaryLocationCalculations, section, calculation.TargetProbability,
                        displayNameForWaterLevelCalculations),
                displayNameForWaterLevelCalculations,
                RiskeerCommonFormsResources.WaterLevel_Calculate_All_ToolTip);
        }

        private ContextMenuStrip HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContextMenuStrip(HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext nodeData, TreeViewControl treeViewControl,
                                                                                                                      Action<HydraulicBoundaryLocationCalculationsForTargetProbability, IAssessmentSection> calculationAction,
                                                                                                                      string nodeDataDisplayName,
                                                                                                                      string calculationItemToolTip)
        {
            var calculationItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                calculationItemToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    calculationAction(nodeData.WrappedData, nodeData.AssessmentSection);
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.AssessmentSection, calculationItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                nodeDataDisplayName,
                () => RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(nodeData.WrappedData.HydraulicBoundaryLocationCalculations));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddExportItem()
                          .AddSeparator()
                          .AddCustomItem(calculationItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(
                              () => IllustrationPointsHelper.HasIllustrationPoints(nodeData.WrappedData.HydraulicBoundaryLocationCalculations),
                              changeHandler)
                          .AddDeleteItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static void WaterLevelHydraulicBoundaryCalculationsForUserDefinedTargetProbabilityOnNodeRemoved(HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext context, object parentNodeData)
        {
            IEnumerable<IObservable> affectedCalculations = RiskeerDataSynchronizationService.ClearWaveConditionsCalculationOutputAndRemoveTargetProbability(
                context.AssessmentSection, context.WrappedData);
            affectedCalculations.ForEachElementDo(c => c.NotifyObservers());

            HydraulicBoundaryCalculationsForUserDefinedTargetProbabilityOnNodeRemoved(context, parentNodeData);
        }

        private static void HydraulicBoundaryCalculationsForUserDefinedTargetProbabilityOnNodeRemoved(HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext context, object parentNodeData)
        {
            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> parent = ((HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext) parentNodeData).WrappedData;

            parent.Remove(context.WrappedData);
            parent.NotifyObservers();
        }

        private ContextMenuStrip WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContextMenuStrip(WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;

            var addTargetProbabilityItem = new StrictContextMenuItem(
                RiskeerCommonPluginResources.ContextMenuStrip_Add_TargetProbability,
                RiskeerCommonPluginResources.ContextMenuStrip_Add_TargetProbability_ToolTip,
                RiskeerCommonFormsResources.GenericInputOutputIcon,
                (sender, args) =>
                {
                    HydraulicBoundaryLocationCalculationsForTargetProbability hydraulicBoundaryLocationCalculationsForTargetProbability = CreateHydraulicBoundaryLocationCalculationsForTargetProbability(assessmentSection);

                    nodeData.WrappedData.Add(hydraulicBoundaryLocationCalculationsForTargetProbability);
                    nodeData.WrappedData.NotifyObservers();
                });

            IMainWindow guiMainWindow = Gui.MainWindow;
            var waveHeightCalculationItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.WaveHeight_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        guiMainWindow,
                        AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivitiesForUserDefinedTargetProbabilities(assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, waveHeightCalculationItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                GetInquiryHelper(),
                RiskeerCommonUtilResources.WaveHeightCalculationsForUserDefinedTargetProbabilities_DisplayName,
                () => RiskeerDataSynchronizationService.ClearIllustrationPointResultsOfWaveHeightCalculationsForUserDefinedTargetProbabilities(nodeData.AssessmentSection));

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddCustomItem(addTargetProbabilityItem)
                          .AddSeparator()
                          .AddCustomItem(waveHeightCalculationItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => WaveHeightCalculationsForUserDefinedTargetProbabilitiesHaveIllustrationPoints(assessmentSection), changeHandler)
                          .AddRemoveAllChildrenItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static object[] WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContextChildNodes(WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext context)
        {
            return context.WrappedData
                          .Select(calculations => (object) new WaveHeightCalculationsForUserDefinedTargetProbabilityContext(calculations, context.AssessmentSection))
                          .ToArray();
        }

        private ContextMenuStrip WaveHeightCalculationsForUserDefinedTargetProbabilityContextMenuStrip(
            WaveHeightCalculationsForUserDefinedTargetProbabilityContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            string displayNameForWaveHeightCalculations = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(nodeData.WrappedData,
                                                                                                                                             nodeData.AssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                                                                                                                             probability => probability.TargetProbability);
            return HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContextMenuStrip(
                nodeData, treeViewControl, (calculation, section) =>
                    hydraulicBoundaryLocationCalculationGuiService.CalculateWaveHeights(
                        calculation.HydraulicBoundaryLocationCalculations, section, calculation.TargetProbability,
                        displayNameForWaveHeightCalculations),
                displayNameForWaveHeightCalculations,
                RiskeerCommonFormsResources.WaveHeight_Calculate_All_ToolTip);
        }

        private static bool HasIllustrationPoints(IAssessmentSection assessmentSection)
        {
            return WaterLevelCalculationsForNormTargetProbabilitiesHaveIllustrationPoints(assessmentSection)
                   || WaterLevelCalculationsForUserDefinedTargetProbabilitiesHaveIllustrationPoints(assessmentSection)
                   || WaveHeightCalculationsForUserDefinedTargetProbabilitiesHaveIllustrationPoints(assessmentSection);
        }

        private static bool WaterLevelCalculationsForNormTargetProbabilitiesHaveIllustrationPoints(IAssessmentSection assessmentSection)
        {
            return IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForSignalingNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
        }

        private static bool WaterLevelCalculationsForUserDefinedTargetProbabilitiesHaveIllustrationPoints(IAssessmentSection assessmentSection)
        {
            return assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                    .Any(wlc => IllustrationPointsHelper.HasIllustrationPoints(wlc.HydraulicBoundaryLocationCalculations));
        }

        private static bool WaveHeightCalculationsForUserDefinedTargetProbabilitiesHaveIllustrationPoints(IAssessmentSection assessmentSection)
        {
            return assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                    .Any(whc => IllustrationPointsHelper.HasIllustrationPoints(whc.HydraulicBoundaryLocationCalculations));
        }

        private static HydraulicBoundaryLocationCalculationsForTargetProbability CreateHydraulicBoundaryLocationCalculationsForTargetProbability(IAssessmentSection assessmentSection)
        {
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);

            calculationsForTargetProbability.HydraulicBoundaryLocationCalculations.AddRange(
                assessmentSection.HydraulicBoundaryDatabase.Locations.Select(hbl => new HydraulicBoundaryLocationCalculation(hbl)));

            return calculationsForTargetProbability;
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