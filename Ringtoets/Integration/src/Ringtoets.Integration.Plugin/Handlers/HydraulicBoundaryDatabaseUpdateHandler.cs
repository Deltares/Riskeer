// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Handlers;

namespace Ringtoets.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class that can properly update a <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public class HydraulicBoundaryDatabaseUpdateHandler : IHydraulicBoundaryDatabaseUpdateHandler
    {
        private readonly AssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseUpdateHandler"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to update for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseUpdateHandler(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;
        }

        public bool IsConfirmationRequired(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase)
        {
            throw new NotImplementedException();
        }

        public bool InquireConfirmation()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IObservable> Update(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase, ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase)
        {
            throw new NotImplementedException();
        }
    }
}