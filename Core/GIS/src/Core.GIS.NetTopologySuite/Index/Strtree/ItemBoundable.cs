namespace Core.GIS.NetTopologySuite.Index.Strtree
{
    /// <summary>
    /// Boundable wrapper for a non-Boundable spatial object. Used internally by
    /// AbstractSTRtree.
    /// </summary>
    public class ItemBoundable : IBoundable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="item"></param>
        public ItemBoundable(object bounds, object item)
        {
            Bounds = bounds;
            Item = item;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Item { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object Bounds { get; private set; }
    }
}