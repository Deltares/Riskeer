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
            LocationsView<T> locationsView = ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(locationsView);
            DataGridViewControl dataGridViewControl = GetDataGridViewControl(locationsView);

            // When
            dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(0, 1));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithoutGeneralResult_ThenIllustrationPointsControlDataSetToEmptyEnumeration()
        {
            // Given
            LocationsView<T> locationsView = ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(locationsView);
            DataGridViewControl dataGridViewControl = GetDataGridViewControl(locationsView);

            // When
            dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(1, 0));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithGeneralResult_ThenGeneralResultSetOnIllustrationPointsControlData()
        {
            // Given
            LocationsView<T> locationsView = ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(locationsView);
            DataGridViewControl dataGridViewControl = GetDataGridViewControl(locationsView);

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
        /// <returns>The fully configured view.</returns>
        protected abstract LocationsView<T> ShowFullyConfiguredLocationsView(Form form);

        private static DataGridViewControl GetDataGridViewControl(LocationsView<T> locationsView)
        {
            return GetControls<DataGridViewControl>(locationsView, "DataGridViewControl").Single();
        }

        private static IllustrationPointsControl GetIllustrationPointsControl(LocationsView<T> locationsView)
        {
            return GetControls<IllustrationPointsControl>(locationsView, "IllustrationPointsControl").Single();
        }

        /// <summary>
        /// Gets all controls of type <typeparamref name="TControl"/> going by the name <paramref name="controlName"/>.
        /// </summary>
        /// <typeparam name="TControl">The type of controls to find.</typeparam>
        /// <param name="locationsView">The locations view to find the controls in.</param>
        /// <param name="controlName">The name of the controls to find.</param>
        /// <returns>The found controls.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="controlName"/> is <c>null</c> or empty.</exception>
        private static IEnumerable<TControl> GetControls<TControl>(LocationsView<T> locationsView, string controlName) where TControl : Control
        {
            return locationsView.Controls.Find(controlName, true).OfType<TControl>();
        }
    }
}