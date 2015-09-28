using System.Drawing;
using SharpMap.Api.Layers;

namespace SharpMap.Api.Delegates
{
    /// <summary>
    /// EventHandler for event fired when all layers have been rendered
    /// </summary>
    public delegate void MapLayerRenderedEventHandler(Graphics g, ILayer layer);
}