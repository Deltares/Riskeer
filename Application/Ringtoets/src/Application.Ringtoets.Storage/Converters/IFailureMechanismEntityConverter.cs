using Application.Ringtoets.Storage.DbContext;

using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Interface that declares a converter that converts between <see cref="IFailureMechanism"/>
    /// and <see cref="FailureMechanismEntity"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of failure mechanism supported by the converter.</typeparam>
    public interface IFailureMechanismEntityConverter<T> : IEntityConverter<T, FailureMechanismEntity> where T : IFailureMechanism
    {
        /// <summary>
        /// Determines if the <see cref="FailureMechanismEntity"/> corresponds to the type
        /// of failure mechanism that this converter can deal with.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>True</c> if the converter can handle the entity, <c>false</c> if not.</returns>
        bool CorrespondsToFailureMechanismType(FailureMechanismEntity entity);
    }
}