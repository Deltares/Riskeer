using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DelftTools.Utils.Collections.Generic;
using GeoAPI.Extensions.Feature;
using GeoAPI.Extensions.Networks;
using GeoAPI.Geometries;

namespace SharpMapTestUtils.TestClasses
{
    /// <summary>
    /// Minimal INetwork implementation
    /// </summary>
    public class TestNetwork:INetwork
    {
        private IEventedList<INodeFeature> nodeFeatures;
        private IEventedList<IBranch> branches;
        private IEventedList<INode> nodes;
        private IEventedList<IBranchFeature> branchFeatures;

        public TestNetwork()
        {
            nodes = new EventedList<INode>();
            branches = new EventedList<IBranch>();
            nodeFeatures= new EventedList<INodeFeature>();
            branchFeatures= new EventedList<IBranchFeature>();
        }
        public long Id
        {
            get;
            set;
        }
     
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public IGeometry Geometry
        {
            get; set;
        }

        public IFeatureAttributeCollection Attributes
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Name
        {
            get;
            set ;
        }

        public bool ContainsVertex(INode vertex)
        {
            throw new NotImplementedException();
        }

        public bool IsVerticesEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public int VertexCount
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<INode> Vertices
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDirected
        {
            get { throw new NotImplementedException(); }
        }

        public bool AllowParallelEdges
        {
            get { throw new NotImplementedException(); }
        }

        public bool ContainsEdge(IBranch edge)
        {
            throw new NotImplementedException();
        }

        public bool IsEdgesEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public int EdgeCount
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IBranch> Edges
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IBranch> AdjacentEdges(INode v)
        {
            throw new NotImplementedException();
        }

        public int AdjacentDegree(INode v)
        {
            throw new NotImplementedException();
        }

        public bool IsAdjacentEdgesEmpty(INode v)
        {
            throw new NotImplementedException();
        }

        public IBranch AdjacentEdge(INode v, int index)
        {
            throw new NotImplementedException();
        }

        public bool ContainsEdge(INode source, INode target)
        {
            throw new NotImplementedException();
        }

        public IEventedList<IBranch> Branches
        {
            get { return branches; }
            set { branches = value; }
        }

        public IEventedList<INode> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        public IEventedList<IBranchFeature> BranchFeatures
        {
            get { return branchFeatures; }
        }

        public IEventedList<INodeFeature> NodeFeatures
        {
            get
            {
                return nodeFeatures;
            }
        }
    }
}