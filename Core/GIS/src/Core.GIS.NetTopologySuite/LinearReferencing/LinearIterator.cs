using System.Collections;
using System.Collections.Generic;
using Core.GIS.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.GIS.NetTopologySuite.LinearReferencing
{
    /// <summary>
    /// An iterator over the components and coordinates of a linear geometry
    /// (<see cref="LineString" />s and <see cref="MultiLineString" />s.
    /// </summary>
    public class LinearIterator : IEnumerator<LinearIterator.LinearElement>,
                                  IEnumerable<LinearIterator.LinearElement>
    {
        // Cached start values - for Reset() call
        private readonly int startComponentIndex = 0;
        private readonly int startVertexIndex = 0;

        private readonly IGeometry linear;
        private int numLines;

        /*
         * Invariant: currentLine <> null if the iterator is pointing at a valid coordinate
         */

        // Used for avoid the first call to Next() in MoveNext()
        private bool atStart;

        // Returned by Ienumerator.Current
        private LinearElement current = null;

        /// <summary>
        /// Creates an iterator initialized to the start of a linear <see cref="Geometry" />.
        /// </summary>
        /// <param name="linear">The linear geometry to iterate over.</param>
        public LinearIterator(IGeometry linear) : this(linear, 0, 0) {}

        /// <summary>
        /// Creates an iterator starting at a <see cref="LinearLocation" /> on a linear <see cref="Geometry" />.
        /// </summary>
        /// <param name="linear">The linear geometry to iterate over.</param>
        /// <param name="start">The location to start at.</param>
        public LinearIterator(IGeometry linear, LinearLocation start) :
            this(linear, start.ComponentIndex, SegmentEndVertexIndex(start)) {}

        /// <summary>
        /// Creates an iterator starting at
        /// a component and vertex in a linear <see cref="Geometry" />.
        /// </summary>
        /// <param name="linear">The linear geometry to iterate over.</param>
        /// <param name="componentIndex">The component to start at.</param>
        /// <param name="vertexIndex">The vertex to start at.</param>
        public LinearIterator(IGeometry linear, int componentIndex, int vertexIndex)
        {
            VertexIndex = 0;
            ComponentIndex = 0;
            startComponentIndex = componentIndex;
            startVertexIndex = vertexIndex;

            this.linear = linear;
            Reset();

            current = new LinearElement(this);
        }

        #region IEnumerator<LinearIterator.LinearElement> Members

        /// <summary>
        /// Gets the <see cref="LinearElement">element</see> in the collection 
        /// at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="LinearElement">element</see> in the collection 
        /// at the current position of the enumerator.
        /// </returns>
        public LinearElement Current
        {
            get
            {
                return current;
            }
        }

        #endregion

        #region IEnumerable<LinearIterator.LinearElement> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used 
        /// to iterate through the collection.
        /// </returns>
        public IEnumerator<LinearElement> GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator (of <see cref="LinearElement" />elements) 
        /// that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object 
        /// that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Evaluate if the iterator could step over.
        /// Does not perform the step at all.
        /// </summary>
        /// <returns></returns>
        /// <returns><c>true</c> if there are more vertices to scan.</returns>
        protected bool HasNext()
        {
            if (ComponentIndex >= numLines)
            {
                return false;
            }
            if ((ComponentIndex == numLines - 1) && (VertexIndex >= Line.NumPoints))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Jump to the next element of the iteration.
        /// </summary>
        protected void Next()
        {
            if (!HasNext())
            {
                return;
            }

            VertexIndex++;
            if (VertexIndex >= Line.NumPoints)
            {
                ComponentIndex++;
                LoadCurrentLine();
                VertexIndex = 0;
            }
        }

        /// <summary>
        /// Checks whether the iterator cursor is pointing to the
        /// endpoint of a linestring.
        /// </summary>
        private bool IsEndOfLine
        {
            get
            {
                if (ComponentIndex >= numLines)
                {
                    return false;
                }
                if (VertexIndex < Line.NumPoints - 1)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// The component index of the vertex the iterator is currently at.
        /// </summary>
        private int ComponentIndex { get; set; }

        /// <summary>
        /// The vertex index of the vertex the iterator is currently at.
        /// </summary>
        private int VertexIndex { get; set; }

        /// <summary>
        /// Gets the <see cref="LineString" /> component the iterator is current at.
        /// </summary>
        private ILineString Line { get; set; }

        /// <summary>
        /// Gets the first <see cref="Coordinate" /> of the current segment
        /// (the coordinate of the current vertex).
        /// </summary>
        private ICoordinate SegmentStart
        {
            get
            {
                return Line.GetCoordinateN(VertexIndex);
            }
        }

        /// <summary>
        /// Gets the second <see cref="Coordinate" /> of the current segment
        /// (the coordinate of the next vertex).
        /// If the iterator is at the end of a line, <c>null</c> is returned.
        /// </summary>
        private ICoordinate SegmentEnd
        {
            get
            {
                if (VertexIndex < Line.NumPoints - 1)
                {
                    return Line.GetCoordinateN(VertexIndex + 1);
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        private static int SegmentEndVertexIndex(LinearLocation loc)
        {
            if (loc.SegmentFraction > 0.0)
            {
                return loc.SegmentIndex + 1;
            }
            return loc.SegmentIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadCurrentLine()
        {
            if (ComponentIndex >= numLines)
            {
                Line = null;
                return;
            }
            Line = (ILineString) linear.GetGeometryN(ComponentIndex);
        }

        #region LinearElement

        /// <summary>
        /// A class that exposes <see cref="LinearIterator" /> elements.
        /// </summary>
        public class LinearElement
        {
            private readonly LinearIterator iterator = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:LinearElement"/> class.
            /// </summary>
            /// <param name="iterator">The iterator.</param>
            public LinearElement(LinearIterator iterator)
            {
                this.iterator = iterator;
            }

            /// <summary>
            /// The component index of the vertex the iterator is currently at.
            /// </summary>
            public int ComponentIndex
            {
                get
                {
                    return iterator.ComponentIndex;
                }
            }

            /// <summary>
            /// The vertex index of the vertex the iterator is currently at.
            /// </summary>
            public int VertexIndex
            {
                get
                {
                    return iterator.VertexIndex;
                }
            }

            /// <summary>
            /// Gets the <see cref="LineString" /> component the iterator is current at.
            /// </summary>
            public ILineString Line
            {
                get
                {
                    return iterator.Line;
                }
            }

            /// <summary>
            /// Checks whether the iterator cursor is pointing to the
            /// endpoint of a linestring.
            /// </summary>
            public bool IsEndOfLine
            {
                get
                {
                    return iterator.IsEndOfLine;
                }
            }

            /// <summary>
            /// Gets the first <see cref="Coordinate" /> of the current segment
            /// (the coordinate of the current vertex).
            /// </summary>
            public ICoordinate SegmentStart
            {
                get
                {
                    return iterator.SegmentStart;
                }
            }

            /// <summary>
            /// Gets the second <see cref="Coordinate" /> of the current segment
            /// (the coordinate of the next vertex).
            /// If the iterator is at the end of a line, <c>null</c> is returned.
            /// </summary>
            public ICoordinate SegmentEnd
            {
                get
                {
                    return iterator.SegmentEnd;
                }
            }
        }

        #endregion

        #region IEnumerator Members

        /// <summary>
        /// Tests whether there are any vertices left to iterator over.
        /// If <c>true</c>, then moves the iterator ahead to the next vertex and (possibly) linear component,
        /// so that <see cref="Current" /> exposes the elements.
        /// </summary>
        /// <returns><c>true</c> if there are more vertices to scan.</returns>
        public bool MoveNext()
        {
            // We must call HasNext() twice because, when in the Next() method
            // another line is loaded, it's necessary to re-ckeck with the new conditions.
            if (HasNext())
            {
                if (atStart)
                {
                    atStart = false;
                }
                else
                {
                    Next();
                }
            }
            return HasNext();
        }

        /// <summary>
        /// Gets the <see cref="LinearElement">element</see> in the collection 
        /// at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="LinearElement">element</see> in the collection 
        /// at the current position of the enumerator.
        /// </returns>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Sets the enumerator to its initial position, 
        /// which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created. 
        /// </exception>
        public void Reset()
        {
            numLines = linear.NumGeometries;
            ComponentIndex = startComponentIndex;
            VertexIndex = startVertexIndex;
            LoadCurrentLine();

            atStart = true;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispose"></param>
        protected void Dispose(bool dispose)
        {
            if (dispose)
            {
                // Dispose unmanaged resources
            }

            // Dispose managed resources
            current = null;
            Line = null;
        }

        #endregion
    }
}