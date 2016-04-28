using Core.Common.Base.Storage;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// This class contains methods which can be performed on the IStorable interface.
    /// </summary>
    public static class IStorableExtensions
    {
        /// <summary>
        /// Checks whether the <see cref="IStorable"/> is concidered new from the database's perspective.
        /// </summary>
        /// <param name="storable">The <see cref="IStorable"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref name="storable"/> is concidered new, <c>false</c> otherwise.</returns>
        public static bool IsNew(this IStorable storable)
        {
            return storable.StorageId <= 0;
        } 
    }
}