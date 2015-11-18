using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;

namespace Core.GIS.SharpMap.Api.Layers
{
    public interface IGroupLayer : ILayer, INotifyCollectionChange
    {
        EventedList<ILayer> Layers { get; set; }

        /// <summary>
        /// Determines whether it is allowed to add or remove child layers in the grouplayer. Also the order of the layers is fixed.
        /// </summary>
        bool LayersReadOnly { get; set; }
    }
}