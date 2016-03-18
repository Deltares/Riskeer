using System;
using Core.Common.Base.Geometry;
using log4net;
using Ringtoets.Piping.Plugin.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Extension methods for the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine"/> class.
    /// </summary>
    public static class RingtoetsPipingSurfaceLineExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RingtoetsPipingSurfaceLineExtensions));

        /// <summary>
        /// Tries to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DitchPolderSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DitchPolderSide"/> was set, <c>false</c> if
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
        /// Tries to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/> was set, <c>false</c> if
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
        /// Tries to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/> was set, <c>false</c> if
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
        /// Tries to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DitchDikeSide"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DitchDikeSide"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DitchDikeSide"/>.</param>
        /// <returns><c>true</c> if the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DitchDikeSide"/> was set, <c>false</c> if
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
        /// Tries to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DikeToeAtRiver"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DikeToeAtRiver"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DikeToeAtRiver"/>.</param>
        /// <returns><c>true</c> if the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DikeToeAtRiver"/> was set, <c>false</c> if
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
        /// Tries to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DikeToeAtPolder"/> at the location of
        /// <paramref name="point"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine"/> to set the 
        /// <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DikeToeAtPolder"/> for.</param>
        /// <param name="point">The point at which to set the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DikeToeAtPolder"/>.</param>
        /// <returns><c>true</c> if the <see cref="Ringtoets.Piping.Primitives.RingtoetsPipingSurfaceLine.DikeToeAtPolder"/> was set, <c>false</c> if
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