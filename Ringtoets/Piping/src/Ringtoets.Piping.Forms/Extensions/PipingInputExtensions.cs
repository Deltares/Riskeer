using System;
using System.Linq;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.Extensions
{
    /// <summary>
    /// This classe extends the <see cref="PipingInput"/> class with methods for modifying
    /// properties and propagating the change to other (dependent) properties throughout
    /// the <see cref="PipingInput"/> instance.
    /// </summary>
    public static class PipingInputExtensions
    {
        /// <summary>
        /// Updates the <see cref="PipingInput.SurfaceLine"/> property and propagates changes to dependent
        /// properties.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to update the surface line for.</param>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to set.</param>
        public static void SetSurfaceLine(this PipingInput input, RingtoetsPipingSurfaceLine surfaceLine)
        {
            input.SurfaceLine = surfaceLine;
            UpdateValuesBasedOnSurfaceLine(input);
        }

        private static void UpdateValuesBasedOnSurfaceLine(PipingInput input)
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

        /// <summary>
        /// Sets the mean of the seepage length stochast.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to update the seepage length for.</param>
        /// <param name="mean">The mean to set.</param>
        public static void SetSeepageLengthMean(this PipingInput input, double mean)
        {
            input.SeepageLength.Mean = mean;
            input.SeepageLength.StandardDeviation = mean * PipingInput.SeepageLengthStandardDeviationFraction;
        }
    }
}