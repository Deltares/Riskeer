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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
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
        private const int columnCount = 4;
        private const int nameColumnIndex = 0;
        private const int sectionStartColumnIndex = 1;
        private const int sectionEndColumnIndex = 2;
        private const int lengthColumnIndex = 3;

        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

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
        public void Constructor_ValidParameters_InitializesViewCorrectly()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            // Call
            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(sections, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<CloseForFailureMechanismView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreEqual(1, view.Controls.Count);

                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);
                Assert.NotNull(sectionsDataGridViewControl);
                Assert.AreEqual(DockStyle.Fill, sectionsDataGridViewControl.Dock);

                DataGridView dataGridView = GetSectionsDataGridView(view);

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);
                Assert.AreEqual("Vaknaam", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Metrering* van [m]", dataGridView.Columns[sectionStartColumnIndex].HeaderText);
                Assert.AreEqual("Metrering* tot [m]", dataGridView.Columns[sectionEndColumnIndex].HeaderText);
                Assert.AreEqual("Lengte* [m]", dataGridView.Columns[lengthColumnIndex].HeaderText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutSections_CreatesViewWithDataGridViewEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            // Call
            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(sections, failureMechanism))
            {
                // Assert
                CollectionAssert.IsEmpty(GetSectionsDataGridViewControl(view).Rows);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithSections_CreatesViewWithDataGridViewCorrectlyFilled()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            FailureMechanismSection[] sections =
            {
                CreateFailureMechanismSection("a"),
                CreateFailureMechanismSection("b"),
                CreateFailureMechanismSection("c")
            };

            // Call
            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(sections, failureMechanism))
            {
                // Assert
                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                AssertSectionsDataGridViewControl(sections, sectionsDataGridViewControl);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithSections_WhenFailureMechanismNotifiesChangeAndSectionsUpdated_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                CreateFailureMechanismSection("a")
            });

            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(failureMechanism.Sections, failureMechanism))
            {
                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                // Precondition
                AssertSectionsDataGridViewControl(failureMechanism.Sections.ToArray(), sectionsDataGridViewControl);

                // When
                FailureMechanismTestHelper.SetSections(failureMechanism, new[]
                {
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
                });
                failureMechanism.NotifyObservers();

                // Then
                AssertSectionsDataGridViewControl(failureMechanism.Sections.ToArray(), sectionsDataGridViewControl);
            }
        }

        [Test]
        public void GivenViewWithSections_WhenFailureMechanismNotifiesChangeAndSectionsNotUpdated_ThenDataGridViewNotUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                CreateFailureMechanismSection("a")
            });

            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(failureMechanism.Sections, failureMechanism))
            {
                DataGridView sectionsDataGridView = GetSectionsDataGridView(view);

                var invalidated = false;

                sectionsDataGridView.Invalidated += (s, e) => { invalidated = true; };

                // When
                failureMechanism.NotifyObservers();

                // Then
                Assert.IsFalse(invalidated);
            }
        }

        private static FailureMechanismSection CreateFailureMechanismSection(string name)
        {
            var random = new Random(39);

            return new FailureMechanismSection(name, new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            });
        }

        private static DataGridViewControl GetSectionsDataGridViewControl(FailureMechanismSectionsView view)
        {
            return ControlTestHelper.GetControls<DataGridViewControl>(view, "failureMechanismSectionsDataGridViewControl").Single();
        }

        private static DataGridView GetSectionsDataGridView(FailureMechanismSectionsView view)
        {
            return ControlTestHelper.GetControls<DataGridView>(view, "dataGridView").Single();
        }

        private static void AssertSectionsDataGridViewControl(FailureMechanismSection[] sections, DataGridViewControl sectionsDataGridViewControl)
        {
            Assert.AreEqual(sections.Length, sectionsDataGridViewControl.Rows.Count);

            for (var i = 0; i < sectionsDataGridViewControl.Rows.Count; i++)
            {
                FailureMechanismSection section = sections[i];
                DataGridViewCellCollection rowCells = sectionsDataGridViewControl.Rows[i].Cells;

                Assert.AreEqual(section.Name, rowCells[nameColumnIndex].Value);

                var sectionLength = (RoundedDouble) rowCells[lengthColumnIndex].Value;
                Assert.AreEqual(section.Length, sectionLength, sectionLength.GetAccuracy());
            }
        }

        private FailureMechanismSectionsView ShowFailureMechanismSectionsView(IEnumerable<FailureMechanismSection> sections,
                                                                              IFailureMechanism failureMechanism)
        {
            var view = new FailureMechanismSectionsView(sections, failureMechanism);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}