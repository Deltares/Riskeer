// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Text.RegularExpressions;

namespace Riskeer.AssemblyTool.IO.Helpers
{
    /// <summary>
    /// Validation helper to validate the id of an exportable object.
    /// </summary>
    public static class IdValidationHelper
    {
        private static readonly Regex regex = new Regex(@"^[A-Za-z\\_][A-Za-z\\_\d\-\.]+$");

        /// <summary>
        /// Throws when <paramref name="id"/> is invalid for use as an identifier.
        /// </summary>
        /// <param name="id">The identifier to validate.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public static void ThrowIfInvalid(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !regex.IsMatch(id))
            {
                throw new ArgumentException($@"'{nameof(id)}' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.");
            }
        }
    }
}