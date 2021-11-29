// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Core.Common.Base;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// This interface describes an <see cref="IFailureMechanism"/> containing <see cref="FailureMechanismSectionResultOld"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of the section results.</typeparam>
    public interface IHasSectionResults<out T> : IFailureMechanism
        where T : FailureMechanismSectionResultOld
    {
        /// <summary>
        /// Gets an <see cref="IObservableEnumerable{T}"/> of <see cref="FailureMechanismSectionResultOld"/>.
        /// </summary>
        IObservableEnumerable<T> SectionResultsOld { get; }
    }

    /// <summary>
    /// This interface describes an <see cref="IFailureMechanism"/> containing <see cref="FailureMechanismSectionResult"/> objects.
    /// </summary>
    /// <typeparam name="TOld">The olt type of the section results.</typeparam>
    /// <typeparam name="T">The type of the section results.</typeparam>
    public interface IHasSectionResults<out TOld, out T> : IHasSectionResults<TOld>
        where TOld : FailureMechanismSectionResultOld
        where T : FailureMechanismSectionResult
    {
        /// <summary>
        /// Gets an <see cref="IObservableEnumerable{T}"/> of <see cref="FailureMechanismSectionResult"/>.
        /// </summary>
        IObservableEnumerable<T> SectionResults { get; }
    }
}