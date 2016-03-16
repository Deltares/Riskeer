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
        // Inputs
        private readonly double upliftFactorOfSafety;
        private readonly double heaveFactorOfSafety;
        private readonly double sellmeijerFactorOfSafety;

        private readonly int returnPeriod;
        private readonly double constantA;
        private readonly double constantB;
        private readonly double assessmentSectionLength;
        private readonly double contribution;

        // Intermediate results
        private double heaveReliability;
        private double upliftReliability;
        private double sellmeijerReliability;

        private double heaveProbability;
        private double upliftProbability;
        private double sellmeijerProbability;

        private double pipingProbability;
        private double pipingReliability;

        private double requiredProbability;
        private double requiredReliability;

        private double pipingFactorOfSafety;

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
                semiProbabilisticParameters.Contribution / 100);
            calculator.Calculate();

            calculation.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                calculator.upliftFactorOfSafety,
                calculator.upliftReliability,
                calculator.upliftProbability,
                calculator.heaveFactorOfSafety,
                calculator.heaveReliability,
                calculator.heaveProbability,
                calculator.sellmeijerFactorOfSafety,
                calculator.sellmeijerReliability,
                calculator.sellmeijerProbability,
                calculator.requiredProbability,
                calculator.requiredReliability,
                calculator.pipingProbability,
                calculator.pipingReliability,
                calculator.pipingFactorOfSafety
                );
        }

        private void Calculate()
        {
            FailureProbabilityUplift();
            FailureProbabilityHeave();
            FailureProbabilitySellmeijer();
            BetaCrossPiping();
            BetaCrossRequired();
            FactorOfSafety();
        }

        /// <summary>
        /// Returns the failure probability of the uplift sub mechanism.
        /// </summary>
        /// <returns>A value represening failure probability.</returns>
        public double FailureProbabilityUplift()
        {
            upliftReliability = SubMechanismReliability(upliftFactorOfSafety, upliftFactors);
            upliftProbability = ReliabilityToProbability(upliftReliability);
            return upliftProbability;
        }

        /// <summary>
        /// Returns the failure probability of the heave sub mechanism.
        /// </summary>
        /// <returns>A value represening failure probability.</returns>
        public double FailureProbabilityHeave()
        {
            heaveReliability = SubMechanismReliability(heaveFactorOfSafety, heaveFactors);
            heaveProbability = ReliabilityToProbability(heaveReliability);
            return heaveProbability;
        }

        /// <summary>
        /// Returns the failure probability of the Sellmeijer sub mechanism.
        /// </summary>
        /// <returns>A value represening failure probability.</returns>
        public double FailureProbabilitySellmeijer()
        {
            sellmeijerReliability = SubMechanismReliability(sellmeijerFactorOfSafety, sellmeijerFactors);
            sellmeijerProbability = ReliabilityToProbability(sellmeijerReliability);
            return sellmeijerProbability;
        }

        /// <summary>
        /// Returns the reliability index of the piping failure mechanism.
        /// </summary>
        /// <returns>A value representing the reliability.</returns>
        public double BetaCrossPiping()
        {
            pipingProbability = Math.Min(Math.Min(heaveProbability, upliftProbability), sellmeijerProbability);
            pipingReliability = ProbabilityToReliability(pipingProbability);
            return pipingReliability;
        }

        /// <summary>
        /// Returns the required reliability of the piping failure mechanism for the complete assessment section.
        /// </summary>
        /// <returns>A value representing the required reliability.</returns>
        public double BetaCrossRequired()
        {
            requiredProbability = (contribution/returnPeriod)/(1 + (constantA*assessmentSectionLength)/constantB);
            requiredReliability = ProbabilityToReliability(requiredProbability);
            return requiredReliability;
        }

        /// <summary>
        /// Returns the safety factor of piping based on the factor of safety of
        /// the sub mechanisms.
        /// </summary>
        /// <returns>A factor of safety value.</returns>
        public double FactorOfSafety()
        {
            pipingFactorOfSafety = requiredReliability/pipingReliability;
            return pipingFactorOfSafety;
        }

        private static void ValidateOutputOnCalculation(PipingCalculation calculation)
        {
            if (!calculation.HasOutput)
            {
                throw new ArgumentNullException("calculation", "Cannot perform a semi-probabilistic calculation without output form the piping kernel.");
            }
        }

        private double SubMechanismReliability(double factorOfSafety, SubCalculationFactors factors)
        {
            var norm = (1.0/returnPeriod);
            var bNorm = ProbabilityToReliability(norm);

            return (1/factors.A)*(Math.Log(factorOfSafety/factors.B) + (factors.C*bNorm));
        }

        private double ReliabilityToProbability(double reliability)
        {
            return Normal.CDF(0, 1, -reliability);
        }

        private double ProbabilityToReliability(double probability)
        {
            return Normal.InvCDF(0, 1, 1 - probability);
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