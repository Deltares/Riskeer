using System.Drawing;
using Core.GIS.SharpMap.Api.Enums;

namespace Core.GIS.SharpMap.Api.Layers
{
    public interface ILabelStyle : IStyle
    {
        /// <summary>
        /// Label Font
        /// </summary>
        Font Font { get; set; }

        /// <summary>
        /// Font color
        /// </summary>
        Color ForeColor { get; set; }

        /// <summary>
        /// The background color of the label. Set to transparent brush or null if background isn't needed
        /// </summary>
        Brush BackColor { get; set; }

        /// <summary>
        /// Creates a halo around the text
        /// </summary>
        Pen Halo { get; set; }

        /// <summary>
        /// Specifies relative position of labels with respect to objects label point
        /// </summary>
        PointF Offset { get; set; }

        /// <summary>
        /// Gets or sets whether Collision Detection is enabled for the labels.
        /// If set to true, label collision will be tested.
        /// </summary>
        bool CollisionDetection { get; set; }

        /// <summary>
        /// Distance around label where collision buffer is active
        /// </summary>
        SizeF CollisionBuffer { get; set; }

        /// <summary>
        /// The horisontal alignment of the text in relation to the labelpoint
        /// </summary>
        HorizontalAlignmentEnum HorizontalAlignment { get; set; }

        /// <summary>
        /// The horisontal alignment of the text in relation to the labelpoint
        /// </summary>
        VerticalAlignmentEnum VerticalAlignment { get; set; }

        object Clone();
    }
}