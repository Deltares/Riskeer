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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Asphalt.Plugin;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Asphalt.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class ForehoreProfilesContextTreeNodeInfoTest
    {
        private WaveImpactAsphaltCoverPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new WaveImpactAsphaltCoverPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ForeshoreProfilesContext));
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
            Assert.AreEqual(typeof(ForeshoreProfilesContext), info.TagType);
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
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var foreShores = new ObservableList<ForeshoreProfile>();

            var foreShoresContext = new ForeshoreProfilesContext(foreShores, assessmentSection);

            // Call
            string text = info.Text(foreShoresContext);

            // Assert
            const string expectedText = "Voorlanden";
            Assert.AreEqual(expectedText, text);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var foreShores = new ObservableList<ForeshoreProfile>();

            var foreShoresContext = new ForeshoreProfilesContext(foreShores, assessmentSection);

            // Call
            Image image = info.Image(foreShoresContext);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionIsEmpty_ReturnGrayText()
        {
            // Setup
            var mocks = new MockRepository();
            var asssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var foreShores = new ObservableList<ForeshoreProfile>();

            // Precondition
            CollectionAssert.IsEmpty(foreShores);

            var foreShoresContext = new ForeshoreProfilesContext(foreShores, asssessmentSection);

            // Call
            Color color = info.ForeColor(foreShoresContext);

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

            var foreShores = new ObservableList<ForeshoreProfile>
            {
                CreateForeShore()
            };

            // Precondition
            CollectionAssert.IsNotEmpty(foreShores);

            var foreShoresContext = new ForeshoreProfilesContext(foreShores, asssessmentSection);

            // Call
            Color color = info.ForeColor(foreShoresContext);

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

            ForeshoreProfile foreShore1 = CreateForeShore();
            ForeshoreProfile foreShore2 = CreateForeShore();
            var foreShores = new ObservableList<ForeshoreProfile>
            {
                foreShore1,
                foreShore2
            };

            var foreShoresContext = new ForeshoreProfilesContext(foreShores, assessmentSection);

            // Call
            var children = info.ChildNodeObjects(foreShoresContext);

            // Assert
            Assert.AreEqual(2, children.Length);
            Assert.AreSame(foreShore1, children.ElementAt(0));
            Assert.AreSame(foreShore2, children.ElementAt(1));
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mocks = new MockRepository();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

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

        private static ForeshoreProfile CreateForeShore()
        {
            return new ForeshoreProfile(new Point2D(0, 0), new Point2D[0], 
                                        null, new ForeshoreProfile.ConstructionProperties());
        }
    }
}