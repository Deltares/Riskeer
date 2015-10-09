using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using OSGeo.OSR;

namespace SharpMap.Extensions.CoordinateSystems
{
    public class OgrCoordinateTransformation : CoordinateTransformation, ICoordinateTransformation
    {
        public OgrCoordinateTransformation(OgrCoordinateSystem src, OgrCoordinateSystem dst)
            : base(src, dst)
        {
            MathTransform = new OgrCoordinateSystemTransform(this);
            SourceCS = src;
            TargetCS = dst;
        }

        public string AreaOfUse { get; private set; }

        public string Authority { get; private set; }

        public long AuthorityCode { get; private set; }

        public IMathTransform MathTransform { get; private set; }

        public string Name { get; private set; }

        public string Remarks { get; private set; }

        public ICoordinateSystem SourceCS { get; private set; }

        public ICoordinateSystem TargetCS { get; private set; }

        public TransformType TransformType { get; private set; }

        public class OgrCoordinateSystemTransform : IMathTransform
        {
            private readonly OgrCoordinateTransformation transformation;
            private OgrCoordinateTransformation transformationInverse;

            public OgrCoordinateSystemTransform(OgrCoordinateTransformation transformation)
            {
                this.transformation = transformation;

                DimSource = 3;
                DimTarget = 3;
            }

            public int DimSource { get; private set; }

            public int DimTarget { get; private set; }

            public string WKT { get; private set; }

            public string XML { get; private set; }

            public bool Identity()
            {
                throw new NotImplementedException();
            }

            public double[,] Derivative(double[] point)
            {
                throw new NotImplementedException();
            }

            public List<double> GetCodomainConvexHull(List<double> points)
            {
                throw new NotImplementedException();
            }

            public DomainFlags GetDomainFlags(List<double> points)
            {
                throw new NotImplementedException();
            }

            public IMathTransform Inverse()
            {
                if (transformationInverse == null)
                {
                    transformationInverse = new OgrCoordinateTransformation((OgrCoordinateSystem) transformation.TargetCS, (OgrCoordinateSystem) transformation.SourceCS);
                }

                return transformationInverse.MathTransform;
            }

            public double[] Transform(double[] point)
            {
                var results = new double[3];

                transformation.TransformPoint(results, point[0], point[1], point.Length == 2 ? 0 : point[2]);

                return results;
            }

            public List<double[]> TransformList(List<double[]> points)
            {
                // assume (x, y) tuples, todo: refactor NTS/SharpMap to support z
                var result = new List<double[]>(points.Count);

                var x = new double[points.Count];
                var y = new double[points.Count];
                var z = new double[points.Count];

                for (var i = 0; i < points.Count; i++)
                {
                    x[i] = points[i][0];
                    y[i] = points[i][1];
                    z[i] = 0;
                }

                transformation.TransformPoints(points.Count, x, y, z);

                for (var i = 0; i < points.Count; i++)
                {
                    result.Add(new[]
                    {
                        x[i],
                        y[i]
                    });
                }

                return result;
            }

            public void Invert()
            {
                throw new NotImplementedException();
            }
        }
    }
}