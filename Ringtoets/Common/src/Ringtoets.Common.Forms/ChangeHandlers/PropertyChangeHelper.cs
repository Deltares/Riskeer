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
using Core.Common.Base;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// This helper can be used to easily notify observers of all objects that were affected by a value change of a property.
    /// </summary>
    public static class PropertyChangeHelper
    {
        /// <summary>
        /// Changes the property value using the <paramref name="setPropertyDelegate"/> and then notifies the observers of the objects
        /// that were affected by the change.
        /// </summary>
        /// <param name="setPropertyDelegate">The property change action.</param>
        /// <param name="changeHandler">The handler which is responsible for determining changes due to the property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void ChangePropertyAndNotify(SetObservablePropertyValueDelegate setPropertyDelegate, IObservablePropertyChangeHandler changeHandler)
        {
            if (setPropertyDelegate == null)
            {
                throw new ArgumentNullException(nameof(setPropertyDelegate));
            }

            if (changeHandler == null)
            {
                throw new ArgumentNullException(nameof(changeHandler));
            }

            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(setPropertyDelegate);
            NotifyAffectedObjects(affectedObjects);
        }

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }
    }
}