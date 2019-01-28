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

using System.Text.RegularExpressions;

namespace Riskeer.AssemblyTool.IO.Model.Helpers
{
    /// <summary>
    /// Validator to validate the id of a serializable object.
    /// </summary>
    public static class SerializableIdValidator
    {
        /// <summary>
        /// Validates whether <paramref name="id"/> is a valid id to be used
        /// as an identifier in an xml context.
        /// </summary>
        /// <param name="id">The identifier to validate.</param>
        /// <returns><c>true</c> when <paramref name="id"/> is valid, <c>false</c> otherwise.</returns>
        public static bool Validate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            var regex = new Regex(@"^[A-Za-z\\_][A-Za-z\\_\d\-\.]+$");
            return regex.IsMatch(id);
        }
    }
}