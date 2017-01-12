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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Plugin;
using NUnit.Framework;

namespace Core.Common.Gui.TestUtil
{
    /// <summary>
    /// Contains routines for testing classes which are related to GUI.
    /// </summary>
    public static class PluginTestHelper
    {
        /// <summary>
        /// Asserts that the given <paramref name="propertyInfos"/> contains a definition for the combination of
        /// <paramref name="dataObjectType"/> and <paramref name="propertyObjectType"/>. 
        /// </summary>
        /// <param name="propertyInfos">Collection of <see cref="PropertyInfo"/> definitions.</param>
        /// <param name="dataObjectType">The type of the data object for which property info is defined.</param>
        /// <param name="propertyObjectType">The type of the object which shows the data object properties.</param>
        /// <returns>The found property info.</returns>
        /// <exception cref="AssertionException">Thrown when the <paramref name="propertyInfos"/> is <c>null</c>
        /// or does not contain a definition for the combination of <paramref name="dataObjectType"/> and 
        /// <paramref name="propertyObjectType"/>.</exception>
        public static PropertyInfo AssertPropertyInfoDefined(IEnumerable<PropertyInfo> propertyInfos, Type dataObjectType, Type propertyObjectType)
        {
            Assert.NotNull(propertyInfos, "The given collection of propertyInfos was undefined.");
            var propertyInfo = propertyInfos.FirstOrDefault(
                tni =>
                tni.DataType == dataObjectType &&
                tni.PropertyObjectType == propertyObjectType);
            Assert.NotNull(propertyInfo, $"The property info object was not found for the given dataType ({dataObjectType}) " +
                                         $"and propertyObjectType ({propertyObjectType}).");
            return propertyInfo;
        }

        /// <summary>
        /// Asserts that a view info is defined in the collection of view infos given. 
        /// </summary>
        /// <param name="viewInfos">The collection of <see cref="ViewInfo"/> to search in.</param>
        /// <param name="dataType">The type of the data which is passed to the <see cref="ViewInfo"/>.</param>
        /// <param name="viewDataType">The type of the data which is set on the view.</param>
        /// <param name="viewType">The type of the view.</param>
        /// <returns>The <see cref="ViewInfo"/> that was found within the collection of <see cref="viewInfos"/>.</returns>
        /// <exception cref="AssertionException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="viewInfos"/> is <c>null</c></item>
        /// <item>no <see cref="ViewInfo"/> can be found for type <paramref name="dataType"/></item>
        /// <item>the found <see cref="ViewInfo"/> does not define the expected <paramref name="viewDataType"/> or 
        /// <paramref name="viewType"/></item>
        /// </list></exception>
        public static ViewInfo AssertViewInfoDefined(IEnumerable<ViewInfo> viewInfos, Type dataType, Type viewDataType, Type viewType)
        {
            Assert.NotNull(viewInfos);
            var viewInfo = viewInfos.SingleOrDefault(vi => vi.DataType == dataType);
            Assert.NotNull(viewInfo, "Could not find viewInfo for the dataType {0}", dataType);
            Assert.AreEqual(viewDataType, viewInfo.ViewDataType);
            Assert.AreEqual(viewType, viewInfo.ViewType);
            return viewInfo;
        }

        /// <summary>
        /// Asserts that a view info is defined in the collection of view infos given. 
        /// </summary>
        /// <param name="viewInfos">The collection of <see cref="ViewInfo"/> to search in.</param>
        /// <param name="dataType">The type of the data which is passed to the <see cref="ViewInfo"/> and is set on the view.</param>
        /// <param name="viewType">The type of the view.</param>
        /// <returns>The <see cref="ViewInfo"/> that was found within the collection of <see cref="viewInfos"/>.</returns>
        /// <exception cref="AssertionException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="viewInfos"/> is <c>null</c></item>
        /// <item>no <see cref="ViewInfo"/> can be found for type <paramref name="dataType"/></item>
        /// <item>the found <see cref="ViewInfo"/> does not define the expected <paramref name="viewType"/></item>
        /// </list></exception>
        public static ViewInfo AssertViewInfoDefined(IEnumerable<ViewInfo> viewInfos, Type dataType, Type viewType)
        {
            return AssertViewInfoDefined(viewInfos, dataType, dataType, viewType);
        }
    }
}