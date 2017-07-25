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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GeneralResultFaultTreeIllustrationPointViewInfoTest
    {
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GeneralResultFaultTreeIllustrationPointView));
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
            Assert.AreEqual(typeof(StructuresOutputContext), info.DataType);
            Assert.AreEqual(typeof(IStructuresCalculation), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralOutputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsCalculationOutputDisplayName()
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint()))
            {
                var calculation = new TestStructuresCalculation();

                // Call
                string viewName = info.GetViewName(view, calculation);

                // Assert
                Assert.AreEqual("Resultaat", viewName);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedStructuresCalculation()
        {
            // Setup
            var calculation = new TestStructuresCalculation();
            var context = new StructuresOutputContext(calculation);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculation, viewData);
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationContextDatas),
            new object[]
            {
                "CloseForData_CorrespondingToCalculationContext_ReturnTrue({0})"
            })]
        public void CloseForData_ViewCorrespondingToRemovedCalculationContext_ReturnsTrue(
            ICalculationContext<IStructuresCalculation, IFailureMechanism> context,
            IStructuresCalculation calculation)
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
            ICalculationContext<IStructuresCalculation, IFailureMechanism> context,
            IStructuresCalculation calculation)
        {
            // Setup
            var calculationToRemove = new TestStructuresCalculation();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
        [TestCaseSource(nameof(GetCalculationGroupDatas),
            new object[]
            {
                "CloseForData_CorrespondingWithCalculationGroupContext_ReturnTrue({0})"
            })]
        public void CloseForData_ViewCorrespondingWithRemovedCalculationGroupContext_ReturnsTrue(
            ICalculationContext<CalculationGroup, IFailureMechanism> context,
            IStructuresCalculation calculation)
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
            IStructuresCalculation calculation)
        {
            // Setup
            var contextToRemove = new HeightStructuresCalculationGroupContext(new CalculationGroup(),
                                                                              new HeightStructuresFailureMechanism(),
                                                                              new AssessmentSection(AssessmentSectionComposition.Dike));
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
        [TestCaseSource(nameof(GetFailureMechanismContextDatas),
            new object[]
            {
                "CloseForData_CorrespondingWithFailureMechanismContext_ReturnTrue({0})"
            })]
        public void CloseForData_ViewCorrespondingWithRemovedFailureMechanismContext_ReturnsTrue(
            IFailureMechanismContext<IFailureMechanism> context,
            IStructuresCalculation calculation)
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
        [TestCaseSource(nameof(GetFailureMechanismContextDatas),
            new object[]
            {
                "CloseForData_NotCorrespondingWithFailureMechanismContext_ReturnFalse({0})"
            })]
        public void CloseForData_ViewNotCorrespondingWithRemovedFailureMechanismContext_ReturnsFalse(
            IFailureMechanismContext<IFailureMechanism> context,
            IStructuresCalculation calculation)
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
        [TestCaseSource(nameof(GetFailureMechanismDatas),
            new object[]
            {
                "CloseForData_CorrespondingWithFailureMechanism_ReturnTrue({0})"
            })]
        public void CloseForData_ViewCorrespondingWithRemovedFailureMechanism_ReturnsTrue(
            IFailureMechanism failureMechanism,
            IStructuresCalculation calculation)
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
        [TestCaseSource(nameof(GetFailureMechanismDatas),
            new object[]
            {
                "CloseForData_NotCorrespondingWithFailureMechanism_ReturnFalse({0})"
            })]
        public void CloseForData_ViewNotCorrespondingWithRemovedFailureMechanism_ReturnsFalse(
            IFailureMechanism failureMechanism,
            IStructuresCalculation calculation)
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
                                                                                         IStructuresCalculation calculation)
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
                                                                                             IStructuresCalculation calculation)
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
            {
                Data = new TestStructuresCalculation()
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
            IStructuresCalculation calculation)
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

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
            IStructuresCalculation calculation)
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

            var contextToRemove = new HeightStructuresCalculationGroupContext(new CalculationGroup(),
                                                                              new HeightStructuresFailureMechanism(),
                                                                              new AssessmentSection(AssessmentSectionComposition.Dike));
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
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
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            yield return new TestCaseData(
                    new HeightStructuresCalculationContext(
                        heightStructuresCalculation,
                        new HeightStructuresFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    heightStructuresCalculation)
                .SetName(string.Format(testNameFormat, "HeightStructuresCalculation"));

            var closingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>();
            yield return new TestCaseData(
                    new ClosingStructuresCalculationContext(
                        closingStructuresCalculation,
                        new ClosingStructuresFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    closingStructuresCalculation)
                .SetName(string.Format(testNameFormat, "ClosingStructuresCalculation"));

            var stabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            yield return new TestCaseData(
                    new StabilityPointStructuresCalculationContext(
                        stabilityPointStructuresCalculation,
                        new StabilityPointStructuresFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    stabilityPointStructuresCalculation)
                .SetName(string.Format(testNameFormat, "StabilityPointStructuresCalculation"));
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismDatas(string testNameFormat)
        {
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            yield return new TestCaseData(
                    GetFailureMechanism(heightStructuresCalculation),
                    heightStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(HeightStructuresFailureMechanism)));

            var closingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>();
            yield return new TestCaseData(
                    GetFailureMechanism(closingStructuresCalculation),
                    closingStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(ClosingStructuresFailureMechanism)));

            var stabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            yield return new TestCaseData(
                    GetFailureMechanism(stabilityPointStructuresCalculation),
                    stabilityPointStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(StabilityPointStructuresFailureMechanism)));
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismContextDatas(string testNameFormat)
        {
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            yield return new TestCaseData(
                    new HeightStructuresFailureMechanismContext(
                        GetFailureMechanism(heightStructuresCalculation),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    heightStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(HeightStructuresFailureMechanismContext)));

            var closingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>();
            yield return new TestCaseData(
                    new ClosingStructuresFailureMechanismContext(
                        GetFailureMechanism(closingStructuresCalculation),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    closingStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(ClosingStructuresFailureMechanismContext)));

            var stabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            yield return new TestCaseData(
                    new StabilityPointStructuresFailureMechanismContext(
                        GetFailureMechanism(stabilityPointStructuresCalculation),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    stabilityPointStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(StabilityPointStructuresFailureMechanismContext)));
        }

        private static IEnumerable<TestCaseData> GetCalculationGroupDatas(string testNameFormat)
        {
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            yield return new TestCaseData(
                    new HeightStructuresCalculationGroupContext(
                        new CalculationGroup
                        {
                            Children =
                            {
                                heightStructuresCalculation
                            }
                        },
                        new HeightStructuresFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    heightStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(HeightStructuresCalculationGroupContext)));

            var closingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>();
            yield return new TestCaseData(
                    new ClosingStructuresCalculationGroupContext(
                        new CalculationGroup
                        {
                            Children =
                            {
                                closingStructuresCalculation
                            }
                        },
                        new ClosingStructuresFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    closingStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(ClosingStructuresCalculationGroupContext)));

            var stabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            yield return new TestCaseData(
                    new StabilityPointStructuresCalculationGroupContext(
                        new CalculationGroup
                        {
                            Children =
                            {
                                stabilityPointStructuresCalculation
                            }
                        },
                        new StabilityPointStructuresFailureMechanism(),
                        new AssessmentSection(AssessmentSectionComposition.Dike)),
                    stabilityPointStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       nameof(StabilityPointStructuresCalculationGroupContext)));
        }

        private static IEnumerable<TestCaseData> GetAssessmentSectionDatas(string testNameFormat)
        {
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            yield return new TestCaseData(
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        HeightStructures =
                        {
                            CalculationsGroup =
                            {
                                Children =
                                {
                                    heightStructuresCalculation
                                }
                            }
                        }
                    },
                    heightStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       "assessmentSectionHeightStructures"));

            var closingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>();
            yield return new TestCaseData(
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        ClosingStructures =
                        {
                            CalculationsGroup =
                            {
                                Children =
                                {
                                    closingStructuresCalculation
                                }
                            }
                        }
                    },
                    closingStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       "assessmentSectionClosingStructures"));

            var stabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            yield return new TestCaseData(
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        StabilityPointStructures =
                        {
                            CalculationsGroup =
                            {
                                Children =
                                {
                                    stabilityPointStructuresCalculation
                                }
                            }
                        }
                    },
                    stabilityPointStructuresCalculation)
                .SetName(string.Format(testNameFormat,
                                       "assessmentSectionStabilityPointStructures"));
        }

        private static HeightStructuresFailureMechanism GetFailureMechanism(StructuresCalculation<HeightStructuresInput> calculation)
        {
            return new HeightStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
        }

        private static ClosingStructuresFailureMechanism GetFailureMechanism(StructuresCalculation<ClosingStructuresInput> calculation)
        {
            return new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
        }

        private static StabilityPointStructuresFailureMechanism GetFailureMechanism(StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            return new StabilityPointStructuresFailureMechanism
            {
                CalculationsGroup =
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