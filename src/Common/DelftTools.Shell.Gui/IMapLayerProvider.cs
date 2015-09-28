using System.Collections.Generic;
using SharpMap.Api.Layers;

namespace DelftTools.Shell.Gui
{
    public interface IMapLayerProvider
    {
        /// <summary>
        /// Create a layer for the provided data
        /// </summary>
        /// <param name="data">Data to create a layer for</param>
        /// <param name="parentData">Parent data of the data</param>
        /// <returns>Layer for the data</returns>
        ILayer CreateLayer(object data, object parentData);

        /// <summary>
        /// Determines if a layer can be created for the data
        /// </summary>
        /// <param name="data">Data to create a layer for</param>
        /// <param name="parentData">Parent data of the data</param>
        /// <returns>Can a layer be created</returns>
        bool CanCreateLayerFor(object data, object parentData);
        
        /// <summary>
        /// Child objects for <paramref name="data"/>. Objects will be used to create child layers
        /// for the group layer (<paramref name="data"/>)
        /// </summary>
        /// <param name="data">Group layer data</param>
        /// <returns>Child objects for <paramref name="data"/></returns>
        IEnumerable<object> ChildLayerObjects(object data);
    }
}