// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Components.Chart.Styles;
using NUnit.Framework;

namespace Core.Components.Chart.Test.Data
{
    [TestFixture]
    public class ChartPointSymbolTest : EnumWithResourcesDisplayNameTestFixture<ChartPointSymbol>
    {
        protected override IDictionary<ChartPointSymbol, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<ChartPointSymbol, string>
                {
                    {
                        ChartPointSymbol.Circle, "Cirkel"
                    },
                    {
                        ChartPointSymbol.Square, "Vierkant"
                    },
                    {
                        ChartPointSymbol.Diamond, "Ruit"
                    },
                    {
                        ChartPointSymbol.Triangle, "Driehoek"
                    },
                    {
                        ChartPointSymbol.Star, "Ster"
                    },
                    {
                        ChartPointSymbol.Cross, "Kruis"
                    },
                    {
                        ChartPointSymbol.Plus, "Plus"
                    }
                };
            }
        }

        protected override IDictionary<ChartPointSymbol, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ChartPointSymbol, int>
                {
                    {
                        ChartPointSymbol.Circle, 0
                    },
                    {
                        ChartPointSymbol.Square, 1
                    },
                    {
                        ChartPointSymbol.Diamond, 2
                    },
                    {
                        ChartPointSymbol.Triangle, 3
                    },
                    {
                        ChartPointSymbol.Star, 4
                    },
                    {
                        ChartPointSymbol.Cross, 5
                    },
                    {
                        ChartPointSymbol.Plus, 6
                    }
                };
            }
        }
    }
}