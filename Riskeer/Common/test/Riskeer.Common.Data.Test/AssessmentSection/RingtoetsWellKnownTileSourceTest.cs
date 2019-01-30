﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class RiskeerWellKnownTileSourceTest : EnumWithResourcesDisplayNameTestFixture<RingtoetsWellKnownTileSource>
    {
        protected override IDictionary<RingtoetsWellKnownTileSource, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<RingtoetsWellKnownTileSource, string>
                {
                    {
                        RingtoetsWellKnownTileSource.OpenStreetMap, null
                    },
                    {
                        RingtoetsWellKnownTileSource.BingAerial, null
                    },
                    {
                        RingtoetsWellKnownTileSource.BingHybrid, null
                    },
                    {
                        RingtoetsWellKnownTileSource.BingRoads, null
                    },
                    {
                        RingtoetsWellKnownTileSource.EsriWorldTopo, null
                    },
                    {
                        RingtoetsWellKnownTileSource.EsriWorldShadedRelief, null
                    }
                };
            }
        }

        protected override IDictionary<RingtoetsWellKnownTileSource, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<RingtoetsWellKnownTileSource, int>
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