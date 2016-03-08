using System.Collections.Generic;

namespace Ringtoets.Piping.Calculation.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing a heave sub calculation.
    /// </summary>
    public interface IHeaveCalculator
    {
        /// <summary>
        /// Sets the DTotal property to use in the heave calculation.
        /// </summary>
        double DTotal { set; }

        /// <summary>
        /// Sets the HExit property to use in the heave calculation.
        /// </summary>
        double HExit { set; }

        /// <summar>
        /// Sets the Ich property to use in the heave calculation.
        /// </summar>
        double Ich { set; }

        /// <summary>
        /// Sets the PhiExit property to use in the heave calculation.
        /// </summary>
        double PhiExit { set; }

        /// <summary>
        /// Sets the PhiPolder property to use in the heave calculation.
        /// </summary>
        double PhiPolder { set; }

        /// <summary>
        /// Sets the RExit property to use in the heave calculation.
        /// </summary>
        double RExit { set; }

        /// <summary>
        /// Returns the Zh property to use in the heave calculation.
        /// </summary>
        double Zh { get; }

        /// <summary>
        /// Returns the FoSh property to use in the heave calculation.
        /// </summary>
        double FoSh { get; }

        /// <summary>
        /// Performs the heave calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Performs the heave validation.
        /// </summary>
        List<string> Validate();
    }
}