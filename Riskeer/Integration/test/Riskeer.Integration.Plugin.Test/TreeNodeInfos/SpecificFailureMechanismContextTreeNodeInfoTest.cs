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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Integration.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class SpecificFailureMechanismContextTreeNodeInfoTest
    {
        private TreeNodeInfo info;
        private RiskeerPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new RiskeerPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(SpecificFailureMechanismContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.CheckedState);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new SpecificFailureMechanism();
            var failureMechanismContext = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            string text = info.Text(failureMechanismContext);

            // Assert
            Assert.AreEqual(failureMechanism.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.FailureMechanismIcon, image);
        }

        [Test]
        public void ForeColor_Always_ReturnsControlText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new SpecificFailureMechanism();
            var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            Color textColor = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), textColor);
        }

        [Test]
        public void CanRename_Always_ReturnTrue()
        {
            // Setup
            // Call
            bool canRename = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(canRename);
        }

        [Test]
        public void CanDrag_Always_ReturnTrue()
        {
            // Setup
            // Call
            bool canDrag = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void OnNodeRenamed_ChangesNameOfFailureMechanismAndNotifiesObservers()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var observer = Substitute.For<IObserver>();
            var failureMechanism = new SpecificFailureMechanism();
            failureMechanism.Attach(observer);

            var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            const string newName = "Updated FailureMechanism name";

            // Call
            info.OnNodeRenamed(context, newName);

            // Assert
            Assert.AreEqual(newName, failureMechanism.Name);
            observer.Received().UpdateObserver();
        }

        [Test]
        public void CanRemove_Always_ReturnTrue()
        {
            // Setup
            // Call
            bool canRename = info.CanRemove(null, null);

            // Assert
            Assert.IsTrue(canRename);
        }

        [Test]
        public void OnNodeRemoved_WithContexts_RemovesFailureMechanismFromCollectionAndNotifiesObservers()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var observer = Substitute.For<IObserver>();
            var failureMechanism = new SpecificFailureMechanism();
            var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            var failureMechanisms = new ObservableList<SpecificFailureMechanism>
            {
                failureMechanism
            };
            failureMechanisms.Attach(observer);
            var parentContext = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

            // Call
            info.OnNodeRemoved(context, parentContext);

            // Assert
            CollectionAssert.IsEmpty(failureMechanisms);
            observer.Received().UpdateObserver();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismInAssemblyTrue_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new SpecificFailureMechanism();
            var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);
            var inputFolder = (CategoryTreeFolder) children[0];

            Assert.AreEqual(2, inputFolder.Contents.Count());
            Assert.AreEqual("Invoer", inputFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputFolder.Category);

            var sectionsContext = (SpecificFailureMechanismSectionsContext) inputFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism, sectionsContext.WrappedData);
            Assert.AreSame(assessmentSection, sectionsContext.AssessmentSection);

            var inAssemblyInputComments = (Comment) inputFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism.InAssemblyInputComments, inAssemblyInputComments);

            var outputFolder = (CategoryTreeFolder) children[1];
            Assert.AreEqual("Oordeel", outputFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputFolder.Category);

            Assert.AreEqual(2, outputFolder.Contents.Count());
            var sectionResultContext = (SpecificFailureMechanismSectionResultContext) outputFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism.SectionResults, sectionResultContext.WrappedData);
            Assert.AreSame(failureMechanism, sectionResultContext.FailureMechanism);
            Assert.AreSame(assessmentSection, sectionResultContext.AssessmentSection);

            var inAssemblyOutputComments = (Comment) outputFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism.InAssemblyOutputComments, inAssemblyOutputComments);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismInAssemblyTrue_ReturnOnlyFailureMechanismNotInAssemblyComments()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new SpecificFailureMechanism
            {
                InAssembly = false
            };

            var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var comment = (Comment) children[0];
            Assert.AreSame(failureMechanism.NotInAssemblyComments, comment);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismInAssemblyTrue_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var failureMechanism = new SpecificFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = Substitute.For<IContextMenuBuilder>();
                menuBuilder.AddOpenItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddRenameItem().Returns(menuBuilder);
                menuBuilder.AddDeleteItem().Returns(menuBuilder);
                menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
                menuBuilder.AddExpandAllItem().Returns(menuBuilder);
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);

                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeView).Returns(menuBuilder);
                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                Received.InOrder(() =>
                {
                    menuBuilder.AddOpenItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                    menuBuilder.AddSeparator();
                    menuBuilder.AddRenameItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddDeleteItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddCollapseAllItem();
                    menuBuilder.AddExpandAllItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddPropertiesItem();
                    menuBuilder.Build();
                });
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismInAssemblyFalse_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new SpecificFailureMechanism
            {
                InAssembly = false
            };
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeView = new TreeViewControl())
            {
                var menuBuilder = Substitute.For<IContextMenuBuilder>();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddRenameItem().Returns(menuBuilder);
                menuBuilder.AddDeleteItem().Returns(menuBuilder);
                menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
                menuBuilder.AddExpandAllItem().Returns(menuBuilder);
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);

                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeView).Returns(menuBuilder);
                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                Received.InOrder(() =>
                {
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                    menuBuilder.AddSeparator();
                    menuBuilder.AddRenameItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddDeleteItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddCollapseAllItem();
                    menuBuilder.AddExpandAllItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddPropertiesItem();
                    menuBuilder.Build();
                });
            }
        }

        [TestFixture]
        public class SpecificFailureMechanismContextInAssemblyTreeNodeInfoTest
            : FailureMechanismInAssemblyTreeNodeInfoTestFixtureBase<RiskeerPlugin, SpecificFailureMechanism, SpecificFailureMechanismContext>
        {
            public SpecificFailureMechanismContextInAssemblyTreeNodeInfoTest() : base(2, 0) {}

            protected override SpecificFailureMechanismContext CreateFailureMechanismContext(SpecificFailureMechanism failureMechanism,
                                                                                             IAssessmentSection assessmentSection)
            {
                return new SpecificFailureMechanismContext(failureMechanism, assessmentSection);
            }
        }
    }
}