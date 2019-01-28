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
using Ringtoets.Common.Data.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.IllustrationPoints
{
    /// <summary>
    /// Factory to create simple <see cref="WindDirection"/> instances that can be used for testing.
    /// </summary>
    public static class WindDirectionTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="WindDirection"/> with arbitrary values for 
        /// wind direction name and angle.
        /// </summary>
        /// <returns>A <see cref="WindDirection"/> which can be readily used for testing.</returns>
        public static WindDirection CreateTestWindDirection()
        {
            return new WindDirection("SSE", 5.0);
        }

        /// <summary>
        /// Creates a new instance of <see cref="WindDirection"/> with a specified name.
        /// </summary>
        /// <param name="name">The name of the wind direction.</param>
        /// <returns>A <see cref="WindDirection"/> which can be readily used for testing.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        public static WindDirection CreateTestWindDirection(string name)
        {
            return new WindDirection(name, 5.0);
        }
    }
}