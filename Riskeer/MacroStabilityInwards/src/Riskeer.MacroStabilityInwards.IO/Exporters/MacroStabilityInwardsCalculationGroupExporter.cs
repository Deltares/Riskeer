// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.Util;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.MacroStabilityInwards.IO.Exporters
{
    /// <summary>
    /// Exports macro stability inwards calculations from a calculation group and stores them as separate stix files.
    /// </summary>
    public class MacroStabilityInwardsCalculationGroupExporter : IFileExporter
    {
        private readonly CalculationGroup calculationGroup;
        private readonly string folderPath;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationGroupExporter"/>.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to export.</param>
        /// <param name="folderPath">The folder path to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationGroup"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="folderPath"/> is invalid.</exception>
        /// <remarks>A valid path:<list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>is not too long.</item>
        /// </list></remarks>
        public MacroStabilityInwardsCalculationGroupExporter(CalculationGroup calculationGroup, string folderPath)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            IOUtils.ValidateFolderPath(folderPath);

            this.calculationGroup = calculationGroup;
            this.folderPath = folderPath;
        }

        public bool Export()
        {
            return false;
        }
    }
}