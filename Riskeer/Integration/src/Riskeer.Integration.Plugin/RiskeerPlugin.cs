// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.ChangeHandlers;
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
using Riskeer.Common.Plugin.FileImporters;
using Riskeer.Common.Service;
using Riskeer.Common.Util;
using Riskeer.Common.Util.Helpers;
using Riskeer.Common.Util.TypeConverters;
using Riskeer.DuneErosion.Plugin.Handlers;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.Integration.Forms.Dialogs;
using Riskeer.Integration.Forms.Merge;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PresentationObjects.StandAlone;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Forms.PropertyClasses.StandAlone;
using Riskeer.Integration.Forms.Views;
using Riskeer.Integration.IO.Exporters;
using Riskeer.Integration.IO.Importers;
using Riskeer.Integration.Plugin.FileImporters;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.Plugin.Merge;
using Riskeer.Integration.Plugin.Properties;
using Riskeer.Integration.Service;
using Riskeer.Integration.Service.Comparers;
using ClosingStructuresCalculationsStateFailureMechanismContext = Riskeer.ClosingStructures.Forms.PresentationObjects.CalculationsState.ClosingStructuresFailureMechanismContext;
using ClosingStructuresRegistrationStateFailureMechanismContext = Riskeer.ClosingStructures.Forms.PresentationObjects.RegistrationState.ClosingStructuresFailureMechanismContext;
using CoreGuiResources = Core.Gui.Properties.Resources;
using DuneErosionHydraulicLoadsStateFailureMechanismContext = Riskeer.DuneErosion.Forms.PresentationObjects.HydraulicLoadsState.DuneErosionFailureMechanismContext;
using DuneErosionRegistrationStateFailureMechanismContext = Riskeer.DuneErosion.Forms.PresentationObjects.RegistrationState.DuneErosionFailureMechanismContext;
using FontFamily = System.Windows.Media.FontFamily;
using GrassCoverErosionInwardsCalculationsStateFailureMechanismContext = Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects.CalculationsState.GrassCoverErosionInwardsFailureMechanismContext;
using GrassCoverErosionInwardsRegistrationStateFailureMechanismContext = Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects.RegistrationState.GrassCoverErosionInwardsFailureMechanismContext;
using GrassCoverErosionOutwardsHydraulicLoadsStateFailureMechanismContext = Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects.HydraulicLoadsState.GrassCoverErosionOutwardsFailureMechanismContext;
using GrassCoverErosionOutwardsRegistrationStateFailureMechanismContext = Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects.RegistrationState.GrassCoverErosionOutwardsFailureMechanismContext;
using HeightStructuresCalculationsStateFailureMechanismContext = Riskeer.HeightStructures.Forms.PresentationObjects.CalculationsState.HeightStructuresFailureMechanismContext;
using HeightStructuresRegistrationStateFailureMechanismContext = Riskeer.HeightStructures.Forms.PresentationObjects.RegistrationState.HeightStructuresFailureMechanismContext;
using MacroStabilityInwardsCalculationsStateFailureMechanismContext = Riskeer.MacroStabilityInwards.Forms.PresentationObjects.CalculationsState.MacroStabilityInwardsFailureMechanismContext;
using MacroStabilityInwardsRegistrationStateFailureMechanismContext = Riskeer.MacroStabilityInwards.Forms.PresentationObjects.RegistrationState.MacroStabilityInwardsFailureMechanismContext;
using PipingCalculationsStateFailureMechanismContext = Riskeer.Piping.Forms.PresentationObjects.CalculationsState.PipingFailureMechanismContext;
using PipingRegistrationStateFailureMechanismContext = Riskeer.Piping.Forms.PresentationObjects.RegistrationState.PipingFailureMechanismContext;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;
using RiskeerCommonUtilResources = Riskeer.Common.Util.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonPluginResources = Riskeer.Common.Plugin.Properties.Resources;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;
using RiskeerDataResources = Riskeer.Integration.Data.Properties.Resources;
using RiskeerFormsResources = Riskeer.Integration.Forms.Properties.Resources;
using RiskeerIOResources = Riskeer.Integration.IO.Properties.Resources;
using StabilityPointStructuresCalculationsStateFailureMechanismContext = Riskeer.StabilityPointStructures.Forms.PresentationObjects.CalculationsState.StabilityPointStructuresFailureMechanismContext;
using StabilityPointStructuresRegistrationStateFailureMechanismContext = Riskeer.StabilityPointStructures.Forms.PresentationObjects.RegistrationState.StabilityPointStructuresFailureMechanismContext;
using StabilityStoneCoverHydraulicLoadsStateFailureMechanismContext = Riskeer.StabilityStoneCover.Forms.PresentationObjects.HydraulicLoadsState.StabilityStoneCoverFailureMechanismContext;
using StabilityStoneCoverRegistrationStateFailureMechanismContext = Riskeer.StabilityStoneCover.Forms.PresentationObjects.RegistrationState.StabilityStoneCoverFailureMechanismContext;
using WaveImpactAsphaltCoverHydraulicLoadsStateFailureMechanismContext = Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects.HydraulicLoadsState.WaveImpactAsphaltCoverFailureMechanismContext;
using WaveImpactAsphaltCoverRegistrationStateFailureMechanismContext = Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects.RegistrationState.WaveImpactAsphaltCoverFailureMechanismContext;

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
                                                                  new AssessmentSectionMergeHandler(Gui.DocumentViewController));
        }

        public override IEnumerable<StateInfo> GetStateInfos()
        {
            yield return new StateInfo(Resources.RiskeerPlugin_GetStateInfos_AssessmentSection, "\uE900", fontFamily, project =>
            {
                if (project is RiskeerProject riskeerProject)
                {
                    return new AssessmentSectionStateRootContext(riskeerProject.AssessmentSection);
                }

                return null;
            });

            yield return new StateInfo(Resources.RiskeerPlugin_GetStateInfos_HydraulicLoads, "\uE901", fontFamily, project =>
            {
                if (project is RiskeerProject riskeerProject)
                {
                    return new HydraulicLoadsStateRootContext(riskeerProject.AssessmentSection);
                }

                return null;
            });

            yield return new StateInfo(Resources.RiskeerPlugin_GetStateInfos_Calculations, "\uE902", fontFamily, project =>
            {
                if (project is RiskeerProject riskeerProject)
                {
                    return new CalculationsStateRootContext(riskeerProject.AssessmentSection);
                }

                return null;
            });

            yield return new StateInfo(Resources.RiskeerPlugin_GetStateInfos_Registration, "\uE903", fontFamily, project =>
            {
                if (project is RiskeerProject riskeerProject)
                {
                    return new RegistrationStateRootContext(riskeerProject.AssessmentSection);
                }

                return null;
            });
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<StateRootContext, AssessmentSectionProperties>
            {
                CreateInstance = context => new AssessmentSectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<RegistrationStateRootContext, RegistrationStateAssessmentSectionProperties>
            {
                CreateInstance = context => new RegistrationStateAssessmentSectionProperties(context.WrappedData,
                                                                                             new AssessmentSectionCompositionChangeHandler(Gui.ViewCommands))
            };
            yield return new PropertyInfo<BackgroundData, BackgroundDataProperties>
            {
                CreateInstance = data => new BackgroundDataProperties(data)
            };
            yield return new PropertyInfo<HydraulicBoundaryDataContext, HydraulicBoundaryDataProperties>
            {
                CreateInstance = context => new HydraulicBoundaryDataProperties(
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
            yield return new PropertyInfo<IFailureMechanismContext<IHasGeneralInput>, StandAloneFailureMechanismProperties>
            {
                CreateInstance = context => new StandAloneFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<SpecificFailureMechanismContext, SpecificFailureMechanismProperties>
            {
                CreateInstance = context => new SpecificFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<PipingStructureFailureMechanismContext, PipingStructureFailureMechanismProperties>
            {
                CreateInstance = context => new PipingStructureFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<ICalculationContext<CalculationGroup, ICalculatableFailureMechanism>, CalculationGroupContextProperties>
            {
                CreateInstance = context => new CalculationGroupContextProperties(context)
            };
            yield return new PropertyInfo<ICalculationContext<ICalculation, ICalculatableFailureMechanism>, CalculationContextProperties>();
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
            yield return new PropertyInfo<StructuresOutputContext, StructuresOutputProperties>
            {
                CreateInstance = context => new StructuresOutputProperties(context.WrappedData.Output)
            };
            yield return new PropertyInfo<FailureMechanismSectionAssemblyGroupsContext, FailureMechanismSectionAssemblyGroupsProperties>
            {
                CreateInstance = context => new FailureMechanismSectionAssemblyGroupsProperties(context.WrappedData)
            };
            yield return new PropertyInfo<AssessmentSectionAssemblyGroupsContext, AssessmentSectionAssemblyGroupsProperties>
            {
                CreateInstance = context => new AssessmentSectionAssemblyGroupsProperties(context.WrappedData)
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new RiskeerViewInfo<WaterLevelCalculationsForNormTargetProbabilityContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, DesignWaterLevelCalculationsView>(() => Gui)
            {
                GetViewName = (view, context) => GetWaterLevelCalculationsForNormTargetProbabilitiesViewName(context.WrappedData, context.AssessmentSection),
                GetViewData = context => context.WrappedData,
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

            yield return new RiskeerViewInfo<WaterLevelCalculationsForUserDefinedTargetProbabilityContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, DesignWaterLevelCalculationsView>(() => Gui)
            {
                GetViewName = (view, context) => GetWaterLevelCalculationsForUserDefinedTargetProbabilitiesViewName(context.WrappedData, context.AssessmentSection),
                GetViewData = context => context.WrappedData.HydraulicBoundaryLocationCalculations,
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

            yield return new RiskeerViewInfo<WaveHeightCalculationsForUserDefinedTargetProbabilityContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, WaveHeightCalculationsView>(() => Gui)
            {
                GetViewName = (view, context) => GetWaveHeightCalculationsForUserDefinedTargetProbabilitiesViewName(context.WrappedData, context.AssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities),
                GetViewData = context => context.WrappedData.HydraulicBoundaryLocationCalculations,
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

            yield return new RiskeerViewInfo<AssessmentSectionStateRootContext, AssessmentSectionReferenceLineView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssessmentSectionMap_DisplayName,
                CreateInstance = context => new AssessmentSectionReferenceLineView(context.WrappedData),
                CloseForData = CloseAssessmentSectionViewForData
            };

            yield return new RiskeerViewInfo<HydraulicLoadsStateRootContext, AssessmentSectionExtendedView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssessmentSectionMap_DisplayName,
                CreateInstance = context => new AssessmentSectionExtendedView(context.WrappedData),
                CloseForData = CloseAssessmentSectionViewForData
            };

            yield return new RiskeerViewInfo<CalculationsStateRootContext, AssessmentSectionExtendedView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssessmentSectionMap_DisplayName,
                CreateInstance = context => new AssessmentSectionExtendedView(context.WrappedData),
                CloseForData = CloseAssessmentSectionViewForData
            };

            yield return new RiskeerViewInfo<RegistrationStateRootContext, AssessmentSectionExtendedView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssessmentSectionMap_DisplayName,
                CreateInstance = context => new AssessmentSectionExtendedView(context.WrappedData),
                CloseForData = CloseAssessmentSectionViewForData
            };

            yield return CreateFailureMechanismViewInfo<MicrostabilityFailureMechanismContext, MicrostabilityFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                (context, sr) => FailureMechanismAssemblyFactory.AssembleSection(sr, context.WrappedData, context.Parent));

            yield return CreateFailureMechanismViewInfo<GrassCoverSlipOffOutwardsFailureMechanismContext, GrassCoverSlipOffOutwardsFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                (context, sr) => FailureMechanismAssemblyFactory.AssembleSection(sr, context.WrappedData, context.Parent));

            yield return CreateFailureMechanismViewInfo<GrassCoverSlipOffInwardsFailureMechanismContext, GrassCoverSlipOffInwardsFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                (context, sr) => FailureMechanismAssemblyFactory.AssembleSection(sr, context.WrappedData, context.Parent));

            yield return CreateFailureMechanismViewInfo<PipingStructureFailureMechanismContext, PipingStructureFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                (context, sr) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, context.Parent));

            yield return CreateFailureMechanismViewInfo<WaterPressureAsphaltCoverFailureMechanismContext, WaterPressureAsphaltCoverFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                (context, sr) => FailureMechanismAssemblyFactory.AssembleSection(sr, context.WrappedData, context.Parent));

            yield return CreateFailureMechanismResultViewInfo<GrassCoverSlipOffInwardsFailureMechanismSectionResultContext, GrassCoverSlipOffInwardsFailureMechanism>(fm => fm.GeneralInput.ApplyLengthEffectInSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism);

            yield return CreateFailureMechanismResultViewInfo<GrassCoverSlipOffOutwardsFailureMechanismSectionResultContext, GrassCoverSlipOffOutwardsFailureMechanism>(fm => fm.GeneralInput.ApplyLengthEffectInSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism);

            yield return CreateFailureMechanismResultViewInfo<MicrostabilityFailureMechanismSectionResultContext, MicrostabilityFailureMechanism>(fm => fm.GeneralInput.ApplyLengthEffectInSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism);

            yield return CreateFailureMechanismResultViewInfo<PipingStructureFailureMechanismSectionResultContext, PipingStructureFailureMechanism>(
                PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism);

            yield return CreateFailureMechanismResultViewInfo<WaterPressureAsphaltCoverFailureMechanismSectionResultContext, WaterPressureAsphaltCoverFailureMechanism>(fm => fm.GeneralInput.ApplyLengthEffectInSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism);

            yield return CreateFailureMechanismResultViewInfo<SpecificFailureMechanismSectionResultContext, SpecificFailureMechanism>(fp => fp.GeneralInput.ApplyLengthEffectInSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism);

            yield return new RiskeerViewInfo<SpecificFailureMechanismContext, SpecificFailureMechanismView>(() => Gui)
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                CreateInstance = context => new SpecificFailureMechanismView(context.WrappedData, context.Parent),
                AdditionalDataCheck = context => context.WrappedData.InAssembly,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailureMechanismView
            };

            yield return new RiskeerViewInfo<Comment, CommentView>(() => Gui)
            {
                GetViewName = (view, comment) => Resources.Comment_DisplayName,
                GetViewData = comment => comment,
                CloseForData = CloseCommentViewForData
            };

            yield return new RiskeerViewInfo<FailureMechanismSectionsContext, IEnumerable<FailureMechanismSection>, FailureMechanismSectionsView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailureMechanismView,
                CreateInstance = context => new FailureMechanismSectionsView(context.WrappedData.Sections, context.WrappedData),
                GetViewData = context => context.WrappedData.Sections
            };

            yield return new RiskeerViewInfo<StructuresOutputContext, IStructuresCalculation, GeneralResultFaultTreeIllustrationPointView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new GeneralResultFaultTreeIllustrationPointView(
                    context.WrappedData, () => context.WrappedData.Output?.GeneralResult)
            };

            yield return new RiskeerViewInfo<AssessmentSectionAssemblyGroupsContext, FailureMechanismContribution, AssessmentSectionAssemblyGroupsView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.AssessmentSectionAssemblyGroups_DisplayName,
                CloseForData = (view, dataToCloseFor) => dataToCloseFor is IAssessmentSection assessmentSection
                                                         && assessmentSection.FailureMechanismContribution == view.FailureMechanismContribution,
                CreateInstance = context => new AssessmentSectionAssemblyGroupsView(context.WrappedData.FailureMechanismContribution)
            };

            yield return new RiskeerViewInfo<AssemblyResultTotalContext, AssessmentSection, AssemblyResultTotalView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.AssemblyResultTotal_DisplayName,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
                CreateInstance = context => new AssemblyResultTotalView(context.WrappedData)
            };

            yield return new RiskeerViewInfo<AssemblyResultPerSectionContext, AssessmentSection, AssemblyResultPerSectionView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssemblyResultPerSection_DisplayName,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
                CreateInstance = context => new AssemblyResultPerSectionView(context.WrappedData)
            };

            yield return new RiskeerViewInfo<AssemblyResultPerSectionMapContext, AssessmentSection, AssemblyResultPerSectionMapView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerFormsResources.AssemblyResultPerSectionMapView_DisplayName,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
                CreateInstance = context => new AssemblyResultPerSectionMapView(context.WrappedData)
            };

            yield return new RiskeerViewInfo<FailureMechanismSectionAssemblyGroupsContext, AssessmentSection, FailureMechanismSectionAssemblyGroupsView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerFormsResources.FailureMechanismSectionAssemblyGroups_DisplayName,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
                CreateInstance = context => new FailureMechanismSectionAssemblyGroupsView(context.WrappedData)
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
                CreateFileImporter = (context, filePath) => new FailureMechanismSectionsImporter(
                    context.WrappedData, context.AssessmentSection.ReferenceLine, filePath,
                    new FailureMechanismSectionReplaceStrategy(context.WrappedData), new ImportMessageProvider())
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

            yield return new ImportInfo<HydraulicBoundaryDataContext>
            {
                Name = RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = RiskeerCommonFormsResources.DatabaseIcon,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                FileFilterGenerator = new FileFilterGenerator(Resources.HydraulicBoundaryDatabase_FilePath_Extension,
                                                              Resources.HydraulicBoundaryDatabase_file_filter_Description),
                CreateFileImporter = (context, filePath) => new HydraulicBoundaryDatabaseImporter(
                    context.WrappedData, new HydraulicBoundaryDataUpdateHandler(context.AssessmentSection,
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

            yield return new ExportInfo<HydraulicBoundaryDataContext>
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
                Name = context => Resources.AssemblyResults_DisplayName,
                Extension = Resources.AssemblyResults_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new AssemblyExporter(context.WrappedData, filePath),
                IsEnabled = context => HasGeometry(context.WrappedData.ReferenceLine),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(Resources.AssemblyResults_file_filter_Extension,
                                                                                                           Resources.AssemblyResults_DisplayName))
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
                            context.AssessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability,
                            context.AssessmentSection.FailureMechanismContribution.MaximumAllowableFloodingProbability),
                        new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                            context.AssessmentSection.WaterLevelCalculationsForSignalFloodingProbability,
                            context.AssessmentSection.FailureMechanismContribution.SignalFloodingProbability)
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
                GrassCoverSlipOffInwardsFailureMechanismSectionsContext, GrassCoverSlipOffInwardsFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                GrassCoverSlipOffOutwardsFailureMechanismSectionsContext, GrassCoverSlipOffOutwardsFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                MicrostabilityFailureMechanismSectionsContext, MicrostabilityFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                PipingStructureFailureMechanismSectionsContext, PipingStructureFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                new NonAdoptableFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                WaterPressureAsphaltCoverFailureMechanismSectionsContext, WaterPressureAsphaltCoverFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy());

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                SpecificFailureMechanismSectionsContext, SpecificFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy());
        }

        public override IEnumerable<object> GetChildDataWithViewDefinitions(object viewData)
        {
            if (viewData is RiskeerProject project)
            {
                yield return project.AssessmentSection;
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

            yield return CreateStateRootTreeNodeInfo<RegistrationStateRootContext>(
                RegistrationStateRootContextChildNodeObjects,
                RegistrationStateRootContextMenuStrip);

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

            yield return RiskeerTreeNodeInfoFactory.CreateRegistrationStateContextTreeNodeInfo<GrassCoverSlipOffInwardsFailureMechanismContext>(
                GrassCoverSlipOffInwardsFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateRegistrationStateContextTreeNodeInfo<GrassCoverSlipOffOutwardsFailureMechanismContext>(
                GrassCoverSlipOffOutwardsFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateRegistrationStateContextTreeNodeInfo<MicrostabilityFailureMechanismContext>(
                MicrostabilityFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateRegistrationStateContextTreeNodeInfo<PipingStructureFailureMechanismContext>(
                PipingStructureFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateRegistrationStateContextTreeNodeInfo<WaterPressureAsphaltCoverFailureMechanismContext>(
                WaterPressureAsphaltCoverFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return CreateSpecificFailureMechanismTreeNodeInfo();

            yield return new TreeNodeInfo<FailureMechanismSectionAssemblyGroupsContext>
            {
                Text = context => RiskeerFormsResources.FailureMechanismSectionAssemblyGroups_DisplayName,
                Image = context => RiskeerCommonFormsResources.NormsIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GenericFailureMechanismsContext>
            {
                Text = context => Resources.GenericFailureMechanisms_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ChildNodeObjects = GenericFailureMechanismsChildNodeObjects,
                ContextMenuStrip = GenericFailureMechanismsContextMenuStrip,
                ExpandOnCreate = context => true
            };

            yield return new TreeNodeInfo<SpecificFailureMechanismsContext>
            {
                Text = context => Resources.SpecificFailureMechanisms_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ChildNodeObjects = SpecificFailureMechanismsChildNodeObjects,
                ContextMenuStrip = SpecificFailureMechanismsContextMenuStrip,
                CanInsert = SpecificFailureMechanismsContext_CanDropOrInsert,
                CanDrop = SpecificFailureMechanismsContext_CanDropOrInsert,
                OnDrop = SpecificFailureMechanismsContext_OnDrop,
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

            yield return new TreeNodeInfo<HydraulicBoundaryDataContext>
            {
                Text = hydraulicBoundaryDatabase => RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = hydraulicBoundaryDatabase => RiskeerCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.IsLinked()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = HydraulicBoundaryDataContextChildNodeObjects,
                ContextMenuStrip = HydraulicBoundaryDataContextMenuStrip
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

            yield return CreateFailureMechanismSectionResultTreeNodeInfo<GrassCoverSlipOffOutwardsFailureMechanismSectionResultContext, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<GrassCoverSlipOffInwardsFailureMechanismSectionResultContext, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<MicrostabilityFailureMechanismSectionResultContext, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<PipingStructureFailureMechanismSectionResultContext, NonAdoptableFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<WaterPressureAsphaltCoverFailureMechanismSectionResultContext, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<SpecificFailureMechanismSectionResultContext, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>();

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

            yield return new TreeNodeInfo<AssessmentSectionAssemblyGroupsContext>
            {
                Text = context => RiskeerCommonFormsResources.AssessmentSectionAssemblyGroups_DisplayName,
                Image = context => RiskeerCommonFormsResources.NormsIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<AssemblyResultTotalContext>
            {
                Text = context => RiskeerCommonFormsResources.AssemblyResultTotal_DisplayName,
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

            yield return new TreeNodeInfo<AssemblyResultsContext>
            {
                Text = context => Resources.AssemblyResults_DisplayName,
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

        private ViewInfo<TContext, IObservableEnumerable<NonAdoptableFailureMechanismSectionResult>, NonAdoptableFailureMechanismResultView<TFailureMechanism>> CreateFailureMechanismResultViewInfo<TContext, TFailureMechanism>(
            Func<TFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> getFailureMechanismAssemblyResultFunc)
            where TContext : FailureMechanismSectionResultContext<NonAdoptableFailureMechanismSectionResult>
            where TFailureMechanism : class, IFailureMechanism<NonAdoptableFailureMechanismSectionResult>
        {
            return new RiskeerViewInfo<
                TContext,
                IObservableEnumerable<NonAdoptableFailureMechanismSectionResult>,
                NonAdoptableFailureMechanismResultView<TFailureMechanism>>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = CloseFailureMechanismResultViewForData<TFailureMechanism, NonAdoptableFailureMechanismSectionResult,
                    NonAdoptableFailureMechanismResultView<TFailureMechanism>, NonAdoptableFailureMechanismSectionResultRow>,
                CreateInstance = context => new NonAdoptableFailureMechanismResultView<TFailureMechanism>(
                    context.WrappedData, (TFailureMechanism) context.FailureMechanism, context.AssessmentSection, getFailureMechanismAssemblyResultFunc)
            };
        }

        private ViewInfo<TContext, IObservableEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>, NonAdoptableWithProfileProbabilityFailureMechanismResultView<TFailureMechanism>> CreateFailureMechanismResultViewInfo<TContext, TFailureMechanism>(
            Func<TFailureMechanism, bool> getUseLengthEffectFunc,
            Func<TFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
            where TContext : FailureMechanismSectionResultContext<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>
            where TFailureMechanism : class, IFailureMechanism<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>, IHasGeneralInput
        {
            return new RiskeerViewInfo<
                TContext,
                IObservableEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>,
                NonAdoptableWithProfileProbabilityFailureMechanismResultView<TFailureMechanism>>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = CloseFailureMechanismResultViewForData<TFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult,
                    NonAdoptableWithProfileProbabilityFailureMechanismResultView<TFailureMechanism>, NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow>,
                CreateInstance = context =>
                {
                    var failureMechanism = (TFailureMechanism) context.FailureMechanism;
                    IAssessmentSection assessmentSection = context.AssessmentSection;
                    return new NonAdoptableWithProfileProbabilityFailureMechanismResultView<TFailureMechanism>(
                        context.WrappedData, failureMechanism, assessmentSection, performFailureMechanismAssemblyFunc,
                        getUseLengthEffectFunc, sr => FailureMechanismAssemblyFactory.AssembleSection(sr, failureMechanism, assessmentSection));
                }
            };
        }

        private TreeNodeInfo<TContext> CreateFailureMechanismSectionResultTreeNodeInfo<TContext, TSectionResult>()
            where TContext : FailureMechanismSectionResultContext<TSectionResult>
            where TSectionResult : NonAdoptableFailureMechanismSectionResult
        {
            return new TreeNodeInfo<TContext>
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

            AssessmentSection assessmentSection = riskeerProject.AssessmentSection;
            if (assessmentSection.HydraulicBoundaryData.IsLinked())
            {
                string validationProblem = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(
                    assessmentSection.HydraulicBoundaryData.FilePath,
                    assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationSettings.FilePath,
                    assessmentSection.HydraulicBoundaryData.EffectivePreprocessorDirectory(),
                    assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationSettings.UsePreprocessorClosure);

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
            else if (e.View is SpecificFailureMechanismView specificFailureMechanismView)
            {
                observers = CreateObservers(specificFailureMechanismView);
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
            if (ReferenceEquals(viewData, assessmentSection.WaterLevelCalculationsForSignalFloodingProbability)
                || ReferenceEquals(viewData, assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability))
            {
                IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculationsForUserDefinedTargetProbability =
                    ReferenceEquals(viewData, assessmentSection.WaterLevelCalculationsForSignalFloodingProbability)
                        ? assessmentSection.WaterLevelCalculationsForSignalFloodingProbability
                        : assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability;

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

        private IEnumerable<IObserver> CreateObservers(SpecificFailureMechanismView specificFailureMechanismView)
        {
            IFailureMechanism failureMechanism = specificFailureMechanismView.FailureMechanism;
            return new[]
            {
                CreateViewTitleObserver(specificFailureMechanismView, failureMechanism, () => failureMechanism.Name)
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

        #region CreateFailureMechanismViewInfo ViewInfo

        private ViewInfo<TFailureMechanismContext, TFailureMechanism, StandAloneFailureMechanismView<TFailureMechanism, TSectionResult>> CreateFailureMechanismViewInfo<
            TFailureMechanismContext, TFailureMechanism, TSectionResult>(Func<TFailureMechanismContext, TSectionResult, FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc)
            where TSectionResult : FailureMechanismSectionResult
            where TFailureMechanism : FailureMechanismBase<TSectionResult>
            where TFailureMechanismContext : IFailureMechanismContext<TFailureMechanism>
        {
            return new RiskeerViewInfo<TFailureMechanismContext, TFailureMechanism,
                StandAloneFailureMechanismView<TFailureMechanism, TSectionResult>>(() => Gui)
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                CloseForData = (view, dataToCloseFor) => ReferenceEquals(view.AssessmentSection, dataToCloseFor),
                AdditionalDataCheck = context => context.WrappedData.InAssembly,
                CreateInstance = context => new StandAloneFailureMechanismView<TFailureMechanism, TSectionResult>(
                    context.WrappedData, context.Parent, sr => performAssemblyFunc(context, sr).AssemblyResult)
            };
        }

        #endregion

        #region FailureMechanismResults ViewInfo

        private static bool CloseFailureMechanismResultViewForData<TFailureMechanism, TSectionResult, TView, TSectionResultRow>(
            TView view, object dataToCloseFor)
            where TFailureMechanism : class, IFailureMechanism<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
            where TSectionResultRow : FailureMechanismSectionResultRow<TSectionResult>
            where TView : FailureMechanismResultView<TSectionResult, TSectionResultRow, TFailureMechanism>
        {
            TFailureMechanism failureMechanism = null;
            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<TFailureMechanism>()
                                                    .FirstOrDefault()
                                   ?? assessmentSection.SpecificFailureMechanisms
                                                       .Cast<TFailureMechanism>()
                                                       .FirstOrDefault(fp => fp == view.FailureMechanism);
            }

            if (dataToCloseFor is IFailureMechanismContext<IFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData as TFailureMechanism;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
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
            if (dataToCloseFor is ICalculationContext<CalculationGroup, ICalculatableFailureMechanism> calculationGroupContext)
            {
                return GetCommentElements(calculationGroupContext.WrappedData)
                    .Any(commentElement => ReferenceEquals(commentView.Data, commentElement));
            }

            var calculationContext = dataToCloseFor as ICalculationContext<ICalculationBase, ICalculatableFailureMechanism>;
            if (calculationContext?.WrappedData is ICalculation calculation)
            {
                return ReferenceEquals(commentView.Data, calculation.Comments);
            }

            if (dataToCloseFor is IFailureMechanismContext<IFailureMechanism> failureMechanismContext)
            {
                return GetCommentElements(failureMechanismContext.WrappedData)
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

            foreach (Comment comment in assessmentSection.SpecificFailureMechanisms.SelectMany(GetCommentElements))
            {
                yield return comment;
            }
        }

        private static IEnumerable<Comment> GetCommentElements(IFailureMechanism failureMechanism)
        {
            yield return failureMechanism.InAssemblyInputComments;
            yield return failureMechanism.InAssemblyOutputComments;
            yield return failureMechanism.NotInAssemblyComments;

            if (failureMechanism is ICalculatableFailureMechanism calculatableFailureMechanism)
            {
                foreach (ICalculation calculation in calculatableFailureMechanism.Calculations)
                {
                    yield return calculation.Comments;
                }

                yield return calculatableFailureMechanism.CalculationsInputComments;
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
                new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData, assessmentSection),
                new StabilityStoneCoverHydraulicLoadsStateFailureMechanismContext(assessmentSection.StabilityStoneCover, assessmentSection),
                new WaveImpactAsphaltCoverHydraulicLoadsStateFailureMechanismContext(assessmentSection.WaveImpactAsphaltCover, assessmentSection),
                new GrassCoverErosionOutwardsHydraulicLoadsStateFailureMechanismContext(assessmentSection.GrassCoverErosionOutwards, assessmentSection),
                new DuneErosionHydraulicLoadsStateFailureMechanismContext(assessmentSection.DuneErosion, assessmentSection)
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
                new PipingCalculationsStateFailureMechanismContext(assessmentSection.Piping, assessmentSection),
                new GrassCoverErosionInwardsCalculationsStateFailureMechanismContext(assessmentSection.GrassCoverErosionInwards, assessmentSection),
                new MacroStabilityInwardsCalculationsStateFailureMechanismContext(assessmentSection.MacroStabilityInwards, assessmentSection),
                new HeightStructuresCalculationsStateFailureMechanismContext(assessmentSection.HeightStructures, assessmentSection),
                new ClosingStructuresCalculationsStateFailureMechanismContext(assessmentSection.ClosingStructures, assessmentSection),
                new StabilityPointStructuresCalculationsStateFailureMechanismContext(assessmentSection.StabilityPointStructures, assessmentSection)
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

            SetCalculateAllMenuItemEnabledStateAndTooltip(nodeData.WrappedData, calculateAllItem);

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

        private static void SetCalculateAllMenuItemEnabledStateAndTooltip(AssessmentSection assessmentSection, StrictContextMenuItem menuItem)
        {
            bool hasCalculations = assessmentSection.Piping.Calculations.Any() ||
                                   assessmentSection.GrassCoverErosionInwards.Calculations.Any() ||
                                   assessmentSection.MacroStabilityInwards.Calculations.Any() ||
                                   assessmentSection.HeightStructures.Calculations.Any() ||
                                   assessmentSection.ClosingStructures.Calculations.Any() ||
                                   assessmentSection.StabilityPointStructures.Calculations.Any();

            if (!hasCalculations)
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = RiskeerCommonFormsResources.Calculate_All_No_calculations_to_run;
            }
        }

        #endregion

        #region RegistrationStateRootContext TreeNodeInfo

        private static object[] RegistrationStateRootContextChildNodeObjects(RegistrationStateRootContext nodeData)
        {
            AssessmentSection assessmentSection = nodeData.WrappedData;

            return new object[]
            {
                new FailureMechanismSectionAssemblyGroupsContext(assessmentSection),
                new GenericFailureMechanismsContext(assessmentSection),
                new SpecificFailureMechanismsContext(assessmentSection.SpecificFailureMechanisms, assessmentSection),
                new AssemblyResultsContext(assessmentSection)
            };
        }

        private ContextMenuStrip RegistrationStateRootContextMenuStrip(RegistrationStateRootContext nodeData,
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

        #region GenericFailureMechanisms TreeNodeInfo

        private static object[] GenericFailureMechanismsChildNodeObjects(GenericFailureMechanismsContext nodeData)
        {
            AssessmentSection assessmentSection = nodeData.WrappedData;
            return new object[]
            {
                new PipingRegistrationStateFailureMechanismContext(assessmentSection.Piping, assessmentSection),
                new GrassCoverErosionInwardsRegistrationStateFailureMechanismContext(assessmentSection.GrassCoverErosionInwards, assessmentSection),
                new MacroStabilityInwardsRegistrationStateFailureMechanismContext(assessmentSection.MacroStabilityInwards, assessmentSection),
                new MicrostabilityFailureMechanismContext(assessmentSection.Microstability, assessmentSection),
                new StabilityStoneCoverRegistrationStateFailureMechanismContext(assessmentSection.StabilityStoneCover, assessmentSection),
                new WaveImpactAsphaltCoverRegistrationStateFailureMechanismContext(assessmentSection.WaveImpactAsphaltCover, assessmentSection),
                new WaterPressureAsphaltCoverFailureMechanismContext(assessmentSection.WaterPressureAsphaltCover, assessmentSection),
                new GrassCoverErosionOutwardsRegistrationStateFailureMechanismContext(assessmentSection.GrassCoverErosionOutwards, assessmentSection),
                new GrassCoverSlipOffOutwardsFailureMechanismContext(assessmentSection.GrassCoverSlipOffOutwards, assessmentSection),
                new GrassCoverSlipOffInwardsFailureMechanismContext(assessmentSection.GrassCoverSlipOffInwards, assessmentSection),
                new HeightStructuresRegistrationStateFailureMechanismContext(assessmentSection.HeightStructures, assessmentSection),
                new ClosingStructuresRegistrationStateFailureMechanismContext(assessmentSection.ClosingStructures, assessmentSection),
                new PipingStructureFailureMechanismContext(assessmentSection.PipingStructure, assessmentSection),
                new StabilityPointStructuresRegistrationStateFailureMechanismContext(assessmentSection.StabilityPointStructures, assessmentSection),
                new DuneErosionRegistrationStateFailureMechanismContext(assessmentSection.DuneErosion, assessmentSection)
            };
        }

        private ContextMenuStrip GenericFailureMechanismsContextMenuStrip(GenericFailureMechanismsContext nodeData,
                                                                          object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .Build();
        }

        #endregion

        #region SpecificFailureMechanisms TreeNodeInfo

        private static object[] SpecificFailureMechanismsChildNodeObjects(SpecificFailureMechanismsContext nodeData)
        {
            return nodeData.WrappedData
                           .Select(sfp => new SpecificFailureMechanismContext(sfp, nodeData.AssessmentSection))
                           .Cast<object>()
                           .ToArray();
        }

        private ContextMenuStrip SpecificFailureMechanismsContextMenuStrip(SpecificFailureMechanismsContext nodeData,
                                                                           object parentData,
                                                                           TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddCustomItem(CreateAddSpecificFailureMechanismItem(nodeData))
                          .AddSeparator()
                          .AddRemoveAllChildrenItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static StrictContextMenuItem CreateAddSpecificFailureMechanismItem(SpecificFailureMechanismsContext nodeData)
        {
            return new StrictContextMenuItem(Resources.RiskeerPlugin_ContextMenuStrip_Add_SpecificFailureMechanism,
                                             Resources.RiskeerPlugin_ContextMenuStrip_Add_SpecificFailureMechanism_Tooltip,
                                             RiskeerCommonFormsResources.FailureMechanismIcon,
                                             (sender, args) => AddSpecificFailureMechanism(nodeData));
        }

        private static void AddSpecificFailureMechanism(SpecificFailureMechanismsContext nodeData)
        {
            ObservableList<SpecificFailureMechanism> failureMechanisms = nodeData.WrappedData;
            var newFailureMechanism = new SpecificFailureMechanism
            {
                Name = NamingHelper.GetUniqueName(failureMechanisms,
                                                  RiskeerCommonDataResources.SpecificFailureMechanism_DefaultName,
                                                  fp => fp.Name)
            };
            failureMechanisms.Add(newFailureMechanism);
            failureMechanisms.NotifyObservers();
        }

        private static bool SpecificFailureMechanismsContext_CanDropOrInsert(object draggedData, object targetData)
        {
            var failureMechanismsContext = (SpecificFailureMechanismsContext) targetData;

            return draggedData is SpecificFailureMechanismContext failureMechanismContext
                   && failureMechanismsContext.WrappedData.Contains(failureMechanismContext.WrappedData);
        }

        private static void SpecificFailureMechanismsContext_OnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
        {
            var failureMechanismsContext = (SpecificFailureMechanismsContext) newParentData;
            var failureMechanismContext = (SpecificFailureMechanismContext) droppedData;

            failureMechanismsContext.WrappedData.Remove(failureMechanismContext.WrappedData);
            failureMechanismsContext.WrappedData.Insert(position, failureMechanismContext.WrappedData);

            failureMechanismsContext.WrappedData.NotifyObservers();
        }

        #endregion

        #region SpecificFailureMechanism TreeNodeInfo

        private TreeNodeInfo CreateSpecificFailureMechanismTreeNodeInfo()
        {
            TreeNodeInfo<SpecificFailureMechanismContext> treeNodeInfo =
                RiskeerTreeNodeInfoFactory.CreateRegistrationStateContextTreeNodeInfo<SpecificFailureMechanismContext>(
                    SpecificFailureMechanismEnabledChildNodeObjects,
                    SpecificFailureMechanismDisabledChildNodeObjects,
                    SpecificFailureMechanismEnabledContextMenuStrip,
                    SpecificFailureMechanismDisabledContextMenuStrip);
            treeNodeInfo.CanRename = (context, o) => true;
            treeNodeInfo.OnNodeRenamed = SpecificFailureMechanismContextOnNodeRenamed;
            treeNodeInfo.CanRemove = (context, o) => true;
            treeNodeInfo.OnNodeRemoved = SpecificFailureMechanismContextOnNodeRemoved;
            treeNodeInfo.CanDrag = (context, o) => true;

            return treeNodeInfo;
        }

        private static void SpecificFailureMechanismContextOnNodeRenamed(SpecificFailureMechanismContext nodeData, string newName)
        {
            nodeData.WrappedData.Name = newName;
            nodeData.WrappedData.NotifyObservers();
        }

        private static void SpecificFailureMechanismContextOnNodeRemoved(SpecificFailureMechanismContext nodeData, object parentNodeData)
        {
            var specificFailureMechanismsContext = (SpecificFailureMechanismsContext) parentNodeData;
            ObservableList<SpecificFailureMechanism> failureMechanisms = specificFailureMechanismsContext.WrappedData;

            failureMechanisms.Remove(nodeData.WrappedData);
            failureMechanisms.NotifyObservers();
        }

        private static object[] SpecificFailureMechanismDisabledChildNodeObjects(SpecificFailureMechanismContext nodeData)
        {
            return new object[]
            {
                nodeData.WrappedData.NotInAssemblyComments
            };
        }

        private static object[] SpecificFailureMechanismEnabledChildNodeObjects(SpecificFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetSpecificFailureMechanismInputs(nodeData),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetSpecificFailureMechanismOutputs(nodeData),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetSpecificFailureMechanismInputs(SpecificFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new SpecificFailureMechanismSectionsContext(nodeData.WrappedData, nodeData.Parent),
                nodeData.WrappedData.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetSpecificFailureMechanismOutputs(SpecificFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new SpecificFailureMechanismSectionResultContext(nodeData.WrappedData.SectionResults,
                                                                 nodeData.WrappedData, nodeData.Parent),
                nodeData.WrappedData.InAssemblyOutputComments
            };
        }

        private ContextMenuStrip SpecificFailureMechanismEnabledContextMenuStrip(SpecificFailureMechanismContext nodeData,
                                                                                 object parentData,
                                                                                 TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleInAssemblyOfFailureMechanismItem(nodeData, RemoveAllViewsForSpecificFailureMechanismContext)
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

        private ContextMenuStrip SpecificFailureMechanismDisabledContextMenuStrip(SpecificFailureMechanismContext nodeData,
                                                                                  object parentData,
                                                                                  TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddToggleInAssemblyOfFailureMechanismItem(nodeData, RemoveAllViewsForSpecificFailureMechanismContext)
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

        private void RemoveAllViewsForSpecificFailureMechanismContext(SpecificFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        #endregion

        #region StandAloneFailureMechanism TreeNodeInfo

        private static object[] StandAloneFailureMechanismDisabledChildNodeObjects(IFailureMechanismContext<IFailureMechanism> nodeData)
        {
            return new object[]
            {
                nodeData.WrappedData.NotInAssemblyComments
            };
        }

        private ContextMenuStrip StandAloneFailureMechanismEnabledContextMenuStrip(IFailureMechanismContext<IFailureMechanism> nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleInAssemblyOfFailureMechanismItem(nodeData, RemoveAllViewsForFailureMechanismContext)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForFailureMechanismContext(IFailureMechanismContext<IFailureMechanism> failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip StandAloneFailureMechanismDisabledContextMenuStrip(IFailureMechanismContext<IFailureMechanism> nodeData,
                                                                                    object parentData,
                                                                                    TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddToggleInAssemblyOfFailureMechanismItem(nodeData, RemoveAllViewsForFailureMechanismContext)
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
                                       GetGrassCoverSlipOffInwardsFailureMechanismOutputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffInwardsFailureMechanismInputs(GrassCoverSlipOffInwardsFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverSlipOffInwardsFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffInwardsFailureMechanismOutputs(GrassCoverSlipOffInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverSlipOffInwardsFailureMechanismSectionResultContext(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.InAssemblyOutputComments
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
                                       GetGrassCoverSlipOffOutwardsFailureMechanismOutputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffOutwardsFailureMechanismInputs(GrassCoverSlipOffOutwardsFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverSlipOffOutwardsFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetGrassCoverSlipOffOutwardsFailureMechanismOutputs(GrassCoverSlipOffOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverSlipOffOutwardsFailureMechanismSectionResultContext(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.InAssemblyOutputComments
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
                                       GetMicrostabilityFailureMechanismOutputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetMicrostabilityFailureMechanismInputs(MicrostabilityFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MicrostabilityFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetMicrostabilityFailureMechanismOutputs(MicrostabilityFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MicrostabilityFailureMechanismSectionResultContext(failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.InAssemblyOutputComments
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
                nodeData.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetPipingStructureFailureMechanismOutputs(PipingStructureFailureMechanism failureMechanism,
                                                                                     IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new PipingStructureFailureMechanismSectionResultContext(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.InAssemblyOutputComments
            };
        }

        #endregion

        #region WaterPressureAsphaltCoverFailureMechanismContext TreeNodeInfo

        private static object[] WaterPressureAsphaltCoverFailureMechanismEnabledChildNodeObjects(WaterPressureAsphaltCoverFailureMechanismContext nodeData)
        {
            IAssessmentSection assessmentSection = nodeData.Parent;
            WaterPressureAsphaltCoverFailureMechanism failureMechanism = nodeData.WrappedData;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetWaterPressureAsphaltCoverFailureMechanismInputs(failureMechanism, assessmentSection),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetWaterPressureAsphaltCoverFailureMechanismOutputs(failureMechanism, assessmentSection),
                                       TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetWaterPressureAsphaltCoverFailureMechanismInputs(WaterPressureAsphaltCoverFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new WaterPressureAsphaltCoverFailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetWaterPressureAsphaltCoverFailureMechanismOutputs(WaterPressureAsphaltCoverFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new WaterPressureAsphaltCoverFailureMechanismSectionResultContext(nodeData.SectionResults, nodeData, assessmentSection),
                nodeData.InAssemblyOutputComments
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

        private static object[] HydraulicBoundaryDataContextChildNodeObjects(HydraulicBoundaryDataContext nodeData)
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
            string validationText = HydraulicBoundaryDataConnectionValidator.Validate(assessmentSection.HydraulicBoundaryData);
            if (!string.IsNullOrEmpty(validationText))
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = validationText;
            }
        }

        private ContextMenuStrip HydraulicBoundaryDataContextMenuStrip(HydraulicBoundaryDataContext nodeData, object parentData, TreeViewControl treeViewControl)
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
                new WaterLevelCalculationsForNormTargetProbabilityContext(context.AssessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability,
                                                                          context.AssessmentSection,
                                                                          () => context.AssessmentSection.FailureMechanismContribution.MaximumAllowableFloodingProbability),
                new WaterLevelCalculationsForNormTargetProbabilityContext(context.AssessmentSection.WaterLevelCalculationsForSignalFloodingProbability,
                                                                          context.AssessmentSection,
                                                                          () => context.AssessmentSection.FailureMechanismContribution.SignalFloodingProbability)
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
            return IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForSignalFloodingProbability)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability);
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
                assessmentSection.HydraulicBoundaryData.Locations.Select(hbl => new HydraulicBoundaryLocationCalculation(hbl)));

            return calculationsForTargetProbability;
        }

        #endregion

        #region AssemblyResults TreeNodeInfo

        private static object[] AssemblyResultsContextChildNodeObjects(AssemblyResultsContext context)
        {
            AssessmentSection assessmentSection = context.WrappedData;
            return new object[]
            {
                new AssessmentSectionAssemblyGroupsContext(assessmentSection),
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