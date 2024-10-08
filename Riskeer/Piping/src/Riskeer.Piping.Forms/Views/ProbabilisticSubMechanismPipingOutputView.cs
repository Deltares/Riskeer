﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// Override of <see cref="GeneralResultSubMechanismIllustrationPointView"/> for making output views for
    /// <see cref="ProbabilisticPipingCalculationScenario"/> uniquely identifiable (when it comes to
    /// opening/closing views).
    /// </summary>
    public class ProbabilisticSubMechanismPipingOutputView : GeneralResultSubMechanismIllustrationPointView
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticSubMechanismPipingOutputView"/>.
        /// </summary>
        /// <param name="calculation">The calculation to show the illustration points for.</param>
        /// <param name="getGeneralResultFunc">A <see cref="Func{TResult}"/> for obtaining the illustration point
        /// data (<see cref="GeneralResult{T}"/> with <see cref="TopLevelSubMechanismIllustrationPoint"/> objects)
        /// that must be presented.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ProbabilisticSubMechanismPipingOutputView(
            ProbabilisticPipingCalculationScenario calculation,
            Func<GeneralResult<TopLevelSubMechanismIllustrationPoint>> getGeneralResultFunc)
            : base(calculation, getGeneralResultFunc) {}
    }
}