using System.Collections.Generic;
using System.Linq;

namespace Wti.Data
{
    /// <summary>
    /// Definition of a surfaceline for piping.
    /// </summary>
    public class PipingSurfaceLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingSurfaceLine"/> class.
        /// </summary>
        public PipingSurfaceLine()
        {
            Name = string.Empty;
            Points = Enumerable.Empty<Point3D>();
        }

        /// <summary>
        /// Gets or sets the name of the surfaceline.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the 3D points describing its geometry.
        /// </summary>
        public IEnumerable<Point3D> Points { get; private set; }

        /// <summary>
        /// Gets or sets the first 3D geometry point defining the surfaceline in world coordinates.
        /// </summary>
        public Point3D StartingWorldPoint { get; private set; }

        /// <summary>
        /// Gets or sets the last 3D geometry point defining the surfaceline in world coordinates.
        /// </summary>
        public Point3D EndingWorldPoint { get; private set; }

        /// <summary>
        /// Sets the geometry of the surfaceline.
        /// </summary>
        /// <param name="points">The collection of points defining the surfaceline geometry.</param>
        public void SetGeometry(IEnumerable<Point3D> points)
        {
            var point3Ds = points.ToArray();
            Points = point3Ds;

            if (point3Ds.Length > 0)
            {
                StartingWorldPoint = point3Ds[0];
                EndingWorldPoint = point3Ds[point3Ds.Length - 1];
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}