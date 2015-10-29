using System;
using System.Collections.Generic;
using System.Linq;
using DelftTools.Utils.Collections.Generic;
using GeoAPI.Extensions.Feature;
using GeoAPI.Extensions.Networks;
using GeoAPI.Geometries;
using DelftTools.Utils.Collections;

namespace GeoAPI.Extentions.Tests.Network
{
    public class TestNetwork : INetwork
    {
        private IEventedList<INode> vertices = new EventedList<INode>();
        private IEventedList<IBranch> edges = new EventedList<IBranch>();

        public IEventedList<INode> Nodes
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public IEventedList<IBranchFeature> BranchFeatures
        {
            get { throw new NotImplementedException(); }
        }

        public IEventedList<INodeFeature> NodeFeatures
        {
            get { throw new NotImplementedException(); }
        }

        public IEventedList<IBranch> Branches
        {
            get { return edges; }
            set { edges = value; }
        }

        public TestNetwork()
        {
            Branches.CollectionChanging += Branches_CollectionChanging;
        }

        public bool IsVerticesEmpty
        {
            get { return (0 == vertices.Count); }
        }

        public int VertexCount
        {
            get { return vertices.Count; }
        }

        public IEnumerable<INode> Vertices
        {
            get { return vertices; }
        }

        public bool ContainsVertex(INode node)
        {
            return vertices.Contains(node);
        }

        public bool IsDirected { get { return true; } }

        public bool AllowParallelEdges { get { return true; } }

        public bool IsEdgesEmpty
        {
            get { return (0 == edges.Count); }
        }

        public int EdgeCount
        {
            get { return edges.Count; }
        }

        public IEnumerable<IBranch> Edges
        {
            get { return edges; }
        }

        public bool ContainsEdge(IBranch edge)
        {
            return edges.Contains(edge);
        }

        public IEnumerable<IBranch> AdjacentEdges(INode v)
        {
            return v.IncomingBranches.Concat(v.OutgoingBranches);
        }

        public int AdjacentDegree(INode v)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAdjacentEdgesEmpty(INode v)
        {
            throw new System.NotImplementedException();
        }

        public IBranch AdjacentEdge(INode v, int index)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsEdge(INode source, INode target)
        {
            throw new System.NotImplementedException();
        }

        // not relevant for this test
        public long Id { get; set; }
        public IGeometry Geometry { get; set; }
        public IFeatureAttributeCollection Attributes { get; set; }
        public string Name { get; set; }
        public object Clone()
        {
            throw new NotImplementedException();
        }

        private void Branches_CollectionChanging(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (sender == Branches)
                    {
                        if (e.Item is TestBranch)
                        {
                            var branch = (TestBranch)e.Item;
                            if (branch.Network != this)
                            {
                                branch.Network = this;
                            }
                        }
                    }
                    break;
            }
        }

    }
}