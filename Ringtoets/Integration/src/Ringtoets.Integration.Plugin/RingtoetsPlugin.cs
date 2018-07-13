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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Core.Components.Gis.Data;
using log4net;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.Common.Plugin;
using Ringtoets.Common.Service;
using Ringtoets.Common.Util.TypeConverters;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Plugin.Handlers;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.Input;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Commands;
using Ringtoets.Integration.Forms.Dialogs;
using Ringtoets.Integration.Forms.Merge;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PresentationObjects.StandAlone;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.PropertyClasses.StandAlone;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Forms.Views.SectionResultRows;
using Ringtoets.Integration.Forms.Views.SectionResultViews;
using Ringtoets.Integration.IO.Exporters;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.Integration.Service;
using Ringtoets.Integration.Service.Comparers;
using Ringtoets.Integration.Service.Merge;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.Storage.Core;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RingtoetsDataResources = Ringtoets.Integration.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Plugin
{
    /// <summary>
    /// The plug-in for the Ringtoets application.
    /// </summary>
    public class RingtoetsPlugin : PluginBase
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
                (mechanism, assessmentSection) => new FailureMechanismContext<IFailureMechanism>(
                    mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(GrassCoverSlipOffOutwardsFailureMechanism),
                (mechanism, assessmentSection) => new FailureMechanismContext<IFailureMechanism>(
                    mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(MicrostabilityFailureMechanism),
                (mechanism, assessmentSection) => new FailureMechanismContext<IFailureMechanism>(
                    mechanism,
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
                (mechanism, assessmentSection) => new FailureMechanismContext<IFailureMechanism>(
                    mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(StrengthStabilityLengthwiseConstructionFailureMechanism),
                (mechanism, assessmentSection) => new FailureMechanismContext<IFailureMechanism>(
                    mechanism,
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
                (mechanism, assessmentSection) => new FailureMechanismContext<IFailureMechanism>(
                    mechanism,
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

        private RingtoetsRibbon ribbonCommandHandler;

        private IAssessmentSectionFromFileCommandHandler assessmentSectionFromFileCommandHandler;
        private IHydraulicBoundaryLocationCalculationGuiService hydraulicBoundaryLocationCalculationGuiService;
        private AssessmentSectionMerger assessmentSectionMerger;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return ribbonCommandHandler;
            }
        }

        public override IGui Gui
        {
            get
            {
                return base.Gui;
            }
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
            assessmentSectionMerger = new AssessmentSectionMerger(new AssessmentSectionMergeFilePathProvider(new DialogBasedInquiryHelper(Gui.MainWindow)),
                                                                  (filePath, assessmentSectionOwner) =>
                                                                  {
                                                                      ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                                                                                       LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(
                                                                                                           assessmentSectionOwner, new LoadAssessmentSectionService(new StorageSqLite()),
                                                                                                           filePath));
                                                                  },
                                                                  new AssessmentSectionMergeComparer(),
                                                                  new AssessmentSectionMergeDataProviderDialog(Gui.MainWindow),
                                                                  new AssessmentSectionMergeHandlerStub());

            ribbonCommandHandler = new RingtoetsRibbon
            {
                AddAssessmentSectionButtonCommand = new AddAssessmentSectionCommand(assessmentSectionFromFileCommandHandler)
            };
        }

        /// <summary>
        /// Returns all <see cref="PropertyInfo"/> instances provided for data of <see cref="RingtoetsPlugin"/>.
        /// </summary>
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<IProject, RingtoetsProjectProperties>();
            yield return new PropertyInfo<IAssessmentSection, AssessmentSectionProperties>();
            yield return new PropertyInfo<BackgroundData, BackgroundDataProperties>
            {
                CreateInstance = data => new BackgroundDataProperties(data)
            };
            yield return new PropertyInfo<HydraulicBoundaryDatabaseContext, HydraulicBoundaryDatabaseProperties>
            {
                CreateInstance = context => new HydraulicBoundaryDatabaseProperties(context.WrappedData)
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
            yield return new PropertyInfo<FailureMechanismContext<IFailureMechanism>, StandAloneFailureMechanismProperties>
            {
                CreateInstance = context => new StandAloneFailureMechanismProperties(context.WrappedData, context.Parent)
            };
            yield return new PropertyInfo<MacroStabilityOutwardsFailureMechanismContext, MacroStabilityOutwardsFailureMechanismProperties>
            {
                CreateInstance = context => new MacroStabilityOutwardsFailureMechanismProperties(context.WrappedData)
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
                CreateInstance = context => new FailureMechanismSectionsProperties(context.WrappedData.Sections,
                                                                                   context.WrappedData)
            };
            yield return new PropertyInfo<ReferenceLineContext, ReferenceLineProperties>
            {
                CreateInstance = context => new ReferenceLineProperties(context.WrappedData)
            };
            yield return new PropertyInfo<FailureMechanismAssemblyCategoriesContextBase, FailureMechanismAssemblyCategoriesProperties>
            {
                CreateInstance = context => new FailureMechanismAssemblyCategoriesProperties(context.GetFailureMechanismCategoriesFunc(),
                                                                                             context.GetFailureMechanismSectionAssemblyCategoriesFunc())
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
        }

        /// <summary>
        /// Returns all <see cref="ViewInfo"/> instances provided for data of <see cref="RingtoetsPlugin"/>.
        /// </summary>
        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<FailureMechanismContributionContext, FailureMechanismContribution, FailureMechanismContributionView>
            {
                GetViewName = (view, context) => RingtoetsDataResources.FailureMechanismContribution_DisplayName,
                GetViewData = context => context.WrappedData,
                Image = RingtoetsCommonFormsResources.FailureMechanismContributionIcon,
                CloseForData = CloseFailureMechanismContributionViewForData,
                CreateInstance = context => new FailureMechanismContributionView(Gui.ViewCommands),
                AfterCreate = (view, context) => view.AssessmentSection = context.Parent
            };

            yield return new ViewInfo<NormContext, FailureMechanismContribution, AssessmentSectionAssemblyCategoriesView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.Norms_DisplayName,
                Image = RingtoetsCommonFormsResources.NormsIcon,
                CloseForData = CloseAssessmentSectionCategoriesViewForData,
                CreateInstance = context => new AssessmentSectionAssemblyCategoriesView(context.AssessmentSection.FailureMechanismContribution)
            };

            yield return new ViewInfo<DesignWaterLevelCalculationsContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, DesignWaterLevelCalculationsView>
            {
                GetViewName = (view, context) => $"{RingtoetsFormsResources.DesignWaterLevelCalculationsContext_DisplayName} - " +
                                                 $"{RingtoetsPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName)}",
                GetViewData = context => context.WrappedData,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseHydraulicBoundaryCalculationsViewForData,
                CreateInstance = context => new DesignWaterLevelCalculationsView(context.WrappedData,
                                                                                 context.AssessmentSection,
                                                                                 context.GetNormFunc,
                                                                                 context.CategoryBoundaryName),
                AfterCreate = (view, context) => { view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService; }
            };

            yield return new ViewInfo<WaveHeightCalculationsContext, IObservableEnumerable<HydraulicBoundaryLocationCalculation>, WaveHeightCalculationsView>
            {
                GetViewName = (view, context) => $"{RingtoetsFormsResources.WaveHeightCalculationsContext_DisplayName} - " +
                                                 $"{RingtoetsPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName)}",
                GetViewData = context => context.WrappedData,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseHydraulicBoundaryCalculationsViewForData,
                CreateInstance = context => new WaveHeightCalculationsView(context.WrappedData,
                                                                           context.AssessmentSection,
                                                                           context.GetNormFunc,
                                                                           context.CategoryBoundaryName),
                AfterCreate = (view, context) => { view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService; }
            };

            yield return new ViewInfo<IAssessmentSection, AssessmentSectionView>
            {
                GetViewName = (view, section) => RingtoetsFormsResources.AssessmentSectionMap_DisplayName,
                Image = RingtoetsFormsResources.Map,
                CreateInstance = section => new AssessmentSectionView(section)
            };

            yield return new ViewInfo<IFailureMechanismContext<IFailureMechanism>, FailureMechanismView<IFailureMechanism>>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new FailureMechanismView<IFailureMechanism>(context.WrappedData, context.Parent)
            };

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
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
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
                Image = RingtoetsCommonFormsResources.EditDocumentIcon,
                CloseForData = CloseCommentViewForData
            };

            yield return new ViewInfo<FailureMechanismSectionsContext, IEnumerable<FailureMechanismSection>, FailureMechanismSectionsView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanismSections_DisplayName,
                Image = RingtoetsCommonFormsResources.SectionsIcon,
                CloseForData = RingtoetsPluginHelper.ShouldCloseFailureMechanismSectionsView,
                CreateInstance = context => new FailureMechanismSectionsView(context.WrappedData.Sections, context.WrappedData),
                GetViewData = context => context.WrappedData.Sections
            };

            yield return new ViewInfo<StructuresOutputContext, IStructuresCalculation, GeneralResultFaultTreeIllustrationPointView>
            {
                Image = RingtoetsCommonFormsResources.GeneralOutputIcon,
                GetViewName = (view, context) => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = RingtoetsPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new GeneralResultFaultTreeIllustrationPointView(() => context.WrappedData.Output?.GeneralResult)
            };

            yield return new ViewInfo<AssemblyResultTotalContext, AssessmentSection, AssemblyResultTotalView>
            {
                GetViewName = (view, context) => Resources.AssemblyResultTotal_DisplayName,
                Image = Resources.AssemblyResultTotal,
                CloseForData = CloseAssemblyResultTotalViewForData,
                CreateInstance = context => new AssemblyResultTotalView(context.WrappedData)
            };

            yield return new ViewInfo<AssemblyResultPerSectionContext, AssessmentSection, AssemblyResultPerSectionView>
            {
                GetViewName = (view, context) => Resources.AssemblyResultPerSection_DisplayName,
                Image = Resources.AssemblyResultPerSection,
                CloseForData = CloseAssemblyResultPerSectionViewForData,
                CreateInstance = context => new AssemblyResultPerSectionView(context.WrappedData)
            };

            yield return new ViewInfo<FailureMechanismAssemblyCategoriesContextBase, IFailureMechanism, FailureMechanismAssemblyCategoriesView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanismAssemblyCategories_DisplayName,
                Image = RingtoetsCommonFormsResources.NormsIcon,
                CloseForData = CloseFailureMechanismAssemblyCategoriesViewForData,
                CreateInstance = context => new FailureMechanismAssemblyCategoriesView(context.WrappedData,
                                                                                       context.AssessmentSection,
                                                                                       context.GetFailureMechanismCategoriesFunc,
                                                                                       context.GetFailureMechanismSectionAssemblyCategoriesFunc)
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<ReferenceLineContext>
            {
                Name = RingtoetsCommonDataResources.ReferenceLine_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.ReferenceLineIcon,
                FileFilterGenerator = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                                              RingtoetsCommonIOResources.Shape_file_filter_Description),
                CreateFileImporter = (context, filePath) => new ReferenceLineImporter(context.WrappedData,
                                                                                      new ReferenceLineReplacementHandler(Gui.ViewCommands),
                                                                                      filePath)
            };

            yield return new ImportInfo<FailureMechanismSectionsContext>
            {
                Name = RingtoetsCommonFormsResources.FailureMechanismSections_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.SectionsIcon,
                FileFilterGenerator = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                                              RingtoetsCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null,
                CreateFileImporter = (context, filePath) => new FailureMechanismSectionsImporter(context.WrappedData,
                                                                                                 context.AssessmentSection.ReferenceLine,
                                                                                                 filePath)
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
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = Resources.Foreshore,
                FileFilterGenerator = CreateForeshoreProfileFileFilterGenerator,
                IsEnabled = context => context.ParentAssessmentSection.ReferenceLine != null,
                VerifyUpdates = context => VerifyForeshoreProfileUpdates(context, Resources.RingtoetsPlugin_VerifyForeshoreProfileUpdates_When_importing_ForeshoreProfile_definitions_assigned_to_calculations_output_will_be_cleared_confirm)
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<ReferenceLineContext>
            {
                Name = RingtoetsCommonDataResources.ReferenceLine_DisplayName,
                CreateFileExporter = (context, filePath) => new ReferenceLineExporter(context.WrappedData.ReferenceLine, context.WrappedData.Id, filePath),
                IsEnabled = context => context.WrappedData.ReferenceLine != null,
                FileFilterGenerator = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                                              RingtoetsCommonIOResources.Shape_file_filter_Description)
            };

            yield return new ExportInfo<HydraulicBoundaryDatabaseContext>
            {
                Name = RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                CreateFileExporter = (context, filePath) => new HydraulicBoundaryLocationsExporter(context.AssessmentSection, filePath),
                IsEnabled = context => context.WrappedData.IsLinked(),
                FileFilterGenerator = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                                              RingtoetsCommonIOResources.Shape_file_filter_Description)
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
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = Resources.Foreshore,
                FileFilterGenerator = CreateForeshoreProfileFileFilterGenerator,
                CurrentPath = context => context.WrappedData.SourcePath,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                VerifyUpdates = context => VerifyForeshoreProfileUpdates(context, Resources.RingtoetsPlugin_VerifyForeshoreProfileUpdates_When_updating_ForeshoreProfile_definitions_assigned_to_calculations_output_will_be_cleared_confirm)
            };
        }

        /// <summary>
        /// Gets the child data instances that have <see cref="ViewInfo"/> definitions of some parent data object.
        /// </summary>
        /// <param name="viewData">The parent data object.</param>
        /// <returns>Sequence of child data.</returns>
        public override IEnumerable<object> GetChildDataWithViewDefinitions(object viewData)
        {
            var project = viewData as RingtoetsProject;
            if (project != null)
            {
                foreach (AssessmentSection item in project.AssessmentSections)
                {
                    yield return item;
                }
            }

            var assessmentSection = viewData as IAssessmentSection;
            if (assessmentSection != null)
            {
                yield return assessmentSection.FailureMechanismContribution;
            }
        }

        /// <summary>
        /// Returns all <see cref="TreeNodeInfo"/> instances provided for data of <see cref="RingtoetsPlugin"/>.
        /// </summary>
        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<AssessmentSection>
            {
                Text = assessmentSection => assessmentSection.Name,
                Image = assessmentSection => RingtoetsFormsResources.AssessmentSectionFolderIcon,
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
                Text = data => Resources.RingtoetsPlugin_BackgroundDataContext_Text,
                Image = data => RingtoetsFormsResources.Map,
                ContextMenuStrip = BackgroundDataMenuStrip,
                ForeColor = data =>
                {
                    var configuration = data.Configuration as WmtsBackgroundDataConfiguration;
                    return configuration != null
                           && configuration.IsConfigured
                               ? Color.FromKnownColor(KnownColor.ControlText)
                               : Color.FromKnownColor(configuration == null
                                                          ? KnownColor.ControlText
                                                          : KnownColor.GrayText);
                }
            };

            yield return new TreeNodeInfo<ReferenceLineContext>
            {
                Text = context => RingtoetsCommonDataResources.ReferenceLine_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ReferenceLineIcon,
                ForeColor = context => context.WrappedData.ReferenceLine == null
                                           ? Color.FromKnownColor(KnownColor.GrayText)
                                           : Color.FromKnownColor(KnownColor.ControlText),
                ContextMenuStrip = ReferenceLineContextMenuStrip
            };

            yield return new TreeNodeInfo<NormContext>
            {
                Text = context => RingtoetsCommonFormsResources.Norms_DisplayName,
                Image = context => RingtoetsCommonFormsResources.NormsIcon,
                ContextMenuStrip = NormContextMenuStrip
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<FailureMechanismContext<IFailureMechanism>>(
                StandAloneFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<MacroStabilityOutwardsFailureMechanismContext>(
                MacroStabilityOutwardsFailureMechanismEnabledChildNodeObjects,
                MacroStabilityOutwardsFailureMechanismDisabledChildNodeObjects,
                MacroStabilityOutwardsFailureMechanismEnabledContextMenuStrip,
                MacroStabilityOutwardsFailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<PipingStructureFailureMechanismContext>(
                PipingStructureFailureMechanismEnabledChildNodeObjects,
                PipingStructureFailureMechanismDisabledChildNodeObjects,
                PipingStructureFailureMechanismEnabledContextMenuStrip,
                PipingStructureFailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionsContext>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanismSections_DisplayName,
                Image = context => RingtoetsCommonFormsResources.SectionsIcon,
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
                Text = failureMechanismContribution => RingtoetsDataResources.FailureMechanismContribution_DisplayName,
                Image = failureMechanismContribution => RingtoetsCommonFormsResources.FailureMechanismContributionIcon,
                ContextMenuStrip = (failureMechanismContribution, parentData, treeViewControl) => Gui.Get(failureMechanismContribution, treeViewControl)
                                                                                                     .AddOpenItem()
                                                                                                     .AddSeparator()
                                                                                                     .AddPropertiesItem()
                                                                                                     .Build()
            };

            yield return new TreeNodeInfo<HydraulicBoundaryDatabaseContext>
            {
                Text = hydraulicBoundaryDatabase => RingtoetsFormsResources.HydraulicBoundaryDatabase_DisplayName,
                Image = hydraulicBoundaryDatabase => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.IsLinked()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = HydraulicBoundaryDatabaseChildNodeObjects,
                ContextMenuStrip = HydraulicBoundaryDatabaseContextMenuStrip
            };

            yield return new TreeNodeInfo<DesignWaterLevelCalculationsGroupContext>
            {
                Text = context => RingtoetsFormsResources.DesignWaterLevelCalculationsContext_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = DesignWaterLevelCalculationsGroupContextMenuStrip,
                ChildNodeObjects = DesignWaterLevelCalculationsGroupContextChildNodeObjects
            };

            yield return new TreeNodeInfo<DesignWaterLevelCalculationsContext>
            {
                Text = context => RingtoetsPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName),
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = DesignWaterLevelCalculationsContextMenuStrip
            };

            yield return new TreeNodeInfo<WaveHeightCalculationsGroupContext>
            {
                Text = context => RingtoetsFormsResources.WaveHeightCalculationsContext_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = WaveHeightCalculationsGroupContextMenuStrip,
                ChildNodeObjects = WaveHeightCalculationsGroupContextChildNodeObjects
            };

            yield return new TreeNodeInfo<WaveHeightCalculationsContext>
            {
                Text = context => RingtoetsPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName),
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = WaveHeightCalculationsContextMenuStrip
            };

            yield return new TreeNodeInfo<ForeshoreProfilesContext>
            {
                Text = context => RingtoetsCommonFormsResources.ForeshoreProfiles_DisplayName,
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

            yield return new TreeNodeInfo<DikeProfile>
            {
                Text = dikeProfile => dikeProfile.Name,
                Image = context => RingtoetsCommonFormsResources.DikeProfile,
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
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<Comment>
            {
                Text = comment => Resources.Comment_DisplayName,
                Image = context => RingtoetsCommonFormsResources.EditDocumentIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<RingtoetsProject>
            {
                Text = project => project.Name,
                Image = project => GuiResources.ProjectIcon,
                ChildNodeObjects = nodeData => nodeData.AssessmentSections.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) =>
                {
                    var addItem = new StrictContextMenuItem(
                        RingtoetsFormsResources.AddAssessmentSection_DisplayName,
                        RingtoetsCommonFormsResources.RingtoetsProject_ToolTip,
                        RingtoetsFormsResources.AddAssessmentSectionFolder,
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
                Text = output => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                ForeColor = context => context.WrappedData.HasOutput
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<AssemblyResultTotalContext>
            {
                Text = context => Resources.AssemblyResultTotal_DisplayName,
                Image = context => Resources.AssemblyResultTotal,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<AssemblyResultPerSectionContext>
            {
                Text = context => Resources.AssemblyResultPerSection_DisplayName,
                Image = context => Resources.AssemblyResultPerSection,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GeotechnicalFailureMechanismAssemblyCategoriesContext>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanismAssemblyCategories_DisplayName,
                Image = context => RingtoetsCommonFormsResources.NormsIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<FailureMechanismAssemblyCategoriesContext>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanismAssemblyCategories_DisplayName,
                Image = context => RingtoetsCommonFormsResources.NormsIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
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
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
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
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
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
            var ringtoetsProject = project as RingtoetsProject;
            if (ringtoetsProject == null)
            {
                return;
            }

            IEnumerable<AssessmentSection> sectionsWithHydraulicBoundaryDatabaseLinked = ringtoetsProject.AssessmentSections.Where(i => i.HydraulicBoundaryDatabase.IsLinked());
            foreach (AssessmentSection section in sectionsWithHydraulicBoundaryDatabaseLinked)
            {
                string validationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(section.HydraulicBoundaryDatabase.FilePath,
                                                                                                       section.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory());
                if (validationProblem != null)
                {
                    log.WarnFormat(
                        RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                        validationProblem);
                }
            }
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

        #region FailureMechanismView ViewInfo

        private static bool CloseFailureMechanismViewForData(FailureMechanismView<IFailureMechanism> view, object o)
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
            var assessmentSection = o as IAssessmentSection;
            return assessmentSection != null && assessmentSection.FailureMechanismContribution == view.Data && assessmentSection == view.AssessmentSection;
        }

        #endregion

        #region NormContext ViewInfo

        private static bool CloseAssessmentSectionCategoriesViewForData(AssessmentSectionAssemblyCategoriesView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            return assessmentSection != null && assessmentSection.FailureMechanismContribution == view.FailureMechanismContribution;
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

            var failureMechanismWithSectionResults = failureMechanism as IHasSectionResults<FailureMechanismSectionResult>;

            return failureMechanism != null &&
                   failureMechanismWithSectionResults != null &&
                   ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanismWithSectionResults.SectionResults);
        }

        #endregion

        #region FailureMechanismResults ViewInfo

        private static bool CloseFailureMechanismAssemblyCategoriesViewForData(FailureMechanismAssemblyCategoriesView view, object dataToCloseFor)
        {
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as IFailureMechanism;
            var failureMechanismContext = dataToCloseFor as IFailureMechanismContext<IFailureMechanism>;

            if (assessmentSection != null)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .Any(fm => ReferenceEquals(view.FailureMechanism, fm));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        #endregion

        #region HydraulicBoundaryCalculationsView ViewInfo

        private static bool CloseHydraulicBoundaryCalculationsViewForData(HydraulicBoundaryCalculationsView view, object dataToCloseFor)
        {
            IAssessmentSection viewData = view.AssessmentSection;
            var assessmentSection = dataToCloseFor as IAssessmentSection;

            return assessmentSection != null && ReferenceEquals(viewData, assessmentSection);
        }

        #endregion

        #region Comment ViewInfo

        private static bool CloseCommentViewForData(CommentView commentView, object o)
        {
            var calculationGroupContext = o as ICalculationContext<CalculationGroup, IFailureMechanism>;
            if (calculationGroupContext != null)
            {
                return GetCommentElements(calculationGroupContext.WrappedData)
                    .Any(commentElement => ReferenceEquals(commentView.Data, commentElement));
            }

            var calculationContext = o as ICalculationContext<ICalculationBase, IFailureMechanism>;
            var calculation = calculationContext?.WrappedData as ICalculation;
            if (calculation != null)
            {
                return ReferenceEquals(commentView.Data, calculation.Comments);
            }

            var failureMechanism = o as IFailureMechanism;

            var failureMechanismContext = o as IFailureMechanismContext<IFailureMechanism>;
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (failureMechanism != null)
            {
                return GetCommentElements(failureMechanism)
                    .Any(commentElement => ReferenceEquals(commentView.Data, commentElement));
            }

            var assessmentSection = o as IAssessmentSection;
            if (assessmentSection != null)
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
            var assessmentSection = o as AssessmentSection;
            return assessmentSection != null && assessmentSection == view.AssessmentSection;
        }

        #endregion

        #region AssemblyResultPerSectionContext ViewInfo

        private static bool CloseAssemblyResultPerSectionViewForData(AssemblyResultPerSectionView view, object o)
        {
            var assessmentSection = o as AssessmentSection;
            return assessmentSection != null && assessmentSection == view.AssessmentSection;
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
                RingtoetsCommonFormsResources.MapsIcon, (sender, args) => SelectMapData(assessmentSection, nodeData));

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
                new ReferenceLineContext(nodeData),
                new NormContext(nodeData.FailureMechanismContribution, nodeData),
                new FailureMechanismContributionContext(nodeData.FailureMechanismContribution, nodeData),
                new HydraulicBoundaryDatabaseContext(nodeData.HydraulicBoundaryDatabase, nodeData),
                nodeData.BackgroundData,
                nodeData.Comments
            };

            childNodes.AddRange(WrapFailureMechanismsInContexts(nodeData));
            childNodes.Add(new CategoryTreeFolder(RingtoetsFormsResources.AssemblyResultCategoryTreeFolder_DisplayName,
                                                  new object[]
                                                  {
                                                      new AssemblyResultTotalContext(nodeData),
                                                      new AssemblyResultPerSectionContext(nodeData)
                                                  }));

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
            var parentProject = (RingtoetsProject) parentNodeData;
            var assessmentSection = (AssessmentSection) nodeData;
            parentProject.AssessmentSections.Remove(assessmentSection);
            parentProject.NotifyObservers();
        }

        private ContextMenuStrip AssessmentSectionContextMenuStrip(IAssessmentSection nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var importItem = new StrictContextMenuItem(
                GuiResources.Import,
                GuiResources.Import_ToolTip,
                GuiResources.ImportIcon,
                (sender, args) => assessmentSectionMerger.StartMerge((AssessmentSection) nodeData));

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(importItem)
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

        #region StandAloneFailureMechanismContext TreeNodeInfo

        private static object[] StandAloneFailureMechanismEnabledChildNodeObjects(FailureMechanismContext<IFailureMechanism> nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private static object[] StandAloneFailureMechanismDisabledChildNodeObjects(FailureMechanismContext<IFailureMechanism> nodeData)
        {
            return new object[]
            {
                nodeData.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(IFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(IFailureMechanism nodeData)
        {
            var grassCoverSlipOffInwards = nodeData as IHasSectionResults<GrassCoverSlipOffInwardsFailureMechanismSectionResult>;
            var grassCoverSlipOffOutwards = nodeData as IHasSectionResults<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>;
            var microstability = nodeData as IHasSectionResults<MicrostabilityFailureMechanismSectionResult>;
            var technicalInnovation = nodeData as IHasSectionResults<TechnicalInnovationFailureMechanismSectionResult>;
            var strengthStabilityLengthwiseConstruction = nodeData as IHasSectionResults<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>;
            var waterPressureAsphaltCover = nodeData as IHasSectionResults<WaterPressureAsphaltCoverFailureMechanismSectionResult>;

            var failureMechanismSectionResultContexts = new object[2];
            if (grassCoverSlipOffInwards != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                        grassCoverSlipOffInwards.SectionResults, nodeData);
            }

            if (grassCoverSlipOffOutwards != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(
                        grassCoverSlipOffOutwards.SectionResults, nodeData);
            }

            if (microstability != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>(
                        microstability.SectionResults, nodeData);
            }

            if (technicalInnovation != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>(
                        technicalInnovation.SectionResults, nodeData);
            }

            if (strengthStabilityLengthwiseConstruction != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(
                        strengthStabilityLengthwiseConstruction.SectionResults, nodeData);
            }

            if (waterPressureAsphaltCover != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>(
                        waterPressureAsphaltCover.SectionResults, nodeData);
            }

            failureMechanismSectionResultContexts[1] = nodeData.OutputComments;
            return failureMechanismSectionResultContexts;
        }

        private ContextMenuStrip StandAloneFailureMechanismEnabledContextMenuStrip(FailureMechanismContext<IFailureMechanism> nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

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

        private void RemoveAllViewsForItem(FailureMechanismContext<IFailureMechanism> failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip StandAloneFailureMechanismDisabledContextMenuStrip(FailureMechanismContext<IFailureMechanism> nodeData,
                                                                                    object parentData,
                                                                                    TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(nodeData, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        #endregion

        #region MacroStabilityOutwardsFailureMechanismContext TreeNodeInfo

        private static object[] MacroStabilityOutwardsFailureMechanismEnabledChildNodeObjects(MacroStabilityOutwardsFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetOutputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Output)
            };
        }

        private static object[] MacroStabilityOutwardsFailureMechanismDisabledChildNodeObjects(MacroStabilityOutwardsFailureMechanismContext nodeData)
        {
            return new object[]
            {
                nodeData.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(MacroStabilityOutwardsFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private ContextMenuStrip MacroStabilityOutwardsFailureMechanismEnabledContextMenuStrip(MacroStabilityOutwardsFailureMechanismContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

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

        private void RemoveAllViewsForItem(MacroStabilityOutwardsFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip MacroStabilityOutwardsFailureMechanismDisabledContextMenuStrip(MacroStabilityOutwardsFailureMechanismContext nodeData,
                                                                                                object parentData,
                                                                                                TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(nodeData, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static IEnumerable<object> GetOutputs(MacroStabilityOutwardsFailureMechanism nodeData,
                                                      IAssessmentSection assessmentSection)
        {
            MacroStabilityOutwardsProbabilityAssessmentInput probabilityAssessmentInput = nodeData.MacroStabilityOutwardsProbabilityAssessmentInput;
            return new object[]
            {
                new GeotechnicalFailureMechanismAssemblyCategoriesContext(nodeData,
                                                                          assessmentSection,
                                                                          () => probabilityAssessmentInput.GetN(probabilityAssessmentInput.SectionLength)),
                new ProbabilityFailureMechanismSectionResultContext<MacroStabilityOutwardsFailureMechanismSectionResult>(
                    nodeData.SectionResults, nodeData, assessmentSection),
                nodeData.OutputComments
            };
        }

        #endregion

        #region PipingStructureFailureMechanismContext TreeNodeInfo

        private static object[] PipingStructureFailureMechanismEnabledChildNodeObjects(PipingStructureFailureMechanismContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetOutputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Output)
            };
        }

        private static object[] PipingStructureFailureMechanismDisabledChildNodeObjects(PipingStructureFailureMechanismContext nodeData)
        {
            return new object[]
            {
                nodeData.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(PipingStructureFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private ContextMenuStrip PipingStructureFailureMechanismEnabledContextMenuStrip(PipingStructureFailureMechanismContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

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

        private void RemoveAllViewsForItem(PipingStructureFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip PipingStructureFailureMechanismDisabledContextMenuStrip(PipingStructureFailureMechanismContext nodeData,
                                                                                         object parentData,
                                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(nodeData, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static IEnumerable<object> GetOutputs(PipingStructureFailureMechanism nodeData,
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
                    return RingtoetsCommonFormsResources.GeneralFolderIcon;
                case TreeFolderCategory.Input:
                    return RingtoetsCommonFormsResources.InputFolderIcon;
                case TreeFolderCategory.Output:
                    return RingtoetsCommonFormsResources.OutputFolderIcon;
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
                RingtoetsCommonFormsResources.Calculate_All,
                RingtoetsCommonFormsResources.DesignWaterLevel_Calculate_All_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
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

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(designWaterLevelItem)
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        private ContextMenuStrip WaveHeightCalculationsContextMenuStrip(WaveHeightCalculationsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var waveHeightItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_All,
                RingtoetsCommonFormsResources.WaveHeight_Calculate_All_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
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

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(waveHeightItem)
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
            var connectionItem = new StrictContextMenuItem(
                RingtoetsFormsResources.HydraulicBoundaryDatabase_Connect,
                RingtoetsFormsResources.HydraulicBoundaryDatabase_Connect_ToolTip,
                RingtoetsCommonFormsResources.DatabaseIcon, (sender, args) => SelectDatabaseFile(nodeData.AssessmentSection));

            var calculateAllItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_All,
                Resources.HydraulicBoundaryDatabase_Calculate_All_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        Gui.MainWindow,
                        AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateHydraulicBoundaryLocationCalculationActivities(nodeData.AssessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.AssessmentSection,
                                                        calculateAllItem);

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(connectionItem)
                      .AddExportItem()
                      .AddSeparator()
                      .AddCustomItem(calculateAllItem)
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        private void SelectDatabaseFile(AssessmentSection assessmentSection)
        {
            using (var dialog = new OpenFileDialog
            {
                Filter = $@"{RingtoetsFormsResources.SelectHydraulicBoundaryDatabaseFile_FilterName} (*.sqlite)|*.sqlite",
                Title = GuiResources.OpenFileDialog_Title
            })
            {
                if (dialog.ShowDialog(Gui.MainWindow) == DialogResult.OK)
                {
                    try
                    {
                        ImportHydraulicBoundaryDatabase(dialog.FileName, assessmentSection);
                    }
                    catch (CriticalFileReadException exception)
                    {
                        log.Error(exception.Message, exception);
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to update the <paramref name="assessmentSection"/> with a <see cref="HydraulicBoundaryDatabase"/> 
        /// imported from the <paramref name="databaseFile"/>.
        /// </summary>
        /// <param name="databaseFile">The file to use to import a <see cref="HydraulicBoundaryDatabase"/> from.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to which the imported 
        /// <see cref="HydraulicBoundaryDatabase"/> will be assigned.</param>
        /// <exception cref="CriticalFileReadException">Thrown when importing from the <paramref name="databaseFile"/>
        /// failed.</exception>
        private void ImportHydraulicBoundaryDatabase(string databaseFile, AssessmentSection assessmentSection)
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            if (HydraulicBoundaryDatabaseHelper.HaveEqualVersion(hydraulicBoundaryDatabase, databaseFile))
            {
                if (hydraulicBoundaryDatabase.FilePath != databaseFile)
                {
                    hydraulicBoundaryDatabase.FilePath = databaseFile;
                    hydraulicBoundaryDatabase.NotifyObservers();
                }
            }
            else
            {
                bool isClearConfirmationRequired = hydraulicBoundaryDatabase.IsLinked();
                if (isClearConfirmationRequired && !IsClearCalculationConfirmationGiven())
                {
                    return;
                }

                using (var hydraulicBoundaryLocationsImporter = new HydraulicBoundaryDatabaseImporter())
                {
                    if (!hydraulicBoundaryLocationsImporter.Import(assessmentSection, databaseFile))
                    {
                        return;
                    }

                    HydraulicBoundaryLocation[] hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();

                    assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
                    assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

                    var duneLocationsReplacementHandler = new DuneLocationsReplacementHandler(Gui.ViewCommands, assessmentSection.DuneErosion);
                    duneLocationsReplacementHandler.Replace(hydraulicBoundaryLocations);
                    duneLocationsReplacementHandler.DoPostReplacementUpdates();

                    NotifyObservers(assessmentSection);

                    if (isClearConfirmationRequired)
                    {
                        ClearCalculations(assessmentSection);
                    }
                }
            }

            log.InfoFormat(RingtoetsFormsResources.RingtoetsPlugin_SetBoundaryDatabaseFilePath_Database_on_path_0_linked,
                           assessmentSection.HydraulicBoundaryDatabase.FilePath);
        }

        private static void NotifyObservers(AssessmentSection assessmentSection)
        {
            assessmentSection.HydraulicBoundaryDatabase.NotifyObservers();
            assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();

            assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.NotifyObservers();
            assessmentSection.WaterLevelCalculationsForSignalingNorm.NotifyObservers();
            assessmentSection.WaterLevelCalculationsForLowerLimitNorm.NotifyObservers();
            assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.NotifyObservers();
            assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.NotifyObservers();
            assessmentSection.WaveHeightCalculationsForSignalingNorm.NotifyObservers();
            assessmentSection.WaveHeightCalculationsForLowerLimitNorm.NotifyObservers();
            assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.NotifyObservers();

            assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.NotifyObservers();
            assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificSignalingNorm.NotifyObservers();
            assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.NotifyObservers();
            assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.NotifyObservers();
            assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificSignalingNorm.NotifyObservers();
            assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.NotifyObservers();

            assessmentSection.DuneErosion.DuneLocations.NotifyObservers();

            assessmentSection.DuneErosion.CalculationsForMechanismSpecificFactorizedSignalingNorm.NotifyObservers();
            assessmentSection.DuneErosion.CalculationsForMechanismSpecificSignalingNorm.NotifyObservers();
            assessmentSection.DuneErosion.CalculationsForMechanismSpecificLowerLimitNorm.NotifyObservers();
            assessmentSection.DuneErosion.CalculationsForLowerLimitNorm.NotifyObservers();
            assessmentSection.DuneErosion.CalculationsForFactorizedLowerLimitNorm.NotifyObservers();
        }

        private static bool IsClearCalculationConfirmationGiven()
        {
            DialogResult confirmation = MessageBox.Show(
                RingtoetsFormsResources.Delete_Calculations_Text,
                BaseResources.Confirm,
                MessageBoxButtons.OKCancel);

            return confirmation == DialogResult.OK;
        }

        private static void ClearCalculations(IAssessmentSection nodeData)
        {
            var affectedCalculations = new List<IObservable>();
            affectedCalculations.AddRange(RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(nodeData));
            affectedCalculations.ForEachElementDo(ac => ac.NotifyObservers());

            log.Info(RingtoetsFormsResources.Calculations_Cleared);
        }

        private ContextMenuStrip DesignWaterLevelCalculationsGroupContextMenuStrip(DesignWaterLevelCalculationsGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;

            var designWaterLevelItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_All,
                RingtoetsCommonFormsResources.DesignWaterLevel_Calculate_All_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        Gui.MainWindow,
                        AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, designWaterLevelItem);

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(designWaterLevelItem)
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .Build();
        }

        private ContextMenuStrip WaveHeightCalculationsGroupContextMenuStrip(WaveHeightCalculationsGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;

            var waveHeightItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_All,
                RingtoetsCommonFormsResources.WaveHeight_Calculate_All_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        Gui.MainWindow,
                        AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, waveHeightItem);

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(waveHeightItem)
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
                                                        RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedSignalingNorm_DisplayName),
                new DesignWaterLevelCalculationsContext(context.AssessmentSection.WaterLevelCalculationsForSignalingNorm,
                                                        context.AssessmentSection,
                                                        () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.SignalingNorm),
                                                        RingtoetsCommonDataResources.AssessmentSectionCategoryType_SignalingNorm_DisplayName),
                new DesignWaterLevelCalculationsContext(context.AssessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                                        context.AssessmentSection,
                                                        () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.LowerLimitNorm),
                                                        RingtoetsCommonDataResources.AssessmentSectionCategoryType_LowerLimitNorm_DisplayName),
                new DesignWaterLevelCalculationsContext(context.AssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                                                        context.AssessmentSection,
                                                        () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedLowerLimitNorm),
                                                        RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedLowerLimitNorm_DisplayName)
            };
        }

        private static object[] WaveHeightCalculationsGroupContextChildNodeObjects(WaveHeightCalculationsGroupContext context)
        {
            return new object[]
            {
                new WaveHeightCalculationsContext(context.AssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm,
                                                  context.AssessmentSection,
                                                  () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedSignalingNorm),
                                                  RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedSignalingNorm_DisplayName),
                new WaveHeightCalculationsContext(context.AssessmentSection.WaveHeightCalculationsForSignalingNorm,
                                                  context.AssessmentSection,
                                                  () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.SignalingNorm),
                                                  RingtoetsCommonDataResources.AssessmentSectionCategoryType_SignalingNorm_DisplayName),
                new WaveHeightCalculationsContext(context.AssessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                                                  context.AssessmentSection,
                                                  () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.LowerLimitNorm),
                                                  RingtoetsCommonDataResources.AssessmentSectionCategoryType_LowerLimitNorm_DisplayName),
                new WaveHeightCalculationsContext(context.AssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                                                  context.AssessmentSection,
                                                  () => context.AssessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedLowerLimitNorm),
                                                  RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedLowerLimitNorm_DisplayName)
            };
        }

        #endregion

        #endregion

        #region Foreshore Profile Update and ImportInfo

        private static FileFilterGenerator CreateForeshoreProfileFileFilterGenerator
        {
            get
            {
                return new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                               RingtoetsCommonIOResources.Shape_file_filter_Description);
            }
        }

        private bool VerifyForeshoreProfileUpdates(ForeshoreProfilesContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.ParentFailureMechanism,
                                                                             query,
                                                                             new DialogBasedInquiryHelper(Gui.MainWindow));

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion
    }
}