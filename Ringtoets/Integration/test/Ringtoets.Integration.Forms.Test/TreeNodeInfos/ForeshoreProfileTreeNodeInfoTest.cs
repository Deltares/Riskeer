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
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Plugin;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class ForeshoreProfileTreeNodeInfoTest
    {
        private RingtoetsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ForeshoreProfile));
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
            Assert.AreEqual(typeof(ForeshoreProfile), info.TagType);
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
        public void Text_Always_ReturnForeshoreProfileName()
        {
            // Setup
            const string profileName = "Random profile name";
            ForeshoreProfile foreshoreProfile = new TestForeshoreProfile(profileName);

            // Call
            string text = info.Text(foreshoreProfile);

            // Assert
            Assert.AreEqual(profileName, text);
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.Foreshore, image);
        }

        [Test]
        public void CanRemove_ParentFailureMechanismIsStabilityStoneCover_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsTrue(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentFailureMechanismIsWaveImpactAsphaltCover_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsTrue(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentFailureMechanismIsGrassCoverErosionOutwards_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsTrue(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentFailureMechanismIsHeightStructures_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsTrue(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentFailureMechanismIsClosingStructures_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsTrue(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentFailureMechanismIsStabilityPointStructures_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsTrue(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_UnsupportedFailureMechanism_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var list = new ObservableList<ForeshoreProfile>();

            var parentData = new ForeshoreProfilesContext(list, failureMechanism, assessmentSection);

            // Call
            bool canRemove = info.CanRemove(null, parentData);

            // Assert
            Assert.IsFalse(canRemove);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_ForeshoreProfileOfStabilityStoneCoverWaveConditionsCalculation_ForeshoreProfileRemovedFromFailureMechanismAndCalculationForeshoreProfileCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());
            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver());
            var calculation3Observer = mocks.StrictMock<IObserver>();
            calculation3Observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mocks.ReplayAll();

            ForeshoreProfile nodeData = new TestForeshoreProfile("A");
            ForeshoreProfile otherProfile = new TestForeshoreProfile("B");

            var calculation1 = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = otherProfile
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                ForeshoreProfiles =
                {
                    nodeData,
                    otherProfile
                },
                WaveConditionsCalculationGroup =
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
            failureMechanism.ForeshoreProfiles.Attach(observer);

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, nodeData);

            Assert.IsNull(calculation1.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation2.InputParameters.ForeshoreProfile);
            Assert.AreSame(otherProfile, calculation3.InputParameters.ForeshoreProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_ForeshoreProfileOfWaveImpactAsphaltCoverWaveConditionsCalculation_ForeshoreProfileRemovedFromFailureMechanismAndCalculationForeshoreProfileCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());
            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver());
            var calculation3Observer = mocks.StrictMock<IObserver>();
            calculation3Observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mocks.ReplayAll();

            ForeshoreProfile nodeData = new TestForeshoreProfile("A");
            ForeshoreProfile otherProfile = new TestForeshoreProfile("B");

            var calculation1 = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = otherProfile
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                ForeshoreProfiles =
                {
                    nodeData,
                    otherProfile
                },
                WaveConditionsCalculationGroup =
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
            failureMechanism.ForeshoreProfiles.Attach(observer);

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, nodeData);

            Assert.IsNull(calculation1.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation2.InputParameters.ForeshoreProfile);
            Assert.AreSame(otherProfile, calculation3.InputParameters.ForeshoreProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_ForeshoreProfileOfGrassCoverErosionOutwardsWaveConditionsCalculation_ForeshoreProfileRemovedFromFailureMechanismAndCalculationForeshoreProfileCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());
            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver());
            var calculation3Observer = mocks.StrictMock<IObserver>();
            calculation3Observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mocks.ReplayAll();

            ForeshoreProfile nodeData = new TestForeshoreProfile("A");
            ForeshoreProfile otherProfile = new TestForeshoreProfile("B");

            var calculation1 = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = otherProfile
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                ForeshoreProfiles =
                {
                    nodeData,
                    otherProfile
                },
                WaveConditionsCalculationGroup =
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
            failureMechanism.ForeshoreProfiles.Attach(observer);

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, nodeData);

            Assert.IsNull(calculation1.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation2.InputParameters.ForeshoreProfile);
            Assert.AreSame(otherProfile, calculation3.InputParameters.ForeshoreProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_ForeshoreProfileOfHeightStructuresWaveConditionsCalculation_ForeshoreProfileRemovedFromFailureMechanismAndCalculationForeshoreProfileCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());
            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver());
            var calculation3Observer = mocks.StrictMock<IObserver>();
            calculation3Observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mocks.ReplayAll();

            ForeshoreProfile nodeData = new TestForeshoreProfile("A");
            ForeshoreProfile otherProfile = new TestForeshoreProfile("B");

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = otherProfile
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var failureMechanism = new HeightStructuresFailureMechanism
            {
                ForeshoreProfiles =
                {
                    nodeData,
                    otherProfile
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
            failureMechanism.ForeshoreProfiles.Attach(observer);

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, nodeData);

            Assert.IsNull(calculation1.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation2.InputParameters.ForeshoreProfile);
            Assert.AreSame(otherProfile, calculation3.InputParameters.ForeshoreProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_ForeshoreProfileOfClosingStructuresWaveConditionsCalculation_ForeshoreProfileRemovedFromFailureMechanismAndCalculationForeshoreProfileCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());
            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver());
            var calculation3Observer = mocks.StrictMock<IObserver>();
            calculation3Observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mocks.ReplayAll();

            ForeshoreProfile nodeData = new TestForeshoreProfile("A");
            ForeshoreProfile otherProfile = new TestForeshoreProfile("B");

            var calculation1 = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = otherProfile
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                ForeshoreProfiles =
                {
                    nodeData,
                    otherProfile
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
            failureMechanism.ForeshoreProfiles.Attach(observer);

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, nodeData);

            Assert.IsNull(calculation1.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation2.InputParameters.ForeshoreProfile);
            Assert.AreSame(otherProfile, calculation3.InputParameters.ForeshoreProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_ForeshoreProfileOfStabilityPointStructuresWaveConditionsCalculation_ForeshoreProfileRemovedFromFailureMechanismAndCalculationForeshoreProfileCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());
            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver());
            var calculation3Observer = mocks.StrictMock<IObserver>();
            calculation3Observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mocks.ReplayAll();

            ForeshoreProfile nodeData = new TestForeshoreProfile("A");
            ForeshoreProfile otherProfile = new TestForeshoreProfile("B");

            var calculation1 = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation1.InputParameters.Attach(calculation1Observer);
            var calculation2 = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = nodeData
                }
            };
            calculation2.InputParameters.Attach(calculation2Observer);
            var calculation3 = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = otherProfile
                }
            };
            calculation3.InputParameters.Attach(calculation3Observer);

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                ForeshoreProfiles =
                {
                    nodeData,
                    otherProfile
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
            failureMechanism.ForeshoreProfiles.Attach(observer);

            var parentData = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection);

            // Call
            info.OnNodeRemoved(nodeData, parentData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, nodeData);

            Assert.IsNull(calculation1.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation2.InputParameters.ForeshoreProfile);
            Assert.AreSame(otherProfile, calculation3.InputParameters.ForeshoreProfile);
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
                    var i = p.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ForeshoreProfile));

                    // Call
                    i.ContextMenuStrip(null, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }
    }
}