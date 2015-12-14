using Core.Common.Base;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// A piping calculation related object.
    /// </summary>
    public interface IPipingCalculationItem : IObservable
    {
        /// <summary>
        /// Gets the name of this calculation object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this calculation item contains piping calculation outputs.
        /// </summary>
        bool HasOutput { get; }
    }
}