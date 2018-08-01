// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class FailureMechanismContextTreeNodeInfoTest
    {
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;
        private const int contextMenuRelevancyIndexWhenRelevant = 2;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNotNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism("name", "code");
            var failureMechanismContext = new FailureMechanismContext<IFailureMechanism>(failureMechanism, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(failureMechanismContext);

                // Assert
                Assert.AreEqual(failureMechanism.Name, text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismIcon, image);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_Always_ReturnsControlText()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                mocks.ReplayAll();

                var failureMechanism = new TestFailureMechanism("C", "C");
                var context = new FailureMechanismContext<IFailureMechanism>(failureMechanism, assessmentSection);

                // Call
                Color textColor = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), textColor);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnFoldersWithInputAndOutput()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var failureMechanism = new TestFailureMechanism("test", "C");
                var failureMechanismContext = mocks.Stub<FailureMechanismContext<IFailureMechanism>>(failureMechanism, assessmentSection);

                mocks.ReplayAll();

                // Call
                object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                Assert.AreEqual(2, children.Length);
                var inputFolder = (CategoryTreeFolder) children[0];
                Assert.AreEqual("Invoer", inputFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Input, inputFolder.Category);

                var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputFolder.Contents.ElementAt(0);
                Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
                Assert.AreSame(assessmentSection, failureMechanismSectionsContext.AssessmentSection);

                var inputComment = (Comment) inputFolder.Contents.ElementAt(1);
                Assert.AreSame(failureMechanism.InputComments, inputComment);

                var outputFolder = (CategoryTreeFolder) children[1];
                Assert.AreEqual("Oordeel", outputFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Output, outputFolder.Category);

                var outputComment = (Comment) outputFolder.Contents.ElementAt(0);
                Assert.AreSame(failureMechanism.OutputComments, outputComment);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(typeof(GrassCoverSlipOffInwardsFailureMechanismSectionResult),
            TestName = "ChildNodeObjects_FailureMechanismIsRelevant_OutputNodeAddedForResult(GrassCoverSlipOffInwardsFailureMechanismSectionResult)")]
        [TestCase(typeof(GrassCoverSlipOffOutwardsFailureMechanismSectionResult),
            TestName = "ChildNodeObjects_FailureMechanismIsRelevant_OutputNodeAddedForResult(GrassCoverSlipOffOutwardsFailureMechanismSectionResult)")]
        [TestCase(typeof(MicrostabilityFailureMechanismSectionResult),
            TestName = "ChildNodeObjects_FailureMechanismIsRelevant_OutputNodeAddedForResult(MicrostabilityFailureMechanismSectionResult)")]
        [TestCase(typeof(TechnicalInnovationFailureMechanismSectionResult),
            TestName = "ChildNodeObjects_FailureMechanismIsRelevant_OutputNodeAddedForResult(TechnicalInnovationFailureMechanismSectionResult)")]
        [TestCase(typeof(StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult),
            TestName = "ChildNodeObjects_FailureMechanismIsRelevant_OutputNodeAddedForResult(StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult)")]
        [TestCase(typeof(WaterPressureAsphaltCoverFailureMechanismSectionResult),
            TestName = "ChildNodeObjects_FailureMechanismIsRelevant_OutputNodeAddedForResult(WaterPressureAsphaltCoverFailureMechanismSectionResult)")]
        public void ChildNodeObjects_FailureMechanismIsRelevant_OutputNodeAddedForResult(Type t)
        {
            // Delegate actual test
            MethodInfo method = GetType().GetMethod(
                nameof(ChildNodeObjects_FailureMechanismIsRelevantWithSectionResults_OutputNodeAdded),
                BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericMethod = method.MakeGenericMethod(t);
            genericMethod.Invoke(this, null);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismNotRelevantComments()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var failureMechanism = new TestFailureMechanism("test", "C")
                {
                    IsRelevant = false
                };

                var failureMechanismContext = new FailureMechanismContext<IFailureMechanism>(failureMechanism, assessmentSection);

                // Call
                object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                Assert.AreEqual(1, children.Length);
                var comment = (Comment) children[0];
                Assert.AreSame(failureMechanism.NotRelevantComments, comment);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var failureMechanism = new TestFailureMechanism("A", "C");
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var context = new FailureMechanismContext<IFailureMechanism>(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, assessmentSection, treeView);
                }

                // Assert
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var failureMechanism = new TestFailureMechanism("A", "C")
                {
                    IsRelevant = false
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var context = new FailureMechanismContext<IFailureMechanism>(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, assessmentSection, treeView);
                }

                // Assert
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_IsRelevantEnabled()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanism = new TestFailureMechanism("A", "C");
                var context = new FailureMechanismContext<IFailureMechanism>(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    plugin.Gui = gui;

                    TreeNodeInfo info = GetInfo(plugin);

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                    {
                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenRelevant,
                                                                      "I&s relevant",
                                                                      "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                      RingtoetsCommonFormsResources.Checkbox_ticked);
                    }
                }

                // Assert
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevant()
        {
            // Setup
            var failureMechanismObserver = mocks.Stub<IObserver>();
            failureMechanismObserver.Expect(o => o.UpdateObserver());

            var failureMechanism = new TestFailureMechanism("A", "C")
            {
                IsRelevant = true
            };
            failureMechanism.Attach(failureMechanismObserver);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new FailureMechanismContext<IFailureMechanism>(failureMechanism, assessmentSection);

            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    plugin.Gui = gui;

                    TreeNodeInfo info = GetInfo(plugin);

                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
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
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevant()
        {
            // Setup
            var failureMechanismObserver = mocks.Stub<IObserver>();
            failureMechanismObserver.Expect(o => o.UpdateObserver());

            var failureMechanism = new TestFailureMechanism("A", "C")
            {
                IsRelevant = false
            };
            failureMechanism.Attach(failureMechanismObserver);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new FailureMechanismContext<IFailureMechanism>(failureMechanism, assessmentSection);

            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    plugin.Gui = gui;

                    TreeNodeInfo info = GetInfo(plugin);

                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
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

        /// <summary>
        /// Used in ChildNodeObjects_FailureMechanismIsRelevant_OutputNodeAddedForResult(Type)
        /// </summary>
        private void ChildNodeObjects_FailureMechanismIsRelevantWithSectionResults_OutputNodeAdded<T>() where T : FailureMechanismSectionResult
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var failureMechanism = mocks.StrictMultiMock<IHasSectionResults<T>>(typeof(IFailureMechanism));
                failureMechanism.Expect(fm => fm.IsRelevant).Return(true);
                failureMechanism.Expect(fm => fm.SectionResults).Return(new ObservableList<T>()).Repeat.Any();
                failureMechanism.Expect(fm => fm.InputComments).Return(new Comment());
                failureMechanism.Expect(fm => fm.OutputComments).Return(new Comment());
                var failureMechanismContext = mocks.Stub<FailureMechanismContext<IFailureMechanism>>(failureMechanism, assessmentSection);

                mocks.ReplayAll();

                // Call
                object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                var outputFolder = (CategoryTreeFolder) children[1];

                var failureMechanismResultsContext = (FailureMechanismSectionResultContext<T>) outputFolder.Contents.ElementAt(0);
                Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
                Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);
            }

            mocks.VerifyAll();
        }

        private TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(FailureMechanismContext<IFailureMechanism>));
        }
    }
}