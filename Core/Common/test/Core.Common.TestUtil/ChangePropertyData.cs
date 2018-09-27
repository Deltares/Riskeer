// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Class to hold data to change a property of an object.
    /// </summary>
    /// <typeparam name="T">The object to change the data for.</typeparam>
    public class ChangePropertyData<T> where T : class
    {
        /// <summary>
        /// Instantiates a new <see cref="ChangePropertyData{T}"/>
        /// </summary>
        /// <param name="actionToChangeProperty">The action to perform to change the property.</param>
        /// <param name="propertyName">The name of the property that is changed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ChangePropertyData(Action<T> actionToChangeProperty,
                                  string propertyName)
        {
            if (actionToChangeProperty == null)
            {
                throw new ArgumentNullException(nameof(actionToChangeProperty));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            ActionToChangeProperty = actionToChangeProperty;
            PropertyName = propertyName;
        }

        /// <summary>
        /// The action to perform to change the property
        /// </summary>
        public Action<T> ActionToChangeProperty { get; }

        /// <summary>
        /// The name of the property that is changed.
        /// </summary>
        public string PropertyName { get; }
    }
}