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
        }

        public static class Core
        {
            public static class Common
            {
                public static readonly TestDataPath Base = System.IO.Path.Combine("Core", "Common", "test", "Core.Common.Base.Test");
                public static readonly TestDataPath Utils = System.IO.Path.Combine("Core", "Common", "test", "Core.Common.Utils.Test");
                public static readonly TestDataPath IO = System.IO.Path.Combine("Core", "Common", "test", "Core.Common.IO.Test");
            }

            public static class Components
            {
                public static readonly TestDataPath DotSpatial = System.IO.Path.Combine("Core", "Components", "test", "Core.Components.DotSpatial.Test");

                public static class Gis
                {
                    public static readonly TestDataPath IO = System.IO.Path.Combine("Core", "Components", "test", "Core.Components.Gis.IO.Test");
                }
            }
            
            public static class Plugins
            {
                public static readonly TestDataPath DotSpatial = System.IO.Path.Combine("Core", "Plugins", "test", "Core.Plugins.DotSpatial.Test");
            }

        }

        public static class Ringtoets
        {
            public static class Piping
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "Piping", "test", "Ringtoets.Piping.IO.Test");
            }

            public static class Common
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "Common", "test", "Ringtoets.Common.IO.Test");
            }

            public static class Integration
            {
                public static readonly TestDataPath Forms = System.IO.Path.Combine("Ringtoets", "Integration", "test", "Ringtoets.Integration.Forms.Test");
            }
        }
    }
}