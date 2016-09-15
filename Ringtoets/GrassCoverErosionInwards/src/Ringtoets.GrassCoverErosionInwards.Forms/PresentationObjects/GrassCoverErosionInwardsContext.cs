﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required grass cover erosion inwards input knowledge to configure and create
    /// related objects. It'll delegate observable behavior to the wrapped data object.
    /// </summary>
    public abstract class GrassCoverErosionInwardsContext<T> : ObservableWrappedObjectContextBase<T> where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionInwardsContext{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">When any input argument is <c>null</c>.</exception>
        protected GrassCoverErosionInwardsContext(
            T wrappedData,
            GrassCoverErosionInwardsFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (failureMechanism == null)
            {
                var message = string.Format(RingtoetsCommonFormsResources.AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.GrassCoverErosionInwardsContext_DataDescription_GrassCoverErosionInwardsFailureMechanism);

                throw new ArgumentNullException("failureMechanism", message);
            }

            if (assessmentSection == null)
            {
                var message = string.Format(RingtoetsCommonFormsResources.AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            RingtoetsCommonFormsResources.FailureMechanismContext_DataDescription_AssessmentSection);
                throw new ArgumentNullException("assessmentSection", message);
            }

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the failure mechanism which the context belongs to.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism FailureMechanism { get; private set; }

        /// <summary>
        /// Gets the assessment section which the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; private set; }

        /// <summary>
        /// Gets the available dike profiles which can be used to assign to a <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        public IEnumerable<DikeProfile> AvailableDikeProfiles
        {
            get
            {
                return FailureMechanism.DikeProfiles;
            }
        }

        /// <summary>
        /// Gets the available hydraulic boundary locations in order for the user to select one to 
        /// set <see cref="GrassCoverErosionInwardsInput.HydraulicBoundaryLocation"/>.
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