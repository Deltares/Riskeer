using System.Collections.Generic;
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// Stub for the real uplift sub calculator of piping.
    /// </summary>
    public class UpliftCalculatorStub : IUpliftCalculator
    {
        public double EffectiveStress { get; set; }
        public double HExit { get; set; }
        public double HRiver { get; set; }
        public double ModelFactorUplift { get; set; }
        public double PhiExit { get; set; }
        public double PhiPolder { get; set; }
        public double RExit { get; set; }
        public double VolumetricWeightOfWater { get; set; }
        public double Zu { get; private set; }
        public double FoSu { get; private set; }

        public void Calculate() {}

        public List<string> Validate()
        {
            return new List<string>();
        }
    }
}