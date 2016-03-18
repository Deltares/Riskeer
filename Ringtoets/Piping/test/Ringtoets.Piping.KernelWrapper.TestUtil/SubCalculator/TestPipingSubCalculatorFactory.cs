using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// This class allows for retrieving the created sub calculators, so that
    /// tests can be performed upon them.
    /// </summary>
    public class TestPipingSubCalculatorFactory : IPipingSubCalculatorFactory
    {
        public EffectiveThicknessCalculatorStub LastCreatedEffectiveThicknessCalculator { get; private set; }
        public UpliftCalculatorStub LastCreatedUpliftCalculator { get; private set; }
        public SellmeijerCalculatorStub LastCreatedSellmeijerCalculator { get; private set; }
        public HeaveCalculatorStub LastCreatedHeaveCalculator { get; private set; }
        public PiezoHeadCalculatorStub LastCreatedPiezometricHeadAtExitCalculator { get; private set; }

        public IUpliftCalculator CreateUpliftCalculator()
        {
            LastCreatedUpliftCalculator = new UpliftCalculatorStub();
            return LastCreatedUpliftCalculator;
        }

        public IHeaveCalculator CreateHeaveCalculator()
        {
            LastCreatedHeaveCalculator = new HeaveCalculatorStub();
            return LastCreatedHeaveCalculator;
        }

        public ISellmeijerCalculator CreateSellmeijerCalculator()
        {
            LastCreatedSellmeijerCalculator = new SellmeijerCalculatorStub();
            return LastCreatedSellmeijerCalculator;
        }

        public IEffectiveThicknessCalculator CreateEffectiveThicknessCalculator()
        {
            LastCreatedEffectiveThicknessCalculator = new EffectiveThicknessCalculatorStub();
            return LastCreatedEffectiveThicknessCalculator;
        }

        public IPiezoHeadCalculator CreatePiezometricHeadAtExitCalculator()
        {
            LastCreatedPiezometricHeadAtExitCalculator = new PiezoHeadCalculatorStub();
            return LastCreatedPiezometricHeadAtExitCalculator;
        }
    }
}