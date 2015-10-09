#region Copyright and License

/****************************************************************************
**
** Copyright (C) 2008 - 2011 Winston Fletcher.
** All rights reserved.
**
** This file is part of the EGIS.ShapeFileLib class library of Easy GIS .NET.
** 
** Easy GIS .NET is free software: you can redistribute it and/or modify
** it under the terms of the GNU Lesser General Public License version 3 as
** published by the Free Software Foundation and appearing in the file
** lgpl-license.txt included in the packaging of this file.
**
** Easy GIS .NET is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
** GNU General Public License for more details.
**
** You should have received a copy of the GNU General Public License and
** GNU Lesser General Public License along with Easy GIS .NET.
** If not, see <http://www.gnu.org/licenses/>.
**
****************************************************************************/

#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SharpMap.Data.Providers.EGIS.ShapeFileLib
{
    /// <summary>Stores a set of four double-precision floating-point numbers that represent the location and size of a rectangle. </summary>
    /// <filterpriority>1</filterpriority>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RectangleD
    {
        /// <summary>Represents an instance of the <see cref="T:System.Drawing.RectangleD"></see> class with its members uninitialized.</summary>
        /// <filterpriority>1</filterpriority>
        public static readonly RectangleD Empty;

        private double x;
        private double y;
        private double width;
        private double height;

        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.RectangleD"></see> class with the specified location and size.</summary>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle. </param>
        /// <param name="width">The width of the rectangle. </param>
        /// <param name="height">The height of the rectangle. </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle. </param>
        public RectangleD(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.RectangleD"></see> class with the specified location and size.</summary>
        /// <param name="size">A <see cref="T:System.Drawing.SizeD"></see> that represents the width and height of the rectangular region. </param>
        /// <param name="location">A <see cref="T:System.Drawing.PointD"></see> that represents the upper-left corner of the rectangular region. </param>
        public RectangleD(PointD location, SizeD size)
        {
            x = location.X;
            y = location.Y;
            width = size.Width;
            height = size.Height;
        }

        /// <summary>Creates a <see cref="T:System.Drawing.RectangleD"></see> structure with upper-left corner and lower-right corner at the specified locations.</summary>
        /// <returns>The new <see cref="T:System.Drawing.RectangleD"></see> that this method creates.</returns>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangular region. </param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangular region. </param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangular region. </param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangular region. </param>
        /// <filterpriority>1</filterpriority>
        public static RectangleD FromLTRB(double left, double top, double right, double bottom)
        {
            return new RectangleD(left, top, right - left, bottom - top);
        }

        /// <summary>Gets or sets the coordinates of the upper-left corner of this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>A <see cref="T:System.Drawing.PointD"></see> that represents the upper-left corner of this <see cref="T:System.Drawing.RectangleD"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public PointD Location
        {
            get
            {
                return new PointD(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>Gets or sets the size of this <see cref="T:System.Drawing.RectangleD"></see>.</summary>
        /// <returns>A <see cref="T:System.Drawing.SizeD"></see> that represents the width and height of this <see cref="T:System.Drawing.RectangleD"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public SizeD Size
        {
            get
            {
                return new SizeD(Width, Height);
            }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>Gets or sets the x-coordinate of the upper-left corner of this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>The x-coordinate of the upper-left corner of this <see cref="T:System.Drawing.RectangleD"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        /// <summary>Gets or sets the y-coordinate of the upper-left corner of this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>The y-coordinate of the upper-left corner of this <see cref="T:System.Drawing.RectangleD"></see> structure. </returns>
        /// <filterpriority>1</filterpriority>
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        /// <summary>Gets or sets the width of this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>The width of this <see cref="T:System.Drawing.RectangleD"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        /// <summary>Gets or sets the height of this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>The height of this <see cref="T:System.Drawing.RectangleD"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        /// <summary>Gets the x-coordinate of the left edge of this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>The x-coordinate of the left edge of this <see cref="T:System.Drawing.RectangleD"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public double Left
        {
            get
            {
                return X;
            }
        }

        /// <summary>Gets the y-coordinate of the top edge of this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>The y-coordinate of the top edge of this <see cref="T:System.Drawing.RectangleD"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public double Top
        {
            get
            {
                return Y;
            }
        }

        /// <summary>Gets the x-coordinate that is the sum of <see cref="P:System.Drawing.RectangleD.X"></see> and <see cref="P:System.Drawing.RectangleD.Width"></see> of this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>The x-coordinate that is the sum of <see cref="P:System.Drawing.RectangleD.X"></see> and <see cref="P:System.Drawing.RectangleD.Width"></see> of this <see cref="T:System.Drawing.RectangleD"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public double Right
        {
            get
            {
                return (X + Width);
            }
        }

        /// <summary>Gets the y-coordinate that is the sum of <see cref="P:System.Drawing.RectangleD.Y"></see> and <see cref="P:System.Drawing.RectangleD.Height"></see> of this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>The y-coordinate that is the sum of <see cref="P:System.Drawing.RectangleD.Y"></see> and <see cref="P:System.Drawing.RectangleD.Height"></see> of this <see cref="T:System.Drawing.RectangleD"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public double Bottom
        {
            get
            {
                return (Y + Height);
            }
        }

        /// <summary>Tests whether the <see cref="P:System.Drawing.RectangleD.Width"></see> or <see cref="P:System.Drawing.RectangleD.Height"></see> property of this <see cref="T:System.Drawing.RectangleD"></see> has a value of zero.</summary>
        /// <returns>This property returns true if the <see cref="P:System.Drawing.RectangleD.Width"></see> or <see cref="P:System.Drawing.RectangleD.Height"></see> property of this <see cref="T:System.Drawing.RectangleD"></see> has a value of zero; otherwise, false.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                if (Width > 0f)
                {
                    return (Height <= 0f);
                }
                return true;
            }
        }

        /// <summary>Tests whether obj is a <see cref="T:System.Drawing.RectangleD"></see> with the same location and size of this <see cref="T:System.Drawing.RectangleD"></see>.</summary>
        /// <returns>This method returns true if obj is a <see cref="T:System.Drawing.RectangleD"></see> and its X, Y, Width, and Height properties are equal to the corresponding properties of this <see cref="T:System.Drawing.RectangleD"></see>; otherwise, false.</returns>
        /// <param name="obj">The <see cref="T:System.Object"></see> to test. </param>
        /// <filterpriority>1</filterpriority>
        public override bool Equals(object obj)
        {
            if (!(obj is RectangleD))
            {
                return false;
            }
            RectangleD ef = (RectangleD) obj;
            return ((((ef.X == X) && (ef.Y == Y)) && (ef.Width == Width)) && (ef.Height == Height));
        }

        /// <summary>Tests whether two <see cref="T:System.Drawing.RectangleD"></see> structures have equal location and size.</summary>
        /// <returns>This operator returns true if the two specified <see cref="T:System.Drawing.RectangleD"></see> structures have equal <see cref="P:System.Drawing.RectangleD.X"></see>, <see cref="P:System.Drawing.RectangleD.Y"></see>, <see cref="P:System.Drawing.RectangleD.Width"></see>, and <see cref="P:System.Drawing.RectangleD.Height"></see> properties.</returns>
        /// <param name="right">The <see cref="T:System.Drawing.RectangleD"></see> structure that is to the right of the equality operator. </param>
        /// <param name="left">The <see cref="T:System.Drawing.RectangleD"></see> structure that is to the left of the equality operator. </param>
        /// <filterpriority>3</filterpriority>
        public static bool operator ==(RectangleD left, RectangleD right)
        {
            return ((((left.X == right.X) && (left.Y == right.Y)) && (left.Width == right.Width)) && (left.Height == right.Height));
        }

        /// <summary>Tests whether two <see cref="T:System.Drawing.RectangleD"></see> structures differ in location or size.</summary>
        /// <returns>This operator returns true if any of the <see cref="P:System.Drawing.RectangleD.X"></see> , <see cref="P:System.Drawing.RectangleD.Y"></see>, <see cref="P:System.Drawing.RectangleD.Width"></see>, or <see cref="P:System.Drawing.RectangleD.Height"></see> properties of the two <see cref="T:System.Drawing.Rectangle"></see> structures are unequal; otherwise false.</returns>
        /// <param name="right">The <see cref="T:System.Drawing.RectangleD"></see> structure that is to the right of the inequality operator. </param>
        /// <param name="left">The <see cref="T:System.Drawing.RectangleD"></see> structure that is to the left of the inequality operator. </param>
        /// <filterpriority>3</filterpriority>
        public static bool operator !=(RectangleD left, RectangleD right)
        {
            return !(left == right);
        }

        /// <summary>Determines if the specified point is contained within this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>This method returns true if the point defined by x and y is contained within this <see cref="T:System.Drawing.RectangleD"></see> structure; otherwise false.</returns>
        /// <param name="y">The y-coordinate of the point to test. </param>
        /// <param name="x">The x-coordinate of the point to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(double x, double y)
        {
            return ((((X <= x) && (x < (X + Width))) && (Y <= y)) && (y < (Y + Height)));
        }

        /// <summary>Determines if the specified point is contained within this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>This method returns true if the point represented by the pt parameter is contained within this <see cref="T:System.Drawing.RectangleD"></see> structure; otherwise false.</returns>
        /// <param name="pt">The <see cref="T:System.Drawing.PointD"></see> to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(PointD pt)
        {
            return Contains(pt.X, pt.Y);
        }

        /// <summary>Determines if the rectangular region represented by rect is entirely contained within this <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>This method returns true if the rectangular region represented by rect is entirely contained within the rectangular region represented by this <see cref="T:System.Drawing.RectangleD"></see>; otherwise false.</returns>
        /// <param name="rect">The <see cref="T:System.Drawing.RectangleD"></see> to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(RectangleD rect)
        {
            return ((((X <= rect.X) && ((rect.X + rect.Width) <= (X + Width))) && (Y <= rect.Y)) && ((rect.Y + rect.Height) <= (Y + Height)));
        }

        /// <summary>Gets the hash code for this <see cref="T:System.Drawing.RectangleD"></see> structure. For information about the use of hash codes, see Object.GetHashCode.</summary>
        /// <returns>The hash code for this <see cref="T:System.Drawing.RectangleD"></see>.</returns>
        /// <filterpriority>1</filterpriority>
        public override int GetHashCode()
        {
            return (int) (((((uint) X) ^ ((((uint) Y) << 13) | (((uint) Y) >> 0x13))) ^ ((((uint) Width) << 0x1a) | (((uint) Width) >> 6))) ^ ((((uint) Height) << 7) | (((uint) Height) >> 0x19)));
        }

        /// <summary>Inflates this <see cref="T:System.Drawing.RectangleD"></see> structure by the specified amount.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="y">The amount to inflate this <see cref="T:System.Drawing.RectangleD"></see> structure vertically. </param>
        /// <param name="x">The amount to inflate this <see cref="T:System.Drawing.RectangleD"></see> structure horizontally. </param>
        /// <filterpriority>1</filterpriority>
        public void Inflate(double x, double y)
        {
            X -= x;
            Y -= y;
            Width += 2f*x;
            Height += 2f*y;
        }

        /// <summary>Inflates this <see cref="T:System.Drawing.RectangleD"></see> by the specified amount.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="size">The amount to inflate this rectangle. </param>
        /// <filterpriority>1</filterpriority>
        public void Inflate(SizeD size)
        {
            Inflate(size.Width, size.Height);
        }

        /// <summary>Creates and returns an inflated copy of the specified <see cref="T:System.Drawing.RectangleD"></see> structure. The copy is inflated by the specified amount. The original rectangle remains unmodified.</summary>
        /// <returns>The inflated <see cref="T:System.Drawing.RectangleD"></see>.</returns>
        /// <param name="rect">The <see cref="T:System.Drawing.RectangleD"></see> to be copied. This rectangle is not modified. </param>
        /// <param name="y">The amount to inflate the copy of the rectangle vertically. </param>
        /// <param name="x">The amount to inflate the copy of the rectangle horizontally. </param>
        /// <filterpriority>1</filterpriority>
        public static RectangleD Inflate(RectangleD rect, double x, double y)
        {
            RectangleD ef = rect;
            ef.Inflate(x, y);
            return ef;
        }

        /// <summary>Replaces this <see cref="T:System.Drawing.RectangleD"></see> structure with the intersection of itself and the specified <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="rect">The rectangle to intersect. </param>
        /// <filterpriority>1</filterpriority>
        public void Intersect(RectangleD rect)
        {
            RectangleD ef = Intersect(rect, this);
            X = ef.X;
            Y = ef.Y;
            Width = ef.Width;
            Height = ef.Height;
        }

        /// <summary>Returns a <see cref="T:System.Drawing.RectangleD"></see> structure that represents the intersection of two rectangles. If there is no intersection, and empty <see cref="T:System.Drawing.RectangleD"></see> is returned.</summary>
        /// <returns>A third <see cref="T:System.Drawing.RectangleD"></see> structure the size of which represents the overlapped area of the two specified rectangles.</returns>
        /// <param name="a">A rectangle to intersect. </param>
        /// <param name="b">A rectangle to intersect. </param>
        /// <filterpriority>1</filterpriority>
        public static RectangleD Intersect(RectangleD a, RectangleD b)
        {
            double x = Math.Max(a.X, b.X);
            double num2 = Math.Min((double) (a.X + a.Width), (double) (b.X + b.Width));
            double y = Math.Max(a.Y, b.Y);
            double num4 = Math.Min((double) (a.Y + a.Height), (double) (b.Y + b.Height));
            if ((num2 >= x) && (num4 >= y))
            {
                return new RectangleD(x, y, num2 - x, num4 - y);
            }
            return Empty;
        }

        /// <summary>Determines if this rectangle intersects with rect.</summary>
        /// <returns>This method returns true if there is any intersection.</returns>
        /// <param name="rect">The rectangle to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool IntersectsWith(RectangleD rect)
        {
            return ((((rect.X < (X + Width)) && (X < (rect.X + rect.Width))) && (rect.Y < (Y + Height))) && (Y < (rect.Y + rect.Height)));
        }

        /// <summary>Creates the smallest possible third rectangle that can contain both of two rectangles that form a union.</summary>
        /// <returns>A third <see cref="T:System.Drawing.RectangleD"></see> structure that contains both of the two rectangles that form the union.</returns>
        /// <param name="a">A rectangle to union. </param>
        /// <param name="b">A rectangle to union. </param>
        /// <filterpriority>1</filterpriority>
        public static RectangleD Union(RectangleD a, RectangleD b)
        {
            double x = Math.Min(a.X, b.X);
            double num2 = Math.Max((double) (a.X + a.Width), (double) (b.X + b.Width));
            double y = Math.Min(a.Y, b.Y);
            double num4 = Math.Max((double) (a.Y + a.Height), (double) (b.Y + b.Height));
            return new RectangleD(x, y, num2 - x, num4 - y);
        }

        /// <summary>Adjusts the location of this rectangle by the specified amount.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="pos">The amount to offset the location. </param>
        /// <filterpriority>1</filterpriority>
        public void Offset(PointD pos)
        {
            Offset(pos.X, pos.Y);
        }

        /// <summary>Adjusts the location of this rectangle by the specified amount.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="y">The amount to offset the location vertically. </param>
        /// <param name="x">The amount to offset the location horizontally. </param>
        /// <filterpriority>1</filterpriority>
        public void Offset(double x, double y)
        {
            X += x;
            Y += y;
        }

        /// <summary>Converts the specified <see cref="T:System.Drawing.Rectangle"></see> structure to a <see cref="T:System.Drawing.RectangleD"></see> structure.</summary>
        /// <returns>The <see cref="T:System.Drawing.RectangleD"></see> structure that is converted from the specified <see cref="T:System.Drawing.Rectangle"></see> structure.</returns>
        /// <param name="r">The <see cref="T:System.Drawing.Rectangle"></see> structure to convert. </param>
        /// <filterpriority>3</filterpriority>
        public static implicit operator RectangleD(Rectangle r)
        {
            return new RectangleD((double) r.X, (double) r.Y, (double) r.Width, (double) r.Height);
        }

        public static implicit operator RectangleD(RectangleF r)
        {
            return new RectangleD((double) r.X, (double) r.Y, (double) r.Width, (double) r.Height);
        }

        public static implicit operator RectangleF(RectangleD r)
        {
            return new RectangleF((float) r.X, (float) r.Y, (float) r.Width, (float) r.Height);
        }

        /// <summary>Converts the Location and <see cref="T:System.Drawing.Size"></see> of this <see cref="T:System.Drawing.RectangleD"></see> to a human-readable string.</summary>
        /// <returns>A string that contains the position, width, and height of this <see cref="T:System.Drawing.RectangleD"></see> structure¾for example, "{X=20, Y=20, Width=100, Height=50}".</returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /></PermissionSet>
        public override string ToString()
        {
            return ("{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) + ",Width=" + Width.ToString(CultureInfo.CurrentCulture) + ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}");
        }

        static RectangleD()
        {
            Empty = new RectangleD();
        }
    }
}