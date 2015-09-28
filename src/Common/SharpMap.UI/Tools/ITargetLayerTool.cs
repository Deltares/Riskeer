using System;
using SharpMap.Api.Layers;

namespace SharpMap.UI.Tools
{
    public interface  ITargetLayerTool
    {
        string LayerName { get; }
        Func<ILayer, bool> LayerFilter { set; }
    }
}