using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Utils.Reflection;

using NUnit.Framework;

using Rhino.Mocks;

using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    [TestFixture]
    public class AvalonDockDockingManagerTest
    {
        [Test]
        [RequiresSTA]
        public void GivenViewDockedAsDocument_WhenViewTextIsChanged_ThenViewContainersTitleChangedToNewViewText()
        {
            // Setup
            var mocks = new MockRepository();
            var dockManager = mocks.Stub<DockingManager>();
            var view = new TestView();

            var dock = new AvalonDockDockingManager(dockManager, new[]
            {
                ViewLocation.Document
            });
            dock.Add(view, ViewLocation.Document);

            var layout = TypeUtils.CallPrivateMethod<LayoutContent>(dock, "GetLayoutContent", view);

            // Precondition
            Assert.AreEqual("", layout.Title);

            // Call
            view.Text = @"Test";

            // Assert
            Assert.AreEqual("Test", layout.Title);
        }

        [Test]
        [RequiresSTA]
        public void ActivateView_ChangingActiveViewToDifferentDockedView_OldActiveViewActiveControlIsNull()
        {
            // Setup
            var view = new TestView();
            var view2 = new TestView();

            var dock = new AvalonDockDockingManager(new DockingManager(), new[]
            {
                ViewLocation.Document
            });
            dock.Add(view, ViewLocation.Document);
            dock.Add(view2, ViewLocation.Document);
            dock.ActivateView(view);

            view.ActiveControl = view.Controls[0];

            // Call
            dock.ActivateView(view2);

            // Assert
            Assert.IsNull(view.ActiveControl);
        }
    }
}