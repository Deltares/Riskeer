﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Controls.DataGrid;
using Core.Common.Util;
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

            EnumDisplayWrapper<SimpleAssessmentResultType>[] simpleAssessmentDataSource =
                Enum.GetValues(typeof(SimpleAssessmentResultType))
                    .OfType<SimpleAssessmentResultType>()
                    .Select(sa => new EnumDisplayWrapper<SimpleAssessmentResultType>(sa))
                    .ToArray();

            dataGridViewControl.AddComboBoxColumn(
                dataPropertyName,
                Resources.FailureMechanismResultView_SimpleAssessmentResult_DisplayName,
                simpleAssessmentDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.DisplayName));
        }
    }
}