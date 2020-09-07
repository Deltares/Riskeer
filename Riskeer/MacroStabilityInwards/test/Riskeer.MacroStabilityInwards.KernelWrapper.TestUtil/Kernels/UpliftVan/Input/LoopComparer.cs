// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input
{
    /// <summary>
    /// This class compares the coordinates of the <see cref="Point2D"/>
    /// of all <see cref="Curve"/> instances of the <see cref="Loop"/>
    /// instances to determine whether they're equal to each other or not.
    /// </summary>
    public class LoopComparer : IComparer<Loop>, IComparer
    {
        private readonly CurveComparer curveComparer = new CurveComparer();

        public int Compare(object x, object y)
        {
            return Compare(x as Loop, y as Loop);
        }

        public int Compare(Loop x, Loop y)
        {
            return x.Curves.Where((curve, index) =>
                                         curveComparer.Compare(curve, y.Curves.ElementAt(index)) == 1).Any()
                       ? 1
                       : 0;
        }
    }
}