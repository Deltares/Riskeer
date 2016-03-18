using Deltares.WTIPiping;

namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    public class EffectiveThicknessCalculator : IEffectiveThicknessCalculator
    {
        private readonly Deltares.WTIPiping.EffectiveThicknessCalculator wrappedCalculator;

        public EffectiveThicknessCalculator()
        {
            wrappedCalculator = new Deltares.WTIPiping.EffectiveThicknessCalculator();
        }

        public double ExitPointXCoordinate
        {
            set
            {
                wrappedCalculator.ExitPointXCoordinate = value;
            }
        }

        public double PhreaticLevel
        {
            set
            {
                wrappedCalculator.PhreaticLevel = value;
            }
        }

        public double VolumicWeightOfWater
        {
            set
            {
                wrappedCalculator.VolumicWeightOfWater = value;
            }
        }

        public PipingProfile SoilProfile
        {
            set
            {
                wrappedCalculator.SoilProfile = value;
            }
        }

        public PipingSurfaceLine SurfaceLine
        {
            set
            {
                wrappedCalculator.SurfaceLine = value;
            }
        }

        public double EffectiveHeight
        {
            get
            {
                return wrappedCalculator.EffectiveHeight;
            }
        }

        public double EffectiveStress
        {
            get
            {
                return wrappedCalculator.EffectiveStress;
            }
        }

        public void Calculate()
        {
            wrappedCalculator.Calculate();
        }
    }
}