// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Factory for creating valid <see cref="FailureMechanismSection"/> which can 
    /// be used for testing.
    /// </summary>
    public static class FailureMechanismSectionTestFactory
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <returns>A valid <see cref="FailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <param name="name"></param>
        /// is <c>null</c>.</exception>
        public static FailureMechanismSection CreateFailureMechanismSection(string name = "test")
        {
            return CreateFailureMechanismSection(name, new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 0)
            });
        }

        /// <summary>
        /// Creates a <see cref="FailureMechanismSection"/> with defined coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates of the <see cref="FailureMechanismSection"/></param>
        /// <returns>A valid <see cref="FailureMechanismSection"/> with the specified coordinates.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinates"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="coordinates"/> does not have at least one geometry point.</item>
        /// <item>One or more <paramref name="coordinates"/> elements are <c>null</c>.</item>
        /// </list> </exception>
        public static FailureMechanismSection CreateFailureMechanismSection(IEnumerable<Point2D> coordinates)
        {
            if (coordinates == null)
            {
                throw new ArgumentNullException(nameof(coordinates));
            }

            return CreateFailureMechanismSection("test", coordinates);
        }

        /// <summary>
        /// Creates a <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="coordinates">The coordinates of the section.</param>
        /// <returns>A valid <see cref="FailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="coordinates"/> does not have at least one geometry point.</item>
        /// <item>One or more <paramref name="coordinates"/> elements are <c>null</c>.</item>
        /// </list> </exception>
        private static FailureMechanismSection CreateFailureMechanismSection(string name, IEnumerable<Point2D> coordinates)
        {
            return new FailureMechanismSection(name, coordinates);
        }
    }
}