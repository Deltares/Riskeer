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
    /// Test class for <see cref="CreateScript"/>.
    /// </summary>
    public class TestCreateScript : CreateScript
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestCreateScript"/>.
        /// </summary>
        /// <param name="version">The version for this <see cref="TestCreateScript"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="version"/> is empty 
        /// or <c>null</c>.</exception>
        public TestCreateScript(string version) : base(version) {}

        protected override IVersionedFile GetEmptyVersionedFile(string location)
        {
            return new TestVersionedFile
            {
                Location = location,
                Version = GetVersion()
            };
        }

        /// <summary>
        /// Test class for <see cref="IVersionedFile"/>.
        /// </summary>
        private class TestVersionedFile : IVersionedFile
        {
            /// <summary>
            /// Sets the version for <seealso cref="GetVersion"/>.
            /// </summary>
            public string Version { private get; set; }

            public string Location { get; set; }

            public string GetVersion()
            {
                return Version;
            }
        }
    }
}