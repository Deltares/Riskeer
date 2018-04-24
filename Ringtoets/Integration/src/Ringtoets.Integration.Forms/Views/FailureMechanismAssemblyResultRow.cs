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
using System.ComponentModel;
using Core.Common.Controls.DataGrid;
using Core.Common.Util;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of a <see cref="IFailureMechanism"/>
    /// and its assembly result.
    /// </summary>
    public class FailureMechanismAssemblyResultRow
    {
        private const int categoryIndex = 3;
        private const int probabilityIndex = 4;
        private readonly IFailureMechanism failureMechanism;
        private readonly Func<FailureMechanismAssembly> getFailureMechanismAssembly;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyResultRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to wrap so 
        /// that it can be displayed as a row.</param>
        /// <param name="getFailureMechanismAssembly">Gets the assembly of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameters is <c>null</c>.</exception>
        public FailureMechanismAssemblyResultRow(IFailureMechanism failureMechanism,
                                                 Func<FailureMechanismAssembly> getFailureMechanismAssembly)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (getFailureMechanismAssembly == null)
            {
                throw new ArgumentNullException(nameof(getFailureMechanismAssembly));
            }

            this.failureMechanism = failureMechanism;
            this.getFailureMechanismAssembly = getFailureMechanismAssembly;

            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();

            Update();
        }

        /// <summary>
        /// Gets the column state definitions for the given indices.
        /// </summary>
        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        /// <summary>
        /// Gets the name of the failure mechanism.
        /// </summary>
        public string Name
        {
            get
            {
                return failureMechanism.Name;
            }
        }

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        public string Code
        {
            get
            {
                return failureMechanism.Code;
            }
        }

        /// <summary>
        /// Gets the group of the failure mechanism.
        /// </summary>
        public int Group
        {
            get
            {
                return failureMechanism.Group;
            }
        }

        /// <summary>
        /// Gets the group of the failure mechanism assembly.
        /// </summary>
        [TypeConverter(typeof(EnumTypeConverter))]
        public FailureMechanismAssemblyCategoryGroup CategoryGroup { get; private set; }

        /// <summary>
        /// Gets the probability of the failure mechanism assembly.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double Probablity { get; private set; }

        /// <summary>
        /// Updates all data and states in the row.
        /// </summary>
        public void Update()
        {
            ResetErrorTexts();
            TryGetCategoryGroup();
            TryGetProbability();
        }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(categoryIndex, new DataGridViewColumnStateDefinition
            {
                ReadOnly = true
            });
            ColumnStateDefinitions.Add(probabilityIndex, new DataGridViewColumnStateDefinition
            {
                ReadOnly = true
            });
        }

        private void ResetErrorTexts()
        {
            ColumnStateDefinitions[categoryIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[probabilityIndex].ErrorText = string.Empty;
        }

        private void TryGetCategoryGroup()
        {
            try
            {
                CategoryGroup = getFailureMechanismAssembly().Group;
            }
            catch (AssemblyException e)
            {
                CategoryGroup = FailureMechanismAssemblyCategoryGroup.None;
                ColumnStateDefinitions[categoryIndex].ErrorText = e.Message;
            }
        }

        private void TryGetProbability()
        {
            try
            {
                Probablity = getFailureMechanismAssembly().Probability;
            }
            catch (AssemblyException e)
            {
                Probablity = double.NaN;
                ColumnStateDefinitions[probabilityIndex].ErrorText = e.Message;
            }
        }
    }
}