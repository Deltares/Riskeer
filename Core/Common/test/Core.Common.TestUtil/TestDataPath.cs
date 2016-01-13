namespace Core.Common.TestUtil
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

        public static class Application
        {
            public static class Ringtoets
            {
                public static readonly TestDataPath Storage = System.IO.Path.Combine("Application", "Ringtoets", "test", "Application.Ringtoets.Storage.Test");
            }

            public static readonly TestDataPath CorePluginsSharpMapGisTests =
                @"Core/Plugins/test/Core.Plugins.SharpMapGis.Test/";
        }

        public static class Common
        {
            public static readonly TestDataPath CoreCommonUtilsTests =
                System.IO.Path.Combine("Core", "Common", "test", "Core.Common.Utils.Test");

            public static class Base
            {
                public static readonly TestDataPath CoreCommonBaseTests = @"Core/Common/test/Core.Common.Base.Test";
            }
        }

        public static class Ringtoets
        {
            public static class Piping
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "Piping", "test", "Ringtoets.Piping.IO.Test");
            }

            public static readonly TestDataPath CorePluginsSharpMapGisTests =
                @"Core/Plugins/test/Core.Plugins.SharpMapGis.Test/";
        }
    }
}