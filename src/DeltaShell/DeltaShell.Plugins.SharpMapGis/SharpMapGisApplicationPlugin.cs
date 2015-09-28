using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using DelftTools.Shell.Core;
using GisSharpBlog.NetTopologySuite.Features;
using Mono.Addins;
using SharpMap;
using SharpMap.Extensions.CoordinateSystems;

namespace DeltaShell.Plugins.SharpMapGis
{
    [Extension(typeof(IPlugin))]
    public class SharpMapGisApplicationPlugin: ApplicationPlugin
    {
        public SharpMapGisApplicationPlugin()
        {
            Map.CoordinateSystemFactory = new OgrCoordinateSystemFactory();
        }

        public override string Name
        {
            get { return "GIS"; }
        }

        public override string DisplayName
        {
            get { return "SharpMap GIS Plugin"; }
        }

        public override string Description
        {
            get { return Properties.Resources.SharpMapGisApplicationPlugin_Description; }
        }

        public override string Version
        {
            get { return GetType().Assembly.GetName().Version.ToString(); }
        }

        public override IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield return new DataItemInfo<Map>
                {
                    Name = "Map",
                    Category = "General",
                    Image = Properties.Resources.Map,
                    CreateData = owner => new Map { Name = "Map" }
                };
        }

        public override Image Image
        {
            get { return new Bitmap(32, 32); }
        }

        public override IEnumerable<Assembly> GetPersistentAssemblies()
        {
            yield return typeof(Feature).Assembly;
            yield return GetType().Assembly;
        }
    }
}