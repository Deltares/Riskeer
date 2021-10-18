// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class SpecificFailurePathsContextTreeNodeInfoTest
    {
        private MockRepository mocks;
        private TreeNodeInfo info;
        private RiskeerPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RiskeerPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(SpecificFailurePathsContext));
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
            Assert.IsNull(info.CheckedState);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsSetText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePaths = new ObservableList<SpecificFailurePath>();
            var failureMechanismContext = new SpecificFailurePathsContext(failurePaths, assessmentSection);

            // Call
            string text = info.Text(failureMechanismContext);

            // Assert
            Assert.AreEqual("Specifieke faalpaden", text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
        }
        
        [Test]
        public void ChildNodeObjects_WithoutFailurePaths_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePaths = new ObservableList<SpecificFailurePath>();
            var context = new SpecificFailurePathsContext(failurePaths, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            CollectionAssert.IsEmpty(children);
        }
        
        [Test]
        public void ChildNodeObjects_WithFailurePaths_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePath = new SpecificFailurePath();
            var failurePaths = new ObservableList<SpecificFailurePath>
            {
                failurePath
            };
            var context = new SpecificFailurePathsContext(failurePaths, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var failurePathContext = (SpecificFailurePathContext) children[0];
            Assert.AreSame(failurePath, failurePathContext.WrappedData);
            Assert.AreSame(assessmentSection, failurePathContext.Parent);
        }
    }
}