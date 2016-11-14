using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Simple dike profile that can be used for testing.
    /// </summary>
    public class TestDikeProfile : DikeProfile
    {
        /// <summary>
        /// Initializes default <see cref="DikeProfile"/> at the world origin.
        /// </summary>
        public TestDikeProfile() : this(new Point2D(0, 0)) {}

        /// <summary>
        /// Initializes default <see cref="DikeProfile"/> at the world origin.
        /// </summary>
        /// <param name="name">The name of the dike profile.</param>
        public TestDikeProfile(string name) : this(name, new Point2D(0, 0)) {}

        /// <summary>
        /// Initializes default <see cref="DikeProfile"/> at the world location.
        /// </summary>
        /// <param name="point">The world coordinate of the dike profile.</param>
        public TestDikeProfile(Point2D point) : this(null, point) {}

        /// <summary>
        ///Initializes default <see cref="DikeProfile"/> at the world location.
        /// </summary>
        /// <param name="name">The name of the dike profile.</param>
        /// <param name="point">The world coordinate of the dike profile.</param>
        public TestDikeProfile(string name, Point2D point) : base(point, Enumerable.Empty<RoughnessPoint>(), Enumerable.Empty<Point2D>(), null, new ConstructionProperties
        {
            Name = name
        }) {}
    }
}