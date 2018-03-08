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
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Builders
{
    /// <summary>
    /// Builder class for adding various columns to a <see cref="DataGridViewControl"/> in
    /// failure mechanism result views.
    /// </summary>
    public static class FailureMechanismSectionResultViewColumnBuilder
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
        /// <see cref="SimpleAssessmentResultValidityOnlyType"/>.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddSimpleAssessmentResultValidityOnlyColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>> dataSource = CreateEnumDisplayWrappers<SimpleAssessmentResultValidityOnlyType>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_SimpleAssessmentResult_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>.DisplayName));
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing a
        /// <see cref="DetailedAssessmentProbabilityOnlyResultType"/>.
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

            IEnumerable<EnumDisplayWrapper<DetailedAssessmentProbabilityOnlyResultType>> dataSource = CreateEnumDisplayWrappers<DetailedAssessmentProbabilityOnlyResultType>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_DetailedAssessmentResult_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<DetailedAssessmentProbabilityOnlyResultType>.Value),
                nameof(EnumDisplayWrapper<DetailedAssessmentProbabilityOnlyResultType>.DisplayName));
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
        /// <see cref="TailorMadeAssessmentProbabilityAndDetailedCalculationResultType"/>.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>> dataSource =
                CreateEnumDisplayWrappers<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_TailorMadeAssessmentResult_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>.Value),
                nameof(EnumDisplayWrapper<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>.DisplayName));
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing a
        /// <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddTailorMadeAssessmentProbabilityCalculationResultColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<TailorMadeAssessmentProbabilityCalculationResultType>> dataSource =
                CreateEnumDisplayWrappers<TailorMadeAssessmentProbabilityCalculationResultType>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_TailorMadeAssessmentResult_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<TailorMadeAssessmentProbabilityCalculationResultType>.Value),
                nameof(EnumDisplayWrapper<TailorMadeAssessmentProbabilityCalculationResultType>.DisplayName));
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

        #region Assembly Category Group

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the simple
        /// assembly category group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddSimpleAssemblyCategoryGroupColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_SimpleAssemblyCategoryGroup_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the detailed
        /// assembly category group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddDetailedAssemblyCategoryGroupColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_DetailedAssemblyCategoryGroup_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the tailor made
        /// assembly category group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddTailorMadeAssemblyCategoryGroupColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_TailorMadeAssemblyCategoryGroup_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the combined
        /// assembly category group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddCombinedAssemblyCategoryGroupColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_CombinedAssemblyCategoryGroup_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the combined
        /// assembly probability.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddCombinedAssemblyProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_CombinedAssemblyProbability_DisplayName);
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
        public static void AddUseManualAssemblyCategoryGroupColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_UseManualAssemblyCategoryGroup_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the
        /// overridden combined assembly group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddManualAssemblyCategoryGroupColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<SelectableFailureMechanismSectionAssemblyCategoryGroup>> dataSource = CreateEnumDisplayWrappers<SelectableFailureMechanismSectionAssemblyCategoryGroup>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_ManualAssembly_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<SelectableFailureMechanismSectionAssemblyCategoryGroup>.Value),
                nameof(EnumDisplayWrapper<SelectableFailureMechanismSectionAssemblyCategoryGroup>.DisplayName));
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the
        /// overridden combined assembly probability.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddManualAssemblyProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_ManualAssembly_DisplayName);
        }

        #endregion
    }
}