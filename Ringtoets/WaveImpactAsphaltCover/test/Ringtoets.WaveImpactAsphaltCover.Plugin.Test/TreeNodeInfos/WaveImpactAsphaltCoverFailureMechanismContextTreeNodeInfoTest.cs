// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
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
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismContextTreeNodeInfoTest
    {
        private const int contextMenuRelevancyIndexWhenRelevant = 2;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;
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
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.IsNull(info.EnsureVisibleOnCreate);
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
        }

        [Test]
        public void Text_Always_ReturnName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism,
                                                                                            assessmentSection);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                string nodeText = info.Text(failureMechanismContext);

                // Assert
                Assert.AreEqual(failureMechanism.Name, nodeText);
            }
        }

        [Test]
        public void ForeColor_FailureMechanismIsRelevant_ReturnControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                IsRelevant = true
            };
            var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism,
                                                                                            assessmentSection);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color foreColor = info.ForeColor(failureMechanismContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);
            }
        }

        [Test]
        public void ForeColor_FailureMechanismIsNotRelevant_ReturnGrayText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism,
                                                                                            assessmentSection);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color foreColor = info.ForeColor(failureMechanismContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), foreColor);
            }
        }

        [Test]
        public void Image_Always_ReturnFailureMechanismIcon()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism,
                                                                                            assessmentSection);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Image icon = info.Image(failureMechanismContext);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismIcon, icon);
            }
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                IsRelevant = true
            };
            var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism,
                                                                                            assessmentSection);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                Assert.AreEqual(3, children.Length);

                var inputsFolder = (CategoryTreeFolder) children[0];
                Assert.AreEqual("Invoer", inputsFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

                Assert.AreEqual(3, inputsFolder.Contents.Count);
                var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents[0];
                Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
                Assert.AreSame(assessmentSection, failureMechanismSectionsContext.ParentAssessmentSection);

                var foreshoreProfilesContext = (ForeshoreProfilesContext) inputsFolder.Contents[1];
                Assert.AreSame(failureMechanism.ForeshoreProfiles, foreshoreProfilesContext.WrappedData);
                Assert.AreSame(failureMechanism, foreshoreProfilesContext.ParentFailureMechanism);
                Assert.AreSame(assessmentSection, foreshoreProfilesContext.ParentAssessmentSection);

                var inputComment = (Comment) inputsFolder.Contents[2];
                Assert.AreSame(failureMechanism.InputComments, inputComment);

                var hydraulicBoundariesCalculationGroup = (WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext) children[1];
                Assert.AreSame(failureMechanism.WaveConditionsCalculationGroup, hydraulicBoundariesCalculationGroup.WrappedData);
                Assert.AreSame(failureMechanism, hydraulicBoundariesCalculationGroup.FailureMechanism);

                var outputsFolder = (CategoryTreeFolder) children[2];
                Assert.AreEqual("Oordeel", outputsFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
                Assert.AreEqual(2, outputsFolder.Contents.Count);

                var failureMechanismResultsContext = (FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>)
                                                     outputsFolder.Contents[0];
                Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
                Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);

                var outputComment = (Comment) outputsFolder.Contents[1];
                Assert.AreSame(failureMechanism.OutputComments, outputComment);
            }
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismNotRelevantComments()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism,
                                                                                            assessmentSection);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                Assert.AreEqual(1, children.Length);
                var comment = (Comment) children[0];
                Assert.AreSame(failureMechanism.NotRelevantComments, comment);
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
                {
                    IsRelevant = true
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism,
                                                                                                assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                using (var plugin = new WaveImpactAsphaltCoverPlugin())
                {
                    plugin.Gui = gui;

                    var info = GetInfo(plugin);

                    // Call
                    info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
                }
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            {
                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
                {
                    IsRelevant = false
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                using (var plugin = new WaveImpactAsphaltCoverPlugin())
                {
                    plugin.Gui = gui;

                    var info = GetInfo(plugin);

                    // Call
                    info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
                }
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevant()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanismObserver = mocks.Stub<IObserver>();
                failureMechanismObserver.Expect(o => o.UpdateObserver());

                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
                {
                    IsRelevant = true
                };
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism,
                                                                                                assessmentSection);

                var viewCommands = mocks.StrictMock<IViewCommands>();
                viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                using (var plugin = new WaveImpactAsphaltCoverPlugin())
                {
                    plugin.Gui = gui;

                    var info = GetInfo(plugin);

                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                    {
                        // Call
                        contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();

                        // Assert
                        Assert.IsFalse(failureMechanism.IsRelevant);
                    }
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevant()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanismObserver = mocks.Stub<IObserver>();
                failureMechanismObserver.Expect(o => o.UpdateObserver());

                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
                {
                    IsRelevant = false
                };
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism,
                                                                                                assessmentSection);

                var viewCommands = mocks.StrictMock<IViewCommands>();
                viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                using (var plugin = new WaveImpactAsphaltCoverPlugin())
                {
                    plugin.Gui = gui;

                    var info = GetInfo(plugin);

                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                    {
                        // Call
                        contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();

                        // Assert
                        Assert.IsTrue(failureMechanism.IsRelevant);
                    }
                }
            }
        }

        private static TreeNodeInfo GetInfo(WaveImpactAsphaltCoverPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(WaveImpactAsphaltCoverFailureMechanismContext));
        }
    }
}