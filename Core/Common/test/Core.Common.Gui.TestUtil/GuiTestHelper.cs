// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Common.Gui.Plugin;
using NUnit.Framework;

namespace Core.Common.Gui.TestUtil
{
    /// <summary>
    /// Contains routines for testing classes which are related to GUI.
    /// </summary>
    public static class GuiTestHelper
    {
        /// <summary>
        /// Asserts that the given <paramref name="propertyInfos"/> contains a definition for the combination of
        /// <typeparamref name="TDataObject"/> and <typeparamref name="TPropertyObject"/>. 
        /// </summary>
        /// <typeparam name="TDataObject">The type of the data object for which property info is defined.</typeparam>
        /// <typeparam name="TPropertyObject">The type of the object which shows the data object properties.</typeparam>
        /// <param name="propertyInfos">Collection of <see cref="PropertyInfo"/> definitions.</param>
        /// <returns>The found property info.</returns>
        /// <exception cref="AssertionException">Thrown when the <paramref name="propertyInfos"/> is <c>null</c>
        /// or does not contain a defintion for the combination of <typeparamref name="TDataObject"/> and 
        /// <typeparamref name="TPropertyObject"/>.</exception>
        public static PropertyInfo AssertPropertyInfoDefined<TDataObject, TPropertyObject>(PropertyInfo[] propertyInfos)
        {
            Assert.NotNull(propertyInfos);
            var propertyInfo = propertyInfos.FirstOrDefault(
                tni => 
                tni.DataType == typeof(TDataObject) && 
                tni.PropertyObjectType == typeof(TPropertyObject)
                );
            Assert.NotNull(propertyInfo);
            return propertyInfo;
        }
    }
}