using System.Windows.Forms;

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Selection;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class ProjectCommandHandlerTest
    {
        [Test]
        public void AddItemToProject_AddToProjectItemsAndNotifyObservers()
        {
            // Setup
            var project = new Project();
            var childData = new object();

            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = project;

            var dialogParent = mocks.Stub<IWin32Window>();
            var applicationCore = mocks.Stub<ApplicationCore>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var documentViewController = mocks.Stub<IDocumentViewController>();

            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            project.Attach(observer);

            var commandHandler = new ProjectCommandHandler(projectOwner, dialogParent, applicationCore, applicationSelection, documentViewController);

            // Call
            commandHandler.AddItemToProject(childData);

            // Assert
            CollectionAssert.Contains(project.Items, childData);
            mocks.VerifyAll();
        }
    }
}