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
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Properties;

namespace Ringtoets.Integration.IO.Helpers
{
    /// <summary>
    /// Class to generate unique ids.
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
        /// Gets a new unique id prefixed by <paramref name="prefix"/>.
        /// </summary>
        /// <param name="prefix">The prefix to be used for the generated id.</param>
        /// <returns>An unique id.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="prefix"/> is
        /// <c>null</c>, empty or consists of only whitespaces.</exception>
        public string GetNewId(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException($@"'{nameof(prefix)}' is null, empty or consists of whitespace.", nameof(prefix));
            }

            if (idLookup.ContainsKey(prefix))
            {
                idLookup[prefix]++;
            }
            else
            {
                idLookup[prefix] = 0;
            }

            return $"{prefix}.{idLookup[prefix]}";
        }

        /// <summary>
        /// Gets an id based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="ExportableAssessmentSection"/>
        /// to generate an id for.</param>
        /// <returns>An id.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static string GeneratedId(ExportableAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return $"{Resources.SerializableAssessmentSection_IdPrefix}.{assessmentSection.Id}";
        }
    }
}