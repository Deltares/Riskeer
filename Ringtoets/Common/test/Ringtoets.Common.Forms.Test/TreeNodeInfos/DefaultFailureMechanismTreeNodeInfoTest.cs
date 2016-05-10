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