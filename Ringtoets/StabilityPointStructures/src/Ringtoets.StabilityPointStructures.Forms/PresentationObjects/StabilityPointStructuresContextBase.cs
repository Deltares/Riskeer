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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.StabilityPointStructures.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required stability point structures input knowledge to configure and create
    /// related objects. It will delegate observable behavior to the wrapped data object.
    /// </summary>
    public abstract class StabilityPointStructuresContextBase<T> : ObservableWrappedObjectContextBase<T> where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StabilityPointStructuresContextBase{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which <paramref name="failureMechanism"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">When any input argument is <c>null</c>.</exception>
        protected StabilityPointStructuresContextBase(T wrappedData,
                                                      StabilityPointStructuresFailureMechanism failureMechanism,
                                                      IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the failure mechanism which the context belongs to.
        /// </summary>
        public StabilityPointStructuresFailureMechanism FailureMechanism { get; private set; }

        /// <summary>
        /// Gets the assessment section which <see cref="FailureMechanism"/> belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; private set; }

        /// <summary>
        /// Gets the available hydraulic boundary locations in order for the user to select one to 
        /// set <see cref="StabilityPointStructuresInput.HydraulicBoundaryLocation"/>.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocation> AvailableHydraulicBoundaryLocations
        {
            get
            {
                return AssessmentSection.HydraulicBoundaryDatabase == null ?
                           Enumerable.Empty<HydraulicBoundaryLocation>() :
                           AssessmentSection.HydraulicBoundaryDatabase.Locations;
            }
        }
    }
}