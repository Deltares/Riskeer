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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StochasticSoilModelCollectionContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private TreeNodeInfo info;

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StochasticSoilModelCollectionContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
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
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var stochasticSoilModelCollectionContext = new StochasticSoilModelCollectionContext(
                failureMechanism.StochasticSoilModels,
                failureMechanism,
                assessmentSection);

            // Call
            string text = info.Text(stochasticSoilModelCollectionContext);

            // Assert
            Assert.AreEqual("Stochastische ondergrondmodellen", text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var stochasticSoilModelCollectionContext = new StochasticSoilModelCollectionContext(
                failureMechanism.StochasticSoilModels,
                failureMechanism,
                assessmentSection);

            // Call
            Image image = info.Image(stochasticSoilModelCollectionContext);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void ForeColor_CollectionWithoutSoilProfiles_ReturnsGrayText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var stochasticSoilModelCollectionContext = new StochasticSoilModelCollectionContext(
                failureMechanism.StochasticSoilModels,
                failureMechanism,
                assessmentSection);

            // Call
            Color foreColor = info.ForeColor(stochasticSoilModelCollectionContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), foreColor);
        }

        [Test]
        public void ForeColor_CollectionWithSoilProfiles_ReturnsControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new StochasticSoilModel("Name")
            }, "path");

            var stochasticSoilModelCollectionContext = new StochasticSoilModelCollectionContext(
                failureMechanism.StochasticSoilModels,
                failureMechanism,
                assessmentSection);

            // Call
            Color foreColor = info.ForeColor(stochasticSoilModelCollectionContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingSoilProfile1 = new PipingSoilProfile("pipingSoilProfile1", 0, new List<PipingSoilLayer>
            {
                new PipingSoilLayer(10)
            }, SoilProfileType.SoilProfile1D, 0);
            var pipingSoilProfile2 = new PipingSoilProfile("pipingSoilProfile2", 0, new List<PipingSoilLayer>
            {
                new PipingSoilLayer(10)
            }, SoilProfileType.SoilProfile1D, 0);
            var stochasticSoilProfile1 = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = pipingSoilProfile1
            };
            var stochasticSoilProfile2 = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = pipingSoilProfile2
            };

            var stochasticSoilModel = new StochasticSoilModel("Name");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile1);
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile2);

            var failureMechanism = new PipingFailureMechanism();
            var stochasticSoilModelCollectionContext = new StochasticSoilModelCollectionContext(
                failureMechanism.StochasticSoilModels,
                failureMechanism,
                assessmentSection);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "path");

            // Call
            object[] objects = info.ChildNodeObjects(stochasticSoilModelCollectionContext);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                stochasticSoilModel
            }, objects);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddUpdateItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var context = new StochasticSoilModelCollectionContext(
                    new StochasticSoilModelCollection(),
                    new PipingFailureMechanism(),
                    assessmentSection);
                var gui = mocks.Stub<IGui>();

                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }
            // Assert
            // Assert expectancies are called in TearDown()
        }
    }
}