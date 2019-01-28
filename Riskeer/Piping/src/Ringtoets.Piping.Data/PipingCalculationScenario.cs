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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class holds the information for a calculation scenario.
    /// </summary>
    public class PipingCalculationScenario : PipingCalculation, ICalculationScenario
    {
        private RoundedDouble contribution;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationScenario"/> with default values set for some of the parameters.
        /// </summary>
        /// <param name="generalInputParameters">General piping calculation parameters that
        /// are the same across all piping calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalInputParameters"/> is <c>null</c>.</exception>
        public PipingCalculationScenario(GeneralPipingInput generalInputParameters)
            : base(generalInputParameters)
        {
            IsRelevant = true;
            contribution = new RoundedDouble(4, 1.0);
        }

        public bool IsRelevant { get; set; }

        public RoundedDouble Contribution
        {
            get
            {
                return contribution;
            }
            set
            {
                contribution = value.ToPrecision(contribution.NumberOfDecimalPlaces);
            }
        }

        public CalculationScenarioStatus Status
        {
            get
            {
                return Output == null
                           ? CalculationScenarioStatus.NotCalculated
                           : CalculationScenarioStatus.Done;
            }
        }
    }
}