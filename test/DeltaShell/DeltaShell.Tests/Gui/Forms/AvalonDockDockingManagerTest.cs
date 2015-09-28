using System.Windows.Forms;
using System.Windows.Forms.Integration;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DelftTools.Utils.Reflection;
using DeltaShell.Gui.Forms.ViewManager;
using DeltaShell.Tests.TestObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace DeltaShell.Tests.Gui.Forms
{
    [TestFixture]
    public class AvalonDockDockingManagerTest
    {
        [Test]
        public void ViewTextChangedResultsInTabNameChanged()
        {
            var mocks = new MockRepository();
            var dockManager = mocks.Stub<DockingManager>();
            var view = new TestView();

            var dock = new AvalonDockDockingManager(dockManager, new[] { ViewLocation.Document });
            dock.Add(view, ViewLocation.Document);

            var layout = TypeUtils.CallPrivateMethod<LayoutContent>(dock, "GetLayoutContent", view);

            Assert.AreEqual("", layout.Title);
            view.Text = "Test";

            Assert.AreEqual("Test", layout.Title);
        }

        [Test]
        public void LockingAndUnlockingViewSetsLockIcon()
        {
            var mocks = new MockRepository();
            var dockManager = mocks.Stub<DockingManager>();
            var view = new ReusableTestView();

            var dock = new AvalonDockDockingManager(dockManager, new[] { ViewLocation.Document });
            dock.Add(view, ViewLocation.Document);

            var layout = TypeUtils.CallPrivateMethod<LayoutContent>(dock, "GetLayoutContent", view);

            Assert.IsNull(layout.IconSource); //null because view doesn't have its own image
            view.Locked = true;
            Assert.IsNotNull(layout.IconSource); //(lock) image set
            view.Locked = false;
            Assert.IsNull(layout.IconSource);
        }

        [Test]
        public void SwitchingTabCausesOldTabsActiveControlToLoseFocusTools9109()
        {
            var view = new TestView();
            var view2 = new TestView();

            // create an avalon dock/tab with two views
            var dock = new AvalonDockDockingManager(new DockingManager(), new[] { ViewLocation.Document });
            dock.Add(view, ViewLocation.Document);
            dock.Add(view2, ViewLocation.Document);
            dock.ActivateView(view);

            // set a textbox active
            view.ActiveControl = view.Controls[0];

            // switch to other tab
            dock.ActivateView(view2);

            // assert the textbox is no longer active 
            Assert.IsNull(view.ActiveControl);
        }

        [Test]
        [Ignore("user interaction: switch to second tab")]
        [Category(TestCategory.WindowsForms)]
        public void SwitchingTabCausesDataBindingTools9109()
        {
            var view = new TestView();
            var view2 = new TestView();

            // create an avalon dock/tab with two views
            var dockingManager = new DockingManager();
            var dock = new AvalonDockDockingManager(dockingManager, new[] { ViewLocation.Document });
            var host = new ElementHost {Child = dockingManager};

            int validated = 0;

            WindowsFormsTestHelper.ShowModal(host, f =>
                {
                    dock.Add(view, ViewLocation.Document);
                    dock.Add(view2, ViewLocation.Document);
                    dock.ActivateView(view);
                    
                    var textBox = (TextBox) view.Controls[0];
                    textBox.Validated += (s, e) => { validated++; };

                    // set a textbox active
                    view.ActiveControl = textBox;
                    textBox.Focus();
                });
            Assert.AreEqual(1, validated);
        }
    }
} 