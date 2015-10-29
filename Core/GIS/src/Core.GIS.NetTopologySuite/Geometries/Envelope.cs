using System;
using Core.GIS.GeoApi.Geometries;

namespace Core.GIS.NetTopologySuite.Geometries
{
    /// <summary>
    /// Defines a rectangular region of the 2D coordinate plane.
    /// It is often used to represent the bounding box of a <c>Geometry</c>,
    /// e.g. the minimum and maximum x and y values of the <c>Coordinate</c>s.
    /// Note that Envelopes support infinite or half-infinite regions, by using the values of
    /// <c>Double.PositiveInfinity</c> and <c>Double.NegativeInfinity</c>.
    /// When Envelope objects are created or initialized,
    /// the supplies extent values are automatically sorted into the correct order.    
    /// </summary>
    [Serializable]
    public class Envelope : IEnvelope
    {
        /*
        *  the minimum x-coordinate
        */

        /*
        *  the maximum x-coordinate
        */

        /*
        * the minimum y-coordinate
        */

        /*
        *  the maximum y-coordinate
        */

        /// <summary>
        /// Creates a null <c>Envelope</c>.
        /// </summary>
        public Envelope()
        {
            Init();
        }

        /// <summary>
        /// Creates an <c>Envelope</c> for a region defined by maximum and minimum values.
        /// </summary>
        /// <param name="x1">The first x-value.</param>
        /// <param name="x2">The second x-value.</param>
        /// <param name="y1">The first y-value.</param>
        /// <param name="y2">The second y-value.</param>
        public Envelope(double x1, double x2, double y1, double y2)
        {
            Init(x1, x2, y1, y2);
        }

        /// <summary>
        /// Creates an <c>Envelope</c> for a region defined by two Coordinates.
        /// </summary>
        /// <param name="p1">The first Coordinate.</param>
        /// <param name="p2">The second Coordinate.</param>
        public Envelope(ICoordinate p1, ICoordinate p2)
        {
            Init(p1, p2);
        }

        /// <summary>
        /// Creates an <c>Envelope</c> for a region defined by a single Coordinate.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        public Envelope(ICoordinate p)
        {
            Init(p);
        }

        /// <summary>
        /// Create an <c>Envelope</c> from an existing Envelope.
        /// </summary>
        /// <param name="env">The Envelope to initialize from.</param>
        public Envelope(IEnvelope env)
        {
            Init(env);
        }

        /// <summary>
        /// Returns <c>true</c> if this <c>Envelope</c> is a "null" envelope.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this <c>Envelope</c> is uninitialized
        /// or is the envelope of the empty point.
        /// </returns>
        public bool IsNull
        {
            get
            {
                return MaxX < MinX;
            }
        }

        /// <summary>
        /// Returns the difference between the maximum and minimum x values.
        /// </summary>
        /// <returns>max x - min x, or 0 if this is a null <c>Envelope</c>.</returns>
        public double Width
        {
            get
            {
                if (IsNull)
                {
                    return 0;
                }
                return MaxX - MinX;
            }
        }

        /// <summary>
        /// Returns the difference between the maximum and minimum y values.
        /// </summary>
        /// <returns>max y - min y, or 0 if this is a null <c>Envelope</c>.</returns>
        public double Height
        {
            get
            {
                if (IsNull)
                {
                    return 0;
                }
                return MaxY - MinY;
            }
        }

        /// <summary>
        /// Returns the <c>Envelope</c>s minimum x-value. min x > max x
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The minimum x-coordinate.</returns>
        public double MinX { get; private set; }

        /// <summary>
        /// Returns the <c>Envelope</c>s maximum x-value. min x > max x
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The maximum x-coordinate.</returns>
        public double MaxX { get; private set; }

        /// <summary>
        /// Returns the <c>Envelope</c>s minimum y-value. min y > max y
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The minimum y-coordinate.</returns>
        public double MinY { get; private set; }

        /// <summary>
        /// Returns the <c>Envelope</c>s maximum y-value. min y > max y
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The maximum y-coordinate.</returns>
        public double MaxY { get; private set; }

        /// <summary>
        /// Computes the coordinate of the centre of this envelope (as long as it is non-null).
        /// </summary>
        /// <returns>
        /// The centre coordinate of this envelope, 
        /// or <c>null</c> if the envelope is null.
        /// </returns>.
        public ICoordinate Centre
        {
            get
            {
                if (IsNull)
                {
                    return null;
                }
                return new Coordinate((MinX + MaxX)/2.0, (MinY + MaxY)/2.0);
            }
        }

        /* BEGIN ADDED BY MPAUL42: monoGIS team */

        /// <summary>
        /// Returns the area of the envelope.
        /// </summary>
        public double Area
        {
            get
            {
                double area = 1;
                area = area*(MaxX - MinX);
                area = area*(MaxY - MinY);
                return area;
            }
        }

        /// <summary>
        /// Test the point q to see whether it intersects the Envelope
        /// defined by p1-p2.
        /// </summary>
        /// <param name="p1">One extremal point of the envelope.</param>
        /// <param name="p2">Another extremal point of the envelope.</param>
        /// <param name="q">Point to test for intersection.</param>
        /// <returns><c>true</c> if q intersects the envelope p1-p2.</returns>
        public static bool Intersects(ICoordinate p1, ICoordinate p2, ICoordinate q)
        {
            if (((q.X >= (p1.X < p2.X ? p1.X : p2.X)) && (q.X <= (p1.X > p2.X ? p1.X : p2.X))) &&
                ((q.Y >= (p1.Y < p2.Y ? p1.Y : p2.Y)) && (q.Y <= (p1.Y > p2.Y ? p1.Y : p2.Y))))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Test the envelope defined by p1-p2 for intersection
        /// with the envelope defined by q1-q2
        /// </summary>
        /// <param name="p1">One extremal point of the envelope Point.</param>
        /// <param name="p2">Another extremal point of the envelope Point.</param>
        /// <param name="q1">One extremal point of the envelope Q.</param>
        /// <param name="q2">Another extremal point of the envelope Q.</param>
        /// <returns><c>true</c> if Q intersects Point</returns>
        public static bool Intersects(ICoordinate p1, ICoordinate p2, ICoordinate q1, ICoordinate q2)
        {
            double minq = Math.Min(q1.X, q2.X);
            double maxq = Math.Max(q1.X, q2.X);
            double minp = Math.Min(p1.X, p2.X);
            double maxp = Math.Max(p1.X, p2.X);
            if (minp > maxq)
            {
                return false;
            }
            if (maxp < minq)
            {
                return false;
            }

            minq = Math.Min(q1.Y, q2.Y);
            maxq = Math.Max(q1.Y, q2.Y);
            minp = Math.Min(p1.Y, p2.Y);
            maxp = Math.Max(p1.Y, p2.Y);
            if (minp > maxq)
            {
                return false;
            }
            if (maxp < minq)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator ==(Envelope obj1, Envelope obj2)
        {
            return Equals(obj1, obj2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator !=(Envelope obj1, Envelope obj2)
        {
            return !(obj1 == obj2);
        }

        /// <summary>
        /// Creates a deep copy of the current envelope.
        /// </summary>
        /// <returns></returns>
        public IEnvelope Clone()
        {
            return new Envelope(MinX, MaxX, MinY, MaxY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (!(other is Envelope))
            {
                return false;
            }

            return Equals((IEnvelope) other);
        }

        /// <summary>
        /// 
        /// </summary>
        public override int GetHashCode()
        {
            int result = 17;
            result = 37*result + GetHashCode(MinX);
            result = 37*result + GetHashCode(MaxX);
            result = 37*result + GetHashCode(MinY);
            result = 37*result + GetHashCode(MaxY);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Env[" + MinX + " : " + MaxX + ", " + MinY + " : " + MaxY + "]";
        }

        /// <summary>
        /// Initialize to a null <c>Envelope</c>.
        /// </summary>
        public void Init()
        {
            SetToNull();
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by maximum and minimum values.
        /// </summary>
        /// <param name="x1">The first x-value.</param>
        /// <param name="x2">The second x-value.</param>
        /// <param name="y1">The first y-value.</param>
        /// <param name="y2">The second y-value.</param>
        public void Init(double x1, double x2, double y1, double y2)
        {
            if (x1 < x2)
            {
                MinX = x1;
                MaxX = x2;
            }
            else
            {
                MinX = x2;
                MaxX = x1;
            }

            if (y1 < y2)
            {
                MinY = y1;
                MaxY = y2;
            }
            else
            {
                MinY = y2;
                MaxY = y1;
            }
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by two Coordinates.
        /// </summary>
        /// <param name="p1">The first Coordinate.</param>
        /// <param name="p2">The second Coordinate.</param>
        public void Init(ICoordinate p1, ICoordinate p2)
        {
            Init(p1.X, p2.X, p1.Y, p2.Y);
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by a single Coordinate.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        public void Init(ICoordinate p)
        {
            Init(p.X, p.X, p.Y, p.Y);
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> from an existing Envelope.
        /// </summary>
        /// <param name="env">The Envelope to initialize from.</param>
        public void Init(IEnvelope env)
        {
            MinX = env.MinX;
            MaxX = env.MaxX;
            MinY = env.MinY;
            MaxY = env.MaxY;
        }

        /// <summary>
        /// Makes this <c>Envelope</c> a "null" envelope..
        /// </summary>
        public void SetToNull()
        {
            MinX = 0;
            MaxX = -1;
            MinY = 0;
            MaxY = -1;
        }

        /// <summary>
        /// Expands this envelope by a given distance in all directions.
        /// Both positive and negative distances are supported.
        /// </summary>
        /// <param name="distance">The distance to expand the envelope.</param>
        public void ExpandBy(double distance)
        {
            ExpandBy(distance, distance);
        }

        /// <summary>
        /// Expands this envelope by a given distance in all directions.
        /// Both positive and negative distances are supported.
        /// </summary>
        /// <param name="deltaX">The distance to expand the envelope along the the X axis.</param>
        /// <param name="deltaY">The distance to expand the envelope along the the Y axis.</param>
        public void ExpandBy(double deltaX, double deltaY)
        {
            if (IsNull)
            {
                return;
            }

            MinX -= deltaX;
            MaxX += deltaX;
            MinY -= deltaY;
            MaxY += deltaY;

            // check for envelope disappearing
            if (MinX > MaxX || MinY > MaxY)
            {
                SetToNull();
            }
        }

        /// <summary>
        /// Enlarges the boundary of the <c>Envelope</c> so that it contains (p).
        /// Does nothing if (p) is already on or within the boundaries.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        public void ExpandToInclude(ICoordinate p)
        {
            ExpandToInclude(p.X, p.Y);
        }

        /// <summary>
        /// Enlarges the boundary of the <c>Envelope</c> so that it contains
        /// (x,y). Does nothing if (x,y) is already on or within the boundaries.
        /// </summary>
        /// <param name="x">The value to lower the minimum x to or to raise the maximum x to.</param>
        /// <param name="y">The value to lower the minimum y to or to raise the maximum y to.</param>
        public void ExpandToInclude(double x, double y)
        {
            if (IsNull)
            {
                MinX = x;
                MaxX = x;
                MinY = y;
                MaxY = y;
            }
            else
            {
                if (x < MinX)
                {
                    MinX = x;
                }
                if (x > MaxX)
                {
                    MaxX = x;
                }
                if (y < MinY)
                {
                    MinY = y;
                }
                if (y > MaxY)
                {
                    MaxY = y;
                }
            }
        }

        /// <summary>
        /// Enlarges the boundary of the <c>Envelope</c> so that it contains
        /// <c>other</c>. Does nothing if <c>other</c> is wholly on or
        /// within the boundaries.
        /// </summary>
        /// <param name="other">the <c>Envelope</c> to merge with.</param>        
        public void ExpandToInclude(IEnvelope other)
        {
            if (other == null || other.IsNull)
            {
                return;
            }
            if (IsNull)
            {
                MinX = other.MinX;
                MaxX = other.MaxX;
                MinY = other.MinY;
                MaxY = other.MaxY;
            }
            else
            {
                if (other.MinX < MinX)
                {
                    MinX = other.MinX;
                }
                if (other.MaxX > MaxX)
                {
                    MaxX = other.MaxX;
                }
                if (other.MinY < MinY)
                {
                    MinY = other.MinY;
                }
                if (other.MaxY > MaxY)
                {
                    MaxY = other.MaxY;
                }
            }
        }

        /// <summary>
        /// Translates this envelope by given amounts in the X and Y direction.
        /// </summary>
        /// <param name="transX">The amount to translate along the X axis.</param>
        /// <param name="transY">The amount to translate along the Y axis.</param>
        public void Translate(double transX, double transY)
        {
            if (IsNull)
            {
                return;
            }
            Init(MinX + transX, MaxX + transX, MinY + transY, MaxY + transY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public IEnvelope Intersection(IEnvelope env)
        {
            if (IsNull || env.IsNull || !Intersects(env))
            {
                return new Envelope();
            }

            return new Envelope(Math.Max(MinX, env.MinX),
                                Math.Min(MaxX, env.MaxX),
                                Math.Max(MinY, env.MinY),
                                Math.Min(MaxY, env.MaxY));
        }

        /// <summary> 
        /// Check if the region defined by <c>other</c>
        /// overlaps (intersects) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="other"> the <c>Envelope</c> which this <c>Envelope</c> is
        /// being checked for overlapping.
        /// </param>
        /// <returns>        
        /// <c>true</c> if the <c>Envelope</c>s overlap.
        /// </returns>
        public bool Intersects(IEnvelope other)
        {
            if (IsNull || other.IsNull)
            {
                return false;
            }
            return !(other.MinX > MaxX || other.MaxX < MinX || other.MinY > MaxY || other.MaxY < MinY);
        }

        /// <summary>
        /// Use Intersects instead. In the future, Overlaps may be
        /// changed to be a true overlap check; that is, whether the intersection is
        /// two-dimensional.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [Obsolete("Use Intersects instead")]
        public bool Overlaps(IEnvelope other)
        {
            return Intersects(other);
        }

        /// <summary>
        /// Use Intersects instead.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Obsolete("Use Intersects instead")]
        public bool Overlaps(ICoordinate p)
        {
            return Intersects(p);
        }

        /// <summary>
        /// Use Intersects instead.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [Obsolete("Use Intersects instead")]
        public bool Overlaps(double x, double y)
        {
            return Intersects(x, y);
        }

        /// <summary>  
        /// Check if the point <c>p</c> overlaps (lies inside) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="p"> the <c>Coordinate</c> to be tested.</param>
        /// <returns><c>true</c> if the point overlaps this <c>Envelope</c>.</returns>
        public bool Intersects(ICoordinate p)
        {
            return Intersects(p.X, p.Y);
        }

        /// <summary>  
        /// Check if the point <c>(x, y)</c> overlaps (lies inside) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="x"> the x-ordinate of the point.</param>
        /// <param name="y"> the y-ordinate of the point.</param>
        /// <returns><c>true</c> if the point overlaps this <c>Envelope</c>.</returns>
        public bool Intersects(double x, double y)
        {
            return !(x > MaxX || x < MinX || y > MaxY || y < MinY);
        }

        /// <summary>  
        /// Returns <c>true</c> if the given point lies in or on the envelope.
        /// </summary>
        /// <param name="p"> the point which this <c>Envelope</c> is
        /// being checked for containing.</param>
        /// <returns>    
        /// <c>true</c> if the point lies in the interior or
        /// on the boundary of this <c>Envelope</c>.
        /// </returns>                
        public bool Contains(ICoordinate p)
        {
            return Contains(p.X, p.Y);
        }

        /// <summary>  
        /// Returns <c>true</c> if the given point lies in or on the envelope.
        /// </summary>
        /// <param name="x"> the x-coordinate of the point which this <c>Envelope</c> is
        /// being checked for containing.</param>
        /// <param name="y"> the y-coordinate of the point which this <c>Envelope</c> is
        /// being checked for containing.</param>
        /// <returns><c>true</c> if <c>(x, y)</c> lies in the interior or
        /// on the boundary of this <c>Envelope</c>.</returns>
        public bool Contains(double x, double y)
        {
            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
        }

        /// <summary>  
        /// Returns <c>true</c> if the <c>Envelope other</c>
        /// lies wholely inside this <c>Envelope</c> (inclusive of the boundary).
        /// </summary>
        /// <param name="other"> the <c>Envelope</c> which this <c>Envelope</c> is being checked for containing.</param>
        /// <returns><c>true</c> if <c>other</c> is contained in this <c>Envelope</c>.</returns>
        public bool Contains(IEnvelope other)
        {
            if (IsNull || other.IsNull)
            {
                return false;
            }
            return other.MinX >= MinX && other.MaxX <= MaxX &&
                   other.MinY >= MinY && other.MaxY <= MaxY;
        }

        /// <summary> 
        /// Computes the distance between this and another
        /// <c>Envelope</c>.
        /// The distance between overlapping Envelopes is 0.  Otherwise, the
        /// distance is the Euclidean distance between the closest points.
        /// </summary>
        /// <returns>The distance between this and another <c>Envelope</c>.</returns>
        public double Distance(IEnvelope env)
        {
            if (Intersects(env))
            {
                return 0;
            }

            double dx = 0.0;

            if (MaxX < env.MinX)
            {
                dx = env.MinX - MaxX;
            }
            if (MinX > env.MaxX)
            {
                dx = MinX - env.MaxX;
            }

            double dy = 0.0;

            if (MaxY < env.MinY)
            {
                dy = env.MinY - MaxY;
            }
            if (MinY > env.MaxY)
            {
                dy = MinY - env.MaxY;
            }

            // if either is zero, the envelopes overlap either vertically or horizontally
            if (dx == 0.0)
            {
                return dy;
            }
            if (dy == 0.0)
            {
                return dx;
            }

            return Math.Sqrt(dx*dx + dy*dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IEnvelope other)
        {
            if (IsNull)
            {
                return other.IsNull;
            }
            if (other == null)
            {
                return false;
            }
            return MaxX == other.MaxX && MaxY == other.MaxY &&
                   MinX == other.MinX && MinY == other.MinY;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            return CompareTo((IEnvelope) other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IEnvelope other)
        {
            if (IsNull && other.IsNull)
            {
                return 0;
            }
            else if (!IsNull && other.IsNull)
            {
                return 1;
            }
            else if (IsNull && !other.IsNull)
            {
                return -1;
            }

            if (Area > other.Area)
            {
                return 1;
            }
            if (Area < other.Area)
            {
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Calculates the union of the current box and the given point.
        /// </summary>
        public IEnvelope Union(IPoint point)
        {
            return Union(point.Coordinate);
        }

        /// <summary>
        /// Calculates the union of the current box and the given coordinate.
        /// </summary>
        public IEnvelope Union(ICoordinate coord)
        {
            Envelope env = (Envelope) Clone();
            env.ExpandToInclude(coord);
            return env;
        }

        /// <summary>
        /// Calculates the union of the current box and the given box.
        /// </summary>
        public IEnvelope Union(IEnvelope box)
        {
            if (box.IsNull)
            {
                return this;
            }
            if (IsNull)
            {
                return box;
            }

            return new Envelope(Math.Min(MinX, box.MinX),
                                Math.Max(MaxX, box.MaxX),
                                Math.Min(MinY, box.MinY),
                                Math.Max(MaxY, box.MaxY));
        }

        /// <summary>
        /// Moves the envelope to the indicated coordinate.
        /// </summary>
        /// <param name="centre">The new centre coordinate.</param>
        public void SetCentre(ICoordinate centre)
        {
            SetCentre(centre, Width, Height);
        }

        /// <summary>
        /// Moves the envelope to the indicated point.
        /// </summary>
        /// <param name="centre">The new centre point.</param>
        public void SetCentre(IPoint centre)
        {
            SetCentre(centre.Coordinate, Width, Height);
        }

        /// <summary>
        /// Resizes the envelope to the indicated point.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        public void SetCentre(double width, double height)
        {
            SetCentre(Centre, width, height);
        }

        /// <summary>
        /// Moves and resizes the current envelope.
        /// </summary>
        /// <param name="centre">The new centre point.</param>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        public void SetCentre(IPoint centre, double width, double height)
        {
            SetCentre(centre.Coordinate, width, height);
        }

        /// <summary>
        /// Moves and resizes the current envelope.
        /// </summary>
        /// <param name="centre">The new centre coordinate.</param>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        public void SetCentre(ICoordinate centre, double width, double height)
        {
            MinX = centre.X - (width/2);
            MaxX = centre.X + (width/2);
            MinY = centre.Y - (height/2);
            MaxY = centre.Y + (height/2);
        }

        /// <summary>
        /// Zoom the box. 
        /// Possible values are e.g. 50 (to zoom in a 50%) or -50 (to zoom out a 50%).
        /// </summary>
        /// <param name="perCent"> 
        /// Negative do Envelope smaller.
        /// Positive do Envelope bigger.
        /// </param>
        /// <example> 
        ///  perCent = -50 compact the envelope a 50% (make it smaller).
        ///  perCent = 200 enlarge envelope by 2.
        /// </example>
        public void Zoom(double perCent)
        {
            double w = (Width*perCent/100);
            double h = (Height*perCent/100);
            SetCentre(w, h);
        }

        /// <summary>
        /// Return HashCode.
        /// </summary>
        /// <param name="x">Value from HashCode computation.</param>
        private static int GetHashCode(double value)
        {
            long f = BitConverter.DoubleToInt64Bits(value);
            return (int) (f ^ (f >> 32));
        }

        /* END ADDED BY MPAUL42: monoGIS team */
    }
}