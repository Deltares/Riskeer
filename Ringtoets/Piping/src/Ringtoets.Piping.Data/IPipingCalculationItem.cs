using Core.Common.Base;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// A piping calculation related object
    /// </summary>
    public interface IPipingCalculationItem : IObservable
    {
        /// <summary>
        /// Gets or sets the name the user gave this this calculation object.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether this calculation item contains piping calculation outputs.
        /// </summary>
        bool HasOutput { get; }
    }
}