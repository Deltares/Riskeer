namespace Core.Common.Base
{
    /// <summary>
    /// Interface that describes the methods that need to be implemented on classes that are stored in the storage.
    /// </summary>
    public interface IStorable
    {
        /// <summary>
        /// Gets or sets the unique identifier for the storage of the class.
        /// </summary>
        long StorageId { get; }
    }
}
