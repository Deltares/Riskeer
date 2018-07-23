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
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.Storage.Core.DbContext
{
    /// <summary>
    /// Denotes a <see cref="Point3D"/> as being used to mark a particular
    /// characteristic point of a <see cref="MacroStabilityInwardsSurfaceLine"/>.
    /// </summary>
    public enum MacroStabilityInwardsCharacteristicPointType
    {
        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/>.
        /// </summary>
        SurfaceLevelOutside = 1,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.DikeTopAtRiver"/>.
        /// </summary>
        DikeTopAtRiver = 2,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/>.
        /// </summary>
        DikeToeAtRiver = 3,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/>.
        /// </summary>
        DikeTopAtPolder = 4,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/>.
        /// </summary>
        ShoulderBaseInside = 5,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.ShoulderTopInside"/>.
        /// </summary>
        ShoulderTopInside = 6,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/>.
        /// </summary>
        DikeToeAtPolder = 7,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.DitchDikeSide"/>.
        /// </summary>
        DitchDikeSide = 8,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/>.
        /// </summary>
        BottomDitchDikeSide = 9,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/>.
        /// </summary>
        BottomDitchPolderSide = 10,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.DitchPolderSide"/>.
        /// </summary>
        DitchPolderSide = 11,

        /// <summary>
        /// Corresponds <see cref="Point3D"/> to <see cref="MacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/>.
        /// </summary>
        SurfaceLevelInside = 12
    }
}