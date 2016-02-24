using System;
using Core.Common.Base.Geometry;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.Properties;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    public static class RingtoetsPipingSurfaceLineExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RingtoetsPipingSurfaceLineExtensions));

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