// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Common.Data.Calculation
{
    /// <summary>
    /// A calculation related object.
    /// </summary>
    public interface ICalculation : ICalculationBase
    {
        /// <summary>
        /// Gets a value indicating whether or not this calculation item contains calculation output.
        /// </summary>
        bool HasOutput { get; }

        /// <summary>
        /// Clears the calculated output.
        /// </summary>
        void ClearOutput();

        /// <summary>
        /// Gets the comments associated with the calculation.
        /// </summary>
        Comment Comments { get; }
    }
}