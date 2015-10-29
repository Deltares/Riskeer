using System.Drawing;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.Api.Delegates
{
    /// <summary>
    /// EventHandler for event fired when all layers have been rendered
    /// </summary>
    public delegate void MapLayerRenderedEventHandler(Graphics g, ILayer layer);
}