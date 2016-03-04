using System;
using System.Linq;
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
        }

        public static void SetSoilProfile(this PipingInput input, PipingSoilProfile soilProfile)
        {
            input.SoilProfile = soilProfile;
            input.UpdateThicknessCoverageLayer();
        }

        /// <summary>
        /// Sets the L-coordinate of the entry point.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to update the entry point for.</param>
        /// <param name="entryPointL">The L-coordinate of the entry point to set.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="entryPointL"/> is less than or equal to 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="entryPointL"/> is <see cref="double.NaN"/>.</exception>
        public static void SetEntryPointL(this PipingInput input, double entryPointL)
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
        public static void SetExitPointL(this PipingInput input, double exitPointL)
        {
            input.ExitPointL = exitPointL;
            input.UpdateThicknessCoverageLayer();
        }

        private static void UpdateThicknessCoverageLayer(this PipingInput input)
        {
            if (input.SurfaceLine != null && input.SoilProfile != null)
            {
                try
                {
                    double derivedThickness = PipingCalculationService.CalculateThicknessCoverageLayer(input);
                    if (!double.IsNaN(derivedThickness))
                    {
                        input.ThicknessCoverageLayer.Mean = derivedThickness;
                        return;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    // error handling performed after try-catch
                }
                catch (PipingCalculatorException)
                {
                    // error handling performed after try-catch
                }
            }

            logger.Warn(Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            input.ThicknessCoverageLayer.Mean = double.NaN;
        }

        private static void UpdateValuesBasedOnSurfaceLine(this PipingInput input)
        {
            if (input.SurfaceLine == null)
            {
                input.ExitPointL = double.NaN;
                input.SeepageLength.Mean = double.NaN;
            }
            else
            {
                var entryPointIndex = Array.IndexOf(input.SurfaceLine.Points, input.SurfaceLine.DikeToeAtRiver);
                var exitPointIndex = Array.IndexOf(input.SurfaceLine.Points, input.SurfaceLine.DikeToeAtPolder);

                var localGeometry = input.SurfaceLine.ProjectGeometryToLZ().ToArray();

                var tempEntryPointL = localGeometry[0].X;
                var tempExitPointL = localGeometry[localGeometry.Length - 1].X;

                var differentPoints = entryPointIndex < 0 || exitPointIndex < 0 || entryPointIndex < exitPointIndex;
                if (differentPoints && exitPointIndex > 0)
                {
                    tempExitPointL = localGeometry.ElementAt(exitPointIndex).X;
                }
                if (differentPoints && entryPointIndex > -1)
                {
                    tempEntryPointL = localGeometry.ElementAt(entryPointIndex).X;
                }

                input.ExitPointL = tempExitPointL;
                input.EntryPointL = tempEntryPointL;
            }
        }
    }
}