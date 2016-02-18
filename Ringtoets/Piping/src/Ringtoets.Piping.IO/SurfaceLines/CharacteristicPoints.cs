using System;
using Core.Common.Base.Geometry;

namespace Ringtoets.Piping.IO.SurfaceLines
{
    /// <summary>
    /// This class represents a collection of characterizing locations on a surface line.
    /// </summary>
    public class CharacteristicPoints
    {
        /// <summary>
        /// Creates an instance of <see cref="CharacteristicPoints"/>.
        /// </summary>
        /// <param name="name">The name of the location for which the <see cref="CharacteristicPoints"/>
        /// defines characteristic points.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public CharacteristicPoints(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", "Cannot make a definition of characteristic points for an unknown location.");
            }
            Name = name;
        }

        /// <summary>
        /// Gets the name of the location for which the <see cref="CharacteristicPoints"/> defines
        /// characteristic points.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the location of surface level inside.
        /// </summary>
        public Point3D SurfaceLevelInside { get; set; }

        /// <summary>
        /// Gets or sets the location of ditch polder side inside.
        /// </summary>
        public Point3D DitchPolderSide { get; set; }

        /// <summary>
        /// Gets or sets the location of bottom ditch polder side.
        /// </summary>
        public Point3D BottomDitchPolderSide { get; set; }

        /// <summary>
        /// Gets or sets the location of bottom ditch dike side.
        /// </summary>
        public Point3D BottomDitchDikeSide { get; set; }

        /// <summary>
        /// Gets or sets the location of ditch dike side.
        /// </summary>
        public Point3D DitchDikeSide { get; set; }

        /// <summary>
        /// Gets or sets the location of dike toe at polder.
        /// </summary>
        public Point3D DikeToeAtPolder { get; set; }

        /// <summary>
        /// Gets or sets the location of top shoulder inside.
        /// </summary>
        public Point3D TopShoulderInside { get; set; }

        /// <summary>
        /// Gets or sets the location of shoulder inside.
        /// </summary>
        public Point3D ShoulderInside { get; set; }

        /// <summary>
        /// Gets or sets the location of dike top at polder.
        /// </summary>
        public Point3D DikeTopAtPolder { get; set; }

        /// <summary>
        /// Gets or sets the location of traffic load inside.
        /// </summary>
        public Point3D TrafficLoadInside { get; set; }

        /// <summary>
        /// Gets or sets the location of traffic load outside.
        /// </summary>
        public Point3D TrafficLoadOutside { get; set; }

        /// <summary>
        /// Gets or sets the location of dike top at river.
        /// </summary>
        public Point3D DikeTopAtRiver { get; set; }

        /// <summary>
        /// Gets or sets the location of shoulder outside.
        /// </summary>
        public Point3D ShoulderOutisde { get; set; }

        /// <summary>
        /// Gets or sets the location of top shoulder outside.
        /// </summary>
        public Point3D TopShoulderOutside { get; set; }

        /// <summary>
        /// Gets or sets the location of dike toe at river.
        /// </summary>
        public Point3D DikeToeAtRiver { get; set; }

        /// <summary>
        /// Gets or sets the location of surface level outside.
        /// </summary>
        public Point3D SurfaceLevelOutside { get; set; }

        /// <summary>
        /// Gets or sets the location of dike table height.
        /// </summary>
        public Point3D DikeTableHeight { get; set; }

        /// <summary>
        /// Gets or sets the location of insert river channel.
        /// </summary>
        public Point3D InsertRiverChannel { get; set; }

        /// <summary>
        /// Gets or sets the location of bottom river channel.
        /// </summary>
        public Point3D BottomRiverChannel { get; set; }
    }
}