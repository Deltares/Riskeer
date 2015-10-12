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
        }
    }
}