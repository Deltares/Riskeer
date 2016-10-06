using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Interface for a row which present a calculation and an independent name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IScenarioRow<out T> where T : ICalculation
    {
        /// <summary>
        /// Gets the name of the <see cref="FailureMechanismSection"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the normative calculation for the section.
        /// </summary>
        T Calculation { get; }
    }
}