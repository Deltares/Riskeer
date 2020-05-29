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
using System.ComponentModel;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="TCalculationScenario"/> in the <see cref="ScenariosView"/>.
    /// </summary>
    /// <typeparam name="TCalculationScenario">The type of calculation scenario.</typeparam>
    public abstract class ScenarioRow<TCalculationScenario>
        where TCalculationScenario : class, ICalculationScenario
    {
        /// <summary>
        /// Creates a new instance of <see cref="ScenarioRow{TCalculation}"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="TCalculationScenario"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationScenario"/>
        /// is <c>null</c>.</exception>
        protected ScenarioRow(TCalculationScenario calculationScenario)
        {
            if (calculationScenario == null)
            {
                throw new ArgumentNullException(nameof(calculationScenario));
            }

            CalculationScenario = calculationScenario;
        }

        /// <summary>
        /// Gets the <see cref="TCalculationScenario"/> this row contains.
        /// </summary>
        public TCalculationScenario CalculationScenario { get; }

        /// <summary>
        /// Gets or sets whether <see cref="TCalculationScenario"/> is relevant.
        /// </summary>
        public bool IsRelevant
        {
            get => CalculationScenario.IsRelevant;
            set
            {
                CalculationScenario.IsRelevant = value;
                CalculationScenario.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the contribution of the <see cref="TCalculationScenario"/>.
        /// </summary>
        public RoundedDouble Contribution
        {
            get => new RoundedDouble(2, CalculationScenario.Contribution * 100);
            set
            {
                CalculationScenario.Contribution = (RoundedDouble) (value / 100);
                CalculationScenario.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the name of the <see cref="TCalculationScenario"/>.
        /// </summary>
        public string Name => CalculationScenario.Name;

        /// <summary>
        /// Gets the failure probability of the <see cref="TCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public abstract string FailureProbability { get; }
    }
}