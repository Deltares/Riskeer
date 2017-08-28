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
using System.Linq;
using System.Windows.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
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

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section which the locations belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        protected HydraulicBoundaryLocationsView(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;
        }

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

        protected override IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;
            if (currentRow == null)
            {
                return Enumerable.Empty<IllustrationPointControlItem>();
            }

            HydraulicBoundaryLocation location = ((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).CalculatableObject;

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = GetCalculation(location);
            HydraulicBoundaryLocationOutput hydraulicBoundaryLocationOutput = hydraulicBoundaryLocationCalculation.Output;
            if (hydraulicBoundaryLocationCalculation.HasOutput
                && hydraulicBoundaryLocationOutput.HasGeneralResult)
            {
                return hydraulicBoundaryLocationOutput.GeneralResult.TopLevelIllustrationPoints.Select(
                    topLevelSubMechanismIllustrationPoint =>
                    {
                        SubMechanismIllustrationPoint subMechanismIllustrationPoint =
                            topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint;
                        return new IllustrationPointControlItem(topLevelSubMechanismIllustrationPoint,
                                                                topLevelSubMechanismIllustrationPoint.WindDirection.Name,
                                                                topLevelSubMechanismIllustrationPoint.ClosingSituation,
                                                                subMechanismIllustrationPoint.Stochasts,
                                                                subMechanismIllustrationPoint.Beta);
                    });
            }

            return Enumerable.Empty<IllustrationPointControlItem>();
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocationCalculation"/> based on 
        /// the <see cref="HydraulicBoundaryLocation"/> data.
        /// </summary>
        /// <param name="location">The <see cref="HydraulicBoundaryLocation"/>
        /// to retrieve the calculation from.</param>
        /// <returns>A <see cref="HydraulicBoundaryLocationCalculation"/>.</returns>
        protected abstract HydraulicBoundaryLocationCalculation GetCalculation(HydraulicBoundaryLocation location);

        /// <summary>
        /// Creates a new row that is added to the data table.
        /// </summary>
        /// <param name="location">The location for which to create a new row.</param>
        /// <returns>The newly created row.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> or 
        /// <see cref="GetCalculation"/> is <c>null</c>.</exception>
        private HydraulicBoundaryLocationRow CreateNewRow(HydraulicBoundaryLocation location)
        {
            return new HydraulicBoundaryLocationRow(location, GetCalculation(location));
        }
    }
}