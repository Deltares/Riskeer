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
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
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
            Assert.AreEqual(typeof(CommentContext<ICommentable>), info.DataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var commentMock = mocks.StrictMock<ICommentable>();
            var viewMock = mocks.StrictMock<CommentView>();

            mocks.ReplayAll();

            // Call
            var viewName = info.GetViewName(viewMock, commentMock);

            // Assert
            Assert.AreEqual("Opmerkingen", viewName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewData_Always_ReturnsIComment()
        {
            // Setup
            var commentMock = mocks.StrictMock<ICommentable>();
            var contextMock = mocks.StrictMock<CommentContext<ICommentable>>(commentMock);
            mocks.ReplayAll();

            // Call
            var viewData = info.GetViewData(contextMock);

            // Assert
            Assert.AreSame(commentMock, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(CommentView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(CommentContext<ICommentable>), dataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(ICommentable), viewDataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.EditDocumentIcon, image);
        }

        [Test]
        public void CloseForData_ObjectIsNotObjectOfInterest_ReturnFalse()
        {
            // Setup
            var commentableMock = mocks.Stub<ICommentable>();
            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = commentableMock
            })
            {
                // Call
                var closeForData = info.CloseForData(view, new object());

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
            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = assessmentSection
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

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

            var viewDataAssessmentSection = mocks.Stub<IAssessmentSection>();
            viewDataAssessmentSection.Stub(s => s.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataAssessmentSection
            })
            {
                // Call
                var closeForData = info.CloseForData(view, deletedAssessmentSection);

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

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation
            });
            failureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = calculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

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

            var deletedCalculation = mocks.Stub<ICalculation>();

            var deletedfailureMechanism = mocks.Stub<IFailureMechanism>();
            deletedfailureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                deletedCalculation
            });
            deletedfailureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            deletedfailureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            deletedfailureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            var deletedAssessmentSection = mocks.Stub<IAssessmentSection>();
            deletedAssessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                deletedfailureMechanism
            });
            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataCalculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, deletedAssessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsInputCommentableOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var commentable = new Commentable();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(commentable);
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = commentable
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCommentableButNotOfDeletedAssessmentSection_ReturnFalse()
        {
            // Setup
            var viewDataCommentable = new Commentable();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataCommentable
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsOutputCommentableOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var commentable = new Commentable();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.OutputComments).Return(commentable);
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = commentable
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsNotRelevantCommentableOfDeletedAssessmentSection_ReturnTrue()
        {
            // Setup
            var commentable = new Commentable();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(commentable);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(s => s.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = commentable
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

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
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var failureMechanismContext = mocks.Stub<IFailureMechanismContext<IFailureMechanism>>();

            failureMechanismContext.Expect(c => c.WrappedData).Return(failureMechanism);
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation
            });
            failureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = calculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, failureMechanismContext);

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
            var deletedCalculation = mocks.Stub<ICalculation>();

            var deletedfailureMechanism = mocks.Stub<IFailureMechanism>();
            var failureMechanismContext = mocks.Stub<IFailureMechanismContext<IFailureMechanism>>();

            failureMechanismContext.Stub(c => c.WrappedData).Return(deletedfailureMechanism);
            deletedfailureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                deletedCalculation
            });
            deletedfailureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            deletedfailureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            deletedfailureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataCalculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, failureMechanismContext);

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
            var failureMechanism = mocks.Stub<IFailureMechanism>();

            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation
            });
            failureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = calculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, failureMechanism);

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
            var deletedCalculation = mocks.Stub<ICalculation>();

            var failureMechanism = mocks.Stub<IFailureMechanism>();

            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                deletedCalculation
            });
            failureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataCalculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewDataIsCommentableButNotOfDeletedFailureMechanism_ReturnFalse()
        {
            // Setup
            var viewDataCommentable = new Commentable();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(Enumerable.Empty<ICalculation>());
            failureMechanism.Stub(fm => fm.InputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.OutputComments).Return(new Commentable());
            failureMechanism.Stub(fm => fm.NotRelevantComments).Return(new Commentable());

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataCommentable
            })
            {
                // Call
                var closeForData = info.CloseForData(view, failureMechanism);

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
            var deletedCalculationContext = mocks.StrictMock<ICalculationContext<ICalculationBase, IFailureMechanism>>();

            deletedCalculationContext.Expect(c => c.WrappedData).Return(viewDataCalculation);

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataCalculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, deletedCalculationContext);

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
            var viewDataCalculation = mocks.Stub<ICalculation>();
            var deletedCalculationContext = mocks.StrictMock<ICalculationContext<ICalculationBase, IFailureMechanism>>();

            deletedCalculationContext.Expect(c => c.WrappedData).Return(calculation);

            mocks.ReplayAll();

            using (var view = new CommentView
            {
                Data = viewDataCalculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, deletedCalculationContext);

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
                Data = viewDataCalculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, deletedGroupContext);

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
            var deletedCalculation = mocks.Stub<ICalculation>();
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
                Data = viewDataCalculation
            })
            {
                // Call
                var closeForData = info.CloseForData(view, deletedGroupContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }
    }
}