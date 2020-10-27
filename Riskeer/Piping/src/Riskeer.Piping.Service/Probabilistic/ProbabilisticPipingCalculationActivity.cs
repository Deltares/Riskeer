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
using Riskeer.Common.Service;
using Riskeer.Piping.Data.Probabilistic;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;

namespace Riskeer.Piping.Service.Probabilistic
{
    /// <summary>
    /// <see cref="CalculatableActivity"/> for running a <see cref="ProbabilisticPipingCalculation"/>.
    /// </summary>
    public class ProbabilisticPipingCalculationActivity : CalculatableActivity
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="ProbabilisticPipingCalculation"/> to perform.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        public ProbabilisticPipingCalculationActivity(ProbabilisticPipingCalculation calculation)
            : base(calculation)
        {
            Description = string.Format(RiskeerCommonServiceResources.Perform_calculation_with_name_0_, calculation.Name);
        }
        
        protected override void OnCancel()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnFinish()
        {
            throw new System.NotImplementedException();
        }

        protected override void PerformCalculation()
        {
            throw new System.NotImplementedException();
        }

        protected override bool Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}