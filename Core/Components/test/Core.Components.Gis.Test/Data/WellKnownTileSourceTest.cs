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

using System.Collections.Generic;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class WellKnownTileSourceTest : EnumWithResourcesDisplayNameTestFixture<WellKnownTileSource>
    {
        protected override IDictionary<WellKnownTileSource, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<WellKnownTileSource, string>
                {
                    {
                        WellKnownTileSource.OpenStreetMap, "OpenStreetMap"
                    },
                    {
                        WellKnownTileSource.BingAerial, "Bing Maps - Satelliet"
                    },
                    {
                        WellKnownTileSource.BingHybrid, "Bing Maps - Satelliet + Wegen"
                    },
                    {
                        WellKnownTileSource.BingRoads, "Bing Maps - Wegen"
                    },
                    {
                        WellKnownTileSource.EsriWorldTopo, "Esri World - Topografisch"
                    },
                    {
                        WellKnownTileSource.EsriWorldShadedRelief, "Esri World - Reliëf"
                    }
                };
            }
        }

        protected override IDictionary<WellKnownTileSource, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<WellKnownTileSource, int>
                {
                    {
                        WellKnownTileSource.OpenStreetMap, 1
                    },
                    {
                        WellKnownTileSource.BingAerial, 2
                    },
                    {
                        WellKnownTileSource.BingHybrid, 3
                    },
                    {
                        WellKnownTileSource.BingRoads, 4
                    },
                    {
                        WellKnownTileSource.EsriWorldTopo, 5
                    },
                    {
                        WellKnownTileSource.EsriWorldShadedRelief, 6
                    }
                };
            }
        }
    }
}