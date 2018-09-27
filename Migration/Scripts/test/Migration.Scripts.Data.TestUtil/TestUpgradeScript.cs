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

using System;

namespace Migration.Scripts.Data.TestUtil
{
    /// <summary>
    /// Test class for <see cref="UpgradeScript"/>.
    /// </summary>
    public class TestUpgradeScript : UpgradeScript
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestUpgradeScript"/>.
        /// </summary>
        /// <param name="fromVersion">The source version.</param>
        /// <param name="toVersion">The target version.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="fromVersion"/> is empty or <c>null</c>,</item>
        /// <item><paramref name="toVersion"/> is empty or <c>null</c>,</item>
        /// </list></exception>
        public TestUpgradeScript(string fromVersion, string toVersion)
            : base(fromVersion, toVersion) {}

        protected override void PerformUpgrade(string sourceLocation, string targetLocation) {}
    }
}