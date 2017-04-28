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

using System;
using Core.Common.Base.Geometry;
using log4net;
using Ringtoets.Piping.Plugin.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Extension methods for the <see cref="RingtoetsPipingSurfaceLine"/> class.
    /// </summary>
    public static class RingtoetsPipingSurfaceLineExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RingtoetsPipingSurfaceLineExtensions));

        /// <summary>
        /// Tries to set the <see cref="RingtoetsPipingSurfaceLine.DitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="RingtoetsPipingSurfaceLine.DitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsPipingSurfaceLine.DitchPolderSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsPipingSurfaceLine.DitchPolderSide"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetDitchPolderSide(this RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDitchPolderSideAt(point);
                    return true;
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetBottomDitchPolderSide(this RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetBottomDitchPolderSideAt(point);
                    return true;
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetBottomDitchDikeSide(this RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetBottomDitchDikeSideAt(point);
                    return true;
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsPipingSurfaceLine.DitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="RingtoetsPipingSurfaceLine.DitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsPipingSurfaceLine.DitchDikeSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsPipingSurfaceLine.DitchDikeSide"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetDitchDikeSide(this RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDitchDikeSideAt(point);
                    return true;
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsPipingSurfaceLine.DikeToeAtRiver"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="RingtoetsPipingSurfaceLine.DikeToeAtRiver"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsPipingSurfaceLine.DikeToeAtRiver"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsPipingSurfaceLine.DikeToeAtRiver"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetDikeToeAtRiver(this RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDikeToeAtRiverAt(point);
                    return true;
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsPipingSurfaceLine.DikeToeAtPolder"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="RingtoetsPipingSurfaceLine.DikeToeAtPolder"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsPipingSurfaceLine.DikeToeAtPolder"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsPipingSurfaceLine.DikeToeAtPolder"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetDikeToeAtPolder(this RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDikeToeAtPolderAt(point);
                    return true;
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
            return false;
        }

        private static void LogError(RingtoetsPipingSurfaceLine surfaceLine, ArgumentException e)
        {
            log.ErrorFormat(Resources.PipingSurfaceLinesCsvImporter_CharacteristicPoint_of_SurfaceLine_0_skipped_cause_1_,
                            surfaceLine.Name,
                            e.Message);
        }
    }
}