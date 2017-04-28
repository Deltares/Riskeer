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

using Core.Common.Base.Geometry;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Denotes a <see cref="Point3D"/> as being used to mark a particular
    /// characteristic point of a <see cref="RingtoetsPipingSurfaceLine"/>.
    /// </summary>
    public enum CharacteristicPointType
    {
        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="RingtoetsPipingSurfaceLine.DikeToeAtRiver"/>.
        /// </summary>
        DikeToeAtRiver = 1,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="RingtoetsPipingSurfaceLine.DikeToeAtPolder"/>.
        /// </summary>
        DikeToeAtPolder = 2,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="RingtoetsPipingSurfaceLine.DitchDikeSide"/>.
        /// </summary>
        DitchDikeSide = 3,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/>.
        /// </summary>
        BottomDitchDikeSide = 4,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/>.
        /// </summary>
        BottomDitchPolderSide = 5,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="RingtoetsPipingSurfaceLine.DitchPolderSide"/>.
        /// </summary>
        DitchPolderSide = 6
    }
}