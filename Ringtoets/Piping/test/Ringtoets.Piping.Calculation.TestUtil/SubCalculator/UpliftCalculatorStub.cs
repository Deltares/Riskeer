using System.Collections.Generic;
using Ringtoets.Piping.Calculation.SubCalculator;

namespace Ringtoets.Piping.Calculation.TestUtil.SubCalculator
{
    public class UpliftCalculatorStub : IUpliftCalculator {
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
        public void Calculate()
        {
            throw new System.NotImplementedException();
        }

        public List<string> Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}