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
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.Test.TreeNodeInfos
{
    public class StabilityStoneCoverWaveConditionsCalculationInputContextTreeNodeInfoTest
    {
        private MockRepository mocks;
        private StabilityStoneCoverPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new StabilityStoneCoverPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationInputContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(StabilityStoneCoverWaveConditionsCalculationInputContext), info.TagType);

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
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.ForeColor);
        }

        [Test]
        public void Text_Always_ReturnName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new WaveConditionsInput();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationInputContext(input,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            // Call
            string nodeText = info.Text(context);

            // Assert
            Assert.AreEqual("Invoer", nodeText);
        }

        [Test]
        public void Image_Always_ReturnOutputIcon()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new WaveConditionsInput();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationInputContext(input,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            // Call
            Image icon = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, icon);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var input = new WaveConditionsInput();
                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                var context = new StabilityStoneCoverWaveConditionsCalculationInputContext(input,
                                                                                           failureMechanism,
                                                                                           assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

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