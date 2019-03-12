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

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Interface for handling clearing illustration point results from a calculation.
    /// </summary>
    public interface IClearIllustrationPointsOfCalculationChangeHandler
    {
        /// <summary>
        /// Gets confirmation for clearing the illustration points.
        /// </summary>
        /// <returns><c>true</c> when confirmation is given, <c>false</c> otherwise.</returns>
        bool InquireConfirmation();

        /// <summary>
        /// Clears all illustration points of the calculation.
        /// </summary>
        /// <returns><c>true</c> if the calculation was affected, <c>false</c> otherwise.</returns>
        bool ClearIllustrationPoints();

        /// <summary>
        /// Perform post-update actions.
        /// </summary>
        void DoPostUpdateActions();
    }
}