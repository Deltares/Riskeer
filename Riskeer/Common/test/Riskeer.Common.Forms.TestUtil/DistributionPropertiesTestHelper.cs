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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class for performing assertions over properties objects made for distributions.
    /// </summary>
    public static class DistributionPropertiesTestHelper
    {
        /// <summary>
        /// Assert the property read-only states of the <see cref="IDistribution"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IDistribution"/>.</typeparam>
        /// <param name="properties">The properties object for which to assert the read-only state of its properties.</param>
        /// <param name="meanReadOnly"><c>true</c> if the <see cref="IDistribution.Mean"/> property should be read-only,
        /// <c>false</c> otherwise.</param>
        /// <param name="deviationReadOnly"><c>true</c> if the <see cref="IDistribution.StandardDeviation"/> property should 
        /// be read-only, <c>false</c> otherwise.</param>
        public static void AssertPropertiesAreReadOnly<T>(
            DistributionPropertiesBase<T> properties,
            bool meanReadOnly,
            bool deviationReadOnly) where T : IDistribution
        {
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const int typePropertyIndex = 0;
            const int meanPropertyIndex = 1;
            const int deviationPropertyIndex = 2;

            Assert.AreEqual(true, dynamicProperties[typePropertyIndex].IsReadOnly);
            Assert.AreEqual(meanReadOnly, dynamicProperties[meanPropertyIndex].IsReadOnly);
            Assert.AreEqual(deviationReadOnly, dynamicProperties[deviationPropertyIndex].IsReadOnly);
        }

        /// <summary>
        /// Assert the property read-only states of the <see cref="IVariationCoefficientDistribution"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IVariationCoefficientDistribution"/>.</typeparam>
        /// <param name="properties">The properties object for which to assert the read-only state of its properties.</param>
        /// <param name="meanReadOnly"><c>true</c> if the <see cref="IVariationCoefficientDistribution.Mean"/> property 
        /// should be read-only, <c>false</c> otherwise.</param>
        /// <param name="variationCoefficientReadOnly"><c>true</c> if the 
        /// <see cref="IVariationCoefficientDistribution.CoefficientOfVariation"/> property should be read-only,
        /// <c>false</c> otherwise.</param>
        public static void AssertPropertiesAreReadOnly<T>(
            VariationCoefficientDistributionPropertiesBase<T> properties,
            bool meanReadOnly,
            bool variationCoefficientReadOnly) where T : IVariationCoefficientDistribution
        {
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const int typePropertyIndex = 0;
            const int meanPropertyIndex = 1;
            const int variationCoefficientPropertyIndex = 2;

            Assert.AreEqual(true, dynamicProperties[typePropertyIndex].IsReadOnly);
            Assert.AreEqual(meanReadOnly, dynamicProperties[meanPropertyIndex].IsReadOnly);
            Assert.AreEqual(variationCoefficientReadOnly, dynamicProperties[variationCoefficientPropertyIndex].IsReadOnly);
        }
    }
}