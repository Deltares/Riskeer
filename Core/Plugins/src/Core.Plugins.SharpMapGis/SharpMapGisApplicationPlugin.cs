using System.Collections.Generic;
using System.Drawing;
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

        public override string Name
        {
            get
            {
                return "GIS";
            }
        }

        public override string DisplayName
        {
            get
            {
                return "SharpMap GIS Plugin";
            }
        }

        public override string Description
        {
            get
            {
                return Properties.Resources.SharpMapGisApplicationPlugin_Description;
            }
        }

        public override string Version
        {
            get
            {
                return GetType().Assembly.GetName().Version.ToString();
            }
        }

        public override IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            //            yield return new DataItemInfo<Map>
            //            {
            //                Name = "Map",
            //                Category = "General",
            //                Image = Properties.Resources.Map,
            //                CreateData = owner => new Map
            //                {
            //                    Name = "Map"
            //                }
            //            };

            yield break;
        }
    }
}