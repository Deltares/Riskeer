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
using System.ComponentModel;
using Core.Common.Controls.DataGrid;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of a <see cref="IFailureMechanism"/>
    /// and its assembly result.
    /// </summary>
    internal class FailureMechanismAssemblyResultRow : IHasColumnStateDefinitions
    {
        private const int probabilityIndex = 2;
        private readonly IFailureMechanism failureMechanism;
        private readonly Func<double> performAssemblyFunc;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyResultRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to wrap so that it can be displayed as a row.</param>
        /// <param name="performAssemblyFunc"></param>
        /// <exception cref="ArgumentNullException">Thrown when any parameters is <c>null</c>.</exception>
        public FailureMechanismAssemblyResultRow(IFailureMechanism failureMechanism,
                                                 Func<double> performAssemblyFunc)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (performAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(performAssemblyFunc));
            }

            this.failureMechanism = failureMechanism;
            this.performAssemblyFunc = performAssemblyFunc;

            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();

            Update();
        }

        /// <summary>
        /// Gets the name of the failure mechanism.
        /// </summary>
        public string Name => failureMechanism.Name;

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        public string Code => failureMechanism.Code;

        /// <summary>
        /// Gets the probability of the failure mechanism assembly.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double Probability { get; private set; }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        /// <summary>
        /// Updates all data and states in the row.
        /// </summary>
        public void Update()
        {
            ResetErrorTexts();
            TryGetAssemblyData();
        }

        private void TryGetAssemblyData()
        {
            try
            {
                Probability = performAssemblyFunc();
            }
            catch (AssemblyException e)
            {
                Probability = double.NaN;
                ColumnStateDefinitions[probabilityIndex].ErrorText = e.Message;
            }
        }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(probabilityIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
        }

        private void ResetErrorTexts()
        {
            ColumnStateDefinitions[probabilityIndex].ErrorText = string.Empty;
        }
    }
}