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

using System.Drawing;
using Core.GIS.SharpMap.Api.Enums;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.Styles
{
    /// <summary>
    /// Defines a style used for rendering labels
    /// </summary>
    public class LabelStyle : Style, ILabelStyle
    {
        /// <summary>
        /// Initializes a new LabelStyle
        /// </summary>
        public LabelStyle()
        {
            Font = new Font("Times New Roman", 12f);
            Offset = new PointF(0, 0);
            CollisionDetection = false;
            CollisionBuffer = new Size(0, 0);
            ForeColor = Color.Black;
            HorizontalAlignment = HorizontalAlignmentEnum.Center;
            VerticalAlignment = VerticalAlignmentEnum.Middle;
        }

        protected LabelStyle(LabelStyle another) : base(another)
        {
            Font = (Font) another.Font.Clone();
            Offset = another.Offset;
            CollisionDetection = another.CollisionDetection;
            CollisionBuffer = another.CollisionBuffer;
            ForeColor = another.ForeColor;

            if (another.BackColor != null && another.BackColor is SolidBrush)
            {
                BackColor = new SolidBrush((another.BackColor as SolidBrush).Color);
            }
            if (another.Halo != null)
            {
                Halo = new Pen(another.Halo.Color, another.Halo.Width);
            }

            Offset = another.Offset;
            HorizontalAlignment = another.HorizontalAlignment;
            VerticalAlignment = another.VerticalAlignment;
        }

        /// <summary>
        /// Label Font
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Font color
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        /// The background color of the label. Set to transparent brush or null if background isn't needed
        /// </summary>
        public Brush BackColor { get; set; }

        /// <summary>
        /// Creates a halo around the text
        /// </summary>
        public Pen Halo { get; set; }

        /// <summary>
        /// Specifies relative position of labels with respect to objects label point
        /// </summary>
        public PointF Offset { get; set; }

        /// <summary>
        /// Gets or sets whether Collision Detection is enabled for the labels.
        /// If set to true, label collision will be tested.
        /// </summary>
        public bool CollisionDetection { get; set; }

        /// <summary>
        /// Distance around label where collision buffer is active
        /// </summary>
        public SizeF CollisionBuffer { get; set; }

        /// <summary>
        /// The horisontal alignment of the text in relation to the labelpoint
        /// </summary>
        public HorizontalAlignmentEnum HorizontalAlignment { get; set; }

        /// <summary>
        /// The horisontal alignment of the text in relation to the labelpoint
        /// </summary>
        public VerticalAlignmentEnum VerticalAlignment { get; set; }

        public override object Clone()
        {
            return new LabelStyle(this);
        }
    }
}