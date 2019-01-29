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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Riskeer.Common.Util
{
    /// <summary>
    /// This class is used to be able to assign calculations to failure mechanism section results without having
    /// to keep in mind the underlying types for either calculations or section results.
    /// </summary>
    public class SectionResultWithCalculationAssignment
    {
        private readonly Action<FailureMechanismSectionResult, ICalculation> setCalculationAction;
        private readonly Func<FailureMechanismSectionResult, ICalculation> getCalculationAction;

        /// <summary>
        /// Creates  new instance of <see cref="SectionResultWithCalculationAssignment"/>.
        /// </summary>
        /// <param name="result">The result which can have a calculation assigned.</param>
        /// <param name="getCalculationAction">The <see cref="Func{T,T}"/> to get an <see cref="ICalculation"/>
        /// from the <paramref name="result"/>.</param>
        /// <param name="setCalculationAction">The <see cref="Action{T,T}"/> to set an <see cref="ICalculation"/>
        /// to the <paramref name="result"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters is <c>null</c>.</exception>
        public SectionResultWithCalculationAssignment(
            FailureMechanismSectionResult result,
            Func<FailureMechanismSectionResult, ICalculation> getCalculationAction,
            Action<FailureMechanismSectionResult, ICalculation> setCalculationAction)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (getCalculationAction == null)
            {
                throw new ArgumentNullException(nameof(getCalculationAction));
            }

            if (setCalculationAction == null)
            {
                throw new ArgumentNullException(nameof(setCalculationAction));
            }

            Result = result;
            this.getCalculationAction = getCalculationAction;
            this.setCalculationAction = setCalculationAction;
        }

        /// <summary>
        /// Gets the <see cref="FailureMechanismSectionResult"/> for which the assignment object is defined.
        /// </summary>
        public FailureMechanismSectionResult Result { get; }

        /// <summary>
        /// Gets or sets the <see cref="ICalculation"/> from the <see cref="Result"/> using the actions passed
        /// to the constructor.
        /// </summary>
        internal ICalculation Calculation
        {
            get
            {
                return getCalculationAction(Result);
            }
            set
            {
                setCalculationAction(Result, value);
            }
        }
    }
}