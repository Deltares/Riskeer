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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismSectionConfigurationsViewTest
    {
        private const int columnCount = 7;
        private const int nameColumnIndex = 0;
        private const int sectionStartColumnIndex = 1;
        private const int sectionEndColumnIndex = 2;
        private const int lengthColumnIndex = 3;
        private const int parameterAColumnIndex = 4;
        private const int lengthEffectNRoundedColumnIndex = 5;
        private const int mechanismSensitiveSectionLengthColumnIndex = 6;

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
            var failureMechanism = new PipingFailureMechanism();

            // Call
            using (PipingFailureMechanismSectionConfigurationsView view = ShowPipingFailureMechanismSectionConfigurationsView(failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionConfigurationsView<PipingFailureMechanismSectionConfiguration, PipingFailureMechanismSectionConfigurationRow>>(view);
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
                Assert.AreEqual("Mechanismegevoelige vaklengte [m]", dataGridView.Columns[mechanismSensitiveSectionLengthColumnIndex].HeaderText);
            }
        }

        [Test]
        public void Constructor_WithSectionConfigurations_CreatesViewWithDataGridViewCorrectlyFilled()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                CreateFailureMechanismSection("a", 0.0, 0.0, 1.0, 1.0),
                CreateFailureMechanismSection("b", 1.0, 1.0, 2.0, 2.0),
                CreateFailureMechanismSection("c", 2.0, 2.0, 3.0, 3.0)
            });

            // Call
            using (PipingFailureMechanismSectionConfigurationsView view = ShowPipingFailureMechanismSectionConfigurationsView(failureMechanism))
            {
                // Assert
                DataGridViewControl sectionsDataGridViewControl = GetSectionsDataGridViewControl(view);

                AssertSectionsDataGridViewControl(failureMechanism.SectionConfigurations, failureMechanism.GeneralInput.B, sectionsDataGridViewControl);
            }
        }

        private static DataGridViewControl GetSectionsDataGridViewControl(PipingFailureMechanismSectionConfigurationsView view)
        {
            return ControlTestHelper.GetControls<DataGridViewControl>(view, "failureMechanismSectionsDataGridViewControl").Single();
        }

        private static DataGridView GetSectionsDataGridView(PipingFailureMechanismSectionConfigurationsView view)
        {
            return ControlTestHelper.GetControls<DataGridView>(view, "dataGridView").Single();
        }

        private static FailureMechanismSection CreateFailureMechanismSection(string name, double x1, double y1, double x2, double y2)
        {
            return new FailureMechanismSection(name, new[]
            {
                new Point2D(x1, y1),
                new Point2D(x2, y2)
            });
        }

        private static void AssertSectionsDataGridViewControl(IEnumerable<PipingFailureMechanismSectionConfiguration> sectionConfigurations,
                                                              double b,
                                                              DataGridViewControl sectionsDataGridViewControl)
        {
            Assert.AreEqual(sectionConfigurations.Count(), sectionsDataGridViewControl.Rows.Count);

            double sectionStart = 0;
            for (var i = 0; i < sectionsDataGridViewControl.Rows.Count; i++)
            {
                FailureMechanismSectionConfiguration sectionConfiguration = sectionConfigurations.ElementAt(i);
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

                var mechanismSensitiveSectionLength = (RoundedDouble) rowCells[mechanismSensitiveSectionLengthColumnIndex].Value;
                Assert.AreEqual(sectionConfiguration.GetFailureMechanismSensitiveSectionLength(), mechanismSensitiveSectionLength, mechanismSensitiveSectionLength.GetAccuracy());

                sectionStart = sectionEnd;
            }
        }

        private PipingFailureMechanismSectionConfigurationsView ShowPipingFailureMechanismSectionConfigurationsView(
            PipingFailureMechanism failureMechanism)
        {
            var view = new PipingFailureMechanismSectionConfigurationsView(failureMechanism.SectionConfigurations, failureMechanism);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}