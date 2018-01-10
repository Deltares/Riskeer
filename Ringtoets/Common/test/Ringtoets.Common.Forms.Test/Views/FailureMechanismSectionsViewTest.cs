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

using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionsViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new FailureMechanismSectionsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(1, view.Controls.Count);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddEmptyTableControl()
        {
            // Call
            using (var view = new FailureMechanismSectionsView())
            {
                // Assert
                FailureMechanismSectionsTable tableControl = GetSectionsTable(view);
                Assert.NotNull(tableControl);
                Assert.AreEqual(DockStyle.Fill, tableControl.Dock);
                CollectionAssert.IsEmpty(tableControl.Rows);
            }
        }

        [Test]
        public void Data_FailureMechanismSectionsContext_DataSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();

            mocks.ReplayAll();

            using (var view = new FailureMechanismSectionsView())
            {
                var context = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

                // Call
                view.Data = context;

                // Assert
                Assert.AreSame(context, view.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Data_OtherThanFailureMechanismSectionsContext_DataNull()
        {
            // Setup
            using (var view = new FailureMechanismSectionsView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_SetToNull_TableDataCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var random = new Random(39);
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            }));
            var context = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

            using (var view = new FailureMechanismSectionsView
            {
                Data = context
            })
            {
                FailureMechanismSectionsTable sectionsTable = GetSectionsTable(view);

                // Precondition
                Assert.NotNull(sectionsTable);
                Assert.AreEqual(1, sectionsTable.Rows.Count);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                CollectionAssert.IsEmpty(sectionsTable.Rows);
            }
        }

        [Test]
        public void GivenViewWithSections_WhenSectionsUpdated_ThenDataTableUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0.0, 0.0)
            }));
            var context = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var view = new FailureMechanismSectionsView
            {
                Data = context
            })
            {
                FailureMechanismSectionsTable sectionsTable = GetSectionsTable(view);

                // Precondition
                Assert.AreEqual(1, sectionsTable.Rows.Count);

                // When
                context.WrappedData.Attach(observer);
                context.WrappedData.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0.0, 0.0)
                }));
                context.WrappedData.NotifyObservers();

                // Then
                Assert.AreEqual(2, sectionsTable.Rows.Count);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithSections_WhenSectionsCleared_ThenDataTableCleared()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0.0, 0.0)
            }));
            var context = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var view = new FailureMechanismSectionsView
            {
                Data = context
            })
            {
                FailureMechanismSectionsTable sectionsTable = GetSectionsTable(view);

                // Precondition
                Assert.AreEqual(1, sectionsTable.Rows.Count);

                // When
                context.WrappedData.Attach(observer);
                context.WrappedData.ClearAllSections();
                context.WrappedData.NotifyObservers();

                // Then
                Assert.AreEqual(0, sectionsTable.Rows.Count);
                mocks.VerifyAll();
            }
        }

        private static FailureMechanismSectionsTable GetSectionsTable(FailureMechanismSectionsView view)
        {
            return ControlTestHelper.GetControls<FailureMechanismSectionsTable>(view, "failureMechanismSectionsTable").Single();
        }
    }
}