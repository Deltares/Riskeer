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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineTreeNodeInfoTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(RingtoetsPipingSurfaceLine));
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
            Assert.AreEqual(typeof(RingtoetsPipingSurfaceLine), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ChildNodeObjects);
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
        public void Text_Always_ReturnsName()
        {
            // Setup
            var ringtoetsPipingSurfaceLine = mocks.StrictMock<RingtoetsPipingSurfaceLine>();
            var testName = "ttt";
            ringtoetsPipingSurfaceLine.Name = testName;

            mocks.ReplayAll();

            // Call
            var text = info.Text(ringtoetsPipingSurfaceLine);

            // Assert
            Assert.AreEqual(testName, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var ringtoetsPipingSurfaceLine = mocks.StrictMock<RingtoetsPipingSurfaceLine>();

            mocks.ReplayAll();

            // Call
            var image = info.Image(ringtoetsPipingSurfaceLine);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.PipingSurfaceLineIcon, image);
        }

        [Test]
        public void CanRemove_Always_ReturnTrue()
        {
            // Call
            bool canRemove = info.CanRemove(null, null);

            // Assert
            Assert.IsTrue(canRemove);
        }

        [Test]
        public void RemoveData_SurfaceLineFromCollection_SurfaceLineRemovedFromParentCollection()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var nodeData = new RingtoetsPipingSurfaceLine();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.Add(nodeData);
            failureMechanism.SurfaceLines.Add(new RingtoetsPipingSurfaceLine());
            failureMechanism.SurfaceLines.Attach(observer);

            var parentNodeData = new RingtoetsPipingSurfaceLinesContext(failureMechanism.SurfaceLines, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.SurfaceLines, nodeData);
            // Expectancies checked in TearDown
        }

        [Test]
        public void OnNodeRemoved_RemovedSurfaceLinePartOfCalculationInput_CalculationSurfaceLineCleared()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());
            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver());
            var calculation3Observer = mocks.StrictMock<IObserver>();
            calculation3Observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mocks.ReplayAll();

            var nodeData = new RingtoetsPipingSurfaceLine();
            nodeData.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.Add(nodeData);
            var otherSurfaceline = new RingtoetsPipingSurfaceLine();
            otherSurfaceline.SetGeometry(new[]
            {
                new Point3D(7, 8, 9),
                new Point3D(0, 1, 2)
            });
            failureMechanism.SurfaceLines.Add(otherSurfaceline);
            failureMechanism.SurfaceLines.Attach(observer);

            var generalInputs = new GeneralPipingInput();
            var calculation1 = new PipingCalculationScenario(generalInputs)
            {
                InputParameters =
                {
                    SurfaceLine = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new PipingCalculationScenario(generalInputs)
            {
                InputParameters =
                {
                    SurfaceLine = nodeData
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new PipingCalculationScenario(generalInputs)
            {
                InputParameters =
                {
                    SurfaceLine = otherSurfaceline
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var groupWithCalculation = new CalculationGroup("A", true)
            {
                Children =
                {
                    calculation2
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(groupWithCalculation);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            var parentNodeData = new RingtoetsPipingSurfaceLinesContext(failureMechanism.SurfaceLines, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.SurfaceLines, nodeData);

            Assert.IsNull(calculation1.InputParameters.SurfaceLine);
            Assert.IsNull(calculation2.InputParameters.SurfaceLine);
            Assert.IsNotNull(calculation3.InputParameters.SurfaceLine,
                             "Calculation with different surfaceline should not be affected.");
            // Expectancies checked in TearDown
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }
            // Assert
            // Assert expectancies are called in TearDown()
        }
    }
}