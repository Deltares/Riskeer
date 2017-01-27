// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Migration.Scripts.Data.TestUtil
{
    /// <summary>
    /// Test class for <see cref="CreateScript"/>.
    /// </summary>
    public class TestCreateScript : CreateScript
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestCreateScript"/>.
        /// </summary>
        /// <param name="version">The version <paramref name="query"/> was designed for.</param>
        /// <param name="query">The SQL query that belongs to <paramref name="version"/>.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="version"/> is empty or <c>null</c>,</item>
        /// <item><paramref name="query"/> is empty, <c>null</c>, or consist out of only whitespace characters.</item>
        /// </list></exception>
        public TestCreateScript(string version, string query) : base(version, query) {}

        protected override IVersionedFile GetEmptyVersionedFile(string location)
        {
            return new TestVersionedFile
            {
                Location = location
            };
        }
    }
}