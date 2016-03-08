using System.Collections.Generic;

namespace Ringtoets.Piping.Calculation.SubCalculator
{
    public interface IUpliftCalculator
    {
        /// <summary>
        /// Sets the EffectiveStress property to use in the uplift calculation
        /// </summary>
        double EffectiveStress { set; }

        /// <summary>
        /// Sets the HExit property to use in the uplift calculation
        /// </summary>
        double HExit { set; }

        /// <summary>
        /// Sets the HRiver property to use in the uplift calculation
        /// </summary>
        double HRiver { set; }

        /// <summary>
        /// Sets the ModelFactorUplift property to use in the uplift calculation
        /// </summary>
        double ModelFactorUplift { set; }

        /// <summary>
        /// Sets the PhiExit property to use in the uplift calculation
        /// </summary>
        double PhiExit { set; }

        /// <summary>
        /// Sets the PhiPolder property to use in the uplift calculation
        /// </summary>
        double PhiPolder { set; }

        /// <summary>
        /// Sets the RExit property to use in the uplift calculation
        /// </summary>
        double RExit { set; }

        /// <summary>
        /// Sets the VolumetricWeightOfWater property to use in the uplift calculation
        /// </summary>
        double VolumetricWeightOfWater { set; }

        /// <summary>
        /// Gets the Zu property of the uplift calculation
        /// </summary>
        double Zu { get; }

        /// <summary>
        /// Gets the FoSu property of the uplift calculation
        /// </summary>
        double FoSu { get; }

        /// <summary>
        /// Performs the uplift validation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Performs the uplift validation.
        /// </summary>
        List<string> Validate();
    }
}