// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Core.Common.Util;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Forms.Builders
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
                Resources.FailureMechanismSection_Name_DisplayName,
                true);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing whether the section result is relevant.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddIsRelevantColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_IsRelevant_DisplayName);
        }

        private static IEnumerable<EnumDisplayWrapper<T>> CreateEnumDisplayWrappers<T>()
        {
            return Enum.GetValues(typeof(T))
                       .OfType<T>()
                       .Select(e => new EnumDisplayWrapper<T>(e))
                       .ToArray();
        }

        #region Initial Failure Mechanism Result

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing a
        /// dropdown with items of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <typeparam name="T">The initial failure mechanism result type enum.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddInitialFailureMechanismResultTypeColumn<T>(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<T>> dataSource = CreateEnumDisplayWrappers<T>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_InitialFailureMechanismResultType_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<T>.Value),
                nameof(EnumDisplayWrapper<T>.DisplayName));
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the initial failure mechanism result probability per profile.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddInitialFailureMechanismResultProfileProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_InitialFailureMechanismResultProfileProbability_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the initial failure mechanism result probability per section.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddInitialFailureMechanismResultSectionProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_InitialFailureMechanismResultSectionProbability_DisplayName);
        }

        #endregion

        #region Further Analysis

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing a
        /// <see cref="FailureMechanismSectionResultFurtherAnalysisType"/>. 
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddFurtherAnalysisTypeColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<FailureMechanismSectionResultFurtherAnalysisType>> dataSource = CreateEnumDisplayWrappers<FailureMechanismSectionResultFurtherAnalysisType>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_FurtherAnalysis_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<FailureMechanismSectionResultFurtherAnalysisType>.Value),
                nameof(EnumDisplayWrapper<FailureMechanismSectionResultFurtherAnalysisType>.DisplayName));
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing a
        /// <see cref="ProbabilityRefinementType"/>.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddProbabilityRefinementTypeColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            if (dataPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dataPropertyName));
            }

            IEnumerable<EnumDisplayWrapper<ProbabilityRefinementType>> dataSource = CreateEnumDisplayWrappers<ProbabilityRefinementType>();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_ProbabilityRefinementType_DisplayName,
                dataSource,
                nameof(EnumDisplayWrapper<ProbabilityRefinementType>.Value),
                nameof(EnumDisplayWrapper<ProbabilityRefinementType>.DisplayName));
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the refined probability per profile.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddRefinedProfileProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_RefinedProfileProbability_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the refined probability per section.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddRefinedSectionProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_RefinedSectionProbability_DisplayName);
        }

        #endregion

        #region Assembly

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the probability per profile.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddAssemblyProfileProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_AssemblyProfileProbability_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the probability per section.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddAssemblySectionProbabilityColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_AssemblySectionProbability_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the section N.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddAssemblySectionNColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.FailureMechanismResultView_AssemblySectionN_Rounded_DisplayName);
        }

        /// <summary>
        /// Adds a column to the <paramref name="dataGridViewControl"/> showing the assembly group.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> to add the column to.</param>
        /// <param name="dataPropertyName">The data property name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddAssemblyGroupColumn(DataGridViewControl dataGridViewControl, string dataPropertyName)
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
                Resources.AssemblyGroup_Name_DisplayName);
        }

        #endregion
    }
}