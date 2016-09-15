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
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.Revetment.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationContextTreeNodeInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext));
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
            Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext), info.TagType);

            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnCalculationName()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            const string name = "cool name";
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = name
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual(name, text);
        }

        [Test]
        public void Image_Always_ReturnCalculationIcon()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            // Call
            Image icon = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.CalculationIcon, icon);
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnTrue()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            bool shouldBeVisible = info.EnsureVisibleOnCreate(null, null);

            // Assert
            Assert.IsTrue(shouldBeVisible);
        }

        [Test]
        public void CanRename_Always_ReturnTrue()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            bool canRename = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(canRename);
        }

        [Test]
        public void OnNodeRenamed_ChangeNameOfCalculationAndNotifyObservers()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            calculation.Attach(observer);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSection);
            const string name = "the new name!";

            // Call
            info.OnNodeRenamed(context, name);

            // Assert
            Assert.AreEqual(name, calculation.Name);
        }

        [Test]
        public void CanRemove_CalculationInParent_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var parentContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            // Call
            bool canRemoveCalculation = info.CanRemove(context, parentContext);

            // Assert
            Assert.IsTrue(canRemoveCalculation);
        }

        [Test]
        public void CanRemove_CalculationNotInParent_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var parentContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            // Call
            bool canRemoveCalculation = info.CanRemove(context, parentContext);

            // Assert
            Assert.IsFalse(canRemoveCalculation);
        }

        [Test]
        public void OnNodeRemoved_CalculationInParent_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Attach(observer);
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var parentContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            // Call
            info.OnNodeRemoved(context, parentContext);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.WaveConditionsCalculationGroup.Children, calculation);
        }

        [Test]
        public void CanDrag_Always_ReturnTrue()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            bool canDrag = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
        }
    }
}