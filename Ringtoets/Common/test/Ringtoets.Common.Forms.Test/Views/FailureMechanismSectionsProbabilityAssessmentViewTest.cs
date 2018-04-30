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
using Core.Common.Controls.Views;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.Probability;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionsProbabilityAssessmentViewTest
    {
        private const int nameColumnIndex = 0;
        private const int lengthColumnIndex = 1;
        private const int lengthEffectColumnIndex = 2;

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
            TestDelegate test = () =>
            {
                new FailureMechanismSectionsProbabilityAssessmentView(null,
                                                                      failureMechanism,
                                                                      new TestProbabilityAssessmentInput(1, 2));
            };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sections", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FailureMechanismSectionsProbabilityAssessmentView(Enumerable.Empty<FailureMechanismSection>(),
                                                                                            null,
                                                                                            new TestProbabilityAssessmentInput(1, 2));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () =>
            {
                new FailureMechanismSectionsProbabilityAssessmentView(Enumerable.Empty<FailureMechanismSection>(),
                                                                      failureMechanism,
                                                                      null);
            };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(1, 2);

            // Call
            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsProbabilityAssessmentView(sections,
                                                                                                             failureMechanism,
                                                                                                             probabilityAssessmentInput))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreEqual(1, view.Controls.Count);

                DataGridViewControl sectionsTable = GetSectionsTable(view);
                Assert.NotNull(sectionsTable);
                Assert.AreEqual(DockStyle.Fill, sectionsTable.Dock);

                DataGridViewColumn nameColumn = sectionsTable.GetColumnFromIndex(nameColumnIndex);
                Assert.AreEqual("Vaknaam", nameColumn.HeaderText);
                DataGridViewColumn lengthColumn = sectionsTable.GetColumnFromIndex(lengthColumnIndex);
                Assert.AreEqual("Lengte* [m]", lengthColumn.HeaderText);
                DataGridViewColumn lengthEffectColumn = sectionsTable.GetColumnFromIndex(lengthEffectColumnIndex);
                Assert.AreEqual("N* [-]", lengthEffectColumn.HeaderText);

                Assert.Throws<ArgumentOutOfRangeException>(() => sectionsTable.GetColumnFromIndex(lengthEffectColumnIndex + 1));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutSections_CreatesViewWithTableEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(1, 2);

            // Call
            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsProbabilityAssessmentView(sections,
                                                                                                             failureMechanism,
                                                                                                             probabilityAssessmentInput))
            {
                // Assert
                CollectionAssert.IsEmpty(GetSectionsTable(view).Rows);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithSections_CreatesViewWithTableCorrectlyFilled()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var sections = new[]
            {
                CreateFailureMechanismSection("a"),
                CreateFailureMechanismSection("b"),
                CreateFailureMechanismSection("c")
            };

            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(1, 2);

            // Call
            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsProbabilityAssessmentView(sections,
                                                                                                             failureMechanism,
                                                                                                             probabilityAssessmentInput))
            {
                // Assert
                DataGridViewControl sectionsTable = GetSectionsTable(view);

                AssertSectionsTable(sections, sectionsTable);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithSections_WhenSectionsUpdated_ThenTableUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(CreateFailureMechanismSection("a"));

            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(1, 2);

            using (FailureMechanismSectionsView view = ShowFailureMechanismSectionsProbabilityAssessmentView(failureMechanism.Sections,
                                                                                                             failureMechanism,
                                                                                                             probabilityAssessmentInput))
            {
                DataGridViewControl sectionsTable = GetSectionsTable(view);

                // Precondition
                AssertSectionsTable(failureMechanism.Sections.ToArray(), sectionsTable);

                // When
                failureMechanism.AddSection(CreateFailureMechanismSection("b"));
                failureMechanism.NotifyObservers();

                // Then
                AssertSectionsTable(failureMechanism.Sections.ToArray(), sectionsTable);
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

        private static DataGridViewControl GetSectionsTable(FailureMechanismSectionsView view)
        {
            return ControlTestHelper.GetControls<DataGridViewControl>(view, "failureMechanismSectionsTable").Single();
        }

        private static void AssertSectionsTable(FailureMechanismSection[] sections, DataGridViewControl sectionsTable)
        {
            Assert.AreEqual(sections.Length, sectionsTable.Rows.Count);

            for (var i = 0; i < sectionsTable.Rows.Count; i++)
            {
                FailureMechanismSection section = sections[i];
                DataGridViewCellCollection rowCells = sectionsTable.Rows[i].Cells;

                Assert.AreEqual(section.Name, rowCells[nameColumnIndex].Value);

                var sectionLength = (RoundedDouble) rowCells[lengthColumnIndex].Value;
                Assert.AreEqual(section.Length, sectionLength, sectionLength.GetAccuracy());
            }
        }

        private FailureMechanismSectionsView ShowFailureMechanismSectionsProbabilityAssessmentView(IEnumerable<FailureMechanismSection> sections,
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