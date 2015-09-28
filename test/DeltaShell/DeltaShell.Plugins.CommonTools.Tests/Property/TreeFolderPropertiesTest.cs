using DelftTools.Shell.Gui.Swf;
using DelftTools.TestUtils;
using DeltaShell.Plugins.CommonTools.Gui.Property;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Property
{
    [TestFixture]
    public class TreeFolderPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowProperties()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new TreeFolderProperties { Data = new TreeFolder(null, "name", FolderImageType.Input) });
        }
    }
}