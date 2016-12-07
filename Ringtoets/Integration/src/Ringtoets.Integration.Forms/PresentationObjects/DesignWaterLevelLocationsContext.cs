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

using System;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an enumeration of <see cref="HydraulicBoundaryLocation"/> 
    /// with <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>.
    /// </summary>
    public class DesignWaterLevelLocationsContext : ObservableWrappedObjectContextBase<IAssessmentSection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelLocationsContext"/>.
        /// </summary>
        /// <param name="wrappedAssessmentSection">The <see cref="IAssessmentSection"/> which the <see cref="DesignWaterLevelLocationsContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedAssessmentSection"/> is <c>null</c>.</exception>
        public DesignWaterLevelLocationsContext(IAssessmentSection wrappedAssessmentSection) : base(wrappedAssessmentSection) {}
    }
}