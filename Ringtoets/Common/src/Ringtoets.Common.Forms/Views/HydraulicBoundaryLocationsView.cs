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

using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.GuiServices;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Base view for <see cref="HydraulicBoundaryLocation"/> views which should be derived in 
    /// order to get a consistent look and feel.
    /// </summary>
    public abstract partial class HydraulicBoundaryLocationsView
        : LocationsView<HydraulicBoundaryLocation>
    {
        private IEnumerable<HydraulicBoundaryLocation> locations;

        public override object Data
        {
            get
            {
                return locations;
            }
            set
            {
                locations = value as IEnumerable<HydraulicBoundaryLocation>;
                UpdateDataGridViewDataSource();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IHydraulicBoundaryLocationCalculationGuiService"/>.
        /// </summary>
        public IHydraulicBoundaryLocationCalculationGuiService CalculationGuiService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAssessmentSection"/>.
        /// </summary>
        public abstract IAssessmentSection AssessmentSection { get; set; }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddCheckBoxColumn(nameof(HydraulicBoundaryLocationRow.IncludeIllustrationPoints),
                                                  RingtoetsCommonFormsResources.HydraulicBoundaryLocationCalculationInput_IncludeIllustrationPoints_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationRow.Name),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationRow.Id),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationRow.Location),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Coordinates_DisplayName);
        }

        /// <summary>
        /// Creates a new row that is added to the data table.
        /// </summary>
        /// <param name="location">The location for which to create a new row.</param>
        /// <returns>The newly created row.</returns>
        protected abstract HydraulicBoundaryLocationRow CreateNewRow(HydraulicBoundaryLocation location);

        /// <summary>
        /// Handles the calculation of the <paramref name="locations"/>.
        /// </summary>
        /// <param name="locations">The enumeration of <see cref="HydraulicBoundaryLocation"/> to use in the calculation.</param>
        protected abstract void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations);

        protected override void CalculateForSelectedRows()
        {
            if (CalculationGuiService == null)
            {
                return;
            }

            IEnumerable<HydraulicBoundaryLocation> selectedLocations = GetSelectedCalculatableObjects();
            HandleCalculateSelectedLocations(selectedLocations);
        }

        protected override void SetDataSource()
        {
            dataGridViewControl.SetDataSource(locations?.Select(CreateNewRow).ToArray());
        }
    }
}