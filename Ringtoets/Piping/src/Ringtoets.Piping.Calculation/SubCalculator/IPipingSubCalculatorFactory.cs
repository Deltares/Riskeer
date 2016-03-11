namespace Ringtoets.Piping.Calculation.SubCalculator
{
    /// <summary>
    /// Factory responsible for creating the sub calculators required for a piping calculation.
    /// </summary>
    public interface IPipingSubCalculatorFactory
    {
        /// <summary>
        /// Creates the uplift calculator.
        /// </summary>
        /// <returns>A new <see cref="IUpliftCalculator"/>.</returns>
        IUpliftCalculator CreateUpliftCalculator();

        /// <summary>
        /// Creates the heave calculator.
        /// </summary>
        /// <returns>A new <see cref="IHeaveCalculator"/>.</returns>
        IHeaveCalculator CreateHeaveCalculator();

        /// <summary>
        /// Creates the Sellmeijer calculator.
        /// </summary>
        /// <returns>A new <see cref="ISellmeijerCalculator"/>.</returns>
        ISellmeijerCalculator CreateSellmeijerCalculator();

        /// <summary>
        /// Creates the effective thickness calculator.
        /// </summary>
        /// <returns>A new <see cref="IEffectiveThicknessCalculator"/>.</returns>
        IEffectiveThicknessCalculator CreateEffectiveThicknessCalculator();

        /// <summary>
        /// Creates the piezometric head at exit calculator.
        /// </summary>
        /// <returns>A new <see cref="IPiezoHeadCalculator"/>.</returns>
        IPiezoHeadCalculator CreatePiezometricHeadAtExitCalculator();
    }
}