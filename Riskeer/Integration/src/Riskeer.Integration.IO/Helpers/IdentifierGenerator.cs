// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System;
using System.Collections.Generic;

namespace Riskeer.Integration.IO.Helpers
{
    /// <summary>
    /// Class to generate ids.
    /// </summary>
    public class IdentifierGenerator
    {
        private readonly Dictionary<string, int> idLookup;

        /// <summary>
        /// Creates a new instance of <see cref="IdentifierGenerator"/>.
        /// </summary>
        public IdentifierGenerator()
        {
            idLookup = new Dictionary<string, int>();
        }

        /// <summary>
        /// Gets an unique id prefixed by <paramref name="prefix"/>.
        /// </summary>
        /// <param name="prefix">The prefix to be used for the generated id.</param>
        /// <returns>An unique id.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="prefix"/> is
        /// <c>null</c>, empty or consists of only whitespaces.</exception>
        public string GetUniqueId(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException($@"'{nameof(prefix)}' is null, empty or consists of whitespace.", nameof(prefix));
            }

            if (!idLookup.ContainsKey(prefix))
            {
                idLookup[prefix] = 0;
            }

            return $"{prefix}.{idLookup[prefix]++}";
        }

        /// <summary>
        /// Gets an id based on <paramref name="prefix"/> and <paramref name="id"/>.
        /// </summary>
        /// <param name="prefix">The prefix to be used for the generated id.</param>
        /// <param name="id">The id to be used for the generated id.</param>
        /// <returns>An id.</returns>
        /// <exception cref="ArgumentException">Thrown when any parameter is
        /// <c>null</c>, empty or consists of only whitespaces.</exception>
        public static string GenerateId(string prefix, string id)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException($@"'{nameof(prefix)}' is null, empty or consists of whitespace.", nameof(prefix));
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($@"'{nameof(id)}' is null, empty or consists of whitespace.", nameof(id));
            }

            return $"{prefix}.{id}";
        }
    }
}