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
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required piping knowledge to configure and create
    /// piping related objects. It'll delegate observable behavior to the wrapped data object.
    /// </summary>
    public abstract class PipingContext<T> : IObservable where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingContext{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="soilProfiles">The soil profiles available within the piping context.</param>
        /// <param name="assessmentSection">The assessment section which the piping context belongs to.</param>
        /// <exception cref="System.ArgumentNullException">When any input parameter is null.</exception>
        protected PipingContext(
            T wrappedData,
            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines,
            IEnumerable<PipingSoilProfile> soilProfiles, 
            AssessmentSectionBase assessmentSection)
        {
            AssertInputsAreNotNull(wrappedData, surfaceLines, soilProfiles, assessmentSection);

            WrappedData = wrappedData;
            AvailablePipingSurfaceLines = surfaceLines;
            AvailablePipingSoilProfiles = soilProfiles;
            AssessmentSection = assessmentSection;
        }

        public override bool Equals(object obj)
        {
            var context = obj as PipingContext<T>;
            if (context != null)
            {
                return WrappedData.Equals(context.WrappedData);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return WrappedData.GetHashCode();
        }

        /// <summary>
        /// Gets the available piping surface lines in order for the user to select one to 
        /// set <see cref="PipingInput.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> AvailablePipingSurfaceLines { get; private set; }

        /// <summary>
        /// Gets the available piping soil profiles in order for the user to select one to 
        /// set <see cref="PipingInput.SoilProfile"/>.
        /// </summary>
        public IEnumerable<PipingSoilProfile> AvailablePipingSoilProfiles { get; private set; }

        /// <summary>
        /// Gets the available hydraulic boundary locations in order for the user to select one to 
        /// set <see cref="PipingInput.HydraulicBoundaryLocation"/>.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocation> AvailableHydraulicBoundaryLocations 
        {
            get
            {
                if (AssessmentSection.HydraulicBoundaryDatabase == null)
                {
                    return Enumerable.Empty<HydraulicBoundaryLocation>();
                }
                return AssessmentSection.HydraulicBoundaryDatabase.Locations;
            }
        }

        /// <summary>
        /// Gets the concrete data instance wrapped by this context object.
        /// </summary>
        public T WrappedData { get; private set; }

        /// <summary>
        /// Gets the assessment section which the piping context belongs to.
        /// </summary>
        public AssessmentSectionBase AssessmentSection { get; private set; }

        /// <summary>
        /// Asserts the inputs are not null.
        /// </summary>
        /// <param name="wrappedData">The wrapped data.</param>
        /// <param name="surfaceLines">The surface lines.</param>
        /// <param name="soilProfiles">The soil profiles.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="System.ArgumentNullException">When any input parameter is null.</exception>
        private static void AssertInputsAreNotNull(object wrappedData, object surfaceLines, object soilProfiles, object assessmentSection)
        {
            if (wrappedData == null)
            {
                var message = String.Format(Resources.PipingContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.PipingContext_DataDescription_WrappedData);
                throw new ArgumentNullException("wrappedData", message);
            }
            if (surfaceLines == null)
            {
                var message = String.Format(Resources.PipingContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.PipingContext_DataDescription_Surfacelines);
                throw new ArgumentNullException("surfaceLines", message);
            }
            if (soilProfiles == null)
            {
                var message = String.Format(Resources.PipingContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.PipingContext_DataDescription_Soilprofiles);
                throw new ArgumentNullException("soilProfiles", message);
            }
            if (assessmentSection == null)
            {
                var message = String.Format(Resources.PipingContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.PipingContext_DataDescription_AssessmentSection);
                throw new ArgumentNullException("assessmentSection", message);
            }
        }

        #region IObservable

        public void Attach(IObserver observer)
        {
            WrappedData.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            WrappedData.Detach(observer);
        }

        public void NotifyObservers()
        {
            WrappedData.NotifyObservers();
        }

        #endregion
    }
}