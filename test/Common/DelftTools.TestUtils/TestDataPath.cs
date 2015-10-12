namespace DelftTools.TestUtils
{
    /// <summary>
    /// Used to make paths strongly typed, see <see cref="TestHelper.GetTestDataPath(TestDataPath)"/>
    /// </summary>
    public class TestDataPath
    {
        public string Path { get; private set; }

        public static implicit operator TestDataPath(string path)
        {
            return new TestDataPath
            {
                Path = path
            };
        }

        public static class Common
        {
            public static readonly TestDataPath DelftToolsUtilsTests =
                System.IO.Path.Combine("Common", "DelftTools.Utils.Tests");

            public static class DelftTools
            {
                public static readonly TestDataPath DelftToolsTests = @"Common/DelftTools.Tests";

                public static readonly TestDataPath SharpMapTests = @"Common/SharpMap.Tests";

                public static readonly TestDataPath DelftToolsTestsUtilsXmlSerialization =
                    @"Common/DelftTools.Tests/Utils/Xml/Serialization";
            }
        }

        public static class DeltaShell
        {
            public static readonly TestDataPath DeltaShellDeltaShellPluginsSharpMapGisTests =
                @"DeltaShell/DeltaShell.Plugins.SharpMapGis.Tests/";

            public static readonly TestDataPath DeltaShellDeltaShellPluginsSharpMapGisTestsRasterData =
                @"DeltaShell/DeltaShell.Plugins.SharpMapGis.Tests/RasterData/";

            public static readonly TestDataPath DeltaShellDeltaShellIntegrationTests =
                @"DeltaShell/DeltaShell.IntegrationTests/";

            public static readonly TestDataPath DeltaShellDeltaShellIntegrationTestsNetCdf =
                @"DeltaShell/DeltaShell.IntegrationTests/NetCdf/";

            public static readonly TestDataPath DeltaShellDeltaShellIntegrationTestsGDAL =
                @"DeltaShell/DeltaShell.IntegrationTests/GDAL/";
        }

        public static class NetCdfData
        {
            public static readonly TestDataPath NetCdfDataPath = @"netCdfData";
        }

        public static class Plugins {}

        public static class RasterData
        {
            public static readonly TestDataPath RasterDataPath = @"rasterData";
        }

        public static class VectorData
        {
            public static readonly TestDataPath VectorDataPath = @"vectorData";
        }
    }
}