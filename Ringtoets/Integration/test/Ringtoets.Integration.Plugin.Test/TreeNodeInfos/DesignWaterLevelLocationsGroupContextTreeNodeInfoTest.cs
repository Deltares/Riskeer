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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DesignWaterLevelLocationsGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

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
        }

        [Test]
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("Toetspeilen", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsGeneralFolderIcon()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
            }
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var locations = new ObservableList<HydraulicBoundaryLocation>();
            var locationsGroupContext = new DesignWaterLevelLocationsGroupContext(locations, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] childNodeObjects = info.ChildNodeObjects(locationsGroupContext);

                // Assert
                Assert.AreEqual(4, childNodeObjects.Length);

                DesignWaterLevelLocationsContext[] locationsContexts = childNodeObjects.OfType<DesignWaterLevelLocationsContext>().ToArray();
                Assert.AreEqual(4, locationsContexts.Length);

                Assert.IsTrue(locationsContexts.All(c => ReferenceEquals(locations, c.WrappedData)));
                Assert.IsTrue(locationsContexts.All(c => ReferenceEquals(assessmentSection, c.AssessmentSection)));

                var testLocation = new TestHydraulicBoundaryLocation();

                Assert.AreEqual("A+->A", locationsContexts[0].CategoryBoundaryName);
                Assert.AreSame(testLocation.DesignWaterLevelCalculation1, locationsContexts[0].GetCalculationFunc(testLocation));

                Assert.AreEqual("A->B", locationsContexts[1].CategoryBoundaryName);
                Assert.AreSame(testLocation.DesignWaterLevelCalculation2, locationsContexts[1].GetCalculationFunc(testLocation));

                Assert.AreEqual("B->C", locationsContexts[2].CategoryBoundaryName);
                Assert.AreSame(testLocation.DesignWaterLevelCalculation3, locationsContexts[2].GetCalculationFunc(testLocation));

                Assert.AreEqual("C->D", locationsContexts[3].CategoryBoundaryName);
                Assert.AreSame(testLocation.DesignWaterLevelCalculation4, locationsContexts[3].GetCalculationFunc(testLocation));
            }

            mocks.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DesignWaterLevelLocationsGroupContext));
        }
    }
}