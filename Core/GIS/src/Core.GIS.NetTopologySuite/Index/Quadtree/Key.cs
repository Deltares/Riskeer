using System;
using Core.GIS.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.GIS.NetTopologySuite.Index.Quadtree
{
    /// <summary> 
    /// A Key is a unique identifier for a node in a quadtree.
    /// It contains a lower-left point and a level number. The level number
    /// is the power of two for the size of the node envelope.
    /// </summary>
    public class Key
    {
        // the fields which make up the key
        private readonly ICoordinate pt = new Coordinate();

        // auxiliary data which is derived from the key for use in computation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemEnv"></param>
        public Key(IEnvelope itemEnv)
        {
            Envelope = null;
            Level = 0;
            ComputeKey(itemEnv);
        }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate Point
        {
            get
            {
                return pt;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnvelope Envelope { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate Centre
        {
            get
            {
                return new Coordinate((Envelope.MinX + Envelope.MaxX)/2, (Envelope.MinY + Envelope.MaxY)/2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static int ComputeQuadLevel(IEnvelope env)
        {
            double dx = env.Width;
            double dy = env.Height;
            double dMax = dx > dy ? dx : dy;
            int level = DoubleBits.GetExponent(dMax) + 1;
            return level;
        }

        /// <summary>
        /// Return a square envelope containing the argument envelope,
        /// whose extent is a power of two and which is based at a power of 2.
        /// </summary>
        /// <param name="itemEnv"></param>
        public void ComputeKey(IEnvelope itemEnv)
        {
            Level = ComputeQuadLevel(itemEnv);
            Envelope = new Envelope();
            ComputeKey(Level, itemEnv);
            // MD - would be nice to have a non-iterative form of this algorithm
            while (!Envelope.Contains(itemEnv))
            {
                Level += 1;
                ComputeKey(Level, itemEnv);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="itemEnv"></param>
        private void ComputeKey(int level, IEnvelope itemEnv)
        {
            double quadSize = DoubleBits.PowerOf2(level);
            pt.X = Math.Floor(itemEnv.MinX/quadSize)*quadSize;
            pt.Y = Math.Floor(itemEnv.MinY/quadSize)*quadSize;
            Envelope.Init(pt.X, pt.X + quadSize, pt.Y, pt.Y + quadSize);
        }
    }
}