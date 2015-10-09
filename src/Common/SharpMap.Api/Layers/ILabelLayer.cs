using System.Drawing.Drawing2D;
using System.Drawing.Text;
using DelftTools.Utils.Aop;
using SharpMap.Api.Delegates;
using SharpMap.Api.Enums;

namespace SharpMap.Api.Layers
{
    public interface ILabelLayer : ILayer
    {
        /// <summary>
        /// Gets or sets labelling behavior on multipart geometries
        /// </summary>
        /// <remarks>Default value is <see cref="MultipartGeometryBehaviourEnum.All"/></remarks>
        MultipartGeometryBehaviourEnum MultipartGeometryBehaviour { get; set; }

        /// <summary>
        /// Render whether smoothing (antialiasing) is applied to lines and curves and the edges of filled areas
        /// </summary>
        SmoothingMode SmoothingMode { get; set; }

        /// <summary>
        /// Specifies the quality of text rendering
        /// </summary>
        TextRenderingHint TextRenderingHint { get; set; }

        /// <summary>
        /// Gets or sets the rendering style of the label layer.
        /// </summary>
        ILabelStyle Style { get; set; }

        /// <summary>
        /// Gets or sets thematic settings for the layer. Set to null to ignore thematics
        /// </summary>
        ITheme Theme { get; set; }

        /// <summary>
        /// Data column or expression where label text is extracted from.
        /// </summary>
        /// <remarks>
        /// This property is overriden by the <see cref="LabelStringDelegate"/>.
        /// </remarks>
        string LabelColumn { get; set; }

        /// <summary>
        /// Gets or sets the method for creating a custom label string based on a feature.
        /// </summary>
        /// <remarks>
        /// <para>If this method is not null, it will override the <see cref="LabelColumn"/> value.</para>
        /// <para>The label delegate must take a <see cref="SharpMap.Data.FeatureDataRow"/> and return a string.</para>
        /// <example>
        /// Creating a label-text by combining attributes "ROADNAME" and "STATE" into one string, using
        /// an anonymous delegate:
        /// <code lang="C#">
        /// myLabelLayer.LabelStringDelegate = delegate(SharpMap.Data.FeatureDataRow fdr)
        ///				{ return fdr["ROADNAME"].ToString() + ", " + fdr["STATE"].ToString(); };
        /// </code>
        /// </example>
        /// </remarks>
        GetLabelMethod LabelStringDelegate { get; set; }

        /// <summary>
        /// Data column from where the label rotation is derived.
        /// If this is empty, rotation will be zero, or aligned to a linestring.
        /// Rotation are in degrees (positive = clockwise).
        /// </summary>
        string RotationColumn { get; set; }

        /// <summary>
        /// A value indication the priority of the label in cases of label-collision detection
        /// </summary>
        int Priority { get; set; }

        [Aggregation]
        ILayer Parent { get; set; }
    }
}