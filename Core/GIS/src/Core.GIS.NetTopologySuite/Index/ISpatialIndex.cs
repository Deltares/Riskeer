using System.Collections;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.GIS.NetTopologySuite.Index
{
    /// <summary> 
    /// The basic insertion and query operations supported by classes
    /// implementing spatial index algorithms.
    /// A spatial index typically provides a primary filter for range rectangle queries. A
    /// secondary filter is required to test for exact intersection. Of course, this
    /// secondary filter may consist of other tests besides intersection, such as
    /// testing other kinds of spatial relationships.
    /// </summary>
    public interface ISpatialIndex
    {
        /// <summary>
        /// Adds a spatial item with an extent specified by the given <c>Envelope</c> to the index.
        /// </summary>
        void Insert(IEnvelope itemEnv, object item);

        /// <summary> 
        /// Queries the index for all items whose extents intersect the given search <c>Envelope</c> 
        /// Note that some kinds of indexes may also return objects which do not in fact
        /// intersect the query envelope.
        /// </summary>
        /// <param name="searchEnv">The envelope to query for.</param>
        /// <returns>A list of the items found by the query.</returns>
        IList Query(IEnvelope searchEnv);

        /// <summary>
        /// Queries the index for all items whose extents intersect the given search <see cref="Envelope" />,
        /// and applies an <see cref="IItemVisitor" /> to them.
        /// Note that some kinds of indexes may also return objects which do not in fact
        /// intersect the query envelope.
        /// </summary>
        /// <param name="searchEnv">The envelope to query for.</param>
        /// <param name="visitor">A visitor object to apply to the items found.</param>
        void Query(IEnvelope searchEnv, IItemVisitor visitor);

        /// <summary> 
        /// Removes a single item from the tree.
        /// </summary>
        /// <param name="itemEnv">The Envelope of the item to remove.</param>
        /// <param name="item">The item to remove.</param>
        /// <returns> <c>true</c> if the item was found.</returns>
        bool Remove(IEnvelope itemEnv, object item);
    }
}