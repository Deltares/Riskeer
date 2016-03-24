using Deltares.WTIPiping;
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// Stub for the real effective thickness sub calculator of piping.
    /// </summary>
    public class EffectiveThicknessCalculatorStub : IEffectiveThicknessCalculator
    {
        public double ExitPointXCoordinate { get; set; }
        public double PhreaticLevel { get; set; }
        public double VolumicWeightOfWater { get; set; }
        public PipingProfile SoilProfile { get; set; }
        public PipingSurfaceLine SurfaceLine { get; set; }
        public double EffectiveHeight
        {
            get
            {
                return 0.1;
            }
        }

        public double EffectiveStress { get; private set; }

        public void Calculate() {}
    }
}