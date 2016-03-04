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
        public static void SetEntryPointL(this PipingInput input, double entryPointL)
        {
            try
            {
                input.SetSeepageLengthMean(input.ExitPointL - entryPointL);
            }
            catch (ArgumentOutOfRangeException)
            {
                var message = string.Format(Resources.PipingInputContextProperties_EntryPointL_Value_0_results_in_invalid_seepage_length, entryPointL);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Sets the L-coordinate of the exit point.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to update the entry point for.</param>
        /// <param name="exitPointL">The L-coordinate of the entry point to set.</param>
        public static void SetExitPointL(this PipingInput input, double exitPointL)
        {
            var exitPointLChange = exitPointL - input.ExitPointL;
            try
            {
                input.SetSeepageLengthMean(input.SeepageLength.Mean + exitPointLChange);
            }
            catch (ArgumentOutOfRangeException)
            {
                var message = string.Format(Resources.PipingInputContextProperties_ExitPointL_Value_0_results_in_invalid_seepage_length, exitPointL);
                throw new ArgumentException(message);
            }
            input.ExitPointL = exitPointL;
            input.UpdateThicknessCoverageLayer();
        }

        private static void UpdateThicknessCoverageLayer(this PipingInput input)
        {
            if (input.SurfaceLine != null && input.SoilProfile != null)
            {
                try
                {
                    input.ThicknessCoverageLayer.Mean = PipingCalculationService.CalculateThicknessCoverageLayer(input);
                    return;
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
                input.SetSeepageLengthMean(tempExitPointL - tempEntryPointL);
            }
        }

        /// <summary>
        /// Sets the mean of the seepage length stochast.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to update the seepage length for.</param>
        /// <param name="mean">The mean to set.</param>
        private static void SetSeepageLengthMean(this PipingInput input, double mean)
        {
            input.SeepageLength.Mean = mean;
            input.SeepageLength.StandardDeviation = mean*PipingInput.SeepageLengthStandardDeviationFraction;
        }
    }
}