﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
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
            var failurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismSectionsView(null, failurePath);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("sections", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidParameters_InitializesViewCorrectly()
        {
            // Setup
            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            // Call
            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(sections, failurePath))
            {
                // Assert
                Assert.IsInstanceOf<CloseForFailurePathView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failurePath, view.FailurePath);
                Assert.AreEqual(1, view.Controls.Count);

                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);
                Assert.NotNull(sectionsDataGridViewControl);
                Assert.AreEqual(DockStyle.Fill, sectionsDataGridViewControl.Dock);

                DataGridView dataGridView = GetSectionsDataGridView(view);

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);
                Assert.AreEqual("Vaknaam", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Metrering van* [m]", dataGridView.Columns[sectionStartColumnIndex].HeaderText);
                Assert.AreEqual("Metrering tot* [m]", dataGridView.Columns[sectionEndColumnIndex].HeaderText);
                Assert.AreEqual("Lengte* [m]", dataGridView.Columns[lengthColumnIndex].HeaderText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutSections_CreatesViewWithDataGridViewEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            // Call
            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(sections, failurePath))
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
            var failurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            FailureMechanismSection[] sections =
            {
                CreateFailureMechanismSection("a"),
                CreateFailureMechanismSection("b"),
                CreateFailureMechanismSection("c")
            };

            // Call
            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(sections, failurePath))
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
            var failurePath = new TestFailurePath();
            FailureMechanismTestHelper.SetSections(failurePath, new[]
            {
                CreateFailureMechanismSection("a")
            });

            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(failurePath.Sections, failurePath))
            {
                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                // Precondition
                AssertSectionsDataGridViewControl(failurePath.Sections.ToArray(), sectionsDataGridViewControl);

                // When
                FailureMechanismTestHelper.SetSections(failurePath, new[]
                {
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
                });
                failurePath.NotifyObservers();

                // Then
                AssertSectionsDataGridViewControl(failurePath.Sections.ToArray(), sectionsDataGridViewControl);
            }
        }

        [Test]
        public void GivenViewWithSections_WhenFailureMechanismNotifiesChangeAndSectionsNotUpdated_ThenDataGridViewNotUpdated()
        {
            // Given
            var failurePath = new TestFailurePath();
            FailureMechanismTestHelper.SetSections(failurePath, new[]
            {
                CreateFailureMechanismSection("a")
            });

            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsView(failurePath.Sections, failurePath))
            {
                DataGridView sectionsDataGridView = GetSectionsDataGridView(view);

                var invalidated = false;

                sectionsDataGridView.Invalidated += (s, e) =>
                {
                    invalidated = true;
                };

                // When
                failurePath.NotifyObservers();

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

            double sectionStart = 0;
            for (var i = 0; i < sectionsDataGridViewControl.Rows.Count; i++)
            {
                FailureMechanismSection section = sections[i];
                DataGridViewCellCollection rowCells = sectionsDataGridViewControl.Rows[i].Cells;

                Assert.AreEqual(section.Name, rowCells[nameColumnIndex].Value);

                var sectionStartValue = (RoundedDouble) rowCells[sectionStartColumnIndex].Value;
                Assert.AreEqual(sectionStart, sectionStartValue, sectionStartValue.GetAccuracy());

                double sectionEnd = sectionStart + section.Length;
                var sectionEndValue = (RoundedDouble) rowCells[sectionEndColumnIndex].Value;
                Assert.AreEqual(sectionEnd, sectionEndValue, sectionEndValue.GetAccuracy());

                var sectionLength = (RoundedDouble) rowCells[lengthColumnIndex].Value;
                Assert.AreEqual(section.Length, sectionLength, sectionLength.GetAccuracy());

                sectionStart = sectionEnd;
            }
        }

        private FailureMechanismSectionsView ShowFailureMechanismSectionsView(IEnumerable<FailureMechanismSection> sections,
                                                                              IFailurePath failurePath)
        {
            var view = new FailureMechanismSectionsView(sections, failurePath);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}