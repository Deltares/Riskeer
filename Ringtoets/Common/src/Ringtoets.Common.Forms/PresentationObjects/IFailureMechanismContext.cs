using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Interface for a failure mechanism context which wraps an implementation of the 
    /// <see cref="IFailureMechanism"/> interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFailureMechanismContext<out T> where T : IFailureMechanism
    {
        /// <summary>
        /// Gets the wrapped <see cref="IFailureMechanism"/> in this presentation object.
        /// </summary>
        T WrappedData { get; }
    }
}