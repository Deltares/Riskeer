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
using Components.Persistence.Stability;
using Core.Common.Base.IO;
using Core.Common.Util;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.Exporters
{
    /// <summary>
    /// Exports a macro stability inwards calculation and stores it in a separate stix file.
    /// </summary>
    public class MacroStabilityInwardsCalculationExporter : IFileExporter
    {
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationExporter"/>.
        /// </summary>
        /// <param name="calculation">The calculation to export.</param>
        /// <param name="persistenceFactory">The persistence factory to use.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> or
        /// <see cref="persistenceFactory"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public MacroStabilityInwardsCalculationExporter(MacroStabilityInwardsCalculation calculation,
                                                        IPersistenceFactory persistenceFactory,
                                                        string filePath)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (persistenceFactory == null)
            {
                throw new ArgumentNullException(nameof(persistenceFactory));
            }

            IOUtils.ValidateFilePath(filePath);
            
            this.filePath = filePath;
        }

        public bool Export()
        {
            return false;
        }
    }
}