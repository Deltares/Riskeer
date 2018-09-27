// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring piping calculations.
    /// </summary>
    public partial class PipingCalculationsView : UserControl, ISelectionProvider, IView
    {
        private const int stochasticSoilModelColumnIndex = 1;
        private const int stochasticSoilProfileColumnIndex = 2;
        private const int selectableHydraulicBoundaryLocationColumnIndex = 4;

        private readonly Observer hydraulicBoundaryLocationsObserver;
        private readonly RecursiveObserver<CalculationGroup, PipingInput> pipingInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> pipingCalculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, PipingCalculationScenario> pipingCalculationObserver;
        private readonly Observer pipingFailureMechanismObserver;
        private readonly RecursiveObserver<PipingSurfaceLineCollection, PipingSurfaceLine> pipingSurfaceLineObserver;
        private readonly Observer pipingStochasticSoilModelsObserver;
        private readonly RecursiveObserver<PipingStochasticSoilModelCollection, PipingStochasticSoilProfile> stochasticSoilProfileObserver;
        private IAssessmentSection assessmentSection;
        private CalculationGroup calculationGroup;
        private PipingFailureMechanism pipingFailureMechanism;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationsView"/>.
        /// </summary>
        public PipingCalculationsView()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeListBox();

            pipingFailureMechanismObserver = new Observer(OnPipingFailureMechanismUpdate);
            hydraulicBoundaryLocationsObserver = new Observer(UpdateSelectableHydraulicBoundaryLocationsColumn);
            // The concat is needed to observe the input of calculations in child groups.
            pipingInputObserver = new RecursiveObserver<CalculationGroup, PipingInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<PipingCalculationScenario>().Select(pc => pc.InputParameters)));
            pipingCalculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children);
            pipingCalculationObserver = new RecursiveObserver<CalculationGroup, PipingCalculationScenario>(dataGridViewControl.RefreshDataGridView, pcg => pcg.Children);

            pipingSurfaceLineObserver = new RecursiveObserver<PipingSurfaceLineCollection, PipingSurfaceLine>(UpdateDataGridViewDataSource, rpslc => rpslc);

            pipingStochasticSoilModelsObserver = new Observer(OnStochasticSoilModelsUpdate);
            stochasticSoilProfileObserver = new RecursiveObserver<PipingStochasticSoilModelCollection, PipingStochasticSoilProfile>(dataGridViewControl.RefreshDataGridView, ssmc => ssmc.SelectMany(ssm => ssm.StochasticSoilProfiles));
        }

        /// <summary>
        /// Gets or sets the piping failure mechanism.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism
        {
            get
            {
                return pipingFailureMechanism;
            }
            set
            {
                pipingFailureMechanism = value;
                pipingStochasticSoilModelsObserver.Observable = pipingFailureMechanism?.StochasticSoilModels;
                pipingFailureMechanismObserver.Observable = pipingFailureMechanism;
                pipingSurfaceLineObserver.Observable = pipingFailureMechanism?.SurfaceLines;
                stochasticSoilProfileObserver.Observable = pipingFailureMechanism?.StochasticSoilModels;

                UpdateStochasticSoilModelColumn();
                UpdateStochasticSoilProfileColumn();
                UpdateSelectableHydraulicBoundaryLocationsColumn();
                UpdateSectionsListBox();
                UpdateGenerateScenariosButtonState();
            }
        }

        /// <summary>
        /// Gets or sets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection
        {
            get
            {
                return assessmentSection;
            }
            set
            {
                assessmentSection = value;

                hydraulicBoundaryLocationsObserver.Observable = assessmentSection?.HydraulicBoundaryDatabase.Locations;

                UpdateSelectableHydraulicBoundaryLocationsColumn();
            }
        }

        public object Selection
        {
            get
            {
                return CreateSelectedItemFromCurrentRow();
            }
        }

        public object Data
        {
            get
            {
                return calculationGroup;
            }
            set
            {
                calculationGroup = value as CalculationGroup;

                if (calculationGroup != null)
                {
                    UpdateDataGridViewDataSource();
                    pipingInputObserver.Observable = calculationGroup;
                    pipingCalculationObserver.Observable = calculationGroup;
                    pipingCalculationGroupObserver.Observable = calculationGroup;
                }
                else
                {
                    dataGridViewControl.SetDataSource(null);
                    pipingInputObserver.Observable = null;
                    pipingCalculationObserver.Observable = null;
                    pipingCalculationGroupObserver.Observable = null;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            // Necessary to correctly load the content of the dropdown lists of the comboboxes...
            UpdateDataGridViewDataSource();
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dataGridViewControl.CellFormatting -= OnCellFormatting;
                dataGridViewControl.CurrentRowChanged -= DataGridViewOnCurrentRowChangedHandler;

                hydraulicBoundaryLocationsObserver.Dispose();
                pipingFailureMechanismObserver.Dispose();
                pipingInputObserver.Dispose();
                pipingCalculationObserver.Dispose();
                pipingSurfaceLineObserver.Dispose();
                pipingCalculationGroupObserver.Dispose();
                stochasticSoilProfileObserver.Dispose();
                pipingStochasticSoilModelsObserver.Dispose();

                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.CurrentRowChanged += DataGridViewOnCurrentRowChangedHandler;
            dataGridViewControl.CellFormatting += OnCellFormatting;

            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.Name),
                Resources.PipingCalculation_Name_DisplayName);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>>(
                nameof(PipingCalculationRow.StochasticSoilModel),
                Resources.PipingInput_StochasticSoilModel_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>.This),
                nameof(DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>.DisplayName));

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>>(
                nameof(PipingCalculationRow.StochasticSoilProfile),
                Resources.PipingInput_StochasticSoilProfile_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>.This),
                nameof(DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>.DisplayName));

            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.StochasticSoilProfileProbability),
                Resources.PipingCalculationsView_InitializeDataGridView_Stochastic_soil_profile_probability);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>>(
                nameof(PipingCalculationRow.SelectableHydraulicBoundaryLocation),
                RingtoetsCommonFormsResources.HydraulicBoundaryLocation_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.This),
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.DisplayName));

            string dampingFactorExitHeader = Resources.PipingInput_DampingFactorExit_DisplayName;
            dampingFactorExitHeader = char.ToLowerInvariant(dampingFactorExitHeader[0]) + dampingFactorExitHeader.Substring(1);

            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.DampingFactorExitMean),
                $"{Resources.Probabilistics_Mean_Symbol} {dampingFactorExitHeader}");

            string phreaticLevelExitHeader = Resources.PipingInput_PhreaticLevelExit_DisplayName;
            phreaticLevelExitHeader = char.ToLowerInvariant(phreaticLevelExitHeader[0]) + phreaticLevelExitHeader.Substring(1);

            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.PhreaticLevelExitMean),
                $"{Resources.Probabilistics_Mean_Symbol} {phreaticLevelExitHeader}");

            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.EntryPointL),
                Resources.PipingInput_EntryPointL_DisplayName);

            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.ExitPointL),
                Resources.PipingInput_ExitPointL_DisplayName);

            UpdateStochasticSoilModelColumn();
            UpdateStochasticSoilProfileColumn();
            UpdateSelectableHydraulicBoundaryLocationsColumn();
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = nameof(FailureMechanismSection.Name);
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void UpdateGenerateScenariosButtonState()
        {
            buttonGenerateScenarios.Enabled = pipingFailureMechanism != null &&
                                              pipingFailureMechanism.SurfaceLines.Any() &&
                                              pipingFailureMechanism.StochasticSoilModels.Any();
        }

        private static void SetItemsOnObjectCollection(DataGridViewComboBoxCell.ObjectCollection objectCollection, object[] comboBoxItems)
        {
            objectCollection.Clear();
            objectCollection.AddRange(comboBoxItems);
        }

        private PipingInputContext CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;

            var pipingCalculationRow = (PipingCalculationRow) currentRow?.DataBoundItem;

            PipingInputContext selection = null;
            if (pipingCalculationRow != null)
            {
                selection = new PipingInputContext(
                    pipingCalculationRow.PipingCalculation.InputParameters,
                    pipingCalculationRow.PipingCalculation,
                    pipingFailureMechanism.SurfaceLines,
                    pipingFailureMechanism.StochasticSoilModels,
                    pipingFailureMechanism,
                    assessmentSection);
            }

            return selection;
        }

        private static IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations(
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, PipingSurfaceLine surfaceLine)
        {
            Point2D referencePoint = surfaceLine?.ReferenceLineIntersectionWorldPoint;
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                hydraulicBoundaryLocations, referencePoint);
        }

        #region Data sources

        private void UpdateDataGridViewDataSource()
        {
            // Skip changes coming from the view itself
            if (dataGridViewControl.IsCurrentCellInEditMode)
            {
                UpdateStochasticSoilProfileColumn();
                dataGridViewControl.AutoResizeColumns();

                return;
            }

            var failureMechanismSection = listBox.SelectedItem as FailureMechanismSection;
            if (failureMechanismSection == null || calculationGroup == null)
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(failureMechanismSection.Points);
            IEnumerable<PipingCalculationScenario> pipingCalculations = calculationGroup
                                                                        .GetCalculations()
                                                                        .OfType<PipingCalculationScenario>()
                                                                        .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));

            PrefillComboBoxListItemsAtColumnLevel();

            List<PipingCalculationRow> dataSource = pipingCalculations.Select(pc => new PipingCalculationRow(pc, new ObservablePropertyChangeHandler(pc, pc.InputParameters))).ToList();
            dataGridViewControl.SetDataSource(dataSource);
            dataGridViewControl.ClearCurrentCell();

            UpdateStochasticSoilModelColumn();
            UpdateStochasticSoilProfileColumn();
            UpdateSelectableHydraulicBoundaryLocationsColumn();
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>> GetStochasticSoilModelsDataSource(
            IEnumerable<PipingStochasticSoilModel> stochasticSoilModels)
        {
            yield return new DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>(null);

            foreach (PipingStochasticSoilModel stochasticSoilModel in stochasticSoilModels)
            {
                yield return new DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>(stochasticSoilModel);
            }
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>> GetSoilProfilesDataSource(
            IEnumerable<PipingStochasticSoilProfile> stochasticSoilProfiles)
        {
            yield return new DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>(null);

            foreach (PipingStochasticSoilProfile stochasticSoilProfile in stochasticSoilProfiles)
            {
                yield return new DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>(stochasticSoilProfile);
            }
        }

        private static List<DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>> GetSelectableHydraulicBoundaryLocationsDataSource(
            IEnumerable<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations = null)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>>
            {
                new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(null)
            };

            if (selectableHydraulicBoundaryLocations != null)
            {
                dataGridViewComboBoxItemWrappers.AddRange(selectableHydraulicBoundaryLocations.Select(hbl => new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(hbl)));
            }

            return dataGridViewComboBoxItemWrappers;
        }

        #endregion

        #region Update combo box list items

        #region Update Selectable Hydraulic Boundary Locations Column

        private void UpdateSelectableHydraulicBoundaryLocationsColumn()
        {
            var column = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(selectableHydraulicBoundaryLocationColumnIndex);

            using (new SuspendDataGridViewColumnResizes(column))
            {
                foreach (DataGridViewRow dataGridViewRow in dataGridViewControl.Rows)
                {
                    FillAvailableSelectableHydraulicBoundaryLocationsList(dataGridViewRow);
                }
            }
        }

        private void FillAvailableSelectableHydraulicBoundaryLocationsList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (PipingCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<SelectableHydraulicBoundaryLocation> locations = GetSelectableHydraulicBoundaryLocationsForCalculation(rowData.PipingCalculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[selectableHydraulicBoundaryLocationColumnIndex];
            DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>[] dataGridViewComboBoxItemWrappers = GetSelectableHydraulicBoundaryLocationsDataSource(locations).ToArray();
            SetItemsOnObjectCollection(cell.Items, dataGridViewComboBoxItemWrappers);
        }

        private IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocationsForCalculation(PipingCalculation pipingCalculation)
        {
            return GetSelectableHydraulicBoundaryLocations(assessmentSection?.HydraulicBoundaryDatabase.Locations,
                                                           pipingCalculation.InputParameters.SurfaceLine);
        }

        #endregion

        #region Update Stochastic Soil Model Column

        private void UpdateStochasticSoilModelColumn()
        {
            using (new SuspendDataGridViewColumnResizes(dataGridViewControl.GetColumnFromIndex(stochasticSoilModelColumnIndex)))
            {
                foreach (DataGridViewRow dataGridViewRow in dataGridViewControl.Rows)
                {
                    FillAvailableSoilModelsList(dataGridViewRow);
                }
            }
        }

        private void FillAvailableSoilModelsList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (PipingCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<PipingStochasticSoilModel> stochasticSoilModels = GetSoilModelsForCalculation(rowData.PipingCalculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilModelColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
        }

        private IEnumerable<PipingStochasticSoilModel> GetSoilModelsForCalculation(PipingCalculation pipingCalculation)
        {
            if (pipingFailureMechanism == null)
            {
                return Enumerable.Empty<PipingStochasticSoilModel>();
            }

            return PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                pipingCalculation.InputParameters.SurfaceLine,
                pipingFailureMechanism.StochasticSoilModels);
        }

        #endregion

        #region Update Stochastic Soil Profile Column

        private void UpdateStochasticSoilProfileColumn()
        {
            using (new SuspendDataGridViewColumnResizes(dataGridViewControl.GetColumnFromIndex(stochasticSoilProfileColumnIndex)))
            {
                foreach (DataGridViewRow dataGridViewRow in dataGridViewControl.Rows)
                {
                    FillAvailableSoilProfilesList(dataGridViewRow);
                }
            }
        }

        private static void FillAvailableSoilProfilesList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (PipingCalculationRow) dataGridViewRow.DataBoundItem;
            PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(rowData.PipingCalculation.InputParameters);

            IEnumerable<PipingStochasticSoilProfile> stochasticSoilProfiles = GetSoilProfilesForCalculation(rowData.PipingCalculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilProfileColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetSoilProfilesDataSource(stochasticSoilProfiles).ToArray());
        }

        private static IEnumerable<PipingStochasticSoilProfile> GetSoilProfilesForCalculation(PipingCalculation pipingCalculation)
        {
            if (pipingCalculation.InputParameters.StochasticSoilModel == null)
            {
                return Enumerable.Empty<PipingStochasticSoilProfile>();
            }

            return pipingCalculation.InputParameters.StochasticSoilModel.StochasticSoilProfiles;
        }

        #endregion

        #endregion

        #region Prefill combo box list items

        private void PrefillComboBoxListItemsAtColumnLevel()
        {
            var stochasticSoilModelColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(stochasticSoilModelColumnIndex);
            var stochasticSoilProfileColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(stochasticSoilProfileColumnIndex);
            var selectableHydraulicBoundaryLocationColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(selectableHydraulicBoundaryLocationColumnIndex);

            // Need to prefill for all possible data in order to guarantee 'combo box' columns
            // do not generate errors when their cell value is not present in the list of available
            // items.
            using (new SuspendDataGridViewColumnResizes(stochasticSoilModelColumn))
            {
                PipingStochasticSoilModelCollection stochasticSoilModels = pipingFailureMechanism.StochasticSoilModels;
                SetItemsOnObjectCollection(stochasticSoilModelColumn.Items, GetStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
            }

            using (new SuspendDataGridViewColumnResizes(stochasticSoilProfileColumn))
            {
                PipingStochasticSoilProfile[] pipingSoilProfiles = GetPipingStochasticSoilProfilesFromStochasticSoilModels();
                SetItemsOnObjectCollection(stochasticSoilProfileColumn.Items, GetSoilProfilesDataSource(pipingSoilProfiles).ToArray());
            }

            using (new SuspendDataGridViewColumnResizes(selectableHydraulicBoundaryLocationColumn))
            {
                SetItemsOnObjectCollection(selectableHydraulicBoundaryLocationColumn.Items,
                                           GetSelectableHydraulicBoundaryLocationsDataSource(GetSelectableHydraulicBoundaryLocationsFromFailureMechanism()).ToArray());
            }
        }

        private IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocationsFromFailureMechanism()
        {
            if (assessmentSection == null)
            {
                return null;
            }

            List<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;

            List<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations = hydraulicBoundaryLocations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, null)).ToList();
            if (PipingFailureMechanism == null || !PipingFailureMechanism.SurfaceLines.Any())
            {
                return selectableHydraulicBoundaryLocations;
            }

            foreach (PipingSurfaceLine surfaceLine in PipingFailureMechanism.SurfaceLines)
            {
                selectableHydraulicBoundaryLocations.AddRange(GetSelectableHydraulicBoundaryLocations(hydraulicBoundaryLocations, surfaceLine));
            }

            return selectableHydraulicBoundaryLocations;
        }

        private PipingStochasticSoilProfile[] GetPipingStochasticSoilProfilesFromStochasticSoilModels()
        {
            return pipingFailureMechanism?.StochasticSoilModels
                                         .SelectMany(ssm => ssm.StochasticSoilProfiles)
                                         .Distinct()
                                         .ToArray();
        }

        #endregion

        #region Event handling

        private void DataGridViewOnCurrentRowChangedHandler(object sender, EventArgs e)
        {
            OnSelectionChanged();
        }

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewDataSource();
        }

        private void OnGenerateScenariosButtonClick(object sender, EventArgs e)
        {
            if (calculationGroup == null)
            {
                return;
            }

            var dialog = new PipingSurfaceLineSelectionDialog(Parent, pipingFailureMechanism.SurfaceLines);
            dialog.ShowDialog();
            IEnumerable<ICalculationBase> calculationsStructure = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                dialog.SelectedItems,
                pipingFailureMechanism.StochasticSoilModels,
                pipingFailureMechanism.GeneralInput);
            foreach (ICalculationBase item in calculationsStructure)
            {
                calculationGroup.Children.Add(item);
            }

            calculationGroup.NotifyObservers();
        }

        private void OnPipingFailureMechanismUpdate()
        {
            UpdateGenerateScenariosButtonState();
            UpdateSectionsListBox();
        }

        private void OnStochasticSoilModelsUpdate()
        {
            UpdateGenerateScenariosButtonState();
            UpdateStochasticSoilModelColumn();
            UpdateStochasticSoilProfileColumn();
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (pipingFailureMechanism != null && pipingFailureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(pipingFailureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = pipingFailureMechanism.Sections.First();
            }
        }

        private void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex == selectableHydraulicBoundaryLocationColumnIndex)
            {
                DataGridViewRow dataGridViewRow = dataGridViewControl.GetRowFromIndex(eventArgs.RowIndex);
                var dataItem = dataGridViewRow.DataBoundItem as PipingCalculationRow;

                dataGridViewRow.Cells[selectableHydraulicBoundaryLocationColumnIndex].ReadOnly = dataItem != null
                                                                                                 && dataItem.PipingCalculation.InputParameters.UseAssessmentLevelManualInput;
            }
        }

        #endregion
    }
}