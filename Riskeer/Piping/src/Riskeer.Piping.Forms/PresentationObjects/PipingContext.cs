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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;

namespace Riskeer.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required piping knowledge to configure and create
    /// piping related objects. It'll delegate observable behavior to the wrapped data object.
    /// </summary>
    public abstract class PipingContext<T> : ObservableWrappedObjectContextBase<T> where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingContext{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models available within the piping context.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism which the piping context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the piping context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        protected PipingContext(
            T wrappedData,
            IEnumerable<PipingSurfaceLine> surfaceLines,
            IEnumerable<PipingStochasticSoilModel> stochasticSoilModels,
            PipingFailureMechanism pipingFailureMechanism,
            IAssessmentSection assessmentSection) : base(wrappedData)
        {
            AssertInputsAreNotNull(surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection);

            AvailablePipingSurfaceLines = surfaceLines;
            AvailableStochasticSoilModels = stochasticSoilModels;
            FailureMechanism = pipingFailureMechanism;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the available piping surface lines in order for the user to select one to 
        /// set <see cref="PipingInput.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<PipingSurfaceLine> AvailablePipingSurfaceLines { get; }

        /// <summary>
        /// Gets the available stochastic soil models in order for the user to select a <see cref="StochasticSoilModel"/> and <see cref="PipingStochasticSoilProfile"/>
        /// to set <see cref="PipingInput.StochasticSoilProfile"/> and <see cref="PipingInput.StochasticSoilModel"/>.
        /// </summary>
        public IEnumerable<PipingStochasticSoilModel> AvailableStochasticSoilModels { get; }

        /// <summary>
        /// Gets the piping failure mechanism which the piping context belongs to.
        /// </summary>
        public PipingFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section which the piping context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Asserts the inputs are not <c>null</c>.
        /// </summary>
        /// <param name="surfaceLines">The surface lines.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        private static void AssertInputsAreNotNull(IEnumerable<PipingSurfaceLine> surfaceLines,
                                                   IEnumerable<PipingStochasticSoilModel> stochasticSoilModels,
                                                   PipingFailureMechanism pipingFailureMechanism,
                                                   IAssessmentSection assessmentSection)
        {
            if (surfaceLines == null)
            {
                throw new ArgumentNullException(nameof(surfaceLines));
            }

            if (stochasticSoilModels == null)
            {
                throw new ArgumentNullException(nameof(stochasticSoilModels));
            }

            if (pipingFailureMechanism == null)
            {
                throw new ArgumentNullException(nameof(pipingFailureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }
        }
    }
}