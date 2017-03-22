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

using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class RingtoetsWellKnownTileSourceTest : EnumTestFixture<RingtoetsWellKnownTileSource>
    {
        protected override IDictionary<RingtoetsWellKnownTileSource, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<RingtoetsWellKnownTileSource, string>
                {
                    {
                        RingtoetsWellKnownTileSource.OpenStreetMap, "OpenStreetMap"
                    },
                    {
                        RingtoetsWellKnownTileSource.BingAerial, "Bing Maps - Satelliet"
                    },
                    {
                        RingtoetsWellKnownTileSource.BingHybrid, "Bing Maps - Satelliet + Wegen"
                    },
                    {
                        RingtoetsWellKnownTileSource.BingRoads, "Bing Maps - Wegen"
                    },
                    {
                        RingtoetsWellKnownTileSource.EsriWorldTopo, "Esri World - Topografisch"
                    },
                    {
                        RingtoetsWellKnownTileSource.EsriWorldShadedRelief, "Esri World - Reliëf"
                    }
                };
            }
        }

        protected override IDictionary<RingtoetsWellKnownTileSource, byte> ExpectedValueForEnumValues {
            get
            {
                return new Dictionary<RingtoetsWellKnownTileSource, byte>
                {
                    {
                        RingtoetsWellKnownTileSource.OpenStreetMap, 1
                    },
                    {
                        RingtoetsWellKnownTileSource.BingAerial, 2
                    },
                    {
                        RingtoetsWellKnownTileSource.BingHybrid, 3
                    },
                    {
                        RingtoetsWellKnownTileSource.BingRoads, 4
                    },
                    {
                        RingtoetsWellKnownTileSource.EsriWorldTopo, 5
                    },
                    {
                        RingtoetsWellKnownTileSource.EsriWorldShadedRelief, 6
                    }
                };
            }
        }
    }
}