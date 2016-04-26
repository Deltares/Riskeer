using Core.Common.Base.Storage;

namespace Application.Ringtoets.Storage.Update
{
    public static class IStorableExtensions
    {
        public static bool IsNew(this IStorable storable)
        {
            return storable.StorageId <= 0;
        } 
    }
}