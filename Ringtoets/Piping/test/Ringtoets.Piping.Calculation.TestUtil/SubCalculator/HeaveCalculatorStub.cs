using System.Collections.Generic;
using Ringtoets.Piping.Calculation.SubCalculator;

namespace Ringtoets.Piping.Calculation.TestUtil.SubCalculator
{
    public class HeaveCalculatorStub : IHeaveCalculator {
        public double DTotal { get; set; }
        public double HExit { get; set; }
        public double Ich { get; set; }
        public double PhiExit { get; set; }
        public double PhiPolder { get; set; }
        public double RExit { get; set; }
        public double Zh { get; private set; }
        public double FoSh { get; private set; }
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