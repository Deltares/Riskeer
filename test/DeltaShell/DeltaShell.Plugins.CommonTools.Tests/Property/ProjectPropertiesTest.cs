using DelftTools.Shell.Core;
using DelftTools.TestUtils;
using DeltaShell.Plugins.CommonTools.Gui.Property;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Property
{
    [TestFixture]
    public class ProjectPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowProperties()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new ProjectProperties { Data = new Project() });
        }
    }
}