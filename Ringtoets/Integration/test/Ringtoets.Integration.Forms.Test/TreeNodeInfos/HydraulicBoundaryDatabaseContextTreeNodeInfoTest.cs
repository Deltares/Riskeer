using System;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.HydraulicBoundary;
using Ringtoets.Integration.Forms.Properties;
using Ringtoets.Integration.Plugin;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private TreeNodeInfo info;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabase");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext));
        }

        [TearDown]
        public override void TearDown()
        {
            plugin.Dispose();
            base.TearDown();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(HydraulicBoundaryDatabaseContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            var name = "Hydraulische randvoorwaarden";
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            mocks.ReplayAll();

            var context = new HydraulicBoundaryDatabaseContext(hydraulicBoundaryDatabase, assessmentSectionMock);

            // Call
            var text = info.Text(context);

            // Assert
            Assert.AreEqual(name, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsGenericIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.GenericInputOutputIcon, image);
        }

        [Test]
        public void CanRenameNode_Always_ReturnsFalse()
        {
            // Call
            var renameAllowed = info.CanRename(null, null);

            // Assert
            Assert.IsFalse(renameAllowed);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var hydraulicBoundaryDatabaseMock = mocks.StrictMock<HydraulicBoundaryDatabase>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            var nodeData = new HydraulicBoundaryDatabaseContext(hydraulicBoundaryDatabaseMock, assessmentSectionMock);

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }

//        [Test]
//        [RequiresSTA]
//        public void GivenNoFilePathIsSet_WhenOpeningFileFromContextMenu_ThenPathWillBeSetAndNotifiesObserverAndLogMessageAdded()
//        {
//            // Given
//            var testFile = Path.Combine(testDataPath, "empty.sqlite");
//            var guiMock = mocks.StrictMock<IGui>();
//            var mainWindow = mocks.Stub<IMainWindow>();
//            var observer = mocks.StrictMock<IObserver>();
//
//            using (var treeViewControl = new TreeViewControl())
//            {
//                var hydraulicBoundaryDatabaseMock = mocks.StrictMock<HydraulicBoundaryDatabase>();
//                var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
//
//                var nodeData = new HydraulicBoundaryDatabaseContext(hydraulicBoundaryDatabaseMock, assessmentSectionMock);
//                nodeData.Attach(observer);
//
//                observer.Expect(o => o.UpdateObserver());
//
//                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
//                guiMock.Expect(g => g.MainWindow).Return(mainWindow);
//
//                var openFolderContextMenuItemIndex = 1;
//
//                mocks.ReplayAll();
//
//                ModalFormHandler = (name, wnd, form) =>
//                {
//                    var tester = new OpenFileDialogTester(wnd);
//                    tester.OpenFile(testFile);
//                };
//
//                plugin.Gui = guiMock;
//
//                var contextMenuAdapter = info.ContextMenuStrip(nodeData, null, treeViewControl);
//
//                // When
//                Action action = () => { contextMenuAdapter.Items[openFolderContextMenuItemIndex].PerformClick(); };
//
//                // Then
//                TestHelper.AssertLogMessageIsGenerated(action, string.Format("Database op pad {0} gekoppeld", testFile));
//                mocks.VerifyAll();
//            }
//        }
    }
}
