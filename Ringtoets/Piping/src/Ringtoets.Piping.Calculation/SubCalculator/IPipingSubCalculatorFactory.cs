namespace Ringtoets.Piping.Calculation.SubCalculator
{
    public interface IPipingSubCalculatorFactory
    {
        IUpliftCalculator CreateUpliftCalculator();
        IHeaveCalculator CreateHeaveCalculator();
        ISellmeijerCalculator CreateSellmeijerCalculator();
    }
}