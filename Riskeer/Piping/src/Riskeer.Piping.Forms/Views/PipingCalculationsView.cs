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
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.PresentationObjects.SemiProbabilistic;
using Riskeer.Piping.Forms.Properties;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring piping calculation scenarios.
    /// </summary>
    public class PipingCalculationsView : CalculationsView<IPipingCalculationScenario<PipingInput>, PipingInput, PipingCalculationRow, PipingFailureMechanism>
    {
        private const int selectableHydraulicBoundaryLocationColumnIndex = 1;
        private const int stochasticSoilModelColumnIndex = 2;
        private const int stochasticSoilProfileColumnIndex = 3;

        private RecursiveObserver<PipingSurfaceLineCollection, PipingSurfaceLine> surfaceLineObserver;
        private Observer stochasticSoilModelsObserver;
        private RecursiveObserver<PipingStochasticSoilModelCollection, PipingStochasticSoilProfile> stochasticSoilProfileObserver;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationsView"/>.
        /// </summary>
        /// <param name="calculationGroup">Calculation group containing all calculation scenarios of the failure mechanism.</param>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> the calculation scenarios belong to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the calculation scenarios belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingCalculationsView(CalculationGroup calculationGroup, PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(calculationGroup, failureMechanism, assessmentSection) {}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GenerateButton.Text = RiskeerCommonFormsResources.CalculationGroup_Generate_Scenarios;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Loaded)
            {
                surfaceLineObserver.Dispose();
                stochasticSoilProfileObserver.Dispose();
                stochasticSoilModelsObserver.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override object CreateSelectedItemFromCurrentRow(PipingCalculationRow currentRow)
        {
            switch (currentRow.Calculation)
            {
                case SemiProbabilisticPipingCalculationScenario semiProbabilisticPipingCalculationScenario:
                    return new SemiProbabilisticPipingInputContext(
                        semiProbabilisticPipingCalculationScenario.InputParameters,
                        semiProbabilisticPipingCalculationScenario,
                        FailureMechanism.SurfaceLines,
                        FailureMechanism.StochasticSoilModels,
                        FailureMechanism,
                        AssessmentSection);
                case ProbabilisticPipingCalculationScenario probabilisticPipingCalculationScenario:
                    return new ProbabilisticPipingInputContext(
                        probabilisticPipingCalculationScenario.InputParameters,
                        probabilisticPipingCalculationScenario,
                        FailureMechanism.SurfaceLines,
                        FailureMechanism.StochasticSoilModels,
                        FailureMechanism,
                        AssessmentSection);
                default:
                    return null;
            }
        }

        protected override IEnumerable<Point2D> GetReferenceLocations()
        {
            return FailureMechanism.SurfaceLines.Select(sl => sl.ReferenceLineIntersectionWorldPoint);
        }

        protected override bool IsCalculationIntersectionWithReferenceLineInSection(IPipingCalculationScenario<PipingInput> calculation, IEnumerable<Segment2D> lineSegments)
        {
            return calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments);
        }

        protected override PipingCalculationRow CreateRow(IPipingCalculationScenario<PipingInput> calculation)
        {
            var assessmentDescription = string.Empty;

            switch (calculation)
            {
                case SemiProbabilisticPipingCalculationScenario _:
                    assessmentDescription = "Semi-probabilistisch";
                    break;
                case ProbabilisticPipingCalculationScenario _:
                    assessmentDescription = "Probabilistisch";
                    break;
            }

            return new PipingCalculationRow(calculation, assessmentDescription, new ObservablePropertyChangeHandler(calculation, calculation.InputParameters));
        }

        protected override bool CanGenerateCalculations()
        {
            return FailureMechanism.SurfaceLines.Any() && FailureMechanism.StochasticSoilModels.Any();
        }

        protected override void GenerateCalculations()
        {
            var calculationGroup = (CalculationGroup) Data;

            using (var dialog = new PipingSurfaceLineSelectionDialog(Parent, FailureMechanism.SurfaceLines))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    IEnumerable<ICalculationBase> calculationsStructure = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                        dialog.SelectedItems,
                        dialog.GenerateSemiProbabilistic,
                        dialog.GenerateProbabilistic,
                        FailureMechanism.StochasticSoilModels);

                    foreach (ICalculationBase item in calculationsStructure)
                    {
                        calculationGroup.Children.Add(item);
                    }

                    calculationGroup.NotifyObservers();
                }
            }
        }

        protected override void InitializeDataGridView()
        {
            DataGridViewControl.CellFormatting += OnCellFormatting;

            base.InitializeDataGridView();
        }

        protected override void AddColumns(Action addNameColumn, Action addHydraulicBoundaryLocationColumn)
        {
            addNameColumn();

            DataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.AssessmentDescription),
                Resources.PipingCalculationsView_InitializeDataGridView_Assessment_description);

            addHydraulicBoundaryLocationColumn();

            DataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>>(
                nameof(PipingCalculationRow.StochasticSoilModel),
                Resources.PipingInput_StochasticSoilModel_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>.This),
                nameof(DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>.DisplayName));

            DataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>>(
                nameof(PipingCalculationRow.StochasticSoilProfile),
                Resources.PipingInput_StochasticSoilProfile_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>.This),
                nameof(DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.StochasticSoilProfileProbability),
                Resources.PipingCalculationsView_InitializeDataGridView_Stochastic_soil_profile_probability);

            DataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.DampingFactorExitMean),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{Resources.PipingCalculationsView_DampingFactorExit_DisplayName}");

            DataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.PhreaticLevelExitMean),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{Resources.PipingCalculationsView_PhreaticLevelExit_DisplayName}");

            DataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.EntryPointL),
                Resources.PipingInput_EntryPointL_DisplayName);

            DataGridViewControl.AddTextBoxColumn(
                nameof(PipingCalculationRow.ExitPointL),
                Resources.PipingInput_ExitPointL_DisplayName);
        }

        protected override void InitializeObservers()
        {
            base.InitializeObservers();

            surfaceLineObserver = new RecursiveObserver<PipingSurfaceLineCollection, PipingSurfaceLine>(UpdateGenerateCalculationsButtonState, rpslc => rpslc)
            {
                Observable = FailureMechanism.SurfaceLines
            };

            stochasticSoilModelsObserver = new Observer(() =>
            {
                PrefillComboBoxListItemsAtColumnLevel();
                UpdateComboBoxColumns();
                UpdateGenerateCalculationsButtonState();
            })
            {
                Observable = FailureMechanism.StochasticSoilModels
            };
            stochasticSoilProfileObserver = new RecursiveObserver<PipingStochasticSoilModelCollection, PipingStochasticSoilProfile>(
                () => DataGridViewControl.RefreshDataGridView(),
                ssmc => ssmc.SelectMany(ssm => ssm.StochasticSoilProfiles))
            {
                Observable = FailureMechanism.StochasticSoilModels
            };
        }

        protected override void UpdateComboBoxColumns()
        {
            base.UpdateComboBoxColumns();
            UpdateStochasticSoilModelColumn();
            UpdateStochasticSoilProfileColumn();
        }

        #region Event handling

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex == selectableHydraulicBoundaryLocationColumnIndex)
            {
                DataGridViewRow dataGridViewRow = DataGridViewControl.GetRowFromIndex(eventArgs.RowIndex);
                dataGridViewRow.Cells[selectableHydraulicBoundaryLocationColumnIndex].ReadOnly = dataGridViewRow.DataBoundItem is PipingCalculationRow dataItem
                                                                                                 && dataItem.Calculation is SemiProbabilisticPipingCalculationScenario semiProbabilisticPipingCalculationScenario
                                                                                                 && semiProbabilisticPipingCalculationScenario.InputParameters.UseAssessmentLevelManualInput;
            }
        }

        #endregion

        #region Data sources

        private static IEnumerable<DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>> GetStochasticSoilModelsDataSource(
            IEnumerable<PipingStochasticSoilModel> stochasticSoilModels)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>>
            {
                new DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>(null)
            };

            dataGridViewComboBoxItemWrappers.AddRange(stochasticSoilModels.Select(stochasticSoilModel => new DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>(stochasticSoilModel)));

            return dataGridViewComboBoxItemWrappers.ToArray();
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>> GetSoilProfilesDataSource(
            IEnumerable<PipingStochasticSoilProfile> stochasticSoilProfiles)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>>
            {
                new DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>(null)
            };

            dataGridViewComboBoxItemWrappers.AddRange(stochasticSoilProfiles.Select(stochasticSoilProfile => new DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>(stochasticSoilProfile)));

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
            var rowData = (PipingCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<PipingStochasticSoilModel> stochasticSoilModels = GetSoilModelsForCalculation(rowData.Calculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilModelColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
        }

        private IEnumerable<PipingStochasticSoilModel> GetSoilModelsForCalculation(ICalculation<PipingInput> pipingCalculation)
        {
            return PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                pipingCalculation.InputParameters.SurfaceLine,
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
            var rowData = (PipingCalculationRow) dataGridViewRow.DataBoundItem;
            PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(rowData.Calculation.InputParameters);

            IEnumerable<PipingStochasticSoilProfile> stochasticSoilProfiles = GetSoilProfilesForCalculation(rowData.Calculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilProfileColumnIndex];
            SetItemsOnObjectCollection(cell.Items, GetSoilProfilesDataSource(stochasticSoilProfiles).ToArray());
        }

        private static IEnumerable<PipingStochasticSoilProfile> GetSoilProfilesForCalculation(ICalculation<PipingInput> pipingCalculation)
        {
            return pipingCalculation.InputParameters.StochasticSoilModel != null
                       ? pipingCalculation.InputParameters.StochasticSoilModel.StochasticSoilProfiles
                       : Enumerable.Empty<PipingStochasticSoilProfile>();
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
                PipingStochasticSoilModelCollection stochasticSoilModels = FailureMechanism.StochasticSoilModels;
                SetItemsOnObjectCollection(stochasticSoilModelColumn.Items, GetStochasticSoilModelsDataSource(stochasticSoilModels).ToArray());
            }

            using (new SuspendDataGridViewColumnResizes(stochasticSoilProfileColumn))
            {
                IEnumerable<PipingStochasticSoilProfile> pipingSoilProfiles = GetPipingStochasticSoilProfilesFromStochasticSoilModels();
                SetItemsOnObjectCollection(stochasticSoilProfileColumn.Items, GetSoilProfilesDataSource(pipingSoilProfiles).ToArray());
            }
        }

        private IEnumerable<PipingStochasticSoilProfile> GetPipingStochasticSoilProfilesFromStochasticSoilModels()
        {
            return FailureMechanism.StochasticSoilModels
                                   .SelectMany(ssm => ssm.StochasticSoilProfiles)
                                   .Distinct();
        }

        #endregion
    }
}