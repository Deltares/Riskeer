using System;
using System.Collections.Generic;
using DelftTools.Utils.Collections.Generic;
using GeoAPI.Extensions.Feature;
using GeoAPI.Extensions.Networks;
using GeoAPI.Geometries;

namespace GeoAPI.Extentions.Tests.Network
{
    /// <summary>
    /// This file provides some minimalistic implementations for INode, IBranch and INetwork just
    /// to test the 
    /// </summary>

    public class TestNode : INode
    {
        public string Name { get; set; }

        IList<IBranch> incomingBranches = new List<IBranch>();
        IList<IBranch> outgoingBranches = new List<IBranch>();

        public IList<IBranch> IncomingBranches
        {
            get { return incomingBranches; }
            set { incomingBranches = value; }
        }

        public IList<IBranch> OutgoingBranches
        {
            get { return outgoingBranches; }
            set { outgoingBranches = value; }
        }

        public IEventedList<INodeFeature> NodeFeatures
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int CompareTo(INetworkFeature other)
        {
            return Network.Nodes.IndexOf(this).CompareTo(Network.Nodes.IndexOf((INode) other));
        }

        public override string ToString()
        {
            return "Node " + Name;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        // not relevant for this test
        public INetwork Network { get; set; }
        public IFeature Owner
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public long Id { get; set; }
        public IGeometry Geometry { get; set; }
        public IFeatureAttributeCollection Attributes { get; set; }
    }
}