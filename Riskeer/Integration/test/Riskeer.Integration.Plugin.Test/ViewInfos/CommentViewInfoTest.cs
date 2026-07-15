// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Gui.Plugin;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class CommentViewInfoTest
    {
        
        private RiskeerPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            
            plugin = new RiskeerPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(CommentView));
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
            Assert.AreEqual(typeof(Comment), info.DataType);
            Assert.AreEqual(typeof(Comment), info.ViewDataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Opmerkingen", viewName);
        }

        [Test]
        public void GetViewData_Always_ReturnsComment()
        {
            // Setup
            var comment = new Comment();

            // Call
            object viewData = info.GetViewData(comment);

            // Assert
            Assert.AreSame(comment, viewData);
        }

        [Test]
        public void CloseForData_ObjectIsNotObjectOfInterest_ReturnFalse()
        {
            // Setup
            var comment = new Comment();
            using (var view = new CommentView
            {
                Data = comment
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.GetFailureMechanisms().Returns(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = assessmentSection.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsOtherInstanceThanDeletedAssessmentSection_ReturnFalse()
        {
            // Setup
            var deletedAssessmentSection = Substitute.For<IAssessmentSection>();
            deletedAssessmentSection.SpecificFailureMechanisms.Returns(new ObservableList<SpecificFailureMechanism>());
            deletedAssessmentSection.GetFailureMechanisms().Returns(Enumerable.Empty<IFailureMechanism>());
            deletedAssessmentSection.Comments.Returns(new Comment());

            var viewDataAssessmentSection = Substitute.For<IAssessmentSection>();
            viewDataAssessmentSection.GetFailureMechanisms().Returns(Enumerable.Empty<IFailureMechanism>());
            viewDataAssessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = viewDataAssessmentSection.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, deletedAssessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsFailureMechanismCalculationOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var calculation = Substitute.For<ICalculation>();
            calculation.Comments.Returns(new Comment());

            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(new[]
            {
                calculation
            });
            failureMechanism.InAssemblyInputComments.Returns(new Comment());
            failureMechanism.InAssemblyOutputComments.Returns(new Comment());
            failureMechanism.NotInAssemblyComments.Returns(new Comment());
            failureMechanism.CalculationsInputComments.Returns(new Comment());

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.GetFailureMechanisms().Returns(new[]
            {
                failureMechanism
            });
            assessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = calculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsCommentButNotOfDeletedAssessmentSection_ReturnFalse()
        {
            // Setup
            var viewDataComment = new Comment();

            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(Enumerable.Empty<ICalculation>());
            failureMechanism.InAssemblyInputComments.Returns(new Comment());
            failureMechanism.InAssemblyOutputComments.Returns(new Comment());
            failureMechanism.NotInAssemblyComments.Returns(new Comment());
            failureMechanism.CalculationsInputComments.Returns(new Comment());

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.SpecificFailureMechanisms.Returns(new ObservableList<SpecificFailureMechanism>());
            assessmentSection.GetFailureMechanisms().Returns(new[]
            {
                failureMechanism
            });
            assessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = viewDataComment
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsFailureMechanismCalculationButNotOfDeletedAssessmentSection_ReturnFalse()
        {
            // Setup
            var viewDataCalculation = Substitute.For<ICalculation>();
            viewDataCalculation.Comments.Returns(new Comment());

            var deletedCalculation = Substitute.For<ICalculation>();

            var deletedFailureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            deletedFailureMechanism.Calculations.Returns(new[]
            {
                deletedCalculation
            });
            deletedFailureMechanism.InAssemblyInputComments.Returns(new Comment());
            deletedFailureMechanism.InAssemblyOutputComments.Returns(new Comment());
            deletedFailureMechanism.NotInAssemblyComments.Returns(new Comment());
            deletedFailureMechanism.CalculationsInputComments.Returns(new Comment());

            var deletedAssessmentSection = Substitute.For<IAssessmentSection>();
            deletedAssessmentSection.SpecificFailureMechanisms.Returns(new ObservableList<SpecificFailureMechanism>());
            deletedAssessmentSection.GetFailureMechanisms().Returns(new[]
            {
                deletedFailureMechanism
            });
            deletedAssessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = viewDataCalculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, deletedAssessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsFailureMechanismInputCommentOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var comment = new Comment();

            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(Enumerable.Empty<ICalculation>());
            failureMechanism.InAssemblyInputComments.Returns(comment);
            failureMechanism.InAssemblyOutputComments.Returns(new Comment());
            failureMechanism.NotInAssemblyComments.Returns(new Comment());

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.GetFailureMechanisms().Returns(new[]
            {
                failureMechanism
            });
            assessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = comment
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsFailureMechanismOutputCommentOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var comment = new Comment();

            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(Enumerable.Empty<ICalculation>());
            failureMechanism.InAssemblyInputComments.Returns(new Comment());
            failureMechanism.InAssemblyOutputComments.Returns(comment);
            failureMechanism.NotInAssemblyComments.Returns(new Comment());

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.GetFailureMechanisms().Returns(new[]
            {
                failureMechanism
            });
            assessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = comment
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsFailureMechanismNotInAssemblyCommentOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var comment = new Comment();

            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(Enumerable.Empty<ICalculation>());
            failureMechanism.InAssemblyInputComments.Returns(new Comment());
            failureMechanism.InAssemblyOutputComments.Returns(new Comment());
            failureMechanism.NotInAssemblyComments.Returns(comment);

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.GetFailureMechanisms().Returns(new[]
            {
                failureMechanism
            });
            assessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = comment
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsFailureMechanismCalculationsInputCommentsOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var comment = new Comment();

            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(Enumerable.Empty<ICalculation>());
            failureMechanism.InAssemblyInputComments.Returns(new Comment());
            failureMechanism.InAssemblyOutputComments.Returns(new Comment());
            failureMechanism.NotInAssemblyComments.Returns(new Comment());
            failureMechanism.CalculationsInputComments.Returns(comment);

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.GetFailureMechanisms().Returns(new[]
            {
                failureMechanism
            });
            assessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = comment
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsCommentButNotOfDeletedFailureMechanism_ReturnFalse()
        {
            // Setup
            var viewDataComment = new Comment();

            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(Enumerable.Empty<ICalculation>());
            failureMechanism.InAssemblyInputComments.Returns(new Comment());
            failureMechanism.InAssemblyOutputComments.Returns(new Comment());
            failureMechanism.NotInAssemblyComments.Returns(new Comment());
            failureMechanism.CalculationsInputComments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = viewDataComment
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsFailureMechanismCommentButNotOfDeletedAssessmentSection_ReturnFalse()
        {
            // Setup
            var viewDataComment = Substitute.For<IFailureMechanism>();
            viewDataComment.InAssemblyInputComments.Returns(new Comment());

            var deletedAssessmentSection = Substitute.For<IAssessmentSection>();
            deletedAssessmentSection.GetFailureMechanisms().Returns(Enumerable.Empty<IFailureMechanism>());
            deletedAssessmentSection.SpecificFailureMechanisms.Returns(new ObservableList<SpecificFailureMechanism>
            {
                new SpecificFailureMechanism()
            });
            deletedAssessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = viewDataComment.InAssemblyInputComments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, deletedAssessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismCommentTestCases))]
        public void CloseForData_ViewDataIsFailureMechanismCommentOfDeletedAssessmentSection_ReturnTrue(Func<SpecificFailureMechanism, Comment> getCommentFunc)
        {
            // Setup
            var failureMechanism = new SpecificFailureMechanism();

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.GetFailureMechanisms().Returns(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.SpecificFailureMechanisms.Returns(new ObservableList<SpecificFailureMechanism>
            {
                failureMechanism
            });
            assessmentSection.Comments.Returns(new Comment());
            using (var view = new CommentView
            {
                Data = getCommentFunc(failureMechanism)
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsICommentOfDeletedFailureMechanismContext_ReturnTrue()
        {
            // Setup
            var affectedComment = new Comment();
            var failureMechanism = Substitute.For<IFailureMechanism>();
            failureMechanism.InAssemblyInputComments.Returns(affectedComment);
            failureMechanism.InAssemblyOutputComments.Returns(new Comment());
            failureMechanism.NotInAssemblyComments.Returns(new Comment());
            var failureMechanismContext = Substitute.For<IFailureMechanismContext<IFailureMechanism>>();

            failureMechanismContext.WrappedData.Returns(failureMechanism);
            using (var view = new CommentView
            {
                Data = affectedComment
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsCommentButNotOfDeletedFailureMechanismContext_ReturnFalse()
        {
            // Setup
            var unaffectedComment = new Comment();

            var failureMechanism = Substitute.For<IFailureMechanism>();
            failureMechanism.InAssemblyInputComments.Returns(new Comment());
            failureMechanism.InAssemblyOutputComments.Returns(new Comment());
            failureMechanism.NotInAssemblyComments.Returns(new Comment());
            var failureMechanismContext = Substitute.For<IFailureMechanismContext<IFailureMechanism>>();

            failureMechanismContext.WrappedData.Returns(failureMechanism);
            using (var view = new CommentView
            {
                Data = unaffectedComment
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculationItem_ReturnsTrue()
        {
            // Setup
            var viewDataCalculation = Substitute.For<ICalculation>();
            viewDataCalculation.Comments.Returns(new Comment());
            var deletedCalculationContext = Substitute.For<ICalculationContext<ICalculationBase, ICalculatableFailureMechanism>>();

            deletedCalculationContext.WrappedData.Returns(viewDataCalculation);
            using (var view = new CommentView
            {
                Data = viewDataCalculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, deletedCalculationContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationItem_ReturnsFalse()
        {
            // Setup
            var calculation = Substitute.For<ICalculation>();
            calculation.Comments.Returns(new Comment());
            var viewDataCalculation = Substitute.For<ICalculation>();
            viewDataCalculation.Comments.Returns(new Comment());
            var deletedCalculationContext = Substitute.For<ICalculationContext<ICalculationBase, ICalculatableFailureMechanism>>();
            deletedCalculationContext.WrappedData.Returns(calculation);
            using (var view = new CommentView
            {
                Data = viewDataCalculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, deletedCalculationContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationOfRemovedCalculationGroup_ReturnsTrue()
        {
            // Setup
            var viewDataCalculation = Substitute.For<ICalculation>();
            viewDataCalculation.Comments.Returns(new Comment());
            var deletedGroupContext = Substitute.For<ICalculationContext<CalculationGroup, ICalculatableFailureMechanism>>();
            var deletedGroup = new CalculationGroup
            {
                Children =
                {
                    viewDataCalculation
                }
            };

            deletedGroupContext.WrappedData.Returns(deletedGroup);
            using (var view = new CommentView
            {
                Data = viewDataCalculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, deletedGroupContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationButNotOfRemovedCalculationGroup_ReturnsFalse()
        {
            // Setup
            var viewDataCalculation = Substitute.For<ICalculation>();
            viewDataCalculation.Comments.Returns(new Comment());
            var deletedCalculation = Substitute.For<ICalculation>();
            deletedCalculation.Comments.Returns(new Comment());
            var deletedGroupContext = Substitute.For<ICalculationContext<CalculationGroup, ICalculatableFailureMechanism>>();
            var deletedGroup = new CalculationGroup
            {
                Children =
                {
                    deletedCalculation
                }
            };

            deletedGroupContext.WrappedData.Returns(deletedGroup);
            using (var view = new CommentView
            {
                Data = viewDataCalculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, deletedGroupContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismCommentTestCases()
        {
            yield return new TestCaseData(new Func<SpecificFailureMechanism, Comment>(fp => fp.InAssemblyInputComments));
            yield return new TestCaseData(new Func<SpecificFailureMechanism, Comment>(fp => fp.InAssemblyOutputComments));
            yield return new TestCaseData(new Func<SpecificFailureMechanism, Comment>(fp => fp.NotInAssemblyComments));
        }
    }
}