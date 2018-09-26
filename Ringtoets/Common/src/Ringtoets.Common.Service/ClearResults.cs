// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Class that stores the affected objects by a clear action.
    /// </summary>
    public class ClearResults
    {
        /// <summary>
        /// Creates a new instances of <see cref="ClearResults"/>.
        /// </summary>
        /// <param name="changedObjects">All objects that have been changed by the clear action.</param>
        /// <param name="removedObjects">All objects that have been removed by the clear action.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public ClearResults(IEnumerable<IObservable> changedObjects, IEnumerable<object> removedObjects)
        {
            if (changedObjects == null)
            {
                throw new ArgumentNullException(nameof(changedObjects));
            }

            if (removedObjects == null)
            {
                throw new ArgumentNullException(nameof(removedObjects));
            }

            ChangedObjects = changedObjects;
            RemovedObjects = removedObjects;
        }

        /// <summary>
        /// The changed objects.
        /// </summary>
        public IEnumerable<IObservable> ChangedObjects { get; }

        /// <summary>
        /// The removed objects.
        /// </summary>
        public IEnumerable<object> RemovedObjects { get; }
    }
}