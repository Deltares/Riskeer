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

using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Data.Structures
{
    /// <summary>
    /// This class holds the information for a calculation scenario.
    /// </summary>
    public class StructuresCalculationScenario<T> : StructuresCalculation<T>, ICalculationScenario
        where T : IStructuresCalculationInput, new()
    {
        private RoundedDouble contribution;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresCalculationScenario{T}"/>.
        /// </summary>
        public StructuresCalculationScenario()
        {
            IsRelevant = true;
            contribution = new RoundedDouble(4, 1);
        }

        public bool IsRelevant { get; set; }

        public RoundedDouble Contribution
        {
            get => contribution;
            set => contribution = value.ToPrecision(contribution.NumberOfDecimalPlaces);
        }
    }
}