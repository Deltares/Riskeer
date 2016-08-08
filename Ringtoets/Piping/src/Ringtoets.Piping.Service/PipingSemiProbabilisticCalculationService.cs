using System;
using MathNet.Numerics.Distributions;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service.Properties;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// This class is responsible for calculating a factor of safety for piping based on the sub calculations.
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
        private readonly double upliftCriticalSafetyFactor;
        private readonly double heaveNormDependentFactor;
        private readonly double sellmeijerNormDependentFactor;
        private readonly double contribution;

        // Intermediate results
        private double heaveReliability;
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
        /// <param name="upliftCriticalSafetyFactor">The critical safety factor which is compared to the safety factor of uplift to determine a probability.</param>
        /// <param name="heaveNormDependentFactor">The norm dependent factor used in determining the reliability of heave.</param>
        /// <param name="sellmeijerNormDependentFactor">The norm dependent factor used in determining the reliability of Sellmeijer.</param>
        /// <param name="contribution">The contribution of piping to the total failure.</param>
        private PipingSemiProbabilisticCalculationService(double upliftFactorOfSafety, double heaveFactorOfSafety, double sellmeijerFactorOfSafety, int returnPeriod, double constantA, double constantB, double assessmentSectionLength, double upliftCriticalSafetyFactor, double heaveNormDependentFactor, double sellmeijerNormDependentFactor, double contribution)
        {
            this.heaveFactorOfSafety = heaveFactorOfSafety;
            this.upliftFactorOfSafety = upliftFactorOfSafety;
            this.sellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
            this.returnPeriod = returnPeriod;
            this.constantA = constantA;
            this.constantB = constantB;
            this.assessmentSectionLength = assessmentSectionLength;
            this.upliftCriticalSafetyFactor = upliftCriticalSafetyFactor;
            this.heaveNormDependentFactor = heaveNormDependentFactor;
            this.sellmeijerNormDependentFactor = sellmeijerNormDependentFactor;
            this.contribution = contribution;
        }

        /// <summary>
        /// Calculates the semi-probabilistic results given a <see cref="PipingCalculation"/> with <see cref="PipingOutput"/>.
        /// </summary>
        /// <param name="calculation">The calculation which is used as input for the semi-probabilistic assessment. If the semi-
        /// probabilistic calculation is successful, <see cref="PipingCalculation.SemiProbabilisticOutput"/> is set.</param>
        /// <param name="pipingProbabilityAssessmentInput">General input that influences the probability estimate for a piping
        /// assessment.</param>
        /// <param name="norm">The return period to assess for.</param>
        /// <param name="contribution">The contribution of piping as a percentage (0-100) to the total of the failure probability
        /// of the assessment section.</param>
        /// <exception cref="ArgumentException">Thrown when calculation has no output from a piping calculation.</exception>
        public static void Calculate(PipingCalculation calculation, PipingProbabilityAssessmentInput pipingProbabilityAssessmentInput, int norm, double contribution)
        {
            ValidateOutputOnCalculation(calculation);

            PipingOutput pipingOutput = calculation.Output;

            var calculator = new PipingSemiProbabilisticCalculationService(
                pipingOutput.UpliftFactorOfSafety,
                pipingOutput.HeaveFactorOfSafety,
                pipingOutput.SellmeijerFactorOfSafety,
                norm,
                pipingProbabilityAssessmentInput.A,
                pipingProbabilityAssessmentInput.B,
                pipingProbabilityAssessmentInput.SectionLength,
                pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor,
                pipingProbabilityAssessmentInput.GetHeaveNormDependentFactor(norm),
                pipingProbabilityAssessmentInput.GetSellmeijerNormDependentFactor(norm),
                contribution/100);

            calculator.Calculate();

            calculation.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                calculator.upliftFactorOfSafety,
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

        /// <summary>
        /// Performs the full semi-probabilistic calculation while setting intermediate results.
        /// </summary>
        private void Calculate()
        {
            CalculatePipingReliability();

            CalculateRequiredReliability();

            pipingFactorOfSafety = pipingReliability/requiredReliability;
        }

        /// <summary>
        /// Calculates the required reliability based on the norm and length of the assessment section and the contribution of piping.
        /// </summary>
        private void CalculateRequiredReliability()
        {
            requiredProbability = RequiredProbability();
            requiredReliability = ProbabilityToReliability(requiredProbability);
        }

        /// <summary>
        /// Calculates the reliability of piping based on the factors of safety from the sub-mechanisms.
        /// </summary>
        private void CalculatePipingReliability()
        {
            upliftProbability = UpliftProbability();

            heaveReliability = HeaveReliability(heaveFactorOfSafety);
            heaveProbability = ReliabilityToProbability(heaveReliability);

            sellmeijerReliability = SellmeijerReliability(sellmeijerFactorOfSafety);
            sellmeijerProbability = ReliabilityToProbability(sellmeijerReliability);

            pipingProbability = PipingProbability(upliftProbability, heaveProbability, sellmeijerProbability);
            pipingReliability = ProbabilityToReliability(pipingProbability);
        }

        /// <summary>
        /// Calculates the probability of occurrence of the piping failure mechanism.
        /// </summary>
        /// <param name="probabilityOfHeave">The calculated probability of the heave sub-mechanism.</param>
        /// <param name="probabilityOfUplift">The calculated probability of the uplift sub-mechanism.</param>
        /// <param name="probabilityOfSellmeijer">The calculated probability of the Sellmeijer sub-mechanism.</param>
        /// <returns>A value representing the probability of occurrence of piping.</returns>
        private static double PipingProbability(double probabilityOfHeave, double probabilityOfUplift, double probabilityOfSellmeijer)
        {
            return Math.Min(Math.Min(probabilityOfHeave, probabilityOfUplift), probabilityOfSellmeijer);
        }

        /// <summary>
        /// Calculates the required probability of the piping failure mechanism for the complete assessment section.
        /// </summary>
        /// <returns>A value representing the required probability.</returns>
        private double RequiredProbability()
        {
            return (contribution/returnPeriod)/(1 + (constantA*assessmentSectionLength)/constantB);
        }

        private double UpliftProbability()
        {
            return upliftFactorOfSafety <= upliftCriticalSafetyFactor ? 1 : 0;
        }

        private double HeaveReliability(double factorOfSafety)
        {
            return 2.08*Math.Log(factorOfSafety/heaveNormDependentFactor);
        }

        private double SellmeijerReliability(double factorOfSafety)
        {
            return 2.7*Math.Log(factorOfSafety/sellmeijerNormDependentFactor);
        }

        private static void ValidateOutputOnCalculation(PipingCalculation calculation)
        {
            if (!calculation.HasOutput)
            {
                throw new ArgumentException(Resources.PipingSemiProbabilisticCalculationService_ValidateOutputOnCalculation_Factor_of_safety_cannot_be_calculated);
            }
        }

        private static double ReliabilityToProbability(double reliability)
        {
            return Normal.CDF(0, 1, -reliability);
        }

        private static double ProbabilityToReliability(double probability)
        {
            return Normal.InvCDF(0, 1, 1 - probability);
        }
    }
}