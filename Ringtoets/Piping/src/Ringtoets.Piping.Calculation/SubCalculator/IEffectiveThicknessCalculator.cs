using Deltares.WTIPiping;

namespace Ringtoets.Piping.Calculation.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing an effective thickness sub calculation.
    /// </summary>
    public interface IEffectiveThicknessCalculator
    {
        /// <summary>
        /// Sets the ExitPointXCoordinate property of the effective thickness calculation.
        /// </summary>
        double ExitPointXCoordinate { set; }

        /// <summary>
        /// Sets the PhreaticLevel property of the effective thickness calculation.
        /// </summary>
        double PhreaticLevel { set; }

        /// <summary>
        /// Sets the VolumicWeightOfWater property of the effective thickness calculation.
        /// </summary>
        double VolumicWeightOfWater { set; }

        /// <summary>
        /// Sets the SoilProfile property of the effective thickness calculation.
        /// </summary>
        PipingProfile SoilProfile { set; }

        /// <summary>
        /// Sets the SurfaceLine property of the effective thickness calculation.
        /// </summary>
        PipingSurfaceLine SurfaceLine { set; }

        /// <summary>
        /// Gets the EffectiveHeight property of the effective thickness calculation.
        /// </summary>
        double EffectiveHeight { get; }

        /// <summary>
        /// Gets the EffectiveStress property of the effective thickness calculation.
        /// </summary>
        double EffectiveStress { get; }

        /// <summary>
        /// Performs the effective thickness calculation.
        /// </summary>
        void Calculate();
    }
}