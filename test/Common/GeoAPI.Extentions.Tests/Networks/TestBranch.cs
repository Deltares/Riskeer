using System;
using DelftTools.Utils.Collections.Generic;
using GeoAPI.Extensions.Feature;
using GeoAPI.Extensions.Networks;
using GeoAPI.Geometries;

namespace GeoAPI.Extentions.Tests.Network
{
    public class TestBranch : IBranch
    {
        IEventedList<IBranchFeature> branchFeatures = new EventedList<IBranchFeature>();

        public TestBranch()
        {
        }

        public TestBranch(INode source, INode target)
        {
            Source = source;
            if (null != Source)
            {
                Source.OutgoingBranches.Add(this);
            }

            Target = target;
            if (null != Target)
            {
                Target.IncomingBranches.Add(this);
            }
        }
        public INode Source { get; set; }
        public INode Target { get; set; }

        public int CompareTo(IBranch other)
        {
            if (this == other)
            {
                return 0;
            }
            if (Network.Branches.IndexOf(this) > Network.Branches.IndexOf(other))
            {
                return 1;
            }
            return -1;
        }

        public int CompareTo(INetworkFeature other)
        {
            return Network.Branches.IndexOf(this).CompareTo(Network.Branches.IndexOf((IBranch) other));
        }

        public override string ToString()
        {
            return Source + " -> " + Target;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new TestBranch(Source, Target) { Geometry = (IGeometry)Geometry.Clone() };
        }

        // not relevant for this test
        public long Id { get; set; }
        public IGeometry Geometry { get; set; }
        public IFeatureAttributeCollection Attributes { get; set; }
        public string Name { get; set; }
        public INetwork Network { get; set; }
        public IFeature Owner
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Length { get; set; }
        public IEventedList<IBranchFeature> BranchFeatures
        {
            get { return branchFeatures; }
            set { branchFeatures = value; }
        }
    }
}