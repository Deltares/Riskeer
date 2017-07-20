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
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Importers
{
    /// <summary>
    /// Extension methods for the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> class.
    /// </summary>
    public static class RingtoetsMacroStabilityInwardsSurfaceLineExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RingtoetsMacroStabilityInwardsSurfaceLineExtensions));

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchPolderSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchPolderSide"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetDitchPolderSide(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetBottomDitchPolderSide(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetBottomDitchDikeSide(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchDikeSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchDikeSide"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetDitchDikeSide(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetSurfaceLevelInside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetSurfaceLevelInsideAt(point);
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetSurfaceLevelOutside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetSurfaceLevelOutsideAt(point);
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetDikeTopAtPolder(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDikeTopAtPolderAt(point);
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetShoulderBaseInside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetShoulderBaseInsideAt(point);
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderTopInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderTopInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderTopInside"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderTopInside"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetShoulderTopInside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetShoulderTopInsideAt(point);
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadInside"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadInside"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetTrafficLoadInside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetTrafficLoadInsideAt(point);
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadOutside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadOutside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadOutside"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadOutside"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetTrafficLoadOutside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetTrafficLoadOutsideAt(point);
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetDikeToeAtRiver(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/>.</param>
        /// <returns><c>true</c> if the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/> was set, <c>false</c> if
        /// <paramref name="point"/> is <c>null</c> or there is no point in <paramref name="surfaceLine"/> at the location
        /// of <paramref name="point"/>.</returns>
        public static bool TrySetDikeToeAtPolder(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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

        /// <summary>
        /// Tries to set the relevant characteristic points from the <paramref name="characteristicPoints"/>
        /// on the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to set characteristic points for.</param>
        /// <param name="characteristicPoints">The characteristic points to set, if the collection is valid.</param>
        /// <returns><c>true</c> if the characteristic points could be set; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static bool SetCharacteristicPoints(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, CharacteristicPoints characteristicPoints)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }
            if (characteristicPoints == null)
            {
                return false;
            }

            surfaceLine.TrySetSurfaceLevelOutside(characteristicPoints.SurfaceLevelOutside);
            surfaceLine.TrySetTrafficLoadOutside(characteristicPoints.TrafficLoadOutside);
            surfaceLine.TrySetTrafficLoadInside(characteristicPoints.TrafficLoadInside);
            surfaceLine.TrySetDikeTopAtPolder(characteristicPoints.DikeTopAtPolder);
            surfaceLine.TrySetShoulderBaseInside(characteristicPoints.ShoulderBaseInside);
            surfaceLine.TrySetShoulderTopInside(characteristicPoints.ShoulderTopInside);
            surfaceLine.TrySetDikeToeAtRiver(characteristicPoints.DikeToeAtRiver);
            surfaceLine.TrySetDitchDikeSide(characteristicPoints.DitchDikeSide);
            surfaceLine.TrySetBottomDitchDikeSide(characteristicPoints.BottomDitchDikeSide);
            surfaceLine.TrySetBottomDitchPolderSide(characteristicPoints.BottomDitchPolderSide);
            surfaceLine.TrySetDitchPolderSide(characteristicPoints.DitchPolderSide);
            surfaceLine.TrySetDikeToeAtPolder(characteristicPoints.DikeToeAtPolder);
            surfaceLine.TrySetSurfaceLevelInside(characteristicPoints.SurfaceLevelInside);

            return true;
        }

        private static void LogError(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, ArgumentException e)
        {
            log.ErrorFormat(Resources.SurfaceLinesCsvImporter_CharacteristicPoint_of_SurfaceLine_0_skipped_cause_1_,
                            surfaceLine.Name,
                            e.Message);
        }
    }
}