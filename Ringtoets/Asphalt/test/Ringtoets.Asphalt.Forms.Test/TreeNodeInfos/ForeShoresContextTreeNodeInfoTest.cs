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
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Asphalt.Plugin;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Asphalt.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class ForeShoresContextTreeNodeInfoTest
    {
        private WaveImpactAsphaltCoverPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new WaveImpactAsphaltCoverPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ForeShoresContext));
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
            Assert.AreEqual(typeof(ForeShoresContext), info.TagType);
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
            var foreShores = new ObservableList<ForeShore>();

            var foreShoresContext = new ForeShoresContext(foreShores);

            // Call
            string text = info.Text(foreShoresContext);

            // Assert
            const string expectedText = "Voorlanden";
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Setup
            var foreShores = new ObservableList<ForeShore>();

            var foreShoresContext = new ForeShoresContext(foreShores);

            // Call
            Image image = info.Image(foreShoresContext);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void ForeColor_CollectionIsEmpty_ReturnGrayText()
        {
            // Setup
            var foreShores = new ObservableList<ForeShore>();

            // Precondition
            CollectionAssert.IsEmpty(foreShores);

            var foreShoresContext = new ForeShoresContext(foreShores);

            // Call
            Color color = info.ForeColor(foreShoresContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
        }

        [Test]
        public void ForeColor_CollectionHasElementsEmpty_ReturnControlText()
        {
            // Setup
            var foreShores = new ObservableList<ForeShore>
            {
                CreateForeShore()
            };

            // Precondition
            CollectionAssert.IsNotEmpty(foreShores);

            var foreShoresContext = new ForeShoresContext(foreShores);

            // Call
            Color color = info.ForeColor(foreShoresContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnDikeProfiles()
        {
            // Setup
            ForeShore foreShore1 = CreateForeShore();
            ForeShore foreShore2 = CreateForeShore();
            var foreShores = new ObservableList<ForeShore>
            {
                foreShore1,
                foreShore2
            };

            var foreShoresContext = new ForeShoresContext(foreShores);

            // Call
            var children = info.ChildNodeObjects(foreShoresContext);

            // Assert
            Assert.AreEqual(2, children.Length);
            Assert.AreSame(foreShore1, children.ElementAt(0));
            Assert.AreSame(foreShore2, children.ElementAt(1));
        }

        private static ForeShore CreateForeShore()
        {
            return new ForeShore();
        }
    }
}