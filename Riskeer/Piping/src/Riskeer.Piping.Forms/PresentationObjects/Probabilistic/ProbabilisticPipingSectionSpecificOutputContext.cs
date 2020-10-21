// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Forms.PresentationObjects.Probabilistic
{
    /// <summary>
    /// Presentation object for the section specific output of <see cref="ProbabilisticPipingCalculationScenario"/>.
    /// </summary>
    public class ProbabilisticPipingSectionSpecificOutputContext : ObservableWrappedObjectContextBase<ProbabilisticPipingCalculation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingOutputContext"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="ProbabilisticPipingCalculationScenario"/> object to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public ProbabilisticPipingSectionSpecificOutputContext(ProbabilisticPipingCalculationScenario calculation) : base(calculation) {}
    }
}