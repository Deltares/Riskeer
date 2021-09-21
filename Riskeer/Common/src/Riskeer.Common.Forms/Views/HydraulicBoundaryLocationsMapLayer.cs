// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Components.Gis.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Factories;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Map layer to show hydraulic boundary locations.
    /// </summary>
    public class HydraulicBoundaryLocationsMapLayer : IDisposable
    {
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsMapLayer"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to get the data from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public HydraulicBoundaryLocationsMapLayer(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;

            MapData = RiskeerMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            SetFeatures();
        }

        /// <summary>
        /// Gets the hydraulic boundary locations map data.
        /// </summary>
        public MapPointData MapData { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {}
        }

        private void SetFeatures()
        {
            MapData.Features = RiskeerMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(assessmentSection);
        }
    }
}