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
using Core.Common.Base.Service;
using Riskeer.Common.Data.Calculation;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Base implementation of an <see cref="Activity"/> for running calculations
    /// of type <see cref="ICalculatable"/>.
    /// </summary>
    public abstract class CalculatableActivity : Activity
    {
        private readonly ICalculatable calculatable;

        /// <summary>
        /// Creates a new instance of <see cref="CalculatableActivity"/>.
        /// </summary>
        /// <param name="calculatable">The calculation to perform.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculatable"/> is <c>null</c>.</exception>
        protected CalculatableActivity(ICalculatable calculatable)
        {
            if (calculatable == null)
            {
                throw new ArgumentNullException(nameof(calculatable));
            }

            this.calculatable = calculatable;
        }

        protected override void OnRun()
        {
            if (!calculatable.ShouldCalculate)
            {
                State = ActivityState.Skipped;
            }
            else if (!Validate())
            {
                State = ActivityState.Failed;
            }
            else
            {
                PerformCalculation();
            }
        }

        /// <summary>
        /// Updates the progress text using the parameters in a predefined format.
        /// </summary>
        /// <param name="currentStepName">A short description of the current step.</param>
        /// <param name="currentStep">The number of the current step.</param>
        /// <param name="totalSteps">The total numbers of steps.</param>
        protected void UpdateProgressText(string currentStepName, int currentStep, int totalSteps)
        {
            ProgressText = string.Format(CoreCommonBaseResources.Activity_UpdateProgressText_CurrentStepNumber_0_of_TotalStepsNumber_1_StepDescription_2_,
                                         currentStep, totalSteps, currentStepName);
        }

        /// <summary>
        /// Performs the calculation. May throw exceptions, which will result in a <see cref="ActivityState.Failed"/>
        /// state for the <see cref="CalculatableActivity"/>.
        /// </summary>
        protected abstract void PerformCalculation();

        /// <summary>
        /// Performs validation over the input of the <see cref="CalculatableActivity"/>. If the input is not valid
        /// then <c>false</c> is returned and the problems are logged.
        /// </summary>
        /// <returns><c>true</c> if no validation problems were found, <c>false</c> otherwise.</returns>
        protected abstract bool Validate();
    }
}