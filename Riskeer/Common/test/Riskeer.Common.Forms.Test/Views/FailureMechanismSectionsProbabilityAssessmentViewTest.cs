﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.Probability;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionsProbabilityAssessmentViewTest
    {
        private const int columnCount = 5;
        private const int nameColumnIndex = 0;
        private const int sectionStartColumnIndex = 1;
        private const int sectionEndColumnIndex = 2;
        private const int lengthColumnIndex = 3;
        private const int lengthEffectColumnIndex = 4;

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
        public void Constructor_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            void Call()
            {
                new FailureMechanismSectionsProbabilityAssessmentView(Enumerable.Empty<FailureMechanismSection>(), failureMechanism, null);
            }

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("probabilityAssessmentInput", paramName);

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
            ProbabilityAssessmentInput probabilityAssessmentInput = CreateProbabilityAssessmentInput();

            // Call
            using (FailureMechanismSectionsProbabilityAssessmentView view = ShowFailureMechanismSectionsProbabilityAssessmentView(sections,
                                                                                                                                  failureMechanism,
                                                                                                                                  probabilityAssessmentInput))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionsView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
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
                Assert.AreEqual("Nvak* [-]", dataGridView.Columns[lengthEffectColumnIndex].HeaderText);
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
            ProbabilityAssessmentInput probabilityAssessmentInput = CreateProbabilityAssessmentInput();

            // Call
            using (FailureMechanismSectionsProbabilityAssessmentView view = ShowFailureMechanismSectionsProbabilityAssessmentView(sections,
                                                                                                                                  failureMechanism,
                                                                                                                                  probabilityAssessmentInput))
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
                CreateFailureMechanismSection("a", 0.0, 0.0, 1.0, 1.0),
                CreateFailureMechanismSection("b", 1.0, 1.0, 2.0, 2.0),
                CreateFailureMechanismSection("c", 2.0, 2.0, 3.0, 3.0)
            };

            ProbabilityAssessmentInput probabilityAssessmentInput = CreateProbabilityAssessmentInput();

            // Call
            using (FailureMechanismSectionsProbabilityAssessmentView view = ShowFailureMechanismSectionsProbabilityAssessmentView(sections,
                                                                                                                                  failureMechanism,
                                                                                                                                  probabilityAssessmentInput))
            {
                // Assert
                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                AssertSectionsDataGridViewControl(sections, probabilityAssessmentInput, sectionsDataGridViewControl);
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
                CreateFailureMechanismSection("a", 0.0, 0.0, 1.0, 1.0)
            });

            ProbabilityAssessmentInput probabilityAssessmentInput = CreateProbabilityAssessmentInput();

            using (FailureMechanismSectionsProbabilityAssessmentView view = ShowFailureMechanismSectionsProbabilityAssessmentView(failureMechanism.Sections,
                                                                                                                                  failureMechanism,
                                                                                                                                  probabilityAssessmentInput))
            {
                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                // Precondition
                AssertSectionsDataGridViewControl(failureMechanism.Sections.ToArray(), probabilityAssessmentInput, sectionsDataGridViewControl);

                // When
                FailureMechanismTestHelper.SetSections(failureMechanism, new[]
                {
                    CreateFailureMechanismSection("a", 1.0, 1.0, 2.0, 2.0)
                });
                failureMechanism.NotifyObservers();

                // Then
                AssertSectionsDataGridViewControl(failureMechanism.Sections.ToArray(), probabilityAssessmentInput, sectionsDataGridViewControl);
            }
        }

        [Test]
        public void GivenViewWithSections_WhenFailureMechanismNotifiesChangeAndProbabilityAssessmentInputChanged_ThenDataGridViewRefreshed()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                CreateFailureMechanismSection("a", 0.0, 0.0, 1.0, 1.0)
            });

            ProbabilityAssessmentInput probabilityAssessmentInput = CreateProbabilityAssessmentInput();

            using (FailureMechanismSectionsProbabilityAssessmentView view = ShowFailureMechanismSectionsProbabilityAssessmentView(failureMechanism.Sections,
                                                                                                                                  failureMechanism,
                                                                                                                                  probabilityAssessmentInput))
            {
                DataGridView sectionsDataGridView = GetSectionsDataGridView(view);

                var dataSourceChanged = false;

                sectionsDataGridView.DataSourceChanged += (s, e) =>
                {
                    dataSourceChanged = true;
                };

                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                // Precondition
                AssertSectionsDataGridViewControl(failureMechanism.Sections.ToArray(), probabilityAssessmentInput, sectionsDataGridViewControl);

                // When
                probabilityAssessmentInput.A = 0.5;
                failureMechanism.NotifyObservers();

                // Then
                Assert.IsFalse(dataSourceChanged);
                AssertSectionsDataGridViewControl(failureMechanism.Sections.ToArray(), probabilityAssessmentInput, sectionsDataGridViewControl);
            }
        }

        [Test]
        public void GivenViewWithSections_WhenFailureMechanismNotifiesChangeButNothingRelevantChanged_ThenDataGridViewNotUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                CreateFailureMechanismSection("a", 0.0, 0.0, 1.0, 1.0)
            });

            ProbabilityAssessmentInput probabilityAssessmentInput = CreateProbabilityAssessmentInput();

            using (FailureMechanismSectionsProbabilityAssessmentView view = ShowFailureMechanismSectionsProbabilityAssessmentView(failureMechanism.Sections,
                                                                                                                                  failureMechanism,
                                                                                                                                  probabilityAssessmentInput))
            {
                DataGridView sectionsDataGridView = GetSectionsDataGridView(view);

                var invalidated = false;

                sectionsDataGridView.Invalidated += (s, e) =>
                {
                    invalidated = true;
                };

                // When
                failureMechanism.NotifyObservers();

                // Then
                Assert.IsFalse(invalidated);
            }
        }

        private static FailureMechanismSection CreateFailureMechanismSection(string name, double x1, double y1, double x2, double y2)
        {
            return new FailureMechanismSection(name, new[]
            {
                new Point2D(x1, y1),
                new Point2D(x2, y2)
            });
        }

        private static TestProbabilityAssessmentInput CreateProbabilityAssessmentInput()
        {
            var random = new Random(39);

            return new TestProbabilityAssessmentInput(random.NextDouble(), random.NextDouble());
        }

        private static DataGridViewControl GetSectionsDataGridViewControl(FailureMechanismSectionsProbabilityAssessmentView view)
        {
            return ControlTestHelper.GetControls<DataGridViewControl>(view, "failureMechanismSectionsDataGridViewControl").Single();
        }

        private static DataGridView GetSectionsDataGridView(FailureMechanismSectionsView view)
        {
            return ControlTestHelper.GetControls<DataGridView>(view, "dataGridView").Single();
        }

        private static void AssertSectionsDataGridViewControl(FailureMechanismSection[] sections,
                                                              ProbabilityAssessmentInput probabilityAssessmentInput,
                                                              DataGridViewControl sectionsDataGridViewControl)
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

                var lengthEffect = (RoundedDouble) rowCells[lengthEffectColumnIndex].Value;
                Assert.AreEqual(probabilityAssessmentInput.GetN(section.Length), lengthEffect, lengthEffect.GetAccuracy());

                sectionStart = sectionEnd;
            }
        }

        private FailureMechanismSectionsProbabilityAssessmentView ShowFailureMechanismSectionsProbabilityAssessmentView(IEnumerable<FailureMechanismSection> sections,
                                                                                                                        IFailureMechanism failureMechanism,
                                                                                                                        ProbabilityAssessmentInput probabilityAssessmentInput)
        {
            var view = new FailureMechanismSectionsProbabilityAssessmentView(sections, failureMechanism, probabilityAssessmentInput);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}