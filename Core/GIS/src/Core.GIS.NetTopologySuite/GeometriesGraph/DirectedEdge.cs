using System;
using System.IO;
using Core.GIS.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.GIS.NetTopologySuite.GeometriesGraph
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectedEdge : EdgeEnd
    {
        /// <summary>
        /// 
        /// </summary>
        protected bool isForward;

        /// <summary> 
        /// The depth of each side (position) of this edge.
        /// The 0 element of the array is never used.
        /// </summary>
        private readonly int[] depth =
        {
            0,
            -999,
            -999
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="isForward"></param>
        public DirectedEdge(Edge edge, bool isForward) : base(edge)
        {
            Visited = false;
            InResult = false;
            this.isForward = isForward;
            if (isForward)
            {
                Init(edge.GetCoordinate(0), edge.GetCoordinate(1));
            }
            else
            {
                int n = edge.NumPoints - 1;
                Init(edge.GetCoordinate(n), edge.GetCoordinate(n - 1));
            }
            ComputeDirectedLabel();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool InResult { get; set; }

        /// <summary>
        /// 
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
        public EdgeRing EdgeRing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EdgeRing MinEdgeRing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DepthDelta
        {
            get
            {
                int depthDelta = edge.DepthDelta;
                if (!IsForward)
                {
                    depthDelta = -depthDelta;
                }
                return depthDelta;
            }
        }

        /// <summary>
        /// VisitedEdge get property returns <c>true</c> if bot Visited 
        /// and Sym.Visited are <c>true</c>.
        /// VisitedEdge set property marks both DirectedEdges attached to a given Edge.
        /// This is used for edges corresponding to lines, which will only
        /// appear oriented in a single direction in the result.
        /// </summary>
        public bool VisitedEdge
        {
            get
            {
                return Visited && Sym.Visited;
            }
            set
            {
                Visited = value;
                Sym.Visited = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsForward
        {
            get
            {
                return isForward;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DirectedEdge Sym { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DirectedEdge Next { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DirectedEdge NextMin { get; set; }

        /// <summary>
        /// This edge is a line edge if
        /// at least one of the labels is a line label
        /// any labels which are not line labels have all Locations = Exterior.
        /// </summary>
        public bool IsLineEdge
        {
            get
            {
                bool isLine = label.IsLine(0) || label.IsLine(1);
                bool isExteriorIfArea0 =
                    !label.IsArea(0) || label.AllPositionsEqual(0, Locations.Exterior);
                bool isExteriorIfArea1 =
                    !label.IsArea(1) || label.AllPositionsEqual(1, Locations.Exterior);
                return isLine && isExteriorIfArea0 && isExteriorIfArea1;
            }
        }

        /// <summary> 
        /// This is an interior Area edge if
        /// its label is an Area label for both Geometries
        /// and for each Geometry both sides are in the interior.
        /// </summary>
        /// <returns><c>true</c> if this is an interior Area edge.</returns>
        public bool IsInteriorAreaEdge
        {
            get
            {
                bool isInteriorAreaEdge = true;
                for (int i = 0; i < 2; i++)
                {
                    if (!(label.IsArea(i)
                          && label.GetLocation(i, Positions.Left) == Locations.Interior
                          && label.GetLocation(i, Positions.Right) == Locations.Interior))
                    {
                        isInteriorAreaEdge = false;
                    }
                }
                return isInteriorAreaEdge;
            }
        }

        /// <summary>
        /// Computes the factor for the change in depth when moving from one location to another.
        /// E.g. if crossing from the Interior to the Exterior the depth decreases, so the factor is -1.
        /// </summary>
        public static int DepthFactor(Locations currLocation, Locations nextLocation)
        {
            if (currLocation == Locations.Exterior && nextLocation == Locations.Interior)
            {
                return 1;
            }
            else if (currLocation == Locations.Interior && nextLocation == Locations.Exterior)
            {
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int GetDepth(Positions position)
        {
            return depth[(int) position];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depthVal"></param>
        public void SetDepth(Positions position, int depthVal)
        {
            if (depth[(int) position] != -999)
            {
                if (depth[(int) position] != depthVal)
                {
                    throw new TopologyException("assigned depths do not match", Coordinate);
                }
            }
            depth[(int) position] = depthVal;
        }

        /// <summary> 
        /// Set both edge depths.  
        /// One depth for a given side is provided.  
        /// The other is computed depending on the Location 
        /// transition and the depthDelta of the edge.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="position"></param>
        public void SetEdgeDepths(Positions position, int depth)
        {
            // get the depth transition delta from R to Curve for this directed Edge
            int depthDelta = Edge.DepthDelta;
            if (!isForward)
            {
                depthDelta = -depthDelta;
            }

            // if moving from Curve to R instead of R to Curve must change sign of delta
            int directionFactor = 1;
            if (position == Positions.Left)
            {
                directionFactor = -1;
            }

            Positions oppositePos = Position.Opposite(position);
            int delta = depthDelta*directionFactor;
            int oppositeDepth = depth + delta;
            SetDepth(position, depth);
            SetDepth(oppositePos, oppositeDepth);
        }

        /// <summary> 
        /// Set both edge depths.  One depth for a given side is provided.  The other is
        /// computed depending on the Location transition and the depthDelta of the edge.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="position"></param>
        [Obsolete("Use SetEdgeDepths instead")]
        public void OLDSetEdgeDepths(Positions position, int depth)
        {
            int depthDelta = Edge.DepthDelta;
            Locations loc = label.GetLocation(0, position);
            Positions oppositePos = Position.Opposite(position);
            Locations oppositeLoc = label.GetLocation(0, oppositePos);
            int delta = Math.Abs(depthDelta)*DepthFactor(loc, oppositeLoc);
            int oppositeDepth = depth + delta;
            SetDepth(position, depth);
            SetDepth(oppositePos, oppositeDepth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outstream"></param>
        public void WriteEdge(StreamWriter outstream)
        {
            Write(outstream);
            outstream.Write(" ");
            if (isForward)
            {
                edge.Write(outstream);
            }
            else
            {
                edge.WriteReverse(outstream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outstream"></param>
        public override void Write(StreamWriter outstream)
        {
            base.Write(outstream);
            outstream.Write(" " + depth[(int) Positions.Left] + "/" + depth[(int) Positions.Right]);
            outstream.Write(" (" + DepthDelta + ")");
            if (InResult)
            {
                outstream.Write(" inResult");
            }
        }

        /// <summary>
        /// Compute the label in the appropriate orientation for this DirEdge.
        /// </summary>
        private void ComputeDirectedLabel()
        {
            label = new Label(edge.Label);
            if (!isForward)
            {
                label.Flip();
            }
        }
    }
}