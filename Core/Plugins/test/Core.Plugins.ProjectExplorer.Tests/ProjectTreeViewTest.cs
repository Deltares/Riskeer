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
        private static readonly MockRepository mocks = new MockRepository();

        [SetUp]
        public void SetUp() {}

        [Test]
        public void Init()
        {
            var gui = mocks.Stub<IGui>();
            var app = mocks.StrictMock<IApplication>();
            var settings = mocks.Stub<ApplicationSettingsBase>();

            var documentViews = mocks.Stub<IViewList>();
            gui.Application = app;
            Expect.Call(gui.DocumentViews).Return(documentViews).Repeat.Any();

            // in case of mock
            Expect.Call(app.UserSettings).Return(settings).Repeat.Any();
            mocks.ReplayAll();

            var pluginGui = new ProjectExplorerGuiPlugin
            {
                Gui = gui
            };

            var projectTreeView = new ProjectTreeView(pluginGui);
            Assert.IsNotNull(projectTreeView);
        }
    }
}