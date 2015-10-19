using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace GisSharpBlog.NetTopologySuite.LinearReferencing
{
    /// <summary>
    /// Builds a linear geometry (<see cref="LineString" /> or <see cref="MultiLineString" />)
    /// incrementally (point-by-point).
    /// </summary>
    public class LinearGeometryBuilder
    {
        private readonly IGeometryFactory geomFact = null;
        private readonly List<IGeometry> lines = new List<IGeometry>();
        private CoordinateList coordList = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geomFact"></param>
        public LinearGeometryBuilder(IGeometryFactory geomFact)
        {
            LastCoordinate = null;
            IgnoreInvalidLines = false;
            FixInvalidLines = false;
            this.geomFact = geomFact;
        }

        /// <summary>
        /// Allows invalid lines to be fixed rather than causing Exceptions.
        /// An invalid line is one which has only one unique point.
        /// </summary>
        public bool FixInvalidLines { get; set; }

        /// <summary>
        /// Allows invalid lines to be ignored rather than causing Exceptions.
        /// An invalid line is one which has only one unique point.
        /// </summary>
        public bool IgnoreInvalidLines { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate LastCoordinate { get; private set; }

        /// <summary>
        /// Adds a point to the current line.
        /// </summary>
        /// <param name="pt">The <see cref="Coordinate" /> to add.</param>
        public void Add(ICoordinate pt)
        {
            Add(pt, true);
        }

        /// <summary>
        /// Adds a point to the current line.
        /// </summary>
        /// <param name="pt">The <see cref="Coordinate" /> to add.</param>
        /// <param name="allowRepeatedPoints">If <c>true</c>, allows the insertions of repeated points.</param>
        public void Add(ICoordinate pt, bool allowRepeatedPoints)
        {
            if (coordList == null)
            {
                coordList = new CoordinateList();
            }
            coordList.Add(pt, allowRepeatedPoints);
            LastCoordinate = pt;
        }

        /// <summary>
        /// Terminate the current <see cref="LineString" />.
        /// </summary>
        public void EndLine()
        {
            if (coordList == null)
            {
                return;
            }

            if (IgnoreInvalidLines && coordList.Count < 2)
            {
                coordList = null;
                return;
            }

            ICoordinate[] rawPts = coordList.ToCoordinateArray();
            ICoordinate[] pts = rawPts;
            if (FixInvalidLines)
            {
                pts = ValidCoordinateSequence(rawPts);
            }

            coordList = null;
            ILineString line = null;
            try
            {
                line = geomFact.CreateLineString(pts);
            }
            catch (ArgumentException)
            {
                // exception is due to too few points in line.
                // only propagate if not ignoring short lines
                if (!IgnoreInvalidLines)
                {
                    throw;
                }
            }

            if (line != null)
            {
                lines.Add(line);
            }
        }

        /// <summary>
        /// Builds and returns the <see cref="Geometry" />.
        /// </summary>
        /// <returns></returns>
        public IGeometry GetGeometry()
        {
            // end last line in case it was not done by user
            EndLine();
            return geomFact.BuildGeometry(lines);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        private ICoordinate[] ValidCoordinateSequence(ICoordinate[] pts)
        {
            if (pts.Length >= 2)
            {
                return pts;
            }
            ICoordinate[] validPts = new ICoordinate[]
            {
                pts[0],
                pts[0]
            };
            return validPts;
        }
    }
}