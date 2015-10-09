using System;
using GisSharpBlog.NetTopologySuite.Index.Quadtree;

namespace GisSharpBlog.NetTopologySuite.Index.Bintree
{
    /// <summary>
    /// A Key is a unique identifier for a node in a tree.
    /// It contains a lower-left point and a level number. The level number
    /// is the power of two for the size of the node envelope.
    /// </summary>
    public class Key
    {
        // the fields which make up the key

        // auxiliary data which is derived from the key for use in computation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public Key(Interval interval)
        {
            Level = 0;
            Point = 0.0;
            ComputeKey(interval);
        }

        /// <summary>
        /// 
        /// </summary>
        public double Point { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Interval Interval { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static int ComputeLevel(Interval interval)
        {
            double dx = interval.Width;
            int level = DoubleBits.GetExponent(dx) + 1;
            return level;
        }

        /// <summary>
        /// Return a square envelope containing the argument envelope,
        /// whose extent is a power of two and which is based at a power of 2.
        /// </summary>
        /// <param name="itemInterval"></param>
        public void ComputeKey(Interval itemInterval)
        {
            Level = ComputeLevel(itemInterval);
            Interval = new Interval();
            ComputeInterval(Level, itemInterval);
            // MD - would be nice to have a non-iterative form of this algorithm
            while (!Interval.Contains(itemInterval))
            {
                Level += 1;
                ComputeInterval(Level, itemInterval);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="itemInterval"></param>
        private void ComputeInterval(int level, Interval itemInterval)
        {
            double size = DoubleBits.PowerOf2(level);
            Point = Math.Floor(itemInterval.Min/size)*size;
            Interval.Init(Point, Point + size);
        }
    }
}