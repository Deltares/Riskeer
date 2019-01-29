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
using Core.Components.Gis.Style;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Style
{
    [TestFixture]
    public class PointSymbolTest : EnumWithResourcesDisplayNameTestFixture<PointSymbol>
    {
        protected override IDictionary<PointSymbol, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<PointSymbol, string>
                {
                    {
                        PointSymbol.Circle, "Cirkel"
                    },
                    {
                        PointSymbol.Square, "Vierkant"
                    },
                    {
                        PointSymbol.Diamond, "Ruit"
                    },
                    {
                        PointSymbol.Triangle, "Driehoek"
                    },
                    {
                        PointSymbol.Star, "Ster"
                    },
                    {
                        PointSymbol.Hexagon, "Zeshoek"
                    },
                    {
                        PointSymbol.Pentagon, "Vijfhoek"
                    }
                };
            }
        }

        protected override IDictionary<PointSymbol, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<PointSymbol, int>
                {
                    {
                        PointSymbol.Circle, 0
                    },
                    {
                        PointSymbol.Square, 1
                    },
                    {
                        PointSymbol.Triangle, 2
                    },
                    {
                        PointSymbol.Diamond, 3
                    },
                    {
                        PointSymbol.Star, 4
                    },
                    {
                        PointSymbol.Hexagon, 5
                    },
                    {
                        PointSymbol.Pentagon, 6
                    }
                };
            }
        }
    }
}