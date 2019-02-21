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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Util;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.IO.WaveConditions
{
    /// <summary>
    /// Class for creating instances of <see cref="ExportableWaveConditions"/>.
    /// </summary>
    public static class ExportableWaveConditionsFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="ExportableWaveConditions"/>.
        /// </summary>
        /// <param name="name">The name of the calculation to which the <see cref="WaveConditionsOutput"/> belong.</param>
        /// <param name="waveConditionsInput">The <see cref="FailureMechanismCategoryWaveConditionsInput"/> used in the calculations.</param>
        /// <param name="output">The <see cref="WaveConditionsOutput"/> resulting from the calculations.</param>
        /// <returns>A collection of <see cref="ExportableWaveConditions"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="FailureMechanismCategoryWaveConditionsInput.HydraulicBoundaryLocation"/> 
        /// is <c>null</c> in <paramref name="waveConditionsInput"/>.</exception>
        public static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(
            string name,
            FailureMechanismCategoryWaveConditionsInput waveConditionsInput,
            IEnumerable<WaveConditionsOutput> output)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            return CreateExportableWaveConditionsCollection(name, waveConditionsInput, output, CoverType.GrassWaveRunUp,
                                                            GetCategoryBoundaryName(waveConditionsInput.CategoryType));
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableWaveConditions"/>.
        /// </summary>
        /// <param name="name">The name of the calculation to which the <see cref="WaveConditionsOutput"/> belong.</param>
        /// <param name="waveConditionsInput">The <see cref="AssessmentSectionCategoryWaveConditionsInput"/> used in the calculations.</param>
        /// <param name="output">The <see cref="WaveConditionsOutput"/> resulting from the calculations.</param>
        /// <param name="coverType">The <see cref="CoverType"/> that the <paramref name="output"/> represents.</param>
        /// <returns>A collection of <see cref="ExportableWaveConditions"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="AssessmentSectionCategoryWaveConditionsInput.HydraulicBoundaryLocation"/> 
        /// is <c>null</c> in <paramref name="waveConditionsInput"/>.</exception>
        public static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(
            string name,
            AssessmentSectionCategoryWaveConditionsInput waveConditionsInput,
            IEnumerable<WaveConditionsOutput> output,
            CoverType coverType)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            if (coverType == null)
            {
                throw new ArgumentNullException(nameof(coverType));
            }

            return CreateExportableWaveConditionsCollection(name, waveConditionsInput, output, coverType,
                                                            GetCategoryBoundaryName(waveConditionsInput.CategoryType));
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableWaveConditions"/>.
        /// </summary>
        /// <param name="name">The name of the calculation to which the <see cref="WaveConditionsOutput"/> belong.</param>
        /// <param name="waveConditionsInput">The <see cref="WaveConditionsInput"/> used in the calculations.</param>
        /// <param name="output">The <see cref="WaveConditionsOutput"/> resulting from the calculations.</param>
        /// <param name="coverType">The type of cover.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary.</param>
        /// <returns>A collection of <see cref="ExportableWaveConditions"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="WaveConditionsInput.HydraulicBoundaryLocation"/> 
        /// is <c>null</c> in <paramref name="waveConditionsInput"/>.</exception>
        private static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(
            string name,
            WaveConditionsInput waveConditionsInput,
            IEnumerable<WaveConditionsOutput> output,
            CoverType coverType,
            string categoryBoundaryName)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return output.Select(waveConditionsOutput => new ExportableWaveConditions(name, waveConditionsInput, waveConditionsOutput,
                                                                                      coverType, categoryBoundaryName)).ToArray();
        }

        private static string GetCategoryBoundaryName<T>(T enumValue)
        {
            return new EnumDisplayWrapper<T>(enumValue).DisplayName;
        }
    }
}