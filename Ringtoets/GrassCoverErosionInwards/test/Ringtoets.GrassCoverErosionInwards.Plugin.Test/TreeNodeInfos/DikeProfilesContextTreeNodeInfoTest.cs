﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DikeProfilesContextTreeNodeInfoTest
    {
        private GrassCoverErosionInwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DikeProfilesContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(DikeProfilesContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ForeColor);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.ContextMenuStrip);
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

        [Test]
        public void Text_Always_ReturnExpectedText()
        {
            // Call
            string text = info.Text(null);

            // Assert
            const string expectedText = "Dijkprofielen";
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void ForeColor_CollectionIsEmpty_ReturnGrayText()
        {
            // Setup
            var mocks = new MockRepository();
            var asssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.DikeProfiles);

            var context = new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, asssessmentSection);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionHasElementsEmpty_ReturnControlText()
        {
            // Setup
            var mocks = new MockRepository();
            var asssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                DikeProfiles =
                {
                    new TestDikeProfile()
                }
            };

            // Precondition
            CollectionAssert.IsNotEmpty(failureMechanism.DikeProfiles);

            var context = new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, asssessmentSection);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnDikeProfiles()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            DikeProfile dikeProfile1 = new TestDikeProfile();
            DikeProfile dikeProfile2 = new TestDikeProfile();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                DikeProfiles =
                {
                    dikeProfile1,
                    dikeProfile2
                }
            };

            var dikeProfilesContext = new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection);

            // Call
            var children = info.ChildNodeObjects(dikeProfilesContext);

            // Assert
            Assert.AreEqual(2, children.Length);
            Assert.AreSame(dikeProfile1, children.ElementAt(0));
            Assert.AreSame(dikeProfile2, children.ElementAt(1));
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mocks = new MockRepository();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddDeleteChildrenItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }
            // Assert
            mocks.VerifyAll();
        }
    }
}