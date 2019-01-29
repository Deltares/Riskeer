﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// View for presenting and performing design water level calculations.
    /// </summary>
    public partial class DesignWaterLevelCalculationsView : HydraulicBoundaryCalculationsView
    {
        private readonly Func<double> getNormFunc;
        private readonly string categoryBoundaryName;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationsView"/>.
        /// </summary>
        /// <param name="calculations">The calculations to show in the view.</param>
        /// <param name="assessmentSection">The assessment section which the calculations belong to.</param>
        /// <param name="getNormFunc"><see cref="Func{TResult}"/> for getting the norm to use during calculations.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/>, <paramref name="assessmentSection"/>
        /// or <paramref name="getNormFunc"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public DesignWaterLevelCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                IAssessmentSection assessmentSection,
                                                Func<double> getNormFunc,
                                                string categoryBoundaryName)
            : base(calculations, assessmentSection)
        {
            if (getNormFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormFunc));
            }

            if (string.IsNullOrEmpty(categoryBoundaryName))
            {
                throw new ArgumentException($"'{nameof(categoryBoundaryName)}' must have a value.");
            }

            InitializeComponent();

            this.categoryBoundaryName = categoryBoundaryName;
            this.getNormFunc = getNormFunc;
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;

            if (currentRow != null)
            {
                return new DesignWaterLevelCalculationContext(((HydraulicBoundaryLocationCalculationRow) currentRow.DataBoundItem).CalculatableObject);
            }

            return null;
        }

        protected override void PerformSelectedCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            CalculationGuiService.CalculateDesignWaterLevels(calculations,
                                                             AssessmentSection,
                                                             getNormFunc(),
                                                             categoryBoundaryName);
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.Result),
                                                 RiskeerCommonFormsResources.WaterLevel_DisplayName);
        }
    }
}