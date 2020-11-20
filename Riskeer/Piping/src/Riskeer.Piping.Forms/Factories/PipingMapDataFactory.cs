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

using System.Drawing;
using Core.Components.Gis.Data;
using Riskeer.Common.Forms.Factories;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Forms.Properties;

namespace Riskeer.Piping.Forms.Factories
{
    /// <summary>
    /// Piping specific factory for creating <see cref="FeatureBasedMapData"/> for data used as input in the assessment section.
    /// </summary>
    public static class PipingMapDataFactory
    {
        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="SemiProbabilisticPipingCalculationScenario"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateSemiProbabilisticCalculationsMapData()
        {
            return RiskeerMapDataFactory.CreateCalculationsMapData(Resources.PipingMapDataFactory_CreateSemiProbabilisticCalculationsMapData_Semi_Probabilistic_Calculations,
                                                                   Color.MediumPurple);
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with custom styling for collections of <see cref="ProbabilisticPipingCalculationScenario"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateProbabilisticCalculationsMapData()
        {
            return RiskeerMapDataFactory.CreateCalculationsMapData(Resources.PipingMapDataFactory_CreateProbabilisticCalculationsMapData_Probabilistis_Calculations,
                                                                   Color.Pink);
        }
    }
}