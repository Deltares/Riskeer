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
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Plugin;
using HeightStructuresFormsResources = Ringtoets.HeightStructures.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HeightStructuresInputContextTreeNodeInfoTest
    {
        private MockRepository mocksRepository;
        private HeightStructuresPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
            plugin = new HeightStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HeightStructuresInputContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocksRepository.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(HeightStructuresInputContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
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
            var assessmentSectionStub = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var heightStructuresCalculation = new HeightStructuresCalculation();
            var heightStructuresInputContext = new HeightStructuresInputContext(
                heightStructuresCalculation.InputParameters,
                heightStructuresCalculation,
                new HeightStructuresFailureMechanism(),
                assessmentSectionStub);

            // Call
            var text = info.Text(heightStructuresInputContext);

            // Assert
            Assert.AreEqual("Invoer", text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSectionStub = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var heightStructuresCalculation = new HeightStructuresCalculation();
            var heightStructuresInputContext = new HeightStructuresInputContext(
                heightStructuresCalculation.InputParameters,
                heightStructuresCalculation,
                new HeightStructuresFailureMechanism(),
                assessmentSectionStub);

            // Call
            var image = info.Image(heightStructuresInputContext);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilderMethods()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);

                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }
            // Assert
            // Assert expectancies are called in TearDown()
        }
    }
}