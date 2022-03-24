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

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Defines a failure mechanism.
    /// </summary>
    public interface IFailureMechanism : IObservable
    {
        /// <summary>
        /// Gets the name of the failure mechanism.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Gets the comments associated with the input of the data object when it is part of the assembly.
        /// </summary>
        Comment InAssemblyInputComments { get; }

        /// <summary>
        /// Gets the comments associated with the output of the data object when it is part of the assembly.
        /// </summary>
        Comment InAssemblyOutputComments { get; }

        /// <summary>
        /// Gets the comments associated when the failure mechanism is set to not be part of the assembly.
        /// </summary>
        Comment NotInAssemblyComments { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this failure mechanism is part of the assembly.
        /// </summary>
        bool InAssembly { get; set; }

        /// <summary>
        /// Gets the source path of the imported <see cref="Sections"/>.
        /// </summary>
        string FailureMechanismSectionSourcePath { get; }

        /// <summary>
        /// Gets the collection of sections that define areas for which a calculation could determine
        /// a representative result.
        /// </summary>
        IEnumerable<FailureMechanismSection> Sections { get; }

        /// <summary>
        /// Gets the assembly result of the failure mechanism.
        /// </summary>
        FailureMechanismAssemblyResult AssemblyResult { get; }

        /// <summary>
        /// Sets a collection of <see cref="FailureMechanismSection"/> to <see cref="Sections"/>.
        /// </summary>
        /// <param name="sections">The sections to set.</param>
        /// <param name="sourcePath">The path of the file the sections originate from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="sourcePath"/> is not a valid file path.</item>
        /// <item><paramref name="sections"/> contains sections that are not properly chained.</item>
        /// </list></exception>
        void SetSections(IEnumerable<FailureMechanismSection> sections, string sourcePath);

        /// <summary>
        /// Clears all sections from <see cref="Sections"/>.
        /// </summary>
        void ClearAllSections();
    }

    /// <summary>
    /// This interface describes an <see cref="IFailureMechanism"/> containing <see cref="FailureMechanismSectionResult"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of the section results.</typeparam>
    public interface IFailureMechanism<out T> : IFailureMechanism
        where T : FailureMechanismSectionResult
    {
        /// <summary>
        /// Gets an <see cref="IObservableEnumerable{T}"/> of <see cref="FailureMechanismSectionResult"/>.
        /// </summary>
        IObservableEnumerable<T> SectionResults { get; }
    }
}