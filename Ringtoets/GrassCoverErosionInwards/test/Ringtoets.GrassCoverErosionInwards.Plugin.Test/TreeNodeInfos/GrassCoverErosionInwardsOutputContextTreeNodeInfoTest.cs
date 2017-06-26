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

using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputContextTreeNodeInfoTest
    {
        private MockRepository mocksRepository;
        private GrassCoverErosionInwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsOutputContext));
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
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNull(info.ContextMenuStrip);
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
        public void Text_Always_ReturnsFromResource()
        {
            // Setup
            mocksRepository.ReplayAll();

            // Call
            string text = info.Text(null);

            // Assert
            Assert.AreEqual("Resultaat", text);
        }

        [Test]
        public void Image_Always_ReturnsGeneralOutputIcon()
        {
            // Setup
            mocksRepository.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralOutputIcon, image);
        }

        [Test]
        public void ChildNodeObjects_EverythingCalculated_ReturnCollectionWithOutputObject()
        {
            // Setup
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var output = new GrassCoverErosionInwardsOutput(new GrassCoverErosionInwardsOvertoppingOutput(
                                                                0, true, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)),
                                                            new TestDikeHeightOutput(0.0),
                                                            new TestOvertoppingRateOutput(0));

            var context = new GrassCoverErosionInwardsOutputContext(output, new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);

            var overtoppingOutput = children[0] as GrassCoverErosionInwardsOvertoppingOutput;
            Assert.AreSame(context.WrappedData.OvertoppingOutput, overtoppingOutput);

            var dikeHeightOutput = children[1] as DikeHeightOutput;
            Assert.AreSame(context.WrappedData.DikeHeightOutput, dikeHeightOutput);
        }

        [Test]
        public void ChildNodeObjects_HBNNotCalculated_ReturnCollectionWithOutputObjects()
        {
            // Setup
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var output = new GrassCoverErosionInwardsOutput(new GrassCoverErosionInwardsOvertoppingOutput(
                                                                0, true, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)),
                                                            null,
                                                            new TestOvertoppingRateOutput(0));

            var context = new GrassCoverErosionInwardsOutputContext(output, new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);

            var resultOutput = children[0] as GrassCoverErosionInwardsOvertoppingOutput;
            Assert.AreSame(context.WrappedData.OvertoppingOutput, resultOutput);

            Assert.IsInstanceOf<EmptyDikeHeightOutput>(children[1]);
        }
    }
}