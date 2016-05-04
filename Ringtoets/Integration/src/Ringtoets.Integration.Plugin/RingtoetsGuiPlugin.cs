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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.IO.Exceptions;
using log4net;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Placeholder;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using RingtoetsDataResources = Ringtoets.Integration.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using UtilsResources = Core.Common.Utils.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Plugin
{
    /// <summary>
    /// The GUI plugin for the Ringtoets application.
    /// </summary>
    public class RingtoetsGuiPlugin : GuiPlugin
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiPlugin));

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new RingtoetsRibbon();
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

        /// <summary>
        /// Returns all <see cref="Core.Common.Gui.Plugin.PropertyInfo"/> instances provided for data of <see cref="RingtoetsGuiPlugin"/>.
        /// </summary>
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<IAssessmentSection, AssessmentSectionProperties>();
            yield return new PropertyInfo<HydraulicBoundaryDatabaseContext, HydraulicBoundaryDatabaseProperties>();
        }

        /// <summary>
        /// Returns all <see cref="ViewInfo"/> instances provided for data of <see cref="RingtoetsGuiPlugin"/>.
        /// </summary>
        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<FailureMechanismContributionContext, FailureMechanismContribution, FailureMechanismContributionView>
            {
                GetViewName = (v, o) => RingtoetsDataResources.FailureMechanismContribution_DisplayName,
                GetViewData = context => context.WrappedData,
                Image = RingtoetsCommonFormsResources.FailureMechanismContributionIcon,
                CloseForData = CloseFailureMechanismContributionViewForData,
                AfterCreate = (view, context) =>
                {
                    view.AssessmentSection = context.Parent;
                    view.ViewCommands = Gui.ViewCommands;
                }
            };

            yield return new ViewInfo<IAssessmentSection, AssessmentSectionView>
            {
                GetViewName = (v, o) => RingtoetsFormsResources.AssessmentSectionMap_DisplayName,
                Image = RingtoetsFormsResources.Map
            };

            yield return new ViewInfo<FailureMechanismSectionResultContext, IEnumerable<FailureMechanismSectionResult>, FailureMechanismResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.SectionResults,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };

            yield return new ViewInfo<CommentContext<ICommentable>, ICommentable, CommentView>
            {
                GetViewName = (v, o) => Resources.Comment_DisplayName,
                GetViewData = context => context.CommentContainer,
                Image = RingtoetsCommonFormsResources.EditDocumentIcon,
                CloseForData = CloseCommentViewForData
            };
        }

        /// <summary>
        /// Gets the child data instances that have <see cref="ViewInfo"/> definitions of some parent data object.
        /// </summary>
        /// <param name="dataObject">The parent data object.</param>
        /// <returns>Sequence of child data.</returns>
        public override IEnumerable<object> GetChildDataWithViewDefinitions(object dataObject)
        {
            var assessmentSection = dataObject as IAssessmentSection;
            if (assessmentSection != null)
            {
                yield return assessmentSection.FailureMechanismContribution;
            }
        }

        /// <summary>
        /// Returns all <see cref="TreeNodeInfo"/> instances provided for data of <see cref="RingtoetsGuiPlugin"/>.
        /// </summary>
        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<IAssessmentSection>
            {
                Text = assessmentSection => assessmentSection.Name,
                Image = assessmentSection => RingtoetsFormsResources.AssessmentSectionFolderIcon,
                EnsureVisibleOnCreate = (assessmentSection, parent) => true,
                ChildNodeObjects = AssessmentSectionChildNodeObjects,
                ContextMenuStrip = AssessmentSectionContextMenuStrip,
                CanRename = (assessmentSection, parentData) => true,
                OnNodeRenamed = AssessmentSectionOnNodeRenamed,
                CanRemove = (assessmentSection, parentNodeData) => true,
                OnNodeRemoved = AssessmentSectionOnNodeRemoved
            };

            yield return new TreeNodeInfo<ReferenceLineContext>
            {
                Text = context => RingtoetsCommonDataResources.ReferenceLine_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ReferenceLineIcon,
                ForeColor = context => context.WrappedData == null ?
                                           Color.FromKnownColor(KnownColor.GrayText) :
                                           Color.FromKnownColor(KnownColor.ControlText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) =>
                                   Gui.Get(nodeData, treeViewControl).AddImportItem().Build()
            };

            yield return new DefaultFailureMechanismTreeNodeInfo<FailureMechanismPlaceholderContext, FailureMechanismPlaceholder>(
                FailureMechanismPlaceholderChildNodeObjects,
                FailureMechanismPlaceholderContextMenuStrip,
                Gui);

            yield return new TreeNodeInfo<FailureMechanismSectionsContext>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                Image = context => RingtoetsCommonFormsResources.Sections,
                ForeColor = context => context.WrappedData.Any() ?
                                           Color.FromKnownColor(KnownColor.ControlText) :
                                           Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = FailureMechanismSectionsContextMenuStrip
            };

            yield return new TreeNodeInfo<PlaceholderWithReadonlyName>
            {
                Text = placeholderWithReadonlyName => placeholderWithReadonlyName.Name,
                Image = placeholderWithReadonlyName => GetIconForPlaceholder(placeholderWithReadonlyName),
                ForeColor = placeholderWithReadonlyName => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = PlaceholderWithReadonlyNameContextMenuStrip
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
                                                                                                     .AddExportItem()
                                                                                                     .Build()
            };

            yield return new TreeNodeInfo<HydraulicBoundaryDatabaseContext>
            {
                Text = hydraulicBoundaryDatabase => RingtoetsFormsResources.HydraulicBoundaryDatabase_DisplayName,
                Image = hydraulicBoundaryDatabase => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                CanRename = (context, o) => false,
                ForeColor = context => context.Parent.HydraulicBoundaryDatabase == null ?
                                           Color.FromKnownColor(KnownColor.GrayText) :
                                           Color.FromKnownColor(KnownColor.ControlText),
                ContextMenuStrip = HydraulicBoundaryDatabaseContextMenuStrip
            };

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext>
            {
                Text = context => RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<CommentContext<ICommentable>>
            {
                Text = comment => Resources.Comment_DisplayName,
                Image = context => RingtoetsCommonFormsResources.EditDocumentIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        private void RemoveOnOpenProjectListener(IProjectOwner projectOwner)
        {
            if (projectOwner != null)
            {
                projectOwner.ProjectOpened -= VerifyHydraulicBoundaryDatabasePath;
            }
        }

        private void AddOnOpenProjectListener(IProjectOwner projectOwner)
        {
            if (projectOwner != null)
            {
                projectOwner.ProjectOpened += VerifyHydraulicBoundaryDatabasePath;
            }
        }

        private static void VerifyHydraulicBoundaryDatabasePath(Project project)
        {
            var sectionsWithDatabase = project.Items.OfType<IAssessmentSection>().Where(i => i.HydraulicBoundaryDatabase != null);
            foreach (IAssessmentSection section in sectionsWithDatabase)
            {
                string selectedFile = section.HydraulicBoundaryDatabase.FilePath;
                var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(selectedFile);
                if (validationProblem != null)
                {
                    log.WarnFormat(
                        Resources.RingtoetsGuiPlugin_VerifyHydraulicBoundaryDatabasePath_Hydraulic_boundary_database_connection_failed_0_,
                        validationProblem);
                }
            }
        }

        #region FailureMechanismContributionContext ViewInfo

        private static bool CloseFailureMechanismContributionViewForData(FailureMechanismContributionView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            return assessmentSection != null && assessmentSection.FailureMechanismContribution == view.Data && assessmentSection == view.AssessmentSection;
        }

        #endregion

        #region FailureMechanismResults ViewInfo

        private static bool CloseFailureMechanismResultViewForData(FailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as IFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<IFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection.GetFailureMechanisms().Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #region FailureMechanismSectionsContext

        private ContextMenuStrip FailureMechanismSectionsContextMenuStrip(FailureMechanismSectionsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddImportItem()
                      .Build();
        }

        #endregion

        #region Comment ViewInfo

        private static bool CloseCommentViewForData(CommentView commentView, object o)
        {
            var calculationContext = o as PipingCalculationScenarioContext;
            if (calculationContext != null)
            {
                return calculationContext.WrappedData == commentView.Data;
            }

            var calculationGroupContext = o as PipingCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                return GetCommentableElements(calculationGroupContext)
                    .Any(commentableElement => ReferenceEquals(commentView.Data, commentableElement));
            }

            var assessmentSection = o as IAssessmentSection;
            if (assessmentSection != null)
            {
                return GetCommentableElements(assessmentSection)
                    .Any(commentableElement => ReferenceEquals(commentView.Data, commentableElement));
            }

            return false;
        }

        private static IEnumerable<ICommentable> GetCommentableElements(PipingCalculationGroupContext calculationGroupContext)
        {
            return calculationGroupContext.WrappedData.GetCalculations();
        }

        private static IEnumerable<ICommentable> GetCommentableElements(IAssessmentSection assessmentSection)
        {
            yield return assessmentSection;
            foreach (var commentable in assessmentSection.GetFailureMechanisms().SelectMany(GetCommentableElements))
            {
                yield return commentable;
            }
        }

        private static IEnumerable<ICommentable> GetCommentableElements(IFailureMechanism failureMechanism)
        {
            yield return failureMechanism;
            foreach (ICalculation commentableCalculation in failureMechanism.Calculations)
            {
                yield return commentableCalculation;
            }
        }

        #endregion

        # region assessmentSection

        private object[] AssessmentSectionChildNodeObjects(IAssessmentSection nodeData)
        {
            var childNodes = new List<object>
            {
                new ReferenceLineContext(nodeData),
                new FailureMechanismContributionContext(nodeData.FailureMechanismContribution, nodeData),
                new HydraulicBoundaryDatabaseContext(nodeData),
                new CommentContext<ICommentable>(nodeData)
            };

            IEnumerable<object> failureMechanismContexts = WrapFailureMechanismsInContexts(nodeData);
            childNodes.AddRange(failureMechanismContexts);

            return childNodes.ToArray();
        }

        private static IEnumerable<object> WrapFailureMechanismsInContexts(IAssessmentSection nodeData)
        {
            foreach (IFailureMechanism failureMechanism in nodeData.GetFailureMechanisms())
            {
                var placeHolder = failureMechanism as FailureMechanismPlaceholder;
                var piping = failureMechanism as PipingFailureMechanism;
                var grassCoverErosionInwards = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                if (placeHolder != null)
                {
                    yield return new FailureMechanismPlaceholderContext(placeHolder, nodeData);
                }
                else if (piping != null)
                {
                    yield return new PipingFailureMechanismContext(piping, nodeData);
                }
                else if (grassCoverErosionInwards != null)
                {
                    yield return new GrassCoverErosionInwardsFailureMechanismContext(grassCoverErosionInwards, nodeData);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void AssessmentSectionOnNodeRenamed(IAssessmentSection nodeData, string newName)
        {
            nodeData.Name = newName;
            nodeData.NotifyObservers();
        }

        private void AssessmentSectionOnNodeRemoved(IAssessmentSection nodeData, object parentNodeData)
        {
            var parentProject = (Project) parentNodeData;

            parentProject.Items.Remove(nodeData);
            parentProject.NotifyObservers();
        }

        private ContextMenuStrip AssessmentSectionContextMenuStrip(IAssessmentSection nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddRenameItem()
                      .AddDeleteItem()
                      .AddSeparator()
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        # endregion

        # region FailureMechanismPlaceHolderContext

        private object[] FailureMechanismPlaceholderChildNodeObjects(FailureMechanismPlaceholderContext nodeData)
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

        private IList GetInputs(FailureMechanismPlaceholder nodeData, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(nodeData, assessmentSection),
                nodeData.Locations,
                nodeData.BoundaryConditions,
                new CommentContext<ICommentable>(nodeData)
            };
        }

        private IList GetOutputs(FailureMechanismPlaceholder nodeData)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext(nodeData.SectionResults, nodeData)
            };
        }

        private ContextMenuStrip FailureMechanismPlaceholderContextMenuStrip(FailureMechanismPlaceholderContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var changeRelevancyItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                RingtoetsCommonFormsResources.Checkbox_ticked,
                (sender, args) =>
                {
                    Gui.ViewCommands.RemoveAllViewsForItem(nodeData);
                    nodeData.WrappedData.IsRelevant = false;
                    nodeData.WrappedData.NotifyObservers();
                }
                );

            var calculateItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                RingtoetsCommonFormsResources.Calculate_all_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                null)
            {
                Enabled = false
            };
            var clearOutputItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Clear_all_output,
                RingtoetsCommonFormsResources.Clear_all_output_ToolTip,
                RingtoetsCommonFormsResources.ClearIcon, null
                )
            {
                Enabled = false
            };

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(changeRelevancyItem)
                      .AddSeparator()
                      .AddCustomItem(calculateItem)
                      .AddCustomItem(clearOutputItem)
                      .AddSeparator()
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        # endregion

        # region PlaceholderWithReadonlyName

        private static Bitmap GetIconForPlaceholder(PlaceholderWithReadonlyName nodeData)
        {
            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                return RingtoetsCommonFormsResources.GenericInputOutputIcon;
            }
            return RingtoetsFormsResources.PlaceholderIcon;
        }

        private ContextMenuStrip PlaceholderWithReadonlyNameContextMenuStrip(PlaceholderWithReadonlyName nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IContextMenuBuilder menuBuilder = Gui.Get(nodeData, treeViewControl);

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                menuBuilder.AddOpenItem();
            }

            if (nodeData is OutputPlaceholder)
            {
                var clearItem = new StrictContextMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase,
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip,
                    RingtoetsCommonFormsResources.ClearIcon,
                    null)
                {
                    Enabled = false
                };

                menuBuilder.AddCustomItem(clearItem);
            }

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                menuBuilder.AddSeparator();
            }
            return menuBuilder.AddImportItem()
                              .AddExportItem()
                              .AddSeparator()
                              .AddPropertiesItem()
                              .Build();
        }

        # endregion

        # region CategoryTreeFolder

        private Image GetFolderIcon(TreeFolderCategory category)
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
                    throw new NotImplementedException();
            }
        }

        private ContextMenuStrip CategoryTreeFolderContextMenu(CategoryTreeFolder nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .Build();
        }

        # endregion

        #region HydraulicBoundaryDatabase

        private ContextMenuStrip HydraulicBoundaryDatabaseContextMenuStrip(HydraulicBoundaryDatabaseContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var connectionItem = new StrictContextMenuItem(
                RingtoetsFormsResources.HydraulicBoundaryDatabase_Connect,
                RingtoetsFormsResources.HydraulicBoundaryDatabase_Connect_ToolTip,
                RingtoetsCommonFormsResources.DatabaseIcon, (sender, args) => { SelectDatabaseFile(nodeData.Parent); });

            var designWaterLevelItem = new StrictContextMenuItem(
                RingtoetsFormsResources.DesignWaterLevel_Calculate,
                RingtoetsFormsResources.DesignWaterLevel_Calculate_ToolTip,
                RingtoetsCommonFormsResources.FailureMechanismIcon,
                (sender, args) =>
                {
                    var hrdFile = nodeData.Parent.HydraulicBoundaryDatabase.FilePath;
                    var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hrdFile);
                    if (validationProblem == null)
                    {
                        var hlcdDirectory = Path.GetDirectoryName(hrdFile);
                        var activities = nodeData.Parent.HydraulicBoundaryDatabase.Locations.Select(hbl => CreateHydraRingTargetProbabilityCalculationActivity(nodeData.Parent, hbl, hlcdDirectory)).ToList();

                        ActivityProgressDialogRunner.Run(Gui.MainWindow, activities);

                        nodeData.Parent.NotifyObservers();
                    }
                    else
                    {
                        log.ErrorFormat(Resources.RingtoetsGuiPlugin_HydraulicBoundaryDatabaseContextMenuStrip_Start_calculation_failed_0_, validationProblem);
                    }
                });

            if (nodeData.Parent.HydraulicBoundaryDatabase == null)
            {
                designWaterLevelItem.Enabled = false;
                designWaterLevelItem.ToolTipText = RingtoetsFormsResources.DesignWaterLevel_No_HRD_To_Calculate;
            }

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(connectionItem)
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddCustomItem(designWaterLevelItem)
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        private void SelectDatabaseFile(IAssessmentSection assessmentSection)
        {
            var windowTitle = RingtoetsFormsResources.SelectHydraulicBoundaryDatabaseFile_Title;
            using (var dialog = new OpenFileDialog
            {
                Filter = string.Format("{0} (*.sqlite)|*.sqlite", RingtoetsFormsResources.SelectHydraulicBoundaryDatabaseFile_FilterName),
                Multiselect = false,
                Title = windowTitle,
                RestoreDirectory = true,
                CheckFileExists = false
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
        private static void ImportHydraulicBoundaryDatabase(string databaseFile, IAssessmentSection assessmentSection)
        {
            var hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            var isHydraulicBoundaryDatabaseSet = hydraulicBoundaryDatabase != null;
            var isClearConfirmationRequired = isHydraulicBoundaryDatabaseSet && !HydraulicDatabaseHelper.HaveEqualVersion(hydraulicBoundaryDatabase, databaseFile);
            var isClearConfirmationGiven = isClearConfirmationRequired && IsClearCalculationConfirmationGiven();

            if (!isHydraulicBoundaryDatabaseSet || !isClearConfirmationRequired || isClearConfirmationGiven)
            {
                var hydraulicBoundaryLocationsImporter = new HydraulicBoundaryDatabaseImporter();
                if (hydraulicBoundaryLocationsImporter.Import(assessmentSection, databaseFile))
                {
                    if (isClearConfirmationRequired)
                    {
                        ClearCalculations(assessmentSection);
                    }
                    assessmentSection.NotifyObservers();
                    log.InfoFormat(RingtoetsFormsResources.RingtoetsGuiPlugin_SetBoundaryDatabaseFilePath_Database_on_path_0_linked, assessmentSection.HydraulicBoundaryDatabase.FilePath);
                }
            }
        }

        private static TargetProbabilityCalculationActivity CreateHydraRingTargetProbabilityCalculationActivity(IAssessmentSection assessmentSection,
                                                                                                                HydraulicBoundaryLocation hydraulicBoundaryLocation, string hlcdDirectory)
        {
            return HydraRingActivityFactory.Create(
                string.Format(Resources.RingtoetsGuiPlugin_Calculate_assessment_level_for_location_0_, hydraulicBoundaryLocation.Id),
                hlcdDirectory,
                assessmentSection.Name, // TODO: Provide name of reference line instead
                HydraRingTimeIntegrationSchemeType.FBC,
                HydraRingUncertaintiesType.All,
                new AssessmentLevelCalculationInput((int) hydraulicBoundaryLocation.Id, assessmentSection.FailureMechanismContribution.Norm),
                output => { ParseHydraRingOutput(hydraulicBoundaryLocation, output); });
        }

        private static void ParseHydraRingOutput(HydraulicBoundaryLocation hydraulicBoundaryLocation, TargetProbabilityCalculationOutput output)
        {
            if (output != null)
            {
                hydraulicBoundaryLocation.DesignWaterLevel = output.Result;
            }
            else
            {
                throw new InvalidOperationException(Resources.RingtoetsGuiPlugin_Error_during_assessment_level_calculation);
            }
        }

        private static bool IsClearCalculationConfirmationGiven()
        {
            var confirmation = MessageBox.Show(
                RingtoetsFormsResources.Delete_Calculations_Text,
                BaseResources.Confirm,
                MessageBoxButtons.OKCancel);

            return (confirmation == DialogResult.OK);
        }

        private static void ClearCalculations(IAssessmentSection nodeData)
        {
            var failureMechanisms = nodeData.GetFailureMechanisms();

            foreach (ICalculation calc in failureMechanisms.SelectMany(fm => fm.Calculations))
            {
                calc.ClearOutput();
                calc.ClearHydraulicBoundaryLocation();
                calc.NotifyObservers();
            }

            log.Info(RingtoetsFormsResources.Calculations_Deleted);
        }

        #endregion
    }
}