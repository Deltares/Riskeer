using System;
using MathNet.Numerics.Distributions;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.SemiProbabilistic
{
    /// <summary>
    /// This class is responsible for calculating a factor of safety for piping based on the sub-calculations.
    /// </summary>
    public class PipingSemiProbabilisticResultTransformer
    {
        private readonly PipingOutput result;
        private readonly int returnPeriod;
        private readonly double constantA;
        private readonly double constantB;
        private readonly double assessmentSectionLength;
        private readonly double contribution;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSemiProbabilisticResultTransformer"/>.
        /// </summary>
        /// <param name="result">The object containing results for piping's sub mechanisms.</param>
        /// <param name="returnPeriod">The return period.</param>
        /// <param name="constantA">The constant a.</param>
        /// <param name="constantB">The constant b.</param>
        /// <param name="assessmentSectionLength">The length of the assessment section.</param>
        /// <param name="contribution">The contribution of piping to the total failure.</param>
        public PipingSemiProbabilisticResultTransformer(PipingOutput result, int returnPeriod, double constantA, double constantB, double assessmentSectionLength, double contribution)
        {
            this.result = result;
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
            var factorOfSafety = result.UpliftFactorOfSafety;
            return FailureProbability(factorOfSafety, upliftFactors);
        }

        /// <summary>
        /// Returns the failure probability of the heave sub mechanism.
        /// </summary>
        /// <returns>A value represening failure probability.</returns>
        public double FailureProbabilityHeave()
        {
            var factorOfSafety = result.HeaveFactorOfSafety;
            return FailureProbability(factorOfSafety, heaveFactors);
        }

        /// <summary>
        /// Returns the failure probability of the Sellmeijer sub mechanism.
        /// </summary>
        /// <returns>A value represening failure probability.</returns>
        public double FailureProbabilitySellmeijer()
        {
            var factorOfSafety = result.SellmeijerFactorOfSafety;
            return FailureProbability(factorOfSafety, sellmeijerFactors);
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