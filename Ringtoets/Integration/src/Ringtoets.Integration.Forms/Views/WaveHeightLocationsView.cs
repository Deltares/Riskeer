// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils.Reflection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.WaveHeight"/>.
    /// </summary>
    public partial class WaveHeightLocationsView : HydraulicBoundaryLocationsView
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightLocationsView"/>.
        /// </summary>
        public WaveHeightLocationsView()
        {
            InitializeComponent();
        }

        protected override void SetDataSource()
        {
            dataGridViewControl.SetDataSource(AssessmentSection != null && AssessmentSection.HydraulicBoundaryDatabase != null
                                                  ? AssessmentSection.HydraulicBoundaryDatabase.Locations.Select(
                                                      hl => new WaveHeightLocationContextRow(
                                                                new WaveHeightLocationContext(AssessmentSection.HydraulicBoundaryDatabase, hl))).ToArray()
                                                  : null);
        }

        protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
        {
            if (AssessmentSection == null || AssessmentSection.HydraulicBoundaryDatabase == null)
            {
                return;
            }
            CalculationGuiService.CalculateWaveHeights(AssessmentSection.HydraulicBoundaryDatabase, locations,
                                                       AssessmentSection.Id, AssessmentSection.FailureMechanismContribution.Norm);
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<WaveHeightLocationContextRow>(row => row.WaveHeight),
                                                 Resources.HydraulicBoundaryDatabase_Locations_WaveHeight_DisplayName);
        }
    }
}