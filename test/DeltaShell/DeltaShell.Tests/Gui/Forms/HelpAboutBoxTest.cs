using DelftTools.Shell.Core;
using DelftTools.TestUtils;
using DeltaShell.Gui.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace DeltaShell.Tests.Gui.Forms
{
    [TestFixture]
    public class HelpAboutBoxTest
    {
        readonly MockRepository mocks = new MockRepository();

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void Show()
        {
            var helpAboutBox = new HelpAboutBox();
            var data = new HelpAboutBoxData();
            var plugin = mocks.Stub<IPlugin>();
            data.ProductName = "DShell";
            data.Version = "1.1";
            data.Description= "Modeling framework";
            data.Copyright = "C 2011";
            data.Plugins = new[] { plugin };

            using (mocks.Record())
            {
                Expect.Call(plugin.DisplayName).Return("My Plugin");
                Expect.Call(plugin.Version).Return("1.0");
                Expect.Call(plugin.Description).Return("Does add stuff");
            }

            using (mocks.Playback())
            {
                helpAboutBox.UpdateAboutBox(data);        
            }

        

            WindowsFormsTestHelper.ShowModal(helpAboutBox);
 
        }
    }
}