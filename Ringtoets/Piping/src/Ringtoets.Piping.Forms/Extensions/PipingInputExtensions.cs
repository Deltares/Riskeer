using System;
using System.Linq;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using log4net;
using Ringtoets.Piping.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Forms.Extensions
{
    /// <summary>
    /// This classe extends the <see cref="PipingInput"/> class with methods for modifying
    /// properties and propagating the change to other (dependent) properties throughout
    /// the <see cref="PipingInput"/> instance.
    /// </summary>
    public static class PipingInputExtensions
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PipingInput));

        /// <summary>
        /// Updates the <see cref="PipingInput.SurfaceLine"/> property and propagates changes to dependent
        /// properties.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to update the surface line for.</param>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to set.</param>
        public static void SetSurfaceLine(this PipingInput input, RingtoetsPipingSurfaceLine surfaceLine)
        {
            input.SurfaceLine = surfaceLine;
            input.UpdateValuesBasedOnSurfaceLine();
            input.UpdateThicknessCoverageLayer();
            input.UpdateThicknessAquiferLayer();
        }

        public static void SetSoilProfile(this PipingInput input, PipingSoilProfile soilProfile)
        {
            input.SoilProfile = soilProfile;
            input.UpdateThicknessCoverageLayer();
            input.UpdateThicknessAquiferLayer();
        }

        /// <summary>
        /// Sets the L-coordinate of the entry point.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to update the entry point for.</param>
        /// <param name="entryPointL">The L-coordinate of the entry point to set.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="entryPointL"/> is less than or equal to 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="entryPointL"/> is <see cref="double.NaN"/>.</exception>
        public static void SetEntryPointL(this PipingInput input, RoundedDouble entryPointL)
        {
            input.EntryPointL = entryPointL;
        }

        /// <summary>
        /// Sets the L-coordinate of the exit point.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to update the entry point for.</param>
        /// <param name="exitPointL">The L-coordinate of the entry point to set.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="exitPointL"/> is less or equal to 0.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="exitPointL"/> is <see cref="double.NaN"/></item>
        /// <item>Setting the value would result in an invalid seepage length.</item>
        /// </list></exception>
        public static void SetExitPointL(this PipingInput input, RoundedDouble exitPointL)
        {
            input.ExitPointL = exitPointL;
            input.UpdateThicknessCoverageLayer();
            input.UpdateThicknessAquiferLayer();
        }

        private static void UpdateThicknessAquiferLayer(this PipingInput input)
        {
            PipingSoilProfile soilProfile = input.SoilProfile;
            RingtoetsPipingSurfaceLine surfaceLine = input.SurfaceLine;
            double exitPointL = input.ExitPointL;

            double thicknessTopAquiferLayer = GetThicknessTopAquiferLayer(soilProfile, surfaceLine, exitPointL);
            TrySetThicknessAquiferLayerMean(input, thicknessTopAquiferLayer);

            if (double.IsNaN(input.ThicknessAquiferLayer.Mean))
            {
                logger.Warn(Resources.PipingInputExtensions_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer);
            }
        }

        private static double GetThicknessTopAquiferLayer(PipingSoilProfile soilProfile, RingtoetsPipingSurfaceLine surfaceLine, double exitPointL)
        {
            var thicknessTopAquiferLayer = double.NaN;

            if (soilProfile != null && surfaceLine != null && !double.IsNaN(exitPointL))
            {
                thicknessTopAquiferLayer = soilProfile.GetTopAquiferLayerThicknessBelowLevel(surfaceLine.GetZAtL(exitPointL));
            }

            return thicknessTopAquiferLayer;
        }

        private static void TrySetThicknessAquiferLayerMean(PipingInput input, double thicknessTopAquiferLayer)
        {
            try
            {
                input.ThicknessAquiferLayer.Mean = thicknessTopAquiferLayer;
            }
            catch (ArgumentOutOfRangeException)
            {
                input.ThicknessAquiferLayer.Mean = double.NaN;
            }
        }

        private static void UpdateThicknessCoverageLayer(this PipingInput input)
        {
            double derivedThickness = GetThicknessCoverageLayer(input);
            TrySetThicknessCoverageLayer(input, derivedThickness);

            if (double.IsNaN(input.ThicknessCoverageLayer.Mean))
            {
                logger.Warn(Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            }
        }

        private static double GetThicknessCoverageLayer(PipingInput input)
        {
            if (input.SurfaceLine != null && input.SoilProfile != null && !double.IsNaN(input.ExitPointL))
            {
                try
                {
                    return PipingCalculationService.CalculateThicknessCoverageLayer(input);
                }
                catch (PipingCalculatorException)
                {
                    return double.NaN;
                }
            }
            return double.NaN;
        }

        private static void TrySetThicknessCoverageLayer(PipingInput input, double derivedThickness)
        {
            try
            {
                input.ThicknessCoverageLayer.Mean = derivedThickness;
            }
            catch (ArgumentOutOfRangeException)
            {
                input.ThicknessCoverageLayer.Mean = Double.NaN;
            }
        }

        private static void UpdateValuesBasedOnSurfaceLine(this PipingInput input)
        {
            if (input.SurfaceLine == null)
            {
                input.ExitPointL = (RoundedDouble)double.NaN;
                input.SeepageLength.Mean = double.NaN;
            }
            else
            {
                int entryPointIndex = Array.IndexOf(input.SurfaceLine.Points, input.SurfaceLine.DikeToeAtRiver);
                int exitPointIndex = Array.IndexOf(input.SurfaceLine.Points, input.SurfaceLine.DikeToeAtPolder);

                Point2D[] localGeometry = input.SurfaceLine.ProjectGeometryToLZ().ToArray();

                double tempEntryPointL = localGeometry[0].X;
                double tempExitPointL = localGeometry[localGeometry.Length - 1].X;

                bool isDifferentPoints = entryPointIndex < 0 || exitPointIndex < 0 || entryPointIndex < exitPointIndex;
                if (isDifferentPoints && exitPointIndex > 0)
                {
                    tempExitPointL = localGeometry.ElementAt(exitPointIndex).X;
                }
                if (isDifferentPoints && entryPointIndex > -1)
                {
                    tempEntryPointL = localGeometry.ElementAt(entryPointIndex).X;
                }

                input.ExitPointL = (RoundedDouble)tempExitPointL;
                input.EntryPointL = (RoundedDouble)tempEntryPointL;
            }
        }
    }
}