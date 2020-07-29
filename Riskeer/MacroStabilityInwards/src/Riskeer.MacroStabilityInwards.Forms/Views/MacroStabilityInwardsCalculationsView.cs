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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.Properties;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.MacroStabilityInwards.Service;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring macro stability inwards calculations.
    /// </summary>
    public class MacroStabilityInwardsCalculationsView : CalculationsView<MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsInput, MacroStabilityInwardsCalculationRow, MacroStabilityInwardsFailureMechanism>
    {
        private const int selectableHydraulicBoundaryLocationColumnIndex = 1;
        private const int stochasticSoilModelColumnIndex = 2;
        private const int stochasticSoilProfileColumnIndex = 3;

        private readonly RecursiveObserver<MacroStabilityInwardsSurfaceLineCollection, MacroStabilityInwardsSurfaceLine> surfaceLineObserver;
        private readonly Observer stochasticSoilModelsObserver;
        private readonly RecursiveObserver<MacroStabilityInwardsStochasticSoilModelCollection, MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfileObserver;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationsView"/>.
        /// </summary>
        public MacroStabilityInwardsCalculationsView(CalculationGroup calculationGroup, MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(calculationGroup, failureMechanism, assessmentSection)
        {
            surfaceLineObserver = new RecursiveObserver<MacroStabilityInwardsSurfaceLineCollection, MacroStabilityInwardsSurfaceLine>(() =>
            {
                UpdateColumns();
                UpdateGenerateCalculationsButtonState();
            }, rpslc => rpslc)
            {
                Observable = failureMechanism.SurfaceLines
            };

            stochasticSoilModelsObserver = new Observer(() =>
            {
                UpdateColumns();
                UpdateGenerateCalculationsButtonState();
            })
            {
                Observable = failureMechanism.StochasticSoilModels
            };
            stochasticSoilProfileObserver = new RecursiveObserver<MacroStabilityInwardsStochasticSoilModelCollection, MacroStabilityInwardsStochasticSoilProfile>(
                () => DataGridViewControl.RefreshDataGridView(),
                ssmc => ssmc.SelectMany(ssm => ssm.StochasticSoilProfiles))
            {
                Observable = failureMechanism.StochasticSoilModels
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GenerateButton.Text = RiskeerCommonFormsResources.CalculationGroup_Generate_Scenarios;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                surfaceLineObserver.Dispose();
                stochasticSoilProfileObserver.Dispose();
                stochasticSoilModelsObserver.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override object CreateSelectedItemFromCurrentRow(MacroStabilityInwardsCalculationRow currentRow)
        {
            return new MacroStabilityInwardsInputContext(
                currentRow.Calculation.InputParameters,
                currentRow.Calculation,
                FailureMechanism.SurfaceLines,
                FailureMechanism.StochasticSoilModels,
                FailureMechanism,
                AssessmentSection);
        }

        protected override IEnumerable<Point2D> GetReferenceLocations()
        {
            return FailureMechanism.SurfaceLines.Select(sl => sl.ReferenceLineIntersectionWorldPoint);
        }

        protected override bool IsCalculationIntersectionWithReferenceLineInSection(MacroStabilityInwardsCalculationScenario calculation, IEnumerable<Segment2D> lineSegments)
        {
            return calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments);
        }

        protected override MacroStabilityInwardsCalculationRow CreateRow(MacroStabilityInwardsCalculationScenario calculation)
        {
            return new MacroStabilityInwardsCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, calculation.InputParameters));
        }

        protected override bool CanGenerateCalculations()
        {
            return FailureMechanism.SurfaceLines.Any() && FailureMechanism.StochasticSoilModels.Any();
        }

        protected override void GenerateCalculations()
        {
            var calculationGroup = (CalculationGroup) Data;

            var dialog = new MacroStabilityInwardsSurfaceLineSelectionDialog(Parent, FailureMechanism.SurfaceLines);
            dialog.ShowDialog();
            IEnumerable<ICalculationBase> calculationsStructure = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                dialog.SelectedItems,
                FailureMechanism.StochasticSoilModels);
            foreach (ICalculationBase item in calculationsStructure)
            {
                calculationGroup.Children.Add(item);
            }

            calculationGroup.NotifyObservers();
        }

        protected override void InitializeDataGridView()
        {
            DataGridViewControl.CellFormatting += OnCellFormatting;

            base.InitializeDataGridView();

            DataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>>(
                nameof(MacroStabilityInwardsCalculationRow.StochasticSoilModel),
                Resources.MacroStabilityInwardsInput_StochasticSoilModel_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>.This),
                nameof(DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>.DisplayName));

            DataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>>(
                nameof(MacroStabilityInwardsCalculationRow.StochasticSoilProfile),
                Resources.MacroStabilityInwardsInput_StochasticSoilProfile_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>.This),
                nameof(DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsCalculationRow.StochasticSoilProfileProbability),
                Resources.MacroStabilityInwardsCalculationsView_InitializeDataGridView_Stochastic_soil_profile_probability);

            UpdateStochasticSoilModelColumn();
            UpdateStochasticSoilProfileColumn();
        }

        protected override void UpdateColumns()
        {
            base.UpdateColumns();
            UpdateStochasticSoilModelColumn();
            UpdateStochasticSoilProfileColumn();
        }

        #region Event handling

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex == selectableHydraulicBoundaryLocationColumnIndex)
            {
                DataGridViewRow dataGridViewRow = DataGridViewControl.GetRowFromIndex(eventArgs.RowIndex);
                dataGridViewRow.Cells[selectableHydraulicBoundaryLocationColumnIndex].ReadOnly = dataGridViewRow.DataBoundItem is MacroStabilityInwardsCalculationRow dataItem
                                                                                                 && dataItem.Calculation.InputParameters.UseAssessmentLevelManualInput;
            }
        }

        #endregion

        #region Data sources

        private static IEnumerable<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>> GetStochasticSoilModelsDataSource(
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>>
            {
                new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>(null)
            };

            dataGridViewComboBoxItemWrappers.AddRange(stochasticSoilModels.Select(stochasticSoilModel => new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>(stochasticSoilModel)));

            return dataGridViewComboBoxItemWrappers.ToArray();
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>> GetSoilProfilesDataSource(
            IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>>
            {
                new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>(null)
            };

            dataGridViewComboBoxItemWrappers.AddRange(stochasticSoilProfiles.Select(stochasticSoilProfile => new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>(stochasticSoilProfile)));

            return dataGridViewComboBoxItemWrappers.ToArray();
        }

        #endregion

        #region Update combo box list items

        #region Update Stochastic Soil Model Column

        private void UpdateStochasticSoilModelColumn()
        {
            using (new SuspendDataGridViewColumnResizes(DataGridViewControl.GetColumnFromIndex(stochasticSoilModelColumnIndex)))
            {
                foreach (DataGridViewRow dataGridViewRow in DataGridViewControl.Rows)
                {
                    FillAvailableSoilModelsList(dataGridViewRow);
                }
            }
        }

        private void FillAvailableSoilModelsList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (MacroStabilityInwardsCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels = GetSoilModelsForCalculation(rowData.Calculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilModelColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
        }

        private IEnumerable<MacroStabilityInwardsStochasticSoilModel> GetSoilModelsForCalculation(MacroStabilityInwardsCalculation calculation)
        {
            return MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                calculation.InputParameters.SurfaceLine,
                FailureMechanism.StochasticSoilModels);
        }

        #endregion

        #region Update Stochastic Soil Profile Column

        private void UpdateStochasticSoilProfileColumn()
        {
            using (new SuspendDataGridViewColumnResizes(DataGridViewControl.GetColumnFromIndex(stochasticSoilProfileColumnIndex)))
            {
                foreach (DataGridViewRow dataGridViewRow in DataGridViewControl.Rows)
                {
                    FillAvailableSoilProfilesList(dataGridViewRow);
                }
            }
        }

        private static void FillAvailableSoilProfilesList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (MacroStabilityInwardsCalculationRow) dataGridViewRow.DataBoundItem;
            MacroStabilityInwardsInputService.SyncStochasticSoilProfileWithStochasticSoilModel(rowData.Calculation.InputParameters);

            IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles = GetSoilProfilesForCalculation(rowData.Calculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilProfileColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetSoilProfilesDataSource(stochasticSoilProfiles).ToArray());
        }

        private static IEnumerable<MacroStabilityInwardsStochasticSoilProfile> GetSoilProfilesForCalculation(MacroStabilityInwardsCalculation calculation)
        {
            return calculation.InputParameters.StochasticSoilModel != null
                       ? calculation.InputParameters.StochasticSoilModel.StochasticSoilProfiles
                       : Enumerable.Empty<MacroStabilityInwardsStochasticSoilProfile>();
        }

        #endregion

        #endregion

        #region Prefill combo box list items

        protected override void PrefillComboBoxListItemsAtColumnLevel()
        {
            base.PrefillComboBoxListItemsAtColumnLevel();

            var stochasticSoilModelColumn = (DataGridViewComboBoxColumn) DataGridViewControl.GetColumnFromIndex(stochasticSoilModelColumnIndex);
            var stochasticSoilProfileColumn = (DataGridViewComboBoxColumn) DataGridViewControl.GetColumnFromIndex(stochasticSoilProfileColumnIndex);

            // Need to prefill for all possible data in order to guarantee 'combo box' columns
            // do not generate errors when their cell value is not present in the list of available
            // items.
            using (new SuspendDataGridViewColumnResizes(stochasticSoilModelColumn))
            {
                MacroStabilityInwardsStochasticSoilModelCollection stochasticSoilModels = FailureMechanism.StochasticSoilModels;
                SetItemsOnObjectCollection(stochasticSoilModelColumn.Items, GetStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
            }

            using (new SuspendDataGridViewColumnResizes(stochasticSoilProfileColumn))
            {
                IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles = GetStochasticSoilProfilesFromStochasticSoilModels();
                SetItemsOnObjectCollection(stochasticSoilProfileColumn.Items, GetSoilProfilesDataSource(stochasticSoilProfiles).ToArray());
            }
        }

        private IEnumerable<MacroStabilityInwardsStochasticSoilProfile> GetStochasticSoilProfilesFromStochasticSoilModels()
        {
            return FailureMechanism.StochasticSoilModels
                                   .SelectMany(ssm => ssm.StochasticSoilProfiles)
                                   .Distinct();
        }

        #endregion
    }
}