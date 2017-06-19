// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;

namespace Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints
{
    public class FaultTreeIllustrationPoint : IIllustrationPoint
    {
        private IIllustrationPoint[] children = new IIllustrationPoint[2];

        public IEnumerable<IIllustrationPoint> Children
        {
            get
            {
                return children;
            }
        }

        public string Combine { get; set; }

        public void AddChild(IIllustrationPoint point)
        {
            if (ReferenceEquals(children[0], point) || ReferenceEquals(children[1], point))
            {
                return;
            }
            if (children[0] == null)
            {
                children[0] = point;
            }
            else if (children[1] == null)
            {
                children[1] = point;
            }
            else
            {
                throw new InvalidOperationException("Kan niet meer dan 2 kinderen toevoegen aan een foutenboomknoop.");
            }
        }
    }
}