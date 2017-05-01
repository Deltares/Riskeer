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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.TestUtil;
using Ringtoets.Revetment.Forms.Views;
using Ringtoets.Revetment.TestUtil;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class WaveConditionsInputViewInfoTest
    {
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(WaveConditionsInputView));
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
            Assert.AreEqual(typeof(WaveConditionsInputContext), info.DataType);
            Assert.AreEqual(typeof(IWaveConditionsCalculation), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsInputResourceName()
        {
            // Setup
            using (var view = new WaveConditionsInputView())
            {
                var calculation = new TestWaveConditionsCalculation();

                // Call
                string viewName = info.GetViewName(view, calculation);

                // Assert
                Assert.AreEqual("Invoer", viewName);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculation()
        {
            // Setup
            var input = new WaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation();
            var context = new TestWaveConditionsInputContext(input, calculation,
                                                             new ForeshoreProfile[0],
                                                             new HydraulicBoundaryLocation[0]);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreEqual(calculation, viewData);
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationContextDatas),
            new object[]
            {
                "CloseForData_CorrespondingToCalculationContext_ReturnTrue({0})"
            })]
        public void CloseForData_ViewCorrespondingToRemovedCalculationContext_ReturnsTrue(
            ICalculationContext<IWaveConditionsCalculation, IFailureMechanism> context,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationContextDatas),
            new object[]
            {
                "CloseForData_NotCorrespondingToCalculationContext_ReturnFalse({0})"
            })]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationContext_ReturnsFalse(
            ICalculationContext<IWaveConditionsCalculation, IFailureMechanism> context,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            var calculationToRemove = new TestWaveConditionsCalculation();

            using (var view = new WaveConditionsInputView
            {
                Data = calculationToRemove
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculation_ReturnsTrue()
        {
            // Setup
            var calculation = new TestWaveConditionsCalculation();

            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, calculation);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculation_ReturnsFalse()
        {
            // Setup
            var calculation = new TestWaveConditionsCalculation();
            var calculationToRemove = new TestWaveConditionsCalculation();

            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationToRemove);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationGroupDatas),
            new object[]
            {
                "CloseForData_CorrespondingWithCalculationGroupContext_ReturnTrue({0})"
            })]
        public void CloseForData_ViewCorrespondingWithRemovedCalculationGroupContext_ReturnsTrue(
            ICalculationContext<CalculationGroup, IFailureMechanism> context,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationGroupDatas),
            new object[]
            {
                "CloseForData_NotCorrespondingWithCalculationGroupContext_ReturnFalse({0})"
            })]
        public void CloseForData_ViewNotCorrespondingWithRemovedCalculationGroupContext_ReturnsFalse(
            ICalculationContext<CalculationGroup, IFailureMechanism> context,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            var contextToRemove = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(new CalculationGroup(),
                                                                                                     new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                                     new AssessmentSection(AssessmentSectionComposition.Dike));
            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, contextToRemove);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetfailureMechanismContextDatas),
            new object[]
            {
                "CloseForData_CorrespondingWithFailureMechanismContext_ReturnTrue({0})"
            })]
        public void CloseForData_ViewCorrespondingWithRemovedFailureMechanismContext_ReturnsTrue(
            IFailureMechanismContext<IFailureMechanism> context,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetfailureMechanismContextDatas),
            new object[]
            {
                "CloseForData_NotCorrespondingWithFailureMechanismContext_ReturnFalse({0})"
            })]
        public void CloseForData_ViewNotCorrespondingWithRemovedFailureMechanismContext_ReturnsFalse(
            IFailureMechanismContext<IFailureMechanism> context,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view,
                                                      new FailureMechanismContext<IFailureMechanism>(
                                                          new TestFailureMechanism(),
                                                          new AssessmentSection(AssessmentSectionComposition.Dike)));

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetfailureMechanismDatas),
            new object[]
            {
                "CloseForData_CorrespondingWithFailureMechanismContext_ReturnTrue({0})"
            })]
        public void CloseForData_ViewCorrespondingWithRemovedFailureMechanism_ReturnsTrue(
            IFailureMechanism failureMechanism,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetfailureMechanismDatas),
            new object[]
            {
                "CloseForData_NotCorrespondingWithFailureMechanismContext_ReturnFalse({0})"
            })]
        public void CloseForData_ViewNotCorrespondingWithRemovedFailureMechanism_ReturnsFalse(
            IFailureMechanism failureMechanism,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, new TestFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetAssessmentSectionDatas),
            new object[]
            {
                "CloseForData_CorrespondingWithAssessmentSection_ReturnTrue({0})"
            })]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue(AssessmentSection assessmentSection,
                                                                                         IWaveConditionsCalculation calculation)
        {
            // Setup
            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetAssessmentSectionDatas),
            new object[]
            {
                "CloseForData_NotCorrespondingWithAssessmentSection_ReturnFalse({0})"
            })]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse(AssessmentSection assessmentSection,
                                                                                             IWaveConditionsCalculation calculation)
        {
            // Setup
            using (var view = new WaveConditionsInputView
            {
                Data = new TestWaveConditionsCalculation()
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationGroupDatas),
            new object[]
            {
                "CloseForData_NestedCorrespondingWithCalculationGroupContext_ReturnTrue({0})"
            })]
        public void CloseForData_NestedViewCorrespondingWithRemovedCalculationGroupContext_ReturnsTrue(
            ICalculationContext<CalculationGroup, IFailureMechanism> context,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            context.WrappedData.Children.RemoveAt(0);
            context.WrappedData.Children.Add(new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            });

            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationGroupDatas),
            new object[]
            {
                "CloseForData_NestedNotCorrespondingWithCalculationGroupContext_ReturnFalse({0})"
            })]
        public void CloseForData_NestedViewNotCorrespondingWithRemovedCalculationGroupContext_ReturnsFalse(
            ICalculationContext<CalculationGroup, IFailureMechanism> context,
            IWaveConditionsCalculation calculation)
        {
            // Setup
            context.WrappedData.Children.RemoveAt(0);
            context.WrappedData.Children.Add(new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            });

            var contextToRemove = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(new CalculationGroup(),
                                                                                                     new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                                     new AssessmentSection(AssessmentSectionComposition.Dike));
            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, contextToRemove);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        #region TestCaseData

        private static IEnumerable<TestCaseData> GetCalculationContextDatas(string testNameFormat)
        {
            var grassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            yield return new TestCaseData(
                    new GrassCoverErosionOutwardsWaveConditionsCalculationContext(
                        grassCoverErosionOutwardsWaveConditionsCalculation,
                        new GrassCoverErosionOutwardsFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    grassCoverErosionOutwardsWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(GrassCoverErosionOutwardsWaveConditionsCalculation)));

            var stabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    new StabilityStoneCoverWaveConditionsCalculationContext(
                        stabilityStoneCoverWaveConditionsCalculation,
                        new StabilityStoneCoverFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    stabilityStoneCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(StabilityStoneCoverWaveConditionsCalculation)));

            var waveImpactAsphaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    new WaveImpactAsphaltCoverWaveConditionsCalculationContext(
                        waveImpactAsphaltCoverWaveConditionsCalculation,
                        new WaveImpactAsphaltCoverFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    waveImpactAsphaltCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(WaveImpactAsphaltCoverWaveConditionsCalculation)));
        }

        private static IEnumerable<TestCaseData> GetfailureMechanismDatas(string testNameFormat)
        {
            var grassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            yield return new TestCaseData(
                    GetFailureMechanism(grassCoverErosionOutwardsWaveConditionsCalculation),
                    grassCoverErosionOutwardsWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(GrassCoverErosionOutwardsFailureMechanism)));

            var stabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    GetFailureMechanism(stabilityStoneCoverWaveConditionsCalculation),
                    stabilityStoneCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(StabilityStoneCoverFailureMechanism)));

            var waveImpactAsphaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    GetFailureMechanism(waveImpactAsphaltCoverWaveConditionsCalculation),
                    waveImpactAsphaltCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(WaveImpactAsphaltCoverFailureMechanism)));
        }

        private static IEnumerable<TestCaseData> GetfailureMechanismContextDatas(string testNameFormat)
        {
            var grassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            yield return new TestCaseData(
                    new GrassCoverErosionOutwardsFailureMechanismContext(
                        GetFailureMechanism(grassCoverErosionOutwardsWaveConditionsCalculation),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    grassCoverErosionOutwardsWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(GrassCoverErosionOutwardsFailureMechanismContext)));

            var stabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    new StabilityStoneCoverFailureMechanismContext(
                        GetFailureMechanism(stabilityStoneCoverWaveConditionsCalculation),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    stabilityStoneCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(StabilityStoneCoverFailureMechanismContext)));

            var waveImpactAsphaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    new WaveImpactAsphaltCoverFailureMechanismContext(
                        GetFailureMechanism(waveImpactAsphaltCoverWaveConditionsCalculation),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    waveImpactAsphaltCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(WaveImpactAsphaltCoverFailureMechanismContext)));
        }

        private static IEnumerable<TestCaseData> GetCalculationGroupDatas(string testNameFormat)
        {
            var grassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            yield return new TestCaseData(
                    new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(
                        new CalculationGroup
                        {
                            Children =
                            {
                                grassCoverErosionOutwardsWaveConditionsCalculation
                            }
                        },
                        new GrassCoverErosionOutwardsFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    grassCoverErosionOutwardsWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext)));

            var stabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    new StabilityStoneCoverWaveConditionsCalculationGroupContext(
                        new CalculationGroup
                        {
                            Children =
                            {
                                stabilityStoneCoverWaveConditionsCalculation
                            }
                        },
                        new StabilityStoneCoverFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    stabilityStoneCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(StabilityStoneCoverWaveConditionsCalculationGroupContext)));

            var waveImpactAsphaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(
                        new CalculationGroup
                        {
                            Children =
                            {
                                waveImpactAsphaltCoverWaveConditionsCalculation
                            }
                        },
                        new WaveImpactAsphaltCoverFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    waveImpactAsphaltCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext)));
        }

        private static IEnumerable<TestCaseData> GetAssessmentSectionDatas(string testNameFormat)
        {
            var grassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            yield return new TestCaseData(
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        GrassCoverErosionOutwards =
                        {
                            WaveConditionsCalculationGroup =
                            {
                                Children =
                                {
                                    grassCoverErosionOutwardsWaveConditionsCalculation
                                }
                            }
                        }
                    },
                    grassCoverErosionOutwardsWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       "assessmentSectionGrassCoverErosionOutwards"));

            var stabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        StabilityStoneCover =
                        {
                            WaveConditionsCalculationGroup =
                            {
                                Children =
                                {
                                    stabilityStoneCoverWaveConditionsCalculation
                                }
                            }
                        }
                    },
                    stabilityStoneCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       "assessmentSectionStabilityStoneCover"));

            var waveImpactAsphaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            yield return new TestCaseData(
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        WaveImpactAsphaltCover =
                        {
                            WaveConditionsCalculationGroup =
                            {
                                Children =
                                {
                                    waveImpactAsphaltCoverWaveConditionsCalculation
                                }
                            }
                        }
                    }, waveImpactAsphaltCoverWaveConditionsCalculation)
                .SetName(string.Format(testNameFormat,
                                       "assessmentSectionWaveImpactAsphaltCover"));
        }

        private static GrassCoverErosionOutwardsFailureMechanism GetFailureMechanism(GrassCoverErosionOutwardsWaveConditionsCalculation calculation)
        {
            return new GrassCoverErosionOutwardsFailureMechanism
            {
                WaveConditionsCalculationGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
        }

        private static StabilityStoneCoverFailureMechanism GetFailureMechanism(StabilityStoneCoverWaveConditionsCalculation calculation)
        {
            return new StabilityStoneCoverFailureMechanism
            {
                WaveConditionsCalculationGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
        }

        private static WaveImpactAsphaltCoverFailureMechanism GetFailureMechanism(WaveImpactAsphaltCoverWaveConditionsCalculation calculation)
        {
            return new WaveImpactAsphaltCoverFailureMechanism
            {
                WaveConditionsCalculationGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
        }

        #endregion
    }
}