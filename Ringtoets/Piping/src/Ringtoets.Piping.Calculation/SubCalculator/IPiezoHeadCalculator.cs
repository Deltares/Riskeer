namespace Ringtoets.Piping.Calculation.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing an piezometric head at exit sub calculation.
    /// </summary>
    public interface IPiezoHeadCalculator
    {
        /// <summary>
        /// Sets the piezometric head at polder parameter.
        /// </summary>
        double PhiPolder { set; }

        /// <summary>
        /// Sets the damping factor at exit parameter.
        /// </summary>
        double RExit { set; }

        /// <summary>
        /// Sets the assessment level parameter.
        /// </summary>
        double HRiver { set; }

        /// <summary>
        /// Gets the piezometric head exit result.
        /// </summary>
        double PhiExit { get; }

        /// <summary>
        /// Performs the piezomteric head at exit calculation.
        /// </summary>
        void Calculate();
    }
}