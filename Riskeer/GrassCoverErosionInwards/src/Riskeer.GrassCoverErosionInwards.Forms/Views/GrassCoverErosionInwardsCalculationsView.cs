﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.Properties;
using CoreGuiResources = Core.Gui.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring grass cover erosion inwards calculations.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationsView : CalculationsView<GrassCoverErosionInwardsCalculationScenario, GrassCoverErosionInwardsInput, GrassCoverErosionInwardsCalculationRow, GrassCoverErosionInwardsFailureMechanism>
    {
        private const int dikeProfileColumnIndex = 1;

        private Observer dikeProfilesObserver;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationsView"/>.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsCalculationsView(CalculationGroup data, GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                        IAssessmentSection assessmentSection)
            : base(data, failureMechanism, assessmentSection) {}

        protected override void Dispose(bool disposing)
        {
            if (disposing && Loaded)
            {
                dikeProfilesObserver.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override object CreateSelectedItemFromCurrentRow(GrassCoverErosionInwardsCalculationRow currentRow)
        {
            return new GrassCoverErosionInwardsInputContext(
                currentRow.Calculation.InputParameters,
                currentRow.Calculation,
                FailureMechanism,
                AssessmentSection);
        }

        protected override IEnumerable<Point2D> GetReferenceLocations()
        {
            return FailureMechanism.DikeProfiles.Select(dp => dp.WorldReferencePoint);
        }

        protected override GrassCoverErosionInwardsCalculationRow CreateRow(GrassCoverErosionInwardsCalculationScenario calculation)
        {
            return new GrassCoverErosionInwardsCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, calculation.InputParameters));
        }

        protected override bool CanGenerateCalculations()
        {
            return FailureMechanism.DikeProfiles.Any();
        }

        protected override void GenerateCalculations()
        {
            var calculationGroup = (CalculationGroup) Data;
            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(Parent, FailureMechanism.DikeProfiles))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GrassCoverErosionInwardsCalculationConfigurationHelper.GenerateCalculations(calculationGroup, dialog.SelectedItems);
                    calculationGroup.NotifyObservers();
                }
            }
        }

        protected override void AddColumns(Action addNameColumn, Action addHydraulicBoundaryLocationColumn)
        {
            addNameColumn();

            DataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<DikeProfile>>(
                nameof(GrassCoverErosionInwardsCalculationRow.DikeProfile),
                Resources.DikeProfile_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<DikeProfile>.This),
                nameof(DataGridViewComboBoxItemWrapper<DikeProfile>.DisplayName));

            addHydraulicBoundaryLocationColumn();

            DataGridViewControl.AddCheckBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.UseBreakWater),
                RiskeerCommonFormsResources.Use_BreakWater_DisplayName);

            DataGridViewControl.AddComboBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.BreakWaterType),
                RiskeerCommonFormsResources.CalculationsView_BreakWaterType_DisplayName,
                EnumDisplayWrapperHelper.GetEnumTypes<BreakWaterType>(),
                nameof(EnumDisplayWrapper<BreakWaterType>.Value),
                nameof(EnumDisplayWrapper<BreakWaterType>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.BreakWaterHeight),
                RiskeerCommonFormsResources.CalculationsView_BreakWaterHeight_DisplayName);

            DataGridViewControl.AddCheckBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.UseForeshoreGeometry),
                RiskeerCommonFormsResources.Use_Foreshore_DisplayName);

            DataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.DikeHeight),
                RiskeerCommonFormsResources.DikeHeight_DisplayName);

            DataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.MeanCriticalFlowRate),
                $"{RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName}\r\n{Resources.GrassCoverErosionInwardsCalculationsView_CriticalFlowRate_DisplayName}");

            DataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsCalculationRow.StandardDeviationCriticalFlowRate),
                $"{RiskeerCommonFormsResources.NormalDistribution_StandardDeviation_DisplayName}\r\n{Resources.GrassCoverErosionInwardsCalculationsView_CriticalFlowRate_DisplayName}");
        }

        protected override void InitializeObservers()
        {
            base.InitializeObservers();

            dikeProfilesObserver = new Observer(() =>
            {
                PrefillComboBoxListItemsAtColumnLevel();
                UpdateComboBoxColumns();
                UpdateGenerateCalculationsButtonState();
            })
            {
                Observable = FailureMechanism.DikeProfiles
            };
        }

        #region Prefill combo box list items

        protected override void PrefillComboBoxListItemsAtColumnLevel()
        {
            base.PrefillComboBoxListItemsAtColumnLevel();

            // Need to prefill for all possible data in order to guarantee 'combo box' columns
            // do not generate errors when their cell value is not present in the list of available
            // items.
            var dikeProfileColumn = (DataGridViewComboBoxColumn) DataGridViewControl.GetColumnFromIndex(dikeProfileColumnIndex);

            using (new SuspendDataGridViewColumnResizes(dikeProfileColumn))
            {
                SetItemsOnObjectCollection(dikeProfileColumn.Items,
                                           GetDikeProfileDataSource(FailureMechanism.DikeProfiles));
            }
        }

        #endregion

        protected override void SubscribeToCalculationRow(GrassCoverErosionInwardsCalculationRow calculationRow)
        {
            base.SubscribeToCalculationRow(calculationRow);

            calculationRow.DikeProfileChanged += DikeProfileChanged;
        }

        protected override void UnsubscribeFromCalculationRow(GrassCoverErosionInwardsCalculationRow calculationRow)
        {
            base.UnsubscribeFromCalculationRow(calculationRow);

            calculationRow.DikeProfileChanged -= DikeProfileChanged;
        }

        private void DikeProfileChanged(object sender, EventArgs e)
        {
            UpdateHydraulicBoundaryLocationColumn();
        }

        private void UpdateHydraulicBoundaryLocationColumn()
        {
            PrefillComboBoxListItemsAtColumnLevel();
            UpdateComboBoxColumns();
        }

        #region Update combo box list items

        protected override void UpdateComboBoxColumns()
        {
            base.UpdateComboBoxColumns();
            UpdateDikeProfilesColumn();
        }

        #region Update DikeProfiles

        private void UpdateDikeProfilesColumn()
        {
            var column = (DataGridViewComboBoxColumn) DataGridViewControl.GetColumnFromIndex(dikeProfileColumnIndex);

            using (new SuspendDataGridViewColumnResizes(column))
            {
                foreach (DataGridViewRow dataGridViewRow in DataGridViewControl.Rows)
                {
                    FillAvailableDikeProfilesList(dataGridViewRow);
                }
            }
        }

        private void FillAvailableDikeProfilesList(DataGridViewRow dataGridViewRow)
        {
            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[dikeProfileColumnIndex];
            DataGridViewComboBoxItemWrapper<DikeProfile>[] dataGridViewComboBoxItemWrappers = GetDikeProfileDataSource(FailureMechanism.DikeProfiles);
            SetItemsOnObjectCollection(cell.Items, dataGridViewComboBoxItemWrappers);
        }

        private static DataGridViewComboBoxItemWrapper<DikeProfile>[] GetDikeProfileDataSource(IEnumerable<DikeProfile> dikeProfiles = null)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<DikeProfile>>
            {
                new DataGridViewComboBoxItemWrapper<DikeProfile>(null)
            };

            if (dikeProfiles != null)
            {
                dataGridViewComboBoxItemWrappers.AddRange(dikeProfiles.Select(dp => new DataGridViewComboBoxItemWrapper<DikeProfile>(dp)));
            }

            return dataGridViewComboBoxItemWrappers.ToArray();
        }

        #endregion

        #endregion
    }
}