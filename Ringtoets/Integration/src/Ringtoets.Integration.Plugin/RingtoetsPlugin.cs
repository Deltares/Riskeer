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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Settings;
using Core.Common.IO.Exceptions;
using Core.Common.Utils.Extensions;
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
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.IO.Hydraulics;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.Common.Service;
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
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms;
using Ringtoets.Integration.Forms.Commands;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Forms.Views.SectionResultViews;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.Service;
using Ringtoets.Integration.Service.MessageProviders;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RingtoetsDataResources = Ringtoets.Integration.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonIoResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsIntegrationPluginResources = Ringtoets.Integration.Plugin.Properties.Resources;
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

        #region failureMechanismAssociations

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
                (mechanism, assessmentSection) => new FailureMechanismContext<IFailureMechanism>(
                    mechanism,
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
                typeof(MacrostabilityInwardsFailureMechanism),
                (mechanism, assessmentSection) => new FailureMechanismContext<IFailureMechanism>(
                    mechanism,
                    assessmentSection)
            ),
            new FailureMechanismContextAssociation(
                typeof(MacrostabilityOutwardsFailureMechanism),
                (mechanism, assessmentSection) => new FailureMechanismContext<IFailureMechanism>(
                    mechanism,
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

            SettingsHelper.Instance = new RingtoetsSettingsHelper();

            ribbonCommandHandler = new RingtoetsRibbon
            {
                AddAssessmentSectionButtonCommand = new AddAssessmentSectionCommand(assessmentSectionFromFileCommandHandler)
            };
        }

        /// <summary>
        /// Returns all <see cref="Core.Common.Gui.Plugin.PropertyInfo"/> instances provided for data of <see cref="RingtoetsPlugin"/>.
        /// </summary>
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<IProject, RingtoetsProjectProperties>();
            yield return new PropertyInfo<IAssessmentSection, AssessmentSectionProperties>();
            yield return new PropertyInfo<BackgroundMapDataContext, BackgroundWmtsMapDataProperties>
            {
                CreateInstance = context => new BackgroundWmtsMapDataProperties(context.WrappedData)
            };
            yield return new PropertyInfo<HydraulicBoundaryDatabaseContext, HydraulicBoundaryDatabaseProperties>();
            yield return new PropertyInfo<FailureMechanismContributionContext, FailureMechanismContributionProperties>
            {
                CreateInstance = context => new FailureMechanismContributionProperties(
                    context.WrappedData,
                    context.Parent,
                    new FailureMechanismContributionNormChangeHandler(),
                    new AssessmentSectionCompositionChangeHandler())
            };
            yield return new PropertyInfo<FailureMechanismContext<IFailureMechanism>, StandAloneFailureMechanismContextProperties>();
            yield return new PropertyInfo<ICalculationContext<CalculationGroup, IFailureMechanism>, CalculationGroupContextProperties>();
            yield return new PropertyInfo<ICalculationContext<ICalculation, IFailureMechanism>, CalculationContextProperties>();
            yield return new PropertyInfo<ProbabilityAssessmentOutput, ProbabilityAssessmentOutputProperties>();
            yield return new PropertyInfo<DesignWaterLevelLocationsContext, DesignWaterLevelLocationsContextProperties>
            {
                CreateInstance = context => new DesignWaterLevelLocationsContextProperties(
                    context.WrappedData.HydraulicBoundaryDatabase)
            };
            yield return new PropertyInfo<DesignWaterLevelLocationContext, DesignWaterLevelLocationContextProperties>();
            yield return new PropertyInfo<WaveHeightLocationsContext, WaveHeightLocationsContextProperties>
            {
                CreateInstance = context => new WaveHeightLocationsContextProperties(
                    context.WrappedData.HydraulicBoundaryDatabase)
            };
            yield return new PropertyInfo<WaveHeightLocationContext, WaveHeightLocationContextProperties>();
            yield return new PropertyInfo<ForeshoreProfile, ForeshoreProfileProperties>();
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
                CreateInstance = () => new FailureMechanismContributionView(Gui.ViewCommands),
                AfterCreate = (view, context) => view.AssessmentSection = context.Parent
            };

            yield return new ViewInfo<DesignWaterLevelLocationsContext, IEnumerable<HydraulicBoundaryLocation>, DesignWaterLevelLocationsView>
            {
                GetViewName = (view, context) => RingtoetsFormsResources.DesignWaterLevelLocationsContext_DisplayName,
                GetViewData = context => context.WrappedData.HydraulicBoundaryDatabase?.Locations,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseDesignWaterLevelLocationsViewForData,
                AfterCreate = (view, context) =>
                {
                    view.AssessmentSection = context.WrappedData;
                    view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService;
                }
            };

            yield return new ViewInfo<WaveHeightLocationsContext, IEnumerable<HydraulicBoundaryLocation>, WaveHeightLocationsView>
            {
                GetViewName = (view, context) => RingtoetsFormsResources.WaveHeightLocationsContext_DisplayName,
                GetViewData = context => context.WrappedData.HydraulicBoundaryDatabase?.Locations,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseWaveHeightLocationsViewForData,
                AfterCreate = (view, context) =>
                {
                    view.AssessmentSection = context.WrappedData;
                    view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService;
                }
            };

            yield return new ViewInfo<IAssessmentSection, AssessmentSectionView>
            {
                GetViewName = (view, section) => RingtoetsFormsResources.AssessmentSectionMap_DisplayName,
                Image = RingtoetsFormsResources.Map
            };

            yield return new ViewInfo<FailureMechanismContext<IFailureMechanism>, FailureMechanismView<IFailureMechanism>>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

            yield return CreateFailureMechanismResultViewInfo<
                GrassCoverSlipOffInwardsFailureMechanismSectionResult,
                GrassCoverSlipOffInwardsResultView>();
            yield return CreateFailureMechanismResultViewInfo<
                GrassCoverSlipOffOutwardsFailureMechanismSectionResult,
                GrassCoverSlipOffOutwardsResultView>();
            yield return CreateFailureMechanismResultViewInfo<
                MicrostabilityFailureMechanismSectionResult,
                MicrostabilityResultView>();
            yield return CreateFailureMechanismResultViewInfo<
                PipingStructureFailureMechanismSectionResult,
                PipingStructureResultView>();
            yield return CreateFailureMechanismResultViewInfo<
                TechnicalInnovationFailureMechanismSectionResult,
                TechnicalInnovationResultView>();
            yield return CreateFailureMechanismResultViewInfo<
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult,
                StrengthStabilityLengthwiseConstructionResultView>();
            yield return CreateFailureMechanismResultViewInfo<
                WaterPressureAsphaltCoverFailureMechanismSectionResult,
                WaterPressureAsphaltCoverResultView>();
            yield return CreateFailureMechanismResultViewInfo<
                MacrostabilityInwardsFailureMechanismSectionResult,
                MacrostabilityInwardsResultView>();
            yield return CreateFailureMechanismResultViewInfo<
                MacrostabilityOutwardsFailureMechanismSectionResult,
                MacrostabilityOutwardsResultView>();

            yield return new ViewInfo<Comment, CommentView>
            {
                GetViewName = (view, context) => RingtoetsIntegrationPluginResources.Comment_DisplayName,
                GetViewData = context => context,
                Image = RingtoetsCommonFormsResources.EditDocumentIcon,
                CloseForData = CloseCommentViewForData
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<ReferenceLineContext>
            {
                Name = RingtoetsCommonDataResources.ReferenceLine_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.ReferenceLineIcon,
                FileFilter = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                              RingtoetsCommonIOResources.Shape_file_filter_Description),
                CreateFileImporter = (context, filePath) => new ReferenceLineImporter(context.WrappedData,
                                                                                      new ReferenceLineReplacementHandler(Gui.ViewCommands),
                                                                                      filePath)
            };

            yield return new ImportInfo<FailureMechanismSectionsContext>
            {
                Name = RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.SectionsIcon,
                FileFilter = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                              RingtoetsCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.ParentAssessmentSection.ReferenceLine != null,
                CreateFileImporter = (context, filePath) => new FailureMechanismSectionsImporter(context.WrappedData,
                                                                                                 context.ParentAssessmentSection.ReferenceLine,
                                                                                                 filePath)
            };
            yield return new ImportInfo<ForeshoreProfilesContext>
            {
                CreateFileImporter = (context, filePath) => new ForeshoreProfilesImporter(context.WrappedData,
                                                                                          context.ParentAssessmentSection.ReferenceLine,
                                                                                          filePath),
                Name = RingtoetsIntegrationPluginResources.ForeshoreProfilesImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsIntegrationPluginResources.Foreshore,
                FileFilter = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                              RingtoetsCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.ParentAssessmentSection.ReferenceLine != null
            };

            yield return new ImportInfo<DikeProfilesContext>
            {
                CreateFileImporter = (context, filePath) => new DikeProfilesImporter(context.WrappedData,
                                                                                     context.ParentAssessmentSection.ReferenceLine,
                                                                                     filePath),
                Name = RingtoetsIntegrationPluginResources.DikeProfilesImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.DikeProfile,
                FileFilter = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                              RingtoetsCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.ParentAssessmentSection.ReferenceLine != null
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<ReferenceLineContext>
            {
                CreateFileExporter = (context, filePath) => new ReferenceLineExporter(context.WrappedData.ReferenceLine, context.WrappedData.Id, filePath),
                IsEnabled = context => context.WrappedData.ReferenceLine != null,
                FileFilter = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                              RingtoetsCommonIOResources.Shape_file_filter_Description)
            };

            yield return new ExportInfo<HydraulicBoundaryDatabaseContext>
            {
                CreateFileExporter = (context, filePath) => new HydraulicBoundaryLocationsExporter(
                    context.WrappedData.HydraulicBoundaryDatabase.Locations, filePath,
                    RingtoetsIntegrationPluginResources.DesignWaterLevel_Description, RingtoetsIntegrationPluginResources.WaveHeight_Description),
                IsEnabled = context => context.WrappedData.HydraulicBoundaryDatabase != null,
                FileFilter = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                              RingtoetsCommonIOResources.Shape_file_filter_Description)
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
                foreach (var item in project.AssessmentSections)
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

            yield return new TreeNodeInfo<BackgroundMapDataContext>
            {
                Text = context => RingtoetsIntegrationPluginResources.RingtoetsPlugin_BackgroundMapDataContext_Text,
                Image = context => RingtoetsFormsResources.Map,
                ContextMenuStrip = BackgroundMapDataContextMenuStrip,
                ForeColor = context => context.WrappedData.IsConfigured ?
                                           Color.FromKnownColor(KnownColor.ControlText) :
                                           Color.FromKnownColor(KnownColor.GrayText)
            };

            yield return new TreeNodeInfo<ReferenceLineContext>
            {
                Text = context => RingtoetsCommonDataResources.ReferenceLine_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ReferenceLineIcon,
                ForeColor = context => context.WrappedData.ReferenceLine == null ?
                                           Color.FromKnownColor(KnownColor.GrayText) :
                                           Color.FromKnownColor(KnownColor.ControlText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) =>
                    Gui.Get(nodeData, treeViewControl)
                       .AddImportItem()
                       .AddExportItem()
                       .Build()
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<FailureMechanismContext<IFailureMechanism>>(
                StandAloneFailureMechanismEnabledChildNodeObjects,
                StandAloneFailureMechanismDisabledChildNodeObjects,
                StandAloneFailureMechanismEnabledContextMenuStrip,
                StandAloneFailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionsContext>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                Image = context => RingtoetsCommonFormsResources.SectionsIcon,
                ForeColor = context => context.WrappedData.Sections.Any() ?
                                           Color.FromKnownColor(KnownColor.ControlText) :
                                           Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = FailureMechanismSectionsContextMenuStrip
            };

            yield return new TreeNodeInfo<CategoryTreeFolder>
            {
                Text = categoryTreeFolder => categoryTreeFolder.Name,
                Image = categoryTreeFolder => GetFolderIcon(categoryTreeFolder.Category),
                ChildNodeObjects = categoryTreeFolder => categoryTreeFolder.Contents.Cast<object>().ToArray(),
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
                Image = hydraulicBoundaryDatabase => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ForeColor = context => context.WrappedData.HydraulicBoundaryDatabase == null ?
                                           Color.FromKnownColor(KnownColor.GrayText) :
                                           Color.FromKnownColor(KnownColor.ControlText),
                ChildNodeObjects = HydraulicBoundaryDatabaseChildNodeObjects,
                ContextMenuStrip = HydraulicBoundaryDatabaseContextMenuStrip
            };

            yield return new TreeNodeInfo<DesignWaterLevelLocationsContext>
            {
                Text = designWaterLevel => RingtoetsFormsResources.DesignWaterLevelLocationsContext_DisplayName,
                Image = designWaterLevel => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = DesignWaterLevelLocationsContextMenuStrip
            };

            yield return new TreeNodeInfo<WaveHeightLocationsContext>
            {
                Text = waveHeight => RingtoetsFormsResources.WaveHeightLocationsContext_DisplayName,
                Image = waveHeight => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = WaveHeightLocationsContextMenuStrip
            };

            yield return new TreeNodeInfo<ForeshoreProfilesContext>
            {
                Text = context => RingtoetsCommonFormsResources.ForeshoreProfiles_DisplayName,
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
                                                                                 .AddDeleteChildrenItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<DikeProfile>
            {
                Text = dikeProfile => dikeProfile.Name,
                Image = context => RingtoetsCommonFormsResources.DikeProfile,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddDeleteItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build(),
                CanRemove = CanRemoveDikeProfile,
                OnNodeRemoved = OnDikeProfileRemoved
            };

            yield return new TreeNodeInfo<ForeshoreProfile>
            {
                Text = foreshoreProfile => foreshoreProfile.Name,
                Image = context => RingtoetsIntegrationPluginResources.Foreshore,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddDeleteItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build(),
                CanRemove = CanRemoveForeshoreProfile,
                OnNodeRemoved = OnForeshoreProfileRemoved
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateEmptyProbabilityAssessmentOutputTreeNodeInfo(
                (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                              .AddPropertiesItem()
                                                              .Build());

            yield return CreateFailureMechanismSectionResultTreeNodeInfo<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<GrassCoverSlipOffInwardsFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<MicrostabilityFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<PipingStructureFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<TechnicalInnovationFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<WaterPressureAsphaltCoverFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<MacrostabilityInwardsFailureMechanismSectionResult>();
            yield return CreateFailureMechanismSectionResultTreeNodeInfo<MacrostabilityOutwardsFailureMechanismSectionResult>();

            yield return new TreeNodeInfo<Comment>
            {
                Text = comment => RingtoetsIntegrationPluginResources.Comment_DisplayName,
                Image = context => RingtoetsCommonFormsResources.EditDocumentIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ProbabilityAssessmentOutput>
            {
                Text = output => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
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
        }

        private static ViewInfo<FailureMechanismSectionResultContext<TResult>, IEnumerable<TResult>, TView> CreateFailureMechanismResultViewInfo<TResult, TView>()
            where TResult : FailureMechanismSectionResult
            where TView : FailureMechanismResultView<TResult>
        {
            return new ViewInfo<
                FailureMechanismSectionResultContext<TResult>,
                IEnumerable<TResult>,
                TView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
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
            var sectionsWithDatabase = ringtoetsProject.AssessmentSections.Where(i => i.HydraulicBoundaryDatabase != null);
            foreach (AssessmentSection section in sectionsWithDatabase)
            {
                string selectedFile = section.HydraulicBoundaryDatabase.FilePath;
                var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(selectedFile);
                if (validationProblem != null)
                {
                    log.WarnFormat(
                        RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                        validationProblem);
                }
            }
        }

        #region FailureMechanismView ViewInfo

        private static bool CloseFailureMechanismViewForData(FailureMechanismView<IFailureMechanism> view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as IFailureMechanism;

            var viewFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) view.Data;
            var viewFailureMechanism = viewFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewFailureMechanism, failureMechanism);
        }

        #endregion

        #region FailureMechanismContributionContext ViewInfo

        private static bool CloseFailureMechanismContributionViewForData(FailureMechanismContributionView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            return assessmentSection != null && assessmentSection.FailureMechanismContribution == view.Data && assessmentSection == view.AssessmentSection;
        }

        #endregion

        #region FailureMechanismResults ViewInfo

        private static bool CloseFailureMechanismResultViewForData<T>(T view, object dataToCloseFor) where T : IView
        {
            var viewData = view.Data;
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as IFailureMechanism;
            var failureMechanismContext = dataToCloseFor as IFailureMechanismContext<IFailureMechanism>;

            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<IHasSectionResults<FailureMechanismSectionResult>>()
                    .Any(fm => ReferenceEquals(viewData, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            var failureMechanismWithSectionResults = failureMechanism as IHasSectionResults<FailureMechanismSectionResult>;

            return failureMechanism != null &&
                   failureMechanismWithSectionResults != null &&
                   ReferenceEquals(viewData, failureMechanismWithSectionResults.SectionResults);
        }

        #endregion

        #region DesignWaterLevelLocationsView ViewInfo

        private static bool CloseDesignWaterLevelLocationsViewForData(DesignWaterLevelLocationsView view, object dataToCloseFor)
        {
            var viewData = view.AssessmentSection;
            var assessmentSection = dataToCloseFor as IAssessmentSection;

            return assessmentSection != null && ReferenceEquals(viewData, assessmentSection);
        }

        #endregion

        #region WaveHeightLocationsView ViewInfo

        private static bool CloseWaveHeightLocationsViewForData(WaveHeightLocationsView view, object dataToCloseFor)
        {
            var viewData = view.AssessmentSection;
            var assessmentSection = dataToCloseFor as IAssessmentSection;

            return assessmentSection != null && ReferenceEquals(viewData, assessmentSection);
        }

        #endregion

        #region FailureMechanismSectionsContext TreeNodeInfo

        private ContextMenuStrip FailureMechanismSectionsContextMenuStrip(FailureMechanismSectionsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddImportItem()
                      .Build();
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

        #region BackgroundMapDataContext treeNodeInfo

        private ContextMenuStrip BackgroundMapDataContextMenuStrip(BackgroundMapDataContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var assessmentSection = parentData as IAssessmentSection;

            var mapDataItem = new StrictContextMenuItem(
                RingtoetsIntegrationPluginResources.BackgroundMapData_SelectMapData,
                RingtoetsIntegrationPluginResources.BackgroundMapData_SelectMapData_Tooltip,
                RingtoetsCommonFormsResources.MapsIcon, (sender, args) => SelectMapData(assessmentSection, nodeData));

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(mapDataItem)
                      .AddPropertiesItem()
                      .Build();
        }

        private void SelectMapData(IAssessmentSection assessmentSection, BackgroundMapDataContext backgroundMapDataContext)
        {
            if (assessmentSection == null)
            {
                return;
            }

            WmtsMapData currentMapData = backgroundMapDataContext.WrappedData;
            using (var dialog = new BackgroundMapDataSelectionDialog(Gui.MainWindow, currentMapData))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SetSelectedMapData(assessmentSection, dialog.SelectedMapData);
                }
            }
        }

        private static void SetSelectedMapData(IAssessmentSection assessmentSection, ImageBasedMapData selectedMapData)
        {
            assessmentSection.BackgroundMapData.MapData = selectedMapData;
            assessmentSection.BackgroundMapData.IsVisible = selectedMapData != null;
            assessmentSection.BackgroundMapData.NotifyObservers();
            assessmentSection.NotifyObservers();
        }

        #endregion

        #region ForeshoreProfile TreeNodeInfo

        private static bool CanRemoveForeshoreProfile(ForeshoreProfile nodeData, object parentNodeData)
        {
            var parentContext = (ForeshoreProfilesContext) parentNodeData;
            IFailureMechanism failureMechanism = parentContext.ParentFailureMechanism;
            return failureMechanism is StabilityStoneCoverFailureMechanism ||
                   failureMechanism is WaveImpactAsphaltCoverFailureMechanism ||
                   failureMechanism is GrassCoverErosionOutwardsFailureMechanism ||
                   failureMechanism is HeightStructuresFailureMechanism ||
                   failureMechanism is ClosingStructuresFailureMechanism ||
                   failureMechanism is StabilityPointStructuresFailureMechanism;
        }

        private static void OnForeshoreProfileRemoved(ForeshoreProfile nodeData, object parentNodeData)
        {
            var parentContext = (ForeshoreProfilesContext) parentNodeData;
            IFailureMechanism failureMechanism = parentContext.ParentFailureMechanism;

            var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
            if (stabilityStoneCoverFailureMechanism != null)
            {
                var affectedObservables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(stabilityStoneCoverFailureMechanism, nodeData);
                foreach (IObservable affectedObservable in affectedObservables)
                {
                    affectedObservable.NotifyObservers();
                }
            }
            var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
            if (waveImpactAsphaltCoverFailureMechanism != null)
            {
                var affectedObservables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(waveImpactAsphaltCoverFailureMechanism, nodeData);
                foreach (IObservable affectedObservable in affectedObservables)
                {
                    affectedObservable.NotifyObservers();
                }
            }
            var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
            if (grassCoverErosionOutwardsFailureMechanism != null)
            {
                var affectedObservables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(grassCoverErosionOutwardsFailureMechanism, nodeData);
                foreach (IObservable affectedObservable in affectedObservables)
                {
                    affectedObservable.NotifyObservers();
                }
            }
            var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
            if (heightStructuresFailureMechanism != null)
            {
                var affectedObservables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(heightStructuresFailureMechanism, nodeData);
                foreach (IObservable affectedObservable in affectedObservables)
                {
                    affectedObservable.NotifyObservers();
                }
            }
            var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
            if (closingStructuresFailureMechanism != null)
            {
                var affectedObservables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(closingStructuresFailureMechanism, nodeData);
                foreach (IObservable affectedObservable in affectedObservables)
                {
                    affectedObservable.NotifyObservers();
                }
            }
            var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
            if (stabilityPointStructuresFailureMechanism != null)
            {
                var affectedObservables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(stabilityPointStructuresFailureMechanism, nodeData);
                foreach (IObservable affectedObservable in affectedObservables)
                {
                    affectedObservable.NotifyObservers();
                }
            }
        }

        #endregion

        #region DikeProfile TreeNodeInfo

        private static bool CanRemoveDikeProfile(DikeProfile nodeData, object parentData)
        {
            return parentData is DikeProfilesContext;
        }

        private static void OnDikeProfileRemoved(DikeProfile nodeData, object parentData)
        {
            var parentContext = (DikeProfilesContext) parentData;
            var changedObservables = RingtoetsDataSynchronizationService.RemoveDikeProfile(parentContext.ParentFailureMechanism,
                                                                                           nodeData).ToArray();

            foreach (IObservable observable in changedObservables)
            {
                observable.NotifyObservers();
            }
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

        #region AssessmentSection TreeNodeInfo

        private static object[] AssessmentSectionChildNodeObjects(AssessmentSection nodeData)
        {
            var childNodes = new List<object>
            {
                new BackgroundMapDataContext((WmtsMapData) nodeData.BackgroundMapData.MapData, nodeData.BackgroundMapData),
                new ReferenceLineContext(nodeData),
                new FailureMechanismContributionContext(nodeData.FailureMechanismContribution, nodeData),
                new HydraulicBoundaryDatabaseContext(nodeData),
                nodeData.Comments
            };

            IEnumerable<object> failureMechanismContexts = WrapFailureMechanismsInContexts(nodeData);
            childNodes.AddRange(failureMechanismContexts);

            return childNodes.ToArray();
        }

        private static IEnumerable<object> WrapFailureMechanismsInContexts(IAssessmentSection assessmentSection)
        {
            return assessmentSection
                .GetFailureMechanisms()
                .Select(failureMechanism => failureMechanismAssociations
                            .First(a => a.Match(failureMechanism))
                            .Create(failureMechanism, assessmentSection)
                );
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
            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
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

        private static IList GetInputs(IFailureMechanism nodeData, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.InputComments
            };
        }

        private static IList GetOutputs(IFailureMechanism nodeData)
        {
            var duneErosion = nodeData as IHasSectionResults<DuneErosionFailureMechanismSectionResult>;
            var grassCoverSlipOffInwards = nodeData as IHasSectionResults<GrassCoverSlipOffInwardsFailureMechanismSectionResult>;
            var grassCoverSlipOffOutwards = nodeData as IHasSectionResults<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>;
            var microstability = nodeData as IHasSectionResults<MicrostabilityFailureMechanismSectionResult>;
            var pipingStructure = nodeData as IHasSectionResults<PipingStructureFailureMechanismSectionResult>;
            var stabilityStoneCover = nodeData as IHasSectionResults<StabilityStoneCoverFailureMechanismSectionResult>;
            var technicalInnovation = nodeData as IHasSectionResults<TechnicalInnovationFailureMechanismSectionResult>;
            var strengthStabilityLengthwiseConstruction = nodeData as IHasSectionResults<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>;
            var waterPressureAsphaltCover = nodeData as IHasSectionResults<WaterPressureAsphaltCoverFailureMechanismSectionResult>;
            var waveImpactAsphaltCover = nodeData as IHasSectionResults<WaveImpactAsphaltCoverFailureMechanismSectionResult>;
            var closingStructures = nodeData as IHasSectionResults<ClosingStructuresFailureMechanismSectionResult>;
            var macrostabilityInwards = nodeData as IHasSectionResults<MacrostabilityInwardsFailureMechanismSectionResult>;
            var macrostabilityOutwards = nodeData as IHasSectionResults<MacrostabilityOutwardsFailureMechanismSectionResult>;
            var stabilityPointConstruction = nodeData as IHasSectionResults<StabilityPointStructuresFailureMechanismSectionResult>;

            var failureMechanismSectionResultContexts = new object[2];
            if (duneErosion != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>(duneErosion.SectionResults, nodeData);
            }
            if (grassCoverSlipOffInwards != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>(grassCoverSlipOffInwards.SectionResults, nodeData);
            }
            if (grassCoverSlipOffOutwards != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(grassCoverSlipOffOutwards.SectionResults, nodeData);
            }
            if (microstability != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>(microstability.SectionResults, nodeData);
            }
            if (pipingStructure != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>(pipingStructure.SectionResults, nodeData);
            }
            if (stabilityStoneCover != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>(stabilityStoneCover.SectionResults, nodeData);
            }
            if (technicalInnovation != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>(technicalInnovation.SectionResults, nodeData);
            }
            if (strengthStabilityLengthwiseConstruction != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(strengthStabilityLengthwiseConstruction.SectionResults, nodeData);
            }
            if (waterPressureAsphaltCover != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>(waterPressureAsphaltCover.SectionResults, nodeData);
            }
            if (waveImpactAsphaltCover != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>(waveImpactAsphaltCover.SectionResults, nodeData);
            }
            if (closingStructures != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>(closingStructures.SectionResults, nodeData);
            }
            if (macrostabilityInwards != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<MacrostabilityInwardsFailureMechanismSectionResult>(macrostabilityInwards.SectionResults, nodeData);
            }
            if (macrostabilityOutwards != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<MacrostabilityOutwardsFailureMechanismSectionResult>(macrostabilityOutwards.SectionResults, nodeData);
            }
            if (stabilityPointConstruction != null)
            {
                failureMechanismSectionResultContexts[0] =
                    new FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>(stabilityPointConstruction.SectionResults, nodeData);
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

        #region CategoryTreeFolder TreeNodeInfo

        private static Image GetFolderIcon(TreeFolderCategory category)
        {
            switch (category)
            {
                case TreeFolderCategory.General:
                    return RingtoetsCommonFormsResources.GeneralFolderIcon;
                case TreeFolderCategory.Input:
                    return RingtoetsCommonFormsResources.InputFolderIcon;
                case TreeFolderCategory.Output:
                    return RingtoetsCommonFormsResources.OutputFolderIcon;
                default:
                    throw new InvalidEnumArgumentException(nameof(category),
                                                           (int) category,
                                                           typeof(TreeFolderCategory));
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
            if (nodeData.WrappedData.HydraulicBoundaryDatabase == null)
            {
                return new object[0];
            }
            return new object[]
            {
                new DesignWaterLevelLocationsContext(nodeData.WrappedData),
                new WaveHeightLocationsContext(nodeData.WrappedData)
            };
        }

        private ContextMenuStrip DesignWaterLevelLocationsContextMenuStrip(DesignWaterLevelLocationsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var designWaterLevelItem = new StrictContextMenuItem(
                RingtoetsFormsResources.DesignWaterLevel_Calculate_All,
                RingtoetsFormsResources.DesignWaterLevel_Calculate_All_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    IAssessmentSection assessmentSection = nodeData.WrappedData;
                    bool successfulCalculation = hydraulicBoundaryLocationCalculationGuiService.CalculateDesignWaterLevels(assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                                           assessmentSection.HydraulicBoundaryDatabase.Locations,
                                                                                                                           assessmentSection.FailureMechanismContribution.Norm,
                                                                                                                           new DesignWaterLevelCalculationMessageProvider());
                    if (successfulCalculation)
                    {
                        nodeData.NotifyObservers();
                    }
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.WrappedData, designWaterLevelItem);

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(designWaterLevelItem)
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        private ContextMenuStrip WaveHeightLocationsContextMenuStrip(WaveHeightLocationsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var waveHeightItem = new StrictContextMenuItem(
                RingtoetsFormsResources.WaveHeight_Calculate_All,
                RingtoetsFormsResources.WaveHeight_Calculate_All_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }
                    IAssessmentSection assessmentSection = nodeData.WrappedData;
                    bool successfulCalculation = hydraulicBoundaryLocationCalculationGuiService.CalculateWaveHeights(assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                                     assessmentSection.HydraulicBoundaryDatabase.Locations,
                                                                                                                     assessmentSection.FailureMechanismContribution.Norm,
                                                                                                                     new WaveHeightCalculationMessageProvider());
                    if (successfulCalculation)
                    {
                        nodeData.NotifyObservers();
                    }
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.WrappedData, waveHeightItem);

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

        private ContextMenuStrip HydraulicBoundaryDatabaseContextMenuStrip(HydraulicBoundaryDatabaseContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var connectionItem = new StrictContextMenuItem(
                RingtoetsFormsResources.HydraulicBoundaryDatabase_Connect,
                RingtoetsFormsResources.HydraulicBoundaryDatabase_Connect_ToolTip,
                RingtoetsCommonFormsResources.DatabaseIcon, (sender, args) => SelectDatabaseFile(nodeData.WrappedData));

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(connectionItem)
                      .AddExportItem()
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
                Filter = string.Format("{0} (*.sqlite)|*.sqlite", RingtoetsFormsResources.SelectHydraulicBoundaryDatabaseFile_FilterName),
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
            var hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            var isHydraulicBoundaryDatabaseSet = hydraulicBoundaryDatabase != null;
            var isClearConfirmationRequired = isHydraulicBoundaryDatabaseSet && !HydraulicDatabaseHelper.HaveEqualVersion(hydraulicBoundaryDatabase, databaseFile);
            var isClearConfirmationGiven = isClearConfirmationRequired && IsClearCalculationConfirmationGiven();

            if (isHydraulicBoundaryDatabaseSet && isClearConfirmationRequired && !isClearConfirmationGiven)
            {
                return;
            }

            var previousHydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            using (var hydraulicBoundaryLocationsImporter = new HydraulicBoundaryDatabaseImporter())
            {
                if (hydraulicBoundaryLocationsImporter.Import(assessmentSection, databaseFile))
                {
                    if (isClearConfirmationGiven)
                    {
                        ClearCalculations(assessmentSection);
                    }

                    if (!ReferenceEquals(previousHydraulicBoundaryDatabase, assessmentSection.HydraulicBoundaryDatabase))
                    {
                        HydraulicBoundaryLocation[] hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase?.Locations.ToArray()
                                                                                 ?? new HydraulicBoundaryLocation[0];

                        assessmentSection.GrassCoverErosionOutwards.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(hydraulicBoundaryLocations);
                        assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.NotifyObservers();

                        var duneLocationsReplacementHandler = new DuneLocationsReplacementHandler(Gui.ViewCommands, assessmentSection.DuneErosion);
                        duneLocationsReplacementHandler.Replace(hydraulicBoundaryLocations);
                        duneLocationsReplacementHandler.DoPostReplacementUpdates();

                        assessmentSection.DuneErosion.DuneLocations.NotifyObservers();
                    }
                    log.InfoFormat(RingtoetsFormsResources.RingtoetsPlugin_SetBoundaryDatabaseFilePath_Database_on_path_0_linked,
                                   assessmentSection.HydraulicBoundaryDatabase.FilePath);
                }
            }
        }

        private static bool IsClearCalculationConfirmationGiven()
        {
            var confirmation = MessageBox.Show(
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

        #endregion
    }
}