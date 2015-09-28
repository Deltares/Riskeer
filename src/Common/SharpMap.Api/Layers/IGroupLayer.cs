using System;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;

namespace SharpMap.Api.Layers
{
    public interface IGroupLayer : ILayer, INotifyCollectionChange
    {
        IEventedList<ILayer> Layers { get; set; }

        /// <summary>
        /// Determines whether it is allowed to add or remove child layers in the grouplayer. Also the order of the layers is fixed.
        /// </summary>
        bool LayersReadOnly { get; set; }
    }
}