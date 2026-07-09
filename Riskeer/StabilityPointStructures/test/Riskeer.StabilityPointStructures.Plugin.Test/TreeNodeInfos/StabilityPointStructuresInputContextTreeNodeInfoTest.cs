// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Structures;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructuresInputContextTreeNodeInfoTest
    {
        private StabilityPointStructuresPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new StabilityPointStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructuresInputContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup

            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.CheckedState);
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
            var stabilityPointStructuresInputContext = new StabilityPointStructuresInputContext(
                Substitute.For<StabilityPointStructuresInput>(),
                Substitute.For<StructuresCalculation<StabilityPointStructuresInput>>(),
                new StabilityPointStructuresFailureMechanism(),
                Substitute.For<IAssessmentSection>());

            // Call
            string text = info.Text(stabilityPointStructuresInputContext);

            // Assert
            Assert.AreEqual("Invoer", text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var stabilityPointStructuresInputContext = new StabilityPointStructuresInputContext(
                Substitute.For<StabilityPointStructuresInput>(),
                Substitute.For<StructuresCalculation<StabilityPointStructuresInput>>(),
                new StabilityPointStructuresFailureMechanism(),
                Substitute.For<IAssessmentSection>());

            // Call
            Image image = info.Image(stabilityPointStructuresInputContext);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilderMethods()
        {
            // Setup
            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            {
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);
                menuBuilder.Build().Returns((ContextMenuStrip) null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(null, treeViewControl).Returns(menuBuilder);

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }
    }
}