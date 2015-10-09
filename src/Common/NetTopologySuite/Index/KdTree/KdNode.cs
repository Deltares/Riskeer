using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace GisSharpBlog.NetTopologySuite.Index.KdTree
{
    /// <summary>
    /// A node of a <see cref="NetTopologySuite.Index.KdTree.KdTree{T}"/>, which represents one or more points in the same location.
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <author>dskea</author>
    public class KdNode<T>
        where T : class
    {
        private readonly Coordinate _p;
        private readonly T _data;

        /// <summary>
        /// Creates a new KdNode.
        /// </summary>
        /// <param name="x">coordinate of point</param>
        /// <param name="y">coordinate of point</param>
        /// <param name="data">A data objects to associate with this node</param>
        public KdNode(double x, double y, T data)
        {
            _p = new Coordinate(x, y);
            Left = null;
            Right = null;
            Count = 1;
            _data = data;
        }

        /// <summary>
        /// Creates a new KdNode.
        /// </summary>
        /// <param name="p">The point location of new node</param>
        /// <param name="data">A data objects to associate with this node</param>
        public KdNode(ICoordinate p, T data)
        {
            _p = new Coordinate(p);
            Left = null;
            Right = null;
            Count = 1;
            _data = data;
        }

        /// <summary>
        /// Gets x-ordinate of this node
        /// </summary>
        public double X
        {
            get
            {
                return _p.X;
            }
        }

        /// <summary>
        /// Gets y-ordinate of this node
        /// </summary>
        public double Y
        {
            get
            {
                return _p.Y;
            }
        }

        /// <summary>
        /// Gets the location of this node
        /// </summary>
        public ICoordinate Coordinate
        {
            get
            {
                return _p;
            }
        }

        /// <summary>
        /// Gets the user data object associated with this node.
        /// </summary>
        public T Data
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// Gets or sets the left node of the tree
        /// </summary>
        public KdNode<T> Left { get; set; }

        /// <summary>
        /// Gets or sets the right node of the tree
        /// </summary>
        public KdNode<T> Right { get; set; }

        /// <summary>
        /// Gets the number of inserted points that are coincident at this location.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets whether more than one point with this value have been inserted (up to the tolerance)
        /// </summary>
        /// <returns></returns>
        public bool IsRepeated
        {
            get
            {
                return Count > 1;
            }
        }

        // Increments counts of points at this location
        internal void Increment()
        {
            Count = Count + 1;
        }
    }
}