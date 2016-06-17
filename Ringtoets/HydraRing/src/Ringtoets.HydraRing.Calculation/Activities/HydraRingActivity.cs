// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Service;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Activities
{
    /// <summary>
    /// <see cref="Activity"/> for running calculations via Hydra-Ring.
    /// </summary>
    public abstract class HydraRingActivity<T> : Activity
    {
        /// <summary>
        /// The output of the calculation.
        /// </summary>
        protected T Output;

        protected abstract override void OnRun();

        protected override void OnCancel()
        {
            HydraRingCalculationService.CancelRunningCalculation();
        }

        protected abstract override void OnFinish();

        /// <summary>
        /// Method for performing the run of the activity. The calculation will be validated
        /// and after the validation is successful, the <paramref name="clearAction"/> will be performed.
        /// After that the calculation will be performed. Error and status information is logged during 
        /// the execution of the operation.
        /// </summary>
        /// <param name="validationFunc">The method to perform for validation.</param>
        /// <param name="clearAction">The method to perform for clearing the data of the output to set.</param>
        /// <param name="calculationFunc">The method to perform for the calculation.</param>
        protected void PerformRun(Func<bool> validationFunc, Action clearAction, Func<T> calculationFunc)
        {
            if (!validationFunc())
            {
                State = ActivityState.Failed;
                return;
            }

            LogMessages.Clear();
            clearAction();

            Output = calculationFunc();

            if (Output == null)
            {
                State = ActivityState.Failed;
            }
        }

        /// <summary>
        /// Method for performing the finish of the activity. If the calculation is successfull
        /// executed, the output will be set on the calculation. After that the observers of 
        /// <paramref name="observableObject"/> will be notified.
        /// </summary>
        /// <param name="setOutputAction">The method to set the output on the object.</param>
        /// <param name="observableObject">The object to notify the observers upon.</param>
        protected void PerformFinish(Action setOutputAction, IObservable observableObject)
        {
            if (State == ActivityState.Executed)
            {
                setOutputAction();
                observableObject.NotifyObservers();
            }
        }
    }
}