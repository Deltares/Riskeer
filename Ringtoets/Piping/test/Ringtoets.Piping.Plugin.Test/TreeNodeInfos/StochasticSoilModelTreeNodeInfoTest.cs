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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
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
using Ringtoets.Piping.Primitives;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StochasticSoilModelTreeNodeInfoTest
    {
        private PipingPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StochasticSoilModel));
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
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
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

        [Test]
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            const string name = "test test 123";
            var model = new StochasticSoilModel(1, name, "a");

            // Call
            var text = info.Text(model);

            // Assert
            Assert.AreEqual(name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var model = new StochasticSoilModel(1, "A", "B");

            // Call
            var image = info.Image(model);

            // Assert
            TestHelper.AssertImagesAreEqual(PipingFormsResources.StochasticSoilModelIcon, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var stochasticSoilProfile1 = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("pipingSoilProfile1", 0, new List<PipingSoilLayer>
                {
                    new PipingSoilLayer(10)
                }, SoilProfileType.SoilProfile1D, 0)
            };
            var stochasticSoilProfile2 = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("pipingSoilProfile2", 0, new List<PipingSoilLayer>
                {
                    new PipingSoilLayer(10)
                }, SoilProfileType.SoilProfile1D, 0)
            };

            var stochasticSoilModel = new StochasticSoilModel(0, "Name", "Name");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile1);
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile2);

            // Call
            var objects = info.ChildNodeObjects(stochasticSoilModel);

            // Assert
            var expectedChildren = new[]
            {
                stochasticSoilProfile1,
                stochasticSoilProfile2
            };
            CollectionAssert.AreEqual(expectedChildren, objects);
        }

        [Test]
        public void CanRemove_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var parentData = new StochasticSoilModelsContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsTrue(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_OtherParentData_ReturnFalse()
        {
            // Call
            bool canRemove = info.CanRemove(null, null);

            // Assert
            Assert.IsFalse(canRemove);
        }

        [Test]
        public void OnNodeRemoved_RemovingSoilModelAssignedToCalculation_SoilModelRemovedFromFailureMechanismAndCalculationModelAndProfileCleared()
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

            var nodeData = new StochasticSoilModel(1, "A", "B")
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 1),
                    new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 2)
                }
            };
            var otherModel = new StochasticSoilModel(2, "C", "D")
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(0.8, SoilProfileType.SoilProfile2D, 3),
                    new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile2D, 4)
                }
            };

            var generalInput = new GeneralPipingInput();
            var calculation1 = new PipingCalculationScenario(generalInput)
            {
                InputParameters =
                {
                    StochasticSoilModel = nodeData,
                    StochasticSoilProfile = nodeData.StochasticSoilProfiles[0]
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new PipingCalculationScenario(generalInput)
            {
                InputParameters =
                {
                    StochasticSoilModel = nodeData,
                    StochasticSoilProfile = nodeData.StochasticSoilProfiles[1]
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new PipingCalculationScenario(generalInput)
            {
                InputParameters =
                {
                    StochasticSoilModel = otherModel,
                    StochasticSoilProfile = otherModel.StochasticSoilProfiles[0]
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var failureMechanism = new PipingFailureMechanism
            {
                StochasticSoilModels =
                {
                    nodeData,
                    otherModel
                },
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation1,
                        new CalculationGroup("A", true)
                        {
                            Children =
                            {
                                calculation2
                            }
                        },
                        calculation3
                    }
                }
            };
            failureMechanism.StochasticSoilModels.Attach(observer);

            var parentData = new StochasticSoilModelsContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.StochasticSoilModels, nodeData);

            Assert.IsNull(calculation1.InputParameters.StochasticSoilModel);
            Assert.IsNull(calculation1.InputParameters.StochasticSoilProfile);
            Assert.IsNull(calculation2.InputParameters.StochasticSoilModel);
            Assert.IsNull(calculation2.InputParameters.StochasticSoilProfile);
            Assert.IsNotNull(calculation3.InputParameters.StochasticSoilModel);
            Assert.IsNotNull(calculation3.InputParameters.StochasticSoilProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var model = new StochasticSoilModel(1, "A", "B");

            var mocks = new MockRepository();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(model, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(model, null, treeViewControl);
            }
            // Assert
            mocks.VerifyAll();
        }
    }
}