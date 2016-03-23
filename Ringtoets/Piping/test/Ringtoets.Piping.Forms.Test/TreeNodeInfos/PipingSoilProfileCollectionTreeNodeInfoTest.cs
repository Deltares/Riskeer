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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingSoilProfileCollectionTreeNodeInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StochasticSoilModelContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(StochasticSoilModelContext), info.TagType);
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
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            var failureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var stochasticSoilModelContextMock = mocks.StrictMock<StochasticSoilModelContext>(failureMechanismMock, assessmentSectionMock);
            mocks.ReplayAll();

            // Call
            var text = info.Text(stochasticSoilModelContextMock);

            // Assert
            Assert.AreEqual(Resources.PipingSoilProfilesCollection_DisplayName, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var failureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var stochasticSoilModelContextMock = mocks.StrictMock<StochasticSoilModelContext>(failureMechanismMock, assessmentSectionMock);
            mocks.ReplayAll();

            // Call
            var image = info.Image(stochasticSoilModelContextMock);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.FolderIcon, image);

            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionWithoutSoilProfiles_ReturnsGrayText()
        {
            // Setup
            var failureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var stochasticSoilModelContextMock = mocks.StrictMock<StochasticSoilModelContext>(failureMechanismMock, assessmentSectionMock);
            mocks.ReplayAll();

            // Call
            var foreColor = info.ForeColor(stochasticSoilModelContextMock);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), foreColor);

            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionWithSoilProfiles_ReturnsControlText()
        {
            // Setup
            var failureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var stochasticSoilModelContextMock = mocks.StrictMock<StochasticSoilModelContext>(failureMechanismMock, assessmentSectionMock);
            failureMechanismMock.StochasticSoilModels.Add(new StochasticSoilModel(0, "Name", "Name"));
            mocks.ReplayAll();

            // Call
            var foreColor = info.ForeColor(stochasticSoilModelContextMock);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnData()
        {
            // Setup
            var pipingSoilProfile1 = new PipingSoilProfile("", 0, new List<PipingSoilLayer>
            {
                new PipingSoilLayer(10)
            }, 0);
            var pipingSoilProfile2 = new PipingSoilProfile("", 0, new List<PipingSoilLayer>
            {
                new PipingSoilLayer(10)
            }, 0);
            var stochasticSoilProfile1 = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = pipingSoilProfile1
            };
            var stochasticSoilProfile2 = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = pipingSoilProfile2
            };

            var stochasticSoilModel = new StochasticSoilModel(0, "Name", "Name");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile1);
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile2);

            var failureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var stochasticSoilModelContextMock = mocks.StrictMock<StochasticSoilModelContext>(failureMechanismMock, assessmentSectionMock);
            failureMechanismMock.StochasticSoilModels.Add(stochasticSoilModel);

            mocks.ReplayAll();

            // Call
            var objects = info.ChildNodeObjects(stochasticSoilModelContextMock);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                pipingSoilProfile1,
                pipingSoilProfile2
            }, objects);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var gui = mocks.StrictMultiMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            gui.Expect(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);

            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(null, null, treeViewControl);

            // Assert
            mocks.VerifyAll();
        }
    }
}