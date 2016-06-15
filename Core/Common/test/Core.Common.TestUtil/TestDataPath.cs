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
                public static class Gis
                {
                    public static readonly TestDataPath IO = System.IO.Path.Combine("Core", "Components", "test", "Core.Components.Gis.IO.Test");
                }
            }
        }

        public static class Ringtoets
        {
            public static class Common
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "Common", "test", "Ringtoets.Common.IO.Test");
            }

            public static class HydraRing
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "HydraRing", "test", "Ringtoets.HydraRing.IO.Test");
                public static readonly TestDataPath Calculation = System.IO.Path.Combine("Ringtoets", "HydraRing", "test", "Ringtoets.HydraRing.Calculation.Test");
            }

            public static class Integration
            {
                public static readonly TestDataPath Forms = System.IO.Path.Combine("Ringtoets", "Integration", "test", "Ringtoets.Integration.Forms.Test");
                public static readonly TestDataPath Service = System.IO.Path.Combine("Ringtoets", "Integration", "test", "Ringtoets.Integration.Service.Test");
            }

            public static class GrassCoverErosionInwards
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "GrassCoverErosionInwards", "test", "Ringtoets.GrassCoverErosionInwards.IO.Test");
            }

            public static class Piping
            {
                public static readonly TestDataPath IO = System.IO.Path.Combine("Ringtoets", "Piping", "test", "Ringtoets.Piping.IO.Test");
                public static readonly TestDataPath Plugin = System.IO.Path.Combine("Ringtoets", "Piping", "test", "Ringtoets.Piping.Plugin.Test");
            }
        }
    }
}