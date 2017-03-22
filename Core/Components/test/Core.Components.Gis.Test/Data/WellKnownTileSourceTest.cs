﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
    public class WellKnownTileSourceTest : EnumTestFixture<WellKnownTileSource>
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

        protected override IDictionary<WellKnownTileSource, byte> ExpectedValueForEnumValues {
            get
            {
                return new Dictionary<WellKnownTileSource, byte>
                {
                    {
                        WellKnownTileSource.OpenStreetMap, 0
                    },
                    {
                        WellKnownTileSource.BingAerial, 1
                    },
                    {
                        WellKnownTileSource.BingHybrid, 2
                    },
                    {
                        WellKnownTileSource.BingRoads, 3
                    },
                    {
                        WellKnownTileSource.EsriWorldTopo, 4
                    },
                    {
                        WellKnownTileSource.EsriWorldShadedRelief, 5
                    }
                };
            }
        }
    }
}