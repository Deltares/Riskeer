using System.Collections.Generic;
using Core.Common.Base;
using Core.GIS.SharpMap.Extensions.CoordinateSystems;
using Core.GIS.SharpMap.Map;

namespace Core.Plugins.SharpMapGis
{
    public class SharpMapGisApplicationPlugin : ApplicationPlugin
    {
        public SharpMapGisApplicationPlugin()
        {
            Map.CoordinateSystemFactory = new OgrCoordinateSystemFactory();
        }

        public override IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield return new DataItemInfo<Map>
            {
                Name = "Map",
                Category = "General",
                Image = Properties.Resources.Map,
                CreateData = owner => new Map
                {
                    Name = "Map"
                }
            };

            yield break;
        }
    }
}