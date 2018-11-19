﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ReferenceLine"/> instances
    /// which can be used for testing.
    /// </summary>
    public static class ReferenceLineTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLine"/> with a default geometry. 
        /// </summary>
        /// <returns>A <see cref="ReferenceLine"/>.</returns>
        public static ReferenceLine CreateReferenceLineWithGeometry()
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(CreateReferenceLineGeometry());
            return referenceLine;
        }

        /// <summary>
        /// Creates default geometry for a <see cref="ReferenceLine"/>.
        /// </summary>
        /// <returns>A geometry</returns>
        public static IEnumerable<Point2D> CreateReferenceLineGeometry()
        {
            return new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2),
                new Point2D(3, 3),
                new Point2D(4, 4)
            };
        }
    }
}