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
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonIoResources = Ringtoets.Common.IO.Properties.Resources;
using MacroStabilityInwardsIOResources = Ringtoets.MacroStabilityInwards.IO.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.IO.Importers
{
    /// <summary>
    /// Extension methods for the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> class.
    /// </summary>
    public static class RingtoetsMacroStabilityInwardsSurfaceLineExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RingtoetsMacroStabilityInwardsSurfaceLineExtensions));

        /// <summary>
        /// Tries to set the relevant characteristic points from the <paramref name="characteristicPoints"/>
        /// on the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to set the characteristic points for.</param>
        /// <param name="characteristicPoints">The characteristic points to set, if the collection is valid.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        /// <exception cref="SurfaceLineTransformException">Thrown when a mandatory characteristic point is not
        /// present or not on the given <paramref name="surfaceLine"/>.</exception>
        public static void SetCharacteristicPoints(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, CharacteristicPoints characteristicPoints)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            if (characteristicPoints == null)
            {
                throw new SurfaceLineTransformException($"Karakteristieke punten definitie voor profielschematisatie '{surfaceLine.Name}' is verplicht.");
            }

            surfaceLine.TrySetSurfaceLevelOutside(characteristicPoints.SurfaceLevelOutside);
            surfaceLine.TrySetDikeToeAtRiver(characteristicPoints.DikeToeAtRiver);
            surfaceLine.TrySetDikeTopAtPolder(characteristicPoints.DikeTopAtPolder);
            surfaceLine.TrySetDikeToeAtPolder(characteristicPoints.DikeToeAtPolder);
            surfaceLine.TrySetSurfaceLevelInside(characteristicPoints.SurfaceLevelInside);

            surfaceLine.TrySetTrafficLoadOutside(characteristicPoints.TrafficLoadOutside);
            surfaceLine.TrySetTrafficLoadInside(characteristicPoints.TrafficLoadInside);
            surfaceLine.TrySetShoulderBaseInside(characteristicPoints.ShoulderBaseInside);
            surfaceLine.TrySetShoulderTopInside(characteristicPoints.ShoulderTopInside);
            surfaceLine.TrySetDitchDikeSide(characteristicPoints.DitchDikeSide);
            surfaceLine.TrySetBottomDitchDikeSide(characteristicPoints.BottomDitchDikeSide);
            surfaceLine.TrySetBottomDitchPolderSide(characteristicPoints.BottomDitchPolderSide);
            surfaceLine.TrySetDitchPolderSide(characteristicPoints.DitchPolderSide);
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchPolderSide"/>.</param>
        private static void TrySetDitchPolderSide(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDitchPolderSideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogOptionalCharacteristicPointError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/>.</param>
        private static void TrySetBottomDitchPolderSide(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetBottomDitchPolderSideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogOptionalCharacteristicPointError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/>.</param>
        private static void TrySetBottomDitchDikeSide(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetBottomDitchDikeSideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogOptionalCharacteristicPointError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DitchDikeSide"/>.</param>
        private static void TrySetDitchDikeSide(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDitchDikeSideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogOptionalCharacteristicPointError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/>.</param>
        /// <exception cref="SurfaceLineTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetSurfaceLevelInside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetSurfaceLevelInsideAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelInside), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/>.</param>
        /// <exception cref="SurfaceLineTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetSurfaceLevelOutside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetSurfaceLevelOutsideAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelOutside), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/>.</param>
        /// <exception cref="SurfaceLineTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetDikeTopAtPolder(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetDikeTopAtPolderAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtPolder), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/>.</param>
        private static void TrySetShoulderBaseInside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetShoulderBaseInsideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogOptionalCharacteristicPointError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderTopInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderTopInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.ShoulderTopInside"/>.</param>
        private static void TrySetShoulderTopInside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetShoulderTopInsideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogOptionalCharacteristicPointError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadInside"/>.</param>
        private static void TrySetTrafficLoadInside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetTrafficLoadInsideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogOptionalCharacteristicPointError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadOutside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadOutside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.TrafficLoadOutside"/>.</param>
        private static void TrySetTrafficLoadOutside(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetTrafficLoadOutsideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogOptionalCharacteristicPointError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/>.</param>
        /// <exception cref="SurfaceLineTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetDikeToeAtRiver(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetDikeToeAtRiverAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtRiver), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        /// <summary>
        /// Tries to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/>.</param>
        /// <exception cref="SurfaceLineTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetDikeToeAtPolder(this RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetDikeToeAtPolderAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtPolder), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        private static void LogOptionalCharacteristicPointError(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, ArgumentException e)
        {
            log.ErrorFormat(RingtoetsCommonIoResources.SurfaceLinesCsvImporter_CharacteristicPoint_of_SurfaceLine_0_skipped_cause_1_,
                            surfaceLine.Name,
                            e.Message);
        }

        private static string CreateMissingMandatoryPointMessage(string surfaceLineName)
        {
            return string.Format(MacroStabilityInwardsIOResources.MacroStabilityInwardsSurfaceLineTransformer_CharacteristicPoint_0_is_undefined, surfaceLineName);
        }

        private static SurfaceLineTransformException CreateMandatoryCharacteristicPointException(string exceptionMessage, string surfaceLineName)
        {
            string message = string.Format(MacroStabilityInwardsIOResources.SurfaceLinesCsvImporter_SurfaceLine_0_skipped_cause_1_CharacteristicPoint_mandatory, surfaceLineName, exceptionMessage);
            return new SurfaceLineTransformException(message);
        }
    }
}