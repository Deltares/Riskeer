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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure calculation input.
    /// </summary>
    /// <typeparam name="TInput">The type of calculation input wrapped by the context object.</typeparam>
    /// <typeparam name ="TCalculation">The type of the calculation containing the calculation input.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism which the context belongs to.</typeparam>
    public abstract class InputContextBase<TInput, TCalculation, TFailureMechanism> : FailureMechanismItemContextBase<TInput, TFailureMechanism>
        where TInput : ICalculationInput
        where TCalculation : ICalculation
        where TFailureMechanism : IFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputContextBase{TData,TCalculation,TFailureMechanism}"/> class.
        /// </summary>
        /// <param name="wrappedData">The calculation input wrapped by the context object.</param>
        /// <param name="calculation">The calculation containing the calculation input.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        protected InputContextBase(TInput wrappedData, TCalculation calculation, TFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(wrappedData, failureMechanism, assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            Calculation = calculation;
        }

        /// <summary>
        /// Gets the calculation item which the context belongs to.
        /// </summary>
        public TCalculation Calculation { get; }
    }
}