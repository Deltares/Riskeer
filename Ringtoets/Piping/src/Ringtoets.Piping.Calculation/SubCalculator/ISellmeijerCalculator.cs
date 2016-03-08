using System.Collections.Generic;

namespace Ringtoets.Piping.Calculation.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing a sellmeijer sub calculation.
    /// </summary>
    public interface ISellmeijerCalculator
    {
        /// <summary>
        /// Sets the BeddingAngle property to use in the Sellmeijer calculation
        /// </summary>
        double BeddingAngle { set; }

        /// <summary>
        /// Sets the D70 to use in the Sellmeijer calculation
        /// </summary>
        double D70 { set; }

        /// <summary>
        /// Sets the D70Mean property to use in the Sellmeijer calculation
        /// </summary>
        double D70Mean { set; }

        /// <summary>
        /// Sets the DAquifer property to use in the Sellmeijer calculation
        /// </summary>
        double DAquifer { set; }

        /// <summary>
        /// Sets the DarcyPermeability property to use in the Sellmeijer calculation
        /// </summary>
        double DarcyPermeability { set; }

        /// <summary>
        /// Sets the DTotal property to use in the Sellmeijer calculation
        /// </summary>
        double DTotal { set; }

        /// <summary>
        /// Sets the GammaSubParticles property to use in the Sellmeijer calculation
        /// </summary>
        double GammaSubParticles { set; }

        /// <summary>
        /// Sets the Gravity property to use in the Sellmeijer calculation
        /// </summary>
        double Gravity { set; }

        /// <summary>
        /// Sets the HExit property to use in the Sellmeijer calculation
        /// </summary>
        double HExit { set; }

        /// <summary>
        /// Sets the HRiver property to use in the Sellmeijer calculation
        /// </summary>
        double HRiver { set; }

        /// <summary>
        /// Sets the KinematicViscosityWater property to use in the Sellmeijer calculation
        /// </summary>
        double KinematicViscosityWater { set; }

        /// <summary>
        /// Sets the ModelFactorPiping property to use in the Sellmeijer calculation
        /// </summary>
        double ModelFactorPiping { set; }

        /// <summary>
        /// Sets the Rc property to use in the Sellmeijer calculation
        /// </summary>
        double Rc { set; }

        /// <summary>
        /// Sets the SeepageLength property to use in the Sellmeijer calculation
        /// </summary>
        double SeepageLength { set; }

        /// <summary>
        /// Sets the VolumetricWeightOfWater property to use in the Sellmeijer calculation
        /// </summary>
        double VolumetricWeightOfWater { set; }

        /// <summary>
        /// Sets the WhitesDragCoefficient property to use in the Sellmeijer calculation
        /// </summary>
        double WhitesDragCoefficient { set; }

        /// <summary>
        /// Gets the Zp property of the Sellmeijer calculation.
        /// </summary>
        double Zp { get; }

        /// <summary>
        /// Gets the FoSp property of the Sellmeijer calculation.
        /// </summary>
        double FoSp { get; }

        /// <summary>
        /// Performs the Sellmeijer calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Performs the Sellmeijer validation.
        /// </summary>
        List<string> Validate();
    }
}