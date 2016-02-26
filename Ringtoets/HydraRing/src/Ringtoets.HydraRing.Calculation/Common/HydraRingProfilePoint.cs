namespace Ringtoets.HydraRing.Calculation.Common
{
    /// <summary>
    /// Container for Hydra-Ring profile point related data.
    /// </summary>
    public class HydraRingProfilePoint
    {
        private readonly double x;
        private readonly double z;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingProfilePoint"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public HydraRingProfilePoint(double x, double z)
        {
            this.x = x;
            this.z = z;
        }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        public double X
        {
            get
            {
                return x;
            }
        }

        /// <summary>
        /// Gets the z coordinate.
        /// </summary>
        public double Z
        {
            get
            {
                return z;
            }
        }

        /// <summary>
        /// Gets the roughness.
        /// </summary>
        public virtual double Roughness
        {
            get
            {
                return 1.0;
            }
        }
    }
}