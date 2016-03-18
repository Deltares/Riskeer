namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Factory which creates the sub calculators from the piping kernel.
    /// </summary>
    public class PipingSubCalculatorFactory : IPipingSubCalculatorFactory
    {
        public IUpliftCalculator CreateUpliftCalculator()
        {
            return new UpliftCalculator();
        }

        public IHeaveCalculator CreateHeaveCalculator()
        {
            return new HeaveCalculator();
        }

        public ISellmeijerCalculator CreateSellmeijerCalculator()
        {
            return new SellmeijerCalculator();
        }

        public IEffectiveThicknessCalculator CreateEffectiveThicknessCalculator()
        {
            return new EffectiveThicknessCalculator();
        }

        public IPiezoHeadCalculator CreatePiezometricHeadAtExitCalculator()
        {
            return new PiezoHeadCalculator();
        }
    }
}