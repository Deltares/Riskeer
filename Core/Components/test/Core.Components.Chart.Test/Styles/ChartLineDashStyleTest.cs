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
using Core.Components.Chart.Styles;
using NUnit.Framework;

namespace Core.Components.Chart.Test.Styles
{
    [TestFixture]
    public class ChartLineDashStyleTest : EnumWithResourcesDisplayNameTestFixture<ChartLineDashStyle>
    {
        protected override IDictionary<ChartLineDashStyle, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<ChartLineDashStyle, string>
                {
                    {
                        ChartLineDashStyle.Solid, "Doorgetrokken"
                    },
                    {
                        ChartLineDashStyle.Dash, "Onderbroken"
                    },
                    {
                        ChartLineDashStyle.Dot, "Gestippeld"
                    },
                    {
                        ChartLineDashStyle.DashDot, "Streep-stip"
                    },
                    {
                        ChartLineDashStyle.DashDotDot, "Streep-stip-stip"
                    }
                };
            }
        }

        protected override IDictionary<ChartLineDashStyle, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ChartLineDashStyle, int>
                {
                    {
                        ChartLineDashStyle.Solid, 0
                    },
                    {
                        ChartLineDashStyle.Dash, 1
                    },
                    {
                        ChartLineDashStyle.Dot, 2
                    },
                    {
                        ChartLineDashStyle.DashDot, 3
                    },
                    {
                        ChartLineDashStyle.DashDotDot, 4
                    }
                };
            }
        }
    }
}