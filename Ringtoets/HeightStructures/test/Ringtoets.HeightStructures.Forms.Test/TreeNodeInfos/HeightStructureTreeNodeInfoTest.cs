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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HeightStructureTreeNodeInfoTest
    {
        private MockRepository mocks;
        private HeightStructuresPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new HeightStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HeightStructure));
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
            Assert.AreEqual(typeof(HeightStructure), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.ForeColor);
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
        public void Text_Always_ReturnNameOfStructure()
        {
            // Setup
            mocks.ReplayAll();
            string name = "very nice name!";
            HeightStructure structure = new TestHeightStructure(name);

            // Call
            string text = info.Text(structure);

            // Assert
            Assert.AreEqual(name, text);
        }

        [Test]
        public void Text_Always_ReturnStructuresIcon()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.StructuresIcon, image);
        }

        [Test]
        public void CanRemove_ParentIsHeightStructuresContext_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();

            var parentData = new HeightStructuresContext(failureMechanism.HeightStructures, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsTrue(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsNotHeightStructuresContext_ReturnFalse()
        {
            // Call
            bool canRemove = info.CanRemove(null, null);

            // Assert
            Assert.IsFalse(canRemove);
        }

        [Test]
        public void OnNodeRemoved_RemovingProfileFromContainer_ProfileRemovedFromContainer()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            HeightStructure nodeData = new TestHeightStructure();
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                HeightStructures =
                {
                    nodeData
                }
            };
            failureMechanism.HeightStructures.Attach(observer);

            var parentData = new HeightStructuresContext(failureMechanism.HeightStructures, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.HeightStructures, nodeData);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_RemovingProfilePartOfCalculation_CalculationProfileCleared()
        {
            // Setup
            var mocks = new MockRepository();
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

            HeightStructure nodeData = new TestHeightStructure("A");
            HeightStructure otherProfile = new TestHeightStructure("B");

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = nodeData
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = otherProfile
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var calculationGroup = new CalculationGroup("A", true)
            {
                Children =
                {
                    calculation2
                }
            };

            var failureMechanism = new HeightStructuresFailureMechanism
            {
                HeightStructures =
                {
                    nodeData,
                    otherProfile
                },
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation1,
                        calculationGroup,
                        calculation3
                    }
                }
            };
            failureMechanism.HeightStructures.Attach(observer);

            var parentData = new HeightStructuresContext(failureMechanism.HeightStructures, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.HeightStructures, nodeData);

            Assert.IsNull(calculation1.InputParameters.Structure);
            Assert.IsNull(calculation2.InputParameters.Structure);
            Assert.IsNotNull(calculation3.InputParameters.Structure);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_RemovingProfilePartOfCalculationOfSectionResult_SectionResultCalculationCleared()
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

            var nodeData = new TestHeightStructure("A");
            var otherProfile1 = new TestHeightStructure("B");
            var otherProfile2 = new TestHeightStructure("C");

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = otherProfile1
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = otherProfile2
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var failureMechanism = new HeightStructuresFailureMechanism
            {
                HeightStructures =
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
            failureMechanism.HeightStructures.Attach(observer);
            failureMechanism.SectionResults.ElementAt(0).Calculation = calculation1;
            failureMechanism.SectionResults.ElementAt(1).Calculation = calculation3;

            var parentData = new HeightStructuresContext(failureMechanism.HeightStructures, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.HeightStructures, nodeData);

            Assert.IsNull(calculation1.InputParameters.Structure);
            Assert.IsNotNull(calculation2.InputParameters.Structure);

            Assert.AreSame(calculation2, failureMechanism.SectionResults.ElementAt(0).Calculation);
            Assert.AreSame(calculation3, failureMechanism.SectionResults.ElementAt(1).Calculation);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                HeightStructure nodeData = new TestHeightStructure("A");

                var guiMock = mocks.StrictMock<IGui>();
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }
    }
}