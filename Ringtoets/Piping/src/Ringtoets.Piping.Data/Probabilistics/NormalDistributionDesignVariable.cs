using MathNet.Numerics.Distributions;

namespace Ringtoets.Piping.Data.Probabilistics
{
    public class NormalDistributionDesignVariable : DesignVariable<NormalDistribution>
    {
        public NormalDistributionDesignVariable(NormalDistribution distribution) : base(distribution) {}

        public override double GetDesignValue()
        {
            // Design factor is determined using the 'probit function', which is the inverse
            // CDF function of the standard normal distribution. For more information see:
            // "Quantile function" https://en.wikipedia.org/wiki/Normal_distribution
            var designFactor = Normal.InvCDF(0.0, 1.0, Percentile);
            return Distribution.Mean + designFactor * Distribution.StandardDeviation;
        }
    }
}