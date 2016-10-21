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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required structures input knowledge to configure and create
    /// related objects. It'll delegate observable behavior to the wrapped data object.
    /// </summary>
    /// <typeparam name="TData">The type of the data wrapped by the context object.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism which the context belongs to.</typeparam>
    public abstract class StructuresContextBase<TData, TFailureMechanism> : ObservableWrappedObjectContextBase<TData>
        where TData : IObservable
        where TFailureMechanism : IFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructuresContextBase{TData, TFailureMechanism}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">When any input argument is <c>null</c>.</exception>
        protected StructuresContextBase(TData wrappedData,
                                        TFailureMechanism failureMechanism,
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
        public TFailureMechanism FailureMechanism { get; private set; }

        /// <summary>
        /// Gets the assessment section which the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; private set; }

        /// <summary>
        /// Gets the available hydraulic boundary locations.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocation> AvailableHydraulicBoundaryLocations
        {
            get
            {
                return AssessmentSection.HydraulicBoundaryDatabase != null
                           ? AssessmentSection.HydraulicBoundaryDatabase.Locations
                           : Enumerable.Empty<HydraulicBoundaryLocation>();
            }
        }
    }
}