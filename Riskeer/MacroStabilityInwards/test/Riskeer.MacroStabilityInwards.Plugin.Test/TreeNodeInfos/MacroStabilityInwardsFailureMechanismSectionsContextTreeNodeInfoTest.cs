// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionsContextTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
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
            using (var plugin = new MacroStabilityInwardsPlugin())
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
            using (var plugin = new MacroStabilityInwardsPlugin())
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
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);

            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            menuBuilder.AddOpenItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddImportItem(Arg.Any<ImportInfo[]>()).Returns(menuBuilder);
            menuBuilder.AddUpdateItem().Returns(menuBuilder);
            menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            var assessmentSection = Substitute.For<IAssessmentSection>();
            var context = new MacroStabilityInwardsFailureMechanismSectionsContext(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                using (var plugin = new MacroStabilityInwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, null, treeViewControl);
                }
            }

            // Assert
            Received.InOrder(() =>
            {
                menuBuilder.AddOpenItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddImportItem(Arg.Any<ImportInfo[]>());
                menuBuilder.AddUpdateItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddPropertiesItem();
                menuBuilder.Build();
            });
        }

        [Test]
        public void ContextMenuStrip_WithAddImportItem_CorrectImportInfosPassed()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);

            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            menuBuilder.AddOpenItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddImportItem(Arg.Any<ImportInfo[]>())
                       .Returns(menuBuilder);
            menuBuilder.When(x => x.AddImportItem(Arg.Any<ImportInfo[]>()))
                       .Do(invocation =>
                       {
                           var importInfos = invocation.Arg<ImportInfo[]>();
                           Assert.AreEqual(1, importInfos.Length);
                           Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(MacroStabilityInwardsFailureMechanismSectionsContext)));
                       });

            menuBuilder.AddUpdateItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            var assessmentSection = Substitute.For<IAssessmentSection>();
            var context = new MacroStabilityInwardsFailureMechanismSectionsContext(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                using (var plugin = new MacroStabilityInwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, null, treeViewControl);
                }
            }

            // Assert
            menuBuilder.Received().Build();
        }

        [Test]
        public void ForeColor_NoSectionsOnFailureMechanism_ReturnGrayText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var context = new MacroStabilityInwardsFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
        }

        [Test]
        public void ForeColor_HasSectionsOnFailureMechanism_ReturnControlText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                new FailureMechanismSection("A", new[]
                {
                    new Point2D(3, 4),
                    new Point2D(5, 6)
                })
            }, "B");

            var context = new MacroStabilityInwardsFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
        }

        private static TreeNodeInfo GetInfo(MacroStabilityInwardsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(MacroStabilityInwardsFailureMechanismSectionsContext));
        }
    }
}