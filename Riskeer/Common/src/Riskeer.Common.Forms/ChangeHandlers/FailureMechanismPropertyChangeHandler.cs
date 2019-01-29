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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Class which properly handles data model changes due to a change of a
    /// failure mechanism property.
    /// </summary>
    public class FailureMechanismPropertyChangeHandler<T> : IFailureMechanismPropertyChangeHandler<T> where T : IFailureMechanism
    {
        public IEnumerable<IObservable> SetPropertyValueAfterConfirmation<TValue>(
            T failureMechanism,
            TValue value, SetFailureMechanismPropertyValueDelegate<T, TValue> setValue)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
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

            if (RequiresConfirmation(failureMechanism))
            {
                if (ConfirmPropertyChange())
                {
                    setValue(failureMechanism, value);
                    changedObjects.AddRange(PropertyChanged(failureMechanism));
                    changedObjects.Add(failureMechanism);
                }
            }
            else
            {
                setValue(failureMechanism, value);
                changedObjects.Add(failureMechanism);
            }

            return changedObjects;
        }

        /// <summary>
        /// Checks whether a call to <see cref="PropertyChanged"/> would have any effect in the given
        /// <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to check for.</param>
        /// <returns><c>true</c> if <see cref="PropertyChanged"/> would result in changes, 
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        protected virtual bool RequiresConfirmation(T failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations.Any(c => c.HasOutput);
        }

        /// <summary>
        /// Propagates the necessary changes to underlying data structure when a property has 
        /// been changed for a failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be updated.</param>
        /// <returns>All objects that have been affected by the change.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        protected virtual IEnumerable<IObservable> PropertyChanged(T failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affected = new List<IObservable>();
            foreach (ICalculation calculation in failureMechanism.Calculations.Where(c => c.HasOutput))
            {
                affected.Add(calculation);
                calculation.ClearOutput();
            }

            return affected;
        }

        /// <summary>
        /// Checks to see if the change of the failure mechanism property should occur or not.
        /// </summary>
        /// <returns><c>true</c> if the change should occur, <c>false</c> otherwise.</returns>
        private static bool ConfirmPropertyChange()
        {
            DialogResult result = MessageBox.Show(Resources.FailureMechanismPropertyChangeHandler_Confirm_change_composition_and_clear_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);

            return result == DialogResult.OK;
        }
    }
}