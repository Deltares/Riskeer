using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Tests.TestObjects;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Common.Tests.Gui.Forms
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

            var dock = new AvalonDockDockingManager(dockManager, new[]
            {
                ViewLocation.Document
            });
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

            var dock = new AvalonDockDockingManager(dockManager, new[]
            {
                ViewLocation.Document
            });
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
            var dock = new AvalonDockDockingManager(new DockingManager(), new[]
            {
                ViewLocation.Document
            });
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
    }
}