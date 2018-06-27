// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.GuiServices;
using Ringtoets.DuneErosion.Forms.Properties;
using Ringtoets.DuneErosion.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.Views
{
    /// <summary>
    /// View for the <see cref="DuneLocationCalculation"/>.
    /// </summary>
    public partial class DuneLocationCalculationsView : DuneLocationCalculationsViewBase
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer duneLocationCalculationsObserver;
        private readonly IObservableEnumerable<DuneLocationCalculation> calculations;
        private readonly Func<double> getNormFunc;
        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> duneLocationCalculationObserver;
        private readonly DuneLocationCalculationMessageProvider messageProvider;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsView"/>.
        /// </summary>
        /// <param name="calculations">The calculations to show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism which the calculations belong to.</param>
        /// <param name="assessmentSection">The assessment section which the calculations belong to.</param>
        /// <param name="getNormFunc"><see cref="Func{TResult}"/> for getting the norm to use during calculations.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public DuneLocationCalculationsView(IObservableEnumerable<DuneLocationCalculation> calculations,
                                            DuneErosionFailureMechanism failureMechanism,
                                            IAssessmentSection assessmentSection,
                                            Func<double> getNormFunc,
                                            string categoryBoundaryName)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getNormFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormFunc));
            }

            InitializeComponent();

            messageProvider = new DuneLocationCalculationMessageProvider(categoryBoundaryName);

            this.calculations = calculations;
            this.getNormFunc = getNormFunc;
            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;

            duneLocationCalculationsObserver = new Observer(UpdateDataGridViewDataSource)
            {
                Observable = calculations
            };
            duneLocationCalculationObserver = new RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>(dataGridViewControl.RefreshDataGridView, list => list)
            {
                Observable = calculations
            };
            failureMechanismObserver = new Observer(UpdateCalculateForSelectedButton)
            {
                Observable = failureMechanism
            };

            UpdateDataGridViewDataSource();
        }

        public override object Data { get; set; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the <see cref="DuneErosionFailureMechanism"/> for which the
        /// calculations are shown.
        /// </summary>
        public DuneErosionFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets or sets the <see cref="DuneLocationCalculationGuiService"/> 
        /// to perform calculations with.
        /// </summary>
        public DuneLocationCalculationGuiService CalculationGuiService { get; set; }

        protected override void Dispose(bool disposing)
        {
            duneLocationCalculationsObserver.Dispose();
            duneLocationCalculationObserver.Dispose();
            failureMechanismObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.Name),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.Id),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.Location),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Coordinates_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.CoastalAreaId),
                                                 Resources.DuneLocation_CoastalAreaId_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.Offset),
                                                 Resources.DuneLocation_Offset_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.WaterLevel),
                                                 Resources.DuneLocationCalculationOutput_WaterLevel_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.WaveHeight),
                                                 Resources.DuneLocationCalculationOutput_WaveHeight_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.WavePeriod),
                                                 Resources.DuneLocationCalculationOutput_WavePeriod_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.D50),
                                                 Resources.DuneLocation_D50_DisplayName);
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;
            return ((DuneLocationCalculationRow) currentRow?.DataBoundItem)?.CalculatableObject;
        }

        protected override void SetDataSource()
        {
            dataGridViewControl.SetDataSource(calculations?.Select(calc => new DuneLocationCalculationRow(calc)).ToArray());
        }

        protected override void CalculateForSelectedRows()
        {
            CalculationGuiService?.Calculate(GetSelectedCalculatableObjects(),
                                             AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                             AssessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                                             getNormFunc(),
                                             messageProvider);
        }
    }
}