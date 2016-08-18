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

namespace Ringtoets.HydraRing.Calculation.Data.Output
{
    /// <summary>
    /// Container of all relevant output generated by a type I calculation via Hydra-Ring:
    /// Given a set of random variables, compute the probability of failure.
    /// </summary>
    public class ExceedanceProbabilityCalculationOutput : ExceedanceProbabilityCalculationOutputBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExceedanceProbabilityCalculationOutput"/>.
        /// </summary>
        /// <param name="ringCombinMethod">The ring combine method used.</param>
        /// <param name="presentationSectionId">The presentation section id used.</param>
        /// <param name="mainMechanismId">The main mechanism id used.</param>
        /// <param name="mainMechanismCombinMethod">The main mechanism combine method used.</param>
        /// <param name="mechanismId">The mechanism id used.</param>
        /// <param name="sectionId">The section id used.</param>
        /// <param name="layerId">The layer id used.</param>
        /// <param name="alternativeId">The alternative id used.</param>
        /// <param name="beta">The beta value.</param>
        public ExceedanceProbabilityCalculationOutput(int ringCombinMethod, int presentationSectionId,
                                                      int mainMechanismId, int mainMechanismCombinMethod, int mechanismId,
                                                      int sectionId, int layerId, int alternativeId, double beta)
            : base(ringCombinMethod, presentationSectionId, mainMechanismId, mainMechanismCombinMethod, mechanismId, sectionId, layerId, alternativeId)
        {
            Beta = beta;
            Alphas = new List<ExceedanceProbabilityCalculationAlphaOutput>();
        }

        /// <summary>
        /// Gets the beta result.
        /// </summary>
        public double Beta { get; private set; }

        /// <summary>
        /// Gets the alpha result values.
        /// </summary>
        public IList<ExceedanceProbabilityCalculationAlphaOutput> Alphas { get; private set; }
    }
}