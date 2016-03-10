using System.Collections.Generic;
using Ringtoets.Piping.Calculation.SubCalculator;

namespace Ringtoets.Piping.Calculation.TestUtil.SubCalculator
{
    public class SellmeijerCalculatorStub : ISellmeijerCalculator {
        public double BeddingAngle { get; set; }
        public double D70 { get; set; }
        public double D70Mean { get; set; }
        public double DAquifer { get; set; }
        public double DarcyPermeability { get; set; }
        public double DTotal { get; set; }
        public double GammaSubParticles { get; set; }
        public double Gravity { get; set; }
        public double HExit { get; set; }
        public double HRiver { get; set; }
        public double KinematicViscosityWater { get; set; }
        public double ModelFactorPiping { get; set; }
        public double Rc { get; set; }
        public double SeepageLength { get; set; }
        public double VolumetricWeightOfWater { get; set; }
        public double WhitesDragCoefficient { get; set; }
        public double Zp { get; private set; }
        public double FoSp { get; private set; }
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