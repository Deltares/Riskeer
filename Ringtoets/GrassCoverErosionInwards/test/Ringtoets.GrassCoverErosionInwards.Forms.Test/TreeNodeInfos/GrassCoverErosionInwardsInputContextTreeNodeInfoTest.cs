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

using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextTreeNodeInfoTest
    {
        private MockRepository mocksRepository;
        private GrassCoverErosionInwardsGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
            plugin = new GrassCoverErosionInwardsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsInputContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(GrassCoverErosionInwardsInputContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ChildNodeObjects);
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
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var assessmentSection = mocksRepository.StrictMock<IAssessmentSection>();
            var grassCoverErosionInwardsInputContext = new GrassCoverErosionInwardsInputContext(
                mocksRepository.StrictMock<GrassCoverErosionInwardsInput>(generalInput),
                mocksRepository.StrictMock<GrassCoverErosionInwardsCalculation>(generalInput),
                mocksRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>(),
                assessmentSection);
            mocksRepository.ReplayAll();

            // Call
            var text = info.Text(grassCoverErosionInwardsInputContext);

            // Assert
            Assert.AreEqual(GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsInputContext_NodeDisplayName, text);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var assessmentSection = mocksRepository.StrictMock<IAssessmentSection>();
            var grassCoverErosionInwardsInputContext = new GrassCoverErosionInwardsInputContext(
                mocksRepository.StrictMock<GrassCoverErosionInwardsInput>(generalInput),
                mocksRepository.StrictMock<GrassCoverErosionInwardsCalculation>(generalInput),
                mocksRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>(),
                assessmentSection);
            mocksRepository.ReplayAll();

            // Call
            var image = info.Image(grassCoverErosionInwardsInputContext);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var gui = mocksRepository.StrictMultiMock<IGui>();
            var treeViewControl = mocksRepository.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            gui.Expect(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);

            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            mocksRepository.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(null, null, treeViewControl);

            // Assert
            mocksRepository.VerifyAll();
        }
    }
}