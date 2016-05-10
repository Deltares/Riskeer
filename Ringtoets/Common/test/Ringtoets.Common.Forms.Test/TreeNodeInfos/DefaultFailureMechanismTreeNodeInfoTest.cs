using System.Drawing;
using System.Windows.Forms;

using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class DefaultFailureMechanismTreeNodeInfoTest
    {
        private const int contextMenuRelevancyIndex = 0;


        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismComments()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = false;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            var treeNodeInfo = new DefaultFailureMechanismTreeNodeInfo<SimpleFailureMechanismContext, IFailureMechanism>(null, null, null);

            // Call
            object[] children = treeNodeInfo.ChildNodeObjects(context);

            // Assert
            Assert.AreEqual(1, children.Length);
            var commentContext = (CommentContext<ICommentable>)children[0];
            Assert.AreSame(failureMechanism, commentContext.CommentContainer);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnResultFromConstructorMethod()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = true;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            var result = new[]
            {
                new object(),
                1.1
            };

            var treeNodeInfo = new DefaultFailureMechanismTreeNodeInfo<SimpleFailureMechanismContext, IFailureMechanism>(
                mechanismContext => result, null, null);

            // Call
            object[] children = treeNodeInfo.ChildNodeObjects(context);

            // Assert
            CollectionAssert.AreEqual(result, children);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevantButMethodIsNull_ReturnEmptyArray()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = true;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            var treeNodeInfo = new DefaultFailureMechanismTreeNodeInfo<SimpleFailureMechanismContext, IFailureMechanism>(null, null, null);

            // Call
            object[] children = treeNodeInfo.ChildNodeObjects(context);

            // Assert
            CollectionAssert.IsEmpty(children);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_ReturnStripWithOnlyRelevancyExpandAndCollapseAllNodes()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            using (var contextMenuStrip = new ContextMenuStrip())
            {
                var mocks = new MockRepository();
                var failureMechanism = mocks.Stub<IFailureMechanism>();
                failureMechanism.IsRelevant = false;

                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

                var builder = mocks.Stub<IContextMenuBuilder>();
                builder.Expect(b => b.AddCustomItem(null)).IgnoreArguments().Return(builder);
                builder.Expect(b => b.AddSeparator()).Return(builder);
                builder.Expect(b => b.AddExpandAllItem()).Return(builder);
                builder.Expect(b => b.AddCollapseAllItem()).Return(builder);
                builder.Expect(b => b.Build()).Return(contextMenuStrip);

                var provider = mocks.Stub<IContextMenuBuilderProvider>();
                provider.Expect(p => p.Get(context, treeView)).Return(builder);
                mocks.ReplayAll();

                var treeNodeInfo = new DefaultFailureMechanismTreeNodeInfo<SimpleFailureMechanismContext, IFailureMechanism>(
                    null, null, provider);

                // Call
                ContextMenuStrip result = treeNodeInfo.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                Assert.AreSame(contextMenuStrip, result);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_ReturnResultFromConstructorMethod()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            using (var contextMenuStrip = new ContextMenuStrip())
            {
                var mocks = new MockRepository();
                var failureMechanism = mocks.Stub<IFailureMechanism>();
                failureMechanism.IsRelevant = true;

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                mocks.ReplayAll();

                var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

                var treeNodeInfo = new DefaultFailureMechanismTreeNodeInfo<SimpleFailureMechanismContext, IFailureMechanism>(
                    null, (mechanismContext, parent, treeViewControl) =>
                    {
                        Assert.AreEqual(context, mechanismContext);
                        Assert.AreEqual(assessmentSection, parent);
                        Assert.AreEqual(treeView, treeViewControl);

                        return contextMenuStrip;
                    }, null);

                // Call
                ContextMenuStrip result = treeNodeInfo.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                Assert.AreSame(contextMenuStrip, result);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevantButMethodIsNull_ReturnStripWithOnlyRelevancyExpandAndCollapseAllNodes()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            using (var contextMenuStrip = new ContextMenuStrip())
            {
                var mocks = new MockRepository();
                var failureMechanism = mocks.Stub<IFailureMechanism>();
                failureMechanism.IsRelevant = true;

                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

                var builder = mocks.Stub<IContextMenuBuilder>();
                builder.Expect(b => b.AddCustomItem(null)).IgnoreArguments().Return(builder);
                builder.Expect(b => b.AddSeparator()).Return(builder);
                builder.Expect(b => b.AddExpandAllItem()).Return(builder);
                builder.Expect(b => b.AddCollapseAllItem()).Return(builder);
                builder.Expect(b => b.Build()).Return(contextMenuStrip);

                var provider = mocks.Stub<IContextMenuBuilderProvider>();
                provider.Expect(p => p.Get(context, treeView)).Return(builder);
                mocks.ReplayAll();

                var treeNodeInfo = new DefaultFailureMechanismTreeNodeInfo<SimpleFailureMechanismContext, IFailureMechanism>(
                    null, null, provider);

                // Call
                ContextMenuStrip result = treeNodeInfo.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                Assert.AreSame(contextMenuStrip, result);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismNotRelevant_ReturnsContextMenuWithCommonItems()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = false;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
                var viewCommandsHandler = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         exportImportHandler, viewCommandsHandler, 
                                                         context, treeViewControl);

                var provider = mocks.Stub<IContextMenuBuilderProvider>();
                provider.Stub(p => p.Get(context, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                var treeNodeInfo = new DefaultFailureMechanismTreeNodeInfo<SimpleFailureMechanismContext, IFailureMechanism>(
                    null, null, provider);

                // Call
                var menu = treeNodeInfo.ContextMenuStrip(context, null, treeViewControl);

                // Assert 
                Assert.AreEqual(4, menu.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(menu, 0, RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant, RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip, RingtoetsCommonFormsResources.Checkbox_empty);

                TestHelper.AssertContextMenuStripContainsItem(menu, 2, CoreCommonGuiResources.Expand_all, CoreCommonGuiResources.Expand_all_ToolTip, CoreCommonGuiResources.ExpandAllIcon, false);
                TestHelper.AssertContextMenuStripContainsItem(menu, 3, CoreCommonGuiResources.Collapse_all, CoreCommonGuiResources.Collapse_all_ToolTip, CoreCommonGuiResources.CollapseAllIcon, false);

                CollectionAssert.AllItemsAreInstancesOfType(new[]
                {
                    menu.Items[1],
                }, typeof(ToolStripSeparator));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnIsRelevantItem_MakeFailureMechanismRelevant()
        {
            // Setup
            var mocks = new MockRepository();
            
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.NotifyObservers());
            failureMechanism.IsRelevant = false;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var provider = mocks.Stub<IContextMenuBuilderProvider>();
                provider.Stub(p => p.Get(context, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                var treeNodeInfo = new DefaultFailureMechanismTreeNodeInfo<SimpleFailureMechanismContext, IFailureMechanism>(
                    null, null, provider);

                var contextMenu = treeNodeInfo.ContextMenuStrip(context, null, treeViewControl);

                // Call
                contextMenu.Items[contextMenuRelevancyIndex].PerformClick();

                // Assert
                Assert.IsTrue(failureMechanism.IsRelevant);
            }
            mocks.VerifyAll();
        }

        private class SimpleFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public SimpleFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) : base(wrappedFailureMechanism, parent) {}
        }
    }
}