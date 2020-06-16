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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.IO.DikeProfiles;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring grass cover erosion inwards calculations.
    /// </summary>
    public partial class GrassCoverErosionInwardsCalculationsView : UserControl, ISelectionProvider, IView
    {
        private const int selectableHydraulicBoundaryLocationColumnIndex = 1;
        private readonly Observer grassCoverErosionInwardsFailureMechanismObserver;
        private readonly Observer hydraulicBoundaryLocationsObserver;
        private readonly RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput> grassCoverErosionInwardsInputObserver;
        private readonly RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculationScenario> grassCoverErosionInwardsCalculationScenarioObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> grassCoverErosionInwardsCalculationGroupObserver;

        private CalculationGroup calculationGroup;
        private IAssessmentSection assessmentSection;
        private GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationsView"/>.
        /// </summary>
        public GrassCoverErosionInwardsCalculationsView()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeListBox();

            grassCoverErosionInwardsFailureMechanismObserver = new Observer(OnGrassCoverErosionInwardsFailureMechanismUpdate);
            hydraulicBoundaryLocationsObserver = new Observer(UpdateSelectableHydraulicBoundaryLocationsColumn);

            // The concat is needed to observe the input of calculations in child groups.
            grassCoverErosionInwardsInputObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<GrassCoverErosionInwardsCalculationScenario>().Select(pc => pc.InputParameters)));
            grassCoverErosionInwardsCalculationScenarioObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculationScenario>(() => dataGridViewControl.RefreshDataGridView(), pcg => pcg.Children);
            grassCoverErosionInwardsCalculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children);
        }

        /// <summary>
        /// Gets or sets the grass cover erosion inwards failure mechanism.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism GrassCoverErosionInwardsFailureMechanism
        {
            get
            {
                return grassCoverErosionInwardsFailureMechanism;
            }
            set
            {
                grassCoverErosionInwardsFailureMechanism = value;
                grassCoverErosionInwardsFailureMechanismObserver.Observable = grassCoverErosionInwardsFailureMechanism;

                UpdateSectionsListBox();

                UpdateGenerateCalculationsButtonState();
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
            }
        }

        public object Selection { get; }

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
                    grassCoverErosionInwardsInputObserver.Observable = calculationGroup;
                    grassCoverErosionInwardsCalculationScenarioObserver.Observable = calculationGroup;
                    grassCoverErosionInwardsCalculationGroupObserver.Observable = calculationGroup;
                }
                else
                {
                    dataGridViewControl.SetDataSource(null);
                    grassCoverErosionInwardsInputObserver.Observable = null;
                    grassCoverErosionInwardsCalculationScenarioObserver.Observable = null;
                    grassCoverErosionInwardsCalculationGroupObserver.Observable = null;
                }
            }
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = nameof(FailureMechanismSection.Name);
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.Name),
                Resources.GrassCoverErosionInwardsCalculation_Name_DisplayName);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>>(
                nameof(GrassCoverErosionInwardsCalculationRow.SelectableHydraulicBoundaryLocation),
                RiskeerCommonFormsResources.HydraulicBoundaryLocation_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.This),
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.DisplayName));

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<DikeProfile>>(
                nameof(GrassCoverErosionInwardsCalculationRow.DikeProfile),
                Resources.DikeProfile_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<DikeProfile>.This),
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.DisplayName));

            dataGridViewControl.AddCheckBoxColumn(nameof(GrassCoverErosionInwardsCalculationRow.UseDam), Resources.GrassCoverErosionInwardsCalculation_Use_Dam);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<DamType>>(
                nameof(GrassCoverErosionInwardsCalculationRow.DamType),
                Resources.GrassCoverErosionInwardsCalculation_Damtype,
                null,
                nameof(DataGridViewComboBoxItemWrapper<DikeProfile>.This),
                nameof(DataGridViewComboBoxItemWrapper<DamType>.DisplayName));

            dataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.Damheight),
                Resources.GrassCoverErosionInwardsCalculation_Damheight);

            dataGridViewControl.AddCheckBoxColumn(nameof(GrassCoverErosionInwardsCalculationRow.UseForeShoreGeometry), Resources.GrassCoverErosionInwardsCalculation_Use_ForeShoreGeometry);

            dataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.DikeHeight),
                Resources.GrassCoverErosionInwardsCalculation_Dikeheight);

            dataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.ExpectedCriticalOvertoppingRate),
                Resources.GrassCoverErosionInwardsCalculation_Expected_Critical_OvertoppingRate);

            dataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.StandardDeviationCriticalOvertoppingRate),
                Resources.GrassCoverErosionInwardsCalculation_StandardDeviation_Critical_OvertoppingRate);
        }

        private void UpdateGenerateCalculationsButtonState()
        {
            buttonGenerateCalculations.Enabled = grassCoverErosionInwardsFailureMechanism != null;
        }

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
            var rowData = (GrassCoverErosionInwardsCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<SelectableHydraulicBoundaryLocation> locations = GetSelectableHydraulicBoundaryLocationsForCalculation(rowData.GrassCoverErosionInwardsCalculationScenario);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[selectableHydraulicBoundaryLocationColumnIndex];
            DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>[] dataGridViewComboBoxItemWrappers = GetSelectableHydraulicBoundaryLocationsDataSource(locations).ToArray();
            SetItemsOnObjectCollection(cell.Items, dataGridViewComboBoxItemWrappers);
        }

        private IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocationsForCalculation(GrassCoverErosionInwardsCalculationScenario grassCoverErosionInwardsCalculationScenario)
        {
            return GetSelectableHydraulicBoundaryLocations(assessmentSection?.HydraulicBoundaryDatabase.Locations,
                                                           grassCoverErosionInwardsCalculationScenario.InputParameters.DikeProfile);
        }

        private static IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations(
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, DikeProfile dikeProfile)
        {
            Point2D referencePoint = dikeProfile?.WorldReferencePoint;
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                hydraulicBoundaryLocations, referencePoint);
        }

        private static void SetItemsOnObjectCollection(DataGridViewComboBoxCell.ObjectCollection objectCollection, object[] comboBoxItems)
        {
            objectCollection.Clear();
            objectCollection.AddRange(comboBoxItems);
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

        #region Data sources

        private void UpdateDataGridViewDataSource()
        {
            // Skip changes coming from the view itself
            if (dataGridViewControl.IsCurrentCellInEditMode)
            {
                dataGridViewControl.AutoResizeColumns();
            }

            var failureMechanismSection = listBox.SelectedItem as FailureMechanismSection;
            if (failureMechanismSection == null || calculationGroup == null)
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            IEnumerable<GrassCoverErosionInwardsCalculationScenario> grassCoverErosionInwardsCalculationScenarios = calculationGroup
                                                                                                                    .GetCalculations()
                                                                                                                    .OfType<GrassCoverErosionInwardsCalculationScenario>();

            PrefillComboBoxListItemsAtColumnLevel();

            List<GrassCoverErosionInwardsCalculationRow> dataSource = grassCoverErosionInwardsCalculationScenarios.Select(pc => new GrassCoverErosionInwardsCalculationRow(pc, new ObservablePropertyChangeHandler(pc, pc.InputParameters))).ToList();
            dataGridViewControl.SetDataSource(dataSource);
            dataGridViewControl.ClearCurrentCell();

            UpdateSelectableHydraulicBoundaryLocationsColumn();
        }

        #endregion

        #region Prefill combo box list items

        private void PrefillComboBoxListItemsAtColumnLevel()
        {
            var selectableHydraulicBoundaryLocationColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(selectableHydraulicBoundaryLocationColumnIndex);

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

            return hydraulicBoundaryLocations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, null)).ToList();
        }

        #endregion

        #region Event handling

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewDataSource();
        }

        private void OnGrassCoverErosionInwardsFailureMechanismUpdate()
        {
            UpdateSectionsListBox();
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (grassCoverErosionInwardsFailureMechanism != null && grassCoverErosionInwardsFailureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(grassCoverErosionInwardsFailureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = grassCoverErosionInwardsFailureMechanism.Sections.First();
            }
        }

        #endregion
    }
}