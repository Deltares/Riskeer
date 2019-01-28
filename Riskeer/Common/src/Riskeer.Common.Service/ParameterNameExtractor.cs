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

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Class responsible for extracting the name of a parameter from a piece of text.
    /// </summary>
    public static class ParameterNameExtractor
    {
        /// <summary>
        /// Gets the parameter name from a display name formatted text.
        /// </summary>
        /// <param name="parameterNameWithUnits">The parameter name with units.</param>
        /// <returns>The parameter name.</returns>
        /// <remarks>Display name format: {Name of the parameter} [{unit of the parameter}].</remarks>
        public static string GetFromDisplayName(string parameterNameWithUnits)
        {
            string[] splitString = parameterNameWithUnits.Split('[');

            return splitString.Length != 0
                       ? splitString[0].Trim()
                       : string.Empty;
        }
    }
}