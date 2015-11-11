using System;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// This class defines a design variable for a lognormal distribution.
    /// </summary>
    public class LognormalDistributionDesignVariable : DesignVariable<LognormalDistribution>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LognormalDistributionDesignVariable"/> class.
        /// </summary>
        /// <param name="distribution">A lognormal distribution.</param>
        public LognormalDistributionDesignVariable(LognormalDistribution distribution) : base(distribution) {}

        public override double GetDesignValue()
        {
            var normalSpaceDesignValue = DetermineDesignValueInNormalDistributionSpace();
            return ProjectFromNormalToLognormalSpace(normalSpaceDesignValue);
        }

        /// <summary>
        /// Projects <see cref="DesignVariable{DistributionType}.Distribution"/> into 'normal
        /// distribution' space and calculates the design value for that value space.
        /// </summary>
        /// <returns>The design value in 'normal distribution' space.</returns>
        /// <remarks>Design values can only be determined in 'normal distribution' space.</remarks>
        private double DetermineDesignValueInNormalDistributionSpace()
        {
            var normalDistribution = CreateNormalDistributionFromLognormalDistribution();
            var normalDistributionDesignVariable = new NormalDistributionDesignVariable(normalDistribution)
            {
                Percentile = Percentile
            };
            return normalDistributionDesignVariable.GetDesignValue();
        }

        private static double ProjectFromNormalToLognormalSpace(double normalSpaceDesignValue)
        {
            return Math.Exp(normalSpaceDesignValue);
        }

        /// <summary>
        /// Determine normal distribution parameters from log-normal parameters, as
        /// design value can only be determined in 'normal distribution' space.
        /// Below formula's come from Tu-Delft College dictaat "b3 Probabilistisch Ontwerpen"
        /// by ir. A.C.W.M. Vrouwenvelder and ir.J.K. Vrijling 5th reprint 1987.
        /// </summary>
        /// <returns>A normal distribution based on the parameters of <see cref="DesignVariable{DistributionType}.Distribution"/>.</returns>
        private NormalDistribution CreateNormalDistributionFromLognormalDistribution()
        {
            double sigmaLogOverMuLog = Distribution.StandardDeviation / Distribution.Mean;
            double sigmaNormal = Math.Sqrt(Math.Log(sigmaLogOverMuLog * sigmaLogOverMuLog + 1.0));
            double muNormal = Math.Log(Distribution.Mean) - 0.5 * sigmaNormal * sigmaNormal;

            return new NormalDistribution
            {
                Mean = muNormal,
                StandardDeviation = sigmaNormal
            };
        }
    }
}