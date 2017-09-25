﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;

        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightLocationsView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section which the locations belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public WaveHeightLocationsView(IAssessmentSection assessmentSection)
            : base(assessmentSection)
        {
            InitializeComponent();

            assessmentSectionObserver = new Observer(UpdateHydraulicBoundaryDatabase);
            hydraulicBoundaryDatabaseObserver = new Observer(HandleHydraulicBoundaryDatabaseUpdate);

            assessmentSectionObserver.Observable = AssessmentSection;
            hydraulicBoundaryDatabaseObserver.Observable = AssessmentSection.HydraulicBoundaryDatabase;
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
            if (AssessmentSection?.HydraulicBoundaryDatabase == null)
            {
                return;
            }

            bool successfulCalculation = CalculationGuiService.CalculateWaveHeights(AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                    locations,
                                                                                    AssessmentSection.FailureMechanismContribution.Norm,
                                                                                    new WaveHeightCalculationMessageProvider());

            if (successfulCalculation)
            {
                AssessmentSection.HydraulicBoundaryDatabase.NotifyObservers();
            }
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationRow.Result),
                                                 Resources.HydraulicBoundaryDatabase_Location_WaveHeight_DisplayName);
        }

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            hydraulicBoundaryDatabaseObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override HydraulicBoundaryLocationCalculation GetCalculation(HydraulicBoundaryLocation location)
        {
            return location.WaveHeightCalculation;
        }

        private void UpdateHydraulicBoundaryDatabase()
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = AssessmentSection.HydraulicBoundaryDatabase;

            if (!ReferenceEquals(Data, hydraulicBoundaryDatabase?.Locations))
            {
                hydraulicBoundaryDatabaseObserver.Observable = hydraulicBoundaryDatabase;
                Data = hydraulicBoundaryDatabase?.Locations;
            }
            else
            {
                HandleHydraulicBoundaryDatabaseUpdate();
            }
        }
    }
}