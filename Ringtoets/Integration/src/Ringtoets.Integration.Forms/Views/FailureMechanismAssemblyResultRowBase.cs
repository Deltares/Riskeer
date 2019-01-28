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
using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.DataGrid;
using Core.Common.Util;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of a <see cref="IFailureMechanism"/>
    /// and its assembly result.
    /// </summary>
    internal abstract class FailureMechanismAssemblyResultRowBase : IHasColumnStateDefinitions
    {
        private const int categoryIndex = 3;
        private readonly IFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyResultRowBase"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to wrap so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameters is <c>null</c>.</exception>
        protected FailureMechanismAssemblyResultRowBase(IFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            this.failureMechanism = failureMechanism;

            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();
        }

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
        public FailureMechanismAssemblyCategoryGroup CategoryGroup { get; protected set; }

        /// <summary>
        /// Gets the probability of the failure mechanism assembly.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double Probability { get; protected set; }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        /// <summary>
        /// Updates all data and states in the row.
        /// </summary>
        public void Update()
        {
            ResetErrorTexts();
            TryGetDerivedData();

            SetCategoryGroupColumnStateDefinition();
        }

        /// <summary>
        /// Gets the derived data for the failure mechanism.
        /// </summary>
        protected abstract void TryGetDerivedData();

        protected DataGridViewColumnStateDefinition GetCategoryGroupColumnStateDefinition()
        {
            return ColumnStateDefinitions[categoryIndex];
        }

        private void SetCategoryGroupColumnStateDefinition()
        {
            GetCategoryGroupColumnStateDefinition().Style =
                new CellStyle(Color.FromKnownColor(KnownColor.ControlText),
                              AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(CategoryGroup));
        }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(categoryIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
        }

        private void ResetErrorTexts()
        {
            ColumnStateDefinitions[categoryIndex].ErrorText = string.Empty;
        }
    }
}