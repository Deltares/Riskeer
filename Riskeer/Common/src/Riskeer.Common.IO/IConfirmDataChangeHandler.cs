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

namespace Riskeer.Common.IO
{
    /// <summary>
    /// Interface for verifying and handling changes as an effect of a change to the target data.
    /// </summary>
    public interface IConfirmDataChangeHandler
    {
        /// <summary>
        /// Verifies whether the change to the data has side-effects;
        /// and therefore a confirmation is required.
        /// </summary>
        /// <returns><c>true</c> if confirmation is required, <c>false</c> otherwise.</returns>
        bool RequireConfirmation();

        /// <summary>
        /// Inquires for a confirmation on whether changing the data should be continued.
        /// </summary>
        /// <returns><c>true</c> if confirmation is given; <c>false</c> otherwise.</returns>
        /// <remarks>Should only be called when <see cref="RequireConfirmation"/> returns <c>true</c>.
        /// </remarks>
        bool InquireConfirmation();
    }
}