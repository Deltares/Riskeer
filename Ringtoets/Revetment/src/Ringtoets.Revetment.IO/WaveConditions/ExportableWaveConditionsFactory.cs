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
using System.Linq;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO.WaveConditions
{
    /// <summary>
    /// Class for constructing <see cref="ExportableWaveConditions"/> objects.
    /// </summary>
    public static class ExportableWaveConditionsFactory
    {
        /// <summary>
        /// Create a collection of <see cref="ExportableWaveConditions"/>.
        /// </summary>
        /// <param name="name">The name of the calculation to which the <see cref="WaveConditionsOutput"/> objects belong.</param>
        /// <param name="waveConditionsInput">The <see cref="WaveConditionsInput"/> used in the calculations.</param>
        /// <param name="columnsOutput">The <see cref="WaveConditionsOutput"/> objects resulting from columns calculations.</param>
        /// <param name="blocksOutput">The <see cref="WaveConditionsOutput"/> objects resulting from blocks calculations.</param>
        /// <returns>A container of <see cref="ExportableWaveConditions"/> objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c> or 
        /// when <see cref="WaveConditionsOutput"/> is <c>null</c> for <paramref name="columnsOutput"/> or <paramref name="blocksOutput"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="WaveConditionsInput.HydraulicBoundaryLocation"/> 
        /// is <c>null</c> for <paramref name="waveConditionsInput"/>.</exception>
        public static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(
            string name, WaveConditionsInput waveConditionsInput,
            IEnumerable<WaveConditionsOutput> columnsOutput, IEnumerable<WaveConditionsOutput> blocksOutput)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            if (columnsOutput == null)
            {
                throw new ArgumentNullException(nameof(columnsOutput));
            }

            if (blocksOutput == null)
            {
                throw new ArgumentNullException(nameof(blocksOutput));
            }

            var exportableWaveConditionsCollection = new List<ExportableWaveConditions>();

            foreach (WaveConditionsOutput waveConditionsOutput in columnsOutput)
            {
                exportableWaveConditionsCollection.Add(new ExportableWaveConditions(name, waveConditionsInput, waveConditionsOutput,
                                                                                    CoverType.StoneCoverColumns));
            }

            foreach (WaveConditionsOutput blocksConditionsOutput in blocksOutput)
            {
                exportableWaveConditionsCollection.Add(new ExportableWaveConditions(name, waveConditionsInput, blocksConditionsOutput,
                                                                                    CoverType.StoneCoverBlocks));
            }

            return exportableWaveConditionsCollection;
        }

        /// <summary>
        /// Create a collection of <see cref="ExportableWaveConditions"/>.
        /// </summary>
        /// <param name="name">The name of the calculation to which the <see cref="WaveConditionsOutput"/> objects belong.</param>
        /// <param name="waveConditionsInput">The <see cref="FailureMechanismCategoryWaveConditionsInput"/> used in the calculations.</param>
        /// <param name="output">The <see cref="WaveConditionsOutput"/> objects resulting from the calculations.</param>
        /// <param name="coverType">The type of cover.</param>
        /// <returns>A container of <see cref="ExportableWaveConditions"/> objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when:
        /// <list type="bullet">
        /// <item>any parameter is <c>null</c></item>
        /// <item>any item in <paramref name="output"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="FailureMechanismCategoryWaveConditionsInput.HydraulicBoundaryLocation"/> 
        /// is <c>null</c> for <paramref name="waveConditionsInput"/>.</exception>
        public static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(
            string name, FailureMechanismCategoryWaveConditionsInput waveConditionsInput,
            IEnumerable<WaveConditionsOutput> output, CoverType coverType)
        {
            return CreateExportableWaveConditionsCollection(name, (WaveConditionsInput) waveConditionsInput, output, coverType);
        }

        /// <summary>
        /// Create a collection of <see cref="ExportableWaveConditions"/>.
        /// </summary>
        /// <param name="name">The name of the calculation to which the <see cref="WaveConditionsOutput"/> objects belong.</param>
        /// <param name="waveConditionsInput">The <see cref="AssessmentSectionCategoryWaveConditionsInput"/> used in the calculations.</param>
        /// <param name="output">The <see cref="WaveConditionsOutput"/> objects resulting from the calculations.</param>
        /// <param name="coverType">The type of cover.</param>
        /// <returns>A container of <see cref="ExportableWaveConditions"/> objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when:
        /// <list type="bullet">
        /// <item>any parameter is <c>null</c></item>
        /// <item>any item in <paramref name="output"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="AssessmentSectionCategoryWaveConditionsInput.HydraulicBoundaryLocation"/> 
        /// is <c>null</c> for <paramref name="waveConditionsInput"/>.</exception>
        public static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(
            string name, AssessmentSectionCategoryWaveConditionsInput waveConditionsInput,
            IEnumerable<WaveConditionsOutput> output, CoverType coverType)
        {
            return CreateExportableWaveConditionsCollection(name, (WaveConditionsInput) waveConditionsInput, output, coverType);
        }

        /// <summary>
        /// Create a collection of <see cref="ExportableWaveConditions"/>.
        /// </summary>
        /// <param name="name">The name of the calculation to which the <see cref="WaveConditionsOutput"/> objects belong.</param>
        /// <param name="waveConditionsInput">The <see cref="WaveConditionsInput"/> used in the calculations.</param>
        /// <param name="output">The <see cref="WaveConditionsOutput"/> objects resulting from the calculations.</param>
        /// <param name="coverType">The type of cover.</param>
        /// <returns>A container of <see cref="ExportableWaveConditions"/> objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when:
        /// <list type="bullet">
        /// <item>any parameter is <c>null</c></item>
        /// <item>any item in <paramref name="output"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="WaveConditionsInput.HydraulicBoundaryLocation"/> 
        /// is <c>null</c> for <paramref name="waveConditionsInput"/>.</exception>
        private static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(
            string name, WaveConditionsInput waveConditionsInput,
            IEnumerable<WaveConditionsOutput> output, CoverType coverType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (coverType == null)
            {
                throw new ArgumentNullException(nameof(coverType));
            }

            return output.Select(waveConditionsOutput => new ExportableWaveConditions(name, waveConditionsInput, waveConditionsOutput, coverType)).ToArray();
        }
    }
}