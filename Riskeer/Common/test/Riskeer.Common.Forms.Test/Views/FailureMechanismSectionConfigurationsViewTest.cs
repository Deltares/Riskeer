// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionConfigurationsViewTest
    {
        private const int columnCount = 6;
        private const int nameColumnIndex = 0;
        private const int sectionStartColumnIndex = 1;
        private const int sectionEndColumnIndex = 2;
        private const int lengthColumnIndex = 3;
        private const int parameterAColumnIndex = 4;
        private const int lengthEffectNRoundedColumnIndex = 5;

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
        public void Constructor_ValidParameters_InitializesViewCorrectly()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>();

            // Call
            using (FailureMechanismSectionConfigurationsView view = ShowFailureMechanismSectionConfigurationsView(sectionConfigurations, failureMechanism))
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
                Assert.AreEqual("a [-]", dataGridView.Columns[parameterAColumnIndex].HeaderText);
                Assert.AreEqual("Nvak* [-]", dataGridView.Columns[lengthEffectNRoundedColumnIndex].HeaderText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutSectionConfigurations_CreatesViewWithDataGridViewEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>();

            // Call
            using (FailureMechanismSectionConfigurationsView view = ShowFailureMechanismSectionConfigurationsView(sectionConfigurations, failureMechanism))
            {
                // Assert
                CollectionAssert.IsEmpty(GetSectionsDataGridViewControl(view).Rows);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithSectionConfigurations_CreatesViewWithDataGridViewCorrectlyFilled()
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
            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>();
            sectionConfigurations.AddRange(CreateSectionConfigurations(sections));

            var random = new Random(21);
            double b = random.NextDouble();

            // Call
            using (FailureMechanismSectionConfigurationsView view = ShowFailureMechanismSectionConfigurationsView(sectionConfigurations,
                                                                                                                  failureMechanism,
                                                                                                                  b))
            {
                // Assert
                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                AssertSectionsDataGridViewControl(sectionConfigurations, b, sectionsDataGridViewControl);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithSectionConfigurations_WhenFailureMechanismNotifiesChange_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismSection[] sections =
            {
                CreateFailureMechanismSection("a", 0.0, 0.0, 1.0, 1.0)
            };
            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>();
            sectionConfigurations.AddRange(CreateSectionConfigurations(sections));

            var random = new Random(21);
            double b = random.NextDouble();

            using (FailureMechanismSectionConfigurationsView view = ShowFailureMechanismSectionConfigurationsView(sectionConfigurations,
                                                                                                                  failureMechanism,
                                                                                                                  b))
            {
                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                // Precondition
                AssertSectionsDataGridViewControl(sectionConfigurations, b, sectionsDataGridViewControl);

                // When
                sectionConfigurations.Clear();
                sectionConfigurations.Add(new TestFailureMechanismSectionConfiguration(CreateFailureMechanismSection("a", 1.0, 1.0, 2.0, 2.0),
                                                                                       random.NextRoundedDouble()));
                failureMechanism.NotifyObservers();

                // Then
                AssertSectionsDataGridViewControl(sectionConfigurations, b, sectionsDataGridViewControl);
            }
        }

        [Test]
        public void GivenViewWithSectionConfigurations_WhenSectionConfigurationNotifiesChange_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismSection[] sections =
            {
                CreateFailureMechanismSection("a", 0.0, 0.0, 1.0, 1.0)
            };
            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>();
            sectionConfigurations.AddRange(CreateSectionConfigurations(sections));

            var random = new Random(21);
            double b = random.NextDouble();

            using (FailureMechanismSectionConfigurationsView view = ShowFailureMechanismSectionConfigurationsView(sectionConfigurations,
                                                                                                                  failureMechanism,
                                                                                                                  b))
            {
                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                // Precondition
                AssertSectionsDataGridViewControl(sectionConfigurations, b, sectionsDataGridViewControl);

                // When
                FailureMechanismSectionConfiguration affectedConfiguration = sectionConfigurations[0];
                affectedConfiguration.A = random.NextRoundedDouble();
                affectedConfiguration.NotifyObservers();

                // Then
                AssertSectionsDataGridViewControl(sectionConfigurations, b, sectionsDataGridViewControl);
            }
        }

        [Test]
        public void GivenViewWithSections_WhenFailureMechanismNotifiesChangeButNothingRelevantChanged_ThenDataGridViewNotUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismSection[] sections =
            {
                CreateFailureMechanismSection("a", 0.0, 0.0, 1.0, 1.0)
            };
            FailureMechanismTestHelper.SetSections(failureMechanism, sections);
            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>();
            sectionConfigurations.AddRange(CreateSectionConfigurations(sections));

            using (FailureMechanismSectionConfigurationsView view = ShowFailureMechanismSectionConfigurationsView(sectionConfigurations,
                                                                                                                  failureMechanism))
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

        private static DataGridViewControl GetSectionsDataGridViewControl(FailureMechanismSectionConfigurationsView view)
        {
            return ControlTestHelper.GetControls<DataGridViewControl>(view, "failureMechanismSectionsDataGridViewControl").Single();
        }

        private static DataGridView GetSectionsDataGridView(FailureMechanismSectionsView view)
        {
            return ControlTestHelper.GetControls<DataGridView>(view, "dataGridView").Single();
        }

        private static IEnumerable<FailureMechanismSectionConfiguration> CreateSectionConfigurations(
            IReadOnlyList<FailureMechanismSection> sections)
        {
            var random = new Random(21);
            var sectionConfigurations = new FailureMechanismSectionConfiguration[sections.Count];
            for (var i = 0; i < sections.Count; i++)
            {
                sectionConfigurations[i] = new TestFailureMechanismSectionConfiguration(sections[i], random.NextRoundedDouble());
            }

            return sectionConfigurations;
        }

        private static FailureMechanismSection CreateFailureMechanismSection(string name, double x1, double y1, double x2, double y2)
        {
            return new FailureMechanismSection(name, new[]
            {
                new Point2D(x1, y1),
                new Point2D(x2, y2)
            });
        }

        private static void AssertSectionsDataGridViewControl(IReadOnlyList<FailureMechanismSectionConfiguration> sectionConfigurations,
                                                              double b,
                                                              DataGridViewControl sectionsDataGridViewControl)
        {
            Assert.AreEqual(sectionConfigurations.Count, sectionsDataGridViewControl.Rows.Count);

            double sectionStart = 0;
            for (var i = 0; i < sectionsDataGridViewControl.Rows.Count; i++)
            {
                FailureMechanismSectionConfiguration sectionConfiguration = sectionConfigurations[i];
                FailureMechanismSection section = sectionConfiguration.Section;
                DataGridViewCellCollection rowCells = sectionsDataGridViewControl.Rows[i].Cells;

                Assert.AreEqual(section.Name, rowCells[nameColumnIndex].Value);

                var sectionStartValue = (RoundedDouble) rowCells[sectionStartColumnIndex].Value;
                Assert.AreEqual(sectionStart, sectionStartValue, sectionStartValue.GetAccuracy());

                double sectionEnd = sectionStart + section.Length;
                var sectionEndValue = (RoundedDouble) rowCells[sectionEndColumnIndex].Value;
                Assert.AreEqual(sectionEnd, sectionEndValue, sectionEndValue.GetAccuracy());

                var sectionLength = (RoundedDouble) rowCells[lengthColumnIndex].Value;
                Assert.AreEqual(section.Length, sectionLength, sectionLength.GetAccuracy());

                var parameterA = (RoundedDouble) rowCells[parameterAColumnIndex].Value;
                Assert.AreEqual(sectionConfiguration.A, parameterA);

                var lengthEffect = (RoundedDouble) rowCells[lengthEffectNRoundedColumnIndex].Value;
                Assert.AreEqual(sectionConfiguration.GetN(b), lengthEffect, lengthEffect.GetAccuracy());

                sectionStart = sectionEnd;
            }
        }

        private FailureMechanismSectionConfigurationsView ShowFailureMechanismSectionConfigurationsView(
            IObservableEnumerable<FailureMechanismSectionConfiguration> sectionConfigurations,
            IFailureMechanism failureMechanism)
        {
            var random = new Random(21);
            return ShowFailureMechanismSectionConfigurationsView(sectionConfigurations, failureMechanism, random.NextDouble());
        }

        private FailureMechanismSectionConfigurationsView ShowFailureMechanismSectionConfigurationsView(
            IObservableEnumerable<FailureMechanismSectionConfiguration> sectionConfigurations,
            IFailureMechanism failureMechanism,
            double b)
        {
            var view = new FailureMechanismSectionConfigurationsView(sectionConfigurations, failureMechanism, b);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}