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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionsViewTest
    {
        [Test]
        public void Constructor_SectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new FailureMechanismSectionsView(null, failureMechanism);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sections", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FailureMechanismSectionsView(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithSections_CreatesViewAndTableWithData()
        {
            // Setup
            var mocks = new MockRepository();
            var random = new Random(39);
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var sections = new[]
            {
                new FailureMechanismSection("a", new[]
                {
                    new Point2D(random.NextDouble(), random.NextDouble())
                })
            };
            mocks.ReplayAll();

            // Call
            using (var view = new FailureMechanismSectionsView(sections, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(failureMechanism, view.FailureMechanism);

                FailureMechanismSectionsTable tableControl = GetSectionsTable(view);
                Assert.NotNull(tableControl);
                Assert.AreEqual(DockStyle.Fill, tableControl.Dock);
                Assert.AreEqual(1, tableControl.Rows.Count);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithSections_WhenSectionsUpdated_ThenDataTableUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0.0, 0.0)
            }));

            using (var view = new FailureMechanismSectionsView(failureMechanism.Sections, failureMechanism))
            {
                FailureMechanismSectionsTable sectionsTable = GetSectionsTable(view);
                failureMechanism.Attach(observer);

                // Precondition
                Assert.AreEqual(1, sectionsTable.Rows.Count);

                // When
                failureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0.0, 0.0)
                }));
                failureMechanism.NotifyObservers();

                // Then
                Assert.AreEqual(2, sectionsTable.Rows.Count);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithSections_WhenSectionsCleared_ThenDataTableCleared()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0.0, 0.0)
            }));

            using (var view = new FailureMechanismSectionsView(failureMechanism.Sections, failureMechanism))
            {
                FailureMechanismSectionsTable sectionsTable = GetSectionsTable(view);
                failureMechanism.Attach(observer);

                // Precondition
                Assert.AreEqual(1, sectionsTable.Rows.Count);

                // When
                failureMechanism.ClearAllSections();
                failureMechanism.NotifyObservers();

                // Then
                Assert.AreEqual(0, sectionsTable.Rows.Count);
            }

            mocks.VerifyAll();
        }

        private static FailureMechanismSectionsTable GetSectionsTable(FailureMechanismSectionsView view)
        {
            return ControlTestHelper.GetControls<FailureMechanismSectionsTable>(view, "failureMechanismSectionsTable").Single();
        }
    }
}