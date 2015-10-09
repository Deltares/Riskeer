// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Drawing;
using SharpMap.Api.Layers;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Defines an axis-aligned box around a label, used for collision detection
    /// </summary>
    public class LabelBox : IComparable<LabelBox>
    {
        /// <summary>
        /// Initializes a new LabelBox instance
        /// </summary>
        /// <param name="left">Left side of box</param>
        /// <param name="top">Top of box</param>
        /// <param name="width">Width of the box</param>
        /// <param name="height">Height of the box</param>
        public LabelBox(float left, float top, float width, float height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Initializes a new LabelBox instance based on a rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        public LabelBox(RectangleF rectangle)
        {
            Left = rectangle.X;
            Top = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        /// <summary>
        /// The Left tie-point for the Label
        /// </summary>
        public float Left { get; set; }

        /// <summary>
        /// The Top tie-point for the label
        /// </summary>
        public float Top { get; set; }

        /// <summary>
        /// Width of the box
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Height of the box
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Right side of the box
        /// </summary>
        public float Right
        {
            get
            {
                return Left + Width;
            }
        }

        /// <summary>
        /// Bottom of th ebox
        /// </summary>
        public float Bottom
        {
            get
            {
                return Top - Height;
            }
        }

        /// <summary>
        /// Determines whether the boundingbox intersects another boundingbox
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Intersects(LabelBox box)
        {
            return !(box.Left > Left + Width ||
                     box.Left + box.Width < Left ||
                     box.Top - box.Height > Top ||
                     box.Top < Top - Height);
        }

        #region IComparable<LabelBox> Members

        /// <summary>
        /// Returns 0 if the boxes intersects each other
        /// </summary>
        /// <param name="other">labelbox to perform intersectiontest with</param>
        /// <returns>0 if the intersect</returns>
        public int CompareTo(LabelBox other)
        {
            if (Intersects(other))
            {
                return 0;
            }
            else if (other.Left > Left + Width ||
                     other.Top - other.Height > Top)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        #endregion
    }

    /// <summary>
    /// Class for storing a label instance
    /// </summary>
    public class Label : IComparable<Label>, IComparer<Label>
    {
        /// <summary>
        /// Initializes a new Label instance
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="labelPoint">Position of label</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="priority">Label priority used for collision detection</param>
        /// <param name="collisionbox">Box around label for collision detection</param>
        /// <param name="style">The style of the label</param>
        public Label(string text, PointF labelPoint, float rotation, int priority, LabelBox collisionbox, ILabelStyle style)
        {
            this.Text = text;
            this.LabelPoint = labelPoint;
            this.Rotation = rotation;
            this.Priority = priority;
            Box = collisionbox;
            this.Style = style;
        }

        /// <summary>
        /// The text of the label
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Label position
        /// </summary>
        public PointF LabelPoint { get; set; }

        /// <summary>
        /// Label font
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Label rotation
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Text rotation in radians
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Label box
        /// </summary>
        public LabelBox Box { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SharpMap.Styles.LabelStyle"/> of this label
        /// </summary>
        public ILabelStyle Style { get; set; }

        #region IComparable<Label> Members

        /// <summary>
        /// Tests if two label boxes intersects
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Label other)
        {
            if (this == other)
            {
                return 0;
            }
            else if (Box == null)
            {
                return -1;
            }
            else if (other.Box == null)
            {
                return 1;
            }
            else
            {
                return Box.CompareTo(other.Box);
            }
        }

        #endregion

        #region IComparer<Label> Members

        /// <summary>
        /// Checks if two labels intersect
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Label x, Label y)
        {
            return x.CompareTo(y);
        }

        #endregion
    }
}