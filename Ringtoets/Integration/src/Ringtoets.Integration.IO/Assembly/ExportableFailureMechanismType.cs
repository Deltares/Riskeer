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

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Enum representing the exportable failure mechanism types.
    /// </summary>
    public enum ExportableFailureMechanismType
    {
        /// <summary>
        /// Represents the failure mechanism macro stability inwards.
        /// </summary>
        STBI = 1,

        /// <summary>
        /// Represents the failure mechanism macro stability outwards.
        /// </summary>
        STBU = 2,

        /// <summary>
        /// Represents the failure mechanism piping.
        /// </summary>
        STPH = 3,

        /// <summary>
        /// Represents the failure mechanism microstability.
        /// </summary>
        STMI = 4,

        /// <summary>
        /// Represents the failure mechanism wave impact asphalt cover.
        /// </summary>
        AGK = 5,

        /// <summary>
        /// Represents the failure mechanism water pressure asphalt cover.
        /// </summary>
        AWO = 6,

        /// <summary>
        /// Represents the failure mechanism grass cover erosion outwards.
        /// </summary>
        GEBU = 7,

        /// <summary>
        /// Represents the failure mechanism grass cover slipoff outwards.
        /// </summary>
        GABU = 8,

        /// <summary>
        /// Represents the failure mechanism grass cover erosion inwards.
        /// </summary>
        GEKB = 9,

        /// <summary>
        /// Represents the failure mechanism grass cover slipoff inwards.
        /// </summary>
        GABI = 10,

        /// <summary>
        /// Represents the failure mechanism stability stone cover.
        /// </summary>
        ZST = 11,

        /// <summary>
        /// Represents the failure mechanism dune erosion.
        /// </summary>
        DA = 12,

        /// <summary>
        /// Represents the failure mechanism height structures.
        /// </summary>
        HTKW = 13,

        /// <summary>
        /// Represents the failure mechanism closing structures.
        /// </summary>
        BSKW = 14,

        /// <summary>
        /// Represents the failure mechanism piping structures.
        /// </summary>
        PKW = 15,

        /// <summary>
        /// Represents the failure mechanism stability point structures.
        /// </summary>
        STKWp = 16,

        /// <summary>
        /// Represents the failure mechanism strength stability lengthwise construction.
        /// </summary>
        STKWl = 17,

        /// <summary>
        /// Represents the failure mechanism technical innovation.
        /// </summary>
        INN = 18
    }
}