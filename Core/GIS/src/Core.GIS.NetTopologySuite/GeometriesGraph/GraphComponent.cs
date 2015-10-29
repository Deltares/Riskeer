using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Utilities;

namespace Core.GIS.NetTopologySuite.GeometriesGraph
{
    /// <summary>
    /// A GraphComponent is the parent class for the objects'
    /// that form a graph.  Each GraphComponent can carry a
    /// Label.
    /// </summary>
    public abstract class GraphComponent
    {
        /// <summary>
        /// 
        /// </summary>
        protected Label label;

        // isInResult indicates if this component has already been included in the result

        private bool isCovered = false;

        /// <summary>
        /// 
        /// </summary>
        public GraphComponent()
        {
            Visited = false;
            IsCoveredSet = false;
            InResult = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        public GraphComponent(Label label)
        {
            Visited = false;
            IsCoveredSet = false;
            InResult = false;
            this.label = label;
        }

        /// <summary>
        /// 
        /// </summary>
        public Label Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool InResult { get; set; }

        /// <summary> 
        /// IsInResult indicates if this component has already been included in the result.
        /// </summary>
        public bool IsInResult
        {
            get
            {
                return InResult;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Covered
        {
            get
            {
                return isCovered;
            }
            set
            {
                isCovered = value;
                IsCoveredSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCovered
        {
            get
            {
                return Covered;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCoveredSet { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Visited { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsVisited
        {
            get
            {
                return Visited;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// A coordinate in this component (or null, if there are none).
        /// </returns>
        public abstract ICoordinate Coordinate { get; }

        /// <summary>
        /// An isolated component is one that does not intersect or touch any other
        /// component.  This is the case if the label has valid locations for
        /// only a single Geometry.
        /// </summary>
        /// <returns><c>true</c> if this component is isolated.</returns>
        public abstract bool IsIsolated { get; }

        /// <summary>
        /// Compute the contribution to an IM for this component.
        /// </summary>
        public abstract void ComputeIM(IntersectionMatrix im);

        /// <summary>
        /// Update the IM with the contribution for this component.
        /// A component only contributes if it has a labelling for both parent geometries.
        /// </summary>
        /// <param name="im"></param>
        public void UpdateIM(IntersectionMatrix im)
        {
            Assert.IsTrue(label.GeometryCount >= 2, "found partial label");
            ComputeIM(im);
        }
    }
}