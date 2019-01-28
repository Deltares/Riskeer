// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Interface to define whether a <see cref="StochasticSoilModel"/> is valid for the calling 
    /// failure mechanism.
    /// </summary>
    public interface IStochasticSoilModelMechanismFilter
    {
        /// <summary>
        /// Validates the <paramref name="stochasticSoilModel"/>.
        /// </summary>
        /// <param name="stochasticSoilModel">The <see cref="StochasticSoilModel"/> to validate.</param>
        /// <returns><c>true</c> if the <paramref name="stochasticSoilModel"/> is valid for the 
        /// calling failure mechanism, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stochasticSoilModel"/> 
        /// is <c>null</c>.</exception>
        bool IsValidForFailureMechanism(StochasticSoilModel stochasticSoilModel);
    }
}