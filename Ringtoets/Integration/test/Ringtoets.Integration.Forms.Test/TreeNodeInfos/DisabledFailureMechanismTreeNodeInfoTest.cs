using System.Drawing;
using System.Linq;

using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;

using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class DisabledFailureMechanismTreeNodeInfoTest
    {
        //[Test]
        //public void Initialized_Always_ExpectedPropertiesSet()
        //{
        //    // Call
        //    using (var plugin = new RingtoetsGuiPlugin())
        //    {
        //        TreeNodeInfo info = GetDisabledFailureMechanismContextTreeNodeInfo(plugin);

        //        // Assert
        //        Assert.AreEqual(typeof(DisabledFailureMechanismContext), info.TagType);
        //        Assert.IsNull(info.EnsureVisibleOnCreate);
        //        Assert.IsNull(info.CanRename);
        //        Assert.IsNull(info.OnNodeRenamed);
        //        Assert.IsNull(info.CanRemove);
        //        Assert.IsNull(info.OnNodeRemoved);
        //        Assert.IsNull(info.CanCheck);
        //        Assert.IsNull(info.IsChecked);
        //        Assert.IsNull(info.OnNodeChecked);
        //        Assert.IsNull(info.CanDrag);
        //        Assert.IsNull(info.CanDrop);
        //        Assert.IsNull(info.CanInsert);
        //        Assert.IsNull(info.OnDrop);
        //    }
        //}

        //[Test]
        //public void Text_Always_ReturnsName()
        //{
        //    // Setup
        //    const string name = "cool name bro!";

        //    var mocks = new MockRepository();
        //    var failureMechanism = mocks.Stub<IFailureMechanism>();
        //    failureMechanism.Stub(fm => fm.Name).Return(name);
        //    mocks.ReplayAll();

        //    var context = new DisabledFailureMechanismContext(failureMechanism);

        //    using (var plugin = new RingtoetsGuiPlugin())
        //    {
        //        TreeNodeInfo info = GetDisabledFailureMechanismContextTreeNodeInfo(plugin);

        //        // Call
        //        string text = info.Text(context);

        //        // Assert
        //        Assert.AreEqual(name, text);
        //    }
        //    mocks.VerifyAll();
        //}

        //[Test]
        //public void Image_Always_ReturnFailureMechanismIcon()
        //{
        //    // Setup
        //    using (var plugin = new RingtoetsGuiPlugin())
        //    {
        //        TreeNodeInfo info = GetDisabledFailureMechanismContextTreeNodeInfo(plugin);

        //        // Call
        //        Image image = info.Image(null);

        //        // Assert
        //        TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.FailureMechanismIcon, image);
        //    }
        //}

        //[Test]
        //public void ForeColor_Always_ReturnsGrayText()
        //{
        //    // Setup
        //    using (var plugin = new RingtoetsGuiPlugin())
        //    {
        //        var info = GetDisabledFailureMechanismContextTreeNodeInfo(plugin);

        //        // Call
        //        var textColor = info.ForeColor(null);

        //        // Assert
        //        Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), textColor);
        //    }
        //}

        //[Test]
        //public void ChildNodeObjects_Always_ReturnFailureMechanismComments()
        //{
        //    // Setup
        //    var mocks = new MockRepository();
        //    var failureMechanism = mocks.Stub<IFailureMechanism>();
        //    failureMechanism.IsRelevant = false;
        //    mocks.ReplayAll();

        //    var context = new DisabledFailureMechanismContext(failureMechanism);

        //    using (var plugin = new RingtoetsGuiPlugin())
        //    {
        //        TreeNodeInfo info = GetDisabledFailureMechanismContextTreeNodeInfo(plugin);

        //        // Call
        //        object[] children = info.ChildNodeObjects(context);

        //        // Assert
        //        Assert.AreEqual(1, children.Length);
        //        var commentContext = (CommentContext<ICommentable>)children[0];
        //        Assert.AreSame(failureMechanism, commentContext.CommentContainer);
        //    }
        //    mocks.VerifyAll();
        //}

        //[Test]
        //public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        //{
        //    // Setup
        //    using (var treeViewControl = new TreeViewControl())
        //    {
        //        var mocks = new MockRepository();
        //        var failureMechanism = mocks.Stub<IFailureMechanism>();
        //        var context = new DisabledFailureMechanismContext(failureMechanism);

        //        var menuBuilder = mocks.Stub<IContextMenuBuilder>();
        //        menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
        //        menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
        //        menuBuilder.Expect(mb => mb.Build()).Return(null);

        //        var gui = mocks.Stub<IGui>();
        //        gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
        //        gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
        //        gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

        //        mocks.ReplayAll();

        //        using (var plugin = new RingtoetsGuiPlugin
        //        {
        //            Gui = gui
        //        })
        //        {
        //            TreeNodeInfo info = GetDisabledFailureMechanismContextTreeNodeInfo(plugin);

        //            // Call
        //            info.ContextMenuStrip(context, null, treeViewControl);
        //        }

        //        // Assert
        //        mocks.VerifyAll();
        //    }
        //}

        //private static TreeNodeInfo GetDisabledFailureMechanismContextTreeNodeInfo(RingtoetsGuiPlugin guiPlugin)
        //{
        //    return guiPlugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DisabledFailureMechanismContext));
        //}
    }
}