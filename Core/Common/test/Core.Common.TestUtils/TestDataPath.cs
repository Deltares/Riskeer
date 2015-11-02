namespace Core.Common.TestUtils
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
                System.IO.Path.Combine("Core", "Common", "test", "Core.Common.Utils.Tests");

            public static class Base
            {
                public static readonly TestDataPath DelftToolsTests = @"Core/Common/test/Core.Common.Base.Tests";
            }
        }

        public static class DeltaShell
        {
            public static readonly TestDataPath DeltaShellDeltaShellPluginsSharpMapGisTests =
                @"Core/Plugins/test/Core.Plugins.SharpMapGis.Tests/";
        }

        public static class Ringtoets
        {
            public static class Piping
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "Piping", "test", "Ringtoets.Piping.IO.Test");
            }
        }
    }
}