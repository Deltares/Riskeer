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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring macro stability inwards calculations.
    /// </summary>
    public partial class MacroStabilityInwardsCalculationsView : UserControl, ISelectionProvider, IView
    {
        private const int stochasticSoilModelColumnIndex = 1;
        private const int stochasticSoilProfileColumnIndex = 2;
        private const int selectableHydraulicBoundaryLocationColumnIndex = 4;

        private readonly Observer hydraulicBoundaryLocationsObserver;
        private readonly RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput> inputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario> calculationObserver;
        private readonly Observer failureMechanismObserver;
        private readonly RecursiveObserver<MacroStabilityInwardsSurfaceLineCollection, MacroStabilityInwardsSurfaceLine> surfaceLineObserver;
        private readonly Observer stochasticSoilModelsObserver;
        private readonly RecursiveObserver<MacroStabilityInwardsStochasticSoilModelCollection, MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfileObserver;
        private IAssessmentSection assessmentSection;
        private CalculationGroup calculationGroup;
        private MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationsView"/>.
        /// </summary>
        public MacroStabilityInwardsCalculationsView()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeListBox();

            failureMechanismObserver = new Observer(OnFailureMechanismUpdate);
            hydraulicBoundaryLocationsObserver = new Observer(UpdateSelectableHydraulicBoundaryLocationsColumn);
            // The concat is needed to observe the input of calculations in child groups.
            inputObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput>(UpdateDataGridViewDataSource, cg => cg.Children
                                                                                                                                      .Concat<object>(cg.Children
                                                                                                                                                        .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                                                                                        .Select(pc => pc.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, cg => cg.Children);
            calculationObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario>(() => dataGridViewControl.RefreshDataGridView(), cg => cg.Children);

            surfaceLineObserver = new RecursiveObserver<MacroStabilityInwardsSurfaceLineCollection, MacroStabilityInwardsSurfaceLine>(UpdateDataGridViewDataSource, rpslc => rpslc);

            stochasticSoilModelsObserver = new Observer(OnStochasticSoilModelsUpdate);
            stochasticSoilProfileObserver = new RecursiveObserver<MacroStabilityInwardsStochasticSoilModelCollection, MacroStabilityInwardsStochasticSoilProfile>(() => dataGridViewControl.RefreshDataGridView(), ssmc => ssmc.SelectMany(ssm => ssm.StochasticSoilProfiles));
        }

        /// <summary>
        /// Gets or sets the macro stability inwards failure mechanism.
        /// </summary>
        public MacroStabilityInwardsFailureMechanism MacroStabilityInwardsFailureMechanism
        {
            get
            {
                return macroStabilityInwardsFailureMechanism;
            }
            set
            {
                macroStabilityInwardsFailureMechanism = value;
                stochasticSoilModelsObserver.Observable = macroStabilityInwardsFailureMechanism?.StochasticSoilModels;
                failureMechanismObserver.Observable = macroStabilityInwardsFailureMechanism;
                surfaceLineObserver.Observable = macroStabilityInwardsFailureMechanism?.SurfaceLines;
                stochasticSoilProfileObserver.Observable = macroStabilityInwardsFailureMechanism?.StochasticSoilModels;

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
                    inputObserver.Observable = calculationGroup;
                    calculationObserver.Observable = calculationGroup;
                    calculationGroupObserver.Observable = calculationGroup;
                }
                else
                {
                    dataGridViewControl.SetDataSource(null);
                    inputObserver.Observable = null;
                    calculationObserver.Observable = null;
                    calculationGroupObserver.Observable = null;
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
                failureMechanismObserver.Dispose();
                inputObserver.Dispose();
                calculationObserver.Dispose();
                surfaceLineObserver.Dispose();
                calculationGroupObserver.Dispose();
                stochasticSoilProfileObserver.Dispose();
                stochasticSoilModelsObserver.Dispose();

                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.CurrentRowChanged += DataGridViewOnCurrentRowChangedHandler;
            dataGridViewControl.CellFormatting += OnCellFormatting;

            dataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsCalculationRow.Name),
                Resources.MacroStabilityInwardsCalculation_Name_DisplayName);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>>(
                nameof(MacroStabilityInwardsCalculationRow.StochasticSoilModel),
                Resources.MacroStabilityInwardsInput_StochasticSoilModel_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>.This),
                nameof(DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>.DisplayName));

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>>(
                nameof(MacroStabilityInwardsCalculationRow.StochasticSoilProfile),
                Resources.MacroStabilityInwardsInput_StochasticSoilProfile_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>.This),
                nameof(DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>.DisplayName));

            dataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsCalculationRow.StochasticSoilProfileProbability),
                Resources.MacroStabilityInwardsCalculationsView_InitializeDataGridView_Stochastic_soil_profile_probability);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>>(
                nameof(MacroStabilityInwardsCalculationRow.SelectableHydraulicBoundaryLocation),
                RingtoetsCommonFormsResources.HydraulicBoundaryLocation_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.This),
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.DisplayName));

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
            buttonGenerateScenarios.Enabled = macroStabilityInwardsFailureMechanism != null &&
                                              macroStabilityInwardsFailureMechanism.SurfaceLines.Any() &&
                                              macroStabilityInwardsFailureMechanism.StochasticSoilModels.Any();
        }

        private static void SetItemsOnObjectCollection(DataGridViewComboBoxCell.ObjectCollection objectCollection, object[] comboBoxItems)
        {
            objectCollection.Clear();
            objectCollection.AddRange(comboBoxItems);
        }

        private MacroStabilityInwardsInputContext CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;

            var calculationRow = (MacroStabilityInwardsCalculationRow) currentRow?.DataBoundItem;

            MacroStabilityInwardsInputContext selection = null;
            if (calculationRow != null)
            {
                selection = new MacroStabilityInwardsInputContext(
                    calculationRow.MacroStabilityInwardsCalculation.InputParameters,
                    calculationRow.MacroStabilityInwardsCalculation,
                    macroStabilityInwardsFailureMechanism.SurfaceLines,
                    macroStabilityInwardsFailureMechanism.StochasticSoilModels,
                    macroStabilityInwardsFailureMechanism,
                    assessmentSection);
            }

            return selection;
        }

        private static IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations(
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, MacroStabilityInwardsSurfaceLine surfaceLine)
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
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = calculationGroup
                                                                                 .GetCalculations()
                                                                                 .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                 .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));

            PrefillComboBoxListItemsAtColumnLevel();

            List<MacroStabilityInwardsCalculationRow> dataSource = calculations.Select(pc => new MacroStabilityInwardsCalculationRow(pc, new ObservablePropertyChangeHandler(pc, pc.InputParameters))).ToList();
            dataGridViewControl.SetDataSource(dataSource);
            dataGridViewControl.ClearCurrentCell();

            UpdateStochasticSoilModelColumn();
            UpdateStochasticSoilProfileColumn();
            UpdateSelectableHydraulicBoundaryLocationsColumn();
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>> GetStochasticSoilModelsDataSource(IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels)
        {
            yield return new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>(null);

            foreach (MacroStabilityInwardsStochasticSoilModel stochasticSoilModel in stochasticSoilModels)
            {
                yield return new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>(stochasticSoilModel);
            }
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>> GetSoilProfilesDataSource(IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles)
        {
            yield return new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>(null);

            foreach (MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile in stochasticSoilProfiles)
            {
                yield return new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>(stochasticSoilProfile);
            }
        }

        private static List<DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>> GetSelectableHydraulicBoundaryLocationsDataSource(IEnumerable<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations = null)
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
            var rowData = (MacroStabilityInwardsCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<SelectableHydraulicBoundaryLocation> locations = GetSelectableHydraulicBoundaryLocationsForCalculation(rowData.MacroStabilityInwardsCalculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[selectableHydraulicBoundaryLocationColumnIndex];
            DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>[] dataGridViewComboBoxItemWrappers = GetSelectableHydraulicBoundaryLocationsDataSource(locations).ToArray();
            SetItemsOnObjectCollection(cell.Items, dataGridViewComboBoxItemWrappers);
        }

        private IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocationsForCalculation(MacroStabilityInwardsCalculation macroStabilityInwardsCalculation)
        {
            return GetSelectableHydraulicBoundaryLocations(assessmentSection?.HydraulicBoundaryDatabase.Locations,
                                                           macroStabilityInwardsCalculation.InputParameters.SurfaceLine);
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
            var rowData = (MacroStabilityInwardsCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels = GetSoilModelsForCalculation(rowData.MacroStabilityInwardsCalculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilModelColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
        }

        private IEnumerable<MacroStabilityInwardsStochasticSoilModel> GetSoilModelsForCalculation(MacroStabilityInwardsCalculation macroStabilityInwardsCalculation)
        {
            if (macroStabilityInwardsFailureMechanism == null)
            {
                return Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>();
            }

            return MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                macroStabilityInwardsCalculation.InputParameters.SurfaceLine,
                macroStabilityInwardsFailureMechanism.StochasticSoilModels);
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

        private void FillAvailableSoilProfilesList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (MacroStabilityInwardsCalculationRow) dataGridViewRow.DataBoundItem;
            MacroStabilityInwardsInputService.SyncStochasticSoilProfileWithStochasticSoilModel(rowData.MacroStabilityInwardsCalculation.InputParameters);

            IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles = GetSoilProfilesForCalculation(rowData.MacroStabilityInwardsCalculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilProfileColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetSoilProfilesDataSource(stochasticSoilProfiles).ToArray());
        }

        private static IEnumerable<MacroStabilityInwardsStochasticSoilProfile> GetSoilProfilesForCalculation(MacroStabilityInwardsCalculation macroStabilityInwardsCalculation)
        {
            if (macroStabilityInwardsCalculation.InputParameters.StochasticSoilModel == null)
            {
                return Enumerable.Empty<MacroStabilityInwardsStochasticSoilProfile>();
            }

            return macroStabilityInwardsCalculation.InputParameters.StochasticSoilModel.StochasticSoilProfiles;
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
                MacroStabilityInwardsStochasticSoilModelCollection stochasticSoilModels = macroStabilityInwardsFailureMechanism.StochasticSoilModels;
                SetItemsOnObjectCollection(stochasticSoilModelColumn.Items, GetStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
            }

            using (new SuspendDataGridViewColumnResizes(stochasticSoilProfileColumn))
            {
                MacroStabilityInwardsStochasticSoilProfile[] soilProfiles = GetStochasticSoilProfilesFromStochasticSoilModels();
                SetItemsOnObjectCollection(stochasticSoilProfileColumn.Items, GetSoilProfilesDataSource(soilProfiles).ToArray());
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
            if (MacroStabilityInwardsFailureMechanism == null || !MacroStabilityInwardsFailureMechanism.SurfaceLines.Any())
            {
                return selectableHydraulicBoundaryLocations;
            }

            foreach (MacroStabilityInwardsSurfaceLine surfaceLine in MacroStabilityInwardsFailureMechanism.SurfaceLines)
            {
                selectableHydraulicBoundaryLocations.AddRange(GetSelectableHydraulicBoundaryLocations(hydraulicBoundaryLocations, surfaceLine));
            }

            return selectableHydraulicBoundaryLocations;
        }

        private MacroStabilityInwardsStochasticSoilProfile[] GetStochasticSoilProfilesFromStochasticSoilModels()
        {
            return macroStabilityInwardsFailureMechanism?.StochasticSoilModels
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

            var dialog = new MacroStabilityInwardsSurfaceLineSelectionDialog(Parent, macroStabilityInwardsFailureMechanism.SurfaceLines);
            dialog.ShowDialog();
            IEnumerable<ICalculationBase> calculationsStructure = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                dialog.SelectedItems,
                macroStabilityInwardsFailureMechanism.StochasticSoilModels);
            foreach (ICalculationBase item in calculationsStructure)
            {
                calculationGroup.Children.Add(item);
            }

            calculationGroup.NotifyObservers();
        }

        private void OnFailureMechanismUpdate()
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

            if (macroStabilityInwardsFailureMechanism != null && macroStabilityInwardsFailureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(macroStabilityInwardsFailureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = macroStabilityInwardsFailureMechanism.Sections.First();
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
                var dataItem = dataGridViewRow.DataBoundItem as MacroStabilityInwardsCalculationRow;

                dataGridViewRow.Cells[selectableHydraulicBoundaryLocationColumnIndex].ReadOnly = dataItem != null
                                                                                                 && dataItem.MacroStabilityInwardsCalculation.InputParameters.UseAssessmentLevelManualInput;
            }
        }

        #endregion
    }
}