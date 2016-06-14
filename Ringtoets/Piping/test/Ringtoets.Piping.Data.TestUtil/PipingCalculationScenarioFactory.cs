using System;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating different instances of <see cref="PipingCalculationScenario"/>
    /// for easier testing.
    /// </summary>
    public static class PipingCalculationScenarioFactory
    {
        /// <summary>
        /// Creates a calculated scenario for which the surface line on the input intersects with <paramref name="section"/>.
        /// </summary>
        /// <param name="probability">The value for <see cref="PipingSemiProbabilisticOutput.PipingProbability"/>.</param>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreatePipingCalculationScenario(double probability, FailureMechanismSection section)
        {
            var scenario = CreateNotCalculatedPipingCalculationScenario(section);
            var random = new Random(21);
            scenario.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                (RoundedDouble) probability,
                random.NextDouble(),
                random.NextDouble());

            scenario.Output = new PipingOutput(
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble());

            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has failed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreateFailedPipingCalculationScenario(FailureMechanismSection section)
        {
            return CreatePipingCalculationScenario((RoundedDouble) double.NaN, section);
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// is marked as not relevant for the assessment.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreateIrreleveantPipingCalculationScenario(FailureMechanismSection section)
        {
            var scenario = CreateNotCalculatedPipingCalculationScenario(section);
            scenario.IsRelevant = false;
            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has not been performed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreateNotCalculatedPipingCalculationScenario(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            var p = section.Points.First();
            ringtoetsPipingSurfaceLine.SetGeometry(new [] { new Point3D(p.X, p.Y, 0), new Point3D(p.X+2, p.Y+2, 0) } );
            ringtoetsPipingSurfaceLine.ReferenceLineIntersectionWorldPoint = section.Points.First();

            var scenario = new PipingCalculationScenario(new GeneralPipingInput())
            {
                IsRelevant = true,
                InputParameters =
                {
                    SurfaceLine = ringtoetsPipingSurfaceLine
                }
            };
            return scenario;
        }
    }
}