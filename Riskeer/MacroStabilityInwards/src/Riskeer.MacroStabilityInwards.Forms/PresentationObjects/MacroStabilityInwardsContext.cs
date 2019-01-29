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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required macro stability inwards knowledge to configure and create
    /// macro stability inwards related objects. It'll delegate observable behavior to the wrapped data object.
    /// </summary>
    public abstract class MacroStabilityInwardsContext<T> : ObservableWrappedObjectContextBase<T> where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsContext{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="surfaceLines">The surface lines available within the context.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models available within the context.</param>
        /// <param name="macroStabilityInwardsFailureMechanism">The macro stability inwards failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        protected MacroStabilityInwardsContext(
            T wrappedData,
            IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines,
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels,
            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism,
            IAssessmentSection assessmentSection) : base(wrappedData)
        {
            AssertInputsAreNotNull(surfaceLines, stochasticSoilModels, macroStabilityInwardsFailureMechanism, assessmentSection);

            AvailableMacroStabilityInwardsSurfaceLines = surfaceLines;
            AvailableStochasticSoilModels = stochasticSoilModels;
            FailureMechanism = macroStabilityInwardsFailureMechanism;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the available macro stability inwards surface lines in order for the user to select one to 
        /// set <see cref="MacroStabilityInwardsInput.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsSurfaceLine> AvailableMacroStabilityInwardsSurfaceLines { get; }

        /// <summary>
        /// Gets the available stochastic soil models in order for the user to select a <see cref="MacroStabilityInwardsStochasticSoilModel"/> and <see cref="MacroStabilityInwardsStochasticSoilProfile"/>
        /// to set <see cref="MacroStabilityInwardsInput.StochasticSoilProfile"/> and <see cref="MacroStabilityInwardsInput.StochasticSoilModel"/>.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsStochasticSoilModel> AvailableStochasticSoilModels { get; }

        /// <summary>
        /// Gets the failure mechanism which the context belongs to.
        /// </summary>
        public MacroStabilityInwardsFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section which the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Asserts the inputs are not <c>null</c>.
        /// </summary>
        /// <param name="surfaceLines">The surface lines.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models.</param>
        /// <param name="macroStabilityInwardsFailureMechanism">The macro stability inwards failure mechanism.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        private static void AssertInputsAreNotNull(IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines,
                                                   IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels,
                                                   MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism,
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

            if (macroStabilityInwardsFailureMechanism == null)
            {
                throw new ArgumentNullException(nameof(macroStabilityInwardsFailureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }
        }
    }
}