// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapDataCollectionVisibilityTest : EnumWithResourcesDisplayNameTestFixture<MapDataCollectionVisibility>
    {
        protected override IDictionary<MapDataCollectionVisibility, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<MapDataCollectionVisibility, int>
                {
                    {
                        MapDataCollectionVisibility.Visible, 1
                    },
                    {
                        MapDataCollectionVisibility.NotVisible, 2
                    },
                    {
                        MapDataCollectionVisibility.Mixed, 3
                    }
                };
            }
        }

        protected override IDictionary<MapDataCollectionVisibility, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<MapDataCollectionVisibility, string>
                {
                    {
                        MapDataCollectionVisibility.Visible, "Wel"
                    },
                    {
                        MapDataCollectionVisibility.NotVisible, "Niet"
                    },
                    {
                        MapDataCollectionVisibility.Mixed, "Deels"
                    }
                };
            }
        }
    }
}