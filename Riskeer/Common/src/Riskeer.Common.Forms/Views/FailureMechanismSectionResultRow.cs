// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.FailureMechanism;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base class for a wrapper of a <see cref="FailureMechanismSectionResult"/>, which takes care of the
    /// representation of properties in a grid.
    /// </summary>
    public abstract class FailureMechanismSectionResultRow<T> : IHasColumnStateDefinitions where T : FailureMechanismSectionResult
    {
        /// <summary>
        /// Fired when the row has started updating.
        /// </summary>
        public event EventHandler RowUpdated;

        /// <summary>
        /// Fired when the row has finished updating.
        /// </summary>
        public event EventHandler RowUpdateDone;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionResultRow{T}"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="FailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="sectionResult"/> is
        /// <c>null</c>.</exception>
        protected FailureMechanismSectionResultRow(T sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            SectionResult = sectionResult;
            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
        }

        /// <summary>
        /// Gets the name of the failure mechanism section.
        /// </summary>
        public string Name
        {
            get
            {
                return SectionResult.Section.Name;
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        /// <summary>
        /// Updates all data and states for the row.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public abstract void Update();

        /// <summary>
        /// Gets the <see cref="FailureMechanismSectionResult"/> that is the source of this row.
        /// </summary>
        protected T SectionResult { get; }

        /// <summary>
        /// Updates all data and notifies the wrapped section result.
        /// </summary>
        protected void UpdateInternalData()
        {
            Update();

            RowUpdated?.Invoke(this, EventArgs.Empty);
            SectionResult.NotifyObservers();
            RowUpdateDone?.Invoke(this, EventArgs.Empty);
        }
    }
}