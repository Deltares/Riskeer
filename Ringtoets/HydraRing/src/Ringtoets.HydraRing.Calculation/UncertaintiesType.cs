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

namespace Ringtoets.HydraRing.Calculation
{
    /// <summary>
    /// Enumeration that defines the possible uncertainties types.
    /// </summary>
    /// <remarks>
    /// Except for <see cref="Undefined"/>, the integer values correspond to uncertainties ids defined by Hydra-Ring.
    /// </remarks>
    public enum UncertaintiesType
    {
        Undefined = -1,
        None = 0, // "No uncertainties"
        All = 1, // "All uncertainties"
        Model = 2, // "Only model uncertainties"
        Statistic = 3 // "Only statistical uncertainties"
    }
}
