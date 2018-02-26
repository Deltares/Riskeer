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
using Core.Common.Controls.DataGrid;
using Core.Common.Util;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Builders
{
    /// <summary>
    /// Builder class for adding various columns to a <see cref="DataGridViewControl"/>.
    /// </summary>
    public static class FailureMechanismSectionResultColumnBuilder
    {
        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the section name.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddSectionNameColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            dataGridViewControl.AddTextBoxColumn(
                dataPropertyName,
                Resources.Section_DisplayName,
                true);
        }

        private static IEnumerable<EnumDisplayWrapper<T>> CreateEnumDisplayWrappers<T>()
        {
            return Enum.GetValues(typeof(T))
                       .OfType<T>()
                       .Select(e => new EnumDisplayWrapper<T>(e))
                       .ToArray();
        }

        #region Assessment

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing a
        /// <see cref="SimpleAssessmentResultType"/>.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddSimpleAssessmentResultColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<SimpleAssessmentResultType>> dataSource = CreateEnumDisplayWrappers<SimpleAssessmentResultType>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_SimpleAssessmentResult_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.DisplayName));
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing a
        /// <see cref="DetailedAssessmentResultType"/>.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddDetailedAssessmentResultColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<DetailedAssessmentResultType>> dataSource = CreateEnumDisplayWrappers<DetailedAssessmentResultType>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_DetailedAssessment_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<DetailedAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<DetailedAssessmentResultType>.DisplayName));
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the detailed
        /// assessment probability.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddDetailedAssessmentProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            dataGridViewControl.AddTextBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_DetailedAssessmentProbability_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing a
        /// <see cref="TailorMadeAssessmentResultType"/>.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddTailorMadeAssessmentResultColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<TailorMadeAssessmentResultType>> dataSource = CreateEnumDisplayWrappers<TailorMadeAssessmentResultType>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_TailorMadeAssessment_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<TailorMadeAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<TailorMadeAssessmentResultType>.DisplayName));
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the tailor made
        /// assessment probability.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddTailorMadeAssessmentProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            dataGridViewControl.AddTextBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_TailorMadeAssessmentProbability_DisplayName);
        }

        #endregion

        #region Assessment Assembly

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the simple
        /// assessment assembly group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddSimpleAssessmentAssemblyColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            dataGridViewControl.AddTextBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_SimpleAssessmentAssembly_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the detailed
        /// assessment assembly group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddDetailedAssessmentAssemblyColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            dataGridViewControl.AddTextBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_DetailedAssessmentAssembly_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the tailor made
        /// assessment assembly group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddTailorMadeAssessmentAssemblyColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            dataGridViewControl.AddTextBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_TailorMadeAssessmentAssembly_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the
        /// combined assessment assembly group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddCombinedAssessmentAssemblyColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            dataGridViewControl.AddTextBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_CombinedAssembly_DisplayName);
        }

        #endregion

        #region Override Assembly

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the
        /// checkbox for the combined assembly overriding.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddOverrideAssemblyColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            dataGridViewControl.AddCheckBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_OverrideAssembly_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the
        /// overridden combined assembly group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddOverrideAssemblyGroupColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<FailureMechanismSectionAssemblyCategoryGroup>> dataSource = CreateEnumDisplayWrappers<FailureMechanismSectionAssemblyCategoryGroup>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_OverrideAssemblyGroup_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<FailureMechanismSectionAssemblyCategoryGroup>.Value),
                nameof(EnumDisplayWrapper<FailureMechanismSectionAssemblyCategoryGroup>.DisplayName));
        }

        #endregion
    }
}