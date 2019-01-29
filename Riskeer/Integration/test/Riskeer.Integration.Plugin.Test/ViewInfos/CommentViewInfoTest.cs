// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class CommentViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsPlugin();
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
        public void Image_Always_ReturnsEditDocumentIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.EditDocumentIcon, image);
        }

        [Test]
        public void CloseForData_ObjectIsNotObjectOfInterest_ReturnFalse()
        {
            // Setup
            var comment = new Comment();
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(s => s.Comments).Return(new Comment());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsOtherInstanceThanDeletedAssessmentSection_ReturnFalse()
        {
            // Setup
            var deletedAssessmentSection = mocks.Stub<IAssessmentSection>();
            deletedAssessmentSection.Stub(s => s.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            deletedAssessmentSection.Stub(s => s.Comments).Return(new Comment());

            var viewDataAssessmentSection = mocks.Stub<IAssessmentSection>();
            viewDataAssessmentSection.Stub(s => s.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            viewDataAssessmentSection.Stub(s => s.Comments).Return(new Comment());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var calculation = mocks.Stub<ICalculation>();
            calculation.Stub(s => s.Comments).Return(new Comment());

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation
            });
            failureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(s => s.Comments).Return(new Comment());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationButNotOfDeletedAssessmentSection_ReturnFalse()
        {
            // Setup
            var viewDataCalculation = mocks.Stub<ICalculation>();
            viewDataCalculation.Stub(s => s.Comments).Return(new Comment());

            var deletedCalculation = mocks.Stub<ICalculation>();

            var deletedfailureMechanism = mocks.Stub<IFailureMechanism>();
            deletedfailureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                deletedCalculation
            });
            deletedfailureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            deletedfailureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            deletedfailureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            var deletedAssessmentSection = mocks.Stub<IAssessmentSection>();
            deletedAssessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                deletedfailureMechanism
            });
            deletedAssessmentSection.Stub(s => s.Comments).Return(new Comment());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsInputCommentOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var comment = new Comment();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(comment);
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(s => s.Comments).Return(new Comment());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCommentButNotOfDeletedAssessmentSection_ReturnFalse()
        {
            // Setup
            var viewDataComment = new Comment();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(s => s.Comments).Return(new Comment());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsOutputCommentOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var comment = new Comment();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.OutputComments).Return(comment);
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(s => s.Comments).Return(new Comment());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsNotRelevantCommentOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var comment = new Comment();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(comment);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(s => s.Comments).Return(new Comment());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationOfDeletedFailureMechanismContext_ReturnTrue()
        {
            // Setup
            var calculation = mocks.Stub<ICalculation>();
            calculation.Stub(s => s.Comments).Return(new Comment());
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var failureMechanismContext = mocks.Stub<IFailureMechanismContext<IFailureMechanism>>();

            failureMechanismContext.Expect(c => c.WrappedData).Return(failureMechanism);
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation
            });
            failureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = calculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationButNotOfDeletedFailureMechanismContext_ReturnFalse()
        {
            // Setup
            var viewDataCalculation = mocks.Stub<ICalculation>();
            viewDataCalculation.Stub(s => s.Comments).Return(new Comment());
            var deletedCalculation = mocks.Stub<ICalculation>();
            deletedCalculation.Stub(s => s.Comments).Return(new Comment());

            var deletedfailureMechanism = mocks.Stub<IFailureMechanism>();
            var failureMechanismContext = mocks.Stub<IFailureMechanismContext<IFailureMechanism>>();

            failureMechanismContext.Stub(c => c.WrappedData).Return(deletedfailureMechanism);
            deletedfailureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                deletedCalculation
            });
            deletedfailureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            deletedfailureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            deletedfailureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataCalculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationOfDeletedFailureMechanism_ReturnTrue()
        {
            // Setup
            var calculation = mocks.Stub<ICalculation>();
            calculation.Stub(s => s.Comments).Return(new Comment());
            var failureMechanism = mocks.Stub<IFailureMechanism>();

            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation
            });
            failureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = calculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationButNotOfDeletedFailureMechanism_ReturnFalse()
        {
            // Setup
            var viewDataCalculation = mocks.Stub<ICalculation>();
            viewDataCalculation.Stub(s => s.Comments).Return(new Comment());
            var deletedCalculation = mocks.Stub<ICalculation>();

            var failureMechanism = mocks.Stub<IFailureMechanism>();

            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                deletedCalculation
            });
            failureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataCalculation.Comments
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCommentButNotOfDeletedFailureMechanism_ReturnFalse()
        {
            // Setup
            var viewDataComment = new Comment();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Comment());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Comment());

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculationItem_ReturnsTrue()
        {
            // Setup
            var viewDataCalculation = mocks.Stub<ICalculation>();
            viewDataCalculation.Stub(c => c.Comments).Return(new Comment());
            var deletedCalculationContext = mocks.StrictMock<ICalculationContext<ICalculationBase, IFailureMechanism>>();

            deletedCalculationContext.Expect(c => c.WrappedData).Return(viewDataCalculation);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationItem_ReturnsFalse()
        {
            // Setup
            var calculation = mocks.Stub<ICalculation>();
            calculation.Stub(s => s.Comments).Return(new Comment());
            var viewDataCalculation = mocks.Stub<ICalculation>();
            viewDataCalculation.Stub(s => s.Comments).Return(new Comment());
            var deletedCalculationContext = mocks.StrictMock<ICalculationContext<ICalculationBase, IFailureMechanism>>();

            deletedCalculationContext.Expect(c => c.WrappedData).Return(calculation);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationOfRemovedCalculationGroup_ReturnsTrue()
        {
            // Setup
            var viewDataCalculation = mocks.Stub<ICalculation>();
            viewDataCalculation.Stub(s => s.Comments).Return(new Comment());
            var deletedGroupContext = mocks.StrictMock<ICalculationContext<CalculationGroup, IFailureMechanism>>();
            var deletedGroup = new CalculationGroup
            {
                Children =
                {
                    viewDataCalculation
                }
            };

            deletedGroupContext.Expect(g => g.WrappedData).Return(deletedGroup);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCalculationButNotOfRemovedCalculationGroup_ReturnsFalse()
        {
            // Setup
            var viewDataCalculation = mocks.Stub<ICalculation>();
            viewDataCalculation.Stub(s => s.Comments).Return(new Comment());
            var deletedCalculation = mocks.Stub<ICalculation>();
            deletedCalculation.Stub(s => s.Comments).Return(new Comment());
            var deletedGroupContext = mocks.StrictMock<ICalculationContext<CalculationGroup, IFailureMechanism>>();
            var deletedGroup = new CalculationGroup
            {
                Children =
                {
                    deletedCalculation
                }
            };

            deletedGroupContext.Expect(g => g.WrappedData).Return(deletedGroup);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }
    }
}