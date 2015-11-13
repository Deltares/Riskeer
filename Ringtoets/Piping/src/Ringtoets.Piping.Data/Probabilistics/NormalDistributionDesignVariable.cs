namespace Ringtoets.Piping.Data.Probabilistics
{
    public class NormalDistributionDesignVariable : DesignVariable<NormalDistribution>
    {
        public NormalDistributionDesignVariable(NormalDistribution distribution) : base(distribution) {}

        public override double GetDesignValue()
        {
            return DetermineDesignValue(Distribution.Mean, Distribution.StandardDeviation);
        }
    }
}