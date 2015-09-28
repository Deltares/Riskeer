using DelftTools.Shell.Core.Workflow;
using DelftTools.TestUtils;
using DeltaShell.Plugins.CommonTools.Gui.Property;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Property
{
    [TestFixture]
    public class CompositeActivityPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowProperties()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new CompositeActivityProperties { Data = new ParallelActivity() });
        }
    }
}