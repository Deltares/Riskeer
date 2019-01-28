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
using Core.Common.Base.Geometry;
using log4net;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.SurfaceLines
{
    /// <summary>
    /// Extension methods for the <see cref="PipingSurfaceLine"/> class.
    /// </summary>
    public static class PipingSurfaceLineExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PipingSurfaceLineExtensions));

        /// <summary>
        /// Tries to set the relevant characteristic points from the <paramref name="characteristicPoints"/>
        /// on the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to set the characteristic points for.</param>
        /// <param name="characteristicPoints">The characteristic points to set, if the collection is valid.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="characteristicPoints"/> defines
        /// a dike toe at polder side in front of the dike toe at river side.</exception>
        public static void SetCharacteristicPoints(this PipingSurfaceLine surfaceLine, CharacteristicPoints characteristicPoints)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            if (characteristicPoints == null)
            {
                return;
            }

            surfaceLine.ValidateDikeToesInOrder(characteristicPoints);

            surfaceLine.TrySetDikeToeAtRiver(characteristicPoints.DikeToeAtRiver);
            surfaceLine.TrySetDitchDikeSide(characteristicPoints.DitchDikeSide);
            surfaceLine.TrySetBottomDitchDikeSide(characteristicPoints.BottomDitchDikeSide);
            surfaceLine.TrySetBottomDitchPolderSide(characteristicPoints.BottomDitchPolderSide);
            surfaceLine.TrySetDitchPolderSide(characteristicPoints.DitchPolderSide);
            surfaceLine.TrySetDikeToeAtPolder(characteristicPoints.DikeToeAtPolder);
        }

        /// <summary>
        /// Tries to set the <see cref="PipingSurfaceLine.DitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to set the 
        /// <see cref="PipingSurfaceLine.DitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="PipingSurfaceLine.DitchPolderSide"/>.</param>
        private static void TrySetDitchPolderSide(this PipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDitchPolderSideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="PipingSurfaceLine.BottomDitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to set the 
        /// <see cref="PipingSurfaceLine.BottomDitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="PipingSurfaceLine.BottomDitchPolderSide"/>.</param>
        private static void TrySetBottomDitchPolderSide(this PipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetBottomDitchPolderSideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="PipingSurfaceLine.BottomDitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to set the 
        /// <see cref="PipingSurfaceLine.BottomDitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="PipingSurfaceLine.BottomDitchDikeSide"/>.</param>
        private static void TrySetBottomDitchDikeSide(this PipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetBottomDitchDikeSideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="PipingSurfaceLine.DitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to set the 
        /// <see cref="PipingSurfaceLine.DitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="PipingSurfaceLine.DitchDikeSide"/>.</param>
        private static void TrySetDitchDikeSide(this PipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDitchDikeSideAt(point);
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="PipingSurfaceLine.DikeToeAtRiver"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to set the 
        /// <see cref="PipingSurfaceLine.DikeToeAtRiver"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="PipingSurfaceLine.DikeToeAtRiver"/>.</param>
        private static void TrySetDikeToeAtRiver(this PipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDikeToeAtRiverAt(point);
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
        }

        /// <summary>
        /// Tries to set the <see cref="PipingSurfaceLine.DikeToeAtPolder"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to set the 
        /// <see cref="PipingSurfaceLine.DikeToeAtPolder"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="PipingSurfaceLine.DikeToeAtPolder"/>.</param>
        private static void TrySetDikeToeAtPolder(this PipingSurfaceLine surfaceLine, Point3D point)
        {
            if (point != null)
            {
                try
                {
                    surfaceLine.SetDikeToeAtPolderAt(point);
                }
                catch (ArgumentException e)
                {
                    LogError(surfaceLine, e);
                }
            }
        }

        private static void LogError(PipingSurfaceLine surfaceLine, ArgumentException e)
        {
            log.ErrorFormat(Resources.SurfaceLinesCsvImporter_CharacteristicPoint_of_SurfaceLine_0_skipped_cause_1_,
                            surfaceLine.Name,
                            e.Message);
        }

        /// <summary>
        /// Validates whether or not the dike toes are in the right order.
        /// </summary>
        /// <param name="surfaceLine">The surface line.</param>
        /// <param name="characteristicPoints">The characteristic points (possibly) containing the dike toes.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when the dike toes are not in the right order.</exception>
        private static void ValidateDikeToesInOrder(this PipingSurfaceLine surfaceLine, CharacteristicPoints characteristicPoints)
        {
            if (characteristicPoints.DikeToeAtRiver != null && characteristicPoints.DikeToeAtPolder != null)
            {
                Point2D localDikeToeAtRiver = surfaceLine.GetLocalPointFromGeometry(characteristicPoints.DikeToeAtRiver);
                Point2D localDikeToeAtPolder = surfaceLine.GetLocalPointFromGeometry(characteristicPoints.DikeToeAtPolder);

                if (localDikeToeAtPolder.X <= localDikeToeAtRiver.X)
                {
                    string message = string.Format(Resources.SurfaceLinesCsvImporter_CheckCharacteristicPoints_EntryPointL_greater_or_equal_to_ExitPointL_for_0_, characteristicPoints.Name);
                    throw new ImportedDataTransformException(message);
                }
            }
        }
    }
}