using DelftTools.TestUtils;
using DelftTools.Utils;
using DeltaShell.Plugins.CommonTools.Gui.Property;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Property
{
    [TestFixture]
    public class DocumentPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowProperties()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new TextDocumentProperties { Data = new TextDocument() });
        }
    }
}