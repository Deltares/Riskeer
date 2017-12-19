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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Properties;
using Ringtoets.Integration.Service.MessageProviders;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.WaveHeight"/>.
    /// </summary>
    public partial class WaveHeightLocationsView : HydraulicBoundaryLocationsView
    {
        private readonly WaveHeightCalculationMessageProvider messageProvider;

        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightLocationsView"/>.
        /// </summary>
        /// <param name="locations">The locations to show in the view.</param>
        /// <param name="assessmentSection">The assessment section which the locations belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locations"/> or 
        /// <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public WaveHeightLocationsView(ObservableList<HydraulicBoundaryLocation> locations, IAssessmentSection assessmentSection)
            : base(locations, assessmentSection)
        {
            InitializeComponent();

            messageProvider = new WaveHeightCalculationMessageProvider();
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;

            return currentRow != null
                       ? new WaveHeightLocationContext(((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).CalculatableObject,
                                                       AssessmentSection.HydraulicBoundaryDatabase)
                       : null;
        }

        protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
        {
            CalculationGuiService.CalculateWaveHeights(AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                       AssessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                                                       locations,
                                                       AssessmentSection.FailureMechanismContribution.Norm,
                                                       messageProvider);
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationRow.Result),
                                                 Resources.HydraulicBoundaryDatabase_Location_WaveHeight_DisplayName);
        }

        protected override HydraulicBoundaryLocationCalculation GetCalculation(HydraulicBoundaryLocation location)
        {
            return location.WaveHeightCalculation;
        }
    }
}