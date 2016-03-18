using Deltares.WTIPiping;
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator
{
    public class EffectiveThicknessCalculatorStub : IEffectiveThicknessCalculator {
        public double ExitPointXCoordinate { get; set; }
        public double PhreaticLevel { get; set; }
        public double VolumicWeightOfWater { get; set; }
        public PipingProfile SoilProfile { get; set; }
        public PipingSurfaceLine SurfaceLine { get; set; }
        public double EffectiveHeight { get; private set; }
        public double EffectiveStress { get; private set; }
        public void Calculate()
        {
        }
    }
}