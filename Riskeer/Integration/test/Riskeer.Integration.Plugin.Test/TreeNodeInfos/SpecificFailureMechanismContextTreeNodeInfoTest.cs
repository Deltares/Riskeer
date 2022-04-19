﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
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
        private MockRepository mocks;
        private TreeNodeInfo info;
        private RiskeerPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RiskeerPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(SpecificFailureMechanismContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.FailureMechanismIcon, image);
        }

        [Test]
        public void ForeColor_Always_ReturnsControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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
            mocks.ReplayAll();

            // Call
            bool canRename = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(canRename);
        }

        [Test]
        public void CanDrag_Always_ReturnTrue()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            bool canDrag = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void OnNodeRenamed_ChangesNameOfFailureMechanismAndNotifiesObservers()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new SpecificFailureMechanism();
            failureMechanism.Attach(observer);

            var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            const string newName = "Updated FailureMechanism name";

            // Call
            info.OnNodeRenamed(context, newName);

            // Assert
            Assert.AreEqual(newName, failureMechanism.Name);
        }

        [Test]
        public void CanRemove_Always_ReturnTrue()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            bool canRename = info.CanRemove(null, null);

            // Assert
            Assert.IsTrue(canRename);
        }

        [Test]
        public void OnNodeRemoved_WithContexts_RemovesFailureMechanismFromCollectionAndNotifiesObservers()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

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
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismInAssemblyTrue_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddDeleteItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                // Assert expectancies are called in TearDown()
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var context = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeView = new TreeViewControl())
            {
                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddDeleteItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                // Assert expectancies are called in TearDown()
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