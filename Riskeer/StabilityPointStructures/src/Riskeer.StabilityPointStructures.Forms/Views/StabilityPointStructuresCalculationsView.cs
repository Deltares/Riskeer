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
using Core.Common.Util;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring closing structures calculations.
    /// </summary>
    public class StabilityPointStructuresCalculationsView : CalculationsView<StructuresCalculationScenario<StabilityPointStructuresInput>, StabilityPointStructuresInput, StabilityPointStructuresCalculationRow, StabilityPointStructuresFailureMechanism>
    {
        private const int foreshoreProfileColumnIndex = 2;

        private Observer foreshoreProfilesObserver;
        private Observer stabilityPointStructuresObserver;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationsView"/>.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationsView(CalculationGroup data, StabilityPointStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(data, failureMechanism, assessmentSection) {}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DataGridViewControl.CellFormatting += HandleCellStyling;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreshoreProfilesObserver.Dispose();
                stabilityPointStructuresObserver.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override object CreateSelectedItemFromCurrentRow(StabilityPointStructuresCalculationRow currentRow)
        {
            return new StabilityPointStructuresInputContext(
                currentRow.Calculation.InputParameters,
                currentRow.Calculation,
                FailureMechanism,
                AssessmentSection);
        }

        protected override IEnumerable<Point2D> GetReferenceLocations()
        {
            return FailureMechanism.StabilityPointStructures.Select(sps => sps.Location);
        }

        protected override bool IsCalculationIntersectionWithReferenceLineInSection(StructuresCalculationScenario<StabilityPointStructuresInput> calculation, IEnumerable<Segment2D> lineSegments)
        {
            return calculation.IsStructureIntersectionWithReferenceLineInSection(lineSegments);
        }

        protected override StabilityPointStructuresCalculationRow CreateRow(StructuresCalculationScenario<StabilityPointStructuresInput> calculation)
        {
            return new StabilityPointStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, calculation.InputParameters));
        }

        protected override bool CanGenerateCalculations()
        {
            return FailureMechanism.StabilityPointStructures.Any();
        }

        protected override void GenerateCalculations()
        {
            var calculationGroup = (CalculationGroup) Data;
            using (var dialog = new StructureSelectionDialog(Parent, FailureMechanism.StabilityPointStructures))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    StructureCalculationConfigurationHelper.GenerateCalculations<StabilityPointStructure, StabilityPointStructuresInput>(calculationGroup, dialog.SelectedItems.Cast<StabilityPointStructure>());
                    calculationGroup.NotifyObservers();
                }
            }
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();

            DataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<ForeshoreProfile>>(
                nameof(StabilityPointStructuresCalculationRow.ForeshoreProfile),
                RiskeerCommonFormsResources.Structure_ForeshoreProfile_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<ForeshoreProfile>.This),
                nameof(DataGridViewComboBoxItemWrapper<ForeshoreProfile>.DisplayName));

            DataGridViewControl.AddCheckBoxColumn(nameof(StabilityPointStructuresCalculationRow.UseBreakWater),
                                                  RiskeerCommonFormsResources.Use_BreakWater_DisplayName);

            DataGridViewControl.AddComboBoxColumn(nameof(StabilityPointStructuresCalculationRow.BreakWaterType),
                                                  RiskeerCommonFormsResources.CalculationsView_BreakWaterType_DisplayName,
                                                  EnumDisplayWrapperHelper.GetEnumTypes<BreakWaterType>(),
                                                  nameof(EnumDisplayWrapper<BreakWaterType>.Value),
                                                  nameof(EnumDisplayWrapper<BreakWaterType>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.BreakWaterHeight),
                RiskeerCommonFormsResources.CalculationsView_BreakWaterHeight_DisplayName);

            DataGridViewControl.AddCheckBoxColumn(nameof(StabilityPointStructuresCalculationRow.UseForeshoreGeometry),
                                                  RiskeerCommonFormsResources.Use_Foreshore_DisplayName);

            DataGridViewControl.AddComboBoxColumn(nameof(StabilityPointStructuresCalculationRow.LoadSchematizationType),
                                                  RiskeerCommonFormsResources.LoadSchematizationType_DisplayName,
                                                  EnumDisplayWrapperHelper.GetEnumTypes<LoadSchematizationType>(),
                                                  nameof(EnumDisplayWrapper<LoadSchematizationType>.Value),
                                                  nameof(EnumDisplayWrapper<LoadSchematizationType>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.ConstructiveStrengthLinearLoadModel),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{RiskeerCommonFormsResources.ConstructiveStrength_Linear_LoadModel_DisplayName}");

            DataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.ConstructiveStrengthQuadraticLoadModel),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{RiskeerCommonFormsResources.ConstructiveStrength_Quadratic_LoadModel_DisplayName}");

            DataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.StabilityLinearLoadModel),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{RiskeerCommonFormsResources.Stability_Linear_LoadModel_DisplayName}");

            DataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.StabilityQuadraticLoadModel),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{RiskeerCommonFormsResources.Stability_Quadratic_LoadModel_DisplayName}");

            DataGridViewControl.AddTextBoxColumn(
                nameof(StabilityPointStructuresCalculationRow.EvaluationLevel),
                RiskeerCommonFormsResources.Evaluation_Level_DisplayName);
        }

        protected override void InitializeObservers()
        {
            base.InitializeObservers();

            foreshoreProfilesObserver = new Observer(() =>
            {
                PrefillComboBoxListItemsAtColumnLevel();
                UpdateColumns();
                UpdateGenerateCalculationsButtonState();
            })
            {
                Observable = FailureMechanism.ForeshoreProfiles
            };

            stabilityPointStructuresObserver = new Observer(UpdateGenerateCalculationsButtonState)
            {
                Observable = FailureMechanism.StabilityPointStructures
            };
        }

        #region Prefill combo box list items

        protected override void PrefillComboBoxListItemsAtColumnLevel()
        {
            base.PrefillComboBoxListItemsAtColumnLevel();

            // Need to prefill for all possible data in order to guarantee 'combo box' columns
            // do not generate errors when their cell value is not present in the list of available
            // items.
            var foreshoreProfiles = (DataGridViewComboBoxColumn) DataGridViewControl.GetColumnFromIndex(foreshoreProfileColumnIndex);

            using (new SuspendDataGridViewColumnResizes(foreshoreProfiles))
            {
                SetItemsOnObjectCollection(foreshoreProfiles.Items,
                                           GetForeshoreProfileDataSource(FailureMechanism.ForeshoreProfiles));
            }
        }

        #endregion

        #region Event handling

        private void HandleCellStyling(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewControl.FormatCellWithColumnStateDefinition(e.RowIndex, e.ColumnIndex);
        }

        #endregion

        #region Update combo box list items

        protected override void UpdateColumns()
        {
            base.UpdateColumns();
            UpdateForeshoreProfilesColumn();
        }

        #region Update ForeshoreProfiles

        private void UpdateForeshoreProfilesColumn()
        {
            var column = (DataGridViewComboBoxColumn) DataGridViewControl.GetColumnFromIndex(foreshoreProfileColumnIndex);

            using (new SuspendDataGridViewColumnResizes(column))
            {
                foreach (DataGridViewRow dataGridViewRow in DataGridViewControl.Rows)
                {
                    FillAvailableForeshoreProfilesList(dataGridViewRow);
                }
            }
        }

        private void FillAvailableForeshoreProfilesList(DataGridViewRow dataGridViewRow)
        {
            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[foreshoreProfileColumnIndex];
            DataGridViewComboBoxItemWrapper<ForeshoreProfile>[] dataGridViewComboBoxItemWrappers = GetForeshoreProfileDataSource(FailureMechanism.ForeshoreProfiles);
            SetItemsOnObjectCollection(cell.Items, dataGridViewComboBoxItemWrappers);
        }

        private static DataGridViewComboBoxItemWrapper<ForeshoreProfile>[] GetForeshoreProfileDataSource(IEnumerable<ForeshoreProfile> foreshoreProfiles = null)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<ForeshoreProfile>>
            {
                new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(null)
            };

            if (foreshoreProfiles != null)
            {
                dataGridViewComboBoxItemWrappers.AddRange(foreshoreProfiles.Select(fp => new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(fp)));
            }

            return dataGridViewComboBoxItemWrappers.ToArray();
        }

        #endregion

        #endregion
    }
}