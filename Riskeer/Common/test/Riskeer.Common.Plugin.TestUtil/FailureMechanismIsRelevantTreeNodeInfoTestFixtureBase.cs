using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Plugin.TestUtil
{
    /// <summary>
    /// Test fixture for verifying the IsRelevant behaviour of a TreeNodeInfo.
    /// </summary>
    /// <typeparam name="TPlugin">The type of plugin to create the tests for.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of <see cref="FailureMechanismBase"/> to create the tests for.</typeparam>
    /// <typeparam name="TFailureMechanismContext">The type of <see cref="FailureMechanismContext{T}"/> to create the tests for.</typeparam>
    [TestFixture]
    public abstract class FailureMechanismIsRelevantTreeNodeInfoTestFixtureBase<TPlugin, TFailureMechanism, TFailureMechanismContext>
        where TPlugin : PluginBase, new()
        where TFailureMechanism : FailureMechanismBase, new()
        where TFailureMechanismContext : FailureMechanismContext<TFailureMechanism>
    {
        private readonly int contextMenuRelevancyIndexWhenNotRelevant;
        private readonly int contextMenuRelevancyIndexWhenRelevant;

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            var mocks = new MockRepository();

            var failureMechanism = new TFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            TFailureMechanismContext context = CreateFailureMechanismContext(failureMechanism, assessmentSection);
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(context));

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new TPlugin
                {
                    Gui = gui
                })
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Call
                        contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();

                        // Assert
                        Assert.IsFalse(failureMechanism.IsRelevant);
                    }
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new TFailureMechanism
            {
                IsRelevant = false
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            TFailureMechanismContext context = CreateFailureMechanismContext(failureMechanism, assessmentSection);
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(context));

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new TPlugin
                {
                    Gui = gui
                })
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Call
                        contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();

                        // Assert
                        Assert.IsTrue(failureMechanism.IsRelevant);
                    }
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_AddCustomItems()
        {
            // Setup
            var mocks = new MockRepository();
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanism = new TFailureMechanism();
                TFailureMechanismContext context = CreateFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new TPlugin
                {
                    Gui = gui
                })
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                    {
                        // Assert
                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenRelevant,
                                                                      "I&s relevant",
                                                                      "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                      RiskeerCommonFormsResources.Checkbox_ticked);
                    }
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_AddCustomItems()
        {
            // Setup
            var mocks = new MockRepository();
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanism = new TFailureMechanism
                {
                    IsRelevant = false
                };

                TFailureMechanismContext context = CreateFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new TPlugin
                {
                    Gui = gui
                })
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                    {
                        // Assert
                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenNotRelevant,
                                                                      "I&s relevant",
                                                                      "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                      RiskeerCommonFormsResources.Checkbox_empty);
                    }
                }
            }

            mocks.VerifyAll();
        }

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismIsRelevantTreeNodeInfoTestFixtureBase{TPlugin,TFailureMechanism,TFailureMechanismContext}"/>.
        /// </summary>
        /// <param name="contextMenuRelevancyIndexWhenRelevant">The index of the IsRelevant context menu item when the <typeparamref name="TFailureMechanism"/>
        /// is relevant.</param>
        /// <param name="contextMenuRelevancyIndexWhenNotRelevant">The index of the IsRelevant context menu item when the <typeparamref name="TFailureMechanism"/>
        /// is not relevant.</param>
        protected FailureMechanismIsRelevantTreeNodeInfoTestFixtureBase(int contextMenuRelevancyIndexWhenRelevant,
                                                                        int contextMenuRelevancyIndexWhenNotRelevant)
        {
            this.contextMenuRelevancyIndexWhenRelevant = contextMenuRelevancyIndexWhenRelevant;
            this.contextMenuRelevancyIndexWhenNotRelevant = contextMenuRelevancyIndexWhenNotRelevant;
        }

        protected abstract TFailureMechanismContext CreateFailureMechanismContext(TFailureMechanism failureMechanism, IAssessmentSection assessmentSection);

        private static TreeNodeInfo GetInfo(TPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(TFailureMechanismContext));
        }
    }
}