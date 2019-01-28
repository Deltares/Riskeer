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

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input
{
    /// <summary>
    /// Phreatic line offset values that are used to perform a calculation.
    /// </summary>
    public class PhreaticLineOffsets
    {
        /// <summary>
        /// Creates a new instance of <see cref="PhreaticLineOffsets"/>.
        /// </summary>
        /// <remarks><see cref="UseDefaults"/> is set to <c>true</c>; <see cref="BelowDikeTopAtRiver"/>,
        /// <see cref="BelowDikeTopAtPolder"/>, <see cref="BelowDikeToeAtPolder"/>
        /// and <see cref="BelowShoulderBaseInside"/> are set to <see cref="double.NaN"/>.</remarks>
        public PhreaticLineOffsets()
        {
            UseDefaults = true;
            BelowDikeTopAtRiver = double.NaN;
            BelowDikeTopAtPolder = double.NaN;
            BelowDikeToeAtPolder = double.NaN;
            BelowShoulderBaseInside = double.NaN;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PhreaticLineOffsets"/>.
        /// </summary>
        /// <param name="belowDikeTopAtRiver">The offset of the phreatic line below dike top at river.</param>
        /// <param name="belowDikeTopAtPolder">The offset of the phreatic line below dike top at polder.</param>
        /// <param name="belowDikeToeAtPolder">The offset of the phreatic line below dike toe at polder.</param>
        /// <param name="belowShoulderBaseInside">The offset of the phreatic line below shoulder base inside.</param>
        /// <remarks><see cref="UseDefaults"/> is set to <c>false</c>.</remarks>
        public PhreaticLineOffsets(double belowDikeTopAtRiver, double belowDikeTopAtPolder,
                                   double belowDikeToeAtPolder, double belowShoulderBaseInside)
        {
            UseDefaults = false;
            BelowDikeTopAtRiver = belowDikeTopAtRiver;
            BelowDikeTopAtPolder = belowDikeTopAtPolder;
            BelowDikeToeAtPolder = belowDikeToeAtPolder;
            BelowShoulderBaseInside = belowShoulderBaseInside;
        }

        /// <summary>
        /// Gets whether the default offsets should be used.
        /// </summary>
        public bool UseDefaults { get; }

        /// <summary>
        /// Gets the offset of the phreatic line below dike top at river.
        /// [m]
        /// </summary>
        public double BelowDikeTopAtRiver { get; }

        /// <summary>
        /// Gets the offset of the phreatic line below dike top at polder.
        /// [m]
        /// </summary>
        public double BelowDikeTopAtPolder { get; }

        /// <summary>
        /// Gets the offset of the phreatic line below dike toe at polder.
        /// [m]
        /// </summary>
        public double BelowDikeToeAtPolder { get; }

        /// <summary>
        /// Gets the offset of the phreatic line below shoulder base inside.
        /// [m]
        /// </summary>
        public double BelowShoulderBaseInside { get; }
    }
}