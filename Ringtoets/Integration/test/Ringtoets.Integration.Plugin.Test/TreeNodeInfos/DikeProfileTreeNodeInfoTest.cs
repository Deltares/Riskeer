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
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DikeProfileTreeNodeInfoTest
    {
        private RingtoetsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DikeProfile));
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
            Assert.AreEqual(typeof(DikeProfile), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnDikeProfileName()
        {
            // Setup
            const string profileName = "Random profile name";
            DikeProfile dikeProfile = new TestDikeProfile(profileName);

            // Call
            string text = info.Text(dikeProfile);

            // Assert
            Assert.AreEqual(profileName, text);
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.DikeProfile, image);
        }

        [Test]
        public void CanRemove_ParentIsDikeProfilesContext_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var parentData = new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsTrue(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsNotDikeProfilesContext_ReturnFalse()
        {
            // Call
            bool canRemove = info.CanRemove(null, null);

            // Assert
            Assert.IsFalse(canRemove);
        }

        [Test]
        public void OnNodeRemoved_RemovingProfileAssignedToCalculationOfSectionResult_ProfileRemovedFromFailureMechanismAndCalculationProfileClearedAndSectionResultCalculationCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());
            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            var calculation3Observer = mocks.StrictMock<IObserver>();
            calculation3Observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mocks.ReplayAll();

            DikeProfile nodeData = new TestDikeProfile(new Point2D(1, 0));
            DikeProfile otherProfile1 = new TestDikeProfile(new Point2D(2, 0));
            DikeProfile otherProfile2 = new TestDikeProfile(new Point2D(7, 0));

            var calculation1 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = otherProfile1
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = otherProfile2
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                DikeProfiles =
                {
                    nodeData,
                    otherProfile1,
                    otherProfile2
                },
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation1,
                        calculation2
                    }
                }
            };
            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0),
                new Point2D(4, 0)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("B", new[]
            {
                new Point2D(4, 0),
                new Point2D(9, 0)
            }));
            failureMechanism.DikeProfiles.Attach(observer);
            failureMechanism.SectionResults.ElementAt(0).Calculation = calculation1;
            failureMechanism.SectionResults.ElementAt(1).Calculation = calculation3;

            var parentData = new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.DikeProfiles, nodeData);

            Assert.IsNull(calculation1.InputParameters.DikeProfile);
            Assert.IsNotNull(calculation2.InputParameters.DikeProfile);

            Assert.AreSame(calculation2, failureMechanism.SectionResults.ElementAt(0).Calculation);
            Assert.AreSame(calculation3, failureMechanism.SectionResults.ElementAt(1).Calculation);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mocks = new MockRepository();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                using (var p = new RingtoetsPlugin())
                {
                    p.Gui = gui;
                    var i = p.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DikeProfile));

                    // Call
                    i.ContextMenuStrip(null, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }
    }
}