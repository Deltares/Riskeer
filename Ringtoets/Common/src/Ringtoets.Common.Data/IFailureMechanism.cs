using System;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// Defines a failure mechanism.
    /// </summary>
    public interface IFailureMechanism
    {
        /// <summary>
        /// Gets the amount of contribution as a percentage (0-100) for the <see cref="IFailureMechanism"/>
        /// as part of the overall verdict.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="value"/> is not in interval [0-100].</exception>
        double Contribution { get; set; }

        /// <summary>
        /// The name of the <see cref="IFailureMechanism"/>.
        /// </summary>
        string Name { get; }
    }
}