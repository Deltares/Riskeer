// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using Core.Components.Gis.Features;
using NUnit.Framework;

namespace Riskeer.Common.Util.TestUtil
{
    /// <summary>
    /// Helper methods for asserting map feature meta data.
    /// </summary>
    public static class MapFeaturesMetaDataTestHelper
    {
        /// <summary>
        /// Asserts whether the meta data for <paramref name="key"/> in <paramref name="feature"/>
        /// contains the correct value.
        /// </summary>
        /// <param name="expectedValue">The value to assert against.</param>
        /// <param name="feature">The <see cref="MapFeature"/> to be asserted.</param>
        /// <param name="key">The name of the meta data element to retrieve the value from.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the meta data of <paramref name="feature"/> does not 
        /// contain a <see cref="KeyValuePair{TKey,TValue}"/> with <paramref name="key"/> as key.</exception>
        /// <exception cref="AssertionException">Thrown when the value and the respective meta data value associated
        /// with <paramref name="key"/> are not equal.
        /// </exception>
        public static void AssertMetaData(string expectedValue,
                                          MapFeature feature,
                                          string key)
        {
            Assert.AreEqual(expectedValue, feature.MetaData[key]);
        }
    }
}