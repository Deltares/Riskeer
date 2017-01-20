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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Utils;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// A collection to store the <see cref="StochasticSoilModel"/> and the source path
    /// they were imported from.
    /// </summary>
    public class StochasticSoilModelCollection : Observable, IEnumerable<StochasticSoilModel>
    {
        private readonly List<StochasticSoilModel> stochasticSoilModels = new List<StochasticSoilModel>();

        /// <summary>
        /// Gets the element at index <paramref name="i"/> in the collection.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The element at index <paramref name="i"/> in the collection.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="i"/> is not 
        /// between [0, <see cref="Count"/>)</exception>
        public StochasticSoilModel this[int i]
        {
            get
            {
                return stochasticSoilModels[i];
            }
        }

        /// <summary>
        /// Gets the amount of items stored in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return stochasticSoilModels.Count;
            }
        }

        /// <summary>
        /// Gets the last known file path from which the <see cref="StochasticSoilModel"/>
        /// elements were imported.
        /// </summary>
        /// <returns>The path where the <see cref="StochasticSoilModel"/> elements originate
        /// from, or <c>null</c> if the collection is cleared.</returns>
        public string SourcePath { get; private set; }

        /// <summary>
        /// Removes the first occurrence of <paramref name="model"/> in the collection.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilModel"/> to be removed.</param>
        /// <returns><c>True</c> if the <paramref name="model"/> was successfully removed from the collection;
        /// <c>False</c> if otherwise or if the <paramref name="model"/> was not found in the collection. </returns>
        public bool Remove(StochasticSoilModel model)
        {
            bool remove = stochasticSoilModels.Remove(model);
            if (remove && Count == 0)
            {
                SourcePath = null;
            }
            return remove;
        }

        /// <summary>
        /// Clears the imported items in the collection and the <see cref="SourcePath"/>.
        /// </summary>
        public void Clear()
        {
            SourcePath = null;
            stochasticSoilModels.Clear();
        }

        /// <summary>
        /// Adds all <see cref="StochasticSoilModel"/> elements originating from a source file.
        /// </summary>
        /// <param name="soilModels">The <see cref="StochasticSoilModel"/></param>
        /// <param name="filePath">The path to the source file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="soilModels"/> contains <c>null</c>.</item>
        /// <item><paramref name="filePath"/> is not a valid file path.</item>
        /// </list>
        /// </exception>
        public void AddRange(IEnumerable<StochasticSoilModel> soilModels, string filePath)
        {
            if (soilModels == null)
            {
                throw new ArgumentNullException(nameof(soilModels));
            }
            if (soilModels.Contains(null))
            {
                throw new ArgumentException("Collection cannot contain null.", nameof(soilModels));
            }
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (!FileUtils.IsValidFilePath(filePath))
            {
                throw new ArgumentException($"'{filePath}' is not a valid filepath.", nameof(filePath));
            }

            SourcePath = filePath;
            stochasticSoilModels.AddRange(soilModels);
        }

        public IEnumerator<StochasticSoilModel> GetEnumerator()
        {
            return stochasticSoilModels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}