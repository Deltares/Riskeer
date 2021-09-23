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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class is an extended view showing map data for an assessment section.
    /// </summary>
    public class AssessmentSectionExtendedView : AssessmentSectionReferenceLineView
    {
        private readonly HydraulicBoundaryLocationsMapLayer mapLayer;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionExtendedView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionExtendedView(IAssessmentSection assessmentSection) : base(assessmentSection)
        {
            mapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            MapDataCollection.Add(mapLayer.MapData);
        }

        protected override void Dispose(bool disposing)
        {
            mapLayer.Dispose();

            base.Dispose(disposing);
        }
    }
}