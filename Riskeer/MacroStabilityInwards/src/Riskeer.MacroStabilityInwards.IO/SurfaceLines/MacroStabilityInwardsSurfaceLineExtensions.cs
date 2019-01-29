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
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SurfaceLines;
using Riskeer.MacroStabilityInwards.Primitives;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonIoResources = Riskeer.Common.IO.Properties.Resources;
using MacroStabilityInwardsIOResources = Riskeer.MacroStabilityInwards.IO.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.IO.SurfaceLines
{
    /// <summary>
    /// Extension methods for the <see cref="MacroStabilityInwardsSurfaceLine"/> class.
    /// </summary>
    public static class MacroStabilityInwardsSurfaceLineExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MacroStabilityInwardsSurfaceLineExtensions));

        /// <summary>
        /// Tries to set the relevant characteristic points from the <paramref name="characteristicPoints"/>
        /// on the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to set the characteristic points for.</param>
        /// <param name="characteristicPoints">The characteristic points to set, if the collection is valid.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when a mandatory characteristic point is not
        /// present or not on the given <paramref name="surfaceLine"/>.</exception>
        public static void SetCharacteristicPoints(this MacroStabilityInwardsSurfaceLine surfaceLine, CharacteristicPoints characteristicPoints)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            if (characteristicPoints == null)
            {
                throw new ImportedDataTransformException($"Karakteristieke punten definitie voor profielschematisatie '{surfaceLine.Name}' is verplicht.");
            }

            surfaceLine.TrySetSurfaceLevelOutside(characteristicPoints.SurfaceLevelOutside);
            surfaceLine.TrySetDikeToeAtRiver(characteristicPoints.DikeToeAtRiver);
            surfaceLine.TrySetDikeTopAtPolder(characteristicPoints.DikeTopAtPolder);
            surfaceLine.TrySetDikeTopAtRiver(characteristicPoints.DikeTopAtRiver);
            surfaceLine.TrySetDikeToeAtPolder(characteristicPoints.DikeToeAtPolder);
            surfaceLine.TrySetSurfaceLevelInside(characteristicPoints.SurfaceLevelInside);

            surfaceLine.TrySetShoulderBaseInside(characteristicPoints.ShoulderBaseInside);
            surfaceLine.TrySetShoulderTopInside(characteristicPoints.ShoulderTopInside);
            surfaceLine.TrySetDitchDikeSide(characteristicPoints.DitchDikeSide);
            surfaceLine.TrySetBottomDitchDikeSide(characteristicPoints.BottomDitchDikeSide);
            surfaceLine.TrySetBottomDitchPolderSide(characteristicPoints.BottomDitchPolderSide);
            surfaceLine.TrySetDitchPolderSide(characteristicPoints.DitchPolderSide);
        }

        /// <summary>
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.DitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.DitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.DitchPolderSide"/>.</param>
        private static void TrySetDitchPolderSide(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.BottomDitchPolderSide"/>.</param>
        private static void TrySetBottomDitchPolderSide(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.BottomDitchDikeSide"/>.</param>
        private static void TrySetBottomDitchDikeSide(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.DitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.DitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.DitchDikeSide"/>.</param>
        private static void TrySetDitchDikeSide(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.SurfaceLevelInside"/>.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetSurfaceLevelInside(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetSurfaceLevelInsideAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RiskeerCommonDataResources.CharacteristicPoint_SurfaceLevelInside), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        /// <summary>
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.SurfaceLevelOutside"/>.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetSurfaceLevelOutside(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetSurfaceLevelOutsideAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RiskeerCommonDataResources.CharacteristicPoint_SurfaceLevelOutside), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        /// <summary>
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.DikeTopAtPolder"/>.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetDikeTopAtPolder(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetDikeTopAtPolderAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RiskeerCommonDataResources.CharacteristicPoint_DikeTopAtPolder), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        /// <summary>
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.DikeTopAtRiver"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.DikeTopAtRiver"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.DikeTopAtRiver"/>.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetDikeTopAtRiver(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetDikeTopAtRiverAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RiskeerCommonDataResources.CharacteristicPoint_DikeTopAtRiver), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        /// <summary>
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.ShoulderBaseInside"/>.</param>
        private static void TrySetShoulderBaseInside(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.ShoulderTopInside"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.ShoulderTopInside"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.ShoulderTopInside"/>.</param>
        private static void TrySetShoulderTopInside(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
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
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.DikeToeAtRiver"/>.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetDikeToeAtRiver(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetDikeToeAtRiverAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RiskeerCommonDataResources.CharacteristicPoint_DikeToeAtRiver), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        /// <summary>
        /// Tries to set the <see cref="MacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to set the 
        /// <see cref="MacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="MacroStabilityInwardsSurfaceLine.DikeToeAtPolder"/>.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="point"/> is <c>null</c> or
        /// not on the <paramref name="surfaceLine"/>.</exception>
        private static void TrySetDikeToeAtPolder(this MacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
        {
            try
            {
                surfaceLine.SetDikeToeAtPolderAt(point);
            }
            catch (ArgumentNullException)
            {
                throw CreateMandatoryCharacteristicPointException(CreateMissingMandatoryPointMessage(RiskeerCommonDataResources.CharacteristicPoint_DikeToeAtPolder), surfaceLine.Name);
            }
            catch (ArgumentException e)
            {
                throw CreateMandatoryCharacteristicPointException(e.Message, surfaceLine.Name);
            }
        }

        private static void LogOptionalCharacteristicPointError(MacroStabilityInwardsSurfaceLine surfaceLine, ArgumentException e)
        {
            log.ErrorFormat(RiskeerCommonIoResources.SurfaceLinesCsvImporter_CharacteristicPoint_of_SurfaceLine_0_skipped_cause_1_,
                            surfaceLine.Name,
                            e.Message);
        }

        private static string CreateMissingMandatoryPointMessage(string surfaceLineName)
        {
            return string.Format(MacroStabilityInwardsIOResources.MacroStabilityInwardsSurfaceLineTransformer_CharacteristicPoint_0_is_undefined, surfaceLineName);
        }

        private static ImportedDataTransformException CreateMandatoryCharacteristicPointException(string exceptionMessage, string surfaceLineName)
        {
            string message = string.Format(MacroStabilityInwardsIOResources.SurfaceLinesCsvImporter_SurfaceLine_0_skipped_cause_1_CharacteristicPoint_mandatory, surfaceLineName, exceptionMessage);
            return new ImportedDataTransformException(message);
        }
    }
}