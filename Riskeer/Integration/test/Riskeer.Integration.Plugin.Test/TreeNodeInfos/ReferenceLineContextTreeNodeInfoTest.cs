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
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ReferenceLineContextTreeNodeInfoTest
    {
        

        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNotNull(info.ForeColor);
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
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var context = new ReferenceLineContext(new ReferenceLine(), assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual("Referentielijn", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var context = new ReferenceLineContext(new ReferenceLine(), assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(context);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ReferenceLineIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            menuBuilder.AddImportItem().Returns(menuBuilder);
            menuBuilder.AddExportItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(Arg.Any<object>(), treeViewControl).Returns(menuBuilder);
                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }
            }

            // Assert
            menuBuilder.Received().Build();
        }

        [Test]
        public void ForeColor_ContextHasReferenceLineWithoutGeometry_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var referenceLineContext = new ReferenceLineContext(new ReferenceLine(), assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(referenceLineContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
        }

        [Test]
        public void ForeColor_ContextHasReferenceLineWithGeometry_ReturnControlText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var referenceLineContext = new ReferenceLineContext(ReferenceLineTestFactory.CreateReferenceLineWithGeometry(),
                                                                assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(referenceLineContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
        }

        private TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ReferenceLineContext));
        }
    }
}