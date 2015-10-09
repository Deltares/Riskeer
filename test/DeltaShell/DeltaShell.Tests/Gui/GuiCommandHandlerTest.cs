using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;
using DeltaShell.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using SharpTestsEx;
using MessageBox = DelftTools.Controls.Swf.MessageBox;

namespace DeltaShell.Tests.Gui
{
    [TestFixture]
    public class GuiCommandHandlerTest
    {
        private MockRepository mocks = new MockRepository();
        private Project project;
        private IGui gui;
        private IApplication application;
        private IViewList documentViewList;
        private IViewResolver viewResolver;
        private IViewList toolWindowViewList;

        [SetUp]
        public void SetUp()
        {
            project = new Project();
            application = mocks.Stub<IApplication>();
            var activityRunner = new ActivityRunner();

            application.Expect(a => a.Project).Return(project).Repeat.Any();
            application.Expect(a => a.ActivityRunner).Return(activityRunner).Repeat.Any();

            application.ProjectOpened += delegate { };
            LastCall.IgnoreArguments().Repeat.Any();

            application.ProjectClosing += delegate { };
            LastCall.IgnoreArguments().Repeat.Any();

            documentViewList = mocks.Stub<IViewList>();
            viewResolver = mocks.Stub<IViewResolver>();
            toolWindowViewList = mocks.Stub<IViewList>();

            gui = mocks.Stub<IGui>();
            gui.Application = application;
            gui.Expect(g => g.DocumentViews).Return(documentViewList).Repeat.Any();
            gui.Expect(g => g.ToolWindowViews).Return(toolWindowViewList).Repeat.Any();
            gui.Expect(g => g.DocumentViewsResolver).Return(viewResolver).Repeat.Any();

            //inject custom messagebnox
            var messageBox = mocks.Stub<IMessageBox>();
            messageBox.Expect(m => m.Show(null, null, MessageBoxButtons.OKCancel))
                      .Return(DialogResult.OK).Repeat.Any().IgnoreArguments();

            MessageBox.CustomMessageBox = messageBox;
        }

        [TearDown]
        public void TearDown()
        {
            MessageBox.CustomMessageBox = null; //be sure to reset the messagebox so that other integration tests will not fail (singleton :().
            mocks = new MockRepository();
        }

        [Test]
        public void ClosingProjectSetsGuiSelectionToNull()
        {
            var mainWindow = mocks.Stub<IMainWindow>();
            gui.Expect(g => g.MainWindow).Return(mainWindow);
            application.Expect(app => app.CloseProject());
            mocks.ReplayAll();

            var guiCommandHandler = new GuiCommandHandler(gui);
            guiCommandHandler.TryCloseWTIProject();

            gui.Selection
               .Should().Be.EqualTo(null);

            mocks.VerifyAll();
        }
    }
}