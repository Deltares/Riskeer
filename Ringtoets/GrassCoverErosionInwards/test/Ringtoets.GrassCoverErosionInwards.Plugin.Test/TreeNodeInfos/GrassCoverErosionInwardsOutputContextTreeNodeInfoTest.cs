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
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputContextTreeNodeInfoTest
    {
        private GrassCoverErosionInwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsOutputContext));
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
        public void Text_Always_ReturnsFromResource()
        {
            // Call
            string text = info.Text(null);

            // Assert
            Assert.AreEqual("Resultaat", text);
        }

        [Test]
        public void Image_Always_ReturnsGeneralOutputIcon()
        {
            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.OutputIcon, image);
        }

        [Test]
        public void ForeColor_HasNoOutput_ReturnGrayText()
        {
            // Call
            Color color = info.ForeColor(new GrassCoverErosionInwardsOutputContext(new GrassCoverErosionInwardsCalculation()));

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
        }

        [Test]
        public void ForeColor_HasOutput_ReturnControlText()
        {
            // Call
            Color color = info.ForeColor(new GrassCoverErosionInwardsOutputContext(new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            }));

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
        }

        [Test]
        public void ChildNodeObjects_HasNoOutput_ReturnsEmptyCollection()
        {
            // Setup
            var grassCoverErosionInwardsOutputContext = new GrassCoverErosionInwardsOutputContext(new GrassCoverErosionInwardsCalculation());

            // Call
            object[] children = info.ChildNodeObjects(grassCoverErosionInwardsOutputContext).ToArray();

            // Assert
            Assert.AreEqual(0, children.Length);
        }

        [Test]
        public void ChildNodeObjects_HasOutput_ReturnsCollectionWithOutputObjects()
        {
            // Setup
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            var grassCoverErosionInwardsOutputContext = new GrassCoverErosionInwardsOutputContext(grassCoverErosionInwardsCalculation);

            // Call
            object[] children = info.ChildNodeObjects(grassCoverErosionInwardsOutputContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var overtoppingOutputContext = children[0] as OvertoppingOutputContext;
            Assert.IsNotNull(overtoppingOutputContext);
            Assert.AreSame(grassCoverErosionInwardsCalculation, overtoppingOutputContext.WrappedData);

            var dikeHeightOutputContext = children[1] as DikeHeightOutputContext;
            Assert.IsNotNull(dikeHeightOutputContext);
            Assert.AreSame(grassCoverErosionInwardsCalculation, dikeHeightOutputContext.WrappedData);

            var overtoppingRateOutputContext = children[2] as OvertoppingRateOutputContext;
            Assert.IsNotNull(overtoppingRateOutputContext);
            Assert.AreSame(grassCoverErosionInwardsCalculation, overtoppingRateOutputContext.WrappedData);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mocks = new MockRepository();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(null, treeViewControl)).Return(menuBuilderMock);
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