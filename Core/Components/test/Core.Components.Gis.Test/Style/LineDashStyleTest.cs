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
using Core.Components.Gis.Style;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Style
{
    [TestFixture]
    public class LineDashStyleTest : EnumWithResourcesDisplayNameTestFixture<LineDashStyle>
    {
        protected override IDictionary<LineDashStyle, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<LineDashStyle, string>
                {
                    {
                        LineDashStyle.Solid, "Doorgetrokken"
                    },
                    {
                        LineDashStyle.Dash, "Onderbroken"
                    },
                    {
                        LineDashStyle.Dot, "Gestippeld"
                    },
                    {
                        LineDashStyle.DashDot, "Streep-stip"
                    },
                    {
                        LineDashStyle.DashDotDot, "Streep-stip-stip"
                    }
                };
            }
        }

        protected override IDictionary<LineDashStyle, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<LineDashStyle, int>
                {
                    {
                        LineDashStyle.Solid, 0
                    },
                    {
                        LineDashStyle.Dash, 1
                    },
                    {
                        LineDashStyle.Dot, 2
                    },
                    {
                        LineDashStyle.DashDot, 3
                    },
                    {
                        LineDashStyle.DashDotDot, 4
                    }
                };
            }
        }
    }
}