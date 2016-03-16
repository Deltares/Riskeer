using System;
using MathNet.Numerics.Distributions;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// This class is responsible for calculating a factor of safety for piping based on the sub-calculations.
    /// </summary>
    public class PipingSemiProbabilisticCalculationService
    {
        private readonly double heaveFactorOfSafety;
        private readonly double upliftFactorOfSafety;
        private readonly double sellmeijerFactorOfSafety;

        private readonly int returnPeriod;
        private readonly double constantA;
        private readonly double constantB;
        private readonly double assessmentSectionLength;
        private readonly double contribution;

        /// <summary>
        /// Calculates the semi-probabilistic results given a <see cref="PipingCalculation"/> with <see cref="PipingOutput"/>.
        /// </summary>
        /// <param name="calculation">The calculation which is used as input for the semi-probabilistic assessment. If the semi-
        /// probabilistic calculation is successful, <see cref="PipingCalculation.SemiProbabilisticOutput"/> is set.</param>
        /// <exception cref="ArgumentNullException">Thrown when calculation has no output from a piping calculation.</exception>
        public static void Calculate(PipingCalculation calculation)
        {
            ValidateOutputOnCalculation(calculation);

            SemiProbabilisticPipingInput semiProbabilisticParameters = calculation.SemiProbabilisticParameters;
            var pipingOutput = calculation.Output;

            var calculator = new PipingSemiProbabilisticCalculationService(
				pipingOutput.UpliftFactorOfSafety,
                pipingOutput.HeaveFactorOfSafety,
                pipingOutput.SellmeijerFactorOfSafety,
                semiProbabilisticParameters.Norm,
                semiProbabilisticParameters.A,
                semiProbabilisticParameters.B,
                semiProbabilisticParameters.SectionLength,
                semiProbabilisticParameters.Contribution/100);

            calculation.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                pipingOutput.UpliftFactorOfSafety,
                pipingOutput.HeaveFactorOfSafety,
                pipingOutput.SellmeijerFactorOfSafety,
                calculator.FactorOfSafety()
            );
        }

        private static void ValidateOutputOnCalculation(PipingCalculation calculation)
        {
            if (!calculation.HasOutput)
            {
                throw new ArgumentNullException("calculation", "Cannot perform a semi-probabilistic calculation without output form the piping kernel.");
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="PipingSemiProbabilisticCalculationService"/>.
        /// </summary>
        /// <param name="upliftFactorOfSafety">The factor of safety for the uplift sub calculation.</param>
        /// <param name="heaveFactorOfSafety">The factor of safety for the heave sub calculation.</param>
        /// <param name="sellmeijerFactorOfSafety">The factor of safety for the Sellmeijer sub calculation.</param>
        /// <param name="returnPeriod">The return period.</param>
        /// <param name="constantA">The constant a.</param>
        /// <param name="constantB">The constant b.</param>
        /// <param name="assessmentSectionLength">The length of the assessment section.</param>
        /// <param name="contribution">The contribution of piping to the total failure.</param>
        public PipingSemiProbabilisticCalculationService(double upliftFactorOfSafety, double heaveFactorOfSafety, double sellmeijerFactorOfSafety, int returnPeriod, double constantA, double constantB, double assessmentSectionLength, double contribution)
        {
            this.heaveFactorOfSafety = heaveFactorOfSafety;
            this.upliftFactorOfSafety = upliftFactorOfSafety;
            this.sellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
            this.returnPeriod = returnPeriod;
            this.constantA = constantA;
            this.constantB = constantB;
            this.assessmentSectionLength = assessmentSectionLength;
            this.contribution = contribution;
        }

        /// <summary>
        /// Returns the failure probability of the uplift sub mechanism.
        /// </summary>
        /// <returns>A value represening failure probability.</returns>
        public double FailureProbabilityUplift()
        {
            return FailureProbability(upliftFactorOfSafety, upliftFactors);
        }

        /// <summary>
        /// Returns the failure probability of the heave sub mechanism.
        /// </summary>
        /// <returns>A value represening failure probability.</returns>
        public double FailureProbabilityHeave()
        {
            return FailureProbability(heaveFactorOfSafety, heaveFactors);
        }

        /// <summary>
        /// Returns the failure probability of the Sellmeijer sub mechanism.
        /// </summary>
        /// <returns>A value represening failure probability.</returns>
        public double FailureProbabilitySellmeijer()
        {
            return FailureProbability(sellmeijerFactorOfSafety, sellmeijerFactors);
        }

        /// <summary>
        /// Returns the reliability index of the piping failure mechanism.
        /// </summary>
        /// <returns>A value representing the reliability.</returns>
        public double BetaCrossPiping()
        {
            var minFailureProbability = Math.Min(Math.Min(FailureProbabilityHeave(), FailureProbabilityUplift()), FailureProbabilitySellmeijer());
            return Normal.InvCDF(0.0, 1.0, 1 - minFailureProbability);
        }

        /// <summary>
        /// Returns the allowed reliability of the piping failure mechanism for the complete assessment section.
        /// </summary>
        /// <returns>A value representing the allowed reliability.</returns>
        public double BetaCrossAllowed()
        {
            var normCross = (contribution/returnPeriod)/(1 + (constantA*assessmentSectionLength)/constantB);
            return Normal.InvCDF(0, 1, 1 - normCross);
        }

        /// <summary>
        /// Returns the safety factor of piping based on the factor of safety of
        /// the sub mechanisms.
        /// </summary>
        /// <returns>A factor of safety value.</returns>
        public double FactorOfSafety()
        {
            return BetaCrossAllowed()/BetaCrossPiping();
        }

        private double FailureProbability(double factorOfSafety, SubCalculationFactors factors)
        {
            var norm = (1.0/returnPeriod);
            var bNorm = Normal.InvCDF(0, 1, 1 - norm);

            var betaCross = (1/factors.A)*(Math.Log(factorOfSafety/factors.B) + (factors.C*bNorm));

            return Normal.CDF(0, 1, -betaCross);
        }

        #region sub-calculation constants

        private struct SubCalculationFactors
        {
            public double A;
            public double B;
            public double C;
        }

        private readonly SubCalculationFactors upliftFactors = new SubCalculationFactors
        {
            A = 0.46, B = 0.48, C = 0.27
        };

        private readonly SubCalculationFactors heaveFactors = new SubCalculationFactors
        {
            A = 0.48, B = 0.37, C = 0.30
        };

        private readonly SubCalculationFactors sellmeijerFactors = new SubCalculationFactors
        {
            A = 0.37, B = 1.04, C = 0.43
        };

        #endregion
    }
}