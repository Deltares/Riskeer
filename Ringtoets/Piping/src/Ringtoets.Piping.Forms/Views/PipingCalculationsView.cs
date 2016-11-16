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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Service;
using CoreCommonControlsResources = Core.Common.Controls.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring piping calculations.
    /// </summary>
    public partial class PipingCalculationsView : UserControl, ISelectionProvider
    {
        private const int stochasticSoilModelColumnIndex = 1;
        private const int stochasticSoilProfileColumnIndex = 2;
        private const int hydraulicBoundaryLocationColumnIndex = 4;
        private readonly Observer assessmentSectionObserver;
        private readonly RecursiveObserver<CalculationGroup, PipingInput> pipingInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> pipingCalculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, PipingCalculationScenario> pipingCalculationObserver;
        private readonly Observer pipingFailureMechanismObserver;
        private readonly Observer pipingStochasticSoilModelsObserver;
        private IAssessmentSection assessmentSection;
        private CalculationGroup calculationGroup;
        private PipingFailureMechanism pipingFailureMechanism;

        private bool updatingDataSource;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of the <see cref="PipingCalculationsView"/> class.
        /// </summary>
        public PipingCalculationsView()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeListBox();

            pipingStochasticSoilModelsObserver = new Observer(OnStochasticSoilModelsUpdate);
            pipingFailureMechanismObserver = new Observer(OnPipingFailureMechanismUpdate);
            assessmentSectionObserver = new Observer(UpdateHydraulicBoundaryLocationsColumn);
            // The concat is needed to observe the input of calculations in child groups.
            pipingInputObserver = new RecursiveObserver<CalculationGroup, PipingInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<PipingCalculationScenario>().Select(pc => pc.InputParameters)));
            pipingCalculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children);
            pipingCalculationObserver = new RecursiveObserver<CalculationGroup, PipingCalculationScenario>(dataGridViewControl.RefreshDataGridView, pcg => pcg.Children);
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
                pipingStochasticSoilModelsObserver.Observable = pipingFailureMechanism != null ? pipingFailureMechanism.StochasticSoilModels : null;
                pipingFailureMechanismObserver.Observable = pipingFailureMechanism;

                UpdateStochasticSoilModelColumn();
                UpdateStochasticSoilProfileColumn();
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

                assessmentSectionObserver.Observable = assessmentSection;

                UpdateHydraulicBoundaryLocationsColumn();
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

        public object Selection
        {
            get
            {
                return CreateSelectedItemFromCurrentRow();
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
            assessmentSectionObserver.Dispose();
            pipingFailureMechanismObserver.Dispose();
            pipingInputObserver.Dispose();
            pipingCalculationObserver.Dispose();
            pipingCalculationGroupObserver.Dispose();
            pipingStochasticSoilModelsObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCellClickHandler(DataGridViewOnCellClick);

            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingCalculationRow>(row => row.Name),
                Resources.PipingCalculation_Name_DisplayName);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<StochasticSoilModel>>(
                TypeUtils.GetMemberName<PipingCalculationRow>(row => row.StochasticSoilModel),
                Resources.PipingInput_StochasticSoilModel_DisplayName,
                null,
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<StochasticSoilModel>>(wrapper => wrapper.This),
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<StochasticSoilModel>>(wrapper => wrapper.DisplayName));

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<StochasticSoilProfile>>(
                TypeUtils.GetMemberName<PipingCalculationRow>(row => row.StochasticSoilProfile),
                Resources.PipingInput_StochasticSoilProfile_DisplayName,
                null,
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<StochasticSoilProfile>>(wrapper => wrapper.This),
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<StochasticSoilProfile>>(wrapper => wrapper.DisplayName));

            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingCalculationRow>(row => row.StochasticSoilProfileProbability),
                Resources.PipingCalculationsView_InitializeDataGridView_Stochastic_soil_profile_probability);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>>(
                TypeUtils.GetMemberName<PipingCalculationRow>(row => row.HydraulicBoundaryLocation),
                Resources.PipingInput_HydraulicBoundaryLocation_DisplayName,
                null,
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>>(wrapper => wrapper.This),
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>>(wrapper => wrapper.DisplayName));

            var dampingFactorExitHeader = Resources.PipingInput_DampingFactorExit_DisplayName;
            dampingFactorExitHeader = char.ToLowerInvariant(dampingFactorExitHeader[0]) + dampingFactorExitHeader.Substring(1);

            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingCalculationRow>(row => row.DampingFactorExitMean),
                string.Format("{0} {1}", Resources.Probabilistics_Mean_Symbol, dampingFactorExitHeader));

            var phreaticLevelExitHeader = Resources.PipingInput_PhreaticLevelExit_DisplayName;
            phreaticLevelExitHeader = char.ToLowerInvariant(phreaticLevelExitHeader[0]) + phreaticLevelExitHeader.Substring(1);

            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingCalculationRow>(row => row.PhreaticLevelExitMean),
                string.Format("{0} {1}", Resources.Probabilistics_Mean_Symbol, phreaticLevelExitHeader));

            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingCalculationRow>(pcs => pcs.EntryPointL),
                Resources.PipingInput_EntryPointL_DisplayName);

            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingCalculationRow>(pcs => pcs.ExitPointL),
                Resources.PipingInput_ExitPointL_DisplayName);

            UpdateHydraulicBoundaryLocationsColumn();
            UpdateStochasticSoilModelColumn();
            UpdateStochasticSoilProfileColumn();
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = TypeUtils.GetMemberName<FailureMechanismSection>(s => s.Name);
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void UpdateHydraulicBoundaryLocationsColumn()
        {
            var hydraulicBoundaryLocationColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(hydraulicBoundaryLocationColumnIndex);

            using (new SuspendDataGridViewColumnResizes(hydraulicBoundaryLocationColumn))
            {
                List<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection != null && assessmentSection.HydraulicBoundaryDatabase != null ?
                                                                                 assessmentSection.HydraulicBoundaryDatabase.Locations :
                                                                                 null;
                SetItemsOnObjectCollection(hydraulicBoundaryLocationColumn.Items, GetHydraulicBoundaryLocationsDataSource(hydraulicBoundaryLocations).ToArray());
            }
        }

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

        private void UpdateGenerateScenariosButtonState()
        {
            buttonGenerateScenarios.Enabled = pipingFailureMechanism != null &&
                                              pipingFailureMechanism.SurfaceLines.Any() &&
                                              pipingFailureMechanism.StochasticSoilModels.Any();
        }

        private void FillAvailableSoilModelsList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (PipingCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<StochasticSoilModel> stochasticSoilModels = GetSoilModelsForCalculation(rowData.PipingCalculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilModelColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
        }

        private void FillAvailableSoilProfilesList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (PipingCalculationRow) dataGridViewRow.DataBoundItem;
            PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(rowData.PipingCalculation.InputParameters);

            IEnumerable<StochasticSoilProfile> stochasticSoilProfiles = GetSoilProfilesForCalculation(rowData.PipingCalculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilProfileColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetSoilProfilesDataSource(stochasticSoilProfiles).ToArray());
        }

        private IEnumerable<StochasticSoilModel> GetSoilModelsForCalculation(PipingCalculation pipingCalculation)
        {
            if (pipingFailureMechanism == null)
            {
                return Enumerable.Empty<StochasticSoilModel>();
            }
            return PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                pipingCalculation.InputParameters.SurfaceLine,
                pipingFailureMechanism.StochasticSoilModels);
        }

        private IEnumerable<StochasticSoilProfile> GetSoilProfilesForCalculation(PipingCalculation pipingCalculation)
        {
            if (pipingCalculation.InputParameters.StochasticSoilModel == null)
            {
                return Enumerable.Empty<StochasticSoilProfile>();
            }
            return pipingCalculation.InputParameters.StochasticSoilModel.StochasticSoilProfiles;
        }

        private static void SetItemsOnObjectCollection(DataGridViewComboBoxCell.ObjectCollection objectCollection, object[] comboBoxItems)
        {
            objectCollection.Clear();
            objectCollection.AddRange(comboBoxItems);
        }

        #region Data sources

        private void UpdateDataGridViewDataSource()
        {
            // Skip changes coming from the view itself
            if (dataGridViewControl.IsCurrentCellInEditMode)
            {
                updatingDataSource = true;

                UpdateStochasticSoilProfileColumn();
                updatingDataSource = false;

                dataGridViewControl.AutoResizeColumns();

                return;
            }

            var failureMechanismSection = listBox.SelectedItem as FailureMechanismSection;
            if (failureMechanismSection == null || calculationGroup == null)
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            var lineSegments = Math2D.ConvertLinePointsToLineSegments(failureMechanismSection.Points);
            var pipingCalculations = calculationGroup
                .GetCalculations()
                .OfType<PipingCalculationScenario>()
                .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));

            updatingDataSource = true;

            PrefillComboBoxListItemsAtColumnLevel();

            var dataSource = pipingCalculations.Select(pc => new PipingCalculationRow(pc)).ToList();
            dataGridViewControl.SetDataSource(dataSource);
            dataGridViewControl.ClearCurrentCell();

            UpdateStochasticSoilModelColumn();
            UpdateStochasticSoilProfileColumn();

            updatingDataSource = false;
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<StochasticSoilModel>> GetPrefillStochasticSoilModelsDataSource(IEnumerable<StochasticSoilModel> stochasticSoilModels)
        {
            yield return new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(null);

            foreach (StochasticSoilModel stochasticSoilModel in stochasticSoilModels)
            {
                yield return new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(stochasticSoilModel);
            }
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<StochasticSoilModel>> GetStochasticSoilModelsDataSource(IEnumerable<StochasticSoilModel> stochasticSoilModels)
        {
            var stochasticSoilModelsArray = stochasticSoilModels.ToArray();
            if (stochasticSoilModelsArray.Length != 1)
            {
                yield return new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(null);
            }
            foreach (StochasticSoilModel stochasticSoilModel in stochasticSoilModelsArray)
            {
                yield return new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(stochasticSoilModel);
            }
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<StochasticSoilProfile>> GetPrefillSoilProfilesDataSource(IEnumerable<StochasticSoilProfile> stochasticSoilProfiles)
        {
            yield return new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(null);

            foreach (StochasticSoilProfile stochasticSoilProfile in stochasticSoilProfiles)
            {
                yield return new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(stochasticSoilProfile);
            }
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<StochasticSoilProfile>> GetSoilProfilesDataSource(IEnumerable<StochasticSoilProfile> stochasticSoilProfiles)
        {
            var stochasticSoilProfilesArray = stochasticSoilProfiles.ToArray();
            if (stochasticSoilProfilesArray.Length != 1)
            {
                yield return new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(null);
            }
            foreach (StochasticSoilProfile stochasticSoilProfile in stochasticSoilProfilesArray)
            {
                yield return new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(stochasticSoilProfile);
            }
        }

        private static List<DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>> GetHydraulicBoundaryLocationsDataSource(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = null)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>>
            {
                new DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>(null)
            };

            if (hydraulicBoundaryLocations != null)
            {
                dataGridViewComboBoxItemWrappers.AddRange(hydraulicBoundaryLocations.Select(hbl => new DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>(hbl)));
            }

            return dataGridViewComboBoxItemWrappers;
        }

        #endregion

        #region Prefill combo box list items

        private void PrefillComboBoxListItemsAtColumnLevel()
        {
            DataGridViewComboBoxColumn stochasticSoilModelColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(stochasticSoilModelColumnIndex);
            DataGridViewComboBoxColumn stochasticSoilProfileColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(stochasticSoilProfileColumnIndex);
            DataGridViewComboBoxColumn hydraulicBoundaryLocationColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(hydraulicBoundaryLocationColumnIndex);

            // Need to prefill for all possible data in order to guarantee 'combo box' columns
            // do not generate errors when their cell value is not present in the list of available
            // items.
            using (new SuspendDataGridViewColumnResizes(stochasticSoilModelColumn))
            {
                var stochasticSoilModels = pipingFailureMechanism.StochasticSoilModels;
                SetItemsOnObjectCollection(stochasticSoilModelColumn.Items, GetPrefillStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
            }
            using (new SuspendDataGridViewColumnResizes(stochasticSoilProfileColumn))
            {
                var pipingSoilProfiles = GetPipingStochasticSoilProfilesFromStochasticSoilModels();
                SetItemsOnObjectCollection(stochasticSoilProfileColumn.Items, GetPrefillSoilProfilesDataSource(pipingSoilProfiles).ToArray());
            }
            using (new SuspendDataGridViewColumnResizes(hydraulicBoundaryLocationColumn))
            {
                var hydraulicBoundaryLocations = assessmentSection != null && assessmentSection.HydraulicBoundaryDatabase != null
                                                     ? assessmentSection.HydraulicBoundaryDatabase.Locations
                                                     : null;
                SetItemsOnObjectCollection(
                    hydraulicBoundaryLocationColumn.Items,
                    GetHydraulicBoundaryLocationsDataSource(hydraulicBoundaryLocations).ToArray());
            }
        }

        private StochasticSoilProfile[] GetPipingStochasticSoilProfilesFromStochasticSoilModels()
        {
            return pipingFailureMechanism != null ? pipingFailureMechanism.StochasticSoilModels
                                                                          .SelectMany(ssm => ssm.StochasticSoilProfiles)
                                                                          .Distinct()
                                                                          .ToArray() : null;
        }

        #endregion

        # region Event handling

        private void DataGridViewOnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (updatingDataSource)
            {
                return;
            }

            OnSelectionChanged();
        }

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewDataSource();
            OnSelectionChanged();
        }

        private void OnGenerateScenariosButtonClick(object sender, EventArgs e)
        {
            if (calculationGroup == null)
            {
                return;
            }
            var dialog = new PipingSurfaceLineSelectionDialog(Parent, pipingFailureMechanism.SurfaceLines);
            dialog.ShowDialog();
            var calculationsStructure = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                dialog.SelectedItems,
                pipingFailureMechanism.StochasticSoilModels,
                pipingFailureMechanism.GeneralInput);
            foreach (var item in calculationsStructure)
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
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new EventArgs());
            }
        }

        private PipingInputContext CreateSelectedItemFromCurrentRow()
        {
            var currentRow = dataGridViewControl.CurrentRow;

            var pipingCalculationRow = currentRow != null
                                           ? (PipingCalculationRow) currentRow.DataBoundItem
                                           : null;

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

        # endregion
    }
}