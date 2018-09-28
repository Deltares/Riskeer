// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;

namespace Core.Common.IO.Readers
{
    /// <summary>
    /// This class can be used in importers to return a result from a method where some critical error
    /// may have occurred. The type of items which are collected is supplied by <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items which are returned in this result as <see cref="IEnumerable{T}"/>.</typeparam>
    public class ReadResult<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadResult{T}"/>, for which the <see cref="CriticalErrorOccurred"/>
        /// is set to <paramref name="errorOccurred"/>.
        /// </summary>
        /// <param name="errorOccurred"><see cref="bool"/> value indicating whether an error has occurred while collecting
        /// the items for this <see cref="ReadResult{T}"/>.</param>
        public ReadResult(bool errorOccurred)
        {
            CriticalErrorOccurred = errorOccurred;
            Items = new T[0];
        }

        /// <summary>
        /// Gets or sets the <see cref="IEnumerable{T}"/> of items that were read.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets the <see cref="bool"/> representing whether an critical error has occurred during
        /// read.
        /// </summary>
        public bool CriticalErrorOccurred { get; }
    }
}