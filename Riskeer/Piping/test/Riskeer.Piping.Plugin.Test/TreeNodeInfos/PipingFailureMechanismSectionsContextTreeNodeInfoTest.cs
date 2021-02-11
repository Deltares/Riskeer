// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingFailureMechanismSectionsContextTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new PipingPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNotNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CheckedState);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            using (var plugin = new PipingPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("Vakindeling", text);
            }
        }

        [Test]
        public void Image_Always_ReturnSectionsIcon()
        {
            // Setup
            using (var plugin = new PipingPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddImportItem()).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddUpdateItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                using (var plugin = new PipingPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_WithAddImportItem_CorrectImportInfosPassed()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddImportItem())
                       .IgnoreArguments()
                       .WhenCalled(invocation =>
                       {
                           var importInfos = (ImportInfo[]) invocation.Arguments[0];
                           Assert.AreEqual(1, importInfos.Length);
                           Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(PipingFailureMechanismSectionsContext)));
                       })
                       .Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddUpdateItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.Build()).Return(null);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                using (var plugin = new PipingPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_NoSectionsOnFailureMechanism_ReturnGrayText()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_HasSectionsOnFailureMechanism_ReturnControlText()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                new FailureMechanismSection("A", new[]
                {
                    new Point2D(3, 4),
                    new Point2D(5, 6)
                })
            }, "B");

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }

            mocks.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(PipingPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingFailureMechanismSectionsContext));
        }
    }
}