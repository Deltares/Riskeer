namespace DelftTools.Utils
{
    // TODO: refactor it to extension method, taking arguments allowing to skip certain properties
    public interface ICopyFrom
    {
        /// <summary>
        /// copy data from source in to object
        /// </summary>
        /// <param name="source"></param>
        void CopyFrom(object source);
    }
}
