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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class for testing data synchronization in <see cref="LocationsView{T}"/> derivatives.
    /// </summary>
    /// <typeparam name="T">The type of the locations contained by the view.</typeparam>
    public abstract class LocationsViewDataSynchronisationTester<T> where T : class
    {
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
        public void GivenFullyConfiguredView_WhenSelectingLocationWithoutOutput_ThenIllustrationPointsControlDataSetToEmptyEnumeration()
        {
            // Given
            ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridViewControl = GetDataGridViewControl();

            // When
            dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(0, 1));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithoutGeneralResult_ThenIllustrationPointsControlDataSetToEmptyEnumeration()
        {
            // Given
            ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridViewControl = GetDataGridViewControl();

            // When
            dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(1, 0));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithGeneralResult_ThenGeneralResultSetOnIllustrationPointsControlData()
        {
            // Given
            ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridViewControl = GetDataGridViewControl();

            // When
            dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(4, 0));

            // Then
            CollectionAssert.IsNotEmpty(illustrationPointsControl.Data);
        }

        /// <summary>
        /// Method for showing a fully configured locations view.
        /// </summary>
        /// <param name="form">The form to use for showing the view.</param>
        /// <remarks>
        /// The view should contain the following location row data:
        /// <list type="bullet">
        /// <item>Row 1: location without output</item>
        /// <item>Row 2: location with output but without general result</item>
        /// <item>Row 4: location with output containing a general result</item>
        /// </list>
        /// </remarks>
        protected abstract void ShowFullyConfiguredLocationsView(Form form);

        private DataGridViewControl GetDataGridViewControl()
        {
            return ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
        }

        private IllustrationPointsControl GetIllustrationPointsControl()
        {
            return ControlTestHelper.GetControls<IllustrationPointsControl>(testForm, "IllustrationPointsControl").Single();
        }
    }
}