﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.IO.FileImporters
{
    /// <summary>
    /// Interface describing the method of updating the data model after new 
    /// failure mechanism sections have been imported.
    /// </summary>
    public interface IFailureMechanismSectionUpdateStrategy
    {
        /// <summary>
        /// Updates the data model with data from <paramref name="importedFailureMechanismSections"/>.
        /// </summary>
        /// <param name="importedFailureMechanismSections">The imported failure mechanism sections.</param>
        /// <param name="sourcePath">The source path from where the failure mechanism sections were imported.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of updated instances.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="UpdateDataException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="sourcePath"/> is not a valid file path.</item>
        /// <item><paramref name="importedFailureMechanismSections"/> contains sections that are not properly chained.</item>
        /// </list>
        /// </exception>
        IEnumerable<IObservable> UpdateSectionsWithImportedData(IEnumerable<FailureMechanismSection> importedFailureMechanismSections,
                                                                string sourcePath);

        /// <summary>
        /// Perform post-update actions.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of updated instances.</returns>
        IEnumerable<IObservable> DoPostUpdateActions();
    }
}