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

using System.Collections.Generic;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object wrapping an instance of <see cref="PipingInput"/>
    /// and allowing for selecting a surfaceline or soil profile based on data available
    /// in a piping failure mechanism.
    /// </summary>
    public class PipingInputContext : PipingContext<PipingInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingInputContext"/>
        /// </summary>
        /// <param name="pipingInput">The piping input instance wrapped by this context object.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models available within the piping context.</param>
        /// <param name="assessmentSection">The assessment section which the piping context belongs to.</param>
        /// <exception cref="System.ArgumentNullException">When any input parameter is null.</exception>
        public PipingInputContext(PipingInput pipingInput, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> stochasticSoilModels, IAssessmentSection assessmentSection)
            : base(pipingInput, surfaceLines, stochasticSoilModels, assessmentSection)
        {
        }
    }
}