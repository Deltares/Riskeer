using System.Configuration;
using Core.Common.Base;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Tests
{
    [TestFixture]
    public class ProjectTreeViewTest
    {
       [Test]
        public void Init()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var documentViews = mocks.Stub<IViewList>();
            var settings = mocks.Stub<ApplicationSettingsBase>();

            Expect.Call(gui.UserSettings).Return(settings).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(documentViews).Repeat.Any();

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();

            gui.ApplicationCore = applicationCore;

            var pluginGui = new ProjectExplorerGuiPlugin
            {
                Gui = gui
            };

            var projectTreeView = new ProjectTreeView(pluginGui);

            Assert.IsNotNull(projectTreeView);

            mocks.VerifyAll();
        }
    }
}