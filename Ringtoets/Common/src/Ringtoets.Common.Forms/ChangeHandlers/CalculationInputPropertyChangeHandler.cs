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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Class which properly handles data model changes due to a change of a
    /// calculation input property.
    /// </summary>
    /// <typeparam name="TCalculationInput">The type of the calculation input.</typeparam>
    /// <typeparam name="TCalculation">The type of the calculation.</typeparam>
    public class CalculationInputPropertyChangeHandler<TCalculationInput, TCalculation>
        : ICalculationInputPropertyChangeHandler<TCalculationInput, TCalculation>
        where TCalculationInput : ICalculationInput
        where TCalculation : ICalculation
    {
        public IEnumerable<IObservable> SetPropertyValueAfterConfirmation<TValue>(
            TCalculationInput calculationInput,
            TCalculation calculation,
            TValue value,
            SetCalculationInputPropertyValueDelegate<TCalculationInput, TValue> setValue)
        {
            if (calculationInput == null)
            {
                throw new ArgumentNullException(nameof(calculationInput));
            }
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (setValue == null)
            {
                throw new ArgumentNullException(nameof(setValue));
            }

            var changedObjects = new List<IObservable>();

            if (RequiresConfirmation(calculation))
            {
                if (ConfirmPropertyChange())
                {
                    setValue(calculationInput, value);
                    PropertyChanged(calculation);
                    changedObjects.Add(calculation);
                    changedObjects.Add(calculationInput);
                }
            }
            else
            {
                setValue(calculationInput, value);
                changedObjects.Add(calculationInput);
            }

            return changedObjects;
        }

        private static void PropertyChanged(TCalculation calculation)
        {
            calculation.ClearOutput();
        }

        private static bool RequiresConfirmation(TCalculation calculation)
        {
            return calculation.HasOutput;
        }

        private static bool ConfirmPropertyChange()
        {
            DialogResult result = MessageBox.Show(
                Resources.CalculationInputPropertyChangeHandler_ConfirmPropertyChange_Confirm_change_input_parameter_and_clear_calculation_output,
                CoreCommonBaseResources.Confirm,
                MessageBoxButtons.OKCancel);

            return result == DialogResult.OK;
        }
    }
}