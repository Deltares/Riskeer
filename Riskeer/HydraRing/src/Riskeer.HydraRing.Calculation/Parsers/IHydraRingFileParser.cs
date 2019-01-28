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
using Riskeer.HydraRing.Calculation.Exceptions;

namespace Riskeer.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// This interface describes components that obtain results from the output files of a Hydra-Ring calculation.
    /// </summary>
    public interface IHydraRingFileParser
    {
        /// <summary>
        /// Tries to parse output from a file in the <paramref name="workingDirectory"/> based on a <paramref name="sectionId"/>.
        /// </summary>
        /// <param name="workingDirectory">The path to the directory which contains the output of the Hydra-Ring calculation.</param>
        /// <param name="sectionId">The section id to get the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="workingDirectory"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="workingDirectory"/>
        /// <list type="bullet">
        /// <item>is zero-length, or</item>
        /// <item>contains only whitespace, or</item>
        /// <item>contains illegal characters, or</item>
        /// <item>contains a colon which is not part of a volume identifier, or</item>
        /// <item>is too long.</item>
        /// </list></exception>
        /// <exception cref="HydraRingFileParserException">Thrown when the HydraRing file parser 
        /// encounters an error while parsing HydraRing output.</exception>
        void Parse(string workingDirectory, int sectionId);
    }
}