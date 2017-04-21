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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructureTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

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
                Assert.IsNotNull(info.CanRemove);
                Assert.IsNotNull(info.OnNodeRemoved);
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
        public void Text_Always_ReturnNameOfStructure()
        {
            // Setup
            const string name = "a nice name";
            StabilityPointStructure structure = new TestStabilityPointStructure(name);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(structure);

                // Assert
                Assert.AreEqual(name, text);
            }
        }

        [Test]
        public void Image_Always_ReturnStructureIcon()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.StructuresIcon, image);
            }
        }

        [Test]
        public void CanRemove_ParentIsStabilityPointStructuresContext_ReturnTrue()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var mocks = new MockRepository();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                mocks.ReplayAll();

                TreeNodeInfo info = GetInfo(plugin);

                var failureMechanism = new StabilityPointStructuresFailureMechanism();

                var parentData = new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures, failureMechanism, assessmentSection);

                // Call
                bool canRemove = info.CanRemove(null, parentData);

                // Assert
                Assert.IsTrue(canRemove);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CanRemove_ParentIsNotStabilityPointStructuresContext_ReturnFalse()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool canRemove = info.CanRemove(null, null);

                // Assert
                Assert.IsFalse(canRemove);
            }
        }

        [Test]
        public void OnNodeRemoved_RemovingProfilePartOfCalculationOfSectionResult_ProfileRemovedFromFailureMechanismAndCalculationProfileClearedAndSectionResultCalculationCleared()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
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

                TreeNodeInfo info = GetInfo(plugin);

                var nodeData = new TestStabilityPointStructure(new Point2D(1, 0));
                var otherProfile1 = new TestStabilityPointStructure(new Point2D(2, 0));
                var otherProfile2 = new TestStabilityPointStructure(new Point2D(6, 0));

                var calculation1 = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    InputParameters =
                    {
                        Structure = nodeData
                    }
                };
                calculation1.InputParameters.Attach(calculation1Observer);
                var calculation2 = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    InputParameters =
                    {
                        Structure = otherProfile1
                    }
                };
                calculation2.InputParameters.Attach(calculation2Observer);
                var calculation3 = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    InputParameters =
                    {
                        Structure = otherProfile2
                    }
                };
                calculation3.InputParameters.Attach(calculation3Observer);

                var failureMechanism = new StabilityPointStructuresFailureMechanism
                {
                    StabilityPointStructures =
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
                            calculation2,
                            calculation3
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
                failureMechanism.StabilityPointStructures.Attach(observer);
                failureMechanism.SectionResults.ElementAt(0).Calculation = calculation1;
                failureMechanism.SectionResults.ElementAt(1).Calculation = calculation3;

                var parentData = new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures, failureMechanism, assessmentSection);

                // Call
                info.OnNodeRemoved(nodeData, parentData);

                // Assert
                CollectionAssert.DoesNotContain(failureMechanism.StabilityPointStructures, nodeData);

                Assert.IsNull(calculation1.InputParameters.Structure);
                Assert.IsNotNull(calculation2.InputParameters.Structure);

                Assert.AreSame(calculation2, failureMechanism.SectionResults.ElementAt(0).Calculation);
                Assert.AreSame(calculation3, failureMechanism.SectionResults.ElementAt(1).Calculation);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilderMethods()
        {
            // Setup
            var mocksRepository = new MockRepository();

            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();
            using (mocksRepository.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                TreeNodeInfo info = GetInfo(plugin);

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }
            // Assert
            mocksRepository.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(StabilityPointStructuresPlugin guiPlugin)
        {
            return guiPlugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructure));
        }
    }
}