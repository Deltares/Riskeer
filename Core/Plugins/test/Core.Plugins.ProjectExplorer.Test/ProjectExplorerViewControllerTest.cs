using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Selection;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class ProjectExplorerViewControllerTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Constructor_ArgumentsNull_ThrowsArgumentNullException(int paramNullIndex)
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = paramNullIndex == 0 ? null : mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = paramNullIndex == 1 ? null : mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = paramNullIndex == 2 ? null : mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = paramNullIndex == 3 ? null : mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = paramNullIndex == 4 ? null : Enumerable.Empty<TreeNodeInfo>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoNullArguments_CreatesNewInstance()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);

            mocks.ReplayAll();

            // Call
            using (var result = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void OpenView_ViewActiveNoOnOpenView_NoProjectExplorerOpened()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (mocks.Ordered())
            {
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);
            }
            mocks.ReplayAll();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.OpenView();
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void OpenView_ViewInactiveNoOnOpenView_OpenToolViewWithProjectExplorer()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            applicationSelection.Expect(a => a.SelectionChanged += null).IgnoreArguments();
            toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false).Repeat.Twice();
            toolViewController.Expect(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Matches(v => true)));

            mocks.ReplayAll();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.OpenView();

            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void OpenView_ViewInactiveOnOpenViewSet_OpenToolViewWithProjectExplorerCallsOnOpenView()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            applicationSelection.Expect(a => a.SelectionChanged += null).IgnoreArguments();
            toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false).Repeat.Twice();
            toolViewController.Expect(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Matches(v => true)));

            mocks.ReplayAll();

            int activated = 0;
            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                controller.OnOpenView += (sender, args) => activated++;

                // Call
                controller.OpenView();

            }
            // Assert
            Assert.AreEqual(1, activated);
            mocks.VerifyAll();
        }

        [Test]
        public void ToggleView_ViewActive_ViewClosed()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (mocks.Ordered())
            {
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                toolViewController.Expect(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Matches(v => true)));
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);
            }

            mocks.ReplayAll();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.ToggleView();
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ToggleView_ViewInactive_ViewOpened()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (mocks.Ordered())
            {
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);
                applicationSelection.Expect(a => a.SelectionChanged += null).IgnoreArguments();
                toolViewController.Expect(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Matches(v => true)));
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                toolViewController.Expect(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Matches(v => true)));
            }

            mocks.ReplayAll();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.ToggleView();
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsViewActive_Always_CallsIsToolWindowOpenWithProjectExplorer(bool isOpen)
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(isOpen).Repeat.Twice();
            if (isOpen)
            {
                toolViewController.Expect(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Matches(v => true)));
            }

            mocks.ReplayAll();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                var result = controller.IsViewActive();

                Assert.AreEqual(isOpen, result);
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ViewIsOpen_UpdateData()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(Project)
                }
            };

            ProjectExplorer projectExplorer = null;

            using (mocks.Ordered())
            {
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);
                applicationSelection.Expect(a => a.SelectionChanged += null).IgnoreArguments();
                toolViewController.Expect(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Matches(v => true))).WhenCalled(m =>
                {
                    projectExplorer = (ProjectExplorer)m.Arguments[0];
                });
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                documentViewController.Expect(dvc => dvc.UpdateToolTips());
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                toolViewController.Expect(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Matches(v => true)));
            }
            mocks.ReplayAll();

            Project project = new Project();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                controller.OpenView();

                // Call
                controller.Update(project);

                // Assert
                Assert.AreSame(project, projectExplorer.Data);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ViewIsClosed_NoDataUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (mocks.Ordered())
            {
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false).Repeat.Twice();
            }
            mocks.ReplayAll();

            Project project = new Project();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.Update(project);
            }
            // Assert
            mocks.VerifyAll();
        }


        [Test]
        public void Dispose_ViewActive_ViewClosed()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (mocks.Ordered())
            {
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
                toolViewController.Expect(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Matches(v => true)));
                toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);
            }

            mocks.ReplayAll();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.Dispose();
            }
            // Assert
            mocks.VerifyAll();
        }
    }
}