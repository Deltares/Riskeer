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
using Core.Common.Util;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring closing structures calculations.
    /// </summary>
    public partial class StabilityPointStructuresCalculationsView : UserControl, ISelectionProvider, IView
    {
        private const int selectableHydraulicBoundaryLocationColumnIndex = 1;
        private const int selectableForeshoreProfileColumnIndex = 2;
        private readonly IAssessmentSection assessmentSection;
        private readonly StabilityPointStructuresFailureMechanism failureMechanism;
        private Observer failureMechanismObserver;
        private Observer hydraulicBoundaryLocationsObserver;
        private Observer stabilityPointStructuresObserver;
        private RecursiveObserver<CalculationGroup, StabilityPointStructuresInput> inputObserver;
        private RecursiveObserver<CalculationGroup, StructuresCalculationScenario<StabilityPointStructuresInput>> calculationScenarioObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;

        private CalculationGroup calculationGroup;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationsView"/>.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationsView(CalculationGroup data, StabilityPointStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;
            calculationGroup = data;

            InitializeObservers();

            InitializeComponent();
            InitializeListBox();
            InitializeDataGridView();

            UpdateSectionsListBox();
            UpdateDataGridViewDataSource();
            UpdateSelectableHydraulicBoundaryLocationsColumn();
            UpdateForeshoreProfilesColumn();
            UpdateGenerateCalculationsButtonState();
        }

        public object Selection => CreateSelectedItemFromCurrentRow();

        public object Data
        {
            get => calculationGroup;
            set => calculationGroup = value as CalculationGroup;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Necessary to correctly load the content of the dropdown lists of the comboboxes...
            UpdateDataGridViewDataSource();

            base.OnLoad(e);

            dataGridViewControl.CellFormatting += HandleCellStyling;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                failureMechanismObserver.Dispose();
                inputObserver.Dispose();
                calculationScenarioObserver.Dispose();
                calculationGroupObserver.Dispose();

                hydraulicBoundaryLocationsObserver.Dispose();
                stabilityPointStructuresObserver.Dispose();

                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.CurrentRowChanged += DataGridViewOnCurrentRowChangedHandler;

            dataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.Name),
                RiskeerCommonFormsResources.FailureMechanism_Name_DisplayName);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>>(
                nameof(StabilityPointStructuresCalculationRow.SelectableHydraulicBoundaryLocation),
                RiskeerCommonFormsResources.HydraulicBoundaryLocation_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.This),
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.DisplayName));

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<ForeshoreProfile>>(
                nameof(StabilityPointStructuresCalculationRow.ForeshoreProfile),
                RiskeerCommonFormsResources.Structure_ForeshoreProfile_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<ForeshoreProfile>.This),
                nameof(DataGridViewComboBoxItemWrapper<ForeshoreProfile>.DisplayName));

            dataGridViewControl.AddCheckBoxColumn(nameof(StabilityPointStructuresCalculationRow.UseBreakWater),
                                                  RiskeerCommonFormsResources.Use_BreakWater_DisplayName);

            dataGridViewControl.AddComboBoxColumn(nameof(StabilityPointStructuresCalculationRow.BreakWaterType),
                                                  RiskeerCommonFormsResources.CalculationsView_BreakWaterType_DisplayName,
                                                  EnumDisplayWrapperHelper.GetEnumTypes<BreakWaterType>(),
                                                  nameof(EnumDisplayWrapper<BreakWaterType>.Value),
                                                  nameof(EnumDisplayWrapper<BreakWaterType>.DisplayName));

            dataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.BreakWaterHeight),
                RiskeerCommonFormsResources.CalculationsView_BreakWaterHeight_DisplayName);

            dataGridViewControl.AddCheckBoxColumn(nameof(StabilityPointStructuresCalculationRow.UseForeshoreGeometry),
                                                  RiskeerCommonFormsResources.Use_Foreshore_DisplayName);

            dataGridViewControl.AddComboBoxColumn(nameof(StabilityPointStructuresCalculationRow.LoadSchematizationType),
                                                  RiskeerCommonFormsResources.LoadSchematizationType_DisplayName,
                                                  EnumDisplayWrapperHelper.GetEnumTypes<LoadSchematizationType>(),
                                                  nameof(EnumDisplayWrapper<LoadSchematizationType>.Value),
                                                  nameof(EnumDisplayWrapper<LoadSchematizationType>.DisplayName));

            dataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.ConstructiveStrengthLinearLoadModel),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{RiskeerCommonFormsResources.ConstructiveStrength_Linear_LoadModel_DisplayName}");

            dataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.ConstructiveStrengthQuadraticLoadModel),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{RiskeerCommonFormsResources.ConstructiveStrength_Quadratic_LoadModel_DisplayName}");

            dataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.StabilityLinearLoadModel),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{RiskeerCommonFormsResources.Stability_Linear_LoadModel_DisplayName}");

            dataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.StabilityQuadraticLoadModel),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{RiskeerCommonFormsResources.Stability_Quadratic_LoadModel_DisplayName}");

            dataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.EvaluationLevel),
                RiskeerCommonFormsResources.Evaluation_Level_DisplayName);
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = nameof(FailureMechanismSection.Name);
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void UpdateGenerateCalculationsButtonState()
        {
            buttonGenerateCalculations.Enabled = failureMechanism.StabilityPointStructures.Any();
        }

        private StabilityPointStructuresInputContext CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;

            var calculationRow = (StabilityPointStructuresCalculationRow) currentRow?.DataBoundItem;

            StabilityPointStructuresInputContext selection = null;
            if (calculationRow != null)
            {
                selection = new StabilityPointStructuresInputContext(
                    calculationRow.CalculationScenario.InputParameters,
                    calculationRow.CalculationScenario,
                    failureMechanism,
                    assessmentSection);
            }

            return selection;
        }

        private static void SetItemsOnObjectCollection(DataGridViewComboBoxCell.ObjectCollection objectCollection, object[] comboBoxItems)
        {
            objectCollection.Clear();
            objectCollection.AddRange(comboBoxItems);
        }

        private static DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>[] GetSelectableHydraulicBoundaryLocationsDataSource(
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

            return dataGridViewComboBoxItemWrappers.ToArray();
        }

        private void InitializeObservers()
        {
            failureMechanismObserver = new Observer(OnFailureMechanismUpdate)
            {
                Observable = failureMechanism
            };
            hydraulicBoundaryLocationsObserver = new Observer(UpdateSelectableHydraulicBoundaryLocationsColumn)
            {
                Observable = assessmentSection.HydraulicBoundaryDatabase.Locations
            };
            stabilityPointStructuresObserver = new Observer(UpdateGenerateCalculationsButtonState)
            {
                Observable = failureMechanism.StabilityPointStructures
            };

            // The concat is needed to observe the input of calculations in child groups.
            inputObserver = new RecursiveObserver<CalculationGroup, StabilityPointStructuresInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<StructuresCalculationScenario<StabilityPointStructuresInput>>().Select(pc => pc.InputParameters)))
            {
                Observable = calculationGroup
            };
            calculationScenarioObserver = new RecursiveObserver<CalculationGroup, StructuresCalculationScenario<StabilityPointStructuresInput>>(() => dataGridViewControl.RefreshDataGridView(), pcg => pcg.Children)
            {
                Observable = calculationGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children)
            {
                Observable = calculationGroup
            };
        }

        #region Data sources

        private void UpdateDataGridViewDataSource()
        {
            // Skip changes coming from the view itself
            if (dataGridViewControl.IsCurrentCellInEditMode)
            {
                dataGridViewControl.AutoResizeColumns();
            }

            if (!(listBox.SelectedItem is FailureMechanismSection failureMechanismSection))
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(failureMechanismSection.Points);
            IEnumerable<StructuresCalculationScenario<StabilityPointStructuresInput>> calculationScenarios = calculationGroup
                                                                                                     .GetCalculations()
                                                                                                     .OfType<StructuresCalculationScenario<StabilityPointStructuresInput>>()
                                                                                                     .Where(cs => cs.IsStructureIntersectionWithReferenceLineInSection(lineSegments));

            PrefillComboBoxListItemsAtColumnLevel();

            List<StabilityPointStructuresCalculationRow> dataSource = calculationScenarios.Select(cs => new StabilityPointStructuresCalculationRow(cs, new ObservablePropertyChangeHandler(cs, cs.InputParameters))).ToList();
            dataGridViewControl.SetDataSource(dataSource);
            dataGridViewControl.ClearCurrentCell();

            UpdateSelectableHydraulicBoundaryLocationsColumn();
            UpdateForeshoreProfilesColumn();
        }

        #endregion

        #region Update combo box list items

        #region Update SelectableHydraulicBoundaryLocations

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
            var rowData = (StabilityPointStructuresCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<SelectableHydraulicBoundaryLocation> locations = GetSelectableHydraulicBoundaryLocationsForCalculation(rowData.CalculationScenario);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[selectableHydraulicBoundaryLocationColumnIndex];
            DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>[] dataGridViewComboBoxItemWrappers = GetSelectableHydraulicBoundaryLocationsDataSource(locations);
            SetItemsOnObjectCollection(cell.Items, dataGridViewComboBoxItemWrappers);
        }

        private IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocationsForCalculation(StructuresCalculationScenario<StabilityPointStructuresInput> calculationScenario)
        {
            return GetSelectableHydraulicBoundaryLocations(assessmentSection?.HydraulicBoundaryDatabase.Locations,
                                                           calculationScenario.InputParameters.Structure);
        }

        private static IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations(
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, StabilityPointStructure stabilityPointStructure)
        {
            Point2D referencePoint = stabilityPointStructure?.Location;
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                hydraulicBoundaryLocations, referencePoint);
        }

        #endregion

        #region Update ForeshoreProfiles

        private void UpdateForeshoreProfilesColumn()
        {
            var column = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(selectableForeshoreProfileColumnIndex);

            using (new SuspendDataGridViewColumnResizes(column))
            {
                foreach (DataGridViewRow dataGridViewRow in dataGridViewControl.Rows)
                {
                    FillAvailableForeshoreProfilesList(dataGridViewRow);
                }
            }
        }

        private void FillAvailableForeshoreProfilesList(DataGridViewRow dataGridViewRow)
        {
            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[selectableForeshoreProfileColumnIndex];
            DataGridViewComboBoxItemWrapper<ForeshoreProfile>[] dataGridViewComboBoxItemWrappers = GetSelectableForeshoreProfileDataSource(failureMechanism.ForeshoreProfiles);
            SetItemsOnObjectCollection(cell.Items, dataGridViewComboBoxItemWrappers);
        }

        private static DataGridViewComboBoxItemWrapper<ForeshoreProfile>[] GetSelectableForeshoreProfileDataSource(IEnumerable<ForeshoreProfile> selectableForeshoreProfiles = null)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<ForeshoreProfile>>
            {
                new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(null)
            };

            if (selectableForeshoreProfiles != null)
            {
                dataGridViewComboBoxItemWrappers.AddRange(selectableForeshoreProfiles.Select(fp => new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(fp)));
            }

            return dataGridViewComboBoxItemWrappers.ToArray();
        }

        #endregion

        #endregion

        #region Prefill combo box list items

        private void PrefillComboBoxListItemsAtColumnLevel()
        {
            var selectableHydraulicBoundaryLocationColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(selectableHydraulicBoundaryLocationColumnIndex);

            // Need to prefill for all possible data in order to guarantee 'combo box' columns
            // do not generate errors when their cell value is not present in the list of available
            // items.
            using (new SuspendDataGridViewColumnResizes(selectableHydraulicBoundaryLocationColumn))
            {
                SetItemsOnObjectCollection(selectableHydraulicBoundaryLocationColumn.Items,
                                           GetSelectableHydraulicBoundaryLocationsDataSource(GetSelectableHydraulicBoundaryLocationsFromFailureMechanism()));
            }

            var selectableForeshoreProfiles = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(selectableForeshoreProfileColumnIndex);

            using (new SuspendDataGridViewColumnResizes(selectableForeshoreProfiles))
            {
                SetItemsOnObjectCollection(selectableForeshoreProfiles.Items,
                                           GetSelectableForeshoreProfileDataSource(failureMechanism.ForeshoreProfiles));
            }
        }

        private IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocationsFromFailureMechanism()
        {
            List<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;

            List<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations = hydraulicBoundaryLocations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, null)).ToList();

            foreach (StabilityPointStructure stabilityPointStructure in failureMechanism.StabilityPointStructures)
            {
                selectableHydraulicBoundaryLocations.AddRange(GetSelectableHydraulicBoundaryLocations(hydraulicBoundaryLocations, stabilityPointStructure));
            }

            return selectableHydraulicBoundaryLocations;
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

        private void OnGenerateCalculationsButtonClick(object sender, EventArgs e)
        {
            using (var dialog = new StructureSelectionDialog(Parent, failureMechanism.StabilityPointStructures))
            {
                dialog.ShowDialog();
                StructureCalculationConfigurationHelper.GenerateCalculations<StabilityPointStructure, StabilityPointStructuresInput>(calculationGroup, dialog.SelectedItems.Cast<StabilityPointStructure>());
            }

            calculationGroup.NotifyObservers();
        }

        private void OnFailureMechanismUpdate()
        {
            UpdateSectionsListBox();
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (failureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(failureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = failureMechanism.Sections.First();
            }
        }

        private void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void HandleCellStyling(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridViewControl.FormatCellWithColumnStateDefinition(e.RowIndex, e.ColumnIndex);
        }

        #endregion
    }
}