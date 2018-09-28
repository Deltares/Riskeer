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

using NUnit.Framework;

namespace Ringtoets.Migration.Core.TestUtil
{
    /// <summary>
    /// Helper class for asserting content of migration log messages.
    /// </summary>
    public static class MigrationLogTestHelper
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The original <see cref="MigrationLogMessage"/>.</param>
        /// <param name="actual">The actual <see cref="MigrationLogMessage"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="expected"/>.</exception>
        public static void AssertMigrationLogMessageEqual(MigrationLogMessage expected, MigrationLogMessage actual)
        {
            Assert.AreEqual(expected.ToVersion, actual.ToVersion);
            Assert.AreEqual(expected.FromVersion, actual.FromVersion);
            Assert.AreEqual(expected.Message, actual.Message);
        }
    }
}